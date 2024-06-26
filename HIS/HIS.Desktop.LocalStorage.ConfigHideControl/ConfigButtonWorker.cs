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
using HIS.Desktop.LocalStorage.ConfigButton.ADO;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.XmlConfig;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.ConfigButton
{
    public class ConfigButtonWorker
    {
        static readonly string APPLICATION_CODE = ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"];//Ma ung dung khai bao tren ACS
        static List<ConfigButtonADO> ConfigButtonADOs;
        static Object thisLock = new Object();

        public static void Init()
        {
            try
            {
                List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON> ConfigButtons;
                List<SDA.EFMODEL.DataModels.SDA_MODULE_BUTTON_USER> ConfigButtonUsers;
                CommonParam param = new CommonParam();
                ConfigButtons = SdaConfigButtonGet.Get();
                if (ConfigButtons == null || ConfigButtons.Count == 0) throw new ArgumentNullException("ConfigApps");

                ConfigButtonADOs = (from m in ConfigButtons select new ConfigButtonADO(m)).ToList();
                var confIds = ConfigButtons.Select(o => o.ID).ToList();
                ConfigButtonUsers = SdaConfigButtonUserGet.Get(confIds).Where(o => confIds.Contains(o.MODULE_BUTTON_ID)).ToList();
                if (ConfigButtonUsers != null && ConfigButtonUsers.Count > 0)
                {
                    foreach (var cfu in ConfigButtonUsers)
                    {
                        var dcfu = ConfigButtonADOs.FirstOrDefault(o => o.MODULE_BUTTON_ID == cfu.MODULE_BUTTON_ID);
                        if (dcfu != null && dcfu.ID > 0)
                        {
                            dcfu.IS_VISIBLE = cfu.IS_VISIBLE;
                            dcfu.NUM_ORDER = cfu.NUM_ORDER;
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("ConfigButtonWorker.Init. ConfigButtonADOs.count =" + (ConfigButtonADOs != null && ConfigButtonADOs.Count > 0 ? ConfigButtonADOs.Count : 0));
            }
            catch (ArgumentNullException ex)
            {
                ConfigButtonADOs = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static List<ConfigButtonADO> GetByModule(string moduleLink, string buttonGroupName)
        {
            List<ConfigButtonADO> data = default(List<ConfigButtonADO>);
            try
            {
                if (ConfigButtonADOs == null || ConfigButtonADOs.Count == 0) throw new ArgumentNullException("ConfigButtonADOs");

                var cfgButton = ConfigButtonADOs.AsQueryable();
                cfgButton = cfgButton.Where(o => o.MODULE_LINK == moduleLink);
                if (!String.IsNullOrEmpty(buttonGroupName))
                {
                    cfgButton = cfgButton.Where(o => o.BUTTON_GROUP_NAME == buttonGroupName);//TODO
                }

                return data = cfgButton.ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return default(List<ConfigButtonADO>);
        }

        public static bool ReloadAll()
        {
            bool result = false;
            try
            {
                ConfigButtonADOs.Clear();
                Init();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
    }
}
