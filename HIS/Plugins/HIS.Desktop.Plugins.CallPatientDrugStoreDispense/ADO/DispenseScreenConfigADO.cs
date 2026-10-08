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

namespace HIS.Desktop.Plugins.CallPatientDrugStoreDispense.ADO
{
    /// <summary>
    /// Settings of the pharmacy dispensing waiting screen, saved per user as JSON via ControlStateWorker
    /// </summary>
    public class DispenseScreenConfigADO
    {
        /// <summary>
        /// Show exports in "Request" status (preparing)
        /// </summary>
        public bool IsShowPreparing { get; set; }

        /// <summary>
        /// Show exports in "Approved" status (waiting for dispensing)
        /// </summary>
        public bool IsShowWaiting { get; set; }

        /// <summary>
        /// Show exports in "Done" status (dispensed)
        /// </summary>
        public bool IsShowDone { get; set; }

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

        public static DispenseScreenConfigADO CreateDefault()
        {
            DispenseScreenConfigADO ado = new DispenseScreenConfigADO();
            ado.IsShowPreparing = true;
            ado.IsShowWaiting = true;
            ado.IsShowDone = true;
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
            DispenseScreenConfigADO def = CreateDefault();
            if (this.TitleFontSize <= 0) this.TitleFontSize = def.TitleFontSize;
            if (this.ListFontSize <= 0) this.ListFontSize = def.ListFontSize;
            if (this.CallingFontSize <= 0) this.CallingFontSize = def.CallingFontSize;
            if (String.IsNullOrWhiteSpace(this.TitleColor)) this.TitleColor = def.TitleColor;
            if (String.IsNullOrWhiteSpace(this.ListColor)) this.ListColor = def.ListColor;
            if (String.IsNullOrWhiteSpace(this.CallingColor)) this.CallingColor = def.CallingColor;
            if (String.IsNullOrWhiteSpace(this.CallingBackColor)) this.CallingBackColor = def.CallingBackColor;
            // No status selected means show all statuses
            if (!this.IsShowPreparing && !this.IsShowWaiting && !this.IsShowDone)
            {
                this.IsShowPreparing = true;
                this.IsShowWaiting = true;
                this.IsShowDone = true;
            }
        }
    }
}
