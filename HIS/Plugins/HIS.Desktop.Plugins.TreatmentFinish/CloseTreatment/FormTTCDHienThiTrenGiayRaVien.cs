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
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.UTILITY;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentFinish.CloseTreatment
{
    public partial class FormTTCDHienThiTrenGiayRaVien : Form
    {
        DelegateSelectData delegateData;
        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;
        internal string AutoCheckIcd = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<String>("HIS.Desktop.Plugins.AutoCheckIcd");
        internal List<MOS.EFMODEL.DataModels.HIS_ICD> listIcd;
        private List<ModuleControlADO> ModuleControls { get; set; }

        internal CultureInfo cultureLang = LanguageManager.GetCulture();
        HIS_TREATMENT data;
        HIS_TREATMENT prosess;
        public FormTTCDHienThiTrenGiayRaVien(HIS_TREATMENT currentHisTreatment_, HIS.Desktop.Common.DelegateSelectData dataResult_)
        {
            InitializeComponent();
            SetIcon();
            delegateData = dataResult_;
            data = currentHisTreatment_;
        }

        private void FormTTCDHienThiTrenGiayRaVien_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();
            listIcd = BackendDataWorker.Get<HIS_ICD>().Where(a => a.IS_ACTIVE == Constant.IS_TRUE && (a.IS_TRADITIONAL == null || a.IS_TRADITIONAL == Constant.IS_FALSE)).OrderBy(o => o.ICD_CODE).ToList();
            InitUcIcd();
            InitUcSecondaryIcd();

            if (!string.IsNullOrEmpty(data.SHOW_ICD_CODE))
            {
                LoadIcd(data.SHOW_ICD_CODE, data.SHOW_ICD_NAME);
            }
            else
            {
                LoadIcd(data.ICD_CODE, data.ICD_NAME);
            }

            if (!string.IsNullOrEmpty(data.SHOW_ICD_SUB_CODE))
            {
                LoaducSecondaryIcd(data.SHOW_ICD_SUB_CODE, data.SHOW_ICD_TEXT);
            }
            else
            {
                LoaducSecondaryIcd(data.ICD_SUB_CODE, data.ICD_TEXT);
            }
        }
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentFinish.Resources.Lang", typeof(FormTTCDHienThiTrenGiayRaVien).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("FormTTCDHienThiTrenGiayRaVien.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("FormTTCDHienThiTrenGiayRaVien.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("FormTTCDHienThiTrenGiayRaVien.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("FormTTCDHienThiTrenGiayRaVien.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("FormTTCDHienThiTrenGiayRaVien.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcIcd()
        {
            try
            {
                icdProcessor = new HIS.UC.Icd.IcdProcessor();
                HIS.UC.Icd.ADO.IcdInitADO ado = new HIS.UC.Icd.ADO.IcdInitADO();
                ado.DelegateNextFocus = NextForcusSubIcd;
                ado.DelegateRequiredCause = LoadRequiredCause;
                ado.IsUCCause = false;
                ado.Width = 440;
                ado.Height = 24;
                ado.DataIcds = listIcd;
                ado.AutoCheckIcd = AutoCheckIcd == "1";
                ucIcd = (UserControl)icdProcessor.Run(ado);

                if (ucIcd != null)
                {
                    this.panelControlIcd.Controls.Add(ucIcd);
                    ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadRequiredCause(bool isRequired)
        {
            try
            {
                ///

                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    this.icdProcessor.SetRequired(this.ucIcd, isRequired, HIS.UC.Icd.ADO.Template.NoFocus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void NextForcusSubIcd()
        {
            try
            {
                if (ucSecondaryIcd != null && ucSecondaryIcd.Visible == true)
                {
                    ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                    ModuleControls = controlProcess.GetControls(ucSecondaryIcd);
                    int count = 0;
                    foreach (var itemCtrl in ModuleControls)
                    {
                        if (itemCtrl.ControlName == "txtIcdSubCode")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                        else if (itemCtrl.ControlName == "txtIcdText")
                        {
                            if (itemCtrl.IsVisible)
                            {
                                count = count + 1;
                            }
                        }
                    }

                    if (count > 0)
                    {
                        subIcdProcessor.FocusControl(ucSecondaryIcd);
                    }
                    else
                    {
                        NextForcusOut();
                    }
                }
                else
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcSecondaryIcd()
        {
            try
            {
                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), listIcd);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextForcusOut;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.Width = 200;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán phụ";
                ado.TextNullValue = GetStringFromKey("IVT_LANGUAGE_KEY__FORM_TREATMENT_FINISH__TXT_ICD_TEXT__NULL_VALUE");
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.panelControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {
                if (this.icdProcessor != null && this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        private void NextForcusOut()
        {
            try
            {
                btnSave.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal string GetStringFromKey(string key)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrEmpty(key))
                {
                    result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
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
                bool vali = true;
                vali = IsValiICD() && vali;
                vali = IsValiICDSub() && vali;
                if (!vali) return;

                prosess = new HIS_TREATMENT();

                if (this.ucIcd != null)
                {
                    var icdValue = this.icdProcessor.GetValue(this.ucIcd);
                    if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                    {
                        prosess.SHOW_ICD_CODE = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                        prosess.SHOW_ICD_NAME = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_NAME;
                    }
                }

                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        prosess.SHOW_ICD_SUB_CODE = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        prosess.SHOW_ICD_TEXT = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                LoadDataHisTreatMent();
                this.delegateData(prosess);
                
               
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prosess), prosess));
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private async Task LoadDataHisTreatMent()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisTreatMent => 1");
                data.SHOW_ICD_CODE = prosess.SHOW_ICD_CODE;
                data.SHOW_ICD_NAME = prosess.SHOW_ICD_NAME;
                data.SHOW_ICD_SUB_CODE = prosess.SHOW_ICD_SUB_CODE;
                data.SHOW_ICD_TEXT = prosess.SHOW_ICD_TEXT;
                BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_TREATMENT), data, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                Inventec.Common.Logging.LogSystem.Info("async Task LoadDataHisTreatMent => 2");
                Inventec.Common.Logging.LogSystem.Info("Kết thúc điều trị");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoadIcd(string icdCode, string icdName)
        {
            try
            {
                IcdInputADO icd = new IcdInputADO();
                icd.ICD_CODE = icdCode;
                icd.ICD_NAME = icdName;
                if (ucIcd != null)
                {
                    icdProcessor.Reload(ucIcd, icd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void LoaducSecondaryIcd(string icdCode, string icdName)
        {
            try
            {
                SecondaryIcdDataADO subIcd = new SecondaryIcdDataADO();
                subIcd.ICD_SUB_CODE = icdCode;
                subIcd.ICD_TEXT = icdName;
                if (ucSecondaryIcd != null)
                {
                    subIcdProcessor.Reload(ucSecondaryIcd, subIcd);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool IsValiICD()
        {
            bool result = true;
            try
            {
                result = (bool)icdProcessor.ValidationIcd(ucIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        bool IsValiICDSub()
        {
            bool result = true;
            try
            {
                result = (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
