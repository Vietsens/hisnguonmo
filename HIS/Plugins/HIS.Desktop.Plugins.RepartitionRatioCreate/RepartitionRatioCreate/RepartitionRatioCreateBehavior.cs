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
using HTC.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RepartitionRatioCreate.RepartitionRatioCreate
{
    class RepartitionRatioCreateBehavior : Tool<IDesktopToolContext>, IRepartitionRatioCreate
    {
        Inventec.Desktop.Common.Modules.Module Module;
        HTC_PERIOD period = null;
        public RepartitionRatioCreateBehavior()
            : base()
        {
        }

        public RepartitionRatioCreateBehavior(Inventec.Desktop.Common.Modules.Module module, HTC_PERIOD data, CommonParam param)
            : base()
        {
            this.Module = module;
            this.period = data;
        }

        object IRepartitionRatioCreate.Run()
        {
            try
            {
                return new frmRepartitionRatioCreate(this.Module, period);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
