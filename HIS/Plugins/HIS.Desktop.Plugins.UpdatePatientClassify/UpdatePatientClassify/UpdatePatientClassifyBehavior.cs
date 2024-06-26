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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.UpdatePatientClassify.UpdatePatientClassify
{
    class UpdatePatientClassifyBehavior : Tool<IDesktopToolContext>, IUpdatePatientClassify
    {
        object[] entity;

        Inventec.Desktop.Common.Modules.Module moduleData = null;
        MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM listBedRoom = new MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM();
        internal UpdatePatientClassifyBehavior()
            : base()
        {

        }

        internal UpdatePatientClassifyBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IUpdatePatientClassify.Run()
        {
            try
            {
                bool isDisable = false;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        else if (item is MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM)
                        {
                            listBedRoom = (MOS.EFMODEL.DataModels.L_HIS_TREATMENT_BED_ROOM)item;
                        }
                    }
                }
                if (moduleData != null && listBedRoom != null)
                {
                    return new frmUpdatePatientClassify(moduleData, listBedRoom);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
