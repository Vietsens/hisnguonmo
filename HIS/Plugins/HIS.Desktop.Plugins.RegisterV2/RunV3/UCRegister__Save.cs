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
using HIS.UC.PlusInfo.ADO;
using HIS.UC.UCPatientRaw;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {

        List<string> lst = new List<string>();
        bool EmergencyBol = false;
        long treatmentTypeID = 0;
        protected bool CheckPreviousDebtTreatment()
        {
            bool rs = true;
            try
            {
                if (lst != null && lst.Count > 0 && !EmergencyBol && treatmentTypeID > 0 && treatmentTypeID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                                String.Format(ResourceMessage.NoVienPhi, string.Join(", ", lst.Distinct())),
                                ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                MessageBoxButtons.OK);
                    rs = false;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }
        private void Save(bool printNow)
        {
            try
            {
                this.resultHisPatientProfileSDO = null;
                this.currentHisExamServiceReqResultSDO = null;
                this.isShowMess = false;
                bool success = false;
                CommonParam param = new CommonParam();
                if (this.CheckValidateForSave(param))
                {
                    if (!this.CheckTreatmentOrder())
                        return;
                    if (chkAssignDoctor.Checked && !this.CheckAssignedExecuteLoginName())
                    {
                        XtraMessageBox.Show(ResourceMessage.ChuaCoThongTinBacSiKham, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    var patientRawInfoValue = ucPatientRaw1.GetValue();
                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3")
                    {
                        var serviceReqInfoValue = ucOtherServiceReqInfo1.GetValue();
                        lst = patientRawInfoValue.lstPreviousDebtTreatments;
                        EmergencyBol = serviceReqInfoValue.IsEmergency;
                        treatmentTypeID = serviceReqInfoValue.TreatmentType_ID;
                        if (!CheckPreviousDebtTreatment())
                        {
                            return;
                        }
                    }
                    string Message = null;
                    string MessageGender = null;
                    string gender = null;
                    List<string> lstServiceName = new List<string>();
                    if (serviceReqDetailSDOs != null && serviceReqDetailSDOs.Count > 0 && serviceReqDetailSDOs.Where(o => o.ServiceId > 0) != null && serviceReqDetailSDOs.Where(o => o.ServiceId > 0).ToList().Count > 0)
                    {
                        foreach (var item in serviceReqDetailSDOs.Where(o => o.ServiceId > 0))
                        {
                            var service = lstService.FirstOrDefault(o => o.ID == item.ServiceId);
                            if (service != null && service.GENDER_ID != null && service.GENDER_ID != patientRawInfoValue.GENDER_ID)
                            {

                                var genders = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>();
                                gender = genders.FirstOrDefault(o => o.ID == patientRawInfoValue.GENDER_ID).GENDER_NAME;
                                lstServiceName.Add(service.SERVICE_NAME);
                            }

                            // check tuổi từ - đến (DVKT)
                            var ageDate = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientRawInfoValue.DOB);
                            TimeSpan timeSpan2 = System.DateTime.Now.Date - ageDate.Value.Date;
                            long ticks = timeSpan2.Ticks;
                            System.DateTime dateTime = new System.DateTime(ticks);
                            int ageMonth = (dateTime.Year - 1) * 12 + dateTime.Month - 1;
                            //Inventec.Common.Logging.LogSystem.Debug("age: " + age);
                            string ageType = null;
                            long? age = 0;
                            if ((service.AGE_FROM != null && service.AGE_FROM > ageMonth) || (service.AGE_TO != null && service.AGE_TO < ageMonth))
                            {
                                if (service.AGE_FROM != null && service.AGE_TO == null)
                                {
                                    if (service.AGE_FROM < 72) { age = service.AGE_FROM; ageType = "tháng tuổi"; }
                                    else if (service.AGE_FROM >= 72) { age = service.AGE_FROM / 12; ageType = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân từ " + age + " " + ageType + "\r\n";
                                }
                                else if (service.AGE_TO != null && service.AGE_FROM == null)
                                {
                                    if (service.AGE_TO < 72) { age = service.AGE_TO; ageType = "tháng tuổi"; }
                                    else if (service.AGE_TO >= 72) { age = service.AGE_TO / 12; ageType = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân dưới " + age + " " + ageType + "\r\n";
                                }
                                else if (service.AGE_TO != null && service.AGE_FROM != null)
                                {
                                    string ageType2 = null;
                                    long? age2 = 0;
                                    if (service.AGE_FROM < 72) { age = service.AGE_FROM; ageType = "tháng tuổi"; }
                                    else if (service.AGE_FROM >= 72) { age = service.AGE_FROM / 12; ageType = "tuổi"; }

                                    if (service.AGE_TO < 72) { age2 = service.AGE_TO; ageType2 = "tháng tuổi"; }
                                    else if (service.AGE_TO >= 72) { age2 = service.AGE_TO / 12; ageType2 = "tuổi"; }

                                    Message += "Dịch vụ " + service.SERVICE_NAME + " chỉ cho phép chỉ định với bệnh nhân từ " + age + " " + ageType + " đến " + age2 + " " + ageType2 + "\r\n";
                                }
                            }
                        }


                        if (lstServiceName != null && lstServiceName.Count > 0)
                        {

                            MessageGender += "Dịch vụ " + String.Join(", ", lstServiceName) + " không cho phép chỉ định đối với bệnh nhân giới tính " + gender + "\r\n";
                            XtraMessageBox.Show(MessageGender, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return;
                        }

                        if (!string.IsNullOrEmpty(Message))
                        {
                            XtraMessageBox.Show(Message, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                            return;
                        }
                    }

                    if (!CheckDuplicateCCCD())
                        return;

                    try
                    {
                        WaitingManager.Show(this.ParentForm);
                        HIS.Desktop.Common.IAppDelegacyT delegacy = new Register.ServiceRequestRegister(param, this, currentPatientSDO);
                        switch (GlobalStore.currentFactorySaveType)
                        {
                            case UCServiceRequestRegisterFactorySaveType.REGISTER:
                                this.currentHisExamServiceReqResultSDO = delegacy.Execute<HisServiceReqExamRegisterResultSDO>();
                                Inventec.Common.Logging.LogSystem.Debug("Save.1");


                                if (this.currentHisExamServiceReqResultSDO != null
                                    && this.currentHisExamServiceReqResultSDO.HisPatientProfile != null
                                    //&& this.currentHisExamServiceReqResultSDO.SereServs != null
                                    //&& this.currentHisExamServiceReqResultSDO.SereServs.Count > 0
                                    && this.currentHisExamServiceReqResultSDO.ServiceReqs != null
                                    && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0)
                                {
                                    this.resultHisPatientProfileSDO = this.currentHisExamServiceReqResultSDO.HisPatientProfile;
                                    this.ExamRegisterSuccess(param);
                                    if (this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0 && HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsDangKyQuaTongDai == "1")
                                    {
                                        frmServiceReqChoice frm = new frmServiceReqChoice(this.currentHisExamServiceReqResultSDO.SereServs, this.currentHisExamServiceReqResultSDO.ServiceReqs);
                                        frm.ShowDialog();
                                    }
                                    ServiceReqList = currentHisExamServiceReqResultSDO.ServiceReqs;
                                    //Cau hinh in tu dong sau khi luu thanh cong
                                    this.isPrintNow = printNow;


                                    if ((this.isSaveWithRoomHasConfigAllowNotChooseService || printNow) && (chkPrintExam.Checked || chkSignExam.Checked))
                                        this.Print(true);

                                    if (printNow)
                                    {
                                        if (chkPrintPatientCard.Checked)
                                        {
                                            this.PrintPatientCard();
                                        }

                                        if (chkAutoCreateBill.Checked)
                                        {
                                            PrintBienLai();
                                        }

                                        if (chkAutoDeposit.Checked)
                                        {
                                            PrintTamThu();
                                            if (this.currentHisExamServiceReqResultSDO.Transactions != null)
                                            {
                                                if (GlobalVariables.RefreshSessionDepositInfo != null)
                                                {
                                                    var listTranDeposit = this.currentHisExamServiceReqResultSDO.Transactions.Where(o => o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).ToList();
                                                    long numOrder = listTranDeposit.Max(o => o.NUM_ORDER);
                                                    GlobalVariables.RefreshSessionDepositInfo(numOrder);
                                                }
                                            }
                                        }
                                    }

                                    if (currentHisExamServiceReqResultSDO.Transactions != null && currentHisExamServiceReqResultSDO.Transactions.Count > 0 && GlobalVariables.RefreshUsingAccountBookModule != null)
                                    {
                                        GlobalVariables.RefreshUsingAccountBookModule(currentHisExamServiceReqResultSDO.Transactions.OrderByDescending(o => o
                                            .NUM_ORDER).FirstOrDefault().NUM_ORDER);
                                    }
                                    else
                                    {
                                        Inventec.Common.Logging.LogSystem.Info("chkAutoCreateBill: " + chkAutoCreateBill.Checked);
                                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisExamServiceReqResultSDO.Transactions), currentHisExamServiceReqResultSDO.Transactions));
                                    }

                                    success = true;
                                }
                                break;
                            case UCServiceRequestRegisterFactorySaveType.PROFILE:
                                this.resultHisPatientProfileSDO = delegacy.Execute<HisPatientProfileSDO>();
                                Inventec.Common.Logging.LogSystem.Debug("Save.2");

                                if (this.resultHisPatientProfileSDO != null)
                                {
                                    this.PatientProfileSuccess(param);
                                    success = true;
                                }
                                break;
                        }
                        if (!success)
                        {
                            if (!this.isShowMess) MessageManager.Show(this.ParentForm, param, false);
                        }
                        if (success)
                            this.EnableControl(false);
                        WaitingManager.Hide();
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (this.ValidatedTTCT == false)
                {
                    bool rsValidTran = false;
                    bool isValidAll = false;
                    if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 1 && this.IsPresent)
                        rsValidTran = true;
                    else if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2 && (this.IsPresent || this.IsPresentAndAppointment))
                        rsValidTran = true;
                    else if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 3 && this.IsPresent)
                    {
                        rsValidTran = true;
                        isValidAll = true;
                    }
                    if (rsValidTran)
                    {
                        try
                        {
                            frm = new frmTransPati(true, this.transPatiADO, UpdateSelectedTranPati, true, isValidAll);
                            frm.SetValidForTTCT(this.CheckValidateFormTTCT);
                            frm.ShowDialog();
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.EnableControl(true);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckDuplicateCCCD()
        {
            bool result = true;
            try
            {
                if ((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2") && (!string.IsNullOrEmpty(ucPlusInfo1.GetValue().CMND_NUMBER) || !string.IsNullOrEmpty(ucPlusInfo1.GetValue().CCCD_NUMBER) || !string.IsNullOrEmpty(ucPlusInfo1.GetValue().PASSPORT_NUMBER)))
                {
                    var data = ucPlusInfo1.GetValue().CMND_NUMBER ?? ucPlusInfo1.GetValue().CCCD_NUMBER ?? ucPlusInfo1.GetValue().PASSPORT_NUMBER;
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    if (data.Length == 9)
                    {
                        filter.CMND_NUMBER__EXACT = data;
                    }
                    else
                    {
                        filter.CCCD_NUMBER__EXACT = data;
                    }
                    CommonParam param = new CommonParam();
                    var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                    if (patients != null && patients.Count > 0)
                    {
                        if ((ucPatientRaw1.patientTD3 != null && !string.IsNullOrEmpty(ucPatientRaw1.patientTD3.PATIENT_CODE) && !patients.Exists(o => o.PATIENT_CODE == ucPatientRaw1.patientTD3.PATIENT_CODE)) || ucPatientRaw1.patientTD3 == null || string.IsNullOrEmpty(ucPatientRaw1.patientTD3.PATIENT_CODE))
                        {
                            if ((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" && XtraMessageBox.Show(string.Format("Số CCCD {0} đã được sử dụng bởi bệnh nhân có mã {1}", data, patients.OrderByDescending(o => o.PATIENT_CODE).ToList()[0].PATIENT_CODE), "Thông báo", MessageBoxButtons.OK) == DialogResult.OK) || (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2" && XtraMessageBox.Show(string.Format("Số CCCD {0} đã được sử dụng bởi bệnh nhân có mã {1}", data, patients.OrderByDescending(o => o.PATIENT_CODE).ToList()[0].PATIENT_CODE), "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.No))
                            {
                                ucPatientRaw1.SearchPatientByCodeOrQrCode(data, HIS.UC.UCPatientRaw.ResourceMessage.typeCodeFind__MaCMCC);
                                result = false;
                            }
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

        private bool BlockingHeinLevelCode()
        {
            try
            {
                var patientRawADO = ucPatientRaw1.GetValue();
                if (!(patientRawADO.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT || patientRawADO.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN))
                    return true;
                var heindata = ucHeinInfo1.GetValue();
                if (patientRawADO.PATIENTTYPE_ID == HisConfigCFG.PatientTypeId__BHYT && heindata != null && !string.IsNullOrEmpty(heindata.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE))
                {
                    var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == heindata.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE);
                    if (!string.IsNullOrEmpty(BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE) && mediOrg != null && (";" + BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE + ";").Contains(";" + mediOrg.LEVEL_CODE + ";"))
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(
                                    String.Format("Nơi đăng ký khám chữa bệnh ban đầu thuộc tuyến {0}, không được hưởng BHYT", mediOrg.LEVEL_CODE == "1" ? "trung ương" : (mediOrg.LEVEL_CODE == "2" ? "Tỉnh" : (mediOrg.LEVEL_CODE == "3" ? "Huyện" : "Xã"))),
                                    ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                    MessageBoxButtons.OK);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
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
            bool validPhoneNumber = true;
            bool validGuarantee = true;
            bool validIsBlockBhyt = true;
            try
            {
                long patientTypeId = GetPatientTypeId();
                bool isUseUCHeinInfo = (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN);
                validHeinInfo = isUseUCHeinInfo ? ucHeinInfo1.ValidateRequiredField() : true;
                validPatientRaw = ucPatientRaw1.ValidateRequiredField();
                validRelative = ucRelativeInfo1.ValidateRequiredField();
                validOtherServiceReq = ucOtherServiceReqInfo1.ValidateRequiredField();
                validPlusInfo = ucPlusInfo1.ValidateRequiredField();
                validServiceRoom = ucServiceRoomInfo1.ValidateRequiredField();
                validAddress = ucAddressCombo1.ValidateRequiredField();
                if (isUseUCHeinInfo)
                    validIsBlockBhyt = this.BlockingHeinLevelCode() && this.BlockingInvalidBhyt();
                //Check ksk             
                if (patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__KSK)
                {
                    if (this.ucKskContract != null && this.kskContractProcessor != null)
                    {
                        validKskContract = this.kskContractProcessor.GetValidate(this.ucKskContract);
                    }
                }

                try
                {
                    if (isUseUCHeinInfo)
                    {
                        this.IsPresent = this.ucHeinInfo1.HeinRightRouteTypeIsPresent();
                        this.IsPresentAndAppointment = this.ucHeinInfo1.HeinRightRouteTypeIsPresentAndAppointment();
                    }
                    else
                    {
                        this.IsPresent = false;
                        this.IsPresentAndAppointment = false;
                    }

                    if ((HisConfigCFG.KeyValueObligatoryTranferMediOrg == 1 || HisConfigCFG.KeyValueObligatoryTranferMediOrg == 3) && this.IsPresent)
                        validTTCT = this.ValidatedTTCT;
                    else if (HisConfigCFG.KeyValueObligatoryTranferMediOrg == 2 && (this.IsPresent || this.IsPresentAndAppointment))
                        validTTCT = this.ValidatedTTCT;
                    else
                        validTTCT = true;
                }
                catch (Exception ex)
                {
                    validTTCT = true;
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                bool validBill = true;
                if (chkAutoCreateBill.Checked && (GlobalVariables.AuthorityAccountBook == null || !GlobalVariables.AuthorityAccountBook.AccountBookId.HasValue))
                {
                    MessageBox.Show(ResourceMessage.BanChuaDuocCapSoHoaDon, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                    validBill = false;
                }

                bool validDeposit = true;

                if (chkAutoDeposit.Checked)
                {

                    if (cboCashierRoom.EditValue == null && (GlobalVariables.SessionInfo == null || (GlobalVariables.SessionInfo != null && GlobalVariables.SessionInfo.CashierWorkingRoomId == null)))
                    {
                        MessageBox.Show(ResourceMessage.BanChuaChonPhongThuNgan, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                        validDeposit = false;
                    }
                    else if (GlobalVariables.SessionInfo == null || GlobalVariables.SessionInfo.DepositAccountBook == null)
                    {
                        MessageBox.Show(ResourceMessage.BanChuaChonSoTamUng, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                        validDeposit = false;
                    }
                    else if (GlobalVariables.SessionInfo == null || GlobalVariables.SessionInfo.PayForm == null)
                    {
                        MessageBox.Show(ResourceMessage.BanChuaChonHinhThucGiaoDich, ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                        validDeposit = false;
                    }
                }

                string phoneNumber = ucAddressCombo1.GetValue().Phone;
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    if (phoneNumber.Length < 10 || phoneNumber.Length > 10)
                    {
                        MessageBox.Show("Số điện thoại mới nhập " + phoneNumber.Length + " ký tự", ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                        this.ucAddressCombo1.FocusPhoneNumber();
                        validPhoneNumber = false;
                    }
                }
                else
                {
                    if (HisConfigCFG.PhoneRequired == "1")
                    {
                        MessageBox.Show("Bạn chưa nhập Điện thoại", ResourceMessage.ThongBao, MessageBoxButtons.OK);
                        this.ucAddressCombo1.FocusPhoneNumber();
                        validPhoneNumber = false;
                    }
                    else if (HisConfigCFG.PhoneRequired == "2")
                    {
                        if (MessageBox.Show("Bạn chưa nhập Điện thoại. Bạn có muốn tiếp tục?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            this.ucAddressCombo1.FocusPhoneNumber();
                            validPhoneNumber = false;
                        }
                    }
                }

                var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == patientTypeId);
                if (patientType != null && patientType.MUST_BE_GUARANTEED == 1)
                {
                    var otherInfo = ucOtherServiceReqInfo1.GetValue();
                    if (String.IsNullOrWhiteSpace(otherInfo.GUARANTEE_LOGINNAME) || String.IsNullOrEmpty(otherInfo.GUARANTEE_USERNAME))
                    {
                        MessageBox.Show(String.Format("Đối tượng {0} bắt buộc nhập thông tin Bảo lãnh", patientType.PATIENT_TYPE_NAME), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.OK);
                        ucOtherServiceReqInfo1.FocusGuarantee();
                        validGuarantee = false;
                    }
                }

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
                    && validBill
                    && validDeposit
                    && validPhoneNumber
                    && validGuarantee
                    && validIsBlockBhyt
                    && CheckRRCodeTTFee(true);

                valid = valid && this.AlertExpriedTimeHeinCardBhyt();

                try
                {
                    GlobalStore.DepartmentId = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModule.RoomId).DEPARTMENT_ID;
                }
                catch { }
                if (GlobalStore.DepartmentId == 0) throw new ArgumentNullException("departmentId == 0");
                if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.MustHaveNCSInfoForChild && ucPatientRaw1.GetIsChild())
                {
                    bool kq = ucRelativeInfo1.ValidateNeedOne();
                    if (kq)
                    {
                        valid = false;
                    }
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

        private bool BlockingInvalidBhyt()
        {
            try
            {
                bool valid = true;
                UCPatientRawADO patientRawADO = ucPatientRaw1.GetValue();
                var heindata = ucHeinInfo1.GetValue();
                if (this.ucPatientRaw1.ResultDataADO != null && this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO != null && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.WarningInvalidCheckHistoryHeinCard && this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.message == "Thẻ BHYT có thông tin kiểm tra thẻ chưa ra viện.")
                {
                    DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.message + " Bạn có muốn tiếp tục?", ResourceMessage.ThongBao, MessageBoxButtons.YesNo);
                    if (drReslt == DialogResult.No)
                    {
                        return false;
                    }
                }
                if (patientRawADO.PATIENTTYPE_ID == HisConfigCFG.PatientTypeId__BHYT && (HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()) && heindata != null && !CheckBhytWhiteListAcceptNoCheckBHYT(heindata.HisPatientTypeAlter.HEIN_CARD_NUMBER) && heindata.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    if (this.ucPatientRaw1.ResultDataADO == null)//thẻ không hợp lệ
                    {
                        Inventec.Common.Logging.LogSystem.Info("ucPatientRaw1.ResultDataADO is null");
                        valid = false;
                    }
                    else if (this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO == null) // lỗi thông tin thẻ
                    {
                        Inventec.Common.Logging.LogSystem.Info("ucPatientRaw1.ResultDataADO.ResultHistoryLDO  is null");
                        valid = false;
                    }
                    else if (HisConfigCFG.MaKetQuaBlockings.Contains(this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.maKetQua))//mã lỗi nằm trong các mã lỗi thẻ hết hạn
                    {
                        valid = false;
                    }

                    if (!valid)
                    {
                        XtraMessageBox.Show("Thẻ BHYT không hợp lệ. Không cho phép đăng ký với đối tượng BHYT");
                        return valid;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return true;
        }

        /// <summary>
        /// Nếu người dùng nhập thông tin thẻ BHYT, mà có đầu mã thẻ được khai báo "Không kiểm tra thông tin trên cổng BHYT" (HIS_BHYT_WHITELIST có IS_NOT_CHECK_BHYT= 1) thì sẽ không thực hiện gọi lên cổng BHYT để lấy thông tin
        /// </summary>
        /// <param name="heinCardNumder"></param>
        /// <returns></returns>
        bool CheckBhytWhiteListAcceptNoCheckBHYT(string heinCardNumder)
        {
            bool valid = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("CheckBhytWhiteListAcceptNoCheckBHYT__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinCardNumder), heinCardNumder));
                if (!String.IsNullOrEmpty(heinCardNumder))
                {
                    var bhytWhiteList = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_BHYT_WHITELIST>().FirstOrDefault(o => o.BHYT_WHITELIST_CODE != null && heinCardNumder.ToUpper().Contains(o.BHYT_WHITELIST_CODE.ToUpper()) && o.IS_NOT_CHECK_BHYT == 1);
                    valid = (bhytWhiteList != null && bhytWhiteList.IS_NOT_CHECK_BHYT == 1);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private UCServiceRequestRegisterFactorySaveType GetEnumSaveType(CommonParam param)
        {
            UCServiceRequestRegisterFactorySaveType result = UCServiceRequestRegisterFactorySaveType.REGISTER;
            try
            {
                this.serviceReqDetailSDOs = ucServiceRoomInfo1.GetDetail();
                //Lúc đăng ký tiếp đón, khi người dùng CÓ chọn phòng khám nhưng KHÔNG chọn dịch vụ khám 
                //==> lúc lưu sẽ hiển thị cảnh báo "Bạn chưa chọn dịch vụ khám. Bạn có muốn tiếp tục không?" 
                //Focus mặc định vào ô chọn "Không".   
                //Nếu người dùng chọn tiếp tục thì chạy luồng tiếp đón chỉ tạo hồ sơ
                //Ngược lại đưa bỏ qua

                //- Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám đó có cấu hình "cho phép không chọn dịch vụ" (ALLOW_NOT_CHOOSE_SERVICE = 1) thì khi thực hiện lưu sẽ xử lý:
                //+ KHÔNG hiển thị thông báo confirm
                //+ Gọi đến API đăng ký khám (thay vì gọi đến api tạo hồ sơ điều trị)
                //+ Lưu ý: nếu người dùng không chọn phòng lẫn dịch vụ thì vẫn hiển thị thông báo xác nhận như cũ.

                //- Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám không có cấu hình "cho phép không chọn dịch vụ" (ALLOW_NOT_CHOOSE_SERVICE khác 1) thì xử lý như cũ. Cụ thể:
                //+ Hiển thị thông báo confirm
                //+ Nếu người dùng đồng ý thì gọi api để tạo hồ sơ điều trị.
                this.isSaveWithRoomHasConfigAllowNotChooseService = false;
                bool isCreateProfileAndRequest = (this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0 && this.serviceReqDetailSDOs.Exists(o => (o.RoomId ?? 0) > 0));
                var validHasShowMess = (isCreateProfileAndRequest ? this.serviceReqDetailSDOs.Where(o => (o.RoomId ?? 0) > 0 && o.ServiceId == 0).ToList() : null);
                if (validHasShowMess != null && validHasShowMess.Count > 0)
                {
                    var roomWorkingAllowNotChooseService = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => p.ALLOW_NOT_CHOOSE_SERVICE == 1 && validHasShowMess.Exists(k => k.RoomId == p.ROOM_ID)).ToList();
                    var roomWorkingUnAllowNotChooseService = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().Where(p => (p.ALLOW_NOT_CHOOSE_SERVICE == null || p.ALLOW_NOT_CHOOSE_SERVICE != 1) && validHasShowMess.Exists(k => k.RoomId == p.ROOM_ID)).ToList();
                    if ((roomWorkingAllowNotChooseService != null && roomWorkingAllowNotChooseService.Count > 0) && (roomWorkingUnAllowNotChooseService == null || roomWorkingUnAllowNotChooseService.Count == 0))
                    {
                        Inventec.Common.Logging.LogSystem.Info("TH nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám đó có cấu hình \"cho phép không chọn dịch vụ\" (ALLOW_NOT_CHOOSE_SERVICE = 1) thì khi thực hiện lưu sẽ xử lý:+ KHÔNG hiển thị thông báo confirm+ Gọi đến API đăng ký khám (thay vì gọi đến api tạo hồ sơ điều trị)");
                        this.isSaveWithRoomHasConfigAllowNotChooseService = true;
                        return UCServiceRequestRegisterFactorySaveType.REGISTER;
                    }
                    else if ((roomWorkingAllowNotChooseService == null || roomWorkingAllowNotChooseService.Count == 0) && (roomWorkingUnAllowNotChooseService != null && roomWorkingUnAllowNotChooseService.Count > 0))
                    {
                        Inventec.Common.Logging.LogSystem.Info("Th Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám không có cấu hình \"cho phép không chọn dịch vụ\" (ALLOW_NOT_CHOOSE_SERVICE khác 1) thì xử lý như cũ. Cụ thể:+ Hiển thị thông báo confirm + Nếu người dùng đồng ý thì gọi api để tạo hồ sơ điều trị.");
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info("Th Nếu người dùng chọn phòng khám và không chọn dịch vụ khám và phòng khám chọn nhiều phòng và có cả phòng khám có và không có cấu hình \"cho phép không chọn dịch vụ\" (ALLOW_NOT_CHOOSE_SERVICE khác 1) thì tạm thời xử lý như cũ.");
                    }

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
                    this.btnSaveAndAssain.Enabled = false;
                    this.btnPrint.Enabled = false;
                    this.btnTreatmentBedRoom.Enabled = false;
                    this.btnDepositDetail.Enabled = this.btnDepositRequest.Enabled = false;
                    this.dropDownButton__Other.Enabled = false;
                    this.btnGiayTo.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = false;
                    this.btnGiayTo.Enabled = true;
                    this.btnSaveAndPrint.Enabled = false;
                    this.btnPrint.Enabled = true;// isPrint;
                    this.btnSaveAndAssain.Enabled = true;
                    this.btnTreatmentBedRoom.Enabled = true;
                    this.btnDepositDetail.Enabled = this.btnDepositRequest.Enabled = true;
                    this.dropDownButton__Other.Enabled = true;
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
        private bool AlertExpriedTimeHeinCardBhyt()
        {
            bool valid = true;
            long resultDayAlert = -1;
            try
            {
                UCPatientRawADO patientRawADO = ucPatientRaw1.GetValue();
                if (patientRawADO.PATIENTTYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    resultDayAlert = this.ucHeinInfo1.GetExpriedTimeHeinCardBhyt(AppConfigs.AlertExpriedTimeHeinCardBhyt, ref resultDayAlert);

                    valid = ((resultDayAlert >= 0) && (MessageBox.Show(String.Format(ResourceMessage.CanhBaoTheBhytSapHatHan, resultDayAlert), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        || resultDayAlert < 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return valid;
        }

        private void ExamRegisterSuccess(CommonParam param)
        {
            bool success = false;
            try
            {
                if (this.currentHisExamServiceReqResultSDO != null)
                {
                    if (this.currentHisExamServiceReqResultSDO.ServiceReqs != null
                        && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0
                        && this.currentHisExamServiceReqResultSDO.SereServs != null)
                    {
                        UCPatientRawADO patientRawADO = ucPatientRaw1.GetValue();
                        patientRawADO.PATIENT_CODE = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.PATIENT_CODE;
                        this.ucPatientRaw1.SetPatientCodeAfterSavePatient(patientRawADO.PATIENT_CODE);
                        if (currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE || currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter.IS_TEMP_QN == 1)
                            this.ucHeinInfo1.ChangeDataHeinInsuranceInfoByPatientTypeAlter(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter);

                        success = true;
                        this.actionType = GlobalVariables.ActionView;
                        this.EnableButton(actionType, true);
                        if (!HisConfigCFG.IsManualInCode && !String.IsNullOrEmpty(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.IN_CODE))
                        {
                            this.ucOtherServiceReqInfo1.SetValueIncode(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.IN_CODE);
                        }
                        this.ucServiceRoomInfo1.InitComboRoom(new ExecuteRoomGet1().GetLCounter1());
                        this.ucPatientRaw1.FocusToPatientName();
                        this.ucHeinInfo1.SetTreatmentId(this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisTreatment.ID);
                    }
                    else
                    {
                        LogSystem.Error("Api thuc hien tao yeu cau dang ky thanh cong nhung ket qua tra ve khong hop le:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisExamServiceReqResultSDO), currentHisExamServiceReqResultSDO));
                        param.Messages.Add(ResourceMessage.HeThongTBKetQuaTraVeCuaServerKhongHopLe);
                    }
                }
                else
                {
                    LogSystem.Error("Api thuc hien dang ky yeu cau xu ly that bai:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHisExamServiceReqResultSDO), currentHisExamServiceReqResultSDO));
                }
                WaitingManager.Hide();
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
                if (this.resultHisPatientProfileSDO != null)
                {
                    if (
                         this.resultHisPatientProfileSDO.HisPatient != null
                        && this.resultHisPatientProfileSDO.HisPatientTypeAlter != null
                        && this.resultHisPatientProfileSDO.HisTreatment != null)
                    {
                        this.ucHeinInfo1.SetTreatmentId(this.resultHisPatientProfileSDO.HisTreatment.ID);
                        this.ucPatientRaw1.SetPatientCodeAfterSavePatient(resultHisPatientProfileSDO.HisPatient.PATIENT_CODE);
                        if (resultHisPatientProfileSDO.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE == MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE || resultHisPatientProfileSDO.HisPatientTypeAlter.IS_TEMP_QN == 1)
                            ucHeinInfo1.ChangeDataHeinInsuranceInfoByPatientTypeAlter(this.resultHisPatientProfileSDO.HisPatientTypeAlter);

                        if (!HisConfigCFG.IsManualInCode && !String.IsNullOrEmpty(this.resultHisPatientProfileSDO.HisTreatment.IN_CODE))
                        {
                            this.ucOtherServiceReqInfo1.SetValueIncode(this.resultHisPatientProfileSDO.HisTreatment.IN_CODE);
                        }
                        success = true;
                        this.actionType = GlobalVariables.ActionView;
                        this.EnableButton(actionType, false);
                    }
                    else
                    {
                        LogSystem.Error("Api thuc hien tao thong tin ho so va chi dinh dich vu thanh cong nhung ket qua tra ve khong hop le:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultHisPatientProfileSDO), this.resultHisPatientProfileSDO));
                        param.Messages.Add(ResourceMessage.HeThongTBKetQuaTraVeCuaServerKhongHopLe);
                    }
                }
                else
                {
                    LogSystem.Error("Api thuc hien tao thong tin ho so va chi dinh dich vu that bai:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultHisPatientProfileSDO), this.resultHisPatientProfileSDO));
                }
                Inventec.Common.Logging.LogSystem.Debug("Service request PatientProfile end process: time=" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            MessageManager.Show(this.ParentForm, param, success);
        }

        private bool CheckAssignedExecuteLoginName()
        {
            bool valid = true;
            try
            {
                var serviceRoomInfoValue = this.ucServiceRoomInfo1.GetDetail();
                if (serviceRoomInfoValue != null && serviceRoomInfoValue.Count() > 0)
                {
                    valid = serviceRoomInfoValue.All(o => !String.IsNullOrWhiteSpace(o.AssignedExecuteLoginName));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private bool CheckTreatmentOrder()
        {
            bool valid = true;
            try
            {
                var ado = this.ucOtherServiceReqInfo1.GetValue();
                if (ado != null && ado.TreatmentOrder.HasValue)
                {
                    HisTreatmentOrderSDO sdo = new HisTreatmentOrderSDO();
                    sdo.TreatmentOrder = ado.TreatmentOrder.Value;
                    sdo.InDate = (ado.IntructionTime - (ado.IntructionTime % 1000000));

                    var rs = new BackendAdapter(new CommonParam()).PostRO<HisTreatmentOrderSDO>("api/HisTreatment/CheckExistsTreatmentOrder", ApiConsumers.MosConsumer, sdo, null);
                    if (rs == null || !rs.Success)
                    {
                        string code = "";
                        if (rs.Param != null && rs.Param.Messages != null && rs.Param.Messages.Count > 0)
                        {
                            code = String.Join(",", rs.Param.Messages);
                        }
                        if (XtraMessageBox.Show(String.Format("Số thứ tự hồ sơ đã tồn tại ({0}), Bạn có muốn tiếp tục?", code), "Cảnh báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, DevExpress.Utils.DefaultBoolean.True) == DialogResult.Yes)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}
