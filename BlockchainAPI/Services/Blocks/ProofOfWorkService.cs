using BlockchainAPI.Models;

namespace BlockchainAPI.Services.Blocks
{
    public class ProofOfWorkService
    {
        private int _nonce;
        private string _hash;
        public ProofOfWorkService(Block block)
        {
            _nonce = new int();
            _hash = "";
            Create(block);
        }
        private void Create(Block block)
        {
            while (!ValidateBlockService.IsValid(block))
                block.Nonce++;
            _nonce = block.Nonce;
            _hash = ValidateBlockService.GetHash(block);
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
