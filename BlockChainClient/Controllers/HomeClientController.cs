using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BlockChainClient.Controllers
{
    public class HomeClientController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MakeTransaction()
        {
            return View();
        }

        public IActionResult ViewTransactions()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ViewTransactions(string node_url)
        {
            var url = node_url.EndsWith('/') ? new Uri(node_url + "chain") : new Uri(node_url + "/chain");
            ViewBag.Blocks = GetChain(url);
            return View();
        }

        public IActionResult WalletTransactions()
        {
            return View(new List<Blockchain.Models.Transaction>());
        }

        [HttpPost]
        public IActionResult WalletTransactions(string publicKey)
        {
            var url = new Uri("https://localhost:44354" + "/chain");
            var blocks = GetChain(url);
            ViewBag.PublicKey = publicKey;
            return View(TransactionByAddress(publicKey, blocks));
        }


        private static List<Blockchain.Models.Block> GetChain(Uri url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var model = new
                {
                    chain = new List<Blockchain.Models.Block>(),
                    length = 0
                };
                string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var data = JsonConvert.DeserializeAnonymousType(json, model);
                return data.chain;
            }
            return null;
        }

        private static List<Blockchain.Models.Transaction> TransactionByAddress(string ownerAddress, List<Blockchain.Models.Block> chain)
        {
            List<Blockchain.Models.Transaction> trns = new();
            foreach (var block in chain.OrderByDescending(x => x.Index))
            {
                var ownerTransactions = block.Transactions.Where(x => x.Sender == ownerAddress || x.Recipient == ownerAddress);
                trns.AddRange(ownerTransactions);
            }
            return trns;
        }
        
    }
}
