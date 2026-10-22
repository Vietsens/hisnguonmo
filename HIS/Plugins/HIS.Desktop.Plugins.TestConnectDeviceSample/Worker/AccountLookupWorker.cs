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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    /// <summary>
    /// Resolves an account card. Always asks ACS (not the RAM cache) so that accounts created or locked
    /// after HIS was opened are seen at once.
    /// </summary>
    internal static class AccountLookupWorker
    {
        internal static bool TryGetActiveUser(string loginname, out ACS_USER user, out string errorMessage)
        {
            user = null;
            errorMessage = null;
            try
            {
                CommonParam param = new CommonParam();
                AcsUserFilter filter = new AcsUserFilter();
                filter.LOGINNAME = loginname;
                List<ACS_USER> users = new BackendAdapter(param).Get<List<ACS_USER>>(
                    AcsRequestUriStore.ACS_USER_GET, ApiConsumers.AcsConsumer, filter, param);
                SessionManager.ProcessTokenLost(param);
                if (users == null)
                {
                    errorMessage = String.Format(ResourceMessage.KhongTraCuuDuocTaiKhoan, loginname);
                    Inventec.Common.Logging.LogSystem.Warn("AcsUser/Get failed for card " + loginname
                        + Inventec.Common.Logging.LogUtil.TraceData("param", param));
                    return false;
                }

                // The filter may match loosely: accept exactly one account with the same loginname
                List<ACS_USER> matches = users
                    .Where(o => String.Equals(o.LOGINNAME, loginname, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                if (matches.Count != 1)
                {
                    errorMessage = String.Format(ResourceMessage.TaiKhoanKhongTonTai, loginname);
                    return false;
                }
                if (matches[0].IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                {
                    errorMessage = String.Format(ResourceMessage.TaiKhoanDaKhoa, loginname);
                    return false;
                }

                user = matches[0];
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                errorMessage = String.Format(ResourceMessage.KhongTraCuuDuocTaiKhoan, loginname);
                return false;
            }
        }

        /// <summary>
        /// Adds or replaces the account in the cached ACS_USER list (UpdateToRam replaces the whole list,
        /// so it must receive every account) so that the sample info popup can preselect it.
        /// </summary>
        internal static void MergeIntoCache(ACS_USER user)
        {
            try
            {
                if (user == null || !BackendDataWorker.IsExistsKey<ACS_USER>())
                    return; // not loaded yet: the popup loads every account itself

                List<ACS_USER> cached = BackendDataWorker.Get<ACS_USER>() ?? new List<ACS_USER>();
                List<ACS_USER> merged = cached
                    .Where(o => !String.Equals(o.LOGINNAME, user.LOGINNAME, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                merged.Add(user);
                BackendDataWorker.UpdateToRam(typeof(ACS_USER), merged, Inventec.Common.DateTime.Get.Now() ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
