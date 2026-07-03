/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using Inventec.Core;
using System;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.HemodialysisSchedule
{
    class HemodialysisScheduleFactory
    {
        internal static IHemodialysisSchedule MakeIControl(CommonParam param, object[] data)
        {
            IHemodialysisSchedule result = null;
            try
            {
                result = new HemodialysisScheduleBehavior(param, data);

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong.", ex);
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
