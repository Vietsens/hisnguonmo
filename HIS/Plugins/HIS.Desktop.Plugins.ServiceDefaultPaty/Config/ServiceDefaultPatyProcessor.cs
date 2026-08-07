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

namespace HIS.Desktop.Plugins.ServiceDefaultPaty.Config
{
    /// <summary>
    /// PT-44730. Entry point of the screen "Thiết lập đối tượng thanh toán cho dịch vụ",
    /// opened from the Thiết lập menu next to the screen of the ancillary services.
    /// </summary>
    [ExtensionOf(typeof(DesktopRootExtensionPoint), "HIS.Desktop.Plugins.ServiceDefaultPaty", "Khác", "Bussiness", 8, "thanh-toan.png", "", Module.MODULE_TYPE_ID__FORM, true, true)]
    class ServiceDefaultPatyProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public ServiceDefaultPatyProcessor()
        {
            param = new CommonParam();
        }

        public ServiceDefaultPatyProcessor(CommonParam paramBussiness)
        {
            param = paramBussiness == null ? new CommonParam() : paramBussiness;
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IServiceDefaultPatyStore behavior = ServiceDefaultPatyFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            bool result = false;
            try
            {
                result = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
