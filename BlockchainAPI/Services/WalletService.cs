using BlockchainAPI.Models;
using NBitcoin;

namespace BlockchainAPI.Services
{
    public static class WalletService
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
        public static string SignMessage(Models.Transaction transaction, string privateKey)
        {
            var secret = Network.Main.CreateBitcoinSecret(privateKey);
            var signature = secret.PrivateKey.SignMessage(transaction.Message);
            return signature;
        }
        public static bool IsVerifiedMessage(Models.Transaction transaction)
        {
            string senderPublicKey = transaction.Sender;
            string originalMessage = transaction.Message;
            string signedMessage = transaction.Signature;
            IPubkeyHashUsable address = (IPubkeyHashUsable)BitcoinAddress.Create(senderPublicKey, Network.Main);
            bool result = address.VerifyMessage(originalMessage, signedMessage);
            return result;
        }
    }
}
