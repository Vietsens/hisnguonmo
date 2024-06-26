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
using Inventec.Desktop.Common;
using HIS.Desktop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.AllergyCard
{
    class AllergyCardBehavior : BusinessBase, IAllergyCard
    {
        object[] entity;
        internal AllergyCardBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IAllergyCard.Run()
        {
            try
            {
                object result = null;
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                long? treatmentId = null;
                V_HIS_PATIENT patient = null;

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
                            if (entity[i] is long?)
                            {
                                treatmentId = (long?)entity[i];
                            }
                            if (entity[i] is V_HIS_PATIENT)
                            {
                                patient = (V_HIS_PATIENT)entity[i];
                            }
                        }
                    }
                }

                if (treatmentId != null && moduleData != null)
                {
                    result = new frmAllergyCard(moduleData, (long)treatmentId);
                }
                else if (patient != null && moduleData != null)
                {
                    result = new frmAllergyCard(moduleData, patient);
                }
                return result;
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
