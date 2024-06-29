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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using MOS.EFMODEL.DataModels;
using EMR.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using MOS.Filter;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;

namespace EMR.Desktop.Plugins.EmrTreatmentList
{
    public partial class UcEmrTreatmentList : HIS.Desktop.Utility.UserControlBase
    {
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private string currentDepartmentCode;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "EMR.Desktop.Plugins.EmrTreatmentList";
		private GridColumn lastColumn;
		private int lastRowHandle;
		private ToolTipControlInfo lastInfo;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        private EmrTreatmentViewFilter emrTreatmentView;
        public UcEmrTreatmentList()
        {
            InitializeComponent();
        }

        public UcEmrTreatmentList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = moduleData;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UcEmrTreatmentList_Load(object sender, EventArgs e)
        {
            try
            {
                //Get ACS
                GetControlAcs();
                VisbleColumnGrid();
                //Gan ngon ngu
                LoadKeysFromlanguage();
                //Load combobox khoa
                LoadCurrentDepartmentCode();
                //Gan gia tri mac dinh
                SetDefaultValueControl();
                //Load du lieu
                FillDataToGrid();
                TxtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCurrentDepartmentCode()
        {
            try
            {
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                if (room != null)
                {
                    this.currentDepartmentCode = room.DEPARTMENT_CODE;
                }

                CommonParam param = new CommonParam();

                HisDepartmentFilter filter = new HisDepartmentFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var datas = new BackendAdapter(param).Get<List<HIS_DEPARTMENT>>("api/HisDepartment/Get", ApiConsumers.MosConsumer, filter, param); 
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "DEPARTMENT_CODE", columnInfos, false, 350);
                ControlEditorLoader.Load(cboCurrentDepartmentCodeNew, datas, controlEditorADO);
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
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlTreatment);
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
                ApiResultObject<List<EMR.EFMODEL.DataModels.V_EMR_TREATMENT>> apiResult = null;
                EMR.Filter.EmrTreatmentViewFilter filter = new EMR.Filter.EmrTreatmentViewFilter();
                SetFilter(ref filter);
                gridViewTreatment.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<EMR.EFMODEL.DataModels.V_EMR_TREATMENT>>
                    (EmrTreatment.GET, ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlTreatment.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlTreatment.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControlTreatment.DataSource = null;
                }
                gridViewTreatment.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                gridViewTreatment.EndUpdate();
                
            }
        }

        private void SetFilter(ref Filter.EmrTreatmentViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.CURRENT_DEPARTMENT_CODE__EXACT = (string)cboCurrentDepartmentCodeNew.EditValue;

                if (!string.IsNullOrEmpty(TxtTreatment.Text))
                {
                    if (chkAdd0.Checked)
                    {
                        TxtTreatment.Text = TxtTreatment.Text.Trim();
                    }
                    else
                    {
                        string code = TxtTreatment.Text.Trim();
                        if (code.Length < 12)
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            TxtTreatment.Text = code;
                        }
                    }
                    filter.TREATMENT_CODE__EXACT = TxtTreatment.Text;
                }
                else if (!string.IsNullOrEmpty(TxtPatient.Text))
                {
                    if (chkAdd0.Checked)
                    {
                        TxtPatient.Text = TxtPatient.Text.Trim();
                    }
                    else
                    {
                        string code = TxtPatient.Text.Trim();
                        if (code.Length < 10)
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            TxtPatient.Text = code;
                        }
                    }
                    filter.PATIENT_CODE__EXACT = TxtPatient.Text;
                }
                else
                {
                    if (DtTimeFromNew.EditValue != null && DtTimeFromNew.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeFromNew.DateTime.ToString("yyyyMMdd") + "000000");

                    if (DtTimeToNew.EditValue != null && DtTimeToNew.DateTime != DateTime.MinValue)
                        filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeToNew.DateTime.ToString("yyyyMMdd") + "235959");

