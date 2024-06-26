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
using HIS.Desktop.Plugins.UpdatePatientExt.Ado;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.UpdatePatientExt
{
    public partial class frmUpdatePatientExt : Form
    {
        private Inventec.Desktop.Common.Modules.Module module;
        private PatientADO patientADO { get; set; }
        private HIS_PATIENT currentPatient { get; set; }
        private long patientId;
        private long? fatherId;
        private long? motherId;
        public frmUpdatePatientExt()
        {
            InitializeComponent();
        }

        public frmUpdatePatientExt(Inventec.Desktop.Common.Modules.Module _module, long _patientId)
        {
            InitializeComponent();
            try
            {
                this.patientId = _patientId;
                this.module = _module;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmUpdatePatientExt_Load(object sender, EventArgs e)
        {
            try
            {
                //Load thông tin bệnh nhân
                LoadPatient();
                LoadDataToControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        private void txtMotherCode_Leave(object sender, EventArgs e)
        {
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!CheckPregnancyVali() || !this.CheckBornVali())
                {
                    return;
                }

                HIS_PATIENT patient = SetPatientFromControl();
                if (patient == null)
                    throw new Exception("patient is null");

                CommonParam param = new CommonParam();
                bool success = false;
                WaitingManager.Show();
                var patientUpdate = new BackendAdapter(param)
                    .Post<HIS_PATIENT>("api/HisPatient/Update", ApiConsumers.MosConsumer, patient, param);
                if (patientUpdate != null)
                {
                    success = true;
                }
                WaitingManager.Hide();

                #region Show message
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFatherCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtFatherCode.Text))
                    {
                        string code = txtFatherCode.Text.Trim();
                        if (code.Length < 10 && checkDigit(code))
                        {
                            code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                            txtFatherCode.Text = code;
                        }

                        CommonParam param = new CommonParam();
                        HisPatientFilter filter = new HisPatientFilter();
                        filter.PATIENT_CODE = txtFatherCode.Text.Trim();
                        HIS_PATIENT patient = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                        if (patient != null)
                        {
                            if (patient.GENDER_ID == 2)
                            {
                                fatherId = patient.ID;
                                lblFatherName.Text = patient.VIR_PATIENT_NAME;
                                lblFatherDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                            }
                            else
                            {
                                MessageBox.Show("Quan hệ cha . Giới tính phải là Nam");
                                fatherId = 0;
                                lblFatherName.Text = "";
                                lblFatherDob.Text = "";
                                txtFatherCode.Text = "";
                                txtFatherCode.Focus();
                                return;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy thông tin");
                            fatherId = 0;
                            lblFatherName.Text = "";
                            lblFatherDob.Text = "";
                            txtFatherCode.Text = "";
                            txtFatherCode.Focus();
                            return;
                        }
                    }

                    txtMotherCode.Focus();
                    txtMotherCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtMotherCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!String.IsNullOrEmpty(txtMotherCode.Text))
                {
                    string code = txtMotherCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtMotherCode.Text = code;
                    }

                    CommonParam param = new CommonParam();
                    HisPatientFilter filter = new HisPatientFilter();
                    filter.PATIENT_CODE = txtMotherCode.Text.Trim();
                    HIS_PATIENT patient = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                    if (patient != null)
                    {

                        if (patient.GENDER_ID == 1)
                        {
                            motherId = patient.ID;
                            lblMotherName.Text = patient.VIR_PATIENT_NAME;
                            lblMotherDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(patient.DOB);
                        }
                        else
                        {
                            MessageBox.Show("Quan hệ mẹ . Giới tính phải là Nữ");
                            motherId = 0;
                            lblMotherName.Text = "";
                            lblMotherDob.Text = "";
                            txtMotherCode.Text = "";
                            txtMotherCode.Focus();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin");
                        motherId = 0;
                        lblMotherName.Text = "";
                        lblMotherDob.Text = "";
                        txtMotherCode.Text = "";
                        txtMotherCode.Focus();
                        return;
                    }
                }

                spinBornWeight.Focus();
                spinBornWeight.SelectAll();
            }
        }

        private void chkIS_BORN_INADEQUACY_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIS_BORN_SUFFOCATE.Focus();
            }
        }

        private void spinBornHeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                chkIS_BORN_INADEQUACY.Focus();
            }
        }

        private void chkIS_BORN_SUFFOCATE_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtPregnancyLastTime.Focus();
                    dtPregnancyLastTime.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtPregnancyLastTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPREGNANCY_COUNT.Focus();
                    spinPREGNANCY_COUNT.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPREGNANCY_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPREGNANCY_MISCARRIAGE_COUNT.Focus();
                    spinPREGNANCY_MISCARRIAGE_COUNT.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPREGNANCY_MISCARRIAGE_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinPREGNANCY_ABORTION_COUNT.Focus();
                    spinPREGNANCY_ABORTION_COUNT.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinPREGNANCY_ABORTION_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_COUNT.Focus();
                    spinBORN_COUNT.SelectAll();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_NORMAL_COUNT.Focus();
                    spinBORN_NORMAL_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_NORMAL_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_CAESAREAN_SECTION_COUNT.Focus();
                    spinBORN_CAESAREAN_SECTION_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_CAESAREAN_SECTION_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_HARD_COUNT.Focus();
                    spinBORN_HARD_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_HARD_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_INADEQUACY_COUNT.Focus();
                    spinBORN_INADEQUACY_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_INADEQUACY_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBORN_LIVING_COUNT.Focus();
                    spinBORN_LIVING_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBORN_LIVING_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinUV_VACCINATION_MOTHER_COUNT.Focus();
                    spinUV_VACCINATION_MOTHER_COUNT.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinUV_VACCINATION_MOTHER_COUNT_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    mmBornMalformation.Focus();
                    mmBornMalformation.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinBornWeight_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinBornHeight.Focus();
                    spinBornHeight.SelectAll();
                }
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
    }
}
