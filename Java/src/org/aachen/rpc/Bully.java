package org.aachen.rpc;

import java.net.InetAddress;
import java.util.TreeMap;

import org.apache.xmlrpc.XmlRpcRequest;
import org.apache.xmlrpc.client.AsyncCallback;
import org.apache.xmlrpc.client.XmlRpcClient;

public class Bully {
	
	private static TreeMap<Integer, String> machines;
	private static int n;
    private static Integer positionValue[] = new Integer[100];
    private static String myIp;
    private static boolean gaveUp = false;
    
    private String classNameLog = "Bully : ";
    
    /***
     * Bully class constructor
     * @param machines machines list in the network
     */
    Bully(TreeMap<Integer, String> machines){
    	this.machines = machines;
    	this.n = machines.size();
    	positionValue = machines.keySet().toArray(new Integer[100]);
    	System.out.println(classNameLog + "New instance of bully generator with machines " + machines);
    	System.out.println(classNameLog + "Key values array" + positionValue);
    	myIp = JavaWsServer.getMyIpAddress();
    }
    
    private class CallBack implements AsyncCallback {
    	private String classNameLog = "callBack Bully : ";

    	@Override
    	public void handleError(XmlRpcRequest arg0, Throwable arg1) {
    		System.out.println(classNameLog + "Bully Async call failed");
    	}

    	@Override
    	public void handleResult(XmlRpcRequest arg0, Object arg1) {
    		System.out.println(classNameLog + "Bully Async call success");
    	}
    }
	
    /***
     * Do election
     * @param thisMachinePriority current trigger machine priority
     * @return status whether giving up or not
     */
	public boolean holdElection(int thisMachinePriority)
    {
		try {
			
			//array position from 0, priority from 1
			int thisMachineArrayPosition = thisMachinePriority - 1;
			
	        for(Integer i=1;i<n;i++)
	        {
	            if(positionValue[thisMachineArrayPosition]<positionValue[i])
	            {
	                System.out.println(classNameLog + "Election message is sent from "+(positionValue[thisMachineArrayPosition+1])+" to "+(positionValue[i+1]));
	                String nextBiggerPriorityIpAddress = machines.get(i);
	                //send message to bigger value machines to hold election
                	Object[] params = new Object[]{ myIp };
                	try{
                		//test if bigger node able to do election
                		boolean response = (boolean)XmlRpcHelper.SendToOneMachine(nextBiggerPriorityIpAddress, "Election.checkLeaderValidity", params);
                		if(response){
                			//gave up election
                			System.out.println(classNameLog + "Someone bigger answer, I gave up");
                			gaveUp = true;
                			//trigger election in bigger node
                			XmlRpcHelper.SendToOneMachineAsync(nextBiggerPriorityIpAddress, "Election.leaderElection", params, new CallBack());
                		}
                	} catch (Exception exception){
                		System.out.println(classNameLog + "Machine " + positionValue[i] + "gave up, continue election");
                	}
	                
	            }
	        }
	        
	        return gaveUp;
	        
        } catch (Exception exception) {
			System.out.println("Something went wrong while electing master : ");
			exception.printStackTrace();
			return true;
		}
    }
}
