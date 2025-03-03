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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.Config
{
    internal class HisConfigCFG
    {
        private const string CONFIG_KEY__TIME_NO_EXECUTE_KIOS = "CONFIG_KEY__HIS_DESKTOP__TIME_NO_EXECUTE_KIOS";
        private const string CONFIG_KEY__DEFAULT_BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";
        private const string CONFIG_KEY__IS_SET_PRIMARY_PATIENT_TYPE = "MOS.HIS_SERE_SERV.IS_SET_PRIMARY_PATIENT_TYPE";
        public const string CONFIG_KEY__NotDisplayedRouteTypeOver = "HIS.Desktop.Plugins.Register.NotDisplayedRouteTypeOver";
        private const string CONFIG_KEY__HOSPITAL_FEE = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE";
        private const string CHECK_PREVIOUS_DEBT_OPTION_CFG = "MOS.HIS_TREATMENT.CHECK_PREVIOUS_DEBT_OPTION";
        public const string CONFIG_KEY__DoNotAllowToEditDefaultRouteType = "HIS.Desktop.Plugins.RegisterExamKiosk.DoNotAllowToEditDefaultRouteType";
        public const string HIS_CAREER_CODE__BASE = "EXE.HIS_CAREER_CODE__BASE";
        public const string NATIONAL_CODE__BASE = "EXE.NATIONAL_CODE_BASE";
        public const string ETHNIC_CODE__BASE = "EXE.ETHNIC_CODE_BASE";
        public const string CONFIG_KEY__IdentityNumberOption = "HIS.Desktop.Plugins.RegisterExamKiosk.IdentityNumberOption";

        private const string CONFIG_KEY__USER_NAME = "MOS.VVN.USER_NAME";
        private const string CONFIG_KEY__KEY = "MOS.VVN.KEY";
        private const string CONFIG_KEY__FACE_RECOGNITION_ADDRESS = "MOS.VVN.FACE_RECOGNITION_ADDRESS";
        private const string CONFIG_KEY__FACE_REGISTER_ADDRESS = "MOS.VVN.FACE_REGISTER_ADDRESS";
        private const string CONFIG_KEY__SAME_PERSON_MATCHING_THRESHOLD = "MOS.HIS_SERVICE_REQ.SAME_PERSON_MATCHING_THRESHOLD";
        public static decimal SamePersonMatchingThreshold
        {
            get
            {
                return decimal.Parse(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__SAME_PERSON_MATCHING_THRESHOLD), CultureInfo.InvariantCulture); //HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__SAME_PERSON_MATCHING_THRESHOLD);//0.65
            }
        }
        public static string UserNameVvn
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__USER_NAME); //"tiepnh@vvndotvn";//HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__USER_NAME);
            }
        }
        public static string KeyVvn
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__KEY); //"4XFGse8qKX1dOvEbn4SKoEoDi5Vje4XR";//HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__KEY);
            }
        }
        public static string FaceRegAddressVvn
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__FACE_RECOGNITION_ADDRESS); //"https://api.smartekyc.com/";// HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__FACE_RECOGNITION_ADDRESS);
            }
        }
        public static string FaceRetAddressVvn
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__FACE_REGISTER_ADDRESS); //"https://api.smartekyc.com/";// HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__FACE_RECOGNITION_ADDRESS);
            }
        }
        public static string IdentityNumberOption
        {
            get
            {
                 return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__IdentityNumberOption);
            }
        }
        private static string checkPreviousDebtOption;
        public static string CHECK_PREVIOUS_DEBT_OPTION
        {
            get
            {
                if (checkPreviousDebtOption == null)
                {
                    checkPreviousDebtOption = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CHECK_PREVIOUS_DEBT_OPTION_CFG);
                }
                return checkPreviousDebtOption;
            }
            set
            {
                checkPreviousDebtOption = value;
            }
        }

        private static long patientTypeIdIsFee;
        public static long PATIENT_TYPE_ID__IS_FEE
        {
            get
            {
                if (patientTypeIdIsFee == 0)
                {
                    patientTypeIdIsFee = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HOSPITAL_FEE));
                }
                return patientTypeIdIsFee;
            }
            set
            {
                patientTypeIdIsFee = value;
            }
        }

        private static long patientTypeIdIsHein;
        public static long PATIENT_TYPE_ID__BHYT
        {
            get
            {
                if (patientTypeIdIsHein == 0)
                {
                    patientTypeIdIsHein = GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__DEFAULT_BHYT));
                }
                return patientTypeIdIsHein;
            }
            set
            {
                patientTypeIdIsHein = value;
            }
        }

        public static long PrimaryPatientType
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>(CONFIG_KEY__IS_SET_PRIMARY_PATIENT_TYPE);
            }
        }

        internal static int timeWaitingMilisecond
        {
            get
            {
                int waitTime = ConfigApplicationWorker.Get<int>(CONFIG_KEY__TIME_NO_EXECUTE_KIOS);
                if (waitTime == 0)
                    return 300000;
                else
                    return waitTime;
            }
        }

        private static long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                if (!(data != null && data.ID > 0)) throw new ArgumentNullException(code + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => code), code));
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }
    }
}
