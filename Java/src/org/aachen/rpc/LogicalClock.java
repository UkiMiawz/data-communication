package org.aachen.rpc;

import java.util.ArrayList;
import java.util.List;
import java.util.TreeMap;

public class LogicalClock {
	
	/*Lamport clock*/
	private int localClock;
	
	public int getLocalClock(){
		return localClock;
	}
	
	public int setLocalClock(int newClock){
		localClock = newClock;
		return localClock;
	}
	
	public int incrementClock(){
		localClock++;
		return localClock;
	}
	
	public int syncClock(int requestClock){
		if(localClock < requestClock){
			localClock = requestClock;
		}
		return localClock;
	}
}
