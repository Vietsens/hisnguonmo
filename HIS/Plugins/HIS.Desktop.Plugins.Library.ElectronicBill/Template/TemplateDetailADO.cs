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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Template
{
    class TemplateDetailADO
    {
        public string Display { get; set; }
        public string Unit { get; set; }
        public string ServiceCodes { get; set; }
        public string ParentServiceCodes { get; set; }
        public string ServiceTypeCodes { get; set; }
        public string HeinServiceTypeCodes { get; set; }
        public string TreatmentTypeCodes { get; set; }
        public string PatientTypeCodes { get; set; }

        public long? IsSplit { get; set; }
        public long? IsBHYT { get; set; }
        public long? NumOrder { get; set; }
        public long? IsSplitBhytPrice { get; set; }
        public long? Stt { get; set; }

        public List<long> ParentServiceIds { get; set; }
        public List<long> ServiceTypeIds { get; set; }
        public List<long> HeinServiceTypeIds { get; set; }
        public List<long> TreatmentTypeIds { get; set; }
        public List<long> PatientTypeIds { get; set; }
    }
}
