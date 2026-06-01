/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using HIS.Desktop.Common;
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    class HisPatientPackageBehavior : BusinessBase, IHisPatientPackage
    {
        object[] entity;

        internal HisPatientPackageBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisPatientPackage.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                    }
                }

                // Plugin nay = man 6.2 Danh sach goi -> tra UC de shell ghim tab (giong XML130).
                // Man 6.1 Dang ky/Sua goi la PLUGIN RIENG: HIS.Desktop.Plugins.PatientPackageRegister
                // (mo qua PluginInstance tu nut "Sua" trong list).
                return new UcHisPatientPackage(moduleData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
