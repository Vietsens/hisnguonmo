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
using Inventec.Common.QrCodeBHYT;
using Inventec.Common.Logging;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.LocalStorage.BackendData;
using His.Bhyt.InsuranceExpertise.LDO;
using Inventec.Core;
using His.Bhyt.InsuranceExpertise;
using HIS.UC.UCPatientRaw.ADO;
using HIS.Desktop.Plugins.Library.CheckHeinGOV;
using CHC.WCFClient.CheckHeinCardService;
using SDA.EFMODEL.DataModels;
using HeinCardData = Inventec.Common.QrCodeBHYT.HeinCardData;
using DevExpress.XtraEditors;
using MOS.SDO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private async void CheckTTFull(HeinCardData heinCard, Action focusNextControl)
        {
            try
            {
                if (!HisConfigCFG.IsCheckExamHistory) return;
                if (this.ucPatientRaw1 != null)
                {
                    UCPatientRawADO patientRawADO = this.ucPatientRaw1.GetValue();
                    if (patientRawADO.PATIENTTYPE_ID != HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT)
                    {
                        return;
                    }
                }

                if (heinCard == null) { return; }
                if (this.isResetForm) { return; }

                HeinGOVManager heinGOVManager = new HeinGOVManager(ResourceMessage.GoiSangCongBHXHTraVeMaLoi);
                if ((HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option1).ToString() || HisConfigCFG.IsBlockingInvalidBhyt == ((int)HisConfigCFG.OptionKey.Option2).ToString()))
                    heinGOVManager.SetDelegateHeinEnableButtonSave(HeinEnableSave);
                if (string.IsNullOrEmpty(heinCard.Dob)
                    || string.IsNullOrEmpty(heinCard.PatientName)
                    || string.IsNullOrEmpty(heinCard.Gender))
                {
                    if (this.ucPatientRaw1 != null)
                    {
                        var dataPatient = (UCPatientRawADO)this.ucPatientRaw1.GetValue();
                        if (dataPatient != null)
                        {
                            if (dataPatient.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                heinCard.Dob = dataPatient.DOB.ToString().Substring(0, 4);
                            }
                            else
                                heinCard.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataPatient.DOB);
                            heinCard.PatientName = dataPatient.PATIENT_NAME;
                            heinCard.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(dataPatient.GENDER_ID.ToString());
                        }
                    }
                    else
                    {
                        LogSystem.Debug("ucPatientRaw1 null, khong khoi tao dc ucPatientRaw1");
                    }
                }

                if (String.IsNullOrEmpty(heinCard.HeinCardNumber)
                    //|| String.IsNullOrEmpty(heinCard.Address)
                        || String.IsNullOrEmpty(heinCard.FromDate)
                        || String.IsNullOrEmpty(heinCard.MediOrgCode))
                {
                    if (this.ucHeinInfo1 != null)
                    {
                        var dataHein = (MOS.SDO.HisPatientProfileSDO)this.ucHeinInfo1.GetValue();
                        if (dataHein != null && dataHein.HisPatientTypeAlter != null)
                        {
                            heinCard.HeinCardNumber = dataHein.HisPatientTypeAlter.HEIN_CARD_NUMBER;
                            heinCard.Address = dataHein.HisPatientTypeAlter.ADDRESS;
                            heinCard.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataHein.HisPatientTypeAlter.HEIN_CARD_FROM_TIME.ToString());
                            heinCard.MediOrgCode = dataHein.HisPatientTypeAlter.HEIN_MEDI_ORG_CODE;
                            heinCard.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(dataHein.HisPatientTypeAlter.HEIN_CARD_TO_TIME.ToString());
                            heinCard.LiveAreaCode = dataHein.HisPatientTypeAlter.LIVE_AREA_CODE;
                        }
                    }
                }


                this.ucPatientRaw1.ResultDataADO = await heinGOVManager.Check(heinCard, focusNextControl, false, (this.currentPatientSDO != null && this.currentPatientSDO.ID > 0 ? this.currentPatientSDO.HeinAddress : ""), this.GetIntructionTime(), this.isReadQrCode);
                if (this.ucHeinInfo1 != null)
                {
                    this.ucHeinInfo1.ResultDataADO = this.ucPatientRaw1.ResultDataADO;
                }
                if (this.ucPatientRaw1.ResultDataADO != null)
                {
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (!String.IsNullOrEmpty(heinCard.HeinCardNumber))
                    {
                        if (this.ucPatientRaw1.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                        {
                            heinCard.HeinCardNumber = this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.maTheMoi;
                        }

                        //data = this.CheckPatientOldByHeinCard(heinCard);
                    }

                    this.CheckTTProcessResultData(heinCard, focusNextControl, false);
                }
                //else
                //{
                //    //data = this.CheckPatientOldByHeinCard(heinCard);
                //}                
            }
            catch (Exception ex)
            {
                HeinEnableSave(true);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task CheckTTProcessResultData(HeinCardData dataHein, Action focusNextControl, bool ischeckChange)
        {
            try
            {
                

                if (this.ucPatientRaw1.ResultDataADO != null && this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO != null)
                {
                    this.ucPatientRaw1.ResultDataADO.HeinCardData.FineYearMonthDate = this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.ngayDu5Nam;
                    //Trường hợp tìm kiếm BN theo qrocde & BN có số thẻ bhyt mới, cần tìm kiếm BN theo số thẻ mới này & người dùng chọn lấy thông tin thẻ mới => tìm kiếm Bn theo số thẻ mới
                    if (this.ucPatientRaw1.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose)
                    {
                        UCPatientRawADO data = new UCPatientRawADO();
                        data.PATIENT_NAME = this.ucPatientRaw1.ResultDataADO.HeinCardData.PatientName;
                        data.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.ucPatientRaw1.ResultDataADO.HeinCardData.Gender);
                        data.DOB_STR = this.ucPatientRaw1.ResultDataADO.HeinCardData.Dob;
                        DateTime? dtPatientDob = null;
                        if (data.DOB_STR.Length == 10)
                        {
                            data.IS_HAS_NOT_DAY_DOB = 0;
                            dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(data.DOB_STR);
                        }
                        else if (data.DOB_STR.Length == 4)
                        {
                            data.IS_HAS_NOT_DAY_DOB = 1;
                            dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate("01/01/" + data.DOB_STR);
                        }

                        data.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPatientDob) ?? 0;
                        this.ucPatientRaw1.UpdateValueAfterCheckTT(data);

                        if (IsPatientTypeUsingHeinInfo())
                            this.ucHeinInfo1.FillDataByHeinCardData(this.ucPatientRaw1.ResultDataADO.HeinCardData);


                    }

                    if (this.ucPatientRaw1.ResultDataADO.IsToDate)
                    {
                        if (IsPatientTypeUsingHeinInfo())
                            this.ucHeinInfo1.FillDataByHeinCardData(this.ucPatientRaw1.ResultDataADO.HeinCardData);
                        Inventec.Common.Logging.LogSystem.Debug("Ket thuc gan du lieu cho benh nhan khi doc the va khong co han den");
                    }

                    if (this.ucPatientRaw1.ResultDataADO.IsAddress || this.ucPatientRaw1.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        if (AppConfigs.CheDoTuDongFillDuLieuDiaChiGhiTrenTheVaoODiaChiBenhNhanHayKhong == 1)
                        {
                            dataAddressPatient = this.ucAddressCombo1.GetValue() ?? new HIS.UC.AddressCombo.ADO.UCAddressADO();
                            Inventec.Common.Address.AddressProcessor adProc = new Inventec.Common.Address.AddressProcessor(BackendDataWorker.Get<V_SDA_PROVINCE>(), BackendDataWorker.Get<V_SDA_DISTRICT>(), BackendDataWorker.Get<V_SDA_COMMUNE>());
                            var data = adProc.SplitFromFullAddress(this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.diaChi);
                            if (data != null)
                            {
                                dataAddressPatient.Province_Code = data.ProvinceCode;
                                dataAddressPatient.Province_Name = data.ProvinceName;
                                dataAddressPatient.District_Code = data.DistrictCode;
                                dataAddressPatient.District_Name = data.DistrictName;
                                dataAddressPatient.Commune_Code = data.CommuneCode;
                                dataAddressPatient.Commune_Name = data.CommuneName;
                            }
                            dataAddressPatient.Address = this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO.diaChi;
                            this.ucAddressCombo1.SetValue(dataAddressPatient);
                        }
                    }

                    if (this.ucPatientRaw1.ResultDataADO.IsThongTinNguoiDungThayDoiSoVoiCong__Choose)
                    {
                        UCPatientRawADO data = new UCPatientRawADO();
                        data.PATIENT_NAME = this.ucPatientRaw1.ResultDataADO.HeinCardData.PatientName;
                        data.GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(this.ucPatientRaw1.ResultDataADO.HeinCardData.Gender);
                        data.DOB_STR = this.ucPatientRaw1.ResultDataADO.HeinCardData.Dob;
                        DateTime? dtPatientDob = null;
                        if (data.DOB_STR.Length == 10)
                        {
                            data.IS_HAS_NOT_DAY_DOB = 0;
                            dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate(data.DOB_STR);
                        }
                        else if (data.DOB_STR.Length == 4)
                        {
                            data.IS_HAS_NOT_DAY_DOB = 1;
                            dtPatientDob = HIS.Desktop.Utility.DateTimeHelper.ConvertDateStringToSystemDate("01/01/" + data.DOB_STR);
                        }

                        data.DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtPatientDob) ?? 0;
                        this.ucPatientRaw1.UpdateValueAfterCheckTT(data);

                        if (IsPatientTypeUsingHeinInfo())
                            this.ucHeinInfo1.FillDataByHeinCardData(this.ucPatientRaw1.ResultDataADO.HeinCardData);


                    }
                    CheckRRCodeTTFee(false);
                    if (HisConfigCFG.IsCheckExamHistory
                        && this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO != null)
                    {
                        if ((this.ucPatientRaw1.ResultDataADO.IsShowQuestionWhileChangeHeinTime__Choose
                        || this.ucPatientRaw1.ResultDataADO.SuccessWithoutMessage))
                        {
                            this.ucCheckTT1.FillDataIntoUCCheckTT(this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO);
                        }
                        else
                        {
                            this.ucCheckTT1.ResetDataControl(this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO);
                        }

                    }


                    Inventec.Common.Logging.LogSystem.Debug("CheckTTProcessResultData 3");
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
                var dt = ucHeinInfo1.GetValue();
                if (dt.HisPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.FALSE && HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.IS_WARNING_WRONG_ROUTE_FEE == 1)
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


        private bool CheckChangeInfo(HeinCardData dataHein, ResultHistoryLDO rsIns, bool isHasNewCard)
        {
            bool result = false;
            try
            {
                string gt = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.TextToNumber(rsIns.gioiTinh);
                bool isUsedNewCard = false;
                DateTime dtIntructionTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.ucOtherServiceReqInfo1.GetValue().IntructionTime) ?? DateTime.MinValue;

                if (isHasNewCard)
                {
                    if (!String.IsNullOrEmpty(rsIns.gtTheTuMoi))
                    {
                        DateTime dtHanTheTuMoi = DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheTuMoi).Value;
                        DateTime dtHanTheDenMoi = (DateTimeHelper.ConvertDateStringToSystemDate(rsIns.gtTheDenMoi) ?? DateTime.MinValue);
                        if (dtHanTheTuMoi.Date <= dtIntructionTime.Date && (dtHanTheDenMoi == DateTime.MinValue || dtIntructionTime.Date <= dtHanTheDenMoi.Date))
                        {
                            isUsedNewCard = true;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(dataHein.Address))
                {
                    //Check lai// xuandv
                    //    if (this.ucPatientRaw1._HeinCardData != null && !String.IsNullOrEmpty(this.ucPatientRaw1._HeinCardData.Address))
                    //    {
                    //        result = result || (dataHein.Address != Inventec.Common.String.Convert.HexToUTF8Fix(this.ucPatientRaw1._HeinCardData.Address));
                    //    }
                    //    else 
                    if (this.currentPatientSDO != null && !String.IsNullOrEmpty(this.currentPatientSDO.HeinAddress))
                    {
                        result = result || (dataHein.Address != this.currentPatientSDO.HeinAddress);
                    }
                    else
                    {
                        result = result || (dataHein.Address != rsIns.diaChi);
                    }
                }
                result = result || (isUsedNewCard ? (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maTheMoi) : (HeinCardHelper.TrimHeinCardNumber(dataHein.HeinCardNumber) != rsIns.maThe));

                if (rsIns.ngaySinh.Length == 4)
                {
                    result = result || dataHein.Dob.Substring(6, 4) != rsIns.ngaySinh;
                }
                else
                {
                    result = result || dataHein.Dob != rsIns.ngaySinh;
                }
                result = result || !dataHein.Gender.Equals(gt);
                result = result || (isUsedNewCard ? (dataHein.FromDate != rsIns.gtTheTuMoi) : (dataHein.FromDate != rsIns.gtTheTu));
                //result = result || (!String.IsNullOrEmpty(dataHein.ToDate) && (isUsedNewCard ? (dataHein.ToDate != rsIns.gtTheDenMoi) : (dataHein.ToDate != rsIns.gtTheDen)));
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

        private void CheckHeinCardByServerBhxh(HeinCardData dataHein)
        {
            try
            {
                if (!HisConfigCFG.IsCheckExamHistory) return;

                CheckTTFull(dataHein, null);
                _HeinCardData = dataHein;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
