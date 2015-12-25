package org.aachen.rpc;

import java.net.InetAddress;
import java.util.HashMap;
import java.util.TreeMap;

public class Bully {
	
	private Integer master;
	private TreeMap<Integer, String> machines;
	private int n;
    private Integer positionValue[] = new Integer[100];
    private int timeout = 3000;
    
    Bully(TreeMap<Integer, String> machines){
    	this.machines = machines;
    	this.n = machines.size();
    	positionValue = machines.keySet().toArray(new Integer[100]);
    }
    
    public Integer getMaster(){
    	return master;
    }
	
	public void holdElection(int initiator)
    {
		try {
			
			initiator = initiator-1;
	        master = initiator+1;
	        for(Integer i=1;i<n;i++)
	        {
	            if(positionValue[initiator]<positionValue[i])
	            {
	                System.out.println("Election message is sent from "+(initiator+1)+" to "+(i+1));
	                if(InetAddress.getByName(machines.get(i)).isReachable(timeout))
	                    holdElection(i+1);
	            }
	        }
	        
        } catch (Exception exception) {
			System.out.println("Something went wrong while electing master : ");
			exception.printStackTrace();
		}
    }
}
