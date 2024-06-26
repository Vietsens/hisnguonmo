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

namespace HIS.Desktop.LocalStorage.SdaConfigKey.Config
{
    public class HisPatientInfoForChildCFG
    {
        private const string MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD = "MOS.HIS_PATIENT.MUST_HAVE_NCS_INFO_FOR_CHILD";
        private const string IsNotInfo = "0";

        private static bool? mustHaveNCSInfoForChild;
        public static bool MustHaveNCSInfoForChild
        {
            get
            {
                if (!mustHaveNCSInfoForChild.HasValue)
                {
                    mustHaveNCSInfoForChild = Get(SdaConfigs.Get<string>(MOS__HIS_PATIENT__MUST_HAVE_NCS_INFO_FOR_CHILD));
                }
                return mustHaveNCSInfoForChild.Value;
            }
        }

        static bool Get(string code)
        {
            bool result = true;
            try
            {
                if (!String.IsNullOrEmpty(code))
                {
                    result = !(code == IsNotInfo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }
    }
}
