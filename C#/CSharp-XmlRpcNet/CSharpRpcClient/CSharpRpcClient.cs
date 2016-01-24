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
    private static ICSharpRpcClient _localProxy;

    private static string _currentMasterUrl;
    private static ICSharpRpcClient _masterProxy;

    /***
	 * Start mutual exclusion process
	 * @param isCentralized To choose whether mutual exclusion using centralized or ricart
	 */
    private static void StartMutualExclusion(Boolean isCentralized)
    {
        try
        {
            String response;

            //call mutual exclusion to write
            Object[] parameters = new Object[] { true, false };

            if (isCentralized)
            {
                Console.WriteLine("Start centralized mutual exclusion");
                response = (String)_localProxy.requestCentralStartMessage(true, false);
                Console.WriteLine("Resource value now :" + response);
            }
            else {
                Console.WriteLine("Start ricart agrawala mutual exclusion");
                response = (String)_localProxy.requestStartMessage(true, false);
                Console.WriteLine("Resource value now :" + response);
            }

            Console.WriteLine("Start waiting process");
            for (int i = 0; i < 20; i++)
            {
                Console.WriteLine(i);
                System.Threading.Thread.Sleep(1000);
            }
			
			//call mutual exclusion to read
			parameters = new Object[] { false, false };
            if (isCentralized)
            {
                Console.WriteLine("Start centralized mutual exclusion read");
                response = (String)_localProxy.requestCentralStartMessage(false, false);
                Console.WriteLine("Resource value now :" + response);
            }
            else {
                Console.WriteLine("Start ricart agrawala mutual exclusion read");
                response = (String)_localProxy.requestStartMessage(false, false);
                Console.WriteLine("Resource value now :" + response);
            }

            String[] parts = response.Split(';');
            Console.WriteLine("Split array value " + string.Join(" | ", parts));

            if (parts[1] == "1")
            {
                Console.WriteLine("!!!!My string is in the final string!!!!");
            }
            else {
                Console.WriteLine("!!!!My string is not in the final string!!!!");
            }

            Console.WriteLine("Final String: " + parts[0]);

        }
        catch (Exception e)
        {
            // TODO Auto-generated catch block
            Console.WriteLine(e.Message);
        }
    }

    static void Main(string[] args)
    {
        ClientObject co = new ClientObject();
        string myIpAddress = co.GetClientIpAdress();

        HttpChannel chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        _localProxy = XmlRpcHelper.Connect("localhost");

        _currentMasterUrl = _localProxy.getIpMaster(myIpAddress);
        _masterProxy = XmlRpcHelper.Connect(_currentMasterUrl);

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

                        XmlRpcStruct masterMap = _masterProxy.getMachines(myIpAddress);

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

                        XmlRpcStruct localMap = _localProxy.getMachines(myIpAddress);

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

                        string result = _localProxy.getIpMaster(myIpAddress);
                        Console.WriteLine("the masterNode is {0}", result);
                        Console.ReadKey();
                        break;

                    case "4":
                        //Menu 4: do election.
                        // masterProxy.checkMasterStatus();

                        Console.WriteLine("Election held!!!");

                        string newMaster = _localProxy.leaderElection(myIpAddress);
                        //string newMasterIp = localProxy.getIpMaster(ipAddress);

                        Console.WriteLine("The new masternode is {0}", newMaster);
                        Console.ReadKey();
                        break;

                    case "5":
                        // Menu 5: Ricart Agrawala Exlusion.
                        StartMutualExclusion(false);
                        Console.ReadKey();
                        break;

                    case "6":
                        // Menu 6: Test Mutual Exclusion.
                        StartMutualExclusion(true);
                        Console.ReadKey();
                        break;

                    //case "7":
                    //    //Menu 7: Rejoining Network.
                    //    localProxy.
                    //    Console.WriteLine("You successfully rejoin the network!!");
                    //    Console.ReadKey();
                    //    break;

                    default:
                        Console.WriteLine("from master {0}", _masterProxy.HelloWorld("master Aderick"));
                        Console.WriteLine("from local {0}", _localProxy.HelloWorld("Aderick"));
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

