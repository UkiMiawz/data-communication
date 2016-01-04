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

        INetworkServerClientProxy localProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        localProxy.Url = defaultServer;

        INetworkServerClientProxy masterProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        masterProxy.Url = defaultServer;

        // ============= Try to join the network ===============
        try
        {
            localProxy.joinNetwork(ipAddress);
            string masterNode = localProxy.getIpMaster(ipAddress);
            if (masterNode != ipAddress)
            {
                string masterNodeServer = "http://" + masterNode + ":1090/networkServer.rem";

                masterProxy.Url = masterNodeServer;
                NetworkMapStruct[] newHashMap = masterProxy.ShowNetworkHashMap();
                localProxy.updateLocalHashmapFromMasterNode(newHashMap);
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
                        NetworkMapStruct[] networkHashMap = masterProxy.ShowNetworkHashMap();

                        Console.WriteLine("The Masternode hashmap");
                        foreach (NetworkMapStruct ipItem in networkHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", ipItem.NetworkPriority, ipItem.IpAddress);
                        }

                        Console.ReadKey();
                        break;

                    case "2":
                        NetworkMapStruct[] localhostHashMap = localProxy.ShowNetworkHashMap();

                        Console.WriteLine("The localhost hashmap");
                        foreach (NetworkMapStruct ipItem in localhostHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", ipItem.NetworkPriority, ipItem.IpAddress);
                        }

                        Console.ReadKey();
                        break;

                    case "3":
                        string result = masterProxy.getIpMaster(ipAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.WriteLine("write your new message: ");
                        string newMessage = Console.ReadLine();
                        masterProxy.addNewMessage(newMessage);
                        Console.WriteLine("Your message successfully sent.");
                        break;

                    case "5":
                        Console.WriteLine("The messages are: ");
                        string[] groupMessages = masterProxy.getMessages();
                        foreach (string msg in groupMessages)
                        {
                            Console.WriteLine("{0}", msg);
                        }
                        Console.WriteLine("===== End of messages =====");
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.WriteLine("Election held!!!");
                        localProxy.doElection();
                        string newMasterNode = localProxy.getIpMaster(ipAddress);

                        // re-assign the new masternode.
                        try
                        {
                            string masterNodeServer = "http://" + newMasterNode + ":1090/networkServer.rem";
                            
                            masterProxy.Url = masterNodeServer;
                            NetworkMapStruct[] newHashMap = masterProxy.ShowNetworkHashMap();
                            localProxy.updateLocalHashmapFromMasterNode(newHashMap);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("{0}", ex.Message);
                        }
                        
                        Console.WriteLine("The new masternode is {0}", newMasterNode);
                        Console.ReadKey();
                        break;
                }
            }
            while (input != "0");

            masterProxy.removeMachine(ipAddress);
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
