using BlockchainAPI.Models;

namespace BlockchainAPI.Services
{
    public class BlockService
    {
        private Block _block;
        public BlockService()
        {
            _block = new();
        }
        public void Create(Block block, int difficulty, string monetaryIssuePublicKey)
        {
            _block = block;
            Mine(difficulty, monetaryIssuePublicKey);
        }
        private string GenerateMessage()
        {
            return $"{_block.Index} [{_block.Timestamp:yyyy-MM-dd HH:mm:ss}] " +
                   $"Nonce: {_block.Nonce} | PrevHash: {_block.PreviousHash}";
        }
        private void Mine(int difficulty, string monetaryIssuePublicKey)
        {
            ProofOfWorkService proofServ = new();
            proofServ.Create(_block, difficulty, monetaryIssuePublicKey);
            _block.Nonce = proofServ.GetNonce();
            _block.Hash = proofServ.GetHash();
        }
        public Block GetBlock()
        {
            return _block;
        }
    }
}
