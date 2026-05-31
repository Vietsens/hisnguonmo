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
using System;

namespace HIS.Desktop.Plugins.HisPatientPackage
{
    class HisPatientPackageFactory
    {
        internal static IHisPatientPackage MakeIControl(CommonParam param, object[] data)
        {
            IHisPatientPackage result = null;
            try
            {
                result = new HisPatientPackageBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory khong khoi tao duoc doi tuong."
                    + (data != null ? data.GetType().ToString() : "null")
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
                result = null;
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
