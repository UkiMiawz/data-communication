package org.aachen.rpc;
import java.io.*;
import java.util.Scanner;
 
class Anele{
    static int n;
    static int pro[] = new int[100];
    static int sta[] = new int[100];
    static int co;
     
    public static void main(String args[])throws IOException
    {
        System.out.println("Enter the number of process");
        Scanner in = new Scanner(System.in);
        n = in.nextInt();
         
        int i,j,k,l,m;
         
        for(i=0;i<n;i++)
        {
            System.out.println("For process "+(i+1)+":");
            System.out.println("Status:");
            sta[i]=in.nextInt();
            System.out.println("Priority");
            pro[i] = in.nextInt();
        }
         
        System.out.println("Which process will initiate election?");
        int ele = in.nextInt();
         
        elect(ele);
        System.out.println("Final coordinator is "+co);
    }
     
    static void elect(int ele)
    {
        ele = ele-1;
        co = ele+1;
        for(int i=0;i<n;i++)
        {
            if(pro[ele]<pro[i])
            {
                System.out.println("Election message is sent from "+(ele+1)+" to "+(i+1));
                if(sta[i]==1)
                    elect(i+1);
            }
        }
    }
}

/**
output
Enter the number of process
7
For process 1:
Status:
1
Priority
1
For process 2:
Status:
1
Priority
2
For process 3:
Status:
1
Priority
3
For process 4:
Status:
1
Priority
4
For process 5:
Status:
1
Priority
5
For process 6:
Status:
1
Priority
6
For process 7:
Status:
0
Priority
7
Which process will initiate election?
4
Election message is sent from 4 to 5
Election message is sent from 5 to 6
Election message is sent from 6 to 7
Election message is sent from 5 to 7
Election message is sent from 4 to 6
Election message is sent from 6 to 7
Election message is sent from 4 to 7
Final coordinator is 6
 
 * /