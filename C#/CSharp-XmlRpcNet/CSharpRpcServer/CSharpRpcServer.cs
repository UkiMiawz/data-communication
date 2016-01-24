using CookComputing.XmlRpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading.Tasks;

class CSharpRpcServer : MarshalByRefObject
{
    private static int timeout = 100;
    private static ElectionHelper electionHelper;
    private static String classNameLog = "CSharpServer : ";

    private static Dictionary<int, String> machines = new Dictionary<int, string>();
    public static Dictionary<int, String> getMachines()
    {
        return machines;
    }

    public Dictionary<int, String> getMachines(String ipAddress)
    {
        Console.WriteLine(classNameLog + "Machines list request from " + ipAddress);
        Console.WriteLine(classNameLog + "Machines : " + machines);
        return machines;
    }

    public static void setMachines(Dictionary<int, String> newMachines)
    {
        machines = newMachines;
        Console.WriteLine(classNameLog + "New machines value " + newMachines);
    }

    private static IPHostEntry myIp;
    public static IPHostEntry getMyIp()
    {
        return myIp;
    }

    private static int myPriority;
    public static int getMyPriority()
    {
        return myPriority;
    }

    private static String myIpAddress;
    public static String getMyIpAddress()
    {
        return myIpAddress;
    }

    private static int keyMaster;
    public static int getKeyMaster()
    {
        return keyMaster;
    }

    private static String ipMaster = "localhost";
    public static String getIpMaster()
    {
        return ipMaster;
    }

    /* ========= SYNC PROPERTIES SETTER AND GETTER ====== */

    private static String sharedString = "";
    public static String getSharedString()
    {
        return sharedString;
    }
    public static String setSharedString(String newString)
    {
        sharedString = newString;
        return sharedString;
    }

    private static String myString = "";
    public static String getMyString()
    {
        return myString;
    }
    public static String setMyString(String newString)
    {
        myString = newString;
        return myString;
    }

    /* ========= REGISTRATION METHODS ====== */

    public static String setMaster(int master)
    {
        Console.WriteLine(classNameLog + "Key Master : " + master);
        keyMaster = master;
        ipMaster = machines.FirstOrDefault(x => x.Key == master).Value;
        Console.WriteLine(classNameLog + "IP Master : " + ipMaster);
        return ipMaster;
    }

    private static int lastPriority = 0;
    public static void setLastPriority()
    {
        if (machines.Count > 0)
        {
            Console.WriteLine("Printing all machines :");
            foreach (KeyValuePair<int, string> machineDetail in machines)
            {
                Console.WriteLine("Priority {0} with IP {1}", machineDetail.Key, machineDetail.Value);
            }

            int biggestPriority = machines.Keys.Max();
            lastPriority = biggestPriority;
        }
    }

    public static int addMachineToMap(String ipAddress)
    {
        if (!machines.ContainsValue(ipAddress))
        {
            lastPriority += 1;
            machines.Add(lastPriority, ipAddress);
            Console.WriteLine(classNameLog + "New Machine added with priority : " + lastPriority);
            Console.WriteLine(classNameLog + "Total number of machines now :" + machines.Count());
        }
        else
        {
            Console.WriteLine(classNameLog + "Machine already in the map" + ipAddress);
        }

        return lastPriority;
    }

    public static int addMachineToMap(String ipAddress, int priority)
    {
        if (!machines.ContainsValue(ipAddress))
        {
            machines.Add(priority, ipAddress);
            Console.WriteLine(classNameLog + "New Machine added with priority : " + priority);
            Console.WriteLine(classNameLog + "Total number of machines now :" + machines.Count());
        }
        else
        {
            Console.WriteLine(classNameLog + "Machine already in the map" + ipAddress);
        }

        return machines.Count();
    }

    public static String removeMachineFromMap(int key)
    {
        String machineIp = machines.First(x => x.Key == key).Value;
        machines.Remove(key);
        return machineIp;
    }

