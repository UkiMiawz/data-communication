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

	private static String classNameLog = "XmlRpcHelper : ";	
	private static int timeout = 3000;
	
	/***
	 * Create a xml rpc client to connect to a specific remote machine
	 * @param ipAddress IP Address of remote machine to connect
	 * @return return created xml rpc client
	 */
	public static XmlRpcClient Connect(String ipAddress){
		try {
			System.out.println(classNameLog + "Connecting to " + ipAddress);
			XmlRpcClient client;
			System.out.println(classNameLog + "XML-RPC Client call to : http://" + ipAddress + ":1090/xmlrpc/xmlrpc");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://" + ipAddress + ":1090/xml-rpc-example/xmlrpc"));
			client = new XmlRpcClient();
			client.setConfig(config);
			return client;
			
		} catch (MalformedURLException e) {
			e.printStackTrace();
			System.out.println(classNameLog + "URL is not valid");
			return null;
		}
	}
	
	/***
	 * Trigger remote machine service async-ly
	 * @param ipAddress machine ip address
	 * @param command service to be called from remote machine
	 * @param params parameters for service calling
	 * @param callback async callback 
	 * @return
	 */
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
				System.out.println(classNameLog +  "Message: " + response);
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
	
	/***
	 * Trigger remote machine service no async
	 * @param ipAddress machine ip address
	 * @param command service to be called from remote machine
	 * @param params parameters for service calling
	 * @return
	 */
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
				System.out.println(classNameLog + "Message: " + response);
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
	
	/***
	 * Trigger service in several remote machines
	 * @param machines machine list
	 * @param command service to be called from remote machine
	 * @param params parameters for service calling
	 * @return
	 */
	public static void SendToAllMachines(TreeMap<Integer, String> machines, String command, Object[] params){
		
		int success = 0;
		
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			String ipAddress = entry.getValue();
			
			try {
				//don't send to self
				String myIpAddress = InetAddress.getLocalHost().getHostAddress();
				if(ipAddress.equals(myIpAddress)){
					System.out.println(classNameLog + "Machine is my own machine");
				} else {
					if (InetAddress.getByName(ipAddress).isReachable(timeout)){
						System.out.println(classNameLog + "Command " + command + " Contacting priority " + entry.getKey() + " => " + ipAddress);
						  
						  //check if machine is on
						  Object response = (Object)SendToOneMachine(ipAddress, command, params);
						  System.out.println(classNameLog + response);
						  success += 1;
					} else {
						System.out.println(classNameLog + "Machine is not active");
					}
				}
				
			} catch (UnknownHostException e) {
				e.printStackTrace();
				System.out.println(classNameLog + "IP is not valid");
			} catch (IOException e) {
				e.printStackTrace();
				System.out.println(classNameLog + "String is not valid");
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		
		System.out.println("Finished sending to all machines, success call " + success);
	}
	
	/***
	 * Trigger service in several remote machines async-ly
	 * @param machines machine list
	 * @param command service to be called from remote machine
	 * @param params parameters for service calling
	 * @param callback async callback 
	 * @return
	 */
	public static void SendToAllMachinesAsync(TreeMap<Integer, String> machines, String command, Object[] params, AsyncCallback callback){
		
		int success = 0;
		
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			String ipAddress = entry.getValue();
			
			try {
				//don't send to self and check if machine alive
				String myIpAddress = InetAddress.getLocalHost().getHostAddress();
				if (!ipAddress.equals(myIpAddress) && InetAddress.getByName(ipAddress).isReachable(timeout)){
					System.out.println(classNameLog + "Command " + command + " Contacting priority " + entry.getKey() + " => " + ipAddress);
					  String response = (String)SendToOneMachineAsync(ipAddress, command, params, callback);
					  System.out.println(classNameLog + response);
					  success += 1;
				} else {
					System.out.println(classNameLog + "Machine is not active or my own machine");
				}
			} catch (UnknownHostException e) {
				e.printStackTrace();
				System.out.println(classNameLog + "IP is not valid");
			} catch (IOException e) {
				e.printStackTrace();
				System.out.println(classNameLog + "String is not valid");
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
		
		System.out.println(classNameLog + "Finished sending to all machines, success call " + success);
	}
}
