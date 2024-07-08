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
using HIS.UC.FormType.Base;
using Inventec.Common.XmlConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.FormType.Base
{
    public class ConfigSystemProcess
    {
        private const string EXE_CONFIG_KEY__MOS_BASE_URI = "KEY__MOS_BASE_URI";
        private const string EXE_CONFIG_KEY__SDA_BASE_URI = "KEY__SDA_BASE_URI";
        private const string EXE_CONFIG_KEY__SAR_BASE_URI = "KEY__SAR_BASE_URI";
        private const string EXE_CONFIG_KEY__MRS_BASE_URI = "KEY__MRS_BASE_URI";
        private const string EXE_CONFIG_KEY__ACS_BASE_URI = "KEY__ACS_BASE_URI";
        private const string EXE_CONFIG_KEY__TOS_BASE_URI = "KEY__TOS_BASE_URI";
        private const string EXE_CONFIG_KEY__FSS_BASE_URI = "KEY__FSS_BASE_URI";
        private const string EXE_CONFIG_KEY__PAC_BASE_URI = "KEY__PAC_BASE_URI";
        private const string EXE_CONFIG_KEY__THE_BASE_URI = "KEY__THE_BASE_URI";
        private const string EXE_CONFIG_KEY__AOS_BASE_URI = "KEY__AOS_BASE_URI";

        public ConfigSystemProcess() { }

        public static void InitConfigSystem()
        {
            XmlApplicationConfig ApplicationConfig = null;
            string filePath = AppDomain.CurrentDomain.BaseDirectory;
            string pathXmlFileConfig = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filePath), @"ConfigSystem.xml");
            string lang = (LanguageManager.GetLanguage() == LanguageManager.GetLanguageVi() ? "Vi" : "En");
            ApplicationConfig = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, lang);
            if (ApplicationConfig != null)
            {
                try
                {
                    UriBaseStore.URI_API_MOS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__MOS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server MOS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__MOS_BASE_URI), EXE_CONFIG_KEY__MOS_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_SDA = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__SDA_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SDA. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__SDA_BASE_URI), EXE_CONFIG_KEY__SDA_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_SAR = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__SAR_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SAR. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__SAR_BASE_URI), EXE_CONFIG_KEY__SAR_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_MRS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__MRS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server MRS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__MRS_BASE_URI), EXE_CONFIG_KEY__MRS_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_ACS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__ACS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server ACS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__ACS_BASE_URI), EXE_CONFIG_KEY__ACS_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_TOS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__TOS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server TOS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__TOS_BASE_URI), EXE_CONFIG_KEY__TOS_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_PAC = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__PAC_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server PAC. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__PAC_BASE_URI), EXE_CONFIG_KEY__PAC_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_FSS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__FSS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server FSS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__FSS_BASE_URI), EXE_CONFIG_KEY__FSS_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_THE = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__THE_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server THE. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__THE_BASE_URI), EXE_CONFIG_KEY__THE_BASE_URI));
                }

                try
                {
                    UriBaseStore.URI_API_AOS = (ApplicationConfig.GetKeyValue(EXE_CONFIG_KEY__AOS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server THE. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => EXE_CONFIG_KEY__THE_BASE_URI), EXE_CONFIG_KEY__THE_BASE_URI));
                }
            }
        }
    }
}
