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
using HIS.Desktop.Plugins.TestConnectDeviceSample.Base;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TestConnectDeviceSample.TestConnectDeviceSample
{
    class TestConnectDeviceSampleBehavior : Tool<IDesktopToolContext>, ITestConnectDeviceSample
    {
        Inventec.Desktop.Common.Modules.Module Module;

        internal TestConnectDeviceSampleBehavior()
            : base()
        {

        }

        internal TestConnectDeviceSampleBehavior(Inventec.Desktop.Common.Modules.Module module, CommonParam param)
            : base()
        {
            this.Module = module;
        }

        object ITestConnectDeviceSample.Run()
        {
            object result = null;
            try
            {
                result = new UCTestConnectDeviceSample(Module);
                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData("Module", Module));
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
