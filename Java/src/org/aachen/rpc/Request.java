package org.aachen.rpc;

public class Request implements java.io.Serializable {
	
	private static final long serialVersionUID = 1L;
	
	private int clock;
	public int getClock(){
		return clock;
	}
	public int setClock(int newClock){
		clock = newClock;
		return clock;
	}
	
	private String requestIp;
	public String getRequestIp(){
		return requestIp;
	}
	public String setRequestIp(String newRequestIp){
		requestIp = newRequestIp;
		return requestIp;
	}
	
	private String requestString;
	public String getRequestString(){
		return requestString;
	}
	public String setRequestString(String newRequestString){
		return requestString;
	}
	
	Request(int clock, String requestIp, String requestString){
		this.clock = clock;
		this.requestIp = requestIp;
		this.requestString = requestString;
	}
}
