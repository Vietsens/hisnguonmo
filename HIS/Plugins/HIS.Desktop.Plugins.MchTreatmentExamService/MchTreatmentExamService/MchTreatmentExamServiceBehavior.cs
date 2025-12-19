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
using HIS.Desktop.Plugins.MchTreatmentExamService.MainForm;
using Inventec.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Core.Tools;
using MCH.EFMODEL.DataModels;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.MchTreatmentExamService
{
    class MchTreatmentExamServiceBehavior : Tool<IDesktopToolContext>, IMchTreatmentExamService
    {
        object[] entity;
        internal MchTreatmentExamServiceBehavior(CommonParam param, object[] filter)
            : base()
        {
            this.entity = filter;
        }

        object IMchTreatmentExamService.Run()
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = null;
                HIS_TREATMENT Treatment = null;
                HIS.Desktop.Common.RefeshReference dlgRefresh = null;
                V_MCH_EXAM_SERVICE ExamService = null;
                if (entity != null && entity.Count() > 0)
                {
                    for (int i = 0; i < entity.Count(); i++)
                    {
                        if (entity[i] is Inventec.Desktop.Common.Modules.Module)
                        {
                            moduleData = (Inventec.Desktop.Common.Modules.Module)entity[i];
                        }
                        if (entity[i] is MOS.EFMODEL.DataModels.HIS_TREATMENT)
                        {
                            Treatment = (MOS.EFMODEL.DataModels.HIS_TREATMENT)entity[i];
                        }
                        if (entity[i] is MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE)
                        {
                            ExamService = (MCH.EFMODEL.DataModels.V_MCH_EXAM_SERVICE)entity[i];
                        }
                        if (entity[i] is HIS.Desktop.Common.RefeshReference)
                        {
                            dlgRefresh = (HIS.Desktop.Common.RefeshReference)entity[i];
                        }
                    }
                }
                if (moduleData != null)
                {
                    return new UCMchTreatmentExamService(moduleData, Treatment, ExamService, dlgRefresh);
                }
                else
                {
                    return null;
                }
                //return new UCMedicineList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
    }
}
