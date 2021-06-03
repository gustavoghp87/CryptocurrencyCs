using NBitcoin;

namespace BlockchainAPI.Models
{
    public class Wallet
    {
        public string PublicKey { get; set; }
        public BitcoinAddress BitcoinAddress { get; set; }
        public string PrivateKey { get; set; }
    }
}
