using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class RegisterHandler: MarshalByRefObject
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
        foreach (KeyValuePair<int, string> mach in machines)
        {
            Console.WriteLine(classNameLog + "priority " + mach.Key + " : " + mach.Value);
        }
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

    public String removeMachine(int key)
    {
        Console.WriteLine(classNameLog + "Remove machine with key " + key);
        String removedIp = CSharpRpcServer.removeMachineFromMap(key);
        return "Machine removed " + removedIp;
    }

    public String removeMachineIp(String ipAddress)
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
        XmlRpcHelper.SendToAllMachines(CSharpRpcServer.getMachines(), GlobalMethodName.addNewMachine, parameters);
        addNewMachine(newIp, callerIp);
        return "Notification from " + callerIp + " Machine added " + newIp;
    }

    [XmlRpcMethod(GlobalMethodName.newMachineJoin)]
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
            if (masterIp == myIp || masterIp == "localhost")
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
                String response = (String)XmlRpcHelper.SendToOneMachine(masterIp, GlobalMethodName.newMachineJoinNotification, parameters);
                Console.WriteLine(response);
            }

            return myIp;
        }
        catch (Exception e)
        {
            Console.WriteLine("{0}", e.Message);
            return "error";
        }
    }

    public static void joinNetwork(String ipAddress, String inputtedNeighbourIp)
    {
        Console.WriteLine(classNameLog + ipAddress + " joining network");

        try
        {
            //get IP addresses
            String myIp = CSharpRpcServer.getMyIpAddress();
            String subnet = ipAddress.Substring(0, (ipAddress.LastIndexOf('.')));
            Console.WriteLine(classNameLog + "Subnet : " + subnet);
            String ipNeighbor = inputtedNeighbourIp;
            
            if (inputtedNeighbourIp == string.Empty)
            {
                //search for neighbor automatically
                int i = 2;
                while (ipNeighbor == string.Empty && i < 255)
                {
                    try
                    {
                        String host = subnet + "." + i;
                        Console.WriteLine(classNameLog + "Contacting " + host);

                        //check if ip neighbor is valid
                        if (host == myIp)
                        {
                            Console.WriteLine(host + " is my own ip address!!");
                        }
                        else if (NetworkHelper.isServerUp(host, 1090, 300) == false)
                        {
                            ipNeighbor = string.Empty;
                            Console.WriteLine(host + " is not reachable");
                        }
                        else
                        {
                            Console.WriteLine(classNameLog + host + " is reachable. Checking validity");
                            Object[] parameters = new Object[] { ipAddress };
                            string neighborResponse = (String)XmlRpcHelper.SendToOneMachine(host, GlobalMethodName.newMachineJoin, parameters);
                            if(NetworkHelper.isValidIpAddress(neighborResponse))
                            {
                                ipNeighbor = neighborResponse;
                                Console.WriteLine(classNameLog + "Neighbor found. IP neighbor " + ipNeighbor);
                            }                                                        
                        }
                        
                        i++;
                    }
                    catch (Exception ex)
                    {
                        // Just continue around.
                    }
                }
            }
            else
            {
                if (inputtedNeighbourIp != myIp && inputtedNeighbourIp != "localhost")
                {
                    Object[] parameters = new Object[] { ipAddress };
                    ipNeighbor = (String)XmlRpcHelper.SendToOneMachine(inputtedNeighbourIp, GlobalMethodName.newMachineJoin, parameters);
                }
            }

            Console.WriteLine(classNameLog + "Finish registering to neighbor");

            if (ipNeighbor != string.Empty && ipNeighbor != "error" && ipNeighbor != myIp && ipNeighbor != "localhost")
            {
                //you're not alone
                Console.WriteLine(classNameLog + "I'm not alone! Requesting key and ip of master");
                //get master from neighbor
                Object[] parameters = new Object[] { myIp };
                int keyMaster = (int)XmlRpcHelper.SendToOneMachine(ipNeighbor, GlobalMethodName.getKeyMaster, parameters);
                String ipMaster = (String)XmlRpcHelper.SendToOneMachine(ipNeighbor, GlobalMethodName.getIpMaster, parameters);
                CSharpRpcServer.setMaster(keyMaster);
                Console.WriteLine(classNameLog + "Ip Master -> " + ipMaster + " Key Master -> " + keyMaster);

                //get hashmap from master
                // machines = Helper.ConvertStructToDict(XmlRpcHelper.SendToOneMachine(ipMaster, GlobalMethodName.getMachines, parameters));
                object result = XmlRpcHelper.SendToOneMachine(ipMaster, GlobalMethodName.getMachines, parameters);
                Dictionary<int, String> machines = Helper.ConvertObjToDict(result);
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

