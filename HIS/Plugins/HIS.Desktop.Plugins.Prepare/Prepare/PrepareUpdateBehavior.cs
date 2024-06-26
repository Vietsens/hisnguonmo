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
using HIS.Desktop.Plugins.Prepare;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.Prepare.Prepare
{
    public sealed class PrepareUpdateBehavior : Tool<IDesktopToolContext>, IPrepare
    {
        HIS_PREPARE prepare;
        Inventec.Desktop.Common.Modules.Module Module;
        DelegateRefreshData refeshData;
        public PrepareUpdateBehavior()
            : base()
        {
        }

        public PrepareUpdateBehavior(CommonParam param, HIS_PREPARE _prepare, Inventec.Desktop.Common.Modules.Module module, DelegateRefreshData _refeshData)
            : base()
        {
            this.prepare = _prepare;
            this.Module = module;
            this.refeshData = _refeshData;
        }

        object IPrepare.Run()
        {
            try
            {
                return new frmPrepare(this.Module, prepare, this.refeshData);
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
