using System;
using System.Collections.Generic;

namespace BlockchainAPI.Models
{
    public class Block
    {
        public int Index { get; set; }
        public int Difficulty { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }
        public string PreviousHash { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
