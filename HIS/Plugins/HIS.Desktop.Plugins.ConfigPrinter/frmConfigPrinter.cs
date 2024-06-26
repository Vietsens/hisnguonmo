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
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.ConfigPrinter.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConfigPrinter
{
    public partial class frmConfigPrinter : HIS.Desktop.Utility.FormBase
    {
        private const string HIS_CONFIG__PRINT_TYPE__PRINTER = "His.Config.PrintType.Printer";

        Dictionary<string, string> dicPrinter = new Dictionary<string, string>();

        List<SAR_PRINT_TYPE> printTypes = null;
        List<PrintTypeADO> listPrintTypeADO = new List<PrintTypeADO>();

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        Inventec.Desktop.Common.Modules.Module currentModule;

        public frmConfigPrinter(Inventec.Desktop.Common.Modules.Module module, List<SAR_PRINT_TYPE> listPrintType)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.SetIcon();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.printTypes = listPrintType;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmConfigPrinter_Load(object sender, EventArgs e)
        {
            try
            {
                LoadKeyFrmLanguage();
                if (printTypes == null || printTypes.Count <= 0)
                {
                    btnSave.Enabled = false;
                    return;
                }
                ProcessListPrinterBeforeConfig();
                LoadDataToCombo();
                LoadDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListPrinterBeforeConfig()
        {
            try
            {
            
                string value = (System.Configuration.ConfigurationSettings.AppSettings[HIS_CONFIG__PRINT_TYPE__PRINTER] ?? "");
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.ConfigPrinter");
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlStateWorker), controlStateWorker);
                Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO);
              
                string value1 = null;
                if (!String.IsNullOrEmpty(value) && this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "His.Config.PrintType.Printer")
                        {
                            value1 = item.VALUE;
                        }
                    }
                    string[] configs = value.Split(';');
                    string[] configs1 = value1.Split(';');
                    if (configs == null || configs.Length <= 0)
                    {
                        throw new NullReferenceException("Khong cat duoc du lieu cau hinh: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => value), value));
                    }
                    if (configs1 == null || configs1.Length <= 0)
                    {
                        throw new NullReferenceException("Khong cat duoc du lieu cau hinh: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => value1), value1));
                    }
                    foreach (var item in configs1)
                    {
                        if (String.IsNullOrEmpty(item))
                            continue;
                        var data = item.Split(':');
                        if (data == null || data.Length != 2)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Du lieu cau hinh khong chinh xac: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            continue;
                        }
                        if (String.IsNullOrEmpty(data[0]) || String.IsNullOrEmpty(data[0].Trim()) || String.IsNullOrEmpty(data[1]) || String.IsNullOrEmpty(data[1].Trim()))
                        {
                            Inventec.Common.Logging.LogSystem.Info("Ma loai in hoac ten may in trong: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            continue;
                        }
                        dicPrinter[data[0].Trim()] = data[1].Trim();
                        dicPrinter.Distinct();
                    }
                    foreach (var item in configs)
                    {
                        if (String.IsNullOrEmpty(item))
                            continue;
                        var data = item.Split(':');
                        if (data == null || data.Length != 2)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Du lieu cau hinh khong chinh xac: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            continue;
                        }
                        if (String.IsNullOrEmpty(data[0]) || String.IsNullOrEmpty(data[0].Trim()) || String.IsNullOrEmpty(data[1]) || String.IsNullOrEmpty(data[1].Trim()))
                        {
                            Inventec.Common.Logging.LogSystem.Info("Ma loai in hoac ten may in trong: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            continue;
                        }
                        dicPrinter[data[0].Trim()] = data[1].Trim();
                        dicPrinter.Distinct();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                repositoryItemCboBoxPrinter.Items.AddRange(Inventec.Common.Print.FlexCelPrintStore.GetSystemPrintNames());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridControl()
        {
            try
            {
                listPrintTypeADO = new List<PrintTypeADO>();
                if (printTypes != null && printTypes.Count > 0)
                {
                    listPrintTypeADO = (from r in printTypes select new PrintTypeADO(r)).ToList();
                }
                gridControlPrintType.BeginUpdate();
                gridControlPrintType.DataSource = listPrintTypeADO;
                gridControlPrintType.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled)
                    return;
                var listData = listPrintTypeADO.Where(o => !String.IsNullOrEmpty(o.PRINT_TYPE_CODE) && !String.IsNullOrEmpty(o.PRINTER_NAME)).ToList();
                if (listData == null || listData.Count <= 0)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                string value = SetValueConfig(listData);
                if (!String.IsNullOrEmpty(value))
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == HIS_CONFIG__PRINT_TYPE__PRINTER && o.MODULE_LINK == "HIS.Desktop.Plugins.ConfigPrinter").FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = value;
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = HIS_CONFIG__PRINT_TYPE__PRINTER;
                        csAddOrUpdate.VALUE = value;
                        csAddOrUpdate.MODULE_LINK = "HIS.Desktop.Plugins.ConfigPrinter";
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);

                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                    this.Close();
                }
                else
                {
                    MessageManager.Show(param, success);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string SetValueConfig(List<PrintTypeADO> listData)
        {
            string result = "";
            try
            {
                List<string> listConfig = new List<string>();
                foreach (var item in listData)
                {
                    dicPrinter[item.PRINT_TYPE_CODE] = item.PRINTER_NAME;
                }
                if (dicPrinter.Count > 0)
                {
                    foreach (var dic in dicPrinter)
                    {
                        listConfig.Add(dic.Key + ":" + dic.Value);
                    }
                }
                if (listConfig.Count > 0)
                {
                    result = String.Join(";", listConfig);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmConfigPrinter;
                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_CONFIG_PRINTER__BTN_SAVE", lang, cul);

                //grid Expense
                this.gridColumn_PrintType_PrinterName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_CONFIG_PRINTER__GRID_PRINT_TYPE__COLUMN_PRINTER_NAME", lang, cul);
                this.gridColumn_PrintType_PrintTypeCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_CONFIG_PRINTER__GRID_PRINT_TYPE__COLUMN_PRINT_TYPE_CODE", lang, cul);
                this.gridColumn_PrintType_PrintTypeName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_CONFIG_PRINTER__GRID_PRINT_TYPE__COLUMN_PRINT_TYPE_NAME", lang, cul);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
