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
//using His.UC.CreateReport.Init;
//using His.UC.CreateReport.Set.Delegate.CreateReport;
//using His.UC.CreateReport.Set.Validation.Department;
//using His.UC.CreateReport.Set.Validation.PatientType;
using HIS.UC.CreateReport.Base;
using Inventec.Core;
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
        public UserControl Generate(CommonParam param, object data)
        {
            UserControl result = null;
            try
            {
                IDelegacy delegacy = new CreateReportGenerate(param, data);
                result = delegacy.Execute() as UserControl;
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
