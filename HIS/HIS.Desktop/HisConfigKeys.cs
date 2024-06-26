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

namespace HIS.Desktop
{
    internal class HisConfigKeys
    {
        internal const string CONFIG_KEY__RUN_AUP_VERSION = "HIS.Desktop.AupVersion";
        internal const string KEY__MASTER_ADDRESS = "MOS.MASTER_ADDRESS";
        internal const string KEY__PATIENT_TYPE_CODE__BHYT = "MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT";//Doi tuong BHYT
        internal const string KEY__GENDER_CODE_BASE = "RAE.HIS_GENDER_CODE__BASE";
        internal const string KEY__NATIONAL_CODE_BASE = "EXE.NATIONAL_CODE_BASE";
        internal const string KEY__ETHNIC_CODE_BASE = "EXE.ETHNIC_CODE_BASE";
        internal const string KEY__CAREER_CODE_BASE = "EXE.HIS_CAREER_CODE__BASE";
        internal const string KEY__CAREER_CODE__UNDER_6_AGE = "EXE.HIS_CAREER_CODE__UNDER_6_AGE";
        internal const string KEY__CAREER_CODE__HOC_SINH = "HIS.DESKTOP.REGISTER.HIS_CAREER.CARRER_CODE_HS";
        internal const string KEY__LANGUAGE__FRMMAIN__CAPTION = "HIS.Desktop.Resources.LanguageFrmMain.Caption";
        internal const string KEY__LANGUAGE__FRMMAIN__CAPTION__EN = "HIS.Desktop.Resources.LanguageFrmMain.Caption.En";
        internal const string KEY__LANGUAGE__FRMMAIN__CAPTION__MY = "HIS.Desktop.Resources.LanguageFrmMain.Caption.My";
        internal const string KEY__HIS_DESKTOP_TIME_APPLICATION_CHECK_TOKEN_TIMEOUT = "HIS.Desktop.TimeApplicationCheckTokenTimeout";
        internal const string KEY__HIS_DESKTOP_IS_RUN_IN_BACKGROUND_LOAD_DATA_TO_RAM_AFTER_LOGIN = "HIS.Desktop.IsRunInBackgroundLoadDataToRamAfterLogin";
        public const string CONFIG_KEY__HIS_IS_USE_REDIS_CACHE_SERVER = "HIS.Desktop.IsUseRedisCacheServer";
        public const string CONFIG_KEY__HIS_IS_ALLOW_OPEN_MPS_TEMPLATE = "HIS.Desktop.IsAllowOpenMpsTemplate";
        public const string CONFIG_KEY__HIS_IS_USE_EMR_SYSTEM = "HIS.HIS.DESKTOP.IS_USE_SIGN_EMR";
        public const string CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN = "HIS.Desktop.IsUseRegistryToken";
        public const string CONFIG_KEY__HIS_IS_SHOW_DECIMAL_OPTION = "HIS.Desktop.Plugins.ShowDecimalOption";
        internal const string CONFIG_KEY__TIMER_AUTO_CHECK_CONNECT_SERVER = "HIS.Desktop.TimerAutoCheckConnectServer";
        internal const string CONFIG_KEY__VPLUS_CUSTOMER_INFO = "HIS.Desktop.VPLUS_CUSTOMER_INFO";
        internal const string CONFIG_KEY__LOG_TIMER = "HIS.Desktop.Plugins.Log.Timer";
        internal const string CONFIG_KEY__MPS_PROCESSOR_BASE__DISPOSE_AFTER_PROCESS = "MPS.ProcessorBase.DisposeAfterProcess";
        internal const string CONFIG_KEY__IsUsingSingleSession = "HIS.Desktop.IsUsingSingleSession";
        internal const string CONFIG_KEY__SyncVlog_IsUAT = "HIS.Desktop.SyncVlog.IsUAT";
    }
}
