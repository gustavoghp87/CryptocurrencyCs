using BlockchainAPI.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BlockchainAPI.Services.Nodes
{
    public static class RegisterMeService
    {
        public static async void Send(List<Node> lstNodes)
        {
            int counter = 0;
            if (lstNodes == null || lstNodes.Count == 0) return;
            foreach (Node node in lstNodes)
            {
                //var response = await new HttpClient().GetAsync(node.ToString() + "register");
                //if (response.IsSuccessStatusCode) counter++;
            }
            Console.WriteLine(counter);
        }
    }
}
