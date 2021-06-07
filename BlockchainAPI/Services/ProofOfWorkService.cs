using BlockchainAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAPI.Services
{
    public class ProofOfWorkService
    {
        private int _nonce;
        private string _hash;
        public ProofOfWorkService(Block block, int difficulty, string monetaryIssuePublic)
        {
            _nonce = new int();
            _hash = "";
            Create(block, difficulty, monetaryIssuePublic);
        }
        private void Create(Block block, int difficulty, string monetaryIssuePublic)
        {
            block.Nonce = 0;
            int nonce = 0;
            while (!IsValid(block, difficulty, monetaryIssuePublic))
                block.Nonce++;
            _nonce = nonce;
            _hash = GetHash(block, monetaryIssuePublic);
        }
        public static bool IsValid(Block block, int difficulty, string monetaryIssuePublic)    // public
        {
            string hash = GetHash(block, monetaryIssuePublic);
            string startsWith = "";
            for (int i = 1; i <= difficulty-2; i++)
            {
                startsWith += "0";
            }
            bool result = hash.StartsWith(startsWith);
            return result;
        }
        private static string GetHash(Block block, string monetaryIssuePublic)
        {
            // string blockText = JsonConvert.SerializeObject(block);
            //var signatures = block.Transactions.Select(x => x.Signature).ToArray();
            string guess = GenerateMessage(block, monetaryIssuePublic);
            // string blockText = JsonConvert.SerializeObject(blockPrevData);
            byte[] bytes = Encoding.Unicode.GetBytes(guess);
            byte[] hash = new SHA256Managed().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");
            return hashBuilder.ToString();
        }
        private static string GenerateMessage(Block block, string monetaryIssuePublic)
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
        public int GetNonce()
        {
            return _nonce;
        }
        public string GetHash()
        {
            return _hash;
        }
    }
}
