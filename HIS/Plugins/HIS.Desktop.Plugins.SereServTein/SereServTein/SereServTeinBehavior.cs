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
using HIS.Desktop.Plugins.SereServTein;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.SereServTein.SereServTein
{
    public sealed class SereServTeinBehavior : Tool<IDesktopToolContext>, ISereServTein
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        public SereServTeinBehavior()
            : base()
        {
        }

        public SereServTeinBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ISereServTein.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    MOS.EFMODEL.DataModels.HIS_SERE_SERV sereserv = null;
                    foreach (var item in entity)
                    {
                        if (item is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                        }
                        else if (item is long)
                        {
                            sereserv = new MOS.EFMODEL.DataModels.HIS_SERE_SERV();
                            sereserv.ID = (long)item;
                        }
                        else if (item is MOS.EFMODEL.DataModels.HIS_SERE_SERV)
                        {
                            sereserv = (MOS.EFMODEL.DataModels.HIS_SERE_SERV)item;
                        }
                        if (currentModule != null && sereserv != null)
                        {
                            result = new frmSereServTein(currentModule, sereserv);
                            break;
                        }
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
