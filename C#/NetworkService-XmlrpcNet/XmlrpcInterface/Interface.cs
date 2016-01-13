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
    NetworkMapStruct[] ShowNetworkHashMap(bool ShowLocalMap = false);

    [XmlRpcMethod("newMachineJoin")]
    void newMachineJoin(string ipAddress, bool DoItLocally = false);

    [XmlRpcMethod("neighborAskToJoin")]
    string NeighborAskToJoin(string senderIpAddress);

    [XmlRpcMethod("getIpMaster")]
    string getIpMaster(string callerIp);

    [XmlRpcMethod("joinNetwork")]
    void joinNetwork(string ipAddress);

    [XmlRpcMethod("removeMachine")]
    void removeMachine(string ipAddress, bool DoItLocally = false);

    [XmlRpcMethod("addNewMessage")]
    void addNewMessage(string newMessage);

    [XmlRpcMethod("getMessages")]
    string[] getMessages();
    
    [XmlRpcMethod("changeMaster")]
    void changeMaster(string newMasterIp);

    [XmlRpcMethod("getCurrentLamportClock")]
    int getCurrentLamportClock();

    [XmlRpcMethod("receiveElectionSignal")]
    string receiveElectionSignal(string senderIP);

    [XmlRpcMethod("DoLocalElection")]
    void DoLocalElection();

    [XmlRpcMethod("checkMasterStatus")]
    void checkMasterStatus();

    [XmlRpcMethod("MutualExclusion.SendMERequest")]
    bool SendMERequestToServer(string methodName, string parameter);

    [XmlRpcMethod("MutualExclusion.ReceiveMEReply")]
    void ReceiveMEReply();

    [XmlRpcMethod("MutualExclusion.ReceiveMERequest")]
    string ReceiveMERequest(string ipAddress);

    [XmlRpcMethod("MutualExclusion.AccessCriticalPart")]
    void AccessCriticalPart(string senderIP, string methodName, string parameter = "");   
}

