﻿using CookComputing.XmlRpc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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