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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.AccidentHurt.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoMapper;
using HIS.Desktop.LocalStorage.Location;
using System.Configuration;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.Utility;
using MOS.SDO;
using HIS.Desktop.Common;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd;
using HIS.UC.DateEditor;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.CustomControl;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.RichEditor.Base;

namespace HIS.Desktop.Plugins.AccidentHurt
{
    public partial class frmAccidentHurt : HIS.Desktop.Utility.FormBase
    {
        internal long treatmentId = 0;
        List<HIS_ACCIDENT_BODY_PART> accidentBodyParts { get; set; }
        List<HIS_ACCIDENT_CARE> accidentCares { get; set; }
        List<HIS_ACCIDENT_HELMET> accidentHelmets { get; set; }
        List<HIS_ACCIDENT_HURT_TYPE> accidentHurtTypes { get; set; }
        List<HIS_ACCIDENT_LOCATION> accidentLocations { get; set; }
        List<HIS_ACCIDENT_POISON> accidentPoisons { get; set; }
        List<HIS_ACCIDENT_RESULT> accidentResults { get; set; }
        List<HIS_ACCIDENT_VEHICLE> accidentVehicles { get; set; }
        HIS_ACCIDENT_HURT accidentHurt { get; set; }

        HisAccidentHurtSDO accidentHurtSDO { get; set; }
        HIS.UC.SecondaryIcd.RefeshReference _refreshClick { get; set; }
        HIS_PATIENT hisPatient = new HIS_PATIENT();
        List<HIS_PATIENT> LstHisPatient = new List<HIS_PATIENT>();
        internal MOS.EFMODEL.DataModels.V_HIS_ACCIDENT_HURT AccidentHurtData = null;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        public int positionHandle = -1;
        HIS_ACCIDENT_HURT accdentHurtResult = new HIS_ACCIDENT_HURT();
        private bool IsTreatmentList;
        HIS_TREATMENT treatment = null;
        int action = 0;
        HIS.Desktop.Common.DelegateRefeshTreatmentPartialData dlg;
        string _TextIcdName = "";
        List<HIS_ICD> currentIcds;
        HIS_ICD icdPopupSelect;


        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal UCDateProcessor ucDateProcessor;

        public frmAccidentHurt()
        {
            InitializeComponent();
        }

        public frmAccidentHurt(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = currentModule;
            this.treatmentId = treatmentId;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
            }
        }

