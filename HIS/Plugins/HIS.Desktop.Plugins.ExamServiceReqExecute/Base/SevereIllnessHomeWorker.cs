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
using HIS.Desktop.LocalStorage.LocalData;
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

namespace HIS.Desktop.Plugins.ExamServiceReqExecute.Base
{
    /// <summary>
    /// 2608 - Bệnh nặng xin về.
    /// Helper trigger popup HIS.Desktop.Plugins.InformationAllowGoHome khi BS chọn Loại ra viện thuộc config.
    /// Dùng chung popup với chức năng Kết thúc điều trị (TreatmentFinish) - KHÔNG dùng popup
    /// HisDeathInfo (Thông tin tử vong) vì popup đó lưu HIS_SEVERE_ILLNESS_INFO.IS_DEATH = 1.
    /// </summary>
    internal static class SevereIllnessHomeWorker
    {
        private const string MODULE_LINK__INFORMATION_ALLOW_GO_HOME = "HIS.Desktop.Plugins.InformationAllowGoHome";

        /// <summary>
        /// Kiểm tra TREATMENT_END_TYPE_ID hiện tại có thuộc danh sách config không.
        /// Tra cứu CODE qua BackendDataWorker, so case-insensitive với configCodes.
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
        /// Mở popup InformationAllowGoHome qua MEF PluginInstance.
        /// Truyền args giống TreatmentFinish: treatmentId, thời gian ra (outTime), isSave = true,
        /// callback nhận lại thời gian xin về để gán vào SDO khi lưu.
        /// </summary>
        internal static void OpenPopup(Module currentModule, long treatmentId, long outTime, Action<long?> deathTimeResult)
        {
            try
            {
                if (treatmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SevereIllnessHomeWorker.OpenPopup: treatmentId <= 0");
                    return;
                }

                Module goHomeModule = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == MODULE_LINK__INFORMATION_ALLOW_GO_HOME);

                if (goHomeModule == null || !goHomeModule.IsPlugin || goHomeModule.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SevereIllnessHomeWorker.OpenPopup: InformationAllowGoHome module not registered");
                    return;
                }

                List<object> listArgs = new List<object>();
                listArgs.Add(treatmentId);
                listArgs.Add(outTime);
                listArgs.Add(true);
                if (deathTimeResult != null)
                    listArgs.Add(deathTimeResult);

                Module resolvedModule = currentModule != null
                    ? PluginInstance.GetModuleWithWorkingRoom(goHomeModule, currentModule.RoomId, currentModule.RoomTypeId)
                    : goHomeModule;

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
        /// Sau khi popup đóng, gọi API kiểm tra HIS_SEVERE_ILLNESS_INFO đã tồn tại cho treatment chưa.
        /// Trả về true nếu đã có bản ghi (popup đã lưu); false nếu chưa.
        /// </summary>
        internal static bool HasValidSevereIllnessInfo(long treatmentId)
        {
            try
            {
                if (treatmentId <= 0) return false;

                CommonParam param = new CommonParam();
                HisSevereIllnessInfoFilter filter = new HisSevereIllnessInfoFilter();
                filter.TREATMENT_ID = treatmentId;
                //Chi tinh ban ghi benh nang xin ve (IS_DEATH = 0), khong tinh ban ghi tu vong
                filter.IS_DEATH = false;

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
