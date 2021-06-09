using BlockchainAPI.Models;

namespace BlockchainAPI.Services.Transactions
{
    public class SignTransactionService
    {
        private Transaction _transaction;
        public SignTransactionService(Transaction transaction, string privateKey)
        {
            _transaction = transaction;
            Sign(privateKey);
        }
        private void Sign(string privateKey)
        {
            _transaction.Message = TransactionMessageService.Generate(_transaction);
            _transaction.Signature = WalletService.SignMessage(_transaction, privateKey);
        }
        public string GetMessage()
        {
            return _transaction.Message;
        }
        public string GetSignature()
        {
            return _transaction.Signature;
        }
    }
}
