/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.HisServiceConsult
{
    class HisServiceConsultFactory
    {
        internal static IHisServiceConsult MakeIControl(CommonParam param, object[] data)
        {
            IHisServiceConsult result = null;
            try
            {
                result = new HisServiceConsultBehavior(param, data);
                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error(
                    "Factory không khởi tạo được đối tượng. Type="
                    + (data != null ? data.GetType().ToString() : "null")
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
