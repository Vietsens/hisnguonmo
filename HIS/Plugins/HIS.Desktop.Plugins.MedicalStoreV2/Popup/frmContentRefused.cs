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
using HIS.Desktop.Plugins.MedicalStoreV2.ADO;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.MedicalStoreV2.Popup
{
    public partial class frmContentRefused : Form
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        TreatmentADO TreatmentADO;

        public frmContentRefused(Inventec.Desktop.Common.Modules.Module _currentModule, TreatmentADO _TreatmentADO)
        {
            InitializeComponent();
            
            try
            {
                this.currentModule = _currentModule;
                this.TreatmentADO = _TreatmentADO;
                SetIcon();
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
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmContentRefused_Load(object sender, EventArgs e)
        {
            SetCaptionByLanguageKey();
            SetDefaultData();
        }


        private void SetDefaultData()
        {
            try
            {
                if (this.TreatmentADO != null)
                {
                    txtRejectStoreReason.Text = this.TreatmentADO.REJECT_STORE_REASON;
                    txtRecordInspectionRejectNote.Text = this.TreatmentADO.RECORD_INSPECTION_REJECT_NOTE;
                }

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
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource_frmContentRefused = new ResourceManager("HIS.Desktop.Plugins.MedicalStoreV2.Resources.Lang", typeof(frmContentRefused).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmContentRefused.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource_frmContentRefused, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmContentRefused.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource_frmContentRefused, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmContentRefused.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource_frmContentRefused, LanguageManager.GetCulture());
                this.btnClose.Text = Inventec.Common.Resource.Get.Value("frmContentRefused.btnClose.Text", Resources.ResourceLanguageManager.LanguageResource_frmContentRefused, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmContentRefused.Text", Resources.ResourceLanguageManager.LanguageResource_frmContentRefused, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);                
            }
        }



    }
}
