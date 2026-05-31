/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.HisServiceConsult
{
    class HisServiceConsultBehavior : BusinessBase, IHisServiceConsult
    {
        object[] entity;

        internal HisServiceConsultBehavior(CommonParam param, object[] filter) : base()
        {
            this.entity = filter;
        }

        object IHisServiceConsult.Run()
        {
            try
            {
                Module moduleData = null;
                long? treatmentId = null;
                DelegateSelectData delegateSelect = null;

                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Module)
                        {
                            moduleData = (Module)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            treatmentId = (long)entity[i];
                        }
                        else if (entity[i] is DelegateSelectData)
                        {
                            delegateSelect = (DelegateSelectData)entity[i];
                        }
                    }
                }

                if (treatmentId == null || treatmentId <= 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("HisServiceConsultBehavior.Run: treatmentId không hợp lệ");
                    return null;
                }

                return new frmHisServiceConsult(moduleData, treatmentId.Value, delegateSelect);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                if (this.param != null) this.param.HasException = true;
                return null;
            }
        }
    }
}
