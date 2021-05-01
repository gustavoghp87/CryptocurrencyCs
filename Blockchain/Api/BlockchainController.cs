using Blockchain.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Blockchain.Api
{
    [EnableCors("MyPolicy")]
    [Produces("application/json")]
    [Route("")]
    public class BlockchainController : Controller
    {
        public static readonly CryptoCurrency blockchain = new();

        [HttpPost("transactions/new")]
        public IActionResult New_transaction([FromBody] Transaction transaction)
        {
            return Ok(blockchain.CreateTransaction(transaction));
        }

        [HttpGet("transactions/get")]
        public IActionResult Get_transactions()
        {
            return Ok(new { transactions = blockchain.GetTransactions() });
        }

        [HttpGet("chain")]
        public IActionResult Full_Chain()
        {
            return Ok(new { chain = blockchain.GetBlocks(), length = blockchain.GetBlocks().Count });
        }

        [HttpGet("mine")]
        public IActionResult Mine()
        {
            var block = blockchain.Mine();
            return Ok(new
            {
                message = "New Block Forged",
                block_number = block.Index,
                transactions = block.Transactions.ToArray(),
                nonce = block.Proof,
                previousHash = block.PreviousHash
            });
        }

        [HttpPost("nodes/register")]
        public IActionResult Register_nodes(string[] nodes)
        {
            blockchain.RegisterNodes(nodes);
            return Created("", new
            {
                message = "New nodes have been created",
                total_nodes = nodes.Count()
            });
        }

        [HttpGet("nodes/resolve")]
        public IActionResult Consensus()
        {
            return Ok(blockchain.Consensus());
        }

        [HttpGet("nodes/get")]
        public IActionResult Get_nodes()
        {
            return Ok(new { nodes = blockchain.GetNodes() });
        }

        [HttpGet("wallet/miner")]
        public IActionResult GetMinersWallet()
        {
            return Ok(blockchain.GetMinersWallets());
        }
    }
}
