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
using HIS.Desktop.ADO;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public sealed class AssignPrescriptionBehavior : Tool<IDesktopToolContext>, IAssignPrescription
    {
        AssignPrescriptionADO assignPrescriptionADO;
        Inventec.Desktop.Common.Modules.Module Module;
        public AssignPrescriptionBehavior()
            : base()
        {
        }

        public AssignPrescriptionBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param, AssignPrescriptionADO data)
            : base()
        {
            this.assignPrescriptionADO = data;
            this.Module = module;
        }

        object IAssignPrescription.Run()
        {
            try
            {
                return new frmAssignPrescription(this.Module, assignPrescriptionADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
