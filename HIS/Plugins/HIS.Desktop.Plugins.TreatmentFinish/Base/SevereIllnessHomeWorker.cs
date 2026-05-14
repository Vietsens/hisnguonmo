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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.Base
{
    /// <summary>
    /// 2608 - Bệnh nặng xin về.
    /// Helper trigger popup HIS.Desktop.Plugins.HisDeathInfo khi BS chọn Loại ra viện thuộc config.
    /// </summary>
    internal static class SevereIllnessHomeWorker
    {
        private const string MODULE_LINK__HIS_DEATH_INFO = "HIS.Desktop.Plugins.HisDeathInfo";

        /// <summary>
        /// Kiểm tra TREATMENT_END_TYPE_ID hiện tại có thuộc danh sách config không.
        /// </summary>
        internal static bool IsMustInputByEndTypeId(long endTypeId, List<string> configCodes)
        {
            try
            {
                if (endTypeId <= 0) return false;
                if (configCodes == null || configCodes.Count == 0) return false;

                var endType = BackendDataWorker.Get<HIS_TREATMENT_END_TYPE>()
                    .FirstOrDefault(o => o.ID == endTypeId);
                if (endType == null || string.IsNullOrWhiteSpace(endType.TREATMENT_END_TYPE_CODE))
                    return false;

                return configCodes.Contains(endType.TREATMENT_END_TYPE_CODE.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra theo TREATMENT_END_TYPE entity (tránh lookup khi đã có sẵn).
        /// </summary>
        internal static bool IsMustInputByEndType(HIS_TREATMENT_END_TYPE endType, List<string> configCodes)
        {
            try
            {
                if (endType == null || string.IsNullOrWhiteSpace(endType.TREATMENT_END_TYPE_CODE)) return false;
                if (configCodes == null || configCodes.Count == 0) return false;
                return configCodes.Contains(endType.TREATMENT_END_TYPE_CODE.Trim().ToUpper());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
        }

        /// <summary>
        /// Mở popup HisDeathInfo qua MEF PluginInstance. Truyền Module + treatmentId.
        /// </summary>
        internal static void OpenPopup(Module currentModule, long treatmentId)
        {
            try
            {
                if (treatmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SevereIllnessHomeWorker.OpenPopup: treatmentId <= 0");
                    return;
                }

                Module hisDeathInfoModule = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == MODULE_LINK__HIS_DEATH_INFO);

                if (hisDeathInfoModule == null || !hisDeathInfoModule.IsPlugin || hisDeathInfoModule.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SevereIllnessHomeWorker.OpenPopup: HisDeathInfo module not registered");
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentId);

                Module resolvedModule = currentModule != null
                    ? PluginInstance.GetModuleWithWorkingRoom(hisDeathInfoModule, currentModule.RoomId, currentModule.RoomTypeId)
                    : hisDeathInfoModule;

                var instance = PluginInstance.GetPluginInstance(resolvedModule, listArgs);
                if (instance == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SevereIllnessHomeWorker.OpenPopup: instance null");
                    return;
                }

                if (instance is Form)
                {
                    ((Form)instance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Kiểm tra HIS_SEVERE_ILLNESS_INFO đã được lưu cho treatment chưa.
        /// </summary>
        internal static bool HasValidSevereIllnessInfo(long treatmentId)
        {
            try
            {
                if (treatmentId <= 0) return false;

                CommonParam param = new CommonParam();
                HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                filter.TREATMENT_ID = treatmentId;

                var data = new BackendAdapter(param).Get<List<HIS_SEVERE_ILLNESS_INFO>>(
                    "api/HisSevereIllnessInfo/Get", ApiConsumers.MosConsumer, filter, param);

                return data != null && data.Count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }
    }
}
