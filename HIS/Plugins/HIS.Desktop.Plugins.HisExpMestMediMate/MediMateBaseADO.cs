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

namespace HIS.Desktop.Plugins.HisExpMestMediMate
{
    class MediMateBaseADO
    {
        public long STT_ID { get; set; }
        public string STT_NAME { get; set; }
        public long? TIME { get; set; }
        public long MEST_ID { get; set; }
        public string MEST_CODE { get; set; }
        public string MEST_TYPE { get; set; }
        public long IMP_MEST_TYPE_ID { get; set; }
        public long EXP_MEST_TYPE_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal? PRICE { get; set; }
        public string MEDI_STOCK_PERIOD_NAME { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string IMP_MEDI_STOCK_NAME { get; set; }
        public string EXP_MEDI_STOCK_NAME { get; set; }
        public string REQ_DEPARTMENT_NAME { get; set; }
        public bool IsExp { get; set; }
        public string DOCUMENT_NUMBER { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string KEY_WORD { get; set; }
        public string MEDI_STOCK_NAME__STR { get; set; }
        public long? CREATE_TIME { get; set; }
        public long? TDL_INTRUCTION_TIME { get; set; }

        protected static string convertToUnSign3(string s)
        {
            if (String.IsNullOrWhiteSpace(s))
                return "";

            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }

        protected void BuildKeyWord(params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                KEY_WORD = string.Empty;
                return;
            }

            var sb = new StringBuilder();
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v)) continue;
                sb.Append(convertToUnSign3(v));
                sb.Append(v);
            }

            KEY_WORD = sb.ToString();
        }
    }
}
