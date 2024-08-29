using System;

namespace Inventec.Common.DateTime
{
    public class Check
    {
        public static bool IsValidTime(long time)
        {
            bool result = false;
            try
            {
                System.DateTime temp = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static bool IsValidTime(string time)
        {
            bool result = false;
            try
            {
                System.DateTime temp = System.DateTime.ParseExact(time, "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static bool IsValidTime(string time, bool normalTime)
        {
            bool result = false;
            try
            {
                if (normalTime)
                {
                    System.DateTime temp = System.Convert.ToDateTime(time);
                    result = true;
                }
                else
                {
                    System.DateTime temp = System.DateTime.ParseExact(time, "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    result = true;
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
