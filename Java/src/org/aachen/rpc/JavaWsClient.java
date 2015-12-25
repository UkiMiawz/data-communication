package org.aachen.rpc;
import java.net.InetAddress;
import java.net.URL;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class JavaWsClient {
	
	private static XmlRpcClient client;
	private static InetAddress ip;
	
	private static void connect(String ipAddress){
		try {
			
			System.out.println("XML-RPC Client call to : http://localhost:1090/xmlrpc/xmlrpc");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://localhost:1090/xml-rpc-example/xmlrpc"));
			client = new XmlRpcClient();
			client.setConfig(config);
			ip = InetAddress.getLocalHost();
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
	
	private static void joinNetwork(){
		try {
			   	Object[] params = new Object[] { ip.getHostAddress() };
			   	//send message to other machine and the machine will send back their details
				String response = (String) client.execute("RegisterHandler.joinNetwork", params);
				System.out.println("Message : " + response);
			} catch (Exception e) {
				e.printStackTrace();
			}
	}
	
	private static void registerSelf(){
		try {
			
		   	Object[] params = new Object[] { ip.getHostAddress() };
			//register to self
			String response = (String) client.execute("RegisterHandler.addNewMachine", params);
			System.out.println("Message : " + response);
			
		} catch (Exception e) {
			e.printStackTrace();
		}
	}
		
	public static void main (String [] args) {   
		connect("localhost");
		joinNetwork();
		registerSelf();
	}
}
