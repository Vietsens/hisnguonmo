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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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
using TYT.EFMODEL.DataModels;

namespace TYT.Desktop.Plugins.TYTMalaria
{
    public partial class frmMalaria : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT patient { get; set; }

        TYT_MALARIA feTusBorn { get; set; }

        int action = 0;

        public frmMalaria()
        {
            InitializeComponent();
        }

        public frmMalaria(Inventec.Desktop.Common.Modules.Module _currentModule, TYT.EFMODEL.DataModels.TYT_MALARIA __FETUS_EXAM)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.feTusBorn = __FETUS_EXAM;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }
            this.action = 2;
        }

        public frmMalaria(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
            : base(_currentModule)
        {
            InitializeComponent();
            SetIcon();
            this.currentModule = _currentModule;
            this.patient = _patient;
            if (this.currentModule != null)
            {
                this.Text = this.currentModule.text;
            }

            this.action = 1;
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

        private void frm_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();

                SetDefault();

                FillDataToControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefault()
        {
            try
            {
                chkIsHasFetus.Focus();
                chkIsHasFetus.BackColor = Color.Gray;
                chkIsHasFetus.CheckState = CheckState.Unchecked;
                chkIsfever.CheckState = CheckState.Unchecked;
                rdoTestType__Lam.CheckState = CheckState.Unchecked;
                rdoTestType__QueThu.CheckState = CheckState.Unchecked;
                txtTestResult.Text = "";
                txtDiagnose.Text = "";
                txtTreatmentResult.Text = "";
                txtMedicine.Text = "";
                txtMedicineCtdt.Text = "";
                dtDiagnoseTime.EditValue = null;
                txtDiagnosePalace.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToControl()
        {
            try
            {
                if (this.feTusBorn != null)
                {

                    chkIsHasFetus.CheckState = this.feTusBorn.IS_HAS_FETUS == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsfever.CheckState = this.feTusBorn.IS_FEVER == 1 ? CheckState.Checked : CheckState.Unchecked;
                    rdoTestType__Lam.CheckState = this.feTusBorn.TEST_TYPE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    rdoTestType__QueThu.CheckState = this.feTusBorn.TEST_TYPE == 2 ? CheckState.Checked : CheckState.Unchecked;
                    txtTestResult.Text = this.feTusBorn.TEST_RESULT;
                    txtDiagnose.Text = this.feTusBorn.DIAGNOSE;
                    txtTreatmentResult.Text = this.feTusBorn.TREATMENT_RESULT;
                    txtMedicine.Text = this.feTusBorn.MEDICINE;
                    txtMedicineCtdt.Text = this.feTusBorn.MEDICINE_CTDT;
                    if (this.feTusBorn.DIAGNOSE_TIME != null)
                    {
                        dtDiagnoseTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.feTusBorn.DIAGNOSE_TIME ?? 0);
                    }
                    else
                    {
                        dtDiagnoseTime.EditValue = null;
                    }
                    txtDiagnosePalace.Text = this.feTusBorn.DIAGNOSE_PLACE;
                }
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
                bool success = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                btnSave.Focus();
                if (this.feTusBorn != null)
                {
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                }
                else
                {
                    this.feTusBorn = new TYT_MALARIA();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_MALARIA>(this.feTusBorn, this.patient);
                    this.feTusBorn.PERSON_ADDRESS = this.patient.VIR_ADDRESS;
                    this.feTusBorn.ID = 0;
                    //this.feTusBorn.BHYT_NUMBER = this.patient.TDL_HEIN_CARD_NUMBER;
                    this.feTusBorn.VIR_PERSON_NAME = this.patient.VIR_PATIENT_NAME;
                }
                if (chkIsHasFetus.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_HAS_FETUS = 1;
                }
                else
                {
                    this.feTusBorn.IS_HAS_FETUS = null;
                }
                if (chkIsfever.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_FEVER = 1;
                }
                else
                {
                    this.feTusBorn.IS_FEVER = null;
                }

                if (rdoTestType__Lam.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.TEST_TYPE = 1;
                }
                else if (rdoTestType__QueThu.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.TEST_TYPE = 2;
                }
                else
                {
                    this.feTusBorn.TEST_TYPE = null;
                }
                this.feTusBorn.TEST_RESULT = txtTestResult.Text.Trim();
                this.feTusBorn.DIAGNOSE = txtDiagnose.Text.Trim();
                this.feTusBorn.TREATMENT_RESULT = txtTreatmentResult.Text.Trim();
                this.feTusBorn.MEDICINE = txtMedicine.Text.Trim();
                this.feTusBorn.MEDICINE_CTDT = txtMedicineCtdt.Text.Trim();
                if (dtDiagnoseTime.EditValue != null && dtDiagnoseTime.DateTime != DateTime.MinValue)
                    this.feTusBorn.DIAGNOSE_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDiagnoseTime.EditValue).ToString("yyyyMMdd") + "000000");
                else
                    this.feTusBorn.DIAGNOSE_TIME = null;

                this.feTusBorn.DIAGNOSE_PLACE = txtDiagnosePalace.Text.Trim();

                this.feTusBorn.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_MALARIA _result = new TYT_MALARIA();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    _result = new BackendAdapter(param).Post<TYT_MALARIA>("api/TytMalaria/Create", ApiConsumers.TytConsumer, this.feTusBorn, param);
                }
                else
                {
                    _result = new BackendAdapter(param).Post<TYT_MALARIA>("api/TytMalaria/Update", ApiConsumers.TytConsumer, this.feTusBorn, param);
                }

                if (_result != null && _result.ID > 0)
                {
                    success = true;
                    this.feTusBorn = _result;
                    this.action = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                this.action = 1;
                this.SetDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonI__Refesh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsHasFetus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsfever.Focus();
                    chkIsfever.SelectAll();
                    chkIsfever.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsHasFetus_Leave(object sender, EventArgs e)
        {
            try
            {
                chkIsHasFetus.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsfever_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoTestType__Lam.Focus();
                    rdoTestType__Lam.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsfever_Leave(object sender, EventArgs e)
        {
            try
            {
                chkIsfever.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoTestType__Lam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoTestType__QueThu.Focus();
                    rdoTestType__QueThu.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoTestType__Lam_Leave(object sender, EventArgs e)
        {
            try
            {
                rdoTestType__Lam.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoTestType__QueThu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTestResult.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoTestType__QueThu_Leave(object sender, EventArgs e)
        {
            try
            {
                rdoTestType__QueThu.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDiagnoseTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDiagnosePalace.Focus();
                    txtDiagnosePalace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDiagnosePalace_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                    btnSave.Select();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoTestType__Lam_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTestType__Lam.CheckState == CheckState.Checked)
                {
                    rdoTestType__QueThu.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoTestType__QueThu_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoTestType__QueThu.CheckState == CheckState.Checked)
                {
                    rdoTestType__Lam.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
