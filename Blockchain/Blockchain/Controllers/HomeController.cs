﻿using Blockchain.Api;
using Blockchain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blockchain.Controllers
{
    public class HomeController : Controller
    {
        private readonly CryptoCurrency _blockchain;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _blockchain = BlockchainController.blockchain;
        }

        public IActionResult Index()
        {
            List<Transaction> transactions = _blockchain.GetTransactions();
            List<Block> blocks = _blockchain.GetBlocks();
            ViewBag.Transactions = transactions;
            ViewBag.Blocks = blocks;
            return View();
        }

        public IActionResult Mine()
        {
            _blockchain.Mine();
            return RedirectToAction("Index");
        }

        public IActionResult Configure()
        {
            return View(_blockchain.GetNodes());
        }

        [HttpPost]
        public IActionResult RegisterNodes(string nodes)
        {
            string[] node = nodes.Split(',');
            _blockchain.RegisterNodes(node);
            return RedirectToAction("Configure");
        }

        [HttpGet]
        public IActionResult CoinBase()
        {
            List<Block> blocks = _blockchain.GetBlocks();
            ViewBag.Blocks = blocks;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
