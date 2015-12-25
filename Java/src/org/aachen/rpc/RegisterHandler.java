package org.aachen.rpc;

import java.net.InetAddress;
import java.net.URL;
import java.util.TreeMap;

import org.aachen.rpc.Bully;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class RegisterHandler {
	
	private static int timeout = 3000;
	
	public static void joinNetwork(String ipAddress) {
		try{
			//get IP addresses
			InetAddress ip = JavaWsServer.getIp();
			InetAddress[] addr = InetAddress.getAllByName(ip.getCanonicalHostName());
		    
			//send IP address to all nodes
			for (int i = 0; i < addr.length; i++){
		    	//check if machine active
		    	String machineIp = addr[i].getHostAddress();
		    	if(InetAddress.getByName(machineIp).isReachable(timeout)){
		    		try{
		    			//connect to the machine
		    			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
		    			config.setServerURL(new URL(
		    					"http://"+ machineIp + ":1090/xml-rpc-example/xmlrpc"));
		    			XmlRpcClient client = new XmlRpcClient();
		    			client.setConfig(config);
		    			
		    			//register to self
		    			Object[] params = new Object[] { ip.getHostAddress() };
		    			String response = (String) client.execute("RegisterHandler.newMachineJoin", params);
		    			System.out.println("Message : " + response);
		    			
		    		} catch (Exception e) {
		    			e.printStackTrace();
		    		}
		    	}
		    }
			
			//sort machines and set last priority on server
			JavaWsServer.setLastPriority();
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public String RegisterMachine(String ipAddress, int priority){
		String myIp = JavaWsServer.getMyIpAddress();
		int numberOfMachines = JavaWsServer.addMachineToMap(ipAddress, priority);
		return "From " + myIp + " You have been registered " + ipAddress + "with priority " + priority + ". Number of machines now " + numberOfMachines;
	}
	
	public String addNewMachine(String ipAddress){
		JavaWsServer.addMachineToMap(ipAddress);
		return "Machine registered " + ipAddress;
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
	
	public String leaderElection() {
		//get machines
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		Bully bullyGenerator = new Bully(machines);
		bullyGenerator.holdElection(1);
		Integer keyMaster = bullyGenerator.getMaster();
		JavaWsServer.setMaster(keyMaster);
		return machines.get(keyMaster);
	}
}