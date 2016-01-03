using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkServerClientProxy : IXmlRpcProxy
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

    [XmlRpcMethod("addNewMessage")]
    void addNewMessage(string newMessage);

    [XmlRpcMethod("getMessages")]
    string[] getMessages();

    [XmlRpcMethod("announceElectionHeld")]
    void announceElectionHeld();

    [XmlRpcMethod("doElection")]
    void doElection();
}