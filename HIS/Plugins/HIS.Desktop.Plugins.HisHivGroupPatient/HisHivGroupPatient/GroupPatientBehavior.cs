using HIS.Desktop.Common;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisHivGroupPatient.HisHivGroupPatient
{
    class GroupPatientBehavior : BusinessBase, IGroupPatient
    {
        object[] entity;
        internal GroupPatientBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IGroupPatient.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduledata = null;
                if (entity.GetType() == typeof(object))
                {   
                    if (entity != null && entity.Count() > 0)
                    {
                        for (int i = 0; i < entity.Count(); i++)
                        {
                            if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                            {
                                moduledata = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            }
                        }
                    }
                }
                return new frmHisHivGroupPatient(moduledata);
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
