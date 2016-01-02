using CookComputing.XmlRpc;
using System.Collections.Generic;

public struct NetworkMapStruct
{
    public int NetworkPriority;
    public string IpAddress;
}

public interface INetworkServer
{
    [XmlRpcMethod("showNetworkHashMap")]
    NetworkMapStruct[] ShowNetworkHashMap();

    [XmlRpcMethod("newMachineJoin")]
    void newMachineJoin(string ipAddress);

    [XmlRpcMethod("getIpMaster")]
    string getIpMaster(string callerIp);

    [XmlRpcMethod("joinNetwork")]
    void joinNetwork(string ipAddress);

    [XmlRpcMethod("removeMachine")]
    void removeMachine(string ipAddress);

    [XmlRpcMethod("updateHashmapFromMaster")]
    void updateLocalHashmapFromMasterNode(NetworkMapStruct[] masterHashmap);
}

