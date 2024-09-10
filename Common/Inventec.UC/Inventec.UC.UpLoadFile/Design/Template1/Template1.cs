using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using System.IO;

namespace Inventec.UC.UpLoadFile.Design.Template1
{
    internal partial class Template1 : UserControl
    {
        private List<Data.DataShowGridControl> ListData = new List<Data.DataShowGridControl>();
        List<FileStream> ListStream = new List<FileStream>();
        private UpLoadFileToServer _UpLoad;

        private int index;
        private int status = 9;

        public Template1(UpLoadFileToServer Upload)
        {
            InitializeComponent();
            this._UpLoad = Upload;
        }

    }
}
