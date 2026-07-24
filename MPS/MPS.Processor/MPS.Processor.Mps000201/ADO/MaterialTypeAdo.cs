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

namespace MPS.Processor.Mps000201.ADO
{
    public class MaterialTypeAdo : V_HIS_MATERIAL_TYPE
    {
        public string SERVICE_UNIT_NAME_STR { get; set; }
        public Nullable<decimal> IMPORT_PRICE { get; set; }
        public Nullable<decimal> EXPORT_PRICE { get; set; }
        public string CREATE_TIME_STR { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO_STR { get; set; }
        public Nullable<decimal> HEIN_LIMIT_RATIO_OLD_STR { get; set; }
        public string IS_SIZE_REQUIRED_STR { get; set; }
        public string MATERIAL_KIND_STR { get; set; }
        public string FILM_SIZE_NAME { get; set; }
        public string SUPPLIER_NAMES { get; set; }
        public string BID_DECISION_STR { get; set; }
        public string BID_PACKAGE_STR { get; set; }
        public string BID_GROUP_STR { get; set; }
        public string BID_YEAR_STR { get; set; }

        private static readonly string[] IdSeparators = new string[] { ";", "," };

        public MaterialTypeAdo() { }

        public MaterialTypeAdo(V_HIS_MATERIAL_TYPE materialType)
        {
            try
            {
                if (materialType != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeAdo>(this, materialType);
                    SetDisplayValues(materialType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public MaterialTypeAdo(V_HIS_MATERIAL_TYPE materialType, MaterialTypeLookup lookup)
        {
            try
            {
                if (materialType != null)
                {
                    // Du lieu man hinh truyen sang co the thieu field (TT_THAU, SUPPLIER_IDS...)
                    // -> uu tien ban ghi day du tu cache danh muc theo ID
                    V_HIS_MATERIAL_TYPE source = materialType;
                    if (lookup != null && lookup.MaterialTypeById != null)
                    {
                        V_HIS_MATERIAL_TYPE full;
                        if (lookup.MaterialTypeById.TryGetValue(materialType.ID, out full))
                        {
                            source = full;
                        }
                    }

                    Inventec.Common.Mapper.DataObjectMapper.Map<MaterialTypeAdo>(this, source);
                    SetDisplayValues(source);
                    SetLookupValues(source, lookup);
                    SetMaterialKind(source);
                    SetBidInfo(source);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // Cac gia tri dien giai khop voi hien thi tren man hinh Danh sach loai vat tu
        private void SetDisplayValues(V_HIS_MATERIAL_TYPE materialType)
        {
            this.SERVICE_UNIT_NAME_STR = materialType.IMP_UNIT_ID.HasValue ? materialType.IMP_UNIT_NAME : materialType.SERVICE_UNIT_NAME;
            if (materialType.LAST_IMP_VAT_RATIO != null)
            {
                if (materialType.LAST_IMP_PRICE != null)
                {
                    this.IMPORT_PRICE = materialType.LAST_IMP_PRICE * (1 + materialType.LAST_IMP_VAT_RATIO);
                }
            }
            else
            {
                this.IMPORT_PRICE = 0;
            }
            if (materialType.LAST_EXP_VAT_RATIO != null)
            {
                if (materialType.LAST_EXP_PRICE != null)
                {
                    this.EXPORT_PRICE = materialType.LAST_EXP_PRICE * (1 + materialType.LAST_EXP_VAT_RATIO);
                }
            }
            else
            {
                this.EXPORT_PRICE = 0;
            }
            if (materialType.HEIN_LIMIT_RATIO.HasValue)
            {
                this.HEIN_LIMIT_RATIO_STR = materialType.HEIN_LIMIT_RATIO * 100;
            }
            if (materialType.HEIN_LIMIT_RATIO_OLD.HasValue)
            {
                this.HEIN_LIMIT_RATIO_OLD_STR = materialType.HEIN_LIMIT_RATIO_OLD * 100;
            }
            this.IS_SIZE_REQUIRED_STR = (materialType.IS_SIZE_REQUIRED == 1) ? "X" : "";
            this.CREATE_TIME_STR = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(materialType.CREATE_TIME ?? 0);
        }

        // Nha cung cap, kich thuoc phim
        private void SetLookupValues(V_HIS_MATERIAL_TYPE materialType, MaterialTypeLookup lookup)
        {
            if (lookup == null) return;

            if (!String.IsNullOrEmpty(materialType.SUPPLIER_IDS) && lookup.SupplierById != null)
            {
                List<string> supplierNames = new List<string>();
                foreach (var idStr in materialType.SUPPLIER_IDS.Split(IdSeparators, StringSplitOptions.RemoveEmptyEntries))
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

            if (materialType.FILM_SIZE_ID.HasValue && lookup.FilmSizeById != null)
            {
                HIS_FILM_SIZE filmSize;
                if (lookup.FilmSizeById.TryGetValue(materialType.FILM_SIZE_ID.Value, out filmSize))
                {
                    this.FILM_SIZE_NAME = filmSize.FILM_SIZE_NAME;
                }
            }
        }

        // Loai vat tu: ghep ten cac co phan loai dang bat
        private void SetMaterialKind(V_HIS_MATERIAL_TYPE materialType)
        {
            List<string> kinds = new List<string>();
            if (materialType.IS_CHEMICAL_SUBSTANCE == 1) kinds.Add("Hóa chất");
            if (materialType.IS_STENT == 1) kinds.Add("Stent");
            if (materialType.IS_RAW_MATERIAL == 1) kinds.Add("LNLBC");
            if (materialType.IS_CONSUMABLE == 1) kinds.Add("VTTH CTTM");
            if (materialType.IS_OUT_HOSPITAL == 1) kinds.Add("Vật tư ngoại viện");
            if (materialType.IS_IDENTITY_MANAGEMENT == 1) kinds.Add("VT đích danh");
            if (materialType.IS_REUSABLE == 1) kinds.Add("VT tái sử dụng");
            if (materialType.IS_FILM == 1) kinds.Add("Phim chụp");
            this.MATERIAL_KIND_STR = String.Join("; ", kinds);
        }

        // Tach thong tin thau: {SoQD};G{goi};N{nhom};{nam};... (vi du: KQ2400569074_2504150719;G1;N4;2025;64)
        private void SetBidInfo(V_HIS_MATERIAL_TYPE materialType)
        {
            if (String.IsNullOrEmpty(materialType.TT_THAU)) return;

            var parts = materialType.TT_THAU.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
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
