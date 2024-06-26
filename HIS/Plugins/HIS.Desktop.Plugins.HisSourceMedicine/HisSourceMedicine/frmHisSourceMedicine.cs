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
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System.Collections;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.HisSourceMedicine.Properties;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisSourceMedicine.Resources;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.HisSourceMedicine.HisSourceMedicine
{
    public partial class frmHisSourceMedicine : HIS.Desktop.Utility.FormBase
    {
        #region
        PagingGrid pagingGrid;
        Module moduleData;
        int ActionType = -1;
        int startPage = 0;
        int dataTotal = 0;
        int rowCount = 0;
        HIS_SOURCE_MEDICINE CurrentData;
        #endregion

        public frmHisSourceMedicine()
        {
            InitializeComponent();
            
        }

        public frmHisSourceMedicine(Module module) : base(module)
        {
            try
            {
                this.StartPosition = FormStartPosition.CenterScreen;
                InitializeComponent();
                pagingGrid = new PagingGrid();
                this.moduleData = module;
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmHisSourceMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                
                SetDefautData();
                EnableControlChanged(this.ActionType);
                SetCaptionByLanguageKey();
                FillDataToGridControl();
                ValidateWarningText(txtSourceMedicineCode, 2);
                ValidateWarningText(txtSourceMedicineName, 200);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.moduleData != null && !String.IsNullOrEmpty(this.moduleData.text))
                {
                    this.Text = moduleData.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisSourceMedicine.Resources.Lang", typeof(HIS.Desktop.Plugins.HisSourceMedicine.HisSourceMedicine.frmHisSourceMedicine).Assembly);

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlGroup1.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.layoutControlGroup1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Root.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.Root.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSourceMedicineCode.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.lciSourceMedicineCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciSourceMedicineName.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.lciSourceMedicineName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSourceMedicineCode.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.txtSourceMedicineCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSourceMedicineName.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.txtSourceMedicineName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.txtSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColHisSourceMedicineCode.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridColHisSourceMedicineCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColHisSourceMedicineName.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridColHisSourceMedicineName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridControlHisSourceMedicine.Text = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.gridControlHisSourceMedicine.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsActive.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.grdColIsActive.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisSourceMedicine.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogAction.Warn(ex);
            }
        }

        private void EnableControlChanged(int actionType)
        {
            try
            {
                btnAdd.Enabled = (actionType == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (actionType == GlobalVariables.ActionEdit);

            }
            catch (Exception ex)
            {

                LogAction.Warn(ex);
            }
        }

        private void SetDefautData()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSourceMedicineCode.Text = "";
                txtSourceMedicineName.Text = "";
                txtSearch.Text = "";
                txtSearch.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewHisSourceMedicine_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (HIS_SOURCE_MEDICINE)gridViewHisSourceMedicine.GetFocusedRow();
                if (datarow != null)
                {
                    this.CurrentData = datarow;
                    ChangeDataRow(datarow);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeDataRow(HIS_SOURCE_MEDICINE datarow)
        {
            try
            {
                if (datarow != null)
                {
                    RestFromData();
                    FillDataEditorControl(datarow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    if (datarow != null)
                    {
                        btnEdit.Enabled = (datarow.IS_ACTIVE == 1);
                    }
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(HIS_SOURCE_MEDICINE datarow)
        {
            try
            {
                txtSourceMedicineCode.Text = datarow.SOURCE_MEDICINE_CODE;
                txtSourceMedicineName.Text = datarow.SOURCE_MEDICINE_NAME;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcessor()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                HIS_SOURCE_MEDICINE UpdateDTO = new HIS_SOURCE_MEDICINE();
                if (CurrentData != null && this.CurrentData.ID > 0)
                {
                    LoadCurrent(this.CurrentData.ID, ref UpdateDTO);
                }
                UpdateDataFromform(ref UpdateDTO);
                WaitingManager.Show();
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var ResultData = new BackendAdapter(param).Post<HIS_SOURCE_MEDICINE>(HisSourceMedicineUriStore.HisSourceMedicine_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {                     
                        success = true;
                        FillDataToGridControl();
                        RestFromData();
                    }
                }
                else
                {
                    var ResultData = new BackendAdapter(param).Post<HIS_SOURCE_MEDICINE>(HisSourceMedicineUriStore.HisSourceMedicine_Update, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<HIS_SOURCE_MEDICINE>();
                        success = true;
                        FillDataToGridControl();                       
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

        private void LoadCurrent(long currentId, ref HIS_SOURCE_MEDICINE currentDTO)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSourceMedicineFilter filter = new HisSourceMedicineFilter();
                filter.ID = currentId;
                currentDTO = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SOURCE_MEDICINE>>("api/HisSourceMedicine/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    pagingSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pagingSize, this.gridControlHisSourceMedicine);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object commonParam)
        {
            try
            {
                startPage = ((CommonParam)commonParam).Start ?? 0;
                int limit = ((CommonParam)commonParam).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_SOURCE_MEDICINE>> apiResult = null;
                HisSourceMedicineFilter filter = new HisSourceMedicineFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.KEY_WORD = txtSearch.Text.Trim();
                }
                gridControlHisSourceMedicine.DataSource = null;
                gridViewHisSourceMedicine.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_SOURCE_MEDICINE>>(HisSourceMedicineUriStore.HisSourceMedicine_Get, ApiConsumers.MosConsumer, filter, paramcommon);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("apiResult--------------------" + Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult));
                if (apiResult != null)
                {
                    var data = (List<HIS_SOURCE_MEDICINE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlHisSourceMedicine.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewHisSourceMedicine.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterNavBar(ref HisSourceMedicineFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void UpdateDataFromform(ref HIS_SOURCE_MEDICINE currentDTO)
        {
            try
            {
                currentDTO.SOURCE_MEDICINE_CODE = txtSourceMedicineCode.Text.Trim();
                currentDTO.SOURCE_MEDICINE_NAME = txtSourceMedicineName.Text.Trim();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                this.CurrentData = null;               
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                RestFromData();
                txtSourceMedicineCode.Focus();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RestFromData()
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
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtSourceMedicineCode.Focus();
                            txtSourceMedicineCode.SelectAll();
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

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
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
                SaveProcessor();
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
                FillDataToGridControl();
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
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
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
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }
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

        private void bbtnReset_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewHisSourceMedicine_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (HIS_SOURCE_MEDICINE)gridViewHisSourceMedicine.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == 0)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewHisSourceMedicine_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_SOURCE_MEDICINE DataRow = (HIS_SOURCE_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (DataRow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = (DataRow.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewHisSourceMedicine_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (HIS_SOURCE_MEDICINE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnDisableDelete : btnEnableDelete);
                        }
                        if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnLook : btnUnLook);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtSourceMedicineCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSourceMedicineName.Focus();
                    txtSourceMedicineName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtSourceMedicineName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                    {
                        btnAdd.Focus();
                    }
                    else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                    {
                        btnEdit.Focus();
                    }
                    else
                        btnReset.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnLook_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_SOURCE_MEDICINE)gridViewHisSourceMedicine.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        HIS_SOURCE_MEDICINE Result = new BackendAdapter(param).Post<HIS_SOURCE_MEDICINE>(HisSourceMedicineUriStore.HisSourceMedicine_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (Result != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            btnEdit.Enabled = false;
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnUnLook_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_SOURCE_MEDICINE)gridViewHisSourceMedicine.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        HIS_SOURCE_MEDICINE Result = new BackendAdapter(param).Post<HIS_SOURCE_MEDICINE>(HisSourceMedicineUriStore.HisSourceMedicine_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (Result != null)
                        {
                            success = true;
                            FillDataToGridControl();
                            btnEdit.Enabled = true;
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnEnableDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (HIS_SOURCE_MEDICINE)gridViewHisSourceMedicine.GetFocusedRow();
                bool success = false;
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisSourceMedicineUriStore.HisSourceMedicine_Delete, ApiConsumer.ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_SOURCE_MEDICINE>();
                            FillDataToGridControl();
                            btnReset_Click(null, null);
                        }
                    }
                    MessageManager.Show(this, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void txtSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ValidateWarningText(DevExpress.XtraEditors.TextEdit textcontrol, int maxlangth)
        {
            try
            {
                ValidateMaxLength vali = new ValidateMaxLength();
                vali.txtEdit = textcontrol;
                vali.Maxlength = maxlangth;
                vali.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                vali.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textcontrol, vali);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