        public frmAccidentHurt(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId, bool isTreatmentList, HIS.Desktop.Common.DelegateRefeshTreatmentPartialData dlg)
            : base(currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = currentModule;
            this.treatmentId = treatmentId;
            this.IsTreatmentList = isTreatmentList;
            if (this.currentModule != null)
            {
                this.Text = currentModule.text;
            }
            this.dlg = dlg;
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmAccidentHurt_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ValidationControl();

                //InitUcIcd();
                //  InitUcSecondaryIcd();
                //ResourceLanguageManager.InitResourceLanguageManager();
                dtAccidentTime.DateTime = DateTime.Now;
                LoadDataToCombo();
                LoadDataAccidentHurt();
                LoadKeysFromLanguage();


                //if ((treatment != null && treatment.IS_PAUSE == 1) || IsTreatmentList)
                if (treatment != null || IsTreatmentList)
                {
                    btnSave.Enabled = true;



                    if (AccidentHurtData == null)
                        btnPrint.Enabled = false;
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadHisPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                LstHisPatient = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                if (LstHisPatient != null && LstHisPatient.Count > 0)
                {
                    hisPatient = LstHisPatient.FirstOrDefault();
                }
                if (hisPatient.CMND_NUMBER != null)
                {
                    txtCMNDNo.Text = hisPatient.CMND_NUMBER;
                    txtTimeProvided.Text = "";
                    dtTimeProvided.Text = "";
                    if (hisPatient.CMND_DATE != null)
                    {
                        this.dtTimeProvided.EditValue = ConvertDateStringToSystemDate(hisPatient.CMND_DATE.ToString());
                        txtTimeProvided.Text = dtTimeProvided.DateTime.ToString();
                    }
                    txtCMNDPlace.Text = hisPatient.CMND_PLACE;
                }
                else
                {
                    txtCMNDNo.Text = hisPatient.CCCD_NUMBER;
                    txtTimeProvided.Text = "";
                    dtTimeProvided.Text = "";
                    if (hisPatient.CCCD_DATE != null)
                    {
                        this.dtTimeProvided.EditValue = ConvertDateStringToSystemDate(hisPatient.CCCD_DATE.ToString());
                        txtTimeProvided.Text = dtTimeProvided.DateTime.ToString();
                    }
                    txtCMNDPlace.Text = hisPatient.CCCD_PLACE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataAccidentHurt()
        {
            try
            {

                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = this.treatmentId;
                    var treatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                    if (treatments != null && treatments.Count() > 0)
                    {
                        treatment = treatments.FirstOrDefault();
                        LoadHisPatient();
                    }
                    param = new CommonParam();
                    MOS.Filter.HisAccidentHurtViewFilter AccidentHurtFilter = new MOS.Filter.HisAccidentHurtViewFilter();
                    AccidentHurtFilter.TREATMENT_ID = this.treatmentId;
                    AccidentHurtData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCIDENT_HURT>>(
                        HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ACCIDENT_HURT_GETVIEW,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        AccidentHurtFilter,
                        param).FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AccidentHurtData), AccidentHurtData));
                    if (AccidentHurtData != null)
                    {
                        this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                        // lOAD DỮ LIỆU LÊN FROM
                        dtAccidentTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime((long)AccidentHurtData.ACCIDENT_TIME) ?? DateTime.MinValue;
                        LoadAllAccidentData(treatment);
                        
                    }
                    else
                    {
                        this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd;
                    }
                    if (AccidentHurtData != null) 
                    {
                        if (!string.IsNullOrEmpty(AccidentHurtData.ACCIDENT_HURT_ICD_CODE) && !string.IsNullOrEmpty(AccidentHurtData.ACCIDENT_HURT_ICD_NAME))
                        {
                            LoadIcdToControl(AccidentHurtData.ACCIDENT_HURT_ICD_CODE, AccidentHurtData.ACCIDENT_HURT_ICD_NAME);
                        }
                    }
                    else
                    {
                        if (treatment != null)
                        {
                            memoGetInHospital.Text = treatment.HOSPITALIZATION_REASON;
                        }
                        LoadIcdToControl(treatment.ICD_CODE, treatment.ICD_NAME);
                    }
                    if (AccidentHurtData != null)
                    {
                        if (!string.IsNullOrEmpty(AccidentHurtData.ACCIDENT_HURT_ICD_SUB_CODE) && !string.IsNullOrEmpty(AccidentHurtData.ACCIDENT_HURT_ICD_TEXT))
                        {
                            LoadIcdCauseToControl(AccidentHurtData.ACCIDENT_HURT_ICD_SUB_CODE, AccidentHurtData.ACCIDENT_HURT_ICD_TEXT);
                        }
                    }
                    else
                    {
                        LoadIcdCauseToControl(treatment.ICD_SUB_CODE, treatment.ICD_TEXT);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAllAccidentData(HIS_TREATMENT treatment)
        {
            txtAccidentLocaltion.Text = AccidentHurtData.ACCIDENT_LOCATION_CODE;
            cboAccidentLocaltion.EditValue = AccidentHurtData.ACCIDENT_LOCATION_ID;

            txtContent.Text = AccidentHurtData.CONTENT;
            if (!string.IsNullOrEmpty(AccidentHurtData.HOSPITALIZATION_REASON))
            {
                memoGetInHospital.Text = AccidentHurtData.HOSPITALIZATION_REASON;
            }
            else
            {
                if (treatment != null)
                {
                    memoGetInHospital.Text = treatment.HOSPITALIZATION_REASON;
                }
            }

            txtAccidentBodyPart.Text = AccidentHurtData.ACCIDENT_BODY_PART_CODE;
            cboAccidentBodyPart.EditValue = AccidentHurtData.ACCIDENT_BODY_PART_ID;

            txtAccidentHurtType.Text = AccidentHurtData.ACCIDENT_HURT_TYPE_CODE;
            cboAccidentHurtType.EditValue = AccidentHurtData.ACCIDENT_HURT_TYPE_ID;

            txtAccidentPoison.Text = AccidentHurtData.ACCIDENT_POISON_CODE;
            cboAccidentPoison.EditValue = AccidentHurtData.ACCIDENT_POISON_ID;

            txtHelmet.Text = AccidentHurtData.ACCIDENT_HELMET_CODE;
            cboHelmet.EditValue = AccidentHurtData.ACCIDENT_HELMET_ID;

            txtAccidentVehicle.Text = AccidentHurtData.ACCIDENT_VEHICLE_CODE;
            cboAccidentVehicle.EditValue = AccidentHurtData.ACCIDENT_VEHICLE_ID;

            txtAccidentResult.Text = AccidentHurtData.ACCIDENT_RESULT_CODE;
            cboAccidentResult.EditValue = AccidentHurtData.ACCIDENT_RESULT_ID;

            txtAccidentCare.Text = AccidentHurtData.ACCIDENT_CARE_CODE;
            cboAccidentCare.EditValue = AccidentHurtData.ACCIDENT_CARE_ID;

            List<HIS_SERVICE_REQ> serviceReq = new List<HIS_SERVICE_REQ>();
            if (String.IsNullOrEmpty(AccidentHurtData.TREATMENT_INFO) || String.IsNullOrEmpty(AccidentHurtData.STATUS_IN))
            {
                //Lấy thông tin Phương pháp điều trị của y lệnh khám đầu tiên
                CommonParam param = new CommonParam();
                MOS.Filter.HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.ORDER_FIELD = "CREATE_TIME";
                serviceReqFilter.ORDER_DIRECTION = "ASC";
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH;
                serviceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumer.ApiConsumers.MosConsumer, serviceReqFilter, param);
            }
            if (!String.IsNullOrEmpty(AccidentHurtData.STATUS_IN))
                txtStatusIn.Text = AccidentHurtData.STATUS_IN;
            else
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReq), serviceReq));
                if (serviceReq != null && serviceReq.Count > 0)
                    txtStatusIn.Text = serviceReq.FirstOrDefault().FULL_EXAM + "\r\n" + serviceReq.FirstOrDefault().PART_EXAM;
            }

            txtStatusOut.Text = AccidentHurtData.STATUS_OUT;
            if (!String.IsNullOrEmpty(AccidentHurtData.TREATMENT_INFO))
                txtTreatmentInfo.Text = AccidentHurtData.TREATMENT_INFO;
            else
            {
                if (serviceReq != null && serviceReq.Count > 0)
                    txtTreatmentInfo.Text = serviceReq.FirstOrDefault().TREATMENT_INSTRUCTION;
            }
            if (AccidentHurtData.IS_USE_ALCOHOL == 1)
            {
                chkUseAlcohol.Checked = true;
            }

            if (AccidentHurtData.NARCOTICS_TEST_RESULT == (short)1)
            {
                chkNarcoticsTestResult_Positve.CheckState = CheckState.Checked;
            }
            else if (AccidentHurtData.NARCOTICS_TEST_RESULT == (short)2)
            {
                chkNarcoticsTestResult_Negative.CheckState = CheckState.Checked;
            }
            else
            {
                chkNarcoticsTestResult_Positve.CheckState = CheckState.Unchecked;
                chkNarcoticsTestResult_Negative.CheckState = CheckState.Unchecked;
            }

            if (AccidentHurtData.ALCOHOL_TEST_RESULT == (short)1)
            {
                chkAncoholtestResult_Over.CheckState = CheckState.Checked;
            }
            else if (AccidentHurtData.ALCOHOL_TEST_RESULT == (short)2)
            {
                chkAncoholtestResult_Enough.CheckState = CheckState.Checked;
            }
            else
            {
                chkAncoholtestResult_Over.CheckState = CheckState.Unchecked;
                chkAncoholtestResult_Enough.CheckState = CheckState.Unchecked;
            }
        }

        private DateTime? ConvertDateStringToSystemDate(string dateTime)
        {
            DateTime? result = null;
            try
            {
                if (!String.IsNullOrEmpty(dateTime))
                {
                    dateTime = dateTime.Replace(" ", "");
                    int day = Int16.Parse(dateTime.Substring(6, 2));
                    int month = Int16.Parse(dateTime.Substring(4, 2));
                    int year = Int16.Parse(dateTime.Substring(0, 4));
                    result = new DateTime(year, month, day);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        private void LoadDataToCombo()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisAccidentHurtViewFilter AccidentHurtFilter = new MOS.Filter.HisAccidentHurtViewFilter();
                HisAccidentBodyPartFilter filter = new HisAccidentBodyPartFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                accidentBodyParts = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_BODY_PART>>(
                    "/api/HisAccidentBodyPart/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                filter,
                param);

                accidentCares = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_CARE>>(
                    "/api/HisAccidentCare/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentHelmets = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_HELMET>>(
                    "/api/HisAccidentHelmet/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentHurtTypes = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_HURT_TYPE>>(
                HIS.Desktop.ApiConsumer.HisRequestUriStore.HIS_ACCIDENT_HURT_TYPE_GET,
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentLocations = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_LOCATION>>(
                    "/api/HisAccidentLocation/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentPoisons = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_POISON>>(
                    "/api/HisAccidentPoison/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentResults = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_RESULT>>(
                    "/api/HisAccidentResult/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);

                accidentVehicles = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_ACCIDENT_VEHICLE>>(
                    "/api/HisAccidentVehicle/Get",
                HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                AccidentHurtFilter,
                param);
                this.currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).OrderBy(o => o.ICD_CODE).ToList();
                DataToComboChuanDoanTD(cboCdChinh, this.currentIcds);
                LoadDataToComboAccidentBodyPart(cboAccidentBodyPart, accidentBodyParts);

                LoadDataToComboAccidentCare(cboAccidentCare, accidentCares);

                LoadDataToComboAccidentHelmet(cboHelmet, accidentHelmets);

                LoadDataToComboAccidentHurtType(cboAccidentHurtType, accidentHurtTypes);

                LoadDataToComboAccidentLocation(cboAccidentLocaltion, accidentLocations);

                LoadDataToComboAccidentPoison(cboAccidentPoison, accidentPoisons);

                LoadDataToComboAccidentResult(cboAccidentResult, accidentResults);

                LoadDataToComboAccidentVehicle(cboAccidentVehicle, accidentVehicles);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentLocaltion_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentLocaltion.EditValue != null)
                    {
                        HIS_ACCIDENT_LOCATION data = accidentLocations.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentLocaltion.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentLocaltion.Text = data.ACCIDENT_LOCATION_CODE;
                            cboAccidentLocaltion.Properties.Buttons[1].Visible = true;
                            txtAccidentBodyPart.Focus();
                            txtAccidentBodyPart.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentBodyPart.EditValue != null)
                    {
                        HIS_ACCIDENT_BODY_PART data = accidentBodyParts.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentBodyPart.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentBodyPart.Text = data.ACCIDENT_BODY_PART_CODE;
                            cboAccidentBodyPart.Properties.Buttons[1].Visible = true;
                            dtAccidentTime.Focus();
                            dtAccidentTime.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentHurtType.EditValue != null)
                    {
                        HIS_ACCIDENT_HURT_TYPE data = accidentHurtTypes.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentHurtType.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentHurtType.Text = data.ACCIDENT_HURT_TYPE_CODE;
                            cboAccidentHurtType.Properties.Buttons[1].Visible = true;
                            txtContent.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentPoison.EditValue != null)
                    {
                        HIS_ACCIDENT_POISON data = accidentPoisons.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentPoison.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentPoison.Text = data.ACCIDENT_POISON_CODE;
                            cboAccidentPoison.Properties.Buttons[1].Visible = true;
                            txtHelmet.Focus();
                            txtHelmet.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboHelmet.EditValue != null)
                    {
                        HIS_ACCIDENT_HELMET data = accidentHelmets.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboHelmet.EditValue.ToString()));
                        if (data != null)
                        {
                            txtHelmet.Text = data.ACCIDENT_HELMET_CODE;
                            cboHelmet.Properties.Buttons[1].Visible = true;
                            txtAccidentVehicle.Focus();
                            txtAccidentVehicle.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentVehicle.EditValue != null)
                    {
                        HIS_ACCIDENT_VEHICLE data = accidentVehicles.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentVehicle.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentVehicle.Text = data.ACCIDENT_VEHICLE_CODE;
                            cboAccidentVehicle.Properties.Buttons[1].Visible = true;
                            chkUseAlcohol.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentResult.EditValue != null)
                    {
                        HIS_ACCIDENT_RESULT data = accidentResults.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentResult.EditValue.ToString()));
                        if (data != null)
                        {
                            txtAccidentResult.Text = data.ACCIDENT_RESULT_CODE;
                            cboAccidentResult.Properties.Buttons[1].Visible = true;
                            txtAccidentCare.Focus();
                            txtAccidentCare.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccidentCare.EditValue != null)
                    {
                        HIS_ACCIDENT_CARE data = accidentCares.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentCare.EditValue.ToString()));
                        if (data != null)
                        {
                            cboAccidentCare.Properties.Buttons[1].Visible = true;
                            txtAccidentCare.Text = data.ACCIDENT_CARE_CODE;
                            txtStatusIn.Focus();
                            txtStatusIn.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAccidentTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (dtAccidentTime.EditValue != null)
                    {
                        txtAccidentPoison.Focus();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentLocaltion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentLocation(strValue, cboAccidentLocaltion, txtAccidentLocaltion, txtAccidentBodyPart);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentLocaltion.Focus();
                    txtAccidentLocaltion.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentBodyPart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentBodyPart(strValue, cboAccidentBodyPart, txtAccidentBodyPart, chkAncoholtestResult_Over);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentHurtType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentHurtType(strValue, cboAccidentHurtType, txtAccidentHurtType, txtContent);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentPoison_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentPoison(strValue, cboAccidentPoison, txtAccidentPoison, txtHelmet);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHelmet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentHelmet(strValue, cboHelmet, txtHelmet, txtAccidentVehicle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentVehicle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentVehicle(strValue, cboAccidentVehicle, txtAccidentVehicle, txtAccidentResult);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentResult(strValue, cboAccidentResult, txtAccidentResult, txtAccidentCare);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtAccidentCare_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text.ToUpper();
                    LoadComboAccidentCare(strValue, cboAccidentCare, txtAccidentCare, txtStatusIn);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtAccidentTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(dtAccidentTime.Text))
                    {
                        try
                        {
                            if (e.KeyCode == Keys.Enter)
                            {
                                txtAccidentPoison.Focus();
                            }
                            if (e.KeyCode == Keys.Tab)
                            {
                                String enddatetime = Inventec.Common.TypeConvert.Parse.ToDateTime(dtAccidentTime.Text).ToString("yyyyMMddHHmm");
                                long endTime = Inventec.Common.TypeConvert.Parse.ToInt64(enddatetime + "00");
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        dtAccidentTime.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtContent.Focus();
                    txtContent.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentLocaltion_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentBodyPart.Focus();
                    txtAccidentBodyPart.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAncoholtestResult_Over.Focus();
                    chkAncoholtestResult_Over.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHelmet.Focus();
                    txtHelmet.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentVehicle.Focus();
                    txtAccidentVehicle.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkUseAlcohol.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkUseAlcohol_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentResult.Focus();
                    txtAccidentResult.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccidentCare.Focus();
                    txtAccidentCare.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtStatusIn.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentLocaltion_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentLocaltion.Properties.Buttons[1].Visible = false;
                    cboAccidentLocaltion.EditValue = null;
                    txtAccidentLocaltion.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentBodyPart_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentBodyPart.Properties.Buttons[1].Visible = false;
                    cboAccidentBodyPart.EditValue = null;
                    txtAccidentBodyPart.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentHurtType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentHurtType.Properties.Buttons[1].Visible = false;
                    cboAccidentHurtType.EditValue = null;
                    txtAccidentHurtType.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentPoison_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentPoison.Properties.Buttons[1].Visible = false;
                    cboAccidentPoison.EditValue = null;
                    txtAccidentPoison.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboHelmet_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboHelmet.Properties.Buttons[1].Visible = false;
                    cboHelmet.EditValue = null;
                    txtHelmet.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentVehicle_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentVehicle.Properties.Buttons[1].Visible = false;
                    cboAccidentVehicle.EditValue = null;
                    txtAccidentVehicle.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentResult_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentResult.Properties.Buttons[1].Visible = false;
                    cboAccidentResult.EditValue = null;
                    txtAccidentResult.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccidentCare_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboAccidentCare.Properties.Buttons[1].Visible = false;
                    cboAccidentCare.EditValue = null;
                    txtAccidentCare.Text = "";
                }

                HisAccidentHurtFilter filter = new HisAccidentHurtFilter();

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
                ValidationControl();
                this.positionHandle = -1;
                if (!dxValidationProvider.Validate())
                {
                    return;
                }
                if (!btnSave.Enabled) return;
                //cboHelmet_ButtonClick(null, null);
                WaitingManager.Show(this);
                accidentHurtSDO = new HisAccidentHurtSDO();
                //if (accdentHurtResult != null)
                //{
                //    accidentHurtSDO.Id = accdentHurtResult.ID;
                //}
                if (AccidentHurtData != null)
                {
                    accidentHurtSDO.Id = AccidentHurtData.ID;
                }


                //if (AccidentHurtData != null)
                //{
                //    Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_ACCIDENT_HURT, MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT>();
                //    accidentHurt = Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_ACCIDENT_HURT, MOS.EFMODEL.DataModels.HIS_ACCIDENT_HURT>(AccidentHurtData);
                //}
                accidentHurtSDO.AccidentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtAccidentTime.DateTime);

                if (cboAccidentLocaltion.EditValue != null)
                {
                    accidentHurtSDO.AccidentLocationId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentLocaltion.EditValue.ToString());
                }
                if (cboAccidentBodyPart.EditValue != null)
                {
                    accidentHurtSDO.AccidentBodyPartId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentBodyPart.EditValue.ToString());
                }

                accidentHurtSDO.AccidentHurtTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentHurtType.EditValue.ToString());

                if (cboAccidentPoison.EditValue != null)
                {
                    accidentHurtSDO.AccidentPoisonId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentPoison.EditValue.ToString());
                }

                if (cboHelmet.EditValue != null)
                {
                    accidentHurtSDO.AccidentHelmetId = Inventec.Common.TypeConvert.Parse.ToInt64(cboHelmet.EditValue.ToString());
                }
                if (cboAccidentVehicle.EditValue != null)
                {
                    accidentHurtSDO.AccidentVehicleId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentVehicle.EditValue.ToString());
                }
                if (chkUseAlcohol.Checked)
                {
                    accidentHurtSDO.IsUseAlcohol = true;
                }
                if (cboAccidentResult.EditValue != null)
                {
                    accidentHurtSDO.AccidentResultId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentResult.EditValue.ToString());
                }
                if (cboAccidentCare.EditValue != null)
                {
                    accidentHurtSDO.AccidentCareId = Inventec.Common.TypeConvert.Parse.ToInt64(cboAccidentCare.EditValue.ToString());
                }
                accidentHurtSDO.TreatmentId = treatmentId;
                accidentHurtSDO.Content = txtContent.Text;
                accidentHurtSDO.StatusIn = txtStatusIn.Text;
                accidentHurtSDO.StatusOut = txtStatusOut.Text;
                accidentHurtSDO.TreatmentInfo = txtTreatmentInfo.Text;

                if (chkAncoholtestResult_Enough.CheckState == CheckState.Checked)
                {
                    //accidentHurtSDO.AlcoholTestResult = 2;
                    accidentHurtSDO.AlcoholTestResult = true;
                }
                else if (chkAncoholtestResult_Over.CheckState == CheckState.Checked)
                {
                    accidentHurtSDO.AlcoholTestResult = false;
                }
                //else
                //{
                //    accidentHurtSDO.AlcoholTestResult = null;
                //}

                if (chkNarcoticsTestResult_Negative.CheckState == CheckState.Checked)
                {
                    accidentHurtSDO.NarcoticsTestResult = true;
                }
                else if (chkNarcoticsTestResult_Positve.CheckState == CheckState.Checked)
                {
                    accidentHurtSDO.NarcoticsTestResult = false;
                }
                //else
                //{
                //    accidentHurtSDO.NarcoticsTestResult = null;
                //}
                if (this.currentModule != null)
                {
                    accidentHurtSDO.ExecuteRoomId = this.currentModule.RoomId;
                }

                accidentHurtSDO.HospitalizationReason = memoGetInHospital.Text;

                if (txtCMNDNo.Text != "")
                {
                    if (txtCMNDNo.Text.Length == 9)
                    {
                        accidentHurtSDO.CmndNumber = txtCMNDNo.Text;
                        if (txtTimeProvided.Text != "")
                        {
                            dtTimeProvided.EditValue = ConvertBtnEditDateStringToSystemDate(txtTimeProvided.Text);
                            accidentHurtSDO.CmndDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeProvided.DateTime) ?? null;
                        }
                        accidentHurtSDO.CmndPlace = txtCMNDPlace.Text;
                    }
                    else
                    {
                        accidentHurtSDO.CccdNumber = txtCMNDNo.Text;
                        if (txtTimeProvided.Text != "")
                        {
                            dtTimeProvided.EditValue = ConvertBtnEditDateStringToSystemDate(txtTimeProvided.Text);
                            accidentHurtSDO.CccdDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeProvided.DateTime) ?? null;
                        }
                        accidentHurtSDO.CccdPlace = txtCMNDPlace.Text;
                    }
                }



                // Chính
                if (chkSua.Checked)
                {
                    this.accidentHurtSDO.AccidentHurtIcdName = txtCd.Text;
                }
                else
                {
                    this.accidentHurtSDO.AccidentHurtIcdName = cboCdChinh.Text;
                }
                this.accidentHurtSDO.AccidentHurtIcdCode = txtCdChinh.Text;
                
                // Phụ
                this.accidentHurtSDO.AccidentHurtIcdSubCode = txtCdPhu.Text;
                this.accidentHurtSDO.AccidentHurtIcdText = cboCdPhu.Text;

                //API
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("accidentHurtSDO____:" + Inventec.Common.Logging.LogUtil.GetMemberName(() => accidentHurtSDO), accidentHurtSDO));
                CommonParam param = new CommonParam();
                bool success = false;
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    //sửa dữ liệu
                    accdentHurtResult = new BackendAdapter(param).Post<HIS_ACCIDENT_HURT>(
                   "api/HisAccidentHurt/UpdateSdo",
                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                   accidentHurtSDO,
                   param);
                }
                else
                {
                    accdentHurtResult = new BackendAdapter(param).Post<HIS_ACCIDENT_HURT>(
                   "api/HisAccidentHurt/CreateSdo",
                   HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                   accidentHurtSDO,
                   param);
                }

                if (accdentHurtResult != null)
                {
                    success = true;
                    btnSave.Enabled = false;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                    btnPrint.Enabled = true;
                }

                if (dlg != null)
                {
                    dlg(accidentHurtSDO.TreatmentId);
                }
                //HIS_PATIENT hisPatientUpdate = new HIS_PATIENT();
                //CommonParam paramPatient = new CommonParam();
                //HisPatientFilter hisPatientFilter = new HisPatientFilter();
                //hisPatientFilter.ID = treatment.PATIENT_ID;
                //var lstHisPatientUpdate = new BackendAdapter(paramPatient).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, hisPatientFilter, paramPatient);
                //if (lstHisPatientUpdate != null && lstHisPatientUpdate.Count > 0)
                //{
                //    hisPatientUpdate = lstHisPatientUpdate.FirstOrDefault();
                //    hisPatientUpdate.CMND_NUMBER = txtCMNDNo.Text;
                //    if (txtTimeProvided.Text != "")
                //    {
                //        dtTimeProvided.EditValue = ConvertBtnEditDateStringToSystemDate(txtTimeProvided.Text);
                //        hisPatientUpdate.CMND_DATE = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeProvided.DateTime) ?? null;
                //    }
                //    hisPatientUpdate.CMND_PLACE = txtCMNDPlace.Text;
                //}

                //var patients = new BackendAdapter(paramPatient).Post<HIS_PATIENT>("api/HisPatient/Update", ApiConsumer.ApiConsumers.MosConsumer, hisPatientUpdate, paramPatient);
                //if (patients == null)
                //    success = false;

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion
                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(MPS.Processor.Mps000284.PDO.Mps000284PDO.printTypeCode, ProcessPrint);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessPrint(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                decimal ratio = 0;
                //if (this.accdentHurtResult == null)
                //    return result;

                CommonParam param = new CommonParam();
                V_HIS_ACCIDENT_HURT accidentHurt = null;
                long id = (this.accdentHurtResult != null && this.accdentHurtResult.ID > 0) ? this.accdentHurtResult.ID : ((this.AccidentHurtData != null && this.AccidentHurtData.ID > 0) ? this.AccidentHurtData.ID : 0);
                MOS.Filter.HisAccidentHurtViewFilter accidentFilter = new HisAccidentHurtViewFilter();
                accidentFilter.ID = id;
                var accidentHurts = new BackendAdapter(param).Get<List<V_HIS_ACCIDENT_HURT>>("api/HisAccidentHurt/GetView", ApiConsumers.MosConsumer, accidentFilter, param);
                if (accidentHurts != null && accidentHurts.Count() > 0)
                {
                    accidentHurt = accidentHurts.FirstOrDefault();
                }

                MOS.Filter.HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                patientFilter.ID = treatment.PATIENT_ID;
                var patients = new BackendAdapter(param).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);

                //lấy dữ liệu mới nhất
                MOS.Filter.HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                treatmentFilter.ID = treatment.ID;
                var treat = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);

                MPS.Processor.Mps000284.PDO.Mps000284PDO rdo = new MPS.Processor.Mps000284.PDO.Mps000284PDO(patients.FirstOrDefault(), accidentHurt, treat.FirstOrDefault());

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.AccidentHurtData != null ? this.AccidentHurtData.TREATMENT_CODE : (treatment != null ? treatment.TREATMENT_CODE : "")), printTypeCode, this.currentModule.RoomId);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });

                //result = MPS.MpsPrinter.Run(printData);

                if (result)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnPrint_Click(null, null);
        }

        private void chkAncoholtestResult_Over_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAncoholtestResult_Over.CheckState == CheckState.Checked)
                {
                    chkAncoholtestResult_Enough.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAncoholtestResult_Enough_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkAncoholtestResult_Enough.CheckState == CheckState.Checked)
                {
                    chkAncoholtestResult_Over.CheckState = CheckState.Unchecked;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNarcoticsTestResult_Positve_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNarcoticsTestResult_Positve.CheckState == CheckState.Checked)
                {
                    chkNarcoticsTestResult_Negative.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNarcoticsTestResult_Negative_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNarcoticsTestResult_Negative.CheckState == CheckState.Checked)
                {
                    chkNarcoticsTestResult_Positve.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAncoholtestResult_Over_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkAncoholtestResult_Enough.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAncoholtestResult_Enough_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNarcoticsTestResult_Positve.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNarcoticsTestResult_Positve_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkNarcoticsTestResult_Negative.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNarcoticsTestResult_Negative_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtAccidentTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnContentSubclinical_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ContentSubclinical").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.ContentSubclinical");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.treatmentId);
                    listArgs.Add((HIS.Desktop.Common.DelegateSelectData)DelegateSelectDataContentSubclinical);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DelegateSelectDataContentSubclinical(object data)
        {
            try
            {
                if (data != null && data is String)
                {
                    txtStatusIn.Text = txtStatusIn.Text + "\r\n" + data.ToString();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDNo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTimeProvided.Focus();
                    txtTimeProvided.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dateCMNDDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCMNDPlace.Focus();
                    txtCMNDPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMNDPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    memoGetInHospital.Focus();
                    memoGetInHospital.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeProvided_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    this.dtTimeProvided.Visible = false;
                    this.dtTimeProvided.Update();
                    // this.txtTimeProvided.Text = this.dtTimeProvided.DateTime.ToString("dd/MM/yyyy");
                    this.txtTimeProvided.Text = this.dtTimeProvided.DateTime.ToString();

                    this.txtCMNDPlace.Focus();
                    this.txtCMNDPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeProvided_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.dtTimeProvided.EditValue != null && this.dtTimeProvided.DateTime != DateTime.MinValue)
                {
                    dtTimeProvided.Visible = false;

                    this.txtTimeProvided.Text = this.dtTimeProvided.DateTime.ToString();
                }
                else
                {
                    dtTimeProvided.Visible = false;
                    this.txtTimeProvided.Text = "";
                }
                if (this.dtTimeProvided.EditValue == null || this.dtTimeProvided.DateTime == DateTime.MinValue)
                {
                    dtTimeProvided.Visible = false;
                    this.txtTimeProvided.Text = "";
                }
                if (this.dtTimeProvided.DateTime < DateTime.Now.AddDays(-100000))
                {
                    dtTimeProvided.Visible = false;
                    this.txtTimeProvided.Text = "";
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtTimeProvided_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dtTimeProvided.Visible = false;
                    this.dtTimeProvided.Update();
                    this.txtTimeProvided.Text = this.dtTimeProvided.DateTime.ToString();
                    //SendKeys.Send("{TAB}");
                    this.txtCMNDPlace.Focus();
                    this.txtCMNDPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTimeProvided_Click(object sender, EventArgs e)
        {

        }

        private void txtTimeProvided_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    //SendKeys.Send("{TAB}");
                    this.txtCMNDPlace.Focus();
                    this.txtCMNDPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTimeProvided_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    this.txtTimeProvided.Text = "";

                    this.dtTimeProvided.Visible = false;
                }

                if (e.Button.Kind == ButtonPredefines.Down)
                {
                    DateTime? dt = ConvertBtnEditDateStringToSystemDate(this.txtTimeProvided.Text);
                    if (dt != null && dt.Value != DateTime.MinValue)
                    {
                        this.dtTimeProvided.EditValue = dt;
                        this.dtTimeProvided.Update();
                    }

                    this.dtTimeProvided.Visible = true;
                    this.dtTimeProvided.Focus();
                    this.dtTimeProvided.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private DateTime? ConvertBtnEditDateStringToSystemDate(string dateTime)
        {
            DateTime? result = null;
            try
            {
                if (!String.IsNullOrEmpty(dateTime))
                {
                    dateTime = dateTime.Replace(" ", "");
                    int day = Int16.Parse(dateTime.Substring(0, 2));
                    int month = Int16.Parse(dateTime.Substring(3, 2));
                    int year = Int16.Parse(dateTime.Substring(6, 4));
                    result = new DateTime(year, month, day);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }

            return result;
        }

        #region Chuẩn đoán chính phụ
        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.Width = 650;
                ado.Height = 24;
                ado.LblIcdMain = "CĐ chính:";
                ado.IsColor = true;
                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList();
                ado.AutoCheckIcd = true;
                ucIcd = (UserControl)icdProcessor.Run(ado);
                if (ucIcd != null)
                {
                    //this.panelControlIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), BackendDataWorker.Get<HIS_ICD>().OrderBy(o => o.ICD_CODE).ToList());
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.Width = 650;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phụ:";
                ado.TextNullValue = "Nhấn F1 để nhập bệnh phụ";
                ado.TextSize = 95;
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    // this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextForcusOut()
        {
            try
            {
                // icdProcessor.FocusControl(ucDate);
                cboAccidentHurtType.Focus();
                cboAccidentHurtType.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Chuẩn đoán chính phụ -Ver2

        private void DataToComboChuanDoanTD(GridLookUpEdit cbo, List<HIS_ICD> data)
        {
            try
            {
                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("ICD_CODE", "", 150, 1));
                //columnInfos.Add(new ColumnInfo("ICD_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("ICD_NAME", "ID", columnInfos, false, 250);
                //ControlEditorLoader.Load(cbo, dataIcds, controlEditorADO);

                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = "ICD_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new System.Drawing.Size(300, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cbo.Properties.View.Columns.AddField("ICD_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cbo.Properties.View.Columns.AddField("ICD_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 100;

                DevExpress.XtraGrid.Columns.GridColumn aColumnNameUnsign = cbo.Properties.View.Columns.AddField("ICD_NAME_UNSIGN");
                aColumnNameUnsign.Visible = true;
                aColumnNameUnsign.VisibleIndex = -1;
                aColumnNameUnsign.Width = 100;

                cbo.Properties.View.Columns["ICD_NAME_UNSIGN"].Width = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboCdPhu_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F1)
                {

                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.SecondaryIcd").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.SecondaryIcd'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.SecondaryIcd' is not plugins");
                    HIS.Desktop.ADO.SecondaryIcdADO secondaryIcdADO = new HIS.Desktop.ADO.SecondaryIcdADO(GetStringIcds, txtCdPhu.Text, cboCdPhu.Text);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(secondaryIcdADO);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null"); WaitingManager.Hide();
                    ((Form)extenceInstance).Show(this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GetStringIcds(string delegateIcdCodes, string delegateIcdNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(delegateIcdNames))
                {
                    cboCdPhu.Text = delegateIcdNames;
                }
                if (!string.IsNullOrEmpty(delegateIcdCodes))
                {
                    txtCdPhu.Text = delegateIcdCodes;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCdPhu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAccidentHurtType.SelectAll();
                    cboAccidentHurtType.Focus();
                    cboAccidentHurtType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCdPhu_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string seperate = ";";
                    string strIcdNames = "";
                    string strWrongIcdCodes = "";
                    string[] periodSeparators = new string[1];
                    periodSeparators[0] = seperate;
                    string[] arrIcdExtraCodes = txtCdPhu.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                    if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                    {
                        var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                        foreach (var itemCode in arrIcdExtraCodes)
                        {
                            var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                            if (icdByCode != null && icdByCode.ID > 0)
                            {
                                strIcdNames += (seperate + icdByCode.ICD_NAME);
                            }
                            else
                            {
                                strWrongIcdCodes += (seperate + itemCode);
                            }
                        }
                        strIcdNames += seperate;
                        if (!String.IsNullOrEmpty(strWrongIcdCodes))
                        {
                            MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                        }
                    }
                    cboCdPhu.Text = strIcdNames;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboCdPhu_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                string seperate = ";";
                string strIcdNames = "";
                string strWrongIcdCodes = "";
                string[] periodSeparators = new string[1];
                periodSeparators[0] = seperate;
                string[] arrIcdExtraCodes = txtCdPhu.Text.Split(periodSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (arrIcdExtraCodes != null && arrIcdExtraCodes.Count() > 0)
                {
                    var icdAlls = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                    foreach (var itemCode in arrIcdExtraCodes)
                    {
                        var icdByCode = icdAlls.FirstOrDefault(o => o.ICD_CODE.ToLower() == itemCode.ToLower());
                        if (icdByCode != null && icdByCode.ID > 0)
                        {
                            strIcdNames += (seperate + icdByCode.ICD_NAME);
                        }
                        else
                        {
                            strWrongIcdCodes += (seperate + itemCode);
                        }
                    }
                    strIcdNames += seperate;
                    if (!String.IsNullOrEmpty(strWrongIcdCodes))
                    {
                        MessageManager.Show(String.Format("Không tìm thấy icd tương ứng với các mã sau: {0}", strWrongIcdCodes));
                    }
                }
                cboCdPhu.Text = strIcdNames;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCdChinh_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.Delete)
            {
                //if (!cboCdChinh.Properties.Buttons[1].Visible)
                //    return;
                //this._TextIcdName = "";
                cboCdChinh.EditValue = null;
                txtCdChinh.Text = "";

                //txtIcdMainText.Text = "";
                cboCdChinh.Properties.Buttons[1].Visible = false;
            }
        }



        private void ValidationtxtCdChinh()
        {
            try
            {
                CboIcds icdMainRule = new CboIcds();
                icdMainRule.txtTextEdit = txtCdChinh;
                icdMainRule.txtTextEditPhu = txtCdPhu;
                icdMainRule.cbo = cboCdChinh;
             

                icdMainRule.ErrorType = ErrorType.Warning;
           
                this.dxValidationProvider.SetValidationRule(txtCdPhu, icdMainRule);


                ComboWithTextEditValidationRule icdMainRule_ = new ComboWithTextEditValidationRule();
                icdMainRule_.txtTextEdit = txtCdChinh;
                icdMainRule_.cbo = cboCdChinh;
                icdMainRule_.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                icdMainRule_.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtCdChinh, icdMainRule_);
               
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

       

        private void chkSua_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkSua.Checked == true)
                {
                    cboCdChinh.Properties.Buttons[1].Visible = false;
                    cboCdChinh.Properties.Buttons[0].Visible = false;
                    cboCdChinh.SelectAll();
                    cboCdChinh.Visible = false;
                    txtCd.Visible = true;
                    txtCd.Text = this._TextIcdName;
                    txtCd.Focus();
                    txtCd.SelectAll();

                }
                else
                {
                    txtCd.Visible = false;
                    cboCdChinh.Visible = true;
                    txtCd.Text = cboCdChinh.Text;
                    cboCdChinh.Properties.Buttons[1].Visible = true;
                    cboCdChinh.Properties.Buttons[0].Visible = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ChangecboChanDoanTD()
        {
            try
            {
                _TextIcdName = cboCdChinh.Text;
                txtCdChinh.ErrorText = "";


                MOS.EFMODEL.DataModels.HIS_ICD icd = currentIcds.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboCdChinh.EditValue ?? 0).ToString()));
                if (icd != null)
                {
                    cboCdChinh.Properties.Buttons[1].Visible = true;
                    txtCdChinh.Text = icd.ICD_CODE;
                    chkSua.Checked = true;
                }
                else
                {

                    cboCdChinh.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCdChinh_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control & e.KeyCode == Keys.A)
                {
                    cboCdChinh.ClosePopup();
                    cboCdChinh.SelectAll();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    cboCdChinh.ClosePopup();
                    if (cboCdChinh.EditValue != null)
                        this.ChangecboChanDoanTD();
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCdChinh_Leave(object sender, EventArgs e)
        {
            try
            {
                chkSua.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboCdChinh_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
               
                this.ChangecboChanDoanTD();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void LoadIcdToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    if (icd != null)
                    {

                        txtCdChinh.Text = icd.ICD_CODE;
                        cboCdChinh.EditValue = icd.ID;
                        txtCd.Text = icdName;   
                        if ((!String.IsNullOrEmpty(icdName) && (icdName ?? "").Trim().ToLower() != (icd.ICD_NAME ?? "").Trim().ToLower()))
                        {
                            chkSua.Checked = true;

                        }
                        else
                        {
                            chkSua.Checked = false;

                        }
                    }
                    else
                    {
                        txtCdChinh.Text = null;
                        cboCdChinh.EditValue = null;
                        chkSua.Checked = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdCauseToControl(string icdCode, string icdName)
        {
            try
            {
                if (!string.IsNullOrEmpty(icdCode))
                {
                    //icdCode.Split(';')
                    // var icd = this.currentIcds.Where(p => p.ICD_CODE == (icdCode)).FirstOrDefault();
                    //if (icd != null)
                    //{
                    txtCdPhu.Text = icdCode;
                    cboCdPhu.Text = icdName;

                    //  }
                    //  else
                    // {
                    //  txtCdPhu.Text = null;
                    //   cboCdPhu.EditValue = null;

                    // }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
