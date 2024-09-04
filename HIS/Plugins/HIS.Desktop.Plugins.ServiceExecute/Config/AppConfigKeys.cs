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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute.Config
{
    class AppConfigKeys
    {
        internal const string CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY = "CONFIG_KEY__MODULE_CAMERA__SHORTCUT_KEY";
        private const string AUTO_SELECT_IMAGE_CAPTURE_CFG = "CONFIG_KEY__MODULE_SERVICE_EXECUTE__AUTO_SELECT_IMAGE_CAPTURE";

        internal const string HIS_DESKTOP_SURGSERVICEREQEXECUTE_EXECUTE_ROLE_DEFAULT = "HIS.DESKTOP.PLUGINS.SURGSERVICEREQEXECUTE.EXECUTE_ROLE_DEFAULT";

        internal const string MOS__HIS_SERVICE_REQ__ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT = "MOS.HIS_SERVICE_REQ.ALLOW_UPDATE_SURG_INFO_AFTER_LOCKING_TREATMENT";

        internal const string IS_CHECKING_PERMISSON = "MOS.HIS_SERE_SERV_PTTT.IS_CHECKING_PERMISSON";

        internal const string CONFIG_KEY__Camera__IsSavingInLocal = "HIS.Desktop.Plugins.Camera.IsSavingInLocal";

        internal const string CONFIG_KEY__IsRequiredPtttPriority = "HIS.Desktop.Plugins.ServiceExecute.IsRequiredPtttPriority";
        internal const string CONFIG_KEY__IsHideConcludeAndNoteByDefault = "HIS.Desktop.Plugins.ServiceExecute.IsHideConcludeAndNoteByDefault";
        internal const string CONFIG_KEY__IsInitCameraDefault = "HIS.Desktop.Plugins.ServiceExecute.IsInitCameraDefault";
        internal const string CONFIG_KEY__IsMachineWarningOption = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.WARNING_OPTION";

        internal const string CONFIG_KEY__IsRequiredConclude = "HIS.Desktop.Plugins.ServiceExecute.IsRequiredConclude";

        internal const string CONFIG_KEY__StartTimeOption = "HIS.Desktop.Plugins.ServiceExecute.StartTimeOption";
        internal const string CONFIG_KEY__StartTimeMustBeGreaterThanInstructionTime = "HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime";
        internal const string CONFIG_KEY__ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR = "MOS.HIS_SERVICE_REQ.ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR";
        internal const string CONFIG_KEY__ALLOW_TO_EDIT_FINISHTIME = "HIS.Desktop.Plugins.ServiceExecute.AllowToEditFinishTimeGreaterThanCurrentTime";
        internal const string CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime = "HIS.Desktop.Plugins.ProcessTimeMustBeLessThanMaxTotalProcessTime";
        internal const string CONFIG_KEY__SAMPLE_INFO_OPTION = "HIS.HIS_SERVICE_REQ.SAMPLE_INFO_OPTION";
        internal const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        internal const string CONFIG_KEY__PATIENT_TYPE_OPTION = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.PATIENT_TYPE_OPTION";
        internal const string CONFIG_KEY__SERVICE_REQ_TYPE_CODE = "MOS.HIS_SERVICE_REQ.AUTO_ADD_EXCUTE_ROLE.SERVICE_REQ_TYPE_CODE";

        internal const string CONFIG_KEY__ASSIGN_SERVICE_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.ASSIGN_SERVICE_SIMULTANEITY_OPTION";
        internal const string CONFIG_KEY__CHECK_SIMULTANEITY_OPTION = "MOS.HIS_SERVICE_REQ.CHECK_SIMULTANEITY_OPTION";
        static MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE GetPatientTypeByCode(string code)
        {
            MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE result = new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
            try
            {
                result = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result ?? new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE();
        }
        internal static string ServiceReqTypeCode
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__SERVICE_REQ_TYPE_CODE);
            }
        }
        internal static List<long> ServiceReqTypeId;
        internal static List<long> GetServiceReqTypeId()
        {

            try
            {
                if (!string.IsNullOrEmpty(ServiceReqTypeCode))
                    return ServiceReqTypeId = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ_TYPE>().Where(o => ServiceReqTypeCode.Split(',').ToList().Exists(p => p.Equals(o.SERVICE_REQ_TYPE_CODE))).Select(o => o.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
        internal static string IsPatientTypeOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_OPTION);
            }
        }
        internal static long PatientTypeId__BHYT
        {
            get
            {
                return GetPatientTypeByCode(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).ID;
            }
        }

        internal static string IsSampleInfoOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__SAMPLE_INFO_OPTION);
            }
        }
        internal static string IsProcessTimeMustBeGreaterThanTotalProcessTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ProcessTimeMustBeGreaterThanTotalProcessTime);
            }
        }
        internal static bool IsAllowToEditFinishTimeGreaterThanCurrentTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ALLOW_TO_EDIT_FINISHTIME) == "1";
            }
        }
        internal static bool IsAllowFinishWhenAccountIsDoctor
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ALLOW_FINISH_WHEN_ACCOUNT_IS_DOCTOR) == "1";
            }
        }
        internal static bool IsInitCameraDefault
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsInitCameraDefault) == "1";
            }
        }

        internal static bool IsHideConcludeAndNoteByDefault
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsHideConcludeAndNoteByDefault) == "1";
            }
        }

        internal static bool IsSavingInLocal
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__Camera__IsSavingInLocal) == "1";
            }
        }

        internal static string StartTimeOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__StartTimeOption);
            }
        }

        internal static string StartTimeMustBeGreaterThanInstructionTime
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__StartTimeMustBeGreaterThanInstructionTime);
            }
        }

        internal static string CheckPermisson
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(IS_CHECKING_PERMISSON);
            }
        }

        internal static bool IsRequiredPtttPriority
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsRequiredPtttPriority) == "1";
            }
        }

        internal static string IsMachineWarningOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsMachineWarningOption);
            }
        }

        internal static bool IsRequiredConclude
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IsRequiredConclude) == "1";
            }
        }

        internal static bool IsAutoSelectImageCapture
        {
            get
            {
                return HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>(AUTO_SELECT_IMAGE_CAPTURE_CFG) == "1";
            }
        }

        internal static bool IsCheckSimulTaneityOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__CHECK_SIMULTANEITY_OPTION) == "2";
            }
        }

        internal static bool IsAssignServiceSimulTaneityOption
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ASSIGN_SERVICE_SIMULTANEITY_OPTION) == "2";
            }
        }
    }
}
