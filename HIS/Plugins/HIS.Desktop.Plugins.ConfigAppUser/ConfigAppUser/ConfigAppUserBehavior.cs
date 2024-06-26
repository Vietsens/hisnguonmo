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

namespace HIS.Desktop.Plugins.ConfigAppUser.ConfigAppUser
{
    class ConfigAppUserBehavior : Tool<IDesktopToolContext>, IConfigAppUser
    {
        object[] entity;

        internal ConfigAppUserBehavior()
            : base()
        {

        }

        internal ConfigAppUserBehavior(CommonParam param, object[] data)
            : base()
        {
            
            entity = data;
        }

        object IConfigAppUser.Run()
        {
            object result = null;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            Inventec.Common.WebApiClient.ApiConsumer SdaConsumer = null;
            long configNumPageSize = 0;
            string applicationCode = "";
            Action delegateRefresh = null;
            string workingModuleLink = "";
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is List<string>)
                        {
                            var sarr = (List<string>)entity[i];
                            workingModuleLink = sarr.Count > 1 ? sarr[1] : "";
                            applicationCode = sarr.Count > 0 ? sarr[0] : "";
                        }
                        if (entity[i] is string)
                        {
                            applicationCode = (string)entity[i];
                        }
                        if (entity[i] is Inventec.Common.WebApiClient.ApiConsumer)
                        {
                            SdaConsumer = (Inventec.Common.WebApiClient.ApiConsumer)entity[i];
                        }
                        if (entity[i] is long)
                        {
                            configNumPageSize = (long)entity[i];
                        }
                        if (entity[i] is Action)
                        {
                            delegateRefresh = (Action)entity[i];
                        }
                    }
                    if (moduleData.ModuleTypeId == Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM)
                        result = new frmConfigAppUser(moduleData, SdaConsumer, delegateRefresh, configNumPageSize, applicationCode, workingModuleLink);
                    else
                        result = new UCConfigAppUser(moduleData, SdaConsumer, delegateRefresh, configNumPageSize, applicationCode);
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
