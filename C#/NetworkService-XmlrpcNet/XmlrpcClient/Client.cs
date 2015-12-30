using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Net;

class _
{
    static void Main(string[] args)
    {
        bool bUseSoap = false;
        if (args.Length > 0 && args[0] == "SOAP")
            bUseSoap = true;

        HttpChannel chnl;
        if (bUseSoap)
            chnl = new HttpChannel();
        else
            chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        string localServer = "http://localhost:1090/networkServer.rem";
        string liveServer = "http://172.16.1.102:1090/networkServer.rem";

        INetworkServer netwS = (INetworkServer)Activator.GetObject(
            typeof(INetworkServer), liveServer);

        ClientObject co = new ClientObject();
        string ipAddress = co.GetClientIpAdress();

        try
        {
            //string result = netwS.AppendString("Khanh");
            //Console.WriteLine("the result is {0}", result);
            string input;

            do
            {
                input = Console.ReadLine();
                switch (input)
                {
                    case "1":                        
                        NetworkMapStruct[] response = netwS.AddNewNode(ipAddress);

                        Console.WriteLine("The Ip map");
                        foreach (NetworkMapStruct ipItem in response)
                        {
                            Console.WriteLine("{0}", ipItem.IpAddress);
                        }

                        Console.ReadLine();
                        break;

                    case "2":
                        string result = netwS.getIpMaster(ipAddress);
                        Console.WriteLine("{0}", result);
                        break;

                    case "3":
                        netwS.joinNetwork(ipAddress);
                        Console.ReadLine();
                        break;
                }
            }
            while (input != "0");            
        }
        catch (XmlRpcFaultException fex)
        {
            Console.WriteLine("Fault response {0} {1} {2}",
                fex.FaultCode, fex.FaultString, fex.Message);
            Console.ReadLine();
        }
    }

    public class ClientObject
    {
        public Dictionary<int, string> NetworkMap;

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
}
