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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.Library.ElectronicBill.Config;
using HIS.Desktop.Plugins.Library.ElectronicBill.Data;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    /// <summary>
    /// Gom toàn bộ chi tiết về 1 dòng hóa đơn duy nhất.
    /// ProdName = "Thu phí (nhóm1: tiền; ...) + {X}% Cùng chi trả (nhóm1: tiền; ...)"
    ///   với X = tỷ lệ cùng chi trả = 100 - mức hưởng BHYT (mức hưởng lấy từ BhytHeinProcessor.GetDefaultHeinRatio
    ///   theo thẻ BHYT của bệnh nhân). Không tính được X (hoặc X = 0) -> nhãn "Cùng chi trả" (không %).
    /// Cách tính khớp Mps000512 (biến thể TOTAL_PATIENT_PRICE_LEFT):
    ///   - Thu phí      = bệnh nhân tự trả + nguồn khác = PRICE - cùng chi trả
    ///   - Cùng chi trả = bệnh nhân cùng chi trả BHYT   = TDL_TOTAL_PATIENT_PRICE_BHYT
    /// PRICE = VIR_TOTAL_PATIENT_PRICE = phần bệnh nhân phải trả; quỹ BHYT chi trả không nằm trong PRICE.
    ///
    /// NHÓM loại dịch vụ: gom theo mã Bộ Y tế BHYT_CODE (1..18) trên HIS_HEIN_SERVICE_TYPE:
    ///   1 Xét nghiệm, 2 Chẩn đoán hình ảnh, 3 Thăm dò chức năng, 4 Thuốc, 7 Máu, 8 Phẫu thuật,
    ///   10 Vật tư y tế, 12 Vận chuyển, 13 Khám bệnh, 14 Ngày giường bệnh ban ngày,
    ///   15 Ngày giường bệnh điều trị nội trú, 16 Ngày giường lưu, 17 Chế phẩm máu, 18 Thủ thuật.
    /// Mã ngoài danh sách (gồm 5, 6, 9, 11) và dịch vụ không có loại BHYT -> "Dịch vụ khác".
    /// Thứ tự nhóm theo mã BHYT_CODE tăng dần; "Dịch vụ khác" xuống cuối.
    /// Đơn vị luôn "Lần", số lượng luôn 1, đơn giá = thành tiền = tổng tiền (= tổng PRICE phần bệnh nhân).
    /// </summary>
    class Template12 : IRunTemplate
    {
        private const string GROUP_OTHER = "Dịch vụ khác";
        private const long ORDER_OTHER = long.MaxValue;

        /// <summary>
        /// Map mã Bộ Y tế (HIS_HEIN_SERVICE_TYPE.BHYT_CODE) -> tên nhóm hiển thị.
        /// Mã 5, 6, 9, 11 không dùng theo quy định -> không khai báo (rơi vào "Dịch vụ khác").
        /// </summary>
        private static readonly Dictionary<int, string> BHYT_GROUP_BY_CODE = new Dictionary<int, string>
        {
            { 1, "Xét nghiệm" },
            { 2, "Chẩn đoán hình ảnh" },
            { 3, "Thăm dò chức năng" },
            { 4, "Thuốc" },
            { 7, "Máu" },
            { 8, "Phẫu thuật" },
            { 10, "Vật tư y tế" },
            { 12, "Vận chuyển" },
            { 13, "Khám bệnh" },
            { 14, "Ngày giường bệnh ban ngày" },
            { 15, "Ngày giường bệnh điều trị nội trú" },
            { 16, "Ngày giường lưu" },
            { 17, "Chế phẩm máu" },
            { 18, "Thủ thuật" },
        };

        private Base.ElectronicBillDataInput DataInput;

        public Template12(Base.ElectronicBillDataInput dataInput)
        {
            this.DataInput = dataInput;
        }

        public object Run()
        {
            List<ProductBase> result = new List<ProductBase>();
            try
            {
                if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
                {
                    List<HIS_HEIN_SERVICE_TYPE> heinTypes = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>();

                    List<string> orderedGroups = new List<string>();
                    Dictionary<string, decimal> thuPhiByGroup = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> cungByGroup = new Dictionary<string, decimal>();
                    Dictionary<string, long> groupOrder = new Dictionary<string, long>();
                    decimal total = 0;

                    foreach (var ss in DataInput.SereServBill)
                    {
                        long orderKey;
                        string group = ResolveGroup(ss, heinTypes, out orderKey);

                        decimal cungChiTra = ss.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0;   
                        decimal thuPhi = Math.Max(0, ss.PRICE - cungChiTra);         

                        total += ss.PRICE;                                           

                        if (!thuPhiByGroup.ContainsKey(group))
                        {
                            orderedGroups.Add(group);
                            thuPhiByGroup[group] = 0;
                            cungByGroup[group] = 0;
                            groupOrder[group] = orderKey;
                        }
                        thuPhiByGroup[group] += thuPhi;
                        cungByGroup[group] += cungChiTra;
                    }

                    // Sắp xếp nhóm theo mã BHYT_CODE tăng dần; "Dịch vụ khác" xuống cuối.
                    orderedGroups = orderedGroups.OrderBy(g => groupOrder[g]).ThenBy(g => g).ToList();

                    // Dựng ProdName
                    List<string> thuPhiParts = new List<string>();
                    List<string> cungParts = new List<string>();
                    foreach (var group in orderedGroups)
                    {
                        decimal tp = thuPhiByGroup[group];
                        decimal cc = cungByGroup[group];

                        if (tp > 0)
                            thuPhiParts.Add(String.Format("{0}: {1}", group, Inventec.Common.Number.Convert.NumberToStringRoundMax4(tp)));
                        if (cc > 0)
                            cungParts.Add(String.Format("{0}: {1}", group, Inventec.Common.Number.Convert.NumberToStringRoundMax4(cc)));
                    }

                    StringBuilder prodName = new StringBuilder();
                    if (thuPhiParts.Count > 0)
                        prodName.Append(String.Format("Thu phí ({0})", String.Join("; ", thuPhiParts)));
                    if (cungParts.Count > 0)
                    {
                        if (prodName.Length > 0)
                            prodName.Append(" + ");

                        // Nhãn kèm % cùng chi trả (= 100 - mức hưởng BHYT). Không tính được -> chỉ ghi "Cùng chi trả".
                        decimal? coPayPercent = GetCoPayPercent();
                        string cungLabel = (coPayPercent.HasValue && coPayPercent.Value > 0)
                            ? String.Format("{0}% BHYT", (long)coPayPercent.Value)
                            : "BHYT";
                        prodName.Append(String.Format("{0} ({1})", cungLabel, String.Join("; ", cungParts)));
                    }

                    if (prodName.Length > 0)
                    {
                        decimal amount = Inventec.Common.Number.Convert.NumberToNumberRoundMax4(total);

                        ProductBase product = new ProductBase();
                        product.ProdName = prodName.ToString();
                        product.ProdCode = "TTVP";
                        product.ProdUnit = "Lần";
                        product.ProdQuantity = 1;
                        product.Amount = amount;
                        product.ProdPrice = amount;
                        product.TaxRateID = Base.ProviderType.tax_KCT;
                        product.Stt = 0;

                        // Làm tròn theo cấu hình (giữ đơn giá = thành tiền)
                        if (HisConfigCFG.RoundTransactionAmountOption == "1" || HisConfigCFG.RoundTransactionAmountOption == "2")
                        {
                            product.Amount = Math.Round(product.Amount, 0, MidpointRounding.AwayFromZero);
                            product.ProdPrice = product.Amount;
                        }

                        result.Add(product);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Xác định nhóm của 1 dòng theo mã Bộ Y tế BHYT_CODE của loại dịch vụ BHYT (HIS_HEIN_SERVICE_TYPE).
        /// orderKey = mã BHYT (để sắp thứ tự).
        /// Fallback: dòng không tra được mã BHYT (vd thuốc/VTYT mua ngoài - hóa đơn không có mã điều trị nên
        /// thiếu loại DV BHYT) -> gom theo SERVICE_TYPE: thuốc -> "Thuốc" (mã 4), VTYT -> "Vật tư y tế" (mã 10).
        /// Còn lại -> "Dịch vụ khác".
        /// </summary>
        private string ResolveGroup(HIS_SERE_SERV_BILL ss, List<HIS_HEIN_SERVICE_TYPE> heinTypes, out long orderKey)
        {
            long heinId = ss.TDL_HEIN_SERVICE_TYPE_ID ?? 0;
            HIS_HEIN_SERVICE_TYPE hein = heinTypes != null ? heinTypes.FirstOrDefault(o => o.ID == heinId) : null;
            if (hein != null)
            {
                string bhytCode = Convert.ToString(hein.BHYT_CODE);
                int code;
                if (!String.IsNullOrWhiteSpace(bhytCode)
                    && int.TryParse(bhytCode.Trim(), out code)
                    && BHYT_GROUP_BY_CODE.ContainsKey(code))
                {
                    orderKey = code;
                    return BHYT_GROUP_BY_CODE[code];
                }
            }

            orderKey = ORDER_OTHER;
            return GROUP_OTHER;
        }

        /// <summary>
        /// % cùng chi trả = 100 - mức hưởng BHYT (mức hưởng lấy từ BhytHeinProcessor.GetDefaultHeinRatio theo
        /// thông tin thẻ BHYT của bệnh nhân - DataInput.LastPatientTypeAlter). Trả null nếu không xác định được.
        /// </summary>
        private decimal? GetCoPayPercent()
        {
            try
            {
                var alter = DataInput.LastPatientTypeAlter;
                if (alter == null)
                    return null;

                decimal ratio = (new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(
                    alter.HEIN_TREATMENT_TYPE_CODE, alter.HEIN_CARD_NUMBER, alter.LEVEL_CODE,
                    alter.RIGHT_ROUTE_CODE, alter.FACILITY_CLASS, alter.FORMER_LEVEL_CODE,
                    (long)(alter.CLASSIFY_POINT ?? 0),
                    DataInput.Treatment != null ? DataInput.Treatment.CLINICAL_IN_TIME ?? 0 : 0) ?? 0) * 100;

                return Math.Round(100 - ratio, 0, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
