using NBitcoin;
using System.Security.Cryptography;
using System.Text;

namespace RSA
{
    public class RSAService
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

        public static string GetSha256(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] hash = new SHA256Managed().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");
            return hashBuilder.ToString();
        }
    }
}
