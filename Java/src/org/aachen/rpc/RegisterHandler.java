package org.aachen.rpc;

import java.util.TreeMap;

import com.google.common.net.InetAddresses;

public class RegisterHandler {
	
	private static String classNameLog = "RegisterHandler : ";
	
	public String getIpMaster(String callerIp){
		String masterIp = JavaWsServer.getIpMaster();
		System.out.println(classNameLog + callerIp + " requesting master ip " + masterIp);
		return masterIp;
	}
	
	public Integer getKeyMaster(String callerIp){
		Integer masterKey = JavaWsServer.getKeyMaster();
		System.out.println(classNameLog + callerIp + " requesting master key " + masterKey);
		return masterKey;
	}
	
	public TreeMap<Integer, String> getMachines(String callerIp){
		System.out.println(classNameLog + callerIp + " requesting hashmap machines");
		TreeMap<Integer, String> machines = JavaWsServer.getMachines();
		System.out.println(classNameLog + machines);
		return machines;
	}
	
	/**
	 * Join a network
	 * @param ipAddress ip address of joined machine
	 * @param neighbourIp neighbour machine ip address
	 */
	public static void joinNetwork(String ipAddress, String neighbourIp) {
		System.out.println(classNameLog + ipAddress + " joining network");
		
		try{
			
			//get IP addresses
			String myIp = JavaWsServer.getMyIpAddress();
		    String subnet = ipAddress.substring(0, ipAddress.lastIndexOf('.'));
		    System.out.println(classNameLog + "Subnet : " + subnet);
		    String ipNeighbor = neighbourIp;
		    
		    int i = 2;
		    
			if(neighbourIp.isEmpty() || neighbourIp == null){
				//search for neighbor automatically
			    while (ipNeighbor == null && i<255){
				       String host= subnet + "." + i;
				       System.out.println(classNameLog + "Contacting " + host);
				       
				       if (!host.equals(ipAddress)){
				    	   try{
				    		   System.out.println(classNameLog + host + " is reachable. Checking validity");
					           Object[] params = new Object[] { ipAddress };
					           
					           ipNeighbor = (String) XmlRpcHelper.SendToOneMachine(host, "RegisterHandler.newMachineJoin", params);
					           //check if ip neighbor is valid
					           if(!InetAddresses.isInetAddress(ipNeighbor)){
					        	   ipNeighbor = null;
					           } else {
					        	   System.out.println(classNameLog + "Neighbor found. IP neighbor " + ipNeighbor);
					           }
				    	   } catch (Exception e){
				    		   System.out.println(classNameLog + host + " is not valid");
				    	   }
				           
				       } else {
				    	   System.out.println(host + " is my self");
				       }
				       i++;
				}
			} else {
				if(!neighbourIp.equals(myIp) && !neighbourIp.equals("localhost")){
					Object[] params = new Object[] { ipAddress };
					//alert neighbour that new machine wants to join
					ipNeighbor = (String) XmlRpcHelper.SendToOneMachine(neighbourIp, "RegisterHandler.newMachineJoin", params);	
				}
			}
		
		    System.out.println(classNameLog + "Finish registering to neighbor");
		    
		    //check if neighbor is indeed other machine and valid
		    if(ipNeighbor != null && !ipNeighbor.equals("error") && !ipNeighbor.equals(myIp) && !ipNeighbor.equals("localhost")){
				
		    	//you're not alone
		    	System.out.println(classNameLog + "I'm not alone! Requesting key and ip of master");
				
		    	//get master from neighbor
		    	Object[] params = new Object[]{myIp};
		    	Integer keyMaster = (Integer) XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getKeyMaster", params);
				String ipMaster = (String) XmlRpcHelper.SendToOneMachine(ipNeighbor, "RegisterHandler.getIpMaster", params);
				System.out.println(classNameLog + "Ip Master -> " + ipMaster + " Key Master -> " + keyMaster);
				
				//get hashmap from master
				TreeMap<Integer, String> machines = new TreeMap<Integer, String>();
				machines = Helper.convertMapResponseToMachinesTreeMap(XmlRpcHelper.SendToOneMachine(ipMaster, "RegisterHandler.getMachines", params));
				JavaWsServer.setMachines(machines);
				System.out.println(classNameLog + "Machines set, number of machines " + machines.size());
				
				//set ip master
				JavaWsServer.setMaster(keyMaster);
			} else {
				//alone in the network :(
				System.out.println("I'm alone here. Guys....");
			}
			
		    System.out.println(classNameLog + "Join finished. Set last priority");
			//sort machines and set last priority on server
			JavaWsServer.setLastPriority();
			
		} catch (Exception e) {
			System.out.println(classNameLog + "Captain we got problem!!");
			e.printStackTrace();
		}
	}
	
