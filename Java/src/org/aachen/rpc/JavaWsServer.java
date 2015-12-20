package org.aachen.rpc;

import java.util.HashMap;
import java.util.UUID;

import org.apache.xmlrpc.server.PropertyHandlerMapping;
import org.apache.xmlrpc.server.XmlRpcServer;
import org.apache.xmlrpc.server.XmlRpcServerConfigImpl;
import org.apache.xmlrpc.webserver.WebServer;

public class JavaWsServer {

	private static final int PORT = 1090;
	public static HashMap<UUID, String> machines = new HashMap<UUID, String>();
	
	public static void networkLog(String logMessage){
		System.out.println(logMessage);
	}
	
	public static int addMachineToMap(String ipAddress){
		UUID uniqueID = UUID.randomUUID();
		machines.put(uniqueID, ipAddress);
		networkLog("New Machine added with ID : " + uniqueID);
		networkLog("Total number of machines now :" + machines.size());
		return machines.size();
	}
	
	public static void main(String[] args) {
		
		try {
			System.out.println("Starting XML-RPC 3.1.1 Server on port : "+PORT+" ... ");

			WebServer webServer = new WebServer(PORT);
			XmlRpcServer xmlRpcServer = webServer.getXmlRpcServer();

			PropertyHandlerMapping propHandlerMapping = new PropertyHandlerMapping();
			propHandlerMapping.load(Thread.currentThread().getContextClassLoader(), "handler.properties");
			xmlRpcServer.setHandlerMapping(propHandlerMapping);

			XmlRpcServerConfigImpl serverConfig = (XmlRpcServerConfigImpl) xmlRpcServer.getConfig();
			webServer.start();

			System.out.println("Server started successfully...");
		} catch (Exception exception) {
			System.out.println("Something went wrong while starting the server : ");
			exception.printStackTrace();
		}
	}

}
