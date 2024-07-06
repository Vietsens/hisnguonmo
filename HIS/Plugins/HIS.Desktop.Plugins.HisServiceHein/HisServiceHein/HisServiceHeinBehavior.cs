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

namespace HIS.Desktop.Plugins.HisServiceHein
{
    class HisServiceHeinBehavior : BusinessBase, IHisServiceHein
    {
        object[] entity;

        internal HisServiceHeinBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IHisServiceHein.Run()
        {
            object rs = null;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                V_HIS_SERVICE currentService = null;
                V_HIS_MEDICINE_TYPE currentMedicine = null;
                V_HIS_MATERIAL_TYPE currentMaterial = null;

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
                            else if (entity[i] is V_HIS_SERVICE)
                            {
                                currentService = (V_HIS_SERVICE)entity[i];
                            }
                            else if (entity[i] is V_HIS_MEDICINE_TYPE)
                            {
                                currentMedicine = (V_HIS_MEDICINE_TYPE)entity[i];
                            }
                            else if (entity[i] is V_HIS_MATERIAL_TYPE)
                            {
                                currentMaterial = (V_HIS_MATERIAL_TYPE)entity[i];
                            }
                        }
                    }
                }

                if (currentService == null && currentMedicine == null && currentMaterial == null && moduleData != null)
                    rs = new frmHisServiceHein(moduleData);
                else if (currentService != null && moduleData != null)
                    rs = new frmHisServiceHein(moduleData, currentService);
                else if (currentMedicine != null && moduleData != null)
                    rs = new frmHisServiceHein(moduleData, currentMedicine);
                else if (currentMaterial != null && moduleData != null)
                    rs = new frmHisServiceHein(moduleData, currentMaterial);

                return rs;
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