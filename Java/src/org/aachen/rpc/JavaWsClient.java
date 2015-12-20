package org.aachen.rpc;
import java.net.InetAddress;
import java.net.URL;

import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class JavaWsClient {
	   public static void main (String [] args) {
		   try {
				System.out.println("XML-RPC Client call to : http://localhost:1090/xmlrpc/xmlrpc");
				XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
				config.setServerURL(new URL(
						"http://localhost:1090/xml-rpc-example/xmlrpc"));
				XmlRpcClient client = new XmlRpcClient();
				client.setConfig(config);
				InetAddress IP=InetAddress.getLocalHost();
				Object[] params = new Object[] { IP.getHostAddress() };
				String response = (String) client.execute("RegisterHandler.joinNetwork", params);
				System.out.println("Message : " + response);
				
			} catch (Exception e) {
				e.printStackTrace();
			}
	   }
	}
