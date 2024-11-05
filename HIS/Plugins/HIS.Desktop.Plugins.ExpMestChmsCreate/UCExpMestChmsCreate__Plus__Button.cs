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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.ExpMestChmsCreate.ADO;
using HIS.Desktop.Print;
using Inventec.Common.Adapter;
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

namespace HIS.Desktop.Plugins.ExpMestChmsCreate
{
    public partial class UCExpMestChmsCreate : HIS.Desktop.Utility.UserControlBase
    {

        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GN_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_GNs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_HTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_TDs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_PXs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_COs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_DTs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_KSs { get; set; }
        List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_LAOs { get; set; }
        List<V_HIS_EXP_MEST_MEDICINE> _ExpMestMedicines { get; set; }
        List<V_HIS_EXP_MEST_BLOOD> _ExpMestBloods { get; set; }
        V_HIS_EXP_MEST chmsExpMest = new V_HIS_EXP_MEST();
        string Req_Department_Name = "";
        string Req_Room_Name = "";
        string Exp_Department_Name = "";
        long roomIdByMediStockIdPrint = 0;
        long keyPhieuTra = 0;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                if (chkPlanningExport.CheckState == CheckState.Checked)
                {
                    string str = "";

                    dicMediMateAdo = new Dictionary<long, MediMateTypeADO>();
                    var DataSourceMedicines = (List<HisMedicineInStockADO>)gridControlMedicine.DataSource;
                    if (DataSourceMedicines != null && DataSourceMedicines.Count() > 0)
                    {
                        List<HisMedicineInStockADO> DataFilter = DataSourceMedicines.Where(o => o.IsCheck).ToList();
                        foreach (var item in DataFilter)
                        {
                            if (item.EXP_AMOUNT <= 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                                return;
                            }
                            if (item.AvailableAmount < item.EXP_AMOUNT)
                            {
                                str += item.MEDICINE_TYPE_NAME + "; ";
                            }
                            MediMateTypeADO medicine = new MediMateTypeADO(item);
                            medicine.EXP_AMOUNT = (item.EXP_AMOUNT ?? 0);
                            medicine.ExpMedicine.Amount = (item.EXP_AMOUNT ?? 0);
                            dicMediMateAdo.Add(item.SERVICE_ID, medicine);

                        }
                    }

                    var DataSourceMaterials = (List<HisMaterialInStockADO>)gridControlMaterial.DataSource;
                    if (DataSourceMaterials != null && DataSourceMaterials.Count() > 0)
                    {
                        List<HisMaterialInStockADO> DataFilter = DataSourceMaterials.Where(o => o.IsCheck).ToList();
                        foreach (var item in DataFilter)
                        {
                            if (item.EXP_AMOUNT <= 0)
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                                return;
                            }
                            if (item.AvailableAmount < item.EXP_AMOUNT)
                            {
                                str += item.MATERIAL_TYPE_NAME + "; ";
                            }
                            MediMateTypeADO material = new MediMateTypeADO(item);
                            material.EXP_AMOUNT = (item.EXP_AMOUNT ?? 0);
                            material.ExpMaterial.Amount = (item.EXP_AMOUNT ?? 0);
                            dicMediMateAdo.Add(item.SERVICE_ID, material);
                        }
                    }

