package org.aachen.rpc;

import java.util.Arrays;

import org.apache.xmlrpc.XmlRpcException;
import org.apache.xmlrpc.metadata.XmlRpcSystemImpl;

public class SystemHandler {
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
}
