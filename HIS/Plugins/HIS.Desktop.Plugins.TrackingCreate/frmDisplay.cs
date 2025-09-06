using HIS.Desktop.Plugins.HisRegimenTemp.UC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    public partial class frmDisplay : Form
    {
        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCDocument UcDocument = null;
        HIS.Desktop.Plugins.HisRegimenTemp.UC.UCPDF UCPdf = null;
        public frmDisplay(UCPDF ucPdf)
        {
            this.UCPdf = ucPdf;
            InitializeComponent();
        }
        public frmDisplay(UCDocument ucDocument)
        {
            this.UcDocument = ucDocument;
            InitializeComponent();
        }
        private void Form2_Load(object sender, EventArgs e)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            if (UcDocument != null)
            {
                this.panel1.Controls.Add(this.UcDocument);
                this.UcDocument.Dock = DockStyle.Fill;
            }
            else if (UCPdf != null)
            {
                this.panel1.Controls.Add(this.UCPdf);
                this.UCPdf.Dock = DockStyle.Fill;
            }
        }
    }
}
