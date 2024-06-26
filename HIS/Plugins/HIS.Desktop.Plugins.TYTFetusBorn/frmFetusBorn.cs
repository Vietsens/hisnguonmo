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

namespace HIS.Desktop.Plugins.TYTFetusBorn
{
    public partial class frmFetusBorn : HIS.Desktop.Utility.FormBase
    {

        internal Inventec.Desktop.Common.Modules.Module currentModule;

        V_HIS_PATIENT patient { get; set; }

        TYT_FETUS_BORN feTusBorn { get; set; }

        int action = 0;

        public frmFetusBorn()
        {
            InitializeComponent();
        }

        public frmFetusBorn(Inventec.Desktop.Common.Modules.Module _currentModule, TYT.EFMODEL.DataModels.TYT_FETUS_BORN __FETUS_EXAM)
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

        public frmFetusBorn(Inventec.Desktop.Common.Modules.Module _currentModule, V_HIS_PATIENT _patient)
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
                chkIsFetusManage.Focus();
                chkIsFetusManage.BackColor = Color.Gray;
                chkIsFetusManage.CheckState = CheckState.Unchecked;
                chkIsUVFull.CheckState = CheckState.Unchecked;
                spinParaNormalCount.EditValue = null;
                spinParaPrematurelyCount.EditValue = null;
                spinParaMiscarriageCount.EditValue = null;
                spinParaChildCount.EditValue = null;
                chkHIVBefore.CheckState = CheckState.Unchecked;
                chkHIVBorn.CheckState = CheckState.Unchecked;
                chkCheckCase33.CheckState = CheckState.Unchecked;
                chkCheckCase43.CheckState = CheckState.Unchecked;
                chkIsChildDeath.CheckState = CheckState.Unchecked;
                chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                txtFirstWeekExam.Text = "";
                chkIsDeath.CheckState = CheckState.Unchecked;
                txtObstetricComplication.Text = "";
                rdoFemale.CheckState = CheckState.Unchecked;
                rdoMale.CheckState = CheckState.Unchecked;
                spinChildWeight.EditValue = null;
                txtNote.Text = "";
                txtExam742.Text = "";
                txtBornMethod.Text = "";
                txtMidWifeName.Text = "";
                txtChildStatus.Text = "";
                txtBornPlace.Text = "";
                chkIsBreastFeedingFirstHour.CheckState = CheckState.Unchecked;
                chkIsK1.CheckState = CheckState.Unchecked;
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

