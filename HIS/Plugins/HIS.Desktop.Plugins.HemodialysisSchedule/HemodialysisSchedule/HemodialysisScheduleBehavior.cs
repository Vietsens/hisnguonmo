/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using HIS.Desktop.Common;
using Inventec.Core;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.HemodialysisSchedule
{
    class HemodialysisScheduleBehavior : BusinessBase, IHemodialysisSchedule
    {
        object[] entity;

        internal HemodialysisScheduleBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHemodialysisSchedule.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;

                if (entity != null && entity.GetType() == typeof(object[]) && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                    }
                }

                return new frmHemodialysisSchedule(moduleData);
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
