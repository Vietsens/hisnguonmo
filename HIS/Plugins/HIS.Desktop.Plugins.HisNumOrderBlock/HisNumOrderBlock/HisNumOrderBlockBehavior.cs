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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisNumOrderBlock.HisNumOrderBlock
{
    class HisNumOrderBlockBehavior : BusinessBase, IHisNumOrderBlock
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
      
        internal HisNumOrderBlockBehavior()
            : base()
        {

        }
        internal HisNumOrderBlockBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IHisNumOrderBlock.Run()
        {

            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_ROOM_TIME HisRoomTime_ = new HIS_ROOM_TIME();
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                            if (entity[i] is HIS_ROOM_TIME)
                            {
                                HisRoomTime_ = (HIS_ROOM_TIME)entity[i];
                            }
                        }
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HisRoomTime_), HisRoomTime_));
                if (HisRoomTime_ != null)
                {
                    return new frmHisNumOrderBlock(moduleData, HisRoomTime_);
                }
                else
                {
                    return new frmHisNumOrderBlock(moduleData);
                }
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
