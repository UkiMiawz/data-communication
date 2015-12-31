package org.aachen.rpc;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

public class JavaWsServer {
	
	private static final int PORT = 1090;
	private static int timeout = 100;
	private static WebServer webServer;
	private static ElectionHelper electionHelper;
	
	private static TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
	public static TreeMap<Integer, String> getMachines(){
		return machines;
	}
	public static void setMachines(TreeMap<Integer, String> newMachines){
		machines = newMachines;
	}
	
	private static InetAddress myIp;
	public static InetAddress getMyIp(){
		return myIp;
	}
	
	private static int myPriority;
	public static int getMyPriority(){
		return myPriority;
	}
	
	private static String myIpAddress;
	public static String getMyIpAddress(){
		return myIpAddress;
	}
	
	private static Integer keyMaster;
	public static Integer getKeyMaster(){
		return keyMaster;
	}
	
	private static String ipMaster = "localhost";
	public static String getIpMaster(String callerIp){
		System.out.println(callerIp + " requesting master ip");
		return ipMaster;
	}
	
	public static String setMaster(Integer master){
		System.out.println("Key Master : " + master);
		keyMaster = master;
		ipMaster = machines.get(keyMaster);
		System.out.println("IP Master : " + ipMaster);
		//remove master from map
		machines.remove(keyMaster);
		return ipMaster;
	}
	
	private static int lastPriority = 0;
	public static void setLastPriority(){
		if(!machines.isEmpty()){
			System.out.println("Printing all machines :");
			for(Map.Entry<Integer,String> entry : machines.entrySet()) {
				
				  Integer priority = entry.getKey();
				  String ipAddress = entry.getValue();

				  System.out.println("Priority " + priority + " with IP " + ipAddress);
			}
			int biggestPriority = machines.lastKey();
			lastPriority = biggestPriority;
		}
	}
	
	public static int addMachineToMap(String ipAddress){
		lastPriority += 1;
		machines.put(lastPriority, ipAddress);
		System.out.println("New Machine added with priority : " + lastPriority);
		System.out.println("Total number of machines now :" + machines.size());
		return machines.size();
	}
	
	public static int addMachineToMap(String ipAddress, int priority){
		machines.put(priority, ipAddress);
		System.out.println("New Machine added with priority : " + priority);
		System.out.println("Total number of machines now :" + machines.size());
		return machines.size();
	}
	
	public static String removeMachineFromMap(int key){
		String machineIp = machines.get(key);
		machines.remove(key);
		return machineIp;
	}
	
	public static void serverShutDown(){
		Object[] params = new Object[] { myPriority };
		XmlRpcHelper.SendToAllMachines(machines, "RegisterHandler.removeMachine", params);
	}
	
	public void serverShutDownFromClient(String ip){
		System.out.println(ip + " client ask to shutdown server");
		Object[] params = new Object[] { myPriority };
		XmlRpcHelper.SendToAllMachines(machines, "RegisterHandler.removeMachine", params);
		webServer.shutdown();
		System.out.println("Server shutdown");
	}
	
	public static void main(String[] args) {
		
		try {
			electionHelper = new ElectionHelper();
			
			String response = "";
			System.out.println("Starting XML-RPC 3.1.1 Server on port : "+PORT+" ... ");

			webServer = new WebServer(PORT);
			XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

			PropertyHandlerMapping propHandlerMapping = new PropertyHandlerMapping();
			propHandlerMapping.load(Thread.currentThread().getContextClassLoader(), "handler.properties");
			xmlRpcServer.setHandlerMapping(propHandlerMapping);
			
			XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer.getConfig();
			webServer.start();
			
			//assign my IP
			myIp=InetAddress.getLocalHost();
			myIpAddress = myIp.getHostAddress();
			
			//join network
			//RegisterHandler.joinNetwork(myIpAddress);
			response = RegisterHandler.addNewMachine(myIpAddress);
			System.out.println(response);
			
			//do election
			System.out.println("Ip Master " + ipMaster);
			if(ipMaster == null || ipMaster.isEmpty()){
				System.out.println("Calling for election");
				electionHelper.leaderElection(myIpAddress);
			}
			
			//wait for command
			BufferedReader reader = new BufferedReader(new InputStreamReader(
		            System.in));
			
	        boolean keepRunning = true;
	        while (keepRunning)
	        {       
	        	try{
	        		System.out.println("Enter command, or 'exit' to quit: ");
	                String command =  reader.readLine();
	                if ("exit".equals(command)) {
	                    keepRunning = false;
	                    System.out.println("Shutting down server...");
	                    serverShutDown();
	                } else if ("ip".equals(command)) {
	                	System.out.println("This machine ip :" + myIpAddress);
	                } else if ("print".equals(command)) {
	                	printAllMachinesInLan();
	                } else if("leader".equals(command)) {
	                	
	                } else {
	                    System.out.println("Command " + command + " not recognized");
	                }
	        	} catch (Exception e) {
	    			e.printStackTrace();
	    		}
	        }
	        
			webServer.shutdown();
			System.out.println("Server shutdown");
			
		} catch (Exception exception) {
			System.out.println("Something went wrong while starting the server : ");
			exception.printStackTrace();
		}
		
	}
	
	public static String testConnection(String ipAddress, String command, Object[] params){
		String response = (String)XmlRpcHelper.SendToOneMachine(ipAddress, command, params);
		return response;
	}
	
	public static void printAllMachinesInLan(){
		try{
			//get IP addresses
			String ipAddress = myIp.getHostAddress();
		    String subnet = ipAddress.substring(0, ipAddress.lastIndexOf('.'));
		    System.out.println("Subnet : " + subnet);
			
		    int counter = 0;
		    long startTime = System.nanoTime();
		    for (int i=1;i<255;i++){
			       String host= subnet + "." + i;
			       if (InetAddress.getByName(host).isReachable(timeout)){
			           System.out.println(host + " is reachable");
			           counter++;
			       } else {
			    	   System.out.println(host + " is not reachable");
			       }
			}
		    
		    long endTime = System.nanoTime();
		    long duration = (endTime - startTime)/1000000;
		    System.out.println("Number of machines detected: " + counter);
		    System.out.println("Operation time in second: " + duration/1000);
		    
		    System.out.println("Finish checking");

		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	public static String hello(String ipAddress) {
		return "Server is running! Hello " + ipAddress;
	}

}
