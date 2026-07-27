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
using HIS.Desktop.Plugins.CoordinationServiceReqCLS.CoordinationServiceReqCLS;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;

namespace HIS.Desktop.Plugins.CoordinationServiceReqCLS
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.CoordinationServiceReqCLS",
        "Điều phối cận lâm sàng",
        "Common",
        68,
        "y-lenh.png",
        "A",
        Module.MODULE_TYPE_ID__UC,
        true,
        true)]
    public class CoordinationServiceReqCLSProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public CoordinationServiceReqCLSProcessor()
        {
            param = new CommonParam();
        }

        public CoordinationServiceReqCLSProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            Inventec.Common.Logging.LogSystem.Info("begin load CoordinationServiceReqCLS");
            object result = null;
            try
            {
                ICoordinationServiceReqCLS behavior = CoordinationServiceReqCLSFactory.MakeICoordinationServiceReqCLS(param, args);
                result = behavior != null ? (behavior.Run()) : null;
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
                result = false;
            }
            return result;
        }
    }
}
