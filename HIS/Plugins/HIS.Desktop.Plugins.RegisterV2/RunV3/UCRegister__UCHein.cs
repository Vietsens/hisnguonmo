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
using Inventec.Core;
using MOS.Filter;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.QrCodeBHYT;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.UC.UCPatientRaw.ADO;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCHeniInfo.Data;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.DelegateRegister;
using HIS.UC.UCOtherServiceReqInfo.ADO;
using MOS.EFMODEL.DataModels;
using MOS.LibraryHein.Bhyt;
using SDA.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataAfterSaerchPatientInUCPatientRaw(DataResultADO dt)
        {
            try
            {
                var heinCardData = dt.HeinCardData;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => heinCardData), heinCardData));
                Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.1");
                var ucPatientRawData = ucPatientRaw1.GetValue();
                if (heinCardData != null
                    && !String.IsNullOrEmpty(heinCardData.HeinCardNumber)
                    && ucPatientRawData != null
                    && (
                        ucPatientRawData.PATIENTTYPE_ID == 0
                        || (ucPatientRawData.PATIENTTYPE_ID != HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT
                            && ucPatientRawData.PATIENTTYPE_ID != HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN))
                    && !ucPatientRawData.IsReadQrCccd)
                {
                    Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.2");
                    Inventec.Common.Logging.LogSystem.Debug("Trường hợp đối tượng BN đang chọn không phải là đối tượng BHYT, người dùng nhập qrcode để tìm kiếm ==>tự động gán đối tượng thanh toán mặc định là đối tượng BHYT," + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ucPatientRawData.PATIENTTYPE_ID), ucPatientRawData.PATIENTTYPE_ID));
                    ucPatientRaw1.SetValuePatientType(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT);
                    ucPatientRawData = ucPatientRaw1.GetValue();
                }
                ucPatientRaw1.InitEthnic();
                if (ucPatientRawData != null && ucPatientRawData.PATIENTTYPE_ID > 0 && (ucPatientRawData.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT || ucPatientRawData.PATIENTTYPE_ID == HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN))
                {
                    Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.3");
                    this.ucHeinInfo1.FillDataByHeinCardData(heinCardData);
                    Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.4");
                }
                this.ucAddressCombo1.SetValue(null);
                //Kiem tra cau hinh co tu dong fill du lieu dia chi ghi tren the vao o dia chi benh nhan, co thi fill du lieu, khong thi bo qua
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                {
                    Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.5");
                    dataAddressPatient = this.ucAddressCombo1.GetValue() ?? new HIS.UC.AddressCombo.ADO.UCAddressADO();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataAddressPatient), dataAddressPatient));
                    Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                    var data = adProc.SplitFromFullAddress(heinCardData.Address);
                    if (data != null)
                    {
                        dataAddressPatient.Province_Code = data.ProvinceCode;
                        dataAddressPatient.Province_Name = data.ProvinceName;
                        dataAddressPatient.District_Code = data.DistrictCode;
                        dataAddressPatient.District_Name = data.DistrictName;
                        dataAddressPatient.Commune_Code = data.CommuneCode;
                        dataAddressPatient.Commune_Name = data.CommuneName;
                    }
                    dataAddressPatient.Address = heinCardData.Address;
                    this.ucAddressCombo1.SetValue(dataAddressPatient);
                    Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.6");
                }
                if (this.ucOtherServiceReqInfo1 != null)
                    this.ucOtherServiceReqInfo1.RefreshUserControl();
                Inventec.Common.Logging.LogSystem.Debug("FillDataAfterSaerchPatientInUCPatientRaw.7");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultFocusUserControl()
        {
            try
            {
                if (this.ucPatientRaw1.cboPatientType.EditValue != null)
                {
                    this.ucHeinInfo1.FocusUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusNextControlUCHein()
        {
            try
            {
                if (this.ucServiceRoomInfo1 != null)
                {
                    this.ucServiceRoomInfo1.FocusUserControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitInputDataUCHeinInfo()
        {
            try
            {
                DataInitUCHeniInfo dataInitUCHeniInfo = new DataInitUCHeniInfo();
                dataInitUCHeniInfo.dlgGetIsChild = GetIsChild;
                dataInitUCHeniInfo.PATIENT_TYPE_ID__BHYT = HisConfigCFG.PatientTypeId__BHYT;
                dataInitUCHeniInfo.isVisibleControl = AppConfigs.TiepDon_HienThiMotSoThongTinThemBenhNhan;
                dataInitUCHeniInfo.IsShowCheckKhongKTHSD = HisConfigCFG.IsShowCheckExpired;
                dataInitUCHeniInfo.IsDefaultRightRouteType = (HisConfigCFG.IsDefaultRightRouteType == GlobalVariables.CommonStringTrue ? true : false);
                dataInitUCHeniInfo.AlertExpriedTimeHeinCardBhyt = AppConfigs.AlertExpriedTimeHeinCardBhyt;
                dataInitUCHeniInfo.dlgFillDataPatientSDOToRegisterForm = this.FillDataIntoFormBySearchCardInUcHein;
                dataInitUCHeniInfo.dlgFocusNextUserControl = this.FocusNextControlUCHein;
                dataInitUCHeniInfo.dlgAutoCheckCC = this.AutoSetCheckCC;
                dataInitUCHeniInfo.dlgCheckExamHistory = this.CheckHeinCardByServerBhxh;
                dataInitUCHeniInfo.dlgProcessFillDataCareerUnder6AgeByHeinCardNumber = null;// this.FillDataCareerUnder6AgeByHeinCardNumberUCHeinInfo;
                dataInitUCHeniInfo.UpdateTranPatiDataByPatientOld = UpdateTranPatiDataByPatientOld;
                dataInitUCHeniInfo.dlgCheckSS = UpdateCheckSS;
                this.ucHeinInfo1.InitInputData(dataInitUCHeniInfo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void UpdateCheckSS(bool isCheck)
        {
            try
            {
                this.isCheckSS = isCheck;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoUCHeinInfo(HisPatientSDO patient)
        {
            try
            {
                if (IsPatientTypeUsingHeinInfo())
                    ucHeinInfo1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataIntoUCHeinInfoByPatientTypeAlter(HisPatientSDO patientSDO)
        {
            try
            {
                if (IsPatientTypeUsingHeinInfo())
                {
                    HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                    patientTypeAlter.HEIN_CARD_FROM_TIME = patientSDO.HeinCardFromTime;
                    patientTypeAlter.HEIN_CARD_NUMBER = patientSDO.HeinCardNumber;
                    patientTypeAlter.HEIN_CARD_TO_TIME = patientSDO.HeinCardToTime;
                    patientTypeAlter.HEIN_MEDI_ORG_CODE = patientSDO.HeinMediOrgCode;
                    patientTypeAlter.HEIN_MEDI_ORG_NAME = patientSDO.HeinMediOrgName;
                    patientTypeAlter.JOIN_5_YEAR = patientSDO.Join5Year;
                    patientTypeAlter.PAID_6_MONTH = patientSDO.Paid6Month;
                    patientTypeAlter.RIGHT_ROUTE_CODE = patientSDO.RightRouteCode;
                    patientTypeAlter.RIGHT_ROUTE_TYPE_CODE = patientSDO.RightRouteTypeCode;
                    patientTypeAlter.LIVE_AREA_CODE = patientSDO.LiveAreaCode;
                    if (ucHeinInfo1 != null)
                        this.ucHeinInfo1.SetValueByPatientTypeAlter(patientTypeAlter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCRegistor/FillDataIntoUCHeinInfoByPatientTypeAlter:\n" + ex);
            }
        }

        private bool FillDataIntoFormBySearchCardInUcHein(HisPatientSDO patient)
        {
            bool valid = true;
            try
            {
                if (!this.ucPatientRaw1.AlertTreatmentInOutInDayForTreatmentMessage(patient))
                {
                    this.currentPatientSDO = null;
                    this.isReadQrCode = true;
                    this.RefreshUserControl();
                    return false;
                }

                this._HeinCardData = this.ConvertFromPatientData(patient);

                this.actionType = GlobalVariables.ActionAdd;
                this.SetPatientSearchPanel(true);
                this.currentPatientSDO = patient;

                this.FillDataToExamServiceReqNewestByPatient(this.currentPatientSDO);
                this.FillDataPatientRawInfo(this.currentPatientSDO);
                this.FillDataIntoUCPlusInfo(this.currentPatientSDO);
                this.FillDataIntoUCRelativeInfo(this.currentPatientSDO);
                this.FillDataIntoUCAddressInfo(this.currentPatientSDO);
                this.PeriosTreatmentMessage();

                if (AppConfigs.DangKyTiepDonHienThiThongBaoTimDuocBenhNhan == GlobalVariables.CommonNumberTrue)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceMessage.TimDuocMotBenhNhanTheoThongTinNguoiDungNhapNeuKhongPhaiBNCuVuiLongNhanNutBNMoi, ResourceMessage.TieuDeCuaSoThongBaoLaThongBao, DevExpress.Utils.DefaultBoolean.True);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void AutoSetCheckCC(bool value)
        {
            try
            {
                UCServiceReqInfoADO serviceReqInfoADO = ucOtherServiceReqInfo1.GetValue();
                serviceReqInfoADO.IsEmergency = value;
                ucOtherServiceReqInfo1.SetValue(serviceReqInfoADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<object> SearchByCode(string code)
        {
            try
            {
                //LogSystem.Debug("SearchByCode => 1");
                var data = new HisPatientSDO();
                if (String.IsNullOrEmpty(code)) throw new ArgumentNullException("code is null");
                if (code.Length > 10 && code.Contains("|"))
                {
                    this.isReadQrCode = true;
                    return await GetDataQrCodeHeinCard(code);
                }
                else
                {
                    //ex khi mã sai==> nhạp la mã bhyt
                    CommonParam param = new CommonParam();
                    HisPatientAdvanceFilter filter = new HisPatientAdvanceFilter();
                    filter.PATIENT_CODE__EXACT = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    data = new BackendAdapter(param).Get<List<HisPatientSDO>>(RequestUriStore.HIS_PATIENT_GETSDOADVANCE, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param).SingleOrDefault();
                }
                //LogSystem.Debug("SearchByCode => 2");
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return null;
        }

        private async Task<HeinCardData> GetDataQrCodeHeinCard(string qrCode)
        {
            HeinCardData dataHein = null;
            try
            {
                //Lay thong tin tren th BHYT cua benh nhan khi quet the doc chuoi qrcode
                ReadQrCodeHeinCard readQrCode = new ReadQrCodeHeinCard();
                dataHein = readQrCode.ReadDataQrCode(qrCode);

                BhytHeinProcessor _BhytHeinProcessor = new BhytHeinProcessor();
                if (!_BhytHeinProcessor.IsValidHeinCardNumber(dataHein.HeinCardNumber))
                {
                    //DevExpress.XtraEditors.XtraMessageBox.Show("Mã QR không hợp lệ. Vui lòng kiểm tra lại.", "Thông báo");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return dataHein;
        }

        private void FillDataAfterFindQrCodeNoExistsCard(HeinCardData dataHein)
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                string paName = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.PatientName);
                patient.PATIENT_NAME = String.IsNullOrEmpty(paName) ? dataHein.PatientName : paName;

                if (!String.IsNullOrEmpty(dataHein.Dob))
                {
                    if (dataHein.Dob.Length == 4)
                    {
                        this.isNotPatientDayDob = true;
                        patient.DOB = long.Parse(dataHein.Dob);
                        patient.DOB_STR = dataHein.Dob;
                    }
                    else
                    {
                        patient.DOB = long.Parse(dataHein.Dob);
                        patient.DOB_STR = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                    }
                }
                if (!String.IsNullOrEmpty(dataHein.Gender))
                {

                    var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                    var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().FirstOrDefault(o => o.ID == dataGenderId);
                    if (gender != null)
                    {
                        //this.ucPatientRaw1.txtGenderName.Text = gender.GENDER_CODE;
                        patient.GENDER_ID = gender.ID;
                    }
                }
                this.CalulatePatientAge(patient.DOB, false);
                this.SetValueCareerComboByCondition();

                //Kiem tra cau hinh co tu dong fill du lieu dia chi ghi tren the vao o dia chi benh nhan, co thi fill du lieu, khong thi bo qua
                string paAddress = Inventec.Common.String.Convert.HexToUTF8Fix(dataHein.Address);
                //if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                //    this.ucPatientRaw1.txtAddress.Text = String.IsNullOrEmpty(paAddress) ? dataHein.Address : paAddress;
                //else
                //    this.ucPatientRaw1.txtAddress.Text = "";

                //Cap nhat thong tin doc tu the vao vung thong tin bhyt
                if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    this.mainHeinProcessor.FillDataAfterFindQrCode(this.ucHeinBHYT, dataHein);
                Inventec.Common.Logging.LogSystem.Error("FillDataAfterFindQrCodeNoExistsCard");
                patient.PATIENT_CODE = "";
                this.ucPatientRaw1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalulatePatientAge(long strDob, bool isHasReset)
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                //LogSystem.Debug("p1. Bat dau ham tinh tuoi benh nhan.");

                patient.DOB = strDob;
                if (patient.DOB != null && Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB) != DateTime.MinValue)
                {
                    bool isGKS = true;
                    DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB).Value;
                    TimeSpan diff = DateTime.Now - dtNgSinh;
                    long tongsogiay = diff.Ticks;
                    if (tongsogiay < 0)
                    {
                        patient.DOB_STR = "";
                        patient.DOB = 4000000;
                        return;
                    }
                    DateTime newDate = new DateTime(tongsogiay);

                    int nam = newDate.Year - 1;
                    int thang = newDate.Month - 1;
                    int ngay = newDate.Day - 1;
                    int gio = newDate.Hour;
                    int phut = newDate.Minute;
                    int giay = newDate.Second;

                    //LogSystem.Debug("p2. Goi thu vien kiem tra nam sinh co phai la tre em hay khong (MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild), Namsinh = " + dtNgSinh.ToString("dd/MM/yyyy HH:mm:ss") + ", ket qua tra ve = " + isGKS);
                    isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);

                    if (nam >= 7)
                    {
                        patient.DOB = 1000000;
                        this.ucPatientRaw1.txtAge.Enabled = false;
                        this.ucPatientRaw1.cboAge.Enabled = false;
                        if (!isGKS)
                        {
                            this.ucPatientRaw1.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                        }
                        else
                        {
                            this.ucPatientRaw1.txtAge.EditValue = nam.ToString();
                        }
                    }
                    else if (nam > 0 && nam < 7)
                    {
                        if (nam == 6)
                        {
                            if (thang > 0 || ngay > 0)
                            {
                                this.ucPatientRaw1.cboAge.EditValue = 1;
                                this.ucPatientRaw1.txtAge.Enabled = false;
                                this.ucPatientRaw1.cboAge.Enabled = false;
                                if (!isGKS)
                                {
                                    this.ucPatientRaw1.txtAge.EditValue = DateTime.Now.Year - dtNgSinh.Year;
                                }
                                else
                                {
                                    this.ucPatientRaw1.txtAge.EditValue = nam.ToString();
                                }
                            }
                            else
                            {
                                this.ucPatientRaw1.txtAge.EditValue = nam * 12 - 1;
                                this.ucPatientRaw1.cboAge.EditValue = 2;
                                this.ucPatientRaw1.txtAge.Enabled = false;
                                this.ucPatientRaw1.cboAge.Enabled = false;
                            }

                        }
                        else
                        {
                            this.ucPatientRaw1.txtAge.EditValue = nam * 12 + thang;
                            this.ucPatientRaw1.cboAge.EditValue = 2;
                            this.ucPatientRaw1.txtAge.Enabled = false;
                            this.ucPatientRaw1.cboAge.Enabled = false;
                        }

                    }
                    else
                    {
                        if (thang > 0)
                        {
                            this.ucPatientRaw1.txtAge.EditValue = thang.ToString();
                            this.ucPatientRaw1.cboAge.EditValue = 2;
                            this.ucPatientRaw1.txtAge.Enabled = false;
                            this.ucPatientRaw1.cboAge.Enabled = false;
                        }
                        else
                        {
                            if (ngay > 0)
                            {
                                this.ucPatientRaw1.txtAge.EditValue = ngay.ToString();
                                this.ucPatientRaw1.cboAge.EditValue = 3;
                                this.ucPatientRaw1.txtAge.Enabled = false;
                                this.ucPatientRaw1.cboAge.Enabled = false;
                            }
                            else
                            {
                                this.ucPatientRaw1.txtAge.EditValue = "";
                                this.ucPatientRaw1.cboAge.EditValue = 4;
                                this.ucPatientRaw1.txtAge.Enabled = true;
                                this.ucPatientRaw1.cboAge.Enabled = false;
                            }
                        }
                    }
                    var chkHasDob = this.mainHeinProcessor.GetchkHasDobCertificate(this.ucHeinBHYT);
                    if (!(chkHasDob != null && chkHasDob.Checked && isGKS)
                        && this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                    {
                        //LogSystem.Debug("p3. Goi ucBhyt cap nhat an hien cac control theo dieu kien la tre em hay khong");
                        this.mainHeinProcessor.UpdateHasDobCertificateEnable(this.ucHeinBHYT, isGKS);
                    }

                    this.SetValidationByChildrenUnder6Years(isGKS, isHasReset);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetValueCareerComboByCondition()
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                MOS.EFMODEL.DataModels.HIS_CAREER career = null;
                bool isGKS = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Convert.ToDateTime(patient.DOB));
                if (isGKS)
                {
                    career = HisConfigCFG.CareerUnder6Age;
                }
                else if (DateTime.Now.Year - Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB).Value.Year <= 18)
                {
                    career = HisConfigCFG.CareerHS;
                }
                else
                {
                    career = HisConfigCFG.CareerBase;
                }
                if (career != null && career.ID > 0)
                {
                    patient.CARRER_ID = career.ID;
                    patient.CARRER_CODE = career.CAREER_CODE;
                }
                Inventec.Common.Logging.LogSystem.Error("SetValueCareerComboByCondition");
                this.ucPatientRaw1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber(HeinCardData heinCard, bool isSearchHeinCardNumber)
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                
                if (heinCard != null && !String.IsNullOrEmpty(heinCard.HeinCardNumber))
                {
                    this.FillDataCareerUnder6AgeByHeinCardNumber(heinCard.HeinCardNumber);
                }
                DateTime? dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB);
                this.AutoCheckPriorityByPriorityType(Inventec.Common.TypeConvert.Parse.ToInt64(dt.Value.ToString("yyyyMMdd") + "000000"), heinCard.HeinCardNumber);
                this.ucPatientRaw1.SetValue(patient);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void ChoiceTemplateHeinCard(string patientTypeCode, bool focusMoveOut)
        {
            try
            {
                var patient = this.ucPatientRaw1.GetValue();
                Inventec.Common.Logging.LogSystem.Debug("t3.1: begin process ChoiceTemplateHeinCard");
                this.ucHeinBHYT = new UserControl();
                this.mainHeinProcessor = new His.UC.UCHein.MainHisHeinBhyt();
                //His.UC.UCHein.Base.LanguageManager.Init(Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                if (patientTypeCode == HisConfigCFG.PatientTypeCode__BHYT || patientTypeCode == HisConfigCFG.PatientTypeCode__QN)
                {
                    Inventec.Common.Logging.LogSystem.Debug("t3.1.1: set default data to control hein");
                    His.UC.UCHein.Data.DataInitHeinBhyt dataHein = new His.UC.UCHein.Data.DataInitHeinBhyt();
                    dataHein.BhytWhiteLists = BackendDataWorker.Get<HIS_BHYT_WHITELIST>();
                    dataHein.BhytBlackLists = BackendDataWorker.Get<HIS_BHYT_BLACKLIST>();
                    dataHein.Genders = BackendDataWorker.Get<HIS_GENDER>();
                    dataHein.Template = His.UC.UCHein.MainHisHeinBhyt.TEMPLATE__BHYT1;
                    if (patient.DOB != null && patient.DOB > 0)
                    {
                        dataHein.IsChild = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(patient.DOB).Value);
                    }
                    dataHein.HEIN_LEVEL_CODE__CURRENT = HIS.Desktop.LocalStorage.HisConfig.HisHeinLevelCFG.HEIN_LEVEL_CODE__CURRENT;
                    dataHein.HeinRightRouteTypes = MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeStore.Get();
                    dataHein.Icds = BackendDataWorker.Get<HIS_ICD>();
                    dataHein.LiveAreas = MOS.LibraryHein.Bhyt.HeinLiveArea.HeinLiveAreaStore.Get();
                    dataHein.MEDI_ORG_CODE__CURRENT = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                    dataHein.MEDI_ORG_CODES__ACCEPTs = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_CODES__ACCEPT;
                    dataHein.MediOrgs = BackendDataWorker.Get<HIS_MEDI_ORG>();
                    dataHein.PATIENT_TYPE_ID__BHYT = HisConfigCFG.PatientTypeId__BHYT;
                    dataHein.PatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                    if (patientTypeCode == HisConfigCFG.PatientTypeCode__QN && !string.IsNullOrEmpty(HisConfigCFG.CheckTempQN))
                    {
                        dataHein.IsTempQN = HisConfigCFG.CheckTempQN.Contains(patientTypeCode);
                    }

                    dataHein.TranPatiForms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_FORM>();
                    dataHein.TranPatiReasons = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TRAN_PATI_REASON>();
                    dataHein.TREATMENT_TYPE_ID__EXAM = IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM;
                    dataHein.TreatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                    dataHein.PatientTypeId = HisConfigCFG.PatientTypeId__BHYT;
                    dataHein.isVisibleControl = AppConfigs.TiepDon_HienThiMotSoThongTinThemBenhNhan;
                    dataHein.IsShowCheckKhongKTHSD = HisConfigCFG.IsShowCheckExpired;
                    dataHein.AutoCheckIcd = HisConfigCFG.AutoCheckIcd;
                    dataHein.IsDefaultRightRouteType = (HisConfigCFG.IsDefaultRightRouteType == IsDefaultRightRouteType__True ? true : false);
                    //dataHein.FillDataPatientSDOToRegisterForm = this.LoadOneBNToControlForUCHein;
                    dataHein.SetFocusMoveOut = this.FocusDelegate;
                    dataHein.SetShortcutKeyDown = this.ShortcutDelegate;
                    dataHein.AutoCheckCC = this.AutoSetCheckCC;
                    //dataHein.CheckExamHistory = this.CheckTTFull;
                    dataHein.ProcessFillDataCareerUnder6AgeByHeinCardNumber = this.UCHeinProcessFillDataCareerUnder6AgeByHeinCardNumber;
                    dataHein.IsDungTuyenCapCuuByTime = this._IsDungTuyenCapCuuByTime;
                    dataHein.IsObligatoryTranferMediOrg = HisConfigCFG.IsObligatoryTranferMediOrg;
                    dataHein.ActChangePatientDob = this.ProcessWhileChangeDOb;
                    Inventec.Common.Logging.LogSystem.Debug("t3.1.2: uCMainHein init");
                    //this.ucHeinBHYT = this.mainHeinProcessor.InitUC(dataHein, ApiConsumers.MosConsumer, null);
                    //    if (this.ucHeinBHYT != null)
                    //    {
                    //        int height = this.ucHeinBHYT.Height;

                    //        //Hien thi vung nhap thong tin the bhyt
                    //        //this.gboxHeinCardInformation.Expanded = true;
                    //        //this.gboxHeinCardInformation.Height = height + 50;

                    //this.ucHeinInfo1.Controls.Add(this.ucHeinBHYT);
                    //    this.ucHeinBHYT.Dock = DockStyle.Fill;
                    //        Inventec.Common.Logging.LogSystem.Debug("t3.1.3: set delegate");
                    //        Inventec.Common.Logging.LogSystem.Debug("t3.1.4: end");
                    //    }
                }
                ////else if (patientTypeCode == HIS.Desktop.LocalStorage.SdaConfigKey.Config.HisPatientTypeCFG.PATIENT_TYPE_CODE__KSK)
                ////{
                ////    His.UC.UCHein.Data.DataInitKskContract dataHein = new His.UC.UCHein.Data.DataInitKskContract();
                ////    dataHein.Template = His.UC.UCHein.MainHisHeinBhyt.TEMPLATE__KSK_CONTRACT1;
                ////    dataHein.SetFocusMoveOut = FocusDelegate;
                ////    dataHein.KskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                ////    ucHein__BHYT = await uCMainHein.InitUC(dataHein, ApiConsumers.MosConsumer, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage()).ConfigureAwait(false);
                ////    if (ucHein__BHYT != null)
                ////    {
                ////        int height = ucHein__BHYT.Height;
                ////        ucHein__BHYT.Dock = DockStyle.Fill;
                ////        pnlUCHeinInformation.Controls.Add(ucHein__BHYT);
                ////        //His.UC.UCHein.Data.DataSetDelegate dataSetDelegate = new His.UC.UCHein.Data.DataSetDelegate();
                ////        //dataSetDelegate.UC = ucHein__BHYT;
                ////        //dataSetDelegate.FocusDelegate = FocusDelegate;
                ////        //uCMainHein.SetDelegateSetFocusMoveOut(dataSetDelegate);

                ////        //Hien thi vung nhap thong tin kham suc khoe
                ////        gboxHeinCardInformation.Expanded = true;
                ////        gboxHeinCardInformation.Height = height + 40;
                ////    }
                ////}
                //else
                //{
                //    this.ucHeinInfo1.Controls.Clear();
                //    this.ucHeinInfo1.Update();
                //    if (focusMoveOut)
                //        this.FocusInServiceRoomInfo();
                //    this.ucHeinBHYT = null;
                //    //this.gboxHeinCardInformation.Expanded = false;
                //}

                //if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                //    this.mainHeinProcessor.ResetValidationControl(this.ucHeinBHYT);


                Inventec.Common.Logging.LogSystem.Debug("t3.2: end process ChoiceTemplateHeinCard");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FocusDelegate()
        {
            try
            {
                this.FocusInServiceRoomInfo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ShortcutDelegate(Keys key)
        {
            try
            {
                switch (key)
                {
                    case Keys.I:
                        this.btnSaveAndPrint.PerformClick();
                        break;
                    case Keys.S:
                        this.btnSave.PerformClick();
                        break;
                    case Keys.N:
                        this.btnNewContinue.PerformClick();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
