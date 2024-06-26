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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.EpidemiologyInfo.Validtion;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.EpidemiologyInfo
{
    public partial class frmEpidemiologyInfo : HIS.Desktop.Utility.FormBase
    {
        internal Inventec.Desktop.Common.Modules.Module Module;
        internal HIS_TREATMENT currentTreatment;
        internal long treatmentId = 0;

        public frmEpidemiologyInfo()
        {
            InitializeComponent();
        }

        public frmEpidemiologyInfo(Inventec.Desktop.Common.Modules.Module moduleData, HIS_TREATMENT _treatment)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.Module = moduleData;
                this.currentTreatment = _treatment;
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("_treatment", _treatment));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void frmEpidemiologyInfo_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboMedicineType();
                ValidateForm();
                SetDefaultValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMedicineType()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMedicineTypeFilter filter = new HisMedicineTypeFilter();
                filter.IS_VACCINE = true;
                var data = new BackendAdapter(param).Get<List<HIS_MEDICINE_TYPE>>("api/HisMedicineType/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICINE_TYPE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICINE_TYPE_NAME", "ID", columnInfos, false, 350);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboMedicineType, data, controlEditorADO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                if (this.currentTreatment != null)
                {
                    txtCovidPatientCode.Text = currentTreatment.COVID_PATIENT_CODE ?? "";
                    if (currentTreatment.VACCINE_ID != null)
                    {
                        cboMedicineType.EditValue = currentTreatment.VACCINE_ID;
                    }
                    if (this.currentTreatment.VACINATION_ORDER != null)
                    {
                        spnVacinationOrder.EditValue = currentTreatment.VACINATION_ORDER;
                    }
                    else
                    {
                        spnVacinationOrder.EditValue = null;
                    }
                    if (this.currentTreatment.EPIDEMILOGY_CONTACT_TYPE != null)
                    {
                        spnEpidemiologyContactType.EditValue = currentTreatment.EPIDEMILOGY_CONTACT_TYPE;
                    }
                    else
                    {
                        spnEpidemiologyContactType.EditValue = null;
                    }
                    if (!string.IsNullOrEmpty(this.currentTreatment.EPIDEMILOGY_SYMPTOM))
                    {
                        txtEpidemiologySymptom.Text = currentTreatment.EPIDEMILOGY_SYMPTOM;
                    }
                    else
                    {
                        txtEpidemiologySymptom.Text = "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!dxValidationProviderEditorInfo.Validate())
                    return;
                if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtEpidemiologySymptom.Text, 4000))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Triệu chứng phải nhỏ hơn 4000 ký tự.", "Thông báo");
                    return;
                }
                WaitingManager.Show();
                EpidemiologyInfoSDO updateDTO = new EpidemiologyInfoSDO();
                UpdateDTOFromDataForm(ref updateDTO);

                var resultData = new BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/UpdateEpidemiologyInfo", ApiConsumers.MosConsumer, updateDTO, param);
                if (resultData != null)
                {
                    success = true;
                    this.Close();
                }


                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDTOFromDataForm(ref EpidemiologyInfoSDO updateDTO)
        {
            try
            {
                updateDTO.TreatmentId = currentTreatment.ID;
                updateDTO.CovidPatientCode = txtCovidPatientCode.Text.Trim();
                if (cboMedicineType.EditValue != null)
                {
                    updateDTO.VaccineId = Convert.ToInt64(cboMedicineType.EditValue);
                }
                else
                {
                    updateDTO.VaccineId = null;
                }
                if (spnVacinationOrder.EditValue != null)
                {
                    updateDTO.VaccinationOrder = Convert.ToInt64(spnVacinationOrder.Value);
                }
                else
                {
                    updateDTO.VaccinationOrder = null;
                }
                if (spnEpidemiologyContactType.EditValue != null)
                {
                    updateDTO.EpidemiologyContactType = Convert.ToInt64(spnEpidemiologyContactType.Value);
                }
                else
                {
                    updateDTO.EpidemiologyContactType = null;
                }
                updateDTO.EpidemiologySympton = txtEpidemiologySymptom.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validate
        private void ValidateForm()
        {
            try
            {
                ValidMaxlengthTextBox(txtCovidPatientCode, 20);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTextBox(TextEdit txtEdit, int? maxLength, bool isRequired = false)
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtEdit;
                validateMaxLength.isRequired = isRequired;
                validateMaxLength.maxLength = maxLength;
                dxValidationProviderEditorInfo.SetValidationRule(txtEdit, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void cboMedicineType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnVacinationOrder.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnVacinationOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spnEpidemiologyContactType.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnEpidemiologyContactType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtEpidemiologySymptom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtCovidPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboMedicineType.Focus();
                    cboMedicineType.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
