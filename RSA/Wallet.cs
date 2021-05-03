using NBitcoin;

namespace RSA
{
    public class Wallet
    {
        public string PublicKey { get; set; }
        public BitcoinAddress BitcoinAddress { get; set; }
        public string PrivateKey { get; set; }

        public Wallet()
        {
            Key privateKey = new();
            PublicKey = privateKey.GetBitcoinSecret(Network.Main).GetAddress(ScriptPubKeyType.Legacy).ToString();
            BitcoinAddress = BitcoinAddress.Create(PublicKey, Network.Main);
            PrivateKey = privateKey.GetBitcoinSecret(Network.Main).ToString();
        }

        public string Sign(string msgToSign)
        {
            var secret = Network.Main.CreateBitcoinSecret(PrivateKey);
            var signature = secret.PrivateKey.SignMessage(msgToSign);
            // var v = secret.PubKey.VerifyMessage(msgToSign, signature);
            return signature;
        }

        public bool Verify(string originalMessage, string signedMessage)
        {
            var pkh = (BitcoinAddress as IPubkeyHashUsable);
            var bol = pkh.VerifyMessage(originalMessage, signedMessage);
            return bol;
        }
    }
}
