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

    [XmlRpcMethod("Server.getMachines")]
    XmlRpcStruct[] getMachines();

    [XmlRpcMethod("Server.serverShutDownFromClient")]
    void serverShutDownFromClient();

    [XmlRpcMethod("Election.leaderElection")]
    string leaderElection(string ipadress);

    [XmlRpcMethod("RegisterHandler.getIpMaster")]
    string getIpMaster(string ipaddress);
    
    [XmlRpcMethod(GlobalMethodName.removeMachineIp)]
    string removeMachineIp(String ipAddress);
}

