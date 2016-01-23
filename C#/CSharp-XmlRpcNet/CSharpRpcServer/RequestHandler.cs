using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Mutual Exclusion with Ricart
/// </summary>
public class RequestHandler
{
    private static Dictionary<int, String> expectedRequestIps = new Dictionary<int, String>();
    private static Dictionary<int, String> differedRequestIps = new Dictionary<int, string>();
    private static LogicalClock localClock = new LogicalClock();
    private static int timeout = 3000;
    private static ResourceHandler resourceHandler = new ResourceHandler();
    private static String classNameLog = "RequestHandler : ";

    private static String masterIp = CSharpRpcServer.getIpMaster();
    private static String myIp = CSharpRpcServer.getMyIpAddress();
    private static int myKey = CSharpRpcServer.getMyPriority();
    private static Dictionary<int, String> machines = CSharpRpcServer.getMachines();

    private static Boolean _haveInterest = false;
    private static Boolean _currentlyAccessing = false;
    private static Boolean _wantWrite = false;

    private static String _finalString = "";
    private static String _myString = "";

    public String startMessage(Boolean wantWrite, Boolean isSignal)
    {

        //initialize variables
        Console.WriteLine(classNameLog + "Start mutual exclusion process");
        Console.WriteLine(classNameLog + "Master IP =>" + masterIp);
        Console.WriteLine(classNameLog + "My IP => " + myIp + " My key => " + myKey);

        if (!isSignal)
        {
            //start signal from client, inform others
            //contact all machines to start
            Dictionary<int, String> machines = CSharpRpcServer.getMachines();
            Console.WriteLine(classNameLog + "Contacting all nodes " + machines);
            Object[] parameters = new Object[] { true };
            XmlRpcHelper.SendToAllMachinesAsync(machines, GlobalMethodName.requestHandlerStartMessage, parameters);
        }

        Console.WriteLine(classNameLog + "Initiating... Want to write => " + wantWrite);
        _wantWrite = wantWrite;
        return sendRequest();
    }

