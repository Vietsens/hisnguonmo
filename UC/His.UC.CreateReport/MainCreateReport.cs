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
using His.UC.CreateReport.Init;
//using His.UC.CreateReport.Set.Delegate.CreateReport;
using His.UC.CreateReport.Set.Validation.Department;
using His.UC.CreateReport.Set.Validation.PatientType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace His.UC.CreateReport
{
    public partial class MainCreateReport
    {
        public enum TemplateEnum
        {
            TEMPLATE_TIME_FROM_TO,
            TEMPLATE_TIME_DEAPRTMENT,
            TEMPLATE_TIME_TREATMENT_TYPE,
            TEMPLATE_TIME_PATIENT_TYPE,
            TEMPLATE_TIME_DEPARTMENT_TREATMENT_TYPE,
            TEMPLATE_TIME_DEPARTMENT_ROOM,
            TEMPLATE_TIME_PATIENT_TYPE_TREATMENT_TYPE
        }

        //public UserControl Init(MainCreateReport.TemplateEnum Template, Data.InitData data)
        //{
        //    UserControl result = null;
        //    try
        //    {
        //        result = InitUCFactory.MakeIInitUC(data).Init(Template);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        result = null;
        //    }
        //    return result;
        //}

        //public bool SetDelegateCreateReport(UserControl UC, ProcessCreateReport createReport)
        //{
        //    bool result = false;
        //    try
        //    {
        //        result = SetProcessCreateReportFactory.MakeISetProcessCreateReport(createReport).Set(UC);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //        result = false;
        //    }
        //    return result;
        //}

        //public void SetValidateDepartment(UserControl UC)
        //{
        //    try
        //    {
        //        SetValidDepartmentFactory.MakeISetValidDepartment().Set(UC);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //public void SetValidatePatientType(UserControl UC)
        //{
        //    try
        //    {
        //        SetValidPatientTypeFactory.MakeISetValidPatientType().Set(UC);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
