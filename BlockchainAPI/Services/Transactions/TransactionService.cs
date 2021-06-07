using BlockchainAPI.Models;
using BlockchainAPI.Services.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAPI.Services
{
    public class TransactionService
    {
        private List<Transaction> _lstTransactions;
        private Transaction _transaction;
        private Transaction _transactionFee;
        public TransactionService()
        {
            _lstTransactions = new();
            _transaction = null;
            _transaction = null;
        }
        public bool Add(TransactionRequest transactionReq)
        {
            bool paysFee = transactionReq.Fees > 0 && transactionReq.Sender != MonetaryIssueService.Get().PublicKey;
            GenerateTransaction(transactionReq);
            if (_transaction != null) return false;
            if (paysFee)
            {
                GenerateFeeTransaction(transactionReq);
                if (_transactionFee == null) return false;
            }
            Create();
            // SendToNodes();
            return true;
        }
        private void GenerateTransaction(TransactionRequest transactionReq)
        {
            _transaction.Amount = transactionReq.Amount;
            //_transaction.Miner = MinerService.minerWallet.PublicKey;
            _transaction.Recipient = transactionReq.Recipient;
            _transaction.Sender = transactionReq.Sender;
            _transaction.Signature = transactionReq.Signature;
            _transaction.Timestamp = transactionReq.Timestamp;
            _transaction.Message = TransactionMessageService.GenerateMessage(_transaction);
            if (!IsVerified(_transaction)) _transaction = null;
        }
        private void GenerateFeeTransaction(TransactionRequest transactionReq)
        {
            _transactionFee.Amount = transactionReq.Fees;
            //_transactionFee.Miner = MinerService.minerWallet.PublicKey;
            _transactionFee.Recipient = MinerService.minerWallet.PublicKey;
            _transactionFee.Sender = transactionReq.Sender;
            _transactionFee.Signature = transactionReq.Signature;
            _transactionFee.Timestamp = DateTime.UtcNow;
            _transactionFee.Message = TransactionMessageService.GenerateMessage(_transactionFee); ;
            if (IsVerified(_transactionFee)) _transactionFee = null;
        }
        private static bool IsVerified(Transaction transaction)
        {
            return WalletService.VerifyMessage(transaction);
        }
        private bool Create()
        {
            if (_transaction.Sender == _transaction.Recipient) return false;
            if (!CheckJustOnePerTurn()) return false;
            if (!WalletService.VerifyMessage(_transaction)) return false;
            if (!HasBalance()) return false;

            if (_transactionFee != null)
            {
                if (_transactionFee.Sender == _transactionFee.Recipient) return false;
                if (!WalletService.VerifyMessage(_transactionFee)) return false;
            }
            return true;
        }
        private bool CheckJustOnePerTurn()
        {
            foreach (var transaction in _lstTransactions)
            {
                if (transaction.Sender == _transaction.Sender) return true; // en realidad son dos si hay fee
            }
            return false;
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
            if (_transactionFee != null && _transactionFee.Amount > 0)
                return balance >= (_transaction.Amount + _transactionFee.Amount);
            else
                return balance >= _transaction.Amount;
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
        public List<Transaction> Get()
        {
            return _lstTransactions;
        }
    }
}
