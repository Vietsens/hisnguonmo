using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisRegimenTemp.UC
{
    public partial class UCPDF : UserControl
    {
        public UCPDF()
        {
            InitializeComponent();
            pdfViewer1.Dock = DockStyle.Fill;
        }

        private void pdfViewer1_Load(object sender, EventArgs e)
        {

        }

        public void LoadDocument (string filePath)
        {
            pdfViewer1.LoadDocument(filePath);
        }

        public void CloseDocument()
        {
            pdfViewer1.CloseDocument();
        }
    }
}
