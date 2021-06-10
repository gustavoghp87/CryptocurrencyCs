using System;
using System.Linq;

namespace BlockchainAPI.Services
{
    public class HashService
    {
        string hash1 = "000zd651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash2 = "000ad651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash3 = "0000zd65as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash4 = "011ad651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash5 = "011Ad651As1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash6 = "011ad651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash7 = "000ad651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash8 = "00000651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash9 = "00gad651as1asdASDADas65d1asd51sds1fsfd1sdf156dsf";
        string hash10 = "011ad651as1asdASDADaS65d1asd51sds1fsfd1sdf156dsf";

        private void Order()
        {
            string[] arrey = new string[] { hash1, hash2, hash3, hash4, hash5, hash6, hash7, hash8, hash9, hash10 };
            string[] ordered = (from x in arrey orderby x select x).ToArray();
            foreach (var item in ordered)
            {
                Console.WriteLine("{0} element",  item);
            }
        }
        public static string GetHigher(string hash1, string hash2)
        {
            string[] hashes = new string[] { hash1, hash2 };
            string[] ordered = (from x in hashes orderby x select x).ToArray();
            return ordered.ElementAt(0);
        }
    }
}
