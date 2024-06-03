using System;
using GoGameClient.script.tool;
using Google.Protobuf;

namespace GoGameClient.script.network.webSocket;

public class RequestConfig<TV> where TV : IMessage<TV>
{
    public string Title { get; init; }
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

public class BodyRequestConfig<T, TV> : RequestConfig<TV>   where TV : IMessage<TV>
{
    public T Data { get; init; }
}