package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.HashMap;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;
import org.apache.xmlrpc.server.XmlRpcNoSuchHandlerException;

import com.google.common.net.InetAddresses;

public class RegisterHandler {
	
	private static int timeout = 200;
	
	public String getIpMaster(String callerIp){
		System.out.println(callerIp + " requesting master key");
		String masterIp = JavaWsServer.getIpMaster();
		return masterIp;
	}
	
	public Integer getKeyMaster(String callerIp){
		System.out.println(callerIp + " requesting master ip");
		Integer masterIp = JavaWsServer.getKeyMaster();
		return masterIp;
	}
	
	public TreeMap<Integer, String> getMachines(String callerIp){
		System.out.println(callerIp + " requesting hashmap machines");
		return JavaWsServer.getMachines();
	}
	
	public static void joinNetwork(String ipAddress) {
		try{
			//get IP addresses
			
			String myIp = JavaWsServer.getMyIpAddress();
		    String subnet = ipAddress.substring(0, ipAddress.lastIndexOf('.'));
		    System.out.println("Subnet : " + subnet);
		    String ipNeighbor = null;
		    int i = 2;
		    
		    while (ipNeighbor == null && i<255){
			       String host= subnet + "." + i;
			       System.out.println("Contacting " + host);
			       
			       if (!host.equals(ipAddress) && InetAddress.getByName(host).isReachable(timeout)){
			           System.out.println(host + " is reachable");
			           Object[] params = new Object[] { ipAddress };
			           ipNeighbor = (String) XmlRpcHelper.SendToOneMachine(host, "RegisterHandler.newMachineJoin", params);
			           //check if ip neighbor is valid
			           if(!InetAddresses.isInetAddress(ipNeighbor)){
			        	   ipNeighbor = null;
			           }
			           System.out.println("Response " + ipNeighbor);
			       } else {
			    	   System.out.println(host + " is not reachable");
			       }
			       
			       i++;
			}
		    
		    System.out.println("Finish registering to neighbor");
		    
		    if(ipNeighbor != null && !ipNeighbor.equals("error")){
				//you're not alone
		    	System.out.println("I'm not alone!");
				//get master from neighbor
		    	Object[] params = new Object[]{myIp};
		    	Integer keyMaster = (Integer) XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getKeyMaster", params);
				String ipMaster = (String) XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getIpMaster", params);
				
				//get hashmap from master
				TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
				machines.putAll((HashMap<Integer, String>) XmlRpcHelper.SendToOneMachine(ipMaster, "RegisterHandler.getMachines", params));
				JavaWsServer.setMachines(machines);
				
				//set ip master
				JavaWsServer.setMaster(keyMaster);
			} else {
				System.out.println("I'm alone here. Guys....");
			}
			
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
	
	public String addNewMachine(String newIpAddress, String callerIpAddress){
		JavaWsServer.addMachineToMap(newIpAddress);
		return "From " + callerIpAddress + " new machine registered " + newIpAddress;
	}
	
	public static String removeMachine(int key){
		String removedIp = JavaWsServer.removeMachineFromMap(key);
		return "Machine removed " + removedIp;
	}
	
	public String newMachineJoin(String ipAddress){
		try{
			//tell everyone in network that new machine join
			String myIp = JavaWsServer.getMyIpAddress();
			//add new machine to map
			addNewMachine(ipAddress, myIp);
			Object[] params = new Object[]{ipAddress, myIp};
			XmlRpcHelper.SendToAllMachines(JavaWsServer.getMachines(), "RegisterHandler.addNewMachine", params);
			return myIp;
		} catch (Exception e) {
			e.printStackTrace();
			return "error";
		}
	}
}