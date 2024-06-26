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
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Modules;
using Inventec.UC.Paging;
using LIS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using LIS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;
using LIS.Desktop.Plugins.LisBacteriumFamily.Resources;
using System.Resources;
using Inventec.Common.Resource;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.LibraryMessage;

namespace LIS.Desktop.Plugins.LisBacteriumFamily.LisBacteriumFamily
{
    public partial class frmLisBacteriumFamily : FormBase
    {
        #region ---Declare
        Module moduleData;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int ActiveTypeFamily = -1;
        int startPage = 0;
        int dataTotal = 0;
        int rowCount = 0;
        int startPagefamil = 0;
        int dataTotalfamil = 0;
        int rowCountfamil = 0;
        LIS_BACTERIUM_FAMILY CurrentDatafamil;
        LIS_BACTERIUM CurrentData;
        #endregion
        public frmLisBacteriumFamily(Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();

                this.moduleData = module;
                pagingGrid = new PagingGrid();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void frmLisBacteriumFamily_Load(object sender, EventArgs e)
        {
            try
            {
                SetDataDefaut();
                EnableControlChanged(this.ActionType);
                EnableControlChangedFamily(this.ActiveTypeFamily);
                SetCaptionByLanguageKey();
                Validate();
                FillDataToGridControl();
                FillDataToGridControlFamil();
                InitFillDataToCombo();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetDataDefaut()
        {
            try
            {
                txtBacteriumCode.Text = "";
                txtBacteriumFamilyCode.Text = "";
                txtBacteriumFamilyName.Text = "";
                txtBacteriumName.Text = "";
                cboBacteriumFamilyID.EditValue = null;
                this.ActionType = GlobalVariables.ActionAdd;
                this.ActiveTypeFamily = GlobalVariables.ActionAdd;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int active)
        {
            try
            {
                btnAdd.Enabled = (active == GlobalVariables.ActionAdd);
                btnEdit.Enabled = (active == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChangedFamily(int activeTypeFamily)
        {
            try
            {
                btnAddFamily.Enabled = (activeTypeFamily == GlobalVariables.ActionAdd);
                btnEditFamily.Enabled = (activeTypeFamily == GlobalVariables.ActionEdit);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                ResourceLanguageManager.LanguageResource = new ResourceManager("LIS.Desktop.Plugins.LisBacteriumFamily.Resources.Lang", typeof(LIS.Desktop.Plugins.LisBacteriumFamily.LisBacteriumFamily.frmLisBacteriumFamily).Assembly);
                this.btnAddFamily.Text = Get.Value("LisBacteriumFamily.btnAddFamily.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEditFamily.Text = Get.Value("LisBacteriumFamily.btnEditFamily.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefreshFamily.Text = Get.Value("LisBacteriumFamily.btnRefreshFamily.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBacteriumFamilyCode.Text = Get.Value("LisBacteriumFamily.lcBacteriumFamilyCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBacteriumFamilyName.Text = Get.Value("LisBacteriumFamily.lcBacteriumFamilyName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Get.Value("LisBacteriumFamily.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Get.Value("LisBacteriumFamily.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Get.Value("LisBacteriumFamily.btnRefresh.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBacteriumCode.Text = Get.Value("LisBacteriumFamily.lcBacteriumCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBacteriumName.Text = Get.Value("LisBacteriumFamily.lcBacteriumName.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBacteriumFamilyID.Text = Get.Value("LisBacteriumFamily.lcBacteriumFamilyID.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriumFamilyCode.Caption = Get.Value("LisBacteriumFamily.grdcolBacteriumFamilyCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriumFamilyCode.ToolTip = Get.Value("LisBacteriumFamily.grdcolBacteriumFamilyCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                 this.grdColBacteriumFamilyName.Caption = Get.Value("LisBacteriumFamily.grdColBacteriumFamilyName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBacteriumFamilyName.ToolTip = Get.Value("LisBacteriumFamily.grdColBacteriumFamilyName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyIsActive.Caption = Get.Value("LisBacteriumFamily.grdcolFamilyIsActive.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyIsActive.ToolTip = Get.Value("LisBacteriumFamily.grdcolFamilyIsActive.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyCreateTime.Caption = Get.Value("LisBacteriumFamily.grdcolFamilyCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyCreateTime.ToolTip = Get.Value("LisBacteriumFamily.grdcolFamilyCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyCreator.Caption = Get.Value("LisBacteriumFamily.grdcolFamilyCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyCreator.ToolTip = Get.Value("LisBacteriumFamily.grdcolFamilyCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyModyfyTime.Caption = Get.Value("LisBacteriumFamily.grdcolFamilyModyfyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyModyfyTime.ToolTip = Get.Value("LisBacteriumFamily.grdcolFamilyModyfyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Get.Value("LisBacteriumFamily.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Get.Value("LisBacteriumFamily.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriunCode.Caption = Get.Value("LisBacteriumFamily.grdcolBacteriunCode.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriunCode.ToolTip = Get.Value("LisBacteriumFamily.grdcolBacteriunCode.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdBacteriumName.Caption = Get.Value("LisBacteriumFamily.grdBacteriumName.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdBacteriumName.ToolTip = Get.Value("LisBacteriumFamily.grdBacteriumName.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriunFamnityID.Caption = Get.Value("LisBacteriumFamily.grdcolBacteriunFamnityID.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolBacteriunFamnityID.ToolTip = Get.Value("LisBacteriumFamily.grdcolBacteriunFamnityID.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAcvice.Caption = Get.Value("LisBacteriumFamily.grdColIsAcvice.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsAcvice.ToolTip = Get.Value("LisBacteriumFamily.grdColIsAcvice.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Get.Value("LisBacteriumFamily.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Get.Value("LisBacteriumFamily.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Get.Value("LisBacteriumFamily.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Get.Value("LisBacteriumFamily.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyModifier.Caption = Get.Value("LisBacteriumFamily.grdcolFamilyModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdcolFamilyModifier.ToolTip = Get.Value("LisBacteriumFamily.grdcolFamilyModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Get.Value("LisBacteriumFamily.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Get.Value("LisBacteriumFamily.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                
               
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void Validate()
        {
            try
            {
                validateEditorControlFamil(50, txtBacteriumFamilyCode);
                validateEditorControlFamil(200, txtBacteriumFamilyName);
                validateEditorControl(50, txtBacteriumCode);
                validateEditorControl(200, txtBacteriumName);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void validateEditorControlFamil(int maxlenth, DevExpress.XtraEditors.TextEdit control)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.required = true;
                validRule.txtEdit = control;
                validRule.maxLenght = maxlenth;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void validateEditorControl(int maxlenth, DevExpress.XtraEditors.TextEdit control)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.required = true;
                validRule.txtEdit = control;
                validRule.maxLenght = maxlenth;
                validRule.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, pagingSize, this.grdControlBacterium);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToGridControlFamil()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pagingSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPagingFamil(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountfamil;
                param.Count = dataTotalfamil;
                ucPaging2.Init(LoadPagingFamil, param, pagingSize, this.grdControlBacteriumFamily);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }
        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<LIS_BACTERIUM>> apiResult = null;
                LisBacteriumFilter filter = new LisBacteriumFilter();
                SetFilter(ref filter);
                grdControlBacterium.DataSource = null;
                grdViewBacterium.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<LIS_BACTERIUM>>(HisRequestUriStores.LisBacterium_Get, ApiConsumers.LisConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<LIS_BACTERIUM>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdControlBacterium.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                grdViewBacterium.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadPagingFamil(object param)
        {
            try
            {
                startPagefamil = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPagefamil, limit);
                ApiResultObject<List<LIS_BACTERIUM_FAMILY>> apiResult = null;
                LisBacteriumFamilyFilter filter = new LisBacteriumFamilyFilter();
                SetFilterFamil(ref filter);
                grdControlBacteriumFamily.DataSource = null;
                grdViewBacteriumFamily.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<LIS_BACTERIUM_FAMILY>>(HisRequestUriStores.LisBacteriumFamily_Get, ApiConsumers.LisConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<LIS_BACTERIUM_FAMILY>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdControlBacteriumFamily.DataSource = data;
                        rowCountfamil = (data == null ? 0 : data.Count);
                        dataTotalfamil = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }

                grdViewBacteriumFamily.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref LisBacteriumFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetFilterFamil(ref LisBacteriumFamilyFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void grdViewBacteriumFamily_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (LIS_BACTERIUM_FAMILY)grdViewBacteriumFamily.GetFocusedRow();
                if (datarow != null)
                {
                    CurrentDatafamil = datarow;
                    ChangeDataRowfamil(datarow);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ChangeDataRowfamil(LIS_BACTERIUM_FAMILY datarow)
        {
            try
            {
                if (datarow != null)
                {
                    FillDataEditorControlFamily(datarow);
                    this.ActiveTypeFamily = GlobalVariables.ActionEdit;
                    EnableControlChangedFamily(this.ActiveTypeFamily);
                    if (datarow != null)
                    {
                        btnEditFamily.Enabled = (datarow.IS_ACTIVE == 1);
                    }
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControlFamily(LIS_BACTERIUM_FAMILY datarow)
        {
            try
            {
                if (datarow != null)
                {
                    txtBacteriumFamilyCode.Text = datarow.BACTERIUM_FAMILY_CODE;
                    txtBacteriumFamilyName.Text = datarow.BACTERIUM_FAMILY_NAME;
                    cboBacteriumFamilyID.EditValue = datarow.ID;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void grdViewBacterium_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (LIS_BACTERIUM)grdViewBacterium.GetFocusedRow();
                if (datarow != null)
                {
                    CurrentData = datarow;
                    ChangeDataRow(datarow);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ChangeDataRow(LIS_BACTERIUM datarow)
        {
            try
            {
                if (datarow != null)
                {
                    FillDataEditorControl(datarow);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(this.ActionType);
                    if (datarow != null)
                    {
                        btnEdit.Enabled = (datarow.IS_ACTIVE == 1);
                    }
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider2);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(LIS_BACTERIUM datarow)
        {
            try
            {
                if (datarow != null)
                {
                    txtBacteriumCode.Text = datarow.BACTERIUM_CODE;
                    txtBacteriumName.Text = datarow.BACTERIUM_NAME;
                    cboBacteriumFamilyID.EditValue = datarow.BACTERIUM_FAMILY_ID;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void grdViewBacteriumFamily_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (LIS_BACTERIUM_FAMILY)grdViewBacteriumFamily.GetRow(e.RowHandle);
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

        private void grdViewBacterium_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var data = (LIS_BACTERIUM)grdViewBacterium.GetRow(e.RowHandle);
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

        private void grdViewBacteriumFamily_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (LIS_BACTERIUM_FAMILY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnEnableDeleteFa : btnDeleteFa);
                        }
                        if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnUnLockFa : btnLockFa);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void grdViewBacteriumFamily_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    LIS_BACTERIUM_FAMILY DataRow = (LIS_BACTERIUM_FAMILY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (DataRow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT_FA")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPagefamil;
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

        private void grdViewBacterium_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (LIS_BACTERIUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnEnableDelete : btnDelete);
                        }
                        if (e.Column.FieldName == "LOCK")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnUnLock : btnLock);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void grdViewBacterium_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    LIS_BACTERIUM DataRow = (LIS_BACTERIUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "BACTERIUM_FAMILY_ID_STR")
                        {
                                e.Value = BackendDataWorker.Get<LIS_BACTERIUM_FAMILY>().FirstOrDefault(o => o.ID == DataRow.BACTERIUM_FAMILY_ID).BACTERIUM_FAMILY_NAME;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnRefreshFamily_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActiveTypeFamily = GlobalVariables.ActionAdd;
                EnableControlChangedFamily(this.ActionType);
                RestFromDataFamil();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                txtBacteriumFamilyCode.Focus();
                txtBacteriumFamilyCode.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                RestFromData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider2, dxErrorProvider2);
                txtBacteriumCode.Focus();
                txtBacteriumCode.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void RestFromDataFamil()
        {
            try
            {
                if (!lcEditorInfofamil.IsInitialized)
                    return;
                lcEditorInfofamil.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfofamil.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is DevExpress.XtraEditors.BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfofamil.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void SaveProcessor()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider2.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS_BACTERIUM UpdateDTO = new LIS_BACTERIUM();
                UpdateDataFromform(UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var ResultData = new BackendAdapter(param).Post<LIS_BACTERIUM>(HisRequestUriStores.LisBacterium_Create, ApiConsumers.LisConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<LIS_BACTERIUM>();
                        FillDataToGridControl();
                        success = true;
                        RestFromData();
                        txtBacteriumCode.Focus();
                        txtBacteriumCode.SelectAll();
                    }
                }
                else
                {
                    if (CurrentData != null)
                    {
                        UpdateDTO.ID = CurrentData.ID;
                        var ResultData = new BackendAdapter(param).Post<LIS_BACTERIUM>(HisRequestUriStores.LisBacterium_Update, ApiConsumers.LisConsumer, UpdateDTO, param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<LIS_BACTERIUM>();
                            FillDataToGridControl();
                            success = true;
                            txtBacteriumCode.Focus();
                            txtBacteriumCode.SelectAll();
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

        private void SaveProcessorfamil()
        {
            try
            {
                bool success = false;
                if (!btnAdd.Enabled && !btnEdit.Enabled)
                    return;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LIS_BACTERIUM_FAMILY UpdateDTO = new LIS_BACTERIUM_FAMILY();
                UpdateDataFromformfamil(UpdateDTO);
                if (this.ActiveTypeFamily == GlobalVariables.ActionAdd)
                {
                    var ResultData = new BackendAdapter(param).Post<LIS_BACTERIUM_FAMILY>(HisRequestUriStores.LisBacteriumFamily_Create, ApiConsumers.LisConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<LIS_BACTERIUM_FAMILY>();
                        FillDataToGridControlFamil();
                        success = true;
                        RestFromDataFamil();
                        txtBacteriumFamilyCode.Focus();
                        txtBacteriumFamilyCode.SelectAll();
                        InitFillDataToCombo();
                    }
                }
                else
                {
                    if (CurrentDatafamil != null)
                    {
                        UpdateDTO.ID = CurrentDatafamil.ID;
                        var ResultData = new BackendAdapter(param).Post<LIS_BACTERIUM_FAMILY>(HisRequestUriStores.LisBacteriumFamily_Update, ApiConsumers.LisConsumer, UpdateDTO, param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<LIS_BACTERIUM_FAMILY>();
                            FillDataToGridControlFamil();
                            success = true;
                            txtBacteriumFamilyCode.Focus();
                            txtBacteriumFamilyCode.SelectAll();
                            InitFillDataToCombo();
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

        private void UpdateDataFromform(LIS_BACTERIUM data)
        {
            try
            {
                data.BACTERIUM_CODE = txtBacteriumCode.Text.Trim();
                data.BACTERIUM_NAME = txtBacteriumName.Text.Trim();
                if (cboBacteriumFamilyID.EditValue != null)
                    data.BACTERIUM_FAMILY_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBacteriumFamilyID.EditValue.ToString());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void UpdateDataFromformfamil(LIS_BACTERIUM_FAMILY data)
        {
            try
            {
                data.BACTERIUM_FAMILY_CODE = txtBacteriumFamilyCode.Text.Trim();
                data.BACTERIUM_FAMILY_NAME = txtBacteriumFamilyName.Text.Trim();

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnEditFamily_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessorfamil();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnAddFamily_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessorfamil();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void btnLockFa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                LIS_BACTERIUM_FAMILY data = new LIS_BACTERIUM_FAMILY();
                bool notHandler = false;
                try
                {
                    LIS_BACTERIUM_FAMILY datarow = (LIS_BACTERIUM_FAMILY)grdViewBacteriumFamily.GetFocusedRow();
                    if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        WaitingManager.Show();
                        data = new BackendAdapter(param).Post<LIS_BACTERIUM_FAMILY>(HisRequestUriStores.LisBacteriumFamily_ChangeLock, ApiConsumers.LisConsumer, datarow, param);
                        WaitingManager.Hide();
                        if (data != null)
                        {
                            FillDataToGridControlFamil();
                            InitFillDataToCombo();
                            notHandler = true;
                        }
                        BackendDataWorker.Reset<LIS_BACTERIUM_FAMILY>();
                        MessageManager.Show(this.ParentForm, param, notHandler);
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnUnLockFa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                LIS_BACTERIUM_FAMILY data = new LIS_BACTERIUM_FAMILY();
                bool notHandler = false;
                try
                {
                    LIS_BACTERIUM_FAMILY datarow = (LIS_BACTERIUM_FAMILY)grdViewBacteriumFamily.GetFocusedRow();
                    if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        WaitingManager.Show();
                        data = new BackendAdapter(param).Post<LIS_BACTERIUM_FAMILY>(HisRequestUriStores.LisBacteriumFamily_ChangeLock, ApiConsumers.LisConsumer, datarow, param);
                        WaitingManager.Hide();
                        if (data != null)
                        {
                            FillDataToGridControlFamil();
                            InitFillDataToCombo();
                            notHandler = true;
                        }
                        BackendDataWorker.Reset<LIS_BACTERIUM_FAMILY>();
                        MessageManager.Show(this.ParentForm, param, notHandler);
                    }
                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnDeleteFa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (LIS_BACTERIUM_FAMILY)grdViewBacteriumFamily.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStores.LisBacteriumFamily_Delete, ApiConsumers.LisConsumer, datarow.ID, null);
                        if (success)
                        {
                            BackendDataWorker.Reset<LIS_BACTERIUM_FAMILY>();
                            FillDataToGridControlFamil();
                            btnRefreshFamily_Click(null, null);
                            InitFillDataToCombo();
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (LIS_BACTERIUM)grdViewBacterium.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStores.LisBacterium_Delete, ApiConsumers.LisConsumer, datarow.ID, null);
                        if (success)
                        {
                            BackendDataWorker.Reset<LIS_BACTERIUM>();
                            FillDataToGridControl();
                            btnRefresh_Click(null, null);
                        }
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this.ParentForm, param, success);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            LIS_BACTERIUM data = new LIS_BACTERIUM();
            bool notHandler = false;
            try
            {
                LIS_BACTERIUM datarow = (LIS_BACTERIUM)grdViewBacterium.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    WaitingManager.Show();
                    data = new BackendAdapter(param).Post<LIS_BACTERIUM>(HisRequestUriStores.LisBacterium_ChangeLock, ApiConsumers.LisConsumer, datarow, param);
                    WaitingManager.Hide();
                    if (data != null)
                    {
                        FillDataToGridControl();
                        notHandler = true;
                    }
                    BackendDataWorker.Reset<LIS_BACTERIUM>();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }                            
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            LIS_BACTERIUM data = new LIS_BACTERIUM();
            bool notHandler = false;
            try
            {
                LIS_BACTERIUM datarow = (LIS_BACTERIUM)grdViewBacterium.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    WaitingManager.Show();
                    data = new BackendAdapter(param).Post<LIS_BACTERIUM>(HisRequestUriStores.LisBacterium_ChangeLock, ApiConsumers.LisConsumer, datarow, param);
                    WaitingManager.Hide();
                    if (data != null)
                    {
                        FillDataToGridControl();
                        notHandler = true;
                    }
                    BackendDataWorker.Reset<LIS_BACTERIUM>();
                    MessageManager.Show(this.ParentForm, param, notHandler);
                }                            
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBacteriumFamilyID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboBacteriumFamilyID.EditValue != null)
                    {
                        if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                            btnAdd.Focus();
                        else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                            btnEdit.Focus();
                        else
                            btnRefresh.Focus();
                    }
                    else
                    {
                        cboBacteriumFamilyID.Focus();
                        cboBacteriumFamilyID.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBacteriumFamilyID_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(cboBacteriumFamilyID.Text))
                    {
                        string key = cboBacteriumFamilyID.Text.ToLower();
                        var listData = BackendDataWorker.Get<LIS_BACTERIUM_FAMILY>().Where(o => o.BACTERIUM_FAMILY_CODE.ToLower().Contains(key) || o.BACTERIUM_FAMILY_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboBacteriumFamilyID.EditValue = listData.First().ID;
                            if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                                btnAdd.Focus();
                            else if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                                btnEdit.Focus();
                            else
                                btnRefresh.Focus();
                        }
                    }
                    if (!valid)
                    {
                        cboBacteriumFamilyID.Focus();
                        cboBacteriumFamilyID.ShowPopup();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitFillDataToCombo()
        {
            try
            {
                try
                {
                    List<Inventec.Common.Controls.EditorLoader.ColumnInfo> columnInfos = new List<Inventec.Common.Controls.EditorLoader.ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("BACTERIUM_FAMILY_CODE", "", 50, 1));
                    columnInfos.Add(new ColumnInfo("BACTERIUM_FAMILY_NAME", "", 100, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("BACTERIUM_FAMILY_NAME", "ID", columnInfos, false, 150);
                    ControlEditorLoader.Load(cboBacteriumFamilyID, BackendDataWorker.Get<LIS_BACTERIUM_FAMILY>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void txtBacteriumFamilyCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBacteriumFamilyName.Focus();
                    txtBacteriumFamilyName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtBacteriumFamilyName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtBacteriumCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBacteriumName.Focus();
                    txtBacteriumName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void txtBacteriumName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboBacteriumFamilyID.Focus();
                    cboBacteriumFamilyID.SelectAll();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActiveTypeFamily == GlobalVariables.ActionEdit && btnEditFamily.Enabled)
                {
                    btnEditFamily_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActiveTypeFamily == GlobalVariables.ActionAdd && btnAddFamily.Enabled)
                {
                    btnAddFamily_Click(null, null);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefreshFamily_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem6_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void barButtonItem7_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem4_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

                LogSystem.Warn(ex);
            }
        }
      
        private void txtBacteriumFamilyName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActiveTypeFamily == GlobalVariables.ActionEdit && btnEditFamily.Enabled)
                    {
                        btnEditFamily.Focus();
                    }
                    else if (this.ActiveTypeFamily == GlobalVariables.ActionAdd && btnAddFamily.Enabled)
                    {
                        btnAddFamily.Focus();
                    }
                    else
                        btnRefreshFamily.Focus();
                    
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                if (!btnImport.Enabled)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImportLisBacterium").FirstOrDefault();

                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisImportLisBacterium'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {

                    moduleData.RoomId = this.moduleData.RoomId;
                    moduleData.RoomTypeId = this.moduleData.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                //WaitingManager.Hide();
                //MessageBox.Show(MessageUtil.GetMessage(L.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBacteriumFamilyID_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBacteriumFamilyID.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

}

