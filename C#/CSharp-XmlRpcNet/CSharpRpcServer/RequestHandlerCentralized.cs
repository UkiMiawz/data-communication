using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections;


public class RequestHandlerCentralized
{
    private static List<Request> queue = new List<Request>();
    private static String classNameLog = "RequestHandlerCentralized : ";
    private static ResourceHandler resourceHandler;

    private static String masterIp = CSharpRpcServer.getIpMaster();
    private static String myIp = CSharpRpcServer.getMyIpAddress();
    private int myKey = CSharpRpcServer.getMyPriority();

    private static bool currentlyAccessing = false;
    private static bool wantWrite = false;
    private static bool haveInterest = false;

    private static String finalString = "";
    private static String myString = "";

    /**
	 * class to handle async call back for centralized mutual exclusion
	 * @author ukimiawz
	 *
	 */
    //private class CallBack : AsyncCallback
    //{

    //    private String classNameLog = "callBack Centralized : ";

    //    @Override
    //    public void handleError(XmlRpcRequest arg0, Throwable arg1)
    //    {
    //        Console.WriteLine(classNameLog + "Centralized Async call failed");
    //    }

    //    @Override
    //    public void handleResult(XmlRpcRequest arg0, Object arg1)
    //    {
    //        Console.WriteLine(classNameLog + "Centralized Async call success");
    //    }
    //}

    /*==================== SLAVE SIDE ===================*/

    /***
     * 
     * @param wantWrite - indicates whether the request is for write or read
     * @return
     */
    public String startMessage(bool localWantWrite, bool isSignal)
    {
        Console.WriteLine(classNameLog + "Start mutual exclusion process");
        masterIp = CSharpRpcServer.getIpMaster();
        myIp = CSharpRpcServer.getMyIpAddress();
        myKey = CSharpRpcServer.getMyPriority();
        Console.WriteLine(classNameLog + "Master IP =>" + masterIp);
        Console.WriteLine(classNameLog + "My IP => " + myIp + " My key => " + myKey);

        if (!isSignal)
        {
            //start signal from client, inform others
            //contact all machines to start
            Dictionary<int, String> machines = CSharpRpcServer.getMachines();
            Console.WriteLine(classNameLog + "Contacting all nodes " + machines);
            Object[] parameters = new Object[] { true };
            XmlRpcHelper.SendToAllMachinesAsync(machines, GlobalMethodName.requestCentralStartMessage, parameters);
        }

        Console.WriteLine(classNameLog + "Initiating resource handler. Want to write => " + localWantWrite);
        resourceHandler = new ResourceHandler();
        wantWrite = localWantWrite;

        return sendRequest(localWantWrite);
    }

    public String sendRequest(bool localWantWrite)
    {
        //send request to master
        Console.WriteLine(classNameLog + "Send request to access resource to master");

        if (masterIp != myIp)
        {
            Object[] parameter = new Object[] { myIp, "Test string" };
            Task<string> receiveRequestAnswer = XmlRpcHelper.SendToOneMachineAsync(masterIp, GlobalMethodName.requestCentralReceiveRequest, parameter);
        }
        else
        {
            Console.WriteLine(classNameLog + "I am master, start async on self");
            //Thread a = new Thread(()-> {receiveRequest(myIp, "RequestCentral.wantAccess")});
            object[] parameter = new object[] { myIp, "Test string" };            
            Thread receiveRequestThreadResult = new Thread(new ParameterizedThreadStart(receiveRequestThread));
            receiveRequestThreadResult.Start(parameter);
        }

        //set have interest to trigger waiting
        haveInterest = true;

        //wait until all finished
        Console.WriteLine(classNameLog + "Wait until process finished");
        while (haveInterest)
        {
            try
            {
                Console.WriteLine(classNameLog + "have interest " + haveInterest);
                Console.Write(".");
                Thread.Sleep(500);
            }
            catch (ThreadInterruptedException e)
            {
                // TODO Auto-generated catch block
                //execption
            }
        }

        if (localWantWrite)
        {
            //return written string
            Console.WriteLine(classNameLog + "Written String => " + myString);
            return myString;
        }

        int containMyString = 0;
        if (finalString.Contains(myString))
        {
            Console.WriteLine(classNameLog + "!!!!My string is in the final string!!!!");
            containMyString = 1;
        }

        Console.WriteLine(classNameLog + "Read process finished, final result => " + finalString + ";" + containMyString);
        return finalString + ";" + containMyString;
    }

