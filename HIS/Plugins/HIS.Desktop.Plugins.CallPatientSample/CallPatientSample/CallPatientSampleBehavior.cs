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
using HIS.Desktop.Plugins.CallPatientSample;
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
using MOS.SDO;

namespace HIS.Desktop.Plugins.CallPatientSample.CallPatientSample
{
    public sealed class CallPatientSampleBehavior : Tool<IDesktopToolContext>, ICallPatientSample
    {
        object[] entity;
        public CallPatientSampleBehavior()
            : base()
        {

        }

        public CallPatientSampleBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object ICallPatientSample.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                foreach (var item in entity)
                {
                    if (item is Inventec.Desktop.Common.Modules.Module)
                    {
                        module = (Inventec.Desktop.Common.Modules.Module)item;
                    }
                }
                return new frmChooseRoomForWaitingScreen(module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
