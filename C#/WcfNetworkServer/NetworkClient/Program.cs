using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Samples.XmlRpc;
using System.ServiceModel;
using WcfNetworkServer;
using System.ServiceModel.Description;
using CookComputing.XmlRpc;

namespace NetworkClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //Uri localServer = new Uri("http://localhost:8733/DCIT/WcfNetworkServer/WcfService/");

            //ChannelFactory<IWcfService> cf = new ChannelFactory<IWcfService>(
            //    new WebHttpBinding(WebHttpSecurityMode.None)  
            //    ,new EndpointAddress(localServer));

            ////cf.Endpoint.Behaviors.Add(new XmlRpcEndpointBehavior());

            //cf.Endpoint.EndpointBehaviors.Add(new XmlRpcEndpointBehavior());
            ////cf.Endpoint.Address = new EndpointAddress(localServer);

            //try
            //{
            //    IWcfService client = cf.CreateChannel();
            //    string answer = client.AppendString("My string");
            //    Console.WriteLine("{0}", answer);
            //    Console.ReadLine();
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine("{0}", ex.Message);
            //    Console.ReadLine();
            //    cf.Close();
            //}

            try
            {
                ICalculatorMethod proxy = XmlRpcProxyGen.Create<ICalculatorMethod>();
                int result = proxy.Add(13, 8);
                Console.WriteLine("{0}", result);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
                Console.ReadLine();
            }

        }
    }

    [XmlRpcUrl("http://www.cookcomputing.com/xmlrpcsamples/math.rem")]
    public interface ICalculatorMethod : IXmlRpcProxy
    {
        [XmlRpcMethod("math.Add")]
        int Add(int a, int b);
    }    
}
