using DevExpress.XtraEditors;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCHeniInfo.Design
{
    public partial class frmApointmentInfo : Form
    {
        private readonly Action<string> setTransferInCode;
        public frmApointmentInfo(Action<string> setTransferInCode)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            InitializeComponent();
            this.setTransferInCode = setTransferInCode;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAppointment != null && !string.IsNullOrEmpty(txtAppointment.Text))
                {
                    this.setTransferInCode?.Invoke(txtAppointment.Text);
                    this.Close();
                }
                else
                {
                    XtraMessageBox.Show("Vui lòng nhập mã hẹn khám!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
