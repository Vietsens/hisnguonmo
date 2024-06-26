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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Desktop.Common.Modules;

namespace HIS.Desktop.Plugins.HisATCSetUp.ATCSetUp
{
    class ATCSetUpBehavior : Tool<IDesktopToolContext>, IATCSetUp
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        DelegateReturnMutilObject resultAtc;
        List<HIS_ATC> listAtc;
        Module module;
        internal ATCSetUpBehavior()
            : base()
        {

        }

        internal ATCSetUpBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IATCSetUp.Run()
        {
            object result = null;

            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is List<HIS_ATC>)
                        {
                            listAtc = (List<HIS_ATC>)entity[i];
                        }
                        else if (entity[i] is DelegateReturnMutilObject)
                        {
                            resultAtc = (DelegateReturnMutilObject)entity[i];
                        }
                        else if (entity[i] is Module)
                        {
                            module = (Module)entity[i];
                        }
                    }
                    if (listAtc != null && resultAtc != null)
                    {
                        result = new frmATCSetUp(resultAtc, listAtc, module);
                    }
                    else
                    {
                        result = new frmATCSetUp();
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

