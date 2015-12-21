using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.Samples.XmlRpc;

namespace WcfServiceLibrary1
{
    [ServiceContract]
    public interface IServiceContract
    {
        [OperationContract(Action = "Hello")]
        string Hello(string name);
    }


    class Program
    {
        static void Main(string[] args)
        {
            ChannelFactory<IServiceContract> cf = new ChannelFactory<IServiceContract>(
                new WebHttpBinding(), "http://www.example.com/xmlrpc");

            cf.Endpoint.EndpointBehaviors.Add(new XmlRpcEndpointBehavior());

            IServiceContract client = cf.CreateChannel();

            string answer = client.Hello("world");

        }

    }
}
