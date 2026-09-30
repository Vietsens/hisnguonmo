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

namespace MPS.Processor.Mps000138
{
    class Mps000138ExtendSingleKey : CommonKey
    {
        internal const string REGISTER_TIME_STR = "REGISTER_TIME_STR";
        internal const string REGISTER_DATE_STR = "REGISTER_DATE_STR";
        internal const string LAST_CALLED_NUM_ORDER = "LAST_CALLED_NUM_ORDER";

        /// <summary>
        /// Ho ten nguoi benh xac dinh duoc tai buoc lay so tren man ki-ot.
        /// Doc tu chuoi JSON luu kem ban ghi cap so - PTTK_54254 muc B.4.2.
        /// Rong khi so thu tu duoc lay theo duong khong dinh danh hoac go tay so dinh danh.
        /// </summary>
        internal const string IDENTITY_PATIENT_NAME = "IDENTITY_PATIENT_NAME";
    }
}
