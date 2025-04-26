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
using Inventec.Common.XmlConfig;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.LocalStorage.ConfigSystem
{
    public class Load
    {
        public static void Init()
        {
            XmlApplicationConfig ApplicationConfig = null;
            string pathXmlFileConfig = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, @"ConfigSystem.xml");
            string lang = (LanguageManager.GetLanguage() == LanguageManager.GetLanguageVi() ? "Vi" : "En");
            ApplicationConfig = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFileConfig, lang);
            if (ApplicationConfig != null)
            {
                try
                {
                    ConfigSystems.URI_API_FSS_FOR_CRM = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__FSS_FOR_CRM_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server FSS For CRM/V+. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__FSS_FOR_CRM_BASE_URI), Keys.EXE_CONFIG_KEY__FSS_FOR_CRM_BASE_URI));
                }
                
                try
                {
                    ConfigSystems.URI_API_CRM = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__CRM_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server CRM. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__CRM_BASE_URI), Keys.EXE_CONFIG_KEY__CRM_BASE_URI));
                }

                try
                {
                    ConfigSystems.URI_API_MPS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__MPS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server MPS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__MPS_BASE_URI), Keys.EXE_CONFIG_KEY__MPS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_VVA = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__VVA_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server VVA. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__VVA_BASE_URI), Keys.EXE_CONFIG_KEY__VVA_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_RSCACHE = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__RDCACHE_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server RSCACHE. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__RDCACHE_BASE_URI), Keys.EXE_CONFIG_KEY__RDCACHE_BASE_URI));
                }
                try
                {
                    ConfigSystems.FOLDER_FONT_BASE = (ApplicationConfig.GetKeyValue(Keys.KEY__FOLDER_FONT_BASE) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key folder font cho tieng ede,.... " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.KEY__FOLDER_FONT_BASE), Keys.KEY__FOLDER_FONT_BASE));
                }
                try
                {
                    ConfigSystems.URI_HIS_PUBSUB_BASE = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__HIS_PUBSUB_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HIS_PUBSUB. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__HIS_PUBSUB_BASE_URI), Keys.EXE_CONFIG_KEY__HIS_PUBSUB_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_HID = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__HID_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HID. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__TYT_BASE_URI), Keys.EXE_CONFIG_KEY__TYT_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_TYT = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__TYT_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HID. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__HID_BASE_URI), Keys.EXE_CONFIG_KEY__HID_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_PACS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__PACS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server PACS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__PACS_BASE_URI), Keys.EXE_CONFIG_KEY__PACS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_FSS_FOR_PACS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__FSS_FOR_PACS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server PACS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__FSS_FOR_PACS_BASE_URI), Keys.EXE_CONFIG_KEY__FSS_FOR_PACS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_MOS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__MOS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server FIN. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__MOS_BASE_URI), Keys.EXE_CONFIG_KEY__MOS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_SDA = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__SDA_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SDA. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__SDA_BASE_URI), Keys.EXE_CONFIG_KEY__SDA_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_SAR = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__SAR_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SAR. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__SAR_BASE_URI), Keys.EXE_CONFIG_KEY__SAR_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_MRS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__MRS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server MRS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__MRS_BASE_URI), Keys.EXE_CONFIG_KEY__MRS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_ACS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__ACS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server ACS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__ACS_BASE_URI), Keys.EXE_CONFIG_KEY__ACS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_FSS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__FSS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server FSS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__FSS_BASE_URI), Keys.EXE_CONFIG_KEY__FSS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_HTC = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__HTC_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HTC. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__HTC_BASE_URI), Keys.EXE_CONFIG_KEY__HTC_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_LIS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__LIS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server LIS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__LIS_BASE_URI), Keys.EXE_CONFIG_KEY__LIS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_SCN = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__SCN_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server SCN. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__SCN_BASE_URI), Keys.EXE_CONFIG_KEY__SCN_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_EMR = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__EMR_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server EMR. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__EMR_BASE_URI), Keys.EXE_CONFIG_KEY__EMR_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_QCS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__QCS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server QCS. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__QCS_BASE_URI), Keys.EXE_CONFIG_KEY__QCS_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_HSSK = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__HSSK_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HSSK. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__HSSK_BASE_URI), Keys.EXE_CONFIG_KEY__HSSK_BASE_URI));
                }
                try
                {
                    ConfigSystems.URI_API_HPS = (ApplicationConfig.GetKeyValue(Keys.EXE_CONFIG_KEY__HPS_BASE_URI) ?? "").ToString();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay key uri resource server HSSK. " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Keys.EXE_CONFIG_KEY__HSSK_BASE_URI), Keys.EXE_CONFIG_KEY__HSSK_BASE_URI));
                }
            }
        }
    }
}
