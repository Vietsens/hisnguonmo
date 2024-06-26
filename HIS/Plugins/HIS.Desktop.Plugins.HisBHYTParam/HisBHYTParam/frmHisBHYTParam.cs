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
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.UC.Paging;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.HisBHYTParam.Resources;
using System.Resources;
using Inventec.Common.Resource;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisBHYTParam.HisBHYTParam
{
    public partial class frmHisBHYTParam : FormBase
    {
        #region Declare Variable
        Module moduleData;
        PagingGrid pagingGrid;
        int ActionType = -1;
        int startPage = 0;
        int dataTotal = 0;
        int rowCount = 0;
        HIS_BHYT_PARAM CurrentData;
        #endregion
        public frmHisBHYTParam(Module module)
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

        private void frmHisBHYTParam_Load(object sender, EventArgs e)
        {
            try
            {
                SetDefautData();
                EnableControlChanged(this.ActionType);
                SetCaptionByLanguageKey();
                FillDataToGridControl();
                ValidateForm();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }
        #region ---Set Data

        private void ValidateForm()
        {
            try
            {
                validateEditorControl(txtBaseSalary);
                validateEditorControl(txtMinTotalBySalary);
                validateEditorControl(txtMaxTotalPackageBySalary);
                validateEditorControl(txtSecondStentPaidRatio);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        void validateEditorControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validate.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
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
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBHYTParam.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBHYTParam.HisBHYTParam.frmHisBHYTParam).Assembly);
                this.btnAdd.Text = Get.Value("frmHisBHYTParam.btnAdd.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Get.Value("frmHisBHYTParam.btnEdit.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Get.Value("frmHisBHYTParam.btnCancel.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcBaseSalary.Text = Get.Value("frmHisBHYTParam.lcBaseSalary.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMinTotalBySalary.Text = Get.Value("frmHisBHYTParam.lcMinTotalBySalary.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcMaxTotalPackageBySalary.Text = Get.Value("frmHisBHYTParam.lcMaxTotalPackageBySalary.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcSecondStentPaidRatio.Text = Get.Value("frmHisBHYTParam.lcSecondStentPaidRatio.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcPriorty.Text = Get.Value("frmHisBHYTParam.lcPriorty.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcFromTime.Text = Get.Value("frmHisBHYTParam.lcFromTime.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcToTime.Text = Get.Value("frmHisBHYTParam.lcToTime.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Get.Value("frmHisBHYTParam.STT.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.ToolTip = Get.Value("frmHisBHYTParam.STT.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBaseSalary.Caption = Get.Value("frmHisBHYTParam.grdColBaseSalary.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColBaseSalary.ToolTip = Get.Value("frmHisBHYTParam.grdColBaseSalary.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMinTotalBySalary.Caption = Get.Value("frmHisBHYTParam.grdColMinTotalBySalary.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMinTotalBySalary.ToolTip = Get.Value("frmHisBHYTParam.grdColMinTotalBySalary.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxTotalPackage.Caption = Get.Value("frmHisBHYTParam.grdColMaxTotalPackage.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxTotalPackage.ToolTip = Get.Value("frmHisBHYTParam.grdColMaxTotalPackage.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSecondStentPaid.Caption = Get.Value("frmHisBHYTParam.grdColSecondStentPaid.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColSecondStentPaid.ToolTip = Get.Value("frmHisBHYTParam.grdColSecondStentPaid.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPriority.Caption = Get.Value("frmHisBHYTParam.grdColPriority.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPriority.ToolTip = Get.Value("frmHisBHYTParam.grdColPriority.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTime.Caption = Get.Value("frmHisBHYTParam.grdColFromTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFromTime.ToolTip = Get.Value("frmHisBHYTParam.grdColFromTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.Caption = Get.Value("frmHisBHYTParam.grdColToTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColToTime.ToolTip = Get.Value("frmHisBHYTParam.grdColToTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Get.Value("frmHisBHYTParam.grdColCreateTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Get.Value("frmHisBHYTParam.grdColCreateTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Get.Value("frmHisBHYTParam.grdColCreator.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Get.Value("frmHisBHYTParam.grdColCreator.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Get.Value("frmHisBHYTParam.grdColModifyTime.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Get.Value("frmHisBHYTParam.grdColModifyTime.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Get.Value("frmHisBHYTParam.grdColModifier.Caption", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Get.Value("frmHisBHYTParam.grdColModifier.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetDefautData()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtBaseSalary.EditValue = null;
                txtMinTotalBySalary.EditValue = null;
                txtMaxTotalPackageBySalary.EditValue = null;
                txtSecondStentPaidRatio.EditValue = null;
                txtPriorty.EditValue = null;
                txtFromTime.EditValue = null;
                txtToTime.EditValue = null;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<HIS_BHYT_PARAM>> apiResult = null;
                HisBhytParamFilter filter = new HisBhytParamFilter();
                SetFilter(ref filter);
                gridControlHisBHYTParam.DataSource = null;
                gridViewHisBHYTParam.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<HIS_BHYT_PARAM>>(HisRequestUriStores.HisBHYTParam_Get, ApiConsumers.MosConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<HIS_BHYT_PARAM>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlHisBHYTParam.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }

                }

                gridViewHisBHYTParam.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref HisBhytParamFilter filter)
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

        private void UpdateDataFromform(HIS_BHYT_PARAM data)
        {
            try
            {
                if (txtBaseSalary.EditValue != null)
                {
                    data.BASE_SALARY = (decimal)txtBaseSalary.EditValue;
                }
                if (txtMinTotalBySalary.EditValue != null)
                {
                    data.MIN_TOTAL_BY_SALARY = (decimal)txtMinTotalBySalary.EditValue;
                }
                if (txtMaxTotalPackageBySalary.EditValue != null)
                {
                    data.MAX_TOTAL_PACKAGE_BY_SALARY = (decimal)txtMaxTotalPackageBySalary.EditValue;
                }
                if (txtSecondStentPaidRatio.EditValue != null)
                {
                    data.SECOND_STENT_PAID_RATIO = (decimal)txtSecondStentPaidRatio.EditValue;
                }
                if (txtPriorty.EditValue != null)
                {
                    data.PRIORITY = Inventec.Common.TypeConvert.Parse.ToInt64((txtPriorty.EditValue.ToString() ?? "").ToString());
                }
                if (txtFromTime.EditValue != null)
                {
                    data.FROM_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(txtFromTime.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else
                {
                    data.FROM_TIME = null;
                }

                if (txtToTime.EditValue != null)
                {
                    data.TO_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(txtToTime.EditValue).ToString("yyyyMMdd") + "235959");
                }
                else
                {
                    data.TO_TIME = null;
                }
                    
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
                if (ucPaging2.pagingGrid != null)
                {
                    pagingSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pagingSize, this.gridControlHisBHYTParam);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SaveProcessor()
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
                HIS_BHYT_PARAM UpdateDTO = new HIS_BHYT_PARAM();
                UpdateDataFromform(UpdateDTO);
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    var ResultData = new BackendAdapter(param).Post<HIS_BHYT_PARAM>(HisRequestUriStores.HisBHYTParam_Create, ApiConsumers.MosConsumer, UpdateDTO, param);
                    if (ResultData != null)
                    {
                        BackendDataWorker.Reset<HIS_BHYT_PARAM>();
                        FillDataToGridControl();
                        success = true;
                        RestFromData();
                    }
                }
                else
                {
                    if (CurrentData != null)
                    {
                        UpdateDTO.ID = CurrentData.ID;
                        var ResultData = new BackendAdapter(param).Post<HIS_BHYT_PARAM>(HisRequestUriStores.HisBHYTParam_Update, ApiConsumers.MosConsumer, UpdateDTO, param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<HIS_BHYT_PARAM>();
                            FillDataToGridControl();
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
                            txtBaseSalary.Focus();
                            txtBaseSalary.SelectAll();
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

        #region ---Button Click
        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcessor();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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
                LogSystem.Warn(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                EnableControlChanged(this.ActionType);
                RestFromData();
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
                txtBaseSalary.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRestFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtBaseSalary.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
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

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region even GridView
        private void gridViewHisBHYTParam_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HIS_BHYT_PARAM DataRow = (HIS_BHYT_PARAM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (DataRow != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "FROM_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(DataRow.FROM_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TO_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(DataRow.TO_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(DataRow.MODIFY_TIME ?? 0);
                        }
                        //else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        //{
                        //    e.Value = (DataRow.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa");
                        //}
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewHisBHYTParam_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    var dataRow = (HIS_BHYT_PARAM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (dataRow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            e.RepositoryItem = (dataRow.IS_ACTIVE == 0 ? btnEnableDelete : btnDelete);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewHisBHYTParam_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            //try
            //{
            //    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            //    if (e.RowHandle >= 0)
            //    {
            //        var data = (HIS_BHYT_PARAM)gridViewHisBHYTParam.GetRow(e.RowHandle);
            //        if (e.Column.FieldName == "IS_ACTIVE_STR")
            //        {
            //            if (data.IS_ACTIVE == 0)
            //                e.Appearance.ForeColor = Color.Red;
            //            else
            //                e.Appearance.ForeColor = Color.Green;
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{

            //    LogSystem.Error(ex);
            //}
        }

        private void gridViewHisBHYTParam_Click(object sender, EventArgs e)
        {
            try
            {
                var datarow = (HIS_BHYT_PARAM)gridViewHisBHYTParam.GetFocusedRow();
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

        private void ChangeDataRow(HIS_BHYT_PARAM datarow)
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

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                }
                
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void FillDataEditorControl(HIS_BHYT_PARAM datarow)
        {
            try
            {
                if (datarow != null)
                {
                    txtBaseSalary.EditValue = datarow.BASE_SALARY;
                    txtMinTotalBySalary.EditValue = datarow.MIN_TOTAL_BY_SALARY;
                    txtMaxTotalPackageBySalary.EditValue = datarow.MAX_TOTAL_PACKAGE_BY_SALARY;
                    txtSecondStentPaidRatio.EditValue = datarow.SECOND_STENT_PAID_RATIO;
                    txtPriorty.EditValue = datarow.PRIORITY;
                    txtFromTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(datarow.FROM_TIME ?? 0);
                    txtToTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(datarow.TO_TIME ?? 0);
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                var datarow = (HIS_BHYT_PARAM)gridViewHisBHYTParam.GetFocusedRow();
                if (MessageBox.Show(Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    WaitingManager.Show();
                    if (datarow != null)
                    {
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStores.HisBHYTParam_Delete, ApiConsumers.MosConsumer, datarow.ID, null);
                        if (success)
                        {
                            BackendDataWorker.Reset<HIS_BHYT_PARAM>();
                            FillDataToGridControl();
                            btnCancel_Click(null, null);
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

        #endregion

        #region ---Key down
        private void txtBaseSalary_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMinTotalBySalary.Focus();
                    txtMinTotalBySalary.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtMaxTotalPackageBySalary_KeyDown(object sender, KeyEventArgs e)
        {
            
        }

        private void txtSecondStentPaidRatio_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPriorty.Focus();
                    txtPriorty.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtPriorty_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFromTime.Focus();
                    txtFromTime.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtFromTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtToTime.Focus();
                    txtToTime.SelectAll();
                }
                e.Handled = true;
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
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit.Focus();
                }
                else if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd.Focus();
                }
                else
                    btnCancel.Focus();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
      

        private void txtMinTotalBySalary_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMaxTotalPackageBySalary.Focus();
                    txtMaxTotalPackageBySalary.SelectAll();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void textEdit1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSecondStentPaidRatio.Focus();
                    txtSecondStentPaidRatio.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion
    }
}
