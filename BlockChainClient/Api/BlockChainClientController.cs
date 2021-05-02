using BlockchainClient.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockChainClient.Api
{
    // [EnableCors("MyPolicy")]
    [Produces("application/json")]
    [Route("")]
    public class BlockChainClientController : Controller
    {
        [HttpGet("wallet/new")]
        public IActionResult New_wallet()
        {
            var wallet = RSA.RSA.KeyGenerate();
            return Ok(new
            {
                private_key = wallet.PrivateKey,
                public_key = wallet.PublicKey
            });
        }

        [HttpPost("generate/transaction")]
        public IActionResult New_transaction(TransactionClient transaction)
        {
            var sign = RSA.RSA.Sign(transaction.SenderPrivKey, transaction.ToString());
            return Ok(new
            {
                transaction,
                signature = sign
            });
        }
    }
}
