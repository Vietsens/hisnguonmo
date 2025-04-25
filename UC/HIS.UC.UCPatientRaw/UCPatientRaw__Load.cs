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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.UCPatientRaw.ClassUCPatientRaw;
using DevExpress.Utils.Menu;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common;
using MOS.SDO;
using Inventec.Common.QrCodeBHYT;
using HIS.UC.UCPatientRaw.Base;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Utility;
using MOS.LibraryHein.Bhyt;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using HIS.UC.UCPatientRaw.ADO;
using DevExpress.Utils;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Common.QrCodeCCCD;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using System.Net;

namespace HIS.UC.UCPatientRaw
{
    public partial class UCPatientRaw : HIS.Desktop.Utility.UserControlBase
    {
        private void InitTypeFind()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemPatientCode = new DXMenuItem(ResourceMessage.typeCodeFind__MaBN, new EventHandler(btnCodeFind_Click));
                itemPatientCode.Tag = "patientCode";
                itemPatientCode.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaBN_ToolTip);
                menu.Items.Add(itemPatientCode);

                DXMenuItem itemProgramCode = new DXMenuItem(ResourceMessage.typeCodeFind__MaCT, new EventHandler(btnCodeFind_Click));
                itemProgramCode.Tag = "programCode";
                itemProgramCode.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaCT_ToolTip);
                menu.Items.Add(itemProgramCode);

                DXMenuItem itemAppointmentCode = new DXMenuItem(ResourceMessage.typeCodeFind__MaHK, new EventHandler(btnCodeFind_Click));
                itemAppointmentCode.Tag = "appointmentCode";
                itemAppointmentCode.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaHK_ToolTip);
                menu.Items.Add(itemAppointmentCode);

                DXMenuItem itemSoThe = new DXMenuItem(ResourceMessage.typeCodeFind__SoThe, new EventHandler(btnCodeFind_Click));
                itemSoThe.Tag = "cardCode";
                itemSoThe.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__SoThe_ToolTip);
                menu.Items.Add(itemSoThe);

                DXMenuItem itemMaNV = new DXMenuItem(ResourceMessage.typeCodeFind__MaNV, new EventHandler(btnCodeFind_Click));
                itemMaNV.Tag = "employeeCode";
                itemMaNV.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaNV_ToolTip);
                menu.Items.Add(itemMaNV);

                DXMenuItem itemMaTV = new DXMenuItem(ResourceMessage.typeCodeFind__MaTV, new EventHandler(btnCodeFind_Click));
                itemMaTV.Tag = "ConsultationRegCode";
                itemMaTV.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaTV);
                menu.Items.Add(itemMaTV);

                DXMenuItem itemMaDT = new DXMenuItem(ResourceMessage.typeCodeFind__MaDT, new EventHandler(btnCodeFind_Click));
                itemMaDT.Tag = "treatmentCode";
                itemMaDT.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaDT_ToolTip);
                menu.Items.Add(itemMaDT);

                DXMenuItem itemMaCMCC = new DXMenuItem(ResourceMessage.typeCodeFind__MaCMCC, new EventHandler(btnCodeFind_Click));
                itemMaCMCC.Tag = "CmndCccd";
                itemMaCMCC.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaCMCC_ToolTip);
                menu.Items.Add(itemMaCMCC);

                DXMenuItem itemSoDT = new DXMenuItem(ResourceMessage.typeCodeFind__SoDT, new EventHandler(btnCodeFind_Click));
                itemSoDT.Tag = "SoDT";
                itemSoDT.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__SoDT_ToolTip);
                menu.Items.Add(itemSoDT);

                DXMenuItem itemMaBA = new DXMenuItem(ResourceMessage.typeCodeFind__MaBA, new EventHandler(btnCodeFind_Click));
                itemMaBA.Tag = "MaBA";
                itemMaBA.SuperTip = GetTooltip(ResourceMessage.typeCodeFind__MaBA_ToolTip);
                menu.Items.Add(itemMaBA);

                btnCodeFind.DropDownControl = menu;
                btnCodeFind.MenuManager = barManager1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCodeFind_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                btnCodeFind.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind = btnMenuCodeFind.Caption;
                if (this.dlgEnableFindType != null)
                {
                    this.dlgEnableFindType(this.typeCodeFind == ResourceMessage.typeCodeFind__SoThe || this.typeCodeFind == ResourceMessage.typeCodeFind__MaTV);
                }
                if (this.dlgShowControlHrmKskCode != null)
                {
                    this.dlgShowControlHrmKskCode(this.typeCodeFind == ResourceMessage.typeCodeFind__MaNV);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        async Task<object> ProcessSearchByCode(string searchCode, int searchType)
        {
            try
            {
                //Lay gia tri ma nhap vao
                //Kiem tra du lieu ma la qrcode hay ma benh nhan 
                //Neu la qrcode se doc chuoi ma hoa tra ve doi tuong heindata
                //Neu la ma benh nhan thi goi api kiem tra co du lieu benh nhan tuong ung voi ma hay khong, co thi tra ve du lieu BN
                var data = SearchByCode(searchCode);
                if (data != null)
                {
                    CccdCardData cccdCard = new CccdCardData();
                    HeinCardData heinCardDataForCheckGOV = new HeinCardData();
                    string heinAddressOfPatient = "";
                    DateTime dtIntructionTime = DateTime.Now;
                    if (!this.TD3)
                        dtIntructionTime = this.dlgGetIntructionTime();

                    if (data is HisPatientSDO)
                    {
                        var patient = data as HisPatientSDO;
                        heinAddressOfPatient = patient.HeinAddress;
                        FillDataPatientToControl(patient, true);

                        //trả luôn về tiếp đón để tránh trường hợp chưa check thông tuyến xong đã lưu thì chưa có thông tin bệnh nhân tại tiếp đón dẫn đến lỗi là số thẻ đã gắn với bệnh nhân
                        if (!this.isAlertTreatmentEndInDay && this.dlgSearchPatient1 != null)
                        {
                            DataResultADO dataResult = new DataResultADO();
                            dataResult.HisPatientSDO = patient;
                            dataResult.OldPatient = true;
                            dataResult.SearchTypePatient = 1;
                            this.dlgSearchPatient1(dataResult);
                        }
                             
                        if (!this.isAlertTreatmentEndInDay)//Them If de khi co thong bao da ra vien k muon update info
                        {
                            heinCardDataForCheckGOV = ConvertFromPatientData(patient);

                            if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
                                this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);
                        }

                        if (this.TD3)
                        {
                            this.isNotPatientDayDob = (patient.IS_HAS_NOT_DAY_DOB == 1);
                        }
                    }
                    else
                    {
                        if (data is HeinCardData)
                        {
                            this.isReadQrCode = true;
                            heinCardDataForCheckGOV = (HeinCardData)data;
                        }
                        else if (data is CccdCardData)
                        {
                            this.isReadQrCode = true;
                            cccdCard = (CccdCardData)data;
                            heinCardDataForCheckGOV.HeinCardNumber = cccdCard.CardData;
                            heinCardDataForCheckGOV.PatientName = cccdCard.PatientName;
                            heinCardDataForCheckGOV.Dob = cccdCard.Dob;
                            heinCardDataForCheckGOV.Gender = cccdCard.Gender == "NAM" ? "1" : "2";
                            heinCardDataForCheckGOV.Address = cccdCard.Address;
                        }
                        string patientName = Inventec.Common.String.Convert.HexToUTF8Fix(heinCardDataForCheckGOV.PatientName);
                        if (!string.IsNullOrEmpty(patientName))
                            heinCardDataForCheckGOV.PatientName = patientName;
                        string address = Inventec.Common.String.Convert.HexToUTF8Fix(heinCardDataForCheckGOV.Address);
                        if (!string.IsNullOrEmpty(address))
                            heinCardDataForCheckGOV.Address = address;
                        this._UCPatientRawADO = new HIS.UC.UCPatientRaw.ADO.UCPatientRawADO();
                        this._UCPatientRawADO.PATIENT_NAME = heinCardDataForCheckGOV.PatientName;
                        this._UCPatientRawADO.DOB_STR = heinCardDataForCheckGOV.Dob;
                        this._UCPatientRawADO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(heinCardDataForCheckGOV.Gender));//FIX
                        if (data is HeinCardData)
                        {
                            FillDataAfterFindQrCodeNoExistsCard(heinCardDataForCheckGOV);
                            if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
                                this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);
                        }
                    }
                    WaitingManager.Show();
                    long patientTypeId = this.cboPatientType.EditValue == null ? 0 : Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPatientType.EditValue.ToString());
                    if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT && !(data is CccdCardData))
                    {
                        HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                        if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
                            heinGOVManager.SetDelegateHeinEnableButtonSave(dlgHeinEnableSave);
                        this.ResultDataADO = await heinGOVManager.Check(heinCardDataForCheckGOV, null, false, heinAddressOfPatient, dtIntructionTime, isReadQrCode);
                    }
                    else if (!this.TD3 && patientTypeId == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT && data is CccdCardData)
                    {
                        HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                        this.ResultDataADO = await heinGOVManager.CheckCccdQrCode(heinCardDataForCheckGOV, null, dtIntructionTime);

                    }
                    dataHeinCardFromQrCccd = null;
                    if (this.ResultDataADO != null && this.ResultDataADO.ResultHistoryLDO != null)
                    {
                        heinCardDataForCheckGOV.LiveAreaCode = this.ResultDataADO.ResultHistoryLDO.maKV;
                        if (!string.IsNullOrEmpty(this.ResultDataADO.ResultHistoryLDO.gioiTinh))
                            heinCardDataForCheckGOV.Gender = this.ResultDataADO.ResultHistoryLDO.gioiTinh.ToUpper() == "NAM" ? "1" : "2";
                        heinCardDataForCheckGOV.HeinCardNumber = this.ResultDataADO.IsUsedNewCard ? this.ResultDataADO.ResultHistoryLDO.maTheMoi : this.ResultDataADO.ResultHistoryLDO.maThe;
                        if (this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw != null)
                            this.dlgFillDataPreviewForSearchByQrcodeInUCPatientRaw(heinCardDataForCheckGOV);
                        //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                        if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber))
                        {
                            if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                            {
                                heinCardDataForCheckGOV = this.ResultDataADO.HeinCardData;
                            }

                            dataResult.HeinCardData = heinCardDataForCheckGOV;
                        }
                        dataHeinCardFromQrCccd = this.ResultDataADO.HeinCardData;
                        WaitingManager.Hide();
                    }
                    if (data is CccdCardData)
                        FillDataAfterFindQrCodeNoExistsCard(heinCardDataForCheckGOV);
                    //1 thẻ chỉ gắn với 1 bệnh nhân. Trường hợp tìm bằng mã bệnh nhân sẽ luôn trả về số thẻ của bệnh nhân đó nên không tìm lại bệnh nhân theo số thẻ nữa.
                    if (!String.IsNullOrEmpty(heinCardDataForCheckGOV.HeinCardNumber) && (data is HeinCardData || data is CccdCardData))
                    {
                        data = this.CheckPatientOldByHeinCard(heinCardDataForCheckGOV, !(data is HisPatientSDO));
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    int n;
                    bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
                    string codeFind = "";
                    if (isNumeric)
                    {
                        codeFind = string.Format("{0:0000000000}", Convert.ToInt64(this.txtPatientCode.Text));
                    }
                    else
                    {
                        codeFind = this.txtPatientCode.Text;
                    }
                    this.txtPatientCode.Text = codeFind;
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaBenhNhanKhongTontai + " '" + codeFind + "'", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    this.txtPatientCode.Focus();
                    this.txtPatientCode.SelectAll();
                }
                if (this.isAlertTreatmentEndInDay)
                {
                    this.RefreshUserControl();
                }
                return (this.isAlertTreatmentEndInDay ? null : data);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        private void ProcessPatientCodeKeydown(object data)
        {
            try
            {
                if (data != null)
                {
                    this.qrCodeBHYTHeinCardData = null;
                    if (data is HisPatientSDO)
                    {
                        var patient = data as HisPatientSDO;
                        FillDataPatientToControl(patient, true);
                    }
                    else if (data is HeinCardData)
                    {
                        var _HeinCardData = (HeinCardData)data;
                        this._UCPatientRawADO = new HIS.UC.UCPatientRaw.ADO.UCPatientRawADO();
                        this._UCPatientRawADO.PATIENT_NAME = _HeinCardData.PatientName;
                        this._UCPatientRawADO.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(_HeinCardData.Gender));
                        this._UCPatientRawADO.DOB_STR = _HeinCardData.Dob;
                        this.ProcessQrCodeData(_HeinCardData);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                    int n;
                    bool isNumeric = int.TryParse(this.txtPatientCode.Text, out n);
                    string codeFind = "";
                    if (isNumeric)
                    {
                        codeFind = string.Format("{0:0000000000}", Convert.ToInt64(this.txtPatientCode.Text));
                    }
                    else
                    {
                        codeFind = this.txtPatientCode.Text;
                    }
                    this.txtPatientCode.Text = codeFind;
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.MaBenhNhanKhongTontai + " '" + codeFind + "'", Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    this.txtPatientCode.Focus();
                    this.txtPatientCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HeinCardData ConvertFromPatientData(HisPatientSDO patient)
        {
            HeinCardData hein = new HeinCardData();
            try
            {
                if (patient.HasBirthCertificate != MOS.LibraryHein.Bhyt.HeinHasBirthCertificate.HeinHasBirthCertificateCode.TRUE)
                {
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
                    var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == patient.GENDER_ID);
                    hein.Gender = (gender != null ? (gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE ? "1" : "2") : "2");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return hein;
        }

        public bool AlertTreatmentInOutInDayForTreatmentMessage(HisPatientSDO patient)
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
                if (HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.IsCheckTodayFinishTreatment && patient.ID > 0 && patient.TodayFinishTreatments != null && patient.TodayFinishTreatments.Count > 0)
                {
                    string treatmentCodeInDay = String.Join(",", patient.TodayFinishTreatments);
                    if (!String.IsNullOrEmpty(treatmentCodeInDay))
                    {
                        LogSystem.Debug("Tiep don: tim thay benh nhan cu co dot dieu tri gan nhat ra vien trong ngay: " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patient), patient));
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

        public object CheckPatientOldByHeinCard(HeinCardData dataHein, bool isCheckPeriosTreatment)
        {
            object data = null;
            try
            {
                if (dataHein == null) throw new ArgumentNullException("CheckPatientOldByHeinCard => dataHein is null");
                if (!String.IsNullOrEmpty(dataHein.HeinCardNumber))
                {
                    if (dataHein.HeinCardNumber.Length > 15)
                        dataHein.HeinCardNumber = dataHein.HeinCardNumber.Substring(0, 15);
                    else if (dataHein.HeinCardNumber.Length < 15 && dataHein.HeinCardNumber.Length != 12)
                        LogSystem.Info("Do dai so the bhyt cua benh nhan khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataHein.HeinCardNumber), dataHein.HeinCardNumber));
                    else
                        LogSystem.Info("Kiem tra Patient theo CCCD: " + dataHein.HeinCardNumber);
                    //Kiểm tra đã tồn tại dữ liệu bệnh nhân theo số thẻ bhyt hay không
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC && (oldValue.Trim().Contains("|") || (oldValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))))
                    {
                        if (dataHein.HeinCardNumber.Length != 12 && dataHein.HeinCardNumber.Length != 9)
                            filter.HEIN_CARD_NUMBER_OR_CCCD_NUMBER = new HeinCardNumberOrCccdNumber() { HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber, CCCD_NUMBER__EXACT = oldValue.Trim().Contains("|") ? oldValue.Split('|')[0] : oldValue.Trim() };
                        else if (dataHein.HeinCardNumber.Length == 9)
                        {
                            filter.CMND_NUMBER__EXACT = dataHein.HeinCardNumber;
                        }
                        else
                        {
                            filter.CCCD_NUMBER__EXACT = dataHein.HeinCardNumber;
                        }
                    }
                    else
                    {
                        filter.HEIN_CARD_NUMBER__EXACT = dataHein.HeinCardNumber;
                    }
                    var patients = (new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param));
                    if (patients != null && patients.Count > 0)
                    {
                        if (this.typeCodeFind == ResourceMessage.typeCodeFind__MaCMCC && (oldValue.Trim().Contains("|") || (oldValue.Trim().Length == 12 && !string.IsNullOrEmpty(txtPatientName.Text) && (!string.IsNullOrEmpty(txtPatientDob.Text) || dtPatientDob.EditValue != null))))
                        {
                            HisPatientSDO patient = patients[0];
                            LogSystem.Debug("Quet the CCCD tim thay thong tin bhyt cua benh nhan cu theo so the CCCD = " + dataHein.HeinCardNumber + ". " + oldValue + LogUtil.TraceData(" HisPatientSDO searched", patient));
                            FillDataPatientToControl(patient, isCheckPeriosTreatment);
                            data = patient;
                            this.dlgFocusNextUserControl();
                        }
                        else
                        {
                            if (patients.Count > 1)
                            {
                                LogSystem.Debug("Quet the BHYT tim thay " + patients.Count + " benh nhan cu => mo form chon benh nhan => chon 1 => fill du lieu bn duoc chon.");
                                frmPatientChoice frm = new frmPatientChoice(patients, this.SelectOnePatientProcess, txtPatientDob.Text);
                                frm.ShowDialog();
                            }
                            else
                            {
                                LogSystem.Debug("Quet the BHYT tim thay thong tin bhyt cua benh nhan cu theo so the HeinCardNumber = " + dataHein.HeinCardNumber + ". " + LogUtil.TraceData("HisPatientSDO searched", patients[0]));
                                FillDataPatientToControl(patients[0], isCheckPeriosTreatment);
                                data = patients[0];
                                this.dlgFocusNextUserControl();
                            }
                        }
                    }
                    else
                    {
                        LogSystem.Debug("Quet the BHYT khong tim thay Bn cu => fill du lieu theo du lieu gih tren the bhyt");
                        FillDataAfterFindQrCodeNoExistsCard(dataHein);
                        txtPatientCode.Text = "";
                        txtPatientCode.Update();
                        data = dataHein;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return data;
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
                        LogSystem.Info("Do dai so the bhyt cua benh nhan khong hop le. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentHeincardNumber), currentHeincardNumber));
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
                        frmPatientChoice frm = new frmPatientChoice(patients, this.SelectOnePatientProcess, txtPatientDob.Text);
                        frm.ShowDialog();
                    }
                    else
                    {
                        LogSystem.Debug("Quet the BHYT tim thay thong tin bhyt cua benh nhan cu theo so the HeinCardNumber = " + dataHein.HeinCardNumber + ". " + LogUtil.TraceData("HisPatientSDO searched", patients[0]));

                        FillDataPatientToControl(patients[0], true);
                        if (!this.isAlertTreatmentEndInDay)
                            this.dlgFocusNextUserControl();
                    }
                }
                else
                {
                    LogSystem.Debug("Quet the BHYT khong tim thay Bn cu => fill du lieu theo du lieu gih tren the bhyt");
                    FillDataAfterFindQrCodeNoExistsCard(dataHein);
                    txtPatientCode.Text = "";
                    txtPatientCode.Update();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void FillDataByQrCode(HisPatientSDO dataFill)
        //{
        //    try
        //    {
        //        if (this.btnCodeFind.Text == ResourceMessage.typeCodeFind__MaBN)
        //        {
        //            this.txtPatientCode.Text = dataFill.PATIENT_CODE;
        //        }
        //        this.txtPatientName.Text = dataFill.VIR_PATIENT_NAME;
        //        MOS.EFMODEL.DataModels.HIS_GENDER gioitinh = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataFill.GENDER_ID);
        //        if (gioitinh != null)
        //        {
        //            this.cboGender.EditValue = gioitinh.ID;
        //        }
        //        if (dataFill.DOB > 0 && dataFill.DOB.ToString().Length >= 6)
        //        {
        //            if (dataFill.IS_HAS_NOT_DAY_DOB == 1)
        //                this.LoadNgayThangNamSinhBNToForm(dataFill.DOB, true);
        //            else
        //                this.LoadNgayThangNamSinhBNToForm(dataFill.DOB, false);
        //        }
        //        var career = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_CAREER>().SingleOrDefault(o => o.ID == dataFill.CAREER_ID);
        //        if (career != null)
        //        {
        //            this.cboCareer.EditValue = dataFill.CAREER_ID;
        //            this.txtCareerCode.Text = career.CAREER_CODE;
        //        }

        //        if (!HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.UsingPatientTypeOfPreviousPatient)
        //        {
        //            var patientType = HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.PatientTypeDefault;
        //            if (patientType != null && patientType.ID > 0)
        //            {
        //                this.txtPatientTypeCode.Text = patientType.PATIENT_TYPE_CODE;
        //                this.cboPatientType.EditValue = patientType.ID;
        //            }
        //        }
        //        this.currentPatientSDO = dataFill;
        //        this.PeriosTreatmentMessage();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void FillDataAfterFindQrCodeNoExistsCard(HeinCardData dataHein)
        {
            try
            {
                string _patientName = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                if (!string.IsNullOrEmpty(_patientName))
                {
                    this.txtPatientName.Text = _patientName;
                }
                else
                    this.txtPatientName.Text = dataHein.PatientName;

                if (!String.IsNullOrEmpty(dataHein.Dob))
                {
                    this.isNotPatientDayDob = false;

                    if (dataHein.Dob.Length == 4)
                    {
                        this.isNotPatientDayDob = true;
                        this.dtPatientDob.EditValue = new DateTime(Inventec.Common.TypeConvert.Parse.ToInt32(dataHein.Dob), 1, 1);
                        this.txtPatientDob.Text = dataHein.Dob;
                    }
                    else if (dataHein.Dob.Length == 7)
                    {
                        int month = Int16.Parse(dataHein.Dob.Substring(0, 2));
                        int year = Int16.Parse(dataHein.Dob.Substring(3, 4));
                        this.dtPatientDob.EditValue = new DateTime(year, month, 1);
                        this.txtPatientDob.Text = dataHein.Dob;
                    }
                    else
                    {
                        this.dtPatientDob.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(dataHein.Dob);
                        this.txtPatientDob.Text = dtPatientDob.DateTime.ToString("dd/MM/yyyy");
                    }
                    this.dtPatientDob.Update();
                }
                if (!String.IsNullOrEmpty(dataHein.Gender))
                {
                    var dataGenderId = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HeinToHisNumber(dataHein.Gender);
                    var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == dataGenderId);
                    if (gender != null)
                    {
                        this.cboGender.EditValue = gender.ID;
                    }
                }
                this.ClearControlCombo();
                this.CalulatePatientAge(txtPatientDob.Text);
                this.SetValueCareerComboByCondition();
                this.txtPatientCode.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo supertip chứa nội dung để barmanager lấy ra hiển thị
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private SuperToolTip GetTooltip(string text)
        {
            SuperToolTip superTip = new SuperToolTip();
            ToolTipItem item1 = new ToolTipItem();
            item1.Text = text;
            superTip.Items.Add(item1);
            superTip.AllowHtmlText = DefaultBoolean.True;
            return superTip;
        }

        private void ReloadDataCboPatientClassifyByPatientType(HIS_PATIENT_TYPE pt)
        {
            try
            {
                if (pt != null)
                {
                    dataClassify = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_CLASSIFY>();

                    dataClassify = dataClassify.Where(o => o.IS_ACTIVE == 1 && (!o.PATIENT_TYPE_ID.HasValue || o.PATIENT_TYPE_ID == pt.ID)).ToList();

                    //this.InitComboCommon(this.cboPatientClassify, dataClassify, "ID", "PATIENT_CLASSIFY_NAME", "PATIENT_CLASSIFY_CODE");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
