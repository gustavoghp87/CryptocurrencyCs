using BlockchainAPI.Models;
using BlockchainAPI.Services.Blockchains;
using BlockchainAPI.Services.Transactions;
using Microsoft.AspNetCore.Mvc;
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
    }
}
