using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;


public class NetworkPingService
{
    private static List<Ping> pingers = new List<Ping>();
    private static int instances = 0;

    private static object @lock = new object();

    private static int result = 0;
    private static int timeOut = 250;
    private static List<string> resultList = new List<string>();

    private static int ttl = 5;

    public List<string> getActiveIps()
    {
        return resultList;
    }

    public string getMyIpAddress()
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

    public void DetectAllNetwork(string myIpAddress)
    {
        //string baseIP = "172.16.1.";
        string baseIP = myIpAddress.Substring(0, (myIpAddress.LastIndexOf('.') + 1));

        //Console.WriteLine("Pinging 255 destinations of D-class in {0}*", baseIP);

        CreatePingers(255);

        PingOptions po = new PingOptions(ttl, true);
        ASCIIEncoding enc = new ASCIIEncoding();
        byte[] data = enc.GetBytes("abababababababababababababababab");

        SpinWait wait = new SpinWait();
        int cnt = 1;

        Stopwatch watch = new Stopwatch();

        foreach (Ping p in pingers)
        {
            lock (@lock)
            {
                instances += 1;
            }
            p.SendAsync(string.Concat(baseIP, cnt.ToString()), timeOut, data, po);
            cnt += 1;
        }

        while (instances > 0)
        {
            wait.SpinOnce();
        }

        watch.Stop();

        DestroyPingers();

        //Console.WriteLine("Completed in {0}. Found {1} active IP-addresses", watch.Elapsed.ToString(), result);
        //Console.ReadLine();
    }

    public static void Ping_completed(object s, PingCompletedEventArgs e)
    {
        lock (@lock)
        {
            instances -= 1;
        }

        bool isValidAddress = true;
        string ipAddress = e.Reply.Address.ToString();
        int portNumber = Convert.ToInt16(ipAddress.Substring(ipAddress.LastIndexOf('.') + 1));
        if (portNumber == 1 || portNumber == 255)
        {
            isValidAddress = false;
        }

        if (e.Reply.Status == IPStatus.Success && isValidAddress)
        {
            //Console.WriteLine(string.Concat("Active IP: ", e.Reply.Address.ToString()));
            resultList.Add(e.Reply.Address.ToString());
            result += 1;
        }
        else
        {
            // inactive address
        }
    }

    private static void CreatePingers(int cnt)
    {
        for (int i = 1; i <= cnt; i++)
        {
            Ping p = new Ping();
            p.PingCompleted += Ping_completed;
            pingers.Add(p);
        }
    }

    private static void DestroyPingers()
    {
        foreach (Ping p in pingers)
        {
            p.PingCompleted -= Ping_completed;
            p.Dispose();
        }

        pingers.Clear();
    }
}

public class ServerStatusCheck
{
    public bool isServerUp(string ip, int port, int timout)
    {
        IPAddress validIP;
        if (IPAddress.TryParse(ip, out validIP))
        {
            IPEndPoint remoteEndPoint = new IPEndPoint(validIP, port);
            bool fg = TimeOutSocket.Connect(remoteEndPoint, timout);
            return fg;
        }
        else
        {
            return false;
        }
    }
}
class TimeOutSocket
{
    private static bool IsConnectionSuccessful = false;
    private static Exception socketexception;
    private static ManualResetEvent TimeoutObject = new ManualResetEvent(false);


    public static bool Connect(IPEndPoint remoteEndPoint, int timeoutMSec)
    {
        TimeoutObject.Reset();
        socketexception = null;

        string serverip = Convert.ToString(remoteEndPoint.Address);
        int serverport = remoteEndPoint.Port;
        TcpClient tcpclient = new TcpClient();

        tcpclient.BeginConnect(serverip, serverport, new AsyncCallback(CallBackMethod), tcpclient);

        if (TimeoutObject.WaitOne(timeoutMSec, false))
        {
            if (IsConnectionSuccessful)
            {
                tcpclient.Close();
                return true;
            }
            else
            {
                tcpclient.Close();
                return false;
            }
        }
        else
        {
            tcpclient.Close();
            return false;
        }
    }
    private static void CallBackMethod(IAsyncResult asyncresult)
    {
        try
        {
            IsConnectionSuccessful = false;
            TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

            if (tcpclient.Client != null)
            {
                tcpclient.EndConnect(asyncresult);
                IsConnectionSuccessful = true;
            }
        }
        catch (Exception ex)
        {
            IsConnectionSuccessful = false;
            socketexception = ex;
        }
        finally
        {
            TimeoutObject.Set();
        }
    }
}
