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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Bordereau
{
    public partial class frmBordereau : FormBase
    {
        /// <summary>
        /// PTTK 2883 - muc 1.1: hien thi tong chi phi theo dieu kien loc dang hien thi tren grid.
        /// Goi SAU MOI LAN gan gridControlBordereau.DataSource (Tim kiem, loc keyword, load lan dau).
        /// - Phai thu     = SUM(VIR_TOTAL_PRICE) cac dich vu dang hien thi
        /// - Phai thu BN  = SUM(VIR_TOTAL_PATIENT_PRICE) cac dich vu dang hien thi
        /// - Da thu       = SUM(VIR_TOTAL_PATIENT_PRICE) cac dich vu da thanh toan (hien mau xanh tren danh sach)
        /// - Can thu them = SUM(VIR_TOTAL_PATIENT_PRICE) cac dich vu chua thanh toan (hien mau den tren danh sach)
        /// </summary>
        internal void LoadFilteredFeeSummary()
        {
            try
            {
                decimal filterTotalPrice = 0;
                decimal filterTotalPatientPrice = 0;
                decimal filterObtainedPrice = 0;
                decimal filterDepositPrice = 0;

                var sereServDisplays = gridControlBordereau.DataSource as List<SereServADO>;
                if (sereServDisplays != null && sereServDisplays.Count > 0)
                {
                    // Dong du tru mau (isAssignBlood) khong phai dich vu co chi phi -> loai khoi tong
                    // HashSet de tra cuu O(1) — dong bo voi logic to mau xanh o gridViewBordereau_RowCellStyle
                    HashSet<long> paidSereServIds = SereServBills != null
                        ? new HashSet<long>(SereServBills
                            .Where(o => o.IS_CANCEL == null || o.IS_CANCEL == 0)
                            .Select(o => o.SERE_SERV_ID))
                        : new HashSet<long>();

                    foreach (var sereServ in sereServDisplays)
                    {
                        if (sereServ.isAssignBlood)
                            continue;

                        filterTotalPrice += sereServ.VIR_TOTAL_PRICE ?? 0;
                        filterTotalPatientPrice += sereServ.VIR_TOTAL_PATIENT_PRICE ?? 0;

                        if (paidSereServIds.Contains(sereServ.ID))
                        {
                            filterObtainedPrice += sereServ.VIR_TOTAL_PATIENT_PRICE ?? 0;
                        }
                        else
                        {
                            filterDepositPrice += sereServ.VIR_TOTAL_PATIENT_PRICE ?? 0;
                        }
                    }
                }

                lblFilterTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(filterTotalPrice, ConfigApplications.NumberSeperator);
                lblFilterTotalPatientPrice.Text = Inventec.Common.Number.Convert.NumberToString(filterTotalPatientPrice, ConfigApplications.NumberSeperator);
                lblFilterTotalObtainedPrice.Text = Inventec.Common.Number.Convert.NumberToString(filterObtainedPrice, ConfigApplications.NumberSeperator);
                lblFilterTotalDepositPrice.Text = Inventec.Common.Number.Convert.NumberToString(filterDepositPrice, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
