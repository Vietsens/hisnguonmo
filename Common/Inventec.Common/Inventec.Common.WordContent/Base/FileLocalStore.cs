using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent.Base
{
    public class FileLocalStore
    {
        private static string _rootpath;
        public static string Rootpath
        {
            get
            {
                if (String.IsNullOrEmpty(_rootpath))
                {
                    _rootpath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "").Replace("file:/", "/") + System.IO.Path.DirectorySeparatorChar, ApplicationStore.ROOT_PATH));
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
