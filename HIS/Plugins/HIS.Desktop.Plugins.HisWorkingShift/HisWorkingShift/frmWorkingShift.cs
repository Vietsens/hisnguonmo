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
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.Common;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.WebApiClient;
using HIS.Desktop.ApiConsumer;
using DevExpress.XtraEditors;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using System.Resources;


namespace HIS.Desktop.Plugins.HisWorkingShift.HisWorkingShift
{
    public partial class frmWorkingShift : HIS.Desktop.Utility.FormBase
    {
        #region ---Decalre---
        Module Currentmodule;
        RefeshReference refeshReference;
        int ActionType = -1;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HIS_WORKING_SHIFT currentData;
        #endregion
        public frmWorkingShift(Module module)
            : this(null, null)
        {

        }
        public frmWorkingShift(Module module, RefeshReference reference)
            : base(module)
        {
            InitializeComponent();
            this.refeshReference = reference;
            this.Currentmodule = module;
            try
            {
                if (this.Currentmodule != null && !String.IsNullOrEmpty(this.Currentmodule.text))
                {
                    this.Text = this.Currentmodule.text;
                }
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.AddBarManager(this.barManager1);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmWorkingShift_Load(object sender, EventArgs e)
        {
            try
            {
                Validate();
                SetDataDefaut();
                EnableControlChange(this.ActionType);
                LoadDataToGridControl();
                SetCapitionByLanguageKey();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #region ---PreviewKeyDown---
        private void txtWorkingShiftCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtWorkingShiftName.Focus();
                    txtWorkingShiftName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtWorkingShiftName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFromTime.Focus();
                    txtFromTime.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFromTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtToTime.Focus();
                    txtToTime.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtToTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (btnAdd.Enabled)
                        btnAdd.Focus();
                    else if (btnEdit.Enabled)
                        btnEdit.Focus();
                    else
                        btnCancel.Focus();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Validate---
        private void Validate()
        {
            try
            {
                ValidateMaxlength(txtWorkingShiftCode, true, 2);
                ValidateMaxlength(txtWorkingShiftName, true, 100);
                ValidateMaxlength(txtFromTime, false, 6);
                ValidateMaxlength(txtToTime, false, 6);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void ValidateMaxlength(DevExpress.XtraEditors.BaseEdit control, bool IsRequired, int maxlength)
        {
            try
            {
                ControlMaxLengthValidationRule valie = new ControlMaxLengthValidationRule();
                valie.editor = control;
                valie.maxLength = maxlength;
                valie.IsRequired = IsRequired;
                valie.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                valie.ErrorText = "Nhập quá ký tự cho phép (" + maxlength + ")";
                dxValidationProvider1.SetValidationRule(control, valie);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---SetData---
        private void SetCapitionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisWorkingShift.Resources.Lang", typeof(HIS.Desktop.Plugins.HisWorkingShift.HisWorkingShift.frmWorkingShift).Assembly);
                this.lcWorkingShiftCode.Text = Setlanguage("txtWorkingShiftCode.Text");
                this.lcWorkingShiftName.Text = Setlanguage("txtWorkingShiftName.Text");
                this.lcFromTime.Text = Setlanguage("txtFromTime.Text");
                this.lcToTime.Text = Setlanguage("txtToTime.Text");
                this.grdColWorkingShiftCode.Caption = Setlanguage("grdColWorkingShiftCode.Caption");
                this.grdColWorkingShiftCode.ToolTip = Setlanguage("grdColWorkingShiftCode.ToolTip");
                this.grdColWorkingShiftName.Caption = Setlanguage("grdColWorkingShiftName.Caption");
                this.grdColWorkingShiftName.ToolTip = Setlanguage("grdColWorkingShiftName.ToolTip");
                this.grdColFromTo.Caption = Setlanguage("grdColFromTo.Caption");
                this.grdColFromTo.ToolTip = Setlanguage("grdColFromTo.ToolTip");
                this.grdColToTime.Caption = Setlanguage("grdColToTime.Caption");
                this.grdColToTime.ToolTip = Setlanguage("grdColToTime.ToolTip");
                this.grdColIsActive.Caption = Setlanguage("grdColIsActive.Caption");
                this.grdColIsActive.ToolTip = Setlanguage("grdColIsActive.ToolTip");
                this.grdColCreateTime.Caption = Setlanguage("grdColCreateTime.Caption");
                this.grdColCreateTime.ToolTip = Setlanguage("grdColCreateTime.ToolTip");
                this.grdColCreator.Caption = Setlanguage("grdColCreator.Caption");
                this.grdColCreator.ToolTip = Setlanguage("grdColCreator.ToolTip");
                this.grdColModifyTime.Caption = Setlanguage("grdColModifyTime.Caption");
                this.grdColModifyTime.ToolTip = Setlanguage("grdColModifyTime.ToolTip");
                this.grdColModifier.Caption = Setlanguage("grdColModifier.Caption");
                this.grdColModifier.ToolTip = Setlanguage("grdColModifier.ToolTip");
                this.btnAdd.Text = Setlanguage("btnAdd.Text");
                this.btnEdit.Text = Setlanguage("btnEdit.Text");
                this.btnCancel.Text = Setlanguage("btnCancel.Text");
                
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private String Setlanguage(string KeyCaption)
        {
            string keycaption = "";
            try
            {
                keycaption = Inventec.Common.Resource.Get.Value("HisWorkingShift." + KeyCaption, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                keycaption = "";
                LogSystem.Warn(ex);
            }
            return keycaption;
        }
        private void SetDataDefaut()
        {
            try
            {
                txtWorkingShiftCode.Text = "";
                txtWorkingShiftName.Text = "";
                txtFromTime.Text = "";
                txtToTime.Text = "";
                this.ActionType = GlobalVariables.ActionAdd;

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void EnableControlChange(int action)
        {
            try
            {
                this.btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
                this.btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //Load data to gridcontrol
        private void LoadDataToGridControl()
        {
            try
            {
                WaitingManager.Show();

                int numPageSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    numPageSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPaging(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, numPageSize, this.GridControlWorKingShift);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_WORKING_SHIFT>> apiResuilt = null;
                HisWorkingShiftFilter filter = new HisWorkingShiftFilter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                GridViewWorKingShift.BeginUpdate();
                apiResuilt = new BackendAdapter(paramCommon).GetRO<List<HIS_WORKING_SHIFT>>(HisRequestUriStore.WorkingShift_GET, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResuilt != null)
                {
                    var data = apiResuilt.Data;
                    if (data != null && data.Count > 0)
                    {
                        GridControlWorKingShift.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResuilt.Param == null ? 0 : apiResuilt.Param.Count ?? 0);
                    }
                }
                GridViewWorKingShift.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        //
        // Update data
        private void ProcessorSave()
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_WORKING_SHIFT UpdateDTO = new HIS_WORKING_SHIFT();
                UpDataDTOFromDataForm(ref UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var Result = new BackendAdapter(param).Post<HIS_WORKING_SHIFT>(HisRequestUriStore.WorkingShift_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (Result != null)
                    {
                        BackendDataWorker.Reset<HIS_WORKING_SHIFT>();
                        success = true;
                        LoadDataToGridControl();
                        btnCancel_Click(null, null);
                    }
                }
                else
                {
                    if (this.currentData != null)
                    {
                        UpdateDTO.ID = this.currentData.ID;
                        var Resutl = new BackendAdapter(param).Post<HIS_WORKING_SHIFT>(HisRequestUriStore.WorkingShift_UPDATE, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (Resutl != null)
                        {
                            BackendDataWorker.Reset<HIS_WORKING_SHIFT>();
                            success = true;
                            LoadDataToGridControl();

                        }
                    }
                }
                WaitingManager.Hide();
                #region ---Thong bao---
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void UpDataDTOFromDataForm(ref HIS_WORKING_SHIFT data)
        {
            try
            {
                data.WORKING_SHIFT_CODE = txtWorkingShiftCode.Text;
                data.WORKING_SHIFT_NAME = txtWorkingShiftName.Text;
                data.FROM_TIME = txtFromTime.Text;
                data.TO_TIME = txtToTime.Text;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        //
        //Set data defaut to control 
        private void RestFormData()
        {
            try
            {
                if (!lcInfor.IsInitialized)
                    return;
                lcInfor.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcInfor.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtWorkingShiftCode.Focus();
                            txtWorkingShiftCode.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcInfor.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedataRow(HIS_WORKING_SHIFT data)
        {
            try
            {
                if (data != null)
                {
                    FillDatatoControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChange(this.ActionType);
                    this.btnEdit.Enabled = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);

                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void FillDatatoControl(HIS_WORKING_SHIFT data)
        {
            try
            {
                if (data != null)
                {
                    txtWorkingShiftCode.Text = data.WORKING_SHIFT_CODE;
                    txtWorkingShiftName.Text = data.WORKING_SHIFT_NAME;
                    txtFromTime.Text = data.FROM_TIME;
                    txtToTime.Text = data.TO_TIME;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
        #region ---Even Button---
        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnEdit.Enabled && this.ActionType == GlobalVariables.ActionEdit)
                    btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnAdd.Enabled && this.ActionType == GlobalVariables.ActionAdd)
                    btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void F2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtWorkingShiftCode.Focus();
                txtWorkingShiftCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.ProcessorSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChange(this.ActionType);
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFormData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #region ---Even GridControl---
        private void GridViewWorKingShift_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (datarow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.startPage;
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = (datarow.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewWorKingShift_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)((System.Collections.IList)((DevExpress.XtraGrid.Views.Base.BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnLock : btnUnLock);
                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        e.RepositoryItem = (datarow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnDelete : btnVisibleDetele);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void GridViewWorKingShift_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views
                    .Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    HIS_WORKING_SHIFT dataRow = (HIS_WORKING_SHIFT)GridViewWorKingShift.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        e.Appearance.ForeColor = (dataRow.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? Color.Green : Color.Red);
                    }
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void GridViewWorKingShift_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)GridViewWorKingShift.GetFocusedRow();
                if (datarow != null)
                {
                    this.currentData = datarow;
                    ChangedataRow(datarow);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        #endregion
        #region ---btn Lock and Delete
        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)GridViewWorKingShift.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_WORKING_SHIFT>(HisRequestUriStore.WorkingShift_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)GridViewWorKingShift.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        var Result = new BackendAdapter(param).Post<HIS_WORKING_SHIFT>(HisRequestUriStore.WorkingShift_CHANGELOCK, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (Result != null)
                        {
                            LoadDataToGridControl();
                            success = true;
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
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

                HIS_WORKING_SHIFT datarow = (HIS_WORKING_SHIFT)GridViewWorKingShift.GetFocusedRow();
                if (datarow != null)
                {
                    if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        WaitingManager.Show();
                        bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.WorkingShift_DELETE, ApiConsumers.MosConsumer, datarow.ID, param);
                        if (success)
                        {
                            LoadDataToGridControl();
                            btnCancel_Click(null, null);
                        }
                        WaitingManager.Hide();
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    LoadDataToGridControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
    }

}
