using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class GlobalMethodName
{
    public const string removeMachineIp = "RegisterHandler.removeMachineIp";
    public const string getMachines = "RegisterHandler.getMachines";
    public const string serverShutDownFromClient = "Server.serverShutDownFromClient";
    public const string leaderElection = "Election.leaderElection";
    public const string getIpMaster = "RegisterHandler.getIpMaster";
    public const string newMachineJoin = "RegisterHandler.newMachineJoin";
    public const string newMachineJoinNotification = "RegisterHandler.newMachineJoinNotification";
    public const string addNewMachine = "RegisterHandler.addNewMachine";
    public const string getKeyMaster = "RegisterHandler.getKeyMaster";
    public const string checkLeaderValidity = "Election.checkLeaderValidity";
    public const string setNewLeader = "Election.setNewLeader";

    #region Request Handler
    public const string requestHandlerStartMessage = "Request.startMessage";
    public const string requestHandlerReceivePermission = "Request.receivePermission";
    public const string requestHandlerRequestPermission = "Request.requestPermission";
    #endregion
}