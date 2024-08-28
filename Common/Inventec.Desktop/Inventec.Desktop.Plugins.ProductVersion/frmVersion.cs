using Inventec.Common.Logging;
using Inventec.Desktop.Common.LocalStorage.Location;
using System;
using System.Configuration;
using System.Drawing;

namespace Inventec.Desktop.Plugins.ProductVersion
{
    public partial class frmVersion : DevExpress.XtraEditors.XtraForm
    {
        public frmVersion()
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmVersion_Load(object sender, EventArgs e)
        {
            try
            {
                txtVersion.Text = System.IO.File.ReadAllText("readme.txt");
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong tim thay file readme.txt hoac du lieu khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ex), ex));
            }

        }
    }
}