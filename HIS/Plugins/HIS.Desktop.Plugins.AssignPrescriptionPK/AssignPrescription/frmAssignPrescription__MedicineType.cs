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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<MedicineMaterialTypeComboADO> mediMateTypeComboADOs = new List<MedicineMaterialTypeComboADO>();
        internal void RebuildMedicineTypeWithInControlContainer()
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col0 = new DevExpress.XtraGrid.Columns.GridColumn();
                col0.FieldName = "IsAssignPresed";
                col0.Caption = " ";
                col0.Width = 25;
                col0.MaxWidth = 25;
                col0.MinWidth = 25;
                col0.VisibleIndex = 0;
                col0.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col0);

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

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "PARENT_NAME";
                col13.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col13.Width = 100;
                col13.VisibleIndex = 8;
                gridViewMediMaty.Columns.Add(col13);

                if (HisConfigCFG.GroupOption)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col17 = new DevExpress.XtraGrid.Columns.GridColumn();
                    col17.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); ;
                    col17.FieldName = "PARENT_NAME";
                    //col17.GroupIndex = 0;
                    col17.OptionsColumn.AllowEdit = false;
                    col17.VisibleIndex = 17;
                    col17.Width = 180;
                    gridViewMediMaty.Columns.Add(col17);
                    this.gridViewMediMaty.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(col17, DevExpress.Data.ColumnSortOrder.Ascending)});
                    this.gridViewMediMaty.GroupCount = 1;
                    this.gridViewMediMaty.OptionsBehavior.AutoExpandAllGroups = true;
                }

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

                this.currentMediMateTypeComboADOs = BackendDataWorker.Get<MedicineMaterialTypeComboADO>(false, true, false, false);
                mediMateTypeComboADOs = new List<MedicineMaterialTypeComboADO>();
                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                long roomId = GetRoomId();
                V_HIS_ROOM room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == roomId);
                if (room != null && (room.IS_RESTRICT_MEDICINE_TYPE ?? 0) == 1)
                {
                    List<HIS_MEDICINE_TYPE_ROOM> medicineTypeRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_MEDICINE_TYPE_ROOM>();
                    medicineTypeRooms = (medicineTypeRooms != null && medicineTypeRooms.Count > 0) ? medicineTypeRooms.Where(o => o.ROOM_ID == roomId).ToList() : null;
                    List<long> medicineTypeIdRooms = medicineTypeRooms != null ? medicineTypeRooms.Select(o => o.MEDICINE_TYPE_ID).ToList() : new List<long>();
                    mediMateTypeComboADOs = (currentMediMateTypeComboADOs != null && medicineTypeIdRooms != null && medicineTypeIdRooms.Count > 0) ? this.currentMediMateTypeComboADOs.Where(o => medicineTypeIdRooms.Contains(o.ID)
                        || o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList() : null;
                }
                else
                    mediMateTypeComboADOs.AddRange(currentMediMateTypeComboADOs);

                long isOnlyDisplayMediMateIsBusiness = Inventec.Common.TypeConvert.Parse.ToInt64(HisConfigs.Get<string>(HisConfigCFG.ONLY_DISPLAY_MEDIMATE_IS_BUSINESS));
                if (isOnlyDisplayMediMateIsBusiness == 1 && mediMateTypeComboADOs != null && mediMateTypeComboADOs.Count > 0)
                    mediMateTypeComboADOs = mediMateTypeComboADOs.Where(o => o.IS_BUSINESS.HasValue && o.IS_BUSINESS.Value == 1).ToList();

                if (chkPDDT.Checked)
                {
                    List<HIS_ICD_SERVICE> icdServices = geticdServices();
                    List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID != null).Select(o => o.SERVICE_ID.Value).ToList();
                    List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID != null).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();

                    List<long> medicineTypeIds = mediMateTypeComboADOs.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID)).Select(o => o.ID).ToList();

                    List<MedicineMaterialTypeComboADO> MedicineMaterialServiceConfig = new List<MedicineMaterialTypeComboADO>();
                    List<MedicineMaterialTypeComboADO> MedicineMaterialAcgrConfig = new List<MedicineMaterialTypeComboADO>();

                    MedicineMaterialServiceConfig = mediMateTypeComboADOs.Where(o => serviceIdTmps.Contains(o.SERVICE_ID)).ToList();

                    var medicineTypeAcinByMety = ValidAcinInteractiveWorker.GetMedicineTypeAcinByMedicineType(medicineTypeIds);
                    if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0)
                    {
                        List<long> medicineTypeIdAcinByMetys = medicineTypeAcinByMety.Where(o => acingrIdTmps.Contains(o.ACTIVE_INGREDIENT_ID)).Select(p => p.MEDICINE_TYPE_ID).ToList();
                        MedicineMaterialAcgrConfig = mediMateTypeComboADOs.Where(o => medicineTypeIdAcinByMetys.Contains(o.ID)).ToList();
                    }
                    mediMateTypeComboADOs = new List<MedicineMaterialTypeComboADO>();
                    mediMateTypeComboADOs.AddRange(MedicineMaterialServiceConfig);
                    mediMateTypeComboADOs.AddRange(MedicineMaterialAcgrConfig);
                }

                if (HisConfigCFG.GroupOption)
                {
                    mediMateTypeComboADOs = mediMateTypeComboADOs.OrderBy(o => o.PARENT_NAME).ToList();
                }
                long index = 1;
                mediMateTypeComboADOs.ForEach(o => o.IdRow = index++);
                gridViewMediMaty.GridControl.DataSource = mediMateTypeComboADOs;
                //TickIsAssignPres();
                gridViewMediMaty.EndUpdate();

                LogSystem.Debug("RebuildMedicineTypeWithInControlContainer__Du lieu thuoc/vat tu____ " + (mediMateTypeComboADOs != null ? mediMateTypeComboADOs.Count : 0));
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        internal async Task RebuildNhaThuocMediMatyWithInControlContainerAsync()
        {
            try
            {
                if (currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0)
                {
                    var mediStockIds = currentMediStockNhaThuocSelecteds.Select(o => o.ID).ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    this.ProcessFilterDontPresExpiredTime(ref filter);//TODO

                    this.mediMatyTypeAvailables = await new BackendAdapter(param).GetAsync<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    this.mediStockD1ADOs = ConvertToDMediStockForNhaThuoc(this.mediMatyTypeAvailables);
                }

                this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildNhaThuocMediMatyWithInControlContainerWithConfig()
        {
            try
            {
                var mediStockIds = this.mediStockAllows.Select(o => o.ID).ToList();
                CommonParam param = new CommonParam();
                MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                filter.MEDI_STOCK_IDs = mediStockIds;
                this.ProcessFilterDontPresExpiredTime(ref filter);//TODO

                this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                this.mediStockD1ADOs = ConvertToDMediStockForNhaThuoc(this.mediMatyTypeAvailables);

                this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildNhaThuocMediMatyWithInControlContainer()
        {
            try
            {
                if ((this.currentMediStockNhaThuocSelecteds != null && this.currentMediStockNhaThuocSelecteds.Count > 0))
                {
                    var mediStockIds = this.currentMediStockNhaThuocSelecteds.Select(o => o.ID).ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock1Filter filter = new MOS.Filter.DHisMediStock1Filter();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    this.ProcessFilterDontPresExpiredTime(ref filter);//TODO

                    this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    this.mediStockD1ADOs = ConvertToDMediStockForNhaThuoc(this.mediMatyTypeAvailables);
                }

                this.RebuildPopupContainerNhaThuocShowMediMatyForSelect(this.mediStockD1ADOs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RebuildPopupContainerNhaThuocShowMediMatyForSelect(List<DMediStock1ADO> dMediStock1ADOs)
        {
            try
            {
                gridViewMediMaty.BeginUpdate();
                gridViewMediMaty.Columns.Clear();
                popupControlContainerMediMaty.Width = theRequiredWidth;
                popupControlContainerMediMaty.Height = theRequiredHeight;

                DevExpress.XtraGrid.Columns.GridColumn col0 = new DevExpress.XtraGrid.Columns.GridColumn();
                col0.FieldName = "IsAssignPresed";
                col0.Caption = " ";
                col0.Width = 25;
                col0.MaxWidth = 25;
                col0.MinWidth = 25;
                col0.VisibleIndex = 0;
                col0.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col0);

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

                DevExpress.XtraGrid.Columns.GridColumn col9 = new DevExpress.XtraGrid.Columns.GridColumn();
                col9.FieldName = "MANUFACTURER_NAME";
                col9.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col9.Width = 150;
                col9.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col9);


                DevExpress.XtraGrid.Columns.GridColumn col4 = new DevExpress.XtraGrid.Columns.GridColumn();
                col4.FieldName = "AMOUNT_DISPLAY";
                col4.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col4.Width = 70;
                col4.VisibleIndex = 5;
                col4.DisplayFormat.FormatString = "#,##0.000000";
                col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col4);

                DevExpress.XtraGrid.Columns.GridColumn col5 = new DevExpress.XtraGrid.Columns.GridColumn();
                col5.FieldName = "IMP_PRICE_DISPLAY";
                col5.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_IMP_PRICE_DISPLAY",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col5.Width = 100;
                col5.VisibleIndex = 6;
                col5.DisplayFormat.FormatString = "#,##0.0000";
                col5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                col5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col5);

                DevExpress.XtraGrid.Columns.GridColumn col6 = new DevExpress.XtraGrid.Columns.GridColumn();
                col6.FieldName = "IMP_VAT_RATIO_DISPLAY";
                col6.Caption = "VAT(%)";
                col6.Width = 100;
                col6.VisibleIndex = 7;
                col6.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                gridViewMediMaty.Columns.Add(col6);



                DevExpress.XtraGrid.Columns.GridColumn col10 = new DevExpress.XtraGrid.Columns.GridColumn();
                col10.FieldName = "NATIONAL_NAME";
                col10.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_NATIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col10.Width = 80;
                col10.VisibleIndex = 8;
                gridViewMediMaty.Columns.Add(col10);



                DevExpress.XtraGrid.Columns.GridColumn col1 = new DevExpress.XtraGrid.Columns.GridColumn();
                col1.FieldName = "MEDICINE_TYPE_CODE";
                col1.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MEDICINE_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col1.Width = 60;
                col1.VisibleIndex = 9;
                gridViewMediMaty.Columns.Add(col1);

                DevExpress.XtraGrid.Columns.GridColumn col15 = new DevExpress.XtraGrid.Columns.GridColumn();
                col15.FieldName = "PARENT_NAME";
                col15.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col15.Width = 100;
                col15.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col15);

                if (HisConfigCFG.GroupOption)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col17 = new DevExpress.XtraGrid.Columns.GridColumn();
                    col17.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); ;
                    col17.FieldName = "PARENT_NAME";
                    //col17.GroupIndex = 0;
                    col17.OptionsColumn.AllowEdit = false;
                    col17.VisibleIndex = 17;
                    col17.Width = 180;
                    gridViewMediMaty.Columns.Add(col17);
                    this.gridViewMediMaty.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(col17, DevExpress.Data.ColumnSortOrder.Ascending)});
                    this.gridViewMediMaty.GroupCount = 1;
                    this.gridViewMediMaty.OptionsBehavior.AutoExpandAllGroups = true;
                }

                //Phuc vu cho tim kiem khong dau

                DevExpress.XtraGrid.Columns.GridColumn col11 = new DevExpress.XtraGrid.Columns.GridColumn();
                col11.FieldName = "MEDICINE_TYPE_CODE__UNSIGN";
                col11.Width = 80;
                col11.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col11);

                DevExpress.XtraGrid.Columns.GridColumn col12 = new DevExpress.XtraGrid.Columns.GridColumn();
                col12.FieldName = "MEDICINE_TYPE_NAME__UNSIGN";
                col12.Width = 80;
                col12.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                col12.VisibleIndex = -1;
                gridViewMediMaty.Columns.Add(col12);

                DevExpress.XtraGrid.Columns.GridColumn col13 = new DevExpress.XtraGrid.Columns.GridColumn();
                col13.FieldName = "ACTIVE_INGR_BHYT_NAME__UNSIGN";
                col13.VisibleIndex = -1;
                col13.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Like;
                gridViewMediMaty.Columns.Add(col13);

                DevExpress.XtraGrid.Columns.GridColumn col14 = new DevExpress.XtraGrid.Columns.GridColumn();
                col14.FieldName = "MEDI_STOCK_NAME";
                col14.Caption = "Nhà thuốc";
                col14.Width = 100;
                col14.VisibleIndex = 10;
                gridViewMediMaty.Columns.Add(col14);

                if (chkPDDT.Checked)
                {
                    List<HIS_ICD_SERVICE> icdServices = geticdServices();
                    List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID != null).Select(o => o.SERVICE_ID.Value).ToList();
                    List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID != null).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();

                    List<string> medicineTypeCodes = dMediStock1ADOs.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID.Value)).Select(o => o.MEDICINE_TYPE_CODE).ToList();

                    List<DMediStock1ADO> MediStock1ServiceConfig = new List<DMediStock1ADO>();
                    List<DMediStock1ADO> MediStock1AcgrConfig = new List<DMediStock1ADO>();

                    MediStock1ServiceConfig = dMediStock1ADOs.Where(o => serviceIdTmps.Contains(o.SERVICE_ID.Value)).ToList();

                    var medicineTypeAcinByMety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeCodes.Contains(o.MEDICINE_TYPE_CODE)).ToList(); ;
                    if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0)
                    {
                        List<string> medicineTypeIdAcinByMetys = medicineTypeAcinByMety.Where(o => acingrIdTmps.Contains(o.ACTIVE_INGREDIENT_ID)).Select(p => p.MEDICINE_TYPE_CODE).ToList();
                        MediStock1AcgrConfig = dMediStock1ADOs.Where(o => medicineTypeIdAcinByMetys.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    }
                    dMediStock1ADOs = new List<DMediStock1ADO>();
                    dMediStock1ADOs.AddRange(MediStock1ServiceConfig);
                    dMediStock1ADOs.AddRange(MediStock1AcgrConfig);
                }

                if (HisConfigCFG.GroupOption)
                {
                    dMediStock1ADOs = dMediStock1ADOs.OrderBy(o => o.PARENT_NAME).ToList();
                }
                long index = 1;
                dMediStock1ADOs.ForEach(o => o.IdRow = index++);
                gridViewMediMaty.GridControl.DataSource = dMediStock1ADOs;
                //TickIsAssignPres();
                gridViewMediMaty.EndUpdate();

                LogSystem.Debug("RebuildPopupContainerNhaThuocShowMediMatyForSelect__Du lieu thuoc/vat tu____ " + (dMediStock1ADOs != null ? dMediStock1ADOs.Count : 0));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void MedicineType_RowClick(object data)
        {
            try
            {
                this.currentMedicineTypeADOForEdit = new MediMatyTypeADO();
                if (data != null)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<MediMatyTypeADO>(this.currentMedicineTypeADOForEdit, data);

                    if (GetSelectedOpionGroup() == 1 && (this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                    {
                        this.currentMedicineTypeADOForEdit.IS_SUB_PRES = 1;
                        if (this.oldServiceReq != null && this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES != 1)
                        {
                            this.currentMedicineTypeADOForEdit.IS_SUB_PRES = null;
                        }
                    }
                    if (currentMedicineTypeADOForEdit.HTU_IDs != null && currentMedicineTypeADOForEdit.HTU_IDs.Count > 0)
                    {
                        if (DataHtuList != null && DataHtuList.Count > 0)
                        {
                            DataHtuList.ForEach(o =>
                            {
                                o.IsChecked = currentMedicineTypeADOForEdit.HTU_IDs.Exists(p => p == o.ID);
                            });
                            this.cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                        }
                        this.cboHtu.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        DataHtuList.ForEach(o => o.IsChecked = false);
                        this.cboHtu.Text = null;
                        this.cboHtu.Properties.Buttons[1].Visible = false;
                    }
                    if (CheckExistMedicinePaymentLimit(this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_CODE))
                    {
                        MessageBox.Show(ResourceMessage.ThuocCoGioiHanChiDinhThanhToanBHYTDeNghiBSXemXetTruocKhiKe, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.btnAdd.Enabled = true;
                    this.btnAddTutorial.Enabled = true;
                    this.spinSang.EditValue = null;
                    this.spinTrua.EditValue = null;
                    this.spinChieu.EditValue = null;
                    this.spinToi.EditValue = null;
                    this.cboMedicineUseForm.EditValue = null;
                    this.spinAmount.Text = "";
                    if (this.spinSoNgay.Value > 0)
                        this.spinSoLuongNgay.Value = this.spinSoNgay.Value;

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;

                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        this.VisibleInputControl(!(currentMedicineTypeADOForEdit.IS_OXYGEN == GlobalVariables.CommonNumberTrue));

                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = true;

                        this.FillDataIntoMedicineUseFormAndTutorial(this.currentMedicineTypeADOForEdit.ID);

                        //Neu la thuoc thi kiem tra co mẫu HDSD chưa, có thì focus vào nút "Bổ sung"
                        if (this.medicineTypeTutSelected != null && !String.IsNullOrEmpty(this.medicineTypeTutSelected.TUTORIAL))
                        {
                            Inventec.Common.Logging.LogSystem.Warn("MedicineType_RowClick");
                            this.btnAdd.Focus();
                        }
                        //Ngược lại kiểm tra có cấu hình PM cho phép sau khi chọn thuốc thì nhảy vào ô số lượng hay ô ngày
                        else
                        {
                            long focusMedicineDefault = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__ASSIGN_PRESCRIPTION__FOCUS_MEDICINE_DEFAULT);
                            //Nếu có cấu hình thì mặc định focus vào ô số lượng
                            if (focusMedicineDefault == 1)
                            {
                                this.spinAmount.Focus();
                                this.spinAmount.SelectAll();
                            }
                            //Ngược lại thì mặc định focus vào ô số ngày
                            else
                            {
                                this.spinSoLuongNgay.Focus();
                                this.spinSoLuongNgay.SelectAll();
                            }
                        }
                    }
                    else if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        VisibleInputControl(true);
                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = false;
                        this.spinTocDoTruyen.EditValue = null;

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
