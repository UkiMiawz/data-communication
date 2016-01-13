package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.List;
import java.util.Map;
import java.util.Random;
import java.util.TreeMap;

public class RequestHandler {
	
	private static TreeMap<Integer, String> expectedRequestIps;
	private static TreeMap<Integer, String> differedRequestIps;
	private static LogicalClock localClock;
	private static int timeout = 200;
	private static ResourceHandler resourceHandler;
	private static String classNameLog = "RequestHandler : ";
	
	private static String masterIp;
	private static String myIp;
	private static TreeMap<Integer, String> machines;
	
	private boolean haveInterest = false;
	private boolean currentlyAccessing = false;
	private boolean wantWrite = false;
	
	private String finalString;
	private String myString;
	
	private int myKey;
	
	public String startMessage(boolean wantWrite){
		System.out.println("Start string append");
		//start sync, send messages to all nodes
		masterIp = JavaWsServer.getIpMaster();
		myIp = JavaWsServer.getMyIpAddress();
		machines = JavaWsServer.getMachines();
		myKey = JavaWsServer.getMyPriority();
		resourceHandler = new ResourceHandler();
		this.wantWrite = wantWrite;
		return sendRequest();
	}
	
	//send request
	private String sendRequest(){
		
		//increment clock by 1
		localClock.incrementClock();
		//append all machines ip to list
		expectedRequestIps.clear();
		expectedRequestIps.putAll(machines);
		haveInterest = true;
		
		//request for permission to all machines
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			String ipAddress = entry.getValue();
			int machineKey = entry.getKey();
			
			//remove me from request waiting list
			if(ipAddress == myIp){
				removeMachineFromExpected(machineKey);
			}
			
			try {
				//check if machine is on
				if (InetAddress.getByName(ipAddress).isReachable(timeout)){
					System.out.println("Asking permission from machine " + entry.getKey() + " => " + ipAddress);
					
					//ask permission to all
					String requestString = "Request.askPermission";
					Object[] params = new Object[]{localClock.getClockValue(), myKey, myIp, requestString};
					Boolean replyOk = (Boolean) XmlRpcHelper.SendToOneMachine(ipAddress, "Request.requestPermission", params);
					System.out.println("Reply :" + replyOk);
					
					if(replyOk){
						removeMachineFromExpected(machineKey);
					}
					
				} else {
					removeMachineFromExpected(machineKey);
					System.out.println("Machine is not active");
				}
			} catch (UnknownHostException e) {
				removeMachineFromExpected(machineKey);
				System.out.println("IP is not valid");
			} catch (IOException e) {
				removeMachineFromExpected(machineKey);
				System.out.println("String is not valid");
			} catch (Exception e){
				e.printStackTrace();
				removeMachineFromExpected(machineKey);
			}
		}
		
		//wait until all finished
		while(haveInterest){
			try {
				Thread.sleep(500);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		
		if(wantWrite){
			//return written string
			return myString;
		}
		
		int containMyString = 0;
		if(finalString.contains(myString))
		{
			containMyString = 1;
		}
		
		return finalString + ";" + containMyString;
	}

	// receive message for ok permission, triggering resource access when all have given permission
	public void receivePermission(int requestClock, int machineKey, String ipAddress){
		//sync clock from message
		localClock.syncClock(requestClock);
		System.out.println("Permission given from " + ipAddress);
		removeMachineFromExpected(machineKey);
		//check if all machine clear
		if(expectedRequestIps.isEmpty() && haveInterest){
			System.out.println("Permission clear, access resource");
			String nowResource;
			nowResource = doResourceAccess();
			System.out.println("Resource value now : " + nowResource);
		}
	}
	
	//receive message request for permission
	public boolean requestPermission(int requestClock, int machineKey, String ipAddress, String requestString){
		//sync clock with clock from message
		localClock.syncClock(requestClock);
		//check if accessing resource
		if(!currentlyAccessing && !haveInterest){
			return true;
		} else if(currentlyAccessing) {
			//add to differed ip
			addMachineToDiffered(machineKey, ipAddress);
		} else if(haveInterest){
			if(requestClock < localClock.getClockValue()){
				return true;
			} else {
				addMachineToDiffered(machineKey, ipAddress);
			}
		}
		
		return false;
	}
	
	private String doResourceAccess(){
		currentlyAccessing = true;
		//clear permission, do request access
		String nowResource;
		if(wantWrite){
			myString = resourceHandler.generateRandomString();
			nowResource = resourceHandler.appendNewString(myIp, masterIp, myString);
		} else {
			nowResource = resourceHandler.readNewString(myIp, masterIp); 
		}
		
		//send ok to all machines in differedRequestIps
		sendOkResponse();
		haveInterest = false;
		currentlyAccessing = false;
		
		return nowResource;
	}
	
	private void sendOkResponse(){
		//increment clock by 1
		localClock.incrementClock();
		for(Map.Entry<Integer,String> entry : differedRequestIps.entrySet()) {
			String ipAddress = entry.getValue();
			int machineKey = entry.getKey();
			
			Object[] params = new Object[]{localClock.getClockValue(), myKey, myIp};
			XmlRpcHelper.SendToOneMachine(ipAddress, "Request.receivePermission", params);
			
			System.out.println("Removing machine from differed request :" + machineKey);
			removeMachineFromDiffered(machineKey);
		}
	}
	
	private void removeMachineFromExpected(int machineKey){
		if(expectedRequestIps.containsKey(machineKey)){
			expectedRequestIps.remove(machineKey);
		}
	}
	
	private void addMachineToDiffered(int machineKey, String ipAddress){
		if(!differedRequestIps.containsKey(machineKey)){
			differedRequestIps.put(machineKey, ipAddress);
		}
	}
	
	private void removeMachineFromDiffered(int machineKey){
		if(differedRequestIps.containsKey(machineKey)){
			differedRequestIps.remove(machineKey);
		}
	}
}
