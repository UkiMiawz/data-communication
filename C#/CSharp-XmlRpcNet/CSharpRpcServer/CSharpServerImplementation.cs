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

    public CSharpServerImplementation()
    {
        myRegisterHandler = new RegisterHandler();
        myElectionHelper = new ElectionHelper();
    }

    [XmlRpcMethod("Hello")]
    public string HelloWorld(string input)
    {
        return "Hello " + input;
    }

    [XmlRpcMethod(GlobalMethodName.getMachines)]
    public XmlRpcStruct[] getMachines(string callerIp)
    {
        XmlRpcStruct[] result = Helper.ConvertDictToStruct(myRegisterHandler.getMachines(callerIp));
        return result;
    }

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

    [XmlRpcMethod(GlobalMethodName.checkLeaderValidity)]
    public bool checkLeaderValidity(String callerIp)
    {
        return myElectionHelper.checkLeaderValidity(callerIp);
    }


    [XmlRpcMethod(GlobalMethodName.setNewLeader)]
    String setNewLeader(int keyMaster)
    {
        return myElectionHelper.setNewLeader(keyMaster);
    }
}

