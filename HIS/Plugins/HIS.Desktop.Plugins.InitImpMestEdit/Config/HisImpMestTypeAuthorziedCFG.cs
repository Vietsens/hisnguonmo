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
using Inventec.Common.LocalStorage.SdaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InImpMestEdit.Config
{
    class HisImpMestTypeAuthorziedCFG
    {
        private const string CONFIG_KEY__IMP_MEST_TYPE__AUTHORIZED = "MOS.HIS_IMP_MEST.HIS_IMP_MEST_TYPE.AUTHORIZED";
        private const string IsAuthorized = "1";

        private static bool? impMestType_IsAuthorized;
        public static bool ImpMestType_IsAuthorized
        {
            get
            {
                if (!impMestType_IsAuthorized.HasValue)
                {
                    impMestType_IsAuthorized = Get(SdaConfigs.Get<string>(CONFIG_KEY__IMP_MEST_TYPE__AUTHORIZED));
                }
                return impMestType_IsAuthorized.Value;
            }
        }

        static bool Get(string code)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    result = (code == IsAuthorized);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
