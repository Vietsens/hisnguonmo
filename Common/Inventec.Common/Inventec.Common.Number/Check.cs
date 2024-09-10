using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventec.Common.Number
{
    public class Check
    {
        /// <summary>
        /// Kiem tra 1 chuoi co chi bao gom ky tu so (co the co 0 o dau)
        /// </summary>
        /// <param groupCode="message"></param>
        /// <returns>
        ///     true - hop le
        ///     false - khong hop le
        /// </returns>
        public static bool IsNumber(string message)
        {
            bool result = true;
            try
            {
                char[] varChar = message.ToCharArray();
                int i = 0;
                int val = 0;
                while (i < varChar.Length)
                {
                    val = System.Convert.ToInt32(varChar[i]);
                    if (val >= 48 && val <= 57) // 0 -> 9
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (i < varChar.Length)
                {
                    result = false;//chua ky tu ko cho phep
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static bool IsDecimal(string message)
        {
            bool result = false;
            try
            {
                Regex regex = new Regex(@"^[0-9]*\,?[0-9]+$");
                if (regex.IsMatch(message))
                {
                    if (message.First() == ',' || message.Last() == ',')
                    {
                        result = false;
                    }
                    else
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
