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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample
{
    class GenerateMessageId
    {
        private static Dictionary<string, string> messageIds = new Dictionary<string, string>();
        private static Object lockObject = new Object();

        internal static string Generate(string holdString)
        {
            try
            {
                lock (lockObject)
                {
                    bool exists = false;
                    short loopCount = 0;
                    short loopMax = 10;
                    string messageId = null;
                    do
                    {
                        messageId = Inventec.Common.String.Generate.Random(6, Inventec.Common.String.Generate.CharacterMode.ONLY_NUMERIC, Inventec.Common.String.Generate.UpperLowerMode.ANY);
                        if (messageIds.ContainsKey(messageId))
                        {
                            messageId = null;
                            exists = true;
                        }
                        else
                        {
                            exists = false;
                            messageIds[messageId] = holdString;
                        }
                    } while (exists && loopCount < loopMax);
                    return messageId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
