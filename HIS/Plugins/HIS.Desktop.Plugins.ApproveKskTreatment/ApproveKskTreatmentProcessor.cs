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
using HIS.Desktop.Plugins.ApproveKskTreatment.ApproveKskTreatment;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;

namespace HIS.Desktop.Plugins.ApproveKskTreatment
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.ApproveKskTreatment",
       "Danh sách xuất tổng hợp",
       "Common",
       16,
       "phieu-linh.png",
       "C",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]

    public class ApproveKskTreatmentProcessor: ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ApproveKskTreatmentProcessor()
        {
            param = new CommonParam();
        }
        public ApproveKskTreatmentProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            CommonParam param = new CommonParam();
            object result = null;
            try
            {
                IApproveKskTreatment behavior = ApproveKskTreatmentFactory.MakeIHisAggrExpMestList(param, args);
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
            bool result = true;
            //try
            //{
            //    if (GlobalVariables.CurrentRoomTypeCode.Contains(SdaConfigs.Get<string>(Inventec.Common.LocalStorage.SdaConfig.ConfigKeys.DBCODE__HIS_RS__HIS_ROOM_TYPE__ROOM_TYPE_CODE__STOCK)))
            //    {
            //        result = true;
            //    }
            //    else
            //        result = false;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //    result = false;
            //}            
            return result;
        }
    }
}
