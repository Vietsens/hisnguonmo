using Inventec.Core;
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.KskInfomantionOfficials
{
    class KskInfomantionOfficialsBehavior : BusinessBase, IKskInfomantionOfficials
    {
        object[] entity;
        internal KskInfomantionOfficialsBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IKskInfomantionOfficials.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                string serviceReqCode = null;

                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                            continue;
                        }

                        if (entity[i] is string)
                        {
                            serviceReqCode = entity[i].ToString();
                            continue;
                        }

                        if (entity[i] is List<object> list && list.Count > 0)
                        {
                            var code = list.FirstOrDefault(x => x is string);
                            if (code != null)
                                serviceReqCode = code.ToString();
                        }
                    }
                }

                return new frmKskInfomantionOfficials(moduleData, serviceReqCode);
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
