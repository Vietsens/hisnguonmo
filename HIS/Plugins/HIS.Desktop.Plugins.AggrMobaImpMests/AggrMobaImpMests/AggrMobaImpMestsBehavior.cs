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
using HIS.Desktop.Plugins.AggrMobaImpMests;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.AggrMobaImpMests.AggrMobaImpMests
{
    public sealed class AggrMobaImpMestsBehavior : Tool<IDesktopToolContext>, IAggrMobaImpMests
    {
        object[] entity;
        public AggrMobaImpMestsBehavior()
            : base()
        {
        }

        public AggrMobaImpMestsBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IAggrMobaImpMests.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                List<V_HIS_IMP_MEST_2> lstMobaImpMestChecks = new List<V_HIS_IMP_MEST_2>();
                HIS.Desktop.Common.RefeshReference refeshData = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is List<V_HIS_IMP_MEST_2>)
                        {
                            lstMobaImpMestChecks = (List<V_HIS_IMP_MEST_2>)entity[i];
                        }
                        else if (entity[i] is HIS.Desktop.Common.RefeshReference)
                        {
                            refeshData = (HIS.Desktop.Common.RefeshReference)entity[i];
                        }
                    }
                    if (moduleData != null && lstMobaImpMestChecks != null && refeshData != null)
                    {
                        return new frmAggrMobaImpMests(moduleData, lstMobaImpMestChecks, refeshData);
                    }
                    else
                    {
                        return null;
                        Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("Du lieu dau vao", lstMobaImpMestChecks));
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
