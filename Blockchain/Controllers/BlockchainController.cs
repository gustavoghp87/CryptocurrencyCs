using BlockchainAPI.Models;
using BlockchainAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlockchainAPI.Api
{
    [EnableCors("MyPolicy")]
    [Produces("application/json")]
    [Route("api/")]
    public class BlockchainController : Controller
    {
        private Blockchain _blockchain;

        public BlockchainController()
        {
            _blockchain = new Blockchain();
            _blockchain = new BlockchainService.Initialize();


            NodeId = "18spHKNekiGLi89nrvCYiTaxtMaLtS3cvT";       // minerWallet.PublicKey;        L3wKuKpao6RmJSz6pVzU5pZDBa3wJ3ZwhKqkGSqD85cQdCDwtK3g
            if (CentralServerConnection.Connect(NodeId)) return;
            GetNodesFromServer();
            Consensus();
            if (_chain.Count == 0)
            {
                CreateNewBlockchain();
                Console.WriteLine("Created blockchain");
            }
        }

        [HttpGet("blockchain")]
        public IActionResult Get()
        {
            return Ok(_blockchain);
        }

        //[HttpPost("transactions/new")]
        //public IActionResult New_transaction([FromBody] Transaction transaction)
        //{
        //    var response = blockchain.CreateTransaction(transaction);
        //    if (!response) return NotFound();
        //    return Ok(response);
        //}

        //[HttpGet("transactions/get")]
        //public IActionResult Get_transactions()
        //{
        //    return Ok(new { transactions = blockchain.Get() });
        //}

        //[HttpGet("blockchain")]
        //public IActionResult Full_Chain()
        //{
        //    return Ok(new
        //    {
        //        chain = blockchain.GetBlocks(),
        //        length = blockchain.GetBlocks().Count
        //    });
        //}

        //[HttpGet("mine")]
        //public IActionResult Mine()
        //{
        //    var block = blockchain.Mine();
        //    return Ok(new
        //    {
        //        message = "New Block Forged",
        //        block_number = block.Index,
        //        transactions = block.Transactions.ToArray(),
        //        nonce = block.Nonce,
        //        previousHash = block.PreviousHash
        //    });
        //}

        //[HttpPost("nodes/register")]
        //public IActionResult Register_nodes(string[] nodes)
        //{
        //    blockchain.RegisterMany(nodes);
        //    return Created("", new
        //    {
        //        message = "New nodes have been created",
        //        total_nodes = nodes.Length
        //    });
        //}

        //[HttpGet("nodes/resolve")]
        //public IActionResult Consensus()
        //{
        //    return Ok(blockchain.Consensus());
        //}

        //[HttpGet("nodes/get")]
        //public IActionResult Get_nodes()
        //{
        //    return Ok(new { nodes = blockchain.GetNodes() });
        //}

        //[HttpGet("wallet/miner")]
        //public IActionResult GetMinersWallet()
        //{
        //    return Ok(blockchain.GetMinersWallets());
        //}
    }
}
