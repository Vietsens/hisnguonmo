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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DiscountSereServ.DiscountSereServ
{
    class DiscountSereServBehavior : Tool<IDesktopToolContext>, IDiscountSereServ
    {
        Inventec.Desktop.Common.Modules.Module moduleData = null;
        HIS_SERE_SERV sereServ = null;

        internal DiscountSereServBehavior()
            : base()
        {

        }

        internal DiscountSereServBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, HIS_SERE_SERV data)
            : base()
        {
            sereServ = data;
            moduleData = module;
        }

        object IDiscountSereServ.Run()
        {
            object result = null;
            try
            {
                result = new frmDiscountSereServ(moduleData, sereServ);
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
