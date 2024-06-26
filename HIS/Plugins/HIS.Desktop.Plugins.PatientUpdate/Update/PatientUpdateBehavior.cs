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
using HIS.Desktop.Plugins.PatientUpdate;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using System.Windows.Forms;
using HIS.Desktop.ADO;
using MOS.EFMODEL.DataModels;

namespace Inventec.Desktop.Plugins.PatientUpdate.Update
{
    public sealed class PatientUpdateBehavior : Tool<IDesktopToolContext>, IPatientUpdate
    {
        object[] entity;
        public PatientUpdateBehavior()
            : base()
        {
        }

        public PatientUpdateBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IPatientUpdate.Run()
        {
            object result = null;
            DelegateSelectData refeshReference = null;
            V_HIS_PATIENT patient = null;
            //V_HIS_TREATMENT treatment = null;
            long TreatmentId = 0;
            long PatientId = 0;
            Inventec.Desktop.Common.Modules.Module currentModule = null;
            try
            {
                if (entity != null && entity.Length > 0)
                {
                    for (int i = 0; i < entity.Length; i++)
                    {
                        if (entity[i] is V_HIS_PATIENT)
                        {
                            patient = (V_HIS_PATIENT)entity[i];
                        }
                        else if (entity[i] is DelegateSelectData)
                        {
                            refeshReference = (DelegateSelectData)entity[i];
                        }
                        else if (entity[i] is long && PatientId == 0)
                        {
                            PatientId = (long)entity[i];
                        }
                        else if (entity[i] is long)
                        {
                            TreatmentId = (long)entity[i];
                        }
                        else if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            currentModule = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                    }
                    if (patient != null)
                    {
                        result = new frmPatientUpdate(currentModule, patient, refeshReference);
                    }
                    if (TreatmentId != null && TreatmentId > 0 && PatientId > 0)
                    {
                        result = new frmPatientUpdate(currentModule,PatientId, TreatmentId, refeshReference);
                    }
                }
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
