using System;

namespace Inventec.Common.Checksum
{
    public class Rs232
    {
        public static int? Calc(string text)
        {
            int? result = null;
            try
            {
                if (!String.IsNullOrEmpty(text))
                {
                    int temp = 0;
                    byte[] byteArr = System.Text.Encoding.ASCII.GetBytes(text);
                    foreach (byte b in byteArr)
                    {
                        temp += b;
                    }
                    if (temp > 0) result = temp;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public static object Check(string message)
        {
            object result = null;
            try
            {
                int checksum = Inventec.Common.TypeConvert.Parse.ToInt32(message.Substring(0, message.IndexOf(",")).Replace(",", ""));

                string dataTransfer = message.Substring(message.IndexOf(",") + 1);
                if (checksum == Calc(dataTransfer))
                    result = dataTransfer;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}
