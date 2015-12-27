package org.aachen.rpc;
import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.InetAddress;
import java.net.URL;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class HelloClient {
	
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
	
	private static void sayHello(){
		try {
			Object[] params = new Object[] { ip.getHostAddress() };
			String response = (String) client.execute("HelloWorld.hello", params);
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
        		System.out.println("Enter ip address, or 'exit' to quit: ");
                String ip =  reader.readLine();
                if ("exit".equals(ip))
                {
                    keepRunning = false;
                }
                else
                {
                	connect(ip);
                	sayHello();
                }
        	} catch (Exception e) {
    			e.printStackTrace();
    		}
       
        }
	}
}