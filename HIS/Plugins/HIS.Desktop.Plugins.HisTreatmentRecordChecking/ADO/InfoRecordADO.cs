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

namespace HIS.Desktop.Plugins.HisTreatmentRecordChecking.ADO
{
    class InfoRecordADO
    {
        public long DOCUMENT_TYPE_ID { get; set; }
        public string CODE { get; set; }
        public string TYPE { get; set; }
        public string CREATE_TIME_STR { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string SEARCH_CODE { get; set; }
        public long REQ_TYPE_STT_ID { get; set; }
        public string CREATOR { get; set; }

        #region Task 53180

        /// <summary>
        /// Record creation time. Used by the doctor filter (QT-05) and shown in the
        /// "Thoi gian tao" column, which is separate from CREATE_TIME_STR because the
        /// latter carries the business time and differs per document type.
        /// </summary>
        public long? CREATE_TIME { get; set; }

        /// <summary>Display string of CREATE_TIME. Bound to column Gv_IR_CreateTimeReal.</summary>
        public string CREATE_TIME_REAL_STR { get; set; }

        /// <summary>
        /// Document status of the order (QT-09). Computed BEFORE binding the grid -
        /// never inside CustomUnboundColumnData.
        /// </summary>
        public EnumRecordDocumentStatus DOC_STATUS { get; set; }

        /// <summary>Localised name of DOC_STATUS. Bound to column Gv_IR_DocStatus.</summary>
        public string DOC_STATUS_NAME { get; set; }

        /// <summary>Treatment id. Only filled in mode 2, used to jump back to mode 1.</summary>
        public long? TREATMENT_ID { get; set; }

        /// <summary>Treatment code. Only filled in mode 2.</summary>
        public string TREATMENT_CODE { get; set; }

        /// <summary>Patient code. Only filled in mode 2.</summary>
        public string PATIENT_CODE { get; set; }

        /// <summary>Patient name. Only filled in mode 2.</summary>
        public string PATIENT_NAME { get; set; }

        #endregion
    }
}
