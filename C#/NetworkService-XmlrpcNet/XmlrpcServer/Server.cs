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
    public NetworkServer()
    {
        NetworkMap = new Dictionary<int, string>();        
    }

    public Dictionary<int, string> NetworkMap;

    #region Private Method
    private int GetLatestPriority()
    {
        if (NetworkMap.Count == 0)
        {
            return 0;
        }

        List<int> priorityList = new List<int>();
        foreach (int priority in NetworkMap.Keys)
        {
            priorityList.Add(priority);
        }
        return priorityList.Max();
    }

    private NetworkMapStruct[] ConvertDictionaryToStruct(Dictionary<int, string> dict)
    {
        NetworkMapStruct[] convertionResult = new NetworkMapStruct[dict.Count()];
        for (int i = 0; i < dict.Count; i++)
        {
            convertionResult[i] = new NetworkMapStruct();
            convertionResult[i].IpAddress = dict.Values.ElementAt(i);
            convertionResult[i].NetworkPriority = dict.Keys.ElementAt(i);
        }

        return convertionResult;
    }
    #endregion

    #region Public Method
    public NetworkMapStruct[] AddNewNode(string ipAddress)
    {
        NetworkMapStruct[] response;
        if (NetworkMap.Count() > 0 && NetworkMap.ContainsValue(ipAddress))
        {
            response = ConvertDictionaryToStruct(NetworkMap);
            return response;
        }

        int currentPriority = GetLatestPriority() + 1;
        NetworkMap.Add(currentPriority, ipAddress);
        response = ConvertDictionaryToStruct(NetworkMap);        
        return response;
    }

    public string AppendString(string value)
    {
        return "This is Aderick Computer" + value;
    }

    #endregion
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
            "networkServer.rem",
            WellKnownObjectMode.Singleton);
        Console.WriteLine("Press any key to end server");
        Console.ReadLine();
    }
}

