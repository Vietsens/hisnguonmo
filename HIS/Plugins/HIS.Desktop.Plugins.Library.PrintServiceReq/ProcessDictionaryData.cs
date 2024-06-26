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
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.PrintServiceReq
{
    internal class ProcessDictionaryData
    {
        private static Dictionary<string, ACS_USER> dicAcsUserMobile { get; set; }
        private static Dictionary<long, V_HIS_SERVICE> dicService { get; set; }

        internal static ACS_USER GetUserMobile(string loginname)
        {
            ACS_USER result = null;

            if (dicAcsUserMobile == null)
            {
                var acsUserMobiles = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => !String.IsNullOrWhiteSpace(o.MOBILE)).ToList();
                if (acsUserMobiles != null && acsUserMobiles.Count > 0)
                {
                    dicAcsUserMobile = acsUserMobiles.ToDictionary(o => o.LOGINNAME, o => o);
                }
            }

            if (dicAcsUserMobile != null && dicAcsUserMobile.ContainsKey(loginname))
            {
                result = dicAcsUserMobile[loginname];
            }

            return result;
        }

        internal static void Reload()
        {
            try
            {
                var acsUserMobiles = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>().Where(o => !String.IsNullOrWhiteSpace(o.MOBILE)).ToList();
                if (acsUserMobiles != null && acsUserMobiles.Count > 0)
                {
                    dicAcsUserMobile = acsUserMobiles.ToDictionary(o => o.LOGINNAME, o => o);
                }

                dicService = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.ID, o => o);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static V_HIS_SERVICE GetService(long id)
        {
            if (dicService == null)
            {
                dicService = BackendDataWorker.Get<V_HIS_SERVICE>().ToDictionary(o => o.ID, o => o);
            }

            if (dicService.ContainsKey(id))
            {
                return dicService[id];
            }

            return null;
        }
    }
}
