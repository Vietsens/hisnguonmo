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
using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestSaleCreate.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__IS_JOIN_NAME_WITH_CONCENTRA = "HIS.HIS_MEDI_STOCK.IS_JOIN_NAME_WITH_CONCENTRA";//Noi ten thuoc vs ham luong
        private const string CONFIG_KEY__MANAGE_PATIENT_IN_SALE = "MOS.HIS_EXP_MEST.MANAGE_PATIENT_IN_SALE";

        private const string CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING = "HIS.Desktop.Plugins.ExpMestSale.MustBeFinishedBeforePrinting";
        private const string CONFIG_KEY__IS_AUTO_SELECT_ACCOUNT_BOOK_IF_HAS_ONE = "HIS.Desktop.Plugins.TransactionBill.AutoSelectAccountBookIfHasOne";

        private const string CONFIG_KEY__IS_ROUND_PRICE_BASE = "HIS.Desktop.Plugins.Transaction.RoundPriceBase";

        internal static bool IS_JOIN_NAME_WITH_CONCENTRA;
        internal static bool IS_MUST_BE_FINISHED_BEFORED_PRINTING;
        internal static bool IS_MANAGE_PATIENT_IN_SALE;
        internal static bool IS_AUTO_SELECT_ACCOUNT_BOOK_IF_HAS_ONE;
        internal static string IS_ROUND_PRICE_BASE;

        internal static void LoadConfig()
        {
            try
            {
                IS_JOIN_NAME_WITH_CONCENTRA = GetValue(CONFIG_KEY__IS_JOIN_NAME_WITH_CONCENTRA) == "1";
                IS_MUST_BE_FINISHED_BEFORED_PRINTING = GetValue(CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING) == "1";
                IS_MANAGE_PATIENT_IN_SALE = GetValue(CONFIG_KEY__MANAGE_PATIENT_IN_SALE) == "1";
                IS_AUTO_SELECT_ACCOUNT_BOOK_IF_HAS_ONE = GetValue(CONFIG_KEY__IS_AUTO_SELECT_ACCOUNT_BOOK_IF_HAS_ONE) == "1";
                IS_ROUND_PRICE_BASE = GetValue(CONFIG_KEY__IS_ROUND_PRICE_BASE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

    }
}
