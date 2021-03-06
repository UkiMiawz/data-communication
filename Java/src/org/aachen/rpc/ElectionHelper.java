package org.aachen.rpc;
import java.util.TreeMap;

public class ElectionHelper {
	
	private static String classNameLog = "ElectionHelper : ";
	
	/***
	 * Trigger leader election in current machine
	 * @param ip IP of caller machine
	 * @return new master if not giving up
	 */
	public String leaderElection(String ip) { 
		
		System.out.println(classNameLog + "Leader election on ip " + ip);
		//get machines
		System.out.println(classNameLog + "Get machines");
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		System.out.println(machines);
		
		//get own ip and priority
		String myIp = JavaWsServer.getMyIpAddress();
		int myPriority = JavaWsServer.getMyPriority();
		System.out.println(classNameLog + "My IP =>" + myIp + " My Priority => " + myPriority);
		
		System.out.println(classNameLog + "Start Bully algorithm");
		Bully bullyGenerator = new Bully(machines);
		boolean iGaveUp = bullyGenerator.holdElection(myPriority);
		
		if(!iGaveUp){
			//I'm the master, send message to all
	    	int newKeyMaster = myPriority;
			String newLeaderIp = JavaWsServer.setMaster(newKeyMaster);
			System.out.println(classNameLog + "New Leader IP =>" + newLeaderIp + " New Leader Priority => " + newKeyMaster);
			
			Object[] params = new Object[] { newKeyMaster };
			XmlRpcHelper.SendToAllMachines(machines, "Election.setNewLeader", params);
			System.out.println(classNameLog + "New leader notification send to all");
		}
		
		//send new master to everyone
		return JavaWsServer.getIpMaster();
	}
	
	/****
	 * Receiving end for new leader notification
	 * @param keyMaster - new master key
	 * @return notification info
	 */
	public String setNewLeader(int keyMaster){
		String newMaster = JavaWsServer.setMaster(keyMaster);
		return "Leader set to machine with IP : " + newMaster;
	}
	
	/***
	 * Method to check for machine availability and the trigger leader election on machine
	 * @param ip - requester ip
	 * @return true because if this method is available means this machine is available
	 */
	public boolean checkLeaderValidity(String ip){
		System.out.println(classNameLog + ip + " asking for leader election. I am available. Returning true.");
		return true;
	}
}
