using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType
{
    internal class ConvertUtils
    {
        internal static object ConvertToObjectFilter(object data)
        {
            string nullString = "null";
            object result = null;
            try
            {
               
                if (data is int)
                {
                    result = (int)data;
                }
                else if (data is int?)
                {
                    if (data != null)
                        result = (int)data;
                    else
                        result = nullString;
                }
                else if (data is short)
                {
                    result = (short)data;
                }
                else if (data is short?)
                {
                    if (data != null)
                        result = (short)data;
                    else
                        result = nullString;
                }
                else if (data is long)
                {
                    result = (long)data;
                }
                else if (data is long?)
                {
                    if (data != null)
                        result = (long)data;
                    else
                        result = nullString;
                }
                else if (data is decimal)
                {
                    result = (decimal)data;
                }
                else if (data is decimal?)
                {
                    if (data != null)
                        result = (decimal)data;
                    else
                        result = nullString;
                }
                else if (data is bool)
                {
                    if ((bool)data == true) data = "true"; else data = "false";
                    result = data;
                }
                else if (data is bool?)
                {
                    if (data != null)
                    {
                        if ((bool)data == true) data = "true"; else data = "false";
                        result = data;
                    }
                    else
                        result = nullString;
                }
                else
                {
                    if (data != null)
                        result = (string)data;
                    else
                        result = nullString;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
