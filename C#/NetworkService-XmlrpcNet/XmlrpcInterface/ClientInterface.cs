﻿using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface INetworkServerClientProxy : IXmlRpcProxy
{
    [XmlRpcMethod("showNetworkHashMap")]
    NetworkMapStruct[] ShowNetworkHashMap(bool ShowLocalMap = false, int inputLamportClock = 0);

    [XmlRpcMethod("newMachineJoin")]
    void newMachineJoin(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false);

    [XmlRpcMethod("getIpMaster")]
    string getIpMaster(string callerIp, int inputLamportClock = 0);

    [XmlRpcMethod("joinNetwork")]
    void joinNetwork(string ipAddress, int inputLamportClock = 0);

    [XmlRpcMethod("removeMachine")]
    void removeMachine(string ipAddress, int inputLamportClock = 0, bool DoItLocally = false);

    [XmlRpcMethod("addNewMessage")]
    void addNewMessage(string newMessage, int inputLamportClock = 0);

    [XmlRpcMethod("getMessages")]
    string[] getMessages(int inputLamportClock = 0);

    [XmlRpcMethod("changeMaster")]
    void changeMaster(string newMasterIp, int inputLamportClock = 0);

    [XmlRpcMethod("getCurrentLamportClock")]
    int getCurrentLamportClock();

    [XmlRpcMethod("changeMaster")]
    void changeMaster(string newMasterIp);

    [XmlRpcMethod("receiveElectionSignal")]
    string receiveElectionSignal(string senderIP);

    [XmlRpcMethod("checkMasterStatus")]
    void checkMasterStatus();

    [XmlRpcMethod("DoLocalElection")]
    void DoLocalElection();

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection();

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection(AsyncCallback acb);

    [XmlRpcBegin("DoLocalElection")]
    IAsyncResult BeginDoLocalElection(AsyncCallback acb, object state);

    [XmlRpcEnd("DoLocalElection")]
    void EndDoLocalElection(IAsyncResult iars);

    [XmlRpcMethod("MutualExclusion.SendMERequest")]
    bool SendMERequestToServer(string methodName, string parameter);

    [XmlRpcBegin("MutualExclusion.ReceiveMEReply")]
    IAsyncResult BeginReceiveMEReply();

    [XmlRpcMethod("MutualExclusion.ReceiveMEReply")]
    void ReceiveMEReply();

    [XmlRpcEnd("MutualExclusion.ReceiveMEReply")]
    void EndReceiveMEReply(IAsyncResult iars);

    [XmlRpcMethod("MutualExclusion.ReceiveMERequest")]
    string ReceiveMERequest(string ipAddress, int inputLamportClock);

    [XmlRpcMethod("MutualExclusion.AccessCriticalPart")]
    void AccessCriticalPart(string senderIP, string methodName, string parameter = "", int inputLamportClock = 0);
}