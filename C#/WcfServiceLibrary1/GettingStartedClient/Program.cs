using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using GettingStartedClient.ServiceReference1;

namespace GettingStartedClient
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceContractClient client = new ServiceContractClient();

            string value1 = "Aderick";
            string result = client.Hello(value1);
            Console.WriteLine("{0}", result);

            Console.ReadLine();

            client.Close();
        }
    }
}
