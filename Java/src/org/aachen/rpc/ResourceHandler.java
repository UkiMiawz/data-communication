package org.aachen.rpc;

public class ResourceHandler {

	public String readString(String ipAddress){
		System.out.println("Read resource request from " + ipAddress);
		return JavaWsServer.getSharedString();
	}
	
	public String setString(String newString, String ipAddress){
		System.out.println("Write resource request from " + ipAddress + " with new string " + newString);
		return JavaWsServer.setSharedString(newString);
	}
}
