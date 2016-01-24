package org.aachen.rpc;

public class LogicalClock {
	
	/*Lamport clock*/
	private int clockValue;
	private static String classNameLog = "Logical clock : ";
	
	/***
	 * Get int value of clock
	 * @return clock value
	 */
	public int getClockValue(){
		System.out.println(classNameLog + "Return clock value " + clockValue);
		return clockValue;
	}
	
	/***
	 * Set clock value to passed value
	 * @param newClock new value to be changed to
	 * @return clock value
	 */
	public int setLocalClock(int newClock){
		System.out.println(classNameLog + "Set clock value from " + clockValue + " to " + newClock);
		clockValue = newClock;
		return clockValue;
	}
	
	/***
	 * Increment clock value by 1
	 * @return clock value
	 */
	public int incrementClock(){
		System.out.println(classNameLog + "Increment clock value " + clockValue);
		clockValue++;
		return clockValue;
	}
	
	/**
	 * Sync clock with incoming request clock
	 * @param requestClock incoming request clock value
	 * @return clock value
	 */
	public int syncClock(int requestClock){
		System.out.println(classNameLog + "Sync clock value " + clockValue + " with clock value " + requestClock);
		
		if(clockValue < requestClock + 1){
			clockValue = requestClock + 1;
		}
		
		System.out.println(classNameLog + "Clock value after sync " + clockValue);
		return clockValue;
	}
}
