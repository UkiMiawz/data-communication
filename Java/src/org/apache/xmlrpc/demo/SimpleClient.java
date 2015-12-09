package org.apache.xmlrpc.demo;

import java.net.URL;
import org.apache.xmlrpc.client.XmlRpcClient;
import org.apache.xmlrpc.client.XmlRpcClientConfigImpl;

public class SimpleClient {

	public SimpleClient() {
		try {
			System.out.println("Try to add 2 + 3 via XML-RPC...");
			XmlRpcClientConfigImpl config = new XmlRpcClientConfigImpl();
			config.setServerURL(new URL(
					"http://127.0.0.1:8080/XmlRpc/xmlrpc"));
			XmlRpcClient client = new XmlRpcClient();
			client.setConfig(config);
			Object[] params = new Object[] { new Integer(2), new Integer(3) };
			Integer result = (Integer) client.execute("Calculator.add", params);
			System.out.println("The returned values are: " + result);
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	public static void main(String[] args) {
		new SimpleClient();
	}
}