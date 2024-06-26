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
using HIS.Desktop.Plugins.HisAssignBlood;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Plugins.HisAssignBlood.ADO;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.HisAssignBlood.HisAssignBlood
{
    public sealed class HisAssignBloodBehavior : Tool<IDesktopToolContext>, IHisAssignBlood
    {
        object[] entity;
        public HisAssignBloodBehavior()
            : base()
        {
        }

        public HisAssignBloodBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisAssignBlood.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                AssignBloodADO assignBloodADO = null;
                HIS_SERVICE_REQ serviceReq = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is AssignBloodADO)
                        {
                            assignBloodADO = (AssignBloodADO)entity[i];
                        }
                        else if (entity[i] is HIS_SERVICE_REQ)
                        {
                            serviceReq = (HIS_SERVICE_REQ)entity[i];
                        }
                    }
                }
                if (assignBloodADO != null && moduleData != null)
                {
                    if (serviceReq != null)
                    {
                        result = new frmHisAssignBlood(moduleData, assignBloodADO, serviceReq);
                    }
                    else
                        result = new frmHisAssignBlood(moduleData, assignBloodADO);
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