    /***
	 * Master signal permission to access resource
	 * @param requestIp
	 * @param requestString
	 */
    private void getPermissionThread(object parameters)
    {
        try
        {
            object[] threadParameter = (object[])parameters;
            getPermission(threadParameter[0].ToString(), threadParameter[1].ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(classNameLog + ":" + ex.Message);
        }
    }

    public void getPermission(String requestIp, String requestString)
    {
        Console.WriteLine(classNameLog + "Server gave permission to access resource");
        doResourceAccess();
        //notify master that finished access

        haveInterest = false;
        currentlyAccessing = false;
        Console.WriteLine(classNameLog + "Finished accessing resource");

        if (!masterIp.Equals(myIp))
        {
            Object[] parameter = new Object[] { myIp };
            Task<string> getPermissionResult = XmlRpcHelper.SendToOneMachineAsync(masterIp, GlobalMethodName.requestCentralFinishRequest , parameter);
        }
        else
        {
            Console.WriteLine(classNameLog + "I am master, start async on self");
            object parameter = myIp;
            Thread finishRequestResult = new Thread(new ParameterizedThreadStart(finishRequestThread));
            finishRequestResult.Start(parameter);
        }
    }

    public string doResourceAccess()
    {
        Console.WriteLine(classNameLog + "Do resource access");
        currentlyAccessing = true;
        //clear permission, do request access
        //String nowResource;

        ResourceHandler resourceHandler = new ResourceHandler();
        if (wantWrite)
        {
            Console.WriteLine(classNameLog + "Write random string to resource");
            myString = resourceHandler.generateRandomString();
            Console.WriteLine(classNameLog + "Random string generated => " + myString);
            finalString = resourceHandler.appendNewString(myIp, masterIp, myString);
        }
        else
        {
            Console.WriteLine(classNameLog + "Read shared string");
            finalString = resourceHandler.readNewString(myIp, masterIp);
        }

        Console.WriteLine(classNameLog + "Resource value now => " + finalString);
        haveInterest = false;
        currentlyAccessing = false;

        return finalString;
    }

    //============MASTER SIDE===================

    /**
     * Receive request to access shared resource
     * @param requestIp
     * @param requestString
     */

    private void receiveRequestThread(object parameters)
    {
        try
        {
            object[] threadParameter = (object[])parameters;
            receiveRequest(threadParameter[0].ToString(), threadParameter[1].ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(classNameLog + ":" + ex.Message);
        }
    }

    public void receiveRequest(String requestIp, String requestString)
    {
        Request incomingRequest = new Request(0, requestIp, requestString);
        if (currentlyAccessing)
        {
            //add to queue
            queue.Add(incomingRequest);
        }
        else
        {
            //send signal that its ok to request
            if (masterIp != myIp)
            {
                Object[] parameter = new Object[] { requestIp, requestString };
                Task<string> getPermissionResult = XmlRpcHelper.SendToOneMachineAsync(requestIp, GlobalMethodName.requestCentralGetPermission , parameter);
            }
            else
            {
                Console.WriteLine(classNameLog + "I am master, start async on self");
                object[] parameters = new object[] { myIp, requestString };
                Thread getPermissionResult = new Thread(new ParameterizedThreadStart(getPermissionThread));
                getPermissionResult.Start(parameters);
            }
        }
    }

    /**
     * A machine accessing node is signaling that its finished
     * @param requestIp
     */
    public void finishRequestThread(object parameter)
    {
        try
        {
            finishRequest(parameter.ToString());
        }
        catch(Exception ex)
        {
            Console.WriteLine(classNameLog + ":" + ex.Message);
        }
    }

    public void finishRequest(String requestIp)
    {
        Console.WriteLine(requestIp + " finished with request");
        //access next item in queue
        if (queue.Count() != 0)
        {
            Console.WriteLine(classNameLog + "Processing next item in the list");

            //get next request and remove after it fetched
            Request nextRequest = queue[0];
            queue.RemoveAt(0);
            Console.WriteLine("Queue now: {0} ", queue);

            Console.WriteLine(classNameLog + "Processing request from IP => " + requestIp);
            if (!myIp.Equals(requestIp))
            {
                Object[] parameter = new Object[] { nextRequest.getRequestIp(), nextRequest.getRequestString() };
                XmlRpcHelper.SendToOneMachine(requestIp, GlobalMethodName.requestCentralGetPermission, parameter);
            }
            else
            {
                //processing my request
                getPermission(nextRequest.getRequestIp(), nextRequest.getRequestString());
            }

        }
        else
        {
            //no queue, clearing flag
            Console.WriteLine(classNameLog + "Queue empty, clearing access flag");
            currentlyAccessing = false;
        }
    }
}
