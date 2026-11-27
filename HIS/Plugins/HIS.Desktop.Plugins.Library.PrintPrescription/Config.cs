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

namespace HIS.Desktop.Plugins.Library.PrintPrescription
{
    class Config
    {
        internal const string mps = "HIS.Desktop.Plugins.Library.PrintPrescription.Mps";

        internal const string CONFIG_KEY__PRINT_BARCODE_NO_ZERO = "HIS.Desktop.Library.Print.BacodeNoZero";

        internal const string TAKE_INFO_TT_CLS = "HIS.DESKTOP.PRINT_PRESCRIPTION.TAKE_INFO_TT_CLS";

        internal const string KEY_IsPrintPrescriptionNoThread = "HIS.Desktop.IsPrintPrescriptionNoThread";

        internal const string ASSIGN_PRESCRIPTION_ODER_OPTION = "HIS.Desktop.Plugins.AssignPrescription.OderOption";
   
        internal static bool IsmergePrint
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.OptionMergePrint.Ismerge") == "1";
            }
        }

        internal const string CONFIG_KEY__IsAllowOddConvertUnitAmount = "HIS.Desktop.Plugins.AssignPrescription.IsAllowOddConvertUnitAmount";

        /// <summary>
        /// #48132 - 1: Cho phep ke le theo don vi quy doi; so luong quy doi in ra duoc lam tron de dung so bac si da ke
        /// </summary>
        internal static bool IsAllowOddConvertUnitAmount
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsAllowOddConvertUnitAmount) == "1";
            }
        }

        /// <summary>
        /// #48132: so luong don vi goc * ty le; bat cau hinh -> lam tron (6 - so chu so phan nguyen cua ty le) chu so thap phan
        /// </summary>
        internal static decimal? ToConvertAmount(decimal? originalAmount, decimal convertRatio)
        {
            if (!originalAmount.HasValue)
                return null;
            decimal result = originalAmount.Value * convertRatio;
            if (IsAllowOddConvertUnitAmount)
            {
                decimal intPart = Math.Floor(Math.Abs(convertRatio));
                int intDigits = intPart >= 1 ? intPart.ToString("0", System.Globalization.CultureInfo.InvariantCulture).Length : 0;
                int digits = 6 - intDigits;
                result = Math.Round(result, digits < 0 ? 0 : digits, MidpointRounding.AwayFromZero);
            }
            return result;
        }
    }
}
