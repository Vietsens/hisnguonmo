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
using AutoMapper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.UC.Icd;
using HIS.UC.Icd.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
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

namespace HIS.Desktop.Plugins.SummaryInforTreatmentRecords
{
    public partial class frmSummaryInforTreatmentRecords : HIS.Desktop.Utility.FormBase
    {
        internal long treatmentId;
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal MOS.EFMODEL.DataModels.HIS_TREATMENT currentTreatment { get; set; }


        public frmSummaryInforTreatmentRecords()
        {
            InitializeComponent();
        }

        public frmSummaryInforTreatmentRecords(Inventec.Desktop.Common.Modules.Module currentModule, long treatmentId)
		:base(currentModule)
        {
            InitializeComponent();
            this.treatmentId = treatmentId;
            this.currentModule = currentModule;
        }

        private void frmSummaryInforTreatmentRecords_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetIconFrm();
                SetCaptionByLanguageKey();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }

                if (this.treatmentId > 0)
                {
                    LoadDataDefault();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SummaryInforTreatmentRecords.Resources.Lang", typeof(HIS.Desktop.Plugins.SummaryInforTreatmentRecords.frmSummaryInforTreatmentRecords).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTreatmentCode.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciTreatmentCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciOpenTimeMedi.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciOpenTimeMedi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientTypeName.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciPatientTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardNumber.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciHeinCardNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardFromDate.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciHeinCardFromDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediOrgName.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciMediOrgName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDepartmentName.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciDepartmentName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCloseTimeMedi.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciCloseTimeMedi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeApDung.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciTimeApDung.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRouteName.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciRouteName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardToDate.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciHeinCardToDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinCardAdress.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lciHeinCardAdress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lci_JOIN_5_YEAR.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lbl_JOIN_5_YEAR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.lbl_PAID_6_MONTH.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmSummaryInforTreatmentRecords.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
