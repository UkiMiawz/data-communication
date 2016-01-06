using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkServerClientProxy : IXmlRpcProxy
{
    [XmlRpcMethod("showNetworkHashMap")]
    NetworkMapStruct[] ShowNetworkHashMap(bool ShowLocalMap = false, int inputLamportClock = 0);

    [XmlRpcMethod("newMachineJoin")]
    void newMachineJoin(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false);

    [XmlRpcMethod("getIpMaster")]
    string getIpMaster(string callerIp, int inputLamportClock = 0);

    [XmlRpcMethod("joinNetwork")]
    void joinNetwork(string ipAddress, int inputLamportClock = 0);

    [XmlRpcMethod("removeMachine")]
    void removeMachine(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false);

    [XmlRpcMethod("addNewMessage")]
    void addNewMessage(string newMessage, int inputLamportClock = 0);

    [XmlRpcMethod("getMessages")]
    string[] getMessages(int inputLamportClock = 0);
    
    [XmlRpcMethod("changeMaster")]
    void changeMaster(string newMasterIp, int inputLamportClock = 0);

    [XmlRpcMethod("getCurrentLamportClock")]
    int getCurrentLamportClock();

    [XmlRpcMethod("changeMaster")]
    void changeMaster(string newMasterIp);

    [XmlRpcMethod("receiveElectionSignal")]
    string receiveElectionSignal(string senderIP);

    [XmlRpcMethod("DoLocalElection")]
    void DoLocalElection();
}