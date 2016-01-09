package org.aachen.rpc;

import java.util.Random;

public class ResourceHandler {

	public String getString(String ipAddress){
		System.out.println("Read resource request from " + ipAddress);
		return JavaWsServer.getSharedString();
	}
	
	public String setString(String newString, String ipAddress){
		System.out.println("Write resource request from " + ipAddress + " with new string " + newString);
		return JavaWsServer.setSharedString(newString);
	}
	
	/*====== RANDOM STRING GENERATOR ======*/
	public String generateRandomString(){
		char[] chars = "abcdefghijklmnopqrstuvwxyz".toCharArray();
		StringBuilder sb = new StringBuilder();
		Random random = new Random();
		for (int i = 0; i < 10; i++) {
		    char c = chars[random.nextInt(chars.length)];
		    sb.append(c);
		}
		String output = sb.toString();
		return output;
	}
	
	/*============= CLIENT SIDE ==============*/
	//access shared resource
	public String appendNewString(String myIp, String masterIp, String myString){
		//get current string from master
		Object[] params = new Object[]{ myIp };
		String currentString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.readString", params);
		//generate random string of 10 chars and append
		setString(myString, myIp);
		params = new Object[] { currentString + myString, myIp };
		String responseString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.setString", params);
		System.out.println("String appended, shared string value now " + responseString);
		return myString;
	}
		
	//access shared resource
	public String readNewString(String myIp, String masterIp){
		//get current string from master
		Object[] params = new Object[]{ myIp };
		String currentString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.readString", params);
		System.out.println("String read, shared string value now " + currentString);
		return currentString;
	}
}
