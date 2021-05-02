namespace Blockchain.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Signature { get; set; }
        public decimal Fees { get; set; }
        public Transaction(decimal amount, string sender, string recipient, string signature, decimal fees)
        {
            Amount = amount;
            Sender = sender;
            Recipient = recipient;
            Signature = signature;
            Fees = fees;
        }
        public override string ToString()
        {
            return Amount.ToString("0.00000000") + Recipient + Sender;
        }
    }
}
