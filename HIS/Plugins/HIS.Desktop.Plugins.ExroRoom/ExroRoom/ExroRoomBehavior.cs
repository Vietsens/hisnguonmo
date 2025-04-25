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
using System;
using System.Collections.Generic;
using System.Linq;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.ExroRoom.ExroRoom
{
    internal class ExroRoomBehavior : Tool<IDesktopToolContext>, IExroRoom
    {
        private object[] entity;
        private V_HIS_EXECUTE_ROOM executeRoom;
        private V_HIS_BED_ROOM bedRoom;
        private MOS.EFMODEL.DataModels.V_HIS_ROOM executeRoom1;
        private V_HIS_RECEPTION_ROOM receptionRoom;



        internal ExroRoomBehavior()
            : base()
        {
        }

        internal ExroRoomBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }


        object IExroRoom.Run()
        {
            object result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is V_HIS_EXECUTE_ROOM)
                        {
                            executeRoom = (V_HIS_EXECUTE_ROOM)item;
                        }
                        if (item is V_HIS_BED_ROOM)
                        {
                            bedRoom = (V_HIS_BED_ROOM)item;
                        }
                        if (item is V_HIS_ROOM)
                        {
                            executeRoom1 = (V_HIS_ROOM)item;
                        }
                        if (item is V_HIS_RECEPTION_ROOM)
                        {
                            receptionRoom = (V_HIS_RECEPTION_ROOM)item;
                        }
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                    }

                    if (executeRoom != null && bedRoom != null)
                    {
                        result = new UCExroRoom(executeRoom, bedRoom, moduleData);
                    }
                    else
                    {    
                        if (executeRoom1 != null)
                        {
                            result = new UCExroRoom(executeRoom1, moduleData);
                        }
                        else
                        {
                            if (receptionRoom != null)
                            {
                                result = new UCExroRoom(receptionRoom, moduleData);
                            }
                            else
                            {
                                result = new UCExroRoom(moduleData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
