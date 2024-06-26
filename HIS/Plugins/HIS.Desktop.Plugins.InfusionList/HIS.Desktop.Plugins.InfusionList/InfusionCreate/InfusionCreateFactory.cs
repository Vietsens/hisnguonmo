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
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.InfusionCreate;
using HIS.Desktop.Plugins.InfusionCreate.InfusionCreate;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Inventec.Desktop.Plugins.InfusionCreate.InfusionCreate
{
 class InfusionCreateFactory
 {
  internal static IInfusionCreate MakeIInfusionCreate(CommonParam param, object[] data)
  {
   IInfusionCreate result = null;
   Inventec.Desktop.Common.Modules.Module moduleData = null;
   InfusionCreateADO InfusionCreateADO = null;
   long InfusionSumId = 0;
   try
   {
    //#region Test
    //InfusionCreateADO = new InfusionCreateADO();
    //InfusionCreateADO.InfusionSumId = 421;
    //InfusionCreateADO.treatmentId = 4630;
    //#endregion
    if (data.GetType() == typeof(object[]))
    {
     if (data != null && data.Count() > 0)
     {
      for (int i = 0; i < data.Count(); i++)
      {
       if (data[i] is long)
       {
        InfusionSumId = (long)data[i];
       }
       else if (data[i] is Inventec.Desktop.Common.Modules.Module)
       {
        moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
       }
      }

      if (moduleData != null && InfusionSumId != 0)
      {
       InfusionCreateADO = new InfusionCreateADO();
       InfusionCreateADO.InfusionSumId = InfusionSumId;

       HisInfusionFilter filter = new HisInfusionFilter();
       filter.ID = InfusionSumId;



       InfusionCreateADO.treatmentId = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_INFUSION_SUM>>("/api/HisInfusionSum/Get", ApiConsumers.MosConsumer, filter, param).First().TREATMENT_ID;
       result = new InfusionCreateBehavior(param, moduleData, (InfusionCreateADO)InfusionCreateADO);
      }

     }
    }

    if (result == null) throw new NullReferenceException();
   }
   catch (NullReferenceException ex)
   {
    Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
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
