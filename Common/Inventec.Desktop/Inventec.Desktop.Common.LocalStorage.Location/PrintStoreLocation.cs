
using System;
using System.Configuration;
namespace Inventec.Desktop.Common.LocalStorage.Location
{
    public class PrintStoreLocation
    {
        static string ROOT_PATH = "PrintTemplate";
        static string _rootpath;
        public static string PrintTemplatePath
        {
            get
            {
                if (String.IsNullOrEmpty(_rootpath))
                {
                    _rootpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "").Replace("file:/", "/") + System.IO.Path.DirectorySeparatorChar, ROOT_PATH));
                }
                return _rootpath;
            }
            set
            {
                _rootpath = value;
            }
        }
    }
}
