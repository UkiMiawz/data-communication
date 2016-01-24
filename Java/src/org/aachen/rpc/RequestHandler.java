package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcRequest;
import org.apache.xmlrpc.client.AsyncCallback;

public class RequestHandler {
	
	private static TreeMap<Integer, String> expectedRequestIps = new TreeMap<Integer, String>();
	private static TreeMap<Integer, String> differedRequestIps = new TreeMap<Integer, String>();
	private static LogicalClock localClock = new LogicalClock();
	private static ResourceHandler resourceHandler = new ResourceHandler();
	private static String classNameLog = "RequestHandler : ";
	
	private static String masterIp = JavaWsServer.getIpMaster();
	private static String myIp = JavaWsServer.getMyIpAddress();
	private static int myKey = JavaWsServer.getMyPriority();
	private static TreeMap<Integer, String> machines = JavaWsServer.getMachines();
	
	private static boolean haveInterest = false;
	private static boolean currentlyAccessing = false;
	private static boolean wantWrite = false;
	
	private static String finalString = "";
	private static String myString = "";
	
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
	
	/***
	 * 
	 * @param wantWrite - indicates whether the request is for write or read
	 * @param isSignal - indicates whether the request coming from client or another server auto trigger
	 * @return
	 */
	public String startMessage(boolean wantWrite, boolean isSignal){
		
		try {
			System.out.println(classNameLog + "Wait for random amount of time");
			Thread.sleep((long)(Math.random() * 2000));
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		//initialize variables
		System.out.println(classNameLog + "Start mutual exclusion process");
		System.out.println(classNameLog + "Master IP =>" + masterIp);
		System.out.println(classNameLog + "My IP => " + myIp + " My key => " + myKey);
		
		if(!isSignal){
			//start signal from client, inform others
			//contact all machines to start
			TreeMap<Integer, String> machines = JavaWsServer.getMachines();
			System.out.println(classNameLog + "Contacting all nodes " + machines);
			Object[] params = new Object[]{wantWrite, true};
			XmlRpcHelper.SendToAllMachinesAsync(machines, "Request.startMessage", params, new CallBack());
		}
		
		System.out.println(classNameLog + "Initiating... Want to write => " + wantWrite);
		this.wantWrite = wantWrite;
		return sendRequest();
	}
	
	/***
	 * Start sending request to access shared resource
	 * @param wantWrite indicates whether the operation is write or read
	 * @return
	 */
	private String sendRequest(){
		
		//increment clock by 1
		localClock.incrementClock();
		System.out.println(classNameLog + "Lamport clock value incremented to " + localClock.getClockValue());
		
		//append all machines ip to list since we need to get permissions from all machine
		expectedRequestIps.clear();
		expectedRequestIps.putAll(machines);
		haveInterest = true;
		System.out.println(classNameLog + "Expecting permissions from " + expectedRequestIps);
		System.out.println(classNameLog + "My IP " + myIp);
		
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
				System.out.println(classNameLog + "Machine is alive. Asking permission from machine " + entry.getKey() + " => " + ipAddress);
				
				//ask permission to all
				String requestString = "Request.askPermission";
				Object[] params = new Object[]{localClock.getClockValue(), myKey, myIp, requestString};
				String replyOk = (String) XmlRpcHelper.SendToOneMachine(ipAddress, "Request.requestPermission", params);
				System.out.println(classNameLog + "Reply permission => " + replyOk);
				
				if(replyOk.equals("true") && replyOk.equals("false")){
					if(replyOk.equals("true")){
						System.out.println(classNameLog + "Got permission from " + machineKey + " removing machine from expected list");
						removeMachineFromExpected(machineKey);
					}
				} else {
					System.out.println(classNameLog + "Reply not as expected, removing machine from waiting list");
					removeMachineFromExpected(machineKey);
				}
				
			} catch (Exception e){
				System.out.println(classNameLog + "Machine is not valid, removing from expected list");
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
		
		System.out.println(classNameLog + "Read String check my string => " + myString);
		System.out.println(classNameLog + "Final String => " + finalString);
		int containMyString = 0;
		
		if(finalString.contains(myString))
		{
			System.out.println(classNameLog + "!!!!My string is in the final string!!!!");
			containMyString = 1;
		}
		
		System.out.println(classNameLog + "Read process finished, final result => " + finalString + ";" + containMyString);
		return finalString + ";" + containMyString;
	}

	/***
	 * Receive message for ok permission, triggering resource access when all have given permission
	 * @param requestClock logical clock of incoming request
	 * @param machineKey key of machine sending ok signal
	 * @param ipAddress 
	 * @return string from resource access
	 */
	public String receivePermission(int requestClock, int machineKey, String ipAddress){
		
		//sync clock from message
		System.out.println(classNameLog + "Received permission");
		System.out.println(classNameLog + "Local clock " + localClock.getClockValue() + " request clock " + requestClock);
		localClock.syncClock(requestClock);
		System.out.println(classNameLog + "Local clock now " + localClock.getClockValue());
		System.out.println("Permission given from " + ipAddress);
		removeMachineFromExpected(machineKey);
		//check if all machine clear
		if(expectedRequestIps.isEmpty() && haveInterest){
			System.out.println("Permission clear, access resource");
			String nowResource;
			nowResource = doResourceAccess();
			System.out.println("Resource value now : " + nowResource);
		}
		
		return finalString;
	}

	/***
	 * Receive message asking permission from other machine
	 * @param requestClock logical clock of incoming request
	 * @param machineKey machine key of requesting machine
	 * @param ipAddress ip address of requesting machine
	 * @param requestString request string
	 * @return
	 */
	public String requestPermission(int requestClock, int machineKey, String ipAddress, String requestString){
		System.out.println(classNameLog + ipAddress + " asking for permission");
		//check if accessing resource
		if(!currentlyAccessing && !haveInterest){
			System.out.println(classNameLog + "I have no interest, go ahead");
			return "true";
		} else if(currentlyAccessing) {
			//add to differed ip
			System.out.println(classNameLog + "I'm currently accessing the resource, put request to waiting list");
			addMachineToDiffered(machineKey, ipAddress);
			System.out.println(classNameLog + differedRequestIps);
		} else if(haveInterest){
			System.out.println(classNameLog + "I'm currently in waiting list");
			if(requestClock < localClock.getClockValue()){
				System.out.println(classNameLog + "Request clock is earlier. You can go ahead before me");
				return "true";
			} else {
				System.out.println(classNameLog + "Request clock is later. Please line up after me");
				addMachineToDiffered(machineKey, ipAddress);
				System.out.println(classNameLog + differedRequestIps);
			}
		}
		
		//sync clock with clock from message
		System.out.println(classNameLog + "Local clock " + localClock.getClockValue() + " request clock " + requestClock);
		localClock.syncClock(requestClock);
		System.out.println(classNameLog + "Local clock now " + localClock.getClockValue());
		
		return "false";
	}
	
	/***
	 * Do resource access to master machine
	 * @return return string from master
	 */
	private String doResourceAccess(){
		System.out.println(classNameLog + "Accessing resource");
		currentlyAccessing = true;
		//clear permission, do request access
		if(wantWrite){
			System.out.println(classNameLog + "Writing to shared resource");
			myString = resourceHandler.generateRandomString();
			System.out.println(classNameLog + "Generated random string " + myString);
			finalString = resourceHandler.appendNewString(myIp, masterIp, myString);
			System.out.println(classNameLog + "Shared resource value now " + finalString);
		} else {
			finalString = resourceHandler.readNewString(myIp, masterIp); 
			System.out.println(classNameLog + "Reading resource with value " + finalString);
		}
		
		//send ok to all machines in differedRequestIps
		sendOkResponse();
		haveInterest = false;
		currentlyAccessing = false;
		System.out.println(classNameLog + "Flags cleared to false");
		
		return finalString;
	}
	
	/***
	 * Send ok signal to all machines in differed list so that they can remove this machine from expected list
	 */
	private void sendOkResponse(){
		//increment clock by 1
		System.out.println(classNameLog + "Sending signal that already finished accessing resource");
		System.out.println(classNameLog + differedRequestIps);
		localClock.incrementClock();
		System.out.println(classNameLog + "Local clock incremented to " + localClock.getClockValue());
		
		for(Map.Entry<Integer,String> entry : differedRequestIps.entrySet()) {
			String ipAddress = entry.getValue();
			int machineKey = entry.getKey();
			System.out.println(classNameLog + "Giving permission to " + ipAddress + " with machine number " + machineKey);
			
			Object[] params = new Object[]{localClock.getClockValue(), myKey, myIp};
			XmlRpcHelper.SendToOneMachineAsync(ipAddress, "Request.receivePermission", params, new CallBack());
			
			System.out.println(classNameLog + "Removing machine from differed request :" + machineKey);
			removeMachineFromDiffered(machineKey);
			System.out.println(classNameLog + differedRequestIps);
		}
	}
	
	/*==== Machine array management ====*/
	
	/***
	 * Remove machine from expected list
	 * @param machineKey machine key to be removed
	 */
	private void removeMachineFromExpected(int machineKey){
		System.out.println(classNameLog + "Remove machine key from expected request list => " + machineKey);
		if(expectedRequestIps.containsKey(machineKey)){
			expectedRequestIps.remove(machineKey);
		}
	}
	
	/***
	 * Add new machine to differed list
	 * @param machineKey machine key to be added
	 * @param ipAddress ip address of new machine to be added
	 */
	private void addMachineToDiffered(int machineKey, String ipAddress){
		System.out.println(classNameLog + "Add machine key to differed request list => " + machineKey);
		if(!differedRequestIps.containsKey(machineKey)){
			differedRequestIps.put(machineKey, ipAddress);
		}
	}
	
	/***
	 * Remove machine from differed list
	 * @param machineKey machine key to be removed
	 */
	private void removeMachineFromDiffered(int machineKey){
		System.out.println(classNameLog + "Remove machine key from differed request list => " + machineKey);
		if(differedRequestIps.containsKey(machineKey)){
			differedRequestIps.remove(machineKey);
		}
	}
}
