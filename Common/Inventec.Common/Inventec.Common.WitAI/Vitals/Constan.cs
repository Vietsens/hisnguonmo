using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WitAI.Vitals
{
    public class Constan
    {
        internal static ConcurrentDictionary<string, string> dicFile = new ConcurrentDictionary<string, string>();

        public static void SetConstan(string wit_Ai_Access_Token, int timereplay)
        {
            if (!String.IsNullOrEmpty(wit_Ai_Access_Token))
            {
                Wit_Ai_Access_Token = wit_Ai_Access_Token;
            }
            if (timereplay > 0)
                TimeReplay = timereplay;
        }
        internal static string Wit_Ai_Access_Token = (ConfigurationManager.AppSettings["Inventec.Common.WitAI.WitAiAccessToken"] ?? "L35S77RR5JM6GMG35CZIU76DTVQNOTZK");

        internal static int TimeReplay = int.Parse(ConfigurationManager.AppSettings["Inventec.Common.WitAI.TimeReplay"] ?? "3000");
    }
}
