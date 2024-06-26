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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ExpMestSaleEdit.ADO;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ExpMestSaleEdit
{
    public partial class UCExpMestSaleEdit
    {
        #region Common Control Event
        private void cboPatientType_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    patientType = null;
                    if (!String.IsNullOrEmpty(cboPatientType.Text))
                    {
                        string key = cboPatientType.Text.ToLower();
                        var listData = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.PATIENT_TYPE_CODE.ToLower().Contains(key) || o.PATIENT_TYPE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            cboPatientType.EditValue = listData.First().ID;
                            patientType = listData.First();
                            txtPrescriptionCode.Focus();
                            txtPrescriptionCode.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboPatientType.Focus();
                        cboPatientType.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    patientType = null;
                    if (cboPatientType.EditValue != null)
                    {
                        patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPatientType.EditValue));
                    }
                    txtPrescriptionCode.Focus();
                    txtPrescriptionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsVisitor_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (checkIsVisitor.Checked)
                {
                    txtPrescriptionCode.Enabled = false;
                    txtPrescriptionCode.Text = "";
                    txtVirPatientName.Enabled = true;
                    txtVirPatientName.Focus();
                    txtVirPatientName.SelectAll();
                }
                else
                {
                    txtPrescriptionCode.Enabled = true;
                    txtPrescriptionCode.Focus();
                    txtPrescriptionCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrescriptionCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (Char.IsDigit(e.KeyChar) || Char.IsControl(e.KeyChar))
                {
                }
                else
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPrescriptionCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    prescription = null;
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtPrescriptionCode.Text.Trim()))
                    {
                        WaitingManager.Show();
                        CommonParam param = new CommonParam();
                        bool success = true;
                        string code = String.Format("{0:000000000000}", Convert.ToInt64(txtPrescriptionCode.Text.Trim()));
                        HisPrescriptionViewFilter presFilter = new HisPrescriptionViewFilter();
                        presFilter.EXP_MEST_CODE__EXACT = code;
                        var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PRESCRIPTION>>(HisRequestUriStore.HIS_PRESCRIPTION_GETVIEW, ApiConsumers.MosConsumer, presFilter, null);
                        if (listData != null && listData.Count != 1)
                        {
                            success = false;
                            param.Messages.Add(Base.ResourceMessageLang.KhongTimDuocTheoMaDonThuoc);
                            WaitingManager.Hide();
                            MessageManager.Show(param, success);
                            txtVirPatientName.Enabled = true;
                            txtVirPatientName.Focus();
                            txtVirPatientName.SelectAll();
                            return;
                        }
                        valid = true;
                        prescription = listData.First();
                        txtPrescriptionCode.Text = prescription.EXP_MEST_CODE;
                        FillDataToGridDetailByTemplate(listData.First());
                        WaitingManager.Hide();
                    }
                    if (valid)
                    {
                        txtVirPatientName.Enabled = false;
                        txtSampleForm.Focus();
                        txtSampleForm.SelectAll();
                    }
                    else
                    {
                        txtVirPatientName.Enabled = true;
                        txtVirPatientName.Focus();
                        txtVirPatientName.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtVirPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtSampleForm.Focus();
                    txtSampleForm.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSampleForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtSampleForm.Text))
                    {
                        string key = txtSampleForm.Text.Trim().ToLower();
                        var listData = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().Where(o => o.EXP_MEST_TEMPLATE_CODE.ToLower().Contains(key) || o.EXP_MEST_TEMPLATE_NAME.ToLower().Contains(key)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtSampleForm.Text = listData.First().EXP_MEST_TEMPLATE_CODE;
                            cboSampleForm.EditValue = listData.First().ID;
                            FillDataToGridDetailByTemplate(listData.First());
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboSampleForm.Focus();
                        cboSampleForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampleForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboSampleForm.EditValue != null)
                    {
                        var template = BackendDataWorker.Get<HIS_EXP_MEST_TEMPLATE>().First(o => o.ID == Convert.ToInt64(cboSampleForm.EditValue));
                        if (template != null)
                        {
                            txtSampleForm.Text = template.EXP_MEST_TEMPLATE_CODE;
                            FillDataToGridDetailByTemplate(template);
                        }
                    }
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SetFocusTreeMediOrMate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Detail Control Event
        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (checkImpExpPrice.Checked)
                    {
                        spinExpPrice.Focus();
                        spinExpPrice.SelectAll();
                    }
                    else
                    {
                        txtTutorial.Focus();
                        txtTutorial.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkImpExpPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                SetEnableControlPriceByCheckBox();
                if (spinExpPrice.Enabled)
                {
                    spinExpPrice.Focus();
                    spinExpPrice.SelectAll();
                }
                else
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExpVatRatio.Focus();
                    spinExpVatRatio.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExpVatRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinDiscount.Focus();
                    spinDiscount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtTutorial.Focus();
                    txtTutorial.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTutorial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtNote_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void FillDataToGridDetailByTemplate(HIS_EXP_MEST_TEMPLATE data)
        {
            try
            {
                if (data != null)
                {
                    dicMediMateAdo.Clear();
                    HisEmteMedicineTypeViewFilter medicineFilter = new HisEmteMedicineTypeViewFilter();
                    medicineFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var listMedicineTem = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MEDICINE_TYPE>>(HisRequestUriStore.HIS_EMTE_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, null);

                    HisEmteMaterialTypeViewFilter materialFilter = new HisEmteMaterialTypeViewFilter();
                    materialFilter.EXP_MEST_TEMPLATE_ID = data.ID;
                    var listMaterialTem = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EMTE_MATERIAL_TYPE>>(HisRequestUriStore.HIS_EMTE_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);

                    if (listMedicineTem != null && listMedicineTem.Count > 0)
                    {
                        foreach (var item in listMedicineTem)
                        {
                            if (dicMedicineTypeStock.ContainsKey(item.MEDICINE_TYPE_ID))
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(dicMedicineTypeStock[item.MEDICINE_TYPE_ID]);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.TUTORIAL = item.TUTORIAL;
                                ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                                ado.ExpMedicine.Tutorial = ado.TUTORIAL;
                                if (this.patientType != null)
                                {
                                    ado.ExpMedicine.PatientTypeId = this.patientType.ID;
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.MEDI_MATE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.IsMedicine = true;
                                ado.IsNotHasMest = true;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }

                    if (listMaterialTem != null && listMaterialTem.Count > 0)
                    {
                        foreach (var item in listMaterialTem)
                        {
                            if (dicMaterialTypeStock.ContainsKey(item.MATERIAL_TYPE_ID))
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(dicMaterialTypeStock[item.MATERIAL_TYPE_ID]);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                                if (ado.EXP_AMOUNT > ado.AVAILABLE_AMOUNT)
                                {
                                    ado.IsGreatAvailable = true;
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.MEDI_MATE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MATERIAL_TYPE_ID;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.IsMedicine = false;
                                ado.IsNotHasMest = true;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }

                    FillDataGridExpMestDetail();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridDetailByTemplate(V_HIS_PRESCRIPTION data)
        {
            try
            {
                if (data != null)
                {
                    txtVirPatientName.Text = data.VIR_PATIENT_NAME;
                    dicMediMateAdo.Clear();
                    HisExpMestMetyViewFilter medicineFilter = new HisExpMestMetyViewFilter();
                    medicineFilter.EXP_MEST_ID = data.EXP_MEST_ID;
                    var listExpMedicine = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_METY>>(HisRequestUriStore.HIS_EXP_MEST_METY_GETVIEW, ApiConsumers.MosConsumer, medicineFilter, null);

                    HisExpMestMatyViewFilter materialFilter = new HisExpMestMatyViewFilter();
                    materialFilter.EXP_MEST_ID = data.EXP_MEST_ID;
                    var listExpMaterial = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATY>>(HisRequestUriStore.HIS_EXP_MEST_MATY_GETVIEW, ApiConsumers.MosConsumer, materialFilter, null);

                    if (listExpMedicine != null && listExpMedicine.Count > 0)
                    {
                        foreach (var item in listExpMedicine)
                        {
                            if (dicMedicineTypeStock.ContainsKey(item.MEDICINE_TYPE_ID))
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(dicMedicineTypeStock[item.MEDICINE_TYPE_ID]);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.TUTORIAL = item.TUTORIAL;
                                ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                                ado.ExpMedicine.Tutorial = ado.TUTORIAL;
                                if (ado.EXP_AMOUNT > ado.AVAILABLE_AMOUNT)
                                {
                                    ado.IsGreatAvailable = true;
                                }
                                if (this.patientType != null)
                                {
                                    ado.ExpMedicine.PatientTypeId = this.patientType.ID;
                                    this.SetExpPriceByPaty(ref ado);
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.MEDI_MATE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MEDICINE_TYPE_ID;
                                ado.IsMedicine = true;
                                ado.IsNotHasMest = true;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }

                    if (listExpMaterial != null && listExpMaterial.Count > 0)
                    {
                        foreach (var item in listExpMaterial)
                        {
                            if (dicMaterialTypeStock.ContainsKey(item.MATERIAL_TYPE_ID))
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(dicMaterialTypeStock[item.MATERIAL_TYPE_ID]);
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                                if (ado.EXP_AMOUNT > ado.AVAILABLE_AMOUNT)
                                {
                                    ado.IsGreatAvailable = true;
                                }
                                if (this.patientType != null)
                                {
                                    this.SetExpPriceByPaty(ref ado);
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO();
                                ado.SERVICE_ID = item.SERVICE_ID;
                                ado.EXP_AMOUNT = item.AMOUNT;
                                ado.MEDI_MATE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                                ado.MEDI_MATE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                                ado.MEDI_MATE_TYPE_ID = item.MATERIAL_TYPE_ID;
                                ado.IsMedicine = false;
                                ado.IsNotHasMest = true;
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }

                    FillDataGridExpMestDetail();
                    this.SetTotalPrice();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetExpPriceByPaty(ref MediMateTypeADO ado)
        {
            try
            {
                var listServicePaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_PATY>>(HisRequestUriStore.HIS_SERVICE_PATY_GET_APPLIED_VIEW, ApiConsumers.MosConsumer, null, null, "serviceId", ado.SERVICE_ID, "treatmentTime", null);
                if (listServicePaty != null)
                {
                    var paty = listServicePaty.FirstOrDefault(o => o.PATIENT_TYPE_ID == this.patientType.ID);
                    if (paty != null)
                    {
                        ado.ADVISORY_PRICE = paty.PRICE;
                        ado.ADVISORY_TOTAL_PRICE = (paty.PRICE * ado.EXP_AMOUNT * (1 + (ado.EXP_VAT_RATIO ?? 0) / 100));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
