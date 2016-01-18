package org.aachen.rpc;

import java.util.Arrays;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.metadata.XmlRpcSystemImpl;

public class SystemHandler {
	private static String classNameLog = "SystemHandler : ";
	
	/**
	 * List methods function to comply with XML RPC service standard
	 * @return list of string of methods name
	 */
	public String[] listMethods(){
		try {
			XmlRpcSystemImpl test = new XmlRpcSystemImpl(JavaWsServer.getMapping());
			System.out.println("Try to print all methods : ");
			String[] listMethods;
			listMethods = test.listMethods();
			System.out.println(Arrays.asList(listMethods));
			return listMethods;
		} catch (XmlRpcException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			return new String[]{};
		}
	}
	
	/***
	 * To check whether machine still alive or not. To substitute ping if have time
	 * @return true because if this method available means machine is alive
	 */
	public boolean stillAlive(){
		//We do what we must because we can
		//For the good of all of us. Except the ones who are dead.
		System.out.println(classNameLog + "I'm doing science and I'm still alive.");
		return true;
		//That was a joke. Ha Ha. Fat Chance!
	}
}
