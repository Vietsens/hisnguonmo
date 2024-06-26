/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
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
    public partial class frmReasonOpenTreatment : Form
    {
        private int positionHandleTime;

        Action<string> dlgReason { get; set; }
        bool IsClose = true;
        Action<bool> dlgClose { get; set; }
        public frmReasonOpenTreatment(Action<string> dlgReason, Action<bool> dlgClose)
        {
            InitializeComponent();
            this.dlgReason = dlgReason;
            this.dlgClose = dlgClose;
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);
        }

        private void frmReasonOpenTreatment_Load(object sender, EventArgs e)
        {
            Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule valid = new Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule();
            valid.editor = memReason;
            valid.ErrorText = "Trường dữ liệu bắt buộc";
            valid.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(memReason, valid);
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
            memReason.Text = memReason.Text.Trim();
            if (!dxValidationProvider1.Validate()) return;
            dlgReason(memReason.Text.Trim());
            IsClose = false;
            this.Close();
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.PerformClick();
        }

        private void frmReasonOpenTreatment_FormClosed(object sender, FormClosedEventArgs e)
        {
            dlgClose(IsClose);
        }
    }
}
