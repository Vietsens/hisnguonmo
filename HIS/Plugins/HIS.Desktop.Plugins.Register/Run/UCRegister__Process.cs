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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Utility;
using HIS.UC.KskContract.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        enum UCServiceRequestRegisterFactorySaveType
        {
            REGISTER,
            PROFILE,
            VALID,
        }

        UCServiceRequestRegisterFactorySaveType GetEnumSaveType(CommonParam param)
        {
            UCServiceRequestRegisterFactorySaveType result = UCServiceRequestRegisterFactorySaveType.REGISTER;
            try
            {
                System.Windows.Forms.Control.ControlCollection controlCollection = this.pnlServiceRoomInfomation.Controls;
                if (controlCollection != null && controlCollection.Count > 0)
                {
                    bool isCreateProfile = true;
                    bool valid = true;
                    this.serviceReqDetailSDOs = new List<ServiceReqDetailSDO>();
                    foreach (Control item in controlCollection)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl))
                        {
                            var dataServ = this.roomExamServiceProcessor.GetDetailSDO(item) as List<ServiceReqDetailSDO>;
                            if (dataServ != null && dataServ.Count > 0)
                            {
                                this.serviceReqDetailSDOs.AddRange(dataServ);
                                isCreateProfile = false;
                            }
                        }
                    }

                    //Lúc đăng ký tiếp đón, khi người dùng CÓ chọn phòng khám nhưng KHÔNG chọn dịch vụ khám 
                    //==> lúc lưu sẽ hiển thị cảnh báo "Bạn chưa chọn dịch vụ khám. Bạn có muốn tiếp tục không?" 
                    //Focus mặc định vào ô chọn "Không".   
                    //Nếu người dùng chọn tiếp tục thì chạy luồng tiếp đón chỉ tạo hồ sơ
                    //Ngược lại đưa bỏ qua
                    valid = (this.serviceReqDetailSDOs != null
                        && this.serviceReqDetailSDOs.Count > 0
                        && this.serviceReqDetailSDOs.Any(o => (o.RoomId ?? 0) > 0 && o.ServiceId == 0)
                        );

                    if (valid)
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
                        result = (isCreateProfile == true ? UCServiceRequestRegisterFactorySaveType.PROFILE : UCServiceRequestRegisterFactorySaveType.REGISTER);
                }
                else
                    result = UCServiceRequestRegisterFactorySaveType.PROFILE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UpdatepatientDob()
        {
            try
            {
                string strPatientDob = "";
                if (this.txtPatientDob.Text.Length == 2 || this.txtPatientDob.Text.Length == 1)
                {
                    strPatientDob = "01/01/" + (DateTime.Now.Year - Inventec.Common.TypeConvert.Parse.ToInt64(this.txtPatientDob.Text)).ToString();
                }
                else if (this.txtPatientDob.Text.Length == 4)
                    strPatientDob = "01/01/" + this.txtPatientDob.Text;
                else if (this.txtPatientDob.Text.Length == 8)
                {
                    strPatientDob = this.txtPatientDob.Text.Substring(0, 2) + "/" + this.txtPatientDob.Text.Substring(2, 2) + "/" + txtPatientDob.Text.Substring(4, 4);
                }
                else
                    strPatientDob = this.txtPatientDob.Text;

                this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strPatientDob);
                this.dtPatientDob.Update();

                this.CalulatePatientAge(strPatientDob, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeLockButtonWhileProcess(bool isLock)
        {
            try
            {
                this.btnSave.Enabled = isLock;
                this.btnSaveAndPrint.Enabled = isLock;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess(HisPatientSDO patient, bool printNow)
        {
            try
            {
                btnSave.Focus();
                this.ChangeLockButtonWhileProcess(false);
                CommonParam param = new CommonParam();
                this.resultHisPatientProfileSDO = null;
                this.currentHisExamServiceReqResultSDO = null;
                this.isShowMess = false;
                Inventec.Common.Logging.LogSystem.Debug("UcRegister => Save Process -> start");
                if (this.Check(param))
                {
                    try
                    {
                        WaitingManager.Show();
                        HIS.Desktop.Common.IAppDelegacyT delegacy = new Register.ServiceRequestRegister(param, this, currentPatientSDO);
                        switch (this.currentFactorySaveType)
                        {
                            case UCServiceRequestRegisterFactorySaveType.REGISTER:
                                this.currentHisExamServiceReqResultSDO = delegacy.Execute<HisServiceReqExamRegisterResultSDO>();
                                if (this.currentHisExamServiceReqResultSDO != null
                                    && this.currentHisExamServiceReqResultSDO.HisPatientProfile != null
                                    && this.currentHisExamServiceReqResultSDO.SereServs != null
                                    && this.currentHisExamServiceReqResultSDO.SereServs.Count > 0
                                    && this.currentHisExamServiceReqResultSDO.ServiceReqs != null
                                    && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0)
                                {
                                    this.resultHisPatientProfileSDO = this.currentHisExamServiceReqResultSDO.HisPatientProfile;
                                    this.ExamRegisterSuccess(param);
                                    if (this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0 && AppConfigs.IsDangKyQuaTongDai == "1")
                                    {
                                        frmServiceReqChoice frm = new frmServiceReqChoice(this.currentHisExamServiceReqResultSDO.SereServs, this.currentHisExamServiceReqResultSDO.ServiceReqs);
                                        frm.ShowDialog();
                                    }
                                    //Inventec.Common.Logging.LogSystem.Debug("UcRegister => Save Process -> ExamRegisterSuccess");
                                    //Cau hinh in tu dong sau khi luu thanh cong
                                    if (printNow)
                                    {
                                        isPrintNow = true;
                                        this.btnPrint_Click(null, null);
                                    }
                                    else if (chkSignExam.Checked)
                                    {
                                        isPrintNow = false;
                                        if (this.currentHisExamServiceReqResultSDO == null || this.currentHisExamServiceReqResultSDO.ServiceReqs == null || this.currentHisExamServiceReqResultSDO.ServiceReqs.Count == 0 || this.actionType == GlobalVariables.ActionAdd)
                                        {
                                            DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.NguoiDungInPhieuYeCauKhamKhongCoDuLieuDangKyKham, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DefaultBoolean.True);
                                            return;
                                        }

                                        this.currentHisExamServiceReqResultSDO.ServiceReqs = this.currentHisExamServiceReqResultSDO.ServiceReqs.Where(o => this.serviceReqPrintIds.Contains(o.ID)).ToList();

                                        this.PrintProcess(this.currentHisExamServiceReqResultSDO);
                                    }
                                }
                                else
                                {
                                    WaitingManager.Hide();
                                    if (!this.isShowMess)
                                    {
                                        MessageManager.Show(this.ParentForm, param, false);
                                    }
                                    this.ChangeLockButtonWhileProcess(true);
                                }
                                break;
                            case UCServiceRequestRegisterFactorySaveType.PROFILE:
                                this.resultHisPatientProfileSDO = delegacy.Execute<HisPatientProfileSDO>();
                                if (this.resultHisPatientProfileSDO != null)
                                {
                                    this.PatientProfileSuccess(param);
                                }
                                else
                                {
                                    WaitingManager.Hide();
                                    if (!this.isShowMess)
                                    {
                                        MessageManager.Show(this.ParentForm, param, false);
                                    }
                                    this.ChangeLockButtonWhileProcess(true);
                                }
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else
                {
                    MessageManager.Show(param, null);
                    this.ChangeLockButtonWhileProcess(true);
                }
                Inventec.Common.Logging.LogSystem.Debug("UcRegister => Save Process -> end");
            }
            catch (Exception ex)
            {
                this.ChangeLockButtonWhileProcess(true);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check(CommonParam param)
        {
            bool valid = true;
            bool validPatientInfo = true;
            bool validPatientPlusInfo = true;
            bool validPhoneNumber = true;
            try
            {
                this.UpdatepatientDob();

                this.positionHandleControl = -1;
                this.positionHandlePlusInfoControl = -1;


                validPatientPlusInfo = this.dxValidationProviderPlusInfomation.Validate();

                valid = this.CheckHeinOrKskContract();
                valid = CheckUseBhytAllow() && valid;
                validPatientInfo = this.dxValidationProviderControl.Validate();

                if (!validPatientInfo)
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    int tabIndexMin = invalidControls[0].TabIndex;
                    int controlFocus = 0;
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        if (tabIndexMin < invalidControls[i].TabIndex)
                        {
                            tabIndexMin = invalidControls[i].TabIndex;
                            controlFocus = i;

                        }
                        LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                }

                this.currentFactorySaveType = this.GetEnumSaveType(param);
                if (currentFactorySaveType == UCServiceRequestRegisterFactorySaveType.VALID)
                {
                    if (validPatientPlusInfo && valid)
                        this.FocusInService();
                    valid = false;
                }
                string phoneNumber = txtPhone.Text;
                if (string.IsNullOrEmpty(phoneNumber.Trim()))
                {
                    if (HisConfigCFG.PhoneRequired == "1")
                    {
                        MessageBox.Show("Bạn chưa nhập Điện thoại", "Thông báo", MessageBoxButtons.OK);
                        txtPhone.Focus();
                        txtPhone.SelectAll();
                        validPhoneNumber = false;
                    }
                    else if (HisConfigCFG.PhoneRequired == "2")
                    {
                        if (MessageBox.Show("Bạn chưa nhập Điện thoại. Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            txtPhone.Focus();
                            txtPhone.SelectAll();
                            validPhoneNumber = false;
                        }
                    }
                }
                valid = valid && validPatientInfo && validPatientPlusInfo && validPhoneNumber;
                valid = valid && this.AlertExpriedTimeHeinCardBhyt();
                valid = valid && this.BlockingInvalidBhyt();
                valid = valid && this.CheckRRCodeTTFee(true);
                valid = valid && this.CheckDuplicateCCCD();
                try
                {
                    departmentId = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId).DEPARTMENT_ID;
                }
                catch { }
                if (departmentId == 0)
                    throw new ArgumentNullException("departmentId == 0");
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }
        private bool CheckDuplicateCCCD()
        {
            bool result = true;
            try
            {
                if ((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2") &&  patientInformation != null && !string.IsNullOrEmpty(patientInformation.CMND_NUMBER))
                {
                    var data = patientInformation.CMND_NUMBER.Trim();
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
                        if (currentPatientSDO == null || string.IsNullOrEmpty(currentPatientSDO.PATIENT_CODE) || (currentPatientSDO != null && !string.IsNullOrEmpty(currentPatientSDO.PATIENT_CODE) && !patients.Exists(o => o.PATIENT_CODE == currentPatientSDO.PATIENT_CODE)))
                        {
                            if ((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" && XtraMessageBox.Show(string.Format("Số CCCD {0} đã được sử dụng bởi bệnh nhân có mã {1}", data, patients.OrderByDescending(o => o.PATIENT_CODE).ToList()[0].PATIENT_CODE), "Thông báo", MessageBoxButtons.OK) == DialogResult.OK) || (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2" && XtraMessageBox.Show(string.Format("Số CCCD {0} đã được sử dụng bởi bệnh nhân có mã {1}", data, patients.OrderByDescending(o => o.PATIENT_CODE).ToList()[0].PATIENT_CODE), "Cảnh báo", MessageBoxButtons.YesNo) == DialogResult.No))
                            {
                                string oldTypeFind = typeCodeFind;
                                this.typeCodeFind = typeCodeFind__CCCDCMND;
                                txtPatientName.Text = null;
                                dtPatientDob.EditValue = null;
                                txtPatientDob.Text = null;
                                SearchPatientByCodeOrQrCode(data);
                                typeCodeFind = oldTypeFind;
                                result = false;
                            } 
                        }
                    }
                }else if((HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "1" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.CHECK_DUPLICATION == "2") && !chkNoCCCD.Checked)
                {
                    XtraMessageBox.Show("Thiếu thông tin trường CMT/CCCD/HC", "Thông báo");
                    result = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }


        private bool CheckUseBhytAllow()
        {
            bool valid = true;
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HisConfigCFG.PatientTypeId__QN)
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                            this.mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, dataPatientProfile);
                            var mediOrg = BackendDataWorker.Get<HIS_MEDI_ORG>().FirstOrDefault(o => o.MEDI_ORG_CODE == dataPatientProfile.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE);
                            if (!string.IsNullOrEmpty(BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE) && mediOrg != null && (";" + BranchDataWorker.Branch.DO_NOT_ALLOW_HEIN_LEVEL_CODE + ";").Contains(";" + mediOrg.LEVEL_CODE + ";"))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(
                                            String.Format("Nơi đăng ký khám chữa bệnh ban đầu thuộc tuyến {0}, không được hưởng BHYT", mediOrg.LEVEL_CODE == "1" ? "trung ương" : (mediOrg.LEVEL_CODE == "2" ? "Tỉnh" : (mediOrg.LEVEL_CODE == "3" ? "Huyện" : "Xã"))),
                                            ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                                            MessageBoxButtons.OK);
                                valid = false;
                            }
                        }
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
                if (this.ucHeinBHYT == null || mainHeinProcessor == null)
                    return true;
                HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                dataPatientProfile.HisPatientTypeAlter = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE_ALTER();
                //Đồng bộ dữ liệu thay đổi từ uchein sang đối tượng dữ liệu phục vụ làm đầu vào cho gọi api
                this.mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, dataPatientProfile);


                if (this.ResultDataADO != null && ResultDataADO.ResultHistoryLDO != null && HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.WarningInvalidCheckHistoryHeinCard && ResultDataADO.ResultHistoryLDO.message == "Thẻ BHYT có thông tin kiểm tra thẻ chưa ra viện.")
                {
                    DialogResult drReslt = DevExpress.XtraEditors.XtraMessageBox.Show(ResultDataADO.ResultHistoryLDO.message + " Bạn có muốn tiếp tục?", "Thông báo", MessageBoxButtons.YesNo);
                    if (drReslt == DialogResult.No)
                    {
                        return false;
                    }
                }

                //không kiểm tra nếu có check vào thẻ tạm
                if (this.cboPatientType.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__BHYT && (HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString())
                    && !CheckBhytWhiteListAcceptNoCheckBHYT(dataPatientProfile.HisPatientTypeAlter.HEIN_CARD_NUMBER) && dataPatientProfile.HisPatientTypeAlter.HAS_BIRTH_CERTIFICATE != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
                    if (this.ResultDataADO == null || ResultDataADO.ResultHistoryLDO == null)//thẻ không hợp lệ
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ResultDataADO), ResultDataADO));
                        XtraMessageBox.Show("Chưa kiểm tra được thông tin thẻ từ hệ thống BHYT. Vui lòng chờ vài giây và thực hiện lưu lại!");
                        return false;
                    }
                    else if (HisConfigCFG.MaKetQuaBlockings.Contains(ResultDataADO.ResultHistoryLDO.maKetQua))//mã lỗi nằm trong các mã lỗi thẻ hết hạn
                    {
                        Inventec.Common.Logging.LogSystem.Info("maKetQua: " + ResultDataADO.ResultHistoryLDO.maKetQua);
                        XtraMessageBox.Show("Thẻ BHYT không hợp lệ. Không cho phép đăng ký với đối tượng BHYT");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
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

        private bool CheckHeinOrKskContract()
        {
            bool vali = true;
            try
            {
                if (cboPatientType.EditValue != null)
                {
                    long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString());
                    if (patientTypeId == HisConfigCFG.PatientTypeId__BHYT || patientTypeId == HisConfigCFG.PatientTypeId__QN)
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                            vali = this.mainHeinProcessor.GetInvalidControls(this.ucHeinBHYT);
                    }
                    else if (patientTypeId == HisConfigCFG.PatientTypeId__KSK)
                    {
                        if (this.kskContractProcessor != null && this.ucKskContract != null)
                            vali = this.kskContractProcessor.GetValidate(this.ucKskContract);
                    }
                }
            }
            catch (Exception ex)
            {
                vali = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return vali;
        }

        private bool AlertExpriedTimeHeinCardBhyt()
        {
            bool valid = false;
            long resultDayAlert = -1;
            try
            {
                if (this.cboPatientType.EditValue != null && Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) == HisConfigCFG.PatientTypeId__BHYT)
                {
                    if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    {
                        resultDayAlert = this.mainHeinProcessor.AlertExpriedTimeHeinCardBhyt(this.ucHeinBHYT, AppConfigs.AlertExpriedTimeHeinCardBhyt, ref resultDayAlert);
                    }

                    if (resultDayAlert > -1)
                    {
                        if (MessageBox.Show(String.Format(ResourceMessage.CanhBaoTheBhytSapHatHan, resultDayAlert), ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            valid = true;
                        }
                        else
                            valid = false;
                    }
                    else
                        valid = true;
                }
                else
                    valid = true;
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
                        this.isNotCheckTT = true;
                        if (this.typeCodeFind != typeCodeFind__MaHK && this.typeCodeFind != this.typeCodeFind__MaNV)
                        {
                            this.txtPatientCode.Text = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.PATIENT_CODE;
                        }
                        if (this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter != null)
                        {
                            this.mainHeinProcessor.FillDataHeinInsuranceInfoByPatientTypeAlter(this.ucHeinBHYT, this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter);
                        }
                        this.serviceReqIdForCreated = this.currentHisExamServiceReqResultSDO.ServiceReqs[0].ID;
                        success = true;
                        this.actionType = GlobalVariables.ActionView;
                        this.EnableButton(actionType, true);
                        this.FillDataToGirdExecuteRoomInfo();
                        this.SuccessLog(this.currentHisExamServiceReqResultSDO.HisPatientProfile);

                        this.txtPatientName.Focus();
                        this.txtPatientName.SelectAll();
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
                        this.isNotCheckTT = true;
                        if (this.typeCodeFind != typeCodeFind__MaHK && this.typeCodeFind != this.typeCodeFind__MaNV)
                        {
                            this.txtPatientCode.Text = this.resultHisPatientProfileSDO.HisPatient.PATIENT_CODE;
                        }

                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                            this.mainHeinProcessor.FillDataHeinInsuranceInfoByPatientTypeAlter(this.ucHeinBHYT, this.resultHisPatientProfileSDO.HisPatientTypeAlter);

                        success = true;
                        this.actionType = GlobalVariables.ActionView;
                        this.EnableButton(actionType, false);
                        this.SuccessLog(this.resultHisPatientProfileSDO);
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

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("Service request PatientProfile end process: time=" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            MessageManager.Show(this.ParentForm, param, success);
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
                    this.btnDepositDetail.Enabled = false;
                    this.btnBill.Enabled = false;
                    this.dropDownButton_Other.Enabled = false;
                }
                else
                {
                    this.btnSave.Enabled = false;
                    this.btnSaveAndPrint.Enabled = false;
                    this.btnPrint.Enabled = true;// isPrint;
                    this.btnSaveAndAssain.Enabled = true;
                    this.btnTreatmentBedRoom.Enabled = true;
                    this.btnDepositDetail.Enabled = true;
                    this.btnBill.Enabled = true;
                    this.dropDownButton_Other.Enabled = true;

                    HIS_PATIENT_TYPE_ALTER hisPatientTypeAlter = null;

                    //resultHisPatientProfileSDO,currentHisExamServiceReqResultSDO = null khi bam nut. Va chi ton tai 1 bien co gia tri. Yen tam di
                    if (currentHisExamServiceReqResultSDO != null && currentHisExamServiceReqResultSDO.HisPatientProfile != null && currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter != null)
                    {
                        hisPatientTypeAlter = currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatientTypeAlter;
                    }

                    if (resultHisPatientProfileSDO != null && resultHisPatientProfileSDO.HisPatientTypeAlter != null)
                    {
                        hisPatientTypeAlter = resultHisPatientProfileSDO.HisPatientTypeAlter;
                    }

                    if (hisPatientTypeAlter != null)
                    {
                        if (hisPatientTypeAlter.TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM)
                        {
                            this.btnTreatmentBedRoom.Enabled = true;
                        }
                        else
                        {
                            this.btnTreatmentBedRoom.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SuccessLog(HisPatientProfileSDO result)
        {
            try
            {
                //if (result != null)
                //{
                //    string message = String.Format(HIS.Desktop.EventLog.EventLogUtil.SetLog(His.EventLog.Message.Enum.DangKyTiepDon), result.HisPatient.PATIENT_CODE, result.HisPatient.VIR_PATIENT_NAME, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(result.HisPatient.DOB), cboGender.Text, result.HisTreatment.TREATMENT_CODE, Newtonsoft.Json.JsonConvert.SerializeObject(result.HisTreatment), cboPatientType.Text, Newtonsoft.Json.JsonConvert.SerializeObject(result.HisPatientTypeAlter));
                //    EventLogSuccess(message);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValidationByChildrenUnder6Years(bool isTreSoSinh, bool isHasReset)
        {
            try
            {
                if (isTreSoSinh && HisConfigCFG.MustHaveNCSInfoForChild)
                {
                    if (this.lcitxtHomePerson.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        this.lcitxtHomePerson.AppearanceItemCaption.ForeColor = Color.Maroon;
                    this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Black;
                    //if (this.lcitxtCorrelated.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //    this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Maroon;

                    //if (this.lcitxtRelativeAddress.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Maroon;
                    //    //this.SetRelativeAddress(false);
                    //}

                    //if (this.lciRelativeCMNDNumber.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //    this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lcitxtHomePerson.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Black;
                    this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Black;
                    if (isHasReset)
                        this.txtRelativeAddress.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong != 1)
                {
                    return;
                }
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                if (cboProvince.EditValue != null)
                {
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh");
                }
                if (this.cboDistrict.EditValue != null)
                {
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Huyện");
                }

                if (this.cboCommune.EditValue != null)
                {
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                    this.ChangeReplaceAddress(cboCommune.Text, "Xã");
                }

                if (isReadQrCode)
                    return;
                if (this.cboPatientType.EditValue == null || Inventec.Common.TypeConvert.Parse.ToInt64((this.cboPatientType.EditValue ?? "0").ToString()) != HisConfigCFG.PatientTypeId__BHYT)
                    return;
                //if (this.currentPatientSDO != null && this.currentPatientSDO.ID > 0)
                //    return;
                //if (this.cboProvince.EditValue == null && String.IsNullOrEmpty(this.txtAddress.Text))
                //    return;

                string address = txtAddress.Text;
                string heinAddress = string.Format("{0}{1}{2}{3}", address, (commune != null ? " " + commune.INITIAL_NAME + " " + commune.COMMUNE_NAME : ""), (district != null ? ", " + district.INITIAL_NAME + " " + district.DISTRICT_NAME : ""), (province != null ? ", " + province.PROVINCE_NAME : ""));
                this.mainHeinProcessor.SetValueAddress(this.ucHeinBHYT, heinAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}
