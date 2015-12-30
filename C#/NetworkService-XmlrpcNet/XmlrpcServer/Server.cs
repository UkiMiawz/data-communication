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
    #region Constructor and Properties
    public NetworkServer()
    {
        NetworkMap = new Dictionary<int, string>();

        // ip Address
        NetworkPingService pingService = new NetworkPingService();
        myIpAddress = pingService.getMyIpAddress();

        // current master node
        CurrentMasterNode = "";

        // default master node
        DefaultMasterNode = "localhost";
    }

    public Dictionary<int, string> NetworkMap;
    public string CurrentMasterNode;
    public string defMstrNode1 = "http://";
    public string defMstrNode2 = ":1090/networkServer.rem";
    public string DefaultMasterNode;
    public string myIpAddress;

    #endregion

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

    public string getIpMaster(string callerIp)
    {
        if (CurrentMasterNode == string.Empty)
            return DefaultMasterNode;
        else
            return CurrentMasterNode;
    }

    public void joinNetwork(string ipAddress)
    {
        // step 1: ping all neighbor
        NetworkPingService nps = new NetworkPingService();
        nps.DetectAllNetwork(ipAddress);
        List<string> activeIps = nps.getActiveIps();

        // step 2: ask for MasterNode to each neighbor
        if (activeIps.Count() == 1 && activeIps.Contains(ipAddress))
        {
            CurrentMasterNode = DefaultMasterNode;
        }
        else
        {
            activeIps.Remove(ipAddress);

            foreach (string neighborIp in activeIps)
            {
                try
                {
                    string neighborServer = defMstrNode1 + neighborIp + defMstrNode2;
                    HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
                    ChannelServices.RegisterChannel(chnl, false);
                    INetworkServer netwS = (INetworkServer)Activator.GetObject(
                         typeof(INetworkServer), neighborServer);
                    CurrentMasterNode = netwS.getIpMaster(ipAddress);
                }
                catch (Exception ex)
                {
                    // Basically, do nothing.
                }
            }

            if (CurrentMasterNode == "")
                CurrentMasterNode = DefaultMasterNode;

            Console.WriteLine("The current master node is {0}", CurrentMasterNode);
        }

    }

    #endregion
}

class _
{
    static void Main(string[] args)
    {
        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 1090;
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

