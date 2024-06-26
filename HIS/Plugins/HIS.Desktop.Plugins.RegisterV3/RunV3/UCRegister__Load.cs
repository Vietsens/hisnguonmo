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

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        async void ProcessSearchByCode(string searchCode)
        {
            try
            {
                //Lay gia tri ma nhap vao
                //Kiem tra du lieu ma la qrcode hay ma benh nhan 
                //Neu la qrcode se doc chuoi ma hoa tra ve doi tuong heindata
                //Neu la ma benh nhan thi goi api kiem tra co du lieu benh nhan tuong ung voi ma hay khong, co thi tra ve du lieu BN
                var data = (ProcessSearchByCodeAsync(searchCode));
                this.Invoke(new MethodInvoker(delegate() { ProcessPatientCodeKeydown(data); }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task<object> ProcessSearchByCodeAsync(string searchCode)
        {
            try
            {
                return SearchByCode(searchCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void ProcessPatientCodeKeydown(object data)
        {
            try
            {
                //Lay gia tri ma nhap vao
                if (data != null)
                {
                    string heinCardNumder = "";
                    this.qrCodeBHYTHeinCardData = null;
                    this.SetPatientSearchPanel(false);
                    //Trường hợp tìm ra bệnh nhân cũ                    
                    if (data is HisPatientSDO)
                    {
                        //An hien cac button lam moi thong tin benh nhan
                        this.SetPatientSearchPanel(true);
                        var patient = data as HisPatientSDO;
                        //Fill du lieu vao vung thong tin benh nhan
                        LoadOneBNToControl(patient, true);
                        heinCardNumder = patient.HeinCardNumber;

                        //xuandv
                        this._HeinCardData = this.ConvertFromPatientData(patient);

                        //Luôn gửi sang con CHC để check thẻ bhyt
                        //this.CheckheinCardFromHeinInsuranceApi(this.ConvertFromPatientData(patient));

                        //this.CheckheinCardFromHeinInsuranceApi(this._HeinCardData);
                    }
                    //Trường hợp quẹt thẻ bhyt với bệnh nhân mới
                    else if (data is HeinCardData)
                    {
                        //xuandv
                        this._HeinCardData = (HeinCardData)data;
                        heinCardNumder = this._HeinCardData.HeinCardNumber;
                        this.ProcessQrCodeData(this._HeinCardData);

                        //Luôn gửi sang con CHC để check thẻ bhyt


                        //this.CheckheinCardFromHeinInsuranceApi(this._HeinCardData);
                    }
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

        private void ProcessQrCodeData(HeinCardData dataHein)
        {
            try
            {
                this.isReadQrCode = true;
                this.qrCodeBHYTHeinCardData = dataHein;
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
                filter.HEIN_CARD_NUMBER__EXACT = this.qrCodeBHYTHeinCardData.HeinCardNumber;
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
                        //if (String.IsNullOrEmpty(this.ucAddressCombo1.txtAddress.Text))
                        //this.ucAddressCombo1.txtAddress.Text = patients[0].ADDRESS;
                    }
                }
                else//Nguoc lai so the chua ton tai tren he thong, hien thi du lieu da doc duoc tu qrcode len form
                {
                    LogSystem.Debug("Quet the BHYT khong tim thay Bn cu => fill du lieu theo du lieu gih tren the bhyt");
                    this.currentPatientSDO = null;

                    FillDataAfterFindQrCodeNoExistsCardWithThread();
                    this.ucPatientRaw1.txtPatientCode.Text = "";
                    this.ucPatientRaw1.txtPatientCode.Update();
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
                //FillDataAfterFindQrCodeNoExistsCard(this.qrCodeBHYTHeinCardData);
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
                LogSystem.Debug("SelectOnePatientProcess => t1. Gan thong tin cua benh nhan len form");
                this.actionType = GlobalVariables.ActionAdd;
                this.SetPatientSearchPanel(true);

                //Nếu thực hiện tìm kiếm theo (họ tên + ngày sinh + giới tính) -> tìm ra BN cũ -> giữ nguyên ngày sinh đang nhập
                //if (this.ucPatientRaw1.txtPatientDob.Text.Length == 4)
                //    patient.IS_HAS_NOT_DAY_DOB = 1;
                //else
                //    patient.IS_HAS_NOT_DAY_DOB = null;

                this.currentPatientSDO = patient;
                this.LoadOneBNToControlWithThread();

                LogSystem.Debug("SelectOnePatientProcess => t2. SetDefaultFocusUserControl");
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan == 1)
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
                //LogSystem.Debug("LoadOneBNToControlWithThread => 1");
                this.FillDataPatientToControlWithThread();
                this.FillDataToExamServiceReqNewestByPatientWithThread();
                this.PeriosTreatmentMessage();
                //this.GetCareerByConfig();
                if (this.qrCodeBHYTHeinCardData != null && !String.IsNullOrEmpty(this.qrCodeBHYTHeinCardData.HeinCardNumber))
                    this.FillDataAfterFindQrCodeNoExistsCardWithThread();
                //else
                //    this.FillDataToHeinInsuranceInfoUCHeinByOldPatientWithThread();
                //LogSystem.Debug("LoadOneBNToControlWithThread => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void PeriosTreatmentMessage()
        {
            try
            {
                //- Kiểm tra cấu hình trên CCC: MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION
                //1: Khi đăng ký tiếp đón, có kiểm tra xem đợt khám/điều trị trước đó của BN đã uống hết thuốc hay chưa 
                //0: Không kiểm tra
                LogSystem.Debug("Tiep don: Cau hinh co kiem tra dot dieu tri truoc cua BN con thuoc chua uong het hay khong: IsCheckPreviousPrescription = " + HisConfigCFG.IsCheckPreviousPrescription);
                string message = "";
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
                if (HisConfigCFG.IsCheckPreviousDebt == "1")
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
                        message += String.Format(ResourceMessage.DotKhamTruocCuaBenhNhanConNoTienVienPhi, treatmentPrevis);
                    }
                }
                if (!String.IsNullOrEmpty(message))
                {
                    MessageManager.Show(message);
                }
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

        private void FillDataToExamServiceReqNewestByPatient(HisPatientSDO data)
        {
            try
            {
                if (data == null) throw new ArgumentNullException("FillDataToExamServiceReqNewestByPatient. Get HisPatientSDO is null");
                if (this.roomExamServiceProcessor != null && this.ucRoomExamService != null)
                {
                    V_HIS_PATIENT vPatient = new V_HIS_PATIENT();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_PATIENT>(vPatient, data);

                    this.roomExamServiceProcessor.SetValueByPatient(this.ucRoomExamService, (AppConfigs.IsAutoFillDataRecentServiceRoom == "1" ? vPatient : new V_HIS_PATIENT()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataPatientToControlWithThread()
        {
            try
            {
                //this.FillDataPatientRawInfo(this.currentPatientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void LoadOneBNToControl(HisPatientSDO patientDTO, bool isReloadUCHein)
        {
            try
            {
                if (patientDTO != null)
                {
                    LogSystem.Debug("Bat dau gan du lieu benh nhan len form");
                    this.currentPatientSDO = patientDTO;
                    //this.FillDataPatientRawInfo(patientDTO);

                    LogSystem.Debug("t3.3. Call uc BHYT Load Combo TheBHYT cua benh nhan id = " + patientDTO.ID);
                    //if (isReloadUCHein && this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    //    this.mainHeinProcessor.FillDataToHeinInsuranceInfoByOldPatient(this.ucHeinBHYT, patientDTO);

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
                else if (this.hisPatientVitaminASDOSave != null)
                {
                    assignServiceADO.PatientName = this.hisPatientVitaminASDOSave.HisPatient.VIR_PATIENT_NAME;
                    assignServiceADO.PatientDob = this.hisPatientVitaminASDOSave.HisPatient.DOB;
                    assignServiceADO.GenderName = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(o => o.ID == this.hisPatientVitaminASDOSave.HisPatient.GENDER_ID).GENDER_NAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
