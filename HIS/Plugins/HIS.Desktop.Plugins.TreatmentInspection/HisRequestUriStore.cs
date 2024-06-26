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

namespace HIS.Desktop.Plugins.TreatmentInspection
{
    class HisRequestUriStore
    {
        internal const string HIS_TREATMENT_GETVIEW11 = "api/HisTreatment/GetView11";
        internal const string HIS_TREATMENT_OUT_OF_MEDI_RECORD_LIST = "api/HisTreatment/OutOfMediRecordList";
        internal const string HIS_TREATMENT_RECORD_INSPECTION_APPROVE = "api/HisTreatment/RecordInspectionApprove";
        internal const string HIS_TREATMENT_RECORD_INSPECTION_UN_APPROVE = "api/HisTreatment/RecordInspectionUnapprove";
        internal const string HIS_TREATMENT_RECORD_INSPECTION_REJECT = "api/HisTreatment/RecordInspectionReject";
        internal const string HIS_TREATMENT_RECORD_INSPECTION_UN_REJECT = "api/HisTreatment/RecordInspectionUnreject";
        
        
    }
}
