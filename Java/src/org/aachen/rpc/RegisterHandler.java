package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;
import org.apache.xmlrpc.server.XmlRpcNoSuchHandlerException;

public class RegisterHandler {
	
	private static int timeout = 200;
	
	public String getIpMaster(String callerIp){
		String masterIp = JavaWsServer.getIpMaster(callerIp);
		return masterIp;
	}
	
	public static void joinNetwork(String ipAddress) {
		try{
			//get IP addresses
			
		    String subnet = ipAddress.substring(0, ipAddress.lastIndexOf('.'));
		    System.out.println("Subnet : " + subnet);
			
		    int counter = 0;
		    long startTime = System.nanoTime();
		    
		    for (int i=1;i<255;i++){
			       String host= subnet + "." + i;
			       System.out.println("Contacting " + host);
			       
			       if (InetAddress.getByName(host).isReachable(timeout)){
			           System.out.println(host + " is reachable");
			           Object[] params = new Object[] { ipAddress };
			           String response = XmlRpcHelper.SendToOneMachine(host, "RegisterHandler.newMachineJoin", params);
			           System.out.println(response);
			       } else {
			    	   System.out.println(host + " is not reachable");
			       }
			}
		    
		    long endTime = System.nanoTime();
		    long duration = (endTime - startTime)/1000000;
		    System.out.println("Number of machines detected: " + counter);
		    System.out.println("Operation time in second: " + duration/1000);
		    
		    System.out.println("Finish contacting all possible machines");
			
			//sort machines and set last priority on server
			JavaWsServer.setLastPriority();
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public static String registerMachine(String ipAddress, int priority){
		String myIp = JavaWsServer.getMyIpAddress();
		int numberOfMachines = JavaWsServer.addMachineToMap(ipAddress, priority);
		return "From " + myIp + " You have been registered " + ipAddress + "with priority " + priority + ". Number of machines now " + numberOfMachines;
	}
	
	public static String addNewMachine(String ipAddress){
		JavaWsServer.addMachineToMap(ipAddress);
		return "Machine registered " + ipAddress;
	}
	
	public static String removeMachine(int key){
		String removedIp = JavaWsServer.removeMachineFromMap(key);
		return "Machine removed " + removedIp;
	}
	
	public void newMachineJoin(String ipAddress){
		try{
			//add new machine to map
			addNewMachine(ipAddress);
			
			//get this machine priority number
			int myPriority = JavaWsServer.getMyPriority();
			
			InetAddress IP=InetAddress.getLocalHost();
			Object[] params = new Object[] { IP.getHostAddress(), myPriority };
			String response = XmlRpcHelper.SendToOneMachine(ipAddress, "RegisterHandler.registerMachine", params);
			System.out.println(response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public String leaderElection(String ip) {
		System.out.println("Leader election on ip " + ip);
		//get machines
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		//get master
		int keyMaster = JavaWsServer.getKeyMaster();
		String ipMaster = JavaWsServer.getIpMaster(ip);
		
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
		
		XmlRpcHelper.SendToAllMachines(machines, "RegisterHandler.setNewLeader", params);
		
		//send new master to everyone
		return newLeaderIp;
	}
	
	public String setNewLeader(int keyMaster){
		String newMaster = JavaWsServer.setMaster(keyMaster);
		return "Leader set to machine with IP : " + newMaster;
	}
}