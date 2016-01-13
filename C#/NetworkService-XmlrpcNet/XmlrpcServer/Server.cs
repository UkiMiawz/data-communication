using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;

class XmlrpcServer
{
    static void Main(string[] args)
    {
        string input;

        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 1090;
        //RemotingConfiguration.Configure("networkServer.exe.config", false);

        HttpChannel channel = new HttpChannel(
            props,
            new XmlRpcClientFormatterSinkProvider(),
            new XmlRpcServerFormatterSinkProvider());
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(HelloWorldService),
            "xml-rpc-example/xmlrpchello",
            WellKnownObjectMode.Singleton);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(NetworkServer),
            "xml-rpc-example/xmlrpc",
            WellKnownObjectMode.Singleton);

        Console.WriteLine("=====Xmlrpc-Server=====");
        Console.WriteLine("type 'exit' to end server");
        do
        {
            input = Console.ReadLine().ToLower();
        }
        while (input != "exit");
    }
}

