using BlockchainAPI.Models;
using System;
using System.Collections.Generic;

namespace BlockchainAPI.Services.Blocks
{
    public class BlockService
    {
        private Block _block;
        public BlockService(int index, string previousHash, List<Transaction> lstTransactions, int difficulty)
        {
            _block = new();
            _block.Index = index;
            _block.Difficulty = new();
            _block.Difficulty = difficulty;
            _block.PreviousHash = previousHash;
            _block.Transactions = new();
            _block.Transactions.AddRange(lstTransactions);
            _block.Timestamp = DateTime.UtcNow;
            _block.Nonce = 0;
            _block.Hash = "";
            Mine();
        }
        private void Mine()
        {
            ProofOfWorkService proofServ = new(_block);
            _block.Nonce = proofServ.GetNonce();
            _block.Hash = proofServ.GetHash();
        }
        private string GenerateMessage()
        {
            return $"{_block.Index} [{_block.Timestamp:yyyy-MM-dd HH:mm:ss}] " +
                   $"Nonce: {_block.Nonce} | PrevHash: {_block.PreviousHash}";
        }
        public Block GetMined()
        {
            return _block;
        }
    }
}
