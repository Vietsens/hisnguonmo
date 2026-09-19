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
using HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive
{
    /// <summary>
    /// Cache RAM danh muc "dich vu khong duoc chi dinh dong thoi".
    ///
    /// KHONG dung BackendDataWorker duoc vi type V_HIS_SERVICE_EXCLUSIVE nam trong assembly nay
    /// (khong phai MOS.EFMODEL.DataModels) nen co che tu suy URI cua BackendDataWorker khong ap dung.
    /// Vi vay tu giu cache tinh: nap 1 lan trong 1 phien lam viec, dung Reset() sau khi sua danh muc.
    /// </summary>
    public class ServiceExclusiveDataWorker
    {
        private static readonly object lockObject = new object();
        private static List<V_HIS_SERVICE_EXCLUSIVE> data;
        private static bool isLoaded = false;

        /// <summary>
        /// Danh sach cap dich vu loai tru dang hieu luc (IS_ACTIVE = 1).
        /// Luon tra ve list khac null de nguoi goi khong phai kiem tra.
        /// </summary>
        public static List<V_HIS_SERVICE_EXCLUSIVE> DATA
        {
            get
            {
                if (!isLoaded)
                {
                    Load();
                }
                return data ?? new List<V_HIS_SERVICE_EXCLUSIVE>();
            }
        }

        /// <summary>Danh muc rong (vien chua khai bao) -> bo qua toan bo viec kiem tra</summary>
        public static bool IsEmpty
        {
            get { return DATA.Count == 0; }
        }

        private static void Load()
        {
            lock (lockObject)
            {
                if (isLoaded)
                {
                    return;
                }

                try
                {
                    CommonParam param = new CommonParam();
                    HisServiceExclusiveFilter filter = new HisServiceExclusiveFilter();
                    filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                    List<V_HIS_SERVICE_EXCLUSIVE> result = new BackendAdapter(param).Get<List<V_HIS_SERVICE_EXCLUSIVE>>(
                        HisRequestUriStore.MOSHIS_SERVICE_EXCLUSIVE_GET_VIEW,
                        HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                        filter,
                        param);

                    data = (result ?? new List<V_HIS_SERVICE_EXCLUSIVE>())
                        .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        .ToList();

                    Inventec.Common.Logging.LogSystem.Info(
                        "ServiceExclusiveDataWorker.Load: nap " + data.Count + " ban ghi danh muc dich vu loai tru");
                }
                catch (Exception ex)
                {
                    // Backend chua co API (chua trien khai bang HIS_SERVICE_EXCLUSIVE) -> coi nhu danh muc rong,
                    // tinh nang tat, cac man chi dinh chay y nhu truoc khi cap nhat.
                    data = new List<V_HIS_SERVICE_EXCLUSIVE>();
                    Inventec.Common.Logging.LogSystem.Warn(
                        "ServiceExclusiveDataWorker.Load that bai, coi nhu danh muc rong (tinh nang tat)", ex);
                }
                finally
                {
                    isLoaded = true;
                }
            }
        }

        /// <summary>Xoa cache, lan lay tiep theo se nap lai tu backend (goi sau khi sua danh muc)</summary>
        public static void Reset()
        {
            lock (lockObject)
            {
                data = null;
                isLoaded = false;
            }
        }
    }
}
