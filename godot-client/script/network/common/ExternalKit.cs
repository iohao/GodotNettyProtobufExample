using Script.Message;

namespace GoGameClient.script.network.common;

public static class ExternalKit
{
    private static int _msgId = 1;

   public static ExternalMessage Of(int cmdMerge)
   {
       _msgId++;
       var message = new ExternalMessage
       {
           MsgId = _msgId,
           CmdMerge = cmdMerge
           
       };
       return message;
   }
}