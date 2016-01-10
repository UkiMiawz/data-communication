package org.aachen.rpc;

import java.util.ArrayList;
import java.util.List;
import java.util.TreeMap;

public class LogicalClock {
	
	/*Lamport clock*/
	private int clockValue;
	
	public int getClockValue(){
		return clockValue;
	}
	
	public int setLocalClock(int newClock){
		clockValue = newClock;
		return clockValue;
	}
	
	public int incrementClock(){
		clockValue++;
		return clockValue;
	}
	
	public int syncClock(int requestClock){
		if(clockValue < requestClock + 1){
			clockValue = requestClock + 1;
		}
		return clockValue;
	}
}
