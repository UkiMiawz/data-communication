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
			Object response = client.execute("HelloWorld.returnKeyMap", params);
			machines = Helper.convertMapResponseToMachinesTreeMap(response);
			System.out.println("Machines value now " + machines);
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
                	connect("172.16.1.102");
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
                } else {
                	System.out.println("Command " + command + " not recognized");
                }
        	} catch (Exception e) {
    			e.printStackTrace();
    		}
       
        }
	}
}