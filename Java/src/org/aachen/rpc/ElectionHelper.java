package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.TreeMap;

public class ElectionHelper {
	
	private static int timeout = 200;
	
	public String leaderElection(String ip) {
		System.out.println("Leader election on ip " + ip);
		//get machines
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		//get master
		int keyMaster = JavaWsServer.getKeyMaster();
		String ipMaster = JavaWsServer.getIpMaster(ip);
		
		//check machines validity, remove unactive machines
		
		
		//check if IP master not LocalHost and still active
		try {
			if(!ipMaster.equals(InetAddress.getLocalHost().getHostAddress()) && InetAddress.getByName(ipMaster).isReachable(timeout))
			{
				//put master back in machines for election
				JavaWsServer.addMachineToMap(ipMaster, keyMaster);
				machines.put(keyMaster, ipMaster);
			}
		} catch (UnknownHostException e) {
			System.out.println("Leader Election: Unknown host from localhost");
		} catch (IOException e) {
			System.out.println("Leader Election: Get by name failed");
		}
		
		Bully bullyGenerator = new Bully(machines);
		bullyGenerator.holdElection(1);
		Integer newKeyMaster = bullyGenerator.getMaster();
		String newLeaderIp = JavaWsServer.setMaster(newKeyMaster);
		Object[] params = new Object[] { newLeaderIp };
		
		XmlRpcHelper.SendToAllMachines(machines, "Election.setNewLeader", params);
		
		//send new master to everyone
		return newLeaderIp;
	}
	
	public String setNewLeader(int keyMaster){
		String newMaster = JavaWsServer.setMaster(keyMaster);
		return "Leader set to machine with IP : " + newMaster;
	}

}