                    if (!String.IsNullOrEmpty(str) && DevExpress.XtraEditors.XtraMessageBox.Show("(" + str + ") " +
                   Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho + "\n Bạn có muốn tiếp tục?",
                   MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                   MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }
                }
                else
                {
                    positionHandleControl = -1;

                    if ((!btnAdd.Enabled || !dxValidationProvider2.Validate() || this.currentMediMate == null))
                        return;

                    long? MediStockId = null;
                    if (radioImport.Checked)
                    {
                        MediStockId = this.currentMediMate.MEDI_STOCK_ID;
                    }
                    else if (radioExport.Checked)
                    {
                        if (cboImpMediStock.EditValue != null)
                            MediStockId = (long)cboImpMediStock.EditValue;
                    }
                    if (MediStockId != null && (KeyOddPolicyOption == "1" || KeyOddPolicyOption == "2"))
                    {                      
                        var MediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == MediStockId);
                        if (MediStock != null && MediStock.IS_CABINET != 1 && MediStock.IS_BUSINESS != 1)
                        {
                            string type = null;
                            long value = 0;
                            bool IsOdd = !Int64.TryParse(spinExpAmount.EditValue.ToString(), out value);
                            if (currentMediMate.IsMedicine)
                            {
                                var medicineType = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == currentMediMate.MEDI_MATE_TYPE_ID);
                                if (medicineType != null && medicineType.IS_ALLOW_EXPORT_ODD != 1 && IsOdd)
                                {
                                    type = "Thuốc";
                                }
                            }
                            else if (!currentMediMate.IsBlood)
                            {
                                var materialType = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == currentMediMate.MEDI_MATE_TYPE_ID);
                                if (materialType != null && materialType.IS_ALLOW_EXPORT_ODD != 1 && IsOdd)
                                {
                                    type = "Vật tư";
                                }
                            }
                            if (!string.IsNullOrEmpty(type))
                            {
                                MessageBoxButtons mesButton = MessageBoxButtons.OK;
                                type += " " + currentMediMate.MEDI_MATE_TYPE_NAME + " không cho phép nhập/xuất lẻ.";
                                if (KeyOddPolicyOption == "1")
                                {
                                    DevExpress.XtraEditors.XtraMessageBox.Show(type, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), mesButton);
                                    return;
                                }
                                else if (KeyOddPolicyOption == "2")
                                {
                                    type += " " + "Bạn có muốn thực hiện không?";
                                    mesButton = MessageBoxButtons.YesNo;
                                    if (DevExpress.XtraEditors.XtraMessageBox.Show(type, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), mesButton) == DialogResult.No)
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if ((decimal?)spinExpAmount.EditValue > this.currentMediMate.AVAILABLE_AMOUNT)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho + "\n Bạn có muốn tiếp tục?",
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    this.currentMediMate.IsPackage = chkHienThiLo.Checked;
                    var dataIsPackage = dicMediMateAdo.Select(s => s.Value).FirstOrDefault(p => p.IsPackage != this.currentMediMate.IsPackage);
                    if (dataIsPackage != null && dataIsPackage.SERVICE_ID > 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Không cho phép tạo phiếu yêu cầu có cả vừa theo lô và không theo lô",
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao));
                        return;
                    }

                    bool check = true;

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("this.currentMediMate____:", this.currentMediMate));

                    List<string> PackageNumbers = new List<string>();


                    if (chkHienThiLo.Checked && !this.currentMediMate.IsBlood && this.currentMediMate.EXPIRED_DATE < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.Date))
                    {
                        check = false;
                    }

                    if (!chkHienThiLo.Checked && !this.currentMediMate.IsBlood)
                    {
                        long? dateNow = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now.Date);

                        if (this.currentMediMate.lstMaterialInStock != null && this.currentMediMate.lstMaterialInStock.Count > 0)
                        {
                            foreach (var item in this.currentMediMate.lstMaterialInStock)
                            {
                                if (item.EXPIRED_DATE < dateNow && item.AvailableAmount != 0 && item.TotalAmount != 0)
                                {
                                    check = false;
                                    if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER) && !PackageNumbers.Contains(item.PACKAGE_NUMBER))
                                    {
                                        PackageNumbers.Add(item.PACKAGE_NUMBER);
                                    }
                                }
                            }
                        }

                        if (this.currentMediMate.lstMedicineInStock != null && this.currentMediMate.lstMedicineInStock.Count > 0)
                        {
                            foreach (var item in this.currentMediMate.lstMedicineInStock)
                            {
                                if (item.EXPIRED_DATE < dateNow && item.AvailableAmount != 0 && item.TotalAmount != 0)
                                {
                                    check = false;
                                    if (!String.IsNullOrEmpty(item.PACKAGE_NUMBER))
                                    {
                                        PackageNumbers.Add(item.PACKAGE_NUMBER);
                                    }
                                }
                            }
                        }
                    }

                    if (!check)
                    {
                        string PackageNumber = this.currentMediMate.PACKAGE_NUMBER;
                        if (PackageNumbers != null && PackageNumbers.Count > 0)
                        {
                            PackageNumber = String.Join(", ", PackageNumbers);
                        }

                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                       String.Format(Base.ResourceMessageLang.ThuocVatTuHetHanSuDung, this.currentMediMate.MEDI_MATE_TYPE_NAME, PackageNumber),
                       MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            if (xtraTabControlMain.SelectedTabPageIndex == 0)
                            {
                                this.txtSearchMedicine.Focus();
                                this.txtSearchMedicine.SelectAll();
                            }
                            else if (xtraTabControlMain.SelectedTabPageIndex == 1)
                            {
                                this.txtSearchMaterial.Focus();
                                this.txtSearchMaterial.SelectAll();
                            }
                            else
                            {
                                this.txtSearch.Focus();
                                this.txtSearch.SelectAll();
                            }
                            return;
                        }
                    }

                    this.currentMediMate.EXP_AMOUNT = spinExpAmount.Value;
                    this.currentMediMate.NOTE = txtNote.Text;
                    //this.currentMediMate.MEDI_STOCK_ID_IPM = (long)cboImpMediStock.EditValue;
                    string message = "";
                    if (this.currentMediMate.IsMedicine)
                    {
                        message = "Thuốc ";
                        this.currentMediMate.ExpMedicine.Amount = spinExpAmount.Value;
                        this.currentMediMate.ExpMedicine.Description = txtNote.Text;

                    }
                    else if (this.currentMediMate.IsBlood)
                    {
                        message = "Máu ";

                        this.currentMediMate.ExpBlood.Amount = (long)spinExpAmount.Value;
                        this.currentMediMate.ExpBlood.Description = txtNote.Text;
                        this.currentMediMate.ExpBlood.BloodAboId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                        this.currentMediMate.ExpBlood.BloodRhId = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                        this.currentMediMate.BLOOD_ABO_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseABO.EditValue ?? 0).ToString());
                        this.currentMediMate.BLOOD_RH_ID = Inventec.Common.TypeConvert.Parse.ToInt64((cboChooseRH.EditValue ?? 0).ToString());
                        var unitName = BackendDataWorker.Get<V_HIS_BLOOD_TYPE>().FirstOrDefault(p => p.ID == this.currentMediMate.MEDI_MATE_TYPE_ID);
                        this.currentMediMate.SERVICE_UNIT_NAME = unitName != null ? unitName.SERVICE_UNIT_NAME : "";
                    }
                    else
                    {
                        message = "Vật tư ";

                        this.currentMediMate.ExpMaterial.Amount = spinExpAmount.Value;
                        this.currentMediMate.ExpMaterial.Description = txtNote.Text;
                    }

                    if (dicMediMateAdo.ContainsKey(this.currentMediMate.SERVICE_ID))
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(String.Format(
                        Base.ResourceMessageLang.ThuocVatTuDaCoTrongDanhSachXuat_BanCoMuonThayThe, message + this.currentMediMate.MEDI_MATE_TYPE_NAME),
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (radioImport.Checked == true)
                    {
                        this.currentMediMate.MEDI_STOCK_ID_IPM = currentMediMate.MEDI_STOCK_ID;
                        var aMaterialType = this.currentMediMate_.FirstOrDefault(o =>
                        o.SERVICE_ID == this.currentMediMate.SERVICE_ID &&
                     o.MEDI_MATE_TYPE_NAME == this.currentMediMate.MEDI_MATE_TYPE_NAME &&
                     o.MEDI_STOCK_ID_IPM == currentMediMate.MEDI_STOCK_ID &&
                     (currentMediMate.IsMedicine ? o.MEDICINE_ID == currentMediMate.MEDICINE_ID : (!currentMediMate.IsMedicine && !currentMediMate.IsBlood)  ? o.MATERIAL_ID == currentMediMate.MATERIAL_ID : o.MEDI_MATE_TYPE_NAME == this.currentMediMate.MEDI_MATE_TYPE_NAME)

                 );
                        //(long)cboExpMediStock.EditValue
                        if (aMaterialType != null && aMaterialType.MEDI_STOCK_ID_IPM > 0)
                        {
                            this.currentMediMate_.RemoveAll(o => o == aMaterialType);
                            currentMediMate_.Add(this.currentMediMate);
                        }

                        else currentMediMate_.Add(this.currentMediMate);
                    }
                    if (radioExport.Checked == true)
                    {
                        if (cboImpMediStock.EditValue != null)
                        {
                            this.currentMediMate.MEDI_STOCK_ID_IPM = (long)cboImpMediStock.EditValue;
                            var aMaterialType = this.currentMediMate_.FirstOrDefault(o =>
                            o.SERVICE_ID == this.currentMediMate.SERVICE_ID &&
                    o.MEDI_MATE_TYPE_NAME == this.currentMediMate.MEDI_MATE_TYPE_NAME &&
                    o.MEDI_STOCK_ID_IPM == (long)cboImpMediStock.EditValue &&
                     (currentMediMate.IsMedicine ? o.MEDICINE_ID == currentMediMate.MEDICINE_ID : (!currentMediMate.IsMedicine && !currentMediMate.IsBlood) ? o.MATERIAL_ID == currentMediMate.MATERIAL_ID : o.MEDI_MATE_TYPE_NAME == this.currentMediMate.MEDI_MATE_TYPE_NAME)
                );
                            if (aMaterialType != null && aMaterialType.MEDI_STOCK_ID_IPM > 0)
                            {
                                this.currentMediMate_.RemoveAll(o => o == aMaterialType);
                                currentMediMate_.Add(this.currentMediMate);
                            }

                            else currentMediMate_.Add(this.currentMediMate);
                        }
                        else
                        {

                            return;

                        }
                    }


                    WaitingManager.Show();

                    //dicMediMateAdo[this.currentMediMate.SERVICE_ID] = this.currentMediMate;
                    Inventec.Common.Logging.LogSystem.Info("Dic danh sach xuat: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediMate_), currentMediMate_));


                    //o.Type == 1

                    //    currentMediMate_.Add(this.currentMediMate);
                    if (this.currentMediMate.IsMedicine)
                    {
                        txtSearchMedicine.Focus();
                        txtSearchMedicine.SelectAll();
                    }
                    else if (this.currentMediMate.IsBlood)
                    {
                        this.txtSearch.Focus();
                        this.txtSearch.SelectAll();
                    }
                    else
                    {
                        txtSearchMaterial.Focus();
                        txtSearchMaterial.SelectAll();
                    }
                    this.currentMediMate = null;
                }

                gridControlExpMestChmsDetail.BeginUpdate();
                // gridControlExpMestChmsDetail.DataSource = (dicMediMateAdo != null && dicMediMateAdo.Count() > 0) ? dicMediMateAdo.Select(s => s.Value).ToList() : null;
                gridControlExpMestChmsDetail.DataSource = null;
                if (chkPlanningExport.Checked)
                {
                    this.gridColumnKho.GroupIndex = -1;
                    this.gridColumnKho.VisibleIndex = 18;
                    this.gridColumnKho.Visible = false;
                    gridControlExpMestChmsDetail.DataSource = (dicMediMateAdo != null && dicMediMateAdo.Count() > 0) ? dicMediMateAdo.Select(s => s.Value).ToList() : null;
                }
                else
                {
                    gridControlExpMestChmsDetail.DataSource = currentMediMate_;
                }

                gridControlExpMestChmsDetail.EndUpdate();
                gridControlExpMestChmsDetail.Refresh();
                ResetValueControlDetail();

                if (currentMediMate_ != null && currentMediMate_.Count > 0)
                {
                    gridViewExpMestChmsDetail.ExpandAllGroups();
                }
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

                if (chkPlanningExport.Checked == true)
                {
                    if (!btnSave.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0)
                        return;
                }
                //if (cboExpMediStock.EditValue == null || cboImpMediStock.EditValue == null)
                //{
                //    return;
                //}

                CommonParam param = new CommonParam();
                bool success = false;
                ProcessSave(ref param, ref success);
                if (success)
                {
                    WaitingManager.Show();
                    //LoadDataToTreeList(mestRoom);
                    FillDataToTrees();//Review---HieuNang
                    SetEnableCboMediStockAndButton(false);
                    ProcessFillDataBySuccess();
                    ddBtnPrint.Enabled = true;
                    WaitingManager.Hide();
                }

                MessageManager.Show(this.ParentForm, param, success);
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
                // Inventec.Common.Logging.LogSystem.Info("Dic danh sach xuat: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dicMediMateAdo), dicMediMateAdo));
                if (chkPlanningExport.Checked)
                {
                    HisExpMestChmsSDO data = new HisExpMestChmsSDO();
                    data.Description = txtDescription.Text;
                    if (radioImport.Checked)
                    {
                        data.Type = ChmsTypeEnum.GET;
                    }
                    else if (radioExport.Checked)
                    {
                        data.Type = ChmsTypeEnum.GIVE_BACK;
                    }

                    if (cboImpMediStock.EditValue != null)
                    {
                        var impMediStock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                        if (impMediStock != null)
                        {
                            data.ImpMediStockId = impMediStock.ID;
                        }
                    }

                    if (cboExpMediStock.EditValue != null)
                    {
                        var expMediStock = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                        if (expMediStock != null)
                        {
                            data.MediStockId = expMediStock.ID;
                        }
                    }

                    var req_room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == this.roomId && o.ROOM_TYPE_ID == this.roomTypeId).FirstOrDefault();
                    if (req_room != null)
                    {
                        data.ReqRoomId = req_room.ID;
                    }

                    data.Medicines = new List<ExpMedicineTypeSDO>();
                    data.Materials = new List<ExpMaterialTypeSDO>();
                    data.Bloods = new List<ExpBloodTypeSDO>();
                    data.ExpMaterialSdos = new List<ExpMaterialSDO>();
                    data.ExpMedicineSdos = new List<ExpMedicineSDO>();
                    string str = "";
                    foreach (var item in dicMediMateAdo)
                    {
                        if (item.Value.EXP_AMOUNT <= 0)
                        {
                            param.Messages.Add(Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong);
                            return;
                        }
                        if (item.Value.AVAILABLE_AMOUNT < item.Value.EXP_AMOUNT)
                        {
                            str += item.Value.MEDI_MATE_TYPE_NAME + "; ";
                        }
                        if (item.Value.IsMedicine)
                        {
                            if (item.Value.IsPackage)
                            {
                                ExpMedicineSDO ado = new ExpMedicineSDO();
                                ado.Amount = item.Value.ExpMedicine.Amount;
                                ado.MedicineId = item.Value.MEDICINE_ID;
                                ado.Description = item.Value.ExpMedicine.Description;
                                data.ExpMedicineSdos.Add(ado);
                            }
                            else
                                data.Medicines.Add(item.Value.ExpMedicine);
                        }
                        else if (item.Value.IsBlood)
                        {
                            data.Bloods.Add(item.Value.ExpBlood);
                        }
                        else
                        {
                            if (item.Value.IsPackage)
                            {
                                ExpMaterialSDO ado = new ExpMaterialSDO();
                                ado.Amount = item.Value.ExpMaterial.Amount;
                                ado.MaterialId = item.Value.MATERIAL_ID;
                                ado.Description = item.Value.ExpMaterial.Description;
                                data.ExpMaterialSdos.Add(ado);
                            }
                            else
                                data.Materials.Add(item.Value.ExpMaterial);
                        }
                    }

                    if (!String.IsNullOrEmpty(str) && DevExpress.XtraEditors.XtraMessageBox.Show("(" + str + ") " +
                        Base.ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho + "\n Bạn có muốn tiếp tục?",
                        MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return;
                    }

                    Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                    WaitingManager.Show();
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisExpMestResultSDO>(
                        (isUpdate && this.resultSdo != null) ? "api/HisExpMest/ChmsUpdate" : "api/HisExpMest/ChmsCreate",
                        ApiConsumers.MosConsumer,
                        data,
                        param);
                    if (rs != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo = rs;

                    }
                }
                else
                {
                    // Bắt đầu gửi đi
                    // -Api: api / HisExpMest / ChmsCreateList
                    //- Input: HisExpMestChmsListSDO
                    //- Output: List<HisExpMestResultSDO>
                    Inventec.Common.Logging.LogSystem.Info("Dic danh sach xuat: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediMate_), currentMediMate_));
                    WaitingManager.Show();
                    HisExpMestChmsListSDO data_ = new HisExpMestChmsListSDO();
                    if (cboReasonRequired.EditValue != null)
                        data_.ExpMestReasonId = Int64.Parse(cboReasonRequired.EditValue.ToString());
                    data_.ExpMests = new List<ExpMestChmsDetailSDO>();
                    if (currentMediMate_ != null && currentMediMate_.Count() > 0)
                    {
                        foreach (var item in currentMediMate_)
                        {
                            ExpMestChmsDetailSDO _data = new ExpMestChmsDetailSDO();
                            data_.Description = txtDescription.Text;
                            _data.MedicineTypes = new List<ExpMedicineTypeSDO>();
                            _data.MaterialTypes = new List<ExpMaterialTypeSDO>();
                            _data.BloodTypes = new List<ExpBloodTypeSDO>();
                            _data.Materials = new List<ExpMaterialSDO>();
                            _data.Medicines = new List<ExpMedicineSDO>();
                            if (radioImport.Checked)
                            {
                                data_.Type = ChmsTypeEnum.GET;
                            }
                            else if (radioExport.Checked)
                            {
                                data_.Type = ChmsTypeEnum.GIVE_BACK;
                            }
                            data_.WorkingRoomId = roomId;
                          

                            if (radioImport.Checked == true)
                            {
                                _data.ExpMediStockId = item.MEDI_STOCK_ID ?? 0;
                                if (cboImpMediStock.EditValue != null)
                                {
                                    var impMediStock = listImpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboImpMediStock.EditValue));
                                    if (impMediStock != null)
                                    {
                                        _data.ImpMediStockId = impMediStock.ID;
                                    }
                                }
                            }
                            else
                            {
                                _data.ImpMediStockId = item.MEDI_STOCK_ID_IPM ?? 0;
                                if (cboExpMediStock.EditValue != null)
                                {
                                    var expMediStocks = listExpMediStock.FirstOrDefault(o => o.ID == Convert.ToInt64(cboExpMediStock.EditValue));
                                    if (expMediStocks != null)
                                    {
                                        _data.ExpMediStockId = expMediStocks.ID;
                                    }
                                }
                            }


                            if (chkHienThiLo.Checked)
                            {
                                if (item.IsMedicine)
                                {
                                    if (item.IsPackage)
                                    {
                                        ExpMedicineSDO ado = new ExpMedicineSDO();
                                        ado.Amount = item.ExpMedicine.Amount;
                                        ado.MedicineId = item.MEDICINE_ID;
                                        ado.Description = item.ExpMedicine.Description;
                                        _data.Medicines.Add(ado);
                                    }
                                }
                                else if (!item.IsMedicine && !item.IsBlood)
                                {
                                    if (item.IsPackage)
                                    {
                                        ExpMaterialSDO ado = new ExpMaterialSDO();
                                        ado.Amount = item.ExpMaterial.Amount;
                                        ado.MaterialId = item.MATERIAL_ID;
                                        ado.Description = item.ExpMaterial.Description;
                                        _data.Materials.Add(ado);
                                    }
                                }
                            }
                            else
                            {
                                if (item.IsBlood)
                                {
                                    _data.BloodTypes.Add(item.ExpBlood);
                                }
                                else if (item.IsMedicine)
                                {

                                    _data.MedicineTypes.Add(item.ExpMedicine);
                                }
                                else
                                {
                                    _data.MaterialTypes.Add(item.ExpMaterial);
                                }
                            }

							var listExpMests = data_.ExpMests.Where(o => o.ExpMediStockId == _data.ExpMediStockId && o.ImpMediStockId == _data.ImpMediStockId).ToList();
							if (listExpMests.Count > 0 && listExpMests != null)
							{
								foreach (var items in listExpMests)
								{
                                    if (chkHienThiLo.Checked)
                                    {
                                        if (item.IsMedicine)
                                        {
                                            if (item.IsPackage)
                                            {
                                                ExpMedicineSDO ado = new ExpMedicineSDO();
                                                ado.Amount = item.ExpMedicine.Amount;
                                                ado.MedicineId = item.MEDICINE_ID;
                                                ado.Description = item.ExpMedicine.Description;
                                                items.Medicines.Add(ado);
                                            }
                                        }
                                        else if (!item.IsMedicine && !item.IsBlood)
                                        {
                                            if (item.IsPackage)
                                            {
                                                ExpMaterialSDO ado = new ExpMaterialSDO();
                                                ado.Amount = item.ExpMaterial.Amount;
                                                ado.MaterialId = item.MATERIAL_ID;
                                                ado.Description = item.ExpMaterial.Description;
                                                items.Materials.Add(ado);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (item.IsBlood)
                                        {
                                            items.BloodTypes.Add(item.ExpBlood);
                                        }
                                        else if (item.IsMedicine)
                                        {

                                            items.MedicineTypes.Add(item.ExpMedicine);
                                        }
                                        else
                                        {
                                            items.MaterialTypes.Add(item.ExpMaterial);
                                        }
                                    }
								}
							}
							else
							{
								data_.ExpMests.Add(_data);
                            }

                        }

                    }

                    Inventec.Common.Logging.LogSystem.Debug("Du lieu gui len: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data_), data_));
                    var rss = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HisExpMestResultSDO>>(
                       "api/HisExpMest/ChmsCreateList",
                      ApiConsumers.MosConsumer,
                      data_,
                      param);
                    if (rss != null)
                    {
                        success = true;
                        isUpdate = true;
                        this.resultSdo_ = new List<HisExpMestResultSDO>();
                        this.resultSdo_ = rss;
                        Inventec.Common.Logging.LogSystem.Debug("Du lieu nhan ve: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.resultSdo_), this.resultSdo_));

                    }
                    WaitingManager.Hide();
                }
            }

            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnUpdate.Enabled || !dxValidationProvider1.Validate() || dicMediMateAdo.Count == 0 || resultSdo == null)
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
                    ddBtnPrint.Enabled = true;
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
                SetEnableCboMediStockAndButton(true);
                radioImport.Checked = true;
                chkHienThiLo.Checked = false;
                LoadDataToCboMediStock();

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
                var data = (MediMateTypeADO)gridViewExpMestChmsDetail.GetFocusedRow();
                if (data != null)
                {
                    if (chkPlanningExport.Checked == true)
                    {
                        dicMediMateAdo.Remove(data.SERVICE_ID);
                    }
                    else
                    {
                        currentMediMate_.Remove(data);
                    }

                }
                gridControlExpMestChmsDetail.BeginUpdate();
                if (chkPlanningExport.Checked == true)
                {
                    gridControlExpMestChmsDetail.DataSource = dicMediMateAdo.Select(s => s.Value).ToList();
                }
                else
                {
                    gridControlExpMestChmsDetail.DataSource = currentMediMate_.ToList();
                }

                gridControlExpMestChmsDetail.EndUpdate();
                gridViewExpMestChmsDetail.ExpandAllGroups();
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
                if (chkPlanningExport.Checked == true)
                {
                    if (this.resultSdo == null)
                        return;
                }
                else
                {
                    if (this.resultSdo_ == null)
                        return;
                }
                Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086, delegatePrintTemplate);
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
                    if (this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
                    {
                        this.resultSdo.ExpMedicines = new List<HIS_EXP_MEST_MEDICINE>();
                        foreach (var item in this.resultSdo.ExpMetyReqs)
                        {
                            HIS_EXP_MEST_MEDICINE ado = new HIS_EXP_MEST_MEDICINE();
                            ado.AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.TDL_MEDICINE_TYPE_ID = item.MEDICINE_TYPE_ID;
                            this.resultSdo.ExpMedicines.Add(ado);
                        }
                    }
                    if (this.resultSdo.ExpMatyReqs != null && this.resultSdo.ExpMatyReqs.Count > 0)
                    {
                        this.resultSdo.ExpMaterials = new List<HIS_EXP_MEST_MATERIAL>();
                        foreach (var item in this.resultSdo.ExpMatyReqs)
                        {
                            HIS_EXP_MEST_MATERIAL ado = new HIS_EXP_MEST_MATERIAL();
                            ado.AMOUNT = item.AMOUNT;
                            ado.DESCRIPTION = item.DESCRIPTION;
                            ado.TDL_MATERIAL_TYPE_ID = item.MATERIAL_TYPE_ID;
                            this.resultSdo.ExpMaterials.Add(ado);
                        }
                    }

                    //}
                    if (this.resultSdo.ExpMedicines != null && this.resultSdo.ExpMedicines.Count > 0)
                    {
                        var Group = this.resultSdo.ExpMedicines.GroupBy(o => o.TDL_MEDICINE_TYPE_ID).ToList();
                        var _medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        foreach (var group in Group)
                        {
                            var dataMediType = _medicineType.FirstOrDefault(p => p.ID == group.Key);
                            var listByGroup = group.ToList<HIS_EXP_MEST_MEDICINE>();
                            if (dataMediType != null)
                            {
                                if (dicMediMateAdo.ContainsKey(dataMediType.SERVICE_ID))
                                {
                                    var medicine = dicMediMateAdo[dataMediType.SERVICE_ID];
                                    medicine.ExpMedicine.Amount = listByGroup.Sum(s => s.AMOUNT);
                                    medicine.ExpMedicine.Description = listByGroup.First().DESCRIPTION;
                                    medicine.ExpMedicine.MedicineTypeId = listByGroup.First().TDL_MEDICINE_TYPE_ID ?? 0;
                                }
                            }
                        }
                    }

                    if (this.resultSdo.ExpMaterials != null && this.resultSdo.ExpMaterials.Count > 0)
                    {

                        var Group = this.resultSdo.ExpMaterials.GroupBy(o => o.TDL_MATERIAL_TYPE_ID).ToList();
                        var _materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        foreach (var group in Group)
                        {
                            var dataMate = _materialType.FirstOrDefault(p => p.ID == group.Key);
                            if (dataMate != null)
                            {
                                var listByGroup = group.ToList<HIS_EXP_MEST_MATERIAL>();
                                if (dicMediMateAdo.ContainsKey(dataMate.SERVICE_ID))
                                {
                                    var material = dicMediMateAdo[dataMate.SERVICE_ID];
                                    material.ExpMaterial.Amount = listByGroup.Sum(s => s.AMOUNT);
                                    material.ExpMaterial.Description = listByGroup.First().DESCRIPTION;
                                    material.ExpMaterial.MaterialTypeId = listByGroup.First().TDL_MATERIAL_TYPE_ID ?? 0;
                                }
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
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKho_MPS000086:
                            InPhieuXuatChuyenKho(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocGayNghienHuongThan_MPS000089:
                            InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref result, printTypeCode, fileName);
                            break;
                        case PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan_MPS000090:
                            InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref result, printTypeCode, fileName);
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

        List<HIS_MEDICINE> _Medicines = null;
        List<HIS_MATERIAL> _Materials = null;
        List<HIS_BLOOD> _Bloods = null;
        List<V_HIS_MEDICINE_PATY> _Medicines_PATY = new List<V_HIS_MEDICINE_PATY>();
        List<V_HIS_MATERIAL_PATY> _Materials_PATY = new List<V_HIS_MATERIAL_PATY>();
        private void InPhieuXuatChuyenKho_(HisExpMestResultSDO resultSdo, bool result, string printTypeCode, string fileName)
        {
            try
            {
                long keyOrder = Convert.ToInt16(HisConfigs.Get<string>(AppConfigKeys.CONFIG_KEY__ODER_OPTION));
                Inventec.Common.Logging.LogSystem.Info("keyOrder____" + keyOrder);
                #region TT Chung
                WaitingManager.Show();
                HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                chmsFilter.ID = resultSdo.ExpMest.ID;
                var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                chmsExpMest = listChmsExpMest.First();

                CommonParam param = new CommonParam();
                Req_Department_Name = "";
                Req_Room_Name = "";
                Exp_Department_Name = "";
                var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == resultSdo.ExpMest.REQ_DEPARTMENT_ID).ToList();
                if (Req_Department != null && Req_Department.Count > 0)
                {
                    Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                }

                var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == resultSdo.ExpMest.REQ_ROOM_ID).ToList();
                if (Req_Room != null && Req_Room.Count > 0)
                {
                    Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                }
                var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == resultSdo.ExpMest.MEDI_STOCK_ID).ToList();
                if (Exp_Department != null && Exp_Department.Count > 0)
                {
                    Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                }

                roomIdByMediStockIdPrint = 0;
                roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.chmsExpMest.MEDI_STOCK_ID).ROOM_ID;

                this._Bloods = new List<HIS_BLOOD>();
                this._Medicines = new List<HIS_MEDICINE>();
                this._Materials = new List<HIS_MATERIAL>();

                _ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                _ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();

                _ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                _ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                if (resultSdo != null)
                {
                    if (resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                        || resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                        || resultSdo.ExpMest.IS_REQUEST_BY_PACKAGE == 1)
                    {
                        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                        mediFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                        if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                        {
                            List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                            MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                            medicineFilter.IDs = _MedicineIds;
                            this._Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                            //var his_medipaty = BackendDataWorker.Get<V_HIS_MEDICINE_PATY>().ToList();
                            if (_Medicines != null && _Medicines.Count > 0)
                            {
                                var lstID = _Medicines.Select(o => o.ID).Distinct().ToList();
                                MOS.Filter.HisMedicinePatyViewFilter medicinepatyFilter = new HisMedicinePatyViewFilter();
                                if (lstID != null)
                                {
                                    medicinepatyFilter.MEDICINE_IDs = lstID;
                                }
                                var his_medipaty = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_PATY>>("api/HisMedicinePaty/GetView", ApiConsumers.MosConsumer, medicinepatyFilter, param);

                                foreach (var item in _Medicines)
                                {
                                    if (item.IS_SALE_EQUAL_IMP_PRICE != 1)
                                    {
                                        var data = his_medipaty.Where(o => o.MEDICINE_ID == item.ID).ToList();
                                        if (data != null && data.Count > 0)
                                        {
                                            _Medicines_PATY.AddRange(data);
                                        }
                                    }
                                }
                            }
                        }

                        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                        matyFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                        if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                        {
                            List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                            MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                            materialFilter.IDs = _MaterialIds;
                            this._Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                           // var his_matepaty = BackendDataWorker.Get<V_HIS_MATERIAL_PATY>().ToList();
                            if (_Materials != null && _Materials.Count > 0)
                            {
                                var lstID = _Materials.Select(o => o.ID).Distinct().ToList();
                                MOS.Filter.HisMaterialPatyViewFilter matepatyFilter = new HisMaterialPatyViewFilter();
                                if (lstID != null)
                                {
                                    matepatyFilter.MATERIAL_IDs = lstID;
                                }
                                var his_matepaty = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_PATY>>("api/HisMaterialPaty/GetView", ApiConsumers.MosConsumer, matepatyFilter, param);

                                foreach (var item in _Materials)
                                {
                                    if (item.IS_SALE_EQUAL_IMP_PRICE != 1)
                                    {
                                        var data = his_matepaty.Where(o => o.MATERIAL_ID == item.ID).ToList();
                                        if (data != null && data.Count > 0)
                                        {
                                            _Materials_PATY.AddRange(data);
                                        }
                                    }
                                }
                            }
                        }


                        MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                        bloodFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                        if (_ExpMestBloods != null && _ExpMestBloods.Count > 0)
                        {
                            List<long> _BloodIds = _ExpMestBloods.Select(p => p.BLOOD_ID).ToList();
                            MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                            bloodNewFilter.IDs = _BloodIds;
                            this._Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                        }

                        MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                        metyReqFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        resultSdo.ExpMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, param);

                        MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                        matyReqFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        resultSdo.ExpMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, param);

                        MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                        bltyReqFilter.EXP_MEST_ID = resultSdo.ExpMest.ID;
                        resultSdo.ExpBltyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, param);

                    }
                    
                }

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                WaitingManager.Hide();
                #endregion
                #region In Tong Hop
                //if (keyPrintType == 1)
                //{
                //    string keyName = "";
                //    if (roomIdByMediStockIdPrint > 0)
                //    {
                //        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                //        {
                //            keyPhieuTra = 1;
                //            keyName = "PHIẾU TRẢ TỔNG HỢP";
                //        }
                //        else
                //        {
                //            keyName = "PHIẾU LĨNH TỔNG HỢP";
                //            keyPhieuTra = 0;
                //        }
                //    }
                //    MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //(
                // chmsExpMest,
                // _ExpMestMedicines,
                // _ExpMestMaterials,
                // _ExpMestBloods,
                // resultSdo.ExpMetyReqs,
                // resultSdo.ExpMatyReqs,
                // resultSdo.ExpBltyReqs,
                // Req_Department_Name,
                // Req_Room_Name,
                // Exp_Department_Name,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                // BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                // BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                // BackendDataWorker.Get<HIS_BLOOD_RH>(),
                // keyName,
                // configKeyMert,
                // keyPhieuTra,
                // this._Medicines,
                // this._Materials,
                // this._Bloods
                //  );
                //    MPS.ProcessorBase.Core.PrintData PrintData = null;
                //    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                //    }
                //    else
                //    {
                //        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //    }

                //    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                //    PrintData.EmrInputADO = inputADO;

                //    result = MPS.MpsPrinter.Run(PrintData);
                //}
                #endregion
                //else
                {
                    #region --- Xu Ly Tach GN_HT -----
                    if (resultSdo != null)
                    {
                        if (resultSdo.ExpMetyReqs != null && resultSdo.ExpMetyReqs.Count > 0)
                        {
                            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                            bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                            bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                            bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                            bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                            var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                            foreach (var item in resultSdo.ExpMetyReqs)
                            {
                                var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                                if (dataMedi != null)
                                {
                                    if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_GNs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                                    {
                                        _ExpMestMetyReq_GN_HTs.Add(item);
                                        _ExpMestMetyReq_HTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                                    {
                                        _ExpMestMetyReq_TDs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                                    {
                                        _ExpMestMetyReq_PXs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                                    {
                                        _ExpMestMetyReq_COs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                                    {
                                        _ExpMestMetyReq_DTs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                                    {
                                        _ExpMestMetyReq_KSs.Add(item);
                                    }
                                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                                    {
                                        _ExpMestMetyReq_LAOs.Add(item);
                                    }
                                    else
                                    {
                                        _ExpMestMetyReq_Ts.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region ----- In GN_HT ------
                    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                    this.resultSdo_1 = new HisExpMestResultSDO();
                    this.resultSdo_1 = resultSdo;
                    richEditorMain.RunPrintTemplate("Mps000198", DelegateRunMps);

                    richEditorMain.RunPrintTemplate("Mps000048", DelegateRunMps);
                    #endregion

                    #region -----In Thuoc Thuong -----
                    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                    {
                        string keyNameAggr = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNameAggr = "PHIẾU TRẢ THUỐC THƯỜNG";
                        }
                        else
                        {
                            keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                            keyPhieuTra = 0;
                        }

                        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                     (
                      chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 _ExpMestMetyReq_Ts,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 null,
                 null,
                 keyNameAggr,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods,
                 keyOrder,
                 BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                  _Medicines_PATY,
                  _Materials_PATY,
                  lstConfig
                       );
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    #endregion

                    #region -----In Vat Tu -----

                    if (resultSdo.ExpMatyReqs != null && resultSdo.ExpMatyReqs.Count > 0)
                    {
                        var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                        foreach (var item in resultSdo.ExpMatyReqs)
                        {
                            var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                            if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                            {
                                _ExpMestMatyReq_HCs.Add(item);
                            }
                            else
                                _ExpMestMatyReq_VTs.Add(item);
                        }
                        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                        {
                            string keyNameAggrHc = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggrHc = "PHIẾU TRẢ HÓA CHẤT";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                         chmsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 null,
                 _ExpMestMatyReq_HCs,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 null,
                 null,
                 keyNameAggrHc,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods,
                 keyOrder,
                 BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                 _Medicines_PATY,
                 _Materials_PATY,
                  lstConfig
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                        {
                            string keyNameAggr = "";
                            if (roomIdByMediStockIdPrint > 0)
                            {
                                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                                {
                                    keyPhieuTra = 1;
                                    keyNameAggr = "PHIẾU TRẢ VẬT TƯ";
                                }
                                else
                                {
                                    keyPhieuTra = 0;
                                    keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                                }
                            }
                            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                       (
                         chmsExpMest,
                 null,
                 _ExpMestMaterials,
                 null,
                 null,
                 _ExpMestMatyReq_VTs,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 null,
                 null,
                 keyNameAggr,
                 configKeyMert,
                 keyPhieuTra,
                 this._Medicines,
                 this._Materials,
                 this._Bloods,
                 keyOrder,
                 BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>(),
                 _Medicines_PATY,
                 _Materials_PATY,
                 lstConfig
                         );
                            MPS.ProcessorBase.Core.PrintData PrintData = null;
                            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            else
                            {
                                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                            }
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKho(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                if (chkPlanningExport.Checked == true)
                {

                    InPhieuXuatChuyenKho_(resultSdo, result, printTypeCode, fileName);
                }
                else
                {
                    foreach (var item in resultSdo_)
                    {
                        InPhieuXuatChuyenKho_(item, result, printTypeCode, fileName);
                    }

                }

                //#region TT Chung
                //WaitingManager.Show();
                //HisExpMestViewFilter chmsFilter = new HisExpMestViewFilter();
                //chmsFilter.ID = this.resultSdo.ExpMest.ID;
                //var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST>>(HisRequestUriStore.HIS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                //if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                //    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                //chmsExpMest = listChmsExpMest.First();

                //CommonParam param = new CommonParam();
                //Req_Department_Name = "";
                //Req_Room_Name = "";
                //Exp_Department_Name = "";
                //var Req_Department = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.ID == resultSdo.ExpMest.REQ_DEPARTMENT_ID).ToList();
                //if (Req_Department != null && Req_Department.Count > 0)
                //{
                //    Req_Department_Name = Req_Department.FirstOrDefault().DEPARTMENT_NAME;
                //}

                //var Req_Room = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == resultSdo.ExpMest.REQ_ROOM_ID).ToList();
                //if (Req_Room != null && Req_Room.Count > 0)
                //{
                //    Req_Room_Name = Req_Room.FirstOrDefault().ROOM_NAME;
                //}
                //var Exp_Department = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.ID == resultSdo.ExpMest.MEDI_STOCK_ID).ToList();
                //if (Exp_Department != null && Exp_Department.Count > 0)
                //{
                //    Exp_Department_Name = Exp_Department.FirstOrDefault().DEPARTMENT_NAME;
                //}

                //roomIdByMediStockIdPrint = 0;
                //roomIdByMediStockIdPrint = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(p => p.ID == this.chmsExpMest.MEDI_STOCK_ID).ROOM_ID;

                //this._Bloods = new List<HIS_BLOOD>();
                //this._Medicines = new List<HIS_MEDICINE>();
                //this._Materials = new List<HIS_MATERIAL>();

                //_ExpMestMedicines = new List<V_HIS_EXP_MEST_MEDICINE>();
                //List<V_HIS_EXP_MEST_MATERIAL> _ExpMestMaterials = new List<V_HIS_EXP_MEST_MATERIAL>();
                //_ExpMestBloods = new List<V_HIS_EXP_MEST_BLOOD>();

                //_ExpMestMetyReq_GN_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_GNs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_HTs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_TDs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_PXs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_COs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_DTs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_KSs = new List<HIS_EXP_MEST_METY_REQ>();
                //_ExpMestMetyReq_LAOs = new List<HIS_EXP_MEST_METY_REQ>();
                //List<HIS_EXP_MEST_METY_REQ> _ExpMestMetyReq_Ts = new List<HIS_EXP_MEST_METY_REQ>();
                //if (this.resultSdo != null)
                //{
                //    if (this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE
                //        || this.resultSdo.ExpMest.EXP_MEST_STT_ID == IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE
                //        || this.resultSdo.ExpMest.IS_REQUEST_BY_PACKAGE == 1)
                //    {
                //        MOS.Filter.HisExpMestMedicineViewFilter mediFilter = new HisExpMestMedicineViewFilter();
                //        mediFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        _ExpMestMedicines = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MEDICINE>>(HisRequestUriStore.HIS_EXP_MEST_MEDICINE_GETVIEW, ApiConsumers.MosConsumer, mediFilter, param);
                //        if (_ExpMestMedicines != null && _ExpMestMedicines.Count > 0)
                //        {
                //            List<long> _MedicineIds = _ExpMestMedicines.Select(p => p.MEDICINE_ID ?? 0).ToList();
                //            MOS.Filter.HisMedicineFilter medicineFilter = new HisMedicineFilter();
                //            medicineFilter.IDs = _MedicineIds;
                //            this._Medicines = new BackendAdapter(param).Get<List<HIS_MEDICINE>>("api/HisMedicine/Get", ApiConsumers.MosConsumer, medicineFilter, param);
                //        }

                //        MOS.Filter.HisExpMestMaterialViewFilter matyFilter = new HisExpMestMaterialViewFilter();
                //        matyFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        _ExpMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL>>(HisRequestUriStore.HIS_EXP_MEST_MATERIAL_GETVIEW, ApiConsumers.MosConsumer, matyFilter, param);
                //        if (_ExpMestMaterials != null && _ExpMestMaterials.Count > 0)
                //        {
                //            List<long> _MaterialIds = _ExpMestMaterials.Select(p => p.MATERIAL_ID ?? 0).ToList();
                //            MOS.Filter.HisMaterialFilter materialFilter = new HisMaterialFilter();
                //            materialFilter.IDs = _MaterialIds;
                //            this._Materials = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, materialFilter, param);
                //        }


                //        MOS.Filter.HisExpMestBloodViewFilter bloodFilter = new HisExpMestBloodViewFilter();
                //        bloodFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        _ExpMestBloods = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_BLOOD>>(HisRequestUriStore.HIS_EXP_MEST_BLOOD_GETVIEW, ApiConsumers.MosConsumer, bloodFilter, param);
                //        if (_ExpMestBloods != null && _ExpMestBloods.Count > 0)
                //        {
                //            List<long> _BloodIds = _ExpMestBloods.Select(p => p.BLOOD_ID).ToList();
                //            MOS.Filter.HisBloodFilter bloodNewFilter = new HisBloodFilter();
                //            bloodNewFilter.IDs = _BloodIds;
                //            this._Bloods = new BackendAdapter(param).Get<List<HIS_BLOOD>>("api/HisBlood/Get", ApiConsumers.MosConsumer, bloodNewFilter, param);
                //        }

                //        MOS.Filter.HisExpMestMetyReqFilter metyReqFilter = new HisExpMestMetyReqFilter();
                //        metyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        this.resultSdo.ExpMetyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_METY_REQ>>("api/HisExpMestMetyReq/Get", ApiConsumers.MosConsumer, metyReqFilter, param);

                //        MOS.Filter.HisExpMestMatyReqFilter matyReqFilter = new HisExpMestMatyReqFilter();
                //        matyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        this.resultSdo.ExpMatyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_MATY_REQ>>("api/HisExpMestMatyReq/Get", ApiConsumers.MosConsumer, matyReqFilter, param);

                //        MOS.Filter.HisExpMestBltyReqFilter bltyReqFilter = new HisExpMestBltyReqFilter();
                //        bltyReqFilter.EXP_MEST_ID = this.resultSdo.ExpMest.ID;
                //        this.resultSdo.ExpBltyReqs = new BackendAdapter(param).Get<List<HIS_EXP_MEST_BLTY_REQ>>("api/HisExpMestBltyReq/Get", ApiConsumers.MosConsumer, bltyReqFilter, param);

                //    }
                //}

                //long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(AppConfigKeys.HIS_DESKTOP_MPS_AGGR_EXP_MEST_MEDICINE_MERGER_DATA));
                //WaitingManager.Hide();
                //#endregion

                ////#region In Tong Hop
                ////if (keyPrintType == 1)
                ////{
                ////    string keyName = "";
                ////    if (roomIdByMediStockIdPrint > 0)
                ////    {
                ////        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                ////        {
                ////            keyPhieuTra = 1;
                ////            keyName = "PHIẾU TRẢ TỔNG HỢP";
                ////        }
                ////        else
                ////        {
                ////            keyName = "PHIẾU LĨNH TỔNG HỢP";
                ////            keyPhieuTra = 0;
                ////        }
                ////    }
                ////    MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                ////(
                //// chmsExpMest,
                //// _ExpMestMedicines,
                //// _ExpMestMaterials,
                //// _ExpMestBloods,
                //// this.resultSdo.ExpMetyReqs,
                //// this.resultSdo.ExpMatyReqs,
                //// this.resultSdo.ExpBltyReqs,
                //// Req_Department_Name,
                //// Req_Room_Name,
                //// Exp_Department_Name,
                //// IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                //// IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                //// BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                //// BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                //// BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                //// BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                //// BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                //// BackendDataWorker.Get<HIS_BLOOD_RH>(),
                //// keyName,
                //// configKeyMert,
                //// keyPhieuTra,
                //// this._Medicines,
                //// this._Materials,
                //// this._Bloods
                ////  );
                ////    MPS.ProcessorBase.Core.PrintData PrintData = null;
                ////    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                ////    {
                ////        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                ////    }
                ////    else
                ////    {
                ////        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                ////    }

                ////    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                ////    PrintData.EmrInputADO = inputADO;

                ////    result = MPS.MpsPrinter.Run(PrintData);
                ////}
                ////#endregion
                ////else
                //{
                //    #region --- Xu Ly Tach GN_HT -----
                //    if (this.resultSdo != null)
                //    {
                //        if (this.resultSdo.ExpMetyReqs != null && this.resultSdo.ExpMetyReqs.Count > 0)
                //        {
                //            var medicineGroupId = BackendDataWorker.Get<HIS_MEDICINE_GROUP>().ToList();
                //            var mediTs = medicineGroupId.Where(o => o.IS_SEPARATE_PRINTING == 1).ToList();
                //            bool gn = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN);
                //            bool ht = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT);
                //            bool doc = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC);
                //            bool px = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX);
                //            bool co = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO);
                //            bool dt = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN);
                //            bool ks = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS);
                //            bool lao = mediTs.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO);

                //            var mediTypes = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                //            foreach (var item in this.resultSdo.ExpMetyReqs)
                //            {
                //                var dataMedi = mediTypes.FirstOrDefault(p => p.ID == item.MEDICINE_TYPE_ID);
                //                if (dataMedi != null)
                //                {
                //                    if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN && gn)
                //                    {
                //                        _ExpMestMetyReq_GN_HTs.Add(item);
                //                        _ExpMestMetyReq_GNs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT && ht)
                //                    {
                //                        _ExpMestMetyReq_GN_HTs.Add(item);
                //                        _ExpMestMetyReq_HTs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DOC && doc)
                //                    {
                //                        _ExpMestMetyReq_TDs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__PX && px)
                //                    {
                //                        _ExpMestMetyReq_PXs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__CO && co)
                //                    {
                //                        _ExpMestMetyReq_COs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__DICH_TRUYEN && dt)
                //                    {
                //                        _ExpMestMetyReq_DTs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS && ks)
                //                    {
                //                        _ExpMestMetyReq_KSs.Add(item);
                //                    }
                //                    else if (dataMedi.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__LAO && lao)
                //                    {
                //                        _ExpMestMetyReq_LAOs.Add(item);
                //                    }
                //                    else
                //                    {
                //                        _ExpMestMetyReq_Ts.Add(item);
                //                    }
                //                }
                //            }
                //        }
                //    }
                //    #endregion

                //    #region ----- In GN_HT ------
                //    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                //    richEditorMain.RunPrintTemplate("Mps000198", DelegateRunMps);

                //    richEditorMain.RunPrintTemplate("Mps000048", DelegateRunMps);
                //    #endregion

                //    #region -----In Thuoc Thuong -----
                //    if (_ExpMestMetyReq_Ts != null && _ExpMestMetyReq_Ts.Count > 0)
                //    {
                //        string keyNameAggr = "";// "THUỐC THƯỜNG";
                //        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                //        {
                //            keyPhieuTra = 1;
                //            keyNameAggr = "PHIẾU TRẢ THUỐC THƯỜNG";
                //        }
                //        else
                //        {
                //            keyNameAggr = "PHIẾU LĨNH THUỐC THƯỜNG";
                //            keyPhieuTra = 0;
                //        }
                //        MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //     (
                //      chmsExpMest,
                // _ExpMestMedicines,
                // null,
                // null,
                // _ExpMestMetyReq_Ts,
                // null,
                // null,
                // Req_Department_Name,
                // Req_Room_Name,
                // Exp_Department_Name,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // null,
                // null,
                // keyNameAggr,
                // configKeyMert,
                // keyPhieuTra,
                // this._Medicines,
                // this._Materials,
                // this._Bloods
                //       );
                //        MPS.ProcessorBase.Core.PrintData PrintData = null;
                //        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //        {
                //            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //        }
                //        else
                //        {
                //            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //        }
                //        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                //        PrintData.EmrInputADO = inputADO;
                //        result = MPS.MpsPrinter.Run(PrintData);
                //    }
                //    #endregion

                //    #region -----In Vat Tu -----

                //    if (this.resultSdo.ExpMatyReqs != null && this.resultSdo.ExpMatyReqs.Count > 0)
                //    {
                //        var _materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                //        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_VTs = new List<HIS_EXP_MEST_MATY_REQ>();
                //        List<HIS_EXP_MEST_MATY_REQ> _ExpMestMatyReq_HCs = new List<HIS_EXP_MEST_MATY_REQ>();
                //        foreach (var item in this.resultSdo.ExpMatyReqs)
                //        {
                //            var dataMaty = _materialTypes.FirstOrDefault(p => p.ID == item.MATERIAL_TYPE_ID);
                //            if (dataMaty != null && dataMaty.IS_CHEMICAL_SUBSTANCE != null)
                //            {
                //                _ExpMestMatyReq_HCs.Add(item);
                //            }
                //            else
                //                _ExpMestMatyReq_VTs.Add(item);
                //        }
                //        if (_ExpMestMatyReq_HCs != null && _ExpMestMatyReq_HCs.Count > 0)
                //        {
                //            string keyNameAggrHc = "";
                //            if (roomIdByMediStockIdPrint > 0)
                //            {
                //                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                //                {
                //                    keyPhieuTra = 1;
                //                    keyNameAggrHc = "PHIẾU TRẢ HÓA CHẤT";
                //                }
                //                else
                //                {
                //                    keyPhieuTra = 0;
                //                    keyNameAggrHc = "PHIẾU LĨNH HÓA CHẤT";
                //                }
                //            }
                //            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //       (
                //         chmsExpMest,
                // null,
                // _ExpMestMaterials,
                // null,
                // null,
                // _ExpMestMatyReq_HCs,
                // null,
                // Req_Department_Name,
                // Req_Room_Name,
                // Exp_Department_Name,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                // BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                // keyNameAggrHc,
                // configKeyMert,
                // keyPhieuTra,
                // this._Medicines,
                // this._Materials,
                // this._Bloods
                //         );
                //            MPS.ProcessorBase.Core.PrintData PrintData = null;
                //            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //            {
                //                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //            }
                //            else
                //            {
                //                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //            }
                //            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                //            PrintData.EmrInputADO = inputADO;
                //            result = MPS.MpsPrinter.Run(PrintData);
                //        }
                //        if (_ExpMestMatyReq_VTs != null && _ExpMestMatyReq_VTs.Count > 0)
                //        {
                //            string keyNameAggr = "";
                //            if (roomIdByMediStockIdPrint > 0)
                //            {
                //                if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                //                {
                //                    keyPhieuTra = 1;
                //                    keyNameAggr = "PHIẾU TRẢ VẬT TƯ";
                //                }
                //                else
                //                {
                //                    keyPhieuTra = 0;
                //                    keyNameAggr = "PHIẾU LĨNH VẬT TƯ";
                //                }
                //            }
                //            MPS.Processor.Mps000086.PDO.Mps000086PDO mps000086PDO = new MPS.Processor.Mps000086.PDO.Mps000086PDO
                //       (
                //         chmsExpMest,
                // null,
                // _ExpMestMaterials,
                // null,
                // null,
                // _ExpMestMatyReq_VTs,
                // null,
                // Req_Department_Name,
                // Req_Room_Name,
                // Exp_Department_Name,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                // IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                // BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                // BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                // BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>(),
                // BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                // keyNameAggr,
                // configKeyMert,
                // keyPhieuTra,
                // this._Medicines,
                // this._Materials,
                // this._Bloods
                //         );
                //            MPS.ProcessorBase.Core.PrintData PrintData = null;
                //            if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //            {
                //                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //            }
                //            else
                //            {
                //                PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //            }
                //            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                //            PrintData.EmrInputADO = inputADO;
                //            result = MPS.MpsPrinter.Run(PrintData);
                //        }
                //    }
                //    #endregion
                //}
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunMps(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000048":
                        Mps000048(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000198":
                        Mps000198(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void Mps000048(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                long keyPrintType = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IN_GOP_GAY_NGHIEN_HUONG_THAN);
                if (keyPrintType == 1)
                {
                    #region ---- gop GN HT ----
                    if (this._ExpMestMetyReq_GN_HTs != null && this._ExpMestMetyReq_GN_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN, HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_GN_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }

                        if (this.resultSdo != null)
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }

                        else
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }

                    }
                    #endregion
                }
                else
                {
                    #region ---- GN ----
                    if (this._ExpMestMetyReq_GNs != null && this._ExpMestMetyReq_GNs.Count > 0)
                    {
                        WaitingManager.Show();
                        string keyAddictive = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyAddictive = "PHIẾU TRẢ THUỐC GÂY NGHIỆN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyAddictive = "PHIẾU LĨNH THUỐC GÂY NGHIỆN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_GNs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyAddictive,
                     configKeyMert,
                     keyPhieuTra
                       );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        if (this.resultSdo != null)
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }

                        else
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion

                    #region ---- HT ----
                    if (this._ExpMestMetyReq_HTs != null && this._ExpMestMetyReq_HTs.Count > 0)
                    {
                        WaitingManager.Show();
                        string keyNeurological = "";// "THUỐC THƯỜNG";
                        if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                        {
                            keyPhieuTra = 1;
                            keyNeurological = "PHIẾU TRẢ THUỐC HƯỚNG THẦN";
                        }
                        else
                        {
                            keyPhieuTra = 0;
                            keyNeurological = "PHIẾU LĨNH THUỐC HƯỚNG THẦN";
                        }
                        MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                     (
                      chmsExpMest,
                     _ExpMestMedicines,
                     null,
                     null,
                     this._ExpMestMetyReq_HTs,
                     null,
                     null,
                     Req_Department_Name,
                     Req_Room_Name,
                     Exp_Department_Name,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                     IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                     BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                     BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                     null,
                     null,
                     keyNeurological,
                     configKeyMert,
                     keyPhieuTra
                       );
                        WaitingManager.Hide();
                        MPS.ProcessorBase.Core.PrintData PrintData = null;
                        if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        else
                        {
                            PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                        }
                        if (this.resultSdo != null)
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }

                        else
                        {
                            Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                            PrintData.EmrInputADO = inputADO;
                            result = MPS.MpsPrinter.Run(PrintData);
                        }
                    }
                    #endregion
                }

                #region ---- DOC ----
                if (this._ExpMestMetyReq_TDs != null && this._ExpMestMetyReq_TDs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC ĐỘC";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC ĐỘC";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_TDs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                }
                #endregion

                #region ---- PX ----
                if (this._ExpMestMetyReq_PXs != null && this._ExpMestMetyReq_PXs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC PHÓNG XẠ";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC PHÓNG XẠ";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_PXs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                #endregion

                #region ---- CO ----
                if (this._ExpMestMetyReq_COs != null && this._ExpMestMetyReq_COs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC CORTICOID";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC CORTICOID";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_COs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                #endregion

                #region ---- DT ----
                if (this._ExpMestMetyReq_DTs != null && this._ExpMestMetyReq_DTs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC DỊCH TRUYỀN";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC DỊCH TRUYỀN";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_DTs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                #endregion

                #region ---- KS ----
                if (this._ExpMestMetyReq_KSs != null && this._ExpMestMetyReq_KSs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC KHÁNG SINH";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC KHÁNG SINH";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_KSs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                #endregion

                #region ---- LAO ----
                if (this._ExpMestMetyReq_LAOs != null && this._ExpMestMetyReq_LAOs.Count > 0)
                {
                    WaitingManager.Show();
                    string keyNeurological = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyNeurological = "PHIẾU TRẢ THUỐC LAO";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyNeurological = "PHIẾU LĨNH THUỐC LAO";
                    }
                    MPS.Processor.Mps000048.PDO.Mps000048PDO mps000086PDO = new MPS.Processor.Mps000048.PDO.Mps000048PDO
                 (
                  chmsExpMest,
                 _ExpMestMedicines,
                 null,
                 null,
                 this._ExpMestMetyReq_LAOs,
                 null,
                 null,
                 Req_Department_Name,
                 Req_Room_Name,
                 Exp_Department_Name,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>(),
                 null,
                 null,
                 keyNeurological,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000086PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }

                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Mps000198(string printTypeCode, string fileName, ref bool result)
        {
            try
            {

                long configKeyMert = Inventec.Common.TypeConvert.Parse.ToInt64(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.DESKTOP.MPS.AGGR_EXP_MEST_MEDICINE.MERGER_DATA"));
                if (this.resultSdo != null && this.resultSdo.ExpBltyReqs == null)
                {
                    this.resultSdo.ExpBltyReqs = this.resultSdo_1.ExpBltyReqs;
                }
                if (this.resultSdo != null && this.resultSdo.ExpBltyReqs != null)
                {
                    WaitingManager.Show();
                    string keyAddictive = "";// "THUỐC THƯỜNG";
                    if (roomIdByMediStockIdPrint == this.chmsExpMest.REQ_ROOM_ID)
                    {
                        keyPhieuTra = 1;
                        keyAddictive = "PHIẾU TRẢ MÁU";
                    }
                    else
                    {
                        keyPhieuTra = 0;
                        keyAddictive = "PHIẾU LĨNH MÁU";
                    }
                    MPS.Processor.Mps000198.PDO.Mps000198PDO mps000198PDO = new MPS.Processor.Mps000198.PDO.Mps000198PDO
                 (
                  chmsExpMest,
                 this.resultSdo.ExpBltyReqs,
                 this._ExpMestBloods,
                 BackendDataWorker.Get<V_HIS_MEDI_STOCK>(),
                 BackendDataWorker.Get<V_HIS_BLOOD_TYPE>(),
                 BackendDataWorker.Get<HIS_BLOOD_ABO>(),
                 BackendDataWorker.Get<HIS_BLOOD_RH>(),
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__EXECUTE,
                 IMSys.DbConfig.HIS_RS.HIS_EXP_MEST_STT.ID__DONE,
                 keyAddictive,
                 configKeyMert,
                 keyPhieuTra
                   );
                    WaitingManager.Hide();
                    MPS.ProcessorBase.Core.PrintData PrintData = null;
                    if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }
                    else
                    {
                        PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000198PDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                    }

                    if (this.resultSdo != null)
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                    else
                    {
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(this.resultSdo_1.ExpMest.TDL_TREATMENT_CODE, printTypeCode, this.roomId);
                        PrintData.EmrInputADO = inputADO;
                        result = MPS.MpsPrinter.Run(PrintData);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                //HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                //chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
                //var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                //if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                //    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                //var chmsExpMest = listChmsExpMest.First();
                ////MPS.Core.Mps000089.Mps000089RDO rdo = new MPS.Core.Mps000089.Mps000089RDO(chmsExpMest, this.resultSdo.ExpMedicines);
                ////WaitingManager.Hide();
                ////result = MPS.Printer.Run(printTypeCode, fileName, rdo);

                //MPS.Processor.Mps000089.PDO.Mps000089PDO mps000089RDO = new MPS.Processor.Mps000089.PDO.Mps000089PDO(
                //    chmsExpMest,
                //    this.resultSdo.ExpMedicines,
                //    HisExpMestSttCFG.HisExpMestSttId__Draft,
                //    HisExpMestSttCFG.HisExpMestSttId__Request,
                //    HisExpMestSttCFG.HisExpMestSttId__Rejected,
                //    HisExpMestSttCFG.HisExpMestSttId__Approved,
                //    HisExpMestSttCFG.HisExpMestSttId__Exported
                //                );
                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000089RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000089RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //}
                //result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuXuatChuyenKhoThuocKhongPhaiGayNghienHuongThan(ref bool result, string printTypeCode, string fileName)
        {
            try
            {
                WaitingManager.Show();
                //HisChmsExpMestViewFilter chmsFilter = new HisChmsExpMestViewFilter();
                //chmsFilter.ID = this.resultSdo.ChmsExpMest.ID;
                //var listChmsExpMest = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_CHMS_EXP_MEST>>(HisRequestUriStore.HIS_CHMS_EXP_MEST_GETVIEW, ApiConsumers.MosConsumer, chmsFilter, null);
                //if (listChmsExpMest == null || listChmsExpMest.Count != 1)
                //    throw new NullReferenceException("Khong lay duoc ChmsExpMest bang ID");
                //var chmsExpMest = listChmsExpMest.First();
                ////MPS.Core.Mps000090.Mps000090RDO rdo = new MPS.Core.Mps000090.Mps000090RDO(chmsExpMest, this.resultSdo.ExpMedicines, this.resultSdo.ExpMaterials);
                ////WaitingManager.Hide();
                ////result = MPS.Printer.Run(printTypeCode, fileName, rdo);

                //MPS.Processor.Mps000090.PDO.Mps000090PDO mps000090RDO = new MPS.Processor.Mps000090.PDO.Mps000090PDO(
                //    chmsExpMest,
                //    this.resultSdo.ExpMedicines,
                //    this.resultSdo.ExpMaterials,
                //    HisExpMestSttCFG.HisExpMestSttId__Draft,
                //    HisExpMestSttCFG.HisExpMestSttId__Request,
                //    HisExpMestSttCFG.HisExpMestSttId__Rejected,
                //    HisExpMestSttCFG.HisExpMestSttId__Approved,
                //    HisExpMestSttCFG.HisExpMestSttId__Exported
                //                );
                //MPS.ProcessorBase.Core.PrintData PrintData = null;
                //if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000090RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                //}
                //else
                //{
                //    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000090RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                //}
                //result = MPS.MpsPrinter.Run(PrintData);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataMedicinePlanning()
        {
            try
            {
                // thuoc
                var mestMetyDepaList = LoadMestMetyDepa();

                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue
                    && dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue
                    && cboImpMediStock.EditValue != null
                    && cboExpMediStock.EditValue != null)
                {
                    HisMetyStockWithImpStockViewFilter medicineFilter = new HisMetyStockWithImpStockViewFilter();
                    medicineFilter.EXP_DATE_FROM = Convert.ToInt64(dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");
                    medicineFilter.EXP_DATE_TO = Convert.ToInt64(dtToTime.DateTime.ToString("yyyyMMdd") + "235959");
                    medicineFilter.IMP_MEDI_STOCK_ID = Convert.ToInt64(cboImpMediStock.EditValue);
                    medicineFilter.MEDI_STOCK_ID = Convert.ToInt64(cboExpMediStock.EditValue);
                    List<HisMedicineTypeInStockSDO> MedicineInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMedicineTypeInStockSDO>>("api/HisMedicineType/GetInStockMedicineTypeWithImpStock", ApiConsumer.ApiConsumers.MosConsumer, medicineFilter, null);
                    if (MedicineInStocks != null && MedicineInStocks.Count > 0 && mestMetyDepaList != null && mestMetyDepaList.Count > 0)
                    {
                        MedicineInStocks = MedicineInStocks.Where(o => !mestMetyDepaList.Select(p => p.MEDICINE_TYPE_ID).Contains(o.Id)).ToList();
                    }
                    // lấy số lượng xuất > 0
                    MedicineInStocks = (MedicineInStocks != null && MedicineInStocks.Count() > 0) ? MedicineInStocks.Where(o => o.ExportedTotalAmount > 0).ToList() : MedicineInStocks;
                    var medicineTypeList = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();

                    MedicineInStocks = (MedicineInStocks != null && MedicineInStocks.Count() > 0) ? MedicineInStocks.Where(o =>
                                        ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.ImpStockTotalAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                                        && IS_GOODS_RESTRICT(o)
                                        && IsCheckMedicine(medicineTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.Id)
                                        ).ToList() : MedicineInStocks;
                    this.listMediInStock = new List<HisMedicineInStockADO>();
                    if (MedicineInStocks != null && MedicineInStocks.Count > 0)
                    {
                        foreach (var item in MedicineInStocks)
                        {
                            HisMedicineInStockADO ado = new HisMedicineInStockADO(item);
                            this.listMediInStock.Add(ado);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataMaterialPlanning()
        {
            try
            {
                // vat tu
                var mestMatyDepaList = LoadMestMatyDepa();

                if (dtFromTime.EditValue != null && dtFromTime.DateTime != DateTime.MinValue
                    && dtToTime.EditValue != null && dtToTime.DateTime != DateTime.MinValue
                    && cboImpMediStock.EditValue != null
                    && cboExpMediStock.EditValue != null)
                {
                    HisMatyStockWithImpStockViewFilter materialFilter = new HisMatyStockWithImpStockViewFilter();
                    materialFilter.EXP_DATE_FROM = Convert.ToInt64(dtFromTime.DateTime.ToString("yyyyMMdd") + "000000");
                    materialFilter.EXP_DATE_TO = Convert.ToInt64(dtToTime.DateTime.ToString("yyyyMMdd") + "235959");
                    materialFilter.IMP_MEDI_STOCK_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboImpMediStock.EditValue.ToString() ?? "0");
                    materialFilter.MEDI_STOCK_ID = Convert.ToInt64(cboExpMediStock.EditValue);

                    List<HisMaterialTypeInStockSDO> MaterialInStocks = new BackendAdapter(new CommonParam()).Get<List<HisMaterialTypeInStockSDO>>("api/HisMaterialType/GetInStockMaterialTypeWithImpStock", ApiConsumer.ApiConsumers.MosConsumer, materialFilter, null);
                    if (MaterialInStocks != null && MaterialInStocks.Count > 0 && mestMatyDepaList != null && mestMatyDepaList.Count > 0)
                    {
                        MaterialInStocks = MaterialInStocks.Where(o => !mestMatyDepaList.Select(p => p.MATERIAL_TYPE_ID).Contains(o.Id)).ToList();
                    }
                    // lấy các bản ghi có số lượng xuất >0
                    MaterialInStocks = (MaterialInStocks != null && MaterialInStocks.Count() > 0) ? MaterialInStocks.Where(o => o.ExportedTotalAmount > 0).ToList() : MaterialInStocks;

                    this.listMateInStock = new List<HisMaterialInStockADO>();

                    var materialTypeList = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();

                    MaterialInStocks = (MaterialInStocks != null && MaterialInStocks.Count() > 0) ? MaterialInStocks.Where(o =>
                           ((o.AvailableAmount > 0 && !chkIsNotAvailableButHaveInStock.Checked) || (o.ImpStockTotalAmount > 0 && chkIsNotAvailableButHaveInStock.Checked))
                           && IS_GOODS_RESTRICT(o)
                           && IsCheckMaterial(materialTypeList, this.mestRoom.IS_BUSINESS == 1 ? true : false, o.Id)
                           ).ToList() : MaterialInStocks;
                    this.listMateInStock = new List<HisMaterialInStockADO>();
                    if (MaterialInStocks != null && MaterialInStocks.Count > 0)
                    {
                        foreach (var item in MaterialInStocks)
                        {
                            HisMaterialInStockADO ado = new HisMaterialInStockADO(item);
                            this.listMateInStock.Add(ado);
                        }
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
