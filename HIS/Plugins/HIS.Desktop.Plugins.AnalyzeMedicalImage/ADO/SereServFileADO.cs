using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnalyzeMedicalImage.ADO
{
    public class SereServFileADO : HIS_SERE_SERV
    {
        public string BracnhCode { get; set; }
        public string ServiceName { get; set; }
        public byte[] FileContent { get; set; }
        public string Content { get; set; }
        public string Extension { get; set; }
        public long? CREATE_TIME { get; set; }
        public string URL_NAME { get; set; }
        public string URL { get; set; }
        public bool IsChecked { get; set; }
        public bool IsFss { get; set; }
        public string Key { get; set; }
        public System.Drawing.Image image { get; set; }
    }
}
