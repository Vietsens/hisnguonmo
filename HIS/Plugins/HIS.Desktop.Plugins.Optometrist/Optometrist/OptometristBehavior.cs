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
using MOS.EFMODEL.DataModels;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.Optometrist.Optometrist
{
    class OptometristBehavior : Tool<DesktopToolContext>, IOptometrist
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal OptometristBehavior()
            : base()
        {

        }

        public OptometristBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IOptometrist.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    HIS_SERE_SERV sereServ = null;
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is HIS_SERE_SERV)
                        {
                            sereServ = (HIS_SERE_SERV)item;
                        }
                    }

                    if (currentModule != null && sereServ != null)
                    {
                        result = new frmOptometrist(currentModule, sereServ);
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
