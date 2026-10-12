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

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Màn hình chờ thu ngân 3: danh sách chờ nộp viện phí (họ tên, năm sinh, nhóm dịch vụ)
    /// + thanh "ĐANG GỌI" nhận lượt gọi từ màn Viện phí. Mở từ máy thu ngân lên màn hình mở rộng.
    /// PTTK_XXXXX_Man_Hinh_Cho_Phong_Thu_Ngan_BVND_115
    /// </summary>
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.CallPatientCashierV3",
       "Màn hình chờ thu ngân 3",
       "popup",
       14,
       "CallPatient_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true
       )
    ]
    public class CallPatientCashierV3Processor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public CallPatientCashierV3Processor()
        {
            param = new CommonParam();
        }

        public CallPatientCashierV3Processor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                ICallPatientCashierV3 behavior = CallPatientCashierV3Factory.MakeICallPatientCashierV3(param, args);
                result = behavior != null ? behavior.Run() : null;
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
