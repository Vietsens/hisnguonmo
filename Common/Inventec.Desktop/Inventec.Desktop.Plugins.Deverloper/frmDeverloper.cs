using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using System.Configuration;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LocalStorage.Location;

namespace Inventec.Desktop.Plugins.Deverloper
{
    public partial class frmDeverloper : DevExpress.XtraEditors.XtraForm
    {
        string strPhatTrien = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.Deverloper"] ?? "";
        string strDonVi = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.GroupName"] ?? "";
        string strImage = ConfigurationSettings.AppSettings["Inventec.Common.UcDeverloper.Image.FileName"] ?? "";

        public frmDeverloper()
        {
            InitializeComponent();
        }

        private void frmDeverloper_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.UC.Development.MainDevelopment UCDeverloper = new Inventec.UC.Development.MainDevelopment();
                var formConfig = UCDeverloper.Init(Inventec.UC.Development.MainDevelopment.EmumTemp.TEMPLATE1, strDonVi, strPhatTrien);
                UCDeverloper.SetImage(formConfig, strImage);
                formConfig.Dock = DockStyle.Fill;
                this.Controls.Add(formConfig);
                try
                {
                    this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}