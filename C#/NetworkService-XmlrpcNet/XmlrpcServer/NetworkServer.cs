﻿using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class NetworkServer : MarshalByRefObject, INetworkServer
{
    #region Constructor and Properties
    public NetworkServer() // Constructor
    {
        NetworkHashMap = new Dictionary<int, string>();

        // ip Address
        NetworkPingService pingService = new NetworkPingService();
        MyIpAddress = pingService.getMyIpAddress();

        // current master node
        CurrentMasterNode = "";

        // default master node
        DefaultMasterNode = MyIpAddress;

        // default group messages
        GroupMessages = new List<string>();

        // default master proxy
        MasterProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        MasterProxy.Url = ServerUrlStart + DefaultMasterNode + ServerUrlEnd;

        // local proxy
        LocalProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        LocalProxy.Url = ServerUrlStart + MyIpAddress + ServerUrlEnd;

        // Lamport clock
        localLamportClock = new LamportClock();
    }

    ~NetworkServer() // Destructor
    {
        localLamportClock.UpdateLamportClock();
        removeMachine(MyIpAddress, localLamportClock.getCurrentTime());
        if (CurrentMasterNode == MyIpAddress && NetworkHashMap.Count() > 0)
        {
            masterNodeResign();
        }
    }

    public Dictionary<int, string> NetworkHashMap;
    public string CurrentMasterNode;
    public string ServerUrlStart = "http://";
    public string ServerUrlEnd = ":1090/networkServer.rem";
    public string DefaultMasterNode;
    public string MyIpAddress;
    public List<string> GroupMessages;
    public INetworkServerClientProxy MasterProxy;
    public INetworkServerClientProxy LocalProxy;
    public LamportClock localLamportClock;
    #endregion

    #region Private Method
    private void masterNodeResign()
    {
        bool successionComplete = false;
        do
        {
            // Do Election before going down.
        } while (successionComplete == false);
    }

    private void sendToAllSlave(string command, string ipAddress = "")
    {
        if (CurrentMasterNode == MyIpAddress && NetworkHashMap.Count() > 1)
        {
            List<string> mySlaves = NetworkHashMap.Values.ToList();

            try
            {
                INetworkServerClientProxy slaveProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();

                foreach (string mySlaveItem in mySlaves)
                {
                    string slaveServer = ServerUrlStart + mySlaveItem + ServerUrlEnd;
                    slaveProxy.Url = slaveServer;

                    switch (command)
                    {
                        case "addNewMachine":
                            slaveProxy.newMachineJoin(ipAddress, localLamportClock.getCurrentTime(), true);
                            break;

                        case "removeMachine":
                            slaveProxy.removeMachine(ipAddress, localLamportClock.getCurrentTime(), true);
                            break;

                        case "changeMaster":
                            slaveProxy.changeMaster(ipAddress, localLamportClock.getCurrentTime());
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
        if (NetworkHashMap.Count == 0)
        {
            return 0;
        }
        return NetworkHashMap.Keys.ToList().Max();
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
    public void checkMasterStatus()
    {
        ServerStatusCheck ssc = new ServerStatusCheck();
        bool MasterAlive = ssc.isServerUp(CurrentMasterNode, 1090, 300);
        if (MasterAlive == false)
        {
            Console.WriteLine("Master Node has been crashed...Preparing for election !");
            DoLocalElection();
        }
    }

    public void addNewMessage(string newMessage, int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == DefaultMasterNode)
        {
            GroupMessages.Add(newMessage);
        }
        else
        {
            MasterProxy.addNewMessage(newMessage, localLamportClock.getCurrentTime());
        }
    }

    public string[] getMessages(int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == DefaultMasterNode)
        {
            return ConvertListToStruct(GroupMessages);
        }
        else
        {
            return MasterProxy.getMessages(localLamportClock.getCurrentTime());
        }
    }

    public NetworkMapStruct[] ShowNetworkHashMap(bool DoItLocally = false, int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == DefaultMasterNode || DoItLocally == true)
        {
            NetworkMapStruct[] result = ConvertDictionaryToStruct(NetworkHashMap);
            return result;
        }
        else
        {
            return MasterProxy.ShowNetworkHashMap(false, localLamportClock.getCurrentTime());
        }
    }

    public void newMachineJoin(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == DefaultMasterNode || DoItLocally == true)
        {
            if (NetworkHashMap.Count() > 0 && NetworkHashMap.ContainsValue(ipAddress))
            {
                Console.WriteLine("Existing machine {0} try to rejoin the network.");
            }
            else
            {
                int currentPriority = GetLatestPriority() + 1;
                NetworkHashMap.Add(currentPriority, ipAddress);
                Console.WriteLine("New machine {0} with priority {1} joined the network!", ipAddress, currentPriority);
                Console.WriteLine("Total machine in the network now is {0}", NetworkHashMap.Count());
                sendToAllSlave("addNewMachine", ipAddress);
            }
        }
        else
        {
            MasterProxy.ShowNetworkHashMap(false, localLamportClock.getCurrentTime());
        }
    }

    public void removeMachine(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == MyIpAddress || DoItLocally)
        {
            KeyValuePair<int, string> removedIp = NetworkHashMap.FirstOrDefault(x => x.Value == ipAddress);
            NetworkHashMap.Remove(removedIp.Key);
            Console.WriteLine("Machine {0} with priority {1} is removed from the network.", removedIp.Value, removedIp.Key);
            Console.WriteLine("Total machine in the network now is {0}", NetworkHashMap.Count());
            sendToAllSlave("removeMachine", ipAddress);
        }
        else
        {
            MasterProxy.removeMachine(ipAddress, localLamportClock.getCurrentTime());
        }
    }

    public string getIpMaster(string callerIp, int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        if (CurrentMasterNode == string.Empty)
            return DefaultMasterNode;
        else
        {
            return CurrentMasterNode;
        }
    }

    public void joinNetwork(string ipAddress, int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

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
                        string neighborServer = ServerUrlStart + neighborIp + ServerUrlEnd;
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

        // step 3: After the master node found, add new machine to the hashmap. then update local hashmap.
        try
        {
            string masterServer = ServerUrlStart + CurrentMasterNode + ServerUrlEnd;
            MasterProxy.Url = masterServer;

            MasterProxy.newMachineJoin(ipAddress);

            NetworkMapStruct[] newHashmap = MasterProxy.ShowNetworkHashMap(false, localLamportClock.getCurrentTime());
            NetworkHashMap = ConvertStructToDictionary(newHashmap);
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occured. {0}", ex.Message);
        }
    } 
    
    public void changeMaster(string newMasterIp, int inputLamportClock = 0)
    {
        localLamportClock.UpdateLamportClock(inputLamportClock);

        try
        {
            //change Masternode
            MasterProxy.Url = ServerUrlStart + newMasterIp + ServerUrlEnd;
            CurrentMasterNode = newMasterIp;
            Console.WriteLine("New master Node is : {0}" + newMasterIp);
            
            //update Hash map from Master
            NetworkMapStruct[] updatedMap = MasterProxy.ShowNetworkHashMap(false, localLamportClock.getCurrentTime());
            Console.WriteLine("local hashmap has been updated from new master node.");
        }
        catch (Exception ex)
        {

        }       
    }

    public int getCurrentLamportClock()
    {
        return localLamportClock.getCurrentTime();
    }           

    public string receiveElectionSignal(string senderIP)
    {
        IAsyncResult asr;
        asr = LocalProxy.BeginDoLocalElection(null,null);
        while(asr.IsCompleted == false)
        {
            return "Ok";
        }
        try
        {
            LocalProxy.EndDoLocalElection(asr);
        }
        catch(Exception ex)
        {
            
        }

        return "OK";
    }

    void DoLocalElectionCallBack(IAsyncResult asr)
    {
        XmlRpcAsyncResult clientResult = (XmlRpcAsyncResult)asr;
        INetworkServer proxy = (INetworkServer)clientResult.ClientProtocol;
        try
        {
            LocalProxy.EndDoLocalElection(asr);
        }
        catch(Exception ex)
        {

        }
    }

    public void DoLocalElection()
    {
        Bully bullyObj = new Bully();
        string newMaster = bullyObj.doElection(MyIpAddress, NetworkHashMap);

        if (newMaster != string.Empty)
        {
            // 1. tell other notes that I'm the new master, update my hashmap
            changeMaster(newMaster);
            // 2. tell other notes to update their hashmap
            sendToAllSlave("changeMaster", newMaster);
        }

    }

    #endregion
}



