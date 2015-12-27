package org.aachen.rpc;

public class HelloWorld {
	
	public String hello(String ipAddress){
		System.out.println("Hello from IP : " + ipAddress);
		return "Greetings IP " + ipAddress;
	}
	
	public String helloServer(String ipAddress, String command, Object[] params){
		String response = JavaWsServer.TestConnection(ipAddress, command, params);
		System.out.println("Response from Server : " + response);
		return response;
	}
}