                    if (!String.IsNullOrEmpty(TxtKeyword.Text))
                    {
                        filter.KEY_WORD = TxtKeyword.Text.Trim();
                    }
                }
				switch (radioGroup1.SelectedIndex)
				{
                    case 0:
                        filter.APPROVAL_SIGN_STATUS = null;
                        break;
                    case 1:
                        filter.APPROVAL_SIGN_STATUS = 1;
                        break;
                    case 2:
                        filter.APPROVAL_SIGN_STATUS = 2;
                        break;
                    case 3:
                        filter.APPROVAL_SIGN_STATUS = 3;
                        break;
                    default:
						break;
				}
                switch (radioGroup2.SelectedIndex)
                {
                    case 0:
                        filter.HAS_OUT_TIME = null;
                        break;
                    case 1:
                        filter.HAS_OUT_TIME = false;
                        break;
                    case 2:
                        filter.HAS_OUT_TIME = true;
                        break;
                    case 3:
                        filter.HAS_OUT_TIME = true;
                        filter.HAS_COUNT_NEXT_SIGN = false;
                        break;
                    default:
                        break;
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
                DtTimeFromNew.DateTime = DateTime.Now;
                DtTimeToNew.DateTime = DateTime.Now;
                TxtPatient.Text = "";
                TxtTreatment.Text = "";
                TxtKeyword.Text = "";
                TxtKeyword.Focus();
                TxtKeyword.SelectAll();
                radioGroup1.SelectedIndex = 0;
                radioGroup2.SelectedIndex = 0;
                btnSave.Enabled = false;
                cboCurrentDepartmentCodeNew.EditValue = this.currentDepartmentCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__BTN_REFRESH");
                this.BtnSearch.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__BTN_SEARCH");
                this.Gc_ClinicalInTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_CLINICAL_IN_TIME");
                this.Gc_CreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_CREATE_TIME");
                this.Gc_Creator.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_CREATOR");
                this.Gc_Department.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_DEPARTMENT");
                this.Gc_Dob.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_DOB");
                this.Gc_EndCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_END_CODE");
                this.Gc_Gender.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_GENDER");
                this.Gc_HeinCardNumber.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_HEIN_CARD_NUMBER");
                this.Gc_IcdName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_ICD_NAME");
                this.Gc_IcdText.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_ICD_TEXT");
                this.Gc_InCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_IN_CODE");
                this.Gc_InTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_IN_TIME");
                this.Gc_Modifier.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_MODIFIER");
                this.Gc_ModifyTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_MODIFY_TIME");
                this.Gc_OutCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_OUT_CODE");
                this.Gc_OutTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_OUT_TIME");
                this.Gc_PatientCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_PATIENT_CODE");
                this.Gc_PatientName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_PATIENT_NAME");
                this.Gc_PatientType.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_PATIENT_TYPE");
                this.Gc_StoreCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_STORE_CODE");
                this.Gc_StoreTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_STORE_TIME");
                this.Gc_Stt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_STT");
                this.Gc_TreatmentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_TREATMENT_CODE");
                this.Gc_TreatmentEndType.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_TREATMENT_END_TYPE");
                this.Gc_TreatmentResult.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_TREATMENT_RESULT");
                this.Gc_TreatmentType.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_TREATMENT_TYPE");
                this.Gc_View.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__GC_VIEW");
                //this.LciTimeFrom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__LCI_TIME_FROM");
                //this.LciTimeTo.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__LCI_TIME_TO");
                this.repositoryItemBtnView.Buttons[0].ToolTip = this.repositoryItemBtnViewDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__RP_BTN_VIEW");
                this.TxtKeyword.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__TXT_KEYWORD__NULL_VALUE");

                this.TxtPatient.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__TXT_PATIENT__NULL_VALUE");
                this.TxtTreatment.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__TXT_TREATMENT__NULL_VALUE");
                //this.BarDepartment.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__BAR_DEPARTMENT");
                this.repositoryItemBtnReqView.Buttons[0].ToolTip = this.repositoryItemBtnReqViewDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_TREATMENT_LIST__RP_BTN_REQ_VIEW");

                this.layoutControl1.Text = GetLanguageControl("UcEmrTreatmentList.layoutControl1.Text");
                this.chkAdd0.Properties.Caption = GetLanguageControl("UcEmrTreatmentList.chkAdd0.Properties.Caption");
                this.chkAdd0.ToolTip = GetLanguageControl("UcEmrTreatmentList.chkAdd0.ToolTip");
                //this.navBarControl1.Text = GetLanguageControl("UcEmrTreatmentList.navBarControl1.Text");
                //this.layoutControl2.Text = GetLanguageControl("UcEmrTreatmentList.layoutControl2.Text");
                //this.layoutControl3.Text = GetLanguageControl("UcEmrTreatmentList.layoutControl3.Text");
                this.cboCurrentDepartmentCodeNew.Properties.NullText = GetLanguageControl("UcEmrTreatmentList.cboCurrentDepartmentCode.Properties.NullText");
                //this.layoutControlItem12.Text = GetLanguageControl("UcEmrTreatmentList.layoutControlItem12.Text");
                this.Gc_ReqView.Caption = GetLanguageControl("UcEmrTreatmentList.Gc_ReqView.Caption");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSearch.Focus();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                BtnRefresh.Focus();
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR.EFMODEL.DataModels.V_EMR_TREATMENT data = (EMR.EFMODEL.DataModels.V_EMR_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "STORE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.STORE_TIME ?? 0);
                        }
                        else if(e.Column.FieldName == "Status")
						{
                            if (data.APPROVAL_SIGN_STATUS == 1)
                            {
                                e.Value = imageList1.Images[0];
                            }
                            else if (data.APPROVAL_SIGN_STATUS == 2)
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else if (data.APPROVAL_SIGN_STATUS == 3)
                            {
                                e.Value = imageList1.Images[2];
                            }
                            else
                            {
                                e.Value = "";
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

        #region Public method
        public void Search()
        {
            try
            {
                BtnSearch_Click(null, null);
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
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Save()
		{
			try
			{
                btnSave_Click(null,null);
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}            
        #endregion

        private void TxtTreatment_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(TxtTreatment.Text))
                        BtnSearch_Click(null, null);
                    else
                    {
                        TxtPatient.Focus();
                        TxtPatient.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtPatient_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(TxtPatient.Text))
                        BtnSearch_Click(null, null);
                    else
                    {
                        TxtKeyword.Focus();
                        TxtKeyword.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatment_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                //REQ_VIEW
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string storeCode = (View.GetRowCellValue(e.RowHandle, "STORE_CODE") ?? "").ToString();
                    V_EMR_TREATMENT data = (V_EMR_TREATMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "REQ_VIEW")
                    {
                        if (String.IsNullOrWhiteSpace(storeCode))
                        {
                            e.RepositoryItem = repositoryItemBtnReqViewDisable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnReqView;
                        }
                    }

                    if (e.Column.FieldName == "Approval")
                    {
                        if (data.APPROVAL_SIGN_STATUS == null || data.APPROVAL_SIGN_STATUS == 1)
                            e.RepositoryItem = repApprovalEnable;
                        else if (data.APPROVAL_SIGN_STATUS == 2 || data.APPROVAL_SIGN_STATUS == 3)
                            e.RepositoryItem = repApprovalDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                 isNotLoadWhileChangeControlStateInFirst = true;
                 this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                 this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                 if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                 {
                     foreach (var item in this.currentControlStateRDO)
                     {
                         if (item.KEY == chkAdd0.Name)
                         {
                             chkAdd0.Checked = item.VALUE == "1";
                         }
                     }
                 }
                 isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAdd0_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAdd0.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAdd0.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAdd0.Name;
                    csAddOrUpdate.VALUE = (chkAdd0.Checked ? "1" : "");
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

        private void cboCurrentDepartmentCode_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCurrentDepartmentCodeNew.EditValue = null;
                    cboCurrentDepartmentCodeNew.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCurrentDepartmentCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboCurrentDepartmentCodeNew.EditValue != null)
                {
                    cboCurrentDepartmentCodeNew.Properties.Buttons[1].Visible = true;
                }
                else 
                {
                    cboCurrentDepartmentCodeNew.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

		private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlTreatment)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlTreatment.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
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
                                long? isConfirm = null;
                                if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "APPROVAL_SIGN_STATUS") ?? "").ToString()))
                                {
                                    isConfirm = Convert.ToInt64((view.GetRowCellValue(lastRowHandle, "APPROVAL_SIGN_STATUS") ?? "").ToString());
                                }
                                if (isConfirm == 1)
                                {
                                    text = "Chưa ký duyệt";
                                }
                                else if (isConfirm == 2)
                                {
                                    text = "Đang ký duyệt";
                                }
                                else if (isConfirm == 3)
                                {
                                    text = "Đã ký duyệt";
                                }
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

        void GetControlAcs()
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.SDO.AcsTokenLoginSDO tokenLoginSDOForAuthorize = new ACS.SDO.AcsTokenLoginSDO();
                tokenLoginSDOForAuthorize.LOGIN_NAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                tokenLoginSDOForAuthorize.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;

                var acsAuthorize = new BackendAdapter(param).Get<ACS.SDO.AcsAuthorizeSDO>(HIS.Desktop.ApiConsumer.AcsRequestUriStore.ACS_TOKEN__AUTHORIZE, HIS.Desktop.ApiConsumer.ApiConsumers.AcsConsumer, tokenLoginSDOForAuthorize, param);

                if (acsAuthorize != null)
                {
                    controlAcs = acsAuthorize.ControlInRoles.ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("ACS control", controlAcs));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void VisbleColumnGrid()
		{
			try
			{
                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnSave) != null)
                { 
                    gridColumn1.VisibleIndex = 4;
                    gridColumn2.VisibleIndex = 5;
                    gridColumn1.Fixed = FixedStyle.Left;
                    gridColumn2.Fixed = FixedStyle.Left;
                }                   
                else
				{
                    btnSave.Enabled = false;
                    gridColumn1.VisibleIndex = -1;
                    gridColumn2.VisibleIndex = -1;
                }                    
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			try
			{
                var selectedRows = gridViewTreatment.GetSelectedRows();
                List<V_EMR_TREATMENT> lstEmrTreatment = new List<V_EMR_TREATMENT>();
                foreach (var i in selectedRows)
				{
                    lstEmrTreatment.Add((V_EMR_TREATMENT)gridViewTreatment.GetRow(i));
				}                    
                frmStatusApproval frm = new frmStatusApproval(lstEmrTreatment,ReloadList);
                frm.Show();
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void ReloadList(bool obj)
		{
			try
			{
                if (obj)
                    FillDataToGrid();                    
			}
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        private void ApproveSign() 
		{ 
            try
            {
                WaitingManager.Show();
                var row = (EMR.EFMODEL.DataModels.V_EMR_TREATMENT)gridViewTreatment.GetFocusedRow();
                CommonParam paramCommon = new CommonParam();
                var apiResult = new Inventec.Common.Adapter.BackendAdapter
                           (paramCommon).Post<bool>
                           ("api/EmrTreatment/ApproveSign", ApiConsumers.EmrConsumer, row.ID, SessionManager.ActionLostToken, paramCommon);
                if (apiResult)
                {
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, paramCommon, apiResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UnapproveSign()
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_TREATMENT)gridViewTreatment.GetFocusedRow();
                var yes = XtraMessageBox.Show("Bạn có chắc muốn thực hiện hủy duyệt hồ sơ không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DefaultBoolean.True);
                if (yes != DialogResult.Yes)
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                var apiResult = new Inventec.Common.Adapter.BackendAdapter
                           (paramCommon).Post<bool>
                           ("api/EmrTreatment/UnapproveSign", ApiConsumers.EmrConsumer, row.ID, SessionManager.ActionLostToken, paramCommon);
                if (apiResult)
                {
                    FillDataToGrid();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, paramCommon, apiResult);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatment_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
                if (gridViewTreatment.GetSelectedRows().Count() > 0)
                {
                    if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.BtnSave) != null)
					{
                        btnSave.Enabled = true;
                    }
					else
					{
                        btnSave.Enabled = false;
                    }                        
                }
				else
				{
                    btnSave.Enabled = false;
				}

            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

		private void gridViewTreatment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
		{
			try
			{
                if (e.Column.Caption == "Selection")
                    return;
                var row = gridViewTreatment.GetFocusedRow() as V_EMR_TREATMENT;
                if (row != null)
                {
                    if (e.Column.FieldName == "Approval")
                    {
                        if(row.APPROVAL_SIGN_STATUS == null ||row.APPROVAL_SIGN_STATUS == 1)
						{
                            ApproveSign();
                        }else if (row.APPROVAL_SIGN_STATUS == 2 || row.APPROVAL_SIGN_STATUS == 3)
                        {
                            UnapproveSign();
                        }
                    }
                    else if (e.Column.FieldName == "VIEW")
                    {
                        if (row != null)
                        {
                            // popup yêu cầu xem
                            List<object> _listObj = new List<object>();
                            _listObj.Add(row.ID);
                            WaitingManager.Hide();
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EmrDocument", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                        }
                    }
                    else if (e.Column.FieldName == "REQ_VIEW")
                    {
                        if (row != null && !String.IsNullOrEmpty(row.STORE_CODE))
                        {
                            // popup yêu cầu xem
                            List<object> _listObj = new List<object>();
                            _listObj.Add(row.ID);

                            WaitingManager.Hide();
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EmrViewerReq", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                        }
                    }
                }
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}
	}
}
