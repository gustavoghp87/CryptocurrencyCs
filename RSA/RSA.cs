using NBitcoin;

namespace RSA
{
    public class RSA
    {
        public static string Sign(string privKey, string msgToSign)
        {
            var a = privKey;
            var secret = Network.Main.CreateBitcoinSecret(privKey);
            var signature = secret.PrivateKey.SignMessage(msgToSign);
            return signature;
        }

        public static bool Verify(string pbKey, string originalMessage, string signedMessage)
        {
            var address = BitcoinAddress.Create(pbKey, Network.Main);
            var pkh = (address as IPubkeyHashUsable);
            var bol = pkh.VerifyMessage(originalMessage, signedMessage);
            return bol;
        }
    }
}
