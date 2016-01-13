package org.aachen.rpc;

import java.util.ArrayList;
import java.util.List;
import java.util.TreeMap;

public class LogicalClock {
	
	/*Lamport clock*/
	private int clockValue;
	private static String classNameLog = "Logical clock : ";
	
	public int getClockValue(){
		System.out.println(classNameLog + "Return clock value " + clockValue);
		return clockValue;
	}
	
	public int setLocalClock(int newClock){
		System.out.println(classNameLog + "Set clock value from " + clockValue + " to " + newClock);
		clockValue = newClock;
		return clockValue;
	}
	
	public int incrementClock(){
		System.out.println(classNameLog + "Increment clock value " + clockValue);
		clockValue++;
		return clockValue;
	}
	
	public int syncClock(int requestClock){
		System.out.println(classNameLog + "Sync clock value " + clockValue + " with clock value " + requestClock);
		
		if(clockValue < requestClock + 1){
			clockValue = requestClock + 1;
		}
		
		System.out.println(classNameLog + "Clock value after sync " + clockValue);
		return clockValue;
	}
}
