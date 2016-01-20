using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

public interface ICSharpRpcClient : IXmlRpcProxy
{
    [XmlRpcMethod("HelloWorld.hello")]
    string HelloWorld(string ipAddress);
}

class CSharpRpcClient
{
    static void Main(string[] args)
    {
        HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        string defaultServer = "http://localhost:1090/xml-rpc-example/xmlrpc";

        ICSharpRpcClient client = XmlRpcProxyGen.Create<ICSharpRpcClient>();
        client.Url = defaultServer;

        string dummy = client.HelloWorld("Aderick");
        Console.WriteLine(dummy);
        Console.ReadKey();
    }
}
