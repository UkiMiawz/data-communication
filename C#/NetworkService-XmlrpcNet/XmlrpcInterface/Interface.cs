using CookComputing.XmlRpc;

public interface INetworkServer
{
    [XmlRpcMethod("server.AppendString")]
    string AppendString(string value);
}