using System;
using Godot;
using GoGameClient.script.tool;
using Google.Protobuf;

namespace GoGameClient.script.network.webSocket;

public class ListenBroadcastConfig<TV> where TV : IMessage<TV>
{
    public string Title { get; init; } = "监听广播";
    public int Cmd { get; init; } = -1;
    public int CmdMethod { get; init; } = -1;
    public Action<TV> Result { get; init; }

    public void Invoke(ByteString data)
    {
        var message = new MessageParser<TV>(Activator.CreateInstance<TV>).ParseFrom(data);
        Log.ResponseContentLog(message);
        Result.Invoke(message);
    }
}