                    chkIsFetusManage.CheckState = this.feTusBorn.IS_FETUS_MANAGE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsUVFull.CheckState = this.feTusBorn.IS_UV_FULL == 1 ? CheckState.Checked : CheckState.Unchecked;
                    spinParaNormalCount.EditValue = this.feTusBorn.PARA_NORMAL_COUNT;
                    spinParaPrematurelyCount.EditValue = this.feTusBorn.PARA_PREMATURELY_COUNT;
                    spinParaMiscarriageCount.EditValue = this.feTusBorn.PARA_MISCARRIAGE_COUNT;
                    spinParaChildCount.EditValue = this.feTusBorn.PARA_CHILD_COUNT;
                    chkHIVBefore.CheckState = this.feTusBorn.IS_HIV_BEFORE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkHIVBorn.CheckState = this.feTusBorn.IS_HIV_BORN == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCase33.CheckState = this.feTusBorn.CHECK_CASE == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCase43.CheckState = this.feTusBorn.CHECK_CASE == 2 ? CheckState.Checked : CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = this.feTusBorn.CHECK_CASE == null ? CheckState.Checked : CheckState.Unchecked;
                    txtFirstWeekExam.Text = this.feTusBorn.FIRST_WEEK_EXAM;
                    chkIsDeath.CheckState = this.feTusBorn.IS_DEATH == 1 ? CheckState.Checked : CheckState.Unchecked;
                    txtObstetricComplication.Text = this.feTusBorn.OBSTETRIC_COMPLICATION;
                    rdoFemale.CheckState = this.feTusBorn.CHILD_GENDER_ID == 1 ? CheckState.Checked : CheckState.Unchecked;
                    rdoMale.CheckState = this.feTusBorn.CHILD_GENDER_ID == 2 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsChildDeath.CheckState = this.feTusBorn.IS_CHILD_DEATH == 1 ? CheckState.Checked : CheckState.Unchecked;
                    spinChildWeight.EditValue = this.feTusBorn.CHILD_WEIGHT;
                    txtNote.Text = this.feTusBorn.NOTE;
                    txtBornMethod.Text = this.feTusBorn.BORN_METHOD;
                    txtMidWifeName.Text = this.feTusBorn.MIDWIFE_NAME;
                    txtChildStatus.Text = this.feTusBorn.CHILD_STATUS;
                    txtBornPlace.Text = this.feTusBorn.BORN_PLACE;
                    txtExam742.Text = this.feTusBorn.EXAM_742;
                    chkIsBreastFeedingFirstHour.CheckState = this.feTusBorn.IS_BREASTFEEDING_FIRST_HOUR == 1 ? CheckState.Checked : CheckState.Unchecked;
                    chkIsK1.CheckState = this.feTusBorn.IS_K1 == 1 ? CheckState.Checked : CheckState.Unchecked;
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
                    this.feTusBorn = new TYT_FETUS_BORN();
                    Inventec.Common.Mapper.DataObjectMapper.Map<TYT_FETUS_BORN>(this.feTusBorn, this.patient);
                    this.feTusBorn.PERSON_ADDRESS = this.patient.VIR_ADDRESS;
                    this.feTusBorn.ID = 0;
                    this.feTusBorn.BHYT_NUMBER = this.patient.TDL_HEIN_CARD_NUMBER;
                    this.feTusBorn.VIR_PERSON_NAME = this.patient.VIR_PATIENT_NAME;
                }
                if (chkIsFetusManage.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_FETUS_MANAGE = 1;
                }
                else
                {
                    this.feTusBorn.IS_FETUS_MANAGE = null;
                }
                if (chkIsUVFull.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_UV_FULL = 1;
                }
                else
                {
                    this.feTusBorn.IS_UV_FULL = null;
                }
                if (spinParaNormalCount.EditValue != null)
                {
                    this.feTusBorn.PARA_NORMAL_COUNT = (long)spinParaNormalCount.Value;
                }
                else
                {
                    this.feTusBorn.PARA_NORMAL_COUNT = null;
                }
                if (spinParaPrematurelyCount.EditValue != null)
                {
                    this.feTusBorn.PARA_PREMATURELY_COUNT = (long)spinParaPrematurelyCount.Value;
                }
                else
                {
                    this.feTusBorn.PARA_PREMATURELY_COUNT = null;
                }
                if (spinParaMiscarriageCount.EditValue != null)
                {
                    this.feTusBorn.PARA_MISCARRIAGE_COUNT = (long)spinParaMiscarriageCount.Value;
                }
                else
                {
                    this.feTusBorn.PARA_MISCARRIAGE_COUNT = null;
                }
                if (spinParaChildCount.EditValue != null)
                {
                    this.feTusBorn.PARA_CHILD_COUNT = (long)spinParaChildCount.Value;
                }
                else
                {
                    this.feTusBorn.PARA_CHILD_COUNT = null;
                }

                if (chkHIVBefore.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_HIV_BEFORE = 1;
                }
                else
                {
                    this.feTusBorn.IS_HIV_BEFORE = null;
                }
                if (chkHIVBorn.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_HIV_BORN = 1;
                }
                else
                {
                    this.feTusBorn.IS_HIV_BORN = null;
                }
                if (chkIsChildDeath.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_CHILD_DEATH = 1;
                }
                else
                {
                    this.feTusBorn.IS_CHILD_DEATH = null;
                }
                if (chkCheckCase33.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.CHECK_CASE = 1;
                }
                else if (chkCheckCase43.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.CHECK_CASE = 2;
                }
                else
                {
                    this.feTusBorn.CHECK_CASE = null;
                }
                this.feTusBorn.FIRST_WEEK_EXAM = txtFirstWeekExam.Text;


