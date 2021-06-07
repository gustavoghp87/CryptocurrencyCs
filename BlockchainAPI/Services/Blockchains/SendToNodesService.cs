using BlockchainAPI.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlockchainAPI.Services.Blockchains
{
    public static class SendToNodesService
    {
        public static void Send(List<Node> lstNodes, Blockchain blockchain)
        {
            foreach (Node node in lstNodes)
            {
                new HttpClient().PostAsJsonAsync(node.ToString() + "/new-blockchain", blockchain);
            }
        }
    }
}
