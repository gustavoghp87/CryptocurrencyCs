using BlockchainAPI.Models;
using BlockchainAPI.Services.Nodes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BlockchainAPI.Services.Blockchains
{
    public class GetBlockchainsFromNetService
    {
        private Blockchain _blockchain;
        private List<Node> _lstNodes;
        public GetBlockchainsFromNetService()
        {
            _blockchain = new();
            _lstNodes = new NodeService().GetAll();
            if (_lstNodes != null && _lstNodes.Count != 0) GetFromNet();
        }
        private void GetFromNet()
        {
            List<Blockchain> lstBlockchains = RequestFromNet();
            if (lstBlockchains == null || lstBlockchains.Count == 0) return;
            GetLargest(lstBlockchains);
        }
        private List<Blockchain> RequestFromNet()
        {
            List<Blockchain> lstBlockchains = new();
            foreach (Node node in _lstNodes)
            {
                try
                {
                    var url = new Uri(node.Address + "chain");
                    Console.WriteLine(url.ToString());
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    var response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var model = new
                        {
                            blockchain = new Blockchain(),
                            length = 0
                        };
                        string json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                        var data = JsonConvert.DeserializeAnonymousType(json, model);
                        lstBlockchains.Add(data.blockchain);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return lstBlockchains;
        }
        private void GetLargest(List<Blockchain> lstBlockchains)
        {
            Blockchain newBlockchain = new();
            foreach (Blockchain blockchain in lstBlockchains)
            {
                if (blockchain.Blocks.Count > newBlockchain.Blocks.Count && ValidateBlockchainService.IsValid(blockchain))
                    newBlockchain = blockchain;
            }
            if (newBlockchain != null) _blockchain = newBlockchain;
        }
        public Blockchain GetLargest()
        {
            return _blockchain;
        }
    }
}
