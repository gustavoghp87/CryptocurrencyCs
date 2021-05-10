using BlockchainClient.Models;
using Microsoft.AspNetCore.Mvc;
using RSA;

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
            var wallet = new Wallet();
            return Ok(new
            {
                private_key = wallet.PrivateKey,
                public_key = wallet.PublicKey
            });
        }

        [HttpPost("generate/transaction")]
        public IActionResult New_transaction(TransactionClient transaction)
        {
            if (transaction.Amount <= 0 || transaction.Sender == null || transaction.Recipient == null || transaction.SenderPrivKey == null) return NotFound();
            if (transaction.Fees !>= 0) transaction.Fees = 0;
            var sign = RSA.RSA.Sign(transaction.SenderPrivKey, transaction.ToString());
            return Ok(new
            {
                transaction,
                signature = sign
            });
        }
    }
}
