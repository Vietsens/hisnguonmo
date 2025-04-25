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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using EMR.EFMODEL.DataModels;
using EMR.Filter;
using EMR.SDO;
using EMR.TDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.SignLibrary;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.RecordChecking
{
    public partial class FormHisTreatmentRecordChecking : FormBase
    {
        #region Declare
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? treatmentId;
        private List<long> listTreatmentId;
        private List<V_HIS_TREATMENT> ListTreatment;
        private List<EmrDocumentTypeADO> ListDocumentType;
        private EmrDocumentTypeADO CurrentType;
        private List<V_EMR_DOCUMENT> ListDocument;
        private HisTreatmentForRecordCheckingSDO CurrentTreatment;
        private List<InfoRecordADO> ListDataInfoRecord;
        private List<InfoRecordADO> CurrentDataInfoRecord = new List<InfoRecordADO>();
        private List<InfoRecordADO> CurrentInfoRecord = new List<InfoRecordADO>();
        private List<long> ListTypeId = new List<long>()
        {
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__CARE,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE,
            IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRANSFUSION,
        };

        private List<long> ReqTypeId = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT
        };

        private int lastRowHandle = -1;
        private DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        private string lastGrid = "";
        private DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        public static HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        public static List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool IsLoadFirstForm = true;
        #endregion

        public FormHisTreatmentRecordChecking()
            : base()
        {
            InitializeComponent();
        }

        public FormHisTreatmentRecordChecking(Inventec.Desktop.Common.Modules.Module moduleData, long? treatmentId)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                SetIcon();
                lciGC_Treatment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                //
                this.moduleData = moduleData;
                this.treatmentId = treatmentId;
                this.Text = moduleData.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public FormHisTreatmentRecordChecking(Inventec.Desktop.Common.Modules.Module moduleData, long? treatmentId, List<long> listTreatmentId)
            : this(moduleData, treatmentId)
        {
            try
            {
                this.listTreatmentId = listTreatmentId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void FormHisTreatmentRecordChecking_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show(); 
                GetControlAcs();
                //SetCaptionByLanguageKey();
                if (this.listTreatmentId != null)
                {
                    lciGC_Treatment.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    FillDataToGridTreatment(this.listTreatmentId);
                }
                InitGridEmrDocumentType();
                SetDefaultValueControl();
                ProcessCaptionGridInfoRecord();
                InitControlState();
                FillDataToGrid();
                SetDefaultProperties();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultProperties()
        {
            try
            {
                //var screenWidth = Screen.PrimaryScreen.Bounds.Width;
                //if (screenWidth >= 1600)
                //{
                //    Gv_Treatment.OptionsView.ColumnAutoWidth = true;
                //}
                SetCustomSizeForGridView(ref Gv_Treatment);
                SetCustomSizeForGridView(ref Gv_EmrDocument);
                SetCustomSizeForGridView(ref Gv_InfoRecord);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment(List<long> listId)
        {
            try
            {
                if (listId != null && listId.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.IDs = listId;
                    this.ListTreatment = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                }
                else
                {
                    this.ListTreatment = null;
                }
                Gc_Treatment.BeginUpdate();
                Gc_Treatment.DataSource = this.ListTreatment;
                Gc_Treatment.EndUpdate();
                if (this.ListTreatment != null && this.ListTreatment.Count == 1)
                {
                    this.hasSelectedTreatment = true;
                    Gv_Treatment.FocusedRowHandle = 0;
                    TxtTreatmentCode.Text = this.ListTreatment.First().TREATMENT_CODE;
                    FillDataToGrid();
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
                IsLoadFirstForm = true;
                controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.HisTreatmentRecordChecking");
                if (currentControlStateRDO != null && currentControlStateRDO.Count > 0)
                {
                    foreach (var item in currentControlStateRDO)
                    {
                        if (item.KEY == chkUuTien.Name)
                        {
                            chkUuTien.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkIncludeCancelDoc.Name)
                        {
                            chkIncludeCancelDoc.Checked = item.VALUE == "1";
                        }
                    }
                }
                IsLoadFirstForm = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataTreatment()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                HisTreatmentForRecordCheckingFilter filter = new HisTreatmentForRecordCheckingFilter();
                if (!String.IsNullOrWhiteSpace(TxtTreatmentCode.Text))
                {
                    string code = TxtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        TxtTreatmentCode.Text = code;
                    }

                    filter.TREATMENT_CODE__EXACT = TxtTreatmentCode.Text;
                }
                else if (treatmentId.HasValue)
                {
                    filter.TREATMENT_ID = treatmentId;
                }
                else
                {
                    filter.TREATMENT_ID = -1;
                }

                CurrentTreatment = new BackendAdapter(paramCommon).Get<HisTreatmentForRecordCheckingSDO>("api/HisTreatment/GetInfoForRecordChecking", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, paramCommon);
                if (CurrentTreatment != null)
                {
                    TxtTreatmentCode.Text = CurrentTreatment.Treatment.TREATMENT_CODE;

                    FillDataToControl(CurrentTreatment.Treatment);
                    ProcessDataADO();
                }
                else
                {
                    SetDefaultValueControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataADO()
        {
            try
            {
                if (CurrentTreatment != null)
                {
                    if (CurrentTreatment.Cares != null && CurrentTreatment.Cares.Count > 0)
                    {
                        foreach (var care in CurrentTreatment.Cares)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__CARE;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(care.CREATE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_CARE:" + care.ID;
                            ado.TYPE = "";
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == care.EXECUTE_DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = care.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Debates != null && CurrentTreatment.Debates.Count > 0)
                    {
                        foreach (var debate in CurrentTreatment.Debates)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(debate.DEBATE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_DEBATE:" + debate.ID;
                            ado.TYPE = "";
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == debate.DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = debate.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Infusions != null && CurrentTreatment.Infusions.Count > 0)
                    {
                        foreach (var infusions in CurrentTreatment.Infusions)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = string.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(infusions.START_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToTimeString(infusions.FINISH_TIME ?? 0));
                            ado.SEARCH_CODE = "HIS_INFUSION:" + infusions.ID;
                            ado.TYPE = "";
                            ado.DEPARTMENT_NAME = infusions.MEDICINE_TYPE_NAME;
                            ado.CREATOR = infusions.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.MediReacts != null && CurrentTreatment.MediReacts.Count > 0)
                    {
                        List<long> medicineId = CurrentTreatment.MediReacts.Select(s => s.MEDICINE_ID.Value).Distinct().ToList();
                        List<V_HIS_MEDICINE> listMedicine = GetMedicineById(medicineId);
                        foreach (var mediReact in CurrentTreatment.MediReacts)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(mediReact.EXECUTE_TIME ?? 0);
                            ado.SEARCH_CODE = "HIS_MEDI_REACT:" + mediReact.ID;
                            ado.TYPE = "";
                            var medicine = listMedicine.FirstOrDefault(o => o.ID == mediReact.MEDICINE_ID);
                            if (medicine != null)
                            {
                                ado.DEPARTMENT_NAME = medicine.MEDICINE_TYPE_NAME;
                            }
                            ado.CREATOR = mediReact.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.ServiceReqs != null && CurrentTreatment.ServiceReqs.Count > 0)
                    {
                        foreach (var req in CurrentTreatment.ServiceReqs)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            if (ReqTypeId.Contains(req.SERVICE_REQ_TYPE_ID))
                            {
                                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION;
                            }
                            else
                            {
                                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN;
                            }

                            ado.CODE = req.SERVICE_REQ_CODE;
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(req.INTRUCTION_TIME);
                            ado.SEARCH_CODE = "SERVICE_REQ_CODE:" + req.SERVICE_REQ_CODE;

                            ado.TYPE = "";
                            var type = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().FirstOrDefault(o => o.ID == req.SERVICE_REQ_TYPE_ID);
                            if (type != null)
                            {
                                ado.TYPE = type.SERVICE_REQ_TYPE_NAME;
                            }

                            ado.DEPARTMENT_NAME = "";
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == req.REQUEST_DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = req.REQUEST_LOGINNAME;
                            ado.REQ_TYPE_STT_ID = req.SERVICE_REQ_STT_ID;

                            ListDataInfoRecord.Add(ado);

                            if (ado.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN)
                            {
                                InfoRecordADO ado1 = new InfoRecordADO();
                                Inventec.Common.Mapper.DataObjectMapper.Map<InfoRecordADO>(ado1, ado);
                                ado1.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                                ListDataInfoRecord.Add(ado1);
                            }
                        }
                    }

                    if (CurrentTreatment.Trackings != null && CurrentTreatment.Trackings.Count > 0)
                    {
                        foreach (var tracking in CurrentTreatment.Trackings)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRACKING;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tracking.TRACKING_TIME);
                            ado.SEARCH_CODE = "HIS_TRACKING:" + tracking.ID;
                            ado.TYPE = "";
                            var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == tracking.DEPARTMENT_ID);
                            if (department != null)
                            {
                                ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            }
                            ado.CREATOR = tracking.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }

                    if (CurrentTreatment.Transfusions != null && CurrentTreatment.Transfusions.Count > 0)
                    {
                        foreach (var tranfusion in CurrentTreatment.Transfusions)
                        {
                            InfoRecordADO ado = new InfoRecordADO();
                            ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__TRANSFUSION;
                            ado.CODE = "";
                            ado.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(tranfusion.MEASURE_TIME);
                            ado.SEARCH_CODE = "HIS_TRANSFUSION:" + tranfusion.ID;
                            ado.TYPE = "";
                            //var department = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == tranfusion.DEPARTMENT_ID);
                            //if (department != null)
                            //{
                            //    ado.DEPARTMENT_NAME = department.DEPARTMENT_NAME;
                            //}
                            ado.CREATOR = tranfusion.CREATOR;
                            ListDataInfoRecord.Add(ado);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_MEDICINE> GetMedicineById(List<long> medicineId)
        {
            List<V_HIS_MEDICINE> result = new List<V_HIS_MEDICINE>();
            try
            {
                if (medicineId != null && medicineId.Count > 0)
                {
                    int skip = 0;
                    while (medicineId.Count - skip > 0)
                    {
                        var listId = medicineId.Skip(skip).Take(500).ToList();
                        skip += 500;

                        CommonParam param = new CommonParam();
                        HisMedicineViewFilter filter = new HisMedicineViewFilter();
                        filter.IDs = listId;
                        var apiResult = new BackendAdapter(param).Get<List<V_HIS_MEDICINE>>("api/HisMedicine/GetView", ApiConsumers.MosConsumer, filter, SessionManager.ActionLostToken, param);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            result.AddRange(apiResult);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = new List<V_HIS_MEDICINE>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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

        private void FillDataToControl(HIS_TREATMENT treatment)
        {
            try
            {
                if (treatment != null)
                {
                    treatmentId = treatment.ID;
                    LblPatientCode.Text = treatment.TDL_PATIENT_CODE;
                    LblPatientName.Text = treatment.TDL_PATIENT_NAME;

                    LblGender.Text = treatment.TDL_PATIENT_GENDER_NAME;
                    LblHeinNumber.Text = treatment.TDL_HEIN_CARD_NUMBER;
                    LblMediOrg.Text = string.Format("{0} - {1}", treatment.TDL_HEIN_MEDI_ORG_CODE, treatment.TDL_HEIN_MEDI_ORG_NAME);
                    LblHeinTime.Text = string.Format("{0} - {1}", Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_HEIN_CARD_FROM_TIME ?? 0), Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_HEIN_CARD_TO_TIME ?? 0));
                    LblAddress.Text = treatment.TDL_PATIENT_ADDRESS;
                    LblMainIcd.Text = string.Format("{0} - {1}", treatment.ICD_CODE, treatment.ICD_NAME);
                    LblSubIcd.Text = string.Format("{0} - {1}", treatment.ICD_SUB_CODE, treatment.ICD_TEXT);
                    LblNote.Text = treatment.APPROVE_FINISH_NOTE;
                    LblIcdYhct.Text = string.Format("{0} - {1}", treatment.TRADITIONAL_ICD_CODE, treatment.TRADITIONAL_ICD_NAME);
                    LblSubIcdYhct.Text = string.Format("{0} - {1}", treatment.TRADITIONAL_ICD_SUB_CODE, treatment.TRADITIONAL_ICD_TEXT);
                    if (treatment.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                    {
                        LblDob.Text = treatment.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                    }
                    else
                    {
                        LblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatment.TDL_PATIENT_DOB);
                    }

                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == treatment.TDL_PATIENT_TYPE_ID);
                    if (patientType != null)
                    {
                        LblPatientType.Text = patientType.PATIENT_TYPE_NAME;
                    }
                    else
                    {
                        LblPatientType.Text = "";
                    }

                    if (treatment.APPROVAL_STORE_STT_ID == null || treatment.APPROVAL_STORE_STT_ID == 1)
                    {
                        btnKhongDat.Enabled = true;
                    }

                    if (treatment.APPROVAL_STORE_STT_ID == null || treatment.APPROVAL_STORE_STT_ID == 2)
                    {
                        btnDat.Enabled = true;
                    }

                    if (treatment.APPROVAL_STORE_STT_ID != null)
                    {
                        btnHuyDuyet.Enabled = true;
                    }
                    lblStatus.Text = treatment.APPROVAL_STORE_STT_ID == null ? "Chưa soát" : ((treatment.APPROVAL_STORE_STT_ID != null && treatment.APPROVAL_STORE_STT_ID == 1) ? "Đã tra soát (Đạt)" : "Đã tra soát (Không đạt)");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridEmrDocumentType()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentTypeFilter filter = new EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                var dt = new BackendAdapter(paramCommon).Get<List<EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);
                ListDocumentType = new List<EmrDocumentTypeADO>();
                if (dt != null && dt.Count > 0)
                {
                    dt = dt.OrderByDescending(o => ListTypeId.Contains(o.ID)).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList();

                    foreach (var item in dt)
                    {
                        EmrDocumentTypeADO ado = new EmrDocumentTypeADO(item);

                        ListDocumentType.Add(ado);
                    }
                }

                GcEmrDocumentType.BeginUpdate();
                GcEmrDocumentType.DataSource = ListDocumentType;
                GcEmrDocumentType.EndUpdate();
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
                LblPatientCode.Text = "";
                LblPatientName.Text = "";
                LblDob.Text = "";
                LblGender.Text = "";
                LblPatientType.Text = "";
                LblHeinNumber.Text = "";
                LblHeinTime.Text = "";
                LblMediOrg.Text = "";
                LblAddress.Text = "";
                LblMainIcd.Text = "";
                LblSubIcd.Text = "";
                LblIcdYhct.Text = "";
                LblSubIcdYhct.Text = "";
                LblNote.Text = "";
                Gc_InfoRecord.DataSource = null;
                Gc_EmrDocument.DataSource = null;
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EmrDocument()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                EmrDocumentViewFilter filter = new EmrDocumentViewFilter();
                filter.TREATMENT_CODE__EXACT = TxtTreatmentCode.Text;
                if (!chkIncludeCancelDoc.Checked)
                    filter.IS_DELETE = false;
                ListDocument = new BackendAdapter(paramCommon).Get<List<V_EMR_DOCUMENT>>("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, paramCommon);


                if (ListDocument == null)
                {
                    ListDocument = new List<V_EMR_DOCUMENT>();
                }

                if (ListDocument.Exists(o => !o.DOCUMENT_TYPE_ID.HasValue))
                {
                    if (!ListDocumentType.Exists(o => o.ID == 0))
                    {
                        //thêm dòng loại văn bản chưa xác định
                        EMR_DOCUMENT_TYPE typeOther = new EMR_DOCUMENT_TYPE();
                        typeOther.DOCUMENT_TYPE_NAME = "Chưa xác định";
                        EmrDocumentTypeADO ado = new EmrDocumentTypeADO(typeOther);
                        ListDocumentType.Add(ado);
                    }
                }
                else
                {

                    //xóa dòng loại văn bản Chưa xác định
                    var other = ListDocumentType.Where(o => o.ID == 0).ToList();
                    if (other != null)
                    {
                        foreach (var item in other)
                        {
                            ListDocumentType.Remove(item);
                        }
                    }

                }
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
                if (String.IsNullOrWhiteSpace(TxtTreatmentCode.Text) && !treatmentId.HasValue) return;
                if (CurrentType == null && ListDocumentType == null) return;

                WaitingManager.Show();
                if (CurrentType == null && ListDocumentType.Count > 0)
                {
                    CurrentType = ListDocumentType.First();
                }

                ListDataInfoRecord = new List<InfoRecordADO>();
                CurrentDataInfoRecord = new List<InfoRecordADO>();

                GetDataTreatment();
                EmrDocument();
                List<EmrDocumentTypeADO> ListDocumentTypeTemp = new List<EmrDocumentTypeADO>();
                foreach (var item in ListDocumentType)
                {
                    EmrDocumentTypeADO ado = new EmrDocumentTypeADO(item);
                    var data = (ListDocument != null && ListDocument.Count > 0) ? ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == item.ID).ToList() : null;
                    ado.IsHasDocument = (data != null && data.Count > 0);
                    if (item.PATIENT_MUST_SIGN == 1 && ado.IsHasDocument)
                    {
                        var dataNoPatientSign = data.FirstOrDefault(o => String.IsNullOrEmpty(o.SIGNERS) || !o.SIGNERS.Contains("#@!@#" + o.PATIENT_CODE));
                        if (dataNoPatientSign != null)
                            ado.IsHasDocumentNoPatientSign = true;
                    }
                    ListDocumentTypeTemp.Add(ado);
                }
                ListDocumentType = ListDocumentTypeTemp;
                OrderListByCheckBox();
                ProcessFillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFillDataToGrid()
        {
            try
            {
                Gc_InfoRecord.DataSource = null;
                Gc_EmrDocument.DataSource = null;
                if (CurrentType == null) return;

                if (ListTypeId.Contains(CurrentType.ID))
                {
                    LciTotalInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    ProcessDataGridInfoRecord();
                }
                else
                {
                    LciTotalInfo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    ProcessDataGridDocument();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridInfoRecord()
        {
            try
            {
                if (ListDocument != null && CurrentType != null)
                {
                    //if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT)
                    //{
                    //    CurrentInfoRecord = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN).ToList();
                    //}
                    //else
                    {

                        //var ListServiceAssign = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN).ToList();

                        //if (ListServiceAssign != null && ListServiceAssign.Count > 0)
                        //{
                        //    if (ListDocument != null && ListDocument.Count > 0)
                        //    {
                        //        foreach (var item in ListServiceAssign)
                        //        {
                        //            var docs = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT && !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(item.SEARCH_CODE)).ToList();
                        //            if (docs != null && docs.Count > 0)
                        //            {
                        //                InfoRecordADO ado = new InfoRecordADO();

                        //                ado.DOCUMENT_TYPE_ID = IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT;
                        //                ado.CODE = item.CODE;
                        //                ado.TYPE = item.TYPE;
                        //                ado.CREATE_TIME_STR = item.CREATE_TIME_STR;
                        //                ado.DEPARTMENT_NAME = item.DEPARTMENT_NAME;
                        //                ado.SEARCH_CODE = item.SEARCH_CODE;
                        //                ado.REQ_TYPE_STT_ID = item.REQ_TYPE_STT_ID;
                        //                ado.CREATOR = item.CREATOR;

                        //                ListDataInfoRecord.Add(ado);
                        //            }
                        //        }
                        //    }
                        //}

                        CurrentInfoRecord = ListDataInfoRecord.Where(o => o.DOCUMENT_TYPE_ID == CurrentType.ID).ToList();
                    }


                    var documents = ListDocument.Where(o => o.DOCUMENT_TYPE_ID == CurrentType.ID).ToList();
                    if (documents != null && documents.Count > 0)
                    {
                        InfoRecordADO ado = new InfoRecordADO();
                        ado.TYPE = "Khác";
                        ado.DOCUMENT_TYPE_ID = CurrentType.ID;

                        var docs = GetDocumentByInfoRecod(ado);
                        if (docs != null && docs.Count > 0)
                        {
                            CurrentInfoRecord.Add(ado);
                        }
                    }

                    if (chkToiTao.Checked && CurrentInfoRecord != null)
                    {
                        CurrentInfoRecord = CurrentInfoRecord.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
                    }

                    Gc_InfoRecord.BeginUpdate();
                    Gc_InfoRecord.DataSource = CurrentInfoRecord;
                    Gc_InfoRecord.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataGridDocument()
        {
            try
            {
                List<V_EMR_DOCUMENT> documents = new List<V_EMR_DOCUMENT>();
                if (ListDocument != null && CurrentType != null)
                {
                    documents = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == CurrentType.ID).ToList();
                    if (ListTypeId.Contains(CurrentType.ID))
                    {
                        var record = (InfoRecordADO)Gv_InfoRecord.GetFocusedRow();
                        if (LciTotalInfo.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && record != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record));
                            documents = GetDocumentByInfoRecod(record, CurrentType.ID);
                        }
                    }
                }
                if (chkToiTao.Checked && documents != null)
                {
                    documents = documents.Where(o => o.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
                }

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => CurrentType), CurrentType)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListTypeId), ListTypeId)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => documents), documents));

                Gc_EmrDocument.BeginUpdate();
                Gc_EmrDocument.DataSource = documents;
                Gc_EmrDocument.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_EMR_DOCUMENT> GetDocumentByInfoRecod(InfoRecordADO record, long documentTypeId = 0)
        {
            List<V_EMR_DOCUMENT> result = new List<V_EMR_DOCUMENT>();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => record), record));
                if (record != null)
                {
                    result = ListDocument.Where(o => (o.DOCUMENT_TYPE_ID ?? 0) == (documentTypeId > 0 ? documentTypeId : record.DOCUMENT_TYPE_ID)).ToList();
                    if (!String.IsNullOrWhiteSpace(record.SEARCH_CODE))
                    {
                        result = result.Where(o => !String.IsNullOrWhiteSpace(o.HIS_CODE) && o.HIS_CODE.Contains(record.SEARCH_CODE)).ToList();
                    }
                    else
                    {
                        List<string> searchCode = CurrentInfoRecord.Select(s => s.SEARCH_CODE).ToList();
                        foreach (var item in searchCode)
                        {
                            if (!String.IsNullOrWhiteSpace(item))
                            {
                                result = result.Where(o => String.IsNullOrWhiteSpace(o.HIS_CODE) || !o.HIS_CODE.Contains(item)).ToList();
                            }
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                }
            }
            catch (Exception ex)
            {
                result = new List<V_EMR_DOCUMENT>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ProcessCaptionGridInfoRecord()
        {
            try
            {
                if (CurrentType == null) return;

                string code = "Mã";
                string type = "Loại";
                string time = "Thời gian";
                string depa = "Khoa tạo";
                string creator = "Người tạo";
                if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT)
                {
                    code = "Mã y lệnh";
                    time = "Thời gian chỉ định";
                    depa = "Khoa chỉ định";
                    creator = "Người chỉ định";
                }
                else
                {
                    if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_ASSIGN
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__SERVICE_RESULT
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__PRESCRIPTION)
                    {
                        code = "Mã y lệnh";
                        time = "Thời gian chỉ định";
                        depa = "Khoa chỉ định";
                        creator = "Người chỉ định";
                    }
                    else if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__INFUSION
                        || CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__MEDI_REACT)
                    {
                        time = "Thời gian truyền";
                        depa = "Dung dịch";
                    }
                    else if (CurrentType.ID == IMSys.DbConfig.EMR_RS.EMR_DOCUMENT_TYPE.ID__DEBATE)
                    {
                        time = "Thời gian hội chẩn";
                    }
                }


                Gv_IR_Code.Caption = code;
                Gv_IR_Type.Caption = type;
                Gv_IR_CreateTime.Caption = time;
                Gv_IR_DepartmentName.Caption = depa;
                Gv_IR_Creator.Caption = creator;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GvEmrDocumentType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    CurrentType = (EmrDocumentTypeADO)GvEmrDocumentType.GetFocusedRow();
                    ProcessCaptionGridInfoRecord();
                    ProcessFillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_InfoRecord_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                ProcessDataGridDocument();
                var row = (InfoRecordADO)Gv_InfoRecord.GetFocusedRow();
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATOR" && !string.IsNullOrEmpty(row.CREATOR))
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfoUser").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfoUser'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.InfoUser' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.CREATOR);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonView_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_EMR_DOCUMENT)Gv_EmrDocument.GetFocusedRow();
                if (row != null)
                {
                    V_HIS_ROOM room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                    CommonParam param = new CommonParam();
                    EmrDocumentDownloadFileSDO filter = new EmrDocumentDownloadFileSDO();
                    filter.EmrDocumentViewFilter = new EmrDocumentViewFilter();
                    filter.EmrDocumentViewFilter.ID = row.ID;
                    filter.IsMerge = false;
                    filter.IsShowPatientSign = null;
                    filter.IsShowWatermark = null;
                    if (room != null)
                    {
                        filter.RoomCode = room.ROOM_CODE;
                        filter.DepartmentCode = room.DEPARTMENT_CODE;
                    }

                    filter.IsView = true;

                    var listDocumentFile = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);

                    if (listDocumentFile != null && listDocumentFile.Count > 0)
                    {
                        String temFile = Path.GetTempFileName();
                        temFile = temFile.Replace(".tmp", ".pdf");
                        Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(listDocumentFile.FirstOrDefault().Base64Data))), temFile);
                        
                        SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                        InputADO inputADO = new InputADO();
                        inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                        inputADO.IsSave = false;
                        if ((row.REJECTER == null
                   && row.NEXT_SIGNER == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                   || (((row.NEXT_SIGNER == null && (row.SIGNERS == null || !row.SIGNERS.Contains("#@!@#" + row.PATIENT_CODE))) || (row.NEXT_SIGNER != null && row.NEXT_SIGNER.Contains("#@!@#" + row.PATIENT_CODE)))
                           && HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>("EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION") == "3"))
                        {
                            inputADO.IsSign = true;
                        }
                        else
                            inputADO.IsSign = false;


                        inputADO.IsSave = false;
                        inputADO.IsExport = false;

                        inputADO.IsPrint = true;

                        inputADO.IsEnableButtonPrint = controlAcs != null && controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "EMR000002") != null;
                        inputADO.IsShowPatientSign = true;
                        //Mở popup 
                        inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
                        inputADO.Treatment.TREATMENT_CODE = row.TREATMENT_CODE;//mã hồ sơ điều trị

                        inputADO.DocumentCode = row.DOCUMENT_CODE;
                        inputADO.DocumentName = row.DOCUMENT_NAME;//Tên văn bản cần tạo

                        inputADO.DlgOpenModuleConfig = OpenSignConfig;
                        if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
                        {
                            libraryProcessor.ShowPopup(temFile, inputADO);
                            BtnSearch_Click(null,null);
                        }
                        else
                        {
                            XtraMessageBox.Show("Không xác định được văn bản ký");
                        }

                        if (File.Exists(temFile)) File.Delete(temFile);
                    }
                    else
                    {
                        #region Hien thi message thong bao
                        MessageManager.Show(this, param, false);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenSignConfig(DocumentTDO obj)
        {
            try
            {
                if (obj != null)
                {
                    EMR.Filter.EmrDocumentFilter filter = new EMR.Filter.EmrDocumentFilter();
                    filter.DOCUMENT_CODE__EXACT = obj.DocumentCode;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT>>(EMR.URI.EmrDocument.GET, ApiConsumer.ApiConsumers.EmrConsumer, filter, SessionManager.ActionLostToken, null);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        List<object> _listObj = new List<object>();
                        _listObj.Add(apiResult.Max(o => o.ID));//truyền vào id lớn nhất;

                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrSign", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void Gv_EmrDocument_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                            {
                                e.Value = imageList1.Images[5];
                            }
                            else if (string.IsNullOrEmpty(data.SIGNERS))//chưa ký
                            {
                                e.Value = null;
                            }
                            else if (!string.IsNullOrEmpty(data.SIGNERS) && !string.IsNullOrEmpty(data.UN_SIGNERS))//Đang ký
                            {
                                e.Value = imageList1.Images[1];
                            }
                            else //Đã ký
                            {
                                e.Value = imageList1.Images[0];
                            }
                        }
                        else if (e.Column.FieldName == "SIGNERS_Str")
                        {
                            if (String.IsNullOrWhiteSpace(data.SIGNERS))
                            {
                                e.Value = null;
                            }
                            else
                            {
                                e.Value = GetSigners(data.SIGNERS);
                            }
                        }
                        else if (e.Column.FieldName == "UN_SIGNERS_Str")
                        {
                            if (String.IsNullOrWhiteSpace(data.UN_SIGNERS))
                            {
                                e.Value = null;
                            }
                            else
                            {
                                e.Value = GetSigners(data.UN_SIGNERS);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_Str")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetSigners(string str)
        {
            string result = "";
            try
            {
                if (String.IsNullOrWhiteSpace(str))
                    return result;
                List<string> listStr = new List<string>();
                var list = str.Split(',');
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if (String.IsNullOrWhiteSpace(item))
                            continue;
                        string signer = "";
                        if (item.Contains("#@!@#")) //Mã bệnh nhân
                        {
                            try
                            {
                                int index = item.LastIndexOf("#@!@#");
                                signer = item.Substring(index + 5, 10) + " - bệnh nhân ký";//Mã bệnh nhân
                            }
                            catch (Exception)
                            {
                                var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.Trim() == item.Trim()).FirstOrDefault();
                                signer = String.Format("{0}{1}", item, user != null ? (" - " + user.USERNAME) : "");
                            }
                        }
                        else
                        {
                            var user = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => o.LOGINNAME.Trim() == item.Trim()).FirstOrDefault();
                            signer = String.Format("{0}{1}", item, user != null ? (" - " + user.USERNAME) : "");
                        }
                        listStr.Add(signer);
                    }
                    result = String.Join("; ", listStr);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void Gv_InfoRecord_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    InfoRecordADO data = (InfoRecordADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            var documents = GetDocumentByInfoRecod(data);
                            if (documents != null && documents.Count > 0)
                            {
                                if (documents.Exists(o => !string.IsNullOrEmpty(o.SIGNERS) && string.IsNullOrEmpty(o.UN_SIGNERS)))//đã ký
                                {
                                    e.Value = imageList1.Images[4];
                                }
                                else if (documents.Exists(o => !string.IsNullOrEmpty(o.UN_SIGNERS) && !string.IsNullOrWhiteSpace(o.SIGNERS)))//Đang ký
                                {
                                    e.Value = imageList1.Images[2];
                                }
                                else//chưa ký
                                {
                                    e.Value = imageList1.Images[5];
                                }
                            }
                            else //không có vb
                            {
                                e.Value = imageList1.Images[3];
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

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && (e.SelectedControl == Gc_InfoRecord || e.SelectedControl == Gc_EmrDocument))
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = ((DevExpress.XtraGrid.GridControl)e.SelectedControl).FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column || lastGrid != e.SelectedControl.Name)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            lastGrid = e.SelectedControl.Name;
                            string text = "";
                            if (info.Column.FieldName == "STT")
                            {
                                if (e.SelectedControl == Gc_InfoRecord)
                                {
                                    InfoRecordADO data = (InfoRecordADO)view.GetRow(info.RowHandle);
                                    if (data != null)
                                    {
                                        var documents = GetDocumentByInfoRecod(data);
                                        if (documents != null && documents.Count > 0)
                                        {
                                            if (documents.Exists(o => !string.IsNullOrEmpty(o.SIGNERS) && string.IsNullOrEmpty(o.UN_SIGNERS)))//đã ký
                                            {
                                                text = "Hoàn thành";
                                            }
                                            else if (documents.Exists(o => !string.IsNullOrEmpty(o.UN_SIGNERS) && !string.IsNullOrWhiteSpace(o.SIGNERS)))//Đang ký
                                            {
                                                text = "Đang ký";
                                            }
                                            else//chưa ký
                                            {
                                                text = "Chưa ký";
                                            }
                                        }
                                        else //không có vb
                                        {
                                            text = "Chưa có văn bản";
                                        }
                                    }
                                }
                                else if (e.SelectedControl == Gc_EmrDocument)
                                {
                                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)view.GetRow(info.RowHandle);
                                    if (data != null)
                                    {
                                        if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                                        {
                                            text = "Đã hủy";
                                        }
                                        else if (string.IsNullOrEmpty(data.SIGNERS))//chưa ký
                                        {
                                            text = "Chưa ký";
                                        }
                                        else if (!string.IsNullOrEmpty(data.SIGNERS) && !string.IsNullOrEmpty(data.UN_SIGNERS))//Đang ký
                                        {
                                            text = "Đang ký";
                                        }
                                        else//Đã ký
                                        {
                                            text = "Hoàn thành";
                                        }
                                    }
                                }
                            }
                            else if (info.Column.FieldName == "CREATOR")
                            {
                                InfoRecordADO data = (InfoRecordADO)view.GetRow(info.RowHandle);
                                if (data != null && !string.IsNullOrEmpty(data.CREATOR))
                                {
                                    text = "Click vào để xem thông tin chi tiết";
                                }

                            }

                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnKhongDat_Click(object sender, EventArgs e)
        {
            try
            {

                frmContentFailed ContentFailed = new frmContentFailed(moduleData, this.treatmentId, DgSeccess);
                ContentFailed.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DgSeccess(bool success)
        {
            try
            {
                if (success)
                {
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDat_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<long?> Input = new List<long?>();
                Input.Add(this.treatmentId);

                var resultData = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/ApprovalStore", ApiConsumers.MosConsumer, Input, param);

                if (resultData != null && resultData.Count > 0)
                {
                    success = true;
                }
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnHuyDuyet_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                List<long?> Input = new List<long?>();
                Input.Add(this.treatmentId);

                var resultData = new BackendAdapter(param).Post<List<HIS_TREATMENT>>("api/HisTreatment/UnapprovalStore", ApiConsumers.MosConsumer, Input, param);

                if (resultData != null && resultData.Count > 0)
                {
                    success = true;
                }
                btnKhongDat.Enabled = false;
                btnDat.Enabled = false;
                btnHuyDuyet.Enabled = false;

                FillDataToGrid();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnKhongDat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnKhongDat.Enabled == true)
                {
                    btnKhongDat_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnDat_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnDat.Enabled == true)
                {

                    btnDat_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnHuyDuyet_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnHuyDuyet.Enabled == true)
                {
                    btnHuyDuyet_Click(null, null);
                    btnKhongDat.Enabled = false;
                    btnDat.Enabled = false;
                    btnHuyDuyet.Enabled = false;

                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_EmrDocument_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var row = (V_EMR_DOCUMENT)Gv_EmrDocument.GetFocusedRow();
                if (row != null)
                {
                    if (e.Column.FieldName == "CREATOR" && !string.IsNullOrEmpty(row.CREATOR))
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfoUser").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfoUser'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.InfoUser' is not plugins");
                        List<object> listArgs = new List<object>();
                        listArgs.Add(row.CREATOR);
                        var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GvEmrDocumentType_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    EmrDocumentTypeADO CurrentType = (EmrDocumentTypeADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DOCUMENT_TYPE_NAME")
                    {
                        if (CurrentType.IsHasDocumentNoPatientSign)
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                        else if (CurrentType.IsHasDocument)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OrderListByCheckBox()
        {
            try
            {
                if (chkUuTien.Checked)
                {
                    var lstSortTemp = ListDocumentType.Where(o => o.IsHasDocument).OrderByDescending(o => o.IsHasDocument).ThenByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList();
                    lstSortTemp.AddRange(ListDocumentType.Where(o => !o.IsHasDocument && o.NUM_ORDER == null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    lstSortTemp.AddRange(ListDocumentType.Where(o => !o.IsHasDocument && o.NUM_ORDER != null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    ListDocumentType = lstSortTemp;
                }
                else
                {
                    var lstSortTemp = ListDocumentType.Where(o => o.NUM_ORDER == null).OrderBy(o => o.DOCUMENT_TYPE_NAME).ToList();
                    lstSortTemp.AddRange(ListDocumentType.Where(o => o.NUM_ORDER != null).OrderByDescending(o => o.NUM_ORDER).ThenBy(o => o.DOCUMENT_TYPE_NAME).ToList());
                    ListDocumentType = lstSortTemp;
                }
                GcEmrDocumentType.BeginUpdate();
                GcEmrDocumentType.DataSource = ListDocumentType;
                GcEmrDocumentType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkUuTien_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                OrderListByCheckBox();


                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDocumentType.Select(o => new { o.DOCUMENT_TYPE_NAME, o.NUM_ORDER }).ToList()), ListDocumentType.Select(o => new { o.DOCUMENT_TYPE_NAME, o.NUM_ORDER }).ToList()));
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkUuTien.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.HisTreatmentRecordChecking").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkUuTien.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkUuTien.Name;
                    csAddOrUpdate.VALUE = chkUuTien.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_Treatment_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "PATIENT_DOB_ForDisplay")
                        {
                            string patientDOB = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_DOB") ?? "").ToString();
                            e.Value = patientDOB.Length >= 4 ? patientDOB.Substring(0, 4) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_Treatment_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.RowHandle == view.FocusedRowHandle && this.hasSelectedTreatment)
                    {
                        e.Appearance.FontStyleDelta = FontStyle.Bold;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private bool hasSelectedTreatment = false;
        private List<ACS_CONTROL> controlAcs;

        private void Gv_Treatment_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT data = (V_HIS_TREATMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        this.hasSelectedTreatment = true;
                        TxtTreatmentCode.Text = data.TREATMENT_CODE;
                        Gv_Treatment.RefreshRow(e.RowHandle);
                        FillDataToGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkToiTao_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void FormHisTreatmentRecordChecking_Resize(object sender, EventArgs e)
        {
            try
            {
                SetCustomSizeForGridView(ref Gv_Treatment);
                SetCustomSizeForGridView(ref Gv_EmrDocument);
                SetCustomSizeForGridView(ref Gv_InfoRecord);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void SetCustomSizeForGridView(ref DevExpress.XtraGrid.Views.Grid.GridView gridView)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo info = gridView.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
                var listEmrDocumentCols = gridView.Columns.ToList();
                if (info.Bounds.Width > listEmrDocumentCols.Sum(o => o.Width))
                {
                    gridView.OptionsView.ColumnAutoWidth = true;
                }
                else
                {
                    gridView.OptionsView.ColumnAutoWidth = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void chkIncludeCancelDoc_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsLoadFirstForm)
                    return;
                FillDataToGrid();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (currentControlStateRDO != null && currentControlStateRDO.Count > 0) ? currentControlStateRDO.Where(o => o.KEY == chkIncludeCancelDoc.Name && o.MODULE_LINK == "HIS.Desktop.Plugins.HisTreatmentRecordChecking").FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkIncludeCancelDoc.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIncludeCancelDoc.Name;
                    csAddOrUpdate.VALUE = chkIncludeCancelDoc.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.HisTreatmentRecordChecking";
                    if (currentControlStateRDO == null)
                        currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    currentControlStateRDO.Add(csAddOrUpdate);
                }
                controlStateWorker.SetData(currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSession.Warn(ex);
            }
        }

        private void Gv_EmrDocument_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_EMR_DOCUMENT)Gv_EmrDocument.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_DELETE == IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        var documentType = ListDocumentType != null && ListDocumentType.Count > 0 ? ListDocumentType.FirstOrDefault(o => o.ID == data.DOCUMENT_TYPE_ID) : null;
                        if (documentType != null && documentType.PATIENT_MUST_SIGN == 1 && (String.IsNullOrEmpty(data.SIGNERS) || !data.SIGNERS.Contains("#@!@#" + data.PATIENT_CODE)))
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Gv_InfoRecord_Click(object sender, EventArgs e)
        {
            ChangeAllEditColumn(false);
        }

        private void Gv_InfoRecord_DoubleClick(object sender, EventArgs e)
        {
            ChangeAllEditColumn(true);
        }
        private void ChangeAllEditColumn(bool IsAllowEdit)
        {
            try
            {
                if (Gv_InfoRecord.FocusedRowHandle > -1)
                {
                    var columnFocus = Gv_InfoRecord.FocusedColumn;
                    if (columnFocus.FieldName == "CODE")
                    {
                        if (IsAllowEdit)
                        {

                        }
                        foreach (var item in Gv_InfoRecord.Columns.ToList())
                        {
                            if (item.FieldName == columnFocus.FieldName)
                            {
                                item.OptionsColumn.AllowEdit = IsAllowEdit;
                                item.OptionsColumn.ReadOnly = true;
                                break;
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
    }
}
