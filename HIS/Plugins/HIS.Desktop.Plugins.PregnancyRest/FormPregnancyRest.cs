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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.Core.RelaytionType;
using HIS.Desktop.Plugins.Library.PrintTreatmentEndTypeExt;
using HIS.Desktop.Plugins.PregnancyRest.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using ACS.Filter;
using ACS.SDO;
using ACS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using HIS.UC.WorkPlace;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.PregnancyRest.Validation;
using His.Bhyt.InsuranceExpertise.LDO;
using His.Bhyt.InsuranceExpertise;

namespace HIS.Desktop.Plugins.PregnancyRest
{
    public partial class FormPregnancyRest : HIS.Desktop.Utility.FormBase
    {
        private MOS.EFMODEL.DataModels.HIS_TREATMENT hisTreatment;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        List<HIS_TREATMENT> lstTreatmentByPatient = null;
        private int positionHandle = -1;
        private List<BabyADO> babyAdos;
        private long TreatmentId;
        private HIS_TREATMENT hisTreatmentResult;
        protected object workPlace { get; set; }

        private Inventec.Desktop.Common.Modules.Module currentModule;
        List<ACS_USER> acsUser;
        String creator = "";

        const string XML2076CFG = "MOS.HIS_TREATMENT.XML2076.EXPORT_OPTION";
        string xml2076ExportOption;

        public FormPregnancyRest()
        {
            InitializeComponent();
        }

