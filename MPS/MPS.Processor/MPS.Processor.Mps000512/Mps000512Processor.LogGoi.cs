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
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000512.ADO;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MPS.Processor.Mps000512
{
    public partial class Mps000512Processor : AbstractProcessor
    {
        private const string LOG_GOI_PREFIX = "MPS000512_GOI: ";

        /// <summary>
        /// Log kiểm tra tiền gói vật tư y tế (trần 45 tháng lương x mức hưởng).
        /// Ghi 2 tầng để so: (1) số liệu THÔ từ DB của từng dịch vụ trong gói, (2) tổng đã gom ra bảng kê (bộ chính + bộ theo khoa).
        /// Không ghi thông tin hành chính bệnh nhân; số thẻ chỉ ghi 3 ký tự đầu.
        /// </summary>
        internal void LogPackageMaterialPrice()
        {
            try
            {
                if (rdo == null || rdo.SereServs == null || rdo.SereServs.Count == 0)
                    return;

                List<long> parentIds = rdo.SereServs
                    .Where(o => o.PARENT_ID.HasValue && o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    .Select(o => o.PARENT_ID.Value).Distinct().ToList();
                if (parentIds.Count == 0)
                    return;

                WriteGoiLog(String.Format("TREATMENT_ID={0}, so goi={1}, IsNotIncludeIsExpend={2}",
                    rdo.Treatment != null ? rdo.Treatment.ID.ToString() : "null", parentIds.Count,
                    rdo.HisConfigValue != null ? rdo.HisConfigValue.IsNotIncludeIsExpend.ToString() : "null"));

                // (1) Số liệu thô từ DB
                foreach (long parentId in parentIds)
                {
                    HIS_SERE_SERV parent = rdo.SereServs.FirstOrDefault(o => o.ID == parentId);
                    WriteGoiLog(String.Format("[THO] GOI PARENT_ID={0} | DV cha: {1}", parentId, parent != null ? RawRow(parent) : "KHONG CO trong rdo.SereServs"));

                    List<HIS_SERE_SERV> children = rdo.SereServs.Where(o => o.PARENT_ID == parentId).OrderBy(o => o.STENT_ORDER ?? 0).ThenBy(o => o.ID).ToList();
                    foreach (HIS_SERE_SERV child in children)
                    {
                        WriteGoiLog(String.Format("[THO]   con: {0}", RawRow(child)));
                    }

                    List<HIS_SERE_SERV> valid = children.Where(o => o.IS_NO_EXECUTE != 1 && o.AMOUNT > 0).ToList();
                    List<HIS_SERE_SERV> validNoExpend = valid.Where(o => o.IS_EXPEND != 1).ToList();
                    decimal sumNoExpend = validNoExpend.Sum(o => o.VIR_TOTAL_PRICE_NO_EXPEND ?? 0);
                    decimal sumHein = validNoExpend.Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0);
                    decimal sumPatientBhyt = validNoExpend.Sum(o => o.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                    WriteGoiLog(String.Format("[THO] TONG GOI PARENT_ID={0}: so dong={1}, khong hao phi={2}, SUM(VIR_TOTAL_PRICE_NO_EXPEND)={3}, SUM(VIR_TOTAL_HEIN_PRICE)={4}, SUM(VIR_TOTAL_PATIENT_PRICE_BHYT)={5}, SUM(HEIN+BN cung tra)={6}, ty le quy/(quy+BN)={7}",
                        parentId, valid.Count, validNoExpend.Count, Num(sumNoExpend), Num(sumHein), Num(sumPatientBhyt), Num(sumHein + sumPatientBhyt),
                        (sumHein + sumPatientBhyt) != 0 ? Num(Math.Round(sumHein / (sumHein + sumPatientBhyt), 6)) : "n/a"));

                    // Tách theo stent: bảng kê chỉ cộng dòng không stent + stent đầu tiên vào TOTAL_HEIN_PRICE của gói
                    if (validNoExpend.Any(o => o.STENT_ORDER.HasValue))
                    {
                        HIS_SERE_SERV firstStent = validNoExpend.Where(o => o.STENT_ORDER.HasValue).OrderBy(o => o.STENT_ORDER).First();
                        decimal heinNoStentPlusFirst = validNoExpend.Where(o => !o.STENT_ORDER.HasValue).Sum(o => o.VIR_TOTAL_HEIN_PRICE ?? 0) + (firstStent.VIR_TOTAL_HEIN_PRICE ?? 0);
                        WriteGoiLog(String.Format("[THO] GOI PARENT_ID={0} co stent: SUM(HEIN) khong stent + stent dau (ID={1}) = {2}",
                            parentId, firstStent.ID, Num(heinNoStentPlusFirst)));
                    }
                }

                // (2) Sau khi processor gom
                LogHeinServiceTypePackages("[GOM-BO CHINH HeinServiceType]", this.heinServiceTypeADOs, this.sereServADOs);
                LogHeinServiceTypePackages("[GOM-THEO KHOA HeinServiceTypeByDepa]", this.heinServiceTypeADOs_ExeRoomByDepa, this.sereServADOs_ExeRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LogHeinServiceTypePackages(string tag, List<HeinServiceTypeADO> heinServiceTypes, List<SereServADO> details)
        {
            if (heinServiceTypes == null)
            {
                WriteGoiLog(tag + " null");
                return;
            }

            List<long> parentIds = rdo.SereServs.Where(o => o.PARENT_ID.HasValue).Select(o => o.PARENT_ID.Value).Distinct().ToList();
            foreach (HeinServiceTypeADO h in heinServiceTypes.Where(o => o.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID || (o.ID.HasValue && parentIds.Contains(o.ID.Value))))
            {
                WriteGoiLog(String.Format("{0} ID={1}, NAME={2}, KEY={3}, KHOA={4}, TOTAL_PRICE_VP={5}, TOTAL_PRICE_BHYT_HST={6}, TOTAL_BHYT_PRICE={7}, TOTAL_HEIN_PRICE={8}, TOTAL_HEIN_PRICE_HST={9}, TOTAL_PATIENT_PRICE_HST={10}, OTHER_SOURCE={11}, TOTAL_PATIENT_PRICE_LEFT={12}",
                    tag, h.ID, h.HEIN_SERVICE_TYPE_NAME, h.KEY_PATY_ALTER, h.GROUP_DEPARTMENT_CODE,
                    Num(h.TOTAL_PRICE_VP), Num(h.TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE), Num(h.TOTAL_BHYT_PRICE), Num(h.TOTAL_HEIN_PRICE),
                    Num(h.TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE), Num(h.TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE), Num(h.OTHER_SOURCE_PRICE), Num(h.TOTAL_PATIENT_PRICE_LEFT)));

                if (details == null || h.ID == HeinServiceTypeExt.GOI_VT_Y_TE__ID)
                    continue;

                foreach (SereServADO s in details.Where(o => o.HEIN_SERVICE_TYPE_ID == h.ID && o.KEY_PATY_ALTER == h.KEY_PATY_ALTER
                    && (h.GROUP_DEPARTMENT_ID == 0 || o.GROUP_DEPARTMENT_ID == h.GROUP_DEPARTMENT_ID)))
                {
                    WriteGoiLog(String.Format("{0}   dong: SERVICE_CODE={1}, AMOUNT={2}, PRICE_VP={3}, PRICE_BHYT={4}, SERVICE_PAY_RATE={5}, BHYT_PAY_RATE={6}, TOTAL_PRICE_VP={7}, TOTAL_PRICE_BHYT={8}, VIR_TOTAL_HEIN_PRICE={9}, VIR_TOTAL_PATIENT_PRICE_BHYT={10}, HEIN_RATIO={11}, STENT_ORDER={12}, IS_EXPEND={13}",
                        tag, s.TDL_SERVICE_CODE, Num(s.AMOUNT), Num(s.PRICE_VP), Num(s.PRICE_BHYT), Num(s.SERVICE_PAY_RATE), Num(s.BHYT_PAY_RATE),
                        Num(s.TOTAL_PRICE_VP), Num(s.TOTAL_PRICE_BHYT), Num(s.VIR_TOTAL_HEIN_PRICE), Num(s.VIR_TOTAL_PATIENT_PRICE_BHYT),
                        Num(s.HEIN_RATIO), s.STENT_ORDER, s.IS_EXPEND));
                }
            }
        }

        private static string RawRow(HIS_SERE_SERV o)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("ID={0}, SERVICE_CODE={1}, SERVICE_TYPE_ID={2}, PATIENT_TYPE_ID={3}, DAU_THE={4}, INTRUCTION_TIME={5}",
                o.ID, o.TDL_SERVICE_CODE, o.TDL_SERVICE_TYPE_ID, o.PATIENT_TYPE_ID,
                String.IsNullOrEmpty(o.HEIN_CARD_NUMBER) ? "" : o.HEIN_CARD_NUMBER.Substring(0, Math.Min(3, o.HEIN_CARD_NUMBER.Length)),
                o.TDL_INTRUCTION_TIME);
            sb.AppendFormat(", AMOUNT={0}, PRICE={1}, ORIGINAL_PRICE={2}, VAT_RATIO={3}, LIMIT_PRICE={4}, HEIN_LIMIT_PRICE={5}, HEIN_PRICE={6}, HEIN_RATIO={7}, PATIENT_PRICE_BHYT={8}, OTHER_SOURCE_PRICE={9}",
                Num(o.AMOUNT), Num(o.PRICE), Num(o.ORIGINAL_PRICE), Num(o.VAT_RATIO), Num(o.LIMIT_PRICE), Num(o.HEIN_LIMIT_PRICE),
                Num(o.HEIN_PRICE), Num(o.HEIN_RATIO), Num(o.PATIENT_PRICE_BHYT), Num(o.OTHER_SOURCE_PRICE));
            sb.AppendFormat(", STENT_ORDER={0}, IS_EXPEND={1}, IS_NO_EXECUTE={2}, IS_OUT_PARENT_FEE={3}",
                o.STENT_ORDER, o.IS_EXPEND, o.IS_NO_EXECUTE, o.IS_OUT_PARENT_FEE);
            sb.AppendFormat(", VIR_TOTAL_PRICE={0}, VIR_TOTAL_PRICE_NO_EXPEND={1}, VIR_TOTAL_HEIN_PRICE={2}, VIR_TOTAL_PATIENT_PRICE_BHYT={3}, VIR_TOTAL_PATIENT_PRICE={4}",
                Num(o.VIR_TOTAL_PRICE), Num(o.VIR_TOTAL_PRICE_NO_EXPEND), Num(o.VIR_TOTAL_HEIN_PRICE), Num(o.VIR_TOTAL_PATIENT_PRICE_BHYT), Num(o.VIR_TOTAL_PATIENT_PRICE));
            return sb.ToString();
        }

        private static string Num(decimal? value)
        {
            return value.HasValue ? value.Value.ToString("0.######", CultureInfo.InvariantCulture) : "null";
        }

        private static void WriteGoiLog(string message)
        {
            Inventec.Common.Logging.LogSystem.Info(LOG_GOI_PREFIX + message);
        }
    }
}
