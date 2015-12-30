using CookComputing.XmlRpc;
using System.Collections.Generic;

public struct NetworkMapStruct
{
    public int NetworkPriority;
    public string IpAddress;
}

public interface INetworkServer
{
    [XmlRpcMethod("server.AppendString")]
    string AppendString(string value);

    [XmlRpcMethod("server.AddNewNode")]
    NetworkMapStruct[] AddNewNode(string ipAddress);
}