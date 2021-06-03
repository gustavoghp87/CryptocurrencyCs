using BlockchainAPI.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAPI.Services
{
    public class BlockService
    {
        public string ToString(int index, DateTime timestamp, int nonce, string previousHash)
        {
            return $"{index} [{timestamp:yyyy-MM-dd HH:mm:ss}] Nonce: {nonce} | PrevHash: {previousHash}";
        }

        public Block Create(BlockPrevData blockPrevData, int nonce, string hash)
        {
            var newBlock = new Block
            {
                Index = blockPrevData.Index,
                Timestamp = blockPrevData.Timestamp,
                PreviousHash = blockPrevData.PreviousHash,
                Transactions = blockPrevData.Transactions,
                Nonce = nonce,
                Hash = hash
            };
            PayReward();
            _chain.Add(newBlock);
            return newBlock;
        }

        public List<Block> GetBlocks()
        {
            return _chain;
        }

        public Block Mine()
        {
            // minar: tengo un timestamp, un index, un prevHash y un transactions list
            // le falta nonce, hash
            var blockPrevData = new BlockPrevData
            {
                Index = _chain.Count,
                Timestamp = DateTime.UtcNow,
                PreviousHash = _chain.Last().Hash ?? "null!",
                Transactions = _currentTransactions.ToList()
            };
            int nonce = CreateProofOfWork(blockPrevData);
            string guess = GenerateGuess(blockPrevData.Transactions, nonce, blockPrevData.PreviousHash);
            string newHash = GetSha256(guess);
            Block block = CreateNewBlock(blockPrevData, nonce, newHash);
            return block;
        }
    }
}
