using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Bully
{
    private static Dictionary<int, String> machines;
    private static int n;
    private static int[] positionValue;
    private static int timeout = 2000;
    private static String myIp;
    private static bool gaveUp = false;

    private String classNameLog = "Bully : ";

    /***
     * Bully class constructor   */
    public Bully(Dictionary<int, String> candidateMachines)
    {
        machines = candidateMachines;
        n = candidateMachines.Count();
        positionValue = new int[candidateMachines.Count()];
        for (int i = 0; i < candidateMachines.Count(); i++)
        {
            positionValue[i] = candidateMachines.Keys.ElementAt(i);
        }

        Console.WriteLine(classNameLog + "New instance of bully generator with machines " + machines);
        Console.WriteLine(classNameLog + "Key values array" + positionValue);
        myIp = CSharpRpcServer.getMyIpAddress();
    }

    //private class CallBack : AsyncCallback
    //{
    //    private String classNameLog = "callBack Bully : ";

    //    public void handleError(XmlRpcRequest arg0, Throwable arg1)
    //    {
    //        Console.WriteLine(classNameLog + "Bully Async call failed");
    //    }

    //    public void handleResult(XmlRpcRequest arg0, Object arg1)
    //    {
    //        Console.WriteLine(classNameLog + "Bully Async call success");
    //    }
    //}

    public bool holdElection(int thisMachinePriority)
    {
        try
        {

            //array position from 0, priority from 1
            int thisMachineArrayPosition = thisMachinePriority - 1;

            for (int i = 1; i < n; i++)
            {
                if (positionValue[thisMachineArrayPosition] < positionValue[i])
                {
                    Console.WriteLine(classNameLog + "Election message is sent from " + (positionValue[thisMachineArrayPosition + 1]) + " to " + (positionValue[i + 1]));
                    String nextBiggerPriorityIpAddress = machines.ElementAt(i).Value;

                    ServerStatusCheck ssc = new ServerStatusCheck();

                    if (ssc.isServerUp(nextBiggerPriorityIpAddress, 1090, 500))
                    {
                        //send message to bigger value machines to hold election
                        holdElection(i + 1);
                        Object[] parameters = new Object[] { myIp };
                        try
                        {
                            //test if bigger node able to do election
                            bool response = (bool)XmlRpcHelper.SendToOneMachine(nextBiggerPriorityIpAddress, "Election.checkLeaderValidity", parameters);
                            if (response)
                            {
                                //gave up election
                                Console.WriteLine(classNameLog + "Someone bigger answer, I gave up");
                                gaveUp = true;
                                //trigger election in bigger node
                                //XmlRpcHelper.SendToOneMachineAsync(nextBiggerPriorityIpAddress, "Election.leaderElection", parameters, new CallBack());
                                //XmlRpcClient client = XmlRpcHelper.Connect(nextBiggerPriorityIpAddress);
                            }
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(classNameLog + "Machine " + positionValue[i] + "gave up, continue election");
                        }
                    }
                }
            }

            return gaveUp;

        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong while electing master : {0}", ex.Message);
            return true;
        }
    }
}



