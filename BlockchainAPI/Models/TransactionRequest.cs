using System;

namespace BlockchainAPI.Models
{
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Fees { get; set; }
        public string Message { get; set; }
        public string Miner { get; set; }
        public string Recipient { get; set; }
        public string Sender { get; set; }
        public string Signature { get; set; }
    }
}
