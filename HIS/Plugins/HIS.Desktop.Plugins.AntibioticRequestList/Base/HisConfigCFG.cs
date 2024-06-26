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

namespace HIS.Desktop.Plugins.AntibioticRequestList.Base
{
    class HisConfigCFG
    {
        /// <summary>
        /// Phải thanh toán mới cho phép thực xuất phiếu xuất bán
        /// </summary>
        private const string CONFIG_KEY__EXP_MEST_SALE__MUST_BILL = "MOS.EXP_MEST.EXPORT_SALE.MUST_BILL";
        private const string CONFIG_KEY__ALLOW_THER_LOGINNAME = "HIS.HIS_TRANSACTION.TRANSACTION_CANCEL.ALLOW_OTHER_LOGINNAME";
        private const string EXP_MEST_SALE__MODULE_UPDATE_OPTION = "HIS.Desktop.Plugins.ExpMestSale.ModuleUpdateOption";
        private const string CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE = "MOS.HIS_EXP_MEST.MUST_CONFIRM_BEFORE_APPROVE.OPTION";
        private const string CONFIG_KEY__WARM_MODIFIEDPRESCRIPTIONOPTION = "HIS.Desktop.Plugins.WarningModifiedPrescriptionOption";


        internal static bool EXPORT_SALE__MUST_BILL;
        internal static bool CANCEL_ALLOW_OTHER_LOGINNAME;
        internal static string EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT;
        internal static string MUST_CONFIRM_BEFORE_APPROVE;
        internal static string WARM_MODIFIEDPRESCRIPTIONOPTION;

        internal static void LoadConfig()
        {
            try
            {
                EXPORT_SALE__MUST_BILL = HisConfigs.Get<string>(CONFIG_KEY__EXP_MEST_SALE__MUST_BILL) == "1";
                CANCEL_ALLOW_OTHER_LOGINNAME = HisConfigs.Get<string>(CONFIG_KEY__ALLOW_THER_LOGINNAME) == "1";
                EXP_MEST_SALE__MODULE_UPDATE_OPTION_SELECT = HisConfigs.Get<string>(EXP_MEST_SALE__MODULE_UPDATE_OPTION);
                MUST_CONFIRM_BEFORE_APPROVE = HisConfigs.Get<string>(CONFIG_KEY__MUST_CONFIRM_BEFORE_APPROVE);
                WARM_MODIFIEDPRESCRIPTIONOPTION = HisConfigs.Get<string>(CONFIG_KEY__WARM_MODIFIEDPRESCRIPTIONOPTION);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
