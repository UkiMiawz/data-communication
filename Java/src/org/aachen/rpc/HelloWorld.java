package org.aachen.rpc;

import java.util.HashMap;
import java.util.Map;
import java.util.TreeMap;

/**
 * Test service with test methods
 * @author ukimiawz
 *
 */
public class HelloWorld {
	
	public static boolean haveInterest;
	
	public String hello(String ipAddress){
		System.out.println("Hello from IP : " + ipAddress);
		return "Greetings IP " + ipAddress;
	}
	
	public HashMap<String, String> returnKeyMap(String ipAddress, int priority){
		System.out.println("Hashmap request from ip " + ipAddress + " with priority " + priority);
		TreeMap<String, String> machines = new TreeMap<String, String>();
		//machines.put(priority, ipAddress);
		machines.put("1", "test1");
		machines.put("3", "test3");
		System.out.println("Machines number now " + machines.size());
		HashMap<String, String> test = new HashMap<String, String>();
		test.putAll(machines);
		return test;
	}
	
	public HashMap<String, String> returnKeyMap2(){
		System.out.println("Hashmap request");
		TreeMap<String, String> machines = new TreeMap<String, String>();
		machines.put("1", "test1");
		machines.put("3", "test3");
		System.out.println("Machines number now " + machines.size());
		HashMap<String, String> test = new HashMap<String, String>();
		test.putAll(machines);
		return test;
	}
	
	public String haveInterest(){
		System.out.println("Interest initiated");
		haveInterest = true;
		while(haveInterest){
			try {
				System.out.println(haveInterest);
				Thread.sleep(500);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
		}
		return "Interest removed";
	}
	
	public String removeInterest(){
		System.out.println("Interest removed");
		haveInterest = false;
		System.out.println(haveInterest);
		return "Interest removed";
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