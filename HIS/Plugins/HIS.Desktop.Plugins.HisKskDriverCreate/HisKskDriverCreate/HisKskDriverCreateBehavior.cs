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
using HIS.Desktop.Plugins.HisKskDriverCreate.Run;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Desktop.Plugins.HisKskDriverCreate.HisKskDriverCreate
{
    class HisKskDriverCreateBehavior : Tool<IDesktopToolContext>, IHisKskDriverCreate
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        RefeshReference delegateRefresh;
        HIS_KSK_DRIVER kskDriver;
        HIS_SERVICE_REQ kskServiceReq;
        public HisKskDriverCreateBehavior()
            : base()
        {
        }

        public HisKskDriverCreateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisKskDriverCreate.Run()
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
                        else if (item is HIS_KSK_DRIVER)
                        {
                            kskDriver = (HIS_KSK_DRIVER)item;
                        }
                        else if (item is HIS_SERVICE_REQ)
                        {
                            kskServiceReq = (HIS_SERVICE_REQ)item;
                        }
                    }

                    result = new FormKskDriver(currentModule, delegateRefresh, kskDriver, kskServiceReq);
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
