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

namespace HIS.Desktop.Plugins.BloodTransfusion.BloodTransfusion
{
    class BloodTransfusionBehavior : Tool<IDesktopToolContext>, IBloodTransfusion
    {
        string treatmentCode;
        Inventec.Desktop.Common.Modules.Module Module;

        internal BloodTransfusionBehavior(Inventec.Desktop.Common.Modules.Module moduleData)
            : base()
        {
            Module = moduleData;
        }

        internal BloodTransfusionBehavior(Inventec.Desktop.Common.Modules.Module moduleData, CommonParam param, string _treatmentCode)
            : base()
        {
            Module = moduleData;
            treatmentCode = _treatmentCode;
        }

        object IBloodTransfusion.Run()
        {
            object result = null;
            try
            {

                result = !string.IsNullOrEmpty(treatmentCode) ? new frmBloodTransfusion(Module, treatmentCode) : new frmBloodTransfusion(Module);
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
