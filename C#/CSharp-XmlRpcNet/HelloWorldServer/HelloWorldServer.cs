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

class HelloWorldServer: MarshalByRefObject
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
            typeof(HelloWorldServer),
            "xml-rpc-example/xmlrpc",
            WellKnownObjectMode.Singleton);
    }

    public static bool haveInterest;

    public String hello(String ipAddress)
    {
        Console.WriteLine("Hello from IP : " + ipAddress);
        return "Greetings IP " + ipAddress;
    }

    public String helloServer(String ipAddress, String command, Object[] parameters)
    {
        throw new NotImplementedException();
        //String response = JavaWsServer.testConnection(ipAddress, command, params);
        //Console.WriteLine("Response from Server : " + response);
        //return response;
    }

    public XmlRpcStruct[] returnKeyMap(String ipAddress, int priority)
    {
        Console.WriteLine("Hashmap request from ip " + ipAddress + " with priority " + priority);
        Dictionary<int, String> machines = new Dictionary<int, String>();
        machines.Add(1, "test1");
        machines.Add(3, "test3");
        Console.WriteLine("Machines number now " + machines.Count());
        XmlRpcStruct[] test = ConvertDictionaryToStruct(machines);
        return test;
    }

    public XmlRpcStruct[] returnKeyMap2()
    {
        Console.WriteLine("Hashmap request");
        Dictionary<int, String> machines = new Dictionary<int, String>();
        machines.Add(1, "test1");
        machines.Add(3, "test3");
        Console.WriteLine("Machines number now " + machines.Count());
        XmlRpcStruct[] test = ConvertDictionaryToStruct(machines);
        return test;
    }

    private XmlRpcStruct[] ConvertDictionaryToStruct(Dictionary<int, string> dict)
    {
        XmlRpcStruct[] convertionResult = new XmlRpcStruct[dict.Count()];
        for (int i = 0; i < dict.Count; i++)
        {
            convertionResult[i] = new XmlRpcStruct();
            convertionResult[i].Add("NetworkPriority", dict.Keys.ElementAt(i));
            convertionResult[i].Add("IpAddress", dict.Values.ElementAt(i));
        }

        return convertionResult;
    }
}

