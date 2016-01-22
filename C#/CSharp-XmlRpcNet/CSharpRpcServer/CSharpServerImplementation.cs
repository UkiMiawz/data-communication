using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CSharpServerImplementation : MarshalByRefObject
{
    RegisterHandler myRegisterHandler;
    ElectionHelper myElectionHelper;
    RequestHandler myRequestHandler;
    RequestHandlerCentralized myRequestCentralHandler;
    ResourceHandler myResourceHandler;

    public CSharpServerImplementation()
    {
        myRegisterHandler = new RegisterHandler();
        myElectionHelper = new ElectionHelper();
        myRequestHandler = new RequestHandler();
        myRequestCentralHandler = new RequestHandlerCentralized();
        myResourceHandler = new ResourceHandler();
    }

    [XmlRpcMethod("Hello")]
    public string HelloWorld(string input)
    {
        return "Hello " + input;
    }

    [XmlRpcMethod(GlobalMethodName.resourceGetString)]
    public String resourceGetString(String ipAddress)
    {
        return myResourceHandler.getString(ipAddress);
    }

    [XmlRpcMethod(GlobalMethodName.resourceSetString)]
    public String resourceSetString(String newString, String ipAddress)
    {
        return myResourceHandler.setString(newString, ipAddress);
    }
    #region Register Handler

    [XmlRpcMethod(GlobalMethodName.getMachines)]
    public XmlRpcStruct getMachines(string callerIp)
    {
        XmlRpcStruct result = Helper.ConvertDictToStruct(myRegisterHandler.getMachines(callerIp));
        return result;
    }

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
    public String newMachineJoin(String ipAddress)
    {
        return myRegisterHandler.newMachineJoin(ipAddress);
    }

    [XmlRpcMethod(GlobalMethodName.newMachineJoinNotification)]
    public String newMachineJoinNotification(String newIp, String callerIp)
    {
        return myRegisterHandler.newMachineJoinNotification(newIp, callerIp);
    }

    [XmlRpcMethod(GlobalMethodName.addNewMachine)]
    public String addNewMachine(String newIpAddress, String callerIpAddress)
    {
        return myRegisterHandler.addNewMachine(newIpAddress, callerIpAddress);
    }

    [XmlRpcMethod(GlobalMethodName.getIpMaster)]
    public string getIpMaster(string ipAddress)
    {
        return myRegisterHandler.getIpMaster(ipAddress);
    }

    [XmlRpcMethod(GlobalMethodName.getKeyMaster)]
    public int getKeyMaster(String callerIp)
    {
        return myRegisterHandler.getKeyMaster(callerIp);
    }

    #endregion

    #region Election
    [XmlRpcMethod(GlobalMethodName.leaderElection)]
    public string leaderElection(string ipAddress)
    {
        try
        {
            return myElectionHelper.leaderElection(ipAddress);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error : {0}", ex.Message);
            return string.Empty;
        }
    }


    [XmlRpcMethod(GlobalMethodName.checkLeaderValidity)]
    public bool checkLeaderValidity(String callerIp)
    {
        return myElectionHelper.checkLeaderValidity(callerIp);
    }

    [XmlRpcMethod(GlobalMethodName.setNewLeader)]
    public String setNewLeader(int keyMaster)
    {
        return myElectionHelper.setNewLeader(keyMaster);
    }
    #endregion

    #region Request Handler
    [XmlRpcMethod(GlobalMethodName.requestHandlerStartMessage)]
    public string requestStartMessage(bool wantWrite, bool isSignal)
    {
        return myRequestHandler.startMessage(wantWrite, isSignal);
    }

    #endregion

    #region RequestCentral Handler
    [XmlRpcMethod(GlobalMethodName.requestCentralStartMessage)]
    public string requestCentralStartMessage(bool localWantWrite)
    {
        return myRequestCentralHandler.startMessage(localWantWrite);
    }

    [XmlRpcMethod(GlobalMethodName.requestCentralReceiveRequest)]
    public void requestCentralReceiveRequest(String requestIp, String requestString)
    {
        myRequestCentralHandler.receiveRequest(requestIp, requestString);
    }

    [XmlRpcMethod(GlobalMethodName.requestCentralGetPermission)]
    public void requestCentralGetPermission(String requestIp, String requestString)
    {
        myRequestCentralHandler.getPermission(requestIp, requestString);
    }

    [XmlRpcMethod(GlobalMethodName.requestCentralFinishRequest)]
    public void requestCentralFinishRequest(String requestIp)
    {
        myRequestCentralHandler.finishRequest(requestIp);
    }
    #endregion
}

