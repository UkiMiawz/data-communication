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

    [XmlRpcMethod(GlobalMethodName.getMachines)]
    XmlRpcStruct[] getMachines(String callerIp);

    [XmlRpcMethod(GlobalMethodName.serverShutDownFromClient)]
    void serverShutDownFromClient();

    [XmlRpcMethod(GlobalMethodName.leaderElection)]
    string leaderElection(String ipAddress);

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

    [XmlRpcMethod(GlobalMethodName.checkLeaderValidity)]
    bool checkLeaderValidity(String callerIp);

    [XmlRpcMethod(GlobalMethodName.setNewLeader)]
    String setNewLeader(int keyMaster);

    #region Request Handler
    [XmlRpcMethod(GlobalMethodName.requestHandlerStartMessage)]
    String requestStartMessage(Boolean wantWrite, Boolean isSignal);

    [XmlRpcMethod(GlobalMethodName.requestHandlerReceivePermission)]
    String requestReceivePermission(int requestClock, int machineKey, String ipAddress);

    [XmlRpcMethod(GlobalMethodName.requestHandlerRequestPermission)]
    String requestRequestPermission(int requestClock, int machineKey, String ipAddress, String requestString);
    #endregion
}


