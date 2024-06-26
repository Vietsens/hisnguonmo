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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;

namespace HIS.Desktop.Plugins.TreatmentPatientUpdate.TreatmentPatientUpdate
{
    class TreatmentPatientUpdateBehavior : Tool<IDesktopToolContext>, ITreatmentPatientUpdate
    {
        object[] entity;

        internal TreatmentPatientUpdateBehavior()
            : base()
        {

        }

        internal TreatmentPatientUpdateBehavior(CommonParam param, object[] data)
            : base()
        {
            entity = data;
        }

        object ITreatmentPatientUpdate.Run()
        {
            long result = 0;
            Inventec.Desktop.Common.Modules.Module moduleData = null;
            HIS.Desktop.Common.DelegateSelectData deleSelect = null;
            List<string> patientCode = null;
            try
            {
                if (entity != null && entity.Count() > 0)
                {
                    foreach (var item in entity)
                    {
                        if (item is long)
                            result = (long)item;
                        if (item is Inventec.Desktop.Common.Modules.Module)
                            moduleData = (Inventec.Desktop.Common.Modules.Module)item;
                        if (item is List<string>)
                            patientCode = (List<string>)item;
                        if (item is HIS.Desktop.Common.DelegateSelectData)
                            deleSelect = (HIS.Desktop.Common.DelegateSelectData)item;
                    }
                }
                if (moduleData != null && result > 0 && patientCode != null && deleSelect != null)
                {
                    return new FormTreatmentPatientUpdate(patientCode, result, moduleData, deleSelect);
                }
                else if (moduleData != null && result > 0 && patientCode == null)
                {
                    return new FormTreatmentPatientUpdate(result, moduleData);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
