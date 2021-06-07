using BlockchainAPI.Models;
using BlockchainAPI.Services.Transactions;

namespace BlockchainAPI.Services
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
            _transaction.Message = TransactionMessageService.GenerateMessage(_transaction);
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
