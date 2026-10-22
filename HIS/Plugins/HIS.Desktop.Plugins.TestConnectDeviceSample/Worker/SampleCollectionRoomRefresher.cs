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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using System;
using System.Reflection;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.Worker
{
    /// <summary>
    /// Reloads the "Lấy mẫu bệnh phẩm" list of the same room after a scan was saved, through the same method that
    /// screen calls after a manual take-sample. Otherwise its row still shows "not sampled" and a manual take-sample
    /// on it would overwrite the scanned sampler with the logged-in account.
    /// Reflection on purpose: plugins do not reference each other. Tab name = ModuleLink_RoomId_RoomTypeId
    /// (HIS.Desktop.ModuleExt.TabControlProcess); the UC is the first control of the tab page.
    /// </summary>
    internal static class SampleCollectionRoomRefresher
    {
        private const string REFRESH_METHOD = "FillDataToGridControl";

        internal static bool Refresh(long roomId, long roomTypeId)
        {
            try
            {
                DevExpress.XtraTab.XtraTabControl tabControl = SessionManager.GetTabControlMain();
                if (tabControl == null)
                    return false;

                string tabName = ModuleLinkString.SampleCollectionRoom + "_" + roomId + "_" + roomTypeId;
                foreach (DevExpress.XtraTab.XtraTabPage page in tabControl.TabPages)
                {
                    if (page.Name != tabName || page.Controls.Count == 0)
                        continue;

                    object uc = page.Controls[0];
                    MethodInfo method = uc.GetType().GetMethod(REFRESH_METHOD,
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
                    if (method == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("SampleCollectionRoom has no " + REFRESH_METHOD + "(): list not refreshed");
                        return false;
                    }
                    method.Invoke(uc, null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }
    }
}
