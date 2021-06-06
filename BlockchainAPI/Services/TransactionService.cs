using BlockchainAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAPI.Services
{
    public class TransactionService
    {
        private List<Transaction> _lstTransactions;
        private Transaction _transaction;
        public TransactionService()
        {
            _transaction = new();
        }
        public List<Transaction> Get()
        {
            return _lstTransactions;
        }
        public bool Add(Transaction transaction)
        {
            _transaction = transaction;
            GenerateMessage();
            if (!Create()) return false;
            transaction = _transaction;

            bool paysFee = transaction.Fees > 0 && transaction.Sender != MonetaryIssueService.Get().PublicKey;
            if (paysFee)
            {
                Transaction feeTransaction = PayFee(transaction);
                if (feeTransaction == null) return false;
                _lstTransactions.Add(transaction);
                _lstTransactions.Add(feeTransaction);
            }
            else
            {
                _lstTransactions.Add(transaction);
            }
            // SendToNodes();
            return true;
        }
        private Transaction PayFee(Transaction transaction)
        {
            _transaction = new();
            _transaction.Fees = 0;
            _transaction.Amount = transaction.Fees;
            _transaction.Sender = transaction.Sender;
            _transaction.Recipient = MinerService.minerWallet.PublicKey;
            _transaction.Signature = transaction.Signature;
            GenerateMessage();
            if (!Create()) return null;
            return _transaction;
        }
        private bool Create()
        {
            if (!CheckJustOnePerTurn()) return false;
            if (_transaction.Sender == _transaction.Recipient) return false;
            if (!VerifySignature()) return false;
            if (!HasBalance()) return false;
            return true;
        }
        private void GenerateMessage()
        {
            _transaction.Message =
                _transaction.Amount.ToString("0.00000000")
                + _transaction.Fees.ToString("0.00000000")
                + _transaction.Recipient
                + MinerService.minerWallet
                + _transaction.Sender;
        }
        private bool CheckJustOnePerTurn()
        {
            foreach (var transaction in _lstTransactions)
            {
                if (transaction.Sender == _transaction.Sender) return true;
            }
            return false;
        }
        private bool VerifySignature()
        {
            return WalletService.VerifyMessage(_transaction);
        }
        private bool HasBalance()
        {
            List<Transaction> lstTransactions = GetAllByAddress();
            decimal balance = 0;
            foreach (var item in lstTransactions)
            {
                if (item.Recipient == _transaction.Sender)
                    balance += item.Amount;
                else if (item.Sender == _transaction.Sender)
                    balance -= item.Amount;
            }
            return balance >= (_transaction.Amount + _transaction.Fees);
        }
        private List<Transaction> GetAllByAddress()
        {
            string senderAddress = _transaction.Sender;
            List<Transaction> lstTransactions = new();
            Blockchain blockchain = BlockchainService._blockchain;
            List<Block> lstBlocks = (from x in blockchain.Blocks select x).ToList();
            foreach (var block in lstBlocks.OrderByDescending(x => x.Index))
            {
                List<Transaction> ownerTransactions = block.Transactions
                    .Where(x => x.Sender == senderAddress || x.Recipient == senderAddress).ToList();
                lstTransactions.AddRange(ownerTransactions);
            }
            foreach (var transaction in _lstTransactions)
            {
                if (transaction.Recipient == senderAddress)
                    lstTransactions.Add(transaction);
            }
            lstTransactions.Add(_transaction);
            return lstTransactions;
        }





        public Transaction Sign(Transaction transaction, string privateKey)
        {
            _transaction = transaction;
            GenerateMessage();
            _transaction.Signature = WalletService.SignMessage(transaction, privateKey);
            return transaction;
        }
    }
}
