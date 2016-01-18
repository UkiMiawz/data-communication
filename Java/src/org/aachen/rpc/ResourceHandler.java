package org.aachen.rpc;

import java.util.Random;

public class ResourceHandler {
	
	private static String classNameLog = "ResourceHandler : ";

	/*======= MASTER SIDE ========*/
	
	/**
	 * Read shared string in server
	 * @param ipAddress caller ip address
	 * @return shared string value
	 */
	public String getString(String ipAddress){
		System.out.println(classNameLog + "Read resource request from " + ipAddress);
		return JavaWsServer.getSharedString();
	}
	
	/**
	 * Write new string to shared string in server
	 * @param ipAddress caller ip address
	 * @return shared string value
	 */
	public String setString(String newString, String ipAddress){
		System.out.println(classNameLog + "Write resource request from " + ipAddress + " with new string " + newString);
		return JavaWsServer.setSharedString(newString);
	}
	
	/*====== RANDOM STRING GENERATOR ======*/
	/***
	 * Generate random string to append to shared string
	 * @return random string generated
	 */
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
	
	/*============= SLAVE SIDE ==============*/
	
	/***
	 * Service for slave to write to shared string in master
	 * @param myIp current machine ip address
	 * @param masterIp master ip address
	 * @param myString current machine random generated string to be appended
	 * @return shared string value
	 */
	public String appendNewString(String myIp, String masterIp, String myString){
		
		System.out.println(classNameLog + "Append string " + myString);
		String currentString = "";
		String finalString = "";
		
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
			finalString = (String) XmlRpcHelper.SendToOneMachine(masterIp, "Resource.setString", params);
			System.out.println(classNameLog + "String appended in master, shared string value now " + finalString);
		} else {
			//generate random string of 10 chars and append
			finalString = setString(currentString + myString, myIp);
			System.out.println(classNameLog + "String appended to me as master, shared string value now " + finalString);
		}
		
		return finalString;
	}
		
	/***
	 * Service for slave to read shared string in master
	 * @param myIp current machine ip address
	 * @param masterIp master ip address
	 * @return shared string value
	 */
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
