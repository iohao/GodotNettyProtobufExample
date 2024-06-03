using Google.Protobuf;

namespace GoGameClient.script.network.webSocket;

public static class ResultListener
{
    public delegate void Callback(ByteString data);
}