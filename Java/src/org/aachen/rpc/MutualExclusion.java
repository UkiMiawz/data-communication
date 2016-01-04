package org.aachen.rpc;

public class MutualExclusion {
	//safety
	//at most 1 process is at the critical section
	//liveness
	//every request will be granted access eventually
	//ordering
	//every request granted according to the order
	
	public int semaphore = 1; //max 1 process
	
	//enter
	public void waitProcess(){
		while(true){
			if(semaphore > 0)
				semaphore --;
			break;
		}
	}
	
	//exit
	public void signalProcess(){
		semaphore ++;
	}

}