	/***
	 * Add new machine by ip address and priority
	 * @param ipAddress ip address of new machine
	 * @param priority priority of new machine
	 * @return
	 */
	public static String registerMachine(String ipAddress, int priority){
		System.out.println(classNameLog + "Register new machine " + ipAddress + " priority " + priority);
		String myIp = JavaWsServer.getMyIpAddress();
		int numberOfMachines = JavaWsServer.addMachineToMap(ipAddress, priority);
		return "From " + myIp + " You have been registered " + ipAddress + "with priority " + priority + ". Number of machines now " + numberOfMachines;
	}
	
	/***
	 * Add new machine by ip address
	 * @param newIpAddress ip address of new machine
	 * @param callerIpAddress ip address of machine that send new ip address
	 * @return
	 */
	public String addNewMachine(String newIpAddress, String callerIpAddress){
		System.out.println(classNameLog + "Add new machine " + newIpAddress + " from " + callerIpAddress);
		JavaWsServer.addMachineToMap(newIpAddress);
		return "From " + callerIpAddress + " new machine registered " + newIpAddress;
	}
	
	/***
	 * Remove machine from map by machine key
	 * @param key key of machine to be removed
	 * @return
	 */
	public String removeMachine(int key){
		System.out.println(classNameLog + "Remove machine with key " + key);
		String removedIp = JavaWsServer.removeMachineFromMap(key);
		return "Machine removed " + removedIp;
	}
	
	/***
	 * Remove machine from map by machine ip address
	 * @param ipAddress ipAddress of machine to be removed
	 * @return
	 */
	public String removeMachineIp(String ipAddress){
		System.out.println(classNameLog + "Remove machine with key " + ipAddress);
		int removedKey = JavaWsServer.removeMachineFromMap(ipAddress);
		return "Machine removed " + removedKey;
	}
	
	/***
	 * As master, get notification that a new machine is joining
	 * @param newIp ip address of new machine to join
	 * @param callerIp ip address of machine sending notification
	 * @return
	 */
	public String newMachineJoinNotification(String newIp, String callerIp){
		System.out.println(classNameLog + "New machine notification " + newIp + " from " + callerIp);
		//inform all others
		Object[] params = new Object[]{newIp, callerIp};
		System.out.println(classNameLog + "Notification, telling all machines new machine IP ");
		XmlRpcHelper.SendToAllMachines(JavaWsServer.getMachines(), "RegisterHandler.addNewMachine", params);
		addNewMachine(newIp, callerIp);
		return "Notification from " + callerIp + " Machine added " + newIp;
	}
	
	/***
	 * As neighbor, receive join request from a new machine
	 * @param ipAddress ip address of new machine that wants to join
	 * @return
	 */
	public String newMachineJoin(String ipAddress){
		System.out.println(classNameLog + "Join request from neighbor " + ipAddress);
		try{
			//tell everyone in network that new machine join
			String myIp = JavaWsServer.getMyIpAddress();
			String masterIp = JavaWsServer.getIpMaster();
			Object[] params = new Object[]{ipAddress, myIp};
			System.out.println(classNameLog + "My IP " + myIp + " Master IP " + masterIp);
			
			//check master
			if(masterIp.equals(myIp)){
				//if master is me, send to all other machines to add new machine
				//add new machine to map
				System.out.println(classNameLog + "Master is me, add new machine, send new ip");
				newMachineJoinNotification(ipAddress, myIp);
			} else {
				System.out.println(classNameLog + "I'm not master. Send notification to master");
				//inform master and let master handle
				String response = (String) XmlRpcHelper.SendToOneMachine(masterIp, "RegisterHandler.newMachineJoinNotification", params);
				System.out.println(response);
			}
			
			return myIp;
		} catch (Exception e) {
			e.printStackTrace();
			return "error";
		}
	}
}