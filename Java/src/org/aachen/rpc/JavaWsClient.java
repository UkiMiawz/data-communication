package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.util.Arrays;
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
	
	/***
	 * Start election from client
	 */
	private static void startElection(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) localClient.execute("Election.leaderElection", params);
			System.out.println("Election Result : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	/***
	 * Consume service from client side
	 */
	private static Object consumeService(Object[] params, String command, boolean isLocal){
		System.out.println("Consume service " + command + " with params " + Arrays.deepToString(params));
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
	
	/***
	 * Establish connection to current master
	 */
	private static void connectToMaster(){
		//get and connect to current master if available
		masterIp = (String) consumeService(new Object[] { ip.getHostAddress() }, "RegisterHandler.getIpMaster", true);
		if(!masterIp.equals("localhost")){
			client = XmlRpcHelper.Connect(masterIp);
			System.out.println("Master IP now " + masterIp);
		}
		else {
			System.out.println("Still connect to localhost");
		}
		
	}
	
	/***
	 * Start mutual exclusion process
	 * @param isCentralized To choose whether mutual exclusion using centralized or ricart
	 */
	private static void StartMutualExclusion(boolean isCentralized){
		try {
			String response;
			
			//call mutual exclusion to write
			Object[] params = new Object[]{ true, false };
			if(isCentralized){
				System.out.println("Start centralized mutual exclusion");
				response = (String)consumeService(params, "RequestCentral.startMessage", true);
				System.out.println("Resource value now :" + response);
			} else {
				System.out.println("Start ricart agrawala mutual exclusion");
				response = (String)consumeService(params, "Request.startMessage", true);
				System.out.println("Resource value now :" + response);
			}
			
			System.out.println("Start waiting process");
			for(int i=0; i<10; i++){
				System.out.println(i);
				Thread.sleep(1000);
			}
			
			//call mutual exclusion to read
			params = new Object[]{ false, false };
			if(isCentralized){
				System.out.println("Start centralized mutual exclusion read");
				response = (String)consumeService(params, "RequestCentral.startMessage",true);
				System.out.println("Resource value now :" + response);
			} else {
				System.out.println("Start ricart agrawala mutual exclusion read");
				response = (String)consumeService(params, "Request.startMessage",true);
				System.out.println("Resource value now :" + response);
			}
			
			String[] parts = response.split(";");
			System.out.println("Split array value " + Arrays.deepToString(parts));
			
			if(parts[1].equals("1")){
				System.out.println("!!!!My string is in the final string!!!!");
			} else {
				System.out.println("!!!!My string is not in the final string!!!!");
			}
			
			System.out.println("Final String: " + parts[0]);
			
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
		
	/***
	 * Main client process
	 */
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
        	System.out.println("5. Ricart Agrawala Mutual Exclusion");
        	System.out.println("6. Centralized Mutual Exclusion");
        	//TODO Rejoin Network
        	System.out.println("7. Rejoin Network");
        	System.out.println("99. Logout and shutdown server");
        	System.out.println("0. Exit");
        	
        	try{
        		System.out.println("Enter your choice: ");
                String command =  reader.readLine();
                if ("0".equals(command))
                {
                    //exit the application
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
                } else if("99".equals(command)){
                	//shutdown server and client
                	keepRunning = false;
                	Object[] params = new Object[] { ip };
                	XmlRpcHelper.SendToOneMachine("localhost", "Server.serverShutDownFromClient", params);
                } else if("5".equals(command)){
                	StartMutualExclusion(false);
                } else if("6".equals(command)){
                	StartMutualExclusion(true);
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
