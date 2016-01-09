using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class ClientUI
{
    public void DisplayMainMenu(string ipAddress)
    {
        Console.Clear();
        Console.WriteLine("Your ip address is {0}", ipAddress);
        Console.WriteLine("=====XMLRPC-Client Main Menu=====");        
        Console.WriteLine("1. Display master node's network hashmap");
        Console.WriteLine("2. Display localhost's network hashmap");
        Console.WriteLine("3. Display master node");
        Console.WriteLine("4. Send message to server");
        Console.WriteLine("5. Get message from server");
        Console.WriteLine("6. Do Election");
        Console.WriteLine("7. Get local Lamport Clock");
        Console.WriteLine("8. Test Mutual Exclusion");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }
}

public class ClientObject
{
    public string GetClientIpAdress()
    {
        IPHostEntry host;
        string myIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress localIp in host.AddressList)
        {
            if (localIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                myIP = localIp.ToString();
            }
        }

        return myIP;
    }
}

