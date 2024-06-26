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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.XmlConfig;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.DesktopConfig
{
    public partial class frmDesktopConfig : Form
    {
        public static XmlApplicationConfig ApplicationXml { get; set; }
        List<ElementNodeADO> listElementNodeAdo;
        Configuration configSave;
        Configuration configEMRSave;

        public frmDesktopConfig()
        {
            InitializeComponent();
            try
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = @"HIS.exe.config";
                configSave = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);

                ExeConfigurationFileMap EMRfileMap = new ExeConfigurationFileMap();
                EMRfileMap.ExeConfigFilename = @"Integrate/EMR/ConnectToEMR.exe.config";
                configEMRSave = ConfigurationManager.OpenMappedExeConfiguration(EMRfileMap,
                ConfigurationUserLevel.None);

                try
                {
                    string url = configSave.AppSettings.Settings["Inventec.Desktop.Icon"].Value;
                    string iconPath = System.IO.Path.Combine(Application.StartupPath, url);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception exx)
                {
                    Inventec.Common.Logging.LogSystem.Warn(exx);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDesktopConfig_Load(object sender, EventArgs e)
        {
            try
            {
                //gridColumn11.Visible = false;
                //gridColumn1.Visible = true;
                //gridColumn8.Visible = false;
                toggleSwitch1.IsOn = false;

                lciGridData.Visibility = toggleSwitch1.IsOn ? DevExpress.XtraLayout.Utils.LayoutVisibility.Never : DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcitxtIpCommon.Visibility = toggleSwitch1.IsOn ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                LoadData2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadData2()
        {
            try
            {
                this.listElementNodeAdo = new List<ElementNodeADO>();
                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                int i = 1;
                string url = Application.StartupPath;
                string pathXmlFile = System.IO.Path.Combine(url, @"ConfigSystem.xml");
                ApplicationXml = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFile, "vi");
                var xml = ApplicationXml.GetElements();

                if (xml != null && xml.Count > 0)
                {
                    foreach (var item in xml)
                    {
                        var ado = new ElementNodeADO();
                        ado.ConfigFileName = "ConfigSystem.xml";
                        ado.CreateTime = item.CreateTime;
                        ado.Creator = item.Creator;
                        ado.DefaultValue = item.DefaultValue;
                        ado.KeyCode = item.KeyCode;
                        ado.Modifier = item.Modifier;
                        ado.ModifyTime = item.ModifyTime;
                        ado.Title = item.Title;
                        ado.Tutorial = item.Tutorial;
                        ado.Value = item.Value;
                        ado.ValueAllowIn = item.ValueAllowIn;
                        ado.ValueAllowMax = item.ValueAllowMax;
                        ado.ValueAllowMin = item.ValueAllowMin;
                        ado.ValueType = item.ValueType;
                        ado.ValueTypeDescription = item.ValueTypeDescription;
                        ado.isConfig = false;
                        ado.ID = i;
                        i++;
                        this.listElementNodeAdo.Add(ado);
                    }
                    if (this.listElementNodeAdo != null && this.listElementNodeAdo[0].Value.ToString().Contains("http://"))
                    {
                        string x = this.listElementNodeAdo[0].Value.ToString().Substring(7);
                        if (x.Contains(":"))
                        {
                            txtIPCommon.Text = x.Split(':')[0];
                        }
                        else
                        {
                            txtIPCommon.Text = "";
                        }
                    }
                }

                if (configSave.AppSettings != null && configSave.AppSettings.Settings.Count > 0)
                {
                    foreach (string key in configSave.AppSettings.Settings.AllKeys)
                    {
                        if (key == "Inventec.Token.ClientSystem.Acs.Base.Uri" ||
                            key == "fss.uri.base" ||
                            key == "Aup.uri.base" ||
                            key == "His.EventLog.Sda"
                            //|| key == "Inventec.ScnConsumer.Base.Uri"
                            //|| key == "Inventec.ScnConsumer.Acs.Uri"
                            //|| key == "Inventec.ScnConsumer.LoginName"
                            //|| key == "Inventec.ScnConsumer.Password"
                            )
                        {
                            var ado = new ElementNodeADO(key);
                            ado.ConfigFileName = "HIS.exe.config";
                            ado.isConfig = true;
                            ado.ConfigType = ConfigType.HIS;
                            ado.Value = configSave.AppSettings.Settings[key].Value;
                            ado.ID = i;
                            i++;
                            this.listElementNodeAdo.Add(ado);
                        }
                    }
                }

                if (configEMRSave.AppSettings != null && configEMRSave.AppSettings.Settings.Count > 0)
                {
                    foreach (string key in configEMRSave.AppSettings.Settings.AllKeys)
                    {
                        if (!String.IsNullOrEmpty(key))
                        {
                            var ado = new ElementNodeADO(key);
                            ado.ConfigFileName = "ConnectToEMR.exe.config";
                            ado.isConfig = true;
                            ado.ConfigType = ConfigType.EMR;
                            ado.Value = configEMRSave.AppSettings.Settings[key].Value;
                            ado.ID = i;
                            i++;
                            this.listElementNodeAdo.Add(ado);
                        }
                    }
                }

                foreach (var item in this.listElementNodeAdo)
                {
                    if (item.Value.ToString().Contains("http://"))
                    {
                        string x = item.Value.ToString().Substring(7);
                        if (x.Contains(":"))
                        {
                            item.IP = x.Split(':')[0];
                        }
                        else
                        {
                            item.IP = x;
                        }
                    }
                    else
                    {
                        item.IP = item.Value.ToString();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => listElementNodeAdo), listElementNodeAdo));
                gridControl1.DataSource = this.listElementNodeAdo;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = false;
                //System.Configuration..UpdateAppSettings("DBServerName", txtNewKeyvalue.text)

                if (this.gridView1.IsEditing)
                    this.gridView1.CloseEditor();

                if (this.gridView1.FocusedRowModified)
                    this.gridView1.UpdateCurrentRow();

                this.listElementNodeAdo = (List<ElementNodeADO>)(gridControl1.DataSource);

                if (toggleSwitch1.IsOn)
                {
                    foreach (var item in this.listElementNodeAdo)
                    {
                        if (item.Value.ToString().Contains("http://"))
                        {
                            if (item.Value.ToString().Substring(7).Contains(':'))
                            {
                                item.Value = "http://" + txtIPCommon.Text.Trim() + ":" + item.Value.ToString().Substring(7).Split(':')[1];
                                item.IP = txtIPCommon.Text.Trim();
                            }
                            else
                            {
                                item.Value = "http://" + txtIPCommon.Text.Trim();
                                item.IP = txtIPCommon.Text.Trim();
                            }
                        }
                        else
                        {
                            item.Value = txtIPCommon.Text.Trim();
                            item.IP = txtIPCommon.Text.Trim();
                        }
                    }
                }

                List<Inventec.Common.XmlConfig.ElementNode> listElementData = new List<ElementNode>();
                int i = 1;
                if (this.listElementNodeAdo != null && this.listElementNodeAdo.Count > 0)
                {
                    var dataXml = this.listElementNodeAdo.Where(o => !o.isConfig).ToList();
                    var dataConfigHIS = this.listElementNodeAdo.Where(o => o.isConfig && o.ConfigType == ConfigType.HIS).ToList();
                    var dataConfigEMR = this.listElementNodeAdo.Where(o => o.isConfig && o.ConfigType == ConfigType.EMR).ToList();
                    if (dataXml != null && dataXml.Count > 0)
                    {
                        foreach (var item in dataXml)
                        {
                            var elementData = new ElementNodeADO();
                            elementData.CreateTime = item.CreateTime;
                            elementData.Creator = item.Creator;
                            elementData.DefaultValue = item.DefaultValue;
                            elementData.KeyCode = item.KeyCode;
                            elementData.Modifier = item.Modifier;
                            elementData.ModifyTime = item.ModifyTime;
                            elementData.Title = item.Title;
                            elementData.Tutorial = item.Tutorial;
                            elementData.Value = item.Value;
                            elementData.ValueAllowIn = item.ValueAllowIn;
                            elementData.ValueAllowMax = item.ValueAllowMax;
                            elementData.ValueAllowMin = item.ValueAllowMin;
                            elementData.ValueType = item.ValueType;
                            elementData.ValueTypeDescription = item.ValueTypeDescription;
                            listElementData.Add(elementData);
                        }
                    }

                    if (dataConfigHIS != null && dataConfigHIS.Count > 0)
                    {
                        foreach (var item in dataConfigHIS)
                        {
                            configSave.AppSettings.Settings[item.Title].Value = item.Value.ToString();
                            configSave.Save(ConfigurationSaveMode.Full);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                    }

                    if ((!toggleSwitch1.IsOn) && dataConfigEMR != null && dataConfigEMR.Count > 0)
                    {
                        foreach (var item in dataConfigEMR)
                        {
                            configEMRSave.AppSettings.Settings[item.Title].Value = item.Value.ToString();
                            configEMRSave.Save(ConfigurationSaveMode.Full);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                    }
                }

                success = ApplicationXml.UpdateElements(listElementData, "");

                if (success)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateRowDataAfterEdit(ElementNodeADO data)
        {
            try
            {
                if (data == null)
                    throw new ArgumentNullException("ElementNodeADO is null");
                ElementNodeADO dataRow = (ElementNodeADO)gridView1.GetFocusedRow();
                if (dataRow != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<ElementNodeADO>(dataRow, data);
                    gridView1.RefreshRow(gridView1.FocusedRowHandle);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtIP_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                var text = (DevExpress.XtraEditors.TextEdit)sender;
                if (text == null)
                {
                    return;
                }
                ElementNodeADO dataRow = (ElementNodeADO)gridView1.GetFocusedRow();
                List<ElementNodeADO> data = (List<ElementNodeADO>)gridControl1.DataSource;
                if (dataRow.Value.ToString().Contains("http://"))
                {
                    if (dataRow.Value.ToString().Substring(7).Contains(':'))
                    {
                        dataRow.Value = "http://" + text.Text + ":" + dataRow.Value.ToString().Substring(7).Split(':')[1];
                        dataRow.IP = text.Text;
                    }
                    else
                    {

                        dataRow.Value = "http://" + text.Text;
                        dataRow.IP = text.Text;
                    }
                }
                else
                {
                    dataRow.Value = text.Text;
                    dataRow.IP = text.Text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ElementNodeADO data = (ElementNodeADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "IP")
                        {
                            e.Value = data.IP;
                        }
                        else if (e.Column.FieldName == "Value")
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toggleSwitch1_Toggled(object sender, EventArgs e)
        {
            if (toggleSwitch1.IsOn == false)
            {
                //gridColumn11.Visible = false;
                //gridColumn8.Visible = false;
                //gridColumn1.Visible = true;
                //gridColumn1.VisibleIndex = 2;

                lciGridData.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                lcitxtIpCommon.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            }
            else
            {
                lciGridData.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                lcitxtIpCommon.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                //gridColumn8.Visible = true;
                //gridColumn11.Visible = true;
                //gridColumn1.Visible = false;
                //gridColumn8.VisibleIndex = 3;
            }
        }
    }
}
