/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
