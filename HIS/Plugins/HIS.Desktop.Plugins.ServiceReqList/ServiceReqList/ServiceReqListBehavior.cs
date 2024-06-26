/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqList.ServiceReqList
{
    class ServiceReqListBehavior : Tool<IDesktopToolContext>, IServiceReqList
    {
        object[] entity;

        internal ServiceReqListBehavior()
            : base()
        {

        }

        internal ServiceReqListBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IServiceReqList.Run()
        {
            object result = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module module = null;
                MOS.EFMODEL.DataModels.HIS_TREATMENT treatment = null;
                MOS.EFMODEL.DataModels.V_HIS_PATIENT patient = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is MOS.EFMODEL.DataModels.HIS_TREATMENT)
                        {
                            treatment = (MOS.EFMODEL.DataModels.HIS_TREATMENT)entity[i];
                        }
                        else if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            module = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        else if (entity[i] is MOS.EFMODEL.DataModels.V_HIS_PATIENT)
                        {
                            patient = (MOS.EFMODEL.DataModels.V_HIS_PATIENT)entity[i];
                        }
                    }
                }
                if (module != null)
                {
                    if (treatment != null)
                    {
                        result = new frmServiceReqList(module, treatment);
                    }
                    else if (patient != null)
                    {
                        result = new frmServiceReqList(module, patient);
                    }
                    else
                        result = new frmServiceReqList(module);
                }
                else
                    result = null;

                if (result == null) throw new NullReferenceException(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => entity), entity));
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
