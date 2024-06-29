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
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AncillaryServicePaty.Config
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint), "HIS.Desktop.Plugins.AncillaryServicePaty", "Khác", "Bussiness", 8, "thanh-toan.png", "", Module.MODULE_TYPE_ID__UC, true, true)]
    class AncillaryServicePatyProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public AncillaryServicePatyProcessor()
        {
            param = new CommonParam();
        }
        public AncillaryServicePatyProcessor(CommonParam paramBussiness)
        {
            param = paramBussiness == null ? new CommonParam() : paramBussiness;
        }
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IAncillaryServicePatyStore behavior = AncillaryServicePatyFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
                return result;
            }
            catch (Exception e)
            {
                Inventec.Common.Logging.LogSystem.Warn(e);
                return result;
            }
        }
        public override bool IsEnable()
        {
            bool result = false;
            try
            {
                result = true;
            }
            catch (Exception e)
            {

                return result;
            }
            return result;
        }
    }
}
