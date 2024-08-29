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
using HIS.Desktop.Utility;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using DevExpress.XtraEditors;
using HIS.UC.UCPatientRaw.ADO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        public List<string> lstPreviousDebtTreatmentsRegister = new List<string>();
        private void PeriosTreatmentMessage()
        {
            try
            {
                //- Kiểm tra cấu hình trên CCC: MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION
                //1: Khi đăng ký tiếp đón, có kiểm tra xem đợt khám/điều trị trước đó của BN đã uống hết thuốc hay chưa 
                //0: Không kiểm tra
                LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con thuoc chua uong het hay khong: IsCheckPreviousPrescription = " + HisConfigCFG.IsCheckPreviousPrescription);
                string message = "";
                lstPreviousDebtTreatmentsRegister = new List<string>();
                if (HisConfigCFG.IsCheckPreviousPrescription)
                {
                    if (this.currentPatientSDO.PreviousPrescriptions != null
                        && this.currentPatientSDO.PreviousPrescriptions.Count > 0)
                    {
                        LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
                        string treatmentPrevis = this.currentPatientSDO.TreatmentCode;
                        string pressMessages = "";
                        for (int i = 0; i < this.currentPatientSDO.PreviousPrescriptions.Count; i++)
                        {
                            pressMessages += String.Format(ResourceMessage.ThuocCoThoiSuDungDen,
                                (" - " + this.currentPatientSDO.PreviousPrescriptions[i].REQUEST_ROOM_NAME + " ")
                                , Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.currentPatientSDO.PreviousPrescriptions[i].USE_TIME_TO ?? 0) + "\r\n");
                        }
                        message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanCoThuocChuaUongHet, treatmentPrevis, "\r\n", pressMessages, "");
                    }
                }
                LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con no tien vien phi hay khong: IsCheckPreviousDebt = " + HisConfigCFG.IsCheckPreviousDebt);
                var dtPatientType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().Find(o => o.PATIENT_TYPE_CODE == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"));
                btnSave.Enabled = true;
                if (HisConfigCFG.IsCheckPreviousDebt == "1" || HisConfigCFG.IsCheckPreviousDebt == "3" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4")
                {
                    if (this.currentPatientSDO.PreviousDebtTreatments != null
                        && this.currentPatientSDO.PreviousDebtTreatments.Count > 0)
                    {
                        LogSystem.Debug("Tiep don: Du lieu benh nhan cu: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.currentPatientSDO), this.currentPatientSDO));
                        string treatmentPrevis = String.Join(",", this.currentPatientSDO.PreviousDebtTreatments);
                        if (!String.IsNullOrEmpty(message))
                        {
                            message += "\r\n";
                        }
                        if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4")
                        {
                            message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân có số tiền phải trả lớn hơn 0  hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}. Bạn có muốn đăng ký tiếp đón không?", treatmentPrevis);
                        }
                        else
                        {
                            if (HisConfigCFG.IsCheckPreviousDebt == "1")
                            {
                                message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi, treatmentPrevis);
                            }
                            if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3" && this.currentPatientSDO.LastTreatmentFee != null && (this.currentPatientSDO.LastTreatmentFee.IS_ACTIVE == 1 || ((this.currentPatientSDO.LastTreatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (this.currentPatientSDO.LastTreatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - (this.currentPatientSDO.LastTreatmentFee.TOTAL_BILL_AMOUNT ?? 0) + (this.currentPatientSDO.LastTreatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) + (this.currentPatientSDO.LastTreatmentFee.TOTAL_REPAY_AMOUNT ?? 0)) > 0))
                            {
                                lstPreviousDebtTreatmentsRegister = this.currentPatientSDO.PreviousDebtTreatments;
                                message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi3, this.currentPatientSDO.LastTreatmentFee.TREATMENT_CODE);
                            }
                        }
                    }
                }
                else if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "2" && !IsEmergency && dtPatientType != null && this.currentPatientSDO.PreviousDebtTreatmentDetails != null
                       && this.currentPatientSDO.PreviousDebtTreatmentDetails.Count > 0)
                {
                    var dtTreatmentDetails = this.currentPatientSDO.PreviousDebtTreatmentDetails.Where(o => o.PATIENT_TYPE_ID == dtPatientType.ID).ToList();
                    if (dtTreatmentDetails != null && dtTreatmentDetails.Count > 0)
                    {
                        string treatmentPrevis = String.Join(",", dtTreatmentDetails.Select(o => o.TDL_TREATMENT_CODE).ToList());
                        if (!String.IsNullOrEmpty(message))
                        {
                            message += "\r\n";
                        }
                        message += String.Format("Đợt khám/điều trị trước đó của bệnh nhân còn nợ viện phí hoặc chưa duyệt khóa viện phí. Mã hồ sơ điều trị {0}. Không cho phép tiếp đón", treatmentPrevis);
                        btnSave.Enabled = false;
                        btnSaveAndPrint.Enabled = false;
                    }
                }
                if (!String.IsNullOrEmpty(message))
                {

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "4" || HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckPreviousDebt == "3")
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(message, "Thông báo", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        {
                            btnSave.Enabled = false;
                            btnSaveAndPrint.Enabled = false;
                        }
                    }
                    else
                    {
                        MessageManager.Show(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToExamServiceReqNewestByPatient(HisPatientSDO data)
        {
            try
            {
                if (data == null) throw new ArgumentNullException("FillDataToExamServiceReqNewestByPatient. Get HisPatientSDO is null");
                if (this.ucServiceRoomInfo1 != null)
                {
                    V_HIS_PATIENT vPatient = new V_HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(vPatient, data);

                    this.ucServiceRoomInfo1.SetValueExamServiceRoom((AppConfigs.IsAutoFillDataRecentServiceRoom == "1" ? vPatient : new V_HIS_PATIENT()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HeinCardData ConvertFromPatientData(HisPatientSDO patient)
        {
            HeinCardData hein = null;
            try
            {
                hein = new HeinCardData();
                hein.Address = patient.HeinAddress;
                if (patient.IS_HAS_NOT_DAY_DOB == 1)
                {
                    hein.Dob = patient.DOB.ToString().Substring(0, 4);
                }
                else
                    hein.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                hein.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardFromTime ?? 0));
                hein.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString((patient.HeinCardToTime ?? 0));
                hein.MediOrgCode = patient.HeinMediOrgCode;
                hein.HeinCardNumber = patient.HeinCardNumber;
                hein.LiveAreaCode = patient.LiveAreaCode;
                hein.PatientName = patient.VIR_PATIENT_NAME;
                hein.FineYearMonthDate = patient.Join5Year;
                hein.Gender = GenderConvert.HisToHein(patient.GENDER_ID.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        private List<HIS_CASHIER_ROOM> GetCashierRoomByUser()
        {
            List<HIS_CASHIER_ROOM> result = new List<HIS_CASHIER_ROOM>();
            try
            {
                //Ci hien thi phong thu ngan ma ng dung chon lam viec
                var roomIds = WorkPlace.GetRoomIds();
                if (roomIds == null || roomIds.Count == 0)
                    throw new ArgumentNullException("Nguoi dung khong chon phong thu ngan nao");
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CASHIER_ROOM>().Where(o => roomIds.Contains(o.ROOM_ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void GetPatientInfoFromResultData(ref AssignServiceADO assignServiceADO)
        {
            try
            {
                if (this.resultHisPatientProfileSDO != null)
                {
                    assignServiceADO.PatientName = this.resultHisPatientProfileSDO.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.resultHisPatientProfileSDO.HisPatient.DOB;
                    assignServiceADO.GenderName = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.resultHisPatientProfileSDO.HisPatient.GENDER_ID).GENDER_NAME;
                }
                else if (this.currentHisExamServiceReqResultSDO != null)
                {
                    assignServiceADO.PatientName = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.DOB;
                    assignServiceADO.GenderName = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.currentHisExamServiceReqResultSDO.HisPatientProfile.HisPatient.GENDER_ID).GENDER_NAME;
                }
                if (this.currentHisExamServiceReqResultSDO != null && this.currentHisExamServiceReqResultSDO.ServiceReqs != null && this.currentHisExamServiceReqResultSDO.ServiceReqs.Count > 0)
                {
                    var serviceReqHasPriority = this.currentHisExamServiceReqResultSDO.ServiceReqs.FirstOrDefault(o => o.PRIORITY == PRIORITY_TRUE);
                    assignServiceADO.IsPriority = serviceReqHasPriority != null ? serviceReqHasPriority.PRIORITY == PRIORITY_TRUE : false;
                }
                if (HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService && this.serviceReqDetailSDOs != null && this.serviceReqDetailSDOs.Count > 0 && this.serviceReqDetailSDOs.Exists(o => (o.RoomId ?? 0) > 0))
                {
                    assignServiceADO.ExamRegisterRoomId = this.serviceReqDetailSDOs.Where(o => (o.RoomId ?? 0) > 0).FirstOrDefault().RoomId;
                    Inventec.Common.Logging.LogSystem.Info("Tiep don benh nhan___co cau hinh:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService), HisConfigCFG.SetDefaultRequestRoomByExamRoomWhenAssigningService) + " => luon mac dinh phong chi dinh khi chi dinh dv tu tiep don theo phong kham dau tien nguoi dung chon");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void ProcessPatientCodeKeydown(object data)
        {
            try
            {
                this.RefreshUserControl();
                //Lay gia tri ma nhap vao
                if (data != null)
                {
                    this.ResultDataADO = null;
                    this._HeinCardData = null;
                    string heinAddressOfPatient = "";
                    this.SetPatientSearchPanel(false);
                    this.isReadQrCode = false;
                    //Trường hợp tìm ra bệnh nhân cũ
                    if (data is HisPatientSDO)
                    {
                        if (!AlertTreatmentInOutInDayForTreatmentMessage(data as HisPatientSDO))
                        {
                            this.currentPatientSDO = null;
                            this._HeinCardData = null;
                            this.ResultDataADO = null;
                            ResetPatientForm();
                            return;
                        }

                        //An hien cac button lam moi thong tin benh nhan
                        this.SetPatientSearchPanel(true);
                        var patient = data as HisPatientSDO;
                        //Fill du lieu vao vung thong tin benh nhan
                        LoadOneBNToControl(patient, true);
                        heinAddressOfPatient = patient.HeinAddress;
                        //heinCardNumder = patient.HeinCardNumber;

                        //xuandv
                        this._HeinCardData = this.ConvertFromPatientData(patient);
                    }
                    //Trường hợp quẹt thẻ bhyt với bệnh nhân mới
                    else if (data is HeinCardData)
                    {
                        //xuandv
                        this._HeinCardData = (HeinCardData)data;
                        this.isReadQrCode = true;
                        string patientName = Inventec.Common.String.Convert.HexToUTF8Fix(this._HeinCardData.PatientName);
                        if (!string.IsNullOrEmpty(patientName))
                            this._HeinCardData.PatientName = patientName;
                        string address = Inventec.Common.String.Convert.HexToUTF8Fix(this._HeinCardData.Address);
                        if (!string.IsNullOrEmpty(address))
                            this._HeinCardData.Address = address;
                        FillDataAfterFindQrCodeNoExistsCard(_HeinCardData);
                        //this.ProcessQrCodeData(this._HeinCardData);
                    }

                    //this.CheckheinCardFromHeinInsuranceApi(this._HeinCardData);

                    HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                    var ado = this.ucOtherServiceReqInfo1.GetValue();

                    if (this.ucPatientRaw1 != null)
                    {
                        UCPatientRawADO patientRawADO = this.ucPatientRaw1.GetValue();
                        if (patientRawADO.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            DateTime time = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ado.IntructionTime).Value;
                            if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
                                heinGOVManager.SetDelegateHeinEnableButtonSave(HeinEnableSave);
                            this.ResultDataADO = await heinGOVManager.Check(this._HeinCardData, null, false, heinAddressOfPatient, time, this.isReadQrCode);
                        }
                    }

                    if (this.ResultDataADO != null)
                    {
                        //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                        ucPatientRaw1.ResultDataADO = ResultDataADO;
                        if (!String.IsNullOrEmpty(this._HeinCardData.HeinCardNumber))
                        {
                            if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                            {
                                this._HeinCardData.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maTheMoi;
                            }
                        }
                    }
                    if (this.isReadQrCode)
                        this.ProcessQrCodeData(this._HeinCardData);
                    if (this.ucPatientRaw1 != null)
                    {
                        UCPatientRawADO patientRawADO = this.ucPatientRaw1.GetValue();
                        if (patientRawADO.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                        {
                            await this.CheckTTProcessResultData(this._HeinCardData);
                        }
                    }

                    this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(this._HeinCardData, true);
                }
                //#4671
                else
                {
                    int n;
                    bool isNumeric = int.TryParse(this.ucPatientRaw1.txtPatientCode.Text, out n);
                    string codeFind = "";
                    if (isNumeric)
                    {
                        codeFind = string.Format("{0:0000000000}", Convert.ToInt64(this.ucPatientRaw1.txtPatientCode.Text));
                    }
                    else
                    {
                        codeFind = this.ucPatientRaw1.txtPatientCode.Text;
                    }
                    this.ucPatientRaw1.txtPatientCode.Text = codeFind;
                    //this.lciProvinceKS.AppearanceItemCaption.ForeColor = (HisConfigCFG.IsSyncHID ? Color.Maroon : Color.Black);
                    //this.lblDescriptionForHID.Text = "";
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaBenhNhanKhongTontai + " '" + codeFind + "'", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    this.ucPatientRaw1.txtPatientCode.Focus();
                    this.ucPatientRaw1.txtPatientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool AlertTreatmentInOutInDayForTreatmentMessage(HisPatientSDO patientDTO)
        {
            bool valid = true;
            this.isAlertTreatmentEndInDay = false;
            try
            {
                //Khi nhập thông tin bệnh nhân => tìm thấy mã BN cũ. Kiểm tra hồ sơ điều trị gần nhất của bn. Nếu có ngày ra = ngày hiện tại thì cảnh báo:
                //BN có hồ sơ điều trị: xxxx ra viện ngày hôm nay. Bạn có muốn tiếp tục?
                //Có, tiếp đón bình thường
                //Không, làm mới màn hình tiếp đón. (xóa tất cả trường dữ liệu)                
                string message = "";
                if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckTodayFinishTreatment && patientDTO.ID > 0 && patientDTO.TodayFinishTreatments != null && patientDTO.TodayFinishTreatments.Count > 0)
                {
                    string treatmentCodeInDay = String.Join(",", patientDTO.TodayFinishTreatments);
                    if (!String.IsNullOrEmpty(treatmentCodeInDay))
                    {
                        LogSystem.Debug("Tiep don: tim thay benh nhan cu co dot dieu tri gan nhat ra vien trong ngay: " + LogUtil.TraceData("HisPatientSDO", patientDTO));
                        message += String.Format(ResourceMessage.DotDieuTriGanNhatCuaBenhNhanCoNgayRaLaHomNay, treatmentCodeInDay);
                    }

                    if (!String.IsNullOrEmpty(message))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                       message,
                       ResourceMessage.TieuDeCuaSoThongBaoLaCanhBao,
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            valid = false;
                            this.isAlertTreatmentEndInDay = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void LoadOneBNToControl(HisPatientSDO patientDTO, bool isReloadUCHein)
        {
            try
            {
                if (patientDTO != null)
                {
                    LogSystem.Debug("Bat dau gan du lieu benh nhan len form");
                    this.currentPatientSDO = patientDTO;
                    this.FillDataPatientToControl(patientDTO);

                    LogSystem.Debug("t3.3. Call uc BHYT Load Combo TheBHYT cua benh nhan id = " + patientDTO.ID);
                    if (isReloadUCHein && this.mainHeinProcessor != null && this.ucHeinInfo1 != null)
                        this.mainHeinProcessor.FillDataToHeinInsuranceInfoByOldPatient(this.ucHeinBHYT, patientDTO);

                    this.SetPatientSearchPanel(true);
                    LogSystem.Debug("SetPatientSearchPanel");

                    //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)
                    this.FillDataToExamServiceReqNewestByPatient(patientDTO);
                    LogSystem.Debug("Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)");

                    PeriosTreatmentMessage();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientToControl(HisPatientSDO patientDTO)
        {
            try
            {

                Inventec.Common.Logging.LogSystem.Warn("FillDataPatientToControl");
                if (patientDTO == null) throw new ArgumentNullException("patientDTO is null");

                Inventec.Common.Logging.LogSystem.Debug("DOB " + patientDTO.DOB);
                if (patientDTO.DOB > 0 && patientDTO.DOB.ToString().Length >= 6)
                {
                    if (patientDTO.IS_HAS_NOT_DAY_DOB == 1)
                        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, true);
                    else
                        this.LoadNgayThangNamSinhBNToForm(patientDTO.DOB, false);
                }

                var oldDataPatient = this.ucPatientRaw1.GetValue();
                UCPatientRawADO patient = new UCPatientRawADO();
                patient.CARRER_ID = patientDTO.CAREER_ID;
                patient.DOB = patientDTO.DOB;
                patient.GENDER_ID = patientDTO.GENDER_ID;
                patient.HEIN_CARD_NUMBER = patientDTO.HeinCardNumber;
                patient.IS_HAS_NOT_DAY_DOB = patientDTO.IS_HAS_NOT_DAY_DOB;
                patient.PATIENT_CODE = patientDTO.PATIENT_CODE;
                patient.PATIENT_NAME = patientDTO.VIR_PATIENT_NAME;
                if (oldDataPatient != null)
                    patient.PATIENTTYPE_ID = oldDataPatient.PATIENTTYPE_ID;

                MOS.EFMODEL.DataModels.HIS_CAREER career = this.GetCareerByBhytWhiteListConfig(patientDTO.HeinCardNumber);

                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                if (career != null)
                {
                    // LogSystem.Debug(patientDTO.HeinCardNumber);
                    this.FillDataCareerUnder6AgeByHeinCardNumber(patientDTO.HeinCardNumber);
                }
                else
                {
                    career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == patientDTO.CAREER_ID);
                }
                if (career != null && career.ID > 0)
                {
                    patient.CARRER_ID = patientDTO.CAREER_ID;
                    patient.CARRER_CODE = career.CAREER_CODE;
                }
                //xuandv Kiểm tra xem là trẻ em hay không để validate thông tin người nhà
                bool isTE = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDTO.DOB) ?? DateTime.Now);
                if (isTE)
                {
                    this.SetValidationByChildrenUnder6Years(isTE, false);
                }
                patient.PATIENT_CLASSIFY_ID = patientDTO.PATIENT_CLASSIFY_ID;
                patient.MILITARY_RANK_ID = patientDTO.MILITARY_RANK_ID;
                patient.POSITION_ID = patientDTO.POSITION_ID;
                patient.WORK_PLACE_ID = patientDTO.WORK_PLACE_ID;
                patient.ETHNIC_CODE = patientDTO.ETHNIC_CODE;
                patient.ETHNIC_NAME = patientDTO.ETHNIC_NAME;
                this.ucPatientRaw1.SetValue(patient);

                var plusInfor = this.ucAddressCombo1.GetValue();
                plusInfor.District_Code = patientDTO.DISTRICT_CODE;
                plusInfor.District_Name = patientDTO.DISTRICT_NAME;
                plusInfor.Province_Code = patientDTO.PROVINCE_CODE;
                plusInfor.Province_Name = patientDTO.PROVINCE_NAME;
                plusInfor.Commune_Code = patientDTO.COMMUNE_CODE;
                plusInfor.Commune_Name = patientDTO.COMMUNE_NAME;
                plusInfor.Address = patientDTO.ADDRESS;
                plusInfor.Phone = patientDTO.PHONE;
                this.ucAddressCombo1.SetValue(plusInfor);

                var relative = this.ucRelativeInfo1.GetValue();
                relative.RelativeAddress = patientDTO.RELATIVE_ADDRESS;
                relative.RelativeCMND = patientDTO.RELATIVE_CMND_NUMBER;
                relative.RelativeName = patientDTO.RELATIVE_NAME;
                relative.FatherName = patientDTO.FATHER_NAME;
                relative.MotherName = patientDTO.MOTHER_NAME;
                relative.RelativePhone = patientDTO.RELATIVE_PHONE;
                if (isTE)
                {
                    this.ucRelativeInfo1.SetValue(relative);
                }
                else
                {
                    this.ucRelativeInfo1.SetValue(relative, isTE);
                }

                //this.ucOtherServiceReqInfo1.CheckExamOnline(!string.IsNullOrEmpty(patientDTO.PERSON_CODE));

                var other = this.ucOtherServiceReqInfo1.GetValue();
                other.PATIENT_CLASSIFY_ID = patientDTO.PATIENT_CLASSIFY_ID;
                other.NOTE = patientDTO.NOTE;
                other.IsHiv = patientDTO.IS_HIV == 1;
                this.ucOtherServiceReqInfo1.SetValue(other);

                var heinInfo = this.ucHeinInfo1.GetValue();
                this.ucHeinInfo1.SetValue(patientDTO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadNgayThangNamSinhBNToForm(long dob, bool hasNotDayDob)
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p1: tinh toan nam sinh");
                if (dob > 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("LoadNgayThangNamSinhBNToForm");
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob) ?? DateTime.MinValue;
                    bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                    patient.DOB = dob;
                    this.isNotPatientDayDob = hasNotDayDob;
                    if (hasNotDayDob)
                    {
                        //this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICEREQUESTREGITER_LCI_PATIENT_DOB_YEAR", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        patient.DOB_STR = dtNgSinh.ToString("yyyy");
                    }
                    else
                    {
                        //this.lciPatientDob.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY_UCSERVICEREQUESTREGITER_LCI_PATIENT_DOB", Resources.ResourceLanguageManager.LanguageUCRegister, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                        patient.DOB_STR = dtNgSinh.ToString("dd/MM/yyyy");
                    }

                    HIS.Desktop.Plugins.RegisterV2.CalculatePatientAge.AgeObject ageObject = CalculatePatientAge.Calculate(dob);
                    //if (ageObject != null)
                    //{
                    //    this.ucPatientRaw1.txtAge.EditValue = ageObject.OutDate;
                    //    this.ucPatientRaw1.cboAge.EditValue = ageObject.AgeType;
                    //}

                    if (this.mainHeinProcessor != null)
                    {
                        LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p2: goi uc bhyt update checkbox co giay khai sinh hay khong");
                        this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, isGKS);
                        LogSystem.Debug("Bat dau gan du lieu nam sinh benh nhan len form. p3: ket thuc update");
                    }
                    this.ucPatientRaw1.SetValue(patient);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValidationByChildrenUnder6Years(bool isTreSoSinh, bool isHasReset)
        {
            //try
            //{
            //    if (isTreSoSinh && HisConfigCFG.MustHaveNCSInfoForChild)
            //    {

            //        if (this.layoutControlItem21.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //            this.layoutControlItem21.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        this.lciHomPerson.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        this.lciCMND.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        this.lciAddress.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        this.lciRelative.AppearanceItemCaption.ForeColor = Color.Maroon;

            //        //if (this.lcitxtCorrelated.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //    this.lcitxtCorrelated.AppearanceItemCaption.ForeColor = Color.Maroon;

            //        //if (this.lcitxtRelativeAddress.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //{
            //        //    this.lcitxtRelativeAddress.AppearanceItemCaption.ForeColor = Color.Maroon;
            //        //    //this.SetRelativeAddress(false);
            //        //}

            //        //if (this.lciRelativeCMNDNumber.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
            //        //    this.lciRelativeCMNDNumber.AppearanceItemCaption.ForeColor = Color.Maroon;
            //    }
            //    else
            //    {
            //        this.lciHomPerson.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.layoutControlItem21.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lciCMND.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lciAddress.AppearanceItemCaption.ForeColor = Color.Black;
            //        this.lciRelative.AppearanceItemCaption.ForeColor = Color.Black;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private void ProcessQrCodeData(HeinCardData dataHein)
        {
            try
            {
                // this.isReadQrCode = true;
                string currentHeincardNumber = dataHein.HeinCardNumber;
                if (dataHein == null) throw new ArgumentNullException("ProcessQrCodeData => dataHein is null");
                if (!String.IsNullOrEmpty(dataHein.HeinCardNumber))
                {
                    if (dataHein.HeinCardNumber.Length > 15)
                        dataHein.HeinCardNumber = dataHein.HeinCardNumber.Substring(0, 15);
                    else if (dataHein.HeinCardNumber.Length < 15)
                        LogSystem.Debug("Do dai so the bhyt cua benh nhan khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHeincardNumber), currentHeincardNumber));
                }

                //Kiểm tra đã tồn tại dữ liệu bệnh nhân theo số thẻ bhyt hay không
                CommonParam param = new CommonParam();
                HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                filter.HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber;
                var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                if (patients != null && patients.Count > 0)
                {
                    if (patients.Count > 1)
                    {
                        LogSystem.Debug("Quet the BHYT tim thay " + patients.Count + " benh nhan cu => mo form chon benh nhan => chon 1 => fill du lieu bn duoc chon.");
                        frmPatientChoice frm = new frmPatientChoice(patients, this.SelectOnePatientProcess);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LogSystem.Debug("Quet the BHYT tim thay thong tin bhyt cua benh nhan cu theo so the HeinCardNumber = " + dataHein.HeinCardNumber + ". " + LogUtil.TraceData("HisPatientSDO searched", patients[0]));
                        //An hien cac button lam moi thong tin benh nhan
                        this.SelectOnePatientProcess(patients[0]);
                        this.SetPatientSearchPanel(true);
                        this.ucPatientRaw1.txtPatientCode.Text = patients[0].PATIENT_CODE;

                    }
                }
                else//Nguoc lai so the chua ton tai tren he thong, hien thi du lieu da doc duoc tu qrcode len form
                {
                    LogSystem.Debug("Quet the BHYT khong tim thay Bn cu => fill du lieu theo du lieu gih tren the bhyt");
                    this.currentPatientSDO = null;

                    FillDataAfterFindQrCodeNoExistsCard(dataHein);
                    this.ucPatientRaw1.txtPatientCode.Text = "";
                    this.ucPatientRaw1.txtPatientCode.Update();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectOnePatientProcess(HisPatientSDO patient)
        {
            try
            {
                var oldpatient = this.ucPatientRaw1.GetValue();
                if (!AlertTreatmentInOutInDayForTreatmentMessage(patient))
                {
                    this.currentPatientSDO = null;
                    ResetPatientForm();
                    return;
                }

                LogSystem.Debug("SelectOnePatientProcess => t1. Gan thong tin cua benh nhan len form");
                this.actionType = GlobalVariables.ActionAdd;
                this.SetPatientSearchPanel(true);

                //Nếu thực hiện tìm kiếm theo (họ tên + ngày sinh + giới tính) -> tìm ra BN cũ -> giữ nguyên ngày sinh đang nhập
                if (oldpatient.DOB.ToString().Length == 4)
                    patient.IS_HAS_NOT_DAY_DOB = 1;
                else
                    patient.IS_HAS_NOT_DAY_DOB = null;

                this.currentPatientSDO = patient;
                this.LoadOneBNToControlWithThread();

                LogSystem.Debug("SelectOnePatientProcess => t2. SetDefaultFocusUserControl");
                this.SetDefaultFocusUserControl();
                if (AppConfigs.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan == 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadOneBNToControlWithThread()
        {
            try
            {
                this.FillDataPatientToControlWithThread();
                this.FillDataToExamServiceReqNewestByPatientWithThread();
                this.PeriosTreatmentMessage();
                this.GetCareerByConfig();
                this.FocusInServiceRoomInfo();
                if (this.isReadQrCode)
                    this.FillDataAfterFindQrCodeNoExistsCardWithThread();
                else
                    this.FillDataToHeinInsuranceInfoUCHeinByOldPatientWithThread();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataPatientToControlWithThread()
        {
            try
            {
                this.FillDataPatientToControl(this.currentPatientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToExamServiceReqNewestByPatientWithThread()
        {
            try
            {
                //Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)
                this.FillDataToExamServiceReqNewestByPatient(this.currentPatientSDO);
                LogSystem.Debug("Fill du lieu yeu cau kham moi nhat cua benh nhan (neu co)");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetCareerByConfig()
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                MOS.EFMODEL.DataModels.HIS_CAREER career = GetCareerByBhytWhiteListConfig(this._HeinCardData.HeinCardNumber);

                //Khi người dùng nhập thẻ BHYT, nếu đầu mã thẻ là TE1, thì tự động chọn giá trị của trường "Nghề nghiệp" là "Trẻ em dưới 6 tuổi"
                if (career != null)
                    career = HisConfigCFG.CareerUnder6Age;
                else if (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB) != DateTime.MinValue)
                {
                    var DOB = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB).Value;
                    if (DOB != null)
                    {
                        if (Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB) != DateTime.MinValue && MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(DOB))
                            career = HisConfigCFG.CareerUnder6Age;
                        else if (DateTime.Now.Year - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB).Value.Year <= 18)
                            career = HisConfigCFG.CareerHS;
                        else
                            career = HisConfigCFG.CareerBase;
                    }
                }

                if (career == null)
                    career = HisConfigCFG.CareerBase;

                if (career != null && career.ID > 0)
                {
                    patient.CARRER_ID = career.ID;
                    patient.CARRER_CODE = career.CAREER_CODE;
                }
                Inventec.Common.Logging.LogSystem.Warn("GetCareerByConfig");
                this.ucPatientRaw1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void FocusInServiceRoomInfo()
        {
            try
            {
                if (this.roomExamServiceProcessor != null)
                {
                    foreach (Control item in this.ucHeinInfo1.Controls)
                    {
                        if (item != null && (item is UserControl || item is XtraUserControl))
                        {
                            this.roomExamServiceProcessor.FocusAndShow(item);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataAfterFindQrCodeNoExistsCardWithThread()
        {
            try
            {
                FillDataAfterFindQrCodeNoExistsCard(this._HeinCardData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToHeinInsuranceInfoUCHeinByOldPatientWithThread()
        {
            try
            {
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.FillDataToHeinInsuranceInfoByOldPatient(this.ucHeinBHYT, this.currentPatientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private async Task CheckTTProcessResultData(HeinCardData dataHein)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("dataHein" + dataHein);
                var patient = this.ucPatientRaw1.GetValue();
                if (this.isNotCheckTT) { return; }
                if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
                {
                    dataHein.FineYearMonthDate = this.ResultDataADO.ResultHistoryLDO.ngayDu5Nam;
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                    {
                        var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataGenderId);
                        if (gender != null && gender.ID > 0)
                        {
                            patient.GENDER_ID = gender.ID;
                        }
                        patient.PATIENT_NAME = this.ResultDataADO.ResultHistoryLDO.hoTen;
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, dataHein);
                        }
                    }

                    if (this.ResultDataADO.IsToDate)
                    {
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, this.ResultDataADO.HeinCardData);
                        }

                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc gan du lieu cho benh nhan khi doc the va khong co han den");
                    }

                    if (this.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataGenderId);
                        if (gender != null && gender.ID > 0)
                        {
                            patient.GENDER_ID = gender.ID;
                        }
                        patient.PATIENT_NAME = this.ResultDataADO.ResultHistoryLDO.hoTen;
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, dataHein);
                        }
                    }

                    if (HisConfigCFG.IsCheckExamHistory
                       && this.ResultDataADO.ResultHistoryLDO != null)
                    {
                        if ((this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose || this.ResultDataADO.SuccessWithoutMessage))
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Mo form lich su voi data rsIns");
                            frmCheckHeinCardGOV frm = new frmCheckHeinCardGOV(this.ResultDataADO.ResultHistoryLDO);
                            frm.ShowDialog();
                            this.ucCheckTT1.FillDataIntoUCCheckTT(this.ResultDataADO.ResultHistoryLDO);
                        }
                        else
                        {
                            this.ucCheckTT1.ResetDataControl(this.ResultDataADO.ResultHistoryLDO);
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Debug("CheckTTProcessResultData 1");
                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                }

                this.ucPatientRaw1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AutoCheckPriorityByPriorityType(long patientDob, string heinCardNumber)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("patientDob " + patientDob);
                Inventec.Common.Logging.LogSystem.Debug("heinCardNumber " + heinCardNumber);
                bool hasData = false;
                long patientAge = 0;
                var ucOtherServiceReqInfo = this.ucOtherServiceReqInfo1.GetValue();
                List<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE> dataPriorityTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PRIORITY_TYPE>();
                if (dataPriorityTypes != null && dataPriorityTypes.Count > 0 && (patientDob > 0 || !String.IsNullOrEmpty(heinCardNumber)))
                {
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patientDob).Value;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    DateTime newDate = new DateTime(tongsogiay);
                    int nam = newDate.Year - 1;
                    patientAge = nam == 0 ? 1 : nam;
                    var checkData = dataPriorityTypes.Where(o =>
                        (o.AGE_FROM == null || (o.AGE_FROM.HasValue && o.AGE_FROM <= patientAge))
                        && (o.AGE_TO == null || (o.AGE_TO.HasValue && o.AGE_TO >= patientAge))
                        && (String.IsNullOrEmpty(o.BHYT_PREFIXS) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS) && StartIn(o.BHYT_PREFIXS, heinCardNumber)))
                        && ((o.AGE_FROM.HasValue && o.AGE_FROM > 0) || (o.AGE_TO.HasValue && o.AGE_TO > 0) || (!String.IsNullOrEmpty(o.BHYT_PREFIXS)))
                        ).ToList();
                    hasData = checkData != null && checkData.Count > 0;
                    if (hasData)
                    {
                        ucOtherServiceReqInfo.IsPriority = hasData;
                        ucOtherServiceReqInfo.PriorityType = checkData.FirstOrDefault().ID;
                    }
                    else
                    {
                        ucOtherServiceReqInfo.IsPriority = false;
                        ucOtherServiceReqInfo.PriorityType = null;
                    }

                    this.ucOtherServiceReqInfo1.SetValue(ucOtherServiceReqInfo);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool StartIn(string BHYT_PREFIXS, string heincardnumber)
        {
            bool valid = false;
            try
            {
                List<string> checkData = null;
                if (!String.IsNullOrEmpty(BHYT_PREFIXS) && !String.IsNullOrEmpty(heincardnumber))
                {
                    var arrPrefixs = BHYT_PREFIXS.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrPrefixs != null && arrPrefixs.Count() > 0)
                    {
                        checkData = arrPrefixs.ToList().Where(o => heincardnumber.StartsWith(o)).ToList();
                        valid = (checkData != null && checkData.Count > 0) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private List<HIS_PATIENT_TYPE> LoadPatientTypeExamByPatientType(long patientTypeId)
        {
            List<HIS_PATIENT_TYPE> result = null;
            try
            {
                CommonParam param = new CommonParam();
                HisPatientTypeAllowFilter patientTypeAllowFilter = new HisPatientTypeAllowFilter();
                patientTypeAllowFilter.PATIENT_TYPE_ID = patientTypeId;
                var patientTypeAllows = new BackendAdapter(param).Get<List<HIS_PATIENT_TYPE_ALLOW>>(HisRequestUriStore.HIS_PATIENT_TYPE_ALLOW_GET, ApiConsumers.MosConsumer, patientTypeAllowFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                var patientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                if (patientTypeAllows != null && patientTypeAllows.Count > 0)
                {
                    var arrpatientTypeAllowIds = patientTypeAllows.Select(o => o.PATIENT_TYPE_ID).ToArray();
                    result = patientTypes.Where(o => arrpatientTypeAllowIds.Contains(o.ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        //private bool LoadOneBNToControlForUCHein(HisPatientSDO patientDTO)
        //{
        //    bool valid = true;
        //    try
        //    {
        //        if (!AlertTreatmentInOutInDayForTreatmentMessage(patientDTO))
        //        {
        //            this.currentPatientSDO = null;
        //            this._HeinCardData = null;
        //            this.ResultDataADO = null;
        //            ResetPatientForm();
        //            return false;
        //        }

        //        LoadOneBNToControl(patientDTO, false);

        //        this._HeinCardData = this.ConvertFromPatientData(patientDTO);

        //        this.CheckTTFull(this._HeinCardData, null);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //    return valid;
        //}
    }
}
