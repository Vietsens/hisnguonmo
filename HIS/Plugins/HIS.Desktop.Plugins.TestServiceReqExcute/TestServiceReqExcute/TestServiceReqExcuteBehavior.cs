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
using HIS.Desktop.Plugins.TestServiceReqExcute;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TestServiceReqExcute.TestServiceReqExcute
{
    public sealed class TestServiceReqExcuteBehavior : Tool<IDesktopToolContext>, ITestServiceReqExcute
    {
        object[] entity;
        MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ currentServiceReq;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public TestServiceReqExcuteBehavior()
            : base()
        {
        }

        public TestServiceReqExcuteBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ITestServiceReqExcute.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)
                        {
                            this.currentServiceReq = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ)item;
                        }
                        else if(item is Inventec.Desktop.Common.Modules.Module)
                        {
                            this.currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        if (currentModule != null && currentServiceReq != null)
                        {
                            result =  new UCTestServiceReqExcute(currentModule, currentServiceReq);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
            return result;
        }
    }
}
