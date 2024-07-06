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

namespace His.UC.CreateReport.Set.Delegate.CreateReport
{
    class SetProcessCreateReport : ISetProcessCreateReport
    {
        private ProcessCreateReport Data { get; set; }

        internal SetProcessCreateReport(ProcessCreateReport data)
        {
            Data = data;
        }

        bool ISetProcessCreateReport.Set(System.Windows.Forms.UserControl UC)
        {
            bool result = false;
            try
            {
                if (UC != null)
                {
                    if (UC.GetType() == typeof(Design.TimeFromAndTo.TimeFromAndTo))
                    {
                        Design.TimeFromAndTo.TimeFromAndTo ucCreateReport = (Design.TimeFromAndTo.TimeFromAndTo)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimeAndDepartment.TimeAndDepartment))
                    {
                        Design.TimeAndDepartment.TimeAndDepartment ucCreateReport = (Design.TimeAndDepartment.TimeAndDepartment)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimeAndTreatmentType.TimeAndTreatmentType))
                    {
                        Design.TimeAndTreatmentType.TimeAndTreatmentType ucCreateReport = (Design.TimeAndTreatmentType.TimeAndTreatmentType)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimeAndPatientType.TimeAndPatientType))
                    {
                        Design.TimeAndPatientType.TimeAndPatientType ucCreateReport = (Design.TimeAndPatientType.TimeAndPatientType)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimeDepartmentAndTreatmentType.TimeDepartmentAndTreatmentType))
                    {
                        Design.TimeDepartmentAndTreatmentType.TimeDepartmentAndTreatmentType ucCreateReport = (Design.TimeDepartmentAndTreatmentType.TimeDepartmentAndTreatmentType)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimeDepartmentAndRoom.TimeDepartmentAndRoom))
                    {
                        Design.TimeDepartmentAndRoom.TimeDepartmentAndRoom ucCreateReport = (Design.TimeDepartmentAndRoom.TimeDepartmentAndRoom)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }
                    else if (UC.GetType() == typeof(Design.TimePatientTypeAndTreatmentType.TimePatientTypeAndTreatmentType))
                    {
                        Design.TimePatientTypeAndTreatmentType.TimePatientTypeAndTreatmentType ucCreateReport = (Design.TimePatientTypeAndTreatmentType.TimePatientTypeAndTreatmentType)UC;
                        result = ucCreateReport.SetDelegateCreateReport(Data);
                    }

                    if (!result)
                    {
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => UC), UC));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}
