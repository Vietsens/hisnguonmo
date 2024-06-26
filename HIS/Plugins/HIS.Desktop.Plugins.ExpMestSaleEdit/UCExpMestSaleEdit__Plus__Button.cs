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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Plugins.ExpMestSaleEdit.ADO;
using HIS.Desktop.Print;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
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
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;
                WaitingManager.Show();
                if (spinAmount.Value > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    WaitingManager.Hide();
                    MessageManager.Show(Base.ResourceMessageLang.XuLyThatBai + Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                    return;
                }
                this.currentMediMate.EXP_AMOUNT = spinAmount.Value;
                this.currentMediMate.NOTE = txtNote.Text;
                this.currentMediMate.TUTORIAL = txtTutorial.Text;

                if (checkImpExpPrice.Checked)
                {
                    this.currentMediMate.EXP_PRICE = spinExpPrice.Value;
                    this.currentMediMate.EXP_VAT_RATIO = spinExpVatRatio.Value / 100;
                    this.currentMediMate.DISCOUNT = spinDiscount.Value;
                    if (this.currentMediMate.EXP_PRICE > 0 && this.currentMediMate.EXP_AMOUNT > 0)
                    {
                        this.currentMediMate.DISCOUNT_RATIO = this.currentMediMate.DISCOUNT.Value / (this.currentMediMate.EXP_PRICE.Value * this.currentMediMate.EXP_AMOUNT);
                    }
                }
                else if (this.patientType != null)
                {
                    this.currentMediMate.PATIENT_TYPE_ID = this.patientType.ID;
                }

                this.currentMediMate.ADVISORY_PRICE = spinExpPrice.Value;
                this.currentMediMate.ADVISORY_TOTAL_PRICE = (spinExpPrice.Value * spinAmount.Value * (1 + spinExpVatRatio.Value / 100));

                if (this.currentMediMate.IsMedicine)
                {
                    this.currentMediMate.ExpMedicine.Amount = this.currentMediMate.EXP_AMOUNT;
                    this.currentMediMate.ExpMedicine.Description = this.currentMediMate.NOTE;
                    this.currentMediMate.ExpMedicine.DiscountRatio = this.currentMediMate.DISCOUNT_RATIO;
                    this.currentMediMate.ExpMedicine.Price = this.currentMediMate.EXP_PRICE;
                    this.currentMediMate.ExpMedicine.VatRatio = this.currentMediMate.EXP_VAT_RATIO;
                    this.currentMediMate.ExpMedicine.Tutorial = this.currentMediMate.TUTORIAL;
                    this.currentMediMate.ExpMedicine.PatientTypeId = this.currentMediMate.PATIENT_TYPE_ID;
                }
                else
                {
                    this.currentMediMate.ExpMaterial.Amount = this.currentMediMate.EXP_AMOUNT;
                    this.currentMediMate.ExpMaterial.Description = this.currentMediMate.NOTE;
                    this.currentMediMate.ExpMaterial.DiscountRatio = this.currentMediMate.DISCOUNT_RATIO;
                    this.currentMediMate.ExpMaterial.Price = this.currentMediMate.EXP_PRICE;
                    this.currentMediMate.ExpMaterial.VatRatio = this.currentMediMate.EXP_VAT_RATIO;
                }

                if (dicMediMateAdo.ContainsKey(this.currentMediMate.SERVICE_ID))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ThuocVatTuDaCoTrongDanhSachXuat_BanCoMuonThayThe, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        ResetValueControlDetail();
                        SetFocusTreeMediOrMate();
                        WaitingManager.Hide();
                        return;
                    }
                }

                dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;
                FillDataGridExpMestDetail();
                this.SetTotalPrice();
                ResetValueControlDetail();
                SetFocusTreeMediOrMate();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0 || this.mediStock == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref success, ref param);
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSavePrint.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0 || this.mediStock == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref success, ref param);
                WaitingManager.Hide();
                if (success)
                {
                    onClickInPhieuXuatBan(null, null);
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //if (!btnNew.Enabled)
                //    return;
                WaitingManager.Show();
                ResetValueControlCommon();
                ResetValueControlDetail();
                FillDataGridExpMestDetail();
                FillDataToTreeMediMate();
                FillDataToGridExpMest();
                SetFocusTreeMediOrMate();
                txtPrescriptionCode.Focus();
                txtPrescriptionCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewExpMestDetail.GetFocusedRow();
                if (data != null)
                {
                    dicMediMateAdo.Remove(data.SERVICE_ID);
                }
                FillDataGridExpMestDetail();
                this.SetTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessSave(ref bool success, ref CommonParam param)
        {
            try
            {
                HisSaleExpMestSDO data = new HisSaleExpMestSDO();
                if (this.resultSdo != null)//isUpdate
                {
                    data.SaleExpMest = this.resultSdo.SaleExpMest;
                    data.ExpMest = this.resultSdo.ExpMest;
                }
                else
                {
                    data.ExpMest = new HIS_EXP_MEST();
                    data.SaleExpMest = new HIS_SALE_EXP_MEST();
                }

                data.ExpMest.EXP_MEST_STT_ID = HisExpMestSttCFG.HisExpMestSttId__Request;
                data.ExpMest.EXP_MEST_TYPE_ID = HisExpMestTypeCFG.HisExpMestTypeId__Sale;
                data.ExpMest.MEDI_STOCK_ID = this.mediStock.ID;
                data.ExpMest.DESCRIPTION = txtDescription.Text;
                if (this.patientType != null)
                {
                    data.PatientTypeId = this.patientType.ID;
                }
                if (cboPatientType.EditValue != null)
                {
                    data.PatientTypeId = (long)cboPatientType.EditValue;
                }
                if (this.prescription != null)
                {
                    data.SaleExpMest.PATIENT_ID = this.prescription.TDL_PATIENT_ID;
                    data.SaleExpMest.SOURCE_EXP_MEST_ID = this.prescription.EXP_MEST_ID;
                    data.SaleExpMest.PATIENT_CODE = this.prescription.PATIENT_CODE;
                    data.SaleExpMest.TREATMENT_CODE = this.prescription.TREATMENT_CODE;
                }
                data.SaleExpMest.CLIENT_NAME = txtVirPatientName.Text;
                data.SaleExpMest.CLIENT_ADDRESS = txtAddress.Text;
                if (dtDob.EditValue == null)
                {
                    data.SaleExpMest.CLIENT_DOB = null;
                }
                else
                {
                    data.SaleExpMest.CLIENT_DOB = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtDob.DateTime);
                }
                if (cboGender.EditValue == null)
                {
                    data.SaleExpMest.CLIENT_GENDER_ID = null;
                }
                else
                {
                    data.SaleExpMest.CLIENT_GENDER_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboGender.EditValue.ToString());
                }

                data.ExpMaterials = new List<HisMaterialTypeAmountWithPriceSDO>();
                data.ExpMedicines = new List<HisMedicineTypeAmountWithPriceSDO>();
                foreach (var item in dicMediMateAdo)
                {
                    if (item.Value.IsNotHasMest)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.TonTaiThuocVatTuKhongCoTrongKho);
                        success = false;
                        return;
                    }
                    if (item.Value.EXP_AMOUNT <= 0)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatPhaiLonHongKhong);
                        success = false;
                        return;
                    }
                    if (item.Value.AVAILABLE_AMOUNT + item.Value.EXP_AMOUNT < item.Value.EXP_AMOUNT)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                        success = false;
                        return;
                    }

                    if (item.Value.IsMedicine)
                    {
                        if (item.Value.AVAILABLE_AMOUNT + item.Value.ExpMedicine.Amount < item.Value.EXP_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                            success = false;
                            return;
                        }
                        item.Value.ExpMedicine.Amount = item.Value.EXP_AMOUNT;
                        item.Value.ExpMedicine.Price = item.Value.ADVISORY_PRICE;
                        data.ExpMedicines.Add(item.Value.ExpMedicine);
                    }
                    else
                    {
                        if (item.Value.AVAILABLE_AMOUNT + item.Value.ExpMaterial.Amount < item.Value.EXP_AMOUNT)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                            success = false;
                            return;
                        }
                        item.Value.ExpMaterial.Price = item.Value.ADVISORY_PRICE;
                        item.Value.ExpMaterial.Amount = item.Value.EXP_AMOUNT;
                        data.ExpMaterials.Add(item.Value.ExpMaterial);
                    }
                }

                if (this.resultSdo != null)//isUpdate && 
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisSaleExpMestResultSDO>(HisRequestUriStore.HIS_SALE_EXP_MEST_UPDATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
                else
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisSaleExpMestResultSDO>(HisRequestUriStore.HIS_SALE_EXP_MEST_CREATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
                if (success)
                {
                    ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    SetTotalPrice();
                    ddBtnPrint.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void ProcessFillDataBySuccess()
        {
            try
            {
                if (this.resultSdo != null)
                {
                    //var listAdo = dicMediMateAdo.Select(s => s.Value).ToList();
                    if (this.resultSdo.ExpMedicines != null && this.resultSdo.ExpMedicines.Count > 0)
                    {
                        if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Approved || this.resultSdo.ExpMest.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Exported)
                        {
                            this.resultSdo.ExpMedicines = this.resultSdo.ExpMedicines.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                        }
                        else
                        {
                            this.resultSdo.ExpMedicines = this.resultSdo.ExpMedicines.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                        }
                        var Group = this.resultSdo.ExpMedicines.GroupBy(o => o.MEDICINE_TYPE_ID).ToList();
                        foreach (var group in Group)
                        {
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                            if (dicMediMateAdo.ContainsKey(listByGroup.First().SERVICE_ID))
                            {
                                var medicine = dicMediMateAdo[listByGroup.First().SERVICE_ID];
                                var expMestMedi = listByGroup.First();
                                medicine.ExpMedicine.Amount = listByGroup.Sum(s => s.AMOUNT);
                                medicineAmount = listByGroup.Sum(s => s.AMOUNT);
                                medicine.ExpMedicine.Description = expMestMedi.DESCRIPTION;
                                medicine.ExpMedicine.MedicineTypeId = expMestMedi.MEDICINE_TYPE_ID;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(listByGroup);
                                if (dicMedicineTypeStock.ContainsKey(ado.MEDI_MATE_TYPE_ID))
                                {
                                    ado.AVAILABLE_AMOUNT = dicMedicineTypeStock[ado.MEDI_MATE_TYPE_ID].AvailableAmount;
                                }
                                else
                                {
                                    ado.AVAILABLE_AMOUNT = 0;
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }
                    if (this.resultSdo.ExpMaterials != null && this.resultSdo.ExpMaterials.Count > 0)
                    {
                        if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Approved || this.resultSdo.ExpMest.EXP_MEST_STT_ID == HisExpMestSttCFG.HisExpMestSttId__Exported)
                        {
                            this.resultSdo.ExpMaterials = this.resultSdo.ExpMaterials.Where(o => o.IN_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_EXECUTE__TRUE).ToList();
                        }
                        else
                        {
                            this.resultSdo.ExpMaterials = this.resultSdo.ExpMaterials.Where(o => o.IN_REQUEST == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_MEDICINE.IN_REQUEST__TRUE).ToList();
                        }
                        var Group = this.resultSdo.ExpMaterials.GroupBy(o => o.MATERIAL_TYPE_ID).ToList();
                        foreach (var group in Group)
                        {
                            var listByGroup = group.ToList<V_HIS_EXP_MEST_MATERIAL>();
                            if (dicMediMateAdo.ContainsKey(listByGroup.First().SERVICE_ID))
                            {
                                var material = dicMediMateAdo[listByGroup.First().SERVICE_ID];
                                material.ExpMaterial.Amount = listByGroup.Sum(s => s.AMOUNT);
                                materialAmount = listByGroup.Sum(s => s.AMOUNT);
                                material.ExpMaterial.Description = listByGroup.First().DESCRIPTION;
                                material.ExpMaterial.MaterialTypeId = listByGroup.First().MATERIAL_TYPE_ID;
                            }
                            else
                            {
                                MediMateTypeADO ado = new MediMateTypeADO(listByGroup);
                                if (dicMaterialTypeStock.ContainsKey(ado.MEDI_MATE_TYPE_ID))
                                {
                                    ado.AVAILABLE_AMOUNT = dicMaterialTypeStock[ado.MEDI_MATE_TYPE_ID].AvailableAmount;
                                }
                                else
                                {
                                    ado.AVAILABLE_AMOUNT = 0;
                                }
                                dicMediMateAdo[ado.SERVICE_ID] = ado;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInPhieuXuatBan(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickInHuongDanSuDung(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null || this.resultSdo.ExpMedicines == null || this.resultSdo.ExpMedicines.Count == 0)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099, deletePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool deletePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode) && !String.IsNullOrEmpty(fileName))
                {
                    switch (printTypeCode)
                    {
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatBan_MPS000092:
                            InPhieuXuatBan(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__HuongDanSuDungThuoc_MPS000099:
                            InHuongDanSuDungThuoc(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void InPhieuXuatBan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                WaitingManager.Show();
                HisSaleExpMestViewFilter saleFilter = new HisSaleExpMestViewFilter();
                saleFilter.ID = this.resultSdo.SaleExpMest.ID;
                var listSaleExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SALE_EXP_MEST>>(HisRequestUriStore.HIS_SALE_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, saleFilter, null);
                if (listSaleExpMest == null || listSaleExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc SaleExpMest bang ID");
                var saleExpMest = listSaleExpMest.First();
                MPS.Processor.Mps000092.PDO.Mps000092PDO rdo = new MPS.Processor.Mps000092.PDO.Mps000092PDO(saleExpMest, this.resultSdo.ExpMedicines, this.resultSdo.ExpMaterials);
                WaitingManager.Hide();
                MPS.ProcessorBase.Core.PrintData printData = null;
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(printData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
        }

        private void InHuongDanSuDungThuoc(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                WaitingManager.Show();
                HisSaleExpMestViewFilter saleFilter = new HisSaleExpMestViewFilter();
                saleFilter.ID = this.resultSdo.SaleExpMest.ID;
                var listSaleExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SALE_EXP_MEST>>(HisRequestUriStore.HIS_SALE_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, saleFilter, null);
                if (listSaleExpMest == null || listSaleExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc SaleExpMest bang ID");
                var saleExpMest = listSaleExpMest.First();
                string genderName = "";
                long? dob = null;
                if (this.prescription != null)
                {
                    genderName = prescription.GENDER_NAME;
                    dob = this.prescription.DOB;
                }

                if (this.resultSdo.ExpMedicines != null && this.resultSdo.ExpMedicines.Count > 0)
                {
                    var Groups = this.resultSdo.ExpMedicines.GroupBy(g => g.MEDICINE_TYPE_ID).ToList();
                    WaitingManager.Hide();
                    foreach (var group in Groups)
                    {
                        var listByGroup = group.ToList<V_HIS_EXP_MEST_MEDICINE>();
                        MPS.Processor.Mps000099.PDO.Mps000099PDO rdo = new MPS.Processor.Mps000099.PDO.Mps000099PDO(saleExpMest, listByGroup, genderName, dob);
                        MPS.ProcessorBase.Core.PrintData printData = null;
                        printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        result = MPS.MpsPrinter.Run(printData);
                    }
                }
                else
                {
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
