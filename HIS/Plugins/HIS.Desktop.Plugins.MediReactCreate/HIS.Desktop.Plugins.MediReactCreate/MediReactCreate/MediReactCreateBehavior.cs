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
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.Plugins.MediReactCreate;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;


using HIS.Desktop.ADO;
using HIS.Desktop.Plugins.MediReactCreate.MediReactCreate;


namespace Inventec.Desktop.Plugins.MediReactCreate.MediReactCreate
{
 public sealed class MediReactCreateBehavior : Tool<IDesktopToolContext>, IMediReactCreate
 {
  MediReactCreateADO data = null;
  Inventec.Desktop.Common.Modules.Module moduleData = null;
  public MediReactCreateBehavior()
   : base()
  {
  }

  public MediReactCreateBehavior(CommonParam param,Inventec.Desktop.Common.Modules.Module moduleData, MediReactCreateADO data)
   : base()
  {
   this.data = data;
   this.moduleData = moduleData;
  }

  object IMediReactCreate.Run()
  {
   try
   {
    return new frmMediReactCreate(moduleData, data);
   }
   catch (Exception ex)
   {
    Inventec.Common.Logging.LogSystem.Error(ex);
    //param.HasException = true;
    return null;
   }
  }
 }
}
