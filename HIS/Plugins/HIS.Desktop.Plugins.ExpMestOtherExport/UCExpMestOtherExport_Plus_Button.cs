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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.ExpMestMedicineGrid;
using MOS.SDO;
using HIS.UC.ExpMestMedicineGrid.ADO;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.Filter;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.ExpMestOtherExport.ADO;
using DevExpress.Utils.Menu;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Plugins.ExpMestOtherExport.Resources;
using HIS.Desktop.Plugins.ExpMestOtherExport.Config;

namespace HIS.Desktop.Plugins.ExpMestOtherExport
{
    public partial class UCExpMestOtherExport : HIS.Desktop.Utility.UserControlBase
    {
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (xtraTabControlMain.SelectedTabPage == xtraTabPageBlood)
                {
                    btnAddBlood_ButtonClick(null, null);
                }
                else
                {
                    xtraTabControlExpMediMate.SelectedTabPage = xtraTabPageExpMediMate;
                    positionHandleControl = -1;
                    if (!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null)
                        return;

                    if (spinAmount.Value > this.currentMediMate.AVAILABLE_AMOUNT)
                    {
                        WaitingManager.Hide();
                        MessageManager.Show(String.Format(ResourceMessage.SoLuongXuatLonHonSoLuongKhadungTrongKho));
                        return;
                    }
                    this.currentMediMate.EXP_AMOUNT = spinAmount.Value;
                    this.currentMediMate.NOTE = txtNote.Text;
                    
                    if (spinExpPrice.EditValue != null)
                    {
                        this.currentMediMate.EXP_PRICE = Inventec.Common.TypeConvert.Parse.ToDecimal(spinExpPrice.Value.ToString());
                    }
                    if (spinPriceVAT.EditValue != null)
                    {
                        this.currentMediMate.EXP_VAT_RATIO = Inventec.Common.TypeConvert.Parse.ToDecimal(spinPriceVAT.Value.ToString()) / 100;
                    }

                    if (this.currentMediMate.EXP_PRICE > 0 && this.currentMediMate.EXP_AMOUNT > 0 && this.currentMediMate.DISCOUNT.HasValue)
                    {
                        this.currentMediMate.DISCOUNT_RATIO = this.currentMediMate.DISCOUNT.Value / (this.currentMediMate.EXP_PRICE.Value * this.currentMediMate.EXP_AMOUNT);
                    }
                    Dictionary<long, MediMateTypeADO> dic = null;
                    if (this.currentMediMate.IsMedicine)
                    {
                        this.currentMediMate.ExpMedicine.MedicineId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                        this.currentMediMate.ExpMedicine.Amount = this.currentMediMate.EXP_AMOUNT;
                        this.currentMediMate.ExpMedicine.Description = this.currentMediMate.NOTE;
                        this.currentMediMate.ExpMedicine.DiscountRatio = this.currentMediMate.DISCOUNT_RATIO;
                        this.currentMediMate.ExpMedicine.Price = this.currentMediMate.EXP_PRICE;
                        this.currentMediMate.ExpMedicine.VatRatio = this.currentMediMate.EXP_VAT_RATIO;
                        if (!dicTypeAdo.ContainsKey(TYPE_MEDICINE))
                            dicTypeAdo[TYPE_MEDICINE] = new Dictionary<long, MediMateTypeADO>();
                        dic = dicTypeAdo[TYPE_MEDICINE];
                    }
                    else if (this.currentMediMate.IsMedicine == false && this.currentMediMate.IsBlood == false)
                    {
                        this.currentMediMate.ExpMaterial.MaterialId = this.currentMediMate.MEDI_MATE_TYPE_ID;
                        this.currentMediMate.ExpMaterial.Amount = this.currentMediMate.EXP_AMOUNT;
                        this.currentMediMate.ExpMaterial.Description = this.currentMediMate.NOTE;
                        this.currentMediMate.ExpMaterial.DiscountRatio = this.currentMediMate.DISCOUNT_RATIO;
                        this.currentMediMate.ExpMaterial.Price = this.currentMediMate.EXP_PRICE;
                        this.currentMediMate.ExpMaterial.VatRatio = this.currentMediMate.EXP_VAT_RATIO;
                        if (!dicTypeAdo.ContainsKey(TYPE_MATERIAL))
                            dicTypeAdo[TYPE_MATERIAL] = new Dictionary<long, MediMateTypeADO>();
                        dic = dicTypeAdo[TYPE_MATERIAL];
                    }

                    if (isClick == false)
                    {
                        if (dic.ContainsKey(this.currentMediMate.MEDI_MATE_TYPE_ID))
                        {
                            //this.currentMediMate.EXP_AMOUNT += dicMediMateAdo[this.currentMediMate.SERVICE_ID].EXP_AMOUNT;
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(ResourceMessage.ThuocVatTuDaCoTrongDanhSachXuatBanCoMuonThayThe), MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) != DialogResult.Yes)
                            {
                                ResetValueControlDetail();
                                return;
                            }
                        }
                    }

