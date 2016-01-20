using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;


public class NetworkPingService
{
    public string GetMyIpAddress()
    {
        string myIpAddress = string.Empty;
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress localIp in host.AddressList)
        {
            if (localIp.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                myIpAddress = localIp.ToString();
            }
        }

        return myIpAddress;
    }
}

