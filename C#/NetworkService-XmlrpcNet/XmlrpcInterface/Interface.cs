using CookComputing.XmlRpc;
using System.Collections.Generic;

public struct NetworkMapStruct
{
    public int NetworkPriority;
    public string IpAddress;
}

public interface INetworkServer
{
    [XmlRpcMethod("server.AddNewNode")]
    NetworkMapStruct[] AddNewNode(string ipAddress);

    [XmlRpcMethod("getIpMaster")]
    string getIpMaster(string callerIp);

    [XmlRpcMethod("joinNetwork")]
    void joinNetwork(string ipAddress);
}