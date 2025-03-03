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
using AutoMapper;
using HID.EFMODEL.DataModels;
using HID.Filter;
using His.UC.UCHein;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.HisSyncToHid;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.ADO;
using HIS.Desktop.Plugins.Register.Run;
using HIS.UC.KskContract.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Register
{
    abstract class ServiceRequestRegisterBehaviorBase : HIS.Desktop.Common.BusinessBase
    {
        protected string provinceCodeKS { get; set; }
        protected string provinceNameKS { get; set; }
        protected string patientName { get; set; }
        protected string address { get; set; }
        protected long? careerId { get; set; }
        protected string careerCode { get; set; }
        protected string careerName { get; set; }
        protected long? cMNDDate { get; set; }
        protected string cMNDNumber { get; set; }
        protected string cMNDPlace { get; set; }
        protected string communeCode { get; set; }
        protected string districtCode { get; set; }
        protected string provinceCode { get; set; }
        protected string communeName { get; set; }
        protected string districtName { get; set; }
        protected string provinceName { get; set; }
        protected long dob { get; set; }
        protected string email { get; set; }
        protected string ethnicName { get; set; }
        protected string ethnicCode { get; set; }
        protected long GenderId { get; set; }
        protected string nationalName { get; set; }
        protected string nationalCode { get; set; }
        protected string mpsNationalCode { get; set; }
        protected string phone { get; set; }
        protected string relativeAddress { get; set; }
        protected string relativeName { get; set; }
        protected string relativeType { get; set; }
        protected string relativeCMNDNumber { get; set; }
        protected string religionName { get; set; }
        protected object workPlace { get; set; }
        protected long patientTypeId { get; set; }
        protected long intructionTime { get; set; }
        protected long militaryId { get; set; }
        protected long emergencyWTimeId { get; set; }
        protected bool chkEmergency { get; set; }
        protected long programId { get; set; }
        protected string appointmentCode { get; set; }
        protected string codeFind { get; set; }
        protected string typeCodeFind__MaBN { get; set; }
        protected string typeCodeFind__MaHK { get; set; }
        protected string typeCodeFind__MaCT { get; set; }

        protected string typeCodeFind__CCCDCMND { get; set; }
        protected string typeCodeFind__SoThe { get; set; }
        protected string typeCodeFind__MaNV { get; set; }
        protected long patientId { get; set; }
        protected bool isNotPatientDayDob { get; set; }
        protected long departmentId { get; set; }
        protected long treatmentTypeId { get; set; }
        protected long oweTypeId { get; set; }
        protected bool chkChronic { get; set; }
        protected string blood_Code { get; set; }
        protected string blood_Rh_Code { get; set; }
        protected HisCardSDO cardSearch { get; set; }
        protected PatientInformationADO patientInformationADO { get; set; }
        protected bool isReadQrCccdData { get; set; }
        protected string CccdReadFromQr { get; set; }
        protected string communeNowCode { get; set; }
        protected string communeNowName { get; set; }
        protected string provinceNowCode { get; set; }
        protected string provinceNowName { get; set; }
        protected string districtNowCode { get; set; }
        protected string districtNowName { get; set; }
        protected long? dateReleaseFromQr { get; set; }
        protected HisPatientSDO patientData { get; set; }
        protected HisPatientProfileSDO patientProfile { get; set; }
        protected MainHisHeinBhyt uCMainHein { get; set; }
        protected UserControl ucHein__BHYT { get; set; }
        protected Module currentModule { get; set; }
        protected UCRegister ucRequestService;
        protected long? kskContractId { get; set; }
        protected string hrmEmployeeCode { get; set; }
        protected string hrmKskCode { get; set; }
        protected long? receptionForm { get; set; }
        protected long? patientClassifyId { get; set; }
        protected string Note { get; set; }
        protected bool IsWarningForNext { get; set; }
        protected string HospitalizeReasonCode { get; set; }
        protected string HospitalizeReasonName { get; set; }
        protected string HospitalizeReason { get; set; }
        internal ServiceRequestRegisterBehaviorBase(CommonParam param, UCRegister ucServiceRequestRegiter)
            : base(param)
        {
            try
            {
                this.ucRequestService = ucServiceRequestRegiter;
                this.patientName = ucServiceRequestRegiter.txtPatientName.Text.Trim();
                this.currentModule = ucServiceRequestRegiter.currentModule;
                this.address = ucServiceRequestRegiter.txtAddress.Text.Trim();
                if (ucServiceRequestRegiter.cboCareer.EditValue != null)
                {
                    this.careerId = (long)(ucServiceRequestRegiter.cboCareer.EditValue);
                    this.careerCode = ucServiceRequestRegiter.txtCareerCode.Text;
                    this.careerName = ucServiceRequestRegiter.cboCareer.Text;
                }
                this.chkEmergency = ucServiceRequestRegiter.chkEmergency.Checked;
                this.HospitalizeReasonCode = ucServiceRequestRegiter.HospitalizeReasonCode;
                this.HospitalizeReasonName = ucServiceRequestRegiter.HospitalizeReasonName;
                this.HospitalizeReason = ucServiceRequestRegiter.HospitalizeReason;
                this.cMNDDate = null;
                this.cMNDNumber = "";
                this.cMNDPlace = "";
                this.email = "";
                this.religionName = "";
                if (ucServiceRequestRegiter.patientInformation != null)
                {
                    this.blood_Code = ucServiceRequestRegiter.patientInformation.BLOOD_ABO_CODE;
                    this.blood_Rh_Code = ucServiceRequestRegiter.patientInformation.BLOOD_RH_CODE;
                }
                this.communeName = (string)(ucServiceRequestRegiter.cboCommune.Text);
                this.communeCode = (string)(ucServiceRequestRegiter.cboCommune.EditValue);
                this.districtName = (string)(ucServiceRequestRegiter.cboDistrict.Text);
                this.districtCode = (string)(ucServiceRequestRegiter.cboDistrict.EditValue);
                this.provinceName = (string)(ucServiceRequestRegiter.cboProvince.Text);
                this.provinceCode = (string)(ucServiceRequestRegiter.cboProvince.EditValue);
                this.provinceNameKS = (string)(ucServiceRequestRegiter.cboProvinceKS.Text);
                this.provinceCodeKS = (string)(ucServiceRequestRegiter.cboProvinceKS.EditValue);
                this.dob = (long)(Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ucServiceRequestRegiter.dtPatientDob.DateTime));
                this.ethnicName = ucServiceRequestRegiter.cboEthnic.Text;
                this.ethnicCode = ucServiceRequestRegiter.txtEthnicCode.Text;
                if (ucServiceRequestRegiter.cboGender.EditValue != null)
                    this.GenderId = (long)(ucServiceRequestRegiter.cboGender.EditValue);
                this.nationalName = ucServiceRequestRegiter.cboNational.Text;
                this.nationalCode = ucServiceRequestRegiter.txtNationalCode.Text;
                this.mpsNationalCode = ucServiceRequestRegiter.mpsNationalCode;
                this.phone = ucServiceRequestRegiter.txtPhone.Text;
                this.relativeAddress = ucServiceRequestRegiter.txtRelativeAddress.Text.Trim();
                this.relativeName = ucServiceRequestRegiter.txtHomePerson.Text.Trim();
                this.relativeType = ucServiceRequestRegiter.txtCorrelated.Text.Trim();
                this.relativeCMNDNumber = ucRequestService.txtRelativeCMNDNumber.Text.Trim();
                this.workPlace = ucServiceRequestRegiter.workPlaceProcessor.GetValue(ucServiceRequestRegiter.ucWorkPlace, ucServiceRequestRegiter.workPlaceTemplate);

                this.patientTypeId = (long)(ucServiceRequestRegiter.cboPatientType.EditValue);
                if (ucServiceRequestRegiter.cboOweType.EditValue != null)
                    this.oweTypeId = (long)(ucServiceRequestRegiter.cboOweType.EditValue);
                if (ucServiceRequestRegiter.cboTreatmentType.EditValue != null)
                    this.treatmentTypeId = (long)(ucServiceRequestRegiter.cboTreatmentType.EditValue);

                ucServiceRequestRegiter.dtIntructionTime.EditValue = HIS.Desktop.Utility.DateTimeHelper.ConvertDateTimeStringToSystemTime(ucServiceRequestRegiter.txtIntructionTime.Text);
                ucServiceRequestRegiter.dtIntructionTime.Update();

                this.intructionTime = Inventec.Common.TypeConvert.Parse.ToInt64(ucServiceRequestRegiter.dtIntructionTime.DateTime.ToString("yyyyMMddHHmm") + "00");
                this.uCMainHein = ucServiceRequestRegiter.mainHeinProcessor;
                this.ucHein__BHYT = ucServiceRequestRegiter.ucHeinBHYT;
                this.codeFind = ucServiceRequestRegiter.typeCodeFind;
                this.typeCodeFind__MaBN = ucServiceRequestRegiter.typeCodeFind__MaBN;
                this.typeCodeFind__MaCT = ucServiceRequestRegiter.typeCodeFind__MaCT;
                this.typeCodeFind__MaHK = ucServiceRequestRegiter.typeCodeFind__MaHK;
                this.typeCodeFind__SoThe = ucServiceRequestRegiter.typeCodeFind__SoThe;
                this.typeCodeFind__MaNV = ucServiceRequestRegiter.typeCodeFind__MaNV;
                this.typeCodeFind__CCCDCMND = ucServiceRequestRegiter.typeCodeFind__CCCDCMND;
                this.programId = ucServiceRequestRegiter.programId;
                this.appointmentCode = ucServiceRequestRegiter.appointmentCode;
                if (ucServiceRequestRegiter.cboMilitaryRank.EditValue != null)
                    this.militaryId = (long)(ucServiceRequestRegiter.cboMilitaryRank.EditValue);
                this.departmentId = ucServiceRequestRegiter.departmentId;
                this.cardSearch = ucServiceRequestRegiter.cardSearch;
                this.patientId = (ucServiceRequestRegiter.currentPatientSDO != null ? ucServiceRequestRegiter.currentPatientSDO.ID : 0);

                if (ucServiceRequestRegiter.chkEmergency.Checked && ucServiceRequestRegiter.cboEmergencyTime.EditValue != null)
                    this.emergencyWTimeId = (long)(ucServiceRequestRegiter.cboEmergencyTime.EditValue);
                this.patientInformationADO = ucServiceRequestRegiter.patientInformation;
                this.patientData = ucServiceRequestRegiter.currentPatientSDO;
                this.isNotPatientDayDob = ucServiceRequestRegiter.isNotPatientDayDob;
                this.chkChronic = ucServiceRequestRegiter.chkIsChronic.Checked;//BN man tinh
                this.kskContractId = GetKskContractValue(ucServiceRequestRegiter);
                this.hrmEmployeeCode = this.codeFind == this.typeCodeFind__MaNV ? ucServiceRequestRegiter.txtPatientCode.Text : "";
                this.hrmKskCode = this.codeFind == this.typeCodeFind__MaNV ? ucServiceRequestRegiter.txtKskCode.Text : "";
                if (ucServiceRequestRegiter.cboPatientClassify.EditValue != null)
                    this.patientClassifyId = Inventec.Common.TypeConvert.Parse.ToInt64(ucServiceRequestRegiter.cboPatientClassify.EditValue.ToString());
                this.receptionForm = ucServiceRequestRegiter.typeReceptionForm;
                this.Note = ucServiceRequestRegiter.txtNote.Text.Trim();
                this.IsWarningForNext = ucServiceRequestRegiter.chkWNext.Checked;
                this.isReadQrCccdData = ucServiceRequestRegiter.isReadQrCccdData;
                this.dateReleaseFromQr = ucServiceRequestRegiter.dateReleaseFromQr;
                this.CccdReadFromQr = ucServiceRequestRegiter.CccdReadFromQr;
                this.communeNowCode = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtCommuneCode : null;
                this.communeNowName = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtCommuneName : null;
                this.provinceNowCode = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtProvinceCode : null;
                this.provinceNowName = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtProvinceName : null;
                this.districtNowCode = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtDistrictCode : null;
                this.districtNowName = ucServiceRequestRegiter.IsReadCardTheViet ? ucServiceRequestRegiter.HtDistrictName : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private long? GetKskContractValue(UCRegister ucServiceRequestRegiter)
        {
            long? result = null;
            try
            {
                if (ucServiceRequestRegiter.cboPatientType.EditValue != null)
                {
                    long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(ucServiceRequestRegiter.cboPatientType.EditValue.ToString());
                    if (patientTypeId == HisConfigCFG.PatientTypeId__KSK && ucServiceRequestRegiter.kskContractProcessor != null && ucServiceRequestRegiter.ucKskContract != null)
                    {
                        KskContractOutput kskContract = (KskContractOutput)ucServiceRequestRegiter.kskContractProcessor.GetValue(ucServiceRequestRegiter.ucKskContract);
                        if (kskContract != null && kskContract.IsVali && kskContract.KskContract != null)
                            result = kskContract.KskContract.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        protected void InitBase()
        {
            try
            {
                if (patientProfile == null)
                    patientProfile = new HisPatientProfileSDO();
                if (patientProfile.HisPatient == null)
                    patientProfile.HisPatient = new HIS_PATIENT();
                if (patientProfile.HisTreatment == null)
                    patientProfile.HisTreatment = new MOS.EFMODEL.DataModels.HIS_TREATMENT();

                if (patientProfile.HisPatientTypeAlter == null)
                    patientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();

                if (this.patientData != null)
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT>(patientProfile.HisPatient, this.patientData);

                if (this.currentModule != null)
                    patientProfile.RequestRoomId = this.currentModule.RoomId;

                //Process patient data from input data
                this.ProcessPatientData();

                //Process patient type alter (bhyt,aia,...) data from input data
                this.ProcessPatientTypeAlterData();

                //Process treatment from input data
                this.ProcessTreatmentData();

                if (this.chkEmergency)
                    this.ProcessEmergency();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected object RunBase(object data, UCRegister ucRequestService)
        {
            try
            {
                bool valid = true;
                if (data == null) throw new ArgumentNullException("Input data is null");
                if (this.ucRequestService == null) throw new ArgumentNullException("ucRequestService is null");

                valid = valid && (patientProfile.HisPatientTypeAlter != null);
                valid = valid && (this.IsChild());
                valid = valid && (patientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE);
                valid = valid && (String.IsNullOrEmpty(patientProfile.DistrictCode) || String.IsNullOrEmpty(patientProfile.ProvinceCode));

                if (valid)
                {
                    this.ucRequestService.txtProvinceCode.Focus();
                    WaitingManager.Hide();
                    param.Messages.Add(ResourceMessage.TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh);
                }
                else
                {
                    CallSyncHID();
                    //LogSystem.Debug("RunBase => begin call api");
                    if (data.GetType() == typeof(HisServiceReqExamRegisterSDO))
                        return new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>(HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (data.GetType() == typeof(HisPatientProfileSDO))
                        return new BackendAdapter(param).Post<HisPatientProfileSDO>(HisRequestUriStore.HIS_PATIENT_REGISTER_PROFILE, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    //LogSystem.Debug("RunBase => end call api");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        void ProcessPatientData()
        {
            try
            {
                if (this.patientProfile == null) patientProfile = new HisPatientProfileSDO();
                if (this.patientProfile.HisPatient == null) this.patientProfile.HisPatient = new HIS_PATIENT();
                if (this.patientId != 0)
                    this.patientProfile.HisPatient.ID = this.patientId;
                this.patientProfile.HisPatient.IS_HAS_NOT_DAY_DOB = (short)(this.isNotPatientDayDob ? 1 : 0);

                if (this.patientInformationADO != null)
                {
                    this.patientProfile.HisPatient.EMAIL = this.patientInformationADO.EMAIL;
                    //filter.NICK_NAME = this.patientInformationADO.NICK_NAME;
                    this.patientProfile.HisPatient.HOUSEHOLD_CODE = this.patientInformationADO.HOUSEHOLD_CODE;
                    this.patientProfile.HisPatient.HOUSEHOLD_RELATION_NAME = this.patientInformationADO.HOUSEHOLD_RELATION_NAME;
                    //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                    if (!String.IsNullOrEmpty(this.patientInformationADO.CMND_NUMBER))
                    {
                        if (this.patientInformationADO.CMND_NUMBER.Length > 9)
                        {
                            this.patientProfile.HisPatient.CCCD_DATE = this.patientInformationADO.CMND_DATE;
                            this.patientProfile.HisPatient.CCCD_NUMBER = this.patientInformationADO.CMND_NUMBER;
                            this.patientProfile.HisPatient.CCCD_PLACE = this.patientInformationADO.CMND_PLACE;
                        }
                        else
                        {
                            this.patientProfile.HisPatient.CMND_DATE = this.patientInformationADO.CMND_DATE;
                            this.patientProfile.HisPatient.CMND_NUMBER = this.patientInformationADO.CMND_NUMBER;
                            this.patientProfile.HisPatient.CMND_PLACE = this.patientInformationADO.CMND_PLACE;
                        }
                    }

                    this.patientProfile.HisPatient.HT_ADDRESS = this.patientInformationADO.HT_ADDRESS;
                    this.patientProfile.HisPatient.HT_COMMUNE_NAME = this.patientInformationADO.HT_COMMUNE_NAME;
                    this.patientProfile.HisPatient.HT_DISTRICT_NAME = this.patientInformationADO.HT_DISTRICT_NAME;
                    this.patientProfile.HisPatient.HT_PROVINCE_NAME = this.patientInformationADO.HT_PROVINCE_NAME;
                    this.patientProfile.HisPatient.HT_COMMUNE_CODE = this.patientInformationADO.HT_COMMUNE_CODE;
                    this.patientProfile.HisPatient.HT_DISTRICT_CODE = this.patientInformationADO.HT_DISTRICT_CODE;
                    this.patientProfile.HisPatient.HT_PROVINCE_CODE = this.patientInformationADO.HT_PROVINCE_CODE;
                    this.patientProfile.HisPatient.MOTHER_NAME = this.patientInformationADO.MOTHER_NAME;
                    this.patientProfile.HisPatient.FATHER_NAME = this.patientInformationADO.FATHER_NAME;
                    this.patientProfile.HisPatient.RELATIVE_MOBILE = this.patientInformationADO.RELATIVE_MOBILE;

                    this.patientProfile.HisPatient.BLOOD_ABO_CODE = this.patientInformationADO.BLOOD_ABO_CODE;
                    this.patientProfile.HisPatient.BLOOD_RH_CODE = this.patientInformationADO.BLOOD_RH_CODE;

                    this.patientProfile.HisPatient.RELATIVE_ADDRESS = this.relativeAddress;
                    this.patientProfile.HisPatient.RELATIVE_NAME = this.relativeName;
                    this.patientProfile.HisPatient.RELATIVE_TYPE = this.relativeType;
                    this.patientProfile.HisPatient.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                    this.patientProfile.HisPatient.BORN_PROVINCE_CODE = GenerateProvinceCode(this.provinceCodeKS);//(*)
                    this.patientProfile.HisPatient.BORN_PROVINCE_NAME = this.provinceNameKS;//(*)
                }

                if ((string.IsNullOrEmpty(this.patientProfile.HisPatient.CCCD_NUMBER) || string.IsNullOrEmpty(this.patientProfile.HisPatient.CMND_NUMBER)) && this.isReadQrCccdData)
                {
                    if (this.CccdReadFromQr.Length > 9)
                    {
                        this.patientProfile.HisPatient.CCCD_DATE = dateReleaseFromQr;
                        this.patientProfile.HisPatient.CCCD_NUMBER = CccdReadFromQr;
                    }
                    else
                    {
                        this.patientProfile.HisPatient.CMND_DATE = dateReleaseFromQr;
                        this.patientProfile.HisPatient.CMND_NUMBER = CccdReadFromQr;
                    }
                }
                int idx = this.patientName.LastIndexOf(" ");
                this.patientProfile.HisPatient.FIRST_NAME = (idx > -1 ? this.patientName.Substring(idx).Trim() : this.patientName);
                this.patientProfile.HisPatient.LAST_NAME = (idx > -1 ? this.patientName.Substring(0, idx).Trim() : "");
                this.patientProfile.HisPatient.BRANCH_ID = WorkPlace.GetBranchId();
                this.patientProfile.HisPatient.PROVINCE_CODE = this.provinceCode;
                this.patientProfile.HisPatient.DOB = this.dob;
                this.patientProfile.HisPatient.GENDER_ID = this.GenderId;
                this.patientProfile.HisPatient.ADDRESS = this.address;
                this.patientProfile.HisPatient.PROVINCE_NAME = this.provinceName;
                this.patientProfile.HisPatient.DISTRICT_NAME = this.districtName;
                this.patientProfile.HisPatient.COMMUNE_NAME = this.communeName;
                this.patientProfile.HisPatient.COMMUNE_CODE = this.communeCode;
                if (careerId.HasValue)
                    this.patientProfile.HisPatient.CAREER_ID = this.careerId.Value;
                this.patientProfile.HisPatient.CAREER_NAME = this.careerName;
                this.patientProfile.HisPatient.CAREER_CODE = this.careerCode;
                this.patientProfile.HisPatient.ETHNIC_NAME = this.ethnicName;
                this.patientProfile.HisPatient.ETHNIC_CODE = this.ethnicCode;
                this.patientProfile.HisPatient.NATIONAL_NAME = this.nationalName;
                this.patientProfile.HisPatient.NATIONAL_CODE = this.nationalCode;
                this.patientProfile.HisPatient.MPS_NATIONAL_CODE = this.mpsNationalCode;
                if (this.workPlace != null && (this.workPlace is long || this.workPlace is long?) && (long?)this.workPlace > 0)
                    this.patientProfile.HisPatient.WORK_PLACE_ID = (long?)this.workPlace;
                else if (this.workPlace != null && this.workPlace is string)
                    this.patientProfile.HisPatient.WORK_PLACE = (string)this.workPlace;
                if (this.militaryId > 0)
                    this.patientProfile.HisPatient.MILITARY_RANK_ID = this.militaryId;
                this.patientProfile.HisPatient.PHONE = this.phone;
                this.patientProfile.HisPatient.RELATIVE_NAME = this.relativeName;
                this.patientProfile.HisPatient.RELATIVE_TYPE = this.relativeType;
                this.patientProfile.HisPatient.RELATIVE_ADDRESS = this.relativeAddress;
                this.patientProfile.HisPatient.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                this.patientProfile.HisPatient.HT_COMMUNE_NAME = this.communeNowName;
                this.patientProfile.HisPatient.HT_DISTRICT_NAME = this.districtNowName;
                this.patientProfile.HisPatient.HT_PROVINCE_NAME = this.provinceNowName;
                this.patientProfile.HisPatient.HT_COMMUNE_CODE = this.communeNowCode;
                this.patientProfile.HisPatient.HT_DISTRICT_CODE = this.districtNowCode;
                this.patientProfile.HisPatient.HT_PROVINCE_CODE = this.provinceNowCode;
                this.patientProfile.IsChronic = this.chkChronic;
                if (this.chkChronic)
                    this.patientProfile.HisPatient.IS_CHRONIC = 1;
                if (!string.IsNullOrEmpty(this.hrmEmployeeCode))
                    this.patientProfile.HisPatient.HRM_EMPLOYEE_CODE = this.hrmEmployeeCode;

                this.patientProfile.HisPatient.PATIENT_CLASSIFY_ID = this.patientClassifyId;
                if (this.IsWarningForNext)
                    this.patientProfile.HisPatient.NOTE = this.Note;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessTreatmentData()
        {
            try
            {
                if (Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData() != null && departmentId > 0)
                {
                    this.patientProfile.DepartmentId = departmentId;
                }

                if (this.programId != 0)
                    this.patientProfile.HisTreatment.PROGRAM_ID = this.programId;

                if (this.oweTypeId > 0)
                {
                    this.patientProfile.HisTreatment.OWE_TYPE_ID = this.oweTypeId;
                    this.patientProfile.HisTreatment.OWE_MODIFY_TIME = this.intructionTime;
                }

                if (this.patientId != 0)
                {
                    this.patientProfile.HisTreatment.PATIENT_ID = this.patientId;
                    if (this.codeFind == typeCodeFind__MaCT && this.programId != 0)
                    {
                        this.patientProfile.HisTreatment.PROGRAM_ID = this.programId;
                    }
                    else if (this.codeFind == typeCodeFind__MaHK && !String.IsNullOrEmpty(this.appointmentCode))
                    {
                        this.patientProfile.HisTreatment.APPOINTMENT_CODE = this.appointmentCode;
                    }
                }

                this.patientProfile.ProvinceCode = provinceCode;
                this.patientProfile.DistrictCode = districtCode;
                this.patientProfile.TreatmentTime = intructionTime;

                if(uCMainHein != null && ucHein__BHYT != null)
                //Đồng bộ dữ liệu thay đổi từ uchein sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api
                    this.uCMainHein.UpdateDataFormIntoPatientProfile(this.ucHein__BHYT, this.patientProfile);

                if (!string.IsNullOrEmpty(this.hrmKskCode))
                    this.patientProfile.HisTreatment.HRM_KSK_CODE = this.hrmKskCode;
                if (this.receptionForm.HasValue)
                {
                    this.patientProfile.HisTreatment.RECEPTION_FORM = this.receptionForm;
                }
                this.patientProfile.HisTreatment.HOSPITALIZE_REASON_CODE = this.HospitalizeReasonCode;
                this.patientProfile.HisTreatment.HOSPITALIZE_REASON_NAME = this.HospitalizeReasonName;
                this.patientProfile.HisTreatment.HOSPITALIZATION_REASON = this.HospitalizeReason;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessPatientTypeAlterData()
        {
            try
            {
                if (this.patientTypeId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                    dataPatientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                    //Đồng bộ dữ liệu thay đổi từ uchein sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api
                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HisConfigCFG.PatientTypeId__QN)
                    {
                        this.uCMainHein.UpdateDataFormIntoPatientTypeAlter(this.ucHein__BHYT, dataPatientProfile);
                        Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>();
                    }

                    patientProfile.HisPatientTypeAlter = Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>(dataPatientProfile.HisPatientTypeAlter);

                    if (patientTypeId == HisConfigCFG.PatientTypeId__KSK && this.kskContractId.HasValue)
                    {
                        patientProfile.HisPatientTypeAlter.KSK_CONTRACT_ID = this.kskContractId.Value;
                    }
                    patientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID = patientTypeId;


                    if (treatmentTypeId > 0)
                        patientProfile.HisPatientTypeAlter.TREATMENT_TYPE_ID = treatmentTypeId;
                    else
                        LogSystem.Debug("Lay doi tuong benh nhan theo gia tri cua combo dien dieu tri man hinh dang ky tiep don khong thanh cong. treatmentTypeId= " + treatmentTypeId);

                    if (this.cardSearch != null && !String.IsNullOrEmpty(this.cardSearch.CardCode))
                        patientProfile.CardCode = this.cardSearch.CardCode;
                    else
                        LogSystem.Debug("Khong co du lieu CardCode. CardCode = " + patientProfile.CardCode);

                    if (patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE
                        && patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                    {
                        this.patientProfile.HisTreatment.IS_TRANSFER_IN = 1;
                    }
                }
                else
                {
                    LogSystem.Debug("Lay doi tuong benh nhan theo gia tri cua combo doi tuong man hinh dang ky tiep don khong thanh cong. patientTypeId= " + patientTypeId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessEmergency()
        {
            try
            {
                if (this.patientProfile.HisTreatment == null)
                    this.patientProfile.HisTreatment = new HIS_TREATMENT();
                this.patientProfile.HisTreatment.IS_EMERGENCY = 1;
                if (this.emergencyWTimeId > 0)
                    this.patientProfile.HisTreatment.EMERGENCY_WTIME_ID = this.emergencyWTimeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsChild()
        {
            bool isChild = false;
            try
            {
                var dtDateOfBird = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientProfile.HisPatient.DOB) ?? DateTime.Now;
                isChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDateOfBird);
            }
            catch (Exception ex)
            {
                isChild = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return isChild;
        }

        /// <summary>
        /// Thêm nghiệp vụ Hồ sơ sức khỏe cá nhân. có sử dụng thẻ cấu hình.
        /// Khi nhấn Lưu => Gọi lên HID:
        /// 1. Nếu HID trả về một bản ghi mới tạo => Set mã y tế vào HIS_PATIENT và gọi đăng ký tiếp đón như bình thương.
        /// 2. Trả về một List các dữ liệu => Hiển thị lên cho người dùng chọn.
        /// a. Nếu người dùng không chọn => gọi API tạo dữ liệu trên HID => HID trả về mã y tế => Set mã y tế vào HIS_PATIENT => Đăng ký bình thường
        /// b. Nếu người dùng chọn => Set mã y tế vào HIS_PATIENT => đăng ký tiếp đón bình thương
        /// </summary>
        /// <returns></returns>
        private void CallSyncHID()
        {
            try
            {
                //Các trường có dấu (*) là các trường bắt buộc phải set giá trị
                if (HisConfigCFG.IsSyncHID && (this.patientData == null || String.IsNullOrEmpty(this.patientData.PERSON_CODE)))
                {
                    CommonParam paramHID = new CommonParam();
                    HID_PERSON filter = new HID_PERSON();
                    if (ucRequestService != null && ucRequestService.cardSearch != null && !string.IsNullOrEmpty(ucRequestService.cardSearch.CardCode))
                    {
                        filter.CARD_CODE = ucRequestService.cardSearch.CardCode;
                    }
                    filter.BRANCH_CODE = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                    filter.BRANCH_NAME = BranchDataWorker.Branch.BRANCH_NAME;
                    filter.BHYT_NUMBER = ((this.patientProfile.HisPatientTypeAlter != null) ? this.patientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER : "");
                    filter.ADDRESS = this.patientProfile.HisPatient.ADDRESS;
                    filter.COMMUNE_NAME = this.patientProfile.HisPatient.COMMUNE_NAME;
                    filter.DISTRICT_NAME = this.patientProfile.HisPatient.DISTRICT_NAME;
                    filter.PROVINCE_NAME = this.patientProfile.HisPatient.PROVINCE_NAME;
                    filter.CAREER_NAME = (this.patientProfile.HisPatient.CAREER_ID > 0 ? (BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.ID == this.patientProfile.HisPatient.CAREER_ID) ?? new HIS_CAREER()).CAREER_NAME : "");
                    filter.DOB = this.patientProfile.HisPatient.DOB;//(*)
                    filter.GENDER_ID = this.patientProfile.HisPatient.GENDER_ID;//(*)
                    filter.FIRST_NAME = this.patientProfile.HisPatient.FIRST_NAME;
                    filter.LAST_NAME = this.patientProfile.HisPatient.LAST_NAME;
                    if (IsChild())
                    {
                        filter.VIR_PERSON_NAME = this.patientProfile.HisPatient.RELATIVE_NAME;//TODO
                    }
                    else
                    {
                        filter.VIR_PERSON_NAME = this.patientProfile.HisPatient.LAST_NAME + " " + this.patientProfile.HisPatient.FIRST_NAME;
                    }
                    filter.IS_HAS_NOT_DAY_DOB = this.patientProfile.HisPatient.IS_HAS_NOT_DAY_DOB;// (this.patientProfile.HisPatient.IS_HAS_NOT_DAY_DOB == GlobalVariables.CommonNumberTrue);
                    filter.ETHNIC_NAME = this.patientProfile.HisPatient.ETHNIC_NAME;
                    filter.EMAIL = this.patientProfile.HisPatient.EMAIL;
                    filter.NATIONAL_NAME = this.patientProfile.HisPatient.NATIONAL_NAME;
                    filter.MOBILE = this.patientProfile.HisPatient.PHONE;
                    filter.BLOOD_ABO_CODE = this.blood_Code;
                    filter.BLOOD_RH_CODE = this.blood_Rh_Code;
                    //filter.RELIGION_NAME = this.patientPlusInformationInfoValue.RELIGION_NAME;

                    if (this.patientInformationADO != null)
                    {
                        //filter.NICK_NAME = this.patientInformationADO.NICK_NAME;
                        filter.HOUSEHOLD_CODE = this.patientInformationADO.HOUSEHOLD_CODE;
                        filter.HOUSEHOLD_RELATION_NAME = this.patientInformationADO.HOUSEHOLD_RELATION_NAME;
                        //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                        if (!String.IsNullOrEmpty(this.patientInformationADO.CMND_NUMBER))
                        {
                            if (this.patientInformationADO.CMND_NUMBER.Length > 9)
                            {
                                filter.CCCD_DATE = this.patientInformationADO.CMND_DATE;
                                filter.CCCD_NUMBER = this.patientInformationADO.CMND_NUMBER;
                                filter.CCCD_PLACE = this.patientInformationADO.CMND_PLACE;
                            }
                            else
                            {
                                filter.CMND_DATE = this.patientInformationADO.CMND_DATE;
                                filter.CMND_NUMBER = this.patientInformationADO.CMND_NUMBER;
                                filter.CMND_PLACE = this.patientInformationADO.CMND_PLACE;
                            }
                        }

                        filter.HT_ADDRESS = this.patientInformationADO.HT_ADDRESS;
                        filter.HT_COMMUNE_NAME = this.patientInformationADO.HT_COMMUNE_NAME;
                        filter.HT_DISTRICT_NAME = this.patientInformationADO.HT_DISTRICT_NAME;
                        filter.HT_PROVINCE_NAME = this.patientInformationADO.HT_PROVINCE_NAME;
                        filter.MOTHER_NAME = this.patientInformationADO.MOTHER_NAME;
                        filter.FATHER_NAME = this.patientInformationADO.FATHER_NAME;

                        //filter.NCSC_MOBILE = this.patientInformationADO.NCSC_MOBILE;                        
                        filter.RELATIVE_PHONE = this.patientInformationADO.RELATIVE_MOBILE;
                    }
                    filter.RELATIVE_ADDRESS = this.relativeAddress;
                    filter.RELATIVE_NAME = this.relativeName;
                    filter.RELATIVE_TYPE = this.relativeType;
                    filter.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                    filter.BORN_PROVINCE_CODE = this.provinceCodeKS;// GenerateProvinceCode(this.provinceCodeKS);//(*)
                    filter.BORN_PROVINCE_NAME = this.provinceNameKS;//(*)
                    //HOH_NAME : tên chủ hộ
                    //BORN_DISTRICT_NAME
                    //BORN_COMMUNE_NAME
                    //BORN_ADDRESS

                    var persons = ApiConsumers.HidWrapConsumer.Post<List<HID_PERSON>>(true, "api/HidPerson/Take", paramHID, filter);// (new BackendAdapter(paramHID).Post<List<HID_PERSON>>(RequestUriStore.HID_PERSON_GET, ApiConsumers.HidWrapConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramHID));

                    if (persons != null && persons.Count > 0)
                    {
                        if (persons.Count == 1)
                        {
                            SelectPerson(persons[0]);
                        }
                        else
                        {
                            frmPersonSelect frmPersonSelect = new frmPersonSelect(persons, SelectPerson);
                            frmPersonSelect.ShowDialog();
                        }

                        if (string.IsNullOrEmpty(this.patientProfile.HisPatient.PERSON_CODE))
                        {
                            var personCreate = ApiConsumers.HidWrapConsumer.Post<HID_PERSON>(true, "api/HidPerson/Create", paramHID, filter);
                            if (personCreate != null)
                            {
                                SelectPerson(personCreate);
                            }
                        }
                    }
                    else
                    {
                        if (paramHID.Messages != null && paramHID.Messages.Count > 0)
                        {
                            this.param.Messages.AddRange(paramHID.Messages);
                        }
                        if (paramHID.BugCodes != null && paramHID.BugCodes.Count > 0)
                        {
                            this.param.BugCodes.AddRange(paramHID.BugCodes);
                        }
                        Inventec.Common.Logging.LogSystem.Debug("Goi len he thong HID lay thong tin ho so suc khoe ca nhan that bai. ____Input data: "
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter) + "____Result data:"
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => paramHID), paramHID)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => persons), persons));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SelectPerson(HID_PERSON data)
        {
            try
            {
                this.patientProfile.HisPatient.PERSON_CODE = data.PERSON_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string GenerateProvinceCode(string provinceCode)
        {
            try
            {
                return string.Format("{0:000}", Convert.ToInt64(provinceCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return provinceCode;
        }
    }
}
