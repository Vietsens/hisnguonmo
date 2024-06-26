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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisExamServiceAdd.Config
{
    public class HisConfig
    {
        private const string IS_USING_SERVER_TIME = "MOS.IS_USING_SERVER_TIME";
        private const string IS_SET_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";

        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT

        private const string CONFIG_KEY__APPOINTMENT_TIME_DEFAULT = "EXE.HIS_TREATMENT_END.APPOINTMENT_TIME_DEFAULT";
        private const string CONFIG_KEY__AutoSetIsMainForAdditionExam = "HIS.Desktop.Plugins.ExamServiceReqExecute.AutoSetIsMainForAdditionExam";


        private const string CONFIG_KEY__AutoCheckChangeDepartment = "HIS.UC.HisExamServiceAdd.AutoCheckChangeDepartment";
        private const string HIS_Desktop_Plugins_ExamServiceReqExecute_IsFinishExamAdd = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsFinishExamAdd";

        public static bool IsUsingServerTime;
        public static string IsSetPrimaryPatientType;

        internal static string PatientTypeCode__VP;
        internal static string PatientTypeCode__BHYT;
        internal static long PatientTypeId__BHYT;

        internal static double AppointmentTime__DEFAULT;
        public static bool IsAutoSetIsMainForAdditionExam;
        internal static string AutoCheckChangeDepartment;
        internal static string IsFinishExamAdd;
        public static void LoadConfig()
        {
            IsAutoSetIsMainForAdditionExam = GetValue(CONFIG_KEY__AutoSetIsMainForAdditionExam) == GlobalVariables.CommonStringTrue;
            IsUsingServerTime = GetValue(IS_USING_SERVER_TIME) == GlobalVariables.CommonStringTrue;
            IsSetPrimaryPatientType = GetValue(IS_SET_PRIMARY_PATIENT_TYPE) ?? "";

            PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
            PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
            PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
            IsFinishExamAdd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HIS_Desktop_Plugins_ExamServiceReqExecute_IsFinishExamAdd);
            AppointmentTime__DEFAULT = Inventec.Common.TypeConvert.Parse.ToDouble(GetValue(CONFIG_KEY__APPOINTMENT_TIME_DEFAULT));

            AppointmentTime__DEFAULT = Inventec.Common.TypeConvert.Parse.ToDouble(GetValue(CONFIG_KEY__APPOINTMENT_TIME_DEFAULT));
            AutoCheckChangeDepartment = GetValue(CONFIG_KEY__AutoCheckChangeDepartment);
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }

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
    }
}
