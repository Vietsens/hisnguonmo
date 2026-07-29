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

namespace HIS.Desktop.Plugins.Library.ServiceDefaultPaty.ADO
{
    /// <summary>
    /// PT-44730. Configuration rule of default patient type (payment object) of a service.
    /// TEMPORARY: replace by MOS.EFMODEL.DataModels.HIS_SERVICE_DEFAULT_PATY after backend gencode.
    /// </summary>
    public class ServiceDefaultPatyDTO
    {
        public long ID { get; set; }

        /// <summary>Condition: service the rule applies to. Required.</summary>
        public long SERVICE_ID { get; set; }

        /// <summary>Condition: patient type of the treatment. Null = match every patient type.</summary>
        public long? PATIENT_TYPE_ID { get; set; }

        /// <summary>Condition: additional (co-payment) patient type of the treatment. Null = match every case.</summary>
        public long? PRIMARY_PATIENT_TYPE_ID { get; set; }

        /// <summary>Result: default patient type filled into the service request. Required.</summary>
        public long DEFAULT_PATIENT_TYPE_ID { get; set; }

        public short? IS_ACTIVE { get; set; }

        public short? IS_DELETE { get; set; }

        public string GROUP_CODE { get; set; }

        public long? CREATE_TIME { get; set; }

        public long? MODIFY_TIME { get; set; }

        public string CREATOR { get; set; }

        public string MODIFIER { get; set; }

        public string APP_CREATOR { get; set; }

        public string APP_MODIFIER { get; set; }
    }
}
