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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MchTreatmentExamService.Config
{
    class HisConfigCFG
    {
        private const string IS_ShowResultWhenReqComplete = "HIS.Desktop.Plugins.ContentSubclinical.ShowResultWhenReqComplete";
        private const string AI_ConnectionInfo = "HIS.Desktop.AI.ConnectionInfo";
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        internal const string HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";//Doi tuong VP
        internal const string HIS_CONFIG_KEY__FormClosingOption = "HIS.Desktop.FormClosingOption";
        internal const string HIS_CONFIG_KEY__ModuleLinkApply = "HIS.Desktop.FormClosingOption.ModuleLinkApply";
        internal const string HIS_CONFIG_KEY__MaxTimeFilter__Option = "HIS.Desktop.Plugins.MaxTimeFilter.Option";
        internal static string AIConnectionInfo
        {
            get
            {
                var AIConec = HisConfigs.Get<string>(AI_ConnectionInfo);
                return AIConec;
            }
        }
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
                var ptBHYT = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE == HisConfigs.Get<string>(HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT)).FirstOrDefault();
                return ptBHYT != null ? ptBHYT.ID : 0;
            }
        }

        internal static string PatientTypeCode__BHYT
        {
            get
            {
                var ptBHYT = HisConfigs.Get<string>(HIS_CONFIG_KEY__PATIENT_TYPE_CODE__BHYT);
                return ptBHYT;
            }
        }

        internal static string PatientTypeCode__VP
        {
            get
            {
                var ptVP = HisConfigs.Get<string>(HIS_CONFIG_KEY__PATIENT_TYPE_CODE__VP);
                return ptVP;
            }
        }
    }
}
