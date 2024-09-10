using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.Loading.Design.Template1
{
    internal partial class Template1 : UserControl
    {

        private BackgroundWorker bw = new BackgroundWorker();
        private BackgroundWorker worker;

        private BWRunWorkerCompleted _RunCompleted;
        private BWDoWorker _DoWorker;

        public Template1()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }
    }
}
