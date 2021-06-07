using BlockchainAPI.Models;
using BlockchainAPI.Services.Blocks;
using BlockchainAPI.Services.Nodes;
using BlockchainAPI.Services.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainAPI.Services.Blockchains
{
    public class BlockchainService
    {
        private static Blockchain _blockchain;
        private readonly Wallet _minerWallet;
        private List<Node> _lstNodes;
        private NodeService _nodeServ;
        private TransactionService _transactionServ;
        public BlockchainService()
        {
            _nodeServ = new();
            _transactionServ = new();
            _minerWallet = MinerService.Get;

            _blockchain = new();
            _lstNodes = _nodeServ.GetAll();
            _nodeServ.RegisterMe();
            _blockchain.Nodes = _lstNodes;
            Initialize();
        }
        private void Initialize()
        {
            _blockchain = new GetBlockchainsFromNetService().GetLargest();
            // RegisterMyNode ...
            if (_blockchain == null || _blockchain.Blocks == null || _blockchain.Blocks.Count == 0) Create();
        }
        private void Create()
        {
            _blockchain.MonetaryIssueWallet = MonetaryIssueService.Get();
            _blockchain.Reward = 50;
            _blockchain.Difficulty = 4;
            _blockchain.Nodes = new List<Node>();
            _blockchain.Blocks = new List<Block>();
            Transaction transaction = new()
            {
                Amount = 50,
                Sender = MonetaryIssueService.Get().PublicKey,
                Recipient = _minerWallet.PublicKey,
                Fees = 0,
                Miner = _minerWallet.PublicKey,
                Timestamp = DateTime.UtcNow
            };
            SignTransactionService signServ = new(transaction, MonetaryIssueService.Get().PrivateKey);
            TransactionRequest transactionReq = new()
            {
                Amount = transaction.Amount,
                Sender = transaction.Sender,
                Recipient = transaction.Recipient,
                Fees = transaction.Fees,
                Miner = transaction.Miner,
                Timestamp = transaction.Timestamp,
                Message = signServ.GetMessage(),
                Signature = signServ.GetSignature()
            };
            TransactionService transactionServ = new();
            transactionServ.Add(transactionReq);
            List<Transaction> lstTransactions = transactionServ.GetAll();
            BlockService blockServ = new(0, "null!", lstTransactions, _blockchain.Difficulty);
            _blockchain.Blocks.Add(blockServ.GetMined());
        }



        public Blockchain Get()
        {
            return _blockchain;
        }

        public void Mine()
        {
            PayMeReward();
            List<Transaction> lstTransactions = _transactionServ.GetAll();
            Block lastBlock = _blockchain.Blocks.Last();
            Block newBlock = new BlockService(
                lastBlock.Index + 1,
                lastBlock.PreviousHash,
                lstTransactions,
                _blockchain.Difficulty)
                .GetMined();
            _blockchain.Blocks.Add(newBlock);
            if (newBlock != null) SendToNodes();
            _nodeServ.UpdateList();
        }
        private void SendToNodes()
        {
            SendToNodesService.Send(_lstNodes, _blockchain);
        }

        private void PayMeReward()
        {
            TransactionRequest tReq = new()
            {
                Amount = GetReward(),
                Fees = 0,
                Sender = MonetaryIssueService.Get().PublicKey,
                Recipient = MinerService.Get().PublicKey,
                Timestamp = DateTime.UtcNow
            };
            _transactionServ.Add(tReq);
        }

        private int GetReward()                 // reducir Reward a la mitad cada 100 bloques
        {
            int reward = 50;
            int auxiliar = _blockchain.Blocks.Count;
            while (auxiliar / 100 > 1)
            {
                auxiliar /= 100;
                reward /= 2;
            }
            _blockchain.Reward = reward;
            return reward;
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
