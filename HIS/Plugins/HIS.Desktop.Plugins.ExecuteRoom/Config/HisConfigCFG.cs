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
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExecuteRoom
{
    internal class HisConfigCFG
    {
        private const string IsNotBillString = "HIS.Desktop.Plugins.TransactionRepay.IsNotBill";
        private const string SendToExtWhenStartOption = "HIS.Desktop.Plugins.ExecuteRoom.SendToExtWhenStartOption";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        private const string CONFIG_KEY__PATIENT_TYPE_CODE__KSK = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.KSK";//Doi tuong khám sức khỏe
        internal const string LockExecuteCFG = "HIS.Desktop.Plugins.ServiceExecute.MustHavePresBeforeExecuteWithDiagnosticImgServiceReq";
        internal const string WarningOverExam = "HIS.Desktop.WarningOverExam";
        private const string CONFIG_KEY__REQUEST_LIMIT_WARNING_OPTION = "HIS.Desktop.Plugins.ExecuteRoom.RequestLimitWarningOption";
        private const string CONFIG_KEY__HIS_MACHINE_WARNING_MAX_SERVICE_PER_DAY = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.WARNING_OPTION";
        private const string CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        private const string SERVICE_HAS_PAYMENT_LIMIT_BHYT = "HIS.Desktop.Plugins.AssignService.ServiceHasPaymentLimitBHYT";
        private const string CONFIG_KEY__START_TIME_MUST_BE_GREATER_THAN_INSTRUCTION_TIME = "HIS.Desktop.Plugins.StartTimeMustBeGreaterThanInstructionTime";
        private const string CONFIG_KEY__SIGN_WARNING_OPTION = "HIS.Desktop.Plugins.ExecuteServiceReq.SignWarningOption";
        private const string CONFIG_KEY__PATIENT_TYPE_OPTION = "HIS.DESKTOP.HIS_MACHINE.MAX_SERVICE_PER_DAY.PATIENT_TYPE_OPTION";
        private const string CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq = "HIS.Desktop.Plugins.ServiceReqList.AutoDeleteEmrDocumentWhenEditReq";
        private const string DISABLE_PART_EXAM_BY_EXECUTOR = "HIS.Desktop.Plugins.ExamServiceReqExecute.DisablePartExamByExecutor";
        private const string AUTO_FINISH_AFTER_UNFINISH = "HIS.HIS_SERVICE_REQ.EXAM.AUTO_FINISH_AFTER_UNFINISH";
        private const string IS_USING_EXECUTE_ROOM_PAYMENT = "MOS.EPAYMENT.IS_USING_EXECUTE_ROOM_PAYMENT";
        private const string IS_ShowResultWhenReqComplete = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        private const string IS_HAS_CONNECTION_EMR = "MOS.HAS_CONNECTION_EMR";
        private const string KEY__IsCheckHeinCard = "HIS.Desktop.Plugins.ExamServiceReqExecute.IsCheckHeinCard";
        internal static bool IsCheckHeinCard;
        internal static bool IsHasConnectionEmr;
        internal static string IsShowResultWhenReqComplete;
        internal static bool IsUsingExecuteRoomPayment;
        internal static string AutoFinishAfterUnfinish;
        internal static bool isDisablePartExamByExecutor;

        internal static string PatientTypeCode__BHYT; 
        internal static long PatientTypeId__BHYT;

        internal static string PatientTypeCode__VP;
        internal static long PatientTypeId__VP;

        internal static string PatientTypeCode__KSK;
        internal static long PatientTypeId__KSK;
        internal static string RequestLimitWarningOption;
        internal static string IsNotBillCFG;
        internal static string SendToExtWhenStart;
        internal static string IsMachineWarningOption;
        internal static string IsSetPrimaryPatientType;
        internal static string ServiceHasPaymentLimitBHYT;
        internal static string StartTimeMustBeGreaterThanInstructionTime;
        internal static string SignWarningOption;
        internal static string PatientTypeOption;
        internal static string AutoDeleteEmrDocumentWhenEditReq;

        internal static string IsSplitTotalReceivePrice
        {
            get
            {
                return GetValue("HIS.Desktop.Plugins.Transaction.IsSplitTotalReceivePrice");
            }
        }

        internal static void LoadConfig()
        {
            try
            {
                LogSystem.Debug("LoadConfig => 1");
                IsCheckHeinCard = GetValue(KEY__IsCheckHeinCard) == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonStringTrue;
                IsHasConnectionEmr = GetValue(IS_HAS_CONNECTION_EMR) == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonStringTrue;
                IsShowResultWhenReqComplete = GetValue(IS_ShowResultWhenReqComplete);
                IsUsingExecuteRoomPayment = GetValue(IS_USING_EXECUTE_ROOM_PAYMENT) == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonStringTrue;
                AutoFinishAfterUnfinish = GetValue(AUTO_FINISH_AFTER_UNFINISH);
                IsNotBillCFG = GetValue(IsNotBillString);
                SendToExtWhenStart = GetValue(SendToExtWhenStartOption);
                IsMachineWarningOption = GetValue(CONFIG_KEY__HIS_MACHINE_WARNING_MAX_SERVICE_PER_DAY);
                PatientTypeCode__BHYT = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                RequestLimitWarningOption = GetValue(CONFIG_KEY__REQUEST_LIMIT_WARNING_OPTION);
                PatientTypeId__BHYT = GetPatientTypeByCode(PatientTypeCode__BHYT).ID;
                PatientTypeCode__KSK = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__KSK);
                PatientTypeId__KSK = GetPatientTypeByCode(PatientTypeCode__KSK).ID;
                PatientTypeCode__VP = GetValue(CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                PatientTypeId__VP = GetPatientTypeByCode(PatientTypeCode__VP).ID;
                IsSetPrimaryPatientType = GetValue(CONFIG_KEY__HIS_SERE_SERV__SET_PRIMARY);
                ServiceHasPaymentLimitBHYT = GetValue(SERVICE_HAS_PAYMENT_LIMIT_BHYT);
                StartTimeMustBeGreaterThanInstructionTime = GetValue(CONFIG_KEY__START_TIME_MUST_BE_GREATER_THAN_INSTRUCTION_TIME);
                SignWarningOption = GetValue(CONFIG_KEY__SIGN_WARNING_OPTION);
                PatientTypeOption = GetValue(CONFIG_KEY__PATIENT_TYPE_OPTION);
                AutoDeleteEmrDocumentWhenEditReq = GetValue(CONFIG_KEY__AutoDeleteEmrDocumentWhenEditReq);
                isDisablePartExamByExecutor = GetValue(DISABLE_PART_EXAM_BY_EXECUTOR) == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CommonStringTrue;
                LogSystem.Debug("LoadConfig => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private static List<string> GetListValue(string key)
        {
            try
            {
                return HisConfigs.Get<List<string>>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }
    }
}
