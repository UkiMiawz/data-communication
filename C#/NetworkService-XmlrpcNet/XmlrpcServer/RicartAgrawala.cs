using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public static class RAStatus
{
    public const string Wanted = "Wanted";
    public const string Held = "Held";
    public const string Released = "Released";
}

public class RicartAgrawala
{
    private List<string> WaitingForAnswerList;
    private Dictionary<int, string> WaitingListOrder;
    private string MyIpAddress;
    private string AccessStatus;
    string ServerUrlStart = "http://";
    string ServerUrlEnd = ":1090/xml-rpc-example/xmlrpc";

    public RicartAgrawala(string ipAddress)
    {
        WaitingForAnswerList = new List<string>();
        WaitingListOrder = new Dictionary<int, string>();
        MyIpAddress = ipAddress;
        AccessStatus = RAStatus.Released;
    }

    public async void TryToAccessCriticalPart(List<string> networkHashmap, RequestPackage reqPack, int lamportClock )
    {
        // This one should be async as well.       
        await Task.Delay(1000);

        // step 1: Send request to everybody and wait for reply.
        sendRequestToEveryone(networkHashmap, lamportClock);

        // Step 2: After get all reply, access the critical part 
        AccessCriticalPart(reqPack, lamportClock);

        // Step 3: Finish access the critical part
        FinishAccessingResource();
    }

    private void sendRequestToEveryone(List<string> networkHashmap, int lamportClock)
    {
        AccessStatus = RAStatus.Wanted;

        WaitingForAnswerList.AddRange(networkHashmap);
        WaitingForAnswerList.Remove(MyIpAddress);

        foreach (string neighborIp in WaitingForAnswerList)
        {
            AsyncCallToOthers(neighborIp, lamportClock);
        }

        do
        {
            Thread.Sleep(10000);
        } while (WaitingForAnswerList.Count != 0);
    }

    private void AsyncCallToOthers(string neighborIp, int lamportClock)
    {
        INetworkServerClientProxy neighborProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        neighborProxy.Url = ServerUrlStart + neighborIp + ServerUrlEnd;

        // call asycn method receive RA request here!!
        string reply = neighborProxy.RAReceiveRequest(MyIpAddress, lamportClock);

        if (reply == "Ok")
        {
            WaitingForAnswerList.Remove(neighborIp);
        }
    }

    private void AccessCriticalPart(RequestPackage reqPack, int lamportClock)
    {
        AccessStatus = RAStatus.Held;

        INetworkServerClientProxy myProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        myProxy.Url = ServerUrlStart + MyIpAddress + ServerUrlEnd;

        myProxy.RAAccessCriticalPart(MyIpAddress, reqPack.methodName, reqPack.parameter);
    }

    private void FinishAccessingResource()
    {
        AccessStatus = RAStatus.Released;
        foreach (string neighborIP in WaitingListOrder.Values)
        {
            // DO something async to send record.
            INetworkServerClientProxy neighborProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
            neighborProxy.Url = ServerUrlStart + neighborIP + ServerUrlEnd;

            neighborProxy.RAGetOKReply(MyIpAddress);
        }
    }

    public void GetOkReply(string senderIp)
    {
        WaitingForAnswerList.Remove(senderIp);
    }

    public void AddAccessWaitingList(string requesterIp, int requesterLamportClock)
    {
        WaitingListOrder.Add(requesterLamportClock, requesterIp);
        WaitingListOrder.OrderBy(x => x.Key);
    }

    public string GetCurrentStatus()
    {
        return AccessStatus;
    }

    public void GetReply(string ipAddress)
    {
        WaitingForAnswerList.Remove(ipAddress);

        if (WaitingForAnswerList.Count == 0)
        {
            // Send the request.
        }
    }

}