                if (chkIsDeath.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_DEATH = 1;
                }
                else
                {
                    this.feTusBorn.IS_DEATH = null;
                }
                this.feTusBorn.OBSTETRIC_COMPLICATION = txtObstetricComplication.Text;

                if (rdoFemale.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.CHILD_GENDER_ID = 1;
                }
                else if (rdoMale.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.CHILD_GENDER_ID = 2;
                }
                else
                {
                    this.feTusBorn.CHILD_GENDER_ID = null;
                }

                if (spinChildWeight.EditValue != null)
                {
                    this.feTusBorn.CHILD_WEIGHT = (long)spinChildWeight.Value;
                }
                this.feTusBorn.NOTE = txtNote.Text;
                this.feTusBorn.EXAM_742 = txtExam742.Text;
                this.feTusBorn.BORN_METHOD = txtBornMethod.Text;
                this.feTusBorn.MIDWIFE_NAME = txtMidWifeName.Text;
                this.feTusBorn.CHILD_STATUS = txtChildStatus.Text;
                this.feTusBorn.BORN_PLACE = txtBornPlace.Text;

                if (chkIsBreastFeedingFirstHour.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_BREASTFEEDING_FIRST_HOUR = 1;
                }
                else
                {
                    this.feTusBorn.IS_BREASTFEEDING_FIRST_HOUR = null;
                }
                if (chkIsK1.CheckState == CheckState.Checked)
                {
                    this.feTusBorn.IS_K1 = 1;
                }
                else
                {
                    this.feTusBorn.IS_K1 = null;
                }

