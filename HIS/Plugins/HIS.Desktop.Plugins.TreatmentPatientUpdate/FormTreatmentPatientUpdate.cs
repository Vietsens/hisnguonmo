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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentPatientUpdate.Base;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.TreatmentPatientUpdate.Config;
using MOS.Filter;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.TreatmentPatientUpdate
{
    public partial class FormTreatmentPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        #region Declare
        long treatmentId;
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        string moduleLink = "HIS.Desktop.Plugins.TreatmentPatientUpdate";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT> treatmentList;
        System.Globalization.CultureInfo cultureLang;
        MOS.EFMODEL.DataModels.V_HIS_PATIENT patient;
        List<Base.HisPatientADO> listPatient;
        ApiResultObject<List<Base.HisPatientADO>> apiResult = null;
        List<string> listPatientCode;
        HIS.Desktop.Common.DelegateSelectData delegateSelect;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();

        #endregion

        #region Construct
        public FormTreatmentPatientUpdate()
        {
            InitializeComponent();
            try
            {
                treatmentList = new List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTreatmentPatientUpdate(long _treatmentId, Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                treatmentList = new List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.treatmentId = _treatmentId;
                LoadTreatment();
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormTreatmentPatientUpdate(List<string> listPatientCode, long _treatmentId, Inventec.Desktop.Common.Modules.Module moduleData,HIS.Desktop.Common.DelegateSelectData dele)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.listPatientCode = listPatientCode;
                this.delegateSelect = dele;
                treatmentList = new List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.treatmentId = _treatmentId;
                LoadTreatment();
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormTreatmentPatientUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIcon();
                Config.HisConfigCFG.LoadConfig();
                LoadKeysFromlanguage();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                LoadControlsState();

                if (listPatientCode != null && listPatientCode.Count > 0)
                {
                    txtPatientCode.Text = listPatientCode.FirstOrDefault();
                }

                    FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadControlsState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ControlStateConstant.chkIsUpdateEmr)
                        {
                            chkIsUpdateEmr.Checked = item.VALUE == "1";
                        }
                    }
                }

                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.rdUpdateAllTreatment && o.IS_ACTIVE == IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE) != null)
                {
                    chkUpdateAllTreatment.Checked = false;
                    chkUpdateAllTreatment.Enabled = true;
                }else
	            {
                    chkUpdateAllTreatment.Checked = false;
                    chkUpdateAllTreatment.Enabled = false;
	            }
                if (controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == ControlCode.rdUpdateUnlockTreatment && o.IS_ACTIVE == IMSys.DbConfig.ACS_RS.COMMON.IS_ACTIVE__TRUE) != null)
                {
                    chkUpdateUnlockTreatment.Checked = false;
                    chkUpdateUnlockTreatment.Enabled = true;
                }
                else
                {
                    chkUpdateUnlockTreatment.Checked = false;
                    chkUpdateUnlockTreatment.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                //filter
                txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__TXT_KEY_WORD__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__TXT_PATIENT_CODE__NULL_VALUE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                btnUpdate.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__BTN_UPDATE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                //gridView
                Gc_Address.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__GC_ADDRESS",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_CareerName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__GC_CAREER_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_CommuneName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__COMMUNE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_Creator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__CREATOR",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_DistrictName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__DISTRICT_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_DOB.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__DOB",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_EthnicName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__ETHNIC_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_Gender.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__GENDER",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_NationalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_PatientCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__PATIENT_CODE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_PatientName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__PATIENT_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_Phone.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__PHONE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_ProvinceName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__PROVINCE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_RelativeAddress.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__RELATIVE_ADDRESS",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_RelativeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__RELATIVE_NAME",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_RelativeType.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__RELATIVE_TYPE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__STT",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
                Gc_WorkPlace.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__FORM_TREATMENT_PATIENT_UPDATE__WORK_PLACE",
                    Resources.ResourceLanguageManager.LanguageFormTreatmentPatientChange,
                    cultureLang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatment()
        {
            try
            {
                MOS.Filter.HisTreatmentViewFilter filter = new MOS.Filter.HisTreatmentViewFilter();
                filter.ID = treatmentId;
                treatmentList = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
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
                if (treatmentList != null && treatmentList.Count > 0)
                {
                    WaitingManager.Show();
                    //int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                    //GridPaging(new CommonParam(0, pagingSize));
                    int pageSize;
                    if (ucPaging.pagingGrid != null)
                    {
                        pageSize = ucPaging.pagingGrid.PageSize;
                    }
                    else
                    {
                        pageSize = (int)ConfigApplications.NumPageSize;
                    }
                    GridPaging(new CommonParam(0, pageSize));
                    CommonParam param = new CommonParam();
                    param.Limit = rowCount;
                    param.Count = dataTotal;
                    ucPaging.Init(GridPaging, param, pageSize);
                    WaitingManager.Hide();
                }
                else
                {
                    gridControl.DataSource = null;
                    rowCount = 0;
                    dataTotal = 0;
                }
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

                MOS.Filter.HisPatientViewFilter filter = new MOS.Filter.HisPatientViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<Base.HisPatientADO>>
                    (ApiConsumer.HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null && apiResult.Success)
                {
                    listPatient = apiResult.Data.Where(o => o.ID != treatmentList.FirstOrDefault().PATIENT_ID).ToList();
                    if (listPatient != null && listPatient.Count > 0)
                    {
                        gridControl.DataSource = listPatient;
                        rowCount = (listPatient == null ? 0 : listPatient.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count - 1 ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = 0;
                        dataTotal = 0;
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
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisPatientViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "ASC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
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
                    var data = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                {
                    gridView.BeginUpdate();
                    Base.HisPatientADO row = (Base.HisPatientADO)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var patientAlls = gridControl.DataSource as List<Base.HisPatientADO>;

                        if (patientAlls != null)
                        {
                            foreach (var pa in patientAlls)
                            {
                                if (pa.ID == row.ID)
                                {
                                    pa.IsChecked = !pa.IsChecked;
                                }
                                else pa.IsChecked = false;
                            }

                            gridView.GridControl.DataSource = patientAlls;
                        }
                    }
                    gridView.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void gridView_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Clicks == 2 && e.Column.FieldName != "IsChecked")
                {
                    gridView.BeginUpdate();
                    Base.HisPatientADO row = (Base.HisPatientADO)gridView.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        var patientAlls = gridControl.DataSource as List<Base.HisPatientADO>;

                        if (patientAlls != null)
                        {
                            foreach (var pa in patientAlls)
                            {
                                if (pa.ID == row.ID)
                                {
                                    pa.IsChecked = !pa.IsChecked;
                                }
                                else pa.IsChecked = false;
                            }

                            gridView.GridControl.DataSource = patientAlls;
                        }
                    }
                    gridView.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                var isChecked = (gridView.GetRowCellValue(e.RowHandle, "IsChecked") ?? "").ToString();

                if (Inventec.Common.TypeConvert.Parse.ToBoolean(isChecked ?? "") == true)
                {
                    e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
                }
                else
                    e.Appearance.ForeColor = Color.Black;
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
                        gridView.Focus();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
                if (e.KeyCode == Keys.Down)
                {
                    gridView.Focus();
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
                    FillDataToGrid();
                    gridView.Focus();
                }
                if (e.KeyCode == Keys.Down)
                {
                    gridView.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckPatient_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var chk = sender as DevExpress.XtraEditors.CheckEdit;
                var row = (Base.HisPatientADO)gridView.GetFocusedRow();
                if (row != null)
                {
                    gridView.BeginUpdate();
                    var patientAlls = gridControl.DataSource as List<Base.HisPatientADO>;

                    if (patientAlls != null)
                    {
                        foreach (var pa in patientAlls)
                        {
                            if (pa.ID == row.ID)
                            {
                                pa.IsChecked = chk.Checked;
                            }
                            else pa.IsChecked = false;
                        }

                        gridView.GridControl.DataSource = patientAlls;
                    }
                    gridView.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                patient = new MOS.EFMODEL.DataModels.V_HIS_PATIENT();
                if (listPatient.Count > 0)
                {
                    foreach (var item in listPatient)
                    {
                        if (item.IsChecked)
                        {
                            patient = item;
                            break;
                        }
                    }
                }
                if (patient == null || patient.ID == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.HeThongThongBaoVuiLongChonMotBenhNhan,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    bool IsNotShowMessage = false;
                    if(HisConfigCFG.IsWarningPreviousDebt)
					{
                        HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                        filter.PATIENT_CODE__EXACT = patient.PATIENT_CODE;
                        var data = (new BackendAdapter(param).Get<List<HisPatientSDO>>("api/HisPatient/GetSdoAdvance", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param)).SingleOrDefault();

						Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                        Inventec.Common.Logging.LogSystem.Debug("HisConfigCFG.PreviousDebptOption_______"+Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.PreviousDebptOption), HisConfigCFG.PreviousDebptOption));                         
                        switch (HisConfigCFG.PreviousDebptOption)
						{                          
                            case "1":
                                if (data.PreviousDebtTreatments != null && data.PreviousDebtTreatments.Count > 0)
                                {
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                                       String.Format(Resources.ResourceMessage.BnNoVienPhiQues, patient.VIR_PATIENT_NAME),
                                       Resources.ResourceMessage.ThongBao,
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                                    {
                                        return;
                                    }
                                    IsNotShowMessage = true;                                  
                                }
                                break;
                            case "2":
                                bool IsPatientTypeBhyt = false;
                                if (data.PreviousDebtTreatmentDetails != null && data.PreviousDebtTreatmentDetails.Count() > 0)
                                {
									foreach (var item in data.PreviousDebtTreatmentDetails.ToList())
									{
                                        HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                                        treatmentFilter.TREATMENT_CODE__EXACT = item.TDL_TREATMENT_CODE;
                                        var dt = (new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param)).SingleOrDefault();
                                        if(dt.TDL_PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
										{
                                            IsPatientTypeBhyt = true;
                                            break;
										}                                           
                                    }
                                }
                                if (IsPatientTypeBhyt)
								{
                                    DevExpress.XtraEditors.XtraMessageBox.Show(
                                    String.Format(Resources.ResourceMessage.BnNoVienPhi, patient.VIR_PATIENT_NAME),
                                    Resources.ResourceMessage.ThongBao,
                                    MessageBoxButtons.OK);
                                    return;
                                }
                                break;
                            case "3":
                                if(treatmentList.First().TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && treatmentList.First().IS_EMERGENCY != 1 && data.PreviousDebtTreatmentDetails !=null && data.PreviousDebtTreatmentDetails.Count > 0)
								{
                                    DevExpress.XtraEditors.XtraMessageBox.Show(
                                   String.Format(Resources.ResourceMessage.BnNoVienPhi, patient.VIR_PATIENT_NAME),
                                   Resources.ResourceMessage.ThongBao,
                                   MessageBoxButtons.OK);
                                    return;
                                }                                    
                                break;                              
                        }                            
					}                        

                    if (IsNotShowMessage ||  DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongThongBaoBanCoMuonCapNhatThongTinBenhNhanNay,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        WaitingManager.Show();
                        MOS.SDO.HisTreatmentUpdatePatiSDO updateDTO = new MOS.SDO.HisTreatmentUpdatePatiSDO();
                        MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();
                        LoadDTOToProcessUpdate(ref updateDTO, ref treatment);
                        
                        var data = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_TREATMENT>>(ApiConsumer.HisRequestUriStore.HIS_TREATMENT_UPDATE_PATIENT, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        WaitingManager.Hide();
                        if (data != null)
                        {
                            success = true;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("treatment.PATIENT_ID", treatment.PATIENT_ID));
                            if (this.delegateSelect != null)
                                this.delegateSelect(treatment.PATIENT_ID);
                            this.Close();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadDTOToProcessUpdate(ref MOS.SDO.HisTreatmentUpdatePatiSDO updateDTO, ref MOS.EFMODEL.DataModels.HIS_TREATMENT treatment)
        {
            try
            {
                AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT, HIS_TREATMENT>();
                treatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT, HIS_TREATMENT>(treatmentList.FirstOrDefault());
                treatment.ID = treatmentId;
                treatment.PATIENT_ID = patient.ID;

                updateDTO.HisTreatment = treatment;

                if (chkIsUpdateEmr.Checked)
                {
                    updateDTO.IsUpdateEmr = true;
                }
                else
                {
                    updateDTO.IsUpdateEmr = false;
                }

                if (chkUpdateAllTreatment.Checked)
                {
                    updateDTO.IsUpdateAllOtherTreatements = true;
                    updateDTO.IsUpdateTreatmentUnLocked = false;
                }
                else if (chkUpdateUnlockTreatment.Checked)
                {
                    updateDTO.IsUpdateTreatmentUnLocked = true;
                    updateDTO.IsUpdateAllOtherTreatements = false;
                }
                else
                {
                    updateDTO.IsUpdateAllOtherTreatements = false;
                    updateDTO.IsUpdateTreatmentUnLocked = false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Shortcut
        private void barButtonItemFocus_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItemUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkUpdateUnlockTreatment_MouseUp(object sender, MouseEventArgs e)
        {
            CheckEdit edit = sender as CheckEdit;
            if (edit.Checked && !_editValueChanging)
            {
                edit.Checked = false;
            }
            else
            {
                _editValueChanging = false;
            }  
        }

        private void chkUpdateAllTreatment_MouseUp(object sender, MouseEventArgs e)
        {
            CheckEdit edit = sender as CheckEdit;
            if (edit.Checked && !_editValueChanging)
            {
                edit.Checked = false;
            }
            else
            {
                _editValueChanging = false;
            }  
        }

        bool _editValueChanging = false;
        private void chkUpdateUnlockTreatment_EditValueChanging(object sender, ChangingEventArgs e)
        {
            _editValueChanging = true;  
        }

        private void chkUpdateAllTreatment_EditValueChanging(object sender, ChangingEventArgs e)
        {
            _editValueChanging = true;  
        }

        private void chkIsUpdateEmr_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == ControlStateConstant.chkIsUpdateEmr && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsUpdateEmr.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = ControlStateConstant.chkIsUpdateEmr;
                    csAddOrUpdate.VALUE = (chkIsUpdateEmr.Checked ? "1" : "");
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
