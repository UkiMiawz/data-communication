﻿using System;
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

        string defaultServer = "http://localhost:1090/xml-rpc-example/xmlrpc";

        INetworkServerClientProxy localProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        localProxy.Url = defaultServer;

        try
        {
            // ====== Begin client input ======
            string input;
            ClientUI clientUi = new ClientUI();

            do
            {
                clientUi.DisplayMainMenu(ipAddress);
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        // Menu 1: Show master hashmap.
                        localProxy.checkMasterStatus();

                        XmlRpcStruct[] networkHashMap = localProxy.GetNetworkHashMap();

                        Console.WriteLine("The Masternode hashmap");
                        foreach (XmlRpcStruct ipItem in networkHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", ipItem["NetworkPriority"], ipItem["IpAddress"]);
                        }

                        Console.ReadKey();
                        break;

                    case "2":
                        // Menu 1: Show local hashmap.
                        localProxy.checkMasterStatus();

                        XmlRpcStruct[] localhostHashMap = localProxy.GetNetworkHashMap();

                        Console.WriteLine("The localhost hashmap");
                        foreach (XmlRpcStruct ipItem in localhostHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", ipItem["NetworkPriority"], ipItem["IpAddress"]);
                        }

                        Console.ReadKey();
                        break;

                    case "3":
                        // Menu 3: Get Master Ip.
                        localProxy.checkMasterStatus();

                        string result = localProxy.getIpMaster(ipAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadKey();
                        break;

                    case "4":
                        // Menu 4: do election.
                        localProxy.checkMasterStatus();

                        Console.WriteLine("Election held!!!");

                        localProxy.DoLocalElection();
                        string newMasterIp = localProxy.getIpMaster(ipAddress);

                        Console.WriteLine("The new masternode is {0}", newMasterIp);
                        Console.ReadKey();
                        break;

                    case "6":
                        // Menu 6: Test Mutual Exclusion.
                        Console.WriteLine("Processing Now...");
                        bool isDirectProcessing = localProxy.SendMERequestToServer("addNewMessage", "this is new message");
                        if (isDirectProcessing)
                        {
                            Console.WriteLine("The process is complete!!!");
                        }
                        else
                        {
                            Console.WriteLine("You are on pending.");
                        }
                        Console.ReadKey();
                        break;

                    case "7":
                        // Menu 7: Rejoining Network.
                        localProxy.joinNetwork(ipAddress);
                        Console.WriteLine("You successfully rejoin the network!!");
                        Console.ReadKey();
                        break;

                    case "14":
                        // Menu 4: add new message.
                        localProxy.checkMasterStatus();

                        Console.WriteLine("write your new message: ");
                        string newMessage = Console.ReadLine();
                        localProxy.addNewMessage(newMessage);
                        break;

                    case "15":
                        // Menu 5: get all messages.
                        localProxy.checkMasterStatus();

                        Console.WriteLine("===== Start of messages ===== ");
                        string groupMessages = localProxy.getMessages();
                        Console.WriteLine("{0}", groupMessages);
                        Console.WriteLine("===== End of messages =====");
                        Console.ReadKey();
                        break;

                    case "17":
                        // Menu 7: show current Lamport clock.
                        localProxy.checkMasterStatus();

                        int lampClock = localProxy.getCurrentLamportClock();
                        Console.WriteLine("The current Lamport Clock is: {0}", lampClock);
                        Console.ReadKey();
                        break;


                }
            }
            while (input != "0");

            localProxy.removeMachine(ipAddress);
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
