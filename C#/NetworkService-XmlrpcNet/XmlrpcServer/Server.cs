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
        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 1090;
        HttpChannel channel = new HttpChannel(
            props,
            new XmlRpcClientFormatterSinkProvider(),
            new XmlRpcServerFormatterSinkProvider());
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(NetworkServer),
            "networkServer.rem",
            WellKnownObjectMode.Singleton);
        Console.WriteLine("=====Xmlrpc-Server=====");
        Console.WriteLine("Press any key to end server");
        Console.ReadLine();
    }
}

