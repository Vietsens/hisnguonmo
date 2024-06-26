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
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using System.IO;
using Inventec.Common.RichEditor.Base;
using System.Threading;
using FlexCel.Report;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Plugins.AntibioticRequestList.Base;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LocalStorage.HisConfig;
using DevExpress.XtraBars;
using HIS.Desktop.ADO;
using AutoMapper;
using Inventec.Common.Logging;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.AntibioticRequestList
{
    public partial class UCAntibioticRequestList : UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;
        private string LoggingName = "";
        bool IsType = true;
        ToolTip toolTip = new ToolTip();
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.AntibioticRequestList";
        string TreatmentCode;
        List<MOS.EFMODEL.DataModels.V_HIS_ANTIBIOTIC_REQUEST> listRequest;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        int indexStt = 0;
        int indexStt_ = 0;
        #endregion

        #region Construct
        public UCAntibioticRequestList()
        {
            InitializeComponent();
            try
            {
                gridControl.ToolTipController = this.toolTipController;
                //FillDataToNavBarStatus();
                //FillDataToNavBarTypes();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCAntibioticRequestList(Inventec.Desktop.Common.Modules.Module _module)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public UCAntibioticRequestList(Inventec.Desktop.Common.Modules.Module _module, string TreatmentCode)
            : base(_module)
        {
            InitializeComponent();
            try
            {
                this.TreatmentCode = TreatmentCode;
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UCAntibioticRequestList_Load(object sender, EventArgs e)
        {
            try
            {
                LoadcboDepartment();
                gridControl.ToolTipController = this.toolTipController;
                var now = DateTime.Now;
                dtCreateTimeFrom.EditValue = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                dtCreateTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                InitControlState();
                RefreshData();
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        public void RefreshData()
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == radioGroupStatus.Name)
                        {
                            if (item.VALUE == "0")
                                indexStt = 0;
                            else if (item.VALUE == "1")
                                indexStt = 1;
                            else if (item.VALUE == "2")
                                indexStt = 2;
                            else if (item.VALUE == "3")
                                indexStt = 3;

                        }
                        if (item.KEY == radioGroupExportBill.Name)
                        {
                            if (item.VALUE == "0")
                                indexStt = 0;
                            else if (item.VALUE == "1")
                                indexStt_ = 1;
                            else if (item.VALUE == "2")
                                indexStt_ = 2;
                        }
                    }
                    radioGroupStatus.SelectedIndex = indexStt;
                    radioGroupExportBill.SelectedIndex = indexStt_;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
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
                cbo.Properties.View.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DefaultBoolean.True;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
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

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
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
                radioGroupStatus.SelectedIndex = 0;

                radioGroupExportBill.SelectedIndex = 0;
                txtExpMestCode.Text = null;
                txtKeyWord.Text = null;
                txtSearchPatientCode.Text = null;
                txtSearchTreatmentCode.Text = null;
                cboDepartment.EditValue = null;
                var now = DateTime.Now;
                dtCreateTimeFrom.EditValue = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
                dtCreateTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
                dtExpTimeFrom.EditValue = null;
                dtExpTimeTo.EditValue = null;
                txtExpMestCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    pagingSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                gridControl.RefreshDataSource();

                //RefreshDisplaySummaryLabel();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        //private void RefreshDisplaySummaryLabel()
        //{
        //    try
        //    {
        //        Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(FillDataToSummaryLabel));
        //        thread.Priority = ThreadPriority.Normal;
        //        thread.IsBackground = true;
        //        thread.SetApartmentState(System.Threading.ApartmentState.STA);
        //        try
        //        {
        //            thread.Start();
        //        }
        //        catch (Exception ex)
        //        {
        //            Inventec.Common.Logging.LogSystem.Error(ex);
        //            thread.Abort();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void FillDataToSummaryLabel()
        //{
        //    try
        //    {
        //        CommonParam param = new CommonParam();
        //        MOS.Filter.HisAntibioticRequestViewFilter filter = new MOS.Filter.HisAntibioticRequestViewFilter();

        //        SetFilter(ref filter);


        //        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("summaryFilter:", filter));
        //        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).GetRO<HisExpMestSummarySDO>("api/HisExpMest/GetSummary", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
        //        if (apiResult != null)
        //        {
        //            var data = apiResult.Data;
        //            if (data != null)
        //            {
        //                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("summaryData:", data));
        //                Invoke(new Action(() =>
        //                {

        //                }));
        //            }
        //            else
        //            {
        //                Invoke(new Action(() =>
        //                {

        //                }));
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_ANTIBIOTIC_REQUEST>> apiResult = null;
                MOS.Filter.HisAntibioticRequestViewFilter filter = new HisAntibioticRequestViewFilter();
                SetFilter(ref filter);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                gridView.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_ANTIBIOTIC_REQUEST>>
                    ("api/HisAntibioticRequest/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listRequest = apiResult.Data;
                    if (listRequest != null && listRequest.Count > 0)
                    {

                        gridControl.DataSource = listRequest;
                        rowCount = (listRequest == null ? 0 : listRequest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);

                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listRequest == null ? 0 : listRequest.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisAntibioticRequestViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (!string.IsNullOrEmpty(txtExpMestCode.Text))
                {
                    string code = txtExpMestCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtExpMestCode.Text = code;
                    }
                    filter.ANTIBIOTIC_REQUEST_CODE__EXACT = txtExpMestCode.Text;
                }
                else
                {
                    if (!string.IsNullOrEmpty(txtSearchPatientCode.Text))
                    {
                        string PatientCode = txtSearchPatientCode.Text.Trim();
                        if (PatientCode.Length < 10 && checkDigit(PatientCode))
                        {
                            PatientCode = string.Format("{0:0000000000}", Convert.ToInt64(PatientCode));
                            txtSearchPatientCode.Text = PatientCode;
                        }
                        filter.TDL_PATIENT_CODE__EXACT = PatientCode;
                    }
                    if (!string.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                    {
                        string TreatmentCode = txtSearchTreatmentCode.Text.Trim();
                        if (TreatmentCode.Length < 12 && checkDigit(TreatmentCode))
                        {
                            TreatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(TreatmentCode));
                            txtSearchTreatmentCode.Text = TreatmentCode;
                        }
                        filter.TREATMENT_CODE__EXACT = TreatmentCode;
                    }
                    if (!string.IsNullOrEmpty(txtKeyWord.Text))
                    {
                        filter.KEY_WORD = txtKeyWord.Text;
                    }

                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                        filter.REQUEST_TIME__FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                           Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                        filter.REQUEST_TIME__TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dtExpTimeFrom.EditValue != null && dtExpTimeFrom.DateTime != DateTime.MinValue)
                        filter.APPROVE_TIME__FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtExpTimeFrom.EditValue).ToString("yyyyMMddHHmmss"));

                    if (dtExpTimeTo.EditValue != null && dtExpTimeTo.DateTime != DateTime.MinValue)
                    {

                        filter.APPROVE_TIME__TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                                Convert.ToDateTime(dtExpTimeTo.EditValue).ToString("yyyyMMddHHmmss"));
                    }


                    if (radioGroupStatus.SelectedIndex == 0)
                    {
                        filter.ANTIBIOTIC_REQUEST_STT = null;
                    }

                    if (radioGroupStatus.SelectedIndex == 1)
                    {
                        filter.ANTIBIOTIC_REQUEST_STT = 1;
                    }
                    if (radioGroupStatus.SelectedIndex == 2)
                    {
                        filter.ANTIBIOTIC_REQUEST_STT = 2;
                    }
                    if (radioGroupStatus.SelectedIndex == 3)
                    {
                        filter.ANTIBIOTIC_REQUEST_STT = 3;
                    }


                    if (radioGroupExportBill.SelectedIndex == 0)
                    {
                        //filter.REQUEST_LOGINNAME__EXACT = null;
                    }

                    else if (radioGroupExportBill.SelectedIndex == 1)
                    {
                        filter.REQUEST_LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }

                    else if (radioGroupExportBill.SelectedIndex == 2)
                    {
                        filter.APPROVAL_LOGINNAME__NULL_OR_EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    }

                    if (cboDepartment.EditValue != null)
                    {
                        filter.REQUEST_DEPARTMENT_ID = (long)cboDepartment.EditValue;
                    }
                }


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

        private bool checkLetter(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsLetter(s[i]) == true)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshData();
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
                RefreshData();
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
                    if (!string.IsNullOrEmpty(txtKeyWord.Text))
                    {
                        btnSearch_Click(null, null);
                        dtCreateTimeFrom.Focus();
                    }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtExpMestCode.Text))
                    {
                        string code = txtExpMestCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtExpMestCode.Text = code;
                        }
                        RefreshData();
                        txtSearchPatientCode.Focus();
                        txtSearchPatientCode.SelectAll();
                    }
                    else
                    {
                        txtSearchPatientCode.Focus();
                        txtSearchPatientCode.SelectAll();
                    }
                }
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

                    MOS.EFMODEL.DataModels.V_HIS_ANTIBIOTIC_REQUEST data = (MOS.EFMODEL.DataModels.V_HIS_ANTIBIOTIC_REQUEST)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            //e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                        else if (e.Column.FieldName == "REQUEST_STR")
                        {
                            e.Value = data.REQUEST_LOGINNAME + "-" + data.REQUEST_USERNAME;
                        }
                        else if (e.Column.FieldName == "REQUEST_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.REQUEST_TIME);
                        }
                        else if (e.Column.FieldName == "APPROVE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.APPROVE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_STR")
                        {
                            e.Value = data.APPROVAL_LOGINNAME + "-" + data.APPROVAL_USERNAME;
                        }
                        else if (e.Column.FieldName == "Status")
                        {
                            if (data.ANTIBIOTIC_REQUEST_STT == 1) // Yêu câu
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            if (data.ANTIBIOTIC_REQUEST_STT == 2) // Đã duyệt đồng ý
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            if (data.ANTIBIOTIC_REQUEST_STT == 3) // Đã duyệt từ chối
                            {
                                e.Value = imageListStatus.Images[4];
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {

                    V_HIS_ANTIBIOTIC_REQUEST pData = (V_HIS_ANTIBIOTIC_REQUEST)gridView.GetRow(e.RowHandle);
                    long statusIdCheckForButtonDelete = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXP_MEST_STT_ID") ?? "").ToString());

                    string LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    if (e.Column.FieldName == "Edit") // sửa
                    {
                        if (LoggingName == pData.REQUEST_LOGINNAME && pData.ANTIBIOTIC_REQUEST_STT != 2)
                        {
                            e.RepositoryItem = ButtonEnableEdit;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableEdit;
                        }
                    }
                    if (e.Column.FieldName == "Approval")
                    {

                        if (CheckLoginAdmin.IsAdmin(LoggingName) ||
                            (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000038") != null))
                        {
                            e.RepositoryItem = ButtonEnableApproval;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableApproval;
                        }

                    }
                    if (e.Column.FieldName == "Delete")
                    {
                        if ((CheckLoginAdmin.IsAdmin(LoggingName) || pData.REQUEST_LOGINNAME == LoggingName) && pData.ANTIBIOTIC_REQUEST_STT != 2)
                        {
                            e.RepositoryItem = ButtonEnableDiscard;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonDisableDiscard;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                            if (info.Column.FieldName == "Status")
                            {
                                long antiSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "ANTIBIOTIC_REQUEST_STT") ?? "").ToString());
                                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => antiSTTId), antiSTTId));
                                if (antiSTTId == 1) // Yêu câu
                                {
                                    text = "Yêu cầu";
                                }
                                if (antiSTTId == 2) // Đã duyệt đồng ý
                                {
                                    text = "Đã duyệt đồng ý";
                                }
                                if (antiSTTId == 3) // Đã duyệt từ chối
                                {
                                    text = "Đã duyệt từ chối";
                                }
                            }
                            if (info.Column.FieldName == "Edit") // sửa
                            {
                                text = "Sửa thông tin yêu cầu sử dụng kháng sinh";
                            }
                            if (info.Column.FieldName == "Approval")
                            {

                                text = "Duyệt yêu cầu sử dụng kháng sinh";
                            }
                            if (info.Column.FieldName == "Delete")
                            {
                                text = "Xóa";
                            }
                            if (info.Column.FieldName == "View")
                            {
                                text = "Xem chi tiết";
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



        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                if (btnSearch.Enabled)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
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
                if (btnRefresh.Enabled)
                {
                    btnRefresh.Focus();
                    btnRefresh_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusExpCode()
        {
            try
            {
                txtExpMestCode.Focus();
                txtExpMestCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Export()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void gridView_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_HIS_EXP_MEST_2)gridView.GetFocusedRow();
                if (row != null)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtSearchPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtSearchPatientCode.Text))
                    {
                        string PatientCode = txtSearchPatientCode.Text.Trim();
                        if (PatientCode.Length < 10 && checkDigit(PatientCode))
                        {
                            PatientCode = string.Format("{0:0000000000}", Convert.ToInt64(PatientCode));
                            txtSearchPatientCode.Text = PatientCode;
                        }
                        RefreshData();
                        txtSearchTreatmentCode.SelectAll();
                    }
                    else
                    {
                        txtSearchTreatmentCode.Focus();
                        txtSearchTreatmentCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        class CustomerFuncMergeSameData : TFlexCelUserFunction
        {
            List<Base.ExportListCodeRDO> rdo;

            public CustomerFuncMergeSameData(List<Base.ExportListCodeRDO> rdo)
            {
                this.rdo = rdo;
            }

            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 1)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");
                bool result = false;
                try
                {
                    int row = Convert.ToInt32(parameters[0]);
                    long id = Convert.ToInt64(parameters[1]);
                    if (row > 0)
                    {
                        if (rdo[row - 1].ROOM_ID == id)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(ex);
                }

                return result;
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                var now = dtCreateTimeTo.DateTime;
                dtCreateTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dtExpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {

            try
            {
                var now = dtExpTimeTo.DateTime;
                dtExpTimeTo.EditValue = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadcboDepartment()
        {
            try
            {
                var department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "Mã khoa", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "Tên khoa", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 300);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDepartment, department, controlEditorADO);
                cboDepartment.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                WaitingManager.Show();
                V_HIS_ANTIBIOTIC_REQUEST row = (V_HIS_ANTIBIOTIC_REQUEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    AntibioticRequestADO ado = new AntibioticRequestADO();
                    ado.AntibioticRequest = row;

                    CommonParam paramCommon = new CommonParam();
                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> apiResult = new List<HIS_EXP_MEST>();
                    MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                    filter.ANTIBIOTIC_REQUEST_ID = row.ID;
                    Inventec.Common.Logging.LogSystem.Info("api/HisExpMest/Get____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));
                    apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                        ("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null && apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID) != null)
                    {

                        ado.ExpMestId = apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID).ID;
                    }
                    ado.processType = null;
                    listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshData);
                    //listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    listArgs.Add(ado);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));

                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                   
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    WaitingManager.Hide();
                  
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AntibioticRequest", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ButtonEnableEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                WaitingManager.Show();
                V_HIS_ANTIBIOTIC_REQUEST row = (V_HIS_ANTIBIOTIC_REQUEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    AntibioticRequestADO ado = new AntibioticRequestADO();

                    CommonParam paramCommon = new CommonParam();
                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> apiResult = new List<HIS_EXP_MEST>();
                    MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                    filter.ANTIBIOTIC_REQUEST_ID = row.ID;
                    Inventec.Common.Logging.LogSystem.Info("api/HisExpMest/Get____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));
                    apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                        ("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null && apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID) != null)
                    {

                        ado.ExpMestId = apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID).ID;
                    }

                    //  ado.TreatmentId = row.TREATMENT_ID;
                    ado.AntibioticRequest = row;
                    ado.processType = AntibioticRequestADO.ProcessType.Request;
                    listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshData);
                    //listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    listArgs.Add(ado);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                 
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                   
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listArgs), listArgs));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AntibioticRequest", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                WaitingManager.Show();
                V_HIS_ANTIBIOTIC_REQUEST row = (V_HIS_ANTIBIOTIC_REQUEST)gridView.GetFocusedRow();
                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    //Inventec.UC.EventLogControl.Data.DataInit dataInit = new Inventec.UC.EventLogControl.Data.DataInit(ConfigApplications.NumPageSize, "", "", "", "", row.EXP_MEST_CODE, "");
                    AntibioticRequestADO ado = new AntibioticRequestADO();
                    //ado.TreatmentId = row.TREATMENT_ID;
                    CommonParam paramCommon = new CommonParam();
                    List<MOS.EFMODEL.DataModels.HIS_EXP_MEST> apiResult = new List<HIS_EXP_MEST>();
                    MOS.Filter.HisExpMestFilter filter = new MOS.Filter.HisExpMestFilter();
                    filter.ANTIBIOTIC_REQUEST_ID = row.ID;
                    Inventec.Common.Logging.LogSystem.Info("api/HisExpMest/Get____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("filter:", filter));
                    apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_EXP_MEST>>
                        ("api/HisExpMest/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);

                    if (apiResult != null && apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID) != null)
                    {

                        ado.ExpMestId = apiResult.FirstOrDefault(o => o.ANTIBIOTIC_REQUEST_ID == row.ID).ID;
                    }
                    ado.AntibioticRequest = row;
                    ado.processType = AntibioticRequestADO.ProcessType.Approval;
                     listArgs.Add((HIS.Desktop.Common.RefeshReference)RefreshData);
                    //listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)RefreshData);
                    listArgs.Add(ado);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado));
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                    
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listArgs), listArgs));
                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.AntibioticRequest", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEnableDiscard_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có chắc muốn xóa dữ liệu không?", "Thông báo", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    var row = (V_HIS_ANTIBIOTIC_REQUEST)gridView.GetFocusedRow();
                    CommonParam param = new CommonParam();
                    if (row != null)
                    {
                        WaitingManager.Show();
                        var apiresul = new Inventec.Common.Adapter.BackendAdapter
                            (param).Post<bool>
                            ("api/HisAntibioticRequest/Delete", ApiConsumer.ApiConsumers.MosConsumer, row.ID, param);
                        if (apiresul)
                        {
                            success = true;
                            RefreshData();
                        }
                    }
                    WaitingManager.Hide();
                    #region Show message
                    Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void radioGroupStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == radioGroupStatus.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {

                    if (radioGroupStatus.SelectedIndex == 0)
                        csAddOrUpdate.VALUE = "0";
                    else if (radioGroupStatus.SelectedIndex == 1)
                        csAddOrUpdate.VALUE = "1";
                    else if (radioGroupStatus.SelectedIndex == 2)
                        csAddOrUpdate.VALUE = "2";
                    else if (radioGroupStatus.SelectedIndex == 3)
                        csAddOrUpdate.VALUE = "3";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = radioGroupStatus.Name;
                    if (radioGroupStatus.SelectedIndex == 0)
                        csAddOrUpdate.VALUE = "0";
                    else if (radioGroupStatus.SelectedIndex == 1)
                        csAddOrUpdate.VALUE = "1";
                    else if (radioGroupStatus.SelectedIndex == 2)
                        csAddOrUpdate.VALUE = "2";
                    else if (radioGroupStatus.SelectedIndex == 3)
                        csAddOrUpdate.VALUE = "3";
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();


                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                this.currentControlStateRDO.Add(csAddOrUpdate);
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        public void radioGroupExportBill_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == radioGroupExportBill.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {

                    if (radioGroupExportBill.SelectedIndex == 0)
                        csAddOrUpdate.VALUE = "0";
                    else if (radioGroupExportBill.SelectedIndex == 1)
                        csAddOrUpdate.VALUE = "1";
                    else if (radioGroupExportBill.SelectedIndex == 2)
                    {
                        csAddOrUpdate.VALUE = "2";
                    }
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = radioGroupExportBill.Name;
                    if (radioGroupExportBill.SelectedIndex == 0)
                        csAddOrUpdate.VALUE = "0";
                    else if (radioGroupExportBill.SelectedIndex == 1)
                        csAddOrUpdate.VALUE = "1";
                    else if (radioGroupExportBill.SelectedIndex == 2)
                    {
                        csAddOrUpdate.VALUE = "2";
                    }
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();

                }
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                this.currentControlStateRDO.Add(csAddOrUpdate);
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtSearchTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(txtSearchTreatmentCode.Text))
                {

                    string TreatmentCode = txtSearchTreatmentCode.Text.Trim();
                    if (TreatmentCode.Length < 12 && checkDigit(TreatmentCode))
                    {
                        TreatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(TreatmentCode));
                        txtSearchTreatmentCode.Text = TreatmentCode;
                    }
                    RefreshData();
                    txtKeyWord.SelectAll();
                }
                else
                {
                    txtKeyWord.Focus();
                    txtKeyWord.SelectAll();
                }
            }
        }

        private void dtCreateTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void dtExpTimeFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtExpTimeFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtExpTimeTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    radioGroupStatus.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroupStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    radioGroupExportBill.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroupExportBill_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboDepartment.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioGroupStatus_SelectedIndexChanged()
        {

        }

        private void radioGroupExportBill_SelectedIndexChanged()
        {

        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
          
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }              
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
             
        }


    }
}
