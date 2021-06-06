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
        public static Blockchain _blockchain;
        public readonly Wallet _minerWallet;
        public BlockchainService()
        {
            _minerWallet = MinerService.minerWallet;
            _blockchain = new();
            Initialize();
        }
        private void Initialize()
        {
            _blockchain.MonetaryIssueWallet = MonetaryIssueService.Get();
            _blockchain.Reward = 50;
            _blockchain.Difficulty = 4;
            _blockchain.Nodes = new List<Node>();
            _blockchain.Blocks = new List<Block>();
            GetFromNet();
            if (_blockchain.Blocks == null || _blockchain.Blocks.Count == 0) Create();
        }
        private void GetFromNet()
        {
            NodeService nodeService = new();
            _blockchain.Nodes = nodeService.GetAllFromServer();
            if (_blockchain.Nodes == null) return;
            List<Blockchain> lstBlockchains = RequestFromNet();
            GetLargest(lstBlockchains);
        }
        private List<Blockchain> RequestFromNet()
        {
            List<Blockchain> lstBlockchains = new();
            foreach (Node node in _blockchain.Nodes)
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
            if (lstBlockchains == null) return;
            Blockchain newBlockchain = new();
            foreach (Blockchain blockchain in lstBlockchains)
            {
                if (blockchain.Blocks.Count > newBlockchain.Blocks.Count && IsValid(blockchain))
                    newBlockchain = blockchain;
            }
            if (newBlockchain != null) _blockchain = newBlockchain;
        }
        private bool IsValid(Blockchain blockchain)
        {
            Block block2 = blockchain.Blocks.ElementAt(2);
            Block block1 = blockchain.Blocks.ElementAt(1);
            if (block1.PreviousHash != "null!") return false;
            if (block1.Hash != "satoshiHash") return false;
            if (block2.PreviousHash != "satoshiHash") return false;
            if (block2.Hash != "satoshiHash") return false;
            if (!ProofOfWorkService.IsValid(block2, blockchain.Difficulty,
                                                     _blockchain.MonetaryIssueWallet.PublicKey)) return false;
            int i = 3;
            while (i < blockchain.Blocks.Count)
            {
                if (i>3)
                {
                    Block block = blockchain.Blocks.ElementAt(i);
                    Block lastBlock = blockchain.Blocks.ElementAt(i - 1);
                    if (block.PreviousHash != ProofOfWorkService.GetHash(lastBlock,
                                                      _blockchain.MonetaryIssueWallet.PublicKey)) return false;
                    if (!ProofOfWorkService.IsValid(block, blockchain.Difficulty,
                                                      _blockchain.MonetaryIssueWallet.PublicKey)) return false;
                }
                i++;
            }
            return true;
        }
        private void Create()
        {
            Transaction firstTransaction = new()    // use service
            {
                Amount = 50,
                Sender = _blockchain.MonetaryIssueWallet.PublicKey,
                Recipient = _minerWallet.PublicKey,
                Signature = "",
                Fees = 0
            };
            List<Transaction> lstTransactions = new();
            lstTransactions.Add(firstTransaction);
            Block firstBlock = new()
            {
                Index = 0,
                Timestamp = DateTime.UtcNow,
                PreviousHash = "null!",
                Transactions = lstTransactions,
                Nonce = new int(),
                Hash = ""
            };
            BlockService blockServ = new();
            blockServ.Create(firstBlock, _blockchain.Difficulty, _blockchain.MonetaryIssueWallet.PublicKey);
            _blockchain.Blocks.Add(blockServ.GetBlock());
        }
        public Blockchain Get()
        {
            return _blockchain;
        }

        // es la hora, crear block nuevo con lstTransactions e insertarlo en la blockchain

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
