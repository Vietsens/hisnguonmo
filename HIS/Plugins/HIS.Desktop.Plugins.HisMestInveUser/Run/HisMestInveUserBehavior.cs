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
using HIS.Desktop.Plugins.HisMestInveUser;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisMestInveUser.Run;
using MOS.EFMODEL.DataModels;
using MOS.SDO;

namespace Inventec.Desktop.Plugins.HisMestInveUser.Run
{
    public sealed class HisMestInveUserBehavior : Tool<IDesktopToolContext>, IHisMestInveUser
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        V_HIS_MEDI_STOCK_PERIOD HisMediStockPeriod = new V_HIS_MEDI_STOCK_PERIOD();
        HisExpMestResultSDO hisLostExpMest = new HisExpMestResultSDO();
        public HisMestInveUserBehavior()
            : base()
        {
        }

        public HisMestInveUserBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisMestInveUser.Run()
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
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is V_HIS_MEDI_STOCK_PERIOD)
                        {
                            HisMediStockPeriod = (V_HIS_MEDI_STOCK_PERIOD)item;
                        }
                        else if (item is HisExpMestResultSDO)
                        {
                            hisLostExpMest = (HisExpMestResultSDO)item;
                        }
                    }
                    if (currentModule != null && HisMediStockPeriod != null && HisMediStockPeriod.ID > 0)
                    {
                        result = new frmHisMestInveUser(currentModule,
                                                    HisMediStockPeriod);
                    }
                    else
                        result = new frmHisMestInveUser(hisLostExpMest);
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
