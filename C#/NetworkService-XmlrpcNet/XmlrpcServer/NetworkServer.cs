using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // default group messages;
        GroupMessages = new List<string>();

    }

    public Dictionary<int, string> NetworkMap;
    public string CurrentMasterNode;
    public string defMstrNode1 = "http://";
    public string defMstrNode2 = ":1090/networkServer.rem";
    public string DefaultMasterNode;
    public string myIpAddress;
    public List<string> GroupMessages;

    #endregion

    #region Private Method
    private void sendToAllSlave(string command, string ipAddress = "")
    {
        if (CurrentMasterNode == myIpAddress && NetworkMap.Count() > 1)
        {
            List<string> mySlaves = new List<string>();
            foreach (KeyValuePair<int, string> nodeItem in NetworkMap)
            {
                if (nodeItem.Value != myIpAddress) mySlaves.Add(nodeItem.Value);
            }

            try
            {
                string defaultServer = defMstrNode1 + DefaultMasterNode + defMstrNode2;
                INetworkServerClientProxy slaveProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();

                foreach (string mySlaveItem in mySlaves)
                {
                    string slaveServer = defMstrNode1 + mySlaveItem + defMstrNode2;
                    slaveProxy.Url = slaveServer;

                    switch (command)
                    {
                        case "addNewMachine":
                            slaveProxy.newMachineJoin(ipAddress);
                            break;

                        case "removeMachine":
                            slaveProxy.removeMachine(ipAddress);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
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

    private string[] ConvertListToStruct(List<string> input)
    {
        string[] result = new string[input.Count];
        for (int i = 0; i < input.Count(); i++)
        {
            result[i] = input[i];
        }
        return result;
    }    
    #endregion

    #region Public Method
    public void addNewMessage(string newMessage)
    {
        GroupMessages.Add(newMessage);
    }

    public string[] getMessages()
    {
        return ConvertListToStruct(GroupMessages);
    }

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
        sendToAllSlave("removeMachine", ipAddress);
    }

    public string getIpMaster(string callerIp)
    {
        if (CurrentMasterNode == string.Empty)
            return DefaultMasterNode;
        else
        {
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
                    ServerStatusCheck ssc = new ServerStatusCheck();
                    bool serverUp = ssc.isServerUp(neighborIp, 1090, 300);

                    if (serverUp)
                    {
                        string neighborServer = defMstrNode1 + neighborIp + defMstrNode2;
                        INetworkServerClientProxy neighborProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
                        neighborProxy.Url = neighborServer;

                        CurrentMasterNode = neighborProxy.getIpMaster(ipAddress);

                        if (CurrentMasterNode != "")
                            break;
                    }
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
            string masterServer = defMstrNode1 + CurrentMasterNode + defMstrNode2;
            INetworkServerClientProxy currentMasterProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
            currentMasterProxy.Url = masterServer;
            currentMasterProxy.newMachineJoin(ipAddress);
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

    public void doElection()
    {
        Bully localBully = new Bully();
        Console.WriteLine("!!!!!!!!!!!!! Election started !!!!!!!!!!!!!!");

        // 1. Update my local hashmap
        NetworkMap = localBully.getActiveMap(NetworkMap);
        Console.WriteLine("Local Hashmap updated.");

        // 2. Held an election
        string newMaster = localBully.electMaster(NetworkMap);
        CurrentMasterNode = newMaster;
        Console.WriteLine("New master of {0} elected!", CurrentMasterNode);

        // 3. Update local hashmap from master node
        try
        {
            string newMasterServer = defMstrNode1 + CurrentMasterNode + defMstrNode2;
            INetworkServerClientProxy neighborProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
            neighborProxy.Url = newMasterServer;
            NetworkMapStruct[] updatedMap = neighborProxy.ShowNetworkHashMap();
            updateLocalHashmapFromMasterNode(updatedMap);
            Console.WriteLine("local hashmap has been updated from new master node.");
        }
        catch (Exception ex)
        {
            // Let it go... Let it go... can't hold it back anymore....
        }

        Console.WriteLine("!!!!!!!!!!!!!!!!!! Election ended !!!!!!!!!!!!!!!!!!!!");
    }

    public void announceElectionHeld()
    {
        Bully mybully = new Bully();
        NetworkMap = mybully.getActiveMap(NetworkMap);
        foreach (string neighborAddress in NetworkMap.Values)
        {
            try
            {
                Console.WriteLine("Doing election in {0}.", neighborAddress);
                string defaultServer = defMstrNode1 + neighborAddress + defMstrNode2;
                INetworkServerClientProxy neighborProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
                neighborProxy.Url = defaultServer;
                neighborProxy.doElection();
                Console.WriteLine("Election in {0} done!", neighborAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Election in {0} could not be done. Move to another neighbor.", neighborAddress);
            }
        }
    }

    #endregion
}