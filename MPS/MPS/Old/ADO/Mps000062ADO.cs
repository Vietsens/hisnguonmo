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
    public class Mps000062ADO : MOS.EFMODEL.DataModels.HIS_TRACKING
    {
        public string TRACKING_TIME_STR { get; set; }

        public string TRACKING_DATE_STR { get; set; }

        public string TRACKING_DATE_SEPARATE_STR { get; set; }

        public long? REMEDY_COUNT { get; set; }

        public string ICD_NAME_TRACKING { get; set; }

        public decimal? BELLY { get; set; }
        public long? BLOOD_PRESSURE_MAX { get; set; }
        public long? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public decimal? CHEST { get; set; }
        public decimal? HEIGHT { get; set; }
        public long? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? VIR_BMI { get; set; }
        public decimal? VIR_BODY_SURFACE_AREA { get; set; }
        public decimal? WEIGHT { get; set; }
    }
}
