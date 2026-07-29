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
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    /// <summary>
    /// PT-44730. Permission to change the patient type of service requests whose service is
    /// declared in HIS_SERVICE_DEFAULT_PATY. This screen converts the patient type of the whole
    /// treatment, so a row the account may not edit keeps its current patient type.
    /// </summary>
    public partial class frmPatientTypeAlter
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
        /// Whether the patient type of this service request row may be changed. The requester is the
        /// account written on the service request (TDL_REQUEST_LOGINNAME). Services without a rule
        /// are never restricted.
        /// </summary>
        private bool IsAllowEditPatientTypeByServiceConfig(V_HIS_SERE_SERV_4 item)
        {
            bool result = true;
            try
            {
                if (item == null) return true;

                var worker = this.GetServiceDefaultPatyWorker();
                if (worker == null || worker.IsEmpty) return true;

                result = worker.IsAllowEditPatientType(item.SERVICE_ID, item.TDL_REQUEST_LOGINNAME);
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
