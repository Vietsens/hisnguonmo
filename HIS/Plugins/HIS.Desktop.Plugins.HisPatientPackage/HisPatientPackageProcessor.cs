/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using System;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    /// <summary>
    /// Entry point MEF cho module Gói dịch vụ bệnh nhân.
    /// - Mở từ menu (không tham số nghiệp vụ) -> màn 6.2 Danh sách gói (UserControl).
    /// - Mở kèm thông tin gói/bệnh nhân + action (Add/Edit) -> màn 6.1 Đăng ký/Sửa gói (Form).
    ///   Việc định tuyến do Behavior xử lý.
    /// </summary>
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
        "HIS.Desktop.Plugins.HisPatientPackage",
        "Danh sách gói dịch vụ bệnh nhân",
        "Bussiness",
        14,
        "kham-suc-khoe.png",
        "A",
        Module.MODULE_TYPE_ID__FORM,
        true,
        true
        )
    ]
    public class HisPatientPackageProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;

        public HisPatientPackageProcessor()
        {
            param = new CommonParam();
        }

        public HisPatientPackageProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IHisPatientPackage behavior = HisPatientPackageFactory.MakeIControl(param, args);
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
