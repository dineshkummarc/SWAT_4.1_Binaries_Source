using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SWAT
{
    public class ReplConnection : JSSHConnection
    {
        private string replInstance = "";
        private static readonly string[] nonGlobalFunctions = {"print", "getWindows", "domDumpFull"};

        public ReplConnection()
        {
            isFF4 = true;
        }

        public ReplConnection(string swatGuid) : base(swatGuid)
        {
            isFF4 = true;
        }

        protected override void ClearWelcomeMessage()
        {
            DateTime timeout = DateTime.Now.AddSeconds(5);
            Regex instanceExpr = new Regex("repl[0-9]*>");
            while (DateTime.Now < timeout && replInstance.Length == 0)
            {
                string welcomeMsg = GetMessage();
                Match m = instanceExpr.Match(welcomeMsg);
                replInstance = m.ToString().TrimEnd(">".ToCharArray());
            }
        }

        private string ConvertMessageToProperContext(string msg)
        {
            return nonGlobalFunctions.Aggregate(msg,
                                                (current, s) =>
                                                current.Replace(s, string.Format("{0}.{1}", replInstance, s)));
        }

        public override string SendMessage(string msg, bool receive, bool setContext)
        {
            isFF4 = true;
            if (setContext)
            {
                string contextmsg = string.Format("{0}.enter(content);", replInstance);
                base.SendMessage(contextmsg, true, true);
                base.SendMessage(replInstance + ".home = function() { return this.enter(content); }", true, true);
            }
            msg = ConvertMessageToProperContext(msg);
            string replInstanceReplace = string.Format("{0}>", replInstance);
            string ret = base.SendMessage(msg, receive, true).Replace(replInstanceReplace, "").Trim();
            return ret.Contains("....>") ? base.SendMessage(";", true, true).Replace(replInstanceReplace, "").Trim() : ret;
        }

        public override void Disconnect()
        {
            isFF4 = false;
            string disconnectMsg = string.Format("{0}.quit();", replInstance);
            base.SendMessage(disconnectMsg, false, false);
            base.Disconnect();
        }
    }
}