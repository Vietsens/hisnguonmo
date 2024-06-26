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
using HIS.Desktop.Plugins.UpdateExamServiceReq;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.UpdateExamServiceReq.Base;

namespace Inventec.Desktop.Plugins.UpdateExamServiceReq.UpdateExamServiceReq
{
    public sealed class UpdateExamServiceReqBehavior : Tool<IDesktopToolContext>, IUpdateExamServiceReq
    {
        object[] entity;
        bool isExecuteRoom = false;

        public UpdateExamServiceReqBehavior()
            : base()
        {
        }

        public UpdateExamServiceReqBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IUpdateExamServiceReq.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long serviceReqId = 0;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            serviceReqId = (long)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            serviceReqId = (long)entity[i];
                        }
                        else if (entity[i] is bool)
                        {
                            isExecuteRoom = true;
                        }
                    }
                }
                if (moduleData != null)
                {
                    if (serviceReqId > 0)
                        return new frmUpdateExamServiceReq(moduleData, serviceReqId, isExecuteRoom);
                    else
                        return new frmUpdateExamServiceReq(moduleData);
                }
                else
                {
                    return null;
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
