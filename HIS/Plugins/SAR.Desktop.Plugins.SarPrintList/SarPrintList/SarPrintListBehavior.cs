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
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;

namespace SAR.Desktop.Plugins.SarPrintList
{
    public sealed class SarPrintListBehavior : Tool<IDesktopToolContext>, ISarPrintList
    {
        string jsonPrintId;
        Inventec.Desktop.Common.Modules.Module Module;
        SarPrintADO sarPrintADO;

        public SarPrintListBehavior()
            : base()
        {
        }

        public SarPrintListBehavior(CommonParam param, SarPrintADO sarPrintADO, Inventec.Desktop.Common.Modules.Module module)
            : base()
        {
            this.sarPrintADO = sarPrintADO;
            this.Module = module;
        }

        object ISarPrintList.Run()
        {
            try
            {
                return new frmSarPrintList(this.Module, sarPrintADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                //param.HasException = true;
                return null;
            }
        }
    }
}
