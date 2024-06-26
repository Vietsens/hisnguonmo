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
using HIS.Desktop.ADO;
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

namespace HIS.Desktop.Plugins.ServiceExecuteGroup.ServiceExecuteGroup
{
    class ServiceExecuteGroupBehavior : Tool<IDesktopToolContext>, IServiceExecuteGroup
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module moduleData = null;     
        RefeshReference delegateRefresh = null;
        List<L_HIS_SERVICE_REQ> lst = null;
        internal ServiceExecuteGroupBehavior()
            : base()
        {

        }

        internal ServiceExecuteGroupBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IServiceExecuteGroup.Run()
        {
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;                
                        else if (item is RefeshReference)                
                            delegateRefresh = (RefeshReference)item;
                        else if (item is List<L_HIS_SERVICE_REQ>)
                            lst = (List<L_HIS_SERVICE_REQ>)item;
                    }
                }

                if (moduleData != null)
                {
                    return new HIS.Desktop.Plugins.ServiceExecuteGroup.Run.frmServiceExecuteGroup(moduleData, delegateRefresh, lst);
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
