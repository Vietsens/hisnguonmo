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
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        internal void RebuildMedicineTypeWithInControlContainer()
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_NAME";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 250;
                col1.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 60;
                col7.VisibleIndex = 2;
                col7.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col7);

                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 160;
                col4.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "CONCENTRA";
                col5.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col5.Width = 100;
                col5.VisibleIndex = 4;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "MANUFACTURER_NAME";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 150;
                col8.VisibleIndex = 5;
                gridViewMediMaty.Columns.Add(col8);

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "NATIONAL_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 80;
                col9.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col9);


                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_CODE";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 60;
                col2.VisibleIndex = 7;
                gridViewMediMaty.Columns.Add(col2);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col10.Width = 80;
                col10.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col11.Width = 80;
                col11.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col11.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col12.VisibleIndex = -1;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col12);

                List<MedicineMaterialTypeComboADO> mediMateTypeComboADOs = BackendDataWorker.Get<MedicineMaterialTypeComboADO>(false, true, false, false);

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                long roomId = GetRoomId();
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                {
                    List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>();
                    medicineTypeRooms = medicineTypeRooms.Where(o => o.ROOM_ID == roomId).ToList();
                    List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                    mediMateTypeComboADOs = mediMateTypeComboADOs.Where(o => medicineTypeIdRooms.Contains(o.ID)
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                }
                //

                long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                if (isOnlyDisplayMediMateIsBusiness == 1 && mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                    mediMateTypeComboADOs = mediMateTypeComboADOs.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();
                gridViewMediMaty.GridControl.DataSource = mediMateTypeComboADOs;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //internal async Task RebuildNhaThuocMediMatyWithInControlContainerAsync()
        //{
        //    try
        //    {
        //        if (cboNhaThuoc.EditValue != null)
        //        {
        //            long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboNhaThuoc.EditValue.ToString());
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
        //            filter.MEDI_STOCK_IDs = new List<long> { mediStockId };
        //            this.ProcessFilterDontPresExpiredTime(ref filter);//TODO

        //            this.mediMatyTypeAvailables = await new BackendAdapter(param).GetAsync<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
        //            this.mediStockD1ADOs = ConvertToDMediStock2(this.mediMatyTypeAvailables);
        //        }

        //        this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //internal void RebuildNhaThuocMediMatyWithInControlContainer()
        //{
        //    try
        //    {
        //        if (cboNhaThuoc.EditValue != null)
        //        {
        //            long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64(cboNhaThuoc.EditValue.ToString());
        //            CommonParam param = new CommonParam();
        //            MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
        //            filter.MEDI_STOCK_IDs = new List<long> { mediStockId };
        //            this.ProcessFilterDontPresExpiredTime(ref filter);//TODO

        //            this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
        //            this.mediStockD1ADOs = ConvertToDMediStock2(this.mediMatyTypeAvailables);
        //        }

        //        this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        //internal void RebuildPopupContainerNhaThuocShowMediMatyForSelect(List<DMediStock1ADO> dMediStock1ADOs)
        //{
        //    try
        //    {               
        //        gridViewMediMaty.BeginUpdate();
        //        gridViewMediMaty.Columns.Clear();
        //        popupControlContainerMediMaty.Width = theRequiredWidth;
        //        popupControlContainerMediMaty.Height = theRequiredHeight;

        //        DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col2.FieldName = "MEDICINE_TYPE_NAME";
        //        col2.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col2.Width = 250;
        //        col2.VisibleIndex = 1;
        //        gridViewMediMaty.Columns.Add(col2);


        //        DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col3.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
        //        col3.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col3.Width = 60;
        //        col3.VisibleIndex = 2;
        //        col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
        //        gridViewMediMaty.Columns.Add(col3);


        //        DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col8.FieldName = "CONCENTRA";
        //        col8.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col8.Width = 100;
        //        col8.VisibleIndex = 3;
        //        gridViewMediMaty.Columns.Add(col8);


        //        DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
        //        col7.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col7.Width = 160;
        //        col7.VisibleIndex = 4;
        //        gridViewMediMaty.Columns.Add(col7);

        //        DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col9.FieldName = "MANUFACTURER_NAME";
        //        col9.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col9.Width = 150;
        //        col9.VisibleIndex = 9;
        //        gridViewMediMaty.Columns.Add(col9);


        //        DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col4.FieldName = "AMOUNT";
        //        col4.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col4.Width = 70;
        //        col4.VisibleIndex = 5;
        //        col4.DisplayFormat.FormatString = "#,##0.00";
        //        col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
        //        gridViewMediMaty.Columns.Add(col4);

        //        DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col5.FieldName = "IMP_PRICE_DISPLAY";
        //        col5.Caption = "Giá bán";
        //        col5.Width = 100;
        //        col5.VisibleIndex = 6;
        //        col5.DisplayFormat.FormatString = "#,##0.00";
        //        col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
        //        col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
        //        gridViewMediMaty.Columns.Add(col5);

        //        DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
        //        col6.Caption = "VAT(%)";
        //        col6.Width = 100;
        //        col6.VisibleIndex = 7;
        //        col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
        //        gridViewMediMaty.Columns.Add(col6);



        //        DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col10.FieldName = "NATIONAL_NAME";
        //        col10.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col10.Width = 80;
        //        col10.VisibleIndex = 8;
        //        gridViewMediMaty.Columns.Add(col10);



        //        DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col1.FieldName = "MEDICINE_TYPE_CODE";
        //        col1.Caption = Inventec.Common.Resource.Get.Value
        //            ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
        //            Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
        //            Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
        //        col1.Width = 60;
        //        col1.VisibleIndex = 9;
        //        gridViewMediMaty.Columns.Add(col1);


        //        //Phuc vu cho tim kiem khong dau

        //        DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col11.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
        //        col11.Width = 80;
        //        col11.VisibleIndex = -1;
        //        gridViewMediMaty.Columns.Add(col11);

        //        DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col12.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
        //        col12.Width = 80;
        //        col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
        //        col12.VisibleIndex = -1;
        //        gridViewMediMaty.Columns.Add(col12);

        //        DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
        //        col13.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
        //        col13.VisibleIndex = -1;
        //        col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
        //        gridViewMediMaty.Columns.Add(col13);

        //        gridViewMediMaty.GridControl.DataSource = dMediStock1ADOs;
        //        gridViewMediMaty.EndUpdate();
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        internal void MedicineType_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);
                    if (CheckExistMedicinePaymentLimit(this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE))
                    {
                        MessageBox.Show(ResourceMessage.ThuocCoGioiHanChiDinhThanhToanBHYTDeNghiBSXemXetTruocKhiKe, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.btnAdd.Enabled = true;
                    this.cboMedicineUseForm.EditValue = null;
                    this.txtTutorial.Text = "";
                    this.txtHtu.Text = "";
                    this.spinAmount.Text = "";
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;

                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        this.FillDataIntoMedicineUseFormAndTutorial(this.currentMedicineTypeADOForEdit.ID);

                        //Neu la thuoc thi kiem tra co mẫu HDSD chưa, có thì focus vào nút "Bổ sung"
                        if (this.medicineTypeTutSelected != null && !String.IsNullOrEmpty(this.medicineTypeTutSelected.TUTORIAL))
                        {
                            this.btnAdd.Focus();
                        }
                        //Ngược lại kiểm tra có cấu hình PM cho phép sau khi chọn thuốc thì nhảy vào ô số lượng hay ô ngày
                        else
                        {
                            this.spinAmount.Focus();
                            this.spinAmount.SelectAll();
                        }
                    }
                    else if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {                       
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }
                    this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(this.currentMedicineTypeADOForEdit.ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                    cboPatientType.EditValue = null;
                    cboPatientType.Properties.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
