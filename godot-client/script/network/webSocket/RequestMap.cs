using System;
using System.Collections.Generic;
using GoGameClient.script.network.common;
using GoGameClient.script.tool;
using Google.Protobuf;
using Script.Message;

namespace GoGameClient.script.network.webSocket;

public class RequestMap
{

    private Dictionary<int, ResultListener.Callback> CallbackMap { get; } = new();


    public void Add(int msgId, ResultListener.Callback listener)
    {
        if (listener == null)
        {
            return;
        }

        CallbackMap.Add(msgId, listener);
    }

    public bool Has(int msgId)
    {
        return CallbackMap.ContainsKey(msgId);
    }

    public void Execute(ExternalMessage message)
    {
        var msgId = message.MsgId;
        var request = CallbackMap.GetValueOrDefault(msgId);
        CallbackMap.Remove(msgId);
        Log.ResponseSocketLog(
            CmdKit.GetCmd(message.CmdMerge),
            CmdKit.GetCmdMethod(message.CmdMerge));
        request(message.Data);
    }
}