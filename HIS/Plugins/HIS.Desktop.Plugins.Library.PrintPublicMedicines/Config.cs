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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintPublicMedicines
{
    class Config
    {
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        private const string CONFIG_KEY__PHIEU_CONG_KHAI_DICH_VU__DAY_SIZE = "HIS.PHIEU_CONG_KHAI_DICH_VU.DAY_SIZE";
        internal static long PatientTypeId__BHYT;
        internal static int congKhaiDichVu_DaySize;

        internal static void LoadConfig()
        {
            try
            {
                PatientTypeId__BHYT = GetPatientTypeByCode(HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).ID;
                congKhaiDichVu_DaySize = HisConfigs.Get<int>(CONFIG_KEY__PHIEU_CONG_KHAI_DICH_VU__DAY_SIZE) == 0 ? 10 : HisConfigs.Get<int>(CONFIG_KEY__PHIEU_CONG_KHAI_DICH_VU__DAY_SIZE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
    }
}
