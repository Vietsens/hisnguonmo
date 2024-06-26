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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.PackingMaterial.ADO;
using HIS.Desktop.Plugins.PackingMaterial.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.RichEditor;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.LocalStorage.Location;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PackingMaterial
{
    public partial class frmPackingMaterial : FormBase
    {
        private int positionHandleControl = 1;

        private HIS_MEDI_STOCK mediStock = null;


        private List<MaterialTypeADO> listMaterialTypeADO = new List<MaterialTypeADO>();
        private List<V_HIS_MATERIAL_TYPE> listRawMaterialType = new List<V_HIS_MATERIAL_TYPE>();
        private List<D_HIS_MEDI_STOCK_1> listDHisMediStock = new List<D_HIS_MEDI_STOCK_1>();
        private List<MaterialPatyADO> materialPatyADO = new List<MaterialPatyADO>();

        bool isShowContainer = false;
        bool isShowContainerForChoose = false;
        bool isShow = true;
        bool isShowMessUpdate = false;

        private HIS_DISPENSE dispense = null;

        private MaterialTypeADO currentTP = null;
        private List<PackingMatyMatyADO> packingMatyADOs = new List<PackingMatyMatyADO>();
        private DMediStock1ADO currentCP = null;
        private HisPackingResultSDO resultSDO = null;

        public frmPackingMaterial(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        public frmPackingMaterial(Inventec.Desktop.Common.Modules.Module module, HIS_DISPENSE editData)
            : base(module)
        {
            InitializeComponent();
            this.dispense = editData;
        }

        private void frmPackingMaterial_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControlThanhPham();
                this.ValidControlChePham();
                this.LoadMediStockFromRoomId();
                this.LoadDataDHisMediStock();
                this.LoadMaterialTypeCombo();
                this.LoadDataToControl();
                this.LoadDispenseEdit();
                btnPrint.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlThanhPham()
        {
            try
            {
                ControlEditValidationRule valiRule = new ControlEditValidationRule();
                valiRule.editor = txtThanhPham;
                valiRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc); ;
                valiRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(txtThanhPham, valiRule);

                AmountValidationRule valiRuleAmount = new AmountValidationRule();
                valiRuleAmount.spinAmount = spinTPAmount;
                this.dxValidationProvider1.SetValidationRule(spinTPAmount, valiRuleAmount);

                ExpDateValidationRule dateRule = new ExpDateValidationRule();
                dateRule.dtExpDate = dtExpTime;
                this.dxValidationProvider1.SetValidationRule(dtExpTime, dateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlChePham()
        {
            try
            {
                ControlEditValidationRule valiRule = new ControlEditValidationRule();
                valiRule.editor = txtChePham;
                valiRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc); ;
                valiRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider2.SetValidationRule(txtChePham, valiRule);

                AmountValidationRule valiRuleAmount = new AmountValidationRule();
                valiRuleAmount.spinAmount = spinCPAmount;
                this.dxValidationProvider2.SetValidationRule(spinCPAmount, valiRuleAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMediStockFromRoomId()
        {
            try
            {

                CommonParam param = new CommonParam();
                this.mediStock = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                if (this.mediStock == null)
                    throw new Exception("mediStock is null");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultControl()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataDHisMediStock()
        {
            try
            {
                if (this.mediStock != null)
                {
                    CommonParam param = new CommonParam();
                    DHisMediStock1Filter filter = new DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = new List<long> { this.mediStock.ID };

                    this.listDHisMediStock = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param)
                        .ToList();
                }
                else
                {
                    this.listDHisMediStock = new List<D_HIS_MEDI_STOCK_1>();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialTypeCombo()
        {
            try
            {
                if (this.mediStock == null)
                {
                    throw new Exception("Khong tim thay thong tin kho");
                }

                if ((this.mediStock.IS_BUSINESS ?? 0) == 1)
                {
                    var listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => (o.IS_BUSINESS ?? 0) == 1 && (o.IS_RAW_MATERIAL ?? 0) != 1).ToList();
                    this.listRawMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => (o.IS_BUSINESS ?? 0) == 1 && (o.IS_RAW_MATERIAL ?? 0) == 1).ToList();
                    if (listMaterialType != null)
                    {
                        this.listMaterialTypeADO = new List<MaterialTypeADO>();
                        Mapper.CreateMap<V_HIS_MATERIAL_TYPE, MaterialTypeADO>();
                        foreach (V_HIS_MATERIAL_TYPE item in listMaterialType)
                        {
                            MaterialTypeADO ado = Mapper.Map<MaterialTypeADO>(item);
                            ado.MATERIAL_TYPE_CODE__UNSIGN = convertToUnSign3(ado.MATERIAL_TYPE_CODE) + ado.MATERIAL_TYPE_CODE;
                            ado.MATERIAL_TYPE_NAME__UNSIGN = convertToUnSign3(ado.MATERIAL_TYPE_NAME) + ado.MATERIAL_TYPE_NAME;
                            this.listMaterialTypeADO.Add(ado);
                        }
                    }
                }
                else
                {
                    var listMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => (o.IS_BUSINESS ?? 0) != 1 && (o.IS_RAW_MATERIAL ?? 0) != 1).ToList();
                    this.listRawMaterialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => (o.IS_BUSINESS ?? 0) != 1 && (o.IS_RAW_MATERIAL ?? 0) == 1).ToList();
                    if (listMaterialType != null)
                    {
                        this.listMaterialTypeADO = new List<MaterialTypeADO>();
                        Mapper.CreateMap<V_HIS_MATERIAL_TYPE, MaterialTypeADO>();
                        foreach (V_HIS_MATERIAL_TYPE item in listMaterialType)
                        {
                            MaterialTypeADO ado = Mapper.Map<MaterialTypeADO>(item);
                            ado.MATERIAL_TYPE_CODE__UNSIGN = convertToUnSign3(ado.MATERIAL_TYPE_CODE) + ado.MATERIAL_TYPE_CODE;
                            ado.MATERIAL_TYPE_NAME__UNSIGN = convertToUnSign3(ado.MATERIAL_TYPE_NAME) + ado.MATERIAL_TYPE_NAME;
                            this.listMaterialTypeADO.Add(ado);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                txtThanhPham.Focus();
                spinTPAmount.EditValue = null;
                spinCPAmount.EditValue = null;
                if (this.mediStock != null)
                {
                    txtMediStockName.Text = this.mediStock.MEDI_STOCK_NAME;
                }
                RebuildControlContainerThanhPham();
                RebuildControlContainerChePham();
                LoadGridServicePaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildControlContainerThanhPham()
        {
            try
            {
                gridViewPopupThanhPham.BeginUpdate();
                gridViewPopupThanhPham.Columns.Clear();
                popupControlContainerThanhPham.Width = 900;
                popupControlContainerThanhPham.Height = 130;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MATERIAL_TYPE_NAME";
                col1.Caption = "Tên vật tư";
                col1.Width = 150;
                col1.VisibleIndex = 1;
                gridViewPopupThanhPham.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MATERIAL_TYPE_CODE";
                col2.Caption = "Mã vật tư";
                col2.Width = 80;
                col2.VisibleIndex = 2;
                gridViewPopupThanhPham.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewPopupThanhPham.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 80;
                col7.VisibleIndex = 7;
                gridViewPopupThanhPham.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà cung cấp";
                col8.Width = 150;
                col8.VisibleIndex = 8;
                gridViewPopupThanhPham.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Tên nước";
                col9.Width = 80;
                col9.VisibleIndex = 9;
                gridViewPopupThanhPham.Columns.Add(col9);

                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MATERIAL_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewPopupThanhPham.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MATERIAL_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewPopupThanhPham.Columns.Add(col11);

                gridViewPopupThanhPham.GridControl.DataSource = this.listMaterialTypeADO;
                gridViewPopupThanhPham.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildControlContainerChePham()
        {
            try
            {
                gridViewPopupChePham.BeginUpdate();
                gridViewPopupChePham.Columns.Clear();
                popupControlContainerChePham.Width = 900;
                popupControlContainerChePham.Height = 130;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = "Tên vật tư";
                col1.Width = 150;
                col1.VisibleIndex = 1;
                gridViewPopupChePham.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = "Mã vật tư";
                col2.Width = 80;
                col2.VisibleIndex = 2;
                gridViewPopupChePham.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "AMOUNT";
                col3.Caption = "Số lượng";
                col3.Width = 80;
                col3.VisibleIndex = 3;
                gridViewPopupChePham.Columns.Add(col3);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewPopupChePham.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 80;
                col7.VisibleIndex = 7;
                gridViewPopupChePham.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà cung cấp";
                col8.Width = 150;
                col8.VisibleIndex = 8;
                gridViewPopupChePham.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Tên nước";
                col9.Width = 80;
                col9.VisibleIndex = 9;
                gridViewPopupChePham.Columns.Add(col9);
                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewPopupChePham.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewPopupChePham.Columns.Add(col11);

                List<long> materialTypeIsRawIDs = this.listRawMaterialType != null ? this.listRawMaterialType.Select(o => o.ID).ToList() : new List<long>();

                var thisMateInStocks = this.listDHisMediStock != null ? this.listDHisMediStock.Where(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && materialTypeIsRawIDs.Contains(o.ID ?? 0)).ToList() : null;

                gridViewPopupChePham.GridControl.DataSource = ConvertToDMediStock1(thisMateInStocks);
                gridViewPopupChePham.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<DMediStock1ADO> ConvertToDMediStock1(List<D_HIS_MEDI_STOCK_1> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    Mapper.CreateMap<D_HIS_MEDI_STOCK_1, DMediStock1ADO>();
                    foreach (var item in listMediStock)
                    {
                        DMediStock1ADO ado = Mapper.Map<DMediStock1ADO>(item);
                        ado.MEDICINE_TYPE_CODE__UNSIGN = convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                        ado.MEDICINE_TYPE_NAME__UNSIGN = convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                        result.Add(ado);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void LoadGridServicePaty()
        {
            try
            {
                List<HIS_PATIENT_TYPE> patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes == null)
                {
                    throw new Exception("Khong tim thay danh sach doi tuong thanh toan");
                }

                materialPatyADO = new List<MaterialPatyADO>();
                foreach (var item in patientTypes)
                {
                    MaterialPatyADO servicePatyADO = new MaterialPatyADO();
                    servicePatyADO.PATIENT_TYPE_ID = item.ID;
                    servicePatyADO.PRICE = 0;
                    servicePatyADO.PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME;
                    materialPatyADO.Add(servicePatyADO);
                }
                gridControlMaterialPaty.BeginUpdate();
                gridControlMaterialPaty.DataSource = materialPatyADO;
                gridControlMaterialPaty.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDispenseEdit()
        {
            try
            {
                if (this.dispense != null)
                {
                    if (this.dispense.MEDI_STOCK_ID != this.mediStock.ID)
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show("Phiếu đóng gói không thuộc kho người dùng đang làm việc", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                        return;
                    }
                    CommonParam param = new CommonParam();
                    HisExpMestFilter expMestFilter = new HisExpMestFilter();
                    expMestFilter.DISPENSE_ID = this.dispense.ID;
                    List<HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<HIS_EXP_MEST>>("api/HisExpMest/Get", ApiConsumers.MosConsumer, expMestFilter, param);

                    if (expMests == null || expMests.Count == 0)
                    {
                        throw new Exception("Khong tim thay expMests");
                    }

                    HisImpMestFilter impMestFilter = new HisImpMestFilter();
                    impMestFilter.DISPENSE_ID = this.dispense.ID;
                    List<HIS_IMP_MEST> impMests = new BackendAdapter(param).Get<List<HIS_IMP_MEST>>("api/HisImpMest/Get", ApiConsumers.MosConsumer, impMestFilter, param);

                    if (impMests == null || impMests.Count == 0)
                    {
                        throw new Exception("Khong tim thay impMests");
                    }
                    LoadImpMestToGrid(impMests, expMests);
                    LoadTPBuImpMest(impMests.FirstOrDefault());
                    LoadCPByExpMest(expMests.FirstOrDefault());
                    btnNew.Enabled = false;
                    txtThanhPham.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadImpMestToGrid(List<HIS_IMP_MEST> impMests, List<HIS_EXP_MEST> expMests)
        {
            try
            {
                List<ExpImpMestADO> listImpExpADOs = new List<ExpImpMestADO>();
                if (impMests != null && impMests.Count > 0)
                {
                    foreach (var item in impMests)
                    {
                        ExpImpMestADO impExp = new ExpImpMestADO(item);
                        listImpExpADOs.Add(impExp);
                    }
                }
                if (expMests != null && expMests.Count > 0)
                {
                    foreach (var item in expMests)
                    {
                        ExpImpMestADO impExp = new ExpImpMestADO(item);
                        listImpExpADOs.Add(impExp);
                    }
                }
                gridControlExpImpMest.BeginUpdate();
                gridControlExpImpMest.DataSource = listImpExpADOs;
                gridControlExpImpMest.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadTPBuImpMest(HIS_IMP_MEST impMest)
        {
            if (impMest != null)
            {
                CommonParam param = new CommonParam();
                HisImpMestMaterialView4Filter filter = new HisImpMestMaterialView4Filter();
                filter.IMP_MEST_ID = impMest.ID;
                var impMestMaterials = new BackendAdapter(param).Get<List<V_HIS_IMP_MEST_MATERIAL_4>>("api/HisImpMestMaterial/GetView4", ApiConsumers.MosConsumer, filter, param);

                if (impMestMaterials == null || impMestMaterials.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Không tìm thấy danh sách vật tư thành phẩm");
                    return;
                }
                var material = impMestMaterials.FirstOrDefault();

                txtThanhPham.Text = material.MATERIAL_TYPE_NAME;
                this.currentTP = new MaterialTypeADO();
                this.currentTP.ID = material.MATERIAL_TYPE_ID;
                spinTPAmount.Value = material.AMOUNT;
                HIS_SERVICE_UNIT serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == material.TDL_SERVICE_UNIT_ID);
                lblTpServiceUnitName.Text = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "";

                txtPackageNumber.Text = material.PACKAGE_NUMBER;
                if (material.EXPIRED_DATE.HasValue)
                {
                    dtExpTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(material.EXPIRED_DATE ?? 0) ?? DateTime.Now;
                }

                if (this.mediStock != null)
                {
                    txtMediStockName.Text = this.mediStock.MEDI_STOCK_NAME;
                }


                HisMaterialFilter mateFilter = new HisMaterialFilter();
                mateFilter.ID = material.MATERIAL_ID;
                HIS_MATERIAL mate = new BackendAdapter(param).Get<List<HIS_MATERIAL>>("api/HisMaterial/Get", ApiConsumers.MosConsumer, mateFilter, param).FirstOrDefault();
                if (mate != null)
                {
                    txtDocumentNumber.Text = mate.TDL_BID_NUMBER ?? "";
                }
                LoadMaterialPaty(material.MATERIAL_ID, null);
            }

        }

        private void LoadCPByExpMest(HIS_EXP_MEST expMest)
        {
            try
            {
                if (expMest != null)
                {
                    packingMatyADOs = new List<PackingMatyMatyADO>();

                    CommonParam param = new CommonParam();

                    HisExpMestMaterialView2Filter mateFilter = new HisExpMestMaterialView2Filter();
                    mateFilter.EXP_MEST_ID = expMest.ID;
                    List<V_HIS_EXP_MEST_MATERIAL_2> expMestMaterials = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST_MATERIAL_2>>("api/HisExpMestMaterial/GetView2", ApiConsumers.MosConsumer, mateFilter, param);

                    if (expMestMaterials != null && expMestMaterials.Count > 0)
                    {
                        foreach (var item in expMestMaterials)
                        {
                            PackingMatyMatyADO ado = new PackingMatyMatyADO();
                            ado.AMOUNT = item.AMOUNT;
                            ado.OLD_AMOUNT = item.AMOUNT;
                            ado.PREPA_MATERIAL_TYPE_ID = item.TDL_MATERIAL_TYPE_ID.Value;
                            ado.MATERIAL_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                            ado.MATERIAL_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                            HIS_SERVICE_UNIT serviceUnit = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == item.TDL_SERVICE_UNIT_ID);
                            ado.SERVICE_UNIT_NAME = serviceUnit != null ? serviceUnit.SERVICE_UNIT_NAME : "";
                            packingMatyADOs.Add(ado);
                        }
                    }
                    gridControlPrepa.BeginUpdate();
                    gridControlPrepa.DataSource = packingMatyADOs;
                    gridControlPrepa.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMaterialPaty(long materialId, List<HIS_MATERIAL_PATY> patys)
        {
            try
            {
                if (patys != null && patys.Count > 0)
                {
                    var patientTypes = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                    foreach (var item in patys)
                    {
                        var ado = materialPatyADO.FirstOrDefault(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID);
                        if (ado != null)
                        {
                            ado.PRICE = item.EXP_PRICE;
                            ado.VAT = item.EXP_VAT_RATIO * 100;
                            ado.ID = item.ID;
                            HIS_PATIENT_TYPE patientType = patientTypes.FirstOrDefault(o => o.ID == item.PATIENT_TYPE_ID);
                            ado.PATIENT_TYPE_NAME = patientType.PATIENT_TYPE_NAME;
                        }
                    }
                }
                else
                {
                    CommonParam param = new CommonParam();
                    HisMaterialPatyViewFilter filter = new HisMaterialPatyViewFilter();
                    filter.MATERIAL_ID = materialId;
                    List<V_HIS_MATERIAL_PATY> medicinePatys = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_PATY>>("api/HisMaterialPaty/GetView", ApiConsumers.MosConsumer, filter, param);

                    if (medicinePatys != null && medicinePatys.Count > 0 && materialPatyADO != null && materialPatyADO.Count > 0)
                    {
                        foreach (var item in medicinePatys)
                        {
                            var ado = materialPatyADO.FirstOrDefault(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID);
                            ado.PATIENT_TYPE_NAME = item.PATIENT_TYPE_NAME;
                            ado.PRICE = item.EXP_PRICE;
                            ado.VAT = item.EXP_VAT_RATIO * 100;
                            ado.ID = item.ID;
                            ado.MATERIAL_ID = item.MATERIAL_ID;
                        }
                    }
                }
                gridControlMaterialPaty.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThanhPham_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == ButtonPredefines.DropDown)
            {
                isShowContainer = !isShowContainer;
                if (isShowContainer)
                {
                    Rectangle buttonBounds = new Rectangle(txtThanhPham.Bounds.X + this.Bounds.X + 8, txtThanhPham.Bounds.Y + this.Bounds.Y, txtThanhPham.Bounds.Width - 90, txtThanhPham.Bounds.Height);
                    popupControlContainerThanhPham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                }
            }
        }

        private void txtThanhPham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MedicineMaterialTypeComboADO mediThanhPham = gridViewPopupThanhPham.GetFocusedRow() as MedicineMaterialTypeComboADO;
                    if (mediThanhPham != null)
                    {
                        popupControlContainerThanhPham.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridThanhPham_RowClick(mediThanhPham);
                    }
                }
                else if (e.KeyCode == Keys.Down)
                {
                    int rowHandlerNext = 0;
                    int countInGridRows = gridViewPopupThanhPham.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtThanhPham.Bounds.X + this.Bounds.X + 8, txtThanhPham.Bounds.Y + this.Bounds.Y, txtThanhPham.Bounds.Width - 90, txtThanhPham.Bounds.Height);
                    popupControlContainerThanhPham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    gridViewPopupThanhPham.Focus();
                    gridViewPopupThanhPham.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtThanhPham.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThanhPham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {

                    spinTPAmount.Focus();
                    spinTPAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtThanhPham_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtThanhPham.Text) && txtThanhPham.IsEditorActive)
                {
                    txtThanhPham.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewPopupThanhPham.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        gridViewPopupThanhPham.ActiveFilterString = String.Format("[MATERIAL_TYPE_NAME] Like '%{0}%' OR [MATERIAL_TYPE_CODE] Like '%{0}%' OR [MATERIAL_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MATERIAL_TYPE_CODE__UNSIGN] Like '%{0}%'", txtThanhPham.Text);

                        gridViewPopupThanhPham.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewPopupThanhPham.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewPopupThanhPham.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewPopupThanhPham.FocusedRowHandle = 0;
                        gridViewPopupThanhPham.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewPopupThanhPham.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtThanhPham.Bounds.X + this.Bounds.X + 8, txtThanhPham.Bounds.Y + this.Bounds.Y, txtThanhPham.Bounds.Width - 90, txtThanhPham.Bounds.Height);

                        if (isShow)
                        {
                            popupControlContainerThanhPham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                            isShow = false;
                        }

                        txtThanhPham.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewPopupThanhPham.ActiveFilter.Clear();
                    if (!isShowContainer)
                    {
                        popupControlContainerThanhPham.HidePopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTPAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinTPAmount.EditValue != null)
                {
                    if (this.packingMatyADOs != null && this.packingMatyADOs.Count > 0)
                    {
                        foreach (var item in this.packingMatyADOs)
                        {
                            if (item.CFG_AMOUNT.HasValue)
                            {
                                item.AMOUNT = item.CFG_AMOUNT.Value * spinTPAmount.Value;
                            }

                            D_HIS_MEDI_STOCK_1 mediStock = this.listDHisMediStock.FirstOrDefault(o => o.ID == item.PREPA_MATERIAL_TYPE_ID && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);

                            if (item.AMOUNT < 0)
                            {
                                item.IsNotAvaliable = true;
                                continue;
                            }

                            if (mediStock != null)
                            {
                                if (mediStock.AMOUNT < (item.AMOUNT - (item.OLD_AMOUNT ?? 0)))
                                {
                                    item.IsNotAvaliable = true;
                                    continue;
                                }
                            }
                            else
                            {
                                if (item.OLD_AMOUNT.HasValue && item.AMOUNT <= item.OLD_AMOUNT)
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
                        gridControlPrepa.RefreshDataSource();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinTPAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPackageNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDocumentNumber.Focus();
                    txtDocumentNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDocumentNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtChePham.Focus();
                    txtChePham.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtChePham_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.DropDown)
                {
                    isShowContainer = !isShowContainer;
                    if (isShowContainer)
                    {
                        Rectangle buttonBounds = new Rectangle(txtChePham.Bounds.X + this.Bounds.X + 8, txtChePham.Bounds.Y + this.Bounds.Y, txtChePham.Bounds.Width - 90, txtChePham.Bounds.Height);
                        popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChePham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DMediStock1ADO matyChePham = gridViewPopupChePham.GetFocusedRow() as DMediStock1ADO;
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
                    int countInGridRows = gridViewPopupChePham.RowCount;
                    if (countInGridRows > 1)
                    {
                        rowHandlerNext = 1;
                    }

                    Rectangle buttonBounds = new Rectangle(txtChePham.Bounds.X + this.Bounds.X + 8, txtChePham.Bounds.Y + this.Bounds.Y, txtChePham.Bounds.Width - 90, txtChePham.Bounds.Height);
                    popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                    gridViewPopupChePham.Focus();
                    gridViewPopupChePham.FocusedRowHandle = rowHandlerNext;
                }
                else if (e.Control && e.KeyCode == Keys.A)
                {
                    txtChePham.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChePham_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinCPAmount.Focus();
                    spinCPAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtChePham_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(txtChePham.Text))
                {
                    txtChePham.Refresh();
                    if (isShowContainerForChoose)
                    {
                        gridViewPopupChePham.ActiveFilter.Clear();
                    }
                    else
                    {
                        if (!isShowContainer)
                        {
                            isShowContainer = true;
                        }

                        //Filter data
                        gridViewPopupChePham.ActiveFilterString = String.Format("[MEDICINE_TYPE_NAME] Like '%{0}%' OR [MEDICINE_TYPE_CODE] Like '%{0}%' OR [MEDICINE_TYPE_NAME__UNSIGN] Like '%{0}%' OR [MEDICINE_TYPE_CODE__UNSIGN] Like '%{0}%'", txtChePham.Text);

                        gridViewPopupChePham.OptionsFilter.FilterEditorUseMenuForOperandsAndOperators = false;
                        gridViewPopupChePham.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
                        gridViewPopupChePham.OptionsFilter.ShowAllTableValuesInFilterPopup = false;
                        gridViewPopupChePham.FocusedRowHandle = 0;
                        gridViewPopupChePham.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
                        gridViewPopupChePham.OptionsFind.HighlightFindResults = true;

                        Rectangle buttonBounds = new Rectangle(txtChePham.Bounds.X + this.Bounds.X + 8, txtChePham.Bounds.Y + this.Bounds.Y, txtChePham.Bounds.Width - 90, txtChePham.Bounds.Height);

                        if (isShow)
                        {
                            popupControlContainerChePham.ShowPopup(new Point(buttonBounds.X, buttonBounds.Bottom + 30));
                            isShow = false;
                        }

                        txtChePham.Focus();
                    }
                    isShowContainerForChoose = false;
                }
                else
                {
                    gridViewPopupChePham.ActiveFilter.Clear();
                    if (!isShowContainer)
                    {
                        popupControlContainerChePham.HidePopup();
                    }

                    this.currentCP = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinCPAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void gridViewPrepa_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                PackingMatyMatyADO row = gridViewPrepa.GetFocusedRow() as PackingMatyMatyADO;
                if (row != null)
                {
                    if (e.Column.FieldName == "AMOUNT")
                    {
                        SpinEdit cbo = ((GridView)sender).ActiveEditor as SpinEdit;
                        if (cbo != null)
                        {
                            D_HIS_MEDI_STOCK_1 mediStock = this.listDHisMediStock.FirstOrDefault(o => o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT && o.ID == row.PREPA_MATERIAL_TYPE_ID);

                            if (cbo.Value < 0)
                            {
                                row.IsNotAvaliable = true;
                                return;
                            }

                            if (mediStock != null)
                            {
                                if (mediStock.AMOUNT < (cbo.Value - (row.OLD_AMOUNT ?? 0)))
                                {
                                    row.IsNotAvaliable = true;
                                    return;
                                }
                            }
                            else
                            {
                                if (row.OLD_AMOUNT.HasValue && cbo.Value <= row.OLD_AMOUNT)
                                {
                                    row.IsNotAvaliable = false;
                                    return;
                                }
                                row.IsNotAvaliable = true;
                                return;
                            }

                            row.IsNotAvaliable = false;
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

        private void gridViewPrepa_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT")
                {
                    var index = this.gridViewPrepa.GetDataSourceRowIndex(e.RowHandle);
                    if (index < 0)
                    {
                        e.Info.ErrorType = ErrorType.None;
                        e.Info.ErrorText = "";
                        return;
                    }
                    var listDatas = this.gridViewPrepa.DataSource as List<PackingMatyMatyADO>;
                    var row = listDatas[index];
                    if (row.AMOUNT <= 0)
                    {
                        e.Info.ErrorType = ErrorType.Warning;
                        e.Info.ErrorText = "Số lượng nhỏ hơn hoặc bằng 0";
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepa_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                PackingMatyMatyADO row = (PackingMatyMatyADO)gridViewPrepa.GetRow(e.RowHandle);
                if (row != null && row.IsNotAvaliable)
                    e.Appearance.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMaterialPaty_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnAdd.Enabled || this.currentTP == null || this.currentCP == null || !dxValidationProvider2.Validate()) return;
                if (this.packingMatyADOs == null) this.packingMatyADOs = new List<PackingMatyMatyADO>();

                PackingMatyMatyADO eixstAdo = this.packingMatyADOs.FirstOrDefault(o => o.PREPA_MATERIAL_TYPE_ID == this.currentCP.ID);

                if (eixstAdo != null)
                {
                    MessageBox.Show("Tồn tại trong danh sách chế phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Check kha dung
                var stockD1 = listDHisMediStock.FirstOrDefault(o => o.ID == this.currentCP.ID && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT);
                if (stockD1.AMOUNT < spinCPAmount.Value)
                {
                    MessageBox.Show("Số lượng chế phẩm không đủ khả dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                //Thêm chế phẩm
                eixstAdo = new PackingMatyMatyADO();
                eixstAdo.PREPA_MATERIAL_TYPE_ID = this.currentCP.ID ?? 0;
                eixstAdo.MATERIAL_TYPE_NAME = this.currentCP.MEDICINE_TYPE_NAME;
                eixstAdo.MATERIAL_TYPE_CODE = this.currentCP.MEDICINE_TYPE_CODE;
                eixstAdo.AMOUNT = spinCPAmount.Value;
                eixstAdo.SERVICE_UNIT_NAME = this.currentCP.SERVICE_UNIT_NAME;
                packingMatyADOs.Add(eixstAdo);
                gridControlPrepa.RefreshDataSource();

                this.ResetControlAfterAdd(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDinhMuc_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnDinhMuc.Enabled) return;
                if (this.currentTP == null)
                {
                    MessageBox.Show("Vui lòng chọn thành phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<HIS_MATY_MATY> matyMatys = new List<HIS_MATY_MATY>();
                if (packingMatyADOs != null && packingMatyADOs.Count > 0)
                {
                    foreach (var item in packingMatyADOs)
                    {
                        HIS_MATY_MATY metyMaty = new HIS_MATY_MATY();
                        metyMaty.MATERIAL_TYPE_ID = this.currentTP.ID;
                        metyMaty.PREPARATION_MATERIAL_TYPE_ID = item.PREPA_MATERIAL_TYPE_ID;
                        metyMaty.PREPARATION_AMOUNT = spinTPAmount.EditValue == null || spinTPAmount.Value == 0 ? item.AMOUNT : item.AMOUNT / spinTPAmount.Value;
                        matyMatys.Add(metyMaty);
                    }
                }
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MatyMaty").FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = HIS.Desktop.Plugins.MatyMaty");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(matyMatys);
                    listArgs.Add(this.currentTP.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).Show();
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
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.currentTP == null) return;
                if (!Check())
                    return;
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                HisPackingResultSDO rs = null;
                if (this.dispense == null)
                {
                    HisPackingCreateSDO createSDO = new HisPackingCreateSDO();
                    this.ProcessDataToCreate(ref createSDO);
                    rs = new BackendAdapter(param).Post<HisPackingResultSDO>("api/HisDispense/PackingCreate", ApiConsumers.MosConsumer, createSDO, param);
                }
                else
                {
                    if (isShowMessUpdate)
                    {
                        WaitingManager.Hide();
                        DialogResult myResult;
                        myResult = MessageBox.Show("bạn có chắc muốn sửa phiếu bào chế hay không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                        if (myResult != DialogResult.OK)
                        {
                            return;
                        }
                        WaitingManager.Show();
                    }
                    HisPackingUpdateSDO updateSDO = new HisPackingUpdateSDO();
                    this.ProcessDataToUpdate(ref updateSDO);
                    rs = new BackendAdapter(param).Post<HisPackingResultSDO>("api/HisDispense/PackingUpdate", ApiConsumers.MosConsumer, updateSDO, param);
                }
                if (rs != null)
                {
                    success = true;
                    this.resultSDO = rs;
                    isShowMessUpdate = true;
                    txtThanhPham.Enabled = false;
                    success = true;
                    this.dispense = rs.HisDispense;
                    LoadImpMestToGrid(new List<HIS_IMP_MEST> { rs.HisImpMest }, new List<HIS_EXP_MEST> { rs.HisExpMest });
                    LoadMaterialPaty(0, rs.MaterialPaties);
                    btnPrint.Enabled = true;
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

        private void ProcessDataToCreate(ref HisPackingCreateSDO createSDO)
        {
            try
            {
                createSDO.RequestRoomId = this.currentModuleBase.RoomId;
                createSDO.MediStockId = this.mediStock.ID;
                createSDO.Amount = spinTPAmount.Value;
                createSDO.DispenseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (dtExpTime.EditValue != null)
                    createSDO.ExpiredDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpTime.DateTime);
                createSDO.HeinDocumentNumber = txtDocumentNumber.Text;
                createSDO.MaterialTypeId = this.currentTP.ID;
                createSDO.PackageNumber = txtPackageNumber.Text;

                foreach (var item in packingMatyADOs)
                {
                    if (createSDO.MaterialTypes == null) createSDO.MaterialTypes = new List<HisPackingMatySDO>();
                    HisPackingMatySDO sdo = new HisPackingMatySDO();
                    sdo.Amount = item.AMOUNT;
                    sdo.MaterialTypeId = item.PREPA_MATERIAL_TYPE_ID;
                    createSDO.MaterialTypes.Add(sdo);
                }
                createSDO.MaterialPaties = new List<HIS_MATERIAL_PATY>();
                List<MaterialPatyADO> patys = materialPatyADO.Where(o => o.PRICE.HasValue).ToList();

                foreach (var item in patys)
                {
                    HIS_MATERIAL_PATY matePaty = new HIS_MATERIAL_PATY();
                    matePaty.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                    matePaty.EXP_PRICE = item.PRICE ?? 0;
                    if (item.VAT.HasValue)
                    {
                        matePaty.EXP_VAT_RATIO = (item.VAT ?? 0) / 100;
                    }

                    createSDO.MaterialPaties.Add(matePaty);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessDataToUpdate(ref HisPackingUpdateSDO updateSDO)
        {
            try
            {
                updateSDO.RequestRoomId = this.currentModuleBase.RoomId;
                updateSDO.Amount = spinTPAmount.Value;
                updateSDO.DispenseTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateTime.Now) ?? 0;
                if (dtExpTime.EditValue != null)
                    updateSDO.ExpiredDate = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtExpTime.DateTime);
                updateSDO.HeinDocumentNumber = txtDocumentNumber.Text;
                updateSDO.PackageNumber = txtPackageNumber.Text;
                updateSDO.Id = this.dispense.ID;
                updateSDO.MaterialTypes = new List<HisPackingMatySDO>();
                foreach (var item in packingMatyADOs)
                {
                    HisPackingMatySDO sdo = new HisPackingMatySDO();
                    sdo.Amount = item.AMOUNT;
                    sdo.MaterialTypeId = item.PREPA_MATERIAL_TYPE_ID;
                    updateSDO.MaterialTypes.Add(sdo);
                }
                updateSDO.MaterialPaties = new List<HIS_MATERIAL_PATY>();
                List<MaterialPatyADO> patys = materialPatyADO.Where(o => o.PRICE.HasValue).ToList();

                foreach (var item in patys)
                {
                    HIS_MATERIAL_PATY matPaty = new HIS_MATERIAL_PATY();
                    matPaty.PATIENT_TYPE_ID = item.PATIENT_TYPE_ID;
                    matPaty.EXP_PRICE = item.PRICE ?? 0;
                    if (item.VAT.HasValue)
                        matPaty.EXP_VAT_RATIO = (item.VAT ?? 0) / 100;
                    matPaty.ID = item.ID;
                    matPaty.MATERIAL_ID = item.MATERIAL_ID;
                    updateSDO.MaterialPaties.Add(matPaty);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool Check()
        {
            bool result = true;
            try
            {

                List<MaterialPatyADO> listMedicineTemp1 = materialPatyADO.Where(o => (!o.PRICE.HasValue && o.VAT.HasValue)).ToList();
                if (listMedicineTemp1 != null && listMedicineTemp1.Count > 0)
                {
                    MessageBox.Show("Lỗi nhập giá theo đối tượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }


                List<MaterialPatyADO> listMedicineTemp2 = materialPatyADO.Where(o => o.PRICE.HasValue).ToList();
                if (listMedicineTemp2 == null || listMedicineTemp2.Count == 0)
                {
                    MessageBox.Show("Nhập ít nhất một giá bán cho một đối tượng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                if (packingMatyADOs == null || packingMatyADOs.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một chế phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                PackingMatyMatyADO ado = packingMatyADOs.FirstOrDefault(o => o.IsNotAvaliable);
                if (ado != null)
                {
                    MessageBox.Show("Tồn tại chế phẩm vượt khả dụng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                PackingMatyMatyADO adoAmount = packingMatyADOs.FirstOrDefault(o => o.AMOUNT <= 0);
                if (adoAmount != null)
                {
                    MessageBox.Show("Tồn tại chế phẩm có số lượng bé hơn hoặc bằng 0", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultSDO == null) return;
                RichEditorStore richEditorMain = new RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("MPS000319", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultSDO == null)
                {
                    throw new Exception("Khong co thong tin ket qua tra ve resultSDO");
                }

                List<V_HIS_IMP_MEST_MATERIAL> impMestMaterials = null;
                List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = null;

                WaitingManager.Show();
                if (this.resultSDO.HisExpMest != null)
                {
                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_ID = this.resultSDO.HisExpMest.ID;
                    expMestMaterials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetView", ApiConsumers.MosConsumer, expMestMaterialFilter, new CommonParam());
                }
                if (this.resultSDO.HisImpMest != null)
                {
                    HisImpMestMaterialViewFilter impMestMaterialFilter = new HisImpMestMaterialViewFilter();
                    impMestMaterialFilter.IMP_MEST_ID = this.resultSDO.HisImpMest.ID;
                    impMestMaterials = new BackendAdapter(new CommonParam()).Get<List<V_HIS_IMP_MEST_MATERIAL>>("api/HisImpMestMaterial/GetView", ApiConsumers.MosConsumer, impMestMaterialFilter, new CommonParam());
                }
                WaitingManager.Hide();
                MPS.Processor.Mps000319.PDO.Mps000319PDO rdo = new MPS.Processor.Mps000319.PDO.Mps000319PDO(this.resultSDO.HisDispense, this.mediStock, impMestMaterials, expMestMaterials);
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, ""));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled) return;
                WaitingManager.Show();
                List<Action> methods = new List<Action>();
                methods.Add(LoadDataDHisMediStock);
                methods.Add(LoadMaterialTypeCombo);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                ResetControlAfterAdd(true);
                packingMatyADOs = new List<PackingMatyMatyADO>();
                resultSDO = null;
                this.dispense = null;
                gridControlPrepa.DataSource = null;
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                gridControlExpImpMest.DataSource = null;
                LoadGridServicePaty();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetControlAfterAdd(bool resetAll)
        {
            try
            {
                if (!resetAll)
                {
                    currentCP = null;
                    txtChePham.Text = "";
                    spinCPAmount.EditValue = null;
                    lblCpServiceUnitName.Text = "";
                    dxValidationProvider2.RemoveControlError(txtChePham);
                    dxValidationProvider2.RemoveControlError(spinCPAmount);
                    txtChePham.Focus();
                }
                else
                {
                    txtPackageNumber.Enabled = true;
                    txtThanhPham.Enabled = true;
                    txtDocumentNumber.Enabled = true;
                    spinTPAmount.Enabled = true;
                    dtExpTime.Enabled = true;
                    dtExpTime.EditValue = null;
                    currentCP = null;
                    currentTP = null;
                    txtPackageNumber.Text = "";
                    txtThanhPham.Text = "";
                    txtChePham.Text = "";
                    txtDocumentNumber.Text = "";
                    spinTPAmount.EditValue = null;
                    spinCPAmount.EditValue = null;
                    lblCpServiceUnitName.Text = "";
                    lblTpServiceUnitName.Text = "";
                    dxValidationProvider2.RemoveControlError(txtChePham);
                    dxValidationProvider2.RemoveControlError(spinCPAmount);
                    dxValidationProvider1.RemoveControlError(txtThanhPham);
                    dxValidationProvider1.RemoveControlError(spinTPAmount);
                    dxValidationProvider1.RemoveControlError(dtExpTime);
                    txtThanhPham.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void dxValidationProvider2_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private void GridThanhPham_RowClick(object data)
        {
            try
            {
                WaitingManager.Show();
                MaterialTypeADO tp = data as MaterialTypeADO;
                if (tp != null)
                {
                    this.currentTP = tp;
                    packingMatyADOs = new List<PackingMatyMatyADO>();
                    txtThanhPham.Text = this.currentTP.MATERIAL_TYPE_NAME;
                    lblTpServiceUnitName.Text = this.currentTP.SERVICE_UNIT_NAME;
                    spinTPAmount.Focus();
                    spinTPAmount.EditValue = null;
                    this.SetMatyMatyByTP();
                    this.LoadPatyByTP();
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetMatyMatyByTP()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.currentTP != null)
                {
                    HisMatyMatyFilter filter = new HisMatyMatyFilter();
                    filter.MATERIAL_TYPE_ID = this.currentTP.ID;
                    List<HIS_MATY_MATY> metyMetys = new BackendAdapter(param).Get<List<HIS_MATY_MATY>>("api/HisMatyMaty/Get", ApiConsumers.MosConsumer, filter, param);

                    List<V_HIS_MATERIAL_TYPE> materialTypes = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>();
                    if (metyMetys != null && metyMetys.Count > 0)
                    {
                        foreach (var item in metyMetys)
                        {
                            V_HIS_MATERIAL_TYPE materialType = materialTypes.FirstOrDefault(o => o.ID == item.PREPARATION_MATERIAL_TYPE_ID);
                            D_HIS_MEDI_STOCK_1 d1Stock = this.listDHisMediStock != null ? this.listDHisMediStock.FirstOrDefault(o => o.ID == item.PREPARATION_MATERIAL_TYPE_ID && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT) : null;
                            PackingMatyMatyADO ado = new PackingMatyMatyADO();
                            ado.PREPA_MATERIAL_TYPE_ID = item.PREPARATION_MATERIAL_TYPE_ID;
                            ado.AMOUNT = item.PREPARATION_AMOUNT;
                            ado.CFG_AMOUNT = item.PREPARATION_AMOUNT;
                            if (d1Stock == null || ado.AMOUNT > d1Stock.AMOUNT)
                            {
                                ado.IsNotAvaliable = true;
                            }

                            if (materialType != null)
                            {
                                ado.MATERIAL_TYPE_CODE = materialType.MATERIAL_TYPE_CODE;
                                ado.MATERIAL_TYPE_NAME = materialType.MATERIAL_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = materialType.SERVICE_UNIT_NAME;
                            }
                            else if (d1Stock != null)
                            {
                                ado.MATERIAL_TYPE_CODE = d1Stock.MEDICINE_TYPE_CODE;
                                ado.MATERIAL_TYPE_NAME = d1Stock.MEDICINE_TYPE_NAME;
                                ado.SERVICE_UNIT_NAME = d1Stock.SERVICE_UNIT_NAME;
                            }
                            if (packingMatyADOs == null)
                                packingMatyADOs = new List<PackingMatyMatyADO>();
                            packingMatyADOs.Add(ado);
                        }
                    }
                }
                gridControlPrepa.BeginUpdate();
                gridControlPrepa.DataSource = this.packingMatyADOs;
                gridControlPrepa.EndUpdate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadPatyByTP()
        {
            try
            {
                CommonParam param = new CommonParam();

                if (this.currentTP != null && this.currentTP.IS_SALE_EQUAL_IMP_PRICE == 1)
                {
                    foreach (var item in materialPatyADO)
                    {
                        item.PRICE = this.currentTP.IMP_PRICE;
                        item.VAT = this.currentTP.IMP_VAT_RATIO * 100;
                    }

                }
                else if (this.currentTP != null)
                {
                    List<V_HIS_SERVICE_PATY> servicePatyTemps = BackendDataWorker.Get<V_HIS_SERVICE_PATY>();
                    List<V_HIS_SERVICE_PATY> servicePatys = servicePatyTemps != null ? servicePatyTemps.Where(o => o.SERVICE_ID == this.currentTP.SERVICE_ID && o.BRANCH_ID == WorkPlace.GetBranchId()).ToList() : null;
                    foreach (var item in materialPatyADO)
                    {
                        V_HIS_SERVICE_PATY servicePaty = servicePatys != null ? servicePatys.FirstOrDefault(o => o.PATIENT_TYPE_ID == item.PATIENT_TYPE_ID) : null;
                        if (servicePaty != null)
                        {
                            item.PRICE = servicePaty.PRICE;
                            item.VAT = servicePaty.VAT_RATIO * 100;
                        }
                        else
                        {
                            item.PRICE = 0;
                            item.VAT = null;
                        }
                    }
                }
                else
                {
                    materialPatyADO.ForEach(o =>
                        {
                            o.PRICE = 0;
                            o.VAT = null;
                            o.ID = 0;
                        });
                }


                gridControlMaterialPaty.RefreshDataSource();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void GridChePham_RowClick(object data)
        {
            try
            {
                DMediStock1ADO cp = data as DMediStock1ADO;
                if (cp != null)
                {
                    this.currentCP = cp;
                    txtChePham.Text = this.currentCP.MEDICINE_TYPE_NAME;
                    lblCpServiceUnitName.Text = this.currentCP.SERVICE_UNIT_NAME;
                    spinCPAmount.Focus();
                    spinCPAmount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public string convertToUnSign3(string s)
        {
            string result = null;
            try
            {
                if (!String.IsNullOrEmpty(s))
                {
                    Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                    string temp = s.Normalize(NormalizationForm.FormD);
                    result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        private void popupControlContainerChePham_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void popupControlContainerThanhPham_CloseUp(object sender, EventArgs e)
        {
            try
            {
                isShow = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewPopupThanhPham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    MaterialTypeADO tp = gridViewPopupThanhPham.GetFocusedRow() as MaterialTypeADO;
                    if (tp != null)
                    {
                        popupControlContainerThanhPham.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridThanhPham_RowClick(tp);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPopupThanhPham_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                MaterialTypeADO tp = gridViewPopupThanhPham.GetFocusedRow() as MaterialTypeADO;
                if (tp != null)
                {
                    popupControlContainerThanhPham.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridThanhPham_RowClick(tp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPopupChePham_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DMediStock1ADO cp = gridViewPopupChePham.GetFocusedRow() as DMediStock1ADO;
                    if (cp != null)
                    {
                        popupControlContainerChePham.HidePopup();
                        isShowContainer = false;
                        isShowContainerForChoose = true;
                        GridChePham_RowClick(cp);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPopupChePham_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                DMediStock1ADO cp = gridViewPopupChePham.GetFocusedRow() as DMediStock1ADO;
                if (cp != null)
                {
                    popupControlContainerChePham.HidePopup();
                    isShowContainer = false;
                    isShowContainerForChoose = true;
                    GridChePham_RowClick(cp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Delete_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                PackingMatyMatyADO row = gridViewPrepa.GetFocusedRow() as PackingMatyMatyADO;
                if (row != null)
                {
                    packingMatyADOs.Remove(row);
                    gridControlPrepa.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPrepa_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    PackingMatyMatyADO data = (PackingMatyMatyADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnDinhMuc_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnDinhMuc_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
