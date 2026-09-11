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
using HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteList;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PaanExecuteList
{
    /// <summary>
    /// Man hinh danh sach benh nhan cho xu ly Giai phau benh.
    ///
    /// LUU Y VE CAPTION: chuoi "Xu ly giai phau benh" duoi day CHI la gia tri
    /// mac dinh. Ten thuc te hien tren ribbon lay tu ACS_MODULE.MODULE_NAME
    /// trong CSDL, khong phai tu tham so nay.
    /// </summary>
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "HIS.Desktop.Plugins.PaanExecuteList",
       "Xử lý giải phẫu bệnh",
       "Danh mục",
       5,
       "mau-benh-pham.png",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]

    public class PaanExecuteListProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public PaanExecuteListProcessor()
        {
            param = new CommonParam();
        }

        public PaanExecuteListProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IPaanExecuteList behavior = PaanExecuteListFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Luon tra ve true.
        /// Ghi chu: ham nay thuc te KHONG duoc goi. Trong HIS.Desktop\Base\Menu.cs
        /// no chi duoc goi qua reflection tu hai ham IsVisible(Module) dong 302 va
        /// IsEnable(Module) dong 340, ma ca hai ham do deu khong co noi nao goi toi
        /// (ma chet). Van giu lai cho dung khuon cac plugin khac.
        /// </summary>
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
