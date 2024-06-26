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
using HIS.Desktop.Plugins.RegisterVaccination.ADO;

namespace HIS.Desktop.Plugins.RegisterVaccination.Run3
{
    public partial class UCRegister : UserControlBase
    {
        HisPatientVaccinationSDO hisPatientVitaminASDODataSave;
        VaccinationRegisterResultSDO hisPatientVitaminASDOSave;
        protected long departmentId { get; set; }

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
        protected string appointmentCodeSave { get; set; }
        protected string codeFind { get; set; }
        protected string typeCodeFind__MaBN { get; set; }
        protected string typeCodeFind__MaHK { get; set; }
        protected string typeCodeFind__MaCT { get; set; }
        protected string typeCodeFind__SoThe { get; set; }
        protected long patientId { get; set; }

        //UCAddress
        protected string communeCode { get; set; }
        protected string communeName { get; set; }
        protected string districtCode { get; set; }
        protected string districtName { get; set; }
        protected string provinceCode { get; set; }
        protected string provinceName { get; set; }
        protected string address { get; set; }
        protected string phone { get; set; }

        // UCRelative
        protected string relativeAddress { get; set; }
        protected string relativeName { get; set; }
        protected string relativeType { get; set; }
        protected string relativeCMNDNumber { get; set; }
        protected string religionName { get; set; }
        protected string relativePhoneNumber { get; set; }

        protected string personCode { get; set; }
        // UCImage
        //protected byte[] img_avatar { get; set; }
        //protected byte[] img_BHYT { get; set; }

        protected string ethnicName { get; set; }
        protected string ethnicCode { get; set; }

        // Declare
        protected HisCardSDO cardSearchSave { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPatientExtendADO patientInformationADO { get; set; }
        protected HIS_PATIENT patientData { get; set; }
        // protected HisPatientProfileSDO patientProfile { get; set; }
        protected UCRegister ucRequestService;
        //protected HisPatientProfileSDO heinInfoValue { get; set; }
        protected UCPatientRawADO patientRawInfoValue { get; set; }
        protected UCAddressADO addressInfoValue { get; set; }
        protected HIS.UC.PlusInfo.ADO.UCPlusInfoADO patientPlusInformationInfoValue { get; set; }
        //protected HIS.UC.UCImageInfo.ADO.UCImageInfoADO imageADO { get; set; }
        //protected List<ServiceReqDetailSDO> serviceRoomInfoValue { get; set; }

        private void Save(bool printNow)
        {
            try
            {
                gridViewVaccinationMety.PostEditor();
                btnSave.Focus();
                this.resultHisPatientProfileSDO = null;
                this.currentHisExamServiceReqResultSDO = null;
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

                        hisPatientVitaminASDODataSave = new HisPatientVaccinationSDO();
                        GetDataSaveFromUc(this, ref hisPatientVitaminASDODataSave);
                        if (this.cardSearchSave != null && !String.IsNullOrEmpty(this.cardSearchSave.CardCode))
                            hisPatientVitaminASDODataSave.CardCode = this.cardSearchSave.CardCode;

                        //if (this.ucRoomVitaminA != null)
                        //{
                        //    var month = Inventec.Common.DateTime.Calculation.DifferenceMonth(hisPatientVitaminASDODataSave.HisPatient.DOB, Inventec.Common.DateTime.Get.Now() ?? 0);
                        //    if (month < 60 && hisPatientVitaminASDODataSave.HisVitaminA.CASE_TYPE != 1)
                        //    {
                        //        WaitingManager.Hide();
                        //        DevExpress.XtraEditors.XtraMessageBox.Show("Bệnh nhân là trẻ em, bắt buộc chọn trường hợp là trẻ em");
                        //        return;
                        //    }
                        //}
                        //return;
                        CallSyncHID(hisPatientVitaminASDODataSave);

                        hisPatientVitaminASDOSave = new BackendAdapter(param).Post<VaccinationRegisterResultSDO>("api/HisVaccinationExam/Register", ApiConsumers.MosConsumer, hisPatientVitaminASDODataSave, param);

                        if (hisPatientVitaminASDOSave != null)
                        {
                            success = true;

                            this.ucPatientRaw1.SetPatientCodeAfterSavePatient(hisPatientVitaminASDOSave.Patient.PATIENT_CODE);
                            LoadDataToGridVaccination();
                        }

                        WaitingManager.Hide();
                        MessageManager.Show(this.ParentForm, param, success);

                        if (success)
                            this.EnableControl(false);
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

        private void GetDataSaveFromUc(UCRegister ucServiceRequestRegiter, ref HisPatientVaccinationSDO hisPatientVitaminASDO)
        {

            try
            {
                // Get Data From UC
                this.ucRequestService = ucServiceRequestRegiter;
                this.patientRawInfoValue = ucServiceRequestRegiter.ucPatientRaw1.GetValue();
                this.addressInfoValue = ucServiceRequestRegiter.ucAddressCombo1.GetValue();
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
                this.phone = this.addressInfoValue.Phone;

                this.patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString() ?? "0");


                if (cboDanToc.EditValue != null)
                {
                    var dataDanToc = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>().FirstOrDefault(p => p.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboDanToc.EditValue.ToString() ?? "0"));
                    if (dataDanToc != null)
                    {
                        this.ethnicCode = dataDanToc.ETHNIC_CODE;
                        this.ethnicName = dataDanToc.ETHNIC_NAME;
                    }
                }
                ////UCRelative
                this.relativeAddress = this.txtDiaChi.Text;
                this.relativeType = this.txtQuanHe.Text;
                this.relativeName = this.txtNguoiNha.Text;
                this.relativeCMNDNumber = this.txtCMND.Text;
                this.relativePhoneNumber = this.txtPhoneNumber.Text;
                //this.religionName = "";

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
                hisPatientVitaminASDO.HisPatient = patient;



                HIS_VACCINATION_EXAM vaccine = new HIS_VACCINATION_EXAM();
                if (dtThoiGian.EditValue != null && dtThoiGian.DateTime != DateTime.MinValue)
                {
                    vaccine.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGian.DateTime) ?? 0;
                }
                vaccine.REQUEST_ROOM_ID = this.currentModule.RoomId;
                vaccine.REQUEST_LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                vaccine.REQUEST_USERNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                vaccine.REQUEST_DEPARTMENT_ID = this.departmentId;

                if (cboPhongKham.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhongKham.EditValue.ToString()));
                    if (room != null)
                    {
                        vaccine.EXECUTE_ROOM_ID = room.ROOM_ID;
                    }
                    // vaccine.EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPhongTiem.EditValue.ToString());//la phong kham
                }

