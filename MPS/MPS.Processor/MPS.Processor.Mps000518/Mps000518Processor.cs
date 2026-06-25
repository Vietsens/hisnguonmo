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
using System.Linq;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MPS.Processor.Mps000518.PDO;
using MPS.ProcessorBase.Core;

namespace MPS.Processor.Mps000518
{
    /// <summary>
    /// Biểu in Biên bản/Hợp đồng cung ứng thuốc - vật tư của nhà cung cấp.
    /// Đầu vào (PDO): V_HIS_MEDICAL_CONTRACT, HIS_SUPPLIER,
    ///                List&lt;V_HIS_MEDI_CONTRACT_METY&gt;, List&lt;V_HIS_MEDI_CONTRACT_MATY&gt;.
    /// Cung cấp cho template:
    ///   - Key đơn: toàn bộ cột của V_HIS_MEDICAL_CONTRACT + HIS_SUPPLIER (sinh tự động qua reflection)
    ///     + thông tin bệnh viện (ORGANIZATION_NAME, PARENT_ORGANIZATION_NAME... do base SetCommonSingleKey).
    ///   - AUTH_LETTER_ISSUE_DATE_STR: ngày cấp giấy ủy quyền dạng "ngày dd tháng mm năm yyyy".
    ///   - SUM_CONTACT_PRICE / SUM_CONTACT_PRICE_TEXT: tổng tiền (số + chữ) = SUM(VIR_CONTACT_PRICE) METY + MATY.
    ///   - Band "Mety": danh sách thuốc; Band "Maty": danh sách vật tư.
    /// </summary>
    public class Mps000518Processor : AbstractProcessor
    {
        Mps000518PDO rdo;

        public Mps000518Processor(CommonParam param, PrintData printData)
            : base(param, printData)
        {
            rdo = (Mps000518PDO)rdoBase;
        }

        public override bool ProcessData()
        {
            bool result = false;
            try
            {
                Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                store.ReadTemplate(System.IO.Path.GetFullPath(fileName));

                SetSingleKey();

                singleTag.ProcessData(store, singleValueDictionary);
                objectTag.AddObjectData(store, "Mety", rdo.ListMety ?? new List<V_HIS_MEDI_CONTRACT_METY>());
                objectTag.AddObjectData(store, "Maty", rdo.ListMaty ?? new List<V_HIS_MEDI_CONTRACT_MATY>());

                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void SetSingleKey()
        {
            try
            {
                // Key đơn từ thông tin chung của hợp đồng (ưu tiên ghi đè trước).
                if (rdo.MedicalContact != null)
                {
                    AddObjectKeyIntoListkey<V_HIS_MEDICAL_CONTRACT>(rdo.MedicalContact);
                }

                // Key đơn từ nhà cung cấp — không ghi đè key đã có để giữ giá trị của hợp đồng nếu trùng tên.
                if (rdo.Supplier != null)
                {
                    AddObjectKeyIntoListkey<HIS_SUPPLIER>(rdo.Supplier, false);
                    SetSingleKey(new KeyValue(Mps000518ExtendSingleKey.AUTH_LETTER_ISSUE_DATE_STR,
                        ToDateVietnameseString(rdo.Supplier.AUTH_LETTER_ISSUE_DATE)));
                }

                SetSumContactPriceKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Tổng tiền = SUM(VIR_CONTACT_PRICE) của danh sách thuốc (METY) và vật tư (MATY).
        /// Sinh key SUM_CONTACT_PRICE (số) và SUM_CONTACT_PRICE_TEXT (chữ).
        /// </summary>
        private void SetSumContactPriceKey()
        {
            try
            {
                decimal sum = 0;
                if (rdo.ListMety != null)
                {
                    foreach (var item in rdo.ListMety)
                    {
                        if (item != null) sum += (item.VIR_CONTRACT_PRICE ?? 0);
                    }
                }
                if (rdo.ListMaty != null)
                {
                    foreach (var item in rdo.ListMaty)
                    {
                        if (item != null) sum += (item.VIR_CONTRACT_PRICE ?? 0);
                    }
                }

                SetSingleKey(new KeyValue(Mps000518ExtendSingleKey.SUM_CONTACT_PRICE,
                    Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sum)));

                string sumString = String.Format("{0:0.####}", Inventec.Common.Number.Convert.NumberToNumberRoundMax4(sum));
                SetSingleKey(new KeyValue(Mps000518ExtendSingleKey.SUM_CONTACT_PRICE_TEXT,
                    Inventec.Common.String.Convert.CurrencyToVneseString(sumString)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chuyển số thời gian yyyyMMddHHmmss (NUMBER 14) thành chuỗi "ngày dd tháng mm năm yyyy".
        /// Trả về rỗng nếu giá trị null hoặc không hợp lệ.
        /// </summary>
        private static string ToDateVietnameseString(long? timeNumber)
        {
            try
            {
                if (!timeNumber.HasValue || timeNumber.Value <= 0) return "";
                string s = timeNumber.Value.ToString();
                if (s.Length < 8) return "";
                string yyyy = s.Substring(0, 4);
                string mm = s.Substring(4, 2);
                string dd = s.Substring(6, 2);
                return String.Format("ngày {0} tháng {1} năm {2}", dd, mm, yyyy);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }
    }
}
