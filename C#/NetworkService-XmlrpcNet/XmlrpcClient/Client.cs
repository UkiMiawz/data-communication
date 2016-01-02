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

        string defaultServer = "http://localhost:1090/networkServer.rem";

        INetworkServer netServer = (INetworkServer)Activator.GetObject(
            typeof(INetworkServer), defaultServer);
        
        // ============= Try to join the network ===============
        try
        {
            netServer.joinNetwork(ipAddress);
            string masterNode = netServer.getIpMaster(ipAddress);
            if (masterNode != ipAddress)
            {
                string masterNodeServer = "http://" + masterNode + ":1090/networkServer.rem";
                netServer = (INetworkServer)Activator.GetObject(
                    typeof(INetworkServer), masterNodeServer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An exception occurred.");
            Console.WriteLine("{0}", ex.Message);
        }
        // ============End of joining network=====================

        // =============== Begin client input ===============
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
                        NetworkMapStruct[] networkHashMap = netServer.ShowNetworkHashMap();

                        Console.WriteLine("The Network hashmap");
                        foreach (NetworkMapStruct ipItem in networkHashMap)
                        {
                           Console.WriteLine("Priority {0} : {1}", ipItem.NetworkPriority, ipItem.IpAddress);
                        }

                        Console.ReadKey();
                        break;

                    case "2":
                        string result = netServer.getIpMaster(ipAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadKey();
                        break;

                    case "3":
                        netServer.newMachineJoin(ipAddress);
                        Console.WriteLine("You successfully join the network");
                        Console.ReadKey();
                        break;
                }
            }
            while (input != "0");

            netServer.removeMachine(ipAddress);
            Console.WriteLine("You are logging out. Your machine have been removed from network");
            Console.ReadKey();
        }
        catch (XmlRpcFaultException fex)
        {
            Console.WriteLine("Fault response {0} {1} {2}",
                fex.FaultCode, fex.FaultString, fex.Message);
            Console.ReadLine();
        }
        // ===========End of Client Input================
    }
}
