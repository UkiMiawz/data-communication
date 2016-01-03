using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;

public class Bully
{
    public Dictionary<int, string> getActiveMap(Dictionary<int, string> NetworkMap)
    {
        //check alive and return new map
        Ping pingsender = new Ping();
        foreach (string ipaddress in NetworkMap.Values)
        {
            PingReply pingreply = pingsender.Send(ipaddress);
            if (pingreply.Status == IPStatus.TimedOut)
            {
                KeyValuePair<int, string> removedip = NetworkMap.FirstOrDefault(x => x.Value == ipaddress);
                NetworkMap.Remove(removedip.Key);
            }
        }

        return NetworkMap;
    }

    public string electMaster(Dictionary<int, string> NetworkMap)
    {
        string newMasterNode = "";

        Bully election = new Bully();
        //get an active Map
        Dictionary<int, string> newMap = election.getActiveMap(NetworkMap);
    
        int maxkey = newMap.Keys.Max();
        string newMasternode = newMap[maxkey];

        return newMasterNode;
    }
}