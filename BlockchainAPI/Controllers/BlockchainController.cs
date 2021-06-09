using BlockchainAPI.Models;
using BlockchainAPI.Services;
using BlockchainAPI.Services.Blockchains;
using BlockchainAPI.Services.Transactions;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockchainAPI.Api
{
    [ApiController]
    //[EnableCors("MyPolicy")]
    [Produces("application/json")]
    [Route("/")]
    public class BlockchainController : ControllerBase
    {
        private Blockchain _blockchain;
        private IBlockchainService _blockchainServ;

        public BlockchainController(IBlockchainService blockchainService)
        {
            _blockchainServ = blockchainService;
            _blockchain = new();
            _blockchain = _blockchainServ.Get();
        }
        private void UpdateBlockchain()
        {
            _blockchain = _blockchainServ.Get();
        }

        [HttpGet()]
        public IActionResult GetBlockchain()
        {
            UpdateBlockchain();
            return Ok(_blockchain);
        }

        [HttpGet("/transaction")]
        public IActionResult GetTransactions()
        {
            List<Transaction> lstTransactions = _blockchainServ.GetTransactionService().GetAll();
            return Ok(lstTransactions);
        }

        [HttpPost("/transaction")]
        public async Task<IActionResult> AddTransaction(Transaction transactionRequest)
        {
            bool response = await _blockchainServ.GetTransactionService().Add(transactionRequest);
            if (!response) return BadRequest();
            UpdateBlockchain();
            return Ok(transactionRequest);
        }

        [HttpPost("/signature")]
        public IActionResult Sign(Transaction transactionRequest, string privateKey)
        {
            if (privateKey == null | privateKey == "") return BadRequest();
            var serv = new SignTransactionService(transactionRequest, privateKey);
            transactionRequest.Message = serv.GetMessage();
            transactionRequest.Signature = serv.GetSignature();
            return Ok(transactionRequest);
        }

        [HttpGet("/mine")]
        public async Task<IActionResult> Mine()
        {
            bool response = await _blockchainServ.Mine();
            if (!response) return BadRequest();
            UpdateBlockchain();
            return Ok(_blockchain);
        }

        [HttpGet("/balance/{publicKey}")]
        public async Task<IActionResult> GetBalance(string publicKey)
        {
            UpdateBlockchain();
            if (publicKey == "") return BadRequest();
            List<Transaction> lstCurrentTransactions = _blockchainServ.GetTransactionService().GetAll();
            decimal balance = await BalanceService.Get(publicKey, lstCurrentTransactions, _blockchain);
            return Ok(balance);
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
