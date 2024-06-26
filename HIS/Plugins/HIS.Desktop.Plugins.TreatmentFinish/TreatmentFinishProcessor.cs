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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;

namespace HIS.Desktop.Plugins.TreatmentFinish
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
           "HIS.Desktop.Plugins.TreatmentFinish",
           "Kết thúc điều trị",
           "Common",
           16,
           "morefunctions_32x32.png",
           "E",
           Module.MODULE_TYPE_ID__FORM,
           true,
           true)
       ]
    public class TreatmentFinishProcessor: ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public TreatmentFinishProcessor()
        {
            param = new CommonParam();
        }
        public TreatmentFinishProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            Inventec.Common.Logging.LogSystem.Error("TreatmentFinish begin");
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                TreatmentFinish.ITreatmentFinish behavior = TreatmentFinish.TreatmentFinishFactory.MakeITreatmentFinish(param, args);
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
            return true;
        }
    }
}