                if (cboPatientType.EditValue != null)
                {
                    vaccine.PATIENT_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                }


                hisPatientVitaminASDO.HisVaccinationExam = vaccine;
                hisPatientVitaminASDO.RequestRoomId = this.currentModule.RoomId;


                hisPatientVitaminASDO.RequestLoginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                hisPatientVitaminASDO.RequestUsername = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                //hisPatientVitaminASDO.VaccinationExamId = vaccinationExam.ID;
                long? vaccinationId = null;
                hisPatientVitaminASDO.VaccinationMeties = GetVaccinationMety(ref vaccinationId);
                //hisPatientVitaminASDO.WorkingRoomId = roomId;
                // hisPatientVitaminASDO.VaccinationId = vaccinationId;
                //hisPatientVitaminASDO.RequestTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGian.DateTime) ?? 0;


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<VaccinationMetySDO> GetVaccinationMety(ref long? vaccinationId)
        {
            List<VaccinationMetySDO> result = new List<VaccinationMetySDO>();
            try
            {
                List<ExpMestMedicineADO> vaccinationMetyADOs = gridControlVaccinationMety.DataSource as List<ExpMestMedicineADO>;
                if (vaccinationMetyADOs != null && vaccinationMetyADOs.Count > 0)
                {
                    foreach (var item in vaccinationMetyADOs)
                    {
                        VaccinationMetySDO sdo = new VaccinationMetySDO();
                        sdo.Amount = item.AMOUNT;
                        sdo.MediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPhongTiem.EditValue.ToString());
                        sdo.MedicineTypeId = item.TDL_MEDICINE_TYPE_ID ?? 0;
                        sdo.PatientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                        result.Add(sdo);
                    }
                    ExpMestMedicineADO expMestMedicine = vaccinationMetyADOs.FirstOrDefault(o => o.TDL_VACCINATION_ID.HasValue);
                    if (expMestMedicine != null)
                    {
                        vaccinationId = expMestMedicine.TDL_VACCINATION_ID;
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

        private HIS_PATIENT ProcessPatientSave(UCRegister ucServiceRequestRegiter)
        {
            HIS_PATIENT rs = new HIS_PATIENT();

            try
            {
                rs.PATIENT_CODE = this.patientCode;
                rs.PERSON_CODE = this.personCode;
                if (this.patientId != 0)
                    rs.ID = this.patientId;
                rs.IS_HAS_NOT_DAY_DOB = (short)(this.isNotPatientDayDob ? 1 : 0);
                //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                //if (!String.IsNullOrEmpty(this.cMNDNumber))
                //{
                //    if (this.cMNDNumber.Length > 9)
                //    {
                //        rs.CCCD_NUMBER = this.cMNDNumber;
                //    }
                //    else
                //    {
                //        rs.CMND_NUMBER = this.cMNDNumber;
                //    }
                //}



                rs.RELATIVE_ADDRESS = this.relativeAddress;
                rs.RELATIVE_NAME = this.relativeName;
                rs.RELATIVE_TYPE = this.relativeType;
                rs.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
                rs.RELATIVE_PHONE = this.relativePhoneNumber;
                rs.FIRST_NAME = this.patient_First_Name;
                rs.LAST_NAME = this.patient_Last_Name;

                rs.PROVINCE_CODE = this.provinceCode;
                rs.DOB = this.dob;
                rs.GENDER_ID = this.GenderId;
                rs.ADDRESS = this.address;
                rs.PHONE = this.phone;
                rs.PROVINCE_NAME = this.provinceName;
                rs.DISTRICT_NAME = this.districtName;
                rs.COMMUNE_NAME = this.communeName;
                if (careerId.HasValue)
                    rs.CAREER_ID = this.careerId.Value;
                rs.CAREER_NAME = this.careerName;
                rs.CAREER_CODE = this.careerCode;
                rs.ETHNIC_NAME = this.ethnicName;
                rs.ETHNIC_CODE = this.ethnicCode;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return new HIS_PATIENT();
            }

            return rs;
        }

        private void CallSyncHID(HisPatientVaccinationSDO dataVitaminASDO)
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
                    //Kiểm tra số ký tự nhập vào trường CMND để phân biệt là nhập theo CMND hay theo thẻ căn cước công dân. Nhập 9 ký tự số => CMND, nhập 12 ký tự số => căn cước
                    //if (!String.IsNullOrEmpty(this.cMNDNumber))
                    //{
                    //    if (this.cMNDNumber.Length > 9)
                    //    {
                    //        filter.CCCD_NUMBER = this.cMNDNumber;
                    //    }
                    //    else
                    //    {
                    //        filter.CMND_NUMBER = this.cMNDNumber;
                    //    }
                    //}
                    filter.RELATIVE_ADDRESS = this.relativeAddress;
                    filter.RELATIVE_NAME = this.relativeName;
                    filter.RELATIVE_TYPE = this.relativeType;
                    filter.RELATIVE_PHONE = this.relativePhoneNumber;
                    filter.RELATIVE_CMND_NUMBER = this.relativeCMNDNumber;
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

        bool IsChild(HisPatientVaccinationSDO vitaminASDO)
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
                //TODO

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
                if (cboPhongTiem.EditValue != null)
                {
                    var room = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPhongTiem.EditValue.ToString()));
                    if (room != null)
                    {
                        rs.EXECUTE_ROOM_ID = room.ROOM_ID;
                    }
                }

                if (cboPatientType.EditValue != null)
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()));
                    if (patientType != null)
                    {
                        rs.PATIENT_TYPE_ID = patientType.ID;
                    }
                }

                rs.REQUEST_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtThoiGian.DateTime) ?? 0;
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
            bool validPatientRaw = true;
            try
            {
                validPatientRaw = ucPatientRaw1.ValidateRequiredField();

                //validAddress = ucAddressCombo1.ValidateRequiredField(); // bắt valid khi trẻ em và có giấy khai sinh.

                //GlobalStore.currentFactorySaveType = this.GetEnumSaveType(param);
                //valid = (GlobalStore.currentFactorySaveType != UCServiceRequestRegisterFactorySaveType.VALID);
                valid = valid
                    && validPatientRaw;

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

        private void EnableButton(int actionType, bool isPrint)
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionAdd)
                {
                    this.btnSave.Enabled = true;
                    this.btnSaveAndPrint.Enabled = true;
                    //this.btnSaveAndAssain.Enabled = false;
                    this.btnPrint.Enabled = false;
                    //this.btnTreatmentBedRoom.Enabled = false;
                    //this.btnDepositDetail.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = false;
                    this.btnSaveAndPrint.Enabled = false;
                    this.btnPrint.Enabled = true;// isPrint;
                    //this.btnSaveAndAssain.Enabled = true;
                    //this.btnTreatmentBedRoom.Enabled = true;
                    //this.btnDepositDetail.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
