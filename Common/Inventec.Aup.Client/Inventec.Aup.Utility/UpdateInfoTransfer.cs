using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Utility
{
    public class UpdateInfoTransfer
    {
        public UpdateInfoTransfer()
        {           
        }
        public string AppCode { get; set; }
        public string OSVersion { get; set; }
        public string VersionContent { get; set; }
        public string VersionLogContent { get; set; }
        public List<FileAppInfo> FileAppInfos { get; set; }
        public FileAppInfo AppVersion { get; set; }
    }
}
