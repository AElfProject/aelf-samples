using AElf.Types;
using AElf.Contracts.MultiToken;

namespace nftIndexer.Tests.TestHelpers;

public class TestIssuedEvent
{
    public Address To { get; set; }
    public string Symbol { get; set; }
    public long Amount { get; set; }
    public string Memo { get; set; }

    public static implicit operator Issued(TestIssuedEvent testEvent)
    {
        return new Issued
        {
            To = testEvent.To,
            Symbol = testEvent.Symbol,
            Amount = testEvent.Amount,
            Memo = testEvent.Memo
        };
    }
}
