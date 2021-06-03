using System;
using System.Collections.Generic;

namespace BlockchainAPI.Models
{
    public class BlockPrevData
    {
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }                      //    with fixed timestamp for all nodes
        public string PreviousHash { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
