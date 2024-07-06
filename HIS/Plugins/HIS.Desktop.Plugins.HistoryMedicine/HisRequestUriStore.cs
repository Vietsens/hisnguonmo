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

namespace HIS.Desktop.Plugins.HistoryMedicine
{
    class HisRequestUriStore
    {
        internal const string MOSHIS_PATIENT_PROGRAM_CREATE = "api/HistoryMedicine/Create";
        internal const string MOSHIS_PATIENT_PROGRAM_DELETE = "api/HistoryMedicine/Delete";
        internal const string MOSHIS_PATIENT_PROGRAM_UPDATE = "api/HistoryMedicine/Update";
        internal const string MOSHIS_PATIENT_PROGRAM_GET = "api/HistoryMedicine/Get";
        internal const string MOSHIS_PATIENT_PROGRAM_GETVIEW = "api/HistoryMedicine/GetView";
        internal const string MOSHIS_PATIENT_PROGRAM_CHANGE_LOCK = "api/HistoryMedicine/ChangeLock";
    }
}