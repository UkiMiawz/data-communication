using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkServerClientProxy : IXmlRpcProxy
{
    [XmlRpcMethod(GlobalMethodName.GetNetworkHashMap)]
    XmlRpcStruct[] GetNetworkHashMap(bool ShowLocalMap = false);

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

    [XmlRpcMethod(GlobalMethodName.checkMasterStatus)]
    void checkMasterStatus();

    [XmlRpcMethod(GlobalMethodName.doLocalElection)]
    void DoLocalElection();

    [XmlRpcBegin(GlobalMethodName.doLocalElection)]
    IAsyncResult BeginDoLocalElection();

    [XmlRpcEnd(GlobalMethodName.doLocalElection)]
    void EndDoLocalElection(IAsyncResult iars);

    [XmlRpcMethod(GlobalMethodName.MESendRequest)]
    bool SendMERequestToServer(string methodName, string parameter);

    [XmlRpcBegin(GlobalMethodName.MEReceiveReply)]
    IAsyncResult BeginReceiveMEReply();

    [XmlRpcMethod(GlobalMethodName.MEReceiveReply)]
    void ReceiveMEReply();

    [XmlRpcEnd(GlobalMethodName.MEReceiveReply)]
    void EndReceiveMEReply(IAsyncResult iars);

    [XmlRpcMethod(GlobalMethodName.MEReceiveRequest)]
    string ReceiveMERequest(string ipAddress);

    [XmlRpcMethod(GlobalMethodName.MEAccessCriticalPart)]
    void AccessCriticalPart(string senderIP, string methodName, string parameter = "");
}