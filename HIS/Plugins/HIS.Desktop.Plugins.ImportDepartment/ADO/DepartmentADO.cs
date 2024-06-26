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

namespace HIS.Desktop.Plugins.ImportDepartment.ADO
{
    public class DepartmentADO : MOS.EFMODEL.DataModels.HIS_DEPARTMENT
    {
        public string BRANCH_CODE { get; set; }
        public bool IsClinical { get; set; }
        public string IsClinicalStr { get; set; }
        public bool IsAutoReceivePatient { get; set; }
        public string IsAutoReceivePatientStr { get; set; }
        public string NumOrderStr { get; set; }
        public string TheoryPatientCountStr { get; set; }
        public string Error { get; set; }

        public string AllowTreatmentTypeCodes { get; set; }
        public string ALLOW_TREATMENT_TYPE_NAMEs { get; set; }
    }
}
