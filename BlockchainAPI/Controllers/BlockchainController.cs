using BlockchainAPI.Models;
using BlockchainAPI.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace BlockchainAPI.Api
{
    [ApiController]
    //[EnableCors("MyPolicy")]
    //[Produces("application/json")]
    [Route("api/")]
    public class BlockchainController : ControllerBase
    {
        private Blockchain _blockchain;

        public BlockchainController()
        {
            _blockchain = new Blockchain();
        }

        [HttpGet("blockchain")]
        public IActionResult Get()
        {
            BlockchainService blockchainServ = new();
            _blockchain = blockchainServ.Get();
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
