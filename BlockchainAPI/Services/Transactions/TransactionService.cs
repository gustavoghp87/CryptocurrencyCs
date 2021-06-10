using BlockchainAPI.Models;
using BlockchainAPI.Services.Blockchains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockchainAPI.Services.Transactions
{
    public class TransactionService
    {
        private List<Transaction> _lstTransactions;
        public TransactionService()
        {
            _lstTransactions = new();
        }
        public async Task<bool> Add(Transaction transactionReq)
        {
            Transaction transaction = new();
            transaction.Amount = transactionReq.Amount;
            transaction.Fees = transactionReq.Fees;
            transaction.Miner = MinerService.Get().PublicKey;
            transaction.Recipient = transactionReq.Recipient;
            transaction.Sender = transactionReq.Sender;
            transaction.Signature = transactionReq.Signature;
            transaction.Timestamp = transactionReq.Timestamp;
            transaction.Message = TransactionMessageService.Generate(transaction);
            if (transactionReq.Amount !> 0 && transaction.Fees !> 0) return false;
            if (transactionReq.Sender == IssuerService.Get().PublicKey && transactionReq.Amount > 0) return false;
            if(!IsVerified(transaction)) return false;
            bool success = await Create(transaction);
            // SendToNodes();
            return success;
        }
        //private bool GenerateTransaction(Transaction transaction)
        //{
            // validar Timestamp dentro de la franja del bloque
          //  if (!IsVerified(transaction)) transaction = null;
        //}
        private static bool IsVerified(Transaction transaction)
        {
            return WalletService.IsVerifiedMessage(transaction);
        }
        private async Task<bool> Create(Transaction transaction)
        {
            if (transaction.Sender == transaction.Recipient) return false;
            if (!CheckJustOnePerTurn(transaction)) return false;
            if (!await HasBalance(transaction)) return false;
            _lstTransactions.Add(transaction);
            return true;
        }
        private bool CheckJustOnePerTurn(Transaction transaction)
        {
            foreach (var aTransaction in _lstTransactions)
            {
                if (aTransaction.Sender == transaction.Sender) return false;
            }
            return true;
        }
        private async Task<bool> HasBalance(Transaction transaction)
        {
            if (transaction.Sender == IssuerService.Get().PublicKey) return true;    // limitarlo a emisión
            
            // ver que el signature no se haya usado ya
            int auxiliar = 0;
            foreach (Transaction aTransaction in _lstTransactions)
            {
                if (aTransaction.Signature == transaction.Signature
                    && aTransaction.Timestamp == transaction.Timestamp)
                {
                    auxiliar++;
                    if (auxiliar > 1) return false;
                }
            }
            decimal balance = await BalanceService.Get(transaction.Sender, _lstTransactions);
            return balance >= transaction.Amount + transaction.Fees;
        }
        public List<Transaction> GetAll()
        {
            return _lstTransactions;
        }
        public void Clear()
        {
            _lstTransactions.Clear();
        }
    }
}
