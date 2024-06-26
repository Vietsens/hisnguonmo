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
using MPS.ProcessorBase;

namespace MPS.Processor.Mps000008
{
    class Mps000008ExtendSingleKey : CommonKey
    {
        internal const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        internal const string CLOSE_TIME_SEPARATE_STR = "CLOSE_TIME_SEPARATE_STR";
        internal const string TREATMENT_CODE_BAR = "TREATMENT_CODE_BAR";
        internal const string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";

        internal const string IS_HEIN = "IS_HEIN";
        internal const string IS_VIENPHI = "IS_VIENPHI";
        internal const string STR_HEIN_CARD_FROM_TIME = "STR_HEIN_CARD_FROM_TIME";
        internal const string STR_HEIN_CARD_TO_TIME = "STR_HEIN_CARD_TO_TIME";
        internal const string HEIN_CARD_NUMBER_1 = "HEIN_CARD_NUMBER_1";
        internal const string HEIN_CARD_NUMBER_2 = "HEIN_CARD_NUMBER_2";
        internal const string HEIN_CARD_NUMBER_3 = "HEIN_CARD_NUMBER_3";
        internal const string HEIN_CARD_NUMBER_4 = "HEIN_CARD_NUMBER_4";
        internal const string HEIN_CARD_NUMBER_5 = "HEIN_CARD_NUMBER_5";
        internal const string HEIN_CARD_NUMBER_6 = "HEIN_CARD_NUMBER_6";
        internal const string HEIN_CARD_NUMBER_SEPARATE = "HEIN_CARD_NUMBER_SEPARATE";
        internal const string HEIN_CARD_ADDRESS = "HEIN_CARD_ADDRESS";
        internal const string TIME_IN_STR = "TIME_IN_STR";
        internal const string TIME_IN_FULL_STR = "TIME_IN_FULL_STR";

        internal const string METHOD = "METHOD";

        internal const string ADDRESS = "ADDRESS";
        internal const string AGE = "AGE";
        internal const string CAREER_NAME = "CAREER_NAME";
        internal const string DOB = "DOB";
        internal const string GENDER_NAME = "GENDER_NAME";
        internal const string MILITARY_RANK_NAME = "MILITARY_RANK_NAME";
        internal const string NATIONAL_NAME = "NATIONAL_NAME";
        internal const string PATIENT_CODE = "PATIENT_CODE";
        internal const string STR_DOB = "STR_DOB";
        internal const string STR_YEAR = "STR_YEAR";
        internal const string VIR_ADDRESS = "VIR_ADDRESS";
        internal const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        internal const string WORK_PLACE = "WORK_PLACE";
        internal const string WORK_PLACE_NAME = "WORK_PLACE_NAME";
        internal const string SURG_DOCTOR_NAME = "SURG_DOCTOR_NAME";

        internal const string END_DEPARTMENT_HEAD_LOGINNAME = "END_DEPARTMENT_HEAD_LOGINNAME";
        internal const string END_DEPARTMENT_HEAD_USERNAME = "END_DEPARTMENT_HEAD_USERNAME";
        internal const string HOSPITAL_DIRECTOR_LOGINNAME = "HOSPITAL_DIRECTOR_LOGINNAME";
        internal const string HOSPITAL_DIRECTOR_USERNAME = "HOSPITAL_DIRECTOR_USERNAME";
        internal const string END_DEPT_SUBS_HEAD_LOGINNAME = "END_DEPT_SUBS_HEAD_LOGINNAME";
        internal const string END_DEPT_SUBS_HEAD_USERNAME = "END_DEPT_SUBS_HEAD_USERNAME";
        internal const string HOSP_SUBS_DIRECTOR_LOGINNAME = "HOSP_SUBS_DIRECTOR_LOGINNAME";
        internal const string HOSP_SUBS_DIRECTOR_USERNAME = "HOSP_SUBS_DIRECTOR_USERNAME";
    }
}
