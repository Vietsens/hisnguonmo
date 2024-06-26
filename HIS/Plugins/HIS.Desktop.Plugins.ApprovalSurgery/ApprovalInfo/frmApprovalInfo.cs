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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ApprovalSurgery.ApprovalInfo
{
    
    public delegate void RefeshDataResult(int action, long time, ACS_USER user);
    public partial class frmApprovalInfo : Form
    {
        RefeshDataResult refeshDataResult;
        public int positionHandle = -1;
        int action; //action = 1 : APPROVAL , 2: UNAPPROVAL, 3 : REJECT, 4 : UNRJECT

        public frmApprovalInfo(int action, RefeshDataResult _refeshDataResult)
        {
            InitializeComponent();
            try
            {
                this.refeshDataResult = _refeshDataResult;
                this.action = action;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool valid = true;
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;
                if (valid)
                {
                    if (refeshDataResult != null)
                    { 
                        long time = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime)??0;
                        ACS_USER user = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS_USER>()
                            .FirstOrDefault(o=>o.LOGINNAME == cboLoginname.EditValue.ToString());
                        refeshDataResult(action, time, user);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItemCtrlS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void frmApprovalInfo_Load(object sender, EventArgs e)
        {
            //Load icon
            this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));

            ValidateControl();
            LoadDataDefault();

        }

        private void txtLoginname_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadLoginName(strValue, false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginname_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboLoginname.EditValue != null)
                    {
                        var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboLoginname.EditValue.ToString());
                        if (data != null)
                        {
                            txtLoginname.Text = data.USERNAME;
                            txtLoginname.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboLoginname_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboLoginname.EditValue.ToString());
                    if (data != null)
                    {
                        txtLoginname.Text = data.USERNAME;
                        txtLoginname.Focus();
                    }
                }
                else
                {
                    cboLoginname.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
