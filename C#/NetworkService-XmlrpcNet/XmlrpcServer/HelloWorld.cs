using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class HelloWorldService : MarshalByRefObject
{
    [XmlRpcMethod("HelloWorld.hello")]
    public string HelloWorld(string ipAddress)
    {
        Console.WriteLine("{0} ask for service", ipAddress);       
        return "Hello " + ipAddress;
    }

    [XmlRpcMethod("HelloWorld.returnKeyMap")]
    public XmlRpcStruct[] getMap(string ipAddress, int priority)
    {
        Console.WriteLine("Receive request from {0} with priority {1}", ipAddress, priority);
        Dictionary<int, string> result = new Dictionary<int, string>();
        result.Add(1, "192.16.1.14");
        result.Add(3, "192.16.1.13");
        result.Add(6, "192.16.1.15");
        result.Add(7, "192.16.1.17");

        return ConvertDictionaryToStruct(result);
    }

    private XmlRpcStruct[] ConvertDictionaryToStruct(Dictionary<int, string> dict)
    {
        XmlRpcStruct[] convertionResult = new XmlRpcStruct[dict.Count()];
        for (int i = 0; i < dict.Count; i++)
        {
            convertionResult[i] = new XmlRpcStruct();
            convertionResult[i].Add("networkPriority", dict.Keys.ElementAt(i));
            convertionResult[i].Add("ipAddress", dict.Values.ElementAt(i));
        }

        return convertionResult;
    }
}
