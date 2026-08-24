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
using System.Text;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.ADO
{
    /// <summary>
    /// Mot dong ket qua tra cuu = mot lan nhap cua 1 loai thuoc/vat tu trong 1 phieu nhap.
    /// Cac truong dung chung cho ca thuoc va vat tu.
    /// </summary>
    class ImpMediMateBaseADO
    {
        /// <summary>So thu tu dong tren luoi</summary>
        public long STT { get; set; }

        /// <summary>Id phieu nhap</summary>
        public long IMP_MEST_ID { get; set; }

        /// <summary>So phieu (ma nhap)</summary>
        public string IMP_MEST_CODE { get; set; }

        /// <summary>Ngay thuc nhap (yyyyMMddHHmmss)</summary>
        public long? IMP_TIME { get; set; }

        /// <summary>So hoa don</summary>
        public string DOCUMENT_NUMBER { get; set; }

        /// <summary>Ngay hoa don (yyyyMMddHHmmss) - ghep tu thong tin phieu nhap</summary>
        public long? DOCUMENT_DATE { get; set; }

        /// <summary>Ma loai thuoc/vat tu</summary>
        public string TYPE_CODE { get; set; }

        /// <summary>Ten loai thuoc/vat tu (da ghep ham luong neu co)</summary>
        public string TYPE_NAME { get; set; }

        /// <summary>Don vi tinh</summary>
        public string SERVICE_UNIT_NAME { get; set; }

        /// <summary>So luong nhap theo don vi tinh cua thuoc/vat tu</summary>
        public decimal AMOUNT { get; set; }

        /// <summary>Nha cung cap</summary>
        public string SUPPLIER_NAME { get; set; }

        /// <summary>Id kho nhap</summary>
        public long MEDI_STOCK_ID { get; set; }

        /// <summary>Ten kho nhap - ghep tu danh muc kho</summary>
        public string MEDI_STOCK_NAME { get; set; }

        /// <summary>Nguon nhap - ghep tu lo thuoc/vat tu + danh muc nguon nhap</summary>
        public string IMP_SOURCE_NAME { get; set; }

        /// <summary>Hang san xuat</summary>
        public string MANUFACTURER_NAME { get; set; }

        /// <summary>Nuoc san xuat</summary>
        public string NATIONAL_NAME { get; set; }

        /// <summary>Don gia nhap thuc te</summary>
        public decimal IMP_PRICE { get; set; }

        /// <summary>Ty le VAT nhap (%)</summary>
        public decimal IMP_VAT_RATIO { get; set; }

        /// <summary>Thanh tien = So luong x Don gia</summary>
        public decimal TOTAL_PRICE
        {
            get { return this.AMOUNT * this.IMP_PRICE; }
        }

        /// <summary>Don gia sau VAT</summary>
        public decimal IMP_PRICE_VAT
        {
            get { return this.IMP_PRICE * (1 + this.IMP_VAT_RATIO / 100); }
        }

        /// <summary>Thanh tien sau VAT</summary>
        public decimal TOTAL_PRICE_VAT
        {
            get { return this.TOTAL_PRICE * (1 + this.IMP_VAT_RATIO / 100); }
        }

        /// <summary>Ngay thuc nhap dang chuoi de hien thi tren luoi/Excel</summary>
        public string IMP_TIME_STR
        {
            get
            {
                return (this.IMP_TIME.HasValue && this.IMP_TIME.Value > 0)
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.IMP_TIME.Value)
                    : "";
            }
        }

        /// <summary>Ngay hoa don dang chuoi de hien thi tren luoi/Excel</summary>
        public string DOCUMENT_DATE_STR
        {
            get
            {
                return (this.DOCUMENT_DATE.HasValue && this.DOCUMENT_DATE.Value > 0)
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.DOCUMENT_DATE.Value)
                    : "";
            }
        }

        /// <summary>Chuoi phuc vu loc nhanh tren luoi (co dau + khong dau)</summary>
        public string KEY_WORD { get; set; }

        internal static string ConvertToUnSign(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('đ', 'd').Replace('Đ', 'D');
        }

        protected void BuildKeyWord(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                this.KEY_WORD = string.Empty;
                return;
            }

            var sb = new StringBuilder();
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v)) continue;
                sb.Append(ConvertToUnSign(v));
                sb.Append(v);
            }
            this.KEY_WORD = sb.ToString();
        }

        /// <summary>Ghep ten loai voi ham luong de hien thi 1 cot "Ten thuoc - ham luong"</summary>
        internal static string BuildDisplayName(string typeName, string concentra)
        {
            if (string.IsNullOrWhiteSpace(concentra))
                return typeName;
            if (string.IsNullOrWhiteSpace(typeName))
                return concentra;
            if (typeName.IndexOf(concentra, StringComparison.OrdinalIgnoreCase) >= 0)
                return typeName;
            return typeName + " " + concentra;
        }
    }
}
