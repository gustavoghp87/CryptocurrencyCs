using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace BlockchainAPI.Services
{
    public class CentralServerConnection
    {
        public Uri GetUri()
        {
            return new Uri("http://localhost:10000/nodes");
        }
        public bool Connect(string nodeId)
        {
            try
            {
                var url = new Uri("http://localhost:10000/nodes");

                using (var client = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["nodeId"] = nodeId;

                    var response = client.UploadValues(url, "POST", data);
                    string responseInString = Encoding.UTF8.GetString(response);
                    Console.WriteLine(responseInString);
                }
                return true;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return false;
            }
        }
    }
}
