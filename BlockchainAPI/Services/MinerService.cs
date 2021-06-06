using BlockchainAPI.Models;

namespace BlockchainAPI.Services
{
    public class MinerService
    {
        public static Wallet minerWallet = new Wallet
        {
            PublicKey = "1BPiqwmT9ig8cSfeRCiJaJU7qK4KrPKWhc",
            PrivateKey = "L4fkiGDz1jdeTqo2rDUehWEWtDi3zhTnHwETi46zN9XGLoiAb9Rd",
            BitcoinAddress = "1CvAdfEfhfhGSF8kbK7r2sB4DcKcSQi8GT"
        };
        public MinerService()
        {

        }
    }
}
