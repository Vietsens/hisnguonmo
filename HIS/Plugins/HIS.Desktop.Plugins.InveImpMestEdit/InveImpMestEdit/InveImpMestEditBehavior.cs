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
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.InveImpMestEdit.InveImpMestEdit
{
    class InveImpMestEditBehavior : Tool<IDesktopToolContext>, IInveImpMestEdit
    {
        object[] entity;

        internal InveImpMestEditBehavior()
            : base()
        { }

        internal InveImpMestEditBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IInveImpMestEdit.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long impMestId = 0;
                HIS.Desktop.Common.RefeshReference RefeshReference = null;
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
                            impMestId = (long)entity[i];
                        }
                        else if (entity[i] is HIS.Desktop.Common.RefeshReference)
                        {
                            RefeshReference = (HIS.Desktop.Common.RefeshReference)entity[i];
                        }
                    }
                }
                if (moduleData != null && impMestId > 0)
                {
                    return new FormInveImpMestEdit(moduleData, impMestId, RefeshReference);
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
