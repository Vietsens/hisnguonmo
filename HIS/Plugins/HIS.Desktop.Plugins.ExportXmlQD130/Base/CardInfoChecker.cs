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
using HIS.Desktop.Plugins.ExportXmlQD130.ADO;
using HIS.Desktop.Plugins.ExportXmlQD130.Resources;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Viec 60261 (BV Da khoa tinh Dien Bien): kiem 6 truong the, ho so chinh cua XML1
    /// (SO_CCCD, NGAY_SINH, MA_THE_BHYT, MA_DKBD, GT_THE_TU, GT_THE_DEN) truoc khi xuat/gui.
    /// Chi canh bao - cot trong, the thua/thieu ky tu - khong chan. Khong goi API, khong co giao dien.
    /// Gia tri dem kiem lay dung nhu Xml1Processor cua thu vien His.Bhyt.ExportXml.XML130.XML1.
    /// </summary>
    internal static class CardInfoChecker
    {
        internal const string SO_CCCD = "SO_CCCD";
        internal const string NGAY_SINH = "NGAY_SINH";
        internal const string MA_THE_BHYT = "MA_THE_BHYT";
        internal const string MA_DKBD = "MA_DKBD";
        internal const string GT_THE_TU = "GT_THE_TU";
        internal const string GT_THE_DEN = "GT_THE_DEN";

        //Ma the BHYT: mau cu 15 ky tu, mau moi 17 ky tu (12 so dinh danh) - nhu HeinCardHelper va man tiep don
        private const int HEIN_CARD_LENGTH = 15;
        private const int HEIN_CARD_LENGTH_NEW = 17;
        private const int MEDI_ORG_CODE_LENGTH = 5;
        //So CCCD/dinh danh ca nhan 12 so; CMND cu 9 so (XML 3176 lui sang CMND khi khong co CCCD)
        private const int CCCD_LENGTH = 12;
        private const int CMND_LENGTH = 9;
        //Thoi gian kieu yyyyMMddHHmmss: nho hon so nay la thieu chu so (thu vien cat 12 ky tu ngay sinh, 8 ky tu han the)
        private const long MIN_TIME_NUMBER = 10000101000000;
        //HIS_PATIENT.IS_NO_CHECK_CCCD: benh nhan duoc tich "Khong CCCD" o man tiep don
        private const short IS_NO_CHECK_CCCD__TRUE = 1;

        /// <summary>
        /// Cac the co trong XML1: moi dong dien dieu tri co so the, bo dong trung ca 4 thong tin
        /// (giong Xml1Processor gom theo so the, noi DKBD, tu ngay, den ngay roi noi bang ';').
        /// </summary>
        internal static List<V_HIS_PATIENT_TYPE_ALTER> GetXml1Cards(List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters)
        {
            List<V_HIS_PATIENT_TYPE_ALTER> result = new List<V_HIS_PATIENT_TYPE_ALTER>();
            try
            {
                if (patientTypeAlters != null)
                {
                    result = patientTypeAlters
                        .Where(o => o != null && !String.IsNullOrWhiteSpace(o.HEIN_CARD_NUMBER))
                        .GroupBy(o => new { o.HEIN_CARD_NUMBER, o.HEIN_MEDI_ORG_CODE, o.HEIN_CARD_FROM_TIME, o.HEIN_CARD_TO_TIME })
                        .Select(g => g.First())
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>The hien tai cua ho so (thong tin the tren dong luoi) - dung cho cot canh bao va check-in (lay the cuoi)</summary>
        internal static List<V_HIS_PATIENT_TYPE_ALTER> GetCurrentCard(V_HIS_TREATMENT_1 treatment)
        {
            List<V_HIS_PATIENT_TYPE_ALTER> result = new List<V_HIS_PATIENT_TYPE_ALTER>();
            try
            {
                if (treatment != null && !String.IsNullOrWhiteSpace(treatment.TDL_HEIN_CARD_NUMBER))
                {
                    V_HIS_PATIENT_TYPE_ALTER card = new V_HIS_PATIENT_TYPE_ALTER();
                    card.HEIN_CARD_NUMBER = treatment.TDL_HEIN_CARD_NUMBER;
                    card.HEIN_MEDI_ORG_CODE = treatment.TDL_HEIN_MEDI_ORG_CODE;
                    card.HEIN_CARD_FROM_TIME = treatment.TDL_HEIN_CARD_FROM_TIME;
                    card.HEIN_CARD_TO_TIME = treatment.TDL_HEIN_CARD_TO_TIME;
                    result.Add(card);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Kiem mot ho so. Tra ve danh sach canh bao, rong = khong co gi phai sua.
        /// Chi kiem ho so BHYT (doi tuong BHYT hoac dot dieu tri co the) - ho so thu phi XML de trong the la dung.
        /// </summary>
        /// <param name="cards">Cac the se nam trong XML: GetXml1Cards (XML 130) hoac GetCurrentCard (cot luoi, check-in)</param>
        /// <param name="is3176">Che do XML 3176: SO_CCCD lui sang CMND, ho chieu khi khong co CCCD (nhu thu vien)</param>
        internal static List<TienGiamDinhErrorADO> Check(V_HIS_TREATMENT_1 treatment, List<V_HIS_PATIENT_TYPE_ALTER> cards, bool is3176, long patientTypeIdBhyt)
        {
            List<TienGiamDinhErrorADO> result = new List<TienGiamDinhErrorADO>();
            try
            {
                if (treatment == null)
                {
                    return result;
                }

                cards = cards ?? new List<V_HIS_PATIENT_TYPE_ALTER>();
                bool isBhyt = cards.Count > 0 || (patientTypeIdBhyt > 0 && treatment.TDL_PATIENT_TYPE_ID == patientTypeIdBhyt);
                if (!isBhyt)
                {
                    return result;
                }

                CheckSoCccd(treatment, is3176, result);

                //Thu vien cat 12 ky tu dau cua ngay sinh: ngay sinh thieu chu so la hong NGAY_SINH. Chi co nam sinh van hop le.
                if (treatment.TDL_PATIENT_DOB < MIN_TIME_NUMBER)
                {
                    Add(result, NGAY_SINH, false, ResourceMessageLang.CardInfoNgaySinhKhongHopLe);
                }

                if (cards.Count == 0)
                {
                    Add(result, MA_THE_BHYT, true, ResourceMessageLang.CardInfoThieuMaThe);
                }
                foreach (V_HIS_PATIENT_TYPE_ALTER card in cards)
                {
                    CheckCard(card, result);
                }
            }
            catch (Exception ex)
            {
                //Loi phia kiem tra chi ghi log - canh bao khong duoc lam hong viec xuat/gui
                LogSystem.Warn(ex);
            }
            return result;
        }

        private static void CheckSoCccd(V_HIS_TREATMENT_1 treatment, bool is3176, List<TienGiamDinhErrorADO> result)
        {
            //Nhu Xml1Processor: XML 3176 lay CCCD ?? CMND ?? ho chieu (?? chi lui khi null)
            string soCccd = is3176
                ? (treatment.TDL_PATIENT_CCCD_NUMBER ?? treatment.TDL_PATIENT_CMND_NUMBER ?? treatment.TDL_PATIENT_PASSPORT_NUMBER)
                : treatment.TDL_PATIENT_CCCD_NUMBER;

            if (String.IsNullOrEmpty(soCccd))
            {
                //Quy uoc chung cua HIS: benh nhan tich "Khong CCCD" o tiep don thi khong kiem CCCD
                if (treatment.TDL_PATIENT_IS_NO_CHECK_CCCD != IS_NO_CHECK_CCCD__TRUE)
                {
                    Add(result, SO_CCCD, false, ResourceMessageLang.CardInfoThieuCccd);
                }
                return;
            }

            if (soCccd.All(Char.IsDigit))
            {
                if (soCccd.Length != CCCD_LENGTH && soCccd.Length != CMND_LENGTH)
                {
                    Add(result, SO_CCCD, false, String.Format(ResourceMessageLang.CardInfoCccdSaiDoDai, soCccd, soCccd.Length));
                }
            }
            else if (!soCccd.All(Char.IsLetterOrDigit))
            {
                //Co chu cai la so ho chieu - khong kiem do dai; dau cach, dau gach... la go thua
                Add(result, SO_CCCD, false, String.Format(ResourceMessageLang.CardInfoCccdKyTuLa, soCccd));
            }
        }

        private static void CheckCard(V_HIS_PATIENT_TYPE_ALTER card, List<TienGiamDinhErrorADO> result)
        {
            string cardNumber = card.HEIN_CARD_NUMBER ?? "";
            if (cardNumber.Length != HEIN_CARD_LENGTH && cardNumber.Length != HEIN_CARD_LENGTH_NEW)
            {
                Add(result, MA_THE_BHYT, true, String.Format(ResourceMessageLang.CardInfoMaTheSaiDoDai, cardNumber, cardNumber.Length));
            }

            if (String.IsNullOrEmpty(card.HEIN_MEDI_ORG_CODE))
            {
                Add(result, MA_DKBD, true, String.Format(ResourceMessageLang.CardInfoThieuNoiDkbd, cardNumber));
            }
            else if (card.HEIN_MEDI_ORG_CODE.Length != MEDI_ORG_CODE_LENGTH)
            {
                Add(result, MA_DKBD, true, String.Format(ResourceMessageLang.CardInfoNoiDkbdSaiDoDai, cardNumber, card.HEIN_MEDI_ORG_CODE, card.HEIN_MEDI_ORG_CODE.Length));
            }

            if (!card.HEIN_CARD_FROM_TIME.HasValue || card.HEIN_CARD_FROM_TIME.Value < MIN_TIME_NUMBER)
            {
                Add(result, GT_THE_TU, true, String.Format(ResourceMessageLang.CardInfoThieuGtTheTu, cardNumber));
            }
            if (!card.HEIN_CARD_TO_TIME.HasValue || card.HEIN_CARD_TO_TIME.Value < MIN_TIME_NUMBER)
            {
                Add(result, GT_THE_DEN, true, String.Format(ResourceMessageLang.CardInfoThieuGtTheDen, cardNumber));
            }
        }

        private static void Add(List<TienGiamDinhErrorADO> result, string fieldCode, bool isCardGroup, string description)
        {
            TienGiamDinhErrorADO warning = new TienGiamDinhErrorADO();
            warning.Group = EnumTienGiamDinhErrorGroup.HeinCard;
            warning.GroupName = isCardGroup ? ResourceMessageLang.CardInfoNhomThe : ResourceMessageLang.CardInfoNhomHoSo;
            warning.IsCritical = false;
            warning.SeverityName = ResourceMessageLang.CardInfoMucDo;
            warning.Code = fieldCode;
            warning.Description = description;
            result.Add(warning);
        }
    }
}
