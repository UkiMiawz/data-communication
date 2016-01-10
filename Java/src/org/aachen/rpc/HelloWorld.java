package org.aachen.rpc;

import java.util.TreeMap;

public class HelloWorld {
	
	public String hello(String ipAddress){
		System.out.println("Hello from IP : " + ipAddress);
		return "Greetings IP " + ipAddress;
	}
	
	public String helloServer(String ipAddress, String command, Object[] params){
		String response = JavaWsServer.testConnection(ipAddress, command, params);
		System.out.println("Response from Server : " + response);
		return response;
	}
	
	public TreeMap<Integer, String> returnKeyMap(String ipAddress, int priority){
		TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
		machines.put(priority, ipAddress);
		machines.put(1, "test1");
		machines.put(3, "test3");
		return machines;
	}
	
	public int returnClassRequest(Request incomingRequest){
		int clock = incomingRequest.getClock() + 1;
		incomingRequest.setClock(clock);
		return clock;
	}
	
	public int returnClassRequest(int clock, String test, String test2){
		int newClock = clock + 1;
		return newClock;
	}
}