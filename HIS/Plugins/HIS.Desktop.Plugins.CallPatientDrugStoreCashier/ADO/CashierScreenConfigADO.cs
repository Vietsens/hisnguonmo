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

namespace HIS.Desktop.Plugins.CallPatientDrugStoreCashier.ADO
{
    /// <summary>
    /// Settings of the pharmacy cashier waiting screen, saved per user as JSON via ControlStateWorker
    /// </summary>
    public class CashierScreenConfigADO
    {
        public const int BILL_FILTER__ALL = 0;
        public const int BILL_FILTER__NOT_BILLED = 1;
        public const int BILL_FILTER__BILLED = 2;

        public int BillFilter { get; set; }
        public bool IsHideAmount { get; set; }

        public int TitleFontSize { get; set; }
        public string TitleColor { get; set; }

        public int ListFontSize { get; set; }
        public string ListColor { get; set; }

        public int CallingFontSize { get; set; }
        public string CallingColor { get; set; }
        public string CallingBackColor { get; set; }

        /// <summary>
        /// Empty means the default invitation text from Lang.*.resx
        /// </summary>
        public string InviteText { get; set; }

        public static CashierScreenConfigADO CreateDefault()
        {
            CashierScreenConfigADO ado = new CashierScreenConfigADO();
            ado.BillFilter = BILL_FILTER__ALL;
            ado.IsHideAmount = false;
            ado.TitleFontSize = 24;
            ado.TitleColor = "#13804A";
            ado.ListFontSize = 18;
            ado.ListColor = "#1F2D3D";
            ado.CallingFontSize = 20;
            ado.CallingColor = "#FFFFFF";
            ado.CallingBackColor = "#F26B21";
            return ado;
        }

        /// <summary>
        /// Fill default values for missing fields (old saved settings or cleared by user)
        /// </summary>
        public void FillDefault()
        {
            CashierScreenConfigADO def = CreateDefault();
            if (this.TitleFontSize <= 0) this.TitleFontSize = def.TitleFontSize;
            if (this.ListFontSize <= 0) this.ListFontSize = def.ListFontSize;
            if (this.CallingFontSize <= 0) this.CallingFontSize = def.CallingFontSize;
            if (String.IsNullOrWhiteSpace(this.TitleColor)) this.TitleColor = def.TitleColor;
            if (String.IsNullOrWhiteSpace(this.ListColor)) this.ListColor = def.ListColor;
            if (String.IsNullOrWhiteSpace(this.CallingColor)) this.CallingColor = def.CallingColor;
            if (String.IsNullOrWhiteSpace(this.CallingBackColor)) this.CallingBackColor = def.CallingBackColor;
            if (this.BillFilter < BILL_FILTER__ALL || this.BillFilter > BILL_FILTER__BILLED) this.BillFilter = BILL_FILTER__ALL;
        }
    }
}
