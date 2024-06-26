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

namespace HIS.Desktop.Plugins.ExpMestViewDetail.Config
{
    class HisConfigCFG
    {
        private const string CONFIG_KEY__IS_JOIN_NAME_WITH_CONCENTRA = "HIS.HIS_MEDI_STOCK.IS_JOIN_NAME_WITH_CONCENTRA";//Noi ten thuoc vs ham luong
        private const string CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING = "HIS.Desktop.Plugins.ExpMestSale.MustBeFinishedBeforePrinting";
        private const string CONFIG_KEY__WARNING_MODIFIED_PRESCRIPTION_OPTION = "HIS.Desktop.Plugins.WarningModifiedPrescriptionOption";
        private const string CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE = "MOS.HIS_EXP_MEST.MUST_CONFIRM_BEFORE_APPROVE.OPTION";
        private const string CONFIG_KEY__ODER_OPTION = "HIS.Desktop.Plugins.AggrExpMest.OderOption";


        internal static bool IS_MUST_BE_FINISHED_BEFORED_PRINTING;
        internal static bool IS_JOIN_NAME_WITH_CONCENTRA;
        internal static string MUST_CONFIRM_BEFORE_APPROVE;
        internal static string WARNING_MODIFIED_PRESCRIPTION_OPTION;
        internal static long ODER_OPTION;

        internal static void LoadConfig()
        {
            try
            {
                IS_JOIN_NAME_WITH_CONCENTRA = GetValue(CONFIG_KEY__IS_JOIN_NAME_WITH_CONCENTRA) == "1";
                IS_MUST_BE_FINISHED_BEFORED_PRINTING = GetValue(CONFIG_KEY__MUST_BE_FINISHED_BEFORED_PRINTING) == "1";
                MUST_CONFIRM_BEFORE_APPROVE = HisConfigs.Get<string>(CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE);
                WARNING_MODIFIED_PRESCRIPTION_OPTION = HisConfigs.Get<string>(CONFIG_KEY__WARNING_MODIFIED_PRESCRIPTION_OPTION);
                ODER_OPTION = Convert.ToInt16(HisConfigs.Get<string>(CONFIG_KEY__ODER_OPTION));

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
