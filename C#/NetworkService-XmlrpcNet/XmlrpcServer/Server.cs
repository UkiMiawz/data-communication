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

public class NetworkServer : MarshalByRefObject, INetworkServer
{
    public string AppendString(string value)
    {
        return value;
    }
}

class _
{
    static void Main (string[] args)
    {
        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 5678;
        HttpChannel channel = new HttpChannel(
            props,
            null,
            new XmlRpcServerFormatterSinkProvider());
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(NetworkServer),
            "calculator.rem",
            WellKnownObjectMode.Singleton);
        Console.WriteLine("Press any key to end server");
        Console.ReadLine();
    }
}

