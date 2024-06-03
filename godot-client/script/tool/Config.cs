namespace GoGameClient.script.tool;

public static class Config
{
    private static string Ip => "127.0.0.1";
    private static int Port => 10100;
    private static string Context => "websocket";
    private static bool _isLogin;


    public static void SetLogin(bool isLogin)
    {
        _isLogin = isLogin;
    }

    public static bool GetLogin()
    {
        return _isLogin;
    }

    public static string GetIp()
    {
        return Ip;
    }

    public static int GetPort()
    {
        return Port;
    }

    public static string GetContext()
    {
        return Context;
    }
}