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
        DefaultMasterNode = myIpAddress;
    }

    public Dictionary<int, string> NetworkMap;
    public string CurrentMasterNode;
    public string defMstrNode1 = "http://";
    public string defMstrNode2 = ":1090/networkServer.rem";
    public string DefaultMasterNode;
    public string myIpAddress;

    #endregion

    #region Private Method
    private void sendToAllSlave(string command, string ipAddress = "")
    {
        if (CurrentMasterNode == myIpAddress)
        {
            List<string> mySlaves = new List<string>();
            foreach (KeyValuePair<int, string> nodeItem in NetworkMap)
            {
                if (nodeItem.Value != myIpAddress) mySlaves.Add(nodeItem.Value);
            }

            try
            {
                // ==== Erase this after the system stabilized ===
                //HttpChannel slaveChnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
                //ChannelServices.RegisterChannel(slaveChnl, false);
                string defaultServer = defMstrNode1 + DefaultMasterNode + defMstrNode2;
                INetworkServer slaveNetServer = (INetworkServer)Activator.GetObject(
                        typeof(INetworkServer), defaultServer);

                foreach (string mySlaveItem in mySlaves)
                {
                    string slaveServer = defMstrNode1 + mySlaveItem + defMstrNode2;
                    slaveNetServer = (INetworkServer)Activator.GetObject(
                         typeof(INetworkServer), slaveServer);

                    switch (command)
                    {
                        case "addNewMachine":
                            slaveNetServer.newMachineJoin(ipAddress);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                
            }            
        }
    }

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

    private Dictionary<int, string> ConvertStructToDictionary(NetworkMapStruct[] netStruct)
    {
        Dictionary<int, string> convertionResult = new Dictionary<int, string>();
        foreach (NetworkMapStruct netStructItem in netStruct)
        {
            convertionResult.Add(netStructItem.NetworkPriority, netStructItem.IpAddress);
        }

        return convertionResult;
    }
    #endregion

    #region Public Method
    public NetworkMapStruct[] ShowNetworkHashMap()
    {
        NetworkMapStruct[] result = ConvertDictionaryToStruct(NetworkMap);
        return result;
    }

    public void newMachineJoin(string ipAddress)
    {
        if (NetworkMap.Count() > 0 && NetworkMap.ContainsValue(ipAddress))
        {
            Console.WriteLine("Existing machine {0} try to rejoin the network.");
        }
        else
        {
            int currentPriority = GetLatestPriority() + 1;
            NetworkMap.Add(currentPriority, ipAddress);
            Console.WriteLine("New machine {0} with priority {1} joined the network!", ipAddress, currentPriority);
            Console.WriteLine("Total machine in the network now is {0}", NetworkMap.Count());
            sendToAllSlave("addNewMachine", ipAddress);
        }
    }

    public void removeMachine(string ipAddress)
    {
        KeyValuePair<int, string> removedIp = NetworkMap.FirstOrDefault(x => x.Value == ipAddress);
        NetworkMap.Remove(removedIp.Key);
        Console.WriteLine("Machine {0} with priority {1} is removed from the network.", removedIp.Value, removedIp.Key);
        Console.WriteLine("Total machine in the network now is {0}", NetworkMap.Count());
    }
    public string getIpMaster(string callerIp)
    {
        if (CurrentMasterNode == string.Empty)
            return DefaultMasterNode;
        else
        {
            Console.WriteLine("ip {0} is asking for master node. The current master node is {1}", callerIp, CurrentMasterNode);
            return CurrentMasterNode;
        }
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
                    // ==== Erase this after the system stabilized ===
                    //HttpChannel neighborChnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
                    //ChannelServices.RegisterChannel(neighborChnl, false);
                    string neighborServer = defMstrNode1 + neighborIp + defMstrNode2;
                    INetworkServer neighborNetServer = (INetworkServer)Activator.GetObject(
                         typeof(INetworkServer), neighborServer);
                    CurrentMasterNode = neighborNetServer.getIpMaster(ipAddress);

                    if (CurrentMasterNode != "")
                        break;
                }
                catch (Exception ex)
                {
                    // Basically, do nothing.
                }
            }

            if (CurrentMasterNode == "")
                CurrentMasterNode = DefaultMasterNode;
        }

        // step 3: After the master node found, add new machine to the hashmap.
        try
        {
            // ==== Erase this after the system stabilized ===
            //HttpChannel masterChnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
            //ChannelServices.RegisterChannel(masterChnl, false);

            string masterServer = defMstrNode1 + CurrentMasterNode + defMstrNode2;
            INetworkServer currentMasterNetServer = (INetworkServer)Activator.GetObject(
                typeof(INetworkServer), masterServer);
            currentMasterNetServer.newMachineJoin(ipAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occured. {0}", ex.Message);
        }

    }

    public void updateLocalHashmapFromMasterNode(NetworkMapStruct[] masterHashmap)
    {
        NetworkMap = ConvertStructToDictionary(masterHashmap);
        Console.WriteLine("local hashmap updated");
    }

    #endregion
}