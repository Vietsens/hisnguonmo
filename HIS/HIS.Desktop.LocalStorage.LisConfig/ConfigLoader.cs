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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.LisConfig
{
    public class ConfigLoader
    {
        const string configUri = "/api/LisConfig/Get";

        public static bool Refresh()
        {
            bool result = false;
            try
            {
                if (!CheckUserInventecLis())
                {
                    LogSystem.Info("Khong ket noi den he thong LIS noi bo");
                    return true;
                }
                CommonParam paramGet = new CommonParam();
                LisConfigFilter configFilter = new LisConfigFilter();
                configFilter.IS_ACTIVE = 1;
                var ro = new BackendAdapter(paramGet).Get<List<LIS_CONFIG>>(configUri, ApiConsumers.LisConsumer, configFilter, paramGet);

                if (ro != null && ro.Count > 0)
                {
                    foreach (var config in ro)
                    {
                        if (!String.IsNullOrWhiteSpace(config.KEY))
                        {
                            if (LisConfigs.dic.ContainsKey(config.KEY))
                            {
                                object outValue = null;
                                if (!LisConfigs.dic.TryRemove(config.KEY, out outValue))
                                {
                                    LogSystem.Info("Khong Remove duoc cau hinh trong dictionary Key: " + config.KEY.ToString());
                                    if (!LisConfigs.dic.TryUpdate(config.KEY, config, LisConfigs.dic[config.KEY]))
                                        LisConfigs.dic[config.KEY] = config;
                                }
                                else
                                {
                                    if (!LisConfigs.dic.TryAdd(config.KEY, config))
                                    {
                                        LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + config.KEY.ToString());
                                    }
                                }
                            }
                            else
                            {
                                if (!LisConfigs.dic.TryAdd(config.KEY, config))
                                {
                                    LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + config.KEY.ToString());
                                }
                            }
                        }
                        else
                        {
                            LogSystem.Warn("Key null." + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => config), config));
                        }
                    }
                    result = true;
                }
                else if (paramGet.HasException)
                {
                    LogSystem.Error("Query LisConfig co exception.");
                }
                else
                {
                    LogSystem.Warn("Khong co du lieu LisConfig & khong co exception.");
                    result = true;
                }
                if (result)
                {
                    LogSystem.Info("Load du lieu cau hinh LisConfig thanh cong.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private static bool CheckUserInventecLis()
        {
            try
            {
                string lisVersion = HisConfigs.Get<string>("MOS.LIS.INTEGRATION_VERSION");
                string lisIntegrateOption = HisConfigs.Get<string>("MOS.LIS.INTEGRATE_OPTION");
                string lisIntegrateType = HisConfigs.Get<string>("MOS.LIS.INTEGRATION_TYPE");

                if (lisVersion == "1" && lisIntegrateOption == "1")
                    return true;

                if (lisVersion == "2" && lisIntegrateType == "1")
                    return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }
    }
}