    //send request
    private String sendRequest()
    {

        //increment clock by 1
        localClock.incrementClock();
        Console.WriteLine(classNameLog + "Lamport clock value incremented to " + localClock.getClockValue());

        //append all machines ip to list since we need to get permissions from all machine
        expectedRequestIps.Clear();

        foreach (KeyValuePair<int, String> entry in machines)
        {
            expectedRequestIps.Add(entry.Key, entry.Value);
        }

        _haveInterest = true;
        Console.WriteLine(classNameLog + "Expecting permissions from " + expectedRequestIps);
        Console.WriteLine(classNameLog + "My IP " + myIp);

        //request for permission to all machines
        foreach (KeyValuePair<int, String> entry in machines)
        {

            String ipAddress = entry.Value;
            int machineKey = entry.Key;
            Console.WriteLine(classNameLog + "Requesting permission from IP => " + ipAddress + " and machine key => " + machineKey);

            //remove me from request waiting list
            if (ipAddress == myIp)
            {
                removeMachineFromExpected(machineKey);
                Console.WriteLine(classNameLog + "Removing myself from machines list. My key " + machineKey);
            }

            try
            {
                //check if machine is on
                if (NetworkHelper.isServerUp(ipAddress, 1090, 500))
                {
                    Console.WriteLine(classNameLog + "Machine is alive. Asking permission from machine " + entry.Key + " => " + ipAddress);

                    //ask permission to all
                    String requestString = "Request.askPermission";
                    Object[] parameters = new Object[] { localClock.getClockValue(), myKey, myIp, requestString };
                    String replyOk = (String)XmlRpcHelper.SendToOneMachine(ipAddress, "Request.requestPermission", parameters);
                    Console.WriteLine(classNameLog + "Reply permission => " + replyOk);

                    if (replyOk == "true" && replyOk == "false")
                    {
                        if (replyOk == "true")
                        {
                            Console.WriteLine(classNameLog + "Got permission from " + machineKey + " removing machine from expected list");
                            removeMachineFromExpected(machineKey);
                        }
                    }
                    else
                    {
                        Console.WriteLine(classNameLog + "Reply not as expected, removing machine from waiting list");
                        removeMachineFromExpected(machineKey);
                    }

                }
                else
                {
                    removeMachineFromExpected(machineKey);
                    Console.WriteLine(classNameLog + "Machine " + machineKey + " is not active. IP Address " + ipAddress);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Machine not valid {0}", e.Message);
                removeMachineFromExpected(machineKey);
                return e.Message;
            }
        }

        Console.WriteLine(classNameLog + "Finished requesting permission");

        if (expectedRequestIps.Count == 0)
        {
            Console.WriteLine(classNameLog + "Everybody gave permission. Start resource access");
            String resource = doResourceAccess();
            Console.WriteLine(classNameLog + "Resource value now " + resource);
        }
        else
        {
            Console.WriteLine(classNameLog + "Wait until process finished");
            //wait until all finished
            while (_haveInterest)
            {
                try
                {
                    Console.WriteLine(".");
                    System.Threading.Thread.Sleep(500);
                }
                catch (Exception e)
                {
                    Console.WriteLine("{0}", e.Message);
                    return e.Message;
                }
            }
        }

        if (_wantWrite)
        {
            //return written string
            Console.WriteLine(classNameLog + "Written String => " + _myString);
            return _myString;
        }

        Console.WriteLine(classNameLog + "Read String check my string => " + _myString);
        Console.WriteLine(classNameLog + "Final String => " + _finalString);
        int containMyString = 0;

        if (_finalString.Contains(_myString))
        {
            Console.WriteLine(classNameLog + "!!!!My string is in the final string!!!!");
            containMyString = 1;
        }

        Console.WriteLine(classNameLog + "Read process finished, final result => " + _finalString + ";" + containMyString);
        return _finalString + ";" + containMyString;
    }

    // receive message for ok permission, triggering resource access when all have given permission
    public String receivePermission(int requestClock, int machineKey, String ipAddress)
    {
        //sync clock from message
        Console.WriteLine(classNameLog + "Received permission");
        Console.WriteLine(classNameLog + "Local clock " + localClock.getClockValue() + " request clock " + requestClock);
        localClock.syncClock(requestClock);
        Console.WriteLine(classNameLog + "Local clock now " + localClock.getClockValue());
        Console.WriteLine("Permission given from " + ipAddress);
        removeMachineFromExpected(machineKey);
        //check if all machine clear
        if (expectedRequestIps.Count == 0 && _haveInterest)
        {
            Console.WriteLine("Permission clear, access resource");
            String nowResource;
            nowResource = doResourceAccess();
            Console.WriteLine("Resource value now : " + nowResource);
        }

        return _finalString;
    }

    //receive message request for permission
    public String requestPermission(int requestClock, int machineKey, String ipAddress, String requestString)
    {
        Console.WriteLine(classNameLog + ipAddress + " asking for permission");
        //check if accessing resource
        if (!_currentlyAccessing && !_haveInterest)
        {
            Console.WriteLine(classNameLog + "I have no interest, go ahead");
            return "true";
        }
        else if (_currentlyAccessing)
        {
            //add to differed ip
            Console.WriteLine(classNameLog + "I'm currently accessing the resource, put request to waiting list");
            addMachineToDiffered(machineKey, ipAddress);
            Console.WriteLine(classNameLog + differedRequestIps);
        }
        else if (_haveInterest)
        {
            Console.WriteLine(classNameLog + "I'm currently in waiting list");
            if (requestClock < localClock.getClockValue())
            {
                Console.WriteLine(classNameLog + "Request clock is earlier. You can go ahead before me");
                return "true";
            }
            else
            {
                Console.WriteLine(classNameLog + "Request clock is later. Please line up after me");
                addMachineToDiffered(machineKey, ipAddress);
                Console.WriteLine(classNameLog + differedRequestIps);
            }
        }

        //sync clock with clock from message
        Console.WriteLine(classNameLog + "Local clock " + localClock.getClockValue() + " request clock " + requestClock);
        localClock.syncClock(requestClock);
        Console.WriteLine(classNameLog + "Local clock now " + localClock.getClockValue());

        return "false";
    }

    private String doResourceAccess()
    {
        Console.WriteLine(classNameLog + "Accessing resource");
        _currentlyAccessing = true;
        //clear permission, do request access
        if (_wantWrite)
        {
            Console.WriteLine(classNameLog + "Writing to shared resource");
            _myString = resourceHandler.generateRandomString();
            Console.WriteLine(classNameLog + "Generated random string " + _myString);
            _finalString = resourceHandler.appendNewString(myIp, masterIp, _myString);
            Console.WriteLine(classNameLog + "Shared resource value now " + _finalString);
        }
        else
        {
            _finalString = resourceHandler.readNewString(myIp, masterIp);
            Console.WriteLine(classNameLog + "Reading resource with value " + _finalString);
        }

        //send ok to all machines in differedRequestIps
        sendOkResponse();
        _haveInterest = false;
        _currentlyAccessing = false;
        Console.WriteLine(classNameLog + "Flags cleared to false");

        return _finalString;
    }

    private void sendOkResponse()
    {
        //increment clock by 1
        Console.WriteLine(classNameLog + "Sending signal that already finished accessing resource");
        Console.WriteLine(classNameLog + differedRequestIps);
        localClock.incrementClock();
        Console.WriteLine(classNameLog + "Local clock incremented to " + localClock.getClockValue());

        foreach (KeyValuePair<int, String> entry in differedRequestIps)
        {
            String ipAddress = entry.Value;
            int machineKey = entry.Key;
            Console.WriteLine(classNameLog + "Giving permission to " + ipAddress + " with machine number " + machineKey);

            Object[] parameters = new Object[] { localClock.getClockValue(), myKey, myIp };
            Task<String> response = XmlRpcHelper.SendToOneMachineAsync(ipAddress, "Request.receivePermission", parameters);

            Console.WriteLine(classNameLog + "Removing machine from differed request :" + machineKey);
            removeMachineFromDiffered(machineKey);
            Console.WriteLine(classNameLog + differedRequestIps);
        }
    }

    #region Machine Array Management
    private void removeMachineFromExpected(int machineKey)
    {
        Console.WriteLine(classNameLog + "Remove machine key from expected request list => " + machineKey);
        if (expectedRequestIps.ContainsKey(machineKey))
        {
            expectedRequestIps.Remove(machineKey);
        }
    }

    private void addMachineToDiffered(int machineKey, String ipAddress)
    {
        Console.WriteLine(classNameLog + "Add machine key to differed request list => " + machineKey);
        if (!differedRequestIps.ContainsKey(machineKey))
        {
            differedRequestIps.Add(machineKey, ipAddress);
        }
    }

    private void removeMachineFromDiffered(int machineKey)
    {
        Console.WriteLine(classNameLog + "Remove machine key from differed request list => " + machineKey);
        if (differedRequestIps.ContainsKey(machineKey))
        {
            differedRequestIps.Remove(machineKey);
        }
    }
    #endregion
}

