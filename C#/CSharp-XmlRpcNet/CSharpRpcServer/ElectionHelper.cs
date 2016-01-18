using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ElectionHelper
{
    private static String classNameLog = "ElectionHelper : ";

    public String leaderElection(String ip)
    {

        Console.WriteLine(classNameLog + "Leader election on ip " + ip);
        //get machines
        Console.WriteLine(classNameLog + "Get machines");
        Dictionary<int, String> machines = CSharpRpcServer.getMachines();
        Console.WriteLine(machines);

        //get own ip and priority
        String myIp = CSharpRpcServer.getMyIpAddress();
        int myPriority = CSharpRpcServer.getMyPriority();
        Console.WriteLine(classNameLog + "My IP =>" + myIp + " My Priority => " + myPriority);

        Console.WriteLine(classNameLog + "Start Bully algorithm");
        Bully bullyGenerator = new Bully(machines);
        bool iGaveUp = bullyGenerator.holdElection(myPriority);

        if (!iGaveUp)
        {
            //I'm the master, send message to all
            int newKeyMaster = myPriority;
            String newLeaderIp = CSharpRpcServer.setMaster(newKeyMaster);
            Console.WriteLine(classNameLog + "New Leader IP =>" + newLeaderIp + " New Leader Priority => " + newKeyMaster);

            Object[] parameters = new Object[] { newKeyMaster };
            XmlRpcHelper.SendToAllMachines(machines, "Election.setNewLeader", parameters);
            Console.WriteLine(classNameLog + "New leader notification send to all");
        }

        //send new master to everyone
        return CSharpRpcServer.getIpMaster();
    }

    /****
	 * Receiving end for new leader notification
	 * @param keyMaster - new master key
	 * @return notification info
	 */
    public String setNewLeader(int keyMaster)
    {
        String newMaster = CSharpRpcServer.setMaster(keyMaster);
        return "Leader set to machine with IP : " + newMaster;
    }

    /***
	 * Method to check for machine availability and the trigger leader election on machine
	 * @param ip - requester ip
	 * @return true because if this method is available means this machine is available
	 */
    public bool checkLeaderValidity(String ip)
    {
        Console.WriteLine(classNameLog + ip + " asking for leader election. I am available. Returning true.");
        return true;
    }
}

