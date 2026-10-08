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
using System;

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier.ADO
{
    /// <summary>
    /// One row of the cashier waiting screen grid (one sale export)
    /// </summary>
    public class CashierRowADO
    {
        public long ID { get; set; }
        public long SORT_NUMBER { get; set; }
        public string STT_DISPLAY { get; set; }
        public string PATIENT_NAME { get; set; }
        public string DOB_YEAR { get; set; }
        public decimal? AMOUNT { get; set; }
        public bool IS_BILLED { get; set; }

        public CashierRowADO(HIS_EXP_MEST expMest)
        {
            this.ID = expMest.ID;
            // Queue number is the prescription number generated at the sale point
            long? stt = expMest.PRES_NUMBER ?? expMest.NUM_ORDER;
            this.SORT_NUMBER = stt ?? long.MaxValue;
            this.STT_DISPLAY = stt.HasValue ? stt.Value.ToString() : "";
            this.PATIENT_NAME = expMest.TDL_PATIENT_NAME;
            this.DOB_YEAR = GetYear(expMest.TDL_PATIENT_DOB);
            this.AMOUNT = expMest.TDL_TOTAL_PRICE;
            this.IS_BILLED = expMest.BILL_ID.HasValue;
        }

        internal static string GetYear(long? dob)
        {
            string dobStr = dob.HasValue && dob.Value > 0 ? dob.Value.ToString() : "";
            return dobStr.Length >= 4 ? dobStr.Substring(0, 4) : "";
        }
    }
}
