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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestOtherExport.Config
{
    class HisConfigCFG
    {
        private const string EXPORT_OPTION_CFG = "MOS.HIS_MEDI_STOCK.EXPORT_OPTION";
        private const string REASON_CFG = "DBCODE.HIS_RS.HIS_EXP_MEST_REASON.THANH_LY";


        internal static long HisExpMestReasonId__ThanhLy;
        internal static int ExportOption;

        internal static void LoadConfig()
        {
            try
            {
                HisExpMestReasonId__ThanhLy = GetId(HisConfigs.Get<string>(REASON_CFG));
                ExportOption = HisConfigs.Get<int>(EXPORT_OPTION_CFG);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static long GetId(string expMestReasonCode)
        {
            long result = 0;
            try
            {
                if (String.IsNullOrEmpty(expMestReasonCode)) throw new NullReferenceException("Code");
                var transactionType = BackendDataWorker.Get<HIS_EXP_MEST_REASON>().FirstOrDefault(o => o.EXP_MEST_REASON_CODE == expMestReasonCode);
                if (transactionType != null)
                {
                    result = transactionType.ID;
                }
                if (result == 0) throw new NullReferenceException(expMestReasonCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }
    }
}
