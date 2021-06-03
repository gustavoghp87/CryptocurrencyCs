using BlockchainAPI.Models;
using NBitcoin;
using System.Security.Cryptography;
using System.Text;

namespace BlockchainAPI.Services
{
    public class WalletService
    {
        public static Wallet Generate()
        {
            Wallet newWallet = new();
            var bitcoinKey = new Key().GetBitcoinSecret(Network.Main);
            newWallet.PrivateKey = bitcoinKey.ToString();
            newWallet.PublicKey = bitcoinKey.GetAddress(ScriptPubKeyType.Legacy).ToString();
            newWallet.BitcoinAddress = BitcoinAddress.Create(newWallet.PublicKey, Network.Main);
            return newWallet;
        }

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
