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
        public static Blockchain Blockchain;
        public readonly Wallet _minerWallet;
        public BlockchainService()
        {
            _minerWallet = MinerService.minerWallet;
            Blockchain = new();
            Initialize();
        }
        private void Initialize()
        {
            Blockchain.MonetaryIssueWallet = MonetaryIssueService.Get();
            Blockchain.Reward = 50;
            Blockchain.Difficulty = 4;
            Blockchain.Nodes = new List<Node>();
            Blockchain.Blocks = new List<Block>();
            GetFromNet();
            if (Blockchain.Blocks == null || Blockchain.Blocks.Count == 0) Create();
        }
        private void GetFromNet()
        {
            NodeService nodeService = new();
            Blockchain.Nodes = nodeService.GetAllFromServer();
            if (Blockchain.Nodes == null) return;
            List<Blockchain> lstBlockchains = RequestFromNet();
            GetLargest(lstBlockchains);
        }
        private List<Blockchain> RequestFromNet()
        {
            List<Blockchain> lstBlockchains = new();
            foreach (Node node in Blockchain.Nodes)
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
            if (newBlockchain != null) Blockchain = newBlockchain;
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
                                                     Blockchain.MonetaryIssueWallet.PublicKey)) return false;
            int i = 3;
            while (i < blockchain.Blocks.Count)
            {
                if (i>3)
                {
                    Block block = blockchain.Blocks.ElementAt(i);
                    Block lastBlock = blockchain.Blocks.ElementAt(i - 1);
                    ProofOfWorkService powService = new(lastBlock, Blockchain.Difficulty,
                        Blockchain.MonetaryIssueWallet.PublicKey);
                    if (block.PreviousHash != powService.GetHash()) return false;
                    if (!ProofOfWorkService.IsValid(block, blockchain.Difficulty,
                                                      Blockchain.MonetaryIssueWallet.PublicKey)) return false;
                }
                i++;
            }
            return true;
        }
        private void Create()
        {
            Transaction transaction = new()
            {
                Amount = 50,
                Sender = MonetaryIssueService.Get().PublicKey,
                Recipient = _minerWallet.PublicKey,
                Fees = 0,
                Miner = _minerWallet.PublicKey,
                Timestamp = DateTime.UtcNow
            };
            SignTransactionService signServ = new(transaction, _minerWallet.PrivateKey);
            TransactionRequest transactionReq = new()
            {
                Amount = 50,
                Sender = MonetaryIssueService.Get().PublicKey,
                Recipient = _minerWallet.PublicKey,
                Fees = 0,
                Miner = _minerWallet.PublicKey,
                Timestamp = DateTime.UtcNow,
                Message = signServ.GetMessage(),
                Signature = signServ.GetSignature()
            };
            TransactionService transactionServ = new();
            transactionServ.Add(transactionReq);
            List<Transaction> lstTransactions = transactionServ.Get();
            BlockService blockServ = new(0, "null!", lstTransactions, Blockchain.Difficulty,
                MonetaryIssueService.Get().PublicKey);
            Blockchain.Blocks.Add(blockServ.GetBlock());
        }
        public Blockchain Get()
        {
            return Blockchain;
        }

        public void Mine()
        {

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
