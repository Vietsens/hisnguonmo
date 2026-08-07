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
using HIS.Desktop.Plugins.AssignServiceEdit.ADO;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty;

namespace HIS.Desktop.Plugins.AssignServiceEdit
{
    /// <summary>
    /// PT-44730. Permission to edit the patient type of a service request whose service is
    /// declared in HIS_SERVICE_DEFAULT_PATY. This screen never fills the default patient type —
    /// it only decides whether the cell may be edited.
    /// </summary>
    public partial class FormAssignServiceEdit
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
        /// written on the service request being edited. Services without a rule are never restricted.
        /// </summary>
        private bool IsAllowEditPatientTypeByServiceConfig(HisSereServADO data)
        {
            bool result = true;
            try
            {
                if (data == null) return true;

                var worker = this.GetServiceDefaultPatyWorker();
                if (worker == null || worker.IsEmpty) return true;

                string requestLoginName = this.HisServiceReq != null ? this.HisServiceReq.REQUEST_LOGINNAME : "";
                result = worker.IsAllowEditPatientType(data.SERVICE_ID, requestLoginName);
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
