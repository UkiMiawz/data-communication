using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class RegisterHandler
{
    private static int timeout = 200;
    private static String classNameLog = "RegisterHandler : ";

    public String getIpMaster(String callerIp)
    {
        String masterIp = CSharpRpcServer.getIpMaster();
        Console.WriteLine(classNameLog + callerIp + " requesting master ip " + masterIp);
        return masterIp;
    }

    public int getKeyMaster(String callerIp)
    {
        int masterKey = CSharpRpcServer.getKeyMaster();
        Console.WriteLine(classNameLog + callerIp + " requesting master key " + masterKey);
        return masterKey;
    }

    public Dictionary<int, String> getMachines(String callerIp)
    {
        Console.WriteLine(classNameLog + callerIp + " requesting hashmap machines");
        Dictionary<int, String> machines = CSharpRpcServer.getMachines();
        Console.WriteLine(classNameLog + machines);
        return machines;
    }

    public static String registerMachine(String ipAddress, int priority)
    {
        Console.WriteLine(classNameLog + "Register new machine " + ipAddress + " priority " + priority);
        String myIp = CSharpRpcServer.getMyIpAddress();
        int numberOfMachines = CSharpRpcServer.addMachineToMap(ipAddress, priority);
        return "From " + myIp + " You have been registered " + ipAddress + "with priority " + priority + ". Number of machines now " + numberOfMachines;
    }

    public String addNewMachine(String newIpAddress, String callerIpAddress)
    {
        Console.WriteLine(classNameLog + "Add new machine " + newIpAddress + " from " + callerIpAddress);
        CSharpRpcServer.addMachineToMap(newIpAddress);
        return "From " + callerIpAddress + " new machine registered " + newIpAddress;
    }

    public static String removeMachine(int key)
    {
        Console.WriteLine(classNameLog + "Remove machine with key " + key);
        String removedIp = CSharpRpcServer.removeMachineFromMap(key);
        return "Machine removed " + removedIp;
    }

    [XmlRpcMethod(GlobalMethodName.removeMachineIp)]
    public static String removeMachineIp(String ipAddress)
    {
        Console.WriteLine(classNameLog + "Remove machine with key " + ipAddress);
        int removedKey = CSharpRpcServer.removeMachineFromMap(ipAddress);
        return "Machine removed " + removedKey;
    }

    public String newMachineJoinNotification(String newIp, String callerIp)
    {
        Console.WriteLine(classNameLog + "New machine notification " + newIp + " from " + callerIp);
        //inform all others
        Object[] parameters = new Object[] { newIp, callerIp };
        Console.WriteLine(classNameLog + "Notification, telling all machines new machine IP ");
        // XmlRpcHelper.SendToAllMachines(JavaWsServer.getMachines(), "RegisterHandler.addNewMachine", params);
        addNewMachine(newIp, callerIp);
        return "Notification from " + callerIp + " Machine added " + newIp;
    }

    public String newMachineJoin(String ipAddress)
    {
        Console.WriteLine(classNameLog + "Join request from neighbor " + ipAddress);
        try
        {
            //tell everyone in network that new machine join
            String myIp = CSharpRpcServer.getMyIpAddress();
            String masterIp = CSharpRpcServer.getIpMaster();
            Object[] parameters = new Object[] { ipAddress, myIp };
            Console.WriteLine(classNameLog + "My IP " + myIp + " Master IP " + masterIp);

            //check master
            if (masterIp == myIp)
            {
                //if master is me, send to all other machines to add new machine
                //add new machine to map
                Console.WriteLine(classNameLog + "Master is me, add new machine, send new ip");
                newMachineJoinNotification(ipAddress, myIp);
            }
            else
            {
                Console.WriteLine(classNameLog + "I'm not master. Send notification to master");
                //inform master and let master handle
                //String response = (String)XmlRpcHelper.SendToOneMachine(masterIp, "RegisterHandler.newMachineJoinNotification", params);
                //System.out.println(response);
            }

            return myIp;
        }
        catch (Exception e)
        {
            Console.WriteLine("{0}", e.Message);
            return "error";
        }
    }

    public static void joinNetwork(String ipAddress, String neighbourIp)
    {
        Console.WriteLine(classNameLog + ipAddress + " joining network");

        try
        {
            //get IP addresses
            String myIp = CSharpRpcServer.getMyIpAddress();
            String subnet = ipAddress.Substring(0, (ipAddress.LastIndexOf('.') + 1));
            Console.WriteLine(classNameLog + "Subnet : " + subnet);
            String ipNeighbor = neighbourIp;

            int i = 2;

            ServerStatusCheck ssc = new ServerStatusCheck();
            Ping pinger = new Ping();
           
            if (neighbourIp == null && neighbourIp == string.Empty)
            {
                //search for neighbor automatically
                while (ipNeighbor == null && i < 255)
                {
                    String host = subnet + "." + i;
                    Console.WriteLine(classNameLog + "Contacting " + host);
                    PingReply pingresult = pinger.Send(host);

                    if (host != ipAddress && pingresult.Status == IPStatus.Success)
                    {
                        Console.WriteLine(classNameLog + host + " is reachable. Checking validity");
                        Object[] parameters = new Object[] { ipAddress };

                        ipNeighbor = (String)XmlRpcHelper.SendToOneMachine(host, "RegisterHandler.newMachineJoin", parameters);
                        //check if ip neighbor is valid
                        if (ssc.isServerUp(host, 1090, 300) == false)
                        {
                            ipNeighbor = null;
                        }
                        else
                        {
                            Console.WriteLine(classNameLog + "Neighbor found. IP neighbor " + ipNeighbor);
                        }
                    }
                    else
                    {
                        Console.WriteLine(host + " is not reachable");
                    }
                    i++;
                }
            }
            else
            {
                if (neighbourIp != myIp && neighbourIp != "localhost")
                {
                    Object[] parameters = new Object[] { ipAddress };
                    ipNeighbor = (String)XmlRpcHelper.SendToOneMachine(neighbourIp, "RegisterHandler.newMachineJoin", parameters);
                }
            }

            Console.WriteLine(classNameLog + "Finish registering to neighbor");

            if (ipNeighbor != null && ipNeighbor != "error" && ipNeighbor != myIp && ipNeighbor != "localhost")
            {
                //you're not alone
                Console.WriteLine(classNameLog + "I'm not alone! Requesting key and ip of master");
                //get master from neighbor
                Object[] parameters = new Object[] { myIp };
                int keyMaster = (int)XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getKeyMaster", parameters);
                String ipMaster = (String)XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getIpMaster", parameters);
                Console.WriteLine(classNameLog + "Ip Master -> " + ipMaster + " Key Master -> " + keyMaster);

                //get hashmap from master
                Dictionary<int, String> machines = new Dictionary<int, string>();
                //machines = Helper.convertMapResponseToMachinesTreeMap(XmlRpcHelper.SendToOneMachine(ipMaster, "RegisterHandler.getMachines", parameters));
                CSharpRpcServer.setMachines(machines);
                Console.WriteLine(classNameLog + "Machines set, number of machines " + machines.Count);

                //set ip master
                CSharpRpcServer.setMaster(keyMaster);
            }
            else
            {
                Console.WriteLine("I'm alone here. Guys....");
            }

            Console.WriteLine(classNameLog + "Join finished. Set last priority");
            //sort machines and set last priority on server
            CSharpRpcServer.setLastPriority();

        }
        catch (Exception e)
        {
            Console.WriteLine("{0}", e.Message);
        }
    }
}

