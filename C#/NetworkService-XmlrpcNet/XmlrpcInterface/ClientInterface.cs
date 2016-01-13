using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkServerClientProxy : IXmlRpcProxy
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

    [XmlRpcMethod("checkMasterStatus")]
    void checkMasterStatus();

    [XmlRpcMethod("DoLocalElection")]
    void DoLocalElection();

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection();

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection(AsyncCallback acb);

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection(AsyncCallback acb, object state);

    [XmlRpcEnd("DoLocalElection")]
    void EndDoLocalElection(IAsyncResult iars);

    [XmlRpcMethod("MutualExclusion.SendMERequest")]
    bool SendMERequestToServer(string methodName, string parameter);

    [XmlRpcBegin("MutualExclusion.ReceiveMEReply")]
    IAsyncResult BeginReceiveMEReply();

    [XmlRpcMethod("MutualExclusion.ReceiveMEReply")]
    void ReceiveMEReply();

    [XmlRpcEnd("MutualExclusion.ReceiveMEReply")]
    void EndReceiveMEReply(IAsyncResult iars);

    [XmlRpcMethod("MutualExclusion.ReceiveMERequest")]
    string ReceiveMERequest(string ipAddress);

    [XmlRpcMethod("MutualExclusion.AccessCriticalPart")]
    void AccessCriticalPart(string senderIP, string methodName, string parameter = "");
}