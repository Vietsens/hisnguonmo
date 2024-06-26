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
using SDA.Desktop.Plugins.SdaField;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDA.Desktop.Plugins.SdaField
{

        [ExtensionOf(typeof(DesktopRootExtensionPoint),
   "SDA.Desktop.Plugins.SdaField",
   "Chọn phòng",
   "Common",
   0,
   "chon-phong.png",
   "A",
   Module.MODULE_TYPE_ID__FORM,
   true,
   true)
    ]

        public class SdaFieldProcessor : ModuleBase, IDesktopRoot
        {
            CommonParam param;
            public SdaFieldProcessor()
            {
                param = new CommonParam();
            }
            public SdaFieldProcessor(CommonParam paramBusiness)
            {
                param = (paramBusiness != null ? paramBusiness : new CommonParam());
            }

            public object Run(object[] args)
            {
                object result = null;
                try
                {
                    ISdaField behavior = SdaFieldFactory.MakeIControl(param, args);
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
                return true;
            }
        }
    }

