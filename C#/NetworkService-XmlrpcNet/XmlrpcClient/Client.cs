using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Net;

class XmlrpcClient
{
    static void Main(string[] args)
    {
        ClientObject co = new ClientObject();
        string ipAddress = co.GetClientIpAdress();

        HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        string liveServer = "http://172.16.1.102:1090/networkServer.rem";
        string defaultServer = "http://localhost:1090/networkServer.rem";

        INetworkServer netServer = (INetworkServer)Activator.GetObject(
            typeof(INetworkServer), defaultServer);
        
        // Try to connect to master node
        try
        {
            string input;
            ClientUI clientUi = new ClientUI();

            do
            {
                clientUi.DisplayMainMenu(ipAddress);
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        NetworkMapStruct[] response = netServer.AddNewNode(ipAddress);

                        Console.WriteLine("The Ip map");
                        foreach (NetworkMapStruct ipItem in response)
                        {
                            Console.WriteLine("{0}", ipItem.IpAddress);
                        }

                        Console.ReadLine();
                        break;

                    case "2":
                        string result = netServer.getIpMaster(ipAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadLine();
                        break;

                    case "3":
                        netServer.joinNetwork(ipAddress);
                        Console.WriteLine("The server successfully called");
                        Console.ReadLine();
                        break;
                }
            }
            while (input != "0");
        }
        catch (XmlRpcFaultException fex)
        {
            Console.WriteLine("Fault response {0} {1} {2}",
                fex.FaultCode, fex.FaultString, fex.Message);
            Console.ReadLine();
        }
    }
}
