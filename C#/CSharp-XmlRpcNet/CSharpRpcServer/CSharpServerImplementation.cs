using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CSharpServerImplementation: MarshalByRefObject
{
    RegisterHandler myRegisterHandler;
    
    public CSharpServerImplementation()
    {
        myRegisterHandler = new RegisterHandler();
    }

    [XmlRpcMethod("Hello")]
    public string HelloWorld(string input)
    {
        return "Hello " + input;
    }

    [XmlRpcMethod(GlobalMethodName.getMachines)]
    XmlRpcStruct[] getMachines(string callerIp)
    {
        XmlRpcStruct[] result = Helper.ConvertDictToStruct(myRegisterHandler.getMachines(callerIp));
        return result;
    }

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
    public String newMachineJoin(String ipAddress)
    {
        return myRegisterHandler.newMachineJoin(ipAddress);
    }

    [XmlRpcMethod(GlobalMethodName.newMachineJoinNotification)]
    String newMachineJoinNotification(String newIp, String callerIp)
    {
        return myRegisterHandler.newMachineJoinNotification(newIp, callerIp);
    }

    [XmlRpcMethod(GlobalMethodName.addNewMachine)]
    String addNewMachine(String newIpAddress, String callerIpAddress)
    {
        return myRegisterHandler.addNewMachine(newIpAddress, callerIpAddress);
    }

    [XmlRpcMethod(GlobalMethodName.getIpMaster)]
    string getIpMaster(string ipaddress)
    {
        return myRegisterHandler.getIpMaster(ipaddress);
    }

    [XmlRpcMethod(GlobalMethodName.getKeyMaster)]
    int getKeyMaster(String callerIp)
    {
        return myRegisterHandler.getKeyMaster(callerIp);
    }
}

