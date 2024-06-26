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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.HisImportLisAntibiotic;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.HisImport;

namespace Inventec.Desktop.Plugins.HisImportLisAntibiotic.Run
{
    public sealed class HisImportLisAntibioticBehavior : Tool<IDesktopToolContext>, IHisImportLisAntibiotic
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        RefeshReference delegateRefresh;
        public HisImportLisAntibioticBehavior()
            : base()
        {
        }

        public HisImportLisAntibioticBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisImportLisAntibiotic.Run()
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
                        else if (item is RefeshReference)
                        {
                            delegateRefresh = (RefeshReference)item;
                        }
                    }
                    if (currentModule != null && delegateRefresh != null)
                    {
                        result = new frmImport(currentModule, delegateRefresh);
                    }
                    else
                    {
                        result = new frmImport(currentModule);
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
