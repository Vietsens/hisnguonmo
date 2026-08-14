/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using Inventec.Core;

namespace HIS.Desktop.Plugins.VlgPortalLookup.VlgPortalLookup
{
    class VlgPortalLookupFactory
    {
        internal static IVlgPortalLookup MakeIVlgPortalLookup(CommonParam param, object[] data)
        {
            IVlgPortalLookup result = null;
            try
            {
                result = new VlgPortalLookupBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory không khởi tạo được đối tượng."
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data),
                    ex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}
