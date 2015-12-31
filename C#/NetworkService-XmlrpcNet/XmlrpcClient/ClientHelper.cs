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
        Console.WriteLine("1. Display network hashmap");
        Console.WriteLine("2. Display master node");
        Console.WriteLine("3. Add current machine to network");
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

