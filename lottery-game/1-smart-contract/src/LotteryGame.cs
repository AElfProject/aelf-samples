using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.LotteryGame
{
    // Contract class must inherit the base class generated from the proto file
    public class LotteryGame : LotteryGameContainer.LotteryGameBase
    {
        private const string TokenContractAddress = "ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx"; // tDVW token contract address
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
            // The below code can be used to replace the above line. The below is a showcase of how you can reference to any contracts.
            // State.TokenContract.Value = Address.FromBase58(TokenContractAddress);
            State.ConsensusContract.Value = Context.GetContractAddressByName(SmartContractConstants.ConsensusContractSystemName);
            
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
            
            // Get a random hash and check if it is available
            var randomHash = State.ConsensusContract.GetRandomHash.Call(new Int64Value
            {
                Value = Context.CurrentHeight
            });
            Assert(randomHash != null && !randomHash.Value.IsNullOrEmpty(), "Still preparing your game result, please wait for a while...");
            
            if(IsWinner(randomHash))
            {
                // Transfer the token from the contract to the sender
                State.TokenContract.Transfer.Send(new TransferInput
                {
                    To = Context.Sender,
                    Symbol = TokenSymbol,
                    Amount = playAmount
                });
                
                // Emit an event to notify listeners about the outcome
                Context.Fire(new PlayOutcomeEvent
                {
                    Amount = input.Value,
                    Won = playAmount
                });
            }
            else
            {
                // Transfer the token from the sender to the contract
                State.TokenContract.TransferFrom.Send(new TransferFromInput
                {
                    From = Context.Sender,
                    To = Context.Self,
                    Symbol = TokenSymbol,
                    Amount = playAmount
                });
                
                // Emit an event to notify listeners about the outcome
                Context.Fire(new PlayOutcomeEvent
                {
                    Amount = input.Value,
                    Won = -playAmount
                });
            }
            
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
        
        // Determines if the player is a winner.
        // This method generates a random number based on the random hash input and checks if it's equal to 0.
        // If the random number is 0, the player is considered a winner.
        private bool IsWinner(Hash randomHash)
        {
            // Improve random distribution by XORing with the origin transaction ID
            var randomHex = HashHelper.XorAndCompute(randomHash, Context.OriginTransactionId).ToHex();
            var randomInt = int.Parse(randomHex.Substring(0, 8), System.Globalization.NumberStyles.HexNumber);
            var result = randomInt % 2;
            return result == 0;
        }
        
        // This method is used to ensure that only the owner of the contract can perform certain actions.
        // If the context sender is not the owner, an exception is thrown with the message "Unauthorized to perform the action."
        private void AssertIsOwner()
        {
            Assert(Context.Sender == State.Owner.Value, "Unauthorized to perform the action.");
        }
    }
    
}