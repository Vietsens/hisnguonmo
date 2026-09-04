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

namespace HIS.Desktop.Plugins.MedicineSaleBill
{
    class Config
    {
        private const string mps = "HIS.Desktop.Plugins.MedicineSaleBill.PrintNow";

        internal static string PrintNowMps
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(mps);
            }
        }

        private const string CtrlKey = "HIS.Desktop.Plugins.MedicineSaleBill.IsUsingFunctionKeyInsteadOfCtrlKey";

        internal static bool IsUsingFunctionKey
        {
            get
            {
                return (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CtrlKey) == "1");
            }
        }

        private const string SaveSignPrintAutoExportKey = "HIS.Desktop.Plugins.MedicineSaleBill.SaveSignPrintAutoExport";

        /// <summary>
        /// = 1: hien checkbox "In" — tick thi nut Luu ky: kiem tra ton -> tao bill + phat hanh HDDT
        /// -> tu dong duyet/thuc xuat phieu (neu chua hoan thanh) -> in thang hoa don (viec 3082).
        /// Man Xuat ban (ExpMestSaleCreate) cung dung key nay de hien checkbox "In" canh nut Luu in.
        /// </summary>
        internal static bool IsSaveSignPrintAutoExport
        {
            get
            {
                return (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SaveSignPrintAutoExportKey) == "1");
            }
        }

        /// <summary>
        /// Viec 3082: marker man Xuat ban truyen vao args (List&lt;string&gt;) de form tu chay
        /// Luu ky + duyet/thuc xuat + in hoa don roi tu dong (nut "Luu in" + tick "In" tai man Xuat ban).
        /// Chuoi nay phai trung voi hang so ben HIS.Desktop.Plugins.ExpMestSaleCreate.
        /// </summary>
        internal const string AUTO_ACTION__SAVE_SIGN_PRINT = "AUTO_SAVE_SIGN_PRINT";

        private const string AutoSelectAccountBookIfHasOneKey = "HIS.Desktop.Plugins.TransactionBill.AutoSelectAccountBookIfHasOne";

        /// <summary>
        /// = 1: neu thu ngan chi duoc gan dung 1 so thu chi thi tu dong chon so do ngay tu lan mo man dau tien
        /// (khong phai doi den khi user tu chon 1 lan). Dung chung key voi cac man
        /// TransactionBill / ExpMestSaleCreate / AdjustmentTransaction / EInvoiceCreate.
        /// </summary>
        internal static bool IsAutoSelectAccountBookIfHasOne
        {
            get
            {
                return (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AutoSelectAccountBookIfHasOneKey) == "1");
            }
        }

    }
}
