package org.aachen.rpc;

import java.io.IOException;
import java.net.InetAddress;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.UnknownHostException;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.XmlRpcRequest;
import org.apache.xmlrpc.client.AsyncCallback;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;
import org.apache.xmlrpc.server.XmlRpcNoSuchHandlerException;

public class XmlRpcHelper {

	
	private static int timeout = 200;
	public static XmlRpcClient Connect(String ipAddress){
		try {
			System.out.println("Connecting to " + ipAddress);
			XmlRpcClient client;
			System.out.println("XML-RPC Client call to : http://" + ipAddress + ":1090/xmlrpc/xmlrpc");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://" + ipAddress + ":1090/xml-rpc-example/xmlrpc"));
			client = new XmlRpcClient();
			client.setConfig(config);
			return client;
			
		} catch (MalformedURLException e) {
			e.printStackTrace();
			System.out.println("URL is not valid");
			return null;
		}
	}
	
	public static String SendToOneMachineAsync(String ipAddress, String command, Object[] params, AsyncCallback callback){
		try{
			//don't send to self
			String myIpAddress = InetAddress.getLocalHost().getHostAddress();
			if(!ipAddress.equals(myIpAddress)){
				//send this machine IP address and priority
				XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
				config.setServerURL(new URL(
						"http://"+ ipAddress + ":1090/xml-rpc-example/xmlrpc"));
				XmlRpcClient client = new XmlRpcClient();
				client.setConfig(config);
				client.executeAsync(command, params, callback);
				String response = "Async call called";
				System.out.println("Message: " + response);
				return response;
			} else {
				return "Sending to self is not permitable";
			}
		} catch (XmlRpcNoSuchHandlerException e){
			return "Method not available";
		} catch (XmlRpcException e) {
			return "Connection refused";
		} catch (MalformedURLException e){
			return "Url is not valid";
		} catch (Exception e){
			e.printStackTrace();
			return e.getMessage();
		}
	}
	
	public static Object SendToOneMachine(String ipAddress, String command, Object[] params){
		
		try{
			//don't send to self
			String myIpAddress = InetAddress.getLocalHost().getHostAddress();
			if(!ipAddress.equals(myIpAddress)){
				//send this machine IP address and priority
				XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
				config.setServerURL(new URL(
						"http://"+ ipAddress + ":1090/xml-rpc-example/xmlrpc"));
				XmlRpcClient client = new XmlRpcClient();
				client.setConfig(config);
				Object response = (Object) client.execute(command, params);
				System.out.println("Message: " + response);
				return response;
			} else {
				return "Sending to self is not permitable";
			}
		} catch (XmlRpcNoSuchHandlerException e){
			return "Method not available";
		} catch (XmlRpcException e) {
			return "Connection refused";
		} catch (MalformedURLException e){
			return "Url is not valid";
		} catch (Exception e){
			e.printStackTrace();
			return e.getMessage();
		}
	}
	
	public static void SendToAllMachines(TreeMap<Integer, String> machines, String command, Object[] params){
		
		int success = 0;
		
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			String ipAddress = entry.getValue();
			
			try {
				//don't send to self
				String myIpAddress = InetAddress.getLocalHost().getHostAddress();
				if (!ipAddress.equals(myIpAddress) && InetAddress.getByName(ipAddress).isReachable(timeout)){
					System.out.println("Command " + command + " Contacting priority " + entry.getKey() + " => " + ipAddress);
					  
					  //check if machine is on
					  Object response = (Object)SendToOneMachine(ipAddress, command, params);
					  System.out.println(response);
					  success += 1;
				} else {
					System.out.println("Machine is not active");
				}
			} catch (UnknownHostException e) {
				e.printStackTrace();
				System.out.println("IP is not valid");
			} catch (IOException e) {
				e.printStackTrace();
				System.out.println("String is not valid");
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		
		System.out.print("Finished sending to all machines, success call " + success);
	}
	
public static void SendToAllMachinesAsync(TreeMap<Integer, String> machines, String command, Object[] params, AsyncCallback callback){
		
		int success = 0;
		
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			String ipAddress = entry.getValue();
			
			try {
				//don't send to self and check if machine alive
				String myIpAddress = InetAddress.getLocalHost().getHostAddress();
				if (!ipAddress.equals(myIpAddress) && InetAddress.getByName(ipAddress).isReachable(timeout)){
					System.out.println("Command " + command + " Contacting priority " + entry.getKey() + " => " + ipAddress);
					  String response = (String)SendToOneMachineAsync(ipAddress, command, params, callback);
					  System.out.println(response);
					  success += 1;
				} else {
					System.out.println("Machine is not active");
				}
			} catch (UnknownHostException e) {
				e.printStackTrace();
				System.out.println("IP is not valid");
			} catch (IOException e) {
				e.printStackTrace();
				System.out.println("String is not valid");
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		
		System.out.print("Finished sending to all machines, success call " + success);
	}
}
