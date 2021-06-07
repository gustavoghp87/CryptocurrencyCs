using BlockchainAPI.Models;
using BlockchainAPI.Services.Transactions;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAPI.Services
{
    public class TransactionService
    {
        private List<Transaction> _lstTransactions;
        private Transaction _transaction;
        private bool _success;
        public TransactionService()
        {
            _lstTransactions = new();
            _transaction = new();
            _success = false;
        }
        public bool Add(TransactionRequest transactionReq)
        {
            if (transactionReq.Sender == MonetaryIssueService.Get().PublicKey && transactionReq.Fees > 0) return false;
            GenerateTransaction(transactionReq);
            if (_transaction == null) return false;
            Create();
            // SendToNodes();
            return _success;
        }
        private void GenerateTransaction(TransactionRequest transactionReq)
        {
            _transaction.Amount = transactionReq.Amount;
            _transaction.Fees = transactionReq.Fees;
            _transaction.Miner = MinerService.minerWallet.PublicKey;
            _transaction.Recipient = transactionReq.Recipient;
            _transaction.Sender = transactionReq.Sender;
            _transaction.Signature = transactionReq.Signature;
            _transaction.Timestamp = transactionReq.Timestamp;
            _transaction.Message = TransactionMessageService.GenerateMessage(_transaction);
            // validar Timestamp dentro de la franja del bloque
            if (!IsVerified(_transaction)) _transaction = null;
        }
        private static bool IsVerified(Transaction transaction)
        {
            return WalletService.VerifyMessage(transaction);
        }
        private void Create()
        {
            if (_transaction.Sender == _transaction.Recipient) _success = false;
            if (!CheckJustOnePerTurn()) _success = false;
            if (!WalletService.VerifyMessage(_transaction)) _success = false;
            if (!HasBalance()) _success = false;
            _lstTransactions.Add(_transaction);
            _success = true;
        }
        private bool CheckJustOnePerTurn()
        {
            foreach (var transaction in _lstTransactions)
            {
                if (transaction.Sender == _transaction.Sender) return true;
            }
            return false;
        }
        private bool HasBalance()
        {
            if (_transaction.Sender == MonetaryIssueService.Get().PublicKey) return true;    // limitarlo a emisión
            List<Transaction> lstTransactions = GetAllByAddress();
            decimal balance = 0;
            foreach (var item in lstTransactions)
            {
                if (item.Recipient == _transaction.Sender)
                    balance += item.Amount;
                else if (item.Miner == _transaction.Sender)
                    balance += item.Amount;
                else if (item.Sender == _transaction.Sender)
                    balance -= item.Amount;
            }
            return balance >= _transaction.Amount + _transaction.Fees;
        }
        private List<Transaction> GetAllByAddress()
        {
            string senderAddress = _transaction.Sender;
            List<Transaction> lstTransactions = new();
            Blockchain blockchain = BlockchainService.Blockchain;
            List<Block> lstBlocks = (from x in blockchain.Blocks select x).ToList();
            foreach (var block in lstBlocks.OrderByDescending(x => x.Index))
            {
                List<Transaction> ownerTransactions =
                    block.Transactions
                    .Where(x => x.Sender == senderAddress || x.Recipient == senderAddress || x.Miner == senderAddress)
                    .ToList();
                lstTransactions.AddRange(ownerTransactions);
            }
            foreach (var transaction in _lstTransactions)
            {
                if (transaction.Recipient == senderAddress || transaction.Miner == senderAddress)
                    lstTransactions.Add(transaction);
            }
            lstTransactions.Add(_transaction);
            return lstTransactions;
        }
        public List<Transaction> Get()
        {
            return _lstTransactions;
        }
    }
}
