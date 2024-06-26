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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Common.XmlConfig;
using Inventec.Common.Xml;
using Inventec.Core;
using Inventec.Common.Logging;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Data;
using System.Collections;
using System.Configuration;
using HIS.Desktop.Plugins.DesktopConfig.ADO;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Controls.Session;
using System.Resources;


namespace HIS.Desktop.Plugins.DesktopConfig
{
    public partial class UCDesktopConfig : UserControl
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        public static XmlApplicationConfig ApplicationXml { get; set; }
        string language;
        string loginName;
        List<ElementNodeADO> listElementNodeAdo;

        public UCDesktopConfig(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                this.language = LanguageManager.GetLanguage();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void UCDesktopConfig_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                listElementNodeAdo = new List<ElementNodeADO>();
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;

                Inventec.Common.Logging.LogSystem.Info(filePath);

                string url = "";
                string[] str = filePath.Split('\\');

                for (int i = 0; i < str.Count(); i++)
                {
                    if (i < str.Count() - 2)
                    {
                        if (i != (str.Count() - 3))
                        {
                            url += str[i] + "\\";
                        }
                        else
                            url += str[i];
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(url);

                string pathXmlFile = System.IO.Path.Combine(url, @"ConfigSystem.xml");
                ApplicationXml = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFile, this.language);
                var xml = ApplicationXml.GetElements();

                if (xml != null && xml.Count > 0)
                {
                    foreach (var item in xml)
                    {
                        if (item.KeyCode == "KEY__MOS_BASE_URI" ||
                            item.KeyCode == "KEY__SDA_BASE_URI" ||
                            item.KeyCode == "KEY__SAR_BASE_URI" ||
                            item.KeyCode == "KEY__MRS_BASE_URI" ||
                            item.KeyCode == "KEY__ACS_BASE_URI" ||
                            item.KeyCode == "KEY__FSS_BASE_URI" ||
                            item.KeyCode == "KEY__LIS_BASE_URI" ||
                            item.KeyCode == "KEY__SCN_BASE_URI")
                        {
                            var ado = new ElementNodeADO();
                            Inventec.Common.Mapper.DataObjectMapper.Map<ElementNodeADO>(ado, item);
                            ado.Value = item.Value;
                            ado.DefaultValue = item.DefaultValue;
                            ado.ValueAllowIn = item.ValueAllowIn;
                            ado.ValueAllowMax = item.ValueAllowMax;
                            ado.ValueAllowMin = item.ValueAllowMin;
                            ado.isConfig = false;
                            listElementNodeAdo.Add(ado);
                        }
                    }
                }

                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = @"HIS.Desktop.exe.config";

                Configuration configSave = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);

                if (configSave.AppSettings != null && configSave.AppSettings.Settings.Count > 0)
                {
                    foreach (string key in configSave.AppSettings.Settings.AllKeys)
                    {
                        if (key == "Inventec.Token.ClientSystem.Acs.Base.Uri" ||
                           key == "fss.uri.base" ||
                            key == "Aup.uri.base" ||
                            key == "His.EventLog.Sda" ||
                            key == "HIS.Desktop.PrintTemplate" ||
                            key == "HIS.Desktop.IsUseRegistryToken")
                        {
                            var ado = new ElementNodeADO(key);
                            ado.isConfig = true;
                            ado.Value = configSave.AppSettings.Settings[key].Value;
                            listElementNodeAdo.Add(ado);
                        }

                    }
                }

                gridControlDesktopConfig.DataSource = listElementNodeAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.DesktopConfig.Resources.Lang", typeof(HIS.Desktop.Plugins.DesktopConfig.UCDesktopConfig).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDesktopConfig.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("UCDesktopConfig.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.ToolTip = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.ToolTip = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.ToolTip = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCDesktopConfig.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void ShowConfig()
        //{
        //    // For read access you do not need to call the OpenExeConfiguraton
        //    foreach (string key in System.Configuration.ConfigurationSettings.AppSettings)
        //    {
        //        string value = System.Configuration.ConfigurationSettings.AppSettings[key];
        //        MessageBox.Show(String.Format("Key: {0}, Value: {1}", key, value));
        //    }
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                bool success = false;
                try
                {
                    if (this.gridViewDesktopConfig.IsEditing)
                        this.gridViewDesktopConfig.CloseEditor();

                    if (this.gridViewDesktopConfig.FocusedRowModified)
                        this.gridViewDesktopConfig.UpdateCurrentRow();

                    var dataSource = (List<ElementNodeADO>)(gridControlDesktopConfig.DataSource);
                    List<Inventec.Common.XmlConfig.ElementNode> listElementData = new List<ElementNode>();

                    if (dataSource != null && dataSource.Count > 0)
                    {
                        var dataXml = dataSource.Where(o => !o.isConfig).ToList();
                        var dataConfig = dataSource.Where(o => o.isConfig).ToList();
                        if (dataSource != null && dataSource.Count > 0)
                        {
                            AutoMapper.Mapper.CreateMap<ElementNodeADO, ElementNode>();
                            listElementData = AutoMapper.Mapper.Map<List<ElementNode>>(dataXml);
                        }

                        if (dataConfig != null && dataConfig.Count > 0)
                        {
                            foreach (var item in dataConfig)
                            {
                                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                                fileMap.ExeConfigFilename = @"HIS.Desktop.exe.config";

                                Configuration configSave = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                                ConfigurationUserLevel.None);

                                configSave.AppSettings.Settings[item.Title].Value = item.Value.ToString();
                                configSave.Save(ConfigurationSaveMode.Full);
                                ConfigurationManager.RefreshSection("appSettings");
                            }
                        }
                    }

                    success = ApplicationXml.UpdateElements(listElementData, loginName);
                    WaitingManager.Hide();

                    //System.Configuration..UpdateAppSettings("DBServerName", txtNewKeyvalue.text)

                }
                catch (Exception ex)
                {
                    WaitingManager.Hide();
                    LogSystem.Warn(ex);
                }

                #region Show message
                MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewDesktopConfig_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ElementNode data = (ElementNode)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
