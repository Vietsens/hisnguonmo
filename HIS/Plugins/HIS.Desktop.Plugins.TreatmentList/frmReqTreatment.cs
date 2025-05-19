using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class frmReqTreatment : Form
    {
        private int positionHandleTime;
        V_HIS_TREATMENT_4 currentTreatment;

        Action<string> dlgReason { get; set; }
        bool IsClose = true;
        Action<bool> dlgClose { get; set; }
        public frmReqTreatment(V_HIS_TREATMENT_4 treatment, Action<string> dlgReason, Action<bool> dlgClose)
        {
            InitializeComponent();
            currentTreatment = treatment;
            this.dlgReason = dlgReason;
            this.dlgClose = dlgClose;
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
        }

        private void frmReqTreatment_Load(object sender, EventArgs e)
        {
            checkReason();
            Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule valid = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
            valid.editor = memReason;
            valid.ErrorText = "Trường dữ liệu bắt buộc";
            valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(memReason, valid);
        }

        private void checkReason()
        {
            try
            {
                memReason.Text = currentTreatment.REASON_UNFINISH;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleTime == -1)
                {
                    positionHandleTime = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleTime > edit.TabIndex)
                {
                    positionHandleTime = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!dxValidationProvider1.Validate()) return;
            dlgReason(memReason.Text.Trim());
            IsClose = false;
            this.Close();
        }


        private void frmReqTreatment_FormClosed(object sender, FormClosedEventArgs e)
        {
            dlgClose(IsClose);
        }
    }
}