                    dic[this.currentMediMate.MEDI_MATE_TYPE_ID] = this.currentMediMate;
                    FillDataGridExpMestDetail();

                    if (xtraTabControlMain.SelectedTabPage == xtraTabPageMedicine)
                    {
                        txtKeyworkMedicineInStock.EditValue = null;
                        txtKeyworkMedicineInStock.Focus();
                        txtKeyworkMedicineInStock.SelectAll();
                    }
                    else if (xtraTabControlMain.SelectedTabPage == xtraTabPageMaterial)
                    {
                        txtKeyworkMaterialInStock.EditValue = null;
                        txtKeyworkMaterialInStock.Focus();
                        txtKeyworkMaterialInStock.SelectAll();
                    }

                    ResetValueControlDetail();
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
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.mediStock == null)
                    return;
                WaitingManager.Show();
                EnableAllButton(false);
                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref success, ref param);
                EnableAllButton(true);
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this.ParentForm, param, success);
                    btnExpFromExcel.Enabled = false;
                }
                else
                {
                    if (resultSdo != null) btnExpFromExcel.Enabled = false;
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

        private void ProcessSave(ref bool success, ref CommonParam param)
        {
            try
            {
                HisExpMestOtherSDO data = new HisExpMestOtherSDO();
                data.MediStockId = this.mediStock.ID;
                data.Description = txtDescription.Text;
                data.Discount = (long?)(this.discountMoney);
                
                data.ReceivingPlace = txtRecevingPlace.Text;
                data.Recipient = txtRecipient.Text;
                data.ReqRoomId = this.roomId;
                if (cboReason.EditValue != null)
                {
                    data.ExpMestReasonId = Convert.ToInt64(cboReason.EditValue);
                }
                data.Materials = new List<ExpMaterialSDO>();
                data.Medicines = new List<ExpMedicineSDO>();
                data.Bloods = new List<ExpBloodSDO>();
                data.SerialNumbers = new List<PresMaterialBySerialNumberSDO>();

                foreach (var dic in dicTypeAdo)
                {
                    foreach (var item in dic.Value)
                    {
                        if (item.Value.IsNotHasMest)
                        {
                            success = false;
                            param.Messages.Add(ResourceMessage.TonTaiThuocVatTuKhongCoTrongKho);
                            return;
                        }
                        if (item.Value.EXP_AMOUNT <= 0)
                        {
                            success = false;
                            param.Messages.Add(ResourceMessage.SoLuongPhaiLonHonKhong);
                            return;
                        }
                        if (item.Value.AVAILABLE_AMOUNT < item.Value.EXP_AMOUNT)
                        {
                            success = false;
                            param.Messages.Add(ResourceMessage.SoLuongXuatLonHonSoLuongKhadungTrongKho);
                            return;
                        }
                        if (item.Value.IsMedicine)
                        {
                            data.Medicines.Add(item.Value.ExpMedicine);

                        }
                        else if ((!item.Value.IsBlood && !item.Value.IsMedicine))
                        {
                            data.Materials.Add(item.Value.ExpMaterial);
                        }
                        else if (item.Value.IsBlood)
                        {
                            item.Value.ExpBlood.Description = txtDescription.Text;
                            data.Bloods.Add(item.Value.ExpBlood);
                        }
                    }
                }
                var dataReuses = (List<V_HIS_MATERIAL_BEAN_1>)gridControlMaterialAdd.DataSource;
                if (dataReuses != null && dataReuses.Count > 0)
                {
                    foreach (var item in dataReuses)
                    {
                        PresMaterialBySerialNumberSDO seri = new PresMaterialBySerialNumberSDO();
                        seri.SerialNumber = item.SERIAL_NUMBER;
                        seri.MediStockId = this.mediStock.ID;
                        data.SerialNumbers.Add(seri);
                    }
                }
                if (data != null
                    && data.Materials.Count == 0
                    && data.Medicines.Count == 0
                    && data.Bloods.Count == 0
                    && data.SerialNumbers.Count == 0)
                {
                    param.Messages.Add("Dữ liệu rỗng");
                    return;
                }
                if (this.resultSdo != null)
                {
                    data.ExpMestId = this.resultSdo.ExpMest.ID;
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("(HisExpMestOtherSDO)data", data));
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>("api/HisExpMest/OtherUpdate", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("(HisExpMestOtherSDO)data", data));
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                        "api/HisExpMest/OtherCreate", ApiConsumers.MosConsumer, data, param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;
                    }
                }
                if (success)
                {
                    //ProcessFillDataBySuccess();
                    FillDataToTreeMediMate();//xuandv Reload data Tree
                    FillDataToGridExpMest();

                    //xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                    //txtKeyworkMedicineInStock.Focus();
                    //txtKeyworkMedicineInStock.SelectAll();
                    ddBtnPrint.Enabled = true;
                    btnSave.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                btnSave.Enabled = true;
                btnExpFromExcel.Enabled = true;
                ddBtnPrint.Enabled = false;
                EnableControlPriceAndAVT(false);
                ResetValueControlCommon();
                ResetValueControlDetail();
                this.resultSdo = null;
                FillDataGridExpMestDetail();
                FillDataToTreeMediMate();
                FillDataToGridExpMest();
                xtraTabControlMain.SelectedTabPage = xtraTabPageMedicine;
                txtKeyworkMedicineInStock.Focus();
                txtKeyworkMedicineInStock.SelectAll();

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
                    if (data.IsBlood)
                    {
                        dicTypeAdo[TYPE_BLOOD].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                    else if (data.IsMedicine)
                    {
                        dicTypeAdo[TYPE_MEDICINE].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                    else
                    {
                        dicTypeAdo[TYPE_MATERIAL].Remove(data.MEDI_MATE_TYPE_ID);
                    }
                    FillDataGridExpMestDetail();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExpFromExcel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnExpFromExcel.Enabled) return;
                CommonParam param = new CommonParam();
                bool success = false;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK || !(ofd.FileName.EndsWith(".xlsx")))
                    return;
                WaitingManager.Show();
                EnableAllButton(false);
                var import = new Inventec.Common.ExcelImport.Import();
                if (import.ReadFileExcel(ofd.FileName))
                {
                    var listImport = import.Get<ImportADO>(0);
                    if (listImport == null || listImport.Count == 0)
                    {
                        param.Messages.Add(ResourceMessage.DuLieuDocTuExcelRong);
                    }
                    else
                    {
                        success = true;
                        ProcessListImportADO(ref success, ref param, listImport);
                        var errors = listImport.Where(o => o.MessageErrors != null && o.MessageErrors.Count > 0).ToList();
                        if (errors != null && errors.Count > 0)
                        {
                            WaitingManager.Hide();
                            ImportExcel.frmExcelError frmImpError = new ImportExcel.frmExcelError(errors);
                            frmImpError.ShowDialog();
                        }
                    }
                }
                EnableAllButton(true);
                this.FillDataGridExpMestDetail();
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                EnableAllButton(true);
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListImportADO(ref bool success, ref CommonParam param, List<ImportADO> listImport)
        {
            try
            {
                this.dicTypeAdo = new Dictionary<long, Dictionary<long, MediMateTypeADO>>();

                Dictionary<long, Dictionary<long, MediMateTypeADO>> dicImport = new Dictionary<long, Dictionary<long, MediMateTypeADO>>();
                foreach (var item in listImport)
                {
                    if (string.IsNullOrWhiteSpace(item.TYPE_CODE))
                    {
                        item.MessageErrors.Add(ResourceMessage.KhongCoMa);
                    }
                    if (item.AMOUNT <= 0)
                    {
                        item.MessageErrors.Add(ResourceMessage.SoLuongPhaiLonHonKhong);
                    }
                    if (item.TYPE_ID < 1 || item.TYPE_ID > 3)
                    {
                        item.MessageErrors.Add(ResourceMessage.LoaiKhongChinhXac);
                    }
                    if (item.MessageErrors.Count > 0)
                    {
                        continue;
                    }
                    item.TYPE_CODE = item.TYPE_CODE.Trim();
                }

                var listNotError = listImport.Where(o => o.MessageErrors == null || o.MessageErrors.Count <= 0).ToList();
                if (listNotError.Count > 0)
                {
                    var Groups = listNotError.GroupBy(o => new { o.TYPE_ID, o.TYPE_CODE }).ToList();
                    foreach (var group in Groups)
                    {
                        if (group.Key.TYPE_ID == 1)
                        {
                            var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == group.Key.TYPE_CODE);
                            if (medicine == null)
                            {
                                group.ToList().ForEach(o => o.MessageErrors.Add(ResourceMessage.MaKhongChinhXac));
                                continue;
                            }
                            var mediInStocks = listMediInStock != null ? listMediInStock.Where(o => o.MEDICINE_TYPE_ID == medicine.ID).ToList() : null;

                            if (mediInStocks == null || mediInStocks.Count <= 0)
                            {
                                group.ToList().ForEach(o => o.MessageErrors.Add(ResourceMessage.ThuocVatTuMauKhongCoTrongKho));
                                continue;
                            }
                            else
                            {
                                if (!dicImport.ContainsKey(group.Key.TYPE_ID))
                                    dicImport[group.Key.TYPE_ID] = new Dictionary<long, MediMateTypeADO>();
                                var dicMedi = dicImport[group.Key.TYPE_ID];
                                this.SplitMedicineStock(mediInStocks, group.Sum(s => s.AMOUNT), dicMedi);
                            }
                        }
                        else if (group.Key.TYPE_ID == 2)
                        {
                            var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == group.Key.TYPE_CODE);
                            if (material == null)
                            {
                                group.ToList().ForEach(o => o.MessageErrors.Add(ResourceMessage.MaKhongChinhXac));
                                continue;
                            }
                            var mateInStocks = listMateInStock != null ? listMateInStock.Where(o => o.MATERIAL_TYPE_ID == material.ID).ToList() : null;

                            if (mateInStocks == null || mateInStocks.Count <= 0)
                            {
                                group.ToList().ForEach(o => o.MessageErrors.Add(ResourceMessage.ThuocVatTuMauKhongCoTrongKho));
                                continue;
                            }
                            else
                            {
                                if (!dicImport.ContainsKey(group.Key.TYPE_ID))
                                    dicImport[group.Key.TYPE_ID] = new Dictionary<long, MediMateTypeADO>();
                                var dicMedi = dicImport[group.Key.TYPE_ID];
                                this.SplitMaterialStock(mateInStocks, group.Sum(s => s.AMOUNT), dicMedi);
                            }
                        }
                        else if (group.Key.TYPE_ID == 3)
                        {
                            var blood = listBloodInStock != null ? listBloodInStock.FirstOrDefault(o => o.BLOOD_CODE == group.Key.TYPE_CODE) : null;
                            if (blood == null)
                            {
                                group.ToList().ForEach(o => o.MessageErrors.Add(ResourceMessage.MaKhongChinhXac));
                                continue;
                            }

                            if (!dicImport.ContainsKey(group.Key.TYPE_ID))
                                dicImport[group.Key.TYPE_ID] = new Dictionary<long, MediMateTypeADO>();
                            var dicMedi = dicImport[group.Key.TYPE_ID];
                            MediMateTypeADO ado = new MediMateTypeADO(blood);
                            ado.EXP_AMOUNT = 1;
                            ado.ExpBlood = new ExpBloodSDO();
                            ado.ExpBlood.BloodId = ado.BLOOD_ID;
                            dicMedi[ado.BLOOD_ID] = ado;
                        }
                    }
                    this.dicTypeAdo = dicImport;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.dicTypeAdo = new Dictionary<long, Dictionary<long, MediMateTypeADO>>();
                success = false;
            }
        }

        private void SplitMedicineStock(List<HisMedicineInStockSDO> inStocks, decimal requireAmount, Dictionary<long, MediMateTypeADO> dicMedi)
        {
            decimal existsAmount = inStocks.Sum(o => (o.AvailableAmount ?? 0));

            //Sap xep uu tien theo HSD, thoi gian nhap va so luong ton de tao bean
            //neu cau hinh uu tien theo thoi gian nhap
            if (HisConfigCFG.ExportOption == (int)1)
            {
                inStocks = inStocks
                .OrderBy(o => o.IMP_TIME)
                .ThenBy(o => o.EXPIRED_DATE.HasValue)//co hsd se bi xep sau, uu tien ko co HSD
                .ThenBy(o => o.EXPIRED_DATE)
                .ThenBy(o => o.AvailableAmount).ToList();
            }
            else
            {
                inStocks = inStocks
                .OrderBy(o => o.EXPIRED_DATE.HasValue) //co hsd se bi xep sau, uu tien ko co HSD
                .ThenBy(o => o.EXPIRED_DATE)
                .ThenBy(o => o.IMP_TIME)
                .ThenBy(o => o.AvailableAmount).ToList();
            }

            decimal leftAmount = requireAmount;
            int i = inStocks.Count;

            foreach (var item in inStocks)
            {
                MediMateTypeADO ado = new MediMateTypeADO(item);
                if (ado.AVAILABLE_AMOUNT >= leftAmount)
                {
                    ado.EXP_AMOUNT = leftAmount;
                    leftAmount = leftAmount - ado.EXP_AMOUNT;
                }
                else
                {
                    if (i <= 1)
                    {
                        ado.EXP_AMOUNT = leftAmount;
                        ado.IsGreatAvailable = true;
                        leftAmount = leftAmount - ado.EXP_AMOUNT;
                    }
                    else
                    {
                        ado.EXP_AMOUNT = ado.AVAILABLE_AMOUNT ?? 0;
                        leftAmount = leftAmount - ado.EXP_AMOUNT;
                    }
                }
                ado.ExpMedicine.Amount = ado.EXP_AMOUNT;
                dicMedi[ado.MEDI_MATE_TYPE_ID] = ado;
                i--;
                if (leftAmount <= 0)
                {
                    break;
                }
            }
        }

        private void SplitMaterialStock(List<HisMaterialInStockSDO> inStocks, decimal requireAmount, Dictionary<long, MediMateTypeADO> dicMedi)
        {
            decimal existsAmount = inStocks.Sum(o => (o.AvailableAmount ?? 0));

            //Sap xep uu tien theo HSD, thoi gian nhap va so luong ton de tao bean
            //neu cau hinh uu tien theo thoi gian nhap
            if (HisConfigCFG.ExportOption == (int)1)
            {
                inStocks = inStocks
                .OrderBy(o => o.IMP_TIME)
                .ThenBy(o => o.EXPIRED_DATE.HasValue)//co hsd se bi xep sau, uu tien ko co HSD
                .ThenBy(o => o.EXPIRED_DATE)
                .ThenBy(o => o.AvailableAmount).ToList();
            }
            else
            {
                inStocks = inStocks
                .OrderBy(o => o.EXPIRED_DATE.HasValue) //co hsd se bi xep sau, uu tien ko co HSD
                .ThenBy(o => o.EXPIRED_DATE)
                .ThenBy(o => o.IMP_TIME)
                .ThenBy(o => o.AvailableAmount).ToList();
            }

            decimal leftAmount = requireAmount;
            int i = inStocks.Count;

            foreach (var item in inStocks)
            {
                MediMateTypeADO ado = new MediMateTypeADO(item);
                if (ado.AVAILABLE_AMOUNT >= leftAmount)
                {
                    ado.EXP_AMOUNT = leftAmount;
                    leftAmount = leftAmount - ado.EXP_AMOUNT;
                }
                else
                {
                    if (i <= 1)
                    {
                        ado.EXP_AMOUNT = leftAmount;
                        ado.IsGreatAvailable = true;
                        leftAmount = leftAmount - ado.EXP_AMOUNT;
                    }
                    else
                    {
                        ado.EXP_AMOUNT = ado.AVAILABLE_AMOUNT ?? 0;
                        leftAmount = leftAmount - ado.EXP_AMOUNT;
                    }
                }
                ado.ExpMaterial.Amount = ado.EXP_AMOUNT;
                dicMedi[ado.MEDI_MATE_TYPE_ID] = ado;
                i--;
                if (leftAmount <= 0)
                {
                    break;
                }
            }
        }

        private void EnableAllButton(bool enable)
        {
            try
            {
                btnExpFromExcel.Enabled = enable;
                btnSave.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
