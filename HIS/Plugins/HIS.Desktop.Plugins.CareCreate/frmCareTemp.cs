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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.CareCreate.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
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

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class frmCareTemp : Form
    { 
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private HIS_CARE_TEMP CareTemp;
        private int positionHandleControl = -1;

        #endregion

        public frmCareTemp( HIS_CARE_TEMP data)
            : this(null, data)
        { }

        public frmCareTemp(Inventec.Desktop.Common.Modules.Module moduleData, HIS_CARE_TEMP data)
        {
            InitializeComponent();
            this.CareTemp = data;
            this.moduleData = moduleData;

            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmCareTemp_Load(object sender, EventArgs e)
        {
            SetDefaultDataControl();
            ValidateColtrol();
        }

        private void SetDefaultDataControl()
        {
            try
            {
                txtTempCode.Text = "";
                txtTempName.Text = "";
                ChkIsPublic.Checked = false;

                txtTempCode.Focus();
                txtTempCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void ValidateColtrol()
        {
            try
            {
                ValidateRule.TextEditTempValidationRule code = new ValidateRule.TextEditTempValidationRule();
                code.required = true;
                code.txtEdit = txtTempCode;
                code.maxLenght = 6;
                code.ErrorText = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                code.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTempCode, code);


                ValidateRule.TextEditTempValidationRule name = new ValidateRule.TextEditTempValidationRule();
                name.required = true;
                name.txtEdit = txtTempName;
                name.maxLenght = 100;
                name.ErrorText = ResourceMessage.ThieuTruongDuLieuBatBuoc;
                name.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTempName, name);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void btnSaveTemp_Click(object sender, EventArgs e)
        {
            try
            {
                btnSaveTemp.Focus();
                if (!btnSaveTemp.Enabled) return;

                this.positionHandleControl = -1;
                if (!dxValidationProvider.Validate())
                    return;
                HIS_CARE_TEMP rsHisCare = null;

                if (CareTemp == null) throw new ArgumentNullException("hisCare is null");
                
                CareTemp.CARE_TEMP_CODE = txtTempCode.Text.Trim();
                CareTemp.CARE_TEMP_NAME = txtTempName.Text.Trim();

                CommonParam param = new CommonParam();
                bool success = false;
                try
                {
                    WaitingManager.Show();
                    rsHisCare = new BackendAdapter(param).Post<HIS_CARE_TEMP>("api/HisCareTemp/Create", ApiConsumers.MosConsumer, CareTemp, param);

                    if (rsHisCare != null && rsHisCare.ID > 0)
                    {
                        success = true;
                    }
                    WaitingManager.Hide();

                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            try
            {
                btnSaveTemp_Click(null,null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void txtTempCode_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTempName.Focus();
                    txtTempName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }

        private void txtTempName_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChkIsPublic.Properties.FullFocusRect = true;
                    ChkIsPublic.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkIsPublic_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnSaveTemp.Enabled)
                    {
                        btnSaveTemp.Focus();
                    }
                }
                if (e.KeyCode == Keys.Space)
                {
                    if (ChkIsPublic.Checked)
                        ChkIsPublic.Checked = false;
                    else
                        ChkIsPublic.Checked = true;
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }
    }
}
