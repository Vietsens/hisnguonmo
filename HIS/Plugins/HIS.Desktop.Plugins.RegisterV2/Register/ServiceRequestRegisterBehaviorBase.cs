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
using HIS.UC.UCHeniInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.RegisterV2.ADO;
using HIS.Desktop.Plugins.RegisterV2.Run2;
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
using DevExpress.XtraEditors;
using His.UC.UCHein;

namespace HIS.Desktop.Plugins.RegisterV2.Register
{
    abstract partial class ServiceRequestRegisterBehaviorBase : HIS.Desktop.Common.BusinessBase
    {
        protected string provinceCodeKS { get; set; }
        protected string provinceNameKS { get; set; }
        protected string districtCodeKS { get; set; }
        protected string districtNameKS { get; set; }
        protected string communeCodeKS { get; set; }
        protected string communeNameKS { get; set; }
        protected string addressKS { get; set; }
        protected string hohName { get; set; }

        // UCPatientRaw
        protected string PeopleCode { get; set; }
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
        protected long? PositionId { get; set; }
        protected List<string> lstPreviousDebtTreatments { get; set; }
        protected long? receptionForm { get; set; }
        protected string CardCode { get; set; }
        protected string CardServiceCode { get; set; }
        protected string BankCardCode { get; set; }
        protected string SocialInsuranceNumberPatient { get; set; }
        // tìm kiếm
        protected string appointmentCode { get; set; }
        protected string codeFind { get; set; }
        protected string typeCodeFind__MaBN { get; set; }
        protected string typeCodeFind__MaHK { get; set; }
        protected string typeCodeFind__MaCT { get; set; }
        protected string typeCodeFind__SoThe { get; set; }
        protected string typeCodeFind__MaMS { get; set; }
        protected long patientId { get; set; }

        // UCPlusInfo
        protected long? cMNDDate { get; set; }
        protected string cMNDNumber { get; set; }
        protected string passPortNumber { get; set; }
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
        protected string mpsNationalCode { get; set; }
        protected string ethnicName { get; set; }
        protected string ethnicCode { get; set; }
        protected long militaryId { get; set; }
        protected object workPlace { get; set; }
        protected long? blood_ABO_ID { get; set; }
        protected string blood_ABO_Code { get; set; }
        protected long? blood_Rh_Id { get; set; }
        protected string blood_Rh_Code { get; set; }
        //protected string fatherName { get; set; }
        //protected string motherName { get; set; }
        protected string houseHold_Code { get; set; }
        protected string hoseHold_Relative { get; set; }
        protected long? houseHoldRelative_ID { get; set; }
        protected string maHoNgheo { get; set; }
        protected string patientStoreCode { get; set; }
        protected string taxCode { get; set; }

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
        protected string fatherName { get; set; }
        protected string motherName { get; set; }
        protected string relativeType { get; set; }
        protected string relativePhone { get; set; }
        protected string relativeCMNDNumber { get; set; }
        protected string religionName { get; set; }
        protected bool? IsNeedSickLeaveCert { get; set; }
        protected decimal? weight { get; set; }
        protected decimal? height { get; set; }

        // UCOtherSerViceReqInfo
        protected long intructionTime { get; set; }
        protected long treatmentTypeId { get; set; }
        protected long oweTypeId { get; set; }
        public bool isPriority { get; set; }
        protected bool chkEmergency { get; set; }
        protected bool isNotPatientDayDob { get; set; }
        protected bool chkChronic { get; set; }
        protected bool chkTuberculosis { get; set; }
        protected long emergencyWTimeId { get; set; }
        protected long departmentId { get; set; }
        protected short? isNotRequireFee { get; set; }
        protected long? priority { get; set; }
        protected long? priorityTypeId { get; set; }
        protected int registerNumber = 0;
        protected long? priorityNumber { get; set; }
        protected long? treatmentOrder { get; set; }
        protected bool chkIsCapMaMS { get; set; }
        protected string mSCode { get; set; }
        protected long? otherPaySourceId { get; set; }
        protected string inCode { get; set; }
        protected long? patientClassifyId { get; set; }
        protected string HospitalizeReasonCode { get; set; }
        protected string HospitalizeReasonName { get; set; }
        protected string HospitalizationReason { get; set; }
        public string GUARANTEE_LOGINNAME { get; set; }
        public string GUARANTEE_USERNAME { get; set; }
        public string GUARANTEE_REASON { get; set; }
        public string NOTE { get; set; }
        protected long FUND_ID { get; set; }
        protected string FUND_NUMBER { get; set; }
        protected decimal? FUND_BUDGET { get; set; }
        protected string FUND_COMPANY_NAME { get; set; }
        protected long? FUND_FROM_TIME { get; set; }
        protected long? FUND_TO_TIME { get; set; }
        protected long? FUND_ISSUE_TIME { get; set; }
        protected string FUND_TYPE_NAME { get; set; }
        protected string FUND_CUSTOMER_NAME { get; set; }
        protected bool IsWarningForNext { get; set; }
        protected bool IsHiv { get; set; }
        //UcheinInfo
        protected short? isBhytHolded { get; set; }

