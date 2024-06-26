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
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Plugins.ExpBloodChmsCreate.ADO;
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

namespace HIS.Desktop.Plugins.ExpBloodChmsCreate
{
    public partial class UCExpBloodChmsCreate
    {
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                    return;
                WaitingManager.Show();

                if (spinExpAmount.Value > this.currentMediMate.AVAILABLE_AMOUNT)
                {
                    WaitingManager.Hide();
                    MessageManager.Show(Base.ResourceMessageLang.XuLyThatBai + Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                    return;
                }

                this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                this.currentMediMate.NOTE = txtNote.Text;

                this.currentMediMate.ExpBlood.Amount = (long)spinExpAmount.Value;
                this.currentMediMate.ExpBlood.Description = txtNote.Text;
                this.currentMediMate.ExpBlood.BloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                this.currentMediMate.ExpBlood.BloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                this.currentMediMate.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                this.currentMediMate.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());

                if (dicMediMateAdo.ContainsKey(this.currentMediMate.SERVICE_ID))
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ThuocVatTuDaCoTrongDanhSachXuat_BanCoMuonThayThe, Base.ResourceMessageLang.TieuDeCuaSoThongBaoLaThongBao, MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        WaitingManager.Hide();
                        return;
                    }
                }

                dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;

                gridControlExpBloodChmsDetail.BeginUpdate();
                gridControlExpBloodChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpBloodChmsDetail.EndUpdate();
                ResetValueControlDetail();
                txtSearch.Focus();
                txtSearch.SelectAll();
                this.currentMediMate = null;

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
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0)
                    return;
                if (cboExpMediStock.EditValue == null || cboImpMediStock.EditValue == null)
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                if (success)
                {
                    SetEnableCboMediStockAndButton(false);
                    ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    //ddBtnPrint.Enabled = true;
                }
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

        void ProcessSave(ref CommonParam param, ref bool success)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Dic danh sach xuat: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo), dicMediMateAdo));
                HisChmsExpMestSDO data = new HisChmsExpMestSDO();
                
                if (isUpdate && this.resultSdo != null)
                {
                    data.ChmsExpMest = this.resultSdo.ChmsExpMest;
                    data.ExpMest = this.resultSdo.ExpMest;
                }
                else
                {
                    data.ExpMest = new MOS.EFMODEL.DataModels.HIS_EXP_MEST();
                    data.ChmsExpMest = new MOS.EFMODEL.DataModels.HIS_CHMS_EXP_MEST();
                }
                data.ExpMest.EXP_MEST_STT_ID = HisExpMestSttCFG.HisExpMestSttId__Request;
                data.ExpMest.EXP_MEST_TYPE_ID = HisExpMestTypeCFG.HisExpMestTypeId__Chms;

                if (cboImpMediStock.EditValue != null)
                {
                    var impMediStock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                    if (impMediStock != null)
                    {
                        data.ChmsExpMest.IMP_MEDI_STOCK_ID = impMediStock.ID;
                    }
                }

                if (cboExpMediStock.EditValue != null)
                {
                    var expMediStock = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                    if (expMediStock != null)
                    {
                        data.ExpMest.MEDI_STOCK_ID = expMediStock.ID;
                    }
                }

                data.ExpMaterials = new List<HisMaterialTypeAmountSDO>();
                data.ExpMedicines = new List<HisMedicineTypeAmountSDO>();
                data.ExpBloodTypes = new List<HisBloodTypeAmountSDO>();
                foreach (var item in dicMediMateAdo)
                {
                    if (item.Value.EXP_AMOUNT <= 0)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                        return;
                    }
                    if (item.Value.AVAILABLE_AMOUNT < item.Value.EXP_AMOUNT)
                    {
                        param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho);
                        return;
                    }
                    data.ExpBloodTypes.Add(item.Value.ExpBlood);

                }

                Inventec.Common.Logging.LogSystem.Info("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                if (isUpdate && this.resultSdo != null)
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisChmsExpMestResultSDO>(HisRequestUriStore.HIS_CHMS_EXP_MEST_UPDATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
                else
                {
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisChmsExpMestResultSDO>(HisRequestUriStore.HIS_CHMS_EXP_MEST_CREATE, ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0 || resultSdo == null)
                    return;
                if (cboExpMediStock.EditValue == null || cboImpMediStock.EditValue == null)
                {
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                if (success)
                {
                    SetEnableCboMediStockAndButton(false);
                    ProcessFillDataBySuccess();
                    FillDataToGridExpMest();
                    //ddBtnPrint.Enabled = true;
                }
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

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                ResetValueControlCommon();
                FillDataToGridExpMest();
                LoadDataToGridBloodType(null);
                V_HIS_MEDI_STOCK mestRoom = null;
                if (cboExpMediStock.EditValue != null)
                {
                    mestRoom = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                }
                LoadDataToGridBloodType(mestRoom);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDeleteDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MediMateTypeADO)gridViewExpBloodChmsDetail.GetFocusedRow();
                if (data != null)
                {
                    dicMediMateAdo.Remove(data.SERVICE_ID);
                }
                gridControlExpBloodChmsDetail.BeginUpdate();
                gridControlExpBloodChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                gridControlExpBloodChmsDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPrintPhieuXuatChuyenKho(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate("Mps000198", delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPrintPhieuGayNghienHuongThan(object sender, EventArgs e)
        {
            if (this.resultSdo == null || this.resultSdo.ExpMedicines == null)
                return;
            Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
            store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089, delegatePrintTemplate);
        }

        private void onClickPrintPhieuKhongPhaiGayNghienHuongThan(object sender, EventArgs e)
        {
            try
            {
                if (this.resultSdo == null)
                    return;
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090, delegatePrintTemplate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                                medicine.ExpMedicine.Amount = listByGroup.Sum(s => s.AMOUNT);
                                medicine.ExpMedicine.Description = listByGroup.First().DESCRIPTION;
                                medicine.ExpMedicine.MedicineTypeId = listByGroup.First().MEDICINE_TYPE_ID;
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
                                material.ExpMaterial.Description = listByGroup.First().DESCRIPTION;
                                material.ExpMaterial.MaterialTypeId = listByGroup.First().MATERIAL_TYPE_ID;
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

        private bool delegatePrintTemplate(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (!String.IsNullOrEmpty(printTypeCode))
                {
                    switch (printTypeCode)
                    {
                        case "Mps000198":
                            InPhieuXuatChuyenKho(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089:
                            //InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090:
                            //InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InPhieuXuatChuyenKho(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                var chmsExpMest = listChmsExpMest.First();
                WaitingManager.Show();
                MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198RDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO(
                 chmsExpMest, 
                 this.resultSdo.ExpBlties
                  );

                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //private void InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
        //        chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
        //        var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
        //        if (listChmsExpMest == null || listChmsExpMest.Count != 1)
        //            throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
        //        var chmsExpMest = listChmsExpMest.First();
        //        MPS.Core.Mps000089.Mps000089RDO rdo = new MPS.Core.Mps000089.Mps000089RDO(chmsExpMest, this.resultSdo.ExpMedicines);
        //        WaitingManager.Hide();
        //        result = MPS.Printer.Run(printTypeCode, fileName, rdo);
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //private void InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        //{
        //    try
        //    {
        //        WaitingManager.Show();
        //        HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
        //        chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
        //        var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
        //        if (listChmsExpMest == null || listChmsExpMest.Count != 1)
        //            throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
        //        var chmsExpMest = listChmsExpMest.First();
        //        MPS.Core.Mps000090.Mps000090RDO rdo = new MPS.Core.Mps000090.Mps000090RDO(chmsExpMest, this.resultSdo.ExpMedicines, this.resultSdo.ExpMaterials);
        //        WaitingManager.Hide();
        //        result = MPS.Printer.Run(printTypeCode, fileName, rdo);
        //    }
        //    catch (Exception ex)
        //    {
        //        WaitingManager.Hide();
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}
    }
}
