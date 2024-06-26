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
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.BidDetail.BidDetail
{
    class BidDetailBehavior : Tool<IDesktopToolContext>, IBidDetail
    {
        object[] entity;

        internal BidDetailBehavior()
            : base()
        {

        }

        internal BidDetailBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IBidDetail.Run()
        {
            MOS.EFMODEL.DataModels.HIS_BID result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is MOS.EFMODEL.DataModels.HIS_BID) 
                            result = (MOS.EFMODEL.DataModels.HIS_BID)item;
                        if (item is Inventec.Desktop.Common.Modules.Module) 
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                    }
                }
                if (moduleData != null)
                {
                    return new FormBidDetail(result, moduleData);
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
