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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Inventec.Core;
using HIS.Desktop.Common;
using Inventec.Desktop.Core;
using Inventec.Desktop.Plugins.TYTFetusBornList.TYTFetusBornList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.LocalStorage.SdaConfig;

namespace Inventec.Desktop.Plugins.TYTFetusBornList
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "TYT.Desktop.Plugins.TYTFetusBornList",
       "Sổ sinh đẻ",
       "Common",
       14,
       "",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true
       )
    ]
    public class TYTFetusBornListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public TYTFetusBornListProcessor()
        {
            param = new CommonParam();
        }
        public TYTFetusBornListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                ITYTFetusBornList behavior = TYTFetusBornListFactory.MakeITYTFetusBornList(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Ham tra ve trang thai cua module la enable hay disable
        /// Ghi de gia tri khac theo nghiep vu tung module
        /// </summary>
        /// <returns>true/false</returns>
        public override bool IsEnable()
        {
            bool result = true;
            return result;
        }
    }
}
