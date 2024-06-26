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
using HIS.Desktop.Plugins.ExroRoom.ExroRoom;
using Inventec.Core;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.ExroRoom
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
    "HIS.Desktop.Plugins.ExroRoom",
    "Cấu hình phòng - phòng",
    "Common",
    62,
    "technology_32x32.png",
    "A",
    Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC,
    true,
    true)]
    public class ExroRoomProcessor : ModuleBase, IDesktopRoot
    {
        private CommonParam param;
        public ExroRoomProcessor()
        {
            param = new CommonParam();
        }
        public ExroRoomProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            var result = (object )null;
            try
            {
                var behavior = ExroRoomFactory.MakeIMedicineTypeRoom(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            return false;
        }
    }
}
