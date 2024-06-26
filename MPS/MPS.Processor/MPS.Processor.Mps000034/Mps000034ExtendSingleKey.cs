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
using MPS.ProcessorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000034
{
    public class Mps000034ExtendSingleKey : CommonKey
    {
			internal const string REQUEST_DEPARTMENT_CODE = "DEPARTMENT_CODE";
			internal const string REQUEST_DEPARTMENT_NAME = "DEPARTMENT_NAME";
			internal const string PATIENT_CODE_BAR = "PATIENT_CODE_BAR";
			internal const string REQUEST_DATE_STR = "REQUEST_DATE_STR";
			internal const string SERVICE_REQ_CODE = "SERVICE_REQ_CODE";
			internal const string EXECUTE_ROOM_NAME = "ROOM_NAME";
			internal const string NUM_ORDER = "NUM_ORDER";
			internal const string PATIENT_TYPE_NAME = "PATIENT_TYPE_NAME";
    }
}
