using CookComputing.XmlRpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

class CSharpRpcServer : MarshalByRefObject
{
    public static void Main(string[] args)
    {
        string input;

        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 1090;

        HttpChannel channel = new HttpChannel(
            props,
            new XmlRpcClientFormatterSinkProvider(),
            new XmlRpcServerFormatterSinkProvider());
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(CSharpRpcServer),
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

    [XmlRpcMethod("HelloWorld.hello")]
    public string Hello(string input)
    {
        return "Hello " + input;
    }
}