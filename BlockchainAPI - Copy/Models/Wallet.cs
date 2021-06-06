namespace BlockchainAPI.Models
{
    public class Wallet
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string BitcoinAddress { get; set; }

        //public BitcoinAddress BitcoinAddress { get; set; }
    }
}
