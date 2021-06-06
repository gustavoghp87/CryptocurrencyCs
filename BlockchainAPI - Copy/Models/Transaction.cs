﻿namespace BlockchainAPI.Models
{
    public class Transaction
    {
        public decimal Amount { get; set; }
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public string Signature { get; set; }
        public decimal Fees { get; set; }
    }
}