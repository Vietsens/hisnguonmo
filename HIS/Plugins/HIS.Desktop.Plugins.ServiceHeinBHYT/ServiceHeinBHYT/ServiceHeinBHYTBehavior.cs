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
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceHeinBHYT.ServiceHeinBHYT
{
    public sealed class ServiceHeinBHYTBehavior : Tool<IDesktopToolContext>, IServiceHeinBHYT
    {
        object[] entity;
        DelegateSelectData delegateSelect;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public ServiceHeinBHYTBehavior()
            : base()
        {
        }

        public ServiceHeinBHYTBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IServiceHeinBHYT.Run()
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
                        if (item is DelegateSelectData)
                        {
                            delegateSelect = (DelegateSelectData)item;
                        }

                    }
                    if (currentModule != null && delegateSelect != null)
                    {
                        result = new FormHeinServiceBHYT(currentModule, delegateSelect);

                    }
                    if (currentModule != null && delegateSelect == null)
                    {
                        result = new FormHeinServiceBHYT(currentModule);
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