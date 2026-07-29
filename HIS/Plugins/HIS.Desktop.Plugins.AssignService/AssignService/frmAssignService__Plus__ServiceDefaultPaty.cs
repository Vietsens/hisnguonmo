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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// PT-44730. Default patient type declared per service by patient type and additional
    /// patient type (table HIS_SERVICE_DEFAULT_PATY), plus the permission to edit that cell.
    /// </summary>
    public partial class frmAssignService
    {
        /// <summary>Configuration of PT-44730, loaded once per form.</summary>
        private ServiceDefaultPatyWorker serviceDefaultPatyWorker;

        /// <summary>
        /// Loads the configuration on first use. An empty configuration keeps every caller
        /// on the current behaviour.
        /// </summary>
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
        /// Patient type declared for this service and this treatment, or null when no rule matches
        /// or the declared patient type is not usable here. Null always means "keep the current flow".
        /// </summary>
        /// <param name="allowedPatientTypeIds">Patient types the patient is entitled to and the service
        /// has a price for — the worker only returns a value present in this list.</param>
        private long? GetDefaultPatientTypeIdByServiceConfig(SereServADO sereServADO, List<long> allowedPatientTypeIds)
        {
            long? result = null;
            try
            {
                if (sereServADO == null || sereServADO.IsNotLoadDefaultPatientType) return null;

                var worker = this.GetServiceDefaultPatyWorker();
                if (worker == null || worker.IsEmpty) return null;

                long? patientTypeId = this.currentHisPatientTypeAlter != null
                    ? (long?)this.currentHisPatientTypeAlter.PATIENT_TYPE_ID : null;
                long? primaryPatientTypeId = (this.currentHisTreatment != null && this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID > 0)
                    ? (long?)this.currentHisTreatment.PRIMARY_PATIENT_TYPE_ID : null;

                result = worker.GetDefaultPatientTypeId(sereServADO.SERVICE_ID, patientTypeId, primaryPatientTypeId, allowedPatientTypeIds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Whether the patient type cell of this row may be edited. On this screen the requester is
        /// the logged in account, so option 2 always allows editing. Services without a rule are
        /// never restricted.
        /// </summary>
        private bool IsAllowEditPatientTypeByServiceConfig(SereServADO sereServADO)
        {
            bool result = true;
            try
            {
                if (sereServADO == null) return true;

                var worker = this.GetServiceDefaultPatyWorker();
                if (worker == null || worker.IsEmpty) return true;

                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                result = worker.IsAllowEditPatientType(sereServADO.SERVICE_ID, loginName);
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
