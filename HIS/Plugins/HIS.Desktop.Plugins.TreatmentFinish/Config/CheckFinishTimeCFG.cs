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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.LocalStorage.SdaConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Config
{
    class CheckFinishTimeCFG
    {
        private const string MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG = "MOS.HIS_TREATMENT.MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT";
        private const string MUST_EXPORT_BEFORE_OUT_OF_DEPARTMENT_WITH_STAY_IN_PATIENT_CFG = "MOS.HIS_PRESCRIPTION.MUST_EXPORT_BEFORE_OUT_OF_DEPARTMENT_WITH_STAY_IN_PATIENT";
        private const string CHECK_ASSIGN_SERVICE_BED = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_ASSIGN_SERVICE_BED";
        private const string SERVICE_CODE__AUTO_FINISH = "MOS.HIS_TREATMENT.AUTO_FINISH_SERVICE_REQ.SERVICE_CODE";
        private const string IS_CHECK = "1";
        private const string IS_CHECK_BETTER_THAN = "2";
        internal const string CHECK_SAME_HEIN = "HIS.DESKTOP.TREATMENT_FINISH.CHECK_SAME_HEIN";
        internal const string UNSIGNDOCUMENT = "HIS.Desktop.Plugins.TreatmentFinish.UnsignDocument";
        private const string CFG_MUST_APPROVE_RATION = "MOS.HIS_TREATMENT.FINISH.MUST_APPROVE_ALL_RATION";
        private const string WARNING_OPTION_IN_CASE_OF_UNASSIGN_TRACKING_SERVICE_REQ = "HIS.Desktop.Plugins.TransDepartment.WarningOptionInCaseOfUnassignTrackingServiceReq";
        private const string IS_NOT_SHOW_OUT_MEDI_AND_MATE = "HIS.Desktop.Plugins.TrackingPrint.IsNotShowOutMediAndMate";

        internal static bool isCheckBedService;
        internal static bool isCheckSameHein;
        internal static bool mustFinishAllServicesBeforeFinishTreatment;
        internal static bool mustExportBeforeOutOfDepartmentWithStayInPatient;
        internal static List<long> autoFinishServiceIds;
        internal static bool isWarningApproveRation;
        internal static string WarningOptionInCaseOfUnassignTrackingServiceReq;
        internal static string IsNotShowOutMediAndMate;

        internal static void GetConfig()
        {
            try
            {
                isCheckBedService = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECK_ASSIGN_SERVICE_BED) == IS_CHECK;
                isCheckSameHein = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECK_SAME_HEIN) == IS_CHECK;
                mustFinishAllServicesBeforeFinishTreatment = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MUST_FINISH_ALL_SERVICES_BEFORE_FINISH_TREATMENT_CFG) == IS_CHECK;
                mustExportBeforeOutOfDepartmentWithStayInPatient = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(MUST_EXPORT_BEFORE_OUT_OF_DEPARTMENT_WITH_STAY_IN_PATIENT_CFG) == IS_CHECK;
                isWarningApproveRation = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CFG_MUST_APPROVE_RATION) == IS_CHECK;

                string listCode = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(SERVICE_CODE__AUTO_FINISH);
                if (!string.IsNullOrEmpty(listCode))
                {
                    var codes = listCode.Split(';');
                    autoFinishServiceIds = GetIds(codes);
                }
                WarningOptionInCaseOfUnassignTrackingServiceReq = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(WARNING_OPTION_IN_CASE_OF_UNASSIGN_TRACKING_SERVICE_REQ);
                IsNotShowOutMediAndMate = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IS_NOT_SHOW_OUT_MEDI_AND_MATE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static List<long> GetIds(string[] value)
        {
            List<long> result = new List<long>();
            try
            {
                if (value != null && value.Count() > 0)
                {
                    List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => value.Contains(o.SERVICE_CODE)).ToList();
                    if (services != null && services.Count > 0)
                        result = services.Select(s => s.ID).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<long>();
            }
            return result;
        }
    }
}
