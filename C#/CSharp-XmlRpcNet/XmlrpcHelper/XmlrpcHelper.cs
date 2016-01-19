﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing;
using System.Net.NetworkInformation;
using CookComputing.XmlRpc;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels;
using System.Collections;

public class XmlRpcHelper
{
    private static String classNameLog = "XmlRpcHelper : ";
    private static int timeout = 2000;
    private static string ServerUrlStart = "http://";
    private static string ServerUrlEnd = ":1090/xml-rpc-example/xmlrpc";

    public static ICSharpRpcClient newProxy;

    public static ICSharpRpcClient Connect(string ipaddress)
    {
        Ping pinger = new Ping();
        PingReply pr = pinger.Send(ipaddress);

        if (pr.Status == IPStatus.Success)
        {
            Console.WriteLine("Connect to {0}", ipaddress);

            newProxy = XmlRpcProxyGen.Create<ICSharpRpcClient>();
            string newProxyUrl = ServerUrlStart + ipaddress + ServerUrlEnd;
            newProxy.Url = newProxyUrl;

            Console.WriteLine("XML-RPC Client call to : http://" + ipaddress + ":1090/xmlrpc/xmlrpc");

            return newProxy;
        }
        else
        {
            return null;
        }
    }

    public static void SendToAllMachinesAsync(Dictionary<int, String> machines, String command, Object[] parameter, AsyncCallback callback)
    {
        throw new NotImplementedException();
    }

    public static Object SendToOneMachine(String ipAddress, String command, Object[] parameter)
    {
        try
        {
            //don't send to self
            NetworkPingService nps = new NetworkPingService();
            String myIpAddress = nps.GetMyIpAddress();

            if (ipAddress != myIpAddress)
            {
                newProxy = XmlRpcProxyGen.Create<ICSharpRpcClient>();
                newProxy.Url = ServerUrlStart + ipAddress + ServerUrlEnd;
                object response;
                switch (command)
                {
                    case GlobalMethodName.getMachines:
                        response = newProxy.getMachines(parameter[0].ToString());
                        break;

                    case GlobalMethodName.serverShutDownFromClient:
                        newProxy.serverShutDownFromClient();
                        response = "success";
                        break;

                    case GlobalMethodName.leaderElection:
                        response = newProxy.leaderElection(parameter[0].ToString());
                        break;

                    case GlobalMethodName.getIpMaster:
                        response = newProxy.getIpMaster(parameter[0].ToString());
                        break;

                    case GlobalMethodName.removeMachineIp:
                        response = newProxy.removeMachineIp(parameter[0].ToString());
                        break;

                    case GlobalMethodName.newMachineJoin:
                        response = newProxy.newMachineJoin(parameter[0].ToString());
                        break;

                    case GlobalMethodName.newMachineJoinNotification:
                        response = newProxy.newMachineJoinNotification(parameter[0].ToString(), parameter[1].ToString());
                        break;

                    case GlobalMethodName.addNewMachine:
                        response = newProxy.addNewMachine(parameter[0].ToString(), parameter[1].ToString());
                        break;

                    case GlobalMethodName.getKeyMaster:
                        response = newProxy.getKeyMaster(parameter[0].ToString());
                        break;

                    default:
                        response = "No such command";
                        break;
                }

                Console.WriteLine(classNameLog + "Message: " + response);
                return response;
            }
            else
            {
                return "Sending to self is not permitable";
            }
        }
        catch (XmlRpcException e)
        {
            return "Connection refused";
        }
        catch (Exception e)
        {
            Console.WriteLine("{0}", e.Message);
            return e.Message;
        }
    }

    public static void SendToAllMachines(Dictionary<int, String> machines, String command, Object[] parameter)
    {
        int success = 0;

        foreach (KeyValuePair<int, string> entry in machines)
        {
            String ipAddress = entry.Value;

            try
            {
                //don't send to self
                NetworkPingService nps = new NetworkPingService();
                ServerStatusCheck ssc = new ServerStatusCheck();
                String myIpAddress = nps.GetMyIpAddress();

                if (ipAddress != myIpAddress && ssc.isServerUp(ipAddress, 1090, 300))
                {
                    Console.WriteLine(classNameLog + "Command " + command + " Contacting priority " + entry.Key + " => " + ipAddress);

                    //check if machine is on
                    Object response = (Object)SendToOneMachine(ipAddress, command, parameter);
                    Console.WriteLine(classNameLog + response);
                    success += 1;
                }
                else
                {
                    Console.WriteLine(classNameLog + "Machine is not active or my own machine");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
        }

        Console.WriteLine("Finished sending to all machines, success call " + success);
    }
}

public class Helper
{
    public static XmlRpcStruct[] ConvertDictToStruct(Dictionary<int, string> inputDict)
    {
        XmlRpcStruct[] convertionResult = new XmlRpcStruct[inputDict.Count()];
        for (int i = 0; i < inputDict.Count; i++)
        {
            convertionResult[i] = new XmlRpcStruct();
            convertionResult[i].Add("NetworkPriority", inputDict.Keys.ElementAt(i));
            convertionResult[i].Add("IpAddress", inputDict.Values.ElementAt(i));
        }

        return convertionResult;
    }

    public static Dictionary<int, string> ConvertObjToDict(Object inputObj)
    {
        Dictionary<int, string> convertionResult = new Dictionary<int, string>();

        if(inputObj.GetType().Name == "XmlRpcStruct[]")
        {
            XmlRpcStruct[] structInput = (XmlRpcStruct[])inputObj;
            foreach(XmlRpcStruct structItem in structInput)
            {
                int itemKey = (int)structItem["NetworkPriority"];
                string itemValue = structItem["IpAddress"].ToString();
                convertionResult.Add(itemKey, itemValue);
            }
        }

        return convertionResult;
    }
}

