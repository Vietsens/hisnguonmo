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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.RegisterV3.ADO;
using HIS.Desktop.Plugins.RegisterV3.RunV3.UC;
using HIS.Desktop.Utility;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.KskContract;
using HIS.UC.ServiceRoom;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.UCTransPati.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        #region Delcare

        public delegate void ShowFormPopup();
        RoomVaccine ucRoomVaccine;
        RoomVitaminA ucRoomVitaminA;
        UCAddressADO dataAddressPatient = new UCAddressADO();
        UCPatientRawADO dataPatientRaw = new UCPatientRawADO();
        int actionType = 0;
        internal HisPatientSDO currentPatientSDO { get; set; }
        internal HisCardSDO cardSearch = null;
        internal UCTransPatiADO transPatiADO = null;
        Inventec.Common.QrCodeBHYT.HeinCardData qrCodeBHYTHeinCardData;
        internal HeinCardData _HeinCardData { get; set; }
        RoomExamServiceProcessor roomExamServiceProcessor;
        UserControl ucRoomExamService = null;
        internal int registerNumber = 0;
        internal List<ServiceReqDetailSDO> serviceReqDetailSDOs;
        internal bool isShowMess;
        internal List<long> serviceReqPrintIds { get; set; }
        internal bool isNotPatientDayDob = false;
        internal bool isPrintNow;
        bool isReadQrCode = true;
        string appointmentCode = "";
        string autoCheckIcd { get; set; }
        bool IsObligatoryTranferMediOrg { get; set; }
        internal long PatientTypeIdBHYT { get; set; }
        bool ValidatedTTCT { get; set; }
        bool IsPresent { get; set; }
        internal long _TreatmnetIdByAppointmentCode = 0;
        internal bool _isPatientAppointmentCode = false;
        bool isResetForm = false;
        List<CaseTypeADO> listCaseTypeAdos;

        internal frmTransPati frm;

        HisPatientProfileSDO resultHisPatientProfileSDO = null;

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;

        internal KskContractProcessor kskContractProcessor;
        internal UserControl ucKskContract;

        bool isNotLoadWhileChangeControlStateInFirst = false;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string modulLink = "HIS.Desktop.Plugins.RegisterV3";
        #endregion

        #region Construct - Load
        public UCRegister(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 1");
                HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.LoadConfig();
                HisConfigCFG.LoadConfig();
                InitializeComponent();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 1");
               
                this.ConfigLayout();
                this.currentModule = module;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCRegister_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 2");
                this.FocusNextUserControl();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister => 3");
                isNotLoadWhileChangeControlStateInFirst = true;
                chkVitaminA.Checked = true;

                //Ẩn combo patient Type với tiếp đón 3
                this.ucPatientRaw1.SetTD3(true);

                this.ucPatientRaw1.RefreshUserControl();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 2");
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 3");
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 5");
                this.SetCurrentModuleForUCWorkPlace();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 6");
                MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientTypeDefault = AppConfigs.PatientTypeDefault;
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 7");
                this.ucPatientRaw1.FocusUserControl();
                this.actionType = GlobalVariables.ActionAdd;
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 8");
                this.FillDataDefaultToControl();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 9");
                this.SetDefaultCashierRoom();
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 10");
                this.autoCheckIcd = HisConfigCFG.AutoCheckIcd;
                this.IsObligatoryTranferMediOrg = HisConfigCFG.IsObligatoryTranferMediOrg;
                this.PatientTypeIdBHYT = HisConfigCFG.PatientTypeId__BHYT;
                Inventec.Common.Logging.LogSystem.Debug("UCRegister_Load => 11");

                InitControlState();
                this.DHSTLoadDataDefault();
                isNotLoadWhileChangeControlStateInFirst = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private Method
        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(modulLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkPhongTiem.Name)
                        {
                            chkPhongTiem.Checked = item.VALUE == "1";
                        }

                        if (item.KEY == chkVitaminA.Name)
                        {
                            chkVitaminA.Checked = item.VALUE == "1";
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetDeaultPatientType()
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE patientType = null;
            if (!String.IsNullOrEmpty(AppConfigs.PatientTypeCodeDefault))
            {
                patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == AppConfigs.PatientTypeCodeDefault);
                if (patientType == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Phan mem HIS da duoc cau hinh doi tuong benh nhan mac dinh, tuy nhien ma doi tuong cau hinh khong ton tai trong danh muc doi tuong benh nhan, he thong tu dong lay doi tuong mac dinh la doi tuong BHYT. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => AppConfigs.PatientTypeCodeDefault), AppConfigs.PatientTypeCodeDefault));
                    patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
                }
            }
            else
            {
                patientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == HisConfigCFG.PatientTypeCode__BHYT);
            }
            return patientType;
        }

        bool GetIsChild()
        {
            bool valid = false;
            try
            {
                valid = ucPatientRaw1.GetIsChild();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void FocusInKskContract()
        {
            try
            {
                if (this.ucKskContract != null && this.kskContractProcessor != null)
                {
                    this.kskContractProcessor.InFocus(this.ucKskContract);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ConfigLayout()
        {
            try
            {
                layoutControl1.BeginUpdate();
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions[i].Width = (100 / 6);
                }

                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 1");
                for (int i = 0; i < layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].SizeType = SizeType.Percent;
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions[i].Height = (100 / 14);
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 2");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Count; i < 6; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.ColumnDefinitions.Add(
                        new ColumnDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Width = (100 / 6)
                        });
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 3");
                for (int i = layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Count; i < 14; i++)
                {
                    layoutControl1.Root.OptionsTableLayoutGroup.RowDefinitions.Add(
                        new RowDefinition()
                        {
                            SizeType = SizeType.Percent,
                            Height = (100 / 14)
                        });
                }
                layoutControl1.EndUpdate();
                Inventec.Common.Logging.LogSystem.Debug("ConfigLayout - 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long GetPatientTypeId()
        {
            try
            {
                return ucPatientRaw1.GetValue().PATIENTTYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return 0;
        }

        private void FillDataAfterSearchPatientInUCPatientRaw(object data)
        {
            try
            {
                if (data != null)
                {
                    string heinCardNumber = "";
                    DataResultADO dataResult = (DataResultADO)data;
                    this.SetValueVariableUCAddressCombo(dataResult);
                    if (dataResult.OldPatient == false && dataResult.UCRelativeADO != null)
                        FillDataIntoUCRelativeInfo(dataResult.UCRelativeADO);
                    else if (dataResult.OldPatient == false && dataResult.HeinCardData != null)
                    {
                        this.FillDataAfterSaerchPatientInUCPatientRaw(dataResult.HeinCardData);
                        heinCardNumber = dataResult.HeinCardData.HeinCardNumber;
                    }
                    else if (dataResult.HisPatientSDO != null)
                    {
                        this._isPatientAppointmentCode = (dataResult.SearchTypePatient == 2);
                        if (!String.IsNullOrEmpty(dataResult.AppointmentCode))
                            this.appointmentCode = dataResult.AppointmentCode;
                        this._TreatmnetIdByAppointmentCode = (dataResult.TreatmnetIdByAppointmentCode == null ? 0 : dataResult.TreatmnetIdByAppointmentCode);
                        this.currentPatientSDO = dataResult.HisPatientSDO;
                        FillDataIntoUCPlusInfo(currentPatientSDO);
                        FillDataIntoUCRelativeInfo(currentPatientSDO);
                        FillDataIntoUCAddressInfo(currentPatientSDO);
                        FillDataToGridVitaminA(currentPatientSDO);
                        FillDataToGridVaccine(currentPatientSDO);
                        //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)//xuandv
                        HeinCardData dataCheck = new HeinCardData();
                        dataCheck.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.DOB);
                        dataCheck.PatientName = currentPatientSDO.VIR_PATIENT_NAME;
                        dataCheck.HeinCardNumber = currentPatientSDO.HeinCardNumber;
                        dataCheck.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(currentPatientSDO.GENDER_ID.ToString());
                        dataCheck.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardFromTime ?? 0);
                        dataCheck.MediOrgCode = currentPatientSDO.HeinMediOrgCode;
                        dataCheck.Address = currentPatientSDO.HeinAddress;
                        dataCheck.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientSDO.HeinCardToTime ?? 0);
                        heinCardNumber = currentPatientSDO.HeinCardNumber;
                    }
                    this.SetPatientSearchPanel(dataResult.OldPatient);
                    this.cardSearch = dataResult.HisCardSDO;

                    if (!String.IsNullOrEmpty(heinCardNumber))
                        this.FillDataCareerUnder6AgeByHeinCardNumber(heinCardNumber);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridVitaminA(HisPatientSDO patient)
        {
            try
            {
                if (patient != null)
                {
                    gridControlVitaminA.BeginUpdate();
                    gridControlVitaminA.DataSource = GetVitaminA(patient.ID);
                    gridControlVitaminA.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridVitaminA(HisPatientVitaminASDO patient)
        {
            try
            {
                if (patient != null)
                {
                    gridControlVitaminA.BeginUpdate();
                    gridControlVitaminA.DataSource = GetVitaminA(patient.HisPatient.ID);
                    gridControlVitaminA.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridVaccine(HisPatientSDO patient)
        {
            try
            {
                if (patient != null)
                {
                    gridControlTiem.BeginUpdate();
                    gridControlTiem.DataSource = GetVaccine(patient.ID);
                    gridControlTiem.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridVaccine(HisPatientVitaminASDO patient)
        {
            try
            {
                if (patient != null)
                {
                    gridControlTiem.BeginUpdate();
                    gridControlTiem.DataSource = GetVaccine(patient.HisPatient.ID);
                    gridControlTiem.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<V_HIS_VITAMIN_A> GetVitaminA(long patientId)
        {
            List<V_HIS_VITAMIN_A> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisVitaminAViewFilter filter = new HisVitaminAViewFilter();
                filter.PATIENT_ID = patientId;
                filter.VITAMIN_STT = HisVitaminAViewFilter.VITAMIN_STT_ENUM.DRINK;
                rs = new BackendAdapter(param).Get<List<V_HIS_VITAMIN_A>>("api/HisVitaminA/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private List<V_HIS_EXP_MEST_MEDICINE_5> GetVaccine(long patientId)
        {
            List<V_HIS_EXP_MEST_MEDICINE_5> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisVaccinationFilter filter = new HisVaccinationFilter();
                filter.PATIENT_ID = patientId;
                List<HIS_VACCINATION> vaccineRs = new BackendAdapter(param).Get<List<HIS_VACCINATION>>("api/HisVaccination/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (vaccineRs != null && vaccineRs.Count > 0)
                {
                    HisExpMestMedicineView5Filter expMedi5Filter = new HisExpMestMedicineView5Filter();
                    expMedi5Filter.TDL_VACCINATION_IDs = vaccineRs.Select(s => s.ID).ToList();
                    rs = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE_5>>("api/HisExpMestMedicine/GetView5", ApiConsumer.ApiConsumers.MosConsumer, expMedi5Filter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return rs;
        }

        private HIS_CAREER GetCareerByBhytWhiteListConfig(string heinCardNumder)
        {
            HIS_CAREER result = null;
            try
            {
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => !String.IsNullOrEmpty(heinCardNumder) && o.BHYT_WHITELIST_CODE.ToUpper() == heinCardNumder.Substring(0, 3).ToUpper());
                    if (bhytWhiteList != null && (bhytWhiteList.CAREER_ID ?? 0) > 0)
                    {
                        result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == bhytWhiteList.CAREER_ID.Value);
                        if (result == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("GetCareerByBhytWhiteListConfig => Khong lay duoc nghe nghiep theo id = " + bhytWhiteList.CAREER_ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FillDataCareerUnder6AgeByHeinCardNumber(string heinCardNumder)
        {
            try
            {
                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                //27/10/2017 => sửa lại => Căn cứ vào đầu thẻ BHYT và dữ liệu cấu hình trong bảng HIS_BHYT_WHITELIST để tự động điền nghề nghiệp tương ứng
                var patientRawObj = this.ucPatientRaw1.GetValue();
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(heinCardNumder);
                if (career == null)
                {
                    if (patientRawObj.DOB > 0)
                    {
                        DateTime dtDob = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientRawObj.DOB).Value;
                        if (dtDob != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDob))
                        {
                            career = HisConfigCFG.CareerUnder6Age;
                        }
                        else if (DateTime.Now.Year - dtDob.Year <= 18)
                        {
                            career = HisConfigCFG.CareerHS;
                        }
                        else
                        {
                            career = HisConfigCFG.CareerBase;
                        }
                    }
                    else
                    {
                        career = HisConfigCFG.CareerBase;
                    }
                }
                if (career != null && career.ID > 0)
                {
                    patientRawObj.CARRER_ID = career.ID;
                    patientRawObj.CARRER_CODE = career.CAREER_CODE;
                    patientRawObj.CARRER_NAME = career.CAREER_NAME;
                    this.ucPatientRaw1.SetValue(patientRawObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueVariableUCAddressCombo(DataResultADO data)
        {
            try
            {
                if (data.OldPatient == true)
                    this.ucAddressCombo1.isPatientBHYT = true;
                else
                    this.ucAddressCombo1.isPatientBHYT = false;
                if (data.HeinCardData != null)
                    this.ucAddressCombo1.isReadCard = true;
                else
                    this.ucAddressCombo1.isReadCard = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Event Control
        private void btnNewContinue_Click(object sender, EventArgs e)
        {
            try
            {
                this.isNotLoadWhileChangeControlStateInFirst = true;
                this.isResetForm = true;
                this.RefreshUserControl();
                this.isResetForm = false;
                this.gridControlTiem.DataSource = null;
                this.gridControlVitaminA.DataSource = null;
                this.DHSTLoadDataDefault();
                this.InitControlState();
                this.isNotLoadWhileChangeControlStateInFirst = false;
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
                Save(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Save(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                this.isPrintNow = false;
                this.Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnDepositDetail_Click(object sender, EventArgs e)
        {
            try
            {
                this.DepositDetail();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            try
            {
                this.Bill();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                this.TreatmentBedRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndAssain_Click(object sender, EventArgs e)
        {
            try
            {
                this.SaveAndAssain();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.ReCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPatientNew_Click(object sender, EventArgs e)
        {
            try
            {
                this.GetDataBeforeClickbtnPatientNew();
                this.RefreshUserControl();
                this.dataPatientRaw.PATIENT_CODE = "";
                this.dataPatientRaw.CARRER_ID = null;
                this.dataPatientRaw.PATIENT_CODE = "";
                this.ucPatientRaw1.SetValue(dataPatientRaw);
                this.SetPatientSearchPanel(false);
                this.ucPatientRaw1.FocusToPatientType();
                this.chkPhongTiem.Checked = false;
                this.chkVitaminA.Checked = false;
                this.gridControlTiem.DataSource = null;
                this.gridControlVitaminA.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void chkPhongTiem_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkPhongTiem.Checked)
                {
                    ucRoomVitaminA = null;
                    chkVitaminA.Checked = false;
                    InitUcRoomVaccine();
                }
                else
                {
                    ucRoomVaccine = null;
                    chkVitaminA.Checked = true;
                    InitUcRoomVitaminA();
                }

                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPhongTiem.Name && o.MODULE_LINK == modulLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPhongTiem.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPhongTiem.Name;
                    csAddOrUpdate.VALUE = (chkPhongTiem.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = modulLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVitaminA_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkVitaminA.Checked)
                {
                    ucRoomVaccine = null;
                    chkPhongTiem.Checked = false;
                    InitUcRoomVitaminA();
                }
                else
                {
                    ucRoomVitaminA = null;
                    chkPhongTiem.Checked = true;
                    InitUcRoomVaccine();
                }

                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkVitaminA.Name && o.MODULE_LINK == modulLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkVitaminA.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkVitaminA.Name;
                    csAddOrUpdate.VALUE = (chkVitaminA.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = modulLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPhongTiem_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkVitaminA.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkVitaminA_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (ucRoomVitaminA != null)
                    {
                        ucRoomVitaminA.dtTime.Focus();
                        ucRoomVitaminA.dtTime.ShowPopup();
                    }
                    else if (ucRoomVaccine != null)
                    {
                        ucRoomVaccine.txtPatientType.Focus();
                        ucRoomVaccine.txtPatientType.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewVitaminA_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A data = (MOS.EFMODEL.DataModels.V_HIS_VITAMIN_A)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTiem_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_EXP_MEST_MEDICINE_5 data = (V_HIS_EXP_MEST_MEDICINE_5)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXECUTE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXECUTE_TIME ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExecuteTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPulse.Focus();
                    spinPulse.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPulse_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMax.Focus();
                    spinBloodPressureMax.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMax_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBloodPressureMin.Focus();
                    spinBloodPressureMin.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBloodPressureMin_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinWeight.Focus();
                    spinWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinHeight.Focus();
                    spinHeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinWeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinHeight_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                DHSTFillDataToBmiAndLeatherArea();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinSPO2.Focus();
                    spinSPO2.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinSPO2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTemperature.Focus();
                    spinTemperature.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTemperature_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBreathRate.Focus();
                    spinBreathRate.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBreathRate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChest.Focus();
                    spinChest.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinChest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBelly.Focus();
                    spinBelly.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBelly_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnConnectBloodPressure_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_DHST data = HIS.Desktop.Plugins.Library.ConnectBloodPressure.ConnectBloodPressureProcessor.GetData();
                if (data != null)
                {
                    if (data.EXECUTE_TIME != null)
                        dtExecuteTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.EXECUTE_TIME ?? 0) ?? DateTime.Now;
                    else
                        dtExecuteTime.EditValue = DateTime.Now;

                    if (data.BLOOD_PRESSURE_MAX.HasValue)
                    {
                        spinBloodPressureMax.EditValue = data.BLOOD_PRESSURE_MAX;
                    }
                    if (data.BLOOD_PRESSURE_MIN.HasValue)
                    {
                        spinBloodPressureMin.EditValue = data.BLOOD_PRESSURE_MIN;
                    }
                    if (data.BREATH_RATE.HasValue)
                    {
                        spinBreathRate.EditValue = data.BREATH_RATE;
                    }
                    if (data.HEIGHT.HasValue)
                    {
                        spinHeight.EditValue = data.HEIGHT;
                    }
                    if (data.CHEST.HasValue)
                    {
                        spinChest.EditValue = data.CHEST;
                    }
                    if (data.BELLY.HasValue)
                    {
                        spinBelly.EditValue = data.BELLY;
                    }
                    if (data.PULSE.HasValue)
                    {
                        spinPulse.EditValue = data.PULSE;
                    }
                    if (data.TEMPERATURE.HasValue)
                    {
                        spinTemperature.EditValue = data.TEMPERATURE;
                    }
                    if (data.WEIGHT.HasValue)
                    {
                        spinWeight.EditValue = data.WEIGHT;
                    }
                    if (!String.IsNullOrWhiteSpace(data.NOTE))
                    {
                        txtNote.Text = data.NOTE;
                    }
                    if (data.SPO2.HasValue)
                        spinSPO2.Value = (data.SPO2.Value * 100);
                    else
                        spinSPO2.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
