namespace BlockchainClient.Models
{
    public class TransactionClient
    {
        public decimal Amount { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string SenderPrivKey { get; set; }   // no Signature
        public decimal Fees { get; set; }
        public override string ToString()
        {
            return Amount.ToString("0.00000000") + Recipient + Sender;
        }
    }
}
