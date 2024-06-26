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
using MOS.EFMODEL.DataModels;
using TYT.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.HisPrepareApprove.HisPrepareApprove
{
    class HisPrepareApproveBehavior : Tool<IDesktopToolContext>, IHisPrepareApprove
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal HisPrepareApproveBehavior()
            : base()
        {

        }

        internal HisPrepareApproveBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IHisPrepareApprove.Run()
        {
            object result = null;
            try
            {
                HIS.Desktop.Common.RefeshReference refreshData = null;
                long prepareId = 0;
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is long)
                        {
                            prepareId = (long)item;
                        }
                        else if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is HIS.Desktop.Common.RefeshReference)
                        {
                            refreshData = (HIS.Desktop.Common.RefeshReference)item;
                        }
                    }

                    if (currentModule != null && prepareId > 0)
                    {
                        result = new frmHisPrepareApprove(currentModule, prepareId, refreshData);
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
