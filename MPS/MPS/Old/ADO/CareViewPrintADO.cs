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
    public class CareViewPrintADO
    {
        public CareViewPrintADO() { }

        public long? AWARENESS_ID { get; set; }
        public string CARE_CODE { get; set; }
        public string DEJECTA { get; set; }
        public long? DHST_ID { get; set; }
        public string EDUCATION { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public long? EXECUTE_TIME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string GROUP_CODE { get; set; }
        public short? HAS_ADD_MEDICINE { get; set; }
        public short? HAS_MEDICINE { get; set; }
        public short? HAS_TEST { get; set; }
        public long ID { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_DELETE { get; set; }
        public string MODIFIER { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string MUCOCUTANEOUS { get; set; }
        public string NUTRITION { get; set; }
        public string SANITARY { get; set; }
        public long TREATMENT_ID { get; set; }
        public string TUTORIAL { get; set; }
        public string URINE { get; set; }

        public string AWARENESS_NAME { get; set; }

        public decimal? BLOOD_PRESSURE_MAX { get; set; }
        public decimal? BLOOD_PRESSURE_MIN { get; set; }
        public decimal? BMI { get; set; }
        public decimal? BODY_SURFACE_AREA { get; set; }
        public decimal? BREATH_RATE { get; set; }
        public string DHST_CODE { get; set; }
        public decimal? HEIGHT { get; set; }
        public decimal? PULSE { get; set; }
        public decimal? TEMPERATURE { get; set; }
        public decimal? WEIGHT { get; set; }

        public string EXECUTE_TIME_DISPLAY { get; set; }

        public string CARE_TITLE1 { get; set; }
        public string CARE_TITLE2 { get; set; }
        public string CARE_1 { get; set; }
        public string CARE_2 { get; set; }
        public string CARE_3 { get; set; }
        public string CARE_4 { get; set; }
        public string CARE_5 { get; set; }
        public string CARE_6 { get; set; }
        public string CARE_7 { get; set; }
        public string CARE_8 { get; set; }
        public string CARE_9 { get; set; }
        public string CARE_10 { get; set; }
        public string CARE_11 { get; set; }
        public string CARE_12 { get; set; }

    }
}
