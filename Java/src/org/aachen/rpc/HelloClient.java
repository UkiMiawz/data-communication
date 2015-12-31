package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.URL;
import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class HelloClient {
	
	private static XmlRpcClient client;
	private static InetAddress ip;
	
	private static void connect(String ipAddress){
		try {
			
			System.out.println("XML-RPC Client call to : http://" + ipAddress + ":1090/xmlrpc/xmlrpc");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://" + ipAddress + ":1090/xml-rpc-example/xmlrpc"));
			client = new XmlRpcClient();
			client.setConfig(config);
			ip = InetAddress.getLocalHost();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void sayHello(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("HelloWorld.hello", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void sayHelloServer(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("Server.hello", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void tryUnexistingMethod(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("HelloWorld.nanana", params);
			System.out.println("Message : " + response);
		} catch (XmlRpcException e) {
			System.out.println("Method not available");
			e.printStackTrace();
		}
	}
	
	private static void printHashMap(){
		connect("localhost");
		Object[] params = new Object[]{ "localhost", 2 };
		TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
		
		try {
			machines.putAll((HashMap<Integer, String>)client.execute("HelloWorld.returnKeyMap", params));
			System.out.println(machines);
			for(Map.Entry<Integer,String> entry : machines.entrySet()) {
				System.out.println(entry.getKey() + " " + entry.getValue());
			}
		} catch (XmlRpcException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
		
	public static void main (String [] args) {
		
		//wait for command
		BufferedReader reader = new BufferedReader(new InputStreamReader(
	            System.in));
		
        boolean keepRunning = true;
        while (keepRunning)
        {       
        	try{
        		System.out.println("Enter command ('server' or 'local'), or 'exit' to quit: ");
                String command =  reader.readLine();
                if ("exit".equals(command))
                {
                    keepRunning = false;
                } else if("map".equals(command)){
                	printHashMap();
                } else if("local".equals(command)) {
                	System.out.println("Enter ip address: ");
                    String ip =  reader.readLine();
                	connect(ip);
                	sayHello();
                	sayHelloServer();
                	tryUnexistingMethod();
                } else if("server".equals(command)) {
                	System.out.println("Enter ip address: ");
                    String ipAddress =  reader.readLine();
                    Object[] params = new Object[] { ip.getHostAddress() };
                	JavaWsServer.testConnection(ipAddress, "HelloWorld.hello", params);
                } else if("print".equals(command)) {
                	JavaWsServer.printAllMachinesInLan();
                } else {
                	System.out.println("Command " + command + " not recognized");
                }
        	} catch (Exception e) {
    			e.printStackTrace();
    		}
       
        }
	}
}