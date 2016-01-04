package org.aachen.rpc;

import java.util.ArrayList;
import java.util.List;
import java.util.TreeMap;

public class LogicalClock {
	
	/*Vector clock*/
	private List<Integer> serverClock;
	private int myNumber;
	
	LogicalClock(){
		myNumber = 0;
		serverClock = new ArrayList<Integer>();
	}
	
	private void updateMyClock(){
		int myLastNumber = serverClock.get(myNumber) + 1;
		serverClock.set(myNumber, myLastNumber);
	}
	
	public void updateClockMember(int counter){
		int clockCounter = serverClock.size();
		if(clockCounter < counter){
			for(int i=counter; i==clockCounter; i--){
				serverClock.add(0);
			}
		}
	}
	
	public void updateMyNumber(int newNumber){
		myNumber = newNumber;
	}
	
	public List<Integer> updateClockByIndex(int nodeNumber, int newValue){
		serverClock.set(nodeNumber - 1, newValue);
		return serverClock;
	}
	
	public List<Integer> addNewNode(){
		serverClock.add(0);
		return serverClock;
	}
	
	public List<Integer> updateClockFromMessage(int nodeNumber, List<Integer> requestClock){
		//increment my number
		updateMyClock();
		//compare node value in current clock and message clock
		if(serverClock.get(nodeNumber) < requestClock.get(nodeNumber)){
			updateClockByIndex(nodeNumber, requestClock.get(nodeNumber));
		}
		return serverClock;
	}
	
	public String returnEarliestEvent(TreeMap<String, List<Integer>> requestMap){
		List<Integer> lastBiggestTime;
		String lastBiggestIp = "";
		
		//TODO sorting
		
		return lastBiggestIp;
	}
}
