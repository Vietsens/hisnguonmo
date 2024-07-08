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
using System.Windows.Forms;

namespace His.UC.CreateReport.Init
{
    class InitUC : IInitUC
    {
        private Data.InitData Data { get; set; }

        internal InitUC(Data.InitData data)
        {
            Data = data;
        }

        System.Windows.Forms.UserControl IInitUC.Init(MainCreateReport.TemplateEnum Template)
        {
            UserControl result = null;
            try
            {
                if (Data != null)
                {
                    if (Template == MainCreateReport.TemplateEnum.TEMPLATE_TIME_FROM_TO)
                    {
                        result = new Design.TimeFromAndTo.TimeFromAndTo(Data);
                    }
                    else if (Template == MainCreateReport.TemplateEnum.TEMPLATE_TIME_DEAPRTMENT)
                    {
                        result = new Design.TimeAndDepartment.TimeAndDepartment(Data);
                    }
                    else if (Template == MainCreateReport.TemplateEnum.TEMPLATE_TIME_TREATMENT_TYPE)
                    {
                        result = new Design.TimeAndTreatmentType.TimeAndTreatmentType(Data);
                    }
                    else if (Template == MainCreateReport.TemplateEnum.TEMPLATE_TIME_PATIENT_TYPE)
                    {
                        result = new Design.TimeAndPatientType.TimeAndPatientType(Data);
                    }
                    else if (Template == MainCreateReport.TemplateEnum.TEMPLATE_TIME_DEPARTMENT_TREATMENT_TYPE)
                    {
                        result = new Design.TimeDepartmentAndTreatmentType.TimeDepartmentAndTreatmentType(Data);
                    }
                    else if (Template== MainCreateReport.TemplateEnum.TEMPLATE_TIME_DEPARTMENT_ROOM)
                    {
                        result = new Design.TimeDepartmentAndRoom.TimeDepartmentAndRoom(Data);
                    }
                    else if (Template== MainCreateReport.TemplateEnum.TEMPLATE_TIME_PATIENT_TYPE_TREATMENT_TYPE)
                    {
                        result = new Design.TimePatientTypeAndTreatmentType.TimePatientTypeAndTreatmentType(Data);
                    }

                    if (result == null)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Template), Template));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Data), Data));
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
