using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Utility
{
    public class FileAppInfo
    {
        public FileAppInfo()
        {
           
        }
        public FileAppInfo(string fullName, long length, DateTime lastWriteTime)
        {
            this.FullName = fullName;
            this.Length = length;
            this.LastWriteTime = lastWriteTime;
        }
        public string FullName { get; set; }
        public long Length { get; set; }
        public DateTime LastWriteTime { get; set; }
        public string Content { get; set; }
        public List<System.IO.FileInfo> TmpNameList { get; set; } // danh sách tên các file template
    }
}
