using BlockchainAPI.Models;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAPI.Services
{
    public static class ProofOfWorkService
    {
        public static int CreateProofOfWork(BlockPrevData blockPrevData, string monetaryIssuePublic, int difficulty)
        {
            Block newBlock = new()
            {
                Index = blockPrevData.Index,
                Timestamp = blockPrevData.Timestamp,
                PreviousHash = blockPrevData.PreviousHash,
                Transactions = blockPrevData.Transactions,
            };
            newBlock.Nonce = 0;
            int nonce = 0;
            while (!IsValid(newBlock, monetaryIssuePublic, difficulty))
                newBlock.Nonce++;
            return nonce;
        }

        public static bool IsValid(Block block, string monetaryIssuePublic, int difficulty)
        {
            string hash = GetHash(block, monetaryIssuePublic);
            string begin = "";
            for (int i = 1; i <= difficulty; i++)
            {
                begin += "0";
            }
            bool result = hash.StartsWith(begin);
            return result;
        }

        public static string GetHash(Block block, string monetaryIssuePublic)
        {
            // string blockText = JsonConvert.SerializeObject(block);
            //var signatures = block.Transactions.Select(x => x.Signature).ToArray();
            string guess = GenerateGuess(block, monetaryIssuePublic);
            // string blockText = JsonConvert.SerializeObject(blockPrevData);
            byte[] bytes = Encoding.Unicode.GetBytes(guess);
            byte[] hash = new SHA256Managed().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");
            return hashBuilder.ToString();
        }

        private static string GenerateGuess(Block block, string monetaryIssuePublic)
        {
            string sign = "";
            foreach (var transaction in block.Transactions)
            {
                if (transaction.Sender != monetaryIssuePublic)
                    sign += transaction.Signature;
            }
            // var signatures = transactions.Select(x => x.Signature).ToArray();
            return $"{sign}{block.Nonce}{block.PreviousHash}";
        }
    }
}
