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
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty;

namespace HIS.Desktop.Plugins.Bordereau
{
    /// <summary>
    /// PT-44730. Permission to edit the patient type of the rows whose service is declared in
    /// HIS_SERVICE_DEFAULT_PATY. Applied per row: a row of a service without a rule stays editable.
    /// </summary>
    public partial class frmBordereau
    {
        /// <summary>Configuration of PT-44730, loaded once per form.</summary>
        private ServiceDefaultPatyWorker serviceDefaultPatyWorker;

        private ServiceDefaultPatyWorker GetServiceDefaultPatyWorker()
        {
            try
            {
                if (this.serviceDefaultPatyWorker == null)
                    this.serviceDefaultPatyWorker = new ServiceDefaultPatyWorker();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return this.serviceDefaultPatyWorker;
        }

        /// <summary>
        /// Whether the patient type cell of this row may be edited. The requester is the account
        /// written on the service request of the row (TDL_REQUEST_LOGINNAME).
        /// </summary>
        private bool IsAllowEditPatientTypeByServiceConfig(SereServADO data)
        {
            bool result = true;
            try
            {
                if (data == null) return true;

                var worker = this.GetServiceDefaultPatyWorker();
                if (worker == null || worker.IsEmpty) return true;

                result = worker.IsAllowEditPatientType(data.SERVICE_ID, data.TDL_REQUEST_LOGINNAME);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = true;
            }
            return result;
        }
    }
}
