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
    /// PT-44730. View of the configuration rule, with code/name of the service and of the three patient types.
    /// TEMPORARY: replace by MOS.EFMODEL.DataModels.V_HIS_SERVICE_DEFAULT_PATY after backend gencode.
    /// </summary>
    public class ServiceDefaultPatyViewDTO : ServiceDefaultPatyDTO
    {
        public string SERVICE_CODE { get; set; }

        public string SERVICE_NAME { get; set; }

        public string PATIENT_TYPE_CODE { get; set; }

        public string PATIENT_TYPE_NAME { get; set; }

        public string PRIMARY_PATIENT_TYPE_CODE { get; set; }

        public string PRIMARY_PATIENT_TYPE_NAME { get; set; }

        public string DEFAULT_PATIENT_TYPE_CODE { get; set; }

        public string DEFAULT_PATIENT_TYPE_NAME { get; set; }
    }
}
