using BlockchainAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlockchainAPI.Services.Blockchains
{
    public static class BalanceService
    {
        public static async Task<decimal> Get(string publicKey, List<Transaction> lstCurrentTransactions,
            Blockchain blockchain = null)
        {
            string sender = publicKey;
            if (blockchain == null || blockchain.Blocks == null || blockchain.Blocks.Count == 0)
                blockchain = await GetBlockchain();
            if (blockchain == null || blockchain.Blocks == null || blockchain.Blocks.Count == 0)
                return 0;
            //if (lstCurrentTransactions == null || lstCurrentTransactions.Count == 0) return 0;
            List<Transaction> lstTransactions = GetAllByAddress(blockchain, lstCurrentTransactions, publicKey);
            decimal balance = 0;
            
            foreach (Transaction aTransaction in lstTransactions)
            {
                if (sender == aTransaction.Recipient)
                    balance += aTransaction.Amount;
                else if (sender == aTransaction.Sender)
                    balance -= aTransaction.Amount;
                if (sender == aTransaction.Miner)
                    balance += aTransaction.Fees;
                if (sender == MonetaryIssueService.Get().PublicKey)
                    balance -= aTransaction.Fees;
            }
            return balance;
        }
        private static List<Transaction> GetAllByAddress(Blockchain blockchain,
            List<Transaction> lstCurrentTransactions, string sender)
        {
            List<Transaction> lstTransactions = new();
            List<Block> lstBlocks = (from x in blockchain.Blocks select x).ToList();
            foreach (var block in lstBlocks.OrderByDescending(x => x.Index))
            {
                List<Transaction> ownerTransactions =
                    block.Transactions
                    .Where(x => x.Sender == sender || x.Recipient == sender || x.Miner == sender)
                    .ToList();
                lstTransactions.AddRange(ownerTransactions);
            }
            foreach (var aTransaction in lstCurrentTransactions)
            {
                if (aTransaction.Recipient == sender || aTransaction.Miner == sender)
                    lstTransactions.Add(aTransaction);
            }
            //lstTransactions.Add(transaction);
            return lstTransactions;
        }
        private static async Task<Blockchain> GetBlockchain()
        {
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
                }
                catch (JsonReaderException)
                {
                    Console.WriteLine("Invalid JSON.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("HTTP Response was invalid and cannot be deserialised.");
                return null;
            }
            return blockchain;
        }
    }
}
