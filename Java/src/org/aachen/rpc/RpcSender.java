package org.aachen.rpc;

import java.net.URL;
import java.util.Map;
import java.util.TreeMap;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class RpcSender {
	
	public static String SendToOneMachine(String ipAddress, String command, Object[] params){
		
		try{
			//send this machine IP address and priority
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://"+ ipAddress + ":1090/xml-rpc-example/xmlrpc"));
			XmlRpcClient client = new XmlRpcClient();
			client.setConfig(config);
			String response = (String) client.execute(command, params);
			System.out.println("Message: " + response);
			return response;
			
		} catch (Exception e) {
			e.printStackTrace();
			return e.getMessage();
		}
	}
	
	public static void SendToAllMachines(TreeMap<Integer, String> machines, String command, Object[] params){
		
		for(Map.Entry<Integer,String> entry : machines.entrySet()) {
			
			  Integer priority = entry.getKey();
			  String ipAddress = entry.getValue();
			  
			  SendToOneMachine(ipAddress, command, params);

			  System.out.println("Command " + command + "Contacting priority " + priority + " => " + ipAddress);
			}
	}
}
