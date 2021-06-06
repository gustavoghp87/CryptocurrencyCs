using BlockchainAPI.Models;
using System.Collections.Generic;

namespace BlockchainAPI.Services
{
    public class TransactionService
    {
        private Transaction _transaction;
        public TransactionService()
        {
            _transaction = new();
        }
        public override string ToString()
        {
            return _transaction.Amount.ToString("0.00000000") + _transaction.Recipient + _transaction.Sender;
        }

        //public void Add(Transaction transaction)
        //{
        //    _currentTransactions.Add(transaction);
        //    if (transaction.Sender != NodeId && transaction.Fees > 0)
        //        _currentTransactions.Add(new Transaction
        //        {
        //            Fees = transaction.Fees,
        //            Sender = transaction.Sender,
        //            Recipient = NodeId,
        //            Signature = "",
        //            Amount = 0
        //        });      // add every transaction's fees
        //}

        //internal bool Create(Transaction transaction)
        //{
        //    if (!CheckJustOnePerTurn(transaction)) return false;                    //    just one transaction per turn
        //    var verified = Verify_transaction_signature(transaction);
        //    if (!verified || transaction.Sender == transaction.Recipient) return false;
        //    if (!HasBalance(transaction)) return false;
        //    AddTransaction(transaction);
        //    // var blockIndex = _chain.Last() != null ? _chain.Last().Index + 1 : 0;
        //    return true;
        //}

        //public bool CheckJustOnePerTurn(Transaction transaction)
        //{
        //    var senderDone = false;
        //    foreach (var newTransaction in _currentTransactions)
        //    {
        //        if (newTransaction.Sender == transaction.Sender) senderDone = true;
        //    }
        //    return !senderDone;
        //}

        //public List<Transaction> Get()
        //{
        //    return _currentTransactions;
        //}

        //private static bool Verify_transaction_signature(Transaction transaction)
        //{
        //    bool verified = BlockchainAPI.WalletService.Verify(transaction.Sender, transaction.ToString(), transaction.Signature);
        //    return verified;
        //}

        //private List<Transaction> GetByAddress(string ownerAddress)
        //{
        //    List<Transaction> trns = new();
        //    foreach (var block in _chain.OrderByDescending(x => x.Index))
        //    {
        //        var ownerTransactions = block.Transactions.Where(x => x.Sender == ownerAddress || x.Recipient == ownerAddress);
        //        trns.AddRange(ownerTransactions);
        //    }
        //    //foreach (var transaction in _currentTransactions)
        //    //{
        //    //    if (transaction.Sender == ownerAddress || transaction.Recipient == ownerAddress)
        //    //    trns.Add(transaction);
        //    //}
        //    return trns;
        //}

        //public bool HasBalance(Transaction transaction)
        //{
        //    var trns = GetByAddress(transaction.Sender);
        //    decimal balance = 0;
        //    foreach (var item in trns)
        //    {
        //        if (item.Recipient == transaction.Sender)
        //            balance += item.Amount;
        //        else
        //            balance -= item.Amount;
        //    }
        //    return balance >= (transaction.Amount + transaction.Fees);
        //}

        //public void PayReward()
        //{
        //    if (_blockCount == 10)
        //    {
        //        _blockCount = 0;
        //        _reward /= 2;
        //    }
        //    AddTransaction(new Transaction
        //    {
        //        _reward,
        //        MonetaryIssue.Public,
        //        NodeId,
        //        "",
        //        0
        //    });                  //   add reward to next block
        //    _blockCount++;
        //}
    }
}
