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
using HIS.Desktop.Common;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisMedicineTypeAcin.HisMedicineTypeAcin
{
    class HisMedicineTypeAcinBehavior : Tool<IDesktopToolContext>, IHisMedicineTypeAcin
    {
        object[] entity;
        Inventec.Desktop.Common.Modules.Module currentModule;
        DelegateReturnMutilObject resultActiveIngredient;
        List<V_HIS_MEDICINE_TYPE_ACIN> listMedicineTypeAcin;
        internal HisMedicineTypeAcinBehavior()
            : base()
        {

        }

        internal HisMedicineTypeAcinBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object IHisMedicineTypeAcin.Run()
        {
            object result = null;
            long medicineTypeId = 0;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is long)
                        {
                            medicineTypeId = (long)entity[i];
                        }
                        else if (entity[i] is DelegateReturnMutilObject)
                        {
                            resultActiveIngredient = (DelegateReturnMutilObject)entity[i];
                        }

                        else if (entity[i] is List<V_HIS_MEDICINE_TYPE_ACIN>)
                        {
                            listMedicineTypeAcin = (List<V_HIS_MEDICINE_TYPE_ACIN>)entity[i];
                        }
                    }
                    if (resultActiveIngredient != null && listMedicineTypeAcin!=null)
                    {
                        result = new frmHisMedicineTypeAcin(medicineTypeId, resultActiveIngredient, listMedicineTypeAcin);
                    }
                    else
                    {
                        result = new frmHisMedicineTypeAcin(medicineTypeId);
                    }
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
