using CookComputing.XmlRpc;
using System.Collections.Generic;

public struct NetworkMapStruct
{
    public int NetworkPriority;
    public string IpAddress;
}

public interface INetworkServer 
{
    [XmlRpcMethod(GlobalMethodName.GetNetworkHashMap)]
    NetworkMapStruct[] GetNetworkHashMap(bool ShowLocalMap = false);

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
    void newMachineJoin(string ipAddress, bool DoItLocally = false);

    [XmlRpcMethod(GlobalMethodName.neighborAskToJoin)]
    string NeighborAskToJoin(string senderIpAddress);

    [XmlRpcMethod(GlobalMethodName.getIpMaster)]
    string getIpMaster(string callerIp);

    [XmlRpcMethod(GlobalMethodName.joinNetwork)]
    void joinNetwork(string ipAddress);

    [XmlRpcMethod(GlobalMethodName.removeMachine)]
    void removeMachine(string ipAddress, bool DoItLocally = false);

    [XmlRpcMethod(GlobalMethodName.addNewMessage)]
    void addNewMessage(string newMessage);

    [XmlRpcMethod(GlobalMethodName.getMessages)]
    string getMessages();
    
    [XmlRpcMethod(GlobalMethodName.changeMaster)]
    void changeMaster(string newMasterIp);

    [XmlRpcMethod(GlobalMethodName.getCurrentLamportClock)]
    int getCurrentLamportClock();

    [XmlRpcMethod(GlobalMethodName.receiveElectionSignal)]
    string receiveElectionSignal(string senderIP);

    [XmlRpcMethod(GlobalMethodName.doLocalElection)]
    void DoLocalElection();

    [XmlRpcMethod(GlobalMethodName.checkMasterStatus)]
    void checkMasterStatus();

    [XmlRpcMethod(GlobalMethodName.MESendRequest)]
    bool SendMERequestToServer(string methodName, string parameter);

    [XmlRpcMethod(GlobalMethodName.MEReceiveReply)]
    void ReceiveMEReply();

    [XmlRpcMethod(GlobalMethodName.MEReceiveRequest)]
    string ReceiveMERequest(string ipAddress);

    [XmlRpcMethod(GlobalMethodName.MEAccessCriticalPart)]
    void AccessCriticalPart(string senderIP, string methodName, string parameter = "");   
}

