using System;
using System.Collections.Generic;
using Godot;
using GoGameClient.script.network.common;
using GoGameClient.script.network.webSocket;
using GoGameClient.script.tool;
using Google.Protobuf;
using Script.Message;

namespace GoGameClient.script.global;

public partial class SocketControl : Node, IChannelListener
{
    private readonly ListenBroadcastMap _listenBroadcastMap = new();
    private readonly RequestMap _requests = new();
    private readonly ClientChannel _clientChannel = new();
    private static SocketControl _socket;

    public static SocketControl Instance()
    {
        return _socket;
    }

    public override void _Ready()
    {
        if (_socket != null) return;
        _socket = this;
        _clientChannel.SetChannelListener(this);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Poll();
    }

    public void SetUrl(string ip, int port, string context)
    {
        GD.Print(_clientChannel.SetUrl(ip, port, context) ? "发送连接成功" : "发送连接失败");
    }

    public void Reconnect()
    {
        GD.Print("Reconnect");
        _clientChannel.Reconnect();
    }


    private void Poll()
    {
        _clientChannel.Poll();
    }

    public void Request<TV>(RequestConfig<TV> config) where TV : IMessage<TV>
    {
        if (config == null)
        {
            throw new NullReferenceException("config不能为空");
        }

        if (config.Cmd == -1 || config.CmdMethod == -1)
        {
            throw new NullReferenceException("必须设置请求路由");
        }

        var message = ExternalKit.Of(CmdKit.Merge(config.Cmd, config.CmdMethod));
        Log.SocketLog(config.Title, config.Cmd, config.CmdMethod, null);
        if (config.Result != null)
        {
            _requests.Add(message.MsgId, config.Invoke);
        }

        _clientChannel.Send(message.ToByteArray());
    }

    public void Request<T, TV>(BodyRequestConfig<T, TV> config) where TV : IMessage<TV>
    {
        if (config == null)
        {
            throw new NullReferenceException("config不能为空");
        }

        if (config.Cmd == -1 || config.CmdMethod == -1)
        {
            throw new NullReferenceException("必须设置请求路由");
        }

        var message = ExternalKit.Of(CmdKit.Merge(config.Cmd, config.CmdMethod));
        if (config.Data != null)
        {
            var configData = config.Data;
            message.Data = configData switch
            {
                IMessage data => data.ToByteString(),
                long data => new LongValue { Value = data }.ToByteString(),
                List<long> data => new LongValueList { Values = { data } }.ToByteString(),
                int data => new IntValue { Value = data }.ToByteString(),
                List<int> data => new IntValueList { Values = { data } }.ToByteString(),
                bool data => new BoolValue { Value = data }.ToByteString(),
                List<bool> data => new BoolValueList { Values = { data } }.ToByteString(),
                string data => new StringValue { Value = data }.ToByteString(),
                List<string> data => new StringValueList { Values = { data } }.ToByteString(),
                _ => message.Data
            };

            Log.SocketLog(config.Title, config.Cmd, config.CmdMethod, config.Data.ToString());
        }
        else
        {
            Log.SocketLog(config.Title, config.Cmd, config.CmdMethod, null);
        }

        if (config.Result != null)
        {
            _requests.Add(message.MsgId, config.Invoke);
        }

        _clientChannel.Send(message.ToByteArray());
    }

    /**
        * 广播监听
        * @param config config
        */
    public void OfListen<TV>(ListenBroadcastConfig<TV> config) where TV : IMessage<TV>
    {
        if (config == null)
        {
            throw new NullReferenceException("config必须传递");
        }

        if (config.Result == null)
        {
            throw new NullReferenceException("广播监听必需配置回调 callback");
        }

        Log.SocketLog(config.Title, config.Cmd, config.CmdMethod, null);
        _listenBroadcastMap.Add(CmdKit.Merge(config.Cmd, config.CmdMethod), config.Invoke);
    }

    public void Open()
    {
        GD.Print("连接服务器成功");
    }

    public void Connecting()
    {
        GD.Print("----- Connecting");
    }

    public void Closing()
    {
        GD.Print("----- Closing");
    }

    public void Closed()
    {
        GD.Print("连接服务器断开连接");
    }

    public void OnMessage(ExternalMessage message)
    {
        GD.Print("----- OnMessage");

        if (message.ResponseStatus != 0)
        {
            Log.SocketErrorLog(
                message.ResponseStatus,
                message.ValidMsg,
                CmdKit.GetCmd(message.CmdMerge),
                CmdKit.GetCmdMethod(message.CmdMerge));
            return;
        }

        if (message.CmdCode == 0)
        {
            // 接收服务器心跳回调
            return;
        }

        var msgId = message.MsgId;
        if (msgId != 0)
        {
            if (_requests.Has(msgId))
            {
                _requests.Execute(message);
                return;
            }
        }

        // 广播
        _listenBroadcastMap.Execute(message);
        Console.WriteLine(message.ToByteString());
    }
}