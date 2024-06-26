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
using HIS.UC.UCHeniInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RegisterV3.ADO;
using HIS.Desktop.Plugins.RegisterV3.Run3;
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
using HIS.UC.UCPatientRaw.ADO;
using HIS.Desktop.Common;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCOtherServiceReqInfo.ADO;
using HIS.UC.UCRelativeInfo.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.UC.UCTransPati.ADO;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.KskContract.ADO;

namespace HIS.Desktop.Plugins.RegisterV3.Register
{
    abstract partial class ServiceRequestRegisterBehaviorBase : BusinessBase
    {

        // UCPatientRaw
        protected string patientName { get; set; }
        protected string patient_Last_Name { get; set; }
        protected string patient_First_Name { get; set; }
        protected long? careerId { get; set; }
        protected string careerName { get; set; }
        protected string careerCode { get; set; }
        protected long GenderId { get; set; }
        protected long dob { get; set; }
        protected long patientTypeId { get; set; }
        protected string patientCode { get; set; }
        protected short? iS_Has_Not_Day_Dob { get; set; }

        // tìm kiếm
        protected string appointmentCode { get; set; }
        protected string codeFind { get; set; }
        protected string typeCodeFind__MaBN { get; set; }
        protected string typeCodeFind__MaHK { get; set; }
        protected string typeCodeFind__MaCT { get; set; }
        protected string typeCodeFind__SoThe { get; set; }
        protected long patientId { get; set; }

        // UCPlusInfo
        protected long? cMNDDate { get; set; }
        protected string cMNDNumber { get; set; }
        protected string cMNDPlace { get; set; }
        protected long? cCCDDate { get; set; }
        protected string cCCDNumber { get; set; }
        protected string cCCDPlace { get; set; }
        protected string communeNowCode { get; set; }
        protected string districtNowCode { get; set; }
        protected string provinceNowCode { get; set; }
        protected string communeNowName { get; set; }
        protected string districtNowName { get; set; }
        protected string provinceNowName { get; set; }
        protected string addressNow { get; set; }
        protected string email { get; set; }
        protected string phone { get; set; }
        protected string born_provinceCode { get; set; }
        protected string born_provinceName { get; set; }
        protected long programId { get; set; }
        protected string programCode { get; set; }
        protected string nationalName { get; set; }
        protected string nationalCode { get; set; }
        protected string ethnicName { get; set; }
        protected string ethnicCode { get; set; }
        protected long militaryId { get; set; }
        protected object workPlace { get; set; }
        protected long? blood_ABO_ID { get; set; }
        protected string blood_ABO_Code { get; set; }
        protected long? blood_Rh_Id { get; set; }
        protected string blood_Rh_Code { get; set; }
        protected string fatherName { get; set; }
        protected string motherName { get; set; }
        protected string houseHold_Code { get; set; }
        protected string hoseHold_Relative { get; set; }
        protected long? houseHoldRelative_ID { get; set; }
        protected string maHoNgheo { get; set; }
        protected string patientStoreCode { get; set; }

        //UCAddress
        protected string communeCode { get; set; }
        protected string communeName { get; set; }
        protected string districtCode { get; set; }
        protected string districtName { get; set; }
        protected string provinceCode { get; set; }
        protected string provinceName { get; set; }
        protected string address { get; set; }

        // UCRelative
        protected string relativeAddress { get; set; }
        protected string relativeName { get; set; }
        protected string relativeType { get; set; }
        protected string relativeCMNDNumber { get; set; }
        protected string religionName { get; set; }

        // UCOtherSerViceReqInfo
        protected long intructionTime { get; set; }
        protected long treatmentTypeId { get; set; }
        protected long oweTypeId { get; set; }
        public bool isPriority { get; set; }
        protected bool chkEmergency { get; set; }
        protected bool isNotPatientDayDob { get; set; }
        protected bool chkChronic { get; set; }
        protected long emergencyWTimeId { get; set; }
        protected long departmentId { get; set; }
        protected bool isNotRequireFee { get; set; }

