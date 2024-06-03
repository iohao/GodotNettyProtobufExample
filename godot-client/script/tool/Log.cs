using System;
using System.Net.Sockets;
using Godot;

namespace GoGameClient.script.tool;

public static class Log
{
    public static void SocketLog(string title, int cmd, int cmdMethod, string data)
    {
        GD.Print($"---------------------{title}------------------------");
        GD.Print($"请求 标题：{title}  模块路由：{cmd} 方法路由：{cmdMethod}");
        GD.Print("----------------------------------------------------");
        if (data != null)
        {
            GD.Print($"请求体：{data}");
        }
       
    }


    public static void SocketErrorLog(int messageResponseStatus, string messageValidMsg, int getCmd, int getCmdMethod)
    {
        GD.PrintErr("----------------------error-----------------------------");
        GD.PrintErr($"[错误码:{messageResponseStatus}] - [消息:{messageValidMsg}] - [路由：{getCmd}-{getCmdMethod}]");
        GD.PrintErr("--------------------------------------------------------");
    }

    public static void ResponseContentLog(object data)
    {
        GD.Print($"响应数据：{data}");
    }
    public static void ResponseSocketLog(int getCmd, int getCmdMethod)
    {
        GD.Print($"-----------------------{getCmd}-{getCmdMethod}--------------------------");
        GD.Print($"响应 模块路由：{getCmd} 方法路由：{getCmdMethod}");
        GD.Print("----------------------------------------------------");
    }
}