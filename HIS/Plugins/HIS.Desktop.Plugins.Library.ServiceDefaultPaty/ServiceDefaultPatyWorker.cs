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
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.Plugins.Library.ServiceDefaultPaty.Config;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.Library.ServiceDefaultPaty
{
    /// <summary>
    /// PT-44730. Loads the default patient type configuration of services once, then answers
    /// the three questions the screens need:
    ///   - which patient type must be filled for a service (assign screen only),
    ///   - whether a service is declared in the configuration,
    ///   - whether the current account may edit the patient type of that service request.
    ///
    /// One instance per form: create it in the Load event and keep it in a field.
    /// When the configuration is empty (feature not used, or backend not deployed yet)
    /// IsEmpty is true and every caller falls back to the current behaviour.
    /// </summary>
    public class ServiceDefaultPatyWorker
    {
        /// <summary>Rules grouped by service — O(1) lookup while filling the service grid.</summary>
        private Dictionary<long, List<HIS_SERVICE_DEFAULT_PATY>> dicByService;

        private long editOption;

        private string currentLoginName;

        private bool isAdmin;

        /// <summary>
        /// Services already reported as holding an unusable rule. The lookup runs once per service
        /// row, so without this the same warning would repeat for every row and every grid reload.
        /// </summary>
        private HashSet<long> loggedUnusableServiceIds = new HashSet<long>();

        /// <summary>True when no rule is declared — every feature of PT-44730 stays off.</summary>
        public bool IsEmpty
        {
            get { return this.dicByService == null || this.dicByService.Count == 0; }
        }

        public ServiceDefaultPatyWorker()
        {
            try
            {
                this.currentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.isAdmin = CheckLoginAdmin.IsAdmin(this.currentLoginName);
                this.editOption = ServiceDefaultPatyCFG.GetEditOption();
                this.LoadData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Loads the active rules from the backend. A null response leaves the worker empty,
        /// so the calling screen keeps running exactly as before.
        /// </summary>
        private void LoadData()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisServiceDefaultPatyFilter filter = new HisServiceDefaultPatyFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var data = new BackendAdapter(param).Get<List<HIS_SERVICE_DEFAULT_PATY>>(
                    ServiceDefaultPatyUriStore.MOSHIS_HIS_SERVICE_DEFAULT_PATY_GET,
                    ApiConsumers.MosConsumer,
                    filter,
                    param);

                if (data == null || data.Count == 0) return;

                this.dicByService = data
                    .Where(o => o.IS_DELETE != IMSys.DbConfig.HIS_RS.COMMON.IS_DELETE__TRUE
                                && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .GroupBy(o => o.SERVICE_ID)
                    .ToDictionary(o => o.Key, o => o.ToList());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Returns the patient type declared for the service, or null when no rule matches
        /// or the declared patient type is not usable for this treatment.
        ///
        /// The declared patient type is only returned when it is present in allowedPatientTypeIds,
        /// so a rule pointing at a patient type the patient is not entitled to — or one the service
        /// has no price for — is ignored and the screen keeps its current behaviour.
        /// Returning null therefore always means "leave the existing flow alone".
        /// </summary>
        /// <param name="serviceId">Service being assigned</param>
        /// <param name="patientTypeId">Current patient type of the treatment</param>
        /// <param name="primaryPatientTypeId">Additional (co-payment) patient type of the treatment, 0 or null when none</param>
        /// <param name="allowedPatientTypeIds">Patient types the patient is entitled to AND the service
        /// has a price for. Null or empty means nothing can be verified, so nothing is filled.</param>
        public long? GetDefaultPatientTypeId(long serviceId, long? patientTypeId, long? primaryPatientTypeId, List<long> allowedPatientTypeIds)
        {
            long? result = null;
            try
            {
                if (allowedPatientTypeIds == null || allowedPatientTypeIds.Count == 0) return null;

                var rule = this.GetMatchedRule(serviceId, patientTypeId, primaryPatientTypeId);
                if (rule == null) return null;

                if (!allowedPatientTypeIds.Contains(rule.DEFAULT_PATIENT_TYPE_ID))
                {
                    // Add returns false once the service has already been reported — one warning per service
                    if (this.loggedUnusableServiceIds.Add(serviceId))
                    {
                        Inventec.Common.Logging.LogSystem.Warn(string.Format(
                            "PT-44730. Bo qua cau hinh DTTT mac dinh: SERVICE_ID={0}, DEFAULT_PATIENT_TYPE_ID={1} khong nam trong danh sach DTTT ap dung duoc.",
                            serviceId, rule.DEFAULT_PATIENT_TYPE_ID));
                    }
                    return null;
                }

                result = rule.DEFAULT_PATIENT_TYPE_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        /// <summary>
        /// Picks the rule to apply: a rule with more filled conditions wins over a rule leaving
        /// them empty; on a tie the rule created last wins.
        /// </summary>
        private HIS_SERVICE_DEFAULT_PATY GetMatchedRule(long serviceId, long? patientTypeId, long? primaryPatientTypeId)
        {
            HIS_SERVICE_DEFAULT_PATY result = null;
            try
            {
                if (this.IsEmpty) return null;

                List<HIS_SERVICE_DEFAULT_PATY> rules;
                if (!this.dicByService.TryGetValue(serviceId, out rules) || rules == null) return null;

                result = rules
                    .Where(o => this.IsMatchedPatientType(o.PATIENT_TYPE_ID, patientTypeId)
                                && this.IsMatchedPatientType(o.PRIMARY_PATIENT_TYPE_ID, primaryPatientTypeId))
                    .OrderByDescending(o => (o.PATIENT_TYPE_ID.HasValue ? 1 : 0) + (o.PRIMARY_PATIENT_TYPE_ID.HasValue ? 1 : 0))
                    .ThenByDescending(o => o.CREATE_TIME ?? 0)
                    .ThenByDescending(o => o.ID)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// A condition left empty on the rule matches every treatment, including a treatment
        /// without an additional patient type.
        /// </summary>
        private bool IsMatchedPatientType(long? ruleValue, long? treatmentValue)
        {
            if (!ruleValue.HasValue) return true;
            return treatmentValue.HasValue && treatmentValue.Value > 0 && ruleValue.Value == treatmentValue.Value;
        }

        /// <summary>True when the service has at least one active rule — the permission config only bites here.</summary>
        public bool IsServiceConfigured(long serviceId)
        {
            bool result = false;
            try
            {
                result = !this.IsEmpty && this.dicByService.ContainsKey(serviceId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Whether the logged in account may edit the patient type of a service request of this service.
        /// Services without a rule are never restricted.
        /// </summary>
        /// <param name="serviceId">Service of the service request row</param>
        /// <param name="requestLoginName">Requester written on the service request. Pass the current
        /// login name on screens that are creating the request.</param>
        public bool IsAllowEditPatientType(long serviceId, string requestLoginName)
        {
            bool result = true;
            try
            {
                if (!this.IsServiceConfigured(serviceId)) return true;
                if (this.isAdmin) return true;

                if (this.editOption == (long)EnumServiceDefaultPatyEditOption.OnlyAdmin)
                {
                    result = false;
                }
                else if (this.editOption == (long)EnumServiceDefaultPatyEditOption.RequestUserOrAdmin)
                {
                    result = !string.IsNullOrEmpty(requestLoginName)
                             && !string.IsNullOrEmpty(this.currentLoginName)
                             && requestLoginName.Trim() == this.currentLoginName.Trim();
                }
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
