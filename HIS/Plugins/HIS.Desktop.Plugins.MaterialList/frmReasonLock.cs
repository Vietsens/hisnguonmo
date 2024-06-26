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
using Inventec.Core;
using MOS.SDO;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using MOS.Filter;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.MaterialList.Reason
{
    public partial class frmReasonLock : HIS.Desktop.Utility.FormBase
    {
        int positionHandleControl = -1;
        V_HIS_MATERIAL_1 material;
        Inventec.Desktop.Common.Modules.Module module;
        DelegateRefreshData delegateSelectData;

        public frmReasonLock(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
        }

        public frmReasonLock(Inventec.Desktop.Common.Modules.Module _module, V_HIS_MATERIAL_1 _material, DelegateRefreshData _delegateSelectData) :
            base(_module)
        {
            InitializeComponent();
            this.material = _material;
            this.delegateSelectData = _delegateSelectData;
            this.module = _module;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmReasonLock_Load(object sender, EventArgs e)
        {
            try
            {
                ValidationMaxlength(txtReason, 1000);
                SetIcon();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var data = new MOS.SDO.HisMaterialChangeLockSDO();
                data.MaterialId = material.ID;
                data.MediStockId = null;
                data.WorkingRoomId = this.module.RoomId;
                data.LockingReason = this.txtReason.Text;
                success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_LOCK, ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                SessionManager.ProcessTokenLost(param);

                if (success && this.delegateSelectData != null)
                {
                    this.delegateSelectData();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                module = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.Load -= new System.EventHandler(this.frmReasonLock_Load);
                emptySpaceItem1 = null;
                btnSave = null;
                layoutControlItem1 = null;
                txtReason = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                dxValidationProvider1 = null;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationMaxlength(MemoEdit control, int maxLength)
        {
            try
            {
                ValidateMaxLength validRule = new ValidateMaxLength();
                validRule.maxLength = maxLength;
                validRule.memoEdit = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                try
                {
                    BaseEdit edit = e.InvalidControl as BaseEdit;
                    if (edit == null)
                        return;

                    BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                    if (viewInfo == null)
                        return;

                    if (this.positionHandleControl == -1)
                    {
                        this.positionHandleControl = edit.TabIndex;
                        if (edit.Visible)
                        {
                            edit.SelectAll();
                            edit.Focus();
                        }
                    }
                    if (this.positionHandleControl > edit.TabIndex)
                    {
                        this.positionHandleControl = edit.TabIndex;
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
