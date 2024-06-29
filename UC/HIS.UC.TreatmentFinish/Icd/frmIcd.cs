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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using HIS.UC.SecondaryIcd;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Core;
using HIS.UC.Icd.ADO;
using HIS.UC.Icd;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.SecondaryIcd;
using HIS.UC.TreatmentFinish.ADO;
using HIS.Desktop.Utility;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.TreatmentFinish.Icd
{
    public partial class frmIcd : FormBase
    {
        Action<IcdTemp> ChooseData;
        List<HIS_ICD> currentIcds;
        internal SecondaryIcdProcessor subIcdProcessor;
        internal UserControl ucSecondaryIcd;

        private IcdTemp currentIcd;

        internal IcdProcessor icdProcessor;
        internal UserControl ucIcd;
        public frmIcd(IcdTemp icd, Action<IcdTemp> ChooseData)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                this.currentIcd = icd;
                this.ChooseData = ChooseData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmIcd_Load(object sender, EventArgs e)
        {
            try
            {
                InitICD();
                InitSubICD();
                LoadIcdToControl();
                LoadIcdSubToControl();
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitICD()
        {
            try
            {
                icdProcessor = new IcdProcessor();
                IcdInitADO ado = new IcdInitADO();
                ado.DelegateNextFocus = NextFocusSubICD;
                ado.Width = 649;
                ado.Height = 24;

                ado.DataIcds = BackendDataWorker.Get<HIS_ICD>();
                ado.IsColor = false;
                this.ucIcd = (UserControl)icdProcessor.Run(ado);

                if (this.ucIcd != null)
                {
                    this.layoutControlIcd.Controls.Add(this.ucIcd);
                    this.ucIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextFocusSubICD()
        {
            try
            {

                if (ucSecondaryIcd != null)
                {
                    this.subIcdProcessor.FocusControl(ucSecondaryIcd);
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmIcd
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.TreatmentFinish.Resources.Lang", typeof(frmIcd).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmIcd.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmIcd.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlSubIcd.Text = Inventec.Common.Resource.Get.Value("frmIcd.layoutControlSubIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlIcd.Text = Inventec.Common.Resource.Get.Value("frmIcd.layoutControlIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmIcd.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmIcd.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitSubICD()
        {
            try
            {
                currentIcds = BackendDataWorker.Get<HIS_ICD>().Where(p => p.IS_ACTIVE == 1).ToList();

                subIcdProcessor = new SecondaryIcdProcessor(new CommonParam(), this.currentIcds);
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdInitADO ado = new UC.SecondaryIcd.ADO.SecondaryIcdInitADO();
                ado.DelegateNextFocus = NextFocusSubIcdToDo;
                ado.DelegateGetIcdMain = GetIcdMainCode;
                ado.HisIcds = currentIcds;
                ado.Width = 649;
                ado.Height = 24;
                ado.TextLblIcd = "CĐ phụ:";
                ado.TootiplciIcdSubCode = "Chẩn đoán phụ";
                ado.TextNullValue = "Nhấn F1 để chọn chẩn đoán phụ";
                ado.limitDataSource = (int)HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumPageSize;
                ucSecondaryIcd = (UserControl)subIcdProcessor.Run(ado);

                if (ucSecondaryIcd != null)
                {
                    this.layoutControlSubIcd.Controls.Add(ucSecondaryIcd);
                    ucSecondaryIcd.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NextFocusSubIcdToDo()
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

        private string GetIcdMainCode()
        {
            string mainCode = "";
            try
            {

                var icdValue = this.UcIcdGetValue();
                if (icdValue != null && icdValue is UC.Icd.ADO.IcdInputADO)
                {
                    mainCode = ((UC.Icd.ADO.IcdInputADO)icdValue).ICD_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return mainCode;
        }

        public object UcIcdGetValue()
        {
            object result = null;
            try
            {
                IcdInputADO outPut = new IcdInputADO();
                IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                outPut.ICD_NAME = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                outPut.ICD_CODE = OjecIcd != null ? OjecIcd.ICD_CODE : "";
                result = outPut;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {

                bool vali = true;
                vali = IsValiICD() && vali;
                vali = IsValiICDSub() && vali;
                if (!vali) return;

                ADO.IcdTemp tempIcd = new IcdTemp();
                IcdInputADO OjecIcd = (IcdInputADO)icdProcessor.GetValue(ucIcd);
                tempIcd.SHOW_ICD_NAME = OjecIcd != null ? OjecIcd.ICD_NAME : "";
                tempIcd.SHOW_ICD_CODE = OjecIcd != null ? OjecIcd.ICD_CODE : "";

                SecondaryIcdDataADO icdSub = (SecondaryIcdDataADO)this.subIcdProcessor.GetValue(this.ucSecondaryIcd);
                tempIcd.SHOW_ICD_SUB_CODE = icdSub != null ? icdSub.ICD_SUB_CODE : "";
                tempIcd.SHOW_ICD_TEXT = icdSub != null ? icdSub.ICD_TEXT : "";
                this.ChooseData(tempIcd);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void LoadIcdToControl()
        {
            try
            {
                HIS.UC.Icd.ADO.IcdInputADO ado = new HIS.UC.Icd.ADO.IcdInputADO();
                ado.ICD_CODE = currentIcd.SHOW_ICD_CODE;
                ado.ICD_NAME = currentIcd.SHOW_ICD_NAME;
                icdProcessor.Reload(this.ucIcd, ado);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadIcdSubToControl()
        {
            try
            {
                HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO ado = new HIS.UC.SecondaryIcd.ADO.SecondaryIcdDataADO();
                ado.ICD_SUB_CODE = currentIcd.SHOW_ICD_SUB_CODE;
                ado.ICD_TEXT = currentIcd.SHOW_ICD_TEXT;
                subIcdProcessor.Reload(this.ucSecondaryIcd, ado);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
