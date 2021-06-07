using BlockchainAPI.Models;
using BlockchainAPI.Services.Blocks;
using System.Linq;

namespace BlockchainAPI.Services.Blockchains
{
    public static class ValidateBlockchainService
    {
        public static bool IsValid(Blockchain blockchain)
        {
            Block block2 = blockchain.Blocks.ElementAt(2);
            Block block1 = blockchain.Blocks.ElementAt(1);
            if (block1.PreviousHash != "null!") return false;
            if (block1.Hash != "satoshiHash") return false;
            if (block2.PreviousHash != "satoshiHash") return false;
            if (block2.Hash != "satoshiHash") return false;
            if (!ValidateBlockService.IsValid(block2)) return false;
            int i = 3;
            while (i < blockchain.Blocks.Count)
            {
                if (i > 3)
                {
                    Block block = blockchain.Blocks.ElementAt(i);
                    Block lastBlock = blockchain.Blocks.ElementAt(i - 1);
                    ProofOfWorkService powService = new(lastBlock);
                    if (block.PreviousHash != powService.GetHash()) return false;
                    if (!ValidateBlockService.IsValid(block)) return false;
                }
                i++;
            }
            return true;
        }
    }
}