        // UCTransPatin
        protected string icd_Code { get; set; }
        protected string icd_Name { get; set; }
        protected string icd_Text { get; set; }
        protected string icd_Sub_Code { get; set; }
        protected string icd_Sub_Name { get; set; }
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
        protected long? transferInTimeFrom { get; set; }
        protected long? transferInTimeTo { get; set; }
        protected short? transferInReviews { get; set; }
        protected byte[] ImgTransferInData { get; set; }

        // UCImage
        protected byte[] img_avatar { get; set; }
        protected byte[] img_BHYT { get; set; }
        protected byte[] FileImageCMNDTruoc { get; set; }
        protected byte[] FileImageCMNDSau { get; set; }

        //KskContract
        protected long? kskContractId { get; set; }

        //Hrm
        protected string hrmEmployeeCode { get; set; }
        protected string hrmKskCode { get; set; }

        // Declare
        protected HisCardSDO cardSearch { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPatientExtendADO patientInformationADO { get; set; }
        protected HisPatientSDO patientData { get; set; }
        protected HisPatientProfileSDO patientProfile { get; set; }
        public bool isCheckSS { get; set; }
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
        protected MainHisHeinBhyt uCMainHein { get; set; }
        protected bool chkAutoCreateBill { get; set; }
        protected bool chkAutoDeposit { get; set; }
        protected bool chkAutoPay { get; set; }
        protected bool chkExamOnline { get; set; }
        protected long? cashierRoom_RoomId { get; set; }

        internal ServiceRequestRegisterBehaviorBase(CommonParam param, UCRegister ucServiceRequestRegiter)
            : base(param)
        {
            try
            {
                // Get Data From UC
                this.ucRequestService = ucServiceRequestRegiter;
                this.heinInfoValue = ucServiceRequestRegiter.ucHeinInfo1.GetValue();
                this.patientRawInfoValue = ucServiceRequestRegiter.ucPatientRaw1.GetValue();
                this.addressInfoValue = ucServiceRequestRegiter.ucAddressCombo1.GetValue();
                this.serviceReqInfoValue = ucServiceRequestRegiter.ucOtherServiceReqInfo1.GetValue();
                this.patientPlusInformationInfoValue = ucServiceRequestRegiter.ucPlusInfo1.GetValue();
                this.relativeInfoValue = ucServiceRequestRegiter.ucRelativeInfo1.GetValue();
                this.serviceRoomInfoValue = ucServiceRequestRegiter.ucServiceRoomInfo1.GetDetail();
                this.imageADO = ucServiceRequestRegiter.ucImageInfo1.GetValue();
                this.UCTransPatiADO = ucServiceRequestRegiter.transPatiADO;
                this.currentModule = ucServiceRequestRegiter.currentModule;
                this.uCMainHein = ucServiceRequestRegiter.mainHeinProcessor;
                // UCPatientRaw
                this.PeopleCode = this.patientRawInfoValue.PERSON_CODE;
                this.patientName = this.patientRawInfoValue.PATIENT_NAME;
                this.patient_Last_Name = this.patientRawInfoValue.PATIENT_LAST_NAME;
                this.patient_First_Name = this.patientRawInfoValue.PATIENT_FIRST_NAME;
                if (this.patientRawInfoValue.GENDER_ID > 0)
                    this.GenderId = this.patientRawInfoValue.GENDER_ID;
                this.dob = this.patientRawInfoValue.DOB;
                this.patientTypeId = this.patientRawInfoValue.PATIENTTYPE_ID;
                if (this.patientRawInfoValue.CARRER_ID != null && this.patientRawInfoValue.CARRER_ID>0)
                    this.careerId = (this.patientRawInfoValue.CARRER_ID);
                this.careerCode = this.patientRawInfoValue.CARRER_CODE;
                this.careerName = this.patientRawInfoValue.CARRER_NAME;
                this.iS_Has_Not_Day_Dob = this.patientRawInfoValue.IS_HAS_NOT_DAY_DOB;
                this.isNotPatientDayDob = (this.patientRawInfoValue.IS_HAS_NOT_DAY_DOB == GlobalVariables.CommonNumberTrue);
                this.hrmEmployeeCode = this.patientRawInfoValue.EMPLOYEE_CODE;
                if (this.patientRawInfoValue.MILITARY_RANK_ID.HasValue)
                {
                    this.militaryId = this.patientRawInfoValue.MILITARY_RANK_ID.Value;
                }
                if (this.patientRawInfoValue.PATIENT_CLASSIFY_ID.HasValue)
                {
                    this.patientClassifyId = this.patientRawInfoValue.PATIENT_CLASSIFY_ID.Value;
                }
                if (this.patientRawInfoValue.POSITION_ID.HasValue)
                {
                    this.PositionId = this.patientRawInfoValue.POSITION_ID.Value;
                }
                if (this.patientRawInfoValue.WORK_PLACE_ID.HasValue)
                {
                    this.workPlace = this.patientRawInfoValue.WORK_PLACE_ID.Value;
                }
                this.lstPreviousDebtTreatments = this.patientRawInfoValue.lstPreviousDebtTreatments;
                if (this.patientRawInfoValue.ReceptionForm.HasValue)
                {
                    this.receptionForm = this.patientRawInfoValue.ReceptionForm.Value;
                }
                this.CardCode = this.patientRawInfoValue.CardCode;
                this.CardServiceCode = this.patientRawInfoValue.CardServiceCode;
                this.BankCardCode = this.patientRawInfoValue.BankCardCode;
                this.SocialInsuranceNumberPatient = this.patientRawInfoValue.SocialInsuranceNumberPatient;
                // UCAddress
                this.address = this.addressInfoValue.Address;
                this.communeName = (string)(this.addressInfoValue.Commune_Name);
                this.communeCode = (string)(this.addressInfoValue.Commune_Code);
                this.districtName = (string)(this.addressInfoValue.District_Name);
                this.districtCode = (string)(this.addressInfoValue.District_Code);
                this.provinceName = (string)(this.addressInfoValue.Province_Name);
                this.provinceCode = (string)(this.addressInfoValue.Province_Code);
                this.phone = this.addressInfoValue.Phone;
                //this.weight = this.addressInfoValue.Weight;
                //this.height = this.addressInfoValue.Height;

                // UCOtherServiceReqInfo
                this.chkExamOnline = this.serviceReqInfoValue.IsExamOnline;
                this.chkEmergency = this.serviceReqInfoValue.IsEmergency;
                this.intructionTime = this.serviceReqInfoValue.IntructionTime;
                if (this.serviceReqInfoValue.TreatmentType_ID > 0)
                    this.treatmentTypeId = this.serviceReqInfoValue.TreatmentType_ID;
                this.isPriority = this.serviceReqInfoValue.IsPriority;
                if (this.serviceReqInfoValue.PriorityType.HasValue && this.serviceReqInfoValue.PriorityType > 0)
                    this.priorityTypeId = this.serviceReqInfoValue.PriorityType;
                this.chkChronic = this.serviceReqInfoValue.IsChronic;
                this.chkTuberculosis = this.serviceReqInfoValue.IsTuberCulosis;
                if (this.serviceReqInfoValue.OweType_ID > 0)
                    this.oweTypeId = this.serviceReqInfoValue.OweType_ID;
                if (this.serviceReqInfoValue.IsEmergency && this.serviceReqInfoValue.EmergencyTime_ID > 0)
                    this.emergencyWTimeId = this.serviceReqInfoValue.EmergencyTime_ID;
                if (this.serviceReqInfoValue.OTHER_PAY_SOURCE_ID > 0)
                    this.otherPaySourceId = this.serviceReqInfoValue.OTHER_PAY_SOURCE_ID;
                this.inCode = this.serviceReqInfoValue.IN_CODE;
                if (this.serviceReqInfoValue.PATIENT_CLASSIFY_ID.HasValue && !this.patientClassifyId.HasValue)
                    this.patientClassifyId = this.serviceReqInfoValue.PATIENT_CLASSIFY_ID;

                this.GUARANTEE_LOGINNAME = this.serviceReqInfoValue.GUARANTEE_LOGINNAME;
                this.GUARANTEE_USERNAME = this.serviceReqInfoValue.GUARANTEE_USERNAME;
                this.GUARANTEE_REASON = this.serviceReqInfoValue.GUARANTEE_REASON;
                this.NOTE = this.serviceReqInfoValue.NOTE;
                this.IsWarningForNext = this.serviceReqInfoValue.IsWarningForNext;
                this.IsHiv = this.serviceReqInfoValue.IsHiv;
                this.treatmentOrder = this.serviceReqInfoValue.TreatmentOrder;
                this.chkIsCapMaMS = this.serviceReqInfoValue.IsCapMaMS;
                this.mSCode = this.serviceReqInfoValue.MaMS;

                this.FUND_ID = this.serviceReqInfoValue.FUND_ID;
                this.FUND_BUDGET = this.serviceReqInfoValue.FUND_BUDGET;
                this.FUND_COMPANY_NAME = this.serviceReqInfoValue.FUND_COMPANY_NAME;
                this.FUND_FROM_TIME = this.serviceReqInfoValue.FUND_FROM_TIME;
                this.FUND_ISSUE_TIME = this.serviceReqInfoValue.FUND_ISSUE_TIME;
                this.FUND_NUMBER = this.serviceReqInfoValue.FUND_NUMBER;
                this.FUND_TO_TIME = this.serviceReqInfoValue.FUND_TO_TIME;
                this.FUND_TYPE_NAME = this.serviceReqInfoValue.FUND_TYPE_NAME;
                this.FUND_CUSTOMER_NAME = this.serviceReqInfoValue.FUND_CUSTOMER_NAME;
                this.HospitalizeReasonCode = this.serviceReqInfoValue.HospitalizeReasonCode;
                this.HospitalizeReasonName = this.serviceReqInfoValue.HospitalizeReasonName;
                this.HospitalizationReason = this.serviceReqInfoValue.HospitalizationReason;
                //UcheinInfo
                if (this.heinInfoValue != null && this.heinInfoValue.HisTreatment != null)
                {
                    this.isBhytHolded = this.heinInfoValue.HisTreatment.IS_BHYT_HOLDED;
                }
                this.isCheckSS = ucServiceRequestRegiter.isCheckSS;
                // UCPlusInfo
                this.born_provinceCode = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_CODE;
                this.born_provinceName = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_NAME;
                this.districtCodeKS = patientPlusInformationInfoValue.DISTRICT_OfBIRTH_CODE;
                this.districtNameKS = patientPlusInformationInfoValue.DISTRICT_OfBIRTH_NAME;
                this.communeCodeKS = patientPlusInformationInfoValue.COMMUNE_OfBIRTH_CODE;
                this.communeNameKS = patientPlusInformationInfoValue.COMMUNE_OfBIRTH_NAME;
                this.addressKS = patientPlusInformationInfoValue.ADDRESS_OfBIRTH;

                this.communeNowCode = ucRequestService.IsReadCardTheViet ? ucRequestService.HtCommuneCode : patientPlusInformationInfoValue.HT_COMMUNE_CODE;
                this.communeNowName = ucRequestService.IsReadCardTheViet ? ucRequestService.HtCommuneName : patientPlusInformationInfoValue.HT_COMMUNE_NAME;
                this.provinceNowCode = ucRequestService.IsReadCardTheViet ? ucRequestService.HtProvinceCode : patientPlusInformationInfoValue.HT_PROVINCE_CODE;
                this.provinceNowName = ucRequestService.IsReadCardTheViet ? ucRequestService.HtProvinceName : patientPlusInformationInfoValue.HT_PROVINCE_NAME;
                this.districtNowCode = ucRequestService.IsReadCardTheViet ? ucRequestService.HtDistrictCode : patientPlusInformationInfoValue.HT_DISTRICT_CODE;
                this.districtNowName = ucRequestService.IsReadCardTheViet ? ucRequestService.HtDistrictName : patientPlusInformationInfoValue.HT_DISTRICT_NAME;
                this.addressNow = patientPlusInformationInfoValue.HT_ADDRESS;

                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.ChangeEthnic != 0)
                {
                    this.ethnicName = this.patientRawInfoValue.ETHNIC_NAME;
                    this.ethnicCode = this.patientRawInfoValue.ETHNIC_CODE;
                }
                else
                {
                    this.ethnicName = this.patientPlusInformationInfoValue.ETHNIC_NAME;
                    this.ethnicCode = this.patientPlusInformationInfoValue.ETHNIC_CODE;
                }


                if (this.patientPlusInformationInfoValue.MILITARYRANK_ID.HasValue && this.militaryId == 0)
                    this.militaryId = (long)(this.patientPlusInformationInfoValue.MILITARYRANK_ID);
                this.nationalName = this.patientPlusInformationInfoValue.NATIONAL_NAME;
                this.nationalCode = this.patientPlusInformationInfoValue.NATIONAL_CODE;
                this.mpsNationalCode = this.patientPlusInformationInfoValue.MPS_NATIONAL_CODE;
                this.programId = this.patientPlusInformationInfoValue.PROGRAM_ID;
                this.programCode = this.patientPlusInformationInfoValue.PROGRAM_CODE;
                if (string.IsNullOrWhiteSpace(this.phone))
                {
                    this.phone = this.patientPlusInformationInfoValue.PHONE_NUMBER;
                }
                if (this.workPlace == null)
                {
                    this.workPlace = this.patientPlusInformationInfoValue.workPlace;
                }
                this.email = this.patientPlusInformationInfoValue.EMAIL;
                this.blood_ABO_Code = this.patientPlusInformationInfoValue.BLOOD_ABO_CODE;
                this.blood_ABO_ID = this.patientPlusInformationInfoValue.BLOOD_ABO_ID;
                this.blood_Rh_Code = this.patientPlusInformationInfoValue.BLOOD_RH_CODE;
                this.blood_Rh_Id = this.patientPlusInformationInfoValue.BLOOD_RH_ID;
                if (!string.IsNullOrEmpty(this.patientPlusInformationInfoValue.CMND_NUMBER))
                {
                    this.cMNDNumber = this.patientPlusInformationInfoValue.CMND_NUMBER;
                }
                else if (!string.IsNullOrEmpty(this.patientPlusInformationInfoValue.CCCD_NUMBER))
                {
                    this.cCCDNumber = this.patientPlusInformationInfoValue.CCCD_NUMBER;
                }
                else if (!string.IsNullOrEmpty(this.patientPlusInformationInfoValue.PASSPORT_NUMBER))
                {
                    this.passPortNumber = this.patientPlusInformationInfoValue.PASSPORT_NUMBER;
                }
                this.cMNDDate = this.patientPlusInformationInfoValue.CMND_DATE;
                this.cMNDPlace = this.patientPlusInformationInfoValue.CMND_PLACE;
                this.houseHold_Code = this.patientPlusInformationInfoValue.HOUSEHOLD_CODE;
                this.houseHoldRelative_ID = this.patientPlusInformationInfoValue.HOUSEHOLD_RELATION_ID;
                this.hoseHold_Relative = this.patientPlusInformationInfoValue.HOUSEHOLD_RELATION_NAME;
                this.maHoNgheo = this.patientPlusInformationInfoValue.HONGHEO_CODE == "" ? "" : this.patientPlusInformationInfoValue.HONGHEO_CODE;
                this.patientStoreCode = this.patientPlusInformationInfoValue.PATIENT_STORE_CODE;
                this.hrmKskCode = this.patientPlusInformationInfoValue.HRM_KSK_CODE;
                this.taxCode = this.patientPlusInformationInfoValue.TAX_CODE;

                //UCRelative
                this.relativeAddress = this.relativeInfoValue.RelativeAddress;
                this.relativeType = this.relativeInfoValue.Correlated;
                this.relativePhone = this.relativeInfoValue.RelativePhone;
                this.fatherName = this.relativeInfoValue.FatherName;
                this.motherName = this.relativeInfoValue.MotherName;
                this.relativeName = this.relativeInfoValue.RelativeName;
                this.relativeCMNDNumber = this.relativeInfoValue.RelativeCMND;
                this.IsNeedSickLeaveCert = this.relativeInfoValue.IsNeedSickLeaveCert;
                this.religionName = "";

                //UCImage
                this.departmentId = GlobalStore.DepartmentId;
                if (this.imageADO != null && this.imageADO.ListImageData != null && this.imageADO.ListImageData.Count > 0)
                {
                    foreach (var image in this.imageADO.ListImageData)
                    {
                        switch (image.Type)
                        {
                            case HIS.UC.UCImageInfo.Base.ImageType.CHAN_DUNG:
                                this.img_avatar = image.FileImage;
                                break;
                            case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_SAU:
                                this.FileImageCMNDSau = image.FileImage;
                                break;
                            case HIS.UC.UCImageInfo.Base.ImageType.CMND_CCCD_TRUOC:
                                this.FileImageCMNDTruoc = image.FileImage;
                                break;
                            case HIS.UC.UCImageInfo.Base.ImageType.THE_BHYT:
                                this.img_BHYT = image.FileImage;
                                break;
                            default:
                                break;
                        }
                    }
                }
                //this.img_avatar = this.imageADO.FileImagePortrait;
                //this.img_BHYT = this.imageADO.FileImageHein;
                //this.FileImageCMNDTruoc = this.imageADO.FileImageCMNDTruoc;//TODO
                //this.FileImageCMNDSau = this.imageADO.FileImageCMNDSau;//TODO

                //UCTransPati
                if (UCTransPatiADO != null)
                {
                    this.icd_Code = this.UCTransPatiADO.ICD_CODE;
                    this.icd_Name = this.UCTransPatiADO.ICD_NAME;
                    this.icd_Text = this.UCTransPatiADO.ICD_TEXT;
                    this.icd_Sub_Code = this.UCTransPatiADO.ICD_SUB_CODE;
                    this.icd_Sub_Name = this.UCTransPatiADO.ICD_SUB_NAME;
                    this.noiChuyenDen_Code = this.UCTransPatiADO.NOICHUYENDEN_CODE;
                    this.noiChuyenDen_Name = this.UCTransPatiADO.NOICHUYENDEN_NAME;
                    this.soChuyenVien = this.UCTransPatiADO.SOCHUYENVIEN;
                    this.right_Router_Type = this.UCTransPatiADO.RIGHT_ROUTER_TYPE;
                    this.hinhThucChuyen_ID = this.UCTransPatiADO.HINHTHUCHUYEN_ID;
                    this.lyDoChuyen_ID = this.UCTransPatiADO.LYDOCHUYEN_ID;
                    this.transfer_In_CMKT = this.UCTransPatiADO.TRANSFER_IN_CMKT;
                    this.isHasDialogText = this.UCTransPatiADO.IsHasDialogText;
                    this.isDisablelblEditICD = this.UCTransPatiADO.IsDisablelblEditICD;
                    this.transferInTimeFrom = this.UCTransPatiADO.TRANSFER_IN_TIME_FROM;
                    this.transferInTimeTo = this.UCTransPatiADO.TRANSFER_IN_TIME_TO;
                    this.transferInReviews = this.UCTransPatiADO.TRANSFER_IN_REVIEWS;
                    this.ImgTransferInData = this.UCTransPatiADO.ImgTransferInData;
                }

                // Other
                this.patientId = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.ID : 0);
                this.cardSearch = this.ucRequestService.cardSearch;
                if (cardSearch != null && string.IsNullOrEmpty(this.PeopleCode))
                    this.PeopleCode = cardSearch.PersonCode;
                this.patientData = this.ucRequestService.currentPatientSDO;
                this.appointmentCode = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.AppointmentCode : "");

