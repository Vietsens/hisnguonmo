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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.HisConfig
{
    public class ConfigLoader
    {
        const string configUri = "/api/HisConfig/Get";

        public static bool Refresh()
        {
            bool result = false;
            try
            {
                CommonParam paramGet = new CommonParam();
                MOS.Filter.HisConfigFilter configFilter = new MOS.Filter.HisConfigFilter();
                configFilter.IS_ACTIVE = 1;
                //configFilter.WORKING_BRANCH_ID = BranchDataWorker.GetCurrentBranchId();//TODO
                var ro = new BackendAdapter(paramGet).Get<List<MOS.EFMODEL.DataModels.HIS_CONFIG>>(configUri, ApiConsumers.MosConsumer, configFilter, paramGet);
                
                if (ro != null && ro.Count > 0)
                {
                    ro = ro.Where(o => o.BRANCH_ID == null || o.BRANCH_ID == BranchWorker.GetCurrentBranchId()).ToList();

                    //BackendDataWorker.TranslateData<MOS.EFMODEL.DataModels.HIS_CONFIG>(ro);
                    foreach (var config in ro)
                    {
                        if (!String.IsNullOrWhiteSpace(config.KEY))
                        {
                            if (HisConfigs.dic.ContainsKey(config.KEY))
                            {
                                object outValue = null;
                                if (!HisConfigs.dic.TryRemove(config.KEY, out outValue))
                                {
                                    LogSystem.Info("Khong Remove duoc cau hinh trong dictionary Key: " + config.KEY.ToString());
                                    if (!HisConfigs.dic.TryUpdate(config.KEY, config, HisConfigs.dic[config.KEY]))
                                        HisConfigs.dic[config.KEY] = config;
                                }
                                else
                                {
                                    if (!HisConfigs.dic.TryAdd(config.KEY, config))
                                    {
                                        LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + config.KEY.ToString());
                                    }
                                }
                            }
                            else
                            {
                                //if (config.KEY=="HIS.Desktop.Plugins.ExamServiceReqExecute.ControlRequired")
                                //{
                                //    var ss = "";
                                //}
                                if (!HisConfigs.dic.TryAdd(config.KEY, config))
                                {
                                    LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + config.KEY.ToString());
                                }
                            }


                            //HisConfigs.dic[config.KEY] = config; //Ghi de du lieu cu ==> luu y tinh huong neu 2 config trung key thi config sau se de len config truoc. Loi nay thuoc ve constraint du lieu ko thuoc trach nhiem cua Loader.
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
                    LogSystem.Error("Query HisConfig co exception.");
                }
                else
                {
                    LogSystem.Warn("Khong co du lieu HisConfig & khong co exception.");
                    result = true;
                }
                if (result)
                {
                    LogSystem.Info("Load du lieu cau hinh HisConfig thanh cong.");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
