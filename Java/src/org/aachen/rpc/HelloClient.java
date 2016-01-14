package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.URL;
import java.util.ArrayList;
import java.util.Arrays;
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
			
			System.out.println("XML-RPC Client call to : http://" + ipAddress + ":1090/xml-rpc-example/xmlrpc");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://" + ipAddress + ":1090/xml-rpc-example/xmlrpchello"));
			config.setEnabledForExtensions(true);
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
	
	public class Test{
		public int networkPriority;
		public String ipAddress;
	}
	
	private static void printHashMap(){
		Object[] params = new Object[]{ "localhost", 2 };
		TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
		
		try {
			Object[] test = (Object[])client.execute("HelloWorld.returnKeyMap", params);
			
			//TODO PARSER
			//check if contains string, if yes that mean it comes from C#
			//parse like C#
			System.out.println(Arrays.deepToString(test));
			//machines.putAll((HashMap<Integer, String>)client.execute("HelloWorld.returnKeyMap", params));
			
			HashMap<Integer, String> testMap = (HashMap<Integer, String>)test[0];
			System.out.println("Test map values : " + testMap.toString());
			System.out.println(testMap.values());
			
			machines.putAll(testMap);
			System.out.println("Machines value : " + machines.toString());
			
			//Test testString = (Test)test[0];
			//System.out.println(testString.networkPriority);
			for(Map.Entry<Integer,String> entry : machines.entrySet()) {
				System.out.println(entry.getKey().toString() + " " + entry.getValue().toString());
			}
			
			//System.out.println("Machines count :" + machines.size());
		} catch (XmlRpcException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	private static void printClassRequest(){
		connect("localhost");
		
		Request newRequest = new Request(3, "localhost", "HelloWorld.returnClassRequest");
		Object[] params = new Object[]{ newRequest };
		
		try {
			int returnRequest = (int)client.execute("HelloWorld.returnClassRequest", params);
			System.out.println("Clock " + returnRequest);
		} catch (XmlRpcException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
		
	private static void waitStart(){
		try {
			connect("localhost");
			Object[] params = new Object[] { };
			String response = (String) client.execute("HelloWorld.haveInterest", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void waitStop(){
		try {
			connect("localhost");
			Object[] params = new Object[] { };
			String response = (String) client.execute("HelloWorld.removeInterest", params);
			System.out.println("Message : " + response);
		} catch (Exception e) {
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
                } else if("waitStart".equals(command)){
                	//test several request
                	waitStart();
                } else if("waitStop".equals(command))
                {
                	waitStop();
                } else if("map".equals(command)){
                	System.out.println("Enter ip address: ");
                	String ip =  reader.readLine();
                	connect(ip);
                	printHashMap();
                } else if("request".equals(command)){
                	printClassRequest();
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