﻿using BlockchainAPI.Models;

namespace BlockchainAPI.Services.Transactions
{
    public static class TransactionMessageService
    {
        public static string GenerateMessage(Transaction transaction)
        {
            return transaction.Timestamp.ToString() + "-"
                 + transaction.Sender + "-"
                 + transaction.Amount.ToString("0.00000000") + "-"
                 + transaction.Recipient + "-"
                 + transaction.Miner + "-"
                 + transaction.Fees;
        }
    }
}
