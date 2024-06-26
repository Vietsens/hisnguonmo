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
using HIS.Desktop.LocalStorage.ConfigApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Config
{
    internal class AppConfigs
    {
        public static long CheDoTuDongCheckThongTinTheBHYT { get; set; }
        public static int SocialInsuranceType { get; set; }

        private const string CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO = "CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO";
        private const string CONFIG_KEY__HIS_DESKTOP__REGISTER__SOCIAL_INSURANCE_TYPE = "CONFIG_KEY__HIS_DESKTOP__REGISTER__SOCIAL_INSURANCE_TYPE";

        public static void LoadConfig()
        {
            try
            {
                SocialInsuranceType = ConfigApplicationWorker.Get<int>(CONFIG_KEY__HIS_DESKTOP__REGISTER__SOCIAL_INSURANCE_TYPE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                CheDoTuDongCheckThongTinTheBHYT = ConfigApplicationWorker.Get<long>(CONFIG_KEY__HIS_DESKTOP__PLUGINS_AUTO_CHECK_HEIN_DATE_TO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
