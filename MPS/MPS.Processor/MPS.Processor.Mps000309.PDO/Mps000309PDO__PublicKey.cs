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
using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000309.PDO
{
    public partial class Mps000309PDO : RDOBase
    {
        public V_HIS_PATIENT_TYPE_ALTER PatyAlter { get; set; }
        public HIS_TREATMENT Treatment { get; set; }
        public V_HIS_PATIENT _currentPatient { get; set; }
        public HIS_DHST DHST { get; set; }
    }

    public class PatientADO : V_HIS_PATIENT
    {
        public string VIR_ADDRESS { get; set; }
        public string VIR_PATIENT_NAME { get; set; }
        public long DOB { get; set; }
        public string AGE { get; set; }
        public string DOB_STR { get; set; }
        public string CMND_DATE_STR { get; set; }
        public string DOB_YEAR { get; set; }
        public string GENDER_MALE { get; set; }
        public string GENDER_FEMALE { get; set; }
    }

    public class PatyAlterBhytADO : V_HIS_PATIENT_TYPE_ALTER
    {
        public string HEIN_CARD_NUMBER_SEPARATE { get; set; }
        public string IS_HEIN { get; set; }
        public string IS_VIENPHI { get; set; }
        public string STR_HEIN_CARD_FROM_TIME { get; set; }
        public string STR_HEIN_CARD_TO_TIME { get; set; }
        public string RATIO { get; set; }
        public string HEIN_CARD_NUMBER_1 { get; set; }
        public string HEIN_CARD_NUMBER_2 { get; set; }
        public string HEIN_CARD_NUMBER_3 { get; set; }
        public string HEIN_CARD_NUMBER_4 { get; set; }
        public string HEIN_CARD_NUMBER_5 { get; set; }
        public string HEIN_CARD_NUMBER_6 { get; set; }
        public long TIME_IN_TREATMENT { get; set; }
    }
}
