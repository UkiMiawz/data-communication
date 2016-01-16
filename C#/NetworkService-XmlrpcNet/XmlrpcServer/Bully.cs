using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using CookComputing.XmlRpc;

public class Bully
{
    public string doElection(string myIp, Dictionary<int, string> NetworkHashMap)
    {

        int myPriority = NetworkHashMap.FirstOrDefault(x => x.Value == myIp).Key;
        int maxPriority = NetworkHashMap.Keys.ToList().Max();

        //higher Priority node
        for (int i = myPriority + 1; i <= maxPriority; i++)
        {
            if (NetworkHashMap.Keys.ToList().Contains(i))
            {
                string nextNode = NetworkHashMap.FirstOrDefault(x => x.Key == i).Value;
                string nextNodeServer = "http://" + nextNode + ":1090/networkServer.rem";

                INetworkServerClientProxy nextNodeProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
                nextNodeProxy.Url = nextNodeServer;
                string result = nextNodeProxy.receiveElectionSignal(myIp);

                if (result == "OK")
                {
                    return string.Empty;
                }
            }
        }

        // If no higher ip, then I am the master
        return myIp;

    }


    public Dictionary<int, string> getActiveMap(Dictionary<int, string> NetworkMap)
    {
        //check alive and return new map
        Ping pingsender = new Ping();
        foreach (string ipaddress in NetworkMap.Values)
        {
            try
            {
                PingReply pingreply = pingsender.Send(ipaddress);
                if (pingreply.Status == IPStatus.TimedOut)
                {
                    KeyValuePair<int, string> removedip = NetworkMap.FirstOrDefault(x => x.Value == ipaddress);
                    NetworkMap.Remove(removedip.Key);
                }

            }
            catch (Exception ex)
            {
                NetworkMap.Remove(NetworkMap.FirstOrDefault(x => x.Value == ipaddress).Key);
            }

        }

        return NetworkMap;
    }

}