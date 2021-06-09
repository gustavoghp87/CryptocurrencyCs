using BlockchainAPI.Models;
using BlockchainAPI.Services.Blocks;
using BlockchainAPI.Services.Nodes;
using BlockchainAPI.Services.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainAPI.Services.Blockchains
{
    public class BlockchainService : IBlockchainService
    {
        private Blockchain _blockchain;
        private readonly Wallet _minerWallet;
        private readonly NodeService _nodeServ;
        private readonly TransactionService _transactionServ;
        private List<Node> _lstNodes;
        public BlockchainService()
        {
            _nodeServ = new();
            _transactionServ = new();
            _blockchain = new();
            //_blockchain.MonetaryIssueWallet = new();
            _blockchain.MonetaryIssueWallet = MonetaryIssueService.Get();
            _minerWallet = MinerService.Get();
            Initialize();
        }
        private async void Initialize()
        {
            _lstNodes = _nodeServ.GetAll();
            _blockchain.Nodes = new();
            _blockchain.Nodes = _lstNodes;
            _nodeServ.RegisterMe();
            Blockchain largestBC = new GetBlockchainsFromNetService().GetLargest();
            if (largestBC != null && largestBC.Blocks != null && largestBC.Blocks.Count != 0)
            {
                _blockchain = largestBC;
            }
            else
            {
                _blockchain.Blocks = new();
                _blockchain.Difficulty = new();
                await Mine();
            }
        }

        public Blockchain Get()
        {
            return _blockchain;
        }
        public TransactionService GetTransactionService()
        {
            return _transactionServ;
        }
        public async Task<bool> Mine()
        {
            bool response = await PayMeReward();
            if (!response) return false;
            List<Transaction> lstTransactions = _transactionServ.GetAll();
            Block lastBlock;
            Block newBlock;
            int newDifficulty = new NewDifficulty().Get();

            if (_blockchain.Blocks.Count != 0)
            {
                lastBlock = _blockchain.Blocks.Last();
                newBlock = new BlockService(
                    lastBlock.Index + 1,
                    lastBlock.Hash,
                    lstTransactions,
                    newDifficulty)
                    .GetMined();
            }
            else
            {
                newBlock = new BlockService(
                1,
                "null!",
                lstTransactions,
                newDifficulty)
                .GetMined();
            }
            foreach (Block block in _blockchain.Blocks)
            {
                if (newBlock.Index == block.Index) return false;
            };
            _blockchain.Blocks.Add(newBlock);
            _blockchain.Difficulty = newDifficulty;
            
            
            // if (newBlock != null) SendToNodes();
            //_nodeServ.UpdateList();
            _transactionServ.Clear();
            return true;
        }
        private void SendToNodes()
        {
            SendToNodesService.Send(_lstNodes, _blockchain);
        }
        private async Task<bool> PayMeReward()
        {
            Transaction transaction = new()
            {
                Amount = 0,
                Fees = GetReward(),
                Miner = MinerService.Get().PublicKey,
                Recipient = MinerService.Get().PublicKey,
                Sender = MonetaryIssueService.Get().PublicKey,
                Timestamp = DateTime.UtcNow
            };
            SignTransactionService signServ = new(transaction, MonetaryIssueService.Get().PrivateKey);
            transaction.Message = signServ.GetMessage();
            transaction.Signature = signServ.GetSignature();
            return await _transactionServ.Add(transaction);
        }
        private int GetReward()                 // reducir Reward a la mitad cada 100 bloques
        {
            int reward = 50;
            int auxiliar = _blockchain.Blocks != null ? _blockchain.Blocks.Count : 0;
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
