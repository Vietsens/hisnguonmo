
using System;
using System.Configuration;
namespace Inventec.Desktop.Common.LocalStorage.Location
{
    public class ApplicationStoreLocation
    {
        static string strApplicationDirectory;
        public static string ApplicationDirectory
        {
            get
            {
                if (System.String.IsNullOrEmpty(strApplicationDirectory))
                {
                    string codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                    System.UriBuilder uri = new System.UriBuilder(codeBase);
                    string path = System.Uri.UnescapeDataString(uri.Path);
                    strApplicationDirectory = System.IO.Path.GetDirectoryName(path);
                }
                return strApplicationDirectory;
            }
        }
    }
}
