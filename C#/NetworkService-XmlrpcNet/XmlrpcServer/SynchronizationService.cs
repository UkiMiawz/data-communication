using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class LamportClock
{
    private int currentTime;

    public LamportClock()
    {
        currentTime = 0;
    }

    public void UpdateLamportClock(int inputTime = 0)
    {
        int maxLamportClock = Math.Max(inputTime, currentTime);
        currentTime = maxLamportClock + 1;
    }

    public int getCurrentTime()
    {
        return currentTime;
    }
}

public class RicartAgrawala
{

}

public static class MEStatus
{
    public const string InProgress = "In Progress";
    public const string Available = "Available";
}

public class MutualExclusion
{
    private Dictionary<int, string> QueueList;
    private string CurrentStatus;
    string ServerUrlStart = "http://";
    string ServerUrlEnd = ":1090/networkServer.rem";


    public MutualExclusion()
    {
        QueueList = new Dictionary<int, string>();
        CurrentStatus = MEStatus.Available;
    }

    public string TryToAccess(string ipAddress)
    {
        if (CurrentStatus == MEStatus.InProgress || QueueList.Count > 0)
        {
            QueueList.Add(QueueList.Keys.Max() + 1, ipAddress);
            QueueList.OrderBy(x => x.Key);
            CurrentStatus = MEStatus.InProgress;
            return MEStatus.InProgress;
        }

        // Since no one accessing the critical part, the requester get the priviledge.
        AccessingTheCriticalPart();
        return MEStatus.Available;
    }

    public void AccessingTheCriticalPart()
    {
        CurrentStatus = MEStatus.InProgress;
    }

    public void FinishAccessing()
    {
        CurrentStatus = MEStatus.Available;
        if (QueueList.Count > 0)
        {
            // Asynchronously send reply to first queue.
            string nextQueue = QueueList.Values.First();
            QueueList.Remove(QueueList.Keys.First());
            doMENextQueue(nextQueue);
        }
    }

    private void doMENextQueue(string nextIP)
    {
        INetworkServerClientProxy nextQueueProxy = XmlRpcProxyGen.Create<INetworkServerClientProxy>();
        nextQueueProxy.Url = ServerUrlStart + nextIP + ServerUrlEnd;

        IAsyncResult sendReplyResult;
        sendReplyResult = nextQueueProxy.BeginReceiveMEReply();
        while (sendReplyResult.IsCompleted == false)
        {
            return; 
        }
        try
        {
            nextQueueProxy.ReceiveMEReply();
        }
        catch (Exception ex)
        {
            // do nothing.
        }
    }

    public string GetCurrentStatus()
    {
        return CurrentStatus;
    }

    public int GetQueueOrderByIp(string ipAddress)
    {
        for (int i = 0; i < QueueList.Count; i++)
        {
            if (QueueList.ElementAt(i).Value == ipAddress)
            {
                return i + 1;
            }
        }
        return 0;
    }
}

public class RequestPackage
{
    public string methodName;
    public string parameter;
    bool IsWaitingProcess;

    public bool CreateNewRequest(string MethodName, string Parameter)
    {
        if (!IsWaitingProcess)
        {
            methodName = MethodName;
            parameter = Parameter;
            IsWaitingProcess = true;
            return true;
        }
        return false;
    }

    public void FinishSending()
    {
        methodName = string.Empty;
        parameter = string.Empty;
        IsWaitingProcess = false;
    }

    public bool getIsWaitingProcess()
    {
        return IsWaitingProcess;
    }
}