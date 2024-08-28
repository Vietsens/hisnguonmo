using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.String
{
    public class CheckString
    {
        public static bool IsOverMaxLength(string text, int maxLength)
        {
            bool result = true;
            try
            {
                if (!System.String.IsNullOrWhiteSpace(text))
                {
                    if (text.Length <= maxLength)
                    {
                        result = false;
                    }
                }
            }
            catch (Exception)
            {
                result = true;
            }
            return result;
        }

        public static bool IsOverMaxLengthUTF8(string text, int maxLength)
        {
            bool result = false;
            try
            {
                if (!System.String.IsNullOrWhiteSpace(text))
                {
                    if (Encoding.UTF8.GetByteCount(text) > maxLength)
                    {
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}
