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
using DevExpress.Utils;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor.Base;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Bordereau
{
    public class UpdatePatientTypeProcess
    {

        public static bool UpdatePatientType(MOS.EFMODEL.DataModels.HIS_SERE_SERV hisSereServ,ref CommonParam param)
        {
            bool success = false;
            try
            {
                WaitingManager.Show();
                if (hisSereServ != null)
                {
                    HisSereServFilter filter = new HisSereServFilter();
                    hisSereServ = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_SERE_SERV>(HisRequestUriStore.HIS_SERE_SERV_UPDATE, ApiConsumers.MosConsumer, hisSereServ, param);
                    if (hisSereServ != null)
                    {
                        success = true;
                    }
                }

                WaitingManager.Hide();

                //#region Show message
                //MessageManager.Show(this, param, success);
                //#endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
            return success;
        }
    }
}
