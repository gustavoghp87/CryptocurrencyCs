using BlockchainAPI.Models;
using BlockchainAPI.Services.Transactions;
using System.Threading.Tasks;

namespace BlockchainAPI.Services.Blockchains
{
    public interface IBlockchainService
    {
        Blockchain Get();
        TransactionService GetTransactionService();
        Task<bool> Mine();
    }
}