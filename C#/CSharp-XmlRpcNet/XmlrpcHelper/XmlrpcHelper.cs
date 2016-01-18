using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing;
using System.Net.NetworkInformation;
using CookComputing.XmlRpc;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;

public class XmlRpcHelper
{
    private static String classNameLog = "XmlRpcHelper : ";
    private static int timeout = 2000;
    private static string ServerUrlStart = "http://";
    private static string ServerUrlEnd = ":1090/xml-rpc-example/xmlrpc";

    public static ICSharpRpcClient newProxy;

    public static ICSharpRpcClient Connect(string ipaddress)
    {
        Ping pinger = new Ping();
        PingReply pr = pinger.Send(ipaddress);

        if (pr.Status == IPStatus.Success)
        {
            Console.WriteLine("Connect to {0}", ipaddress);

            newProxy = XmlRpcProxyGen.Create<ICSharpRpcClient>();
            string newProxyUrl = ServerUrlStart + ipaddress + ServerUrlEnd;
            newProxy.Url = newProxyUrl;

            Console.WriteLine("XML-RPC Client call to : http://" + ipaddress + ":1090/xmlrpc/xmlrpc");
            

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
        try
        {
            //don't send to self
            NetworkPingService nps = new NetworkPingService();
            String myIpAddress = nps.GetMyIpAddress();

            if (ipAddress != myIpAddress)
            {
                //send this machine IP address and priority
                //XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
                //config.setServerURL(new URL(
                //        "http://" + ipAddress + ":1090/xml-rpc-example/xmlrpc"));
                //XmlRpcClient client = new XmlRpcClient();
                //client.setConfig(config);
                //Object response = (Object)client.execute(command, params);
                //Console.WriteLine(classNameLog + "Message: " + response);
                //return response;
                newProxy.Url = ServerUrlStart + ipAddress + ServerUrlEnd;
                object response;
                switch(command)
                {
                    case "Hello":
                        response = newProxy.HelloWorld(parameter.ToString());
                        break;

                    default:
                        response = "No such command";
                        break;
                }

                Console.WriteLine(classNameLog + "Message: " + response);
                return response;
            }
            else
            {
                return "Sending to self is not permitable";
            }
        }       
        catch (XmlRpcException e)
        {
            return "Connection refused";
        }        
        catch (Exception e)
        {
            Console.WriteLine("{0}", e.Message);
            return e.Message;
        }
    }

    public static void SendToAllMachines(Dictionary<int, String> machines, String command, Object[] parameter)
    {
        throw new NotImplementedException();
    }

}