                this.feTusBorn.BRANCH_CODE = WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.currentModule.RoomId).BranchCode;

                TYT_FETUS_BORN _result = new TYT_FETUS_BORN();
                if (this.action == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd)
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_BORN>("api/TytFetusBorn/Create", ApiConsumers.TytConsumer, this.feTusBorn, param);
                }
                else
                {
                    _result = new BackendAdapter(param).Post<TYT_FETUS_BORN>("api/TytFetusBorn/Update", ApiConsumers.TytConsumer, this.feTusBorn, param);
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

        private void spinTrongLuong_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaPrematurelyCount.Focus();
                    spinParaPrematurelyCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaPrematurelyCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaMiscarriageCount.Focus();
                    spinParaMiscarriageCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaMiscarriageCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaChildCount.Focus();
                    spinParaChildCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinParaChildCount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHIVBefore.Focus();
                    chkHIVBefore.Focus();
                    chkHIVBefore.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsFetusManage_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsUVFull.Focus();
                    chkIsUVFull.SelectAll();
                    chkIsUVFull.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsUVFull_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinParaNormalCount.Focus();
                    spinParaNormalCount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHIVBefore_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkHIVBorn.Focus();
                    chkHIVBorn.Focus();
                    chkHIVBorn.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHIVBorn_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCase33.Focus();
                    chkCheckCase33.Focus();
                    chkCheckCase33.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCase33_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCase43.Focus();
                    chkCheckCase43.SelectAll();
                    chkCheckCase43.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCase43_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkCheckCaseHaveNotYet.Focus();
                    chkCheckCaseHaveNotYet.Focus();
                    chkCheckCaseHaveNotYet.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckCaseHaveNotYet_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtFirstWeekExam.Focus();
                    txtFirstWeekExam.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFirstWeekExam_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExam742.Focus();
                    txtExam742.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExam742_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsDeath.Focus();
                    chkIsDeath.SelectAll();
                    chkIsDeath.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsDeath_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtObstetricComplication.Focus();
                    txtObstetricComplication.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtObstetricComplication_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoFemale.Focus();
                    rdoFemale.SelectAll();
                    rdoFemale.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoFemale_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    rdoMale.Focus();
                    rdoMale.SelectAll();
                    rdoMale.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void rdoMale_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinChildWeight.Focus();
                    spinChildWeight.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinChildWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtNote.Focus();
                    txtNote.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBornMethod_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtMidWifeName.Focus();
                    txtMidWifeName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMidWifeName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBornPlace.Focus();
                    txtBornPlace.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtChildStatus_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsBreastFeedingFirstHour.Focus();
                    chkIsBreastFeedingFirstHour.SelectAll();
                    chkIsBreastFeedingFirstHour.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBornPlace_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtChildStatus.Focus();
                    txtChildStatus.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsBreastFeedingFirstHour_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsK1.Focus();
                    chkIsK1.SelectAll();
                    chkIsK1.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsK1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void chkIsChildDeath_KeyDown(object sender, KeyEventArgs e)
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

        private void chkIsK1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    chkIsChildDeath.Focus();
                    chkIsChildDeath.SelectAll();
                    chkIsChildDeath.BackColor = Color.Gray;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkIsFetusManage_Leave(object sender, EventArgs e)
        {
            chkIsFetusManage.BackColor = Color.Transparent;
        }

        private void chkIsUVFull_Leave(object sender, EventArgs e)
        {
            chkIsUVFull.BackColor = Color.Transparent;
        }

        private void chkHIVBefore_Leave(object sender, EventArgs e)
        {
            chkHIVBefore.BackColor = Color.Transparent;
        }

        private void chkHIVBorn_Leave(object sender, EventArgs e)
        {
            chkHIVBorn.BackColor = Color.Transparent;
        }

        private void chkCheckCase33_Leave(object sender, EventArgs e)
        {
            try
            {
                chkCheckCase33.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            
        }

        private void chkCheckCase43_Leave(object sender, EventArgs e)
        {
            try
            {
                chkCheckCase43.BackColor = Color.Transparent;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            
        }

        private void chkCheckCaseHaveNotYet_Leave(object sender, EventArgs e)
        {
            chkCheckCaseHaveNotYet.BackColor = Color.Transparent;
        }

        private void chkIsDeath_Leave(object sender, EventArgs e)
        {
            chkIsDeath.BackColor = Color.Transparent;
        }

        private void rdoFemale_Leave(object sender, EventArgs e)
        {
            rdoFemale.BackColor = Color.Transparent;
        }

        private void rdoMale_Leave(object sender, EventArgs e)
        {
            rdoMale.BackColor = Color.Transparent;
        }

        private void chkIsBreastFeedingFirstHour_Leave(object sender, EventArgs e)
        {
            chkIsBreastFeedingFirstHour.BackColor = Color.Transparent;
        }

        private void chkIsK1_Leave(object sender, EventArgs e)
        {
            chkIsK1.BackColor = Color.Transparent;
        }

        private void chkIsChildDeath_Leave(object sender, EventArgs e)
        {
            chkIsChildDeath.BackColor = Color.Transparent;
        }

        private void chkCheckCase33_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCase33.CheckState == CheckState.Checked)
                {
                    chkCheckCase43.CheckState = CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckCase43_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCase43.CheckState == CheckState.Checked)
                {
                    chkCheckCase33.CheckState = CheckState.Unchecked;
                    chkCheckCaseHaveNotYet.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckCaseHaveNotYet_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckCaseHaveNotYet.CheckState == CheckState.Checked)
                {
                    chkCheckCase33.CheckState = CheckState.Unchecked;
                    chkCheckCase43.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoFemale_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoFemale.CheckState == CheckState.Checked)
                {
                    rdoMale.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void rdoMale_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (rdoMale.CheckState == CheckState.Checked)
                {
                    rdoFemale.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
