using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisSereServPtttTemp.Base
{
    public class ImageADO : HIS_SERE_SERV_FILE
    {
        public long ID { get; set; }
        public int ImageIndex { get; set; }
        public bool IsChecked { get; set; }
        public string FileName { get; set; }
        public Image IMAGE_DISPLAY { get; set; }
        //public TileItem Tile { get; set; }
        public int? STTImage { get; set; }
        public System.IO.Stream streamImage { get; set; }
    }
}
