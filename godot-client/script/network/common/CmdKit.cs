namespace GoGameClient.script.network.common;

public static class CmdKit
{
    public static int GetCmd(int cmdMerge)
    {
        return cmdMerge >> 16;
    }

    public static int GetCmdMethod(int cmdMerge)
    {
        return cmdMerge & 0xFFFF;
    }

    public static int Merge(int cmd, int cmdMethod)
    {
        return (cmd << 16) + cmdMethod;
    }

    public static CmdInfo Of(int cmdMerge)
    {
        var cmd = GetCmd(cmdMerge);
        var cmdMethod = GetCmdMethod(cmdMerge);

        return new CmdInfo(cmd, cmdMethod);
    }

    public class CmdInfo
    {
        private readonly int _cmd;
        private readonly int _cmdMethod;

        public CmdInfo(int cmd, int cmdMethod)
        {
            _cmd = cmd;
            _cmdMethod = cmdMethod;
        }

        public override string ToString()
        {
            return $"{_cmd}-{_cmdMethod}";
        }
    }
}