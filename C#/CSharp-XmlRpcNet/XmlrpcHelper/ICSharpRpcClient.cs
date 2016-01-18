using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICSharpRpcClient : IXmlRpcProxy
{
    [XmlRpcMethod("Hello")]
    string HelloWorld(string input);

    [XmlRpcMethod(GlobalMethodName.getMachines)]
    XmlRpcStruct[] getMachines();

    [XmlRpcMethod(GlobalMethodName.serverShutDownFromClient)]
    void serverShutDownFromClient();

    [XmlRpcMethod(GlobalMethodName.leaderElection)]
    string leaderElection(string ipadress);

    [XmlRpcMethod(GlobalMethodName.getIpMaster)]
    string getIpMaster(string ipaddress);
    
    [XmlRpcMethod(GlobalMethodName.removeMachineIp)]
    string removeMachineIp(String ipAddress);

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
    String newMachineJoin(String ipAddress);
}

