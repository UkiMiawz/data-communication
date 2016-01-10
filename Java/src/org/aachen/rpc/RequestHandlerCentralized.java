package org.aachen.rpc;

import java.util.List;
import java.util.TreeMap;

public class RequestHandlerCentralized {

	private static List<Request> queue;
	
	private static int timeout = 200;
	private static ResourceHandler resourceHandler;
	
	private static String masterIp;
	private static String myIp;

	private boolean currentlyAccessing = false;
	private boolean wantWrite = false;
	private boolean haveInterest = false;
	
	private String finalString;
	private String myString;
	
	private int myKey;
	
	public String startMessage(boolean wantWrite){
		System.out.println("Start string append");
		masterIp = JavaWsServer.getIpMaster();
		myIp = JavaWsServer.getMyIpAddress();
		myKey = JavaWsServer.getMyPriority();
		resourceHandler = new ResourceHandler();
		this.wantWrite = wantWrite;
		return sendRequest(wantWrite);
	}
	
	public String sendRequest(boolean wantWrite){
		//send request to master
		Object[] params = new Object[]{myIp, "RequestCentral.wantAccess"};
		haveInterest = true;
		
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
	
	public void GetPermission(String requestIp, String requestString){
		doResourceAccess();
		//notify master that finished access
		Object[] params = new Object[]{ myIp };
		haveInterest = false;
		currentlyAccessing = false;
		XmlRpcHelper.SendToOneMachine(masterIp, "RequestCentral.FinishRequest", params);
	}
	
	public String doResourceAccess(){
		currentlyAccessing = true;
		//clear permission, do request access
		String nowResource;
		
		if(wantWrite){
			myString = resourceHandler.generateRandomString();
			nowResource = resourceHandler.appendNewString(myIp, masterIp, myString);
		} else {
			nowResource = resourceHandler.readNewString(myIp, masterIp); 
		}
		
		haveInterest = false;
		currentlyAccessing = false;
		
		return nowResource;
	}
	
	//============SERVER SIDE
	
	public void receiveRequest(String requestIp, String requestString){
		Request incomingRequest = new Request(0, requestIp, requestString);
		if(currentlyAccessing){
			//add to queue
			queue.add(incomingRequest);
		} else {
			//send signal that its ok to request
			Object[] params = new Object[] { requestIp, requestString };
			XmlRpcHelper.SendToOneMachine(requestIp, "RequestCentral.GetPermission", params);
		}
	}
	
	public void finishRequest(String requestIp){
		System.out.println(requestIp + " finished with request");
		//access next item in queue
		if(!queue.isEmpty()){
			Request nextRequest = queue.get(0);
			queue.remove(0);
			Object[] params = new Object[] { nextRequest.getRequestIp(), nextRequest.getRequestString() };
			XmlRpcHelper.SendToOneMachine(requestIp, "RequestCentral.GetPermission", params);
		}
	}
}
