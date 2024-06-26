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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AggrExpMestPrintFilter
{
    internal class AppConfigKeys
    {
        #region Public key

        internal const string CONFIG_KEY__HIS_DESKTOP__CHE_DO_IN_GOP_PHIEU_LINH = "CONFIG_KEY__CHE_DO_IN_GOP_PHIEU_LINH";
        internal const string CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN = "CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN";

        internal const string CONFIG_KEY__HIS_PHIEU_TRA_DOI_THUOC_COLUMN_SIZE = "HIS.PHIEU_TRA_DOI_THUOC.COLUMN_SIZE";

        #endregion

        internal static long PatientTypeId__BHYT
        {
            get
            {
                return GetPatientTypeBhyt();
            }
        }

        internal static bool IsmergePrint
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.OptionMergePrint.Ismerge") == "1";
            }
        }

        private static long GetPatientTypeBhyt()
        {
            long result = 0;
            try
            {
                string code = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                if (!String.IsNullOrWhiteSpace(code))
                {
                    var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE.ToLower() == code.ToLower().Trim());
                    if (patientType != null)
                    {
                        result = patientType.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                result = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal static List<string> ListParentMedicine
        {
            get
            {
                return ProcessListParentConfig();
            }
        }

        private static List<string> ProcessListParentConfig()
        {
            List<string> result = null;
            try
            {
                string code = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.AggrExpMestPrint.ParentMety");
                if (!String.IsNullOrWhiteSpace(code))
                {
                    result = code.Split(',').ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
