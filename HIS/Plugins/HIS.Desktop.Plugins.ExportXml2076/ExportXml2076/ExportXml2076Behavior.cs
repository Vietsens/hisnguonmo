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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExportXml2076.ExportXml2076
{
    class ExportXml2076Behavior : Tool<IDesktopToolContext>, IExportXml2076
    {
        object[] entity;
        internal ExportXml2076Behavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IExportXml2076.Run()
        {
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = item as Inventec.Desktop.Common.Modules.Module;
                        }
                    }
                }
                if (WorkPlace.GetBranchId() > 0 && moduleData != null)
                {
                    return new UCExportXml2076(moduleData);
                }
                throw new NullReferenceException("BranchId<=0 OR moduleData = null");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
