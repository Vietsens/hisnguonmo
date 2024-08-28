using System;

namespace Inventec.Common.Resource
{
    public class Get
    {
        public static string Value(string key, System.Resources.ResourceManager resource, System.Globalization.CultureInfo cul)
        {
            string result = "";
            try
            {
                result = resource.GetString(key, cul);
            }
            catch (Exception ex)
            {
                Logging.LogSystem.Warn("Get gia tri trong resource that bai. key = " + key + " , value = " + result, ex);
                result = "";
            }
            return result;
        }
    }
}
