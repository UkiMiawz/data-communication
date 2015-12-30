package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;

public class JavaWsClient {
	
	private static XmlRpcClient client;
	private static InetAddress ip;
	private String masterIp;
	
	JavaWsClient(){
		try {
			ip = InetAddress.getLocalHost();
			client = XmlRpcHelper.Connect("localhost");
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void startElection(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("RegisterHandler.leaderElection", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static String consumeService(Object[] params, String command){
		try {
			String response = (String) client.execute(command, params);
			return response;
		} catch (XmlRpcException e) {
			e.printStackTrace();
			return "Method not available";
		}
	}
	
	private static void connectToMaster(){
		//get and connect to current master if available
		String masterIp = consumeService(new Object[] {null}, "Server.getIpMaster");
		if(masterIp != null && !masterIp.isEmpty()){
			XmlRpcHelper.Connect(masterIp);
		}
	}
		
	public static void main (String [] args) {
		
		//wait for command
		BufferedReader reader = new BufferedReader(new InputStreamReader(
	            System.in));
		
		//if command need a service, try to connect to master server
        boolean keepRunning = true;
        while (keepRunning)
        {       
        	try{
        		System.out.println("Enter command, or 'exit' to quit: ");
                String command =  reader.readLine();
                if ("exit".equals(command))
                {
                    keepRunning = false;
                } else if("elect".equals(command)){
                	//start election on localhost
                	startElection();
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
