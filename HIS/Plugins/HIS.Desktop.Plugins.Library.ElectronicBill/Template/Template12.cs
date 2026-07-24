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
    /// ProdName = "Thu phí (nhóm1: tiền; nhóm2: tiền;...) + Cùng chi trả (nhóm1: tiền; nhóm2: tiền;...)"
    /// Cách tính khớp Mps000512 (biến thể TOTAL_PATIENT_PRICE_LEFT):
    ///   - Thu phí      = bệnh nhân tự trả + nguồn khác = PRICE - cùng chi trả
    ///   - Cùng chi trả = bệnh nhân cùng chi trả BHYT   = TDL_TOTAL_PATIENT_PRICE_BHYT
    /// PRICE = VIR_TOTAL_PATIENT_PRICE = phần bệnh nhân phải trả (= tự trả + cùng chi trả + nguồn khác);
    /// quỹ BHYT chi trả (TDL_TOTAL_HEIN_PRICE) KHÔNG nằm trong PRICE nên không hiển thị / không cộng vào tổng.
    /// "nguồn khác" đã nằm sẵn trong PRICE (PRICE - cùng chi trả đã gồm nó) nên KHÔNG cộng TDL_OTHER_SOURCE_PRICE riêng.
    /// Nhóm loại dịch vụ được phân loại từ SereServBill theo cấu hình chi tiết (TemplateDetail).
    /// Đơn vị luôn "Lần", số lượng luôn 1, đơn giá = thành tiền = tổng tiền (= tổng PRICE phần bệnh nhân).
    /// </summary>
    class Template12 : IRunTemplate
    {
        private const string OTHER_GROUP = "Dịch vụ khác";

        private Base.ElectronicBillDataInput DataInput;

        public Template12(Base.ElectronicBillDataInput dataInput)
        {
            this.DataInput = dataInput;
        }

        public object Run()
        {
            // Vẫn trả về List<ProductBase> (chỉ chứa 1 phần tử) vì tất cả provider ép kiểu (List<ProductBase>)Run(). 
            List<ProductBase> result = new List<ProductBase>();
            try
            {
                if (DataInput.SereServBill != null && DataInput.SereServBill.Count > 0)
                {
                    // 1. Lấy cấu hình chi tiết để phân loại nhóm dịch vụ
                    List<TemplateDetailADO> classificationDetails = LoadClassificationDetails();

                    // 2. Duyệt từng dòng SereServBill -> gom "Thu phí" (tự trả + nguồn khác)
                    //    và "Cùng chi trả" (cùng chi trả BHYT) theo nhóm; tổng lấy trực tiếp từ PRICE.
                    List<string> orderedGroups = new List<string>();
                    Dictionary<string, decimal> thuPhiByGroup = new Dictionary<string, decimal>();
                    Dictionary<string, decimal> cungByGroup = new Dictionary<string, decimal>();
                    decimal total = 0;

                    foreach (var ss in DataInput.SereServBill)
                    {
                        string group = ClassifyGroup(ss, classificationDetails);

                        // Tính đồng nhất mọi dòng như Mps000512 (field = 0 với dòng không BHYT), không gate loại BN.
                        decimal cungChiTra = ss.TDL_TOTAL_PATIENT_PRICE_BHYT ?? 0;   // cùng chi trả BHYT
                        decimal thuPhi = Math.Max(0, ss.PRICE - cungChiTra);         // tự trả + nguồn khác (đã nằm trong PRICE)

                        total += ss.PRICE;                                           // tổng phần bệnh nhân = Sum(PRICE)

                        if (!thuPhiByGroup.ContainsKey(group))
                        {
                            orderedGroups.Add(group);
                            thuPhiByGroup[group] = 0;
                            cungByGroup[group] = 0;
                        }
                        thuPhiByGroup[group] += thuPhi;
                        cungByGroup[group] += cungChiTra;
                    }

                    // 3. Sắp xếp nhóm theo thứ tự cấu hình (NumOrder), "Dịch vụ khác" xuống cuối
                    Dictionary<string, long> groupOrder = new Dictionary<string, long>();
                    foreach (var detail in classificationDetails)
                    {
                        if (!String.IsNullOrWhiteSpace(detail.Display) && !groupOrder.ContainsKey(detail.Display))
                        {
                            groupOrder[detail.Display] = detail.NumOrder ?? 9999;
                        }
                    }
                    orderedGroups = orderedGroups
                        .OrderBy(g => groupOrder.ContainsKey(g) ? groupOrder[g] : long.MaxValue)
                        .ToList();

                    // 4. Dựng ProdName
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
                        prodName.Append(String.Format("Cùng chi trả ({0})", String.Join("; ", cungParts)));
                    }

                    // 5. Tạo 1 dòng tổng hợp
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
        /// Lấy các chi tiết cấu hình dùng để phân loại nhóm dịch vụ.
        /// Bỏ qua chi tiết đánh dấu BHYT (IsBHYT) vì Template12 tự tính phần BHYT theo nhóm,
        /// chỉ giữ chi tiết có tiêu chí phân loại loại dịch vụ và đúng loại điều trị.
        /// </summary>
        private List<TemplateDetailADO> LoadClassificationDetails()
        {
            List<TemplateDetailADO> result = new List<TemplateDetailADO>();

            string templateDetailStr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.TemplateDetail);
            List<TemplateDetailADO> dataDetail = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TemplateDetailADO>>(templateDetailStr);
            if (dataDetail == null || dataDetail.Count == 0)
                return result;

            dataDetail = dataDetail.OrderBy(o => o.NumOrder ?? 9999).ToList();
            foreach (var detail in dataDetail)
            {
                // Chỉ dùng chi tiết phân loại loại dịch vụ, bỏ dòng đánh dấu BHYT
                if (detail.IsBHYT == 1)
                    continue;

                bool hasClassification =
                    !String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes) ||
                    !String.IsNullOrWhiteSpace(detail.ServiceTypeCodes) ||
                    !String.IsNullOrWhiteSpace(detail.ServiceCodes) ||
                    !String.IsNullOrWhiteSpace(detail.ParentServiceCodes) ||
                    !String.IsNullOrWhiteSpace(detail.PatientTypeCodes);
                if (!hasClassification)
                    continue;

                // Gate theo loại điều trị (giống Template8)
                if (!String.IsNullOrWhiteSpace(detail.TreatmentTypeCodes))
                {
                    List<string> treatmentTypeCodes = detail.TreatmentTypeCodes.Split('|').ToList();
                    detail.TreatmentTypeIds = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => treatmentTypeCodes.Contains(o.TREATMENT_TYPE_CODE)).Select(s => s.ID).ToList();
                    if (detail.TreatmentTypeIds != null && detail.TreatmentTypeIds.Count > 0
                        && (DataInput.Treatment == null || !detail.TreatmentTypeIds.Contains(DataInput.Treatment.TDL_TREATMENT_TYPE_ID ?? 0)))
                    {
                        continue;
                    }
                }

                // Resolve các bộ Id để so khớp
                if (!String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes))
                {
                    List<string> heinServiceTypeCodes = detail.HeinServiceTypeCodes.Split('|').ToList();
                    detail.HeinServiceTypeIds = BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>().Where(o => heinServiceTypeCodes.Contains(o.HEIN_SERVICE_TYPE_CODE)).Select(s => s.ID).ToList();
                }
                if (!String.IsNullOrWhiteSpace(detail.ServiceTypeCodes))
                {
                    List<string> serviceTypeCodes = detail.ServiceTypeCodes.Split('|').ToList();
                    detail.ServiceTypeIds = BackendDataWorker.Get<HIS_SERVICE_TYPE>().Where(o => serviceTypeCodes.Contains(o.SERVICE_TYPE_CODE)).Select(s => s.ID).ToList();
                }
                if (!String.IsNullOrWhiteSpace(detail.ParentServiceCodes))
                {
                    List<string> parentServiceCodes = detail.ParentServiceCodes.Split('|').ToList();
                    detail.ParentServiceIds = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => parentServiceCodes.Contains(o.SERVICE_CODE)).Select(s => s.ID).ToList();
                }
                if (!String.IsNullOrWhiteSpace(detail.PatientTypeCodes))
                {
                    List<string> patientTypeCodes = detail.PatientTypeCodes.Split('|').ToList();
                    detail.PatientTypeIds = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => patientTypeCodes.Contains(o.PATIENT_TYPE_CODE)).Select(s => s.ID).ToList();
                }

                result.Add(detail);
            }

            return result;
        }

        /// <summary>
        /// Trả về tên nhóm (Display) của chi tiết cấu hình khớp đầu tiên với dòng SereServBill.
        /// Không khớp chi tiết nào -> "Dịch vụ khác".
        /// </summary>
        private string ClassifyGroup(HIS_SERE_SERV_BILL ss, List<TemplateDetailADO> classificationDetails)
        {
            foreach (var detail in classificationDetails)
            {
                if (IsMatch(ss, detail))
                {
                    return !String.IsNullOrWhiteSpace(detail.Display) ? detail.Display : OTHER_GROUP;
                }
            }
            return OTHER_GROUP;
        }

        /// <summary>
        /// Dòng SereServBill khớp chi tiết khi thỏa TẤT CẢ tiêu chí được khai báo (giống chuỗi lọc ở Template8).
        /// </summary>
        private bool IsMatch(HIS_SERE_SERV_BILL ss, TemplateDetailADO detail)
        {
            if (!String.IsNullOrWhiteSpace(detail.PatientTypeCodes))
            {
                if (detail.PatientTypeIds == null ||
                    !(detail.PatientTypeIds.Contains(ss.TDL_PATIENT_TYPE_ID ?? 0) || detail.PatientTypeIds.Contains(ss.TDL_PRIMARY_PATIENT_TYPE_ID ?? 0)))
                    return false;
            }

            if (!String.IsNullOrWhiteSpace(detail.HeinServiceTypeCodes))
            {
                if (detail.HeinServiceTypeIds == null || !detail.HeinServiceTypeIds.Contains(ss.TDL_HEIN_SERVICE_TYPE_ID ?? 0))
                    return false;
            }

            if (!String.IsNullOrWhiteSpace(detail.ServiceTypeCodes))
            {
                if (detail.ServiceTypeIds == null || !detail.ServiceTypeIds.Contains(ss.TDL_SERVICE_TYPE_ID ?? 0))
                    return false;
            }

            if (!String.IsNullOrWhiteSpace(detail.ParentServiceCodes))
            {
                List<long> serviceIds = detail.ParentServiceIds != null
                    ? BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => detail.ParentServiceIds.Contains(o.PARENT_ID ?? 0)).Select(s => s.ID).ToList()
                    : new List<long>();
                if (!serviceIds.Contains(ss.TDL_SERVICE_ID ?? 0))
                    return false;
            }

            if (!String.IsNullOrWhiteSpace(detail.ServiceCodes))
            {
                List<string> serviceCodes = detail.ServiceCodes.Split('|').ToList();
                if (!serviceCodes.Contains(ss.TDL_SERVICE_CODE))
                    return false;
            }

            return true;
        }
    }
}
