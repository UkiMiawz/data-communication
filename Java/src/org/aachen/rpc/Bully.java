package org.aachen.rpc;

import java.net.InetAddress;
import java.util.HashMap;
import java.util.TreeMap;

public class Bully {
	
	private TreeMap<Integer, String> machines;
	private int n;
    private Integer positionValue[] = new Integer[100];
    private int timeout = 3000;
    private String myIp;
    private boolean gaveUp = false;
    
    private String classNameLog = "Bully : ";
    
    Bully(TreeMap<Integer, String> machines){
    	this.machines = machines;
    	this.n = machines.size();
    	positionValue = machines.keySet().toArray(new Integer[100]);
    	System.out.println(classNameLog + "New instance of bully generator with machines " + machines);
    	System.out.println(classNameLog + "Key values array" + positionValue);
    	myIp = JavaWsServer.getMyIpAddress();
    }
	
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
	                if(InetAddress.getByName(nextBiggerPriorityIpAddress).isReachable(timeout)){
	                	//send message to bigger value machines to hold election
	                	holdElection(i+1);
	                	Object[] params = new Object[]{ myIp };
	                	try{
	                		boolean response = (boolean)XmlRpcHelper.SendToOneMachine(nextBiggerPriorityIpAddress, "Election.leaderElection", params);
	                		if(response){
	                			//gave up election
	                			System.out.println(classNameLog + "Someone bigger answer, I gave up");
	                			gaveUp = true;
	                			break;
	                		}
	                	} catch (Exception exception){
	                		System.out.println(classNameLog + "Machine " + positionValue[i] + "gave up, continue election");
	                	}
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
