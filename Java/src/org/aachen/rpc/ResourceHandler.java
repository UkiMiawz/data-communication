package org.aachen.rpc;

import java.util.Random;

public class ResourceHandler {
	
	private static String classNameLog = "ResourceHandler : ";

	public String getString(String ipAddress){
		System.out.println(classNameLog + "Read resource request from " + ipAddress);
		return JavaWsServer.getSharedString();
	}
	
	public String setString(String newString, String ipAddress){
		System.out.println(classNameLog + "Write resource request from " + ipAddress + " with new string " + newString);
		return JavaWsServer.setSharedString(newString);
	}
	
	/*====== RANDOM STRING GENERATOR ======*/
	public String generateRandomString(){
		System.out.println(classNameLog + "Generate random string");
		char[] chars = "abcdefghijklmnopqrstuvwxyz".toCharArray();
		StringBuilder sb = new StringBuilder();
		Random random = new Random();
		for (int i = 0; i < 10; i++) {
		    char c = chars[random.nextInt(chars.length)];
		    sb.append(c);
		}
		String output = sb.toString();
		System.out.println(classNameLog + "Random string generated => " + output);
		return output;
	}
	
	/*============= CLIENT SIDE ==============*/
	//access shared resource
	public String appendNewString(String myIp, String masterIp, String myString){
		
		System.out.println(classNameLog + "Append string " + myString);
		String currentString = "";
		
		//get current string from master
		
		if(!masterIp.equals(myIp)){
			Object[] params = new Object[]{ myIp };
			currentString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.getString", params);
		} else {
			currentString = getString(myIp);
		}
		
		System.out.println(classNameLog + "Current string " + currentString);
		
		if(!masterIp.equals(myIp)){
			Object[] params = new Object[] { currentString + myString, myIp };
			String responseString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.setString", params);
			System.out.println(classNameLog + "String appended in master, shared string value now " + responseString);
		} else {
			//generate random string of 10 chars and append
			String finalString = setString(myString, myIp);
			System.out.println(classNameLog + "String appended to me as master, shared string value now " + finalString);
		}
		
		return myString;
	}
		
	//access shared resource
	public String readNewString(String myIp, String masterIp){
		String currentString = "";
		System.out.println(classNameLog + "Read shared string");
		
		if(!masterIp.equals(myIp)){
			//read string from master
			System.out.println(classNameLog + "Read shared string from master => " + masterIp);
			Object[] params = new Object[]{ myIp };
			currentString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.getString", params);
		} else {
			//read from myself
			System.out.println(classNameLog + "Read shared string from me as master => " + masterIp);
			currentString = getString(myIp);
		}
		
		System.out.println(classNameLog + "String read, shared string value now " + currentString);
		return currentString;
	}
}
