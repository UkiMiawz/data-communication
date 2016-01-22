using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GlobalMethodName
{
    public const string serverShutDownFromClient = "Server.serverShutDownFromClient";
    public const string resourceGetString = "Resource.getString";

    #region Register Handler
    public const string removeMachineIp = "RegisterHandler.removeMachineIp";
    public const string getMachines = "RegisterHandler.getMachines";
    public const string getIpMaster = "RegisterHandler.getIpMaster";
    public const string newMachineJoin = "RegisterHandler.newMachineJoin";
    public const string newMachineJoinNotification = "RegisterHandler.newMachineJoinNotification";
    public const string addNewMachine = "RegisterHandler.addNewMachine";
    public const string getKeyMaster = "RegisterHandler.getKeyMaster";

    #endregion

    #region Election
    public const string leaderElection = "Election.leaderElection";
    public const string checkLeaderValidity = "Election.checkLeaderValidity";
    public const string setNewLeader = "Election.setNewLeader";
    #endregion 

    #region Request Handler
    public const string requestHandlerStartMessage = "Request.startMessage";
    public const string requestHandlerReceivePermission = "Request.receivePermission";
    public const string requestHandlerRequestPermission = "Request.requestPermission";
    #endregion

    #region RequestCentral Handler
    public const string requestCentralStartMessage = "RequestCentral.startMessage";
    public const string requestCentralReceiveRequest = "RequestCentral.receiveRequest";
    public const string requestCentralGetPermission = "RequestCentral.getPermission";
    public const string requestCentralFinishRequest = "RequestCentral.finishRequest";
    #endregion
}