package org.aachen.rpc;

public class Bully {
	static int coordinator;
	static int n;
    static int positionValue[] = new int[100];
    static int status[] = new int[100];
	
	public void HoldElection(int ele)
    {
        ele = ele-1;
        coordinator = ele+1;
        for(int i=0;i<n;i++)
        {
            if(positionValue[ele]<positionValue[i])
            {
                System.out.println("Election message is sent from "+(ele+1)+" to "+(i+1));
                if(status[i]==1)
                    HoldElection(i+1);
            }
        }
    }
}
