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
using HIS.Desktop.Plugins.TreatmentLog;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.Plugins.TreatmentLog;
using HIS.Desktop.ADO;


namespace Inventec.Desktop.Plugins.TreatmentLog.TreatmentLog
{
    public sealed class TreatmentLogBehavior : Tool<IDesktopToolContext>, ITreatmentLog
    {
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module ModuleData;
        long CurrentId = 0;
        public TreatmentLogBehavior()
            : base()
        {
        }

        public TreatmentLogBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module moduleData, TreatmentLogADO TreatmentADO)
            : base()
        {
            this.treatmentId = TreatmentADO.TreatmentId;
            this.ModuleData = moduleData;
            this.CurrentId = TreatmentADO.RoomId;
        }

        object ITreatmentLog.Run()
        {
            try
            {
                return new frmTreatmentLog(ModuleData, treatmentId, CurrentId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
