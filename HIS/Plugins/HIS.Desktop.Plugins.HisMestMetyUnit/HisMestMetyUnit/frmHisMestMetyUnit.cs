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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Modules;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.LibraryMessage;
using System.Resources;
using System.Collections;
using HIS.Desktop.Plugins.HisMestMetyUnit.Resources;
using Inventec.Common.Resource;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.CustomControl;

namespace HIS.Desktop.Plugins.HisMestMetyUnit.HisMestMetyUnit
{
    public partial class frmHisMestMetyUnit : FormBase
    {
        #region Declare Variable
        Module ModuleData;
        int ActiveType = -1;
        int RowCount = 0;
        int dataTotal = 0;
        int Start = 0;
        V_HIS_MEST_METY_UNIT CurrentData;
        #endregion
        public frmHisMestMetyUnit(Module Module)
            : base(Module)
        {
            try
            {
                InitializeComponent();
                this.ModuleData = Module;
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
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }

        }

        private void frmHisMestMetyUnit_Load(object sender, EventArgs e)
        {
            try
            {

                MeShow();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        #region ---Set Data
        private void MeShow()
        {
            try
            {
                SetDafaultData();

                EnableControlChanged(this.ActiveType);

                ValidateForm();

                SetCaptionByLanguageKey();

                fillDataToGridcontrol();

                LoadDataToCombobox();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.ModuleData != null && !String.IsNullOrEmpty(this.ModuleData.text))
                {
                    this.Text = ModuleData.text;
                }
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMestMetyUnit.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMestMetyUnit.HisMestMetyUnit.frmHisMestMetyUnit).Assembly);
                this.btnAdd.Text = Get.Value("frmHisMestMetyUnit.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Get.Value("frmHisMestMetyUnit.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Get.Value("frmHisMestMetyUnit.btnCancel.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Get.Value("frmHisMestMetyUnit.btnSearch.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMediStockID.Text = Get.Value("frmHisMestMetyUnit.lcMediStockID.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMedicineID.Text = Get.Value("frmHisMestMetyUnit.lcMedicineID.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcUseOriginalUnitForPres.Text = Get.Value("frmHisMestMetyUnit.lcUseOriginalUnitForPres.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Get.Value("frmHisMestMetyUnit.Caption.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Get.Value("frmHisMestMetyUnit.ToolTip.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediStockID.Caption = Get.Value("frmHisMestMetyUnit.grdColMediStockID.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMediStockID.ToolTip = Get.Value("frmHisMestMetyUnit.grdColMediStockID.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicineTypeID.Caption = Get.Value("frmHisMestMetyUnit.grdColMedicineTypeID.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMedicineTypeID.ToolTip = Get.Value("frmHisMestMetyUnit.grdColMedicineTypeID.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColUseOriginalUnitForPres.Caption = Get.Value("frmHisMestMetyUnit.grdColUseOriginalUnitForPres.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColUseOriginalUnitForPres.ToolTip = Get.Value("frmHisMestMetyUnit.grdColUseOriginalUnitForPres.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Get.Value("frmHisMestMetyUnit.grdColIsActive.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.ToolTip = Get.Value("frmHisMestMetyUnit.grdColIsActive.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCreateTime.Caption = Get.Value("frmHisMestMetyUnit.grdCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCreateTime.ToolTip = Get.Value("frmHisMestMetyUnit.grdCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Get.Value("frmHisMestMetyUnit.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Get.Value("frmHisMestMetyUnit.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Get.Value("frmHisMestMetyUnit.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Get.Value("frmHisMestMetyUnit.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Get.Value("frmHisMestMetyUnit.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Get.Value("frmHisMestMetyUnit.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDafaultData()
        {
            try
            {
                this.ActiveType = GlobalVariables.ActionAdd;
                cboMedicineTypeID.EditValue = null;
                c.EditValue = null;
                chkUseOriginalUnitForPres.CheckState = CheckState.Unchecked;
                txtSearch.Text = "";
                txtSearch.Focus();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int activeType)
        {
            try
            {
                btnAdd.Enabled = (activeType == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (activeType == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidateEditorControl(cboMedicineTypeID);
                ValidateEditorControl(c);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void ValidateEditorControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule vali = new ControlEditValidationRule();
                vali.editor = control;
                vali.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, vali);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void fillDataToGridcontrol()
        {
            try
            {
                WaitingManager.Show();
                int pageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pageSize));
                CommonParam param = new CommonParam();
                param.Limit = RowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pageSize, this.gridControlMestMetyUnit);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object Param)
        {
            try
            {
                this.Start = ((CommonParam)Param).Start ?? 0;
                int limit = ((CommonParam)Param).Limit ?? 0;
                CommonParam commonparam = new CommonParam(Start, limit);
                ApiResultObject<List<V_HIS_MEST_METY_UNIT>> apiResult = null;
                HisMestMetyUnitViewFilter filter = new HisMestMetyUnitViewFilter();
                SetFilter(ref filter);
                gridControlMestMetyUnit.DataSource = null;
                gridViewMestMetyUnit.BeginUpdate();
                apiResult = new BackendAdapter(commonparam).GetRO<List<V_HIS_MEST_METY_UNIT>>(HisRequestUriStore.HisMestMetyUnit_GetView, ApiConsumers.MosConsumer, filter, commonparam);
                if (apiResult != null)
                {
                    var Data = (List<V_HIS_MEST_METY_UNIT>)apiResult.Data;
                    if (Data != null && Data.Count > 0)
                    {
                        gridControlMestMetyUnit.DataSource = Data;
                        RowCount = (Data == null ? 0 : Data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    }
                }
                gridViewMestMetyUnit.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(commonparam);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisMestMetyUnitViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCombobox()
        {
            try
            {
                LoadDataTocboMediStockID();
                LoadDataTocboMedicineTypeID();

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void LoadDataTocboMediStockID()
        {
            try
            {
                List<V_HIS_MEDI_STOCK> data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columminfor = new List<ColumnInfo>();
                columminfor.Add(new ColumnInfo("MEDI_STOCK_CODE", "Mã kho thuốc", 100, 1));
                columminfor.Add(new ColumnInfo("MEDI_STOCK_NAME", "Tên kho thuốc", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDI_STOCK_NAME", "ID", columminfor, true, 300);
                ControlEditorLoader.Load(c, data, controlEditorADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void LoadDataTocboMedicineTypeID()
        {
            try
            {
                List<V_HIS_MEDICINE_TYPE> data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columninfo = new List<ColumnInfo>();
                columninfo.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "Mã loại thuốc", 100, 1));
                columninfo.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "Tên loại thuốc", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columninfo, true, 300);
                ControlEditorLoader.Load(cboMedicineTypeID, data, controlEditorADO);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FilldatatoEditorControl(V_HIS_MEST_METY_UNIT data)
        {
            try
            {
                if (data != null)
                {
                    c.EditValue = data.MEDI_STOCK_ID;
                    cboMedicineTypeID.EditValue = data.MEDICINE_TYPE_ID;
                    chkUseOriginalUnitForPres.Checked = (data.USE_ORIGINAL_UNIT_FOR_PRES == 1 ? true : false);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SaveProcessors()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                CommonParam param = new CommonParam();
                V_HIS_MEST_METY_UNIT UpdateDTO = new V_HIS_MEST_METY_UNIT();
                UpdateDTOFromDataForm(UpdateDTO);
                WaitingManager.Show();
                if (this.ActiveType == GlobalVariables.ActionAdd)
                {
                    var Result = new BackendAdapter(param).Post<V_HIS_MEST_METY_UNIT>(HisRequestUriStore.HisMestMetyUnit_Create, ApiConsumers.MosConsumer, UpdateDTO, null);
                    if (Result != null)
                    {
                        BackendDataWorker.Reset<V_HIS_MEST_METY_UNIT>();
                        fillDataToGridcontrol();
                        success = true;
                        RestFormData();
                    }
                }
                else
                {
                    if (CurrentData != null)
                    {
                        UpdateDTO.ID = CurrentData.ID;
                        var Result = new BackendAdapter(param).Post<V_HIS_MEST_METY_UNIT>(HisRequestUriStore.HisMestMetyUnit_Update, ApiConsumers.MosConsumer, UpdateDTO, null);
                        if (Result != null)
                        {
                            BackendDataWorker.Reset<V_HIS_MEST_METY_UNIT>();
                            fillDataToGridcontrol();
                            success = true;
                        }
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void UpdateDTOFromDataForm(V_HIS_MEST_METY_UNIT data)
        {
            try
            {
                if (c.EditValue != null)
                {
                    data.MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64((c.EditValue ?? "").ToString());
                }

                if (cboMedicineTypeID.EditValue != null)
                {
                    data.MEDICINE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineTypeID.EditValue ?? "").ToString());
                }
                data.USE_ORIGINAL_UNIT_FOR_PRES = (short)(chkUseOriginalUnitForPres.Checked ? 1 : 0);

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void RestFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            c.Focus();

                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---even Button

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessors();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessors();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActiveType);
                RestFormData();
                txtSearch.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                c.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActiveType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActiveType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                c.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                fillDataToGridcontrol();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                V_HIS_MEST_METY_UNIT HisMestMetyUnit = new V_HIS_MEST_METY_UNIT();
                if (MessageBox.Show(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var dataRow = (V_HIS_MEST_METY_UNIT)gridViewMestMetyUnit.GetFocusedRow();
                    if (dataRow != null)
                    {
                        WaitingManager.Show();
                        HisMestMetyUnit = new BackendAdapter(param).Post<V_HIS_MEST_METY_UNIT>(HisRequestUriStore.HisMestMetyUnit_ChangeLock, ApiConsumers.MosConsumer, dataRow.ID, null);
                        if (HisMestMetyUnit != null)
                        {
                            BackendDataWorker.Reset<V_HIS_MEST_METY_UNIT>();
                            fillDataToGridcontrol();
                            success = true;
                            WaitingManager.Hide();
                            btnEdit.Enabled = false;
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    CommonParam param = new CommonParam();
                    bool success = false;
                    V_HIS_MEST_METY_UNIT HisMestMetyUnit = new V_HIS_MEST_METY_UNIT();
                    if (MessageBox.Show(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        var dataRow = (V_HIS_MEST_METY_UNIT)gridViewMestMetyUnit.GetFocusedRow();
                        if (dataRow != null)
                        {
                            WaitingManager.Show();
                            HisMestMetyUnit = new BackendAdapter(param).Post<V_HIS_MEST_METY_UNIT>(HisRequestUriStore.HisMestMetyUnit_ChangeLock, ApiConsumers.MosConsumer, dataRow.ID, null);
                            if (HisMestMetyUnit != null)
                            {
                                BackendDataWorker.Reset<V_HIS_MEST_METY_UNIT>();
                                fillDataToGridcontrol();
                                success = true;
                                WaitingManager.Hide();
                            }
                        }
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                }
                catch (Exception ex)
                {

                    LogSystem.Error(ex);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var dataRow = (V_HIS_MEST_METY_UNIT)gridViewMestMetyUnit.GetFocusedRow();
                if (MessageBox.Show(MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (dataRow != null)
                    {
                        WaitingManager.Show();
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisMestMetyUnit_Delete, ApiConsumers.MosConsumer, dataRow.ID, null);
                        WaitingManager.Hide();
                        if (success)
                        {
                            BackendDataWorker.Reset<V_HIS_MEST_METY_UNIT>();
                            fillDataToGridcontrol();
                            this.ActiveType = GlobalVariables.ActionAdd;
                            EnableControlChanged(this.ActiveType);
                            c.EditValue = null;
                            cboMedicineTypeID.EditValue = null;
                            chkUseOriginalUnitForPres.CheckState = CheckState.Unchecked;
                            txtSearch.Text = "";
                            txtSearch.Focus();
                            Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, success);

                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
                WaitingManager.Hide();
            }

        }
        #endregion

        #region ---even Cobobox
        private void cboMediStockID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (c.EditValue != null && c.EditValue != c.OldEditValue)
                    {
                        cboMedicineTypeID.Focus();

                    }
                    else
                    {
                        c.ShowPopup();
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboMediStockID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (c.EditValue != null && c.EditValue != c.EditValue)
                    {
                        cboMedicineTypeID.Focus();
                    }
                    else
                    {
                        c.ShowPopup();
                    }
                }
                else
                {
                    c.ShowPopup();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboMedicineTypeID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMedicineTypeID.EditValue != null && cboMedicineTypeID.EditValue != cboMedicineTypeID.OldEditValue)
                    {
                        chkUseOriginalUnitForPres.Focus();

                    }
                    else
                    {
                        cboMedicineTypeID.SelectAll();
                        cboMedicineTypeID.ShowPopup();
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void cboMedicineTypeID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboMedicineTypeID.EditValue != null && cboMedicineTypeID.EditValue != cboMedicineTypeID.EditValue)
                    {
                        chkUseOriginalUnitForPres.Focus();
                    }
                    else
                    {
                        cboMedicineTypeID.ShowPopup();
                    }
                }
                else
                {
                    cboMedicineTypeID.SelectAll();
                    cboMedicineTypeID.ShowPopup();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void chkUseOriginalUnitForPres_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {

                if (e.KeyCode == Keys.Space)
                {
                    chkUseOriginalUnitForPres.Checked = !chkUseOriginalUnitForPres.Checked;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActiveType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else if (this.ActiveType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnCancel.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region---even GridView

        private void gridViewMestMetyUnit_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (V_HIS_MEST_METY_UNIT)gridViewMestMetyUnit.GetFocusedRow();
                if (data != null)
                {
                    CurrentData = data;
                    FilldatatoEditorControl(data);
                    this.ActiveType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActiveType);
                    btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewMestMetyUnit_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_MEST_METY_UNIT data = (V_HIS_MEST_METY_UNIT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + Start;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? "Hoạt động" : "Tạm khóa");
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "USE_ORIGINAL_UNIT_FOR_PRES_STR")
                        {
                            e.Value = (data.USE_ORIGINAL_UNIT_FOR_PRES == 1 ? true : false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewMestMetyUnit_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView views = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_MEST_METY_UNIT DataRow = (V_HIS_MEST_METY_UNIT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (DataRow.IS_ACTIVE == 1 ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (DataRow.IS_ACTIVE == 1 ? btnDelete : btnEnableDelete);
                    }

                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewMestMetyUnit_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_MEST_METY_UNIT data = (V_HIS_MEST_METY_UNIT)gridViewMestMetyUnit.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            {
                                e.Appearance.ForeColor = Color.Red;
                            }
                            else
                            {
                                e.Appearance.ForeColor = Color.Green;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        private void cboMediStockID_Click(object sender, EventArgs e)
        {
            try
            {
                c.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
