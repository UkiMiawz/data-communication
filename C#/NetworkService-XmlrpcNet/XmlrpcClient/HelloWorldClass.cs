using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;



public interface IHelloWorld : IXmlRpcProxy
{
    [XmlRpcMethod("HelloWorld.hello")]
    string HelloWorld(string ipAddress);

    [XmlRpcMethod("HelloWorld.returnKeyMap")]
    XmlRpcStruct[] getMap(string ipAddress, int priority);     
}

class HelloWorldConsoleClient
{
    static void Main(string[] args)
    {
        ClientObject co = new ClientObject();
        string ipAddress = co.GetClientIpAdress();

        HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        string defaultServer = "http://localhost:1090/xml-rpc-example/xmlrpchello";
        string javaServer = "http://172.16.1.100:1090/xml-rpc-example/xmlrpc";

        IHelloWorld localProxy = XmlRpcProxyGen.Create<IHelloWorld>();
        localProxy.Url = javaServer;

        Console.WriteLine("{0}", localProxy.HelloWorld(ipAddress));

        object myObject = localProxy.getMap(ipAddress, 1);

        //foreach (KeyValuePair<int, string> mapDetail in newHashmap)
        //{
        //    Console.WriteLine("priority {0} : {1}", mapDetail.Key, mapDetail.Value);
        //}
        Console.ReadKey();
    }
}