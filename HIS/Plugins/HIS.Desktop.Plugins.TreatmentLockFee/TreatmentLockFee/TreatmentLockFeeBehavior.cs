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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.TreatmentLockFee;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentLockFee.TreatmentLockFee
{
    public sealed class TreatmentLockFeeBehavior : Tool<IDesktopToolContext>, ITreatmentLockFee
    {
        Inventec.Desktop.Common.Modules.Module moduleData;
        long treatmentId;
        public TreatmentLockFeeBehavior()
            : base()
        {
        }

        public TreatmentLockFeeBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module module, long data)
            : base()
        {
            moduleData = module;
            treatmentId = data;
        }

        object ITreatmentLockFee.Run()
        {
            object result = null;
            try
            {
                result = new frmTreatmentLockFee(moduleData, treatmentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
        }
    }
}
