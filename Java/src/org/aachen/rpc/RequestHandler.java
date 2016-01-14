package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcRequest;
import org.apache.xmlrpc.client.AsyncCallback;

public class RequestHandler {
	
	private static TreeMap<Integer, String> expectedRequestIps;
	private static TreeMap<Integer, String> differedRequestIps;
	private static LogicalClock localClock;
	private static int timeout = 3000;
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
	
	/**
	 * class to handle async call back for ricart agrawala mutual exclusion
	 * @author ukimiawz
	 *
	 */
	private class CallBack implements AsyncCallback {
		private String classNameLog = "callBack Ricart Agrawala : ";

		@Override
		public void handleError(XmlRpcRequest arg0, Throwable arg1) {
			System.out.println(classNameLog + "Ricart Agrawala Async call failed");
		}

		@Override
		public void handleResult(XmlRpcRequest arg0, Object arg1) {
			System.out.println(classNameLog + "Ricart Agrawala Async call success");
		}
	}
	
	public String startMessage(boolean wantWrite){
		
		//initialize variables
		System.out.println(classNameLog + "Start mutual exclusion process");
		masterIp = JavaWsServer.getIpMaster();
		myIp = JavaWsServer.getMyIpAddress();
		machines = JavaWsServer.getMachines();
		
		myKey = JavaWsServer.getMyPriority();
		System.out.println(classNameLog + "Master IP =>" + masterIp);
		System.out.println(classNameLog + "My IP => " + myIp + " My key => " + myKey);
		
		//contact all machines to start
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		System.out.println(classNameLog + "Contacting all nodes " + machines);
		Object[] params = new Object[]{true};
		XmlRpcHelper.SendToAllMachinesAsync(machines, "Request.startMessage", params, new CallBack());
		
		System.out.println(classNameLog + "Initiating resource handler. Want to write => " + wantWrite);
		resourceHandler = new ResourceHandler();
		this.wantWrite = wantWrite;
		return sendRequest();
	}
	
	//send request
	private String sendRequest(){
		
		//increment clock by 1
		localClock.incrementClock();
		System.out.println(classNameLog + "Lamport clock value incremented to " + localClock.getClockValue());
		
		//append all machines ip to list since we need to get permissions from all machine
		expectedRequestIps.clear();
		expectedRequestIps.putAll(machines);
		haveInterest = true;
		System.out.println(classNameLog + "Expecting permissions from " + expectedRequestIps);
		
		//request for permission to all machines
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			
			String ipAddress = entry.getValue();
			int machineKey = entry.getKey();
			System.out.println(classNameLog + "Requesting permission from IP => " + ipAddress + " and machine key => " + machineKey);
			
			//remove me from request waiting list
			if(ipAddress == myIp){
				removeMachineFromExpected(machineKey);
				System.out.println(classNameLog + "Removing myself from machines list. My key " + machineKey);
			}
			
			try {
				//check if machine is on
				if (InetAddress.getByName(ipAddress).isReachable(timeout)){
					System.out.println(classNameLog + "Machine is alive. Asking permission from machine " + entry.getKey() + " => " + ipAddress);
					
					//ask permission to all
					String requestString = "Request.askPermission";
					Object[] params = new Object[]{localClock.getClockValue(), myKey, myIp, requestString};
					Boolean replyOk = (Boolean) XmlRpcHelper.SendToOneMachine(ipAddress, "Request.requestPermission", params);
					System.out.println(classNameLog + "Reply permission => " + replyOk);
					
					if(replyOk){
						System.out.println(classNameLog + "Got permission from " + machineKey + " removing machine from expected list");
						removeMachineFromExpected(machineKey);
					}
					
				} else {
					removeMachineFromExpected(machineKey);
					System.out.println(classNameLog + "Machine " + machineKey + " is not active. IP Address " + ipAddress);
				}
			} catch (UnknownHostException e) {
				removeMachineFromExpected(machineKey);
				System.out.println(classNameLog + "IP is not valid");
			} catch (IOException e) {
				removeMachineFromExpected(machineKey);
				System.out.println(classNameLog + "String is not valid");
			} catch (Exception e){
				e.printStackTrace();
				removeMachineFromExpected(machineKey);
			}
		}
		
		System.out.println(classNameLog + "Finished requesting permission");
		
		if(expectedRequestIps.isEmpty()){
			System.out.println(classNameLog + "Everybody gave permission. Start resource access");
			String resource = doResourceAccess();
			System.out.println(classNameLog + "Resource value now " + resource);
		} else {
			System.out.println(classNameLog + "Wait until process finished");
			//wait until all finished
			while(haveInterest){
				try {
					System.out.println(".");
					Thread.sleep(500);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
		}
		
		if(wantWrite){
			//return written string
			System.out.println(classNameLog + "Written String => " + myString);
			return myString;
		}
		
		int containMyString = 0;
		if(finalString.contains(myString))
		{
			containMyString = 1;
		}
		
		System.out.println(classNameLog + "Read process finished, final result => " + finalString + ";" + containMyString);
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
	
	/*==== Machine array management ====*/
	
	private void removeMachineFromExpected(int machineKey){
		System.out.println(classNameLog + "Remove machine key from expected request list => " + machineKey);
		if(expectedRequestIps.containsKey(machineKey)){
			expectedRequestIps.remove(machineKey);
		}
	}
	
	private void addMachineToDiffered(int machineKey, String ipAddress){
		System.out.println(classNameLog + "Add machine key to differed request list => " + machineKey);
		if(!differedRequestIps.containsKey(machineKey)){
			differedRequestIps.put(machineKey, ipAddress);
		}
	}
	
	private void removeMachineFromDiffered(int machineKey){
		System.out.println(classNameLog + "Remove machine key from differed request list => " + machineKey);
		if(differedRequestIps.containsKey(machineKey)){
			differedRequestIps.remove(machineKey);
		}
	}
}
