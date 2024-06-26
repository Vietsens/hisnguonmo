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

namespace MPS.ADO
{
    public class DhstADOPrint
    {
        public string BELLY { get; set; }
        public string BLOOD_PRESSURE_MAX { get; set; }
        public string BLOOD_PRESSURE_MIN { get; set; }
        public string BREATH_RATE { get; set; }
        public string CHEST { get; set; }
        public string HEIGHT { get; set; }
        public string PULSE { get; set; }
        public string TEMPERATURE { get; set; }
        public long TREATMENT_ID { get; set; }
        public string VIR_BMI { get; set; }
        public string VIR_BODY_SURFACE_AREA { get; set; }
        public string WEIGHT { get; set; }
    }
}
