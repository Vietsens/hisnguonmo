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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignServiceEdit.AssignServiceEdit
{
    class AssignServiceEditBehavior : Tool<IDesktopToolContext>, IAssignServiceEdit
    {
        object[] entity;

        Inventec.Desktop.Common.Modules.Module moduleData = null;
        HIS.Desktop.Common.RefeshReference _RefeshReference;
        long serviceReqId = 0;
        long instructionTime = 0;


        internal AssignServiceEditBehavior()
            : base()
        {

        }

        internal AssignServiceEditBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IAssignServiceEdit.Run()
        {
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        else if (item is AssignServiceEditADO)
                        {
                            this._RefeshReference = ((AssignServiceEditADO)item).DelegateRefeshReference;
                            this.serviceReqId = ((AssignServiceEditADO)item).serviceReqId;
                            this.instructionTime = ((AssignServiceEditADO)item).instructionTime;
                        }
                    }
                }
                if (moduleData != null && this.serviceReqId > 0)
                {
                    return new FormAssignServiceEdit(serviceReqId, instructionTime, _RefeshReference, moduleData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
