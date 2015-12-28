package org.aachen.rpc;

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.util.TreeMap;

import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

public class JavaWsServer {

	private static final int PORT = 1090;
	private static int lastPriority = 0;
	private static TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
	private static Integer keyMaster;
	private static String ipMaster;
	private static int myKey;
	private static String myIpAddress;
	private static InetAddress myIp;
	
	public static InetAddress getIp(){
		return myIp;
	}
	
	public static int getMyPriority(){
		return myKey;
	}
	
	public static String getMyIpAddress(){
		return myIpAddress;
	}
	
	public static String setMaster(Integer master){
		keyMaster = master;
		ipMaster = machines.get(keyMaster);
		return ipMaster;
	}
	
	public static void setLastPriority(){
		int biggestPriority = machines.lastKey();
		lastPriority = biggestPriority;
	}
	
	public static void networkLog(String logMessage){
		System.out.println(logMessage);
	}
	
	public static int addMachineToMap(String ipAddress){
		machines.put(lastPriority, ipAddress);
		lastPriority += 1;
		networkLog("New Machine added with priority : " + lastPriority);
		networkLog("Total number of machines now :" + machines.size());
		return machines.size();
	}
	
	public static int addMachineToMap(String ipAddress, int priority){
		machines.put(priority, ipAddress);
		networkLog("New Machine added with priority : " + priority);
		networkLog("Total number of machines now :" + machines.size());
		return machines.size();
	}
	
	public static String removeMachineFromMap(int key){
		String machineIp = machines.get(key);
		machines.remove(key);
		return machineIp;
	}
	
	public static TreeMap<Integer, String> getMachines(){
		return machines;
	}
	
	public static void ServerShutDown(){
		Object[] params = new Object[] { myKey };
		RpcSender.SendToAllMachines(machines, "RegisterHandler.removeMachine", params);
	}
	
	public static void main(String[] args) {
		
		try {
			System.out.println("Starting XML-RPC 3.1.1 Server on port : "+PORT+" ... ");

			WebServer webServer = new WebServer(PORT);
			XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

			PropertyHandlerMapping propHandlerMapping = new PropertyHandlerMapping();
			propHandlerMapping.load(Thread.currentThread().getContextClassLoader(), "handler.properties");
			xmlRpcServer.setHandlerMapping(propHandlerMapping);

			XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer.getConfig();
			webServer.start();
			
			//assign my IP
			myIp=InetAddress.getLocalHost();
			myIpAddress = myIp.getHostAddress();
			
			//wait for command
			BufferedReader reader = new BufferedReader(new InputStreamReader(
		            System.in));
			
	        boolean keepRunning = true;
	        while (keepRunning)
	        {       
	        	try{
	        		System.out.println("Enter command, or 'exit' to quit: ");
	                String command =  reader.readLine();
	                if ("exit".equals(command))
	                {
	                    keepRunning = false;
	                    System.out.println("Shutting down server...");
	                    ServerShutDown();
	                }
	                else if ("ip".equals(command))
	                {
	                	System.out.println("This machine ip :" + myIpAddress);
	                }
	                else
	                {
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
	
	public static String TestConnection(String ipAddress, String command, Object[] params){
		String response = RpcSender.SendToOneMachine(ipAddress, command, params);
		return response;
	}

}
