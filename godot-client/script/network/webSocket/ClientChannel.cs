using System;
using Godot;
using GoGameClient.script.tool;
using Script.Message;

namespace GoGameClient.script.network.webSocket;

public interface IChannelListener
{
    public void Open();
    public void Connecting();
    public void Closing();
    public void Closed();
    public void OnMessage(ExternalMessage message);
}

public class ClientChannel
{
    private IChannelListener _channelListener;
    private WebSocketPeer.State _lastState = WebSocketPeer.State.Closed;
    private WebSocketPeer Socket { get; } = new();
    private string _url;
    private bool _isReConnect;

    public bool SetUrl(string ip, int port, string context)
    {
        var url = $"ws://{ip}:{port}/{context}";
        GD.Print($"websocket-url {url}");
        
        var error = Socket.ConnectToUrl(_url = url);
        GD.Print($"error --- {error}");
        if (error != 0)
        {
            return false;
        }

        _lastState = Socket.GetReadyState();
        GD.Print($"_lastState --- {_lastState}");
        return true;
    }

    public void SetChannelListener(IChannelListener channelListener)
    {
        _channelListener = channelListener;
    }


    public void Poll()
    {
        if (Socket.GetReadyState() != WebSocketPeer.State.Closed)
        {
            Socket.Poll();
        }

        while (Socket.GetAvailablePacketCount() > 0)
        {
            _channelListener?.OnMessage(ExternalMessage.Parser.ParseFrom(Socket.GetPacket()));
        }

        var state = Socket.GetReadyState();
        if (_lastState == state) return;
        _lastState = state;
        if (_channelListener == null) return;
        switch (state)
        {
            case WebSocketPeer.State.Open:
                _channelListener.Open();
                //发送连接成功回调
                break;
            case WebSocketPeer.State.Connecting:
                _channelListener.Connecting();
                //发送连接中回调
                break;
            case WebSocketPeer.State.Closing:
                _channelListener.Closing();
                //发送关闭成功回调
                break;
            case WebSocketPeer.State.Closed:
                _channelListener.Closed();
                if (_isReConnect)
                {
                    _isReConnect = false;
                    Socket.ConnectToUrl(_url);
                    _lastState = Socket.GetReadyState();
                }

                //发送关闭成功回调
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Send(byte[] toByteArray)
    {
        GD.Print("----------------------Send!-----------------------------" + toByteArray);
        Socket.Send(toByteArray);
    }

    public void Reconnect()
    {
        Socket.Close();
        _isReConnect = true;
    }
}