    public static int removeMachineFromMap(String ipAddress)
    {
        int machineKey = machines.First(x => x.Value == ipAddress).Key;
        machines.Remove(machineKey);
        return machineKey;
    }

    /* ========= SERVER SHUTDOWN METHODS ====== */

    public static void serverShutDown()
    {
        Object[] parameters = new Object[] { myIpAddress };
        XmlRpcHelper.SendToAllMachines(machines, GlobalMethodName.removeMachineIp, parameters);
    }

    public void serverShutDownFromClient(String ip)
    {
        Console.WriteLine(ip + " client ask to shutdown server");
        Object[] parameters = new Object[] { myIpAddress };
        XmlRpcHelper.SendToAllMachines(machines, GlobalMethodName.removeMachineIp, parameters);
        Console.WriteLine("Server shutdown");
    }

    /* ========= MAIN METHODS ====== */

    public static void Main(string[] args)
    {
        IDictionary props = new Hashtable();
        props["name"] = "MyHttpChannel";
        props["port"] = 1090;

        HttpChannel channel = new HttpChannel(
            props,
            new XmlRpcClientFormatterSinkProvider(),
            new XmlRpcServerFormatterSinkProvider());
        ChannelServices.RegisterChannel(channel, false);

        RemotingConfiguration.RegisterWellKnownServiceType(
            typeof(CSharpServerImplementation),
            "xml-rpc-example/xmlrpc",
            WellKnownObjectMode.Singleton);

        #region Section 1
        // ===========================Section 1: Joining the network================================
        try
        {
            electionHelper = new ElectionHelper();

            //assign my IP
            myIp = Dns.GetHostEntry(Dns.GetHostName());
            myIpAddress = NetworkHelper.GetMyIpAddress();

            //join network
            String myNeighbourIp = "";

            //ask for neighbour ip or not

            Console.WriteLine("=====Connecting to Network=====");
            Console.WriteLine("1. Input neighbour IP Address manually");
            Console.WriteLine("2. Detect neighbour IP Address automatically");
            Console.WriteLine("Enter choice, or 'exit' to quit: ");
            String command = Console.ReadLine();

            if (command == "1")
            {
                Console.WriteLine("Please input neighbor IP address :");
                myNeighbourIp = Console.ReadLine();
            }

            Console.WriteLine("");

            RegisterHandler.joinNetwork(myIpAddress, myNeighbourIp);
            if (!machines.ContainsValue(myIpAddress))
            {
                Console.WriteLine("Add myself to hashmap");
                myPriority = addMachineToMap(myIpAddress);
            }
            else
            {
                //get priority
                myPriority = machines.First(x => x.Value == myIpAddress).Key;
            }

            //set myself as master if null
            if (ipMaster == "localhost" || ipMaster == null || ipMaster == string.Empty)
            {
                setMaster(myPriority);
            }

            Console.WriteLine(classNameLog + "Master IP now " + ipMaster);
            Console.WriteLine(classNameLog + "Master Key now {0}", machines.First(x => x.Value == ipMaster).Value);
            Console.WriteLine(classNameLog + "My IP now " + myIpAddress);
            Console.WriteLine(classNameLog + "My priority now " + myPriority);
            Console.WriteLine(classNameLog + "Machines now " + machines.Count());
        }
        catch(Exception ex)
        {
            Console.WriteLine("{0}", ex.Message);
        }
        // ====================================End of Section 1========================================
        #endregion

        #region Section 2
        // =============================Section 2: Begin service===========================================
        string input;
        do
        {
            input = Console.ReadLine().ToLower();
            try
            {
                Console.WriteLine("Enter command, or 'exit' to quit: ");
                switch (input)
                {
                    case "ip":
                        Console.WriteLine("This machine ip :" + myIpAddress);
                        break;                   
                    default:
                        Console.WriteLine("Command " + input + " not recognized");
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0}", e.Message);
            }
        }
        while (input != "exit");
        // ================================End of Section 2=============================================
        #endregion

        Console.WriteLine("Shutting down server...");
        serverShutDown();
    }   
}