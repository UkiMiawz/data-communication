package org.aachen.rpc;

import java.net.InetAddress;
import java.net.URL;
import java.util.TreeMap;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class RegisterHandler {
	
	private static int timeout = 3000;
	
	public static void joinNetwork(String ipAddress) {
		try{
			//get IP addresses
			
		    String subnet = ipAddress.substring(0, ipAddress.lastIndexOf('.'));
		    System.out.println("Subnet : " + subnet);
			
		    int counter = 0;
		    long startTime = System.nanoTime();
		    
		    for (int i=1;i<255;i++){
			       String host= subnet + "." + i;
			       if (InetAddress.getByName(host).isReachable(timeout)){
			           System.out.println(host + " is reachable");
			           try{
			    			//connect to the machine
			    			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			    			config.setServerURL(new URL(
			    					"http://"+ host + ":1090/xml-rpc-example/xmlrpc"));
			    			XmlRpcClient client = new XmlRpcClient();
			    			client.setConfig(config);
			    			
			    			//register to self
			    			Object[] params = new Object[] { ipAddress };
			    			String response = (String) client.execute("RegisterHandler.newMachineJoin", params);
			    			System.out.println("Message : " + response);
			    			counter++;
			    		} catch (Exception e) {
				    		e.printStackTrace();
				    	}
			       } else {
			    	   System.out.println(host + " is not reachable");
			       }
			}
		    
		    long endTime = System.nanoTime();
		    long duration = (endTime - startTime)/1000000;
		    System.out.println("Number of machines detected: " + counter);
		    System.out.println("Operation time in second: " + duration/1000);
		    
		    System.out.println("Finish contacting all active machines");
			
			//sort machines and set last priority on server
			JavaWsServer.setLastPriority();
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public static String RegisterMachine(String ipAddress, int priority){
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
		//add new machine to map
		addNewMachine(ipAddress);
		
		//get this machine priority number
		int myPriority = JavaWsServer.getMyPriority();
		
		try{
			//send this machine IP address and priority
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://"+ ipAddress + ":1090/xml-rpc-example/xmlrpc"));
			XmlRpcClient client = new XmlRpcClient();
			client.setConfig(config);
			
			//register self to the new machine
			InetAddress IP=InetAddress.getLocalHost();
			Object[] params = new Object[] { IP.getHostAddress(), myPriority };
			String response = (String) client.execute("RegisterHandler.RegisterMachine", params);
			System.out.println("Message : " + response);
			
		} catch (Exception e) {
			e.printStackTrace();
		}
		
	}
	
	public static String leaderElection(String ip) {
		System.out.println("Leader election on ip " + ip);
		//get machines
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		Bully bullyGenerator = new Bully(machines);
		bullyGenerator.holdElection(1);
		Integer keyMaster = bullyGenerator.getMaster();
		String newLeaderIp = JavaWsServer.setMaster(keyMaster);
		Object[] params = new Object[] { newLeaderIp };
		
		RpcSender.SendToAllMachines(machines, "RegisterHandler.setNewLeader", params);
		
		//send new master to everyone
		return newLeaderIp;
	}
	
	public String setNewLeader(int keyMaster){
		String newMaster = JavaWsServer.setMaster(keyMaster);
		return "Leader set to machine with IP : " + newMaster;
	}
}