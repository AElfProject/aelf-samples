using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Types;
using AetherLink.Contracts.Consumer;
using AetherLink.Contracts.Oracle;
using AetherLink.Contracts.VRF.Coordinator;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.DiceMaster
{
    // Contract class must inherit the base class generated from the proto file
    public class DiceMaster : DiceMasterContainer.DiceMasterBase
    {
        private const string OracleContractAddress = "21Fh7yog1B741yioZhNAFbs3byJ97jvBmbGAPPZKZpHHog5aEg"; // tDVW
        //private const string OracleContractAddress = "BGhrBNTPcLccaxPv6hHJrn4CHHzeMovTsrkhFse5o2nwfvQyG"; // tDVV
        private const string TokenSymbol = "ELF";
        private const long MinimumPlayAmount = 1_000_000; // 0.01 ELF
        private const long MaximumPlayAmount = 1_000_000_000; // 10 ELF
        
        // Initializes the contract
        public override Empty Initialize(Empty input)
        {
            // Check if the contract is already initialized
            Assert(State.Initialized.Value == false, "Already initialized.");
            // Set the contract state
            State.Initialized.Value = true;
            // Set the owner address
            State.Owner.Value = Context.Sender;
            
            // Initialize the token contract
            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            State.OracleContract.Value =Address.FromBase58(OracleContractAddress);
            
            return new Empty();
        }
        
        public override Empty HandleOracleFulfillment(HandleOracleFulfillmentInput input)
        {
            var playedRecord = State.PlayedRecords[input.TraceId];
            if (playedRecord == null || playedRecord.Address == null) return new Empty();
            var address = playedRecord.Address;
            var blockNumber = playedRecord.BlockNumber;

            if (blockNumber != State.PlayerInfos[address].BlockNumber)
            {
                return new Empty();
            }
            
            var randomHashList = HashList.Parser.ParseFrom(input.Response);
            
            var dice1 = randomHashList.Data[0].ToInt64() % 6;
            var dice2 = randomHashList.Data[1].ToInt64() % 6;
            dice1 = ((dice1 < 0)? dice1 * -1 : dice1) + 1;
            dice2 = ((dice2 < 0)? dice2 * -1 : dice2) + 1;
            
            State.PlayerInfos[address].Dice1 = dice1;
            State.PlayerInfos[address].Dice2 = dice2;
            State.PlayerInfos[address].Pending = false;

            var playAmount = State.PlayerInfos[address].Amount;
            
            if(IsWinner(dice1, dice2))
            {
                // Transfer the token from the contract to the sender
                State.TokenContract.Transfer.Send(new TransferInput
                {
                    To = address,
                    Symbol = TokenSymbol,
                    Amount = playAmount * 2
                });
                
                State.PlayerInfos[address].Win = true;
                
                // Emit an event to notify listeners about the outcome
                Context.Fire(new PlayOutcomeEvent
                {
                    Amount = playAmount,
                    Won = playAmount,
                    From = address
                });
            }
            else
            {
                State.PlayerInfos[address].Win = false;
                
                // Emit an event to notify listeners about the outcome
                Context.Fire(new PlayOutcomeEvent
                {
                    Amount = playAmount,
                    Won = -playAmount,
                    From = address
                });
            }
        
            return new Empty();
        }
        
        // Plays the lottery game with a specified amount of tokens.
        // The method checks if the play amount is within the limit.
        // If the player wins, tokens are transferred from the contract to the sender and a PlayOutcomeEvent is fired with the won amount.
        // If the player loses, tokens are transferred from the sender to the contract and a PlayOutcomeEvent is fired with the lost amount.
        public override Empty Play(Int64Value input)
        {
            var playAmount = input.Value;
            
            // Check if input amount is within the limit
            Assert(playAmount is >= MinimumPlayAmount and <= MaximumPlayAmount, "Invalid play amount.");
            
            // Check if the sender has enough tokens
            var balance = State.TokenContract.GetBalance.Call(new GetBalanceInput
            {
                Owner = Context.Sender,
                Symbol = TokenSymbol
            }).Balance;
            Assert(balance >= playAmount, "Insufficient balance.");
            
            // Check if the contract has enough tokens
            var contractBalance = State.TokenContract.GetBalance.Call(new GetBalanceInput
            {
                Owner = Context.Self,
                Symbol = TokenSymbol
            }).Balance;
            Assert(contractBalance >= playAmount, "Insufficient contract balance.");

            if(State.PlayerInfos[Context.Sender] == null)
            {
                State.PlayerInfos[Context.Sender] = new PlayerInfo
                {
                    Pending = false,
                    Win = false,
                    Dice1 = 1,
                    Dice2 = 1,
                    Amount = playAmount,
                    Address = Context.Sender,
                    BlockNumber = Context.CurrentHeight
                };
            }
            Assert(State.PlayerInfos[Context.Sender].Pending == false, "Pending result. Please wait for the result.");
            
            // use VRF to get random number
            var keyHashs = State.OracleContract.GetProvingKeyHashes.Call(new Empty());
            var keyHash = keyHashs.Data[State.OracleNodeId.Value];
            var specificData = new SpecificData
            {
                KeyHash = keyHash,
                NumWords = 2,
                RequestConfirmations = 1
            }.ToByteString();
            
            var request = new SendRequestInput
            {
                SubscriptionId = State.SubscriptionId.Value,
                RequestTypeIndex = 2,
                SpecificData = specificData,
            };

            var traceId = HashHelper.ConcatAndCompute(
                HashHelper.ConcatAndCompute(HashHelper.ComputeFrom(Context.CurrentBlockTime),
                    HashHelper.ComputeFrom(Context.Origin)), HashHelper.ComputeFrom(request));
            request.TraceId = traceId;
            State.OracleContract.SendRequest.Send(request);

            var blockNumber = Context.CurrentHeight;
            
            State.PlayedRecords[traceId] = new PlayedRecord
            {
                Address = Context.Sender,
                BlockNumber = blockNumber,
            };

            State.PlayerInfos[Context.Sender].Pending = true;
            State.PlayerInfos[Context.Sender].Win = false;
            State.PlayerInfos[Context.Sender].Amount = playAmount;
            State.PlayerInfos[Context.Sender].BlockNumber = blockNumber;
            
            // Transfer the token from the sender to the contract
            State.TokenContract.TransferFrom.Send(new TransferFromInput
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = TokenSymbol,
                Amount = playAmount
            });
            
            return new Empty();
        }
        
        // Withdraws a specified amount of tokens from the contract.
        // This method can only be called by the owner of the contract.
        // After the tokens are transferred, a WithdrawEvent is fired to notify any listeners about the withdrawal.
        public override Empty Withdraw(Int64Value input)
        {
            AssertIsOwner();
            
            // Transfer the token from the contract to the sender
            State.TokenContract.Transfer.Send(new TransferInput
            {
                To = Context.Sender,
                Symbol = TokenSymbol,
                Amount = input.Value
            });
            
            // Emit an event to notify listeners about the withdrawal
            Context.Fire(new WithdrawEvent
            {
                Amount = input.Value,
                From = Context.Self,
                To = State.Owner.Value
            });
            
            return new Empty();
        }
        
        public override Empty SetSubscriptionId(Int64Value input)
        {
            AssertIsOwner();
            State.SubscriptionId.Value = input.Value;
            return new Empty();
        }

        public override Empty SetOracleNodeId(Int32Value input)
        {
            AssertIsOwner();
            State.OracleNodeId.Value = input.Value;
            return new Empty();
        }
        
        // Deposits a specified amount of tokens into the contract.
        // This method can only be called by the owner of the contract.
        // After the tokens are transferred, a DepositEvent is fired to notify any listeners about the deposit.
        public override Empty Deposit(Int64Value input)
        {
            AssertIsOwner();
            
            // Transfer the token from the sender to the contract
            State.TokenContract.TransferFrom.Send(new TransferFromInput
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = TokenSymbol,
                Amount = input.Value
            });
            
            // Emit an event to notify listeners about the deposit
            Context.Fire(new DepositEvent
            {
                Amount = input.Value,
                From = Context.Sender,
                To = Context.Self
            });
            
            return new Empty();
        }
        
        // Transfers the ownership of the contract to a new owner.
        // This method can only be called by the current owner of the contract.
        public override Empty TransferOwnership(Address input)
        {
            AssertIsOwner();
            
            // Set the new owner address
            State.Owner.Value = input;
            
            return new Empty();
        }

        // A method that read the contract's play amount limit
        public override PlayAmountLimitMessage GetPlayAmountLimit(Empty input)
        {
            // Wrap the value in the return type
            return new PlayAmountLimitMessage
            {
                MinimumAmount = MinimumPlayAmount,
                MaximumAmount = MaximumPlayAmount
            };
        }
        
        // A method that read the contract's current balance
        public override Int64Value GetContractBalance(Empty input)
        {
            // Get the balance of the contract
            var balance = State.TokenContract.GetBalance.Call(new GetBalanceInput
            {
                Owner = Context.Self,
                Symbol = TokenSymbol
            }).Balance;
            
            // Wrap the value in the return type
            return new Int64Value
            {
                Value = balance
            };
        }

        // A method that read the contract's owner
        public override StringValue GetOwner(Empty input)
        {
            return State.Owner.Value == null ? new StringValue() : new StringValue {Value = State.Owner.Value.ToBase58()};
        }
        
        public override Int64Value GetSubscriptionId(Empty input)
        {
            return new Int64Value{
                Value = State.SubscriptionId.Value
            };
        }

        public override Int32Value GetOracleNodeId(Empty input)
        {
            return new Int32Value{
                Value = State.OracleNodeId.Value
            };
        }
        
        public override PlayerInfo GetPlayerInfo(Address address)
        {
            Assert(State.PlayerInfos[address] != null, "No player info found.");
            return State.PlayerInfos[address];
        }
        
        // Determines if the player is a winner.
        // The player is considered a winner if he has an odd number.
        private bool IsWinner(long dice1, long dice2)
        {
            var result = (dice1 + dice2) % 2;
            return result == 1;
        }
        
        // This method is used to ensure that only the owner of the contract can perform certain actions.
        // If the context sender is not the owner, an exception is thrown with the message "Unauthorized to perform the action."
        private void AssertIsOwner()
        {
            Assert(Context.Sender == State.Owner.Value, "Unauthorized to perform the action.");
        }
    }
    
}