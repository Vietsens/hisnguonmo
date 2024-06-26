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
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using SDA.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.SarUserReportTypeList
{
    public class ConfigApplicationWorker
    {
        internal const int DEFAULT_NUM_PAGESIZE = 20;
        internal const int DEFAULT_PRINT_PREVIEW = 1;
        static List<ConfigAppADO> ConfigAppADOs;
        static Dictionary<string, ConfigAppADO> dicConfig = new Dictionary<string, ConfigAppADO>();
        static Object thisLock = new Object();

        public static void Init()
        {
            try
            {
                List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP> ConfigApps;
                List<SDA.EFMODEL.DataModels.SDA_CONFIG_APP_USER> ConfigAppUsers;
                CommonParam param = new CommonParam();
                ConfigApps = SdaConfigAppGet.Get();
                if (ConfigApps == null || ConfigApps.Count == 0) throw new ArgumentNullException("ConfigApps");

                ConfigAppADOs = (from m in ConfigApps select new ConfigAppADO(m)).ToList();
                dicConfig = ConfigAppADOs.ToDictionary(o => o.KEY, o => o);

                var confIds = ConfigApps.Select(o => o.ID).ToList();
                ConfigAppUsers = SdaConfigAppUserGet.Get(null).Where(o => confIds.Contains(o.CONFIG_APP_ID)).ToList();
                if (ConfigAppUsers != null && ConfigAppUsers.Count > 0)
                {
                    Dictionary<long, SDA.EFMODEL.DataModels.SDA_CONFIG_APP> dicConfigApp = ConfigApps.ToDictionary(o => o.ID, o => o);

                    foreach (var cfu in ConfigAppUsers)
                    {
                        var dcfu = dicConfigApp[cfu.CONFIG_APP_ID];
                        if (dcfu != null && dcfu.ID > 0)
                        {
                            dicConfig[dcfu.KEY].VALUE = cfu.VALUE;
                        }
                    }
                }

                //Lấy tất cả các cấu hình có mã ứng dụng là HIS & các cấu hình không có mã ứng dụng (cấu hình chung)
                //Tiếp tục lọc các cấu hình này có trong cấu hình của người dùng (ConfigAppUser) thì mới lấy, không thì bỏ qua
                ConfigApplications.NumPageSize = Get<long>(KeyCFGs.CONFIG_KEY__NUM_PAGESIZE);
                if (ConfigApplications.NumPageSize == 0)
                    ConfigApplications.NumPageSize = DEFAULT_NUM_PAGESIZE;

                ConfigApplications.CheDoInChoCacChucNangTrongPhanMem = Get<long>(KeyCFGs.CONFIG_KEY__CHE_DO_IN_CHO_CAC_CHUC_NANG_TRONG_PHAN_MEM);
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 0)
                    ConfigApplications.CheDoInChoCacChucNangTrongPhanMem = DEFAULT_PRINT_PREVIEW;
                GlobalVariables.CheDoInChoCacChucNangTrongPhanMem = ConfigApplications.CheDoInChoCacChucNangTrongPhanMem;
                ConfigApplications.NumberSeperator = Get<int>(KeyCFGs.CONFIG_KEY__HIS_DESKTOP_COMMON_NUMBER_SEPERATE);
            }
            catch (ArgumentNullException ex)
            {
                dicConfig = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static T Get<T>(string key)
        {
            object data = default(T);
            try
            {
                if (dicConfig == null || dicConfig.Count == 0) throw new ArgumentNullException("dicConfig");

                object result = default(T);
                result = GetDataByType<T>(key);
                return (T)result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return default(T);
        }

        static object GetDataByType<T>(string key)
        {
            try
            {
                T t = default(T);

                if (typeof(T) == typeof(String))
                {
                    return (GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(short))
                {
                    return Inventec.Common.TypeConvert.Parse.ToInt16(GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(int))
                {
                    return Inventec.Common.TypeConvert.Parse.ToInt32(GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(long))
                {
                    return Inventec.Common.TypeConvert.Parse.ToInt64(GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(decimal))
                {
                    return Inventec.Common.TypeConvert.Parse.ToDecimal(GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(double))
                {
                    return Inventec.Common.TypeConvert.Parse.ToDouble(GetStringConfigApplication(key));
                }
                else if (typeof(T) == typeof(bool))
                {
                    return Inventec.Common.TypeConvert.Parse.ToBoolean(GetStringConfigApplication(key));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                LogSystem.Info("Get key fail. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key));
            }
            return default(T);
        }

        static string GetStringConfigApplication(string key)
        {
            try
            {
                var cf = dicConfig[key];
                return ((cf != null ? cf.VALUE : "") ?? "").ToString();
            }
            catch (Exception ex)
            {
                LogSystem.Warn("Khong tim thay key. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key), ex);
            }
            return "";
        }

        public static bool ReloadAll()
        {
            bool result = false;
            try
            {
                dicConfig.Clear();
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
