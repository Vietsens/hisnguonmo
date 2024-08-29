using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ConnectDevice.Message
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
