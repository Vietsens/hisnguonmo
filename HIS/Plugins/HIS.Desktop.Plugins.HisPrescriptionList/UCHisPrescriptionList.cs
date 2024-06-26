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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.HisPrescriptionList
{
    public partial class UCHisPrescriptionList : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        int lastRowHandle = -1;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        private string LoggingName = "";
        #endregion

        #region Construct
        public UCHisPrescriptionList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                gridControl.ToolTipController = this.toolTipController;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisPrescriptionList(long roomId, long roomTypeId)
            : this()
        {
            try
            {
                medistock = Base.GlobalStore.ListMediStock.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisPrescriptionList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                FillDataNavStatus();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

                txtExpCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtExpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_EXP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupHasAggr.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_HAS_AGGR",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.chkHasAggr.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__CHK_HAS_AGGR",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.txtPatientCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__TXT_PATIENT_CODE_NUll_VLAUE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__NAV_BAR_EXP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__LCI_EXP_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__LCI_EXP_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);

                //gridView
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__APPROVAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcDob.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__DOB",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_MEST_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcExpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__EXP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcGender.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__GENDER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcIntructionTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__INTRUCTION_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MIDIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcPatientCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__PATIENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcTreatmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__TREATMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcReqDepartmentCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_DEPARTMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_DEPARTMENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__REQ_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.GcVirPatientName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GRID_COLUMN__VIR_PATIENT_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);

                //grid button
                this.ButtonApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonCancelExport.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_CANCEL_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonCancelExportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_CANCEL_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonDisApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonDiscardEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonEditDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonEditEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonExportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonExportEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_EXPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonReApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonReApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
                this.ButtonViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_PRESCRIPTION_LIST__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisPrescriptionList,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataNavStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisExpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.EXP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(150, 19);
                    checkEdit.StyleController = this.layoutControlStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                txtExpCode.Text = "";
                txtPatientCode.Text = "";
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                txtExpCode.Focus();
                SetDefaultStatus();
                chkHasAggr.Checked = false;
                navBarGroupHasAggr.Expanded = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultStatus()
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            checkEdit.Checked = false;
                        }
                    }
                }
                navBarGroupStatus.Expanded = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_1>> apiResult = null;
                MOS.Filter.HisPrescriptionView1Filter filter = new MOS.Filter.HisPrescriptionView1Filter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_1>>
                    (ApiConsumer.HisRequestUriStore.HIS_PRESCRIPTION_1_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null && apiResult.Data.Count > 0)
                {
                    var data = apiResult.Data;
                    //if (data != null)
                    //{
                    //List<Base.PrescriptionADO> ado = new List<Base.PrescriptionADO>();
                    //MOS.Filter.HisTreatmentFilter filtertreatment = new HisTreatmentFilter();
                    //filtertreatment.IDs = apiResult.Data.Select(s => s.TREATMENT_ID).ToList();
                    //var treatment = new BackendAdapter(new CommonParam())
                    //    .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filtertreatment, null).ToList();
                    //if (treatment != null && treatment.Count > 0)
                    //{
                    //    foreach (var item in data)
                    //    {
                    //        Base.PrescriptionADO a = new Base.PrescriptionADO(item);
                    //        a.TREATMENT_CODE = treatment.FirstOrDefault(o => o.ID == item.TREATMENT_ID).TREATMENT_CODE;
                    //        ado.Add(a);
                    //    }

                    if (data != null && data.Count > 0)
                    {
                        gridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    //    }
                    //}
                }
                else
                {
                    gridControl.DataSource = null;
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisPrescriptionView1Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "EXP_MEST_MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();

                if (!chkHasAggr.Checked) filter.HAS_AGGR = false;

                if (roomId == 0)
                {
                    filter.CREATOR = LoggingName;
                }
                else
                {
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }

                if (!String.IsNullOrEmpty(txtExpCode.Text))
                {
                    string code = txtExpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpCode.Text = code;
                    }
                    filter.EXP_MEST_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }

                if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                    filter.EXP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    filter.EXP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                SetFilterStatus(ref filter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void SetFilterStatus(ref MOS.Filter.HisPrescriptionView1Filter filter)
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.EXP_MEST_STT_IDs == null)
                                    filter.EXP_MEST_STT_IDs = new List<long>();
                                filter.EXP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }

        private void txtExpCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtPatientCode.Focus();
                        txtPatientCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "EXP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "EXP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "EXP_MEST_STT_NAME") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long statusIdCheckForButtonEdit = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    long expMestTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_TYPE_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "EDIT_DISPLAY") // sửa
                    {
                        if ((creator == LoggingName) &&
                            (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            e.RepositoryItem = ButtonEditEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditDisable;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY") //hủy
                    {
                        if ((creator == LoggingName) &&
                            (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            e.RepositoryItem = ButtonDiscardEnable;
                        }
                        else
                            e.RepositoryItem = ButtonDiscardDisable;
                    }
                    else if (e.Column.FieldName == "APPROVAL_DISPLAY") //duyet
                    {
                        if (medistock != null &&
                            (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonApprovalEnable;
                            else
                                e.RepositoryItem = ButtonApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonApprovalDisable;
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt
                    {
                        if (medistock != null &&
                            (statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT &&
                            statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED &&
                            statusIdCheckForButtonEdit != Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonDisApprovalEnable;
                            else
                                e.RepositoryItem = ButtonDisApprovalDisable;
                        }
                        else
                            e.RepositoryItem = ButtonDisApprovalDisable;
                    }
                    else if (e.Column.FieldName == "EXPORT_DISPLAY")// thực xuất
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonExportEnable;
                            else
                                e.RepositoryItem = ButtonExportDisable;
                        }
                        else
                            e.RepositoryItem = ButtonExportDisable;
                    }
                    else if (e.Column.FieldName == "CANCEL_EXPORT_DISPLAY")//hủy thực xuất
                    {
                        if (medistock != null && statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED)
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonCancelExport;
                            else
                                e.RepositoryItem = ButtonCancelExportDisable;
                        }
                        else
                            e.RepositoryItem = ButtonCancelExportDisable;
                    }
                    else if (e.Column.FieldName == "RE_APPROVAL")// Hủy duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED ||
                            statusIdCheckForButtonEdit == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED))
                        {
                            e.RepositoryItem = ButtonReApproval;
                        }
                        else
                            e.RepositoryItem = ButtonReApprovalDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_1 data = (MOS.EFMODEL.DataModels.V_HIS_PRESCRIPTION_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "EXP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__DRAFT) //tam thoi
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REQUEST) //yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__REJECTED) // tu choi duyet
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__APPROVED) // duyet
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__EXPORTED) // da xuat
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                            else if (data.EXP_MEST_STT_ID == Base.HisExpMestSttCFG.EXP_MEST_STT_ID__AGGREGATE) //Tong hop
                            {
                                e.Value = imageListStatus.Images[5];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "EXP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "INTRUCTION_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "EXP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.EXP_LOGINNAME;
                            string IMP_USERNAME = data.EXP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtExpCode.Focus();
                txtExpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
