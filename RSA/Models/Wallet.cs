using NBitcoin;

namespace RSA
{
    public class Wallet
    {
        public string PublicKey { get; private set; }
        public BitcoinAddress BitcoinAddress { get; private set; }
        public string PrivateKey { get; private set; }

        public Wallet()
        {
            var bitcoinKey = new Key().GetBitcoinSecret(Network.Main);
            PrivateKey = bitcoinKey.ToString();
            PublicKey = bitcoinKey.GetAddress(ScriptPubKeyType.Legacy).ToString();
            BitcoinAddress = BitcoinAddress.Create(PublicKey, Network.Main);
        }
    }
}
