using BlockchainAPI.Models;
using NBitcoin;

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
            newWallet.BitcoinAddress = BitcoinAddress.Create(newWallet.PublicKey, Network.Main).ToString();
            return newWallet;
        }

        public static string Sign(string privKey, string msgToSign)
        {
            var secret = Network.Main.CreateBitcoinSecret(privKey);
            var signature = secret.PrivateKey.SignMessage(msgToSign);
            return signature;
        }

        public static bool VerifyTransaction(Models.Transaction transaction)
        {
            string senderPublicKey = transaction.Sender;
            string originalMessage = transaction.Recipient;
            string signedMessage = transaction.Signature;
            IPubkeyHashUsable address = (IPubkeyHashUsable)BitcoinAddress.Create(senderPublicKey, Network.Main);
            bool result = address.VerifyMessage(originalMessage, signedMessage);
            return result;
        }
    }
}
