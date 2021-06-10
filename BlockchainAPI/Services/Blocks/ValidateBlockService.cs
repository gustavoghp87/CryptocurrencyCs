using BlockchainAPI.Models;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAPI.Services.Blocks
{
    public static class ValidateBlockService
    {
        public static bool IsValid(Block block)
        {
            string hash = GetHash(block);
            string startsWith = "";
            for (int i = 1; i <= block.Difficulty; i++)
            {
                startsWith += "0";
            }
            bool result = hash.StartsWith(startsWith);
            return result;
        }
        public static string GetHash(Block block)
        {
            // string blockText = JsonConvert.SerializeObject(block);
            //var signatures = block.Transactions.Select(x => x.Signature).ToArray();
            string guess = GenerateMessage(block);
            // string blockText = JsonConvert.SerializeObject(blockPrevData);
            byte[] bytes = Encoding.Unicode.GetBytes(guess);
            byte[] hash = new SHA256Managed().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");
            return hashBuilder.ToString();
        }
        private static string GenerateMessage(Block block)
        {
            string sign = "";
            foreach (var transaction in block.Transactions)
            {
                if (transaction.Sender != IssuerService.Get().PublicKey)
                    sign += transaction.Signature;
            }
            // var signatures = transactions.Select(x => x.Signature).ToArray();
            return $"{sign}{block.Nonce}{block.PreviousHash}";
        }
    }
}
