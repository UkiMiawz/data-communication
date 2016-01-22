using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;


class CSharpRpcClient
{
    static void Main(string[] args)
    {
        ClientObject co = new ClientObject();
        string myIpAddress = co.GetClientIpAdress();

        HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        ICSharpRpcClient localProxy = XmlRpcHelper.Connect("localhost");

        string currentMasterUrl = localProxy.getIpMaster(myIpAddress);
        ICSharpRpcClient masterProxy = XmlRpcHelper.Connect(currentMasterUrl);

        try
        {
            // ====== Begin client input ======
            string input;
            ClientUI clientUi = new ClientUI();

            do
            {
                clientUi.DisplayMainMenu(myIpAddress);
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        // Menu 1: Show master hashmap.
                        // localProxy.checkMasterStatus();

                        XmlRpcStruct masterMap = masterProxy.getMachines(myIpAddress);

                        Dictionary<int, string> networkHashMap = Helper.ConvertObjToDict(masterMap);

                        Console.WriteLine("The Masternode hashmap");
                        foreach (KeyValuePair<int, string> networkNode in networkHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", networkNode.Key, networkNode.Value);
                        }

                        Console.ReadKey();
                        break;

                    case "2":
                        // Menu 2: Show local hashmap.
                        // localProxy.checkMasterStatus();

                        XmlRpcStruct localMap = localProxy.getMachines(myIpAddress);

                        Dictionary<int, string> localhostHashMap = Helper.ConvertObjToDict(localMap);

                        Console.WriteLine("The localnode hashmap");
                        foreach (KeyValuePair<int, string> localNode in localhostHashMap)
                        {
                            Console.WriteLine("Priority {0} : {1}", localNode.Key, localNode.Value);
                        }
                        Console.ReadKey();
                        break;

                    case "3":
                        // Menu 3: Get Master Ip.
                        //localProxy.checkMasterStatus();

                        string result = masterProxy.getIpMaster(myIpAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadKey();
                        break;

                    case "4":
                        //Menu 4: do election.
                        // masterProxy.checkMasterStatus();

                        Console.WriteLine("Election held!!!");

                        string newMaster = masterProxy.leaderElection(myIpAddress);
                        //string newMasterIp = localProxy.getIpMaster(ipAddress);

                        Console.WriteLine("The new masternode is {0}", newMaster);
                        Console.ReadKey();
                        break;

                    case "5":
                        // Menu 5: Ricart Agrawala Exlusion.
                        Console.WriteLine("Start Ricart Agrawala exclusion");
                        string RAresponse = masterProxy.requestStartMessage(true, true);
                        Console.WriteLine("Resource value now :" + RAresponse);
                        Console.ReadKey();
                        break;

                    case "6":
                        // Menu 6: Test Mutual Exclusion.
                        Console.WriteLine("Start centralized mutual exclusion");
                        string CMEresponse = masterProxy.requestCentralStartMessage(true);
                        Console.WriteLine("Resource value now :" + CMEresponse);
                        Console.ReadKey();
                        break;

                    //case "7":
                    //    //Menu 7: Rejoining Network.
                    //    localProxy.
                    //    Console.WriteLine("You successfully rejoin the network!!");
                    //    Console.ReadKey();
                    //    break;

                    default:
                        Console.WriteLine("from master {0}", masterProxy.HelloWorld("master Aderick"));
                        Console.WriteLine("from local {0}", localProxy.HelloWorld("Aderick"));
                        Console.ReadKey();
                        break;
                }
            }
            while (input != "0");

            //localProxy.removeMachine(ipAddress);
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

