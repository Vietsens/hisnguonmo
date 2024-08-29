using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Aup.Utility
{
    public class FileDataResult
    {
        public FileDataResult()
        {
            IsUpdate = false;
        }

        public bool IsUpdate { get; set; }
        public string AppCode { get; set; }
        public string ZipFileUpdate { get; set; }
        public byte[] ByteFileUpdate { get; set; }
        public string FileZipName { get; set; }
        public List<string> FileDeletes { get; set; }
        public long TotalFileUpdate { get; set; }
        public string VersionServer { get; set; }
    }
}
