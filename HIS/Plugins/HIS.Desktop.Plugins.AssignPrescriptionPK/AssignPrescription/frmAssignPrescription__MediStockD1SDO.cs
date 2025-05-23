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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Worker;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        List<DMediStock1ADO> dMediStock1s = new List<DMediStock1ADO>();
        internal async Task RebuildMediMatyWithInControlContainer(bool isTSD = false)
        {
            try
            {
                this.InitDataMetyMatyTypeInStockD();

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                dMediStock1s = new List<DMediStock1ADO>();
                dMediStock1s.AddRange(this.mediStockD1ADOs);

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


                if (isTSD)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col1a = new DevExpress.XtraGrid.Columns.GridColumn();
                    col1a.FieldName = "SERIAL_NUMBER";
                    col1a.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_SERIAL_NUMBER",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col1a.Width = 150;
                    col1a.VisibleIndex = 3;
                    gridViewMediMaty.Columns.Add(col1a);

                    DevExpress.XtraGrid.Columns.GridColumn col4a = new DevExpress.XtraGrid.Columns.GridColumn();
                    col4a.FieldName = "USE_COUNT_DISPLAY";
                    col4a.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_USE_COUNT__USE_REMAIN_COUNT",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col4a.Width = 140;
                    col4a.VisibleIndex = 4;
                    gridViewMediMaty.Columns.Add(col4a);
                }
                else
                {
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
                    col4.FieldName = "AMOUNT_DISPLAY";
                    col4.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col4.Width = 90;
                    col4.VisibleIndex = 5;
                    col4.DisplayFormat.FormatString = "#,##0.0000";
                    col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    col4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    gridViewMediMaty.Columns.Add(col4);
                }


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
                col5.DisplayFormat.FormatString = "#,##0.0000";
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

                DevExpress.XtraGrid.Columns.GridColumn col16 = new DevExpress.XtraGrid.Columns.GridColumn();
                col16.FieldName = "PARENT_NAME";
                col16.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col16.Width = 100;
                col16.VisibleIndex = 15;
                gridViewMediMaty.Columns.Add(col16);

                DevExpress.XtraGrid.Columns.GridColumn col15 = new DevExpress.XtraGrid.Columns.GridColumn();
                col15.FieldName = "MATERIAL_TYPE_MAP_NAME";
                col15.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MATERIAL_TYPE_MAP_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col15.Width = 150;
                col15.VisibleIndex = 16;
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

                if (chkPDDT.Checked)
                {
                    List<HIS_ICD_SERVICE> icdServices = geticdServices();
                    List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID != null).Select(o => o.SERVICE_ID.Value).ToList();
                    List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID != null).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();

                    List<string> medicineTypeCodes = dMediStock1s.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID.Value)).Select(o => o.MEDICINE_TYPE_CODE).ToList();

                    List<DMediStock1ADO> MediStock1ServiceConfig = new List<DMediStock1ADO>();
                    List<DMediStock1ADO> MediStock1AcgrConfig = new List<DMediStock1ADO>();

                    MediStock1ServiceConfig = dMediStock1s.Where(o => serviceIdTmps.Contains(o.SERVICE_ID.Value)).ToList();

                    var medicineTypeAcinByMety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeCodes.Contains(o.MEDICINE_TYPE_CODE)).ToList(); ;
                    if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0)
                    {
                        List<string> medicineTypeIdAcinByMetys = medicineTypeAcinByMety.Where(o => acingrIdTmps.Contains(o.ACTIVE_INGREDIENT_ID)).Select(p => p.MEDICINE_TYPE_CODE).ToList();
                        MediStock1AcgrConfig = dMediStock1s.Where(o => medicineTypeIdAcinByMetys.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    }
                    dMediStock1s = new List<DMediStock1ADO>();
                    dMediStock1s.AddRange(MediStock1ServiceConfig);
                    dMediStock1s.AddRange(MediStock1AcgrConfig);
                }

                if (HisConfigCFG.GroupOption)
                {
                    dMediStock1s = dMediStock1s.OrderBy(o => o.PARENT_NAME).ToList();
                }
                long index = 1;
                dMediStock1s.ForEach(o => o.IdRow = index++);
                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                //TickIsAssignPres();
                gridViewMediMaty.EndUpdate();
                //Inventec.Common.Logging.LogSystem.Info(" RebuildMediMatyWithInControlContainer: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dMediStock1s), dMediStock1s));
                //LogSystem.Debug("RebuildMediMatyWithInControlContainer__Du lieu thuoc/vat tu____ " + (dMediStock1s != null ? dMediStock1s.Count : 0));
            }
            catch (Exception ex)
            {
                gridViewMediMaty.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal async Task RebuildMediMatyWithInControlContainerAsync(bool isTSD = false)
        {
            try
            {
                await this.InitDataMetyMatyTypeInStockDAsync(this.currentMediStock);

                //Tại màn hình kê đơn, nếu phòng mà người dùng đang làm việc có "Giới hạn thuốc được phép sử dụng" (IS_RESTRICT_MEDICINE_TYPE trong HIS_ROOM bằng true) thì danh sách thuốc khi kê thuốc trong kho chỉ hiển thị các thuốc được khai cấu hình tương ứng với phòng đấy (dữ liệu lưu trong bảng HIS_MEDICINE_TYPE_ROOM)
                dMediStock1s = new List<DMediStock1ADO>();
                dMediStock1s.AddRange(this.mediStockD1ADOs);

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


                if (isTSD)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col1a = new DevExpress.XtraGrid.Columns.GridColumn();
                    col1a.FieldName = "SERIAL_NUMBER";
                    col1a.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_SERIAL_NUMBER",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col1a.Width = 150;
                    col1a.VisibleIndex = 3;
                    gridViewMediMaty.Columns.Add(col1a);

                    DevExpress.XtraGrid.Columns.GridColumn col4a = new DevExpress.XtraGrid.Columns.GridColumn();
                    col4a.FieldName = "USE_COUNT_DISPLAY";
                    col4a.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_USE_COUNT__USE_REMAIN_COUNT",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col4a.Width = 140;
                    col4a.VisibleIndex = 4;
                    gridViewMediMaty.Columns.Add(col4a);
                }
                else
                {
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
                    col4.FieldName = "AMOUNT_DISPLAY";
                    col4.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GC_AVAILABLE_AMOUNT",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    col4.Width = 90;
                    col4.VisibleIndex = 5;
                    col4.DisplayFormat.FormatString = "#,##0.000000";
                    col4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    col4.UnboundType = DevExpress.Data.UnboundColumnType.Object;
                    gridViewMediMaty.Columns.Add(col4);
                }


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
                col5.DisplayFormat.FormatString = "#,##0.0000";
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

                DevExpress.XtraGrid.Columns.GridColumn col17 = new DevExpress.XtraGrid.Columns.GridColumn();
                col17.FieldName = "PARENT_NAME";
                col17.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col17.Width = 100;
                col17.VisibleIndex = 15;
                gridViewMediMaty.Columns.Add(col17);

                DevExpress.XtraGrid.Columns.GridColumn col16 = new DevExpress.XtraGrid.Columns.GridColumn();
                col16.FieldName = "MATERIAL_TYPE_MAP_NAME";
                col16.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MATERIAL_TYPE_MAP_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col16.Width = 150;
                col16.VisibleIndex = 16;
                gridViewMediMaty.Columns.Add(col16);

                DevExpress.XtraGrid.Columns.GridColumn col15 = new DevExpress.XtraGrid.Columns.GridColumn();
                col15.FieldName = "MATERIAL_TYPE_MAP_NAME";
                col15.Caption = Inventec.Common.Resource.Get.Value
                    ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_MATERIAL_TYPE_MAP_NAME",
                    Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                col15.Width = 150;
                col15.VisibleIndex = 17;
                gridViewMediMaty.Columns.Add(col15);

                if (HisConfigCFG.GroupOption)
                {
                    DevExpress.XtraGrid.Columns.GridColumn col18 = new DevExpress.XtraGrid.Columns.GridColumn();
                    col18.Caption = Inventec.Common.Resource.Get.Value
                        ("IVT_LANGUAGE_KEY__UC_HIS_ASSIGN_PRESCRIPTION__GV_MEDICINE__GC_PARENT_NAME",
                        Resources.ResourceLanguageManager.LanguagefrmAssignPrescription,
                        Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()); ;
                    col18.FieldName = "PARENT_NAME";
                    //col18.GroupIndex = 0;
                    col18.OptionsColumn.AllowEdit = false;
                    col18.VisibleIndex = 17;
                    col18.Width = 180;
                    gridViewMediMaty.Columns.Add(col18);
                    this.gridViewMediMaty.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(col18, DevExpress.Data.ColumnSortOrder.Ascending)});
                    this.gridViewMediMaty.GroupCount = 1;
                    this.gridViewMediMaty.OptionsBehavior.AutoExpandAllGroups = true;
                }

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

                if (chkPDDT.Checked)
                {
                    List<HIS_ICD_SERVICE> icdServices = geticdServices();
                    List<long> serviceIdTmps = icdServices.Where(o => o.SERVICE_ID != null).Select(o => o.SERVICE_ID.Value).ToList();
                    List<long> acingrIdTmps = icdServices.Where(o => o.ACTIVE_INGREDIENT_ID != null).Select(o => o.ACTIVE_INGREDIENT_ID.Value).ToList();

                    List<string> medicineTypeCodes = dMediStock1s.Where(o => !serviceIdTmps.Contains(o.SERVICE_ID.Value)).Select(o => o.MEDICINE_TYPE_CODE).ToList();

                    List<DMediStock1ADO> MediStock1ServiceConfig = new List<DMediStock1ADO>();
                    List<DMediStock1ADO> MediStock1AcgrConfig = new List<DMediStock1ADO>();

                    MediStock1ServiceConfig = dMediStock1s.Where(o => serviceIdTmps.Contains(o.SERVICE_ID.Value)).ToList();

                    var medicineTypeAcinByMety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE_ACIN>()
                    .Where(o => medicineTypeCodes.Contains(o.MEDICINE_TYPE_CODE)).ToList(); ;
                    if (medicineTypeAcinByMety != null && medicineTypeAcinByMety.Count > 0)
                    {
                        List<string> medicineTypeIdAcinByMetys = medicineTypeAcinByMety.Where(o => acingrIdTmps.Contains(o.ACTIVE_INGREDIENT_ID)).Select(p => p.MEDICINE_TYPE_CODE).ToList();
                        MediStock1AcgrConfig = dMediStock1s.Where(o => medicineTypeIdAcinByMetys.Contains(o.MEDICINE_TYPE_CODE)).ToList();
                    }
                    dMediStock1s = new List<DMediStock1ADO>();
                    dMediStock1s.AddRange(MediStock1ServiceConfig);
                    dMediStock1s.AddRange(MediStock1AcgrConfig);
                }

                if (HisConfigCFG.GroupOption)
                {
                    dMediStock1s = dMediStock1s.OrderBy(o => o.PARENT_NAME).ToList();
                }
                long index = 1;
                dMediStock1s.ForEach(o => o.IdRow = index++);
                gridViewMediMaty.GridControl.DataSource = dMediStock1s;
                //TickIsAssignPres();
                gridViewMediMaty.EndUpdate();

                LogSystem.Debug("RebuildMediMatyWithInControlContainerAsync__Du lieu thuoc/vat tu____ " + (this.mediStockD1ADOs != null ? this.mediStockD1ADOs.Count : 0));
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

                    if (GetSelectedOpionGroup() == 1 && (this.serviceReqMain != null && this.serviceReqMain.IS_MAIN_EXAM != 1) && (HisConfigCFG.IsUsingSubPrescriptionMechanism == "1") && !GlobalStore.IsCabinet)
                    {
                        this.currentMedicineTypeADOForEdit.IS_SUB_PRES = 1;
                        if (this.oldServiceReq != null && this.serviceReqMain != null && this.serviceReqMain.IS_SUB_PRES != 1)
                        {
                            this.currentMedicineTypeADOForEdit.IS_SUB_PRES = null;
                        }
                    }
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
                    this.btnAddTutorial.Enabled = true;
                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;
                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var metyAlls = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>();
                        var metys = (metyAlls != null && metyAlls.Count > 0) ? metyAlls.Where(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID).ToList() : null;
                        var mety = (metys != null && metys.Count > 0) ? metys.FirstOrDefault() : null;
                        if (mety != null)
                        {
                            this.currentMedicineTypeADOForEdit.IS_KIDNEY = mety.IS_KIDNEY;
                            if (mety.HTU_ID != null)
                                currentMedicineTypeADOForEdit.HTU_IDs = new List<long>() { mety.HTU_ID ?? 0 };
                        }
                        else
                        {
                            this.currentMedicineTypeADOForEdit.IS_KIDNEY = null;
                        }
                        if (currentMedicineTypeADOForEdit.HTU_IDs != null)
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
                        this.VisibleInputControl(!(mety != null && mety.IS_OXYGEN == GlobalVariables.CommonNumberTrue));

                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = true;

                        //nếu không có ngày theo HDSD thì gán theo số ngày đơn
                        if (this.spinSoNgay.Value > 0 && this.spinSoLuongNgay.Value <= 0)
                            this.spinSoLuongNgay.Value = this.spinSoNgay.Value;

                        this.FillDataIntoMedicineUseFormAndTutorial(currentMedicineTypeADOForEdit.ID);

                        //Neu la thuoc thi kiem tra co mẫu HDSD chưa, có thì focus vào nút "Bổ sung"
                        if (this.medicineTypeTutSelected != null && this.medicineTypeTutSelected.ID > 0)
                        {
                            //- Với kê tủ trực, kê thuốc điều trị:
                            //--nếu thuốc có hdsd => nhảy con trỏ vào ô số lượng sau khi chọn thuốc.
                            //--nếu không có hdsd => như kê đơn phòng khám
                            //if (GlobalStore.IsTreatmentIn || GlobalStore.IsCabinet || GlobalStore.IsExecutePTTT)
                            //{
                            //    spinAmount.Focus();
                            //    spinAmount.SelectAll();
                            //}
                            //else
                            {
                                //this.btnAdd.Focus();
                                this.txtTutorial.Focus();
                                this.txtTutorial.SelectionStart = txtTutorial.Text.Length + 1;
                                this.memHtu.SelectionStart = memHtu.Text.Length + 1;
                            }
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
                    else
                    {
                        //if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        //{
                        //    FillDataIntoMaterialTutorial(this.currentMedicineTypeADOForEdit.ID);
                        //}

                        this.VisibleInputControl(true);

                        if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                            this.lciTocDoTruyen.Enabled = false;
                        this.spinTocDoTruyen.EditValue = null;

                        //Neu la vat tu thi mặc định focus vào ô số lượng
                        this.spinAmount.Focus();
                        this.spinAmount.SelectAll();
                    }

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);    
                    SetPatientType();
                }
            }
            catch (Exception ex)      
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPatientType()
        {
            try
            {
                HIS_PATIENT_TYPE patientTypeDefault = ChoosePatientTypeDefaultlServiceOther(currentHisPatientTypeAlter.PATIENT_TYPE_ID,
                        this.currentMedicineTypeADOForEdit.SERVICE_ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);
                bool IsHasPatientType = false;
                var selectedOpionGroup = GetSelectedOpionGroup();
                if (this.serviceReqParentId != null && ListAncillaryServPaty != null && ListAncillaryServPaty.Count > 0)
                {
                    if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                    {
                        var sv = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID).FirstOrDefault();
                        if (sv != null && !string.IsNullOrEmpty(sv.HEIN_SERVICE_BHYT_CODE))
                        {
                            var acMety = ListAncillaryServPaty.Where(o => (o.CHILD_SERVICE_ID == sv.SERVICE_ID || (!string.IsNullOrEmpty(o.PREFIX_BHYT_CODE) && sv.HEIN_SERVICE_BHYT_CODE.StartsWith(o.PREFIX_BHYT_CODE))) && currentPatientTypeWithPatientTypeAlter.Exists(p => p.ID == o.PATIENT_TYPE_ID)).ToList();
                            if (acMety != null && acMety.Count > 0)
                            {
                                acMety = acMety.OrderByDescending(o => o.ID).ToList();
                                cboPatientType.EditValue = acMety.FirstOrDefault().PATIENT_TYPE_ID;
                                IsHasPatientType = true;
                            }
                        }
                    }
                    else if (this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                    {
                        var sv = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.SERVICE_ID == this.currentMedicineTypeADOForEdit.SERVICE_ID).FirstOrDefault();
                        if (sv != null && !string.IsNullOrEmpty(sv.HEIN_SERVICE_BHYT_CODE))
                        {
                            var acMaty = ListAncillaryServPaty.Where(o => (o.CHILD_SERVICE_ID == sv.SERVICE_ID || (!string.IsNullOrEmpty(o.PREFIX_BHYT_CODE) && sv.HEIN_SERVICE_BHYT_CODE.StartsWith(o.PREFIX_BHYT_CODE))) && currentPatientTypeWithPatientTypeAlter.Exists(p => p.ID == o.PATIENT_TYPE_ID)).ToList();
                            if (acMaty != null && acMaty.Count > 0)
                            {
                                acMaty = acMaty.OrderByDescending(o => o.ID).ToList();
                                cboPatientType.EditValue = acMaty.FirstOrDefault().PATIENT_TYPE_ID;
                                IsHasPatientType = true;
                            }
                        }
                    }
                }
                if (!IsHasPatientType)
                {
                    if (HisConfigCFG.DefaultPatientTypeOption && this.serviceReqParentId != null)
                    {
                        CommonParam param = new CommonParam();
                        HisSereServFilter filter = new HisSereServFilter();
                        filter.SERVICE_REQ_ID = this.serviceReqParentId;
                        var SereServ = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, param);
                        if (SereServ != null && SereServ.Count > 0)
                        {
                            cboPatientType.EditValue = SereServ.FirstOrDefault().PATIENT_TYPE_ID;
                        }
                        else if (patientTypeDefault != null)
                        {
                            cboPatientType.EditValue = patientTypeDefault.ID;
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Error("Khong tim thay doi tuong thanh toan mac dinh cho dinh vu");
                            cboPatientType.EditValue = null;
                        }
                    }
                    else if (patientTypeDefault != null)
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

        private void MaterialTypeTSD_RowClick(object data)
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
                    currentMedicineTypeADOForEdit.IsExpend = currentMedicineTypeADOForEdit.IS_AUTO_EXPEND == 1;
                    if (data.GetType() == typeof(DMediStock1ADO))
                    {
                        DMediStock1ADO dMediStock = data as DMediStock1ADO;
                        this.currentMedicineTypeADOForEdit.IsStent = ((dMediStock.IS_STENT ?? 0) == GlobalVariables.CommonNumberTrue ? true : false);
                    }
                    this.actionBosung = GlobalVariables.ActionAdd;
                    this.VisibleButton(this.actionBosung);
                    this.btnAdd.Enabled = true;
                    //this.btnAddTutorial.Enabled = true;
                    if (lciTocDoTruyen.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                        this.lciTocDoTruyen.Enabled = false;

                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidProviderBoXung__DuongDung, this.dxErrorProvider1);
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderMaterialTypeTSD, this.dxErrorProvider1);

                    this.txtMediMatyForPrescription.Text = this.currentMedicineTypeADOForEdit.MEDICINE_TYPE_NAME;

                    this.btnAdd.Focus();

                    Inventec.Common.Logging.LogSystem.Warn("MaterialTypeTSD_RowClick");
                    //this.currentMedicineTypeADOForEdit.IsAllowOdd = this.GetIsAllowOdd(this.currentMedicineTypeADOForEdit.ID, this.currentMedicineTypeADOForEdit.SERVICE_TYPE_ID);

                    ///Khoi tao cbo PatientType va set gia tri mac dinh theo service
                    FillDataIntoPatientTypeCombo(this.currentMedicineTypeADOForEdit, cboPatientType);
                    SetPatientType();
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
                            result = (maType.IS_ALLOW_ODD == 1) ? true : false;
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
                //Lấy dữ liệu cấu hình hướng dẫn sử dụng của thuốc (HIS_MEDICINE_TYPE_TUT) theo tài khoản đăng nhập và loại thuốc
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var medicineTypeTuts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDICINE_TYPE_TUT>();
                if (medicineTypeTuts != null && medicineTypeTuts.Count > 0)
                {
                    this.medicineTypeTutFilters = medicineTypeTuts.OrderByDescending(o => o.MODIFY_TIME).Where(o => o.MEDICINE_TYPE_ID == medicineTypeId).ToList();

                    this.RebuildTutorialWithInControlContainer(medicineTypeTutFilters);      
                    this.RebuildHtuWithInControlContainer(medicineTypeTutFilters);
                    this.medicineTypeTutSelected = medicineTypeTutFilters.FirstOrDefault();
                    if (this.medicineTypeTutSelected != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.medicineTypeTutSelected), this.medicineTypeTutSelected));
                        //Nếu hướng dẫn sử dụng mẫu có đường dùng thì lấy ra
                        if (this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID > 0)
                        {
                            this.cboMedicineUseForm.EditValue = this.medicineTypeTutSelected.MEDICINE_USE_FORM_ID;
                        }   
                        //Nếu không có đường dùng thì lấy đường dùng từ danh mục loại thuốc
                        else
                        {
                            var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                            if (medicineType != null && (medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                            {
                                this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;
                            }
                        }
                        this.cboHtu.Text = null;
                        if (DataHtuList != null && DataHtuList.Count > 0 && this.medicineTypeTutSelected.HTU_ID != null)
                        {
                            DataHtuList.ForEach(o => o.IsChecked = o.ID == this.medicineTypeTutSelected.HTU_ID);
                            this.cboHtu.Text = string.Join(", ", DataHtuList.Where(o => o.IsChecked).Select(o => o.HTU_NAME));
                        }
                        if (this.medicineTypeTutSelected.HTU_ID != null)
                            this.cboHtu.Properties.Buttons[1].Visible = true;
                        else
                            this.cboHtu.Properties.Buttons[1].Visible = false;   

                        if (this.spinSoNgay.Value < (this.medicineTypeTutSelected.DAY_COUNT ?? 0))
                            this.spinSoNgay.EditValue = this.medicineTypeTutSelected.DAY_COUNT;
                        this.spinSoLuongNgay.EditValue = this.medicineTypeTutSelected.DAY_COUNT;

                        Inventec.Common.Logging.LogSystem.Info("Truong hop co HDSD thuoc theo tai khoan cua loai thuoc (HIS_MEDICINE_TYPE_TUT)--> lay truong DAY_COUNT gan vao spinSoNgay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTutSelected), medicineTypeTutSelected));
                        IsSetByMedicineTut = true;
                        this.spinSang.EditValue = this.medicineTypeTutSelected.MORNING;
                        this.spinTrua.EditValue = this.medicineTypeTutSelected.NOON;
                        this.spinChieu.EditValue = this.medicineTypeTutSelected.AFTERNOON;
                        this.spinToi.EditValue = this.medicineTypeTutSelected.EVENING;
                        IsSetByMedicineTut = false;
                        if (String.IsNullOrEmpty(this.txtTutorial.Text)
                            || (lciLadder.Visible && String.IsNullOrEmpty(txtLadder.Text)))
                        {
                            //Nếu có trường hướng dẫn thì sử dụng luôn
                            if (!String.IsNullOrEmpty(this.medicineTypeTutSelected.TUTORIAL))
                            {
                                this.txtTutorial.Text = this.medicineTypeTutSelected.TUTORIAL;
                            }
                            //Nếu không có hướng dẫn sử dụng thì tự động set theo các trường như lúc nhập liệu
                            else
                            {
                                this.CalculateAmount();
                                this.SetHuongDanFromSoLuongNgay();
                            }
                        }

                        if (String.IsNullOrEmpty(this.memHtu.Text))
                        {
                            if (this.medicineTypeTutSelected.HTU_TEXT != null)
                                this.memHtu.Text = this.medicineTypeTutSelected.HTU_TEXT;
                            else
                            {
                                string autoValue = cboMedicineUseForm.Text + " " + cboHtu.Text;

                                memHtu.Text = autoValue;
                            }
                        }
                    }
                    
                }
                //Trường hợp thuốc không có cấu hình hướng dẫn sử dụng thì lấy hướng dẫn sử dụng ở danh mục loại thuốc fill vào
                else
                    this.medicineTypeTutSelected = null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineTypeTutSelected), medicineTypeTutSelected));
                if (this.medicineTypeTutSelected == null)
                {
                    var medicineType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == medicineTypeId);
                    if (medicineType != null)
                    {
                        if ((medicineType.MEDICINE_USE_FORM_ID ?? 0) > 0)
                        {
                            this.cboMedicineUseForm.EditValue = medicineType.MEDICINE_USE_FORM_ID;

                            string autoValue = cboMedicineUseForm.Text;

                            memHtu.Text = autoValue;
                        }
                        if ((String.IsNullOrEmpty(this.txtTutorial.Text)
                            || String.IsNullOrEmpty(txtLadder.Text)))
                        {
                            Inventec.Common.Logging.LogSystem.Debug(medicineType.TUTORIAL);
                            this.txtTutorial.Text = medicineType.TUTORIAL;
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => medicineType), medicineType));
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
                if (HisConfigCFG.ManyDayPrescriptionOption == 2
                        && this.UcDateGetValueForMedi() != null
                        && this.UcDateGetValueForMedi().Count > 0)
                {
                    intructionTimeAdds = this.UcDateGetValueForMedi();
                    if (intructionTimeAdds != null && intructionTimeAdds.Count > 0)
                    {
                        intructionTimeAdds = intructionTimeAdds.OrderByDescending(o => o).ToList();
                    }
                }
                else if (this.intructionTimeSelecteds != null && this.intructionTimeSelecteds.Count > 0)
                {
                    intructionTimeAdds = this.intructionTimeSelecteds.OrderByDescending(o => o).ToList();
                }
                if (UseTime > 0 || (intructionTimeAdds != null
                    && intructionTimeAdds.Count > 0
                    && intructionTimeAdds.First() > 0
                    && !string.IsNullOrEmpty(this.spinSoLuongNgay.Text)))
                {
                    long useTime = UseTime > 0 ? UseTime : intructionTimeAdds.First();
                    DateTime dtUseTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(useTime) ?? DateTime.MinValue;
                    DateTime dtUseTimeTo = dtUseTime.AddDays((double)this.spinSoLuongNgay.Value - 1);
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

        private bool ExistsAssianInDay(MediMatyTypeADO mediMatyType)
        {
            bool existsData = false;
            try
            {
                SereServInDay = new List<HIS_SERE_SERV>();
                if (this.sereServWithTreatment != null && this.sereServWithTreatment.Count > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.1");
                    SereServInDay = this.sereServWithTreatment.Where(o => o.SERVICE_ID == mediMatyType.SERVICE_ID
                        && o.TDL_INTRUCTION_TIME.ToString().Substring(0, 8) == intructionTimeSelecteds.OrderByDescending(t => t).First().ToString().Substring(0, 8)).ToList();
                    existsData = SereServInDay.Any();
                }
                if (existsData) return existsData;

                //if (this.serviceReqMetyInDay == null && this.serviceReqMatyInDay == null)
                //{
                //    InitDataServiceReqAllInDay();
                //    Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.2");
                //}

                if ((this.serviceReqMetyInDay != null && this.serviceReqMetyInDay.Count > 0) || (this.serviceReqMatyInDay != null && this.serviceReqMatyInDay.Count > 0))
                {
                    Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.3");
                    if (this.serviceReqMetyInDay != null && this.serviceReqMetyInDay.Count > 0 && (mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC || mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_DM))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.4");
                        existsData = existsData || this.serviceReqMetyInDay.Where(o => o.MEDICINE_TYPE_ID == mediMatyType.ID).Any();
                    }
                    if (this.serviceReqMetyInDay != null && this.serviceReqMetyInDay.Count > 0 && (mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.THUOC_TUTUC))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.5");
                        existsData = existsData || this.serviceReqMetyInDay.Where(o => o.MEDICINE_TYPE_NAME == mediMatyType.MEDICINE_TYPE_NAME && o.UNIT_NAME == mediMatyType.SERVICE_UNIT_NAME).Any();
                    }
                    if (this.serviceReqMatyInDay != null && this.serviceReqMatyInDay.Count > 0 && (mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU || mediMatyType.DataType == HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO.VATTU_DM))
                    {
                        Inventec.Common.Logging.LogSystem.Debug("ExistsAssianInDay.6");
                        existsData = existsData || this.serviceReqMatyInDay.Where(o => o.MATERIAL_TYPE_ID == mediMatyType.ID).Any();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => existsData), existsData)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqMetyInDay), serviceReqMetyInDay)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqMatyInDay), serviceReqMatyInDay));
            }
            catch (Exception ex)
            {
                existsData = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return existsData;
        }

        /// <summary>
        /// Sửa chức năng "Kê đơn" (Kê đơn phòng khám, Kê đơn tủ trực, Kê đơn điều trị):
        ///- Khi kê "Thuốc/vật tư mua ngoài", khi chọn thuốc/vật tư và nhấn nút "Bổ sung", nếu đã tồn tại đơn thuốc/vật tư (kể cả đơn trong kho và đơn tự mua) đã kê cho BN đó và có thời gian y lệnh trong ngày, thì:
        ///+ Hiển thị biểu tượng "chấm than" trên grid tương ứng với dòng thuốc đó và có tooltip "Thuốc đã kê trong ngày"
        ///- Sửa lại phần kê thuốc/vật tư trong kho:
        /// Hiện tại chỉ kiểm tra với thuốc/vật tư kê trong kho, bổ sung, nếu thuốc/vật tư được chọn trùng với thuốc/vật tư của đơn tự mua cũng xử lý tương tự.
        ///- Khi nhấn "Lưu", nếu tồn tại thuốc/vật tư (cả trong kho và tự mua), nếu có đánh dấu "Đã kê trong ngày" thì hiển thị thông báo "XXX đã kê trong ngày. Bạn có muốn tiếp tục"
        /// Trong đó, XXX là tên thuốc đã kê trong ngày.
        /// </summary>
        List<HIS_SERVICE_REQ> serviceReqAllInDays = null;

        public bool IsSetByMedicineTut { get; private set; }

        private async Task InitDataServiceReqAllInDay()
        {
            try
            {
                this.InstructionTime = this.intructionTimeSelecteds.OrderByDescending(o => o).First();

                CommonParam param = new CommonParam();
                HisServiceReqFilter serviceReqFilter = new HisServiceReqFilter();
                serviceReqFilter.TREATMENT_ID = this.treatmentId;
                serviceReqFilter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT, IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT };

                //serviceReqFilter.INTRUCTION_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"));

                serviceReqAllInDays = new BackendAdapter(param)
                      .Get<List<MOS.EFMODEL.DataModels.HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, serviceReqFilter, param);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => InstructionTime), InstructionTime)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqFilter), serviceReqFilter)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceReqAllInDays), serviceReqAllInDays));

                this.serviceReqMetyInDay = new List<HIS_SERVICE_REQ_METY>();
                this.serviceReqMatyInDay = new List<HIS_SERVICE_REQ_MATY>();
                if (serviceReqAllInDays != null && serviceReqAllInDays.Count > 0)
                {
                    var serviceReqAllInDayIds = serviceReqAllInDays.Select(o => o.ID).ToList();
                    param = new CommonParam();

                    HisServiceReqMetyFilter expMestMetyFilter = new HisServiceReqMetyFilter();
                    expMestMetyFilter.SERVICE_REQ_IDs = serviceReqAllInDayIds;
                    this.serviceReqMetyInBatch = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_METY>>(RequestUriStore.HIS_SERVICE_REQ_METY__GET, ApiConsumers.MosConsumer, expMestMetyFilter, ProcessLostToken, param);

                    HisServiceReqMatyFilter expMestMatyFilter = new HisServiceReqMatyFilter();
                    expMestMatyFilter.SERVICE_REQ_IDs = serviceReqAllInDayIds;
                    this.serviceReqMatyInBatch = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ_MATY>>(RequestUriStore.HIS_SERVICE_REQ_MATY__GET, ApiConsumers.MosConsumer, expMestMatyFilter, ProcessLostToken, param);

                    serviceReqAllInDays = serviceReqAllInDays.Where(o => o.INTRUCTION_DATE == Inventec.Common.TypeConvert.Parse.ToInt64((InstructionTime.ToString().Substring(0, 8) + "000000"))).ToList();
                    if (serviceReqAllInDays != null && serviceReqAllInDays.Count > 0)
                    {
                        serviceReqMetyInDay = serviceReqMetyInBatch.Where(o => serviceReqAllInDays.Exists(p => p.ID == o.SERVICE_REQ_ID)).ToList();
                        serviceReqMatyInDay = serviceReqMatyInBatch.Where(o => serviceReqAllInDays.Exists(p => p.ID == o.SERVICE_REQ_ID)).ToList();
                    }
                }
                if (this.serviceReqMetyInDay == null)
                    this.serviceReqMetyInDay = new List<HIS_SERVICE_REQ_METY>();

                if (this.serviceReqMatyInDay == null)
                    this.serviceReqMatyInDay = new List<HIS_SERVICE_REQ_MATY>();

                if (this.serviceReqMetyInBatch == null)
                    this.serviceReqMetyInBatch = new List<HIS_SERVICE_REQ_METY>();

                if (this.serviceReqMatyInBatch == null)
                    this.serviceReqMatyInBatch = new List<HIS_SERVICE_REQ_MATY>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                var selectedOpionGroup = GetSelectedOpionGroup();
                bool isTSD = selectedOpionGroup == 3;

                Inventec.Common.Logging.LogSystem.Debug("InitDataMetyMatyTypeInStockDAsync .1");
                this.mediStockD1ADOs = new List<DMediStock1ADO>();
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock2Filter filter = new MOS.Filter.DHisMediStock2Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    //if (isTSD)
                    //{
                    //    filter.IS_REUSABLE = isTSD;
                    //}

                    this.ProcessFilterDontPresExpiredTime(ref filter);

                    if (chkShowLo.Checked)
                    {
                        this.mediMatyTypeAvailables = await new BackendAdapter(param).GetAsync<List<D_HIS_MEDI_STOCK_2>>(RequestUriStore.HIS_MEDISTOCKDISDO_GET2, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    }
                    else
                    {
                        this.mediMatyTypeAvailables = await new BackendAdapter(param).GetAsync<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    }

                    this.ProcessResultDataMetyMatyTypeInStock(mediStockIds, isTSD);
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
                var selectedOpionGroup = GetSelectedOpionGroup();
                bool isTSD = selectedOpionGroup == 3;

                //Inventec.Common.Logging.LogSystem.Debug("InitDataMetyMatyTypeInStockD1 .1");
                this.mediStockD1ADOs = new List<DMediStock1ADO>();
                if (currentMediStock != null && currentMediStock.Count > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.DHisMediStock2Filter filter = new MOS.Filter.DHisMediStock2Filter();
                    List<long> mediStockIds = new List<long>();

                    mediStockIds = currentMediStock.Select(o => o.MEDI_STOCK_ID).ToList();
                    filter.MEDI_STOCK_IDs = mediStockIds;
                    //if (isTSD)
                    //{
                    //    filter.IS_REUSABLE = isTSD;
                    //}

                    this.ProcessFilterDontPresExpiredTime(ref filter);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                    if (chkShowLo.Checked)
                    {
                        this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(RequestUriStore.HIS_MEDISTOCKDISDO_GET2, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    }
                    else
                    {
                        this.mediMatyTypeAvailables = new BackendAdapter(param).Get<List<D_HIS_MEDI_STOCK_2>>(HisRequestUriStore.HIS_MEDISTOCKDISDO_GET1, ApiConsumers.MosConsumer, filter, ProcessLostToken, param);
                    }

                    this.ProcessResultDataMetyMatyTypeInStock(mediStockIds, isTSD);
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
                LogSystem.Debug("cấu hình FilterByRestrict.____ " + "____ket qua tim thay sau khi loc " + (this.mediMatyTypeAvailables != null ? this.mediMatyTypeAvailables.Count : 0));
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
                    mediStockD1s = mediStockD1s.Where(o => (o.IS_REUSABLE == 1 || o.IS_IDENTITY_MANAGEMENT == 1) && o.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                    this.ProcessMapingDataForTSD(mediStockD1s, mediStockIds);
                }
                else
                {
                    mediStockD1s = mediStockD1s.Where(o => (o.IS_REUSABLE == null || o.IS_REUSABLE != 1) && (o.IS_IDENTITY_MANAGEMENT == null || o.IS_IDENTITY_MANAGEMENT != 1)).ToList();
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
                materialBeanViewFilter.IS_REUSABLE__OR__IS_IDENTITY_MANAGEMENT = true;
                materialBeanViewFilter.MEDI_STOCK_IDs = mediStockIds;
                materialBeanViewFilter.MATERIAL_IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                materialBeanViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;

                var matyIds = mediStockD1s.Select(o => o.ID ?? 0).ToList();
                this.mediStockD1ADOs = new BackendAdapter(param).Get<List<DMediStock1ADO>>(RequestUriStore.HIS_MATERIAL_BEAN__GET_VIEW1, ApiConsumers.MosConsumer, materialBeanViewFilter, ProcessLostToken, param);
                if (this.mediStockD1ADOs != null && this.mediStockD1ADOs.Count > 0)
                {
                    this.mediStockD1ADOs = this.mediStockD1ADOs.Where(o => matyIds.Contains(o.MATERIAL_TYPE_ID)).ToList();
                }

                //Bỏ đoạn code này do khi Mapper có thể sẽ bị gán nhầm trường, lúc gọi api có thể map trực tiếp sang class cần luôn không cần mapper nữa
                //AutoMapper.Mapper.CreateMap<V_HIS_MATERIAL_BEAN_1, DMediStock1ADO>();
                //this.mediStockD1ADOs = AutoMapper.Mapper.Map<List<DMediStock1ADO>>(matyBeans);

                foreach (var item in this.mediStockD1ADOs)
                {
                    item.USE_COUNT_DISPLAY = String.Format("{0} / {1}", item.REMAIN_REUSE_COUNT, item.TDL_MATERIAL_MAX_REUSE_COUNT);
                    item.ID = item.MATERIAL_TYPE_ID;
                    item.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT;

                    var mtF = mediStockD1s.Where(o => o.ID == item.MATERIAL_TYPE_ID && o.MEDI_STOCK_ID == item.MEDI_STOCK_ID).FirstOrDefault();

                    if (mtF != null)
                    {
                        item.IS_STENT = mtF.IS_STENT;
                        item.IS_IDENTITY_MANAGEMENT = mtF.IS_IDENTITY_MANAGEMENT;
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
                        item.IS_AUTO_EXPEND = mtF.IS_AUTO_EXPEND;
                        item.IS_OUT_PARENT_FEE = mtF.IS_OUT_PARENT_FEE;

                        item.LAST_EXP_PRICE = mtF.LAST_EXP_PRICE;
                        item.LAST_EXP_VAT_RATIO = mtF.LAST_EXP_VAT_RATIO;
                        //item.IS_SPLIT_COMPENSATION = mtF.IS_SPLIT_COMPENSATION;

                        item.PARENT_ID = mtF.PARENT_ID;
                        item.PARENT_CODE = mtF.PARENT_CODE;

                        item.PARENT_NAME = mtF.PARENT_NAME;
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
                        V_HIS_MATERIAL_TYPE maty = null;

                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            mety = currentMedicineTypeTemps != null && currentMedicineTypeTemps.Any(o => o.SERVICE_ID == item.SERVICE_ID) ? currentMedicineTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault() : null;
                        }
                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                        {
                            maty = currentMaterialTypeTemps != null && currentMaterialTypeTemps.Any(o => o.SERVICE_ID == item.SERVICE_ID) ? currentMaterialTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault() : null;
                        }

                        if (this.oldServiceReq != null && this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1 && mety != null)
                        {
                            isUse = (mety != null && mety.IS_KIDNEY == 1);
                        }
                        else
                        {
                            isUse = true;
                        }

                        if (isUse)
                        {
                            DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                            dMediStock1ADO.IS_IDENTITY_MANAGEMENT = item.IS_IDENTITY_MANAGEMENT;
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
                            dMediStock1ADO.CONTRAINDICATION = item.CONTRAINDICATION;
                            dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                            dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                            dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                            dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                            dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                            dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                            dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));

                            dMediStock1ADO.PARENT_ID = item.PARENT_ID;
                            dMediStock1ADO.PARENT_CODE = item.PARENT_CODE;

                            dMediStock1ADO.PARENT_NAME = item.PARENT_NAME;

                            if (chkShowLo.Checked)
                            {
                                dMediStock1ADO.MAME_ID = item.MAME_ID;
                                dMediStock1ADO.IsAssignPackage = true;//Thuoc theo lo
                                dMediStock1ADO.TDL_PACKAGE_NUMBER = item.TDL_PACKAGE_NUMBER;//Thuoc theo lo
                                dMediStock1ADO.EXPIRED_DATE = item.EXPIRED_DATE;//Thuoc theo lo
                                dMediStock1ADO.REGISTER_NUMBER = item.MEDICINE_REGISTER_NUMBER;//Thuoc theo lo
                            }

                            dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                            dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                            dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                            UpdateUnit(dMediStock1ADO, GlobalStore.HisMestMetyUnit);
                            dMediStock1ADO.AMOUNT = ((dMediStock1ADO.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;

                            SetAmountOddByKeyShowRoundAvailableAmount(ref dMediStock1ADO, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);

                            if (mety != null)
                            {
                                dMediStock1ADO.IS_OXYGEN = mety.IS_OXYGEN;
                                dMediStock1ADO.SERVICE_ID = mety.SERVICE_ID;
                                dMediStock1ADO.SERVICE_TYPE_ID = mety.SERVICE_TYPE_ID;
                                dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                //dMediStock1ADO.REGISTER_NUMBER = mety.REGISTER_NUMBER;
                                dMediStock1ADO.HEIN_SERVICE_TYPE_ID = mety.HEIN_SERVICE_TYPE_ID;
                                dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = mety.HEIN_SERVICE_TYPE_CODE;
                                dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = mety.HEIN_SERVICE_BHYT_CODE;
                                dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = mety.HEIN_SERVICE_BHYT_NAME;
                                dMediStock1ADO.IS_BLOCK_MAX_IN_PRESCRIPTION = mety.IS_BLOCK_MAX_IN_PRESCRIPTION;
                                dMediStock1ADO.IS_BLOCK_MAX_IN_TREATMENT = mety.IS_BLOCK_MAX_IN_TREATMENT;
                                dMediStock1ADO.ALERT_MAX_IN_DAY = mety.ALERT_MAX_IN_DAY;
                                dMediStock1ADO.IS_BLOCK_MAX_IN_DAY = mety.IS_BLOCK_MAX_IN_DAY;
                                dMediStock1ADO.IS_SPLIT_COMPENSATION = mety.IS_SPLIT_COMPENSATION;
                                dMediStock1ADO.ATC_CODES = mety.ATC_CODES;
                                dMediStock1ADO.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                                dMediStock1ADO.CONTRAINDICATION_IDS = mety.CONTRAINDICATION_IDS;
                                dMediStock1ADO.DESCRIPTION = mety.DESCRIPTION;
                                dMediStock1ADO.IsAllowOdd = (mety.IS_ALLOW_ODD == 1) ? true : false;
                                dMediStock1ADO.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                dMediStock1ADO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                                dMediStock1ADO.ODD_WARNING_CONTENT = mety.ODD_WARNING_CONTENT;
                            }
                            if (maty != null)
                            {
                                dMediStock1ADO.SERVICE_ID = maty.SERVICE_ID;
                                dMediStock1ADO.SERVICE_TYPE_ID = maty.SERVICE_TYPE_ID;
                                dMediStock1ADO.HEIN_SERVICE_TYPE_ID = maty.HEIN_SERVICE_TYPE_ID;
                                dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = maty.HEIN_SERVICE_TYPE_CODE;
                                dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = maty.HEIN_SERVICE_BHYT_CODE;
                                dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = maty.HEIN_SERVICE_BHYT_NAME;
                                dMediStock1ADO.DESCRIPTION = maty.DESCRIPTION;
                                dMediStock1ADO.MATERIAL_TYPE_MAP_ID = maty.MATERIAL_TYPE_MAP_ID;
                                dMediStock1ADO.MATERIAL_TYPE_MAP_CODE = maty.MATERIAL_TYPE_MAP_CODE;
                                dMediStock1ADO.MATERIAL_TYPE_MAP_NAME = maty.MATERIAL_TYPE_MAP_NAME;
                                dMediStock1ADO.IsAllowOdd = (maty.IS_ALLOW_ODD == 1) ? true : false;
                                dMediStock1ADO.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                dMediStock1ADO.ALERT_MAX_IN_DAY = maty.ALERT_MAX_IN_DAY;
                                //dMediStock1ADO.IS_SPLIT_COMPENSATION = maty.IS_SPLIT_COMPENSATION;
                                //dMediStock1ADO.CONTRAINDICATION_IDS = maty.CONTRAINDICATION_IDS;
                            }

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

        private bool UpdateUnit(DMediStock1ADO mediMatyTypeADO, List<HIS_MEST_METY_UNIT> mestMetyUnitData = null)
        {
            bool success = false;
            try
            {
                if (mediMatyTypeADO == null || (mediMatyTypeADO.MEDI_STOCK_ID ?? 0) == 0 || mediMatyTypeADO.ID == 0)
                    throw new ArgumentNullException("data");
                if (mestMetyUnitData != null && mestMetyUnitData.Count > 0 && mediMatyTypeADO.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                {
                    var oneCheck = mestMetyUnitData.Where(o => o.MEDI_STOCK_ID == mediMatyTypeADO.MEDI_STOCK_ID && o.MEDICINE_TYPE_ID == mediMatyTypeADO.ID).FirstOrDefault();
                    success = (oneCheck != null && oneCheck.USE_ORIGINAL_UNIT_FOR_PRES == 1);
                    mediMatyTypeADO.IsUseOrginalUnitForPres = success ? (bool?)true : null;
                }
                else
                {
                    mediMatyTypeADO.IsUseOrginalUnitForPres = null;
                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        private List<DMediStock1ADO> ConvertToDMediStockForNhaThuoc(List<D_HIS_MEDI_STOCK_2> listMediStock)
        {
            List<DMediStock1ADO> result = new List<DMediStock1ADO>();
            try
            {
                var currentMedicineTypeTemps = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                var currentMaterialTypeTemps = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (listMediStock != null && listMediStock.Count > 0)
                {
                    //- Khi chọn nhà thuốc, hiển thị các thuốc/vật tư thỏa mãn 1 trong 2 điều kiện:
                    //+ Các thuốc/vật tư có tồn > 0 và các thuốc
                    //+ Các thuốc/vật tư là thuốc/vật tư ngoại viện (HIS_MATERIAL_TYPE/HIS_MEDICINE_TYPE có IS_OUT_HOSPITAL = 1)


                    foreach (var item in listMediStock)
                    {
                        bool isUse = false;
                        V_HIS_MEDICINE_TYPE mety = null;
                        V_HIS_MATERIAL_TYPE maty = null;

                        if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            mety = currentMedicineTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault();
                        }
                        if (this.oldServiceReq != null && this.oldServiceReq.IS_EXECUTE_KIDNEY_PRES == 1 && item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC)
                        {
                            isUse = (mety != null && mety.IS_KIDNEY == 1);
                        }
                        else
                        {
                            isUse = true;
                        }


                        if (isUse)
                        {
                            if (item.SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)
                            {
                                maty = currentMaterialTypeTemps != null && currentMaterialTypeTemps.Any(o => o.SERVICE_ID == item.SERVICE_ID) ? currentMaterialTypeTemps.Where(o => o.SERVICE_ID == item.SERVICE_ID).FirstOrDefault() : null;
                            }

                            DMediStock1ADO m1 = null;
                            //if (!HisConfigCFG.IsAutoCreateSaleExpMest)
                            if ((!HisConfigCFG.IsAutoCreateSaleExpMest) || (HisConfigCFG.OutStockListItemInCaseOfNoStockChosenOption == "2" && !(currentMediStockNhaThuocSelecteds != null && currentMediStockNhaThuocSelecteds.Count > 0)))
                            {
                                m1 = result.Where(o => o.ID == item.ID && o.SERVICE_TYPE_ID == item.SERVICE_TYPE_ID && o.EXP_PRICE_DISPLAY == (o.LAST_EXP_PRICE * (1 + o.LAST_EXP_VAT_RATIO))).FirstOrDefault();
                                if (m1 != null)
                                {
                                    decimal? am = ((m1.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;
                                    m1.AMOUNT += am;
                                    SetAmountOddByKeyShowRoundAvailableAmount(ref m1, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);
                                }
                            }

                            if (m1 == null)
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
                                dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                                dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                                dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                                dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                                dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                                dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                                dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                                dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                                dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                                dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                                dMediStock1ADO.PARENT_ID = item.PARENT_ID;
                                dMediStock1ADO.PARENT_CODE = item.PARENT_CODE;
                                dMediStock1ADO.PARENT_NAME = item.PARENT_NAME;


                                UpdateUnit(dMediStock1ADO, GlobalStore.HisMestMetyUnit);
                                dMediStock1ADO.AMOUNT = ((dMediStock1ADO.IsUseOrginalUnitForPres ?? false) == false && item.CONVERT_RATIO.HasValue && item.CONVERT_RATIO > 0) ? item.AMOUNT * item.CONVERT_RATIO : item.AMOUNT;

                                SetAmountOddByKeyShowRoundAvailableAmount(ref dMediStock1ADO, mety, currentMedicineTypeTemps, currentMaterialTypeTemps);

                                if (mety != null)
                                {
                                    dMediStock1ADO.IS_OXYGEN = mety.IS_OXYGEN;
                                    dMediStock1ADO.SERVICE_ID = mety.SERVICE_ID;
                                    dMediStock1ADO.SERVICE_TYPE_ID = mety.SERVICE_TYPE_ID;
                                    dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = mety.ACTIVE_INGR_BHYT_CODE;
                                    dMediStock1ADO.HEIN_SERVICE_TYPE_ID = mety.HEIN_SERVICE_TYPE_ID;
                                    dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = mety.HEIN_SERVICE_TYPE_CODE;
                                    dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = mety.HEIN_SERVICE_BHYT_CODE;
                                    dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = mety.HEIN_SERVICE_BHYT_NAME;
                                    dMediStock1ADO.IS_BLOCK_MAX_IN_PRESCRIPTION = mety.IS_BLOCK_MAX_IN_PRESCRIPTION;
                                    dMediStock1ADO.IS_BLOCK_MAX_IN_TREATMENT = mety.IS_BLOCK_MAX_IN_TREATMENT;
                                    dMediStock1ADO.ALERT_MAX_IN_DAY = mety.ALERT_MAX_IN_DAY;
                                    dMediStock1ADO.IS_BLOCK_MAX_IN_DAY = mety.IS_BLOCK_MAX_IN_DAY;
                                    dMediStock1ADO.IS_SPLIT_COMPENSATION = mety.IS_SPLIT_COMPENSATION;
                                    dMediStock1ADO.IS_OUT_HOSPITAL = mety.IS_OUT_HOSPITAL;
                                    dMediStock1ADO.IsAllowOdd = (mety.IS_ALLOW_ODD == 1) ? true : false;
                                    dMediStock1ADO.IsAllowOddAndExportOdd = (mety.IS_ALLOW_ODD == 1 && mety.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                    dMediStock1ADO.DESCRIPTION = mety.DESCRIPTION;
                                    dMediStock1ADO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
                                    dMediStock1ADO.ODD_WARNING_CONTENT = mety.ODD_WARNING_CONTENT;
                                    dMediStock1ADO.ATC_CODES = mety.ATC_CODES;
                                    dMediStock1ADO.ATC_GROUP_CODES = mety.ATC_GROUP_CODES;
                                }

                                if (maty != null)
                                {
                                    dMediStock1ADO.MATERIAL_TYPE_MAP_ID = maty.MATERIAL_TYPE_MAP_ID;
                                    dMediStock1ADO.MATERIAL_TYPE_MAP_CODE = maty.MATERIAL_TYPE_MAP_CODE;
                                    dMediStock1ADO.MATERIAL_TYPE_MAP_NAME = maty.MATERIAL_TYPE_MAP_NAME;
                                    dMediStock1ADO.IS_OUT_HOSPITAL = maty.IS_OUT_HOSPITAL;
                                    dMediStock1ADO.IsAllowOdd = (maty.IS_ALLOW_ODD == 1) ? true : false;
                                    dMediStock1ADO.IsAllowOddAndExportOdd = (maty.IS_ALLOW_ODD == 1 && maty.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                                    dMediStock1ADO.DESCRIPTION = maty.DESCRIPTION;
                                    dMediStock1ADO.ALERT_MAX_IN_DAY = maty.ALERT_MAX_IN_DAY;

                                    //dMediStock1ADO.IS_SPLIT_COMPENSATION = maty.IS_SPLIT_COMPENSATION;
                                }
                                if ((dMediStock1ADO.IS_OUT_HOSPITAL.HasValue && dMediStock1ADO.IS_OUT_HOSPITAL == GlobalVariables.CommonNumberTrue))
                                {
                                    Inventec.Common.Logging.LogSystem.Debug("MEDICINE_TYPE_CODE: " + dMediStock1ADO.MEDICINE_TYPE_CODE + ", MEDICINE_TYPE_NAME: " + dMediStock1ADO.MEDICINE_TYPE_NAME + ", IS_OUT_HOSPITAL: " + dMediStock1ADO.IS_OUT_HOSPITAL);
                                }
                                //if (dMediStock1ADO.MEDICINE_TYPE_CODE == "VTNT_195")
                                //{
                                //    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => maty), maty));
                                //    Inventec.Common.Logging.LogSystem.Debug("MEDICINE_TYPE_CODE: " + dMediStock1ADO.MEDICINE_TYPE_CODE + ", MEDICINE_TYPE_NAME: " + dMediStock1ADO.MEDICINE_TYPE_NAME + ", IS_OUT_HOSPITAL: " + dMediStock1ADO.IS_OUT_HOSPITAL);
                                //}
                                //if ((dMediStock1ADO.AMOUNT.HasValue && dMediStock1ADO.AMOUNT.Value > 0) || (dMediStock1ADO.IS_OUT_HOSPITAL.HasValue && dMediStock1ADO.IS_OUT_HOSPITAL == GlobalVariables.CommonNumberTrue))

                                result.Add(dMediStock1ADO);
                            }
                        }
                    }
                }

                var currentMedicineTypeTempsHasOutHospital = currentMedicineTypeTemps != null ? currentMedicineTypeTemps.Where(o => o.IS_OUT_HOSPITAL.HasValue && o.IS_OUT_HOSPITAL == 1 && (result.Count == 0 || !result.Exists(k => k.SERVICE_ID == o.SERVICE_ID))).ToList() : null;
                if (currentMedicineTypeTempsHasOutHospital != null && currentMedicineTypeTempsHasOutHospital.Count > 0)
                {
                    foreach (var item in currentMedicineTypeTempsHasOutHospital)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                        dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                        dMediStock1ADO.ALERT_MAX_IN_TREATMENT = item.ALERT_MAX_IN_TREATMENT;
                        dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                        dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                        dMediStock1ADO.GENDER_ID = item.TDL_GENDER_ID;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.ID = item.ID;
                        dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                        dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                        dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                        dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                        dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                        dMediStock1ADO.IS_STAR_MARK = item.IS_STAR_MARK;
                        //dMediStock1ADO.IS_STENT = item.IS_STENT;
                        dMediStock1ADO.IS_VACCINE = item.IS_VACCINE;
                        dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                        dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        //dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                        //dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        //dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
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
                        dMediStock1ADO.USE_ON_DAY = item.USE_ON_DAY;
                        dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                        dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                        dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                        dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                        dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                        dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                        dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_CODE) + item.MEDICINE_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MEDICINE_TYPE_NAME) + item.MEDICINE_TYPE_NAME;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_NAME__UNSIGN = StringUtil.convertToUnSign3(item.ACTIVE_INGR_BHYT_NAME) + item.ACTIVE_INGR_BHYT_NAME;

                        dMediStock1ADO.IS_OXYGEN = item.IS_OXYGEN;
                        dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                        dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                        dMediStock1ADO.ACTIVE_INGR_BHYT_CODE = item.ACTIVE_INGR_BHYT_CODE;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = item.HEIN_SERVICE_TYPE_CODE;
                        dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                        dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        dMediStock1ADO.IS_BLOCK_MAX_IN_PRESCRIPTION = item.IS_BLOCK_MAX_IN_PRESCRIPTION;
                        dMediStock1ADO.IS_BLOCK_MAX_IN_TREATMENT = item.IS_BLOCK_MAX_IN_TREATMENT;
                        dMediStock1ADO.ALERT_MAX_IN_DAY = item.ALERT_MAX_IN_DAY;
                        dMediStock1ADO.IS_BLOCK_MAX_IN_DAY = item.IS_BLOCK_MAX_IN_DAY;
                        dMediStock1ADO.IS_SPLIT_COMPENSATION = item.IS_SPLIT_COMPENSATION;
                        dMediStock1ADO.IS_OUT_HOSPITAL = item.IS_OUT_HOSPITAL;
                        dMediStock1ADO.IsAllowOdd = (item.IS_ALLOW_ODD == 1) ? true : false;
                        dMediStock1ADO.IsAllowOddAndExportOdd = (item.IS_ALLOW_ODD == 1 && item.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                        dMediStock1ADO.DESCRIPTION = item.DESCRIPTION;

                        dMediStock1ADO.PARENT_ID = item.PARENT_ID;
                        dMediStock1ADO.PARENT_CODE = item.PARENT_CODE;
                        dMediStock1ADO.PARENT_NAME = item.PARENT_NAME;

                        dMediStock1ADO.MEDICINE_GROUP_ID = item.MEDICINE_GROUP_ID;
                        dMediStock1ADO.ODD_WARNING_CONTENT = item.ODD_WARNING_CONTENT;

                        result.Add(dMediStock1ADO);
                    }
                }
                var currentMaterialTypeTempsHasOutHospital = currentMaterialTypeTemps != null ? currentMaterialTypeTemps.Where(o => o.IS_OUT_HOSPITAL.HasValue && o.IS_OUT_HOSPITAL == 1 && (result.Count == 0 || !result.Exists(k => k.SERVICE_ID == o.SERVICE_ID))).ToList() : null;
                if (currentMaterialTypeTempsHasOutHospital != null && currentMaterialTypeTempsHasOutHospital.Count > 0)
                {
                    foreach (var item in currentMaterialTypeTempsHasOutHospital)
                    {
                        DMediStock1ADO dMediStock1ADO = new DMediStock1ADO();
                        dMediStock1ADO.ALERT_MAX_IN_PRESCRIPTION = item.ALERT_MAX_IN_PRESCRIPTION;
                        dMediStock1ADO.ALERT_MIN_IN_STOCK = item.ALERT_MIN_IN_STOCK;
                        dMediStock1ADO.CONCENTRA = item.CONCENTRA;
                        dMediStock1ADO.GENDER_ID = item.TDL_GENDER_ID;
                        dMediStock1ADO.HEIN_SERVICE_TYPE_ID = item.HEIN_SERVICE_TYPE_ID;
                        dMediStock1ADO.ID = item.ID;
                        dMediStock1ADO.IMP_PRICE = item.IMP_PRICE;
                        dMediStock1ADO.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                        dMediStock1ADO.IS_ACTIVE = item.IS_ACTIVE;
                        dMediStock1ADO.IS_AUTO_EXPEND = item.IS_AUTO_EXPEND;
                        dMediStock1ADO.IS_CHEMICAL_SUBSTANCE = item.IS_CHEMICAL_SUBSTANCE;
                        dMediStock1ADO.IS_OUT_PARENT_FEE = item.IS_OUT_PARENT_FEE;
                        dMediStock1ADO.IS_STENT = item.IS_STENT;
                        dMediStock1ADO.MANUFACTURER_CODE = item.MANUFACTURER_CODE;
                        dMediStock1ADO.MANUFACTURER_ID = item.MANUFACTURER_ID;
                        dMediStock1ADO.MANUFACTURER_NAME = item.MANUFACTURER_NAME;
                        //dMediStock1ADO.MEDI_STOCK_CODE = item.MEDI_STOCK_CODE;
                        //dMediStock1ADO.MEDI_STOCK_ID = item.MEDI_STOCK_ID;
                        //dMediStock1ADO.MEDI_STOCK_NAME = item.MEDI_STOCK_NAME;
                        dMediStock1ADO.MEDICINE_TYPE_CODE = item.MATERIAL_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME = item.MATERIAL_TYPE_NAME;
                        dMediStock1ADO.NATIONAL_NAME = item.NATIONAL_NAME;
                        dMediStock1ADO.SERVICE_ID = item.SERVICE_ID;
                        dMediStock1ADO.SERVICE_TYPE_ID = item.SERVICE_TYPE_ID;
                        dMediStock1ADO.SERVICE_UNIT_CODE = item.SERVICE_UNIT_CODE;
                        dMediStock1ADO.SERVICE_UNIT_ID = item.SERVICE_UNIT_ID;
                        dMediStock1ADO.SERVICE_UNIT_NAME = item.SERVICE_UNIT_NAME;
                        dMediStock1ADO.CONVERT_RATIO = item.CONVERT_RATIO;
                        dMediStock1ADO.CONVERT_UNIT_CODE = item.CONVERT_UNIT_CODE;
                        dMediStock1ADO.CONVERT_UNIT_NAME = item.CONVERT_UNIT_NAME;
                        dMediStock1ADO.LAST_EXP_PRICE = item.LAST_EXP_PRICE;
                        dMediStock1ADO.LAST_EXP_VAT_RATIO = item.LAST_EXP_VAT_RATIO;
                        dMediStock1ADO.EXP_PRICE_DISPLAY = (item.LAST_EXP_PRICE * (1 + item.LAST_EXP_VAT_RATIO));
                        dMediStock1ADO.MEDICINE_TYPE_CODE__UNSIGN = StringUtil.convertToUnSign3(item.MATERIAL_TYPE_CODE) + item.MATERIAL_TYPE_CODE;
                        dMediStock1ADO.MEDICINE_TYPE_NAME__UNSIGN = StringUtil.convertToUnSign3(item.MATERIAL_TYPE_NAME) + item.MATERIAL_TYPE_NAME;

                        dMediStock1ADO.HEIN_SERVICE_TYPE_CODE = item.HEIN_SERVICE_TYPE_CODE;
                        dMediStock1ADO.HEIN_SERVICE_BHYT_CODE = item.HEIN_SERVICE_BHYT_CODE;
                        dMediStock1ADO.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                        dMediStock1ADO.MATERIAL_TYPE_MAP_ID = item.MATERIAL_TYPE_MAP_ID;
                        dMediStock1ADO.MATERIAL_TYPE_MAP_CODE = item.MATERIAL_TYPE_MAP_CODE;
                        dMediStock1ADO.MATERIAL_TYPE_MAP_NAME = item.MATERIAL_TYPE_MAP_NAME;
                        dMediStock1ADO.IS_OUT_HOSPITAL = item.IS_OUT_HOSPITAL;
                        dMediStock1ADO.IsAllowOdd = (item.IS_ALLOW_ODD == 1) ? true : false;
                        dMediStock1ADO.IsAllowOddAndExportOdd = (item.IS_ALLOW_ODD == 1 && item.IS_ALLOW_EXPORT_ODD == 1) ? true : false;
                        dMediStock1ADO.DESCRIPTION = item.DESCRIPTION;

                        dMediStock1ADO.PARENT_ID = item.PARENT_ID;
                        dMediStock1ADO.PARENT_CODE = item.PARENT_CODE;
                        dMediStock1ADO.PARENT_NAME = item.PARENT_NAME;
                        dMediStock1ADO.ALERT_MAX_IN_DAY = item.ALERT_MAX_IN_DAY;


                        result.Add(dMediStock1ADO);
                    }
                }
                //Inventec.Common.Logging.LogSystem.Debug("ConvertToDMediStockForNhaThuoc___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
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
                        medicineTypeSDO.MEDICINE_GROUP_ID = mety.MEDICINE_GROUP_ID;
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
