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
using HIS.Desktop.Plugins.PackingMaterial;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.PackingMaterial.PackingMaterial
{
    public sealed class PackingMaterialUpdateBehavior : Tool<IDesktopToolContext>, IPackingMaterial
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS_DISPENSE _Dispense;

        public PackingMaterialUpdateBehavior()
            : base()
        {
        }

        public PackingMaterialUpdateBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module moduleData, HIS_DISPENSE dispense)
            : base()
        {
            this.moduleData = moduleData;
            this._Dispense = dispense;
        }

        object IPackingMaterial.Run()
        {
            try
            {
                return new frmPackingMaterial(this.moduleData, this._Dispense);
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
