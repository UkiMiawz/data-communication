package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;

public class JavaWsClient {
	
	private static XmlRpcClient client;
	private static XmlRpcClient localClient;
	private static InetAddress ip;
	private static String masterIp;
	
	private static void startElection(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("Election.leaderElection", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static Object consumeService(Object[] params, String command, boolean isLocal){
		XmlRpcClient usingClient = new XmlRpcClient();
		if(isLocal){
			usingClient = localClient;
		} else {
			usingClient = client;
		}
		try {
			Object response = usingClient.execute(command, params);
			return response;
		} catch (XmlRpcException e) {
			e.printStackTrace();
			return null;
		}
	}
	
	private static void connectToMaster(){
		//get and connect to current master if available
		String masterIp = (String) consumeService(new Object[] { ip.getHostAddress() }, "RegisterHandler.getIpMaster", true);
		if(!masterIp.equals("localhost")){
			client = XmlRpcHelper.Connect(masterIp);
			System.out.println("Master IP now " + masterIp);
		}
		else {
			System.out.println("Still connect to localhost");
		}
		
	}
		
	public static void main (String [] args) {
		
		try {
			System.out.print("Constructor called");
			ip = InetAddress.getLocalHost();
			System.out.println("Local IP " + ip);
			client = XmlRpcHelper.Connect("localhost");
			localClient = XmlRpcHelper.Connect("localhost");
		} catch (Exception e) {
			e.printStackTrace();
			return;
		}
		
		connectToMaster();
		
		//wait for command
		BufferedReader reader = new BufferedReader(new InputStreamReader(
	            System.in));
		
		//if command need a service, try to connect to master server
        boolean keepRunning = true;
        while (keepRunning)
        {
        	System.out.println("Your ip address is " + ip.getHostAddress());
        	System.out.println("=====XMLRPC-Client Main Menu=====");        
        	System.out.println("1. Display master node's network hashmap");
        	System.out.println("2. Display localhost's network hashmap");
        	System.out.println("3. Display master node");
        	System.out.println("4. Do election");
        	System.out.println("5. Logout and shutdown server");
        	System.out.println("0. Exit");
        	
        	try{
        		System.out.println("Enter your choice: ");
                String command =  reader.readLine();
                if ("0".equals(command))
                {
                    keepRunning = false;
                } else if("1".equals(command)){
                	Object[] params = new Object[]{ip.getHostAddress()};
                	TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
                	machines.putAll((HashMap<Integer, String>)consumeService(params, "Server.getMachines", true));
                	for(Map.Entry<Integer,String> entry : machines.entrySet()) {
            				System.out.println(entry.getKey() + " " + entry.getValue());
            		}
                } else if("2".equals(command)){
                	Object[] params = new Object[]{ip.getHostAddress()};
                	TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
                	machines.putAll((HashMap<Integer, String>)consumeService(params, "Server.getMachines", false));
                	for(Map.Entry<Integer,String> entry : machines.entrySet()) {
            				System.out.println(entry.getKey() + " " + entry.getValue());
            		}
                } else if("3".equals(command)){
                	//display master ip
                	System.out.println("Master IP now : " + masterIp);
                } else if("4".equals(command)){
                	//start election on localhost
                	startElection();
                } else if("5".equals(command)){
                	//shutdown server and client
                	keepRunning = false;
                	Object[] params = new Object[] { ip };
                	XmlRpcHelper.SendToOneMachine("localhost", "Server.serverShutDownFromClient", params);
                } else
                {
                    System.out.println("Command " + command + " not recognized");
                }
        	} catch (Exception e) {
    			e.printStackTrace();
    		}
       
        }
	}
}
