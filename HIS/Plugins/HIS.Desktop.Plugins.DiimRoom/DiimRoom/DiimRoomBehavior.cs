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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.DiimRoom.DiimRoom
{
    class DiimRoomBehavior : BusinessBase, IDiimRoom
    {
        object[] entity;
        internal DiimRoomBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IDiimRoom.Run()
        {
            try
            {
				Inventec.Desktop.Common.Modules.Module module = null;
                V_HIS_SERE_SERV_6 sereServ6 = null;
				RefeshReference refeshReference = null;
				foreach (var item in entity)
                {
                    if (item is Inventec.Desktop.Common.Modules.Module)
                    {
                        module = (Inventec.Desktop.Common.Modules.Module)item;
                    }
					else if (item is RefeshReference)
                    {
                        refeshReference = (RefeshReference)item;
                    }
                    else if (item is V_HIS_SERE_SERV_6)
                    {
                        sereServ6 = (V_HIS_SERE_SERV_6)item;
                    }
                }
                return new frmPacsView(module, sereServ6, refeshReference);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
