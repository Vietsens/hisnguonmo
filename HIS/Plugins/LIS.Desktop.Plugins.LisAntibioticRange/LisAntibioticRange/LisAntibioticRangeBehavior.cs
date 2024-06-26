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
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using LIS.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIS.Desktop.Plugins.LisAntibioticRange.LisAntibioticRange
{
    class LisAntibioticRangeBehavior : Tool<IDesktopToolContext>, ILisAntibioticRange
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        internal LisAntibioticRangeBehavior()
            : base()
        {

        }

        internal LisAntibioticRangeBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ILisAntibioticRange.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;

                    }
                }

                if (currentModule != null)
                {
                    return new LIS.Desktop.Plugins.LisAntibioticRange.Run.frmLisAntibioticRange(currentModule);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
