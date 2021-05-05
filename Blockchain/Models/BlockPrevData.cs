using System;
using System.Collections.Generic;

namespace Blockchain.Models
{
    public class BlockPrevData
    {
        public int Index { get; private set; }
        public DateTime Timestamp { get; private set; }                      //    with fixed timestamp for all nodes
        public string PreviousHash { get; private set; }
        public List<Transaction> Transactions { get; private set; }

        public BlockPrevData(int index, DateTime timestamp, string previousHash, List<Transaction> transactions)
        {
            Index = index;
            Timestamp = timestamp;
            PreviousHash = previousHash;
            Transactions = transactions;
        }
    }
}
