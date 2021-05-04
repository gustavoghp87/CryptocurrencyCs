using NBitcoin;

namespace RSA
{
    public class RSA
    {
        public static string Sign(string privKey, string msgToSign)
        {
            var secret = Network.Main.CreateBitcoinSecret(privKey);
            var signature = secret.PrivateKey.SignMessage(msgToSign);
            return signature;
        }

        public static bool Verify(string publicKey, string originalMessage, string signedMessage)
        {
            var address = (IPubkeyHashUsable)BitcoinAddress.Create(publicKey, Network.Main);
            var bol = address.VerifyMessage(originalMessage, signedMessage);
            return bol;
        }
    }
}
