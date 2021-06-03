using BlockchainAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace BlockchainAPI.Services
{
    public class BlockchainService
    {
        private Blockchain _blockchain;
        public BlockchainService()
        {
            _blockchain = new();
            GetFromNet();
            if (_blockchain == null) Create();
        }
        public Blockchain Get()
        {
            return _blockchain;
        }
        private void GetFromNet()
        {
            var nodeService = new NodeService();
            var nodes = nodeService.GetAllFromServer();
            List<Blockchain> lstBlockchains = RequestFromNet(nodes);
            GetLargest(lstBlockchains);
        }
        private List<Blockchain> RequestFromNet(List<Node> nodes)
        {
            List<Blockchain> lstBlockchains = new();
            foreach (Node node in nodes)
            {
                try
                {
                    var url = new Uri(node.Address + "chain");
                    Console.WriteLine(url.ToString());
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var model = new
                        {
                            blockchain = new Blockchain(),
                            length = 0
                        };
                        string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        var data = JsonConvert.DeserializeAnonymousType(json, model);
                        lstBlockchains.Add(data.blockchain);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return lstBlockchains;
        }
        private void GetLargest(List<Blockchain> lstBlockchains)
        {
            Blockchain newBlockchain = new();
            foreach (Blockchain blockchain in lstBlockchains)
            {
                if (blockchain.Blocks.Count > newBlockchain.Blocks.Count && IsValid(blockchain))
                    newBlockchain = blockchain;
            }
            _blockchain = newBlockchain;
        }
        private bool IsValid(Blockchain blockchain)
        {
            Block block2 = blockchain.Blocks.ElementAt(2);
            Block block1 = blockchain.Blocks.ElementAt(1);
            if (block1.PreviousHash != "null!") return false;
            if (block1.Hash != "satoshiHash") return false;
            if (block2.PreviousHash != "satoshiHash") return false;
            if (block2.Hash != "satoshiHash") return false;
            if (!ProofOfWorkService.IsValid(block2, _blockchain.MonetaryIssueWallet.PublicKey,
                                                                            blockchain.Difficulty)) return false;
            int i = 3;
            while (i < blockchain.Blocks.Count)
            {
                if (i>3)
                {
                    Block block = blockchain.Blocks.ElementAt(i);
                    Block lastBlock = blockchain.Blocks.ElementAt(i - 1);
                    if (block.PreviousHash != ProofOfWorkService.GetHash(lastBlock,
                                                            _blockchain.MonetaryIssueWallet.PublicKey)) return false;
                    if (!ProofOfWorkService.IsValid(block, _blockchain.MonetaryIssueWallet.PublicKey,
                                                                            blockchain.Difficulty)) return false;
                }
                i++;
            }
            return true;
        }
        private void Create()
        {
            Wallet monetaryIssueWallet = new()
            {
                PrivateKey = "L27gRq59TSnXTWanV1SdgHRucFtfqZciec5Grooc6MDPe4o47T5V",
                PublicKey = "1GPuEJZ6rjh7WfdQwNqUPWgsud95RLBUfK"
            };
            Wallet minerWallet = new();
            minerWallet = WalletService.Generate();

            Blockchain newBlockchain = new()
            {
                MonetaryIssueWallet = monetaryIssueWallet,
                CurrentTransactions = new List<Transaction>(),
                MinerWallet = minerWallet,
                Difficulty = 4,
                Blocks = new List<Block>(),
                Nodes = new List<Node>(),
                Reward = 50
            };
            Transaction newTransaction = new()
            {
                Amount = 50,
                Sender = monetaryIssueWallet.PublicKey,
                Recipient = minerWallet.PublicKey,
                Signature = "",
                Fees = 0
            };
            newBlockchain.CurrentTransactions.Add(newTransaction);

            var firstBlockPrevData = new BlockPrevData
            {
                Index = 0,
                Timestamp = DateTime.UtcNow,
                PreviousHash = "null!",
                Transactions = newBlockchain.CurrentTransactions
            };
            Block newBlock = new BlockService().Create(firstBlockPrevData, 0, "satoshiHash");
            Miner(newBlock)
            _blockchain = newBlockchain;
        }


        //public string GetFull()
        //{
        //    var response = new
        //    {
        //        chain = _chain.ToArray(),
        //        lenght = _chain.Count
        //    };
        //    return JsonConvert.SerializeObject(response);
        //}

        //internal object Consensus()
        //{
        //    bool replaced = ResolveConflicts();
        //    string message = replaced ? "was replaced" : "is authoritive";
        //    var response = new
        //    {
        //        Message = $"Our chain {message}",
        //        Chain = _chain
        //    };
        //    return response;
        //}

    }
}
