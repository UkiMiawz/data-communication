package org.aachen.rpc;

import java.util.ArrayList;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcRequest;
import org.apache.xmlrpc.client.AsyncCallback;

public class RequestHandlerCentralized {

	private static ArrayList<Request> queue = new ArrayList<Request>();
	private static String classNameLog = "RequestHandlerCentralized : ";
	private static ResourceHandler resourceHandler;
	
	private static String masterIp = JavaWsServer.getIpMaster();
	private static String myIp = JavaWsServer.getMyIpAddress();
	private int myKey = JavaWsServer.getMyPriority();

	private static boolean currentlyAccessing = false;
	private static boolean wantWrite = false;
	private static boolean haveInterest = false;
	
	private static String finalString = "";
	private static String myString = "";
	
	/**
	 * class to handle async call back for centralized mutual exclusion
	 * @author ukimiawz
	 *
	 */
	private class CallBack implements AsyncCallback {
		private String classNameLog = "callBack Centralized : ";

		@Override
		public void handleError(XmlRpcRequest arg0, Throwable arg1) {
			System.out.println(classNameLog + "Centralized Async call failed");
		}

		@Override
		public void handleResult(XmlRpcRequest arg0, Object arg1) {
			System.out.println(classNameLog + "Centralized Async call success");
		}
	}
	
	/*==================== SLAVE SIDE ===================*/
	
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
		
		System.out.println(classNameLog + "Start mutual exclusion process");
		System.out.println(classNameLog + "Master IP =>" + masterIp);
		System.out.println(classNameLog + "My IP => " + myIp + " My key => " + myKey);
		
		if(!isSignal){
			//start signal from client, inform others
			//contact all machines to start write and read process
			TreeMap<Integer, String> machines = JavaWsServer.getMachines();
			System.out.println(classNameLog + "Contacting all nodes " + machines);
			Object[] params = new Object[]{wantWrite, true};
			XmlRpcHelper.SendToAllMachinesAsync(machines, "RequestCentral.startMessage", params, new CallBack());
		}
		
		System.out.println(classNameLog + "Initiating resource handler. Want to write => " + wantWrite);
		resourceHandler = new ResourceHandler();
		this.wantWrite = wantWrite;
		
		return sendRequest(wantWrite);
	}
	
	/***
	 * Start sending request to access shared resource
	 * @param wantWrite indicates whether the operation is write or read
	 * @return
	 */
	public String sendRequest(boolean wantWrite){
		//send request to master
		System.out.println(classNameLog + "Send request to access resource to master");
		
		if(!masterIp.equals(myIp)){
			Object[] params = new Object[]{myIp, "RequestCentral.wantAccess"};
			XmlRpcHelper.SendToOneMachineAsync(masterIp, "RequestCentral.receiveRequest", params, new CallBack());
		} else {
			System.out.println(classNameLog + "I am master, start async on self");
			Thread a = new Thread(() -> {
			    receiveRequest(myIp, "RequestCentral.wantAccess");
			});
			a.start();
		}
		
		//set have interest to trigger waiting
		haveInterest = true;
		
		//wait until all finished
		System.out.println(classNameLog + "Wait until process finished");
		while(haveInterest){
			try {
				System.out.print(".");
				Thread.sleep(500);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
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
	 * Master signal permission to access resource
	 * @param requestIp
	 * @param requestString
	 */
	public String getPermission(String requestIp, String requestString){
		System.out.println(classNameLog + "Server gave permission to access resource");
		
		doResourceAccess();
		
		//notify master that finished access
		System.out.println(classNameLog + "Finished accessing resource");
		
		if(!masterIp.equals(myIp)){
			Object[] params = new Object[]{ myIp };
			XmlRpcHelper.SendToOneMachineAsync(masterIp, "RequestCentral.finishRequest", params, new CallBack());
		} else {
			System.out.println(classNameLog + "Finishing request with me as master, start async on self");
			Thread a = new Thread(() -> {
			    finishRequest(myIp);
			});
			a.start();
		}
		
		return finalString;
	}
	
	/***
	 * Do resource access in master machine
	 * @return string returned from master
	 */
	public String doResourceAccess(){
		System.out.println(classNameLog + "Do resource access ");
		System.out.println(classNameLog + "Want write " + wantWrite);
		currentlyAccessing = true;
		//clear permission, do request access
		
		if(wantWrite){
			System.out.println(classNameLog + "Write random string to resource");
			myString = resourceHandler.generateRandomString();
			System.out.println(classNameLog + "Random string generated => " + myString);
			finalString = resourceHandler.appendNewString(myIp, masterIp, myString);
		} else {
			System.out.println(classNameLog + "Read shared string");
			finalString = resourceHandler.readNewString(myIp, masterIp); 
		}
		
		System.out.println(classNameLog + "Want write " + wantWrite + " Resource value now => " + finalString);
		haveInterest = false;
		
		return finalString;
	}
	
	//============MASTER SIDE===================
	
	/**
	 * Receive request to access shared resource
	 * @param requestIp
	 * @param requestString
	 */
	public String receiveRequest(String requestIp, String requestString){
		System.out.println(classNameLog + "Receiving request from " + requestIp + " with request string " + requestString);
		Request incomingRequest = new Request(0, requestIp, requestString);
		System.out.println(classNameLog + "Currently accessing " + currentlyAccessing);
		if(currentlyAccessing){
			//add to queue
			queue.add(incomingRequest);
			System.out.println(classNameLog + "New request added to queue " + queue);
		} else {
			//send signal that its ok to request
			System.out.println(classNameLog + "Master IP " + masterIp + " My IP " + myIp);
			if(!requestIp.equals(myIp)){
				System.out.println(classNameLog + "Give permission to " + requestIp);
				Object[] params = new Object[] { requestIp, requestString };
				XmlRpcHelper.SendToOneMachineAsync(requestIp, "RequestCentral.getPermission", params, new CallBack());
			} else {
				System.out.println(classNameLog + "Request from myself as master, receive request, start async on self");
				Thread a = new Thread(() -> {
					getPermission(myIp, requestString);
				});
			a.start();
			}
		}
		
		return Integer.toString(queue.size());
	}
	
	/**
	 * A machine accessing node is signaling that its finished
	 * @param requestIp
	 */
	public String finishRequest(String requestIp){
		System.out.println(classNameLog + requestIp + " finished with request");
		//access next item in queue
		System.out.println(classNameLog + "Queue value now " + queue);
		if(!queue.isEmpty()){
			System.out.println(classNameLog + "Processing next item in the list");
			
			//get next request and remove after it fetched
			Request nextRequest = queue.get(0);
			queue.remove(0);
			
			System.out.println(classNameLog + "Processing request from IP => " + requestIp);
			if(!myIp.equals(requestIp)){
				//processing next IP request
				Object[] params = new Object[] { nextRequest.getRequestIp(), nextRequest.getRequestString() };
				XmlRpcHelper.SendToOneMachine(requestIp, "RequestCentral.getPermission", params);
			} else {
				//processing my request
				getPermission(nextRequest.getRequestIp(), nextRequest.getRequestString());
			}
			
		} else {
			//no queue, clearing flag
			System.out.println(classNameLog + "Queue empty, clearing access flag");
			currentlyAccessing = false;
		}
		
		return Integer.toString(queue.size());
	}
}
