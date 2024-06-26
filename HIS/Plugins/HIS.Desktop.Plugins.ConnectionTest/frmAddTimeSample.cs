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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using LIS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class frmAddTimeSample : Form
    {
        LisSampleADO lisSampleADO;
        Action<bool> refesh;
        public frmAddTimeSample(Action<bool> refesh, LisSampleADO lisSampleADO)
        {
            InitializeComponent();
            this.refesh = refesh;
            this.lisSampleADO = lisSampleADO;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!dxValidationProvider1.Validate())
                    return;
                if (dtSimpleTime.EditValue != null)
                {
                    bool success = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    lisSampleADO.SAMPLE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtSimpleTime.DateTime);
                    var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Update", ApiConsumers.LisConsumer, lisSampleADO, param);
                    if (rs != null)
                    {
                        success = true;
                        this.refesh(success);
                        this.Close();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void frmAddTimeSample_Load(object sender, EventArgs e)
        {
            string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
            this.Icon = Icon.ExtractAssociatedIcon(iconPath);

            this.SetCaptionByLanguageKey();

            dtSimpleTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.lisSampleADO.INTRUCTION_TIME ?? 0) ?? DateTime.Now;

            ValidationSampleTime(this.lisSampleADO.INTRUCTION_TIME ?? 0);
        }

        public void ValidationSampleTime(long intructionTime)
        {
            try
            {
                Validation.ValidateSampleTime rule = new Validation.ValidateSampleTime();
                rule.dtTime = dtSimpleTime;
                rule.intructionTime = intructionTime;
                this.dxValidationProvider1.SetValidationRule(dtSimpleTime, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmAddTimeSample
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample = new ResourceManager("HIS.Desktop.Plugins.ConnectionTest.Resources.Lang", typeof(frmAddTimeSample).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmAddTimeSample.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmAddTimeSample.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmAddTimeSample.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmAddTimeSample.bar1.Text", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
                this.bbtnSave.Caption = Inventec.Common.Resource.Get.Value("frmAddTimeSample.bbtnSave.Caption", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmAddTimeSample.Text", Resources.ResourceLanguageManager.LanguageResource__frmAddTimeSample, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
