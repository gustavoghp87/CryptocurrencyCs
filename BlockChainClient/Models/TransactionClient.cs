namespace BlockchainClient.Models
{
    public class TransactionClient
    {
        public decimal Amount { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string SenderPrivKey { get; set; }        // sender private key, no signature
        public decimal Fees { get; set; }

        public TransactionClient()
        {
            // empty
        }

        public TransactionClient(decimal amount, string recipient, string sender, string senderPrivKey, decimal fees)
        {
            Amount = amount;
            Recipient = recipient;
            Sender = sender;
            SenderPrivKey = senderPrivKey;               // sender private key, no signature
            Fees = fees;
        }

        public override string ToString()
        {
            return Amount.ToString("0.00000000") + Recipient + Sender;
        }
    }
}
