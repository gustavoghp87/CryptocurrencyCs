using System;
using System.Collections.Generic;

namespace Blockchain.Models
{
    public class Block
    {
        public int Index { get; private set; }
        public DateTime Timestamp { get; private set; }
        public List<Transaction> Transactions { get; private set; }
        public int Nonce { get; private set; }
        public string PreviousHash { get; private set; }

        public Block(int index, DateTime timestamp, List<Transaction> transactions, int nonce, string previousHash)
        {
            Index = index;
            Timestamp = timestamp;
            Transactions = transactions;
            Nonce = nonce;
            PreviousHash = previousHash;
        }

        public override string ToString()
        {
            return $"{Index} [{Timestamp:yyyy-MM-dd HH:mm:ss}] Nonce: {Nonce} | PrevHash: {PreviousHash}";
        }
    }
}