        // UCTransPatin

        protected string icd_Code { get; set; }
        protected string icd_Name { get; set; }
        protected string icd_Text { get; set; }
        protected string noiChuyenDen_Code { get; set; }
        protected string noiChuyenDen_Name { get; set; }
        protected string soChuyenVien { get; set; }
        protected string right_Router_Type { get; set; }
        protected long? hinhThucChuyen_ID { get; set; }
        protected long? lyDoChuyen_ID { get; set; }
        protected long? transfer_In_CMKT { get; set; }
        protected bool isHasDialogText { get; set; }
        protected bool isDisablelblEditICD { get; set; }
        protected long chuyenTuyen_ID { get; set; }
        protected string chuyenTuyen_Name { get; set; }
        protected string chuyenTuyen_MoTa { get; set; }

        // UCImage
        protected byte[] img_avatar { get; set; }
        protected byte[] img_BHYT { get; set; }

        //KskContract

        protected long? kskContractId { get; set; }

        // Declare
        protected HisCardSDO cardSearch { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPatientExtendADO patientInformationADO { get; set; }
        protected HisPatientSDO patientData { get; set; }
        protected HisPatientProfileSDO patientProfile { get; set; }
        protected Module currentModule { get; set; }
        protected UCRegister ucRequestService;
        protected HisPatientProfileSDO heinInfoValue { get; set; }
        protected UCPatientRawADO patientRawInfoValue { get; set; }
        protected UCAddressADO addressInfoValue { get; set; }
        protected UCServiceReqInfoADO serviceReqInfoValue { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPlusInfoADO patientPlusInformationInfoValue { get; set; }
        protected UCRelativeADO relativeInfoValue { get; set; }
        protected UCTransPatiADO UCTransPatiADO { get; set; }
        protected HIS.UC.UCImageInfo.ADO.UCImageInfoADO imageADO { get; set; }
        protected List<ServiceReqDetailSDO> serviceRoomInfoValue { get; set; }
        

        internal ServiceRequestRegisterBehaviorBase(CommonParam param, UCRegister ucServiceRequestRegiter)
            : base(param)
        {
            try
            {
                // Get Data From UC
                this.ucRequestService = ucServiceRequestRegiter;
                this.patientRawInfoValue = ucServiceRequestRegiter.ucPatientRaw1.GetValue();
                this.addressInfoValue = ucServiceRequestRegiter.ucAddressCombo1.GetValue();
                this.patientPlusInformationInfoValue = ucServiceRequestRegiter.ucPlusInfo1.GetValue();
                this.relativeInfoValue = ucServiceRequestRegiter.ucRelativeInfo1.GetValue();
                this.UCTransPatiADO = ucServiceRequestRegiter.transPatiADO;
                this.currentModule = ucServiceRequestRegiter.currentModule;

                // UCPatientRaw
                this.patientName = this.patientRawInfoValue.PATIENT_NAME;
                this.patient_Last_Name = this.patientRawInfoValue.PATIENT_LAST_NAME;
                this.patient_First_Name = this.patientRawInfoValue.PATIENT_FIRST_NAME;
                if (this.patientRawInfoValue.GENDER_ID > 0)
                    this.GenderId = this.patientRawInfoValue.GENDER_ID;
                this.dob = this.patientRawInfoValue.DOB;
                this.patientTypeId = this.patientRawInfoValue.PATIENTTYPE_ID;
                if (this.patientRawInfoValue.CARRER_ID != null)
                    this.careerId = (long)(this.patientRawInfoValue.CARRER_ID);
                this.careerCode = this.patientRawInfoValue.CARRER_CODE;
                this.careerName = this.patientRawInfoValue.CARRER_NAME;
                this.iS_Has_Not_Day_Dob = this.patientRawInfoValue.IS_HAS_NOT_DAY_DOB;
                this.isNotPatientDayDob = (this.patientRawInfoValue.IS_HAS_NOT_DAY_DOB == GlobalVariables.CommonNumberTrue);

                // UCAddress
                this.address = this.addressInfoValue.Address;
                this.communeName = (string)(this.addressInfoValue.Commune_Name);
                this.communeCode = (string)(this.addressInfoValue.Commune_Code);
                this.districtName = (string)(this.addressInfoValue.District_Name);
                this.districtCode = (string)(this.addressInfoValue.District_Code);
                this.provinceName = (string)(this.addressInfoValue.Province_Name);
                this.provinceCode = (string)(this.addressInfoValue.Province_Code);

                // UCOtherServiceReqInfo
                this.chkEmergency = this.serviceReqInfoValue.IsEmergency;
                this.intructionTime = this.serviceReqInfoValue.IntructionTime;
                if (this.serviceReqInfoValue.TreatmentType_ID > 0)
                    this.treatmentTypeId = this.serviceReqInfoValue.TreatmentType_ID;
                this.isPriority = this.serviceReqInfoValue.IsPriority;
                this.chkChronic = this.serviceReqInfoValue.IsChronic;
                if (this.serviceReqInfoValue.OweType_ID > 0)
                    this.oweTypeId = this.serviceReqInfoValue.OweType_ID;
                if (this.serviceReqInfoValue.IsEmergency && this.serviceReqInfoValue.EmergencyTime_ID > 0)
                    this.emergencyWTimeId = this.serviceReqInfoValue.EmergencyTime_ID;
                this.isNotRequireFee = this.serviceReqInfoValue.IsNotRequireFee;


                // UCPlusInfo
                this.born_provinceCode = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_CODE;
                this.born_provinceName = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_NAME;
                this.communeNowCode = patientPlusInformationInfoValue.HT_COMMUNE_CODE;
                this.communeNowName = patientPlusInformationInfoValue.HT_COMMUNE_NAME;
                this.provinceNowCode = patientPlusInformationInfoValue.HT_PROVINCE_CODE;
                this.provinceNowName = patientPlusInformationInfoValue.HT_PROVINCE_NAME;
                this.districtNowCode = patientPlusInformationInfoValue.HT_DISTRICT_CODE;
                this.districtNowName = patientPlusInformationInfoValue.HT_DISTRICT_NAME;
                this.addressNow = patientPlusInformationInfoValue.HT_ADDRESS;
                this.fatherName = patientPlusInformationInfoValue.FATHER_NAME;
                this.motherName = patientPlusInformationInfoValue.MOTHER_NAME;
                this.ethnicName = this.patientPlusInformationInfoValue.ETHNIC_NAME;
                this.ethnicCode = this.patientPlusInformationInfoValue.ETHNIC_CODE;
                if (this.patientPlusInformationInfoValue.MILITARYRANK_ID.HasValue)
                    this.militaryId = (long)(this.patientPlusInformationInfoValue.MILITARYRANK_ID);
                this.nationalName = this.patientPlusInformationInfoValue.NATIONAL_NAME;
                this.nationalCode = this.patientPlusInformationInfoValue.NATIONAL_CODE;
                this.programId = this.patientPlusInformationInfoValue.PROGRAM_ID;
                this.programCode = this.patientPlusInformationInfoValue.PROGRAM_CODE;
                this.phone = this.patientPlusInformationInfoValue.PHONE_NUMBER;
                this.email = this.patientPlusInformationInfoValue.EMAIL;
                this.workPlace = this.patientPlusInformationInfoValue.workPlace;
                this.blood_ABO_Code = this.patientPlusInformationInfoValue.BLOOD_ABO_CODE;
                this.blood_ABO_ID = this.patientPlusInformationInfoValue.BLOOD_ABO_ID;
                this.blood_Rh_Code = this.patientPlusInformationInfoValue.BLOOD_RH_CODE;
                this.blood_Rh_Id = this.patientPlusInformationInfoValue.BLOOD_RH_ID;
                this.cMNDNumber = this.patientPlusInformationInfoValue.CMND_NUMBER;
                this.cMNDDate = this.patientPlusInformationInfoValue.CMND_DATE;
                this.cMNDPlace = this.patientPlusInformationInfoValue.CMND_PLACE;
                this.houseHold_Code = this.patientPlusInformationInfoValue.HOUSEHOLD_CODE;
                this.houseHoldRelative_ID = this.patientPlusInformationInfoValue.HOUSEHOLD_RELATION_ID;
                this.hoseHold_Relative = this.patientPlusInformationInfoValue.HOUSEHOLD_RELATION_NAME;
                this.maHoNgheo = this.patientPlusInformationInfoValue.HONGHEO_CODE == "" ? "" : this.patientPlusInformationInfoValue.HONGHEO_CODE;
                this.patientStoreCode = this.patientPlusInformationInfoValue.PATIENT_STORE_CODE;

                //UCRelative
                this.relativeAddress = this.relativeInfoValue.RelativeAddress;
                this.relativeType = this.relativeInfoValue.Correlated;
                this.relativeName = this.relativeInfoValue.RelativeName;
                this.relativeCMNDNumber = this.relativeInfoValue.RelativeCMND;
                this.religionName = "";

                //UCImage
                this.departmentId = GlobalStore.DepartmentId;
//                this.img_avatar = this.imageADO.FileImagePortrait;
//               this.img_BHYT = this.imageADO.FileImageHein;

                //UCTransPati
                if (UCTransPatiADO != null)
                {
                    this.icd_Code = this.UCTransPatiADO.ICD_CODE;
                    this.icd_Name = this.UCTransPatiADO.ICD_NAME;
                    this.icd_Text = this.UCTransPatiADO.ICD_TEXT;
                    this.noiChuyenDen_Code = this.UCTransPatiADO.NOICHUYENDEN_CODE;
                    this.noiChuyenDen_Name = this.UCTransPatiADO.NOICHUYENDEN_NAME;
                    this.soChuyenVien = this.UCTransPatiADO.SOCHUYENVIEN;
                    this.right_Router_Type = this.UCTransPatiADO.RIGHT_ROUTER_TYPE;
                    this.hinhThucChuyen_ID = this.UCTransPatiADO.HINHTHUCHUYEN_ID;
                    this.lyDoChuyen_ID = this.UCTransPatiADO.LYDOCHUYEN_ID;
                    this.transfer_In_CMKT = this.UCTransPatiADO.TRANSFER_IN_CMKT;
                    this.isHasDialogText = this.UCTransPatiADO.IsHasDialogText;
                    this.isDisablelblEditICD = this.UCTransPatiADO.IsDisablelblEditICD;
                }

                // Other
                this.patientId = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.ID : 0);
                this.cardSearch = this.ucRequestService.cardSearch;
                this.patientData = this.ucRequestService.currentPatientSDO;
                this.appointmentCode = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.AppointmentCode : "");

                //kskContract
                if (this.ucRequestService.ucKskContract != null && this.ucRequestService.kskContractProcessor != null)
                {
                    KskContractOutput kskContractOutput = (KskContractOutput) this.ucRequestService.kskContractProcessor.GetValue(this.ucRequestService.ucKskContract);
                    if (kskContractOutput != null && kskContractOutput.IsVali && kskContractOutput.KskContract!=null)
                        this.kskContractId = kskContractOutput.KskContract.ID;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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
                    this.ucRequestService.ucAddressCombo1.FocusToProvince();
                    WaitingManager.Hide();
                    param.Messages.Add(ResourceMessage.TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh);
                }
                else
                {
                    CallSyncHID();

                    LogSystem.Debug("RunBase => begin call api");
                    if (data.GetType() == typeof(HisServiceReqExamRegisterSDO))
                        return new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>(HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (data.GetType() == typeof(HisPatientProfileSDO))
                        return new BackendAdapter(param).Post<HisPatientProfileSDO>(HisRequestUriStore.HIS_PATIENT_REGISTER_PROFILE, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    LogSystem.Debug("RunBase => end call api");
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
                this.patientProfile.HisPatient.EMAIL = this.email;
                this.patientProfile.HisPatient.PATIENT_STORE_CODE = this.patientStoreCode;
                this.patientProfile.HisPatient.HOUSEHOLD_CODE = this.houseHold_Code;
                this.patientProfile.HisPatient.HOUSEHOLD_RELATION_NAME = this.hoseHold_Relative;
                //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                if (!String.IsNullOrEmpty(this.cMNDNumber))
                {
                    if (this.cMNDNumber.Length > 9)
                    {
                        this.patientProfile.HisPatient.CCCD_DATE = this.cMNDDate;
                        this.patientProfile.HisPatient.CCCD_NUMBER = this.cMNDNumber;
                        this.patientProfile.HisPatient.CCCD_PLACE = this.cMNDPlace;
                    }
                    else
                    {
                        this.patientProfile.HisPatient.CMND_DATE = this.cMNDDate;
                        this.patientProfile.HisPatient.CMND_NUMBER = this.cMNDNumber;
                        this.patientProfile.HisPatient.CMND_PLACE = this.cMNDPlace;
                    }
                }

                this.patientProfile.HisPatient.HT_ADDRESS = this.addressNow;
                this.patientProfile.HisPatient.HT_COMMUNE_NAME = this.communeNowName;
                this.patientProfile.HisPatient.HT_DISTRICT_NAME = this.districtNowName;
                this.patientProfile.HisPatient.HT_PROVINCE_NAME = this.provinceNowName;
                this.patientProfile.HisPatient.MOTHER_NAME = this.motherName;
                this.patientProfile.HisPatient.FATHER_NAME = this.fatherName;
                this.patientProfile.HisPatient.RELATIVE_MOBILE = this.phone;

                this.patientProfile.HisPatient.BLOOD_ABO_CODE = this.blood_ABO_Code;
                this.patientProfile.HisPatient.BLOOD_RH_CODE = this.blood_Rh_Code;

                this.patientProfile.HisPatient.RELATIVE_ADDRESS = this.relativeAddress;
                this.patientProfile.HisPatient.RELATIVE_NAME = this.relativeName;
                this.patientProfile.HisPatient.RELATIVE_TYPE = this.relativeType;
                this.patientProfile.HisPatient.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                this.patientProfile.HisPatient.BORN_PROVINCE_CODE = GenerateProvinceCode(this.born_provinceCode);//(*)
                this.patientProfile.HisPatient.BORN_PROVINCE_NAME = this.born_provinceName;//(*)
                this.patientProfile.HisPatient.FIRST_NAME = this.patient_First_Name;
                this.patientProfile.HisPatient.LAST_NAME = this.patient_Last_Name;

                this.patientProfile.HisPatient.PROVINCE_CODE = this.provinceCode;
                this.patientProfile.HisPatient.DOB = this.dob;
                this.patientProfile.HisPatient.GENDER_ID = this.GenderId;
                this.patientProfile.HisPatient.ADDRESS = this.address;
                this.patientProfile.HisPatient.PROVINCE_NAME = this.provinceName;
                this.patientProfile.HisPatient.DISTRICT_NAME = this.districtName;
                this.patientProfile.HisPatient.COMMUNE_NAME = this.communeName;
                if (careerId.HasValue)
                    this.patientProfile.HisPatient.CAREER_ID = this.careerId.Value;
                this.patientProfile.HisPatient.CAREER_NAME = this.careerName;
                this.patientProfile.HisPatient.CAREER_CODE = this.careerCode;
                this.patientProfile.HisPatient.ETHNIC_NAME = this.ethnicName;
                this.patientProfile.HisPatient.ETHNIC_CODE = this.ethnicCode;
                this.patientProfile.HisPatient.NATIONAL_NAME = this.nationalName;
                this.patientProfile.HisPatient.NATIONAL_CODE = this.nationalCode;
                if (this.workPlace != null && (this.workPlace is long || this.workPlace is long?) && (long?)this.workPlace > 0)
                    this.patientProfile.HisPatient.WORK_PLACE_ID = (long?)this.workPlace;
                else if (this.workPlace != null && this.workPlace is string)
                    this.patientProfile.HisPatient.WORK_PLACE = (string)this.workPlace;
                if (this.militaryId > 0)
                    this.patientProfile.HisPatient.MILITARY_RANK_ID = this.militaryId;
                this.patientProfile.HisPatient.PHONE = this.phone;
                this.patientProfile.IsChronic = this.chkChronic;
                if (this.chkChronic)
                    this.patientProfile.HisPatient.IS_CHRONIC = 1;
                this.patientProfile.ImgAvatarData = this.img_avatar;
                this.patientProfile.ImgBhytData = this.img_BHYT;
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

                //Đồng bộ dữ liệu thay đổi từ UCTransPatiADO sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api        
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_CODE = this.icd_Code;
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_NAME = (!String.IsNullOrEmpty(this.icd_Text) ? this.icd_Text : this.icd_Name);
                this.patientProfile.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE = this.noiChuyenDen_Code;
                this.patientProfile.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME = this.noiChuyenDen_Name;
                this.patientProfile.HisTreatment.TRANSFER_IN_FORM_ID = this.hinhThucChuyen_ID;
                this.patientProfile.HisTreatment.TRANSFER_IN_REASON_ID = this.lyDoChuyen_ID;
                this.patientProfile.HisTreatment.TRANSFER_IN_CODE = this.soChuyenVien;
                this.patientProfile.HisTreatment.TRANSFER_IN_CMKT = this.transfer_In_CMKT;
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
                    {
                        CommonParam param = new CommonParam();
                        HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                        dataPatientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                        //Đồng bộ dữ liệu thay đổi từ uchein sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api
                        //this.uCMainHein.UpdateDataFormIntoPatientTypeAlter(this.ucHein__BHYT, dataPatientProfile);

                        if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HisConfigCFG.PatientTypeId__QN)
                        {
                            dataPatientProfile.HisPatientTypeAlter = this.heinInfoValue.HisPatientTypeAlter;
                            Mapper.CreateMap<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>();
                           
                        }

                        patientProfile.HisPatientTypeAlter = Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>(dataPatientProfile.HisPatientTypeAlter);

                        if (patientTypeId == HisConfigCFG.PatientTypeId__KSK && this.kskContractId.HasValue)
                        {
                            patientProfile.HisPatientTypeAlter.KSK_CONTRACT_ID = this.kskContractId.Value;
                        }


                        patientProfile.HisPatientTypeAlter.PATIENT_TYPE_ID = patientTypeId;
                        patientProfile.HisPatientTypeAlter.HNCODE = this.maHoNgheo;
                        if (treatmentTypeId > 0)
                            patientProfile.HisPatientTypeAlter.TREATMENT_TYPE_ID = treatmentTypeId;
                        else
                            LogSystem.Debug("Lay doi tuong benh nhan theo gia tri cua combo dien dieu tri man hinh dang ky tiep don khong thanh cong. treatmentTypeId= " + treatmentTypeId);

                        if (this.cardSearch != null && !String.IsNullOrEmpty(this.cardSearch.CardCode))
                            patientProfile.CardCode = this.cardSearch.CardCode;
                        else
                            LogSystem.Debug("Khong co du lieu CardCode. CardCode = " + patientProfile.CardCode);
                        if (this.patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT
                     || this.patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN)
                        {
                            if (patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE
                                && patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE == MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeCode.PRESENT)
                            {
                                this.patientProfile.HisTreatment.IS_TRANSFER_IN = 1;
                            }
                        }
                        else
                        {
                            patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE = null;
                            patientProfile.HisPatientTypeAlter.RIGHT_ROUTE_TYPE_CODE = null;
                        }
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
    }
}
