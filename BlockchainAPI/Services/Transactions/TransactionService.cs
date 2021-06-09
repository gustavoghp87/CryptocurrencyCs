using BlockchainAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlockchainAPI.Services.Transactions
{
    public class TransactionService
    {
        private List<Transaction> _lstTransactions;
        private bool _success;
        public TransactionService()
        {
            _lstTransactions = new();
            _success = false;
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
            if (transactionReq.Sender == MonetaryIssueService.Get().PublicKey && transactionReq.Fees > 0) return false;
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
            return WalletService.VerifyMessage(transaction);
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
            if (transaction.Sender == MonetaryIssueService.Get().PublicKey) return true;    // limitarlo a emisión
            List<Transaction> lstTransactions = await GetAllByAddress(transaction);
            decimal balance = 0;
            int auxiliar = 0;
            foreach (Transaction aTransaction in lstTransactions)
            {
                if (aTransaction.Signature == transaction.Signature
                    && aTransaction.Timestamp == transaction.Timestamp)
                {
                    auxiliar++;
                    if (auxiliar > 1) return false;
                }
                if (aTransaction.Recipient == transaction.Sender)
                    balance += aTransaction.Amount;
                else if (aTransaction.Miner == transaction.Sender)
                    balance += aTransaction.Amount;
                else if (aTransaction.Sender == transaction.Sender)
                    balance -= aTransaction.Amount;
            }
            return balance >= transaction.Amount + transaction.Fees;
        }
        private async Task<List<Transaction>> GetAllByAddress(Transaction transaction)
        {
            string senderAddress = transaction.Sender;
            List<Transaction> lstTransactions = new();
            Blockchain blockchain = null;
            using var httpResponse = await new HttpClient().GetAsync("https://localhost:5001/", HttpCompletionOption.ResponseHeadersRead);
            httpResponse.EnsureSuccessStatusCode();
            if (httpResponse.Content is object && httpResponse.Content.Headers.ContentType.MediaType == "application/json")
            {
                var contentStream = await httpResponse.Content.ReadAsStreamAsync();
                using var streamReader = new StreamReader(contentStream);
                using var jsonReader = new JsonTextReader(streamReader);
                JsonSerializer serializer = new();
                try
                {
                    blockchain = serializer.Deserialize<Blockchain>(jsonReader);
                    List<Block> lstBlocks = (from x in blockchain.Blocks select x).ToList();
                    foreach (var block in lstBlocks.OrderByDescending(x => x.Index))
                    {
                        List<Transaction> ownerTransactions =
                            block.Transactions
                            .Where(x => x.Sender == senderAddress || x.Recipient == senderAddress || x.Miner == senderAddress)
                            .ToList();
                        lstTransactions.AddRange(ownerTransactions);
                    }
                    foreach (var aTransaction in _lstTransactions)
                    {
                        if (aTransaction.Recipient == senderAddress || aTransaction.Miner == senderAddress)
                            lstTransactions.Add(aTransaction);
                    }
                    lstTransactions.Add(transaction);
                }
                catch (JsonReaderException) { Console.WriteLine("Invalid JSON."); }
            }
            else Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
            return lstTransactions;
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
