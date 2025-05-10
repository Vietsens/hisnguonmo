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
