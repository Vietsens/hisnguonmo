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
using HIS.Desktop.Plugins.Bordereau.ADO;
using HIS.Desktop.Plugins.Bordereau.Base;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Bordereau.ChoosePatientType
{
    public partial class frmChoosePatientType : Form
    {
        List<SereServADO> sereServADOSelecteds { get; set; }
        DelegateSelectData refeshData { get; set; }
        List<V_HIS_PATIENT_TYPE_ALTER> patientTypeAlters { get; set; }
        public string AllowAssignOffListMedicineMaterialHeinCardNumberPrefix { get; private set; }
        V_HIS_TREATMENT currentTreatment { get; set; }
        public frmChoosePatientType(V_HIS_TREATMENT currentTreatment, List<SereServADO> _sereServADOSelecteds, List<V_HIS_PATIENT_TYPE_ALTER> _patientTypeAlters, DelegateSelectData _refeshData)
        {
            InitializeComponent();
            SetCaptionByLanguageKey();
            try
            {
                this.currentTreatment = currentTreatment;
                this.sereServADOSelecteds = _sereServADOSelecteds;
                this.refeshData = _refeshData;
                this.patientTypeAlters = _patientTypeAlters;
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
                if (btnSave.Enabled)
                {
                    btnSave_Click(null, null);
                }
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
                if (cboPatientType.EditValue == null)
                {
                    MessageBox.Show("Vui lòng chọn đối tượng thanh toán");
                    return;
                }
                if (refeshData != null)
                {
                    refeshData(Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientType.EditValue.ToString()));
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChoosePatientType_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                AllowAssignOffListMedicineMaterialHeinCardNumberPrefix = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.BHYT.ALLOW_ASSIGN_OFF_LIST_MEDICINE_MATERIAL__HEIN_CARD_NUMBER_PREFIX");
                LoadComboPatientType();


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

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

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmChoosePatientType.layoutControl1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmChoosePatientType.btnSave.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmChoosePatientType.cboPatientType.Properties.NullText", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmChoosePatientType.layoutControlItem1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmChoosePatientType.bar1.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmChoosePatientType.barButtonItem1.Caption", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmChoosePatientType.Text", ResourceLangManager.LanguageFrmBorderau, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmChoosePatientType_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                patientTypeAlters = null;
                refeshData = null;
                sereServADOSelecteds = null;
                this.btnSave.Click -= new System.EventHandler(this.btnSave_Click);
                this.cboPatientType.Closed -= new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboPatientType_Closed);
                this.barButtonItem1.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
                this.Load -= new System.EventHandler(this.frmChoosePatientType_Load);
                gridLookUpEdit1View.GridControl.DataSource = null;
                cboPatientType.Properties.DataSource = null;
                barDockControlRight = null;
                barDockControlLeft = null;
                barDockControlBottom = null;
                barDockControlTop = null;
                barButtonItem1 = null;
                bar1 = null;
                barManager1 = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                layoutControlItem1 = null;
                layoutControlGroup1 = null;
                gridLookUpEdit1View = null;
                cboPatientType = null;
                btnSave = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
