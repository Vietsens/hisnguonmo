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
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Forms;
using System.Drawing;
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.MixedInfusion
{
    public partial class frmMixedInfusion : Form
    {
        DelegateSelectData ReloadData;
        MediMatyTypeADO mediMatyTypeADO = new MediMatyTypeADO();
        public frmMixedInfusion(DelegateSelectData _ReloadData, MediMatyTypeADO _mediMatyTypeADO)
        {
            InitializeComponent();
            this.ReloadData = _ReloadData;
            this.mediMatyTypeADO = _mediMatyTypeADO;
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

        private void frmMixedInfusion_Load(object sender, EventArgs e)
        {
            try
            {
                //SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
                txtTutorialMix.Text = this.mediMatyTypeADO != null ? this.mediMatyTypeADO.TUTORIAL : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguagefrmAssignPrescription = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription.frmIsExpendType).Assembly);

                this.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.btnUpdate.Text", Resources.ResourceLanguageManager.LanguagefrmAssignPrescription, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện frmMixedInfusion
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion = new ResourceManager("HIS.Desktop.Plugins.AssignPrescriptionPK.Resources.Lang", typeof(frmMixedInfusion).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.btnUpdate.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
                this.bar2.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.bar2.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
                this.bbtnUpdate.Caption = Inventec.Common.Resource.Get.Value("frmMixedInfusion.bbtnUpdate.Caption", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMixedInfusion.Text", Resources.ResourceLanguageManager.LanguageResourcefrmMixedInfusion, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                this.ReloadData(this.txtTutorialMix.Text);
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void bbtnUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            
            try
            {
                btnUpdate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
            
        }


    }
}
