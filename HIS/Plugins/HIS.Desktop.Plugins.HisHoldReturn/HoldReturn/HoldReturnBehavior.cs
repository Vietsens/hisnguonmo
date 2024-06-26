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

namespace HIS.Desktop.Plugins.HisHoldReturn.HoldReturn
{
    public sealed class HoldReturnBehavior : Tool<IDesktopToolContext>, IHoldReturn
    {
        Inventec.Desktop.Common.Modules.Module Module;
        HoldReturnADO currentHoldReturnADO;

        public HoldReturnBehavior()
            : base()
        {
        }

        public HoldReturnBehavior(CommonParam param, Inventec.Desktop.Common.Modules.Module module, HoldReturnADO holdReturnADO)
            : base()
        {
            this.Module = module;
            this.currentHoldReturnADO = holdReturnADO;
        }

        object IHoldReturn.Run()
        {
            try
            {
                return new UCHoldReturn(this.Module, this.currentHoldReturnADO);
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
