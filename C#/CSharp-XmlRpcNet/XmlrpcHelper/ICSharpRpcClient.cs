using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICSharpRpcClient : IXmlRpcProxy
{
    [XmlRpcMethod("Hello")]
    string HelloWorld(String input);

    [XmlRpcMethod(GlobalMethodName.serverShutDownFromClient)]
    void serverShutDownFromClient();

    [XmlRpcMethod(GlobalMethodName.resourceGetString)]
    String resourceGetString(String ipAddress);

    #region Register Handler
    [XmlRpcMethod(GlobalMethodName.getMachines)]
    XmlRpcStruct getMachines(string callerIp);

    [XmlRpcMethod(GlobalMethodName.getIpMaster)]
    string getIpMaster(String ipAddress);

    [XmlRpcMethod(GlobalMethodName.removeMachineIp)]
    string removeMachineIp(String ipAddress);

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
    String newMachineJoin(String ipAddress);

    [XmlRpcMethod(GlobalMethodName.newMachineJoinNotification)]
    String newMachineJoinNotification(String newIp, String callerIp);

    [XmlRpcMethod(GlobalMethodName.addNewMachine)]
    String addNewMachine(String newIpAddress, String callerIpAddress);

    [XmlRpcMethod(GlobalMethodName.getKeyMaster)]
    int getKeyMaster(String callerIp);

    #endregion

    #region Election
    [XmlRpcMethod(GlobalMethodName.leaderElection)]
    string leaderElection(String ipAddress);

    [XmlRpcMethod(GlobalMethodName.checkLeaderValidity)]
    bool checkLeaderValidity(String callerIp);

    [XmlRpcMethod(GlobalMethodName.setNewLeader)]
    String setNewLeader(int keyMaster);

    #endregion
    
    #region Request Handler
    [XmlRpcMethod(GlobalMethodName.requestHandlerStartMessage)]
    string requestStartMessage(bool wantWrite, bool isSignal);

    [XmlRpcMethod(GlobalMethodName.requestHandlerReceivePermission)]
    String requestReceivePermission(int requestClock, int machineKey, String ipAddress);

    [XmlRpcMethod(GlobalMethodName.requestHandlerRequestPermission)]
    String requestRequestPermission(int requestClock, int machineKey, String ipAddress, String requestString);
    #endregion

    #region RequestCentral Handler
    [XmlRpcMethod(GlobalMethodName.requestCentralStartMessage)]
    String requestCentralStartMessage(bool localWantWrite);

    [XmlRpcMethod(GlobalMethodName.requestCentralReceiveRequest)]
    void requestCentralReceiveRequest(String requestIp, String requestString);

    [XmlRpcMethod(GlobalMethodName.requestCentralGetPermission)]
    void requestCentralGetPermission(String requestIp, String requestString);

    [XmlRpcMethod(GlobalMethodName.requestCentralFinishRequest)]
    void requestCentralFinishRequest(String requestIp);
    #endregion
}


