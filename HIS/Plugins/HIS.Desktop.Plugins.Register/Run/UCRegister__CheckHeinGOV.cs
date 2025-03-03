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
using His.Bhyt.InsuranceExpertise;
using His.Bhyt.InsuranceExpertise.LDO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.QrCodeBHYT;
using Inventec.Core;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using MOS.SDO;
using SDA.EFMODEL.DataModels;
using DevExpress.XtraEditors;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private void UpdateControlEditorTime(DevExpress.XtraEditors.ButtonEdit txtEditorTime, DevExpress.XtraEditors.DateEdit dtEditorTime)
        {
            try
            {
                string strtxtIntructionTime = "";
                if (txtEditorTime.Text.Length == 2 || txtEditorTime.Text.Length == 1)
                {
                    strtxtIntructionTime = "01/01/" + (DateTime.Now.Year - Inventec.Common.TypeConvert.Parse.ToInt64(txtEditorTime.Text)).ToString();
                }
                else if (txtEditorTime.Text.Length == 4)
                    strtxtIntructionTime = "01/01/" + txtEditorTime.Text;
                else if (txtEditorTime.Text.Length == 8)
                {
                    strtxtIntructionTime = txtEditorTime.Text.Substring(0, 2) + "/" + txtEditorTime.Text.Substring(2, 2) + "/" + txtEditorTime.Text.Substring(4, 4);
                }
                else
                    strtxtIntructionTime = txtEditorTime.Text;

                dtEditorTime.EditValue = DateTimeHelper.ConvertDateStringToSystemDate(strtxtIntructionTime);
                dtEditorTime.Update();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async void CheckTTFull(HeinCardData heinCard)
        {
            try
            {
                if (!HisConfigCFG.IsCheckExamHistory) return;
                if (this.isNotCheckTT) { return; }
                if (heinCard == null) heinCard = new HeinCardData();

                heinCard.Dob = txtPatientDob.Text;
                heinCard.PatientName = txtPatientName.Text.Trim();
                var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboGender.EditValue ?? "0").ToString()));
                heinCard.Gender = (gender != null ? GenderConvert.HisToHein(gender.ID.ToString()) : "2");

                if (String.IsNullOrEmpty(heinCard.HeinCardNumber)
                    // || String.IsNullOrEmpty(heinCard.Address)
                        || String.IsNullOrEmpty(heinCard.FromDate)
                        || String.IsNullOrEmpty(heinCard.MediOrgCode))
                {
                    if (this.ucHeinBHYT != null && mainHeinProcessor != null)
                    {
                        HisPatientProfileSDO patientProfileSDO = new HisPatientProfileSDO();
                        mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, patientProfileSDO);
                        if (patientProfileSDO != null
                            && patientProfileSDO.HisPatientTypeAlter != null
                            && !String.IsNullOrEmpty(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER)
                            )
                        {
                            heinCard.HeinCardNumber = patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_NUMBER;
                            heinCard.Address = patientProfileSDO.HisPatientTypeAlter.ADDRESS;
                            heinCard.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_FROM_TIME.ToString());
                            heinCard.MediOrgCode = patientProfileSDO.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                            heinCard.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.HEIN_CARD_TO_TIME.ToString());
                            if (patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME != null && patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME > 0)
                            {
                                heinCard.FineYearMonthDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientProfileSDO.HisPatientTypeAlter.JOIN_5_YEAR_TIME.ToString());
                            }
                        }
                    }
                }

                HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);

                this.ResultDataADO = await heinGOVManager.Check(heinCard, null, true, (this.currentPatientSDO != null && this.currentPatientSDO.ID > 0 ? this.currentPatientSDO.HeinAddress : ""), this.dtIntructionTime.DateTime, this.isReadQrCode);
                mainHeinProcessor.SetResultDataADOBhyt(ucHeinBHYT, this.ResultDataADO);
                if (this.ResultDataADO != null)//nếu không thay đổi thông tin sẽ chỉ trả ra kết quả check thông tuyến và không thực hiện tìm kiếm
                {
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (!String.IsNullOrEmpty(heinCard.HeinCardNumber))
                    {
                        if (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                        {
                            heinCard.HeinCardNumber = this.ResultDataADO.ResultHistoryLDO.maTheMoi;
                        }
                    }

                    await this.CheckTTProcessResultData(heinCard);
                }
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
                            this.cboGender.EditValue = gender.ID;
                            this.txtGenderCode.Text = gender.GENDER_CODE;
                        }
                        this.txtPatientName.Text = this.ResultDataADO.ResultHistoryLDO.hoTen;
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

                    if (this.ResultDataADO.IsAddress || this.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose || this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                    {
                        if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                        {
                            Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                            var data = adProc.SplitFromFullAddress(this.ResultDataADO.ResultHistoryLDO.diaChi);
                            if ((typeCodeFind == typeCodeFind__CCCDCMND && (currentPatientSDO == null)) || (currentPatientSDO != null && data != null && (currentPatientSDO.PROVINCE_CODE != data.ProvinceCode || currentPatientSDO.DISTRICT_CODE != data.DistrictCode || currentPatientSDO.COMMUNE_CODE != data.CommuneCode)))
                            {
                                cboProvince.EditValue = data.ProvinceCode;
                                txtProvinceCode.EditValue = data.ProvinceCode;
                                cboDistrict.EditValue = data.DistrictCode;
                                txtDistrictCode.EditValue = data.DistrictCode;
                                cboCommune.EditValue = data.CommuneCode;
                                txtCommuneCode.EditValue = data.CommuneCode;
                            }
                            this.txtAddress.Text = this.ResultDataADO.ResultHistoryLDO.diaChi;
                        }
                    }

                    if (this.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        var dataGenderId = GenderConvert.HeinToHisNumber(dataHein.Gender);
                        var gender = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_GENDER>().SingleOrDefault(o => o.ID == dataGenderId);
                        if (gender != null && gender.ID > 0)
                        {
                            this.cboGender.EditValue = gender.ID;
                            this.txtGenderCode.Text = gender.GENDER_CODE;
                        }
                        this.txtPatientName.Text = this.ResultDataADO.ResultHistoryLDO.hoTen;
                        if (this.mainHeinProcessor != null && this.ucHeinBHYT != null)
                        {
                            this.mainHeinProcessor.FillDataAfterCheckBHYT(this.ucHeinBHYT, dataHein);
                        }
                    }
                    CheckRRCodeTTFee(false);
                    if (HisConfigCFG.IsCheckExamHistory && (this.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose || this.ResultDataADO.SuccessWithoutMessage))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Mo form lich su voi data rsIns");
                        frmCheckHeinCardGOV frm = new frmCheckHeinCardGOV(this.ResultDataADO.ResultHistoryLDO);
                        frm.ShowDialog();
                    }

                    Inventec.Common.Logging.LogSystem.Debug("CheckHanSDTheBHYT => 3");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool CheckRRCodeTTFee(bool IsSave)
        {
            bool result = true;
            try
            {
                HisPatientProfileSDO dataPatientProfile = new HisPatientProfileSDO();
                this.mainHeinProcessor.UpdateDataFormIntoPatientTypeAlter(this.ucHeinBHYT, dataPatientProfile);
                if (dataPatientProfile.HisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE && HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.IS_WARNING_WRONG_ROUTE_FEE == 1)
                {
                    result = IsSave ? XtraMessageBox.Show("Bệnh nhân trái tuyến cần thu tiền khám. Bạn có muốn tiếp tục không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes : XtraMessageBox.Show("Bệnh nhân trái tuyến cần thu tiền khám.", "Thông báo", MessageBoxButtons.OK) == DialogResult.OK;
                }               
                        
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        bool CheckChangeInfo(HeinCardData dataHein, ResultHistoryLDO rsIns, bool isHasNewCard)
        {
            bool result = false;
            try
            {
                string gt = (rsIns.gioiTinh == "Nữ") ? "2" : "1";
                bool isUsedNewCard = false;

                if (isHasNewCard)
                {
                    if (!String.IsNullOrEmpty(rsIns.gtTheTuMoi))
                    {
                        DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheTuMoi).Value;
                        DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheDenMoi) ?? DateTime.MinValue);
                        if (dtHanTheTuMoi.Date <= this.dtIntructionTime.DateTime.Date && (dtHanTheDenMoi == DateTime.MinValue || this.dtIntructionTime.DateTime.Date <= dtHanTheDenMoi.Date))
                        {
                            isUsedNewCard = true;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(dataHein.Address))
                {
                    result = result || (dataHein.Address != rsIns.diaChi);
                }
                result = result || (isUsedNewCard ? (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maTheMoi) : (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maThe));
                result = result || dataHein.Dob != rsIns.ngaySinh;
                result = result || !dataHein.Gender.Equals(gt);
                result = result || (isUsedNewCard ? (dataHein.FromDate != rsIns.gtTheTuMoi) : (dataHein.FromDate != rsIns.gtTheTu));
                // result = result || (!String.IsNullOrEmpty(dataHein.ToDate) && (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen)));CodeCu
                result = result || (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen));
                result = result || (!String.IsNullOrEmpty(dataHein.MediOrgCode) && (isUsedNewCard ? (dataHein.MediOrgCode != rsIns.maDKBDMoi) : (dataHein.MediOrgCode != rsIns.maDKBD)));
                result = result || dataHein.PatientName.ToUpper() != rsIns.hoTen.ToUpper();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
