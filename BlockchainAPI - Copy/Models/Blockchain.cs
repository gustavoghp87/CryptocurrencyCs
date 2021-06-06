using System.Collections.Generic;

namespace BlockchainAPI.Models
{
    public class Blockchain
    {
        public List<Block> Blocks { get; set; }
        public decimal Reward { get; set; }
        public List<Node> Nodes { get; set; }
        public Wallet MonetaryIssueWallet { get; set; }
        public int Difficulty { get; set; }
    }
}
