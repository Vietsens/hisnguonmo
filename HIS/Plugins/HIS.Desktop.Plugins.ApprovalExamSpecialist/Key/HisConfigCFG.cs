using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ApprovalExamSpecialist.Key
{
    class HisConfigCFG
    {
        private const string IS_ShowResultWhenReqComplete = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        internal static string IsShowResultWhenReqComplete
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(IS_ShowResultWhenReqComplete);
                return ptBHYT;
            }
        }
        private const string APPROVE_AND_SIGN_OPTION = "HIS.Desktop.Plugins.ApprovalExamSpecialist.ApproveAndSignOption";

        /// <summary>
        /// Viec 57944: "Duyet va ky" ky phieu nao (1: to dieu tri, 2: phieu ket qua kham CK Mps000500, 3: ca hai).
        /// Rong / khong hop le => 1 (giu nguyen hanh vi cu).
        /// </summary>
        internal static EnumApproveAndSignOption ApproveAndSignOption
        {
            get
            {
                EnumApproveAndSignOption result = EnumApproveAndSignOption.TrackingOnly;
                try
                {
                    long value = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(APPROVE_AND_SIGN_OPTION));
                    if (value == (long)EnumApproveAndSignOption.ExamResultOnly)
                        result = EnumApproveAndSignOption.ExamResultOnly;
                    else if (value == (long)EnumApproveAndSignOption.Both)
                        result = EnumApproveAndSignOption.Both;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return result;
            }
        }

        internal static long PatientTypeId__BHYT
        {
            get
            {
                var ptBHYT = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).FirstOrDefault();
                return ptBHYT != null ? ptBHYT.ID : 0;
            }
        }

        internal static string PatientTypeCode__BHYT
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                return ptBHYT;
            }
        }

        internal static string PatientTypeCode__VP
        {
            get
            {
                var ptVP = HisConfigs.Get<string>(Key.HisConfigKeys.HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                return ptVP;
            }
        }
    }
}
