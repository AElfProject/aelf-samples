using AElf.Contracts.MultiToken;
using AElf.Sdk.CSharp;
using AElf.Types;
using Google.Protobuf.WellKnownTypes;

namespace AElf.Contracts.NftSale
{
    // Contract class must inherit the base class generated from the proto file
    public class NftSale : NftSaleContainer.NftSaleBase
    {
        // Initializes the contract
        public override Empty Initialize(NftPrice input)
        {
            // Check if the contract is already initialized
            Assert(State.Initialized.Value == false, "Already initialized.");
            // Set the contract state
            State.Initialized.Value = true;
            // Set the owner address
            State.Owner.Value = Context.Sender;
            State.NftPrice.Value = input.Price;
            
            // Initialize the token contract
            State.TokenContract.Value = Context.GetContractAddressByName(SmartContractConstants.TokenContractSystemName);
            // The below code can be used to replace the above line. The below is a showcase of how you can reference to any contracts.
            State.ConsensusContract.Value = Context.GetContractAddressByName(SmartContractConstants.ConsensusContractSystemName);
            
            return new Empty();
        }
        
        // transfer nft
        public override Empty Purchase(PurchaseInput input)
        {
            var price = State.NftPrice.Value;
            // transfer token
            State.TokenContract.TransferFrom.Send(new TransferFromInput
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = price.Symbol,
                Amount = price.Amount * input.Amount
            });
            // transfer nft
            State.TokenContract.Transfer.Send(new TransferInput
            {
                To = Context.Sender,
                Symbol = input.Symbol,
                Amount = input.Amount
            });
            
            Context.Fire(new SaleNft
            {
                To = Context.Self,
                Symbol = input.Symbol,
                Amount = input.Amount,
            });
            
            return new Empty();
        }
        
        // 
        public override Empty SetPriceAndSymbol(NftPrice input)
        {
            AssertIsOwner();
            State.NftPrice.Value = input.Price;
            return new Empty();
        }
        
        public override Price GetPrice(Empty input)
        {
            return State.NftPrice.Value;
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
                Symbol = State.NftPrice.Value.Symbol,
                Amount = input.Value
            });
            
            // Emit an event to notify listeners about the withdrawal
            Context.Fire(new WithdrawEvent
            {
                Amount = input.Value,
                From = Context.Self,
                To = Context.Sender
            });
            
            return new Empty();
        }
        
        // Deposits a specified amount of tokens into the contract.
        // This method can only be called by the owner of the contract.
        // After the tokens are transferred, a DepositEvent is fired to notify any listeners about the deposit.
        public override Empty Deposit(DepositeInput input)
        {
            AssertIsOwner();
            
            // Transfer the token from the sender to the contract
            State.TokenContract.TransferFrom.Send(new TransferFromInput
            {
                From = Context.Sender,
                To = Context.Self,
                Symbol = input.Symbol,
                Amount = input.Amount
            });
            
            // Emit an event to notify listeners about the deposit
            Context.Fire(new DepositEvent
            {
                Amount = input.Amount,
                From = Context.Sender,
                To = Context.Self
            });
            
            return new Empty();
        }

        
        // A method that read the contract's current balance
        public override Int64Value GetContractBalance(GetContractBalanceInput input)
        {
            // Get the balance of the contract
            var balance = State.TokenContract.GetBalance.Call(new GetBalanceInput
            {
                Owner = input.Address,
                Symbol = State.NftPrice.Value.Symbol
            }).Balance;
            
            // Wrap the value in the return type
            return new Int64Value
            {
                Value = balance
            };
        }
        
        // This method is used to ensure that only the owner of the contract can perform certain actions.
        // If the context sender is not the owner, an exception is thrown with the message "Unauthorized to perform the action."
        private void AssertIsOwner()
        {
            Assert(Context.Sender == State.Owner.Value, "Unauthorized to perform the action.");
        }

        // A method that read the contract's owner
        public override StringValue GetOwner(Empty input)
        {
            return State.Owner.Value == null ? new StringValue() : new StringValue {Value = State.Owner.Value.ToBase58()};
        }
    }
    
}