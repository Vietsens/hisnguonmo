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
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Worker;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
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
        internal async Task RebuildMediMatyWithInControlContainer()
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                List<DMediStock1ADO> dMediStock1s = new List<DMediStock1ADO>();
                dMediStock1s.AddRange(this.mediStockD1ADOs);

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_NAME";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 250;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
                col3.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col3.Width = 60;
                col3.VisibleIndex = 2;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 100;
                col8.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col8);


                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 160;
                col7.VisibleIndex = 4;
                gridViewMediMaty.Columns.Add(col7);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 90;
                col4.VisibleIndex = 5;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col4);


                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "MEDI_STOCK_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MEDI_STOCK",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 100;
                col9.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col9);


                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "IMP_PRICE_DISPLAY";
                col5.Caption = "Giá bán";
                col5.Width = 100;
                col5.VisibleIndex = 7;
                col5.DisplayFormat.FormatString = "#,##0.00";
                col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
                col6.Caption = "VAT(%)";
                col6.Width = 100;
                col6.VisibleIndex = 8;
                col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col6);


                DevExpress.XtraGrid.Columns.GridColumn col6a = new DevExpress.XtraGrid.Columns.GridColumn();
                col6a.FieldName = "TDL_PACKAGE_NUMBER";
                col6a.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_PACKAGE_NUMBER",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6a.Width = 100;
                col6a.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col6a);

                DevExpress.XtraGrid.Columns.GridColumn col6b = new DevExpress.XtraGrid.Columns.GridColumn();
                col6b.FieldName = "REGISTER_NUMBER";
                col6b.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_REGISTER_NUMBER",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6b.Width = 100;
                col6b.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col6b);

                DevExpress.XtraGrid.Columns.GridColumn col6c = new DevExpress.XtraGrid.Columns.GridColumn();
                col6c.FieldName = "EXPIRED_DATE_DISPLAY";
                col6c.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                col6c.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_EXPIRED_DATE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6c.Width = 100;
                col6c.VisibleIndex = 11;
                gridViewMediMaty.Columns.Add(col6c);


                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MANUFACTURER_NAME";
                col10.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col10.Width = 150;
                col10.VisibleIndex = 12;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "NATIONAL_NAME";
                col11.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col11.Width = 80;
                col11.VisibleIndex = 13;
                gridViewMediMaty.Columns.Add(col11);


                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 60;
                col1.VisibleIndex = 14;
                gridViewMediMaty.Columns.Add(col1);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col14);

                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal async Task RebuildMediMatyWithInControlContainerAsync()
        {
            try
            {
                await this.InitDataMetyMatyTypeInStockDAsync(this.currentMediStock);

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                List<DMediStock1ADO> dMediStock1s = new List<DMediStock1ADO>();
                dMediStock1s.AddRange(this.mediStockD1ADOs);

                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col2 = new DevExpress.XtraGrid.Columns.GridColumn();
                col2.FieldName = "MEDICINE_TYPE_NAME";
                col2.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col2.Width = 250;
                col2.VisibleIndex = 1;
                gridViewMediMaty.Columns.Add(col2);


                DevExpress.XtraGrid.Columns.GridColumn col3 = new DevExpress.XtraGrid.Columns.GridColumn();
                col3.FieldName = "SERVICE_UNIT_NAME_DISPLAY";
                col3.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_SERVICE_UNIT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col3.Width = 60;
                col3.VisibleIndex = 2;
                col3.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col3);


                DevExpress.XtraGrid.Columns.GridColumn col8 = new DevExpress.XtraGrid.Columns.GridColumn();
                col8.FieldName = "CONCENTRA";
                col8.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_CONCENTRA",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col8.Width = 100;
                col8.VisibleIndex = 3;
                gridViewMediMaty.Columns.Add(col8);


                DevExpress.XtraGrid.Columns.GridColumn col7 = new DevExpress.XtraGrid.Columns.GridColumn();
                col7.FieldName = "ACTIVE_INGR_BHYT_NAME";
                col7.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_ACTIVE_INGR_BHYT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col7.Width = 160;
                col7.VisibleIndex = 4;
                gridViewMediMaty.Columns.Add(col7);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 90;
                col4.VisibleIndex = 5;
                col4.DisplayFormat.FormatString = "#,##0.00";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                gridViewMediMaty.Columns.Add(col4);


                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "MEDI_STOCK_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MEDI_STOCK",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 100;
                col9.VisibleIndex = 6;
                gridViewMediMaty.Columns.Add(col9);


                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "IMP_PRICE_DISPLAY";
                col5.Caption = "Giá bán";
                col5.Width = 100;
                col5.VisibleIndex = 7;
                col5.DisplayFormat.FormatString = "#,##0.00";
                col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
                col6.Caption = "VAT(%)";
                col6.Width = 100;
                col6.VisibleIndex = 8;
                col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col6);


                DevExpress.XtraGrid.Columns.GridColumn col6a = new DevExpress.XtraGrid.Columns.GridColumn();
                col6a.FieldName = "TDL_PACKAGE_NUMBER";
                col6a.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_PACKAGE_NUMBER",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6a.Width = 100;
                col6a.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col6a);

                DevExpress.XtraGrid.Columns.GridColumn col6b = new DevExpress.XtraGrid.Columns.GridColumn();
                col6b.FieldName = "REGISTER_NUMBER";
                col6b.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_REGISTER_NUMBER",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6b.Width = 100;
                col6b.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col6b);

                DevExpress.XtraGrid.Columns.GridColumn col6c = new DevExpress.XtraGrid.Columns.GridColumn();
                col6c.FieldName = "EXPIRED_DATE_DISPLAY";
                col6c.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                col6c.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_EXPIRED_DATE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col6c.Width = 100;
                col6c.VisibleIndex = 11;
                gridViewMediMaty.Columns.Add(col6c);


                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "MANUFACTURER_NAME";
                col10.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col10.Width = 150;
                col10.VisibleIndex = 12;
                gridViewMediMaty.Columns.Add(col10);

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "NATIONAL_NAME";
                col11.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col11.Width = 80;
                col11.VisibleIndex = 13;
                gridViewMediMaty.Columns.Add(col11);


                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 60;
                col1.VisibleIndex = 14;
                gridViewMediMaty.Columns.Add(col1);


                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col12.Width = 80;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col13.Width = 80;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col13.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col14.VisibleIndex = -1;
                col14.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col14);

                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                gridViewMediMaty.EndUpdate();
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MetyMatyTypeInStock_RowClick(object data)
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

                    if (this.currentMedicineTypeADOForEdit == null) throw new ArgumentNullException("currentMedicineTypeADOForEdit is null");
                    if (data.GetType() == typeof(DMediStock1ADO))
                    {
                        DMediStock1ADO dMediStock = data as DMediStock1ADO;
                        this.currentMedicineTypeADOForEdit.IsStent = ((dMediStock.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                        this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(dMediStock != null && dMediStock.ID.HasValue ? dMediStock.ID.Value : 0, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                        this.currentMedicineTypeADOForEdit.TDL_GENDER_ID = dMediStock.GENDER_ID;
                        this.SetControlSoLuongNgayNhapChanLe(this.currentMedicineTypeADOForEdit);
                    }

                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.ReSetDataInputAfterAdd__MedicinePage();
                    this.btnAdd.Enabled = true;
                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    this.chkPhimHong.Enabled = (this.currentMedicineTypeADOForEdit.IS_FILM.HasValue && this.currentMedicineTypeADOForEdit.IS_FILM.Value == 1);
                    if (!this.chkPhimHong.Enabled)
                    {
                        this.spinSoPhimHong.EditValue = null;
                    }
                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        this.FillDataIntoMedicineUseFormAndTutorial(currentMedicineTypeADOForEdit.ID);
                        //Neu la thuoc thi kiem tra co mẫu HDSD chưa, có thì focus vào nút "Bổ sung"
                        if (this.medicineTypeTutSelected != null && this.medicineTypeTutSelected.ID > 0)
                        {
                            //- Với kê tủ trực, kê thuốc điều trị:
                            //--nếu thuốc có hdsd => nhảy con trỏ vào ô số lượng sau khi chọn thuốc.
                            //--nếu không có hdsd => như kê đơn phòng khám
                            if (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet || GlobalStore.IsExecutePTTT)
                            {
                                spinAmount.Focus();
                                spinAmount.SelectAll();
                            }
                            else
                            {
                                //this.btnAdd.Focus();
                                this.txtTutorial.Focus();
                                this.txtTutorial.SelectionStart = txtTutorial.Text.Length + 1;
                                this.txtHtu.SelectionStart = txtHtu.Text.Length + 1;
                            }
                        }
                        //Ngược lại kiểm tra có cấu hình PM cho phép sau khi chọn thuốc thì nhảy vào ô số lượng hay ô ngày
                        else
                        {
                            this.spinAmount.Focus();
                            this.spinAmount.SelectAll();
                        }
                    }
                    else
                    {
                        //Neu la vat tu thi mặc định focus vào ô số lượng
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);
                    if (!HisConfigCFG.IsDefaultPatientTypeOption)
                    {
                        HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlServiceOther(currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                            this.currentMedicineTypeADOForEdit.SERVICE_ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                        if (patientTypeDefault != null)
                        {
                            cboPatientType.EditValue = patientTypeDefault.ID;
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan mac dinh cho dinh vu");
                            cboPatientType.EditValue = null;
                        }
                    }
                    else
                    {
                        ChooseDefaultPatientTypeFromKey();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void MaterialTypeTSD_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);

                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.btnAdd.Enabled = true;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderMaterialTypeTSD, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    this.btnAdd.Focus();

                    //this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(this.currentMedicineTypeADOForEdit.ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);
                    HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlServiceOther(currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                        this.currentMedicineTypeADOForEdit.SERVICE_ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                    if (patientTypeDefault != null)
                    {
                        cboPatientType.EditValue = patientTypeDefault.ID;
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan mac dinh cho dinh vu");
                        cboPatientType.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool GetIsAllowOdd(long id, long serviceTypeId)
        {
            bool result = false;
            try
            {
                if (id > 0)
                {
                    if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        V_HIS_MEDICINE_TYPE medicineType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == id);
                        if (medicineType != null && medicineType.IS_ALLOW_ODD.HasValue)
                        {
                            result = medicineType.IS_ALLOW_ODD == 1 ? true : false;
                        }
                    }
                    else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        V_HIS_MATERIAL_TYPE maType = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == id);
                        if (maType != null && maType.IS_ALLOW_ODD.HasValue)
                        {
                            result = maType.IS_ALLOW_ODD == 1 ? true : false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void FillDataIntoMedicineUseFormAndTutorial(long medicineTypeId)
        {
            try
            {
                this.txtTutorial.Text = string.Empty;
                this.txtHtu.Text = string.Empty;
                this.cboMedicineUseForm.EditValue = null;
                this.medicineTypeTutSelected = null;

                //Lấy dữ liệu cấu hình hướng dẫn sử dụng của thuốc (HIS_MEDICINE_TYPE_TUT) theo tài khoản đăng nhập và loại thuốc
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                if (medicineTypeTuts != null && medicineTypeTuts.Count > 0)
                {
                    List<HIS_MEDICINE_TYPE_TUT> medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == medicineTypeId && o.LOGINNAME == loginName).ToList();

                    this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);
                    this.RebuildHtulWithInControlContainer(medicineTypeTutFilters);
                    this.medicineTypeTutSelected = medicineTypeTutFilters.FirstOrDefault();
                    if (this.medicineTypeTutSelected != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineTypeTutSelected), this.medicineTypeTutSelected));
                        //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                        if (this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID > 0)
                        {
                            this.cboMedicineUseForm.EditValue = this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID;
                            this.txtHtu.Text = this.cboMedicineUseForm.Text;
                        }
                        //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                        else
                        {
                            var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                            {
                                this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                                this.txtHtu.Text = this.cboMedicineUseForm.Text;
                            }
                        }

                        Inventec.Common.Logging.LogSystem.Debug("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTutSelected), medicineTypeTutSelected));

                        if (String.IsNullOrEmpty(this.txtTutorial.Text))
                        {
                            //Nếu có trường hướng dẫn thì sử dụng luôn
                            if (!String.IsNullOrEmpty(this.medicineTypeTutSelected.TUTORIAL))
                            {
                                this.txtTutorial.Text = this.medicineTypeTutSelected.TUTORIAL;
                                this.txtHtu.Text = this.cboMedicineUseForm.Text;
                            }
                            //Nếu không có hướng dẫn sử dụng thì tự động set theo các trường như lúc nhập liệu
                            else
                            {
                                //this.CalculateAmount();//TODO
                                //this.SetHuongDanFromSoLuongNgay();//TODO
                            }
                        }
                        if (String.IsNullOrEmpty(this.txtHtu.Text))
                        {
                            this.txtHtu.Text = this.medicineTypeTutSelected.HTU_TEXT;                           
                        }
                    }
                }
                //Trường hợp thuốc không có cấu hình hướng dẫn sử dụng thì lấy hướng dẫn sử dụng ở danh mục loại thuốc fill vào
                else
                    this.medicineTypeTutSelected = null;

                if (this.medicineTypeTutSelected == null)
                {
                    var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                    if (medicineType != null)
                    {
                        if ((medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                            this.txtHtu.Text = this.cboMedicineUseForm.Text;
                        }
                        if (String.IsNullOrEmpty(this.txtTutorial.Text))
                        {
                            this.txtTutorial.Text = medicineType.TUTORIAL;
                            this.txtHtu.Text = this.cboMedicineUseForm.Text;
                        }                                
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? CalulateUseTimeTo()
        {
            long? result = null;
            try
            {
                List<long> intructionTimeAdds = new List<long>();
                intructionTimeAdds = this.intructionTimeSelecteds.OrderByDescending(o => o).ToList();
                if (intructionTimeAdds != null
                    && intructionTimeAdds.Count > 0
                    && intructionTimeAdds.First() > 0
                    //&& !string.IsNullOrEmpty(this.spinSoLuongNgay.Text)
                    )
                {
                    long useTime = intructionTimeAdds.First();
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime) ?? DateTime.MinValue;
                    DateTime dtUseTimeTo = dtUseTime.AddDays((double)1 - 1);//dtUseTime.AddDays((double)this.spinSoLuongNgay.Value - 1);
                    long useTimeTo = Inventec.Common.TypeConvert.Parse.ToInt64(dtUseTimeTo.ToString("yyyyMMddHHmm") + "00");
                    result = useTimeTo;
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private bool ExistsAssianInDay(long serviceId)
        {
            bool result = false;
            try
            {
                if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                {
                    result = this.sereServWithTreatment.Where(o => o.SERVICE_ID == serviceId
                        && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8)).Any();
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void InitDataMetyMatyTypeInStockD()
        {
            try
            {
                InitDataMetyMatyTypeInStockD(this.currentMediStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitDataMetyMatyTypeInStockDAsync(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitDataMetyMatyTypeInStockDAsync .1");
                this.mediStockD1ADOs = new List<DMediStock1ADO>();
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock2Filter filter = new MOS.Filter.DHisMediStock2Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    this.ProcessFilterDontPresExpiredTime(ref filter);
                    this.mediMatyTypeAvailables = await new BackendAdapter(param).GetAsync<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    this.ProcessResultDataMetyMatyTypeInStock(mediStockIds, false);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong co kho duo chon____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            LogSystem.Debug("Du lieu thuoc/vat tu theo kho sau khi loc____ " + "____ket qua tim thay " + (this.mediStockD1ADOs != null ? this.mediStockD1ADOs.Count : 0) + "____Kho:" + Inventec.Common.Logging.LogUtil.TraceData("currentMediStock.count", currentMediStock != null ? currentMediStock.Count : 0));
        }

        /// <summary>
        /// Gọi api lấy dữ liệu thuốc và vật tư gộp trong 1 danh sách, lọc theo các điều kiện và cấu hình khác => sử dụng trong phòng khám
        /// </summary>
        private void InitDataMetyMatyTypeInStockD(List<MOS.EFMODEL.DataModels.V_HIS_MEST_ROOM> currentMediStock)
        {
            try
            {
                this.mediStockD1ADOs = new List<DMediStock1ADO>();
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock2Filter filter = new MOS.Filter.DHisMediStock2Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;

                    this.ProcessFilterDontPresExpiredTime(ref filter);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);

                    this.ProcessResultDataMetyMatyTypeInStock(mediStockIds, false);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong co kho duo chon____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentMediStock), currentMediStock));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            LogSystem.Debug("Du lieu thuoc/vat tu theo kho sau khi loc____ " + "____ket qua tim thay " + (this.mediStockD1ADOs != null ? this.mediStockD1ADOs.Count : 0) + "____" + Inventec.Common.Logging.LogUtil.TraceData("currentMediStock.count", currentMediStock != null ? currentMediStock.Count : 0));
        }

        private void ProcessResultDataMetyMatyTypeInStock(List<long> mediStockIds, bool isTSD)
        {
            try
            {
                LogSystem.Debug("Load du lieu kho theo dieu kien loc 1____ " + String.Join(",", mediStockIds) + "____ket qua tim thay " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0) + "____isTSD:" + isTSD);

                MediStockWorker.FilterByMediStockMetiD2(mediStockIds, ref this.mediMatyTypeAvailables);
                LogSystem.Debug("Kiểm tra cấu hình có hiển thị thuốc hay không trong bảng MEDI_STOCK_METY____ " + "____ket qua tim thay sau khi loc " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));
                MediStockWorker.FilterByMestMetyDepaD2(mediStockIds, this.currentWorkPlace, ref this.mediMatyTypeAvailables);
                LogSystem.Debug("Thuốc trong kho cấu hình chỉ có khoa nào được phép sử dụng. HIS_MEST_METY_DEPA.____ " + "____ket qua tim thay sau khi loc " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));
                MediStockWorker.FilterByRestrict(GetRoomId(), ref this.mediMatyTypeAvailables);

                //Loc du lieu theo Rank
                if (cboUser.EditValue != null)
                {
                    string loginname = cboUser.EditValue.ToString();
                    MediStockWorker.FilterByRankEmployee(loginname, ref this.mediMatyTypeAvailables);
                }

                //if (this.currentHisPatientTypeAlter != null && this.servicePatyAllows != null)
                //{
                //    this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => this.servicePatyAllows.ContainsKey((o.SERVICE_ID ?? 0))).ToList();
                //    LogSystem.Debug("Loc theo chinh sach gia " + ".____ " + "____ket qua tim thay sau khi loc " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));
                //}
                this.mediMatyTypeAvailables = this.mediMatyTypeAvailables.Where(o => (o.AMOUNT ?? 0) > 0).ToList();

                LogSystem.Debug("Du lieu thuoc/vat tu theo kho sau khi loc theo cac dieu kien loc____ket qua tim thay " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));

                List<D_HIS_MEDI_STOCK_2> mediStockD1s = new List<D_HIS_MEDI_STOCK_2>();
                mediStockD1s.AddRange(this.mediMatyTypeAvailables);

                if (isTSD)
                {
                    mediStockD1s = mediStockD1s.Where(o => o.IS_REUSABLE == 1 && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                    this.ProcessMapingDataForTSD(mediStockD1s, mediStockIds);
                }
                else
                {
                    mediStockD1s = mediStockD1s.Where(o => (o.IS_REUSABLE == null || o.IS_REUSABLE != 1)).ToList();
                    this.mediStockD1ADOs = this.ConvertToDMediStock2(mediStockD1s);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ProcessMapingDataForTSD(List<D_HIS_MEDI_STOCK_2> mediStockD1s, List<long> mediStockIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisMaterialBeanView1Filter materialBeanViewFilter = new MOS.Filter.HisMaterialBeanView1Filter();
                materialBeanViewFilter.IS_REUSABLE = true;
                materialBeanViewFilter.MEDI_STOCK_IDs = mediStockIds;
                materialBeanViewFilter.MATERIAL_IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                materialBeanViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var matyIds = mediStockD1s.Select(o => o.ID ?? 0).ToList();
                var matyBeans = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_BEAN_1>>(RequestUriStore.HIS_MATERIAL_BEAN__GET_VIEW1, ApiConsumers.MosConsumer, materialBeanViewFilter, ProcessLostToken, param);
                matyBeans = matyBeans.Where(o => matyIds.Contains(o.MATERIAL_TYPE_ID)).ToList();

                AutoMapper.Mapper.CreateMap<V_HIS_MATERIAL_BEAN_1, DMediStock1ADO>();
                this.mediStockD1ADOs = AutoMapper.Mapper.Map<List<DMediStock1ADO>>(matyBeans);

                foreach (var item in this.mediStockD1ADOs)
                {
                    item.USE_COUNT_DISPLAY = String.Format("{0} / {1}", item.REMAIN_REUSE_COUNT, item.TDL_MATERIAL_MAX_REUSE_COUNT);
                    item.ID = item.MATERIAL_TYPE_ID;
                    item.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;

                    var mtF = mediStockD1s.Where(o => o.ID == item.MATERIAL_TYPE_ID && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID).FirstOrDefault();

                    if (mtF != null)
                    {
                        item.MEDICINE_TYPE_CODE = mtF.MEDICINE_TYPE_CODE;
                        item.MEDICINE_TYPE_NAME = mtF.MEDICINE_TYPE_NAME;
                        item.MEDI_STOCK_NAME = mtF.MEDI_STOCK_NAME;
                        item.MEDI_STOCK_CODE = mtF.MEDI_STOCK_CODE;
                        item.CONVERT_RATIO = mtF.CONVERT_RATIO;
                        item.CONVERT_UNIT_CODE = mtF.CONVERT_UNIT_CODE;
                        item.CONVERT_UNIT_NAME = mtF.CONVERT_UNIT_NAME;

                        item.MANUFACTURER_ID = mtF.MANUFACTURER_ID;
                        item.MANUFACTURER_CODE = mtF.MANUFACTURER_CODE;
                        item.MANUFACTURER_NAME = mtF.MANUFACTURER_NAME;
                        item.MEDICINE_TYPE_CODE__UNSIGN = mtF.MEDICINE_TYPE_CODE;
                        item.MEDICINE_TYPE_NAME__UNSIGN = mtF.MEDICINE_TYPE_NAME;
                        item.MEDICINE_USE_FORM_ID = mtF.MEDICINE_USE_FORM_ID;
                        item.NATIONAL_NAME = mtF.NATIONAL_NAME;
                        item.RANK = mtF.RANK;
                        item.SERVICE_UNIT_CODE = mtF.SERVICE_UNIT_CODE;
                        item.SERVICE_UNIT_ID = mtF.SERVICE_UNIT_ID;
                        item.SERVICE_UNIT_NAME = mtF.SERVICE_UNIT_NAME;
                        item.TUTORIAL = mtF.TUTORIAL;
                        
                        item.MEDICINE_USE_FORM_ID = mtF.MEDICINE_USE_FORM_ID;
                        item.IS_AUTO_EXPEND = mtF.IS_AUTO_EXPEND;
                        item.IS_OUT_PARENT_FEE = mtF.IS_OUT_PARENT_FEE;

                        item.LAST_EXP_PRICE = mtF.LAST_EXP_PRICE;
                        item.LAST_EXP_VAT_RATIO = mtF.LAST_EXP_VAT_RATIO;
                    }
                    else
                    {
                        item.ID = 0;
                        Inventec.Common.Logging.LogSystem.Debug("Vat tu khong con kha dung trong kho____" + item.MEDICINE_TYPE_CODE + "-" + item.MEDICINE_TYPE_NAME + " : " + item.MEDI_STOCK_CODE + "-" + item.MEDI_STOCK_NAME);
                    }
                }
                this.mediStockD1ADOs = this.mediStockD1ADOs.Where(o => o.ID > 0).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<DMediStock1ADO> ConvertToDMediStock2(List<D_HIS_MEDI_STOCK_2> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    var currentMedicineTypeTemps = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    var currentMaterialTypeTemps = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    foreach (var item in listMediStock)
                    {
                        bool isUse = false;
                        V_HIS_MEDICINE_TYPE mety = null;
                        if (this.oldServiceReq != null && this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1 && item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            mety = currentMedicineTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();
                            isUse = (mety != null && mety.IS_KIDNEY == 1);
                        }
                        else
                        {
                            isUse = true;
                        }
                        if (isUse)
                        {
                            DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                            dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                            dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                            dMediStock1ADO.ALERT_MAX_IN_TREATMENT = item.ALERT_MAX_IN_TREATMENT;
                            dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                            dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                            dMediStock1ADO.GENDER_ID = item.GENDER_ID;
                            dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                            dMediStock1ADO.ID = item.ID;
                            dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                            dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                            dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                            dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                            dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                            dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                            dMediStock1ADO.IS_STAR_MARK = item.IS_STAR_MARK;
                            dMediStock1ADO.IS_STENT = item.IS_STENT;
                            dMediStock1ADO.IS_VACCINE = item.IS_VACCINE;
                            dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                            dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                            dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                            dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                            dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                            dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                            dMediStock1ADO.MEDICINE_TYPE_CODE = item.MEDICINE_TYPE_CODE;
                            dMediStock1ADO.MEDICINE_TYPE_NAME = item.MEDICINE_TYPE_NAME;
                            dMediStock1ADO.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                            dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                            dMediStock1ADO.RANK = item.RANK;
                            dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                            dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                            dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                            dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                            dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                            dMediStock1ADO.TUTORIAL = item.TUTORIAL;
                            dMediStock1ADO.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                            dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                            dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                            dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                            dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                            dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                            dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                            var medicineTpe = currentMedicineTypeTemps.FirstOrDefault(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE);
                            if (medicineTpe != null)
                            {
                                dMediStock1ADO.ATC_CODES = medicineTpe.ATC_CODES;
                                dMediStock1ADO.ATC_GROUP_CODES = medicineTpe.ATC_GROUP_CODES;
                            }
                            if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                var maty = currentMaterialTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();
                                dMediStock1ADO.IS_FILM = (maty != null ? maty.IS_FILM : null);
                            }

                            dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                            dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                            MestMetyUnitWorker.UpdateUnit(dMediStock1ADO);
                            dMediStock1ADO.AMOUNT = ((dMediStock1ADO.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;

                            SetAmountOddByKeyShowRoundAvailableAmount(ref dMediStock1ADO, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);
                            result.Add(dMediStock1ADO);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        ///Hiện tại: Hệ thống chưa có luồng xử lý đối với các thuốc, vật tư hết hạn sử dụng khi kê đơn.
        ///Mong muốn:
        ///Đối với các thuốc, vật tư có hạn sử dụng nhỏ hơn ngày y lệnh thì không hiển thị lên tại màn hình kê đơn.
        ///#17940
        /// </summary>
        private void ProcessFilterDontPresExpiredTime(ref MOS.Filter.DHisMediStock2Filter filter)
        {
            try
            {
                //- Bổ sung cấu hình hệ thống: "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM": "1: Không cho phép kê thuốc/vật tư hết hạn sử dụng."
                //- Sửa chức năng "Kê đơn dược", "Kê đơn YHCT", "Tủ trực":
                //Khi bật cấu hình trên (giá trị = 1), thì khi load thông tin tồn kho để người dùng chọn, sẽ lấy thông tin tồn kho của các thuốc ko có hạn sử dụng hoặc hạn sử dụng thỏa mãn: 
                //+ Nếu là kê đơn từng ngày, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh.
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo cả đơn, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh của ngày lớn nhất được chọn
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo từng thuốc, thì HSD phải lớn hơn hoặc bằng thời gian chỉ định của đơn 

                //(server bổ sung filter để cho phép lấy thông tin tồn theo ngày)
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //if (HisConfigCFG.ManyDayPrescriptionOption == 2)
                    //{
                    //    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        ///Hiện tại: Hệ thống chưa có luồng xử lý đối với các thuốc, vật tư hết hạn sử dụng khi kê đơn.
        ///Mong muốn:
        ///Đối với các thuốc, vật tư có hạn sử dụng nhỏ hơn ngày y lệnh thì không hiển thị lên tại màn hình kê đơn.
        ///#17940
        /// </summary>
        private void ProcessFilterDontPresExpiredTime(ref MOS.Filter.DHisMediStock1Filter filter)
        {
            try
            {
                //- Bổ sung cấu hình hệ thống: "MOS.HIS_MEDI_STOCK.DONT_PRES_EXPIRED_ITEM": "1: Không cho phép kê thuốc/vật tư hết hạn sử dụng."
                //- Sửa chức năng "Kê đơn dược", "Kê đơn YHCT", "Tủ trực":
                //Khi bật cấu hình trên (giá trị = 1), thì khi load thông tin tồn kho để người dùng chọn, sẽ lấy thông tin tồn kho của các thuốc ko có hạn sử dụng hoặc hạn sử dụng thỏa mãn: 
                //+ Nếu là kê đơn từng ngày, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh.
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo cả đơn, thì hạn sử dụng phải lớn hơn hoặc bằng thời gian y lệnh của ngày lớn nhất được chọn
                //+ Nếu là kê đơn nhiều ngày, và cấu hình chọn nhiều ngày theo từng thuốc, thì HSD phải lớn hơn hoặc bằng thời gian chỉ định của đơn 

                //(server bổ sung filter để cho phép lấy thông tin tồn theo ngày)
                if (HisConfigCFG.IsDontPresExpiredTime)
                {
                    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //if (HisConfigCFG.ManyDayPrescriptionOption == 2)
                    //{
                    //    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //}
                    //else
                    //{
                    //    filter.EXPIRED_DATE__NULL_OR_GREATER_THAN_OR_EQUAL = this.intructionTimeSelecteds.OrderByDescending(o => o).FirstOrDefault();
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetAmountOddByKeyShowRoundAvailableAmount(ref DMediStock1ADO dMediStock1ADO, V_HIS_MEDICINE_TYPE mety, List<V_HIS_MEDICINE_TYPE> medicineTypes, List<V_HIS_MATERIAL_TYPE> materialTypes)
        {
            try
            {
                if (!GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet) return;//|| GlobalStore.IsExecutePTTT
                if (!HisConfigCFG.InPatientPrescription__ShowRoundAvailableAmount) return;

                long? serviceId = dMediStock1ADO.SERVICE_ID;
                long? id = dMediStock1ADO.ID;
                decimal? serviceTypeId = dMediStock1ADO.SERVICE_TYPE_ID;
                decimal? amount = dMediStock1ADO.AMOUNT;

                if (amount == (long)amount) return;

                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    if (mety == null)
                        mety = medicineTypes.Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    if (mety != null && (mety.IS_ALLOW_EXPORT_ODD ?? -1) != 1)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MEDICINE_TYPE_NAME", dMediStock1ADO.MEDICINE_TYPE_NAME) + "____" + Inventec.Common.Logging.LogUtil.TraceData("AMOUNT", dMediStock1ADO.AMOUNT));
                        dMediStock1ADO.AMOUNT = (long)dMediStock1ADO.AMOUNT;
                    }
                }
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                {
                    var maty = materialTypes.Where(o => o.SERVICE_ID == serviceId).FirstOrDefault();
                    if (maty != null && (maty.IS_ALLOW_EXPORT_ODD ?? -1) != 1)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("MEDICINE_TYPE_NAME", dMediStock1ADO.MEDICINE_TYPE_NAME) + "____" + Inventec.Common.Logging.LogUtil.TraceData("AMOUNT", dMediStock1ADO.AMOUNT));
                        dMediStock1ADO.AMOUNT = (long)dMediStock1ADO.AMOUNT;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateMedicineUseFormInDataRow(MediMatyTypeADO medicineTypeSDO)
        {
            try
            {
                bool hasUseForm = false;
                if (medicineTypeSDO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    V_HIS_MEDICINE_TYPE mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.SERVICE_ID == medicineTypeSDO.SERVICE_ID);
                    if (mety != null)
                    {
                        medicineTypeSDO.MEDICINE_USE_FORM_ID = mety.MEDICINE_USE_FORM_ID;
                        medicineTypeSDO.MEDICINE_USE_FORM_CODE = mety.MEDICINE_USE_FORM_CODE;
                        medicineTypeSDO.MEDICINE_USE_FORM_NAME = mety.MEDICINE_USE_FORM_NAME;
                        hasUseForm = true;
                    }
                }
                if (!hasUseForm)
                {
                    medicineTypeSDO.MEDICINE_USE_FORM_ID = null;
                    medicineTypeSDO.MEDICINE_USE_FORM_CODE = "";
                    medicineTypeSDO.MEDICINE_USE_FORM_NAME = "";
                    medicineTypeSDO.ErrorMessageMedicineUseForm = "";
                    medicineTypeSDO.ErrorTypeMedicineUseForm = ErrorType.None;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
