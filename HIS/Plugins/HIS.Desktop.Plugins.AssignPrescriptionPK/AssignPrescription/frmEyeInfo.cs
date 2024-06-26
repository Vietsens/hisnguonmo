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
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmEyeInfo : Form
    {
        HIS_SERVICE_REQ serviceReq;
        Action<HIS_SERVICE_REQ> actUpdate;
        const string lbl10 = "/10";

        public frmEyeInfo()
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public frmEyeInfo(HIS_SERVICE_REQ serviceReq, Action<HIS_SERVICE_REQ> actUpdate)
        {
            InitializeComponent();
            this.serviceReq = serviceReq;
            this.actUpdate = actUpdate;
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmEyeInfo_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();
            if (this.serviceReq != null && this.serviceReq.ID > 0)
            {
                txtTREAT_EYE_TENSION_RIGHT.Text = serviceReq.TREAT_EYE_TENSION_RIGHT;
                txtTREAT_EYE_TENSION_LEFT.Text = serviceReq.TREAT_EYE_TENSION_LEFT;
                txtTREAT_EYESIGHT_RIGHT.Text = serviceReq.TREAT_EYESIGHT_RIGHT;
                txtTREAT_EYESIGHT_LEFT.Text = serviceReq.TREAT_EYESIGHT_LEFT;
                txtTREAT_EYESIGHT_GLASS_RIGHT.Text = serviceReq.TREAT_EYESIGHT_GLASS_RIGHT;
                txtTREAT_EYESIGHT_GLASS_LEFT.Text = serviceReq.TREAT_EYESIGHT_GLASS_LEFT;
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmEyeInfo
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguagefrmEyeInfo = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmEyeInfo).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControl1.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelFortxtTREAT_EYESIGHT_GLASS_LEFT.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelFortxtTREAT_EYESIGHT_GLASS_LEFT.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelFortxtTREAT_EYESIGHT_LEFT.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelFortxtTREAT_EYESIGHT_LEFT.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelFortxtTREAT_EYESIGHT_GLASS_RIGHT.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelFortxtTREAT_EYESIGHT_GLASS_RIGHT.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelFortxtTREAT_EYESIGHT_RIGHT.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelFortxtTREAT_EYESIGHT_RIGHT.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelControl2.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.labelControl1.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.bbtnOk.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.bbtnOk.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem2.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem2.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem4.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem4.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.layoutControlItem4.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.bar2.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmEyeInfo.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmEyeInfo.Text", Resources.ResourceLanguageManager.LanguagefrmEyeInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void txtTREAT_EYESIGHT_RIGHT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTREAT_EYESIGHT_RIGHT.Text) && Inventec.Common.TypeConvert.Parse.ToDecimal(txtTREAT_EYESIGHT_RIGHT.Text) > 0)
                {
                    labelFortxtTREAT_EYESIGHT_RIGHT.Text = lbl10;
                }
                else
                    labelFortxtTREAT_EYESIGHT_RIGHT.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTREAT_EYESIGHT_LEFT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTREAT_EYESIGHT_LEFT.Text) && Inventec.Common.TypeConvert.Parse.ToDecimal(txtTREAT_EYESIGHT_LEFT.Text) > 0)
                {
                    labelFortxtTREAT_EYESIGHT_LEFT.Text = lbl10;
                }
                else
                    labelFortxtTREAT_EYESIGHT_LEFT.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTREAT_EYESIGHT_GLASS_RIGHT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTREAT_EYESIGHT_GLASS_RIGHT.Text) && Inventec.Common.TypeConvert.Parse.ToDecimal(txtTREAT_EYESIGHT_GLASS_RIGHT.Text) > 0)
                {
                    labelFortxtTREAT_EYESIGHT_GLASS_RIGHT.Text = lbl10;
                }
                else
                    labelFortxtTREAT_EYESIGHT_GLASS_RIGHT.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTREAT_EYESIGHT_GLASS_LEFT_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtTREAT_EYESIGHT_GLASS_LEFT.Text) && Inventec.Common.TypeConvert.Parse.ToDecimal(txtTREAT_EYESIGHT_GLASS_LEFT.Text) > 0)
                {
                    labelFortxtTREAT_EYESIGHT_GLASS_LEFT.Text = lbl10;
                }
                else
                    labelFortxtTREAT_EYESIGHT_GLASS_LEFT.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnOk_Click(object sender, EventArgs e)
        {
            try
            {
                HIS_SERVICE_REQ serviceReqData = new HIS_SERVICE_REQ();
                serviceReqData.TREAT_EYE_TENSION_RIGHT = txtTREAT_EYE_TENSION_RIGHT.Text;
                serviceReqData.TREAT_EYE_TENSION_LEFT = txtTREAT_EYE_TENSION_LEFT.Text;
                serviceReqData.TREAT_EYESIGHT_RIGHT = txtTREAT_EYESIGHT_RIGHT.Text;
                serviceReqData.TREAT_EYESIGHT_LEFT = txtTREAT_EYESIGHT_LEFT.Text;
                serviceReqData.TREAT_EYESIGHT_GLASS_RIGHT = txtTREAT_EYESIGHT_GLASS_RIGHT.Text;
                serviceReqData.TREAT_EYESIGHT_GLASS_LEFT = txtTREAT_EYESIGHT_GLASS_LEFT.Text;
                this.actUpdate(serviceReqData);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            bbtnOk_Click(null, null);
        }
    }
}
