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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.Base;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {
        Inventec.Desktop.Common.Modules.Module module { get; set; }
        long roomId { get; set; }
        long roomTypeId { get; set; }
        HIS_MEDI_STOCK mediStock { get; set; }
        List<D_HIS_MEDI_STOCK_1> mediStockD1SDOs { get; set; }
        int theRequiredWidth = 900, theRequiredHeight = 130;
        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        MetyProductADO currentMetyThanhPham { get; set; }
        DMediStock1ADO currentMateChePham { get; set; }
        List<DispenseMedyMatyADO> dispenseMetyMatyADOs { get; set; }
        ACTION action { get; set; }
        bool isShow = true;
        int positionHandleControlLeft = 1;
        List<MetyProductADO> MetyProductADO = new List<MetyProductADO>();
        HisDispenseHandlerResultSDO hisDispenseResultSDO { get; set; }
        List<MedicinePatyADO> medicinePatyADOs { get; set; }
        long? dispenseId { get; set; }

        private bool isShowMessUpdate { get; set; }


        public enum ACTION
        {
            CREATE,
            UPDATE
        }


        public frmDispenseMedicine(Inventec.Desktop.Common.Modules.Module module)
        {
            InitializeComponent();
            try
            {
                this.module = module;
                this.roomId = this.module.RoomId;
                this.roomTypeId = this.module.RoomTypeId;
                this.action = ACTION.CREATE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmDispenseMedicine(Inventec.Desktop.Common.Modules.Module module, long dispenseId)
        {
            InitializeComponent();
            try
            {
                this.module = module;
                this.roomId = this.module.RoomId;
                this.roomTypeId = this.module.RoomTypeId;
                this.dispenseId = dispenseId;
                this.action = ACTION.UPDATE;
                txtThuocThanhPham.Focus();
                txtThuocThanhPham.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmDispenseMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(Inventec.Desktop.Common.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
                this.ValidateControl();
                WaitingManager.Show();
                LoadMediStockFromRoomId();
                List<Action> methods = new List<Action>();
                methods.Add(InitDataMetyMatyTypeInStockD1);
                methods.Add(LoadMetyProductADOs);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();
                LoadDataToControl();
                LoadDispenseMedicineEdit(dispenseId);
                InitEnabledAction(action);
                EnabledButtonPrint();
                ValidateExpTime();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThuocThanhPham_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.DropDown)
            {
                isShowContainer = !isShowContainer;
                if (isShowContainer)
                {
                    Rectangle buttonBounds = new Rectangle(txtThuocThanhPham.Bounds.X + this.Bounds.X + 8, txtThuocThanhPham.Bounds.Y + this.Bounds.Y, txtThuocThanhPham.Bounds.Width - 90, txtThuocThanhPham.Bounds.Height);
                    popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                }
            }
        }

        private void txtThuocChePham_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        Rectangle buttonBounds = new Rectangle(txtThuocChePham.Bounds.X + this.Bounds.X + 8, txtThuocChePham.Bounds.Y + this.Bounds.Y, txtThuocChePham.Bounds.Width - 90, txtThuocChePham.Bounds.Height);
                        popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewThuocTP_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                MetyProductADO mediThanhPham = gridViewThuocTP.GetFocusedRow() as MetyProductADO;
                if (mediThanhPham != null)
                {
                    popupControlContainer.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridThanhPham_RowClick(mediThanhPham);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewThuocCP_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                DMediStock1ADO mediChePham = gridViewThuocCP.GetFocusedRow() as DMediStock1ADO;
                if (mediChePham != null)
                {
                    popupControlContainerChePham.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridChePham_RowClick(mediChePham);
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
                Inventec.Common.Logging.LogSystem.Info("start check");
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider2.Validate() || !Check())
                    return;

                // Danh sách thuốc thành phẩm được check
                bool success = false;
                CommonParam param = new CommonParam();

                Inventec.Common.Logging.LogSystem.Info("Finish check");

                if (this.action == ACTION.CREATE)
                {
                    Inventec.Common.Logging.LogSystem.Info("Begin call api create");

                    WaitingManager.Show();
                    this.EnableControlSave(false);
                    HisDispenseSDO hisDispenseSDO = new HisDispenseSDO();
                    Inventec.Common.Logging.LogSystem.Info("Gan dl creat");
                    this.ProcessDataToCreate(dispenseMetyMatyADOs, ref hisDispenseSDO);
                    Inventec.Common.Logging.LogSystem.Info("ket thuc gan dl creat");
                    hisDispenseResultSDO = new BackendAdapter(param)
                        .Post<HisDispenseHandlerResultSDO>("api/HisDispense/Create", ApiConsumers.MosConsumer, hisDispenseSDO, param);
                    Inventec.Common.Logging.LogSystem.Info("End call api create");
                    this.EnableControlSave(true);
                    WaitingManager.Hide();

                }
                else if (this.action == ACTION.UPDATE)
                {
                    Inventec.Common.Logging.LogSystem.Info("Begin call api update");
                    if (isShowMessUpdate)
                    {
                        DialogResult myResult;
                        myResult = MessageBox.Show("bạn có chắc muốn sửa phiếu bào chế hay không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            return;
                        }
                    }
                    WaitingManager.Show();
                    this.EnableControlSave(false);
                    HisDispenseUpdateSDO dispenseUpdateSDO = new HisDispenseUpdateSDO();
                    Inventec.Common.Logging.LogSystem.Info("Gan dl");
                    this.ProcessDataToUpdate(dispenseMetyMatyADOs, ref dispenseUpdateSDO);
                    Inventec.Common.Logging.LogSystem.Info("ket thuc gan");
                    hisDispenseResultSDO = new BackendAdapter(param)
                            .Post<HisDispenseHandlerResultSDO>("api/HisDispense/Update", ApiConsumers.MosConsumer, dispenseUpdateSDO, param);
                    Inventec.Common.Logging.LogSystem.Info("End call api update");
                    this.EnableControlSave(true);
                    WaitingManager.Hide();

                }


                if (hisDispenseResultSDO != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("begin");
                    isShowMessUpdate = true;
                    success = true;
                    this.action = ACTION.UPDATE;
                    dispenseId = hisDispenseResultSDO.HisDispense.ID;
                    LoadImpMestToGrid(
                        new List<HIS_IMP_MEST> { hisDispenseResultSDO.HisImpMest }
                        , new List<HIS_EXP_MEST> { hisDispenseResultSDO.HisExpMest });
                    InitEnabledAction(ACTION.UPDATE);
                    LoadMedicinePaty(hisDispenseResultSDO.MedicinePaties);
                    Inventec.Common.Logging.LogSystem.Info("end");
                }

                EnabledButtonPrint();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {

                List<MedicinePatyADO> listMedicineTemp1 = medicinePatyADOs.Where(o => (!o.Price.HasValue && o.Vat.HasValue)).ToList();
                if (listMedicineTemp1 != null && listMedicineTemp1.Count > 0)
                {
                    MessageBox.Show("Lỗi nhập giá theo đối tượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }


                List<MedicinePatyADO> listMedicineTemp2 = medicinePatyADOs.Where(o => o.Price.HasValue).ToList();
                if (listMedicineTemp2 == null || listMedicineTemp2.Count == 0)
                {
                    MessageBox.Show("Nhập ít nhất một giá bán cho một đối tượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (dispenseMetyMatyADOs == null || dispenseMetyMatyADOs.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một thuốc chế phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                DispenseMedyMatyADO dispenseMedyMatyADO = dispenseMetyMatyADOs.FirstOrDefault(o => o.IsNotAvaliable);
                if (dispenseMedyMatyADO != null)
                {
                    MessageBox.Show("Tồn tại thuốc chế phẩm vượt khả dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                DispenseMedyMatyADO dispenseMedyMatyADO1 = dispenseMetyMatyADOs.FirstOrDefault(o => o.Amount <= 0);
                if (dispenseMedyMatyADO1 != null)
                {
                    return false;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void EnableControlSave(bool enabled)
        {
            try
            {
                btnSave.Enabled = enabled;
                btnPrint.Enabled = !enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnBoSung_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateControl();
                this.positionHandleControlLeft = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                if (dispenseMetyMatyADOs == null)
                    dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();

                DispenseMedyMatyADO dispenseMatyADOExist = dispenseMetyMatyADOs.FirstOrDefault(o => o.ServiceTypeId == this.currentMateChePham.SERVICE_TYPE_ID && o.PreparationMediMatyTypeId == this.currentMateChePham.ID);

                if (dispenseMatyADOExist != null)
                {
                    MessageBox.Show("Tồn tại trong danh sách chế phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Check kha dung
                var mediStockD1SDO = mediStockD1SDOs.FirstOrDefault(o => o.ID == this.currentMateChePham.ID && o.AMOUNT < spinMateAmount.Value);
                if (mediStockD1SDO != null)
                {
                    MessageBox.Show("Số lượng chế phẩm không đủ khả dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Thêm chế phẩm
                DispenseMedyMatyADO dispenseMatyADO = new DispenseMedyMatyADO();
                dispenseMatyADO.PreparationMediMatyTypeId = this.currentMateChePham.ID ?? 0;
                dispenseMatyADO.PreparationMediMatyTypeName = this.currentMateChePham.MEDICINE_TYPE_NAME;
                dispenseMatyADO.Amount = spinMateAmount.Value;
                dispenseMatyADO.ServiceUnitName = this.currentMateChePham.SERVICE_UNIT_NAME;
                dispenseMatyADO.ServiceTypeId = (long)this.currentMateChePham.SERVICE_TYPE_ID;
                dispenseMetyMatyADOs.Add(dispenseMatyADO);
                this.CapNhatKhaDungThuocChePham(this.currentMateChePham.ID ?? 0, dispenseMatyADO.ServiceTypeId, spinMateAmount.Value, PROCESS.TRU);
                gridControlDSChePham.RefreshDataSource();

                this.ResetControlAfterAdd(RESET.ADD);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetMatyFromMetyMaty(long medicineTypeId)
        {
            try
            {
                //Them che pham theo cau hinh
                CommonParam param = new CommonParam();
                #region Load Thuoc
                HisMetyMatyFilter metyMatyFilter = new HisMetyMatyFilter();
                List<MOS.EFMODEL.DataModels.V_HIS_METY_MATY> metyMatysResult = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_METY_MATY>>("api/HisMetyMaty/GetView", ApiConsumers.MosConsumer, metyMatyFilter, param);
                List<MOS.EFMODEL.DataModels.V_HIS_METY_MATY> metyMatys = new List<V_HIS_METY_MATY>();
                if (metyMatysResult != null && metyMatysResult.Count > 0)
                    metyMatys = metyMatysResult.Where(o => o.METY_PRODUCT_ID == medicineTypeId).ToList();
                List<V_HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                if (metyMatys != null && metyMatys.Count > 0)
                {
                    foreach (var item in metyMatys)
                    {

                        V_HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                        DispenseMedyMatyADO dispenseMatyADO = new DispenseMedyMatyADO();
                        dispenseMatyADO.PreparationMediMatyTypeId = item.MATERIAL_TYPE_ID;
                        dispenseMatyADO.PreparationMediMatyTypeName = item.MATERIAL_TYPE_NAME;
                        dispenseMatyADO.Amount = item.MATERIAL_TYPE_AMOUNT;
                        dispenseMatyADO.CFGAmount = item.MATERIAL_TYPE_AMOUNT;
                        dispenseMatyADO.ServiceTypeId = materialType.SERVICE_TYPE_ID;
                        dispenseMatyADO.ProductAmount = item.PRODUCT_AMOUNT;
                        D_HIS_MEDI_STOCK_1 mediStock = mediStockD1SDOs.FirstOrDefault(o => o.SERVICE_TYPE_ID == materialType.SERVICE_TYPE_ID && o.ID == item.MATERIAL_TYPE_ID);
                        if (mediStock == null || mediStock.AMOUNT < item.MATERIAL_TYPE_AMOUNT)
                        {
                            dispenseMatyADO.IsNotAvaliable = true;
                        }
                        else
                        {
                            this.CapNhatKhaDungThuocChePham(item.MATERIAL_TYPE_ID, materialType.SERVICE_TYPE_ID, item.MATERIAL_TYPE_AMOUNT, PROCESS.TRU);
                        }



                        if (materialType != null)
                        {
                            dispenseMatyADO.ServiceUnitName = materialType.SERVICE_UNIT_NAME;
                        }
                        if (dispenseMetyMatyADOs == null)
                            dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                        dispenseMetyMatyADOs.Add(dispenseMatyADO);
                    }
                }
                #endregion

                #region Load Vat Tu theo cau hinh
                HisMetyMetyFilter metyMetyFilter = new HisMetyMetyFilter();
                List<MOS.EFMODEL.DataModels.V_HIS_METY_METY> metyMetysResult = new BackendAdapter(param)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_METY_METY>>("api/HisMetyMety/GetView", ApiConsumers.MosConsumer, metyMetyFilter, param);
                List<MOS.EFMODEL.DataModels.V_HIS_METY_METY> metyMetys = new List<V_HIS_METY_METY>();
                if (metyMetysResult != null && metyMetysResult.Count > 0)
                {
                    metyMetys = metyMetysResult.Where(o => o.METY_PRODUCT_ID == medicineTypeId).ToList();
                }
                List<V_HIS_MEDICINE_TYPE> medicineTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                if (metyMetys != null && metyMetys.Count > 0)
                {
                    foreach (var item in metyMetys)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = medicineTypes.FirstOrDefault(o => o.ID == item.PREPARATION_MEDICINE_TYPE_ID);
                        DispenseMedyMatyADO dispenseMatyADO = new DispenseMedyMatyADO();
                        dispenseMatyADO.PreparationMediMatyTypeId = item.PREPARATION_MEDICINE_TYPE_ID;
                        dispenseMatyADO.PreparationMediMatyTypeName = item.PREPARATION_MEDICINE_TYPE_NAME;
                        dispenseMatyADO.Amount = item.PREPARATION_AMOUNT;
                        dispenseMatyADO.CFGAmount = item.PREPARATION_AMOUNT;
                        dispenseMatyADO.ServiceTypeId = medicineType.SERVICE_TYPE_ID;
                        dispenseMatyADO.ProductAmount = item.PRODUCT_AMOUNT;
                        if (medicineType != null)
                        {
                            dispenseMatyADO.ServiceUnitName = medicineType.SERVICE_UNIT_NAME;
                        }
                        if (dispenseMetyMatyADOs == null)
                            dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                        dispenseMetyMatyADOs.Add(dispenseMatyADO);
                    }
                }
                #endregion

                gridControlDSChePham.DataSource = dispenseMetyMatyADOs;



            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadPriceVatFromMedicineType(long serviceId)
        {
            try
            {
                CommonParam param = new CommonParam();

                List<V_HIS_SERVICE_PATY> servicePatyTemps = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                if (servicePatyTemps != null)
                {
                    HIS_MEDICINE_TYPE medicineType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>()
                        .FirstOrDefault(o => o.SERVICE_ID == serviceId && o.IS_SALE_EQUAL_IMP_PRICE == 1);
                    if (medicineType != null)
                    {
                        foreach (var item in medicinePatyADOs)
                        {
                            item.Price = medicineType.IMP_PRICE;
                            item.Vat = medicineType.IMP_VAT_RATIO * 100;
                        }
                    }
                    else
                    {

                        List<V_HIS_SERVICE_PATY> servicePatys = servicePatyTemps.Where(o => o.SERVICE_ID == serviceId && o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()).ToList();
                        if (servicePatys != null)
                        {
                            foreach (var item in medicinePatyADOs)
                            {
                                V_HIS_SERVICE_PATY servicePaty = servicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == item.PatientTypeId);
                                if (servicePaty != null)
                                {
                                    item.Price = servicePaty.PRICE;
                                    item.Vat = servicePaty.VAT_RATIO * 100;
                                }
                                else
                                {
                                    item.Price = 0;
                                    item.Vat = null;
                                }
                            }
                        }
                    }

                    gridControlPaty.RefreshDataSource();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnAddRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                ResetControlAfterAdd(RESET.REFESH);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditDelete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                DispenseMedyMatyADO dispenseMatyADO = gridViewDSChePham.GetFocusedRow() as DispenseMedyMatyADO;
                List<DispenseMedyMatyADO> dispenseMatyADOTemps = gridControlDSChePham.DataSource as List<DispenseMedyMatyADO>;
                if (dispenseMatyADO != null)
                {
                    dispenseMetyMatyADOs.Remove(dispenseMatyADO);
                    dispenseMatyADOTemps.Remove(dispenseMatyADO);
                    gridControlDSChePham.RefreshDataSource();

                    this.CapNhatKhaDungThuocChePham(dispenseMatyADO.PreparationMediMatyTypeId, dispenseMatyADO.ServiceTypeId, dispenseMatyADO.Amount, PROCESS.CONG);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ReloadDataFromConfig(object data)
        {
            try
            {
                //if (data != null && data is List<HIS_METY_MATY>)
                //{
                //    List<HIS_METY_MATY> metyMatys = data as List<HIS_METY_MATY>;
                //    if (dispenseMetyADO == null)
                //        throw new Exception("Khong tim thay dispenseMetyADO");
                //    // Xoa du lieu cu

                //    dispenseMatyADOs = dispenseMatyADOs != null ? dispenseMatyADOs.Where(o => medicineTypeId != o.MedicineTypeId).ToList() : new List<DispenseMatyADO>();

                //    List<V_HIS_MATERIAL_TYPE> materialTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                //    if (metyMatys != null && metyMatys.Count > 0)
                //    {
                //        foreach (var item in metyMatys)
                //        {
                //            V_HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID);
                //            DispenseMatyADO dispenseMatyADO = new DispenseMatyADO();
                //            dispenseMatyADO.MaterialTypeId = item.MATERIAL_TYPE_ID;
                //            dispenseMatyADO.Amount = item.MATERIAL_TYPE_AMOUNT;
                //            dispenseMatyADO.MedicineTypeId = item.MEDICINE_TYPE_ID;
                //            dispenseMatyADO.MedicineTypeName = dispenseMetyADO.MedicineTypeName;
                //            dispenseMatyADO.CFGAmount = item.MATERIAL_TYPE_AMOUNT;
                //            dispenseMatyADO.Amount = item.MATERIAL_TYPE_AMOUNT * dispenseMetyADO.Amount;
                //            if (materialType != null)
                //            {
                //                dispenseMatyADO.ServiceUnitName = materialType.SERVICE_UNIT_NAME;
                //                dispenseMatyADO.MaterialTypeName = materialType.MATERIAL_TYPE_NAME;
                //            }
                //            if (dispenseMatyADOs == null)
                //                dispenseMatyADOs = new List<DispenseMatyADO>();
                //            dispenseMatyADOs.Add(dispenseMatyADO);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void gridViewDSChePham_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "Delete" || e.Column.FieldName == "Amount")
                    return;

                DispenseMedyMatyADO dispenseMatyADO = gridViewDSChePham.GetFocusedRow() as DispenseMedyMatyADO;
                if (dispenseMatyADO != null)
                {
                    //DispenseMetyADO dispenseMetyADO = dispenseMetyADO.FirstOrDefault(o => o.MedicineTypeId == dispenseMatyADO.MedicineTypeId);
                    //LoadDataControlFromDispenseMetyADO(dispenseMetyADO);
                    //LoadDataControlFromDispenseMatyADO(dispenseMatyADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(InitDataMetyMatyTypeInStockD1);
                methods.Add(LoadMetyProductADOs);
                methods.Add(LoadMetyProductADOs);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                WaitingManager.Hide();

                ResetControlAfterAdd(RESET.REFESH);
                gridControlDSChePham.DataSource = null;
                dispenseMetyMatyADOs = null;
                hisDispenseResultSDO = null;
                //this.EnableControlSave(true);
                this.action = ACTION.CREATE;
                dispenseId = null;
                gridControlImpExp.DataSource = null;
                LoadGridServicePaty();
                InitEnabledAction(ACTION.CREATE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinMetyAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    dtExpTime.Focus();
                    dtExpTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void dtExpTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtPackageNumber.Focus();
                    txtPackageNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPackageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtHeinDocumentNumber.Focus();
                    txtHeinDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtHeinDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    txtThuocChePham.Focus();
                    txtThuocChePham.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThuocChePham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    spinMateAmount.Focus();
                    spinMateAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThuocThanhPham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            //try
            //{
            //    if (e.KeyCode == Keys.Enter)
            //    {

            //        spinMetyAmount.Focus();
            //        spinMetyAmount.SelectAll();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}
        }

        private void txtThuocThanhPham_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtThuocThanhPham.Text) && txtThuocThanhPham.IsEditorActive)
                {
                    txtThuocThanhPham.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewThuocTP.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }
                         
                        //Filter data
                        gridViewThuocTP.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%'", txtThuocThanhPham.Text);
                        //OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%'

                        //+ " OR [MEDI_STOCK_NAME] Like '%" + txtMediMatyForPrescription.Text + "%'";
                        gridViewThuocTP.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewThuocTP.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewThuocTP.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewThuocTP.FocusedRowHandle = 0;
                        gridViewThuocTP.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewThuocTP.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtThuocThanhPham.Bounds.X + this.Bounds.X + 8, txtThuocThanhPham.Bounds.Y + this.Bounds.Y, txtThuocThanhPham.Bounds.Width - 90, txtThuocThanhPham.Bounds.Height);

                        if (isShow)
                        {
                            popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                            isShow = false;
                        }

                        txtThuocThanhPham.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewThuocTP.ActiveFilter.Clear();
                    gridViewThuocTP.FocusedRowHandle = 0;
                    gridViewThuocTP.Focus();
                    txtThuocThanhPham.Focus();
                    if (!isShowContainer)
                    {
                        popupControlContainer.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainer_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThuocThanhPham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if(MetyProductADO !=null && MetyProductADO.Count > 0)
					{
                        popupControlContainer.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridThanhPham_RowClick(MetyProductADO.First());
                    }                                       
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewThuocTP.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtThuocThanhPham.Bounds.X + this.Bounds.X + 8, txtThuocThanhPham.Bounds.Y + this.Bounds.Y, txtThuocThanhPham.Bounds.Width - 90, txtThuocThanhPham.Bounds.Height);
                    popupControlContainer.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    gridViewThuocTP.Focus();
                    gridViewThuocTP.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtThuocThanhPham.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlThuocTP_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MetyProductADO mediThanhPham = gridViewThuocTP.GetFocusedRow() as MetyProductADO;
                    if (mediThanhPham != null)
                    {
                        popupControlContainer.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridThanhPham_RowClick(mediThanhPham);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void txtThuocChePham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DMediStock1ADO matyChePham = gridViewThuocCP.GetFocusedRow() as DMediStock1ADO;
                    if (matyChePham != null)
                    {
                        popupControlContainerChePham.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridChePham_RowClick(matyChePham);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewThuocCP.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtThuocChePham.Bounds.X + this.Bounds.X + 8, txtThuocChePham.Bounds.Y + this.Bounds.Y, txtThuocChePham.Bounds.Width - 90, txtThuocChePham.Bounds.Height);
                    popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    gridViewThuocCP.Focus();
                    gridViewThuocCP.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtThuocChePham.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewThuocCP_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DMediStock1ADO matyChePham = gridViewThuocCP.GetFocusedRow() as DMediStock1ADO;
                    if (matyChePham != null)
                    {
                        popupControlContainerChePham.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridChePham_RowClick(matyChePham);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThuocChePham_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtThuocChePham.Text))
                {
                    txtThuocChePham.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewThuocCP.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        //Filter data
                        gridViewThuocCP.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME] Like '%{0}%'", txtThuocChePham.Text);
                        //OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%' OR [ACTIVE_INGR_BHYT_NAME__UNSIGN] Like '%{0}%'


                        gridViewThuocCP.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewThuocCP.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewThuocCP.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewThuocCP.FocusedRowHandle = 0;
                        gridViewThuocCP.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewThuocCP.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtThuocChePham.Bounds.X + this.Bounds.X + 8, txtThuocChePham.Bounds.Y + this.Bounds.Y, txtThuocChePham.Bounds.Width - 90, txtThuocChePham.Bounds.Height);

                        if (isShow)
                        {
                            popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                            isShow = false;
                        }

                        txtThuocChePham.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewThuocCP.ActiveFilter.Clear();
                    gridViewThuocCP.FocusedRowHandle = 0;
                    if (!isShowContainer)
                    {
                        popupControlContainerChePham.HidePopup();
                    }

                    currentMateChePham = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void popupControlContainerChePham_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave.Focus();
            btnSave_Click(null, null);
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnRefesh_Click(null, null);
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnBoSung.Enabled)
                {
                    btnBoSung_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void barButtonItem5_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled)
            {
                btnPrint_Click(null, null);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), Inventec.Desktop.Common.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(PrintTypeCodeWorker.PRINT_TYPE_CODE__BIEUMAU__BAO_CHE_THUOC__MPS000244, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlLeft == -1)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlLeft > edit.TabIndex)
                {
                    positionHandleControlLeft = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewImpExp_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImpExpADO impExpADO = (ImpExpADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (impExpADO != null)
                    {
                        if (e.Column.FieldName == "CreateTimeDisplay")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(impExpADO.CreateTime ?? 0);
                        }
                        else if (e.Column.FieldName == "LoaiPhieu")
                        {
                            if (impExpADO.IsImpMest)
                            {
                                e.Value = "Nhập";
                            }
                            else if (impExpADO.IsExpMest)
                            {
                                e.Value = "Xuất";
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDSChePham_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DispenseMedyMatyADO dispenseMedyMatyADO = (DispenseMedyMatyADO)gridViewDSChePham.GetRow(e.RowHandle);
                if (dispenseMedyMatyADO != null
                    && dispenseMedyMatyADO.ServiceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    e.Appearance.ForeColor = Color.Blue;
                else
                    e.Appearance.ForeColor = Color.Black;

                if (dispenseMedyMatyADO.IsNotAvaliable)
                    e.Appearance.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewDSChePham_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                DispenseMedyMatyADO dispenseMedyMatyADO = gridViewDSChePham.GetFocusedRow() as DispenseMedyMatyADO;
                if (dispenseMedyMatyADO != null)
                {
                    if (e.Column.FieldName == "Amount")
                    {
                        SpinEdit cbo = ((GridView)sender).ActiveEditor as SpinEdit;
                        if (cbo != null)
                        {
                            D_HIS_MEDI_STOCK_1 mediStock = this.mediStockD1SDOs.FirstOrDefault(o => o.SERVICE_TYPE_ID == dispenseMedyMatyADO.ServiceTypeId && o.ID == dispenseMedyMatyADO.PreparationMediMatyTypeId);

                            if (cbo.Value < 0)
                            {
                                dispenseMedyMatyADO.IsNotAvaliable = true;
                                return;
                            }

                            if (mediStock != null)
                            {
                                if (mediStock.AMOUNT < (cbo.Value - (dispenseMedyMatyADO.OldAmount ?? 0)))
                                {
                                    dispenseMedyMatyADO.IsNotAvaliable = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (dispenseMedyMatyADO.OldAmount.HasValue
                                    && cbo.Value <= dispenseMedyMatyADO.OldAmount)
                                {
                                    dispenseMedyMatyADO.IsNotAvaliable = false;
                                    return;
                                }
                                dispenseMedyMatyADO.IsNotAvaliable = true;
                                return;
                            }

                            dispenseMedyMatyADO.IsNotAvaliable = false;
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void spinMetyAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinMetyAmount.EditValue != null)
                {
                    if (dispenseMetyMatyADOs != null && dispenseMetyMatyADOs.Count > 0)
                    {
                        foreach (var item in dispenseMetyMatyADOs)
                        {
                            if (!item.CFGAmount.HasValue)
                            {
                                item.Amount = item.CFGAmount.Value * spinMetyAmount.Value;
                            }
                            else
                            {
                                if (item.ProductAmount.HasValue && item.ProductAmount.Value != 0)
                                {
                                    item.Amount = item.CFGAmount.Value * spinMetyAmount.Value / item.ProductAmount.Value;
                                }
                            }

                            D_HIS_MEDI_STOCK_1 mediStock = this.mediStockD1SDOs.FirstOrDefault(o => o.ID == item.PreparationMediMatyTypeId && o.SERVICE_TYPE_ID == item.ServiceTypeId);

                            if (item.Amount < 0)
                            {
                                item.IsNotAvaliable = true;
                                continue;
                            }

                            if (mediStock != null)
                            {
                                if (mediStock.AMOUNT < (item.Amount - (item.OldAmount ?? 0)))
                                {
                                    item.IsNotAvaliable = true;
                                    continue;
                                }
                            }
                            else
                            {
                                if (item.OldAmount.HasValue && item.Amount <= item.OldAmount)
                                {
                                    item.IsNotAvaliable = false;
                                    continue;
                                }
                                item.IsNotAvaliable = true;
                                continue;
                            }
                            item.IsNotAvaliable = false;
                            continue;
                        }
                        gridControlDSChePham.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPaty_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                MedicinePatyADO medicinePatyADO = gridViewPaty.GetFocusedRow() as MedicinePatyADO;
                if (medicinePatyADO != null)
                {
                    if (e.Column.FieldName == "Price")
                    {
                        SpinEdit cbo = ((GridView)sender).ActiveEditor as SpinEdit;
                        if (cbo != null)
                        {
                            if (cbo.Value < 0)
                            {
                                MessageBox.Show("Giá bán không được âm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
                                cbo.EditValue = cbo.OldEditValue;
                                return;
                            }
                        }
                    }

                    if (e.Column.FieldName == "Vat")
                    {
                        SpinEdit cbo = ((GridView)sender).ActiveEditor as SpinEdit;
                        if (cbo != null)
                        {
                            if (cbo.Value < 0)
                            {
                                MessageBox.Show("VAT không được âm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning); ;
                                cbo.EditValue = cbo.OldEditValue;
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void gridViewDSChePham_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "Amount")
                {
                    var index = this.gridViewDSChePham.GetDataSourceRowIndex(e.RowHandle);
                    if (index < 0)
                    {
                        e.Info.ErrorType = ErrorType.None;
                        e.Info.ErrorText = "";
                        return;
                    }
                    var listDatas = this.gridControlDSChePham.DataSource as List<DispenseMedyMatyADO>;
                    var row = listDatas[index];
                    if (row.Amount <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Sai định dạng số lượng";
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnDinhMuc_Click(object sender, EventArgs e)
        {
            try
            {
                this.CauHinhDinhMuc();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPaty_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MedicinePatyADO medicinePatyADO = (MedicinePatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (medicinePatyADO != null)
                    {
                        if (e.Column.FieldName == "Price")
                        {

                            e.Value = Inventec.Common.Number.Convert.NumberToString(medicinePatyADO.Price ?? 0, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
