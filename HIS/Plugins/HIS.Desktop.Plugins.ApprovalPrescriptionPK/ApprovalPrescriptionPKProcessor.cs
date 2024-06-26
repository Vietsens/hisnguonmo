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

namespace HIS.Desktop.Plugins.ApprovalPrescriptionPK
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.ApprovalPrescriptionPK",
        "Duyệt tổng hợp đơn phòng khám",
        "Common",
        190,
        "xuat-kho.png",
        "A",
        Module.MODULE_TYPE_ID__COMBO,
        true,
        true)]
    public class ApprovalPrescriptionPKProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ApprovalPrescriptionPKProcessor()
        {
            param = new CommonParam();
        }
        public ApprovalPrescriptionPKProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        object IDesktopRoot.Run(object[] args)
        {
            object result = null;
            try
            {
                ApprovalPrescriptionPK.IApprovalPrescriptionPK behavior = ApprovalPrescriptionPK.ApprovalPrescriptionPKFactory.MakeIApprovalExportPrescription(param, args);
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
            return false;
        }
    }
}