                if (this.patientId <= 0)
                {
                    this.patientId = patientRawInfoValue.PATIENT_ID;
                    this.patientCode = patientRawInfoValue.PATIENT_CODE;
                }
                //kskContract
                if (this.ucRequestService.ucKskContract != null && this.ucRequestService.kskContractProcessor != null)
                {
                    KskContractOutput kskContractOutput = (KskContractOutput)this.ucRequestService.kskContractProcessor.GetValue(this.ucRequestService.ucKskContract);
                    if (kskContractOutput != null && kskContractOutput.IsVali && kskContractOutput.KskContract != null)
                        this.kskContractId = kskContractOutput.KskContract.ID;
                }

                this.chkAutoCreateBill = ucServiceRequestRegiter.chkAutoCreateBill.Checked;
                this.chkAutoDeposit = ucServiceRequestRegiter.chkAutoDeposit.Checked;
                this.chkAutoPay = ucServiceRequestRegiter.chkAutoPay.Checked;
                if (ucServiceRequestRegiter.cboCashierRoom.EditValue != null)
                {
                    V_HIS_CASHIER_ROOM cashierRoom = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ID == Convert.ToInt64(ucServiceRequestRegiter.cboCashierRoom.EditValue));
                    if (cashierRoom != null)
                        this.cashierRoom_RoomId = cashierRoom.ROOM_ID;
                }
                else
                {
                    if (GlobalVariables.SessionInfo != null)
                    {
                        this.cashierRoom_RoomId = GlobalVariables.SessionInfo.CashierWorkingRoomId;
                    }
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
                //bool valid = true;
                if (data == null) throw new ArgumentNullException("Input data is null");
                if (this.ucRequestService == null) throw new ArgumentNullException("ucRequestService is null");

                //valid = CheckProfile();

                if (CheckProfile())
                {
                    this.ucRequestService.ucAddressCombo1.FocusToProvince();
                    WaitingManager.Hide();
                    param.Messages.Add(ResourceMessage.TreEmCoGiayKhaiSinhPhaiNhapThongTinHanhChinh);
                }
                else
                {
                    CallSyncHID();
                    this.ucRequestService.ucServiceRoomInfo1.GetDetail();
                    LogSystem.Debug("RunBase => begin call api____Input data:");
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    if (data.GetType() == typeof(HisServiceReqExamRegisterSDO))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Tiep don goi api: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER), HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER));
                        return new BackendAdapter(param).Post<HisServiceReqExamRegisterResultSDO>(HisRequestUriStore.HIS_SERVICE_REQ_EXAMREGISTER, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }
                    if (data.GetType() == typeof(HisPatientProfileSDO))
                    {
                        return new BackendAdapter(param).Post<HisPatientProfileSDO>(HisRequestUriStore.HIS_PATIENT_REGISTER_PROFILE, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    }
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
                this.patientProfile.HisPatient.IS_HIV = this.IsHiv ? (short?)1 : null;
                this.patientProfile.HisPatient.BRANCH_ID = WorkPlace.GetBranchId();
                //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                if (!String.IsNullOrEmpty(this.cMNDNumber))
               {                   
                        this.patientProfile.HisPatient.CMND_DATE = this.cMNDDate;
                        this.patientProfile.HisPatient.CMND_NUMBER = this.cMNDNumber;
                        this.patientProfile.HisPatient.CMND_PLACE = this.cMNDPlace;
                }
                else if (!String.IsNullOrEmpty(this.cCCDNumber))
                {
                    this.patientProfile.HisPatient.CCCD_DATE = this.cMNDDate;
                    this.patientProfile.HisPatient.CCCD_NUMBER = this.cCCDNumber;
                    this.patientProfile.HisPatient.CCCD_PLACE = this.cMNDPlace;
                }
                else if (!String.IsNullOrEmpty(this.passPortNumber))
                {
                    this.patientProfile.HisPatient.PASSPORT_DATE = this.cMNDDate;
                    this.patientProfile.HisPatient.PASSPORT_NUMBER = this.passPortNumber;
                    this.patientProfile.HisPatient.PASSPORT_PLACE = this.cMNDPlace;
                }
                this.patientProfile.HisPatient.COMMUNE_CODE = this.communeCode;
                this.patientProfile.HisPatient.HT_ADDRESS = this.addressNow;
                this.patientProfile.HisPatient.HT_COMMUNE_NAME = this.communeNowName;
                this.patientProfile.HisPatient.HT_DISTRICT_NAME = this.districtNowName;
                this.patientProfile.HisPatient.HT_PROVINCE_NAME = this.provinceNowName;
                this.patientProfile.HisPatient.HT_COMMUNE_CODE = this.communeNowCode;
                this.patientProfile.HisPatient.HT_DISTRICT_CODE = this.districtNowCode;
                this.patientProfile.HisPatient.HT_PROVINCE_CODE = this.provinceNowCode;
                this.patientProfile.HisPatient.RELATIVE_MOBILE = this.phone;

                this.patientProfile.HisPatient.BLOOD_ABO_CODE = this.blood_ABO_Code;
                this.patientProfile.HisPatient.BLOOD_RH_CODE = this.blood_Rh_Code;

                this.patientProfile.HisPatient.RELATIVE_ADDRESS = this.relativeAddress;
                this.patientProfile.HisPatient.RELATIVE_NAME = this.relativeName;
                this.patientProfile.HisPatient.FATHER_NAME = this.fatherName;
                this.patientProfile.HisPatient.MOTHER_NAME = this.motherName;
                this.patientProfile.HisPatient.RELATIVE_TYPE = this.relativeType;
                this.patientProfile.HisPatient.RELATIVE_PHONE = this.relativePhone;
                this.patientProfile.HisPatient.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                this.patientProfile.HisPatient.BORN_PROVINCE_CODE = GenerateProvinceCode(this.born_provinceCode);//(*)
                this.patientProfile.HisPatient.BORN_PROVINCE_NAME = this.born_provinceName;//(*)
                this.patientProfile.HisPatient.FIRST_NAME = this.patient_First_Name;
                this.patientProfile.HisPatient.LAST_NAME = this.patient_Last_Name;
                this.patientProfile.HisPatient.PERSON_CODE = this.PeopleCode;

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
                this.patientProfile.HisPatient.MPS_NATIONAL_CODE = this.mpsNationalCode;
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
                else
                    this.patientProfile.HisPatient.IS_CHRONIC = null;
                this.patientProfile.HisPatient.IS_TUBERCULOSIS = chkTuberculosis ? (short?)1 : 0;
                this.patientProfile.ImgAvatarData = this.img_avatar;
                this.patientProfile.ImgBhytData = this.img_BHYT;
                this.patientProfile.ImgCmndBeforeData = this.FileImageCMNDTruoc;//TODO
                this.patientProfile.ImgCmndAfterData = this.FileImageCMNDSau;//TODO

                this.patientProfile.HisPatient.HRM_EMPLOYEE_CODE = this.hrmEmployeeCode;
                this.patientProfile.HisPatient.TAX_CODE = this.taxCode;
                //this.patientProfile.HisPatient.MS_CODE = this.mSCode;//TODO
                this.patientProfile.HisPatient.PATIENT_CLASSIFY_ID = this.patientClassifyId;
                this.patientProfile.HisPatient.POSITION_ID = this.PositionId;
                this.patientProfile.HisPatient.SOCIAL_INSURANCE_NUMBER = SocialInsuranceNumberPatient;
                if (!string.IsNullOrEmpty(patientCode))
                {
                    this.patientProfile.HisPatient.PATIENT_CODE = this.patientCode;
                }
                if (IsWarningForNext)
                    this.patientProfile.HisPatient.NOTE = this.NOTE;
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
                if (this.otherPaySourceId > 0)
                {
                    this.patientProfile.HisTreatment.OTHER_PAY_SOURCE_ID = this.otherPaySourceId;
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
                if (this.UCTransPatiADO != null
                                && (!string.IsNullOrEmpty(this.icd_Code)
                                || !string.IsNullOrEmpty(this.icd_Text)
                                || !string.IsNullOrEmpty(this.icd_Name)
                                || !string.IsNullOrEmpty(this.noiChuyenDen_Code)
                                || !string.IsNullOrEmpty(this.noiChuyenDen_Name)
                                || this.hinhThucChuyen_ID > 0
                                || !string.IsNullOrEmpty(this.soChuyenVien)
                                || this.transfer_In_CMKT > 0
                                || this.ImgTransferInData != null
                                || !string.IsNullOrEmpty(this.icd_Sub_Code)
                                || !string.IsNullOrEmpty(this.icd_Sub_Name))
                                )
                {
                    this.patientProfile.HisTreatment.IS_TRANSFER_IN = 1;
                }
                if (this.ImgTransferInData != null)
                    this.patientProfile.ImgTransferInData = this.ImgTransferInData;
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_CODE = this.icd_Code;
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_NAME = (!String.IsNullOrEmpty(this.icd_Text) ? this.icd_Text : this.icd_Name);
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_SUB_CODE = this.icd_Sub_Code;
                this.patientProfile.HisTreatment.TRANSFER_IN_ICD_TEXT = this.icd_Sub_Name;
                this.patientProfile.HisTreatment.TRANSFER_IN_MEDI_ORG_CODE = this.noiChuyenDen_Code;
                this.patientProfile.HisTreatment.TRANSFER_IN_MEDI_ORG_NAME = this.noiChuyenDen_Name;
                this.patientProfile.HisTreatment.TRANSFER_IN_FORM_ID = this.hinhThucChuyen_ID;
                this.patientProfile.HisTreatment.TRANSFER_IN_REASON_ID = this.lyDoChuyen_ID;
                this.patientProfile.HisTreatment.TRANSFER_IN_CODE = this.soChuyenVien;
                this.patientProfile.HisTreatment.TRANSFER_IN_CMKT = this.transfer_In_CMKT;
                this.patientProfile.HisTreatment.HRM_KSK_CODE = this.hrmKskCode;
                this.patientProfile.HisTreatment.TRANSFER_IN_TIME_FROM = this.transferInTimeFrom;
                this.patientProfile.HisTreatment.TRANSFER_IN_TIME_TO = this.transferInTimeTo;
                this.patientProfile.HisTreatment.TRANSFER_IN_REVIEWS = this.transferInReviews;
                this.patientProfile.HisTreatment.TREATMENT_ORDER = this.treatmentOrder;
                this.patientProfile.HisTreatment.IN_CODE = this.inCode;
                this.patientProfile.HisTreatment.IS_BHYT_HOLDED = this.isBhytHolded;
                this.patientProfile.HisTreatment.IS_HIV = this.IsHiv ? (short?)1 : null;
                //Du lieu doi tuong cung chi tra
                if (this.FUND_ID > 0)
                {
                    this.patientProfile.HisTreatment.FUND_ID = this.FUND_ID;
                    this.patientProfile.HisTreatment.FUND_BUDGET = this.FUND_BUDGET;
                    this.patientProfile.HisTreatment.FUND_COMPANY_NAME = this.FUND_COMPANY_NAME;
                    this.patientProfile.HisTreatment.FUND_FROM_TIME = this.FUND_FROM_TIME;
                    this.patientProfile.HisTreatment.FUND_ISSUE_TIME = this.FUND_ISSUE_TIME;
                    this.patientProfile.HisTreatment.FUND_NUMBER = this.FUND_NUMBER;
                    this.patientProfile.HisTreatment.FUND_TO_TIME = this.FUND_TO_TIME;
                    this.patientProfile.HisTreatment.FUND_TYPE_NAME = this.FUND_TYPE_NAME;
                    this.patientProfile.HisTreatment.FUND_CUSTOMER_NAME = this.FUND_CUSTOMER_NAME;
                }
                if (this.IsNeedSickLeaveCert.HasValue)
                    this.patientProfile.HisTreatment.NEED_SICK_LEAVE_CERT = this.IsNeedSickLeaveCert.Value ? (short?)1 : null;
                if (this.receptionForm.HasValue)
                {
                    this.patientProfile.HisTreatment.RECEPTION_FORM = this.receptionForm;
                }
                this.patientProfile.HisTreatment.TDL_SOCIAL_INSURANCE_NUMBER = SocialInsuranceNumberPatient;
                this.patientProfile.HisTreatment.HOSPITALIZE_REASON_CODE = this.HospitalizeReasonCode;
                this.patientProfile.HisTreatment.HOSPITALIZE_REASON_NAME = this.HospitalizeReasonName;
                this.patientProfile.HisTreatment.HOSPITALIZATION_REASON = this.HospitalizationReason;
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
                    //this.uCMainHein.UpdateDataFormIntoPatientTypeAlter(this.ucHein__BHYT, dataPatientProfile);

                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HisConfigCFG.PatientTypeId__QN)
                    {
                        dataPatientProfile.HisPatientTypeAlter = this.heinInfoValue.HisPatientTypeAlter;
                    }

                    if (patientProfile.HisPatientTypeAlter == null)
                    {
                        patientProfile.HisPatientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    }
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_PATIENT_TYPE_ALTER>(patientProfile.HisPatientTypeAlter, dataPatientProfile.HisPatientTypeAlter);
                    //patientProfile.HisPatientTypeAlter = Mapper.Map<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER, MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER>(dataPatientProfile.HisPatientTypeAlter);

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
                    {
                        patientProfile.CardCode = this.cardSearch.CardCode;
                        patientProfile.CardServiceCode = this.cardSearch.ServiceCode;
                        patientProfile.BankCardCode = this.cardSearch.BankCardCode;
                    }
                    else if (!string.IsNullOrEmpty(CardCode))
                    {
                        patientProfile.CardCode = this.CardCode;
                        patientProfile.CardServiceCode = this.CardServiceCode;
                        patientProfile.BankCardCode = this.BankCardCode;
                    }
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

                    if (HisConfigCFG.IsSetPrimaryPatientType == "2" && this.patientRawInfoValue != null)
                    {
                        patientProfile.HisPatientTypeAlter.PRIMARY_PATIENT_TYPE_ID = this.patientRawInfoValue.PRIMARY_PATIENT_TYPE_ID;
                    }

                    patientProfile.HisPatientTypeAlter.GUARANTEE_LOGINNAME = this.GUARANTEE_LOGINNAME;
                    patientProfile.HisPatientTypeAlter.GUARANTEE_USERNAME = this.GUARANTEE_USERNAME;
                    patientProfile.HisPatientTypeAlter.GUARANTEE_REASON = this.GUARANTEE_REASON;
                    patientProfile.HisPatientTypeAlter.IS_NEWBORN = (short)(isCheckSS ? 1 : 0);
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
