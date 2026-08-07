using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Pharmacology
{
    class PharmacologyBehavior : BusinessBase, IPharmacology
    {
        object[] entity;

        internal PharmacologyBehavior(CommonParam param, object[] filter)
            : base(param)
        {
            this.entity = filter;
        }

        /// <summary>
        /// Tra ve form danh muc duoc ly, module data lay tu args cua ModuleLink
        /// </summary>
        object IPharmacology.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
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

                return new frmPharmacology(moduleData);
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
