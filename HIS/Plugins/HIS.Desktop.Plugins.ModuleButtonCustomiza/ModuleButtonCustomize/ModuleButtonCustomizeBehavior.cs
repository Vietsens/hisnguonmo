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
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Core;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ModuleButtonCustomize.ModuleButtonCustomize
{
    class ModuleButtonCustomizeBehavior : BusinessBase, IModuleButtonCustomize
    {
        object[] entity;
        internal ModuleButtonCustomizeBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IModuleButtonCustomize.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                RefeshReference refeshReference = null;
                SDA_CUSTOMIZE_BUTTON hideControl = null;
                List<ModuleControlADO> moduleControlADOs = null;
                foreach (var item in entity)
                {
                    if (item is Inventec.Desktop.Common.Modules.Module)
                    {
                        module = (Inventec.Desktop.Common.Modules.Module)item;
                    }
                    if (item is RefeshReference)
                    {
                        refeshReference = (RefeshReference)item;
                    }
                    if (item is List<ModuleControlADO>)
                    {
                        moduleControlADOs = (List<ModuleControlADO>)item;
                    }
                    if (item is SDA_CUSTOMIZE_BUTTON)
                    {
                        hideControl = (SDA_CUSTOMIZE_BUTTON)item;
                    }
                }
                return new frmModuleButtonCustomize(module, refeshReference, moduleControlADOs, hideControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
