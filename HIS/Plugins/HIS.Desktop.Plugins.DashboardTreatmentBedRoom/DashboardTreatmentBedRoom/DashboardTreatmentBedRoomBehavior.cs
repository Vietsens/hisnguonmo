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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.DashboardTreatmentBedRoom
{
    class DashboardTreatmentBedRoomBehavior : BusinessBase, IDashboardTreatmentBedRoom
    {
        object[] entity;

        internal DashboardTreatmentBedRoomBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IDashboardTreatmentBedRoom.Run()
        {
            try
            {
                Module moduleData = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Module)
                        {
                            moduleData = (Module)entity[i];
                        }
                    }
                }

                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc module dau vao nen khong mo duoc form danh sach phong trong khoa.");
                    return null;
                }

                return new frmTreatmentBedRoom(moduleData);
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
