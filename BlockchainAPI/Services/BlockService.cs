using BlockchainAPI.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAPI.Services
{
    public class BlockService
    {
        private Block _block;
        public BlockService(int index, string previousHash, List<Transaction> lstTransactions, int difficulty, string monetaryIssuePublicKey)
        {
            _block = new();
            _block.Index = index;
            _block.Timestamp = DateTime.UtcNow;
            _block.PreviousHash = previousHash;
            _block.Transactions = lstTransactions;
            _block.Nonce = 0;
            _block.Hash = "";
            Mine(difficulty, monetaryIssuePublicKey);
        }
        private void Mine(int difficulty, string monetaryIssuePublicKey)
        {
            ProofOfWorkService proofServ = new(_block, difficulty, monetaryIssuePublicKey);
            _block.Nonce = proofServ.GetNonce();
            _block.Hash = proofServ.GetHash();
        }
        private string GenerateMessage()
        {
            return $"{_block.Index} [{_block.Timestamp:yyyy-MM-dd HH:mm:ss}] " +
                   $"Nonce: {_block.Nonce} | PrevHash: {_block.PreviousHash}";
        }
        public Block GetBlock()
        {
            return _block;
        }
    }
}
