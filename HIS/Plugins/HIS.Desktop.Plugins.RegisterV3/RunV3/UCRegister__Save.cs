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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using MOS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.LibraryMessage;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.UCServiceRoomInfo;
using HID.Filter;
using HID.EFMODEL.DataModels;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCRelativeInfo.ADO;
using DevExpress.XtraLayout.Utils;
using HIS.Desktop.Plugins.Library.HisSyncToHid;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        HisPatientVitaminASDO hisPatientVitaminASDODataSave;
        HisPatientVitaminASDO hisPatientVitaminASDOSave;
        protected long departmentId { get; set; }

        protected string provinceCodeKS { get; set; }
        protected string provinceNameKS { get; set; }
        protected string districtCodeKS { get; set; }
        protected string districtNameKS { get; set; }
        protected string communeCodeKS { get; set; }
        protected string communeNameKS { get; set; }
        protected string addressKS { get; set; }
        protected string hohName { get; set; }

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
        protected long? PositionId { get; set; }
        protected long? patientClassifyId { get; set; }

        // tìm kiếm
        protected string appointmentCodeSave { get; set; }
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
        protected string ethnicName { get; set; }
        protected string ethnicCode { get; set; }
        protected long militaryId { get; set; }
        protected object workPlace { get; set; }
        protected long? blood_ABO_ID { get; set; }
        protected string blood_ABO_Code { get; set; }
        protected long? blood_Rh_Id { get; set; }
        protected string blood_Rh_Code { get; set; }
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

        protected string personCode { get; set; }
        // UCImage
        protected byte[] img_avatar { get; set; }
        protected byte[] img_BHYT { get; set; }

        // Declare
        protected HisCardSDO cardSearchSave { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPatientExtendADO patientInformationADO { get; set; }
        protected HIS_PATIENT patientData { get; set; }
        protected UCRegister ucRequestService;
        protected UCPatientRawADO patientRawInfoValue { get; set; }
        protected UCAddressADO addressInfoValue { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPlusInfoADO patientPlusInformationInfoValue { get; set; }
        protected UCRelativeADO relativeInfoValue { get; set; }

        private void Save(bool printNow)
        {
            try
            {
                this.resultHisPatientProfileSDO = null;
                this.isShowMess = false;
                bool success = false;
                CommonParam param = new CommonParam();

                if (!dxValidationProvider1.Validate())
                    return;

                if (this.CheckValidateForSave(param))
                {
                    try
                    {
                        WaitingManager.Show();

                        hisPatientVitaminASDODataSave = new HisPatientVitaminASDO();
                        GetDataSaveFromUc(this, ref hisPatientVitaminASDODataSave);
                        HIS_DHST dhst = ProcessDhst();
                        hisPatientVitaminASDODataSave.HisDhst = dhst;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisPatientVitaminASDODataSave.HisDhst), hisPatientVitaminASDODataSave.HisDhst));
                        if (this.cardSearchSave != null && !String.IsNullOrEmpty(this.cardSearchSave.CardCode))
                            hisPatientVitaminASDODataSave.CardCode = this.cardSearchSave.CardCode;

                        if (this.ucRoomVitaminA != null)
                        {
                            var month = Inventec.Common.DateTime.Calculation.DifferenceMonth(hisPatientVitaminASDODataSave.HisPatient.DOB, Inventec.Common.DateTime.Get.Now() ?? 0);
                            if (month < 60 && hisPatientVitaminASDODataSave.HisVitaminA.CASE_TYPE != 1)
                            {
                                WaitingManager.Hide();
                                DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân là trẻ em, bắt buộc chọn trường hợp là trẻ em");
                                return;
                            }
                        }

						//CallSyncHID(hisPatientVitaminASDODataSave);

						Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisPatientVitaminASDODataSave), hisPatientVitaminASDODataSave));
                        hisPatientVitaminASDOSave = new BackendAdapter(param).Post<HisPatientVitaminASDO>("api/HisPatient/RegisterVitaminA", ApiConsumers.MosConsumer, hisPatientVitaminASDODataSave, param);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisPatientVitaminASDOSave), hisPatientVitaminASDOSave));
                        if (hisPatientVitaminASDOSave != null)
                        {
                            success = true;
                            this.EnableControl(false);
                            this.ucPatientRaw1.SetPatientCodeAfterSavePatient(hisPatientVitaminASDOSave);
                            FillDataToGridVitaminA(hisPatientVitaminASDOSave);
                            FillDataToGridVaccine(hisPatientVitaminASDOSave);
                            if (printNow)
                            {
                                this.isPrintNow = true;
                                Print();
                            }
                        }

                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                this.EnableControl(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataSaveFromUc(UCRegister ucServiceRequestRegiter, ref HisPatientVitaminASDO hisPatientVitaminASDO)
        {

            try
            {
                // Get Data From UC
                this.ucRequestService = ucServiceRequestRegiter;
                this.patientRawInfoValue = ucServiceRequestRegiter.ucPatientRaw1.GetValue();
                this.addressInfoValue = ucServiceRequestRegiter.ucAddressCombo1.GetValue();
                this.patientPlusInformationInfoValue = ucServiceRequestRegiter.ucPlusInfo1.GetValue();
                this.relativeInfoValue = ucServiceRequestRegiter.ucRelativeInfo1.GetValue();
                this.currentModule = ucServiceRequestRegiter.currentModule;

                // UCPatientRaw
                this.patientCode = this.patientRawInfoValue.PATIENT_CODE;
                this.personCode = this.patientRawInfoValue.PERSON_CODE;
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
                // UCAddress
                this.address = this.addressInfoValue.Address;
                this.communeName = (string)(this.addressInfoValue.Commune_Name);
                this.communeCode = (string)(this.addressInfoValue.Commune_Code);
                this.districtName = (string)(this.addressInfoValue.District_Name);
                this.districtCode = (string)(this.addressInfoValue.District_Code);
                this.provinceName = (string)(this.addressInfoValue.Province_Name);
                this.provinceCode = (string)(this.addressInfoValue.Province_Code);
                this.phone = this.addressInfoValue.Phone;

                // UCPlusInfo
                this.born_provinceCode = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_CODE;
                this.born_provinceName = patientPlusInformationInfoValue.PROVINCE_OfBIRTH_NAME;
                this.districtCodeKS = patientPlusInformationInfoValue.DISTRICT_OfBIRTH_CODE;
                this.districtNameKS = patientPlusInformationInfoValue.DISTRICT_OfBIRTH_NAME;
                this.communeCodeKS = patientPlusInformationInfoValue.COMMUNE_OfBIRTH_CODE;
                this.communeNameKS = patientPlusInformationInfoValue.COMMUNE_OfBIRTH_NAME;
                this.addressKS = patientPlusInformationInfoValue.ADDRESS_OfBIRTH;

                this.communeNowCode = patientPlusInformationInfoValue.HT_COMMUNE_CODE;
                this.communeNowName = patientPlusInformationInfoValue.HT_COMMUNE_NAME;
                this.provinceNowCode = patientPlusInformationInfoValue.HT_PROVINCE_CODE;
                this.provinceNowName = patientPlusInformationInfoValue.HT_PROVINCE_NAME;
                this.districtNowCode = patientPlusInformationInfoValue.HT_DISTRICT_CODE;
                this.districtNowName = patientPlusInformationInfoValue.HT_DISTRICT_NAME;
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


                // Other
                this.patientId = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.ID : 0);
                this.cardSearchSave = this.ucRequestService.cardSearch;
                this.patientData = this.ucRequestService.currentPatientSDO;
                this.appointmentCode = (this.ucRequestService.currentPatientSDO != null ? this.ucRequestService.currentPatientSDO.AppointmentCode : "");

                //UCImage
                this.departmentId = GlobalStore.DepartmentId;
                //this.img_avatar = this.imageADO.FileImagePortrait;
                //this.img_BHYT = this.imageADO.FileImageHein;

                HIS_PATIENT patient = ProcessPatientSave(ucServiceRequestRegiter);
                HIS_VITAMIN_A vitaminA = null;
                HIS_VACCINATION_EXAM vaccine = null;

                if (this.ucRoomVitaminA != null)
                {
                    vitaminA = ProcessVitaminASave(ucServiceRequestRegiter, this.dob);
                }

                if (this.ucRoomVaccine != null)
                {
                    vaccine = ProcessVaccineSave(ucServiceRequestRegiter);
                }
                hisPatientVitaminASDO.HisPatient = patient;
                hisPatientVitaminASDO.RequestRoomId = this.currentModule.RoomId;
                hisPatientVitaminASDO.HisVitaminA = vitaminA;
                hisPatientVitaminASDO.HisVaccinationExam = vaccine;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public object UcDHSTGetValue()
        {
            object result = null;
            try
            {
                DHSTADO outPut = new DHSTADO();
                if (dtExecuteTime.EditValue != null)
                    outPut.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                if (spinBloodPressureMax.EditValue != null)
                    outPut.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    outPut.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    outPut.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                if (spinHeight.EditValue != null)
                    outPut.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                if (spinChest.EditValue != null)
                    outPut.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (spinBelly.EditValue != null)
                    outPut.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinPulse.EditValue != null)
                    outPut.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    outPut.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    outPut.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    outPut.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                outPut.NOTE = txtNote.Text;
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private HIS_DHST ProcessDhst()
        {
            HIS_DHST outPut = new HIS_DHST();
            try
            {
                if (dtExecuteTime.EditValue != null)
                    outPut.EXECUTE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExecuteTime.DateTime);
                if (spinBloodPressureMax.EditValue != null)
                    outPut.BLOOD_PRESSURE_MAX = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMax.Value.ToString());
                if (spinBloodPressureMin.EditValue != null)
                    outPut.BLOOD_PRESSURE_MIN = Inventec.Common.TypeConvert.Parse.ToInt64(spinBloodPressureMin.Value.ToString());
                if (spinBreathRate.EditValue != null)
                    outPut.BREATH_RATE = Inventec.Common.Number.Get.RoundCurrency(spinBreathRate.Value, 2);
                if (spinHeight.EditValue != null)
                    outPut.HEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinHeight.Value, 2);
                if (spinChest.EditValue != null)
                    outPut.CHEST = Inventec.Common.Number.Get.RoundCurrency(spinChest.Value, 2);
                if (spinBelly.EditValue != null)
                    outPut.BELLY = Inventec.Common.Number.Get.RoundCurrency(spinBelly.Value, 2);
                if (spinPulse.EditValue != null)
                    outPut.PULSE = Inventec.Common.TypeConvert.Parse.ToInt64(spinPulse.Value.ToString());
                if (spinTemperature.EditValue != null)
                    outPut.TEMPERATURE = Inventec.Common.Number.Get.RoundCurrency(spinTemperature.Value, 2);
                if (spinWeight.EditValue != null)
                    outPut.WEIGHT = Inventec.Common.Number.Get.RoundCurrency(spinWeight.Value, 2);
                if (spinSPO2.EditValue != null)
                    outPut.SPO2 = Inventec.Common.Number.Get.RoundCurrency(spinSPO2.Value, 2) / 100;
                outPut.NOTE = txtNote.Text;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new HIS_DHST();
            }
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => outPut), outPut));
            return outPut;
        }

        private HIS_PATIENT ProcessPatientSave(UCRegister ucServiceRequestRegiter)
        {
            HIS_PATIENT rs = new HIS_PATIENT();

            try
            {
                rs.PATIENT_CODE = this.patientCode;
                rs.PERSON_CODE = this.personCode;
                rs.VIR_PATIENT_NAME = this.patientName;
                if (this.patientId != 0)
                    rs.ID = this.patientId;
                rs.IS_HAS_NOT_DAY_DOB = (short)(this.isNotPatientDayDob ? 1 : 0);
                rs.EMAIL = this.email;
                rs.PATIENT_STORE_CODE = this.patientStoreCode;
                rs.HOUSEHOLD_CODE = this.houseHold_Code;
                rs.HOUSEHOLD_RELATION_NAME = this.hoseHold_Relative;
                //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                if (!String.IsNullOrEmpty(this.cMNDNumber))
                {
                    rs.CMND_DATE = this.cMNDDate;
                    rs.CMND_NUMBER = this.cMNDNumber;
                    rs.CMND_PLACE = this.cMNDPlace;
                }
                else if (!String.IsNullOrEmpty(this.cCCDNumber))
                {
                    rs.CCCD_DATE = this.cMNDDate;
                    rs.CCCD_NUMBER = this.cCCDNumber;
                    rs.CCCD_PLACE = this.cMNDPlace;
                }
                else if (!String.IsNullOrEmpty(this.passPortNumber))
                {
                    rs.PASSPORT_DATE = this.cMNDDate;
                    rs.PASSPORT_NUMBER = this.passPortNumber;
                    rs.PASSPORT_PLACE = this.cMNDPlace;
                }
               
                rs.RELATIVE_MOBILE = this.phone;

                rs.BLOOD_ABO_CODE = this.blood_ABO_Code;
                rs.BLOOD_RH_CODE = this.blood_Rh_Code;

                rs.RELATIVE_ADDRESS = this.relativeAddress;
                rs.RELATIVE_NAME = this.relativeName;
                rs.MOTHER_NAME = this.motherName;
                rs.FATHER_NAME = this.fatherName;
                rs.RELATIVE_TYPE = this.relativeType;
                rs.RELATIVE_PHONE = this.relativePhone;

                rs.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                rs.BORN_PROVINCE_CODE = GenerateProvinceCode(this.born_provinceCode);//(*)
                rs.BORN_PROVINCE_NAME = this.born_provinceName;//(*)
                rs.FIRST_NAME = this.patient_First_Name;
                rs.LAST_NAME = this.patient_Last_Name;
                rs.DOB = this.dob;
                rs.GENDER_ID = this.GenderId;

                rs.ADDRESS = this.address;
                rs.PROVINCE_CODE = this.provinceCode;
                rs.PROVINCE_NAME = this.provinceName;
                rs.DISTRICT_CODE = this.districtCode;
                rs.DISTRICT_NAME = this.districtName;
                rs.COMMUNE_CODE = this.communeCode;
                rs.COMMUNE_NAME = this.communeName;

                if (careerId.HasValue)
                    rs.CAREER_ID = this.careerId.Value;
                rs.CAREER_NAME = this.careerName;
                rs.CAREER_CODE = this.careerCode;
               
                if (this.workPlace != null && (this.workPlace is long || this.workPlace is long?) && (long?)this.workPlace > 0)
                    rs.WORK_PLACE_ID = (long?)this.workPlace;
                else if (this.workPlace != null && this.workPlace is string)
                    rs.WORK_PLACE = (string)this.workPlace;
                if (this.militaryId > 0)
                    rs.MILITARY_RANK_ID = this.militaryId;
                if(this.patientClassifyId > 0)
                    rs.PATIENT_CLASSIFY_ID = this.patientClassifyId;
                if (this.PositionId > 0)
                    rs.POSITION_ID = PositionId;
                rs.PHONE = this.phone;

                rs.BORN_PROVINCE_CODE = this.born_provinceCode;
                rs.BORN_PROVINCE_NAME = this.born_provinceName;
                rs.HT_ADDRESS = this.addressNow;
                rs.HT_COMMUNE_NAME = this.communeNowName;
                rs.HT_DISTRICT_NAME = this.districtNowName;
                rs.HT_PROVINCE_NAME = this.provinceNowName;
                rs.ETHNIC_NAME = this.ethnicName;
                rs.ETHNIC_CODE = this.ethnicCode;
                rs.NATIONAL_NAME = this.nationalName;
                rs.NATIONAL_CODE = this.nationalCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new HIS_PATIENT();
            }

            return rs;
        }

        private void CallSyncHID(HisPatientVitaminASDO dataVitaminASDO)
        {
            try
            {
                //Các trường có dấu (*) là các trường bắt buộc phải set giá trị
                if (HisConfigCFG.IsSyncHID && (dataVitaminASDO == null || String.IsNullOrEmpty(dataVitaminASDO.HisPatient.PERSON_CODE)))
                {
                    CommonParam paramHID = new CommonParam();
                    HID_PERSON filter = new HID_PERSON();
                    if (ucRequestService != null && ucRequestService.cardSearch != null && !String.IsNullOrEmpty(ucRequestService.cardSearch.CardCode))
                        filter.CARD_CODE = ucRequestService.cardSearch.CardCode;
                    //filter.BHYT_NUMBER = ((this.patientProfile.HisPatientTypeAlter != null) ? this.patientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER : "");
                    filter.BRANCH_CODE = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                    filter.BRANCH_NAME = BranchDataWorker.Branch.BRANCH_NAME;
                    filter.ADDRESS = dataVitaminASDO.HisPatient.ADDRESS;
                    filter.COMMUNE_NAME = dataVitaminASDO.HisPatient.COMMUNE_NAME;
                    filter.DISTRICT_NAME = dataVitaminASDO.HisPatient.DISTRICT_NAME;
                    filter.PROVINCE_NAME = dataVitaminASDO.HisPatient.PROVINCE_NAME;
                    filter.CAREER_NAME = (dataVitaminASDO.HisPatient.CAREER_ID > 0 ? (BackendDataWorker.Get<HIS_CAREER>().FirstOrDefault(o => o.ID == dataVitaminASDO.HisPatient.CAREER_ID) ?? new HIS_CAREER()).CAREER_NAME : "");
                    filter.DOB = dataVitaminASDO.HisPatient.DOB;//(*)
                    filter.GENDER_ID = dataVitaminASDO.HisPatient.GENDER_ID;//(*)
                    filter.FIRST_NAME = dataVitaminASDO.HisPatient.FIRST_NAME;
                    filter.LAST_NAME = dataVitaminASDO.HisPatient.LAST_NAME;
                    if (IsChild(dataVitaminASDO))
                    {
                        filter.VIR_PERSON_NAME = dataVitaminASDO.HisPatient.RELATIVE_NAME;//TODO
                    }
                    else
                    {
                        filter.VIR_PERSON_NAME = dataVitaminASDO.HisPatient.LAST_NAME + " " + dataVitaminASDO.HisPatient.FIRST_NAME;
                    }
                    filter.IS_HAS_NOT_DAY_DOB = dataVitaminASDO.HisPatient.IS_HAS_NOT_DAY_DOB;
                    filter.ETHNIC_NAME = dataVitaminASDO.HisPatient.ETHNIC_NAME;
                    filter.EMAIL = dataVitaminASDO.HisPatient.EMAIL;
                    filter.NATIONAL_NAME = dataVitaminASDO.HisPatient.NATIONAL_NAME;
                    filter.MOBILE = dataVitaminASDO.HisPatient.PHONE;
                    filter.HOUSEHOLD_CODE = this.houseHold_Code;
                    filter.HOUSEHOLD_RELATION_NAME = this.hoseHold_Relative;
                    //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                    if (!String.IsNullOrEmpty(this.cMNDNumber))
                    {
                        if (this.cMNDNumber.Length > 9)
                        {
                            filter.CCCD_DATE = this.cMNDDate;
                            filter.CCCD_NUMBER = this.cMNDNumber;
                            filter.CCCD_PLACE = this.cMNDPlace;
                        }
                        else
                        {
                            filter.CMND_DATE = this.cMNDDate;
                            filter.CMND_NUMBER = this.cMNDNumber;
                            filter.CMND_PLACE = this.cMNDPlace;
                        }
                    }
                    filter.HT_ADDRESS = this.addressNow;
                    filter.HT_COMMUNE_NAME = this.communeNowName;
                    filter.HT_DISTRICT_NAME = this.districtNowName;
                    filter.HT_PROVINCE_NAME = this.provinceNowName;
                    filter.MOTHER_NAME = this.motherName;
                    filter.FATHER_NAME = this.fatherName;
                    filter.RELATIVE_PHONE = this.phone;
                    filter.RELATIVE_ADDRESS = this.relativeAddress;
                    filter.RELATIVE_NAME = this.relativeName;
                    filter.RELATIVE_TYPE = this.relativeType;
                    filter.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                    filter.BORN_PROVINCE_CODE = GenerateProvinceCode(this.born_provinceCode);//(*)
                    filter.BORN_PROVINCE_NAME = this.born_provinceName;//(*)
                    filter.BLOOD_ABO_CODE = this.blood_ABO_Code;
                    filter.BLOOD_RH_CODE = this.blood_Rh_Code;
                    //var persons = (new BackendAdapter(paramHID).Post<List<HID_PERSON>>(RequestUriStore.HID_PERSON_GET, ApiConsumers.HidConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramHID));
                    var persons = ApiConsumers.HidWrapConsumer.Post<List<HID_PERSON>>(true, "api/HidPerson/Take", paramHID, filter);
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
                        if (string.IsNullOrEmpty(this.hisPatientVitaminASDODataSave.HisPatient.PERSON_CODE))
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
                        //if (paramHID.Messages != null && paramHID.Messages.Count > 0)
                        //{
                        //    this.param.Messages.AddRange(paramHID.Messages);
                        //}
                        //if (paramHID.BugCodes != null && paramHID.BugCodes.Count > 0)
                        //{
                        //    this.param.BugCodes.AddRange(paramHID.BugCodes);
                        //}
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
                hisPatientVitaminASDODataSave.HisPatient.PERSON_CODE = data.PERSON_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsChild(HisPatientVitaminASDO vitaminASDO)
        {
            bool isChild = false;
            try
            {
                var dtDateOfBird = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(vitaminASDO.HisPatient.DOB) ?? DateTime.Now;
                isChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtDateOfBird);
            }
            catch (Exception ex)
            {
                isChild = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return isChild;
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

        private HIS_VITAMIN_A ProcessVitaminASave(UCRegister ucServiceRequestRegiter, long dob)
        {
            HIS_VITAMIN_A rs = new HIS_VITAMIN_A();

            try
            {
                //if (cboCaseType.EditValue != null)
                //{
                //    rs.CASE_TYPE = Inventec.Common.TypeConvert.Parse.ToInt64(cboCaseType.EditValue.ToString());
                //    if (rs.CASE_TYPE == 1)
                //    {
                //        rs.IS_SICK = (chkSick.Checked && layoutControlItem13.Visibility == LayoutVisibility.Always) ? (short?)1 : null;
                //    }
                //    else if (rs.CASE_TYPE == 2)
                //    {
                //        rs.IS_ONE_MONTH_BORN = (chkOneMonth.Checked && layoutControlItem25.Visibility == LayoutVisibility.Always) ? (short?)1 : null;
                //    }
                //}
                if (this.ucRoomVitaminA != null)
                {
                    if (this.ucRoomVitaminA.cboRoom.EditValue != null)
                    {
                        var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.ucRoomVitaminA.cboRoom.EditValue.ToString()));
                        if (room != null)
                        {
                            rs.EXECUTE_ROOM_ID = room.ROOM_ID;
                        }
                    }

                    if (this.ucRoomVitaminA.chkTreEm.Checked)
                    {
                        rs.CASE_TYPE = 1;
                        rs.IS_SICK = this.ucRoomVitaminA.chkSick.Checked ? (short?)1 : null;
                        rs.IS_MALNUTRITION = this.ucRoomVitaminA.chkSuyDinhDuong.Checked ? (short?)1 : null;
                        //rs.MONTH_OLD = Inventec.Common.DateTime.Calculation.DifferenceMonth(dob, Inventec.Common.DateTime.Get.Now() ?? 0);
                    }
                    else if (this.ucRoomVitaminA.chkPhuNuSauSinh.Checked)
                    {
                        rs.CASE_TYPE = 2;
                        rs.IS_ONE_MONTH_BORN = this.ucRoomVitaminA.chkOneMonthBorn.Checked ? (short?)1 : null;
                    }
                    else if (this.ucRoomVitaminA.chkKhac.Checked)
                    {
                        rs.CASE_TYPE = 3;
                    }

                    rs.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.ucRoomVitaminA.dtTime.DateTime) ?? 0;
                }

                if (this.ucRoomVaccine != null)
                {

                }

                rs.REQUEST_ROOM_ID = this.currentModule.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new HIS_VITAMIN_A();
            }

            return rs;
        }

        private HIS_VACCINATION_EXAM ProcessVaccineSave(UCRegister ucServiceRequestRegiter)
        {
            HIS_VACCINATION_EXAM rs = new HIS_VACCINATION_EXAM();

            try
            {
                if (this.ucRoomVaccine.cboRoom.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.ucRoomVaccine.cboRoom.EditValue.ToString()));
                    if (room != null)
                    {
                        rs.EXECUTE_ROOM_ID = room.ROOM_ID;
                    }
                }

                if (this.ucRoomVaccine.cboPatientType.EditValue != null)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(this.ucRoomVaccine.cboPatientType.EditValue.ToString()));
                    if (patientType != null)
                    {
                        rs.PATIENT_TYPE_ID = patientType.ID;
                    }
                }

                rs.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.ucRoomVaccine.dtRequestTime.DateTime) ?? 0;
                rs.REQUEST_ROOM_ID = this.currentModule.RoomId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new HIS_VACCINATION_EXAM();
            }

            return rs;
        }

        private bool CheckValidateForSave(CommonParam param)
        {
            bool valid = true;
            bool validHeinInfo = true;
            bool validPlusInfo = true;
            bool validPatientRaw = true;
            bool validRelative = true;
            bool validAddress = true;
            bool validOtherServiceReq = true;
            bool validServiceRoom = true;
            bool validTTCT = true;
            bool validKskContract = true;
            bool validVitaminA = true;
            bool validVaccine = true;
            try
            {
                validPatientRaw = ucPatientRaw1.ValidateRequiredField();
                validRelative = ucRelativeInfo1.ValidateRequiredField();
                validPlusInfo = ucPlusInfo1.ValidateRequiredField();

                if (ucRoomVitaminA != null)
                    validVitaminA = ucRoomVitaminA.dxValidationProvider1.Validate();
                if (ucRoomVaccine != null)
                    validVaccine = ucRoomVaccine.dxValidationProvider1.Validate();

                //Check ksk
                long patientTypeId = GetPatientTypeId();
                if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__KSK)
                {
                    if (this.ucKskContract != null && this.kskContractProcessor != null)
                    {
                        validKskContract = this.kskContractProcessor.GetValidate(this.ucKskContract);
                    }
                }

                if (this.IsObligatoryTranferMediOrg && this.IsPresent)
                    validTTCT = this.ValidatedTTCT;
                else
                {
                    UpdateSelectedTranPati(null);
                    validTTCT = true;
                }
                //validAddress = ucAddressCombo1.ValidateRequiredField(); // bắt valid khi trẻ em và có giấy khai sinh.

                GlobalStore.currentFactorySaveType = this.GetEnumSaveType(param);
                valid = (GlobalStore.currentFactorySaveType != UCServiceRequestRegisterFactorySaveType.VALID);
                valid = valid
                    && validHeinInfo
                    && validPatientRaw
                    && validRelative
                    && validOtherServiceReq
                    && validPlusInfo
                    && validServiceRoom
                    && validAddress
                    && validTTCT
                    && validKskContract
                    && validVitaminA
                    && validVaccine;

                try
                {
                    GlobalStore.DepartmentId = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId).DEPARTMENT_ID;
                }
                catch { }
                if (GlobalStore.DepartmentId == 0) throw new ArgumentNullException("departmentId == 0");
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private UCServiceRequestRegisterFactorySaveType GetEnumSaveType(CommonParam param)
        {
            UCServiceRequestRegisterFactorySaveType result = UCServiceRequestRegisterFactorySaveType.REGISTER;
            try
            {
                //Lúc đăng ký tiếp đón, khi người dùng CÓ chọn phòng khám nhưng KHÔNG chọn dịch vụ khám 
                //==> lúc lưu sẽ hiển thị cảnh báo "Bạn chưa chọn dịch vụ khám. Bạn có muốn tiếp tục không?" 
                //Focus mặc định vào ô chọn "Không".   
                //Nếu người dùng chọn tiếp tục thì chạy luồng tiếp đón chỉ tạo hồ sơ
                //Ngược lại đưa bỏ qua
                bool isCreateProfileAndRequest = (this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0);
                bool validHasShowMess = (isCreateProfileAndRequest && this.serviceReqDetailSDOs.Any(o => (o.RoomId ?? 0) > 0 && o.ServiceId == 0));
                if (validHasShowMess)
                {
                    if (MessageBox.Show(ResourceMessage.BanChuaChonDichVuKham, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
                    {
                        result = UCServiceRequestRegisterFactorySaveType.VALID;
                    }
                    else
                    {
                        return UCServiceRequestRegisterFactorySaveType.PROFILE;
                    }
                }
                else
                    result = (isCreateProfileAndRequest ? UCServiceRequestRegisterFactorySaveType.REGISTER : UCServiceRequestRegisterFactorySaveType.PROFILE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Kiểm tra hạn của thẻ bhyt, nếu hạn thẻ nhỏ hơn cấu hình giới hạn số ngày cảnh báo hạn thẻ => show cảnh báo cho người dùng
        /// số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1
        /// </summary>
        /// <param name="alertExpriedTimeHeinCardBhyt"></param>
        /// <param name="resultDayAlert"></param>
        /// <returns>số ngày còn lại của hạn thẻ bhyt nếu sắp hết hạn, nếu có giấy chứng sinh và các trường hợp ngược lại trả về -1</returns>

        private void ExamRegisterSuccess(CommonParam param)
        {
            bool success = false;
            try
            {
                //if (this.currentHisExamServiceReqResultSDO != null)
                //{
                //    if (this.currentHisExamServiceReqResultSDO.ServiceReqs != null
                //        && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0
                //        && this.currentHisExamServiceReqResultSDO.SereServs != null)
                //    {
                //        UCPatientRawADO patientRawADO = ucPatientRaw1.GetValue();
                //        patientRawADO.PATIENT_CODE = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.PATIENT_CODE;
                //        //this.ucPatientRaw1.SetValue(patientRawADO);
                //        this.ucPatientRaw1.SetPatientCodeAfterSavePatient(patientRawADO.PATIENT_CODE);
                //        if (currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE || currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter.IS_TEMP_QN == 1)
                //            //this.ucHeinInfo1.SetTuyenKhiTreEmSuDungTheTam();

                //            //this.serviceReqIdForCreated = this.currentHisExamServiceReqResultSDO.ServiceReqs[0].ID;//TODO
                //            success = true;
                //        this.actionType = GlobalVariables.ActionView;
                //        this.EnableButton(actionType, true);
                //        //this.FillDataToGirdExecuteRoomInfo();//TODO
                //        this.ucPatientRaw1.FocusToPatientName();
                //    }
                //    else
                //    {
                //        LogSystem.Error("Api thuc hien tao yeu cau dang ky thanh cong nhung ket qua tra ve khong hop le:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisExamServiceReqResultSDO), currentHisExamServiceReqResultSDO));
                //        param.Messages.Add(ResourceMessage.HeThongTBKetQuaTraVeCuaServerKhongHopLe);
                //    }
                //}
                //else
                //{
                //    LogSystem.Error("Api thuc hien dang ky yeu cau xu ly that bai:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisExamServiceReqResultSDO), currentHisExamServiceReqResultSDO));
                //}
                //WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            MessageManager.Show(this.ParentForm, param, success);
        }

        private void PatientProfileSuccess(CommonParam param)
        {
            bool success = false;
            try
            {
                //if (this.resultHisPatientProfileSDO != null)
                //{
                //    if (
                //         this.resultHisPatientProfileSDO.HisPatient != null
                //        && this.resultHisPatientProfileSDO.HisPatientTypeAlter != null
                //        && this.resultHisPatientProfileSDO.HisTreatment != null)
                //    {
                //        this.ucPatientRaw1.SetPatientCodeAfterSavePatient(resultHisPatientProfileSDO.HisPatient.PATIENT_CODE);
                //        //this.ucHeinInfo1.SetTuyenKhiTreEmSuDungTheTam();
                //        success = true;
                //        this.actionType = GlobalVariables.ActionView;
                //        this.EnableButton(actionType, false);
                //    }
                //    else
                //    {
                //        LogSystem.Error("Api thuc hien tao thong tin ho so va chi dinh dich vu thanh cong nhung ket qua tra ve khong hop le:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultHisPatientProfileSDO), this.resultHisPatientProfileSDO));
                //        param.Messages.Add(ResourceMessage.HeThongTBKetQuaTraVeCuaServerKhongHopLe);
                //    }
                //}
                //else
                //{
                //    LogSystem.Error("Api thuc hien tao thong tin ho so va chi dinh dich vu that bai:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultHisPatientProfileSDO), this.resultHisPatientProfileSDO));
                //}

                //WaitingManager.Hide();
                //Inventec.Common.Logging.LogSystem.Debug("Service request PatientProfile end process: time=" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            MessageManager.Show(this.ParentForm, param, success);
        }

    }
}
