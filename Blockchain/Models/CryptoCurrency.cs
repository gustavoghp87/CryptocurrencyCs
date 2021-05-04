using Newtonsoft.Json;
using RSA;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Blockchain.Models
{
    public class CryptoCurrency
    {
        private int _blockCount = 0;
        private decimal _reward = 50;
        private readonly Wallet minerWallet = new();
        public List<Transaction> _currentTransactions = new();
        public List<Node> _nodes = new();
        public List<Block> _chain = new();
        public string NodeId { get; private set; }

        public CryptoCurrency()
        {
            NodeId = "18spHKNekiGLi89nrvCYiTaxtMaLtS3cvT";           // minerWallet.PublicKey;        L3wKuKpao6RmJSz6pVzU5pZDBa3wJ3ZwhKqkGSqD85cQdCDwtK3g
            _currentTransactions.Add(new Transaction(50, "Monetary Issue", NodeId, "", 0));
            CreateNewBlock(nonce: 100, previousHash: "1");
        }

        private void RegisterNode(string address)
        {
            _nodes.Add(new Node { Address = new Uri(address) });
        }

        private Block CreateNewBlock(int nonce, string previousHash = null)
        {
            // Console.WriteLine(_chain.Count.ToString(), DateTime.UtcNow, _currentTransactions.ToList(), proof, previousHash ?? GetHash(_chain.Last()));
            var block = new Block
            (
                _chain.Count,
                DateTime.UtcNow,
                _currentTransactions.ToList(),
                nonce,
                previousHash ?? GetHash(_chain.Last())
            );
            _currentTransactions.Clear();
            _chain.Add(block);
            return block;
        }

        private static string GetHash(Block block)
        {
            string blockText = JsonConvert.SerializeObject(block);
            return GetSha256(blockText);
        }

        private static string GetSha256(string data)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(data);
            byte[] hash = new SHA256Managed().ComputeHash(bytes);
            var hashBuilder = new StringBuilder();
            foreach (byte x in hash)
                hashBuilder.Append($"{x:x2}");
            return hashBuilder.ToString();
        }

        private int CreateProofOfWork(string previousHash)
        {
            int nonce = 0;
            while (!IsValidProof(_currentTransactions, nonce, previousHash))
                nonce++;
            if (_blockCount==10)
            {
                _blockCount = 0;
                _reward = _reward / 2;
            }
            AddTransaction(new Transaction(_reward, "Monetary Issue Again", NodeId, "", 0));
            _blockCount++;
            return nonce;
        }

        private static bool IsValidProof(List<Transaction> transactions, int nonce, string previousHash)
        {
            var signatures = transactions.Select(x => x.Signature).ToArray();
            string guess = $"{signatures}{nonce}{previousHash}";
            string result = GetSha256(guess);
            return result.StartsWith("00000");
        }

        private static bool Verify_transaction_signature(Transaction transaction)
        {
            bool verified = RSA.RSA.Verify(transaction.Sender, transaction.ToString(), transaction.Signature);
            return verified;
        }

        private List<Transaction> TransactionByAddress(string ownerAddress)
        {
            List<Transaction> trns = new();
            foreach (var block in _chain.OrderByDescending(x => x.Index))
            {
                var ownerTransactions = block.Transactions.Where(x => x.Sender == ownerAddress || x.Recipient == ownerAddress);
                trns.AddRange(ownerTransactions);
            }
            //foreach (var transaction in _currentTransactions)
            //{
            //    if (transaction.Sender == ownerAddress || transaction.Recipient == ownerAddress)
            //    trns.Add(transaction);
            //}
            return trns;
        }

        public bool HasBalance(Transaction transaction)
        {
            var trns = TransactionByAddress(transaction.Sender);
            decimal balance = 0;
            foreach (var item in trns)
            {
                if (item.Recipient == transaction.Sender)
                    balance += item.Amount;
                else
                    balance -= item.Amount;
            }
            return balance >= (transaction.Amount + transaction.Fees);
        }

        private void AddTransaction(Transaction transaction)
        {
            _currentTransactions.Add(transaction);
            if (transaction.Sender != NodeId && transaction.Fees > 0)
                _currentTransactions.Add(new Transaction (transaction.Fees, transaction.Sender, NodeId, "", 0 ) );      // add every transaction's fees
        }

        private bool ResolveConflicts()
        {
            List<Block> newChain = null;
            foreach (Node node in _nodes)
            {
                var url = new Uri(node.Address, "/chain");
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var model = new { chain = new List<Block>(), length = 0 };
                    string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    var data = JsonConvert.DeserializeAnonymousType(json, model);
                    if (data.chain.Count > _chain.Count && IsValidChain(data.chain))
                        newChain = data.chain;
                }
            }
            if (newChain != null)
            {
                _chain = newChain;
                return true;
            }
            return false;
        }

        private static bool IsValidChain(List<Block> chain)
        {
            int currentIndex = 1;
            while (currentIndex < chain.Count)
            {
                Block block = chain.ElementAt(currentIndex);
                Block lastBlock = chain.ElementAt(currentIndex-1);
                if (block.PreviousHash != GetHash(lastBlock)) return false;
                if (!IsValidProof(block.Transactions, block.Nonce, lastBlock.PreviousHash)) return false;
                currentIndex++;
            }
            return true;
        }


        // web server calls

        internal Block Mine()
        {
            int nonce = CreateProofOfWork(_chain.Last().PreviousHash);
            Block block = CreateNewBlock(nonce);
            return block;
        }

        internal string GetFullChain()
        {
            var response = new { chain = _chain.ToArray(), lenght = _chain.Count };
            return JsonConvert.SerializeObject(response);
        }

        internal string RegisterNodes(string[] nodes)
        {
            var builder = new StringBuilder();
            foreach (string node in nodes)
            {
                string url = node;
                RegisterNode(url);
                builder.Append($"{url}, ");
            }
            builder.Insert(0, $"{nodes.Length} new nodes have been added: ");
            string result = builder.ToString();
            return result.Substring(0, result.Length - 2);
        }

        internal object Consensus()
        {
            bool replaced = ResolveConflicts();
            string message = replaced ? "was replaced" : "is authoritive";
            var response = new { Message = $"Our chain {message}", Chain = _chain };
            return response;
        }

        internal bool CreateTransaction(Transaction transaction)
        {
            if (!CheckJustOneTransactionPerTurn(transaction)) return false;                    //    just one transaction per turn
            var verified = Verify_transaction_signature(transaction);
            if (!verified || transaction.Sender == transaction.Recipient) return false;
            if (!HasBalance(transaction)) return false;
            AddTransaction(transaction);
            // var blockIndex = _chain.Last() != null ? _chain.Last().Index + 1 : 0;
            return true;
        }

        internal List<Transaction> GetTransactions()
        {
            return _currentTransactions;
        }

        internal List<Block> GetBlocks()
        {
            return _chain;
        }

        internal List<Node> GetNodes()
        {
            return _nodes;
        }

        internal Wallet GetMinersWallets()
        {
            return minerWallet;
        }

        public bool CheckJustOneTransactionPerTurn(Transaction transaction)
        {
            var senderDone = false;
            foreach (var newTransaction in _currentTransactions)
            {
                if (newTransaction.Sender == transaction.Sender) senderDone = true;
            }
            return !senderDone;
        }
    }
}
