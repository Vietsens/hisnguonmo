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
using HIS.Desktop.Plugins.MediReactCreate;
using HIS.Desktop.Plugins.MediReactCreate.MediReactCreate;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Inventec.Desktop.Plugins.MediReactCreate.MediReactCreate
{
 class MediReactCreateFactory
 {
  internal static IMediReactCreate MakeIMediReactCreate(CommonParam param, object[] data)
  {
   IMediReactCreate result = null;
   Inventec.Desktop.Common.Modules.Module moduleData = null;
   MediReactCreateADO MediReactCreateADO = null;
   long MediReactSumId = 0;
   try
   {
    //#region Test
    //MediReactCreateADO = new MediReactCreateADO();
    //MediReactCreateADO.MediReactSumId = 1;
    //MediReactCreateADO.treatmentId = 4630;
    //#endregion
    if (data.GetType() == typeof(object[]))
    {
     if (data != null && data.Count() > 0)
     {
      for (int i = 0; i < data.Count(); i++)
      {
       if (data[i] is long)
       {
        MediReactSumId = (long)data[i];
       }
       else if (data[i] is Inventec.Desktop.Common.Modules.Module)
       {
        moduleData = (Inventec.Desktop.Common.Modules.Module)data[i];
       }
       }
      MediReactCreateADO = new MediReactCreateADO();
      MediReactCreateADO.MediReactSumId = MediReactSumId;
      if (moduleData != null && MediReactCreateADO != null)
      {
       
MOS.Filter.HisMediReactSumFilter hisMediReactSumFilter = new MOS.Filter.HisMediReactSumFilter();
hisMediReactSumFilter.ID = MediReactSumId;
MediReactCreateADO.treatmentId = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_MEDI_REACT_SUM>>("/api/HisMediReactSum/Get", ApiConsumers.MosConsumer, hisMediReactSumFilter, param).First().TREATMENT_ID;
       result = new MediReactCreateBehavior(param, moduleData,(MediReactCreateADO)MediReactCreateADO);
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