        public FormPregnancyRest(long treatmentId, Inventec.Desktop.Common.Modules.Module moduleData)
            : this()
        {
            try
            {
                // TODO: Complete member initialization
                this.TreatmentId = treatmentId;
                this.moduleData = moduleData;
                this.currentModule = moduleData;
                this.Text = moduleData.text;

                this.creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormPregnancyRest_Load(object sender, EventArgs e)
        {
            try
            {
                checkConfig();
                this.xml2076ExportOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(XML2076CFG);
                SetIcon();
                ValidateForm();
                InitWorkPlaceControl();
                InitComboGender();
                InitComboEthnic();
                InitComboRelaytionType();
                InitComboEndTypeExt();
                InitComboDocumentBookId();
                InitComboSickUserName();
                LoadDataToGrid();
                SetDefaultValueControl();
                CboTreatmentEndTypExt.Focus();
               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        List<string> connectInfors = new List<string>();
        string api = "";
        string nameCb = "";
        string cccdCb = "";
        private void checkConfig()
        {
            try
            {
                HisConfigCHECKHEINCARD.LoadConfig();
                string connect_infor = HisConfigCHECKHEINCARD.CHECK_HEIN_CARD_BHXH__API;
                string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employee = GetEmployee(username);
                LogSystem.Debug("employee: " + LogUtil.TraceData("employee", employee));
                if (!string.IsNullOrEmpty(connect_infor))
                {
                    connectInfors = connect_infor.Split('|').ToList();
                    api = connectInfors.Count > 0 ? connectInfors[0] : string.Empty;

                    nameCb = connectInfors.Count > 1 && !string.IsNullOrEmpty(connectInfors[1]) ? connectInfors[1] : employee.TDL_USERNAME;
                    cccdCb = connectInfors.Count > 2 && !string.IsNullOrEmpty(connectInfors[2]) ? connectInfors[2] : employee.IDENTIFICATION_NUMBER;

                    LogSystem.Debug("BHXHLoginCFG.OFFICERNAME: " + connectInfors[1]);
                    LogSystem.Debug("BHXHLoginCFG.CCCDOFFICER: " + connectInfors[2]);
                }
                else
                {
                    nameCb = employee.TDL_USERNAME;
                    cccdCb = employee.IDENTIFICATION_NUMBER;
                    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private HIS_EMPLOYEE GetEmployee(string username)
        {
            HIS_EMPLOYEE result = new HIS_EMPLOYEE();
            try
            {
                var rs = BackendDataWorker.Get<HIS_EMPLOYEE>().Where(s => s.LOGINNAME == username).FirstOrDefault();
                if(rs != null)
                {
                    result = rs;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
        private void LoadDataToGrid()
        {
            try
            {
                babyAdos = new List<BabyADO>();
                if (this.TreatmentId > 0)
                {
                    HisBabyFilter filter = new HisBabyFilter();
                    filter.TREATMENT_ID = TreatmentId;

                    var listBaby = new BackendAdapter(new CommonParam()).Get<List<HIS_BABY>>("api/HisBaby/Get", ApiConsumers.MosConsumer, filter, new CommonParam());
                    if (listBaby != null && listBaby.Count > 0)
                    {
                        foreach (var item in listBaby)
                        {
                            babyAdos.Add(new BabyADO(item));
                        }
                    }
                    else
                    {
                        babyAdos.Add(new BabyADO());
                    }
                }

                if (babyAdos.Count <= 0)
                {
                    babyAdos.Add(new BabyADO());
                }

                gridControlBaby.BeginUpdate();
                gridControlBaby.DataSource = babyAdos;
                gridControlBaby.EndUpdate();
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
                this.Icon = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_ICON;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboSickUserName()
        {
            try
            {
                acsUser = BackendDataWorker.Get<ACS_USER>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "LOGINNAME", columnInfos, false, 150);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboSickUserName, acsUser, controlEditorADO);
                cboSickUserName.Properties.Buttons[1].Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboDocumentBookId()
        {
            try
            {
                List<V_HIS_DOCUMENT_BOOK> rs = null;
                try
                {
                    HisDocumentBookViewFilter dBookFilter = new HisDocumentBookViewFilter();
                    dBookFilter.FOR_SICK_BHXH = true;
                    dBookFilter.IS_OUT_NUM_ORDER = false;
                    dBookFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    rs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_DOCUMENT_BOOK>>("api/HisDocumentBook/GetView", ApiConsumers.MosConsumer, dBookFilter, null);

                    long year = Convert.ToInt64((this.DtRestTimeTo.EditValue != null && this.DtRestTimeTo.DateTime != DateTime.MinValue) ? this.DtRestTimeTo.DateTime.ToString("yyyy") : DateTime.Now.ToString("yyyy"));
                    LogSystem.Debug("LoadDocumentBook.Year: " + year);
                    rs = rs != null ? rs.Where(o => !o.YEAR.HasValue || o.YEAR.Value == year).ToList() : null;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    rs = null;
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("DOCUMENT_BOOK_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DOCUMENT_BOOK_NAME", "ID", columnInfos, false, 150);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboDocumentBookId, rs, controlEditorADO);

                if (rs != null && rs.Count == 1)
                {
                    cboDocumentBookId.EditValue = rs.First().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboGender()
        {
            try
            {
                var gender = BackendDataWorker.Get<HIS_GENDER>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("GENDER_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("GENDER_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("GENDER_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(repositoryItemCboGender, gender, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboEthnic()
        {
            try
            {
                var ethnic = BackendDataWorker.Get<SDA_ETHNIC>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ETHNIC_CODE", "", 50, 1, true));
                columnInfos.Add(new ColumnInfo("ETHNIC_NAME", "", 100, 2, true));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ETHNIC_NAME", "ETHNIC_CODE", columnInfos, false, 150);
                ControlEditorLoader.Load(repositoryItemCboEthnic, ethnic, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRelaytionType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "Name", columnInfos, false, 250);
                ControlEditorLoader.Load(CboRelativeType, RelaytionTypeDataWorker.RelaytionTypeADOs, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboEndTypeExt()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("TREATMENT_END_TYPE_EXT_NAME", "", 150, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_END_TYPE_EXT_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(CboTreatmentEndTypExt, HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_TREATMENT_END_TYPE_EXT>(), controlEditorADO);
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
                if (this.TreatmentId > 0)
                {
                    HisTreatmentViewFilter treatmentFilter = new HisTreatmentViewFilter();
                    treatmentFilter.ID = this.TreatmentId;
                    var treatments = new BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treatmentFilter, new CommonParam());
                    if (treatments != null && treatments.Count > 0)
                    {
                        this.hisTreatment = treatments.FirstOrDefault();
                    }

                    FillDataToControl(hisTreatment);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl(HIS_TREATMENT hisTreatment)
        {
            try
            {
                if (hisTreatment != null)
                {
                    if (!String.IsNullOrWhiteSpace(hisTreatment.TDL_PATIENT_WORK_PLACE_NAME))
                    {
                        var workPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => !String.IsNullOrWhiteSpace(o.WORK_PLACE_NAME) && o.WORK_PLACE_NAME.ToLower() == hisTreatment.TDL_PATIENT_WORK_PLACE_NAME.ToLower());
                        if (workPlace != null)
                        {
                            cboWorkPlace.EditValue = workPlace.ID;
                            txtCodeWorkPlace.Text = workPlace.WORK_PLACE_CODE;
                            cboWorkPlace.Properties.Buttons[1].Visible = true;
                        }
                    }

                    txtWorkPlace.Text = hisTreatment.TDL_PATIENT_WORK_PLACE;

                    if (!String.IsNullOrEmpty(hisTreatment.SICK_LOGINNAME))
                    {
                        txtSickUserName.Text = hisTreatment.SICK_LOGINNAME;
                        cboSickUserName.EditValue = hisTreatment.SICK_LOGINNAME;
                        cboSickUserName.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        txtSickUserName.Text = creator;
                        cboSickUserName.EditValue = creator;
                        cboSickUserName.Properties.Buttons[1].Visible = true;
                    }

                    //có thì gán không thì gán theo tự động nếu có 1 bản ghi
                    if (hisTreatment.DOCUMENT_BOOK_ID.HasValue)
                    {
                        cboDocumentBookId.EditValue = hisTreatment.DOCUMENT_BOOK_ID;
                    }

                    SpLeaveDay.EditValue = hisTreatment.SICK_LEAVE_DAY;
                    if (hisTreatment.SICK_LEAVE_FROM != null)
                    {
                        DateTime? FromTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.SICK_LEAVE_FROM.Value);
                        DtRestTimeFrom.EditValue = FromTime;
                    }
                    else
                    {
                        DtRestTimeFrom.EditValue = null;
                    }

                    if (hisTreatment.SICK_LEAVE_TO != null)
                    {
                        DateTime? ToTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.SICK_LEAVE_TO.Value);
                        DtRestTimeTo.EditValue = ToTime;
                    }
                    else
                    {
                        DtRestTimeTo.EditValue = null;
                    }

                    txtPatientMotherName.Text = hisTreatment.TDL_PATIENT_MOTHER_NAME;
                    txtPatientFatherName.Text = hisTreatment.TDL_PATIENT_FATHER_NAME;
                    TxtRelativeName.Text = hisTreatment.TDL_PATIENT_RELATIVE_NAME;//TODO
                    LblExtraEndCode.Text = hisTreatment.EXTRA_END_CODE;
                    txtTreatmentMethod.Text = hisTreatment.TREATMENT_METHOD;
                    txtMaBHXH.Text = hisTreatment.TDL_SOCIAL_INSURANCE_NUMBER ?? "";
                    if (!String.IsNullOrWhiteSpace(hisTreatment.SICK_HEIN_CARD_NUMBER))
                    {
                        txtHeinCardNumber.Text = hisTreatment.SICK_HEIN_CARD_NUMBER;
                        if (String.IsNullOrWhiteSpace(hisTreatment.TDL_SOCIAL_INSURANCE_NUMBER))
                        {
                            if (hisTreatment.SICK_HEIN_CARD_NUMBER.Length == 10)
                            {
                                txtMaBHXH.Text = hisTreatment.SICK_HEIN_CARD_NUMBER;
                            }
                            else
                            {
                                txtMaBHXH.Text = hisTreatment.SICK_HEIN_CARD_NUMBER.Substring(5, 10); ;
                            }
                        }
                    }
                    else if (!String.IsNullOrWhiteSpace(hisTreatment.TDL_HEIN_CARD_NUMBER))
                    {
                        txtHeinCardNumber.Text = hisTreatment.TDL_HEIN_CARD_NUMBER;
                        if (String.IsNullOrWhiteSpace(hisTreatment.TDL_SOCIAL_INSURANCE_NUMBER))
                        {
                            if (hisTreatment.TDL_HEIN_CARD_NUMBER.Length == 10)
                            {
                                txtMaBHXH.Text = hisTreatment.TDL_HEIN_CARD_NUMBER;
                            }
                            else
                            {
                                txtMaBHXH.Text = hisTreatment.TDL_HEIN_CARD_NUMBER.Substring(5, 10); ;
                            }
                        }
                    }
                    else
                        txtHeinCardNumber.Text = "";

                    if (hisTreatment.TDL_PATIENT_RELATIVE_TYPE != null)
                    {
                        if (ConvertString(hisTreatment.TDL_PATIENT_RELATIVE_TYPE) != "Cha" && ConvertString(hisTreatment.TDL_PATIENT_RELATIVE_TYPE) != "Mẹ")
                            CboRelativeType.EditValue = "Khác";
                        else
                            CboRelativeType.EditValue = ConvertString(hisTreatment.TDL_PATIENT_RELATIVE_TYPE);
                    }
                    chkIsPregnancyTermination.Checked = hisTreatment.IS_PREGNANCY_TERMINATION == 1 ? true : false;
                    if (hisTreatment.GESTATIONAL_AGE != null)
                    {
                        txtGestationalAge.Text = hisTreatment.GESTATIONAL_AGE.ToString();
                    }
                    else
                    {
                        txtGestationalAge.Text = "";
                    }
                    if (hisTreatment.PREGNANCY_TERMINATION_TIME != null)
                    {
                        DateTime pregnancyTerminationTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisTreatment.PREGNANCY_TERMINATION_TIME.Value) ?? DateTime.Now;
                        dtPregnancyTerminationTime.DateTime = pregnancyTerminationTime;
                    }
                    else
                        dtPregnancyTerminationTime.EditValue = null;
                    txtPregnancyTerminationReason.Text = hisTreatment.PREGNANCY_TERMINATION_REASON;
                    txtEndTypeExtNote.Text = hisTreatment.END_TYPE_EXT_NOTE;
                    CboTreatmentEndTypExt.EditValue = hisTreatment.TREATMENT_END_TYPE_EXT_ID;

                    EditCaption();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnPrint.Enabled) return;
                ProcessPrint(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSaveNPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSaveNPrint.Enabled) return;
                ProcessSaveNPrint(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSave.Enabled) return;

                ProcessSaveNPrint(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadTreatmentByPatient()
        {
            try
            {
                if (this.hisTreatment != null)
                {
                    this.lstTreatmentByPatient = new List<HIS_TREATMENT>();
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter filter = new HisTreatmentFilter();
                    filter.PATIENT_ID = this.hisTreatment.PATIENT_ID;
                    filter.TREATMENT_END_TYPE_EXT_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM;
                    filter.TDL_TREATMENT_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    this.lstTreatmentByPatient = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);

                }
                else
                {
                    this.lstTreatmentByPatient = null;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ProcessSaveNPrint(bool Print)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProvider1.Validate()) return;
                //save
                bool success = false;
                HisTreatmentExtraEndInfoSDO sdo = new HisTreatmentExtraEndInfoSDO();

                sdo.TreatmentId = this.hisTreatment.ID;

                long TreatmentEndTypExtID = CboTreatmentEndTypExt.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(CboTreatmentEndTypExt.EditValue.ToString()) : -99;
                if (this.hisTreatment.TDL_TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM && TreatmentEndTypExtID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                {
                    if (SpLeaveDay.EditValue != null && SpLeaveDay.Value > 30)
                    {
                        SpLeaveDay.Focus();
                        SpLeaveDay.SelectAll();
                        DevExpress.XtraEditors.XtraMessageBox.Show("Số ngày nghỉ không được vượt quá 30 ngày", "Thông báo");
                        return;
                    }

                    LoadTreatmentByPatient();

                    if (this.lstTreatmentByPatient != null && this.lstTreatmentByPatient.Count > 0)
                    {
                        var dt = this.lstTreatmentByPatient.Where(o => o.ID != this.hisTreatment.ID
                                            && o.SICK_LEAVE_FROM != null && Int64.Parse(DtRestTimeFrom.DateTime.ToString("yyyyMMdd") + "000000") >= o.SICK_LEAVE_FROM
                                            && o.SICK_LEAVE_TO != null && Int64.Parse(DtRestTimeFrom.DateTime.ToString("yyyyMMdd") + "000000") <= o.SICK_LEAVE_TO).ToList();
                        if (dt != null && dt.Count > 0)
                        {
                            var treatmentCheck = dt.OrderByDescending(o => o.OUT_TIME).ToList()[0];
                            DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Ngày nghỉ ốm giao với ngày nghỉ ốm được cấp của đợt khám trước đó: {0} (nghỉ từ {1} - {2})", treatmentCheck.TREATMENT_CODE,
                                    Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_FROM ?? 0),
                                    Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentCheck.SICK_LEAVE_TO ?? 0)), "Thông báo");
                            DtRestTimeFrom.Focus();
                            return;
                        }
                    }
                }

                if (CboTreatmentEndTypExt.EditValue != null)
                {
                    //if (!string.IsNullOrEmpty(txtCodeWorkPlace.Text))
                    //{
                    //    sdo.WorkPlaceId = Inventec.Common.TypeConvert.Parse.ToInt32(this.cboWorkPlace.EditValue.ToString());
                    //}

                    if (cboDocumentBookId.EditValue != null)
                        sdo.DocumentBookId = Inventec.Common.TypeConvert.Parse.ToInt64(cboDocumentBookId.EditValue.ToString());

                    if (cboSickUserName.EditValue != null)
                    {
                        ACS_USER user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.LOGINNAME == cboSickUserName.EditValue.ToString());
                        if (user != null)
                        {
                            sdo.SickUsername = user.USERNAME;
                            sdo.SickLoginname = user.LOGINNAME;
                        }
                    }

                    sdo.TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(CboTreatmentEndTypExt.EditValue.ToString());
                    //sdo.PatientRelativeName = TxtRelativeName.Text;
                    //sdo.PatientRelativeType = CboRelativeType.Text.Trim();
                    sdo.SickHeinCardNumber = txtHeinCardNumber.Text.Trim();
                    //sdo.SocialInsuranceNumber = txtMaBHXH.Text.Trim();

                    if (String.IsNullOrWhiteSpace(sdo.SocialInsuranceNumber) && !String.IsNullOrWhiteSpace(sdo.SickHeinCardNumber))
                    {
                        if (sdo.SickHeinCardNumber.Length == 10)
                        {
                            txtMaBHXH.Text = sdo.SickHeinCardNumber;
                        }
                        else
                        {
                            txtMaBHXH.Text = sdo.SickHeinCardNumber.Substring(5, 10); ;
                        }

                        sdo.SocialInsuranceNumber = txtMaBHXH.Text;
                    }

                    sdo.TreatmentMethod = txtTreatmentMethod.Text.Trim();
                    //sdo.PatientWorkPlace = txtWorkPlace.Text;

                    if (DtRestTimeFrom.EditValue != null)
                    {
                        sdo.SickLeaveFrom = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtRestTimeFrom.DateTime);
                    }

                    if (DtRestTimeTo.EditValue != null)
                    {
                        sdo.SickLeaveTo = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DtRestTimeTo.DateTime);
                    }

                    if (SpLeaveDay.EditValue != null)
                    {
                        sdo.SickLeaveDay = Inventec.Common.TypeConvert.Parse.ToDecimal(SpLeaveDay.EditValue.ToString());
                    }

                }

                if (!string.IsNullOrEmpty(txtCodeWorkPlace.Text))
                {
                    sdo.WorkPlaceId = Inventec.Common.TypeConvert.Parse.ToInt32(this.cboWorkPlace.EditValue.ToString());
                }
                sdo.PatientWorkPlace = txtWorkPlace.Text.Trim();
                sdo.MotherName = txtPatientMotherName.Text.Trim();
                sdo.FatherName = txtPatientFatherName.Text.Trim();
                sdo.PatientRelativeName = TxtRelativeName.Text.Trim();
                sdo.PatientRelativeType = CboRelativeType.Text.Trim();
                sdo.SocialInsuranceNumber = txtMaBHXH.Text.Trim();
                sdo.IsPregnancyTermination = chkIsPregnancyTermination.Checked ? true : false;
                if (!String.IsNullOrEmpty(txtGestationalAge.Text.Trim()))
                {
                    sdo.GestationalAge = Inventec.Common.TypeConvert.Parse.ToInt64(txtGestationalAge.Text);
                }
                else
                    sdo.GestationalAge = null;
                sdo.PregnancyTerminationReason = txtPregnancyTerminationReason.Text;
                sdo.EndTypeExtNote = txtEndTypeExtNote.Text;
                if (dtPregnancyTerminationTime.EditValue != null)
                {
                    sdo.PregnancyTerminationTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPregnancyTerminationTime.DateTime);
                }
                else
                    sdo.PregnancyTerminationTime = null;
                Inventec.Common.Logging.LogSystem.Debug("____________________" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                CommonParam param = new CommonParam();
                hisTreatmentResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateExtraEndInfo", ApiConsumer.ApiConsumers.MosConsumer, sdo, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                FillDataToControl(hisTreatmentResult);
                if (hisTreatmentResult != null)
                {
                    success = true;
                }

                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion

                if (success && Print) ProcessPrint(Print);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrint(bool PrintNow)
        {
            try
            {
                long? extType = hisTreatmentResult != null ? hisTreatmentResult.TREATMENT_END_TYPE_EXT_ID : hisTreatment.TREATMENT_END_TYPE_EXT_ID;
                if (extType.HasValue)
                {
                    var process = new PrintTreatmentEndTypeExtProcessor(hisTreatmentResult != null ? hisTreatmentResult.ID : hisTreatment.ID, Library.PrintTreatmentEndTypeExt.CreateMenu.TYPE.NORMAL, moduleData != null ? moduleData.RoomId : 0);

                    if (process != null)
                    {
                        switch (extType.Value)
                        {
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI:
                                process.Print(Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_DUONG_THAI, Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT, PrintNow);
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM:
                                process.Print(Library.PrintTreatmentEndTypeExt.Base.PrintTreatmentEndTypeExPrintType.TYPE.NGHI_OM, Library.PrintTreatmentEndTypeExt.PrintTreatmentEndTypeExtProcessor.OPTION.PRINT, PrintNow);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Không xác định được loại thông tin bổ sung");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSaveNPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSaveNPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtRestTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    DtRestTimeTo.Focus();
                    DtRestTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtRestTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtPatientMotherName.Focus();
                    txtPatientMotherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void DtRestTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientMotherName.Focus();
                    txtPatientMotherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void txtPatientMotherName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientFatherName.Focus();
                    txtPatientFatherName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientFatherName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TxtRelativeName.Focus();
                    TxtRelativeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboRelativeType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    txtCodeWorkPlace.Focus();
                    txtCodeWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SpLeaveDay_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DtRestTimeFrom.Focus();
                    DtRestTimeFrom.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SpLeaveDay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (DtRestTimeFrom.EditValue != null)
                {
                    this.CalculateDateTo();
                }
                else
                {
                    this.CalculateDateFrom();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtRestTimeFrom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (DtRestTimeTo.EditValue == null || DtRestTimeTo.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateTo();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtRestTimeTo_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (DtRestTimeFrom.EditValue == null || DtRestTimeFrom.DateTime == DateTime.MinValue)
                {
                    this.CalculateDateFrom();
                }

                this.CalculateDayNum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDayNum()
        {
            try
            {
                if (DtRestTimeFrom.EditValue != null && DtRestTimeFrom.DateTime != DateTime.MinValue
                    && DtRestTimeTo.EditValue != null && DtRestTimeTo.DateTime != DateTime.MinValue
                    && DtRestTimeFrom.DateTime.Date <= DtRestTimeTo.DateTime.Date)
                {
                    TimeSpan ts = (TimeSpan)(DtRestTimeTo.DateTime.Date - DtRestTimeFrom.DateTime.Date);
                    SpLeaveDay.Value = ts.Days + 1;
                }
                else
                    SpLeaveDay.Value = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateTo()
        {
            try
            {
                if (DtRestTimeFrom.EditValue != null && DtRestTimeFrom.DateTime != DateTime.MinValue && SpLeaveDay.EditValue != null && SpLeaveDay.Value > 0)
                {
                    DtRestTimeTo.EditValue = DtRestTimeFrom.DateTime.AddDays((double)(SpLeaveDay.Value - 1));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalculateDateFrom()
        {
            try
            {
                if (DtRestTimeTo.EditValue != null && DtRestTimeTo.DateTime != DateTime.MinValue && SpLeaveDay.EditValue != null && SpLeaveDay.Value > 0)
                {
                    DtRestTimeFrom.DateTime = DtRestTimeTo.DateTime.AddDays((double)(-SpLeaveDay.Value + 1));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnRemove_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (BabyADO)gridViewBaby.GetFocusedRow();
                if (row != null)
                {
                    babyAdos = (List<BabyADO>)gridControlBaby.DataSource;
                    babyAdos.Remove(row);

                    if (babyAdos.Count <= 0)
                    {
                        babyAdos.Add(new BabyADO());
                    }
                    gridControlBaby.BeginUpdate();
                    gridControlBaby.DataSource = babyAdos;
                    gridControlBaby.EndUpdate();
                }
                //babyAdos.Remove
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtnAdd_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                babyAdos.Add(new BabyADO());
                gridControlBaby.BeginUpdate();
                gridControlBaby.DataSource = babyAdos;
                gridControlBaby.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #region ---Validate rule
        private void ValidationMaxLength(Control control, int? maxLength, bool required = false)
        {
            try
            {
                ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.IsRequired = required;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxLengthRequired(bool IsRequied, BaseControl control, int maxlength, string tooltip)
        {
            try
            {
                ValidateMaxLength valid = new ValidateMaxLength();
                valid.tooltip = tooltip;
                valid.IsRequired = IsRequied;
                valid.txt = control;
                valid.maxLength = maxlength;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidationBHXH(Control control, int? maxLength, int minLength, bool required = false)
        {
            try
            {
                ValidateBHXHCode valid = new ValidateBHXHCode();
                valid.txtControl = txtMaBHXH;
                valid.maxLength = maxLength;
                valid.minLength = minLength;
                valid.isRequired = required;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateAgeRelativeName()
        {
            try
            {
                string age = AgeUtil.CalculateFullAge(this.hisTreatment.TDL_PATIENT_DOB);
                long Age = Inventec.Common.TypeConvert.Parse.ToInt64(age ?? "");
                ValidateAgeRelativeName valid = new ValidateAgeRelativeName();
                valid.txtControl = TxtRelativeName;
                valid.Maxlength = 100;
                valid.ErrorType = ErrorType.Warning;
                valid.age = Age;
                valid.cbo = CboTreatmentEndTypExt;
                dxValidationProvider1.SetValidationRule(TxtRelativeName, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateRelativeType()
        {
            try
            {
                string age = AgeUtil.CalculateFullAge(this.hisTreatment.TDL_PATIENT_DOB);
                long Age = Inventec.Common.TypeConvert.Parse.ToInt64(age ?? "");
                ValidateRelativeType valid = new ValidateRelativeType();
                valid.txtControl = CboRelativeType;
                valid.ErrorType = ErrorType.Warning;
                valid.age = Age;
                valid.cbo = CboTreatmentEndTypExt;
                dxValidationProvider1.SetValidationRule(CboRelativeType, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                //ValidationMaxLength(cboSickUserName, 100);
                //ValidationMaxLength(txtSickUserName, 50);
                //ValidationMaxLength(txtWorkPlace, 100);
                ValidationMaxLength(txtTreatmentMethod, 3000);
                ValidationMaxLength(txtHeinCardNumber, 20);
                ValidationMaxLength(txtEndTypeExtNote, 1000);
                ValidMaxLengthRequired(false, txtPregnancyTerminationReason, 1000, null);
                ValidateRelativeType();
                ValidateAgeRelativeName();
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationRequired(BaseEdit control)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = "Trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateSickUser()
        {
            try
            {
                this.lciProvider.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidateCombo valid = new ValidateCombo();
                valid.txtControl = this.txtSickUserName;
                valid.cbo = this.cboSickUserName;
                valid.CheckAll = true;
                valid.ErrorType = ErrorType.Warning;
                valid.ErrorText = "Trường dữ liệu bắt buộc";
                dxValidationProvider1.SetValidationRule(txtSickUserName, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateWorkPlace()
        {
            try
            {
                this.lciWorkPlace.AppearanceItemCaption.ForeColor = Color.Maroon;
                ValidateCombo valid = new ValidateCombo();
                valid.txtControl = this.txtWorkPlace;
                valid.cbo = this.cboWorkPlace;
                valid.ErrorType = ErrorType.Warning;
                valid.ErrorText = "Trường dữ liệu bắt buộc";
                dxValidationProvider1.SetValidationRule(txtCodeWorkPlace, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearValidControl(Control control)
        {
            try
            {
                if (control != null)
                    dxValidationProvider1.SetValidationRule(control, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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

        private void CboTreatmentEndTypExt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                lblPPDT.AppearanceItemCaption.ForeColor = Color.Black;
                ClearValidControl(this.txtSickUserName);
                ClearValidControl(this.txtCodeWorkPlace);
                ClearValidControl(this.DtRestTimeFrom);
                ClearValidControl(this.DtRestTimeTo);
                ClearValidControl(this.txtMaBHXH);

                this.lciProvider.AppearanceItemCaption.ForeColor = Color.Black;
                this.lciWorkPlace.AppearanceItemCaption.ForeColor = Color.Black;
                this.LciRestTimeFrom.AppearanceItemCaption.ForeColor = Color.Black;
                this.LciRestTimeTo.AppearanceItemCaption.ForeColor = Color.Black;
                this.lciMaBHXH.AppearanceItemCaption.ForeColor = Color.Black;

                lciCheckTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lciPregnancyTerminationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (CboTreatmentEndTypExt.EditValue != null)
                {
                    EnableButton(true);
                    long typeId = Inventec.Common.TypeConvert.Parse.ToInt64(CboTreatmentEndTypExt.EditValue.ToString());
                    lciCboDocumentBookId.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                    if (typeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                    {
                        lciCboDocumentBookId.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                        ValidationBHXH(this.txtMaBHXH, 10, 10);
                    }
                    else if (typeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM || typeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        if (xml2076ExportOption == "1")
                        {
                            ValidateSickUser();
                            ValidateWorkPlace();
                            ValidateTime();
                        }
                        ValidationBHXH(this.txtMaBHXH, 10, 10, true);
                        this.lciMaBHXH.AppearanceItemCaption.ForeColor = Color.Maroon;
                    }

                    LciInfantInformation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    LciGridControl.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                    if (typeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                    {
                        if (this.hisTreatment.TDL_PATIENT_GENDER_ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                        {
                            LciInfantInformation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                            LciGridControl.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        }
                        else
                        {
                            MessageBox.Show("Giới tính không phù hợp");
                            CboTreatmentEndTypExt.Focus();
                            CboTreatmentEndTypExt.ShowPopup();
                        }
                    }
                    if (typeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                    {
                        lciCheckTT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciIsPregnancyTermination.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciGestationalAge.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciPregnancyTerminationReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                        lciPregnancyTerminationTime.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;

                        lblPPDT.AppearanceItemCaption.ForeColor = Color.Maroon;
                        ValidationRequired(txtTreatmentMethod);
                    }
                }
                else
                {
                    EnableButton(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateTime()
        {
            try
            {
                this.LciRestTimeFrom.AppearanceItemCaption.ForeColor = Color.Maroon;
                Validation.ValidateTimeFromAndTo validTimeFrom = new Validation.ValidateTimeFromAndTo();
                validTimeFrom.dtTimeFrom = DtRestTimeFrom;
                validTimeFrom.dtTimeTo = DtRestTimeTo;
                validTimeFrom.ErrorType = ErrorType.Warning;
                validTimeFrom.ErrorText = "Trường dữ liệu bắt buộc";
                dxValidationProvider1.SetValidationRule(DtRestTimeFrom, validTimeFrom);

                this.LciRestTimeTo.AppearanceItemCaption.ForeColor = Color.Maroon;
                ControlEditValidationRule validTimeTo = new ControlEditValidationRule();
                validTimeTo.editor = DtRestTimeTo;
                validTimeTo.ErrorType = ErrorType.Warning;
                validTimeTo.ErrorText = "Trường dữ liệu bắt buộc";
                dxValidationProvider1.SetValidationRule(DtRestTimeTo, validTimeTo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentEndTypExt_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciCboDocumentBookId.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        cboDocumentBookId.Focus();
                    else
                    {
                        SpLeaveDay.Focus();
                        SpLeaveDay.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentEndTypExt_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboTreatmentEndTypExt.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboTreatmentEndTypExt_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (lciCboDocumentBookId.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        cboDocumentBookId.Focus();
                    else
                    {
                        SpLeaveDay.Focus();
                        SpLeaveDay.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableButton(bool enable)
        {
            try
            {
                BtnSave.Enabled = enable;
                BtnPrint.Enabled = enable;
                BtnSaveNPrint.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EditCaption()
        {
            try
            {
                string age = AgeUtil.CalculateFullAge(this.hisTreatment.TDL_PATIENT_DOB);
                long Age = Inventec.Common.TypeConvert.Parse.ToInt64(age ?? "");
                if (Age < 7)
                {
                    LciRelativeType.Text = "QH với BN:";
                    LciRelativeType.OptionsToolTip.ToolTip = "Quan hệ với bệnh nhân";
                    lciWorkPlace.Text = "Nơi làm việc:";
                    lciWorkPlace.OptionsToolTip.ToolTip = "Nơi làm việc của bệnh nhân";

                    LciRelativeName.AppearanceItemCaption.ForeColor = Color.Maroon;
                    LciRelativeType.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    LciRelativeType.Text = "Quan hệ:";
                    lciWorkPlace.Text = "Nơi làm việc:";
                    LciRelativeType.OptionsToolTip.ToolTip = "";
                    lciWorkPlace.OptionsToolTip.ToolTip = "";
                    LciRelativeName.AppearanceItemCaption.ForeColor = Color.Black;
                    LciRelativeType.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboRelativeType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    CboRelativeType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string ConvertString(string str)
        {
            char[] ch = str.ToCharArray();
            try
            {

                for (int i = 0; i < str.Length; i++)
                {
                    if (i == 0 && ch[i] != ' ' ||
                        ch[i] != ' ' && ch[i - 1] == ' ')
                    {
                        if (ch[i] >= 'a' && ch[i] <= 'z')
                        {
                            ch[i] = (char)(ch[i] - 'a' + 'A');
                        }
                    }
                    else if (ch[i] >= 'A' && ch[i] <= 'Z')

                        ch[i] = (char)(ch[i] + 'a' - 'A');
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            String st = new String(ch);

            return st;
        }

        private void InitWorkPlaceControl()
        {
            try
            {
                var workPlace = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.IS_ACTIVE == (short)1).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORK_PLACE_CODE", "", 70, 1));
                columnInfos.Add(new ColumnInfo("WORK_PLACE_NAME", "", 180, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORK_PLACE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboWorkPlace, workPlace, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DtRestTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DtRestTimeTo.Focus();
                    DtRestTimeTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void txtSickUserName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool showCbo = true;
                    if (!String.IsNullOrEmpty(txtSickUserName.Text.Trim()))
                    {
                        string code = txtSickUserName.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<ACS_USER>().Where(o => o.LOGINNAME.ToLower().Contains(code)).ToList();
                        var result = listData != null ? (listData.Count > 0 ? listData.Where(o => o.LOGINNAME.ToLower() == code).ToList() : listData) : null;
                        if (result != null && result.Count == 1)
                        {
                            showCbo = false;
                            txtSickUserName.Text = result.First().LOGINNAME;
                            cboSickUserName.EditValue = result.First().LOGINNAME;
                            cboSickUserName.Properties.Buttons[1].Visible = true;
                            SpLeaveDay.Focus();
                            SpLeaveDay.SelectAll();
                        }
                    }
                    if (showCbo)
                    {
                        cboSickUserName.Focus();
                        cboSickUserName.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSickUserName_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboSickUserName.EditValue = null;
                    txtSickUserName.Text = "";
                    cboSickUserName.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSickUserName_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SpLeaveDay.Focus();
                    SpLeaveDay.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentBookId_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDocumentBookId.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtWorkPlace_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtHeinCardNumber.Focus();
                    txtHeinCardNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TxtRelativeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    CboRelativeType.Focus();
                    CboRelativeType.SelectAll();
                    if (CboRelativeType.Text == "")
                        CboRelativeType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboRelativeType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtCodeWorkPlace.Focus();
                    txtCodeWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtHeinCardNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtHeinCardNumber.Text))
                    {
                        string hein = txtHeinCardNumber.Text.Trim();
                        if (hein.Length == 10)
                        {
                            txtMaBHXH.Text = hein;
                        }
                        else if (hein.Length == 15)
                        {
                            txtMaBHXH.Text = hein.Substring(5, 10);
                        }
                    }
                    if (String.IsNullOrWhiteSpace(txtMaBHXH.Text))
                    {
                        txtMaBHXH.Focus();
                        txtMaBHXH.SelectAll();
                    }
                    else
                    {
                        if (lciIsPregnancyTermination.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        {
                            chkIsPregnancyTermination.Focus();
                        }
                        else
                        {

                            txtTreatmentMethod.Focus();
                            txtTreatmentMethod.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMaBHXH_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (lciIsPregnancyTermination.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        chkIsPregnancyTermination.Focus();
                    }
                    else
                    {
                        txtTreatmentMethod.Focus();
                        txtTreatmentMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocumentBookId_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSickUserName.Focus();
                    txtSickUserName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentMethod_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEndTypeExtNote.Focus();
                    txtEndTypeExtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSickUserName_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboSickUserName.EditValue == null)
                {
                    txtSickUserName.Text = "";
                    cboSickUserName.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboSickUserName.Properties.Buttons[1].Visible = true;
                    var acs = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == (short)1 && o.LOGINNAME == cboSickUserName.EditValue.ToString());
                    if (acs != null)
                    {
                        txtSickUserName.Text = acs.LOGINNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboWorkPlace.EditValue == null)
                {
                    txtCodeWorkPlace.Text = "";
                    cboWorkPlace.EditValue = null;
                    cboWorkPlace.Properties.Buttons[1].Visible = false;
                }
                else
                {
                    cboWorkPlace.Properties.Buttons[1].Visible = true;
                    var wp = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_WORK_PLACE>().FirstOrDefault(o => o.IS_ACTIVE == (short)1 && o.ID == Int32.Parse(cboWorkPlace.EditValue.ToString()));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => wp), wp));
                    if (wp != null)
                    {
                        txtCodeWorkPlace.Text = wp.WORK_PLACE_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCodeWorkPlace_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtCodeWorkPlace.Text))
                    {
                        string text = txtCodeWorkPlace.Text.Trim().ToLower();
                        var lstData = BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.IS_ACTIVE == (short)1 && o.WORK_PLACE_CODE.ToLower().Contains(text)).ToList();
                        if (lstData != null && lstData.Count == 1)
                        {
                            cboWorkPlace.EditValue = lstData[0].ID;
                            txtCodeWorkPlace.Text = lstData[0].WORK_PLACE_CODE;
                        }
                        else
                        {
                            cboWorkPlace.Focus();
                            cboWorkPlace.ShowPopup();
                        }
                    }
                    else
                    {
                        cboWorkPlace.Focus();
                        cboWorkPlace.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    txtWorkPlace.Focus();
                    txtWorkPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboWorkPlace_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboWorkPlace.EditValue = null;
                    txtCodeWorkPlace.Text = "";
                    cboWorkPlace.Properties.Buttons[1].Visible = false;
                    Inventec.Common.Logging.LogSystem.Error("CLICK DELETE");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboWorkPlace_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtWorkPlace.Focus();
                    txtWorkPlace.SelectAll();
                }
                else
                {
                    cboWorkPlace.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnInfantInformation_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.InfantInformation").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.InfantInformation'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("______________this.moduleData: ", moduleData));
                    List<object> listArgs = new List<object>();
                    listArgs.Add(TreatmentId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");
                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();
                    LoadDataToGrid();
                }
                else
                {
                    MessageManager.Show("Chức năng chưa được hỗ trợ trong phiên bản này");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGestationalAge_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPregnancyTermination_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkIsPregnancyTermination.Checked)
                {
                    this.lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.lciPregnancyTerminationReason.Enabled = true;
                    this.lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.lciPregnancyTerminationTime.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.lciPregnancyTerminationTime.Enabled = true;
                    ValidMaxLengthRequired(true, txtPregnancyTerminationReason, 1000, "Bắt buộc nhập thông tin lý do đình chỉ trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, txtGestationalAge, 0, "Bắt buộc nhập thông tin tuổi thai trong trường hợp đình chỉ thai nghén");
                    ValidMaxLengthRequired(true, dtPregnancyTerminationTime, 0, "Bắt buộc nhập thông tin thời gian đình chỉ trong trường hợp đình chỉ thai nghén");
                }
                else
                {
                    this.lciGestationalAge.AppearanceItemCaption.ForeColor = Color.Black;
                    this.txtPregnancyTerminationReason.Text = "";
                    this.lciPregnancyTerminationReason.Enabled = false;
                    this.lciPregnancyTerminationReason.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dtPregnancyTerminationTime.EditValue = null;
                    this.lciPregnancyTerminationTime.Enabled = false;
                    this.lciPregnancyTerminationTime.AppearanceItemCaption.ForeColor = Color.Black;
                    dxValidationProvider1.SetValidationRule(txtGestationalAge, null);
                    dxValidationProvider1.SetValidationRule(txtPregnancyTerminationReason, null);
                    dxValidationProvider1.SetValidationRule(dtPregnancyTerminationTime, null);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGestationalAge_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPregnancyTerminationReason.Enabled)
                    {
                        txtPregnancyTerminationReason.Focus();
                        txtPregnancyTerminationReason.SelectAll();
                    }
                    else
                    {

                        txtTreatmentMethod.Focus();
                        txtTreatmentMethod.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtEndTypeExtNote_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    BtnSave.Focus();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkIsPregnancyTermination_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.lciPregnancyTerminationReason.Enabled)
                    {
                        dtPregnancyTerminationTime.Focus();
                        dtPregnancyTerminationTime.SelectAll();
                        dtPregnancyTerminationTime.ShowPopup();
                    }
                    else
                    {
                        txtGestationalAge.Focus();
                        txtGestationalAge.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtPregnancyTerminationTime_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGestationalAge.Focus();
                    txtGestationalAge.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheckTT_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.hisTreatment == null || !dxValidationProvider1.Validate(txtMaBHXH)) return;
                MOS.Filter.HisPatientFilter patientFilter = new MOS.Filter.HisPatientFilter();
                patientFilter.ID = this.hisTreatment.PATIENT_ID;
                HIS_PATIENT patient = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null).FirstOrDefault();
                if (patient == null) return;
                CheckBhxh(patient);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private async Task CheckBhxh(HIS_PATIENT hisPatient)
        {
            ResultHistoryLDO rsData = null;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên cán bộ:{0}", nameCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("CCCD cán bộ:{0}", cccdCb));
                Inventec.Common.Logging.LogSystem.Debug(String.Format("Tên api:{0}", api));

                BHXHLoginCFG.LoadConfig();
                CommonParam param = new CommonParam();
                ApiInsuranceExpertise apiInsuranceExpertise = new ApiInsuranceExpertise();
                apiInsuranceExpertise.ApiEgw = api;
                CheckHistoryLDO checkHistoryLDO = new CheckHistoryLDO();
                checkHistoryLDO.maThe = txtMaBHXH.Text.Trim();
                checkHistoryLDO.ngaySinh = hisPatient.IS_HAS_NOT_DAY_DOB == 1 ? hisPatient.DOB.ToString().Substring(0, 4) : ((Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(hisPatient.DOB) ?? DateTime.MinValue).ToString("dd/MM/yyyy"));
                checkHistoryLDO.hoTen = Inventec.Common.String.Convert.HexToUTF8Fix(hisPatient.VIR_PATIENT_NAME.ToLower());
                checkHistoryLDO.hoTen = (String.IsNullOrEmpty(checkHistoryLDO.hoTen) ? hisPatient.VIR_PATIENT_NAME.ToLower() : checkHistoryLDO.hoTen);
                checkHistoryLDO.cccdCb = cccdCb;
                checkHistoryLDO.hoTenCb = nameCb;
                Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 1");
                if (!string.IsNullOrEmpty(BHXHLoginCFG.USERNAME)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.PASSWORD)
                    || !string.IsNullOrEmpty(BHXHLoginCFG.ADDRESS))
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checkHistoryLDO), checkHistoryLDO));
                    rsData = await apiInsuranceExpertise.CheckHistory(BHXHLoginCFG.USERNAME, BHXHLoginCFG.PASSWORD, BHXHLoginCFG.ADDRESS, checkHistoryLDO, BHXHLoginCFG.ADDRESS_OPTION);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rsData), rsData));
                    if (rsData != null)
                    {
                        if (rsData.maKetQua == "000" || rsData.maKetQua == "001" || rsData.maKetQua == "002" || rsData.maKetQua == "004")
                        {
                            txtHeinCardNumber.Text = rsData.maThe;
                            if (!string.IsNullOrEmpty(rsData.ngaySinh) && checkHistoryLDO.ngaySinh != rsData.ngaySinh && hisPatient != null && DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn cập nhật lại ngày sinh của bệnh nhân không?", "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                WaitingManager.Show();
                                var splDob = rsData.ngaySinh.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (splDob.Count > 2)
                                {
                                    hisPatient.DOB = Int64.Parse(splDob[2] + splDob[1] + splDob[0] + "000000");
                                    hisPatient.IS_HAS_NOT_DAY_DOB = null;
                                }
                                else
                                {
                                    hisPatient.DOB = Int64.Parse(splDob[0] + "0101000000");
                                    hisPatient.IS_HAS_NOT_DAY_DOB = 1;

                                }
                                HisPatientUpdateSDO sdo = new HisPatientUpdateSDO();
                                sdo.HisPatient = hisPatient;
                                sdo.TreatmentId = hisTreatment.ID;
                                var resultData = new BackendAdapter(param).Post<HIS_PATIENT>("api/HisPatient/UpdateSdo", ApiConsumers.MosConsumer, sdo, param);
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, resultData != null);
                            }
                        }
                        else if (!string.IsNullOrEmpty(rsData.ghiChu))
                        {
                            DevExpress.XtraEditors.XtraMessageBox.Show(rsData.ghiChu, "Thông báo");
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Error("Kiem tra lai cau hinh 'HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS'  -- 'HIS.CHECK_HEIN_CARD.BHXH__ADDRESS' ==>BHYT");
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

