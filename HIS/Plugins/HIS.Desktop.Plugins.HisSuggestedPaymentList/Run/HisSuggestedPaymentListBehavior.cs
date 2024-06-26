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
using HIS.Desktop.Plugins.HisSuggestedPaymentList;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.SDO;
using HIS.Desktop.Plugins.HisSuggestedPaymentList.Run;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisSuggestedPaymentList.Run
{
    public sealed class HisSuggestedPaymentListBehavior : Tool<IDesktopToolContext>, IHisSuggestedPaymentList
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        long treatmentId = 0;
        HIS_DHST currentDhst;
        public HisSuggestedPaymentListBehavior()
            : base()
        {
        }

        public HisSuggestedPaymentListBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisSuggestedPaymentList.Run()
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
                    }
                    if (currentModule != null )
                    {
                        result = new frmHisSuggestedPaymentList(currentModule);
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
