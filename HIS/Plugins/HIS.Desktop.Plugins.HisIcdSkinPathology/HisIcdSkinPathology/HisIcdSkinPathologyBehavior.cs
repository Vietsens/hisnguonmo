using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisIcdSkinPathology.HisIcdSkinPathology
{
    class HisIcdSkinPathologyBehavior: BusinessBase, IHisIcdSkinPathology
    {
        object[] entity;
        internal HisIcdSkinPathologyBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }
        object IHisIcdSkinPathology.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                if (entity.GetType() == typeof(object[]))
                {
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                        }
                    }
                }
                return new frmHisIcdSkinpathology(moduleData);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                param.HasException = true;
                return null;
            }
        }
    }
}
