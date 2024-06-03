using System.Collections.Generic;
using GoGameClient.script.network.common;
using GoGameClient.script.tool;
using Script.Message;

namespace GoGameClient.script.network.webSocket;

public class ListenBroadcastMap
{
    private Dictionary<int, List<ResultListener.Callback>> CallbackMap { get; } = new();

    public void Add(int cmdMerge, ResultListener.Callback listener)
    {
        if (CallbackMap.ContainsKey(cmdMerge))
        {
            var listenBroadcast = CallbackMap.GetValueOrDefault(cmdMerge);
            listenBroadcast.Add(listener);
            return;
        }

        var list = new List<ResultListener.Callback> { listener };
        CallbackMap.Add(cmdMerge, list);
    }

    public void Execute(ExternalMessage message)
    {
        var cmdMerge = message.CmdMerge;
        var request = CallbackMap.GetValueOrDefault(cmdMerge);
        if (request == null)
        {
            return;
        }

        Log.ResponseSocketLog(
            CmdKit.GetCmd(message.CmdMerge),
            CmdKit.GetCmdMethod(message.CmdMerge));
        foreach (var callback in request)
        {
            callback(message.Data);
        }
    }
}