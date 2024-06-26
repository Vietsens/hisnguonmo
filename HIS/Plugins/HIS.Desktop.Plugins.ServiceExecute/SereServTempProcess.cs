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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute
{
    internal class SereServTempProcess
    {
        static List<HIS_SERE_SERV_TEMP> listTemplate;

        internal static HIS_SERE_SERV_TEMP GetDescription(HIS_SERE_SERV_TEMP data)
        {
            HIS_SERE_SERV_TEMP resutl = null;
            if (listTemplate != null && listTemplate.Count > 0)
            {
                resutl = listTemplate.FirstOrDefault(o => o.ID == data.ID);
            }
            return resutl;
        }

        internal static void SetDescription(HIS_SERE_SERV_TEMP data)
        {
            try
            {
                if (data != null)
                {
                    if (listTemplate == null)
                    {
                        listTemplate = new List<HIS_SERE_SERV_TEMP>();
                    }

                    var oldData = listTemplate.FirstOrDefault(o => o.ID == data.ID);
                    if (oldData != null && oldData.MODIFY_TIME != data.MODIFY_TIME)
                    {
                        oldData.DESCRIPTION = data.DESCRIPTION;
                        oldData.MODIFY_TIME = data.MODIFY_TIME;
                    }
                    else if (oldData == null)
                    {
                        listTemplate.Add(data);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
