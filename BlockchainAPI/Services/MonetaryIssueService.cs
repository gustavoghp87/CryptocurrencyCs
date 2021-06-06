using BlockchainAPI.Models;

namespace BlockchainAPI.Services
{
    public static class MonetaryIssueService
    {
        public static Wallet Get()
        {
            return new Wallet
            {
                PrivateKey = "L27gRq59TSnXTWanV1SdgHRucFtfqZciec5Grooc6MDPe4o47T5V",
                PublicKey = "1GPuEJZ6rjh7WfdQwNqUPWgsud95RLBUfK",
                BitcoinAddress = "12EWT461aNMMfjEGteJ6Bz8BWmDeB1Efkj"
            };
        }
    }
}
