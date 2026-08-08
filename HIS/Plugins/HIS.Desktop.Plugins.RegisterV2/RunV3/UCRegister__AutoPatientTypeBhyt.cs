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
using HIS.Desktop.Utility;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HeinCardData = Inventec.Common.QrCodeBHYT.HeinCardData;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    /// <summary>
    /// Tu dong chuyen doi tuong benh nhan sang BHYT khi benh nhan co the BHYT con hieu luc.
    /// Nam sau khoa cau hinh HIS.Desktop.Plugins.RegisterV2.IsCheckHeinByCccdWithoutPatientType
    /// (MAC DINH TAT => giu nguyen y het luong cu).
    /// Tham chieu: PTTK_XXXXX_Tu_Dong_Chuyen_Doi_Tuong_BHYT_Khi_Co_The.md
    /// </summary>
    public partial class UCRegister : UserControlBase
    {
        /// <summary>
        /// Tu dong chuyen doi tuong benh nhan sang BHYT khi benh nhan tim duoc co the BHYT con hieu luc
        /// tai ngay tiep don.
        ///
        /// THU TU GOI (bat buoc):
        /// - Goi SAU khi da tim duoc benh nhan => chay sau moi co che dat doi tuong mac dinh
        ///   (theo tai khoan / theo phong tiep don / giu doi tuong benh nhan lien truoc).
        /// - Goi TRUOC FillDataIntoUCHeinInfo va FillDataIntoUCHeinInfoByPatientTypeAlter => hai ham nay
        ///   chi nap thong tin the khi doi tuong dang chon la BHYT hoac Quan nhan (IsPatientTypeUsingHeinInfo).
        ///   Neu goi sau thi o doi tuong doi nhung vung thong tin BHYT van trong.
        /// </summary>
        /// <param name="patientSDO">Ho so benh nhan tim duoc (kem thong tin the BHYT)</param>
        private void ProcessAutoSetPatientTypeBhytByHeinCard(HisPatientSDO patientSDO)
        {
            try
            {
                if (patientSDO == null || this.ucPatientRaw1 == null) return;

                // Khoa cau hinh TAT (mac dinh) => giu nguyen y het luong cu: khong tu chuyen doi tuong.
                // Xac nhan nguoi yeu cau 2026-08-07 - co vien van muon logic cu hoan toan.
                if (!Config.HisConfigCFG.IsAutoSetPatientTypeBhytByHeinCard()) return;

                long patientTypeIdBhyt = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__BHYT;
                if (patientTypeIdBhyt <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("ProcessAutoSetPatientTypeBhytByHeinCard: chua khai bao ma doi tuong BHYT hoac ma khai bao khong ton tai trong danh muc doi tuong benh nhan => bo qua viec tu dong chuyen doi tuong.");
                    return;
                }

                var patientRawADO = this.ucPatientRaw1.GetValue();
                if (patientRawADO == null) return;

                long currentPatientTypeId = patientRawADO.PATIENTTYPE_ID;
                long patientTypeIdQn = HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.PatientTypeId__QN;

                // Doi tuong dang chon da dung vung thong tin BHYT (BHYT / Quan nhan) => khong chuyen.
                // Thong nhat voi co che tu chuyen khi quet QR the BHYT dang chay tren tat ca vien.
                if (currentPatientTypeId == patientTypeIdBhyt
                    || (patientTypeIdQn > 0 && currentPatientTypeId == patientTypeIdQn))
                {
                    return;
                }

                if (!this.IsHeinCardValidForAutoSetPatientType(patientSDO)) return;

                this.ucPatientRaw1.SetValuePatientType(patientTypeIdBhyt);

                // SetValuePatientType chi dat duoc gia tri neu doi tuong BHYT nam trong danh sach doi tuong
                // cua phong tiep don (HIS_RECEPTION_ROOM.PATIENT_TYPE_IDS). Kiem tra lai de ghi nhat ky
                // ro nguyen nhan khi khong chuyen duoc - khong hien loi cho nguoi dung.
                var patientRawADOAfterSet = this.ucPatientRaw1.GetValue();
                if (patientRawADOAfterSet == null || patientRawADOAfterSet.PATIENTTYPE_ID != patientTypeIdBhyt)
                {
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ProcessAutoSetPatientTypeBhytByHeinCard: khong dat duoc doi tuong BHYT - kiem tra danh sach doi tuong cho phep cua phong tiep don."
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdBhyt), patientTypeIdBhyt)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeId), currentPatientTypeId));
                    return;
                }

                Inventec.Common.Logging.LogSystem.Info(
                    "ProcessAutoSetPatientTypeBhytByHeinCard: benh nhan co the BHYT con hieu luc => tu dong chuyen doi tuong sang BHYT."
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentPatientTypeId), currentPatientTypeId)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => patientTypeIdBhyt), patientTypeIdBhyt));

                // Nap vung thong tin BHYT tu du lieu the CO SAN trong ket qua tim benh nhan.
                // Vai tro: DU PHONG. Luong nap the san co ben duoi (FillDataIntoUCHeinInfo) doc lai the tu CSDL
                // va LOC THEO DOI TUONG BHYT - benh nhan truoc gio chi tiep don Vien phi thi khong co ban ghi the
                // gan doi tuong BHYT => luong do thoat som => o doi tuong da doi thanh BHYT nhung vung BHYT trong.
                // Goi TRUOC luong san co: neu luong san co lay duoc du lieu tu CSDL (day du hon: dia chi the,
                // ngay du 5 nam...) thi no ghi de len du lieu nay; neu khong lay duoc thi du lieu nay duoc giu.
                this.FillHeinInfoAfterAutoSetPatientTypeBhyt(patientSDO);

                // Kiem tra the tren cong BHXH ngay sau khi tu chuyen doi tuong.
                // BAT BUOC: viec nap the o tren gan TRUC TIEP du lieu vao vung BHYT nen KHONG kich hoat
                // cac su kien cua o So the - day moi la noi goi kiem tra the tren cong
                // (UCHeinInfo.CheckExamHistoryFromBHXHApi -> CheckTTFull). Neu khong goi o day thi quet xong
                // doi tuong da thanh BHYT ma the chua duoc kiem tra, nguoi dung phai bam vao o So the
                // roi Enter moi kiem tra duoc.
                this.CheckHeinCardOnGovAfterAutoSetPatientTypeBhyt(patientSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Goi kiem tra the BHYT tren cong BHXH sau khi tu dong chuyen doi tuong sang BHYT.
        /// Dung dung ham ma thao tac tay dang dung (CheckTTFull) => cung hanh vi, cung xu ly ket qua
        /// (nap lai vung BHYT, canh bao the het han, chan luu theo cau hinh).
        ///
        /// KHONG phat sinh luot goi cong ngoai y muon:
        ///  - CheckTTFull tu thoat khi vien KHONG bat cau hinh kiem tra the (IsCheckExamHistory)
        ///    hoac doi tuong dang chon khong phai BHYT.
        ///  - Da co ket qua tra cuu cong trong chinh luot tim benh nhan nay thi bo qua (khong goi 2 lan).
        /// </summary>
        private void CheckHeinCardOnGovAfterAutoSetPatientTypeBhyt(HisPatientSDO patientSDO)
        {
            try
            {
                if (patientSDO == null || String.IsNullOrWhiteSpace(patientSDO.HeinCardNumber)) return;

                // Dang trong buoc tim benh nhan theo ma: buoc do TU goi kiem tra the tren cong o cuoi ham
                // (sau khi doi tuong vua duoc chuyen sang BHYT) => bo qua, tranh 2 luot goi cong.
                if (this.ucPatientRaw1 != null && this.ucPatientRaw1.IsInSearchByCodeProcess)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckHeinCardOnGovAfterAutoSetPatientTypeBhyt: buoc tim benh nhan se tu kiem tra the tren cong => khong goi lai.");
                    return;
                }

                // Luong quet CCCD khi cau hinh bat, hoac luong quet QR the BHYT, da tra cong truoc do
                // => ket qua da co san, khong goi lai.
                if (this.ucPatientRaw1 != null && this.ucPatientRaw1.ResultDataADO != null
                    && this.ucPatientRaw1.ResultDataADO.ResultHistoryLDO != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckHeinCardOnGovAfterAutoSetPatientTypeBhyt: da co ket qua tra cuu cong BHXH trong luot tim benh nhan nay => khong goi lai.");
                    return;
                }

                HeinCardData heinCard = new HeinCardData();
                heinCard.HeinCardNumber = patientSDO.HeinCardNumber;
                heinCard.PatientName = patientSDO.VIR_PATIENT_NAME;
                heinCard.Address = patientSDO.HeinAddress;
                heinCard.MediOrgCode = patientSDO.HeinMediOrgCode;
                heinCard.LiveAreaCode = patientSDO.LiveAreaCode;
                heinCard.FineYearMonthDate = patientSDO.Join5Year;
                heinCard.FromDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.HeinCardFromTime ?? 0);
                heinCard.ToDate = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.HeinCardToTime ?? 0);
                if (patientSDO.IS_HAS_NOT_DAY_DOB == 1)
                    heinCard.Dob = patientSDO.DOB.ToString().Length >= 4 ? patientSDO.DOB.ToString().Substring(0, 4) : "";
                else
                    heinCard.Dob = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patientSDO.DOB);
                heinCard.Gender = HIS.Desktop.Plugins.Library.RegisterConfig.GenderConvert.HisToHein(patientSDO.GENDER_ID.ToString());

                this.CheckTTFull(heinCard, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nap vung thong tin BHYT ngay sau khi tu dong chuyen doi tuong sang BHYT.
        ///
        /// Du lieu the trong ho so benh nhan (HisPatientSDO) o luong quet CCCD / VNeID CHI co ma the,
        /// khu vuc va gioi tinh - phan tra cuu cong BHXH khong copy ma noi DKKCB (maDKBD) va han the
        /// (gtTheTu / gtTheDen) sang. Thieu noi DKKCB con chan luon viec nap han the
        /// (ChangeDataHeinInsuranceInfoByPatientTypeAlter chi nap khi co HEIN_MEDI_ORG_CODE).
        /// => Bo sung cac truong con thieu tu ket qua tra cuu cong BHXH (ResultHistoryLDO) truoc khi nap.
        /// </summary>
        private void FillHeinInfoAfterAutoSetPatientTypeBhyt(HisPatientSDO patientSDO)
        {
            try
            {
                if (patientSDO == null || this.ucHeinInfo1 == null) return;

                HIS_PATIENT_TYPE_ALTER patientTypeAlter = new HIS_PATIENT_TYPE_ALTER();
                patientTypeAlter.HEIN_CARD_NUMBER = patientSDO.HeinCardNumber;
                patientTypeAlter.HEIN_CARD_FROM_TIME = patientSDO.HeinCardFromTime;
                patientTypeAlter.HEIN_CARD_TO_TIME = patientSDO.HeinCardToTime;
                patientTypeAlter.HEIN_MEDI_ORG_CODE = patientSDO.HeinMediOrgCode;
                patientTypeAlter.HEIN_MEDI_ORG_NAME = patientSDO.HeinMediOrgName;
                patientTypeAlter.JOIN_5_YEAR = patientSDO.Join5Year;
                patientTypeAlter.PAID_6_MONTH = patientSDO.Paid6Month;
                patientTypeAlter.RIGHT_ROUTE_CODE = patientSDO.RightRouteCode;
                patientTypeAlter.RIGHT_ROUTE_TYPE_CODE = patientSDO.RightRouteTypeCode;
                patientTypeAlter.LIVE_AREA_CODE = patientSDO.LiveAreaCode;

                this.FillMissingHeinFieldFromGovResult(patientTypeAlter);

                this.ucHeinInfo1.SetValueByPatientTypeAlter(patientTypeAlter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bu cac truong con thieu (noi DKKCB, han the tu/den, khu vuc, ngay du 5 nam) tu ket qua tra cuu
        /// cong BHXH. CHI bu khi truong dang trong - khong ghi de du lieu da co.
        /// </summary>
        private void FillMissingHeinFieldFromGovResult(HIS_PATIENT_TYPE_ALTER patientTypeAlter)
        {
            try
            {
                if (patientTypeAlter == null || this.ucPatientRaw1 == null) return;

                var resultDataADO = this.ucPatientRaw1.ResultDataADO;
                if (resultDataADO == null || resultDataADO.ResultHistoryLDO == null) return;

                var resultHistoryLDO = resultDataADO.ResultHistoryLDO;
                bool isUsedNewCard = resultDataADO.IsUsedNewCard;

                string mediOrgCode = (isUsedNewCard && !String.IsNullOrEmpty(resultHistoryLDO.maDKBDMoi))
                    ? resultHistoryLDO.maDKBDMoi : resultHistoryLDO.maDKBD;
                string cardFromDate = (isUsedNewCard && !String.IsNullOrEmpty(resultHistoryLDO.gtTheTuMoi))
                    ? resultHistoryLDO.gtTheTuMoi : resultHistoryLDO.gtTheTu;
                string cardToDate = (isUsedNewCard && !String.IsNullOrEmpty(resultHistoryLDO.gtTheDenMoi))
                    ? resultHistoryLDO.gtTheDenMoi : resultHistoryLDO.gtTheDen;

                if (String.IsNullOrEmpty(patientTypeAlter.HEIN_MEDI_ORG_CODE) && !String.IsNullOrEmpty(mediOrgCode))
                    patientTypeAlter.HEIN_MEDI_ORG_CODE = mediOrgCode;

                if ((patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0) <= 0)
                    patientTypeAlter.HEIN_CARD_FROM_TIME = this.ConvertGovDateStringToDateNumber(cardFromDate);

                if ((patientTypeAlter.HEIN_CARD_TO_TIME ?? 0) <= 0)
                    patientTypeAlter.HEIN_CARD_TO_TIME = this.ConvertGovDateStringToDateNumber(cardToDate);

                if (String.IsNullOrEmpty(patientTypeAlter.LIVE_AREA_CODE) && !String.IsNullOrEmpty(resultHistoryLDO.maKV))
                    patientTypeAlter.LIVE_AREA_CODE = resultHistoryLDO.maKV;

                if ((patientTypeAlter.JOIN_5_YEAR_TIME ?? 0) <= 0)
                    patientTypeAlter.JOIN_5_YEAR_TIME = this.ConvertGovDateStringToDateNumber(resultHistoryLDO.ngayDu5Nam);

                Inventec.Common.Logging.LogSystem.Debug(
                    "FillMissingHeinFieldFromGovResult: da bu thong tin the tu ket qua tra cuu cong BHXH."
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isUsedNewCard), isUsedNewCard));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyen ngay dang chuoi dd/MM/yyyy tra ve tu cong BHXH sang so yyyyMMdd000000.
        /// Tra ve null neu chuoi rong hoac khong dung dinh dang.
        /// </summary>
        private long? ConvertGovDateStringToDateNumber(string govDateString)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(govDateString)) return null;

                string[] dateParts = govDateString.Trim().Split('/');
                if (dateParts.Length != 3) return null;

                return Convert.ToInt64(dateParts[2] + dateParts[1].PadLeft(2, '0') + dateParts[0].PadLeft(2, '0') + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>
        /// Kiem tra the BHYT cua benh nhan co du dieu kien de tu dong chuyen doi tuong hay khong.
        ///
        /// Dieu kien: co SO THE THAT (khong phai so CCCD/CMND duoc gan tam o luong quet CCCD).
        /// KHONG kiem tra han the - the het han / khong hop le van chuyen, xem giai thich ben trong.
        /// </summary>
        private bool IsHeinCardValidForAutoSetPatientType(HisPatientSDO patientSDO)
        {
            bool result = false;
            try
            {
                if (patientSDO == null) return false;

                if (String.IsNullOrWhiteSpace(patientSDO.HeinCardNumber))
                {
                    Inventec.Common.Logging.LogSystem.Debug("IsHeinCardValidForAutoSetPatientType: benh nhan khong co so the BHYT => khong tu dong chuyen doi tuong.");
                    return false;
                }

                // Luong quet QR CCCD / VNeID gan SO CCCD vao truong so the BHYT lam gia tri tam.
                // Neu tra cuu cong BHXH khong ra the that (cong loi, CCCD khong co the BHYT) thi truong nay
                // VAN la so CCCD => KHONG duoc coi la "benh nhan co the BHYT", neu khong se chuyen doi tuong
                // sang BHYT cho benh nhan chua xac thuc co the va de lai so the = so CCCD => sai du lieu khi luu.
                string heinCardNumber = patientSDO.HeinCardNumber.Trim();
                if (heinCardNumber.Equals((patientSDO.CCCD_NUMBER ?? "").Trim(), StringComparison.OrdinalIgnoreCase)
                    || heinCardNumber.Equals((patientSDO.CMND_NUMBER ?? "").Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    Inventec.Common.Logging.LogSystem.Warn("IsHeinCardValidForAutoSetPatientType: so the trung so CCCD/CMND => chua tra cuu duoc the BHYT that => khong tu dong chuyen doi tuong.");
                    return false;
                }

                // KHONG kiem tra han the o day.
                // Nguoi yeu cau chot 2026-08-06: "the het han hay khong hop le thi cu giu nguyen nhu hien tai".
                // Co che tu chuyen san co (benh nhan moi + quet QR the BHYT) VON KHONG kiem tra han the,
                // nen neu them kiem tra o day thi hai luong xu ly nguoc nhau voi cung 1 loai the het han.
                // The het han / khong hop le duoc xu ly boi co che kiem tra the tren cong BHXH va cac cau hinh
                // canh bao / chan luu san co (IsRequiredToUpdateNewBhytCardInCaseOfExpiry, IsBlockingInvalidBhyt,
                // WarningInvalidCheckHistoryHeinCard) - khong chan o buoc chuyen doi tuong.
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

    }
}
