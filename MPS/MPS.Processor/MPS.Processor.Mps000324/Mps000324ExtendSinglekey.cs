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

namespace MPS.Processor.Mps000324
{
    class Mps000324ExtendSingleKey : CommonKey
    {
        public const string PARENT_ORGANIZATION_NAME = "PARENT_ORGANIZATION_NAME";
        public const string ORGANIZATION_NAME = "ORGANIZATION_NAME";
        public const string EXECUTE_ROOM_NAME = "EXECUTE_ROOM_NAME";
        public const string VIR_PATIENT_NAME = "VIR_PATIENT_NAME";
        public const string GENDER_MALE = "GENDER_MALE";
        public const string GENDER_FEMALE = "GENDER_FEMALE";
        public const string DOB = "DOB_YEAR";
        public const string AGE = "AGE";
        public const string PROVINCE_NAME = "PROVINCE_NAME";
        public const string WORK_PLACE = "WORK_PLACE";
        public const string OPEN_TIME_SEPARATE_STR = "OPEN_TIME_SEPARATE_STR";
        public const string START_TIME_STR = "START_TIME_STR";
        public const string FINISH_TIME_STR = "FINISH_TIME_STR";
        public const string DEPARTMENT_NAME = "DEPARTMENT_NAME";
        public const string LOGIN_NAME_EXECUTE_ROLE_MAIN = "LOGIN_NAME_EXECUTE_ROLE_MAIN";
        public const string USERNAME_EXECUTE_ROLE_MAIN = "USERNAME_NAME_EXECUTE_ROLE_MAIN";
        public const string LOGIN_NAME_EXECUTE_ROLE_TT = "LOGIN_NAME_EXECUTE_ROLE_TT";
        public const string USERNAME_EXECUTE_ROLE_TT = "USERNAME_NAME_EXECUTE_ROLE_TT";
        public const string LOGIN_NAME_EXECUTE_ROLE_PM1 = "LOGIN_NAME_EXECUTE_ROLE_PM1";
        public const string USERNAME_EXECUTE_ROLE_PM1 = "USERNAME_EXECUTE_ROLE_TT1";
        public const string LOGIN_NAME_EXECUTE_ROLE_PM2 = "LOGIN_NAME_EXECUTE_ROLE_PM2";
        public const string USERNAME_EXECUTE_ROLE_PM2 = "USERNAME_EXECUTE_ROLE_PM2";
        public const string LOGIN_NAME_EXECUTE_ROLE_PME1 = "LOGIN_NAME_EXECUTE_ROLE_PME1";
        public const string USERNAME_EXECUTE_ROLE_PME1 = "USERNAME_EXECUTE_ROLE_PME1";
        public const string LOGIN_NAME_EXECUTE_ROLE_PME2 = "LOGIN_NAME_EXECUTE_ROLE_PME2";
        public const string USERNAME_EXECUTE_ROLE_PME2 = "USERNAME_EXECUTE_ROLE_PME2";
        public const string LOGIN_NAME_EXECUTE_ROLE_GMHS = "LOGIN_NAME_EXECUTE_ROLE_GMHS";
        public const string USERNAME_EXECUTE_ROLE_GMHS = "USERNAME_EXECUTE_ROLE_GMHS";
        public const string LOGIN_NAME_EXECUTE_ROLE_GV = "LOGIN_NAME_EXECUTE_ROLE_GV";
        public const string USERNAME_EXECUTE_ROLE_GV = "USERNAME_EXECUTE_ROLE_GV";
    }
}
