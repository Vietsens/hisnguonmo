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
using System.Text;
using System.Threading.Tasks;

using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;

namespace MPS.Processor.Mps000200.ADO
{
    public class MedicineTypeAdo : V_HIS_MEDICINE_TYPE
    {
        public string PARENT_NAME { get; set; }
        public string SERVICE_UNIT_NAME_STR { get; set; }
        public Nullable<decimal> IMPORT_PRICE { get; set; }
        public Nullable<decimal> EXPORT_PRICE { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO_STR { get; set; }
        public string IS_NUTRITION_FOOD_STR { get; set; }
        public string CREATE_TIME_STR { get; set; }
        public string ACTIVE_INGREDIENT_CODES { get; set; }
        public string ACTIVE_INGREDIENT_NAMES { get; set; }
        public string ATC_NAMES { get; set; }
        public string SUPPLIER_NAMES { get; set; }
        public string CONTRAINDICATION_NAMES { get; set; }
        public string MEDICINE_KIND_STR { get; set; }
        public string BID_DECISION_STR { get; set; }
        public string BID_PACKAGE_STR { get; set; }
        public string BID_GROUP_STR { get; set; }
        public string BID_YEAR_STR { get; set; }

        private static readonly string[] IdSeparators = new string[] { ";", "," };

        public MedicineTypeAdo() { }

        public MedicineTypeAdo(V_HIS_MEDICINE_TYPE medicineType)
        {
            try
            {
                if (medicineType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeAdo>(this, medicineType);
                    if (medicineType.PARENT_ID.HasValue)
                    {
                        var rs = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == medicineType.PARENT_ID.Value);
                        if (rs != null)
                        {
                            this.PARENT_NAME = rs.MEDICINE_TYPE_NAME;
                        }
                    }

                    SetDisplayValues(medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MedicineTypeAdo(V_HIS_MEDICINE_TYPE medicineType, MedicineTypeLookup lookup)
        {
            try
            {
                if (medicineType != null)
                {
                    // Du lieu man hinh truyen sang co the thieu field (TT_THAU, SUPPLIER_IDS...)
                    // -> uu tien ban ghi day du tu cache danh muc theo ID
                    V_HIS_MEDICINE_TYPE source = medicineType;
                    if (lookup != null && lookup.ParentById != null)
                    {
                        V_HIS_MEDICINE_TYPE full;
                        if (lookup.ParentById.TryGetValue(medicineType.ID, out full))
                        {
                            source = full;
                        }
                    }

                    Inventec.Common.Mapper.DataObjectMapper.Map<MedicineTypeAdo>(this, source);
                    if (source.PARENT_ID.HasValue && lookup != null && lookup.ParentById != null)
                    {
                        V_HIS_MEDICINE_TYPE parent;
                        if (lookup.ParentById.TryGetValue(source.PARENT_ID.Value, out parent))
                        {
                            this.PARENT_NAME = parent.MEDICINE_TYPE_NAME;
                        }
                    }

                    SetDisplayValues(source);
                    SetLookupValues(source, lookup);
                    SetMedicineKind(source);
                    SetBidInfo(source);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Cac gia tri dien giai khop voi hien thi tren man hinh Danh sach loai thuoc
        private void SetDisplayValues(V_HIS_MEDICINE_TYPE medicineType)
        {
            this.SERVICE_UNIT_NAME_STR = medicineType.IMP_UNIT_ID.HasValue ? medicineType.IMP_UNIT_NAME : medicineType.SERVICE_UNIT_NAME;
            if (medicineType.LAST_IMP_VAT_RATIO != null)
            {
                if (medicineType.LAST_IMP_PRICE != null)
                {
                    this.IMPORT_PRICE = medicineType.LAST_IMP_PRICE * (1 + medicineType.LAST_IMP_VAT_RATIO);
                }
            }
            else
            {
                this.IMPORT_PRICE = 0;
            }
            if (medicineType.LAST_EXP_VAT_RATIO != null)
            {
                if (medicineType.LAST_EXP_PRICE != null)
                {
                    this.EXPORT_PRICE = medicineType.LAST_EXP_PRICE * (1 + medicineType.LAST_EXP_VAT_RATIO);
                }
            }
            else
            {
                this.EXPORT_PRICE = 0;
            }
            if (medicineType.HEIN_LIMIT_RATIO.HasValue)
            {
                this.HEIN_LIMIT_RATIO_STR = medicineType.HEIN_LIMIT_RATIO * 100;
            }
            this.IS_NUTRITION_FOOD_STR = (medicineType.IS_NUTRITION_FOOD == 1) ? "X" : "";
            this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(medicineType.CREATE_TIME ?? 0);
        }

        // Hoat chat noi bo, ten ATC, nha cung cap, truong hop chong chi dinh
        private void SetLookupValues(V_HIS_MEDICINE_TYPE medicineType, MedicineTypeLookup lookup)
        {
            if (lookup == null) return;

            if (lookup.AcinByMedicineTypeId != null)
            {
                var acins = lookup.AcinByMedicineTypeId[medicineType.ID].ToList();
                if (acins.Count > 0)
                {
                    this.ACTIVE_INGREDIENT_CODES = String.Join("; ", acins.Select(o => o.ACTIVE_INGREDIENT_CODE));
                    this.ACTIVE_INGREDIENT_NAMES = String.Join("; ", acins.Select(o => o.ACTIVE_INGREDIENT_NAME));
                }
            }

            if (!String.IsNullOrEmpty(medicineType.ATC_CODES) && lookup.AtcByCode != null)
            {
                List<string> atcNames = new List<string>();
                foreach (var code in medicineType.ATC_CODES.Split(IdSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    HIS_ATC atc;
                    if (lookup.AtcByCode.TryGetValue(code.Trim(), out atc))
                    {
                        atcNames.Add(atc.ATC_NAME);
                    }
                }
                this.ATC_NAMES = String.Join("; ", atcNames);
            }

            if (!String.IsNullOrEmpty(medicineType.SUPPLIER_IDS) && lookup.SupplierById != null)
            {
                List<string> supplierNames = new List<string>();
                foreach (var idStr in medicineType.SUPPLIER_IDS.Split(IdSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    long id;
                    HIS_SUPPLIER supplier;
                    if (long.TryParse(idStr.Trim(), out id) && lookup.SupplierById.TryGetValue(id, out supplier))
                    {
                        supplierNames.Add(supplier.SUPPLIER_NAME);
                    }
                }
                this.SUPPLIER_NAMES = String.Join("; ", supplierNames);
            }

            if (!String.IsNullOrEmpty(medicineType.CONTRAINDICATION_IDS) && lookup.ContraindicationById != null)
            {
                List<string> contraindicationNames = new List<string>();
                foreach (var idStr in medicineType.CONTRAINDICATION_IDS.Split(IdSeparators, StringSplitOptions.RemoveEmptyEntries))
                {
                    long id;
                    HIS_CONTRAINDICATION contraindication;
                    if (long.TryParse(idStr.Trim(), out id) && lookup.ContraindicationById.TryGetValue(id, out contraindication))
                    {
                        contraindicationNames.Add(contraindication.CONTRAINDICATION_NAME);
                    }
                }
                this.CONTRAINDICATION_NAMES = String.Join("; ", contraindicationNames);
            }
        }

        // Loai thuoc: ghep ten cac co phan loai dang bat
        private void SetMedicineKind(V_HIS_MEDICINE_TYPE medicineType)
        {
            List<string> kinds = new List<string>();
            if (medicineType.IS_CHEMICAL_SUBSTANCE == 1) kinds.Add("Hóa chất");
            if (medicineType.IS_FUNCTIONAL_FOOD == 1) kinds.Add("Sản phẩm không phải là thuốc");
            if (medicineType.IS_STAR_MARK == 1) kinds.Add("Thuốc dấu sao");
            if (medicineType.IS_GENERIC == 1) kinds.Add("Generic");
            if (medicineType.IS_VACCINE == 1) kinds.Add("Vaccine");
            if (medicineType.IS_VITAMIN_A == 1) kinds.Add("Vitamin A");
            if (medicineType.IS_TCMR == 1) kinds.Add("Tiêm chủng mở rộng");
            if (medicineType.IS_BIOLOGIC == 1) kinds.Add("Sinh phẩm");
            if (medicineType.IS_OXYGEN == 1) kinds.Add("Ô xy");
            if (medicineType.IS_ANAESTHESIA == 1) kinds.Add("Gây tê");
            if (medicineType.IS_ORIGINAL_BRAND_NAME == 1) kinds.Add("Biệt dược gốc");
            if (medicineType.IS_KIDNEY == 1) kinds.Add("Thuốc chạy thận");
            if (medicineType.IS_RAW_MEDICINE == 1) kinds.Add("Nguyên liệu điều chế");
            if (medicineType.IS_NUTRITION_FOOD == 1) kinds.Add("Thực phẩm dinh dưỡng");
            this.MEDICINE_KIND_STR = String.Join("; ", kinds);
        }

        // Tach thong tin thau: {SoQD};G{goi};N{nhom};{nam};... (vi du: KQ2400569074_2504150719;G1;N4;2025;64)
        private void SetBidInfo(V_HIS_MEDICINE_TYPE medicineType)
        {
            if (String.IsNullOrEmpty(medicineType.TT_THAU)) return;

            var parts = medicineType.TT_THAU.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
            {
                this.BID_DECISION_STR = parts[0].Trim();
            }
            for (int i = 1; i < parts.Length; i++)
            {
                string part = parts[i].Trim();
                long num;
                if (part.Length > 1 && (part[0] == 'G' || part[0] == 'g') && long.TryParse(part.Substring(1), out num))
                {
                    this.BID_PACKAGE_STR = part.Substring(1);
                }
                else if (part.Length > 1 && (part[0] == 'N' || part[0] == 'n') && long.TryParse(part.Substring(1), out num))
                {
                    this.BID_GROUP_STR = part.Substring(1);
                }
                else if (part.Length == 4 && long.TryParse(part, out num))
                {
                    this.BID_YEAR_STR = part;
                }
            }
        }
    }
}
