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
using HIS.Desktop.LocalStorage.BackendData.ADO;
using HIS.Desktop.Plugins.DispenseMedicine.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DispenseMedicine
{
    public partial class frmDispenseMedicine : FormBase
    {
        private void LoadMediStockFromRoomId()
        {
            try
            {

                CommonParam param = new CommonParam();
                HisMediStockFilter filter = new HisMediStockFilter();
                filter.ROOM_ID = this.roomId;
                this.mediStock = new BackendAdapter(param)
                    .Get<List<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>>("api/HisMediStock/Get", ApiConsumers.MosConsumer, filter, param).FirstOrDefault();
                if (this.mediStock == null)
                    throw new Exception("mediStock is null");

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToControl()
        {
            try
            {
                txtThuocThanhPham.Focus();
                spinMetyAmount.EditValue = null;
                spinMateAmount.EditValue = null;
                if (this.mediStock != null)
                {
                    txtMediStockName.Text = this.mediStock.MEDI_STOCK_NAME;
                }
                RebuildControlContainerThanhPham();
                RebuildControlContainerChePham();
                LoadGridServicePaty();
                //Load cac doi tuong benh nhan len grid
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadGridServicePaty()
        {
            try
            {
                List<HIS_PATIENT_TYPE> patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                if (patientTypes == null)
                {
                    throw new Exception("Khong tim thay danh sach doi tuong thanh toan");
                }

                medicinePatyADOs = new List<MedicinePatyADO>();
                foreach (var item in patientTypes)
                {
                    MedicinePatyADO servicePatyADO = new MedicinePatyADO();
                    servicePatyADO.PatientTypeId = item.ID;
                    servicePatyADO.Price = 0;
                    servicePatyADO.PatientTypeName = item.PATIENT_TYPE_NAME;
                    medicinePatyADOs.Add(servicePatyADO);
                }

                gridControlPaty.DataSource = medicinePatyADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDataMetyMatyTypeInStockD1()
        {
            try
            {
                if (this.mediStock != null)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    List<long> mediStockIds = new List<long>();
                    filter.MEDI_STOCK_IDs = new List<long> { this.mediStock.ID };

                    this.mediStockD1SDOs = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.D_HIS_MEDI_STOCK_1>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param)
                        .ToList();
                }
                else
                {
                    this.mediStockD1SDOs = new List<D_HIS_MEDI_STOCK_1>();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadMetyProductADOs()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisMetyProductViewFilter filter = new HisMetyProductViewFilter();
                this.MetyProductADO = new BackendAdapter(param).Get<List<MetyProductADO>>("api/HisMetyProduct/GetView", ApiConsumers.MosConsumer, filter, param);
                if (this.MetyProductADO != null && this.MetyProductADO.Count > 0)
                {
                    foreach (var item in this.MetyProductADO)
                    {
                        var data = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID);
                        if (data != null)
                        {
                            item.ACTIVE_INGR_BHYT_NAME = data.ACTIVE_INGR_BHYT_NAME;
                            item.NATIONAL_NAME = data.NATIONAL_NAME;
                            item.MANUFACTURER_NAME = data.MANUFACTURER_NAME;
                            item.SERVICE_ID=data.SERVICE_ID;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MetyProductADO = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RebuildControlContainerThanhPham()
        {
            try
            {
                gridViewThuocTP.BeginUpdate();
                gridViewThuocTP.Columns.Clear();
                popupControlContainer.Width = theRequiredWidth;
                popupControlContainer.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = "Tên thuốc vật tư";
                col1.Width = 150;
                col1.VisibleIndex = 1;
                gridViewThuocTP.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = "Mã thuốc/vật tư";
                col2.Width = 80;
                col2.VisibleIndex = 2;
                gridViewThuocTP.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = "Hoạt chất";
                col4.Width = 200;
                col4.VisibleIndex = 4;
                gridViewThuocTP.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewThuocTP.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 80;
                col7.VisibleIndex = 7;
                gridViewThuocTP.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà cung cấp";
                col8.Width = 150;
                col8.VisibleIndex = 8;
                gridViewThuocTP.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Tên nước";
                col9.Width = 80;
                col9.VisibleIndex = 9;
                gridViewThuocTP.Columns.Add(col9);

                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewThuocTP.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewThuocTP.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col12.VisibleIndex = -1;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewThuocTP.Columns.Add(col12);

                gridViewThuocTP.GridControl.DataSource = this.MetyProductADO;
                gridViewThuocTP.EndUpdate();
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
                gridViewThuocCP.BeginUpdate();
                gridViewThuocCP.Columns.Clear();
                popupControlContainerChePham.Width = theRequiredWidth;
                popupControlContainerChePham.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = "Tên thuốc vật tư";
                col1.Width = 150;
                col1.VisibleIndex = 1;
                gridViewThuocCP.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = "Mã thuốc/vật tư";
                col2.Width = 80;
                col2.VisibleIndex = 2;
                gridViewThuocCP.Columns.Add(col2);

                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "AMOUNT";
                col3.Caption = "Số lượng";
                col3.Width = 80;
                col3.VisibleIndex = 3;
                gridViewThuocCP.Columns.Add(col3);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = "Hoạt chất";
                col4.Width = 200;
                col4.VisibleIndex = 4;
                gridViewThuocCP.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = "Hàm lượng";
                col5.Width = 100;
                col5.VisibleIndex = 5;
                gridViewThuocCP.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME";
                col7.Caption = "Đơn vị tính";
                col7.Width = 80;
                col7.VisibleIndex = 7;
                gridViewThuocCP.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = "Nhà cung cấp";
                col8.Width = 150;
                col8.VisibleIndex = 8;
                gridViewThuocCP.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = "Tên nước";
                col9.Width = 80;
                col9.VisibleIndex = 9;
                gridViewThuocCP.Columns.Add(col9);
                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewThuocCP.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewThuocCP.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col12.VisibleIndex = -1;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewThuocCP.Columns.Add(col12);

                List<HIS_MATERIAL_TYPE> materialTypeIsRaws =HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MATERIAL_TYPE>()
                    .Where(o => o.IS_RAW_MATERIAL == 1).ToList();
                List<long> materialTypeIsRawIDs = materialTypeIsRaws != null ? materialTypeIsRaws.Select(o => o.ID).ToList() : new List<long>();
                List<HIS_MEDICINE_TYPE> medicineTypeIsRaws = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE>()
                    .Where(o => o.IS_RAW_MEDICINE == 1).ToList();
                List<long> medicineTypeIsRawIDs = medicineTypeIsRaws != null ? medicineTypeIsRaws.Select(o => o.ID).ToList() : new List<long>();

                var thisMateInStocks = this.mediStockD1SDOs.Where(o => materialTypeIsRawIDs.Contains(o.ID ?? 0) || medicineTypeIsRawIDs.Contains(o.ID ?? 0)).ToList();

                gridViewThuocCP.GridControl.DataSource = ConvertToDMediStock1(thisMateInStocks);
                gridViewThuocCP.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridChePham_RowClick(object data)
        {
            try
            {
                DMediStock1ADO mediMateChePham = data as DMediStock1ADO;
                if (mediMateChePham != null)
                {
                    currentMateChePham = mediMateChePham;
                    txtThuocChePham.Text = currentMateChePham.MEDICINE_TYPE_NAME;
                    lblCPUnitName.Text = currentMateChePham.SERVICE_UNIT_NAME;
                    spinMateAmount.Focus();
                    spinMateAmount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridThanhPham_RowClick(object data)
        {
            try
            {
                WaitingManager.Show();
                MetyProductADO mediMateThanhPham = data as MetyProductADO;
                if (mediMateThanhPham != null)
                {
                    currentMetyThanhPham = mediMateThanhPham;
                    dispenseMetyMatyADOs = new List<DispenseMedyMatyADO>();
                    txtThuocThanhPham.Text = currentMetyThanhPham.MEDICINE_TYPE_NAME;
                    lblTPUnitName.Text = currentMetyThanhPham.SERVICE_UNIT_NAME;
                    spinMetyAmount.Focus();
                    spinMetyAmount.EditValue = mediMateThanhPham.AMOUNT;
                    this.GetMatyFromMetyMaty(mediMateThanhPham.ID);
                    this.LoadPriceVatFromMedicineType(mediMateThanhPham.SERVICE_ID);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlAfterAdd(RESET reset)
        {
            try
            {
                if (reset == RESET.ADD)
                {
                    //txtPackageNumber.Enabled = false;
                    //txtThuocThanhPham.Enabled = false;
                    //txtHeinDocumentNumber.Enabled = false;
                    //spinMetyAmount.Enabled = false;
                    //dtExpTime.Enabled = false;
                    currentMateChePham = null;
                    txtThuocChePham.Text = "";
                    spinMateAmount.EditValue = null;
                    lblTPUnitName.Text = "";
                    lblCPUnitName.Text = "";
                    txtThuocChePham.Focus();
                    lciMatyAmount.AppearanceItemCaption.ForeColor = Color.Black;
                }
                else if (reset == RESET.REFESH)
                {
                    txtPackageNumber.Enabled = true;
                    txtThuocThanhPham.Enabled = true;
                    txtHeinDocumentNumber.Enabled = true;
                    spinMetyAmount.Enabled = true;
                    dtExpTime.Enabled = true;
                    dtExpTime.EditValue = null;
                    currentMateChePham = null;
                    currentMetyThanhPham = null;
                    txtPackageNumber.Text = "";
                    txtThuocThanhPham.Text = "";
                    txtThuocChePham.Text = "";
                    txtHeinDocumentNumber.Text = "";
                    spinMetyAmount.EditValue = null;
                    spinMateAmount.EditValue = null;
                    lblCPUnitName.Text = "";
                    lblTPUnitName.Text = "";
                    txtThuocThanhPham.Focus();
                    lciMatyAmount.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public enum RESET
        {
            ADD,
            REFESH
        }

        public void LoadImpMestToGrid(List<HIS_IMP_MEST> impMests, List<HIS_EXP_MEST> expMests)
        {
            try
            {
                List<ImpExpADO> listImpExpADOs = new List<ImpExpADO>();
                if (impMests != null && impMests.Count > 0)
                {
                    foreach (var item in impMests)
                    {
                        ImpExpADO impExp = new ImpExpADO();
                        impExp.ImpExpMestCode = item.IMP_MEST_CODE;
                        impExp.IsImpMest = true;
                        impExp.CreateTime = item.CREATE_TIME;
                        listImpExpADOs.Add(impExp);
                    }
                }
                if (expMests != null && expMests.Count > 0)
                {
                    foreach (var item in expMests)
                    {
                        ImpExpADO impExp = new ImpExpADO();
                        impExp.ImpExpMestCode = item.EXP_MEST_CODE;
                        impExp.IsExpMest = true;
                        impExp.CreateTime = item.CREATE_TIME;
                        listImpExpADOs.Add(impExp);
                    }
                }

                gridControlImpExp.DataSource = listImpExpADOs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public void InitEnabledAction(ACTION action)
        {
            try
            {
                if (action == ACTION.CREATE)
                {
                    txtThuocThanhPham.Enabled = true;
                    btnSave.Text = "Lưu (Ctrl S)";
                }
                else
                {
                    txtThuocThanhPham.Enabled = false;
                    btnPrint.Enabled = true;
                    btnSave.Text = "Sửa (Ctrl S)";
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private List<DMediStock1ADO> ConvertToDMediStock1(List<D_HIS_MEDI_STOCK_1> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    foreach (var item in listMediStock)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<DMediStock1ADO>(dMediStock1ADO, item);
                        dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;
                        result.Add(dMediStock1ADO);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        public static string convertToUnSign3(string s)
        {
            string result = null;
            try
            {
                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = s.Normalize(NormalizationForm.FormD);
                result = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
