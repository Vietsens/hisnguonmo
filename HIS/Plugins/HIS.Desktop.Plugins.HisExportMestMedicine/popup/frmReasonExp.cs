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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.HisExportMestMedicine.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisExportMestMedicine.popup
{
    public partial class frmReasonExp : FormBase
    {
        V_HIS_EXP_MEST expMest;
        int positionHandle = -1;
        DelegateRefeshData refeshData;
        long roomId;
        public frmReasonExp(Inventec.Desktop.Common.Modules.Module module, V_HIS_EXP_MEST _expMest, DelegateRefeshData _refeshData)
            : base(module)
        {
            InitializeComponent();
            this.expMest = _expMest;
            this.refeshData = _refeshData;
            this.roomId = module.RoomId;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmReasonExp_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboReasonExp();
                SetDefaultData();
                if (HisConfigCFG.IS_REASON_REQUIRED == "1")
                {
                    ValidationSingleControl(cboReasonExp);
                    layoutControlItem1.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SetDefaultData()
        {
            try
            {
                if (this.expMest != null)
                {
                    cboReasonExp.EditValue = this.expMest.EXP_MEST_REASON_ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboReasonExp()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXP_MEST_REASON_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXP_MEST_REASON_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboReasonExp, data, controlEditorADO);
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
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                ExpMestUpdateReasonSDO sdo = new ExpMestUpdateReasonSDO();
                sdo.ExpMestId = this.expMest.ID;
                sdo.ExpMestReasonId = cboReasonExp.EditValue != null ? (long?)Convert.ToInt64(cboReasonExp.EditValue) : null;
                sdo.WorkingRoomId = this.roomId;

                var rs = new BackendAdapter(param).Post<HIS_EXP_MEST>("api/HisExpMest/UpdateReason", ApiConsumers.MosConsumer, sdo, param);
                if (rs != null) success = true;
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmReasonExp_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                DisposeForm();
                if (this.refeshData != null) this.refeshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisposeForm()
        {
            try
            {
                roomId = 0;
                refeshData = null;
                positionHandle = 0;
                expMest = null;
                this.simpleButton1.Click -= new System.EventHandler(this.simpleButton1_Click);
                this.bbtnSave.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnSave_ItemClick);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.frmReasonExp_FormClosed);
                this.Load -= new System.EventHandler(this.frmReasonExp_Load);
                customGridLookUpEdit1View.GridControl.DataSource = null;
                customGridLookUpEdit1View = null;
                cboReasonExp = null;
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                bbtnSave = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                simpleButton1 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
