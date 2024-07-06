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

namespace HIS.Desktop.Plugins.SetupMedicineTypeWithAcin.SetupMedicineTypeWithAcin
{
    class MedicineTypeWABehavior : Tool<IDesktopToolContext>, IMedicineTypeWA
    {
        object[] entity;
        long treatmentId;
        Inventec.Desktop.Common.Modules.Module currentModule;

        internal MedicineTypeWABehavior()
            : base()
        {

        }

        internal MedicineTypeWABehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IMedicineTypeWA.Run()
        {
            object result = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                //    foreach (var item in entity)
                //    {
                //        if (item is long)
                //        {
                //            treatmentId = (long)item;
                //        }
                //        else if (item is Inventec.Desktop.Common.Modules.Module)
                //        {
                //            currentModule = (Inventec.Desktop.Common.Modules.Module)item;
                //        }
                //        if (currentModule != null && treatmentId > 0)
                //        {
                //            result = new UCRoomAccount(currentModule, treatmentId);
                //            break;
                //        }
                //    }
                    result = new UC_SetupMedicineTypeWithAcin();
                }
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