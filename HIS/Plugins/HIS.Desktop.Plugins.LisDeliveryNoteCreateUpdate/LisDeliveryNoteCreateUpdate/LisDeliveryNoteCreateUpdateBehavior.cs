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
using LIS.EFMODEL.DataModels;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.LisDeliveryNoteCreateUpdate.LisDeliveryNoteCreateUpdate
{
    class LisDeliveryNoteCreateUpdateBehavior : Tool<IDesktopToolContext>, ILisDeliveryNoteCreateUpdate
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        LIS_DELIVERY_NOTE lisDeliveryNote = null;
        RefeshReference refeshReference;
        internal LisDeliveryNoteCreateUpdateBehavior()
            : base()
        {

        }

        internal LisDeliveryNoteCreateUpdateBehavior(CommonParam param, object[] filter)
            : base()
        {
            entity = filter;
        }
        object ILisDeliveryNoteCreateUpdate.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            this.moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is LIS_DELIVERY_NOTE)
                        {
                            lisDeliveryNote = (LIS_DELIVERY_NOTE)item;
                        }
                        else if (item is RefeshReference)
                        {
                            refeshReference = (RefeshReference)item;
                        }
                    }
                    result = new UCLisDeliveryNoteCreateUpdate(moduleData, lisDeliveryNote, refeshReference);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
            return result;
        }
    }
}
