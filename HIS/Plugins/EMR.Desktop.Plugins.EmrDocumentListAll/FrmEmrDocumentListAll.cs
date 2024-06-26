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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.WebApiClient;
using EMR.Desktop.Plugins.EmrDocumentListAll.ADO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using System.IO;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.SignLibrary;
using HIS.Desktop.LocalStorage.ConfigSystem;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using Inventec.Common.Logging;
using EMR.Filter;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace EMR.Desktop.Plugins.EmrDocumentListAll
{
    public partial class FrmEmrDocumentListAll : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        long roomId = 0;
        long roomTypeId = 0;
        List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> documentTypeSelecteds;
        List<HIS_DEPARTMENT> departmentSelecteds;
        Inventec.Desktop.Common.Modules.Module moduleData;


        bool notAutoCompleteZero { get; set; }
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "EMR.Desktop.Plugins.EmrDocumentListAll";
        #endregion

        #region Construct
        public FrmEmrDocumentListAll()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FrmEmrDocumentListAll(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> GetDocumentType()
        {
            List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> result = new List<EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrDocumentTypeFilter filter = new Filter.EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_DEPARTMENT> GetDepartment()
        {
            List<HIS_DEPARTMENT> department = new List<HIS_DEPARTMENT>();
            try
            {
                CommonParam param = new CommonParam();
                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = 1;

                department = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return department;
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__TXT_TREARTMENTCODE",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                  "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__TXT_PATIENTCODE",
                  Resources.ResourceLanguageManager.LanguageUCHisBidList,
                  cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                //this.navBarGroupCreateDate.Caption = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__NAV_BAR_CREATE_TIME",
                //    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                //    cultureLang);
                this.navBarGroupCreateDate.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__NAV_BAR_CREATE_TIME",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                //gridView
                this.GcSTT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_DOCUMENT_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__DISPENSE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
               
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_CREATE_TIME_DISPLAY",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_CREATOR",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_MODIFY_TIME_DISPLAY",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_MODIFER",
                   Resources.ResourceLanguageManager.LanguageUCHisBidList,
                   cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboSTT(List<StatusADO> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("STT_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("STT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboStatus, data, controlEditorADO);

                //mặc định: chờ ký
                cboStatus.EditValue = 2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<StatusADO> LoadDataToComboStatus()
        {
            List<StatusADO> Result = new List<StatusADO>();
            try
            {
                StatusADO StatusADO_TatCa = new StatusADO();
                StatusADO_TatCa.ID = 0;
                StatusADO_TatCa.STT_NAME = "Tất cả";
                Result.Add(StatusADO_TatCa);

                StatusADO StatusADO_DaKy = new StatusADO();
                StatusADO_DaKy.ID = 1;
                StatusADO_DaKy.STT_NAME = "Đã ký xong";
                Result.Add(StatusADO_DaKy);

                StatusADO StatusADO_DangKy = new StatusADO();
                StatusADO_DangKy.ID = 2;
                StatusADO_DangKy.STT_NAME = "Đang ký";
                Result.Add(StatusADO_DangKy);

                StatusADO StatusADO_BiTuChoi = new StatusADO();
                StatusADO_BiTuChoi.ID = 3;
                StatusADO_BiTuChoi.STT_NAME = "Bị từ chối";
                Result.Add(StatusADO_BiTuChoi);

                StatusADO StatusADO_DaXoa = new StatusADO();
                StatusADO_DaXoa.ID = 4;
                StatusADO_DaXoa.STT_NAME = "Đã xóa";
                Result.Add(StatusADO_DaXoa);
            }
            catch (Exception ex)
            {
                Result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Result;
        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(GridPaging, param, pagingSize, this.gridControl1);
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
                ApiResultObject<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>> apiResult = null;
                EMR.Filter.EmrDocumentViewFilter filter = new EMR.Filter.EmrDocumentViewFilter();
                SetFilter(ref filter);
                gridView1.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>
                    ("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl1.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl1.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl1.DataSource = null;
                }
                gridView1.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView1.EndUpdate();
            }
        }

        private void SetFilter(ref EMR.Filter.EmrDocumentViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && !notAutoCompleteZero)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && !notAutoCompleteZero)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
               
                if (cboStatus.EditValue != null)
                {
                    long Stt = Int64.Parse(cboStatus.EditValue.ToString());
                    if (Stt == 1)// da ky xong
                    {
                        filter.IS_NEXT_SIGNER_NOT_NULL = false;
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 2) // dang ky
                    {
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_NEXT_SIGNER_NOT_NULL = true;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 3) // bi tu choi
                    {
                        filter.IS_REJECTER_NOT_NULL = true;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 4) // da xoa
                    {
                        filter.IS_DELETE = true;
                    }
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (this.documentTypeSelecteds != null && this.documentTypeSelecteds.Count() > 0)
                {
                    filter.DOCUMENT_TYPE_IDs = this.documentTypeSelecteds.Select(o => o.ID).Distinct().ToList();
                }

                if (this.departmentSelecteds != null && this.departmentSelecteds.Count() > 0)
                {
                    filter.CURRENT_DEPARTMENT_CODEs = this.departmentSelecteds.Select(o => o.DEPARTMENT_CODE).Distinct().ToList();
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
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void BtnRefreshs()
        {
            try
            {
                if (btnRefresh.Enabled)
                {
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        public void BtnSearch()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR.EFMODEL.DataModels.V_EMR_DOCUMENT data = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "STORE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.STORE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                        else if (e.Column.FieldName == "NEXT_SIGNER_STR")
                        {
                            e.Value = String.IsNullOrWhiteSpace(data.REJECTER) ? data.NEXT_SIGNER : "";
                        }
                        else if (e.Column.FieldName == "REJECT_REASON")
                        {
                            e.Value = data.REJECT_REASON;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public bool HasSpecialChars(string stString)
        {
            if (stString.Trim().StartsWith("#@!@#"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)gridView1.GetRow(e.RowHandle);
                    int? isConfirm = ToNullableInt((gridView1.GetRowCellValue(e.RowHandle, "IS_CONFIRM") ?? "").ToString());
                    int? isActive = ToNullableInt((gridView1.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    string nextSigner = (gridView1.GetRowCellValue(e.RowHandle, "NEXT_SIGNER") ?? "").ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Public method

        public void FocusDispenseCode()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void gridLookUpEdit1_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void FrmEmrDocumentListAll_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitComboSTT(LoadDataToComboStatus());

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                InitCheck(cboDocumentType, SelectionGrid__Status);

                InitCombo(cboDocumentType, GetDocumentType(), "DOCUMENT_TYPE_NAME", "ID");

                InitCheck(cboDepartment, SelectionGrid__Department);

                InitCombo(cboDepartment, GetDepartment(), "DEPARTMENT_NAME", "ID");

                InitControlState();

                //Load du lieu
                FillDataToGrid();

                txtKeyWord.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    //gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                documentTypeSelecteds = new List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
                foreach (EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        documentTypeSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                departmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        departmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.documentTypeSelecteds != null && this.documentTypeSelecteds.Count > 0)
                {
                    foreach (var item in this.documentTypeSelecteds)
                    {
                        dayName += item.DOCUMENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IS_DELETE == 1)// đã xóa
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        else if (data.COUNT_RESIGN_FAILED != null && data.COUNT_RESIGN_FAILED > 0)
                        {
                            e.Appearance.ForeColor = Color.Maroon;
                        }
                        else if (!String.IsNullOrEmpty(data.REJECTER))// bị từ chối
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }

                        else if (data.COUNT_RESIGN_WAIT != null && data.COUNT_RESIGN_WAIT > 0)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (!String.IsNullOrWhiteSpace(data.NEXT_SIGNER))
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        this.FillDataToGrid();
                    } 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
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
                    if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                    {
                        this.FillDataToGrid();
                    } 
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";
                txtTreatmentCode.Text = "";
                txtPatientCode.Text = "";
                cboStatus.EditValue = 0;
                cboDocumentType.EditValue = 0;
                cboDepartment.EditValue = 0;
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                dtCreateTimeFrom.DateTime = startDate;
                dtCreateTimeTo.DateTime = endDate;

                txtKeyWord.Focus();
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

        private void InitControlState()
        {

            try
            {
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkNotAutoCompleteZero.Name)
                        {
                            chkNotAutoCompleteZero.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
        private void chkNotAutoCompleteZero_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.notAutoCompleteZero = chkNotAutoCompleteZero.Checked;
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == chkNotAutoCompleteZero.Name && o.MODULE_LINK == moduleLink) : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkNotAutoCompleteZero.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkNotAutoCompleteZero.Name;
                    csAddOrUpdate.VALUE = (chkNotAutoCompleteZero.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {

        }

        private void cboDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count > 0)
                {
                    foreach (var item in this.departmentSelecteds)
                    {
                        dayName += item.DEPARTMENT_NAME + ", ";
                    }
                    e.DisplayText = dayName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        //private void InitCboDepartment()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
        //        filter.IS_ACTIVE = 1;

        //        DepartmentSelecteds = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param).ToList();
        //        List<ColumnInfo> columnInfos = new List<ColumnInfo>();
        //        columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 100, 1));
        //        columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 250, 2));
        //        ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, true, 350);
        //        ControlEditorLoader.Load(cboDepartment, DepartmentSelecteds, controlEditorADO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}
    }
}
