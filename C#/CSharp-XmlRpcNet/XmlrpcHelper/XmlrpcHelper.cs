using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing;
using System.Net.NetworkInformation;

public class XmlRpcHelper : MarshalByRefObject
{
    public static string ServerUrlStart = "http://";
    public static string ServerUrlEnd = ":1090/networkServer.rem";

    public static ICSharpRpcClient newProxy;

    public static ICSharpRpcClient Connect(string ipaddress)
    {
        Ping pinger = new Ping();
        PingReply pr = pinger.Send(ipaddress);

        if (pr.Status == IPStatus.Success)
        {
            Console.WriteLine("Connect to {0}", ipaddress);
            string newProxyUrl = ServerUrlStart + ipaddress + ServerUrlEnd;

            Console.WriteLine("XML-RPC Client call to : http://" + ipaddress + ":1090/xmlrpc/xmlrpc");
            newProxy.Url = newProxyUrl;

            return newProxy;
        }
        else
        {
            return null;
        }
    }

    public static void SendToAllMachinesAsync(Dictionary<int, String> machines, String command, Object[] parameter, AsyncCallback callback)
    {
        throw new NotImplementedException();
    }

    public static Object SendToOneMachine(String ipAddress, String command, Object[] parameter)
    {
        throw new NotImplementedException();
    }

    public static void SendToAllMachines(Dictionary<int, String> machines, String command, Object[] parameter)
    {
        throw new NotImplementedException();
    }

}

