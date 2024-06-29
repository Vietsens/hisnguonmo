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
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList.Data;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MediStockSummary.ADO;
using HIS.Desktop.Plugins.MediStockSummary.Base;
using HIS.UC.HisBloodTypeInStock;
using HIS.UC.HisMaterialInStock;
using HIS.UC.HisMaterialInStock.ADO;
using HIS.UC.HisMedicineInStock;
using HIS.UC.HisMedicineInStock.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using MOS.Filter;
using System.Dynamic;
using System.Reflection;

namespace HIS.Desktop.Plugins.MediStockSummary
{
    public partial class UCMediStockSummary : HIS.Desktop.Utility.UserControlBase
    {
        #region ---Decalre
        List<HisMedicineInStockSDO> lstMediInStocks { get; set; }
        List<HisMaterialInStockSDO> lstMateInStocks { get; set; }
        List<HisBloodInStockSDO> lstBloodInStocks { get; set; }
        List<HisBloodTypeInStockSDO> lstBlood { get; set; }

        HisMedicineInStockProcessor hisMediInStockProcessor;
        HisMaterialInStockProcessor hisMateInStockProcessor;
        HisBloodTypeInStockProcessor hisBloodProcessor;

        UserControl ucMedicineInfo;
        UserControl ucMaterialInfo;
        UserControl ucBloodInfo;

        V_HIS_MEDI_STOCK currentMediStock = null;
        internal List<long> mediStockIds = new List<long>();
        List<DevExpress.Utils.Menu.DXMenuItem> dXmenuItem;
        HisMedicineInStockADO mediStockAdo;
        HisMaterialInStockADO mateStockAdo;
        bool isCheck;

        Inventec.Desktop.Common.Modules.Module moduleData;
        long RoomId;
        long RoomTypeId;
        HisMedicineInStockADO medicineTypeAdo = new HisMedicineInStockADO();
        HisMaterialInStockADO materialTypeAdo = new HisMaterialInStockADO();
        MOS.Filter.HisMedicineStockViewFilter mediFilter;
        internal string fileName = "";

        internal List<HIS_MEDI_STOCK_MATY> HisMediStockMaty { get; set; }
        internal List<HIS_MEDI_STOCK_MATY> HisMediStockMatyByStocks { get; set; }

        internal List<HIS_MEDI_STOCK_METY> HisMediStockMety { get; set; }
        internal List<HIS_MEDI_STOCK_METY> HisMediStockMetyByStocks { get; set; }

        internal List<MediStockADO> _MediStocks { get; set; }

        bool isCheckAll = false;
        public string IdComboStt { get; set; }
        public long IdComboPatientType { get; set; }
        private HisMedicineInStockADO currentRowMedicine { get; set; }
        private HisMaterialInStockADO currentRowMaterial { get; set; }
        private decimal? parentAvailableAmount { get; set; }
        #endregion
        public UCMediStockSummary(Inventec.Desktop.Common.Modules.Module module, long roomId, long roomTypeId)
            : base(module)
        {
            InitializeComponent();
            try
            {
                WaitingManager.Show();
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.moduleData = module;
                this.RoomId = roomId;
                this.RoomTypeId = roomTypeId;
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MediStockSummary.Resources.Lang", typeof(UCMediStockSummary).Assembly);
                InitMedicineTree();
                InitHisMaterialInStockTree();
                InitHisBloodTree();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMediStockSummary_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadDataGridMediStock();
                InitCboIsActive();
                InitCboPatientType();
                CheckCboPatient();
                //ShowUCControl();
                var date = Inventec.Common.TypeConvert.Parse.ToDateTime(DateTime.Now.ToString());
                var DateTo = date.Year;
                DateTo++;
                var lastDayOfMonthTo = DateTime.DaysInMonth(DateTo, date.Month);
                var dat2 = Inventec.Common.TypeConvert.Parse.ToDateTime(lastDayOfMonthTo + "/" + date.Month + "/" + DateTo);
                dtExpiredDate.EditValue = dat2.ToString("MM/yyyy");
                dtValidToTime.EditValue = date.ToString("dd/MM/yyyy");
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboIsActive()
        {
            try
            {
                List<StatusADO> li = new List<StatusADO>();
                li.Add(new StatusADO("01", Inventec.Common.Resource.Get.Value("UCMediStockSummary.cboValue1", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                li.Add(new StatusADO("02", Inventec.Common.Resource.Get.Value("UCMediStockSummary.cboValue2", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                li.Add(new StatusADO("03", Inventec.Common.Resource.Get.Value("UCMediStockSummary.cboValue3", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture())));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ID", "", 50, 1));
                columnInfos.Add(new ColumnInfo("STATUS_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("STATUS_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboIsActive, li, controlEditorADO);
                cboIsActive.EditValue = "01";
                IdComboStt = "01";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckCboPatient()
        {
            try
            {
                var medi = BackendDataWorker.Get<HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == RoomId);
                if (medi != null)
                {
                    if (medi.IS_BUSINESS == 1)
                    {
                        var codeFee = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.HOSPITAL_FEE");
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == codeFee);
                        cboPatientType.EditValue = patientType != null ? patientType.ID : 0;
                        IdComboPatientType = patientType != null ? patientType.ID : 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCboPatientType()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_CODE", "", 50, 1));
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cboPatientType, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIsActive.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockSummary.cboIsActive.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkValidToTime.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.ChkValidToTime.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPatientType.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMediStockSummary.cboPatientType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkNoExpiredDate.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.ChkNoExpiredDate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkNoExpiredDate.ToolTip = Inventec.Common.Resource.Get.Value("UCMediStockSummary.ChkNoExpiredDate.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkExpiredDate.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.ChkExpiredDate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ChkExpiredDate.ToolTip = Inventec.Common.Resource.Get.Value("UCMediStockSummary.ChkExpiredDate.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAlertMinStock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.chkAlertMinStock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkShowLineZero.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.chkShowLineZero.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnXuatExcel.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.btnXuatExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBlood.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.chkBlood.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMaterial.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.chkMaterial.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.chkMedicine.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControlItem19.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControlItem22.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWork.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMediStockSummary.txtKeyWork.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value("UCMediStockSummary.btnRefesh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridViewMediStock.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridViewMediStock.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnCheck.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridColumnCheck.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMediStockSummary.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitMedicineTree()
        {
            try
            {
                hisMediInStockProcessor = new HisMedicineInStockProcessor();
                HisMedicineInStockInitADO ado = new HisMedicineInStockInitADO();
                ado.HisMedicineInStockColumns = new List<HisMedicineInStockColumn>();
                ado.IsShowSearchPanel = true;
                ado.KeyFieldName = "NodeId";
                ado.ParentFieldName = "ParentNodeId";
                ado.NameButtonClose = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonClose", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.NameButtonOpen = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonOpen", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.HisMedicineInStock_GetSelectImage = medicineType_GetSelectImage;
                ado.HisMedicineInStock_GetStateImage = medicineType_GetStateImage;
                ado.MenuItems = menuItemMedicine;
                ado.HisMedicineInStock_CustomUnboundColumnData = medicineType_CustomUnboundColumnData;
                ado.HisMedicineInStockDoubleClick = medicineType_DoubleClick;
                // ado.HisMedicineInStock_CustomDrawNodeCell = medicineType_CustomDrawNodeCell;
                ado.SelectImageCollection = imageCollection1;
                ado.StateImageCollection = imageCollection1;
                ado.HisMedicineInStock_SelectImageClick = medicineType_SelectImageClick;
                ado.HisMedicineInStock_StateImageClick = medicineType_StateImageClick;
                ado.HisMedicineInStockNodeCellStyle = medicineType_NodeCellStyle;
                ado.btnLock_buttonClick = btnLock_buttonClick;
                ado.btnUnLock_buttonClick = btnUnLock_buttonClick;
                ////khóa
                HisMedicineInStockColumn mediTypeCodeLockCol = new HisMedicineInStockColumn(" ", "LOCK", 30, true, true);
                mediTypeCodeLockCol.VisibleIndex = 0;
                ado.HisMedicineInStockColumns.Add(mediTypeCodeLockCol);

                //Column mã
                HisMedicineInStockColumn mediTypeCodeNameCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_CODE", 75, true, true);
                mediTypeCodeNameCol.VisibleIndex = 1;
                mediTypeCodeNameCol.AllowEdit = true;//thêm thuộc tính
                mediTypeCodeNameCol.ReadOnly = true;
                ado.HisMedicineInStockColumns.Add(mediTypeCodeNameCol);

                //Column tên
                HisMedicineInStockColumn mediTypeNameCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_TYPE_NAME", 200, true, true);
                mediTypeNameCol.VisibleIndex = 2;
                mediTypeCodeNameCol.AllowEdit = true;//thêm thuộc tính
                mediTypeCodeNameCol.ReadOnly = true;
                ado.HisMedicineInStockColumns.Add(mediTypeNameCol);

                //Column hoạt chất
                HisMedicineInStockColumn hoatChatCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ACTIVE_INGR_BHYT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ACTIVE_INGR_BHYT_NAME", 150, false, false);
                hoatChatCol.VisibleIndex = 3;
                ado.HisMedicineInStockColumns.Add(hoatChatCol);

                //Column hàm lượng
                HisMedicineInStockColumn hamLuongCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONCENTRA", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONCENTRA", 100, false, false);
                hamLuongCol.VisibleIndex = 4;
                ado.HisMedicineInStockColumns.Add(hamLuongCol);

                //Column đường dùng
                HisMedicineInStockColumn hamDuongDung = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_USE_FORM_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MEDICINE_USE_FORM_NAME", 100, false, false);
                hamDuongDung.VisibleIndex = 5;
                ado.HisMedicineInStockColumns.Add(hamDuongDung);

                //Column đơn vị tính
                HisMedicineInStockColumn serviceUnitNameCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 60, false, false);
                serviceUnitNameCol.VisibleIndex = 6;
                ado.HisMedicineInStockColumns.Add(serviceUnitNameCol);

                //Column đơn vị qui đổi
                HisMedicineInStockColumn serviceUnitNameQuiDoiCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONVERT_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONVERT_UNIT_NAME", 60, false, false);
                serviceUnitNameQuiDoiCol.VisibleIndex = 7;
                ado.HisMedicineInStockColumns.Add(serviceUnitNameQuiDoiCol);

                //Column tỉ lệ qui đổi
                HisMedicineInStockColumn serviceConvertRatioCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONVERT_RATIO", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONVERT_RATIO", 60, false, false);
                serviceConvertRatioCol.VisibleIndex = 8;
                serviceConvertRatioCol.Format = new DevExpress.Utils.FormatInfo();
                serviceConvertRatioCol.Format.FormatString = "#,##0.";
                serviceConvertRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(serviceConvertRatioCol);

                //Column số lượng tồn
                HisMedicineInStockColumn totalAmountCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmount_Display", 100, false, false);
                totalAmountCol.VisibleIndex = 9;
                totalAmountCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountCol.Format.FormatString = "#,##0.00";
                totalAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(totalAmountCol);

                //Column Tồn theo đơn vị quy đổi (lấy số lượng tồn x tỷ lệ quy đổi)
                HisMedicineInStockColumn totalAmountConvertCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT_CONVERT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmountConvert_Display", 100, false, false);
                totalAmountConvertCol.VisibleIndex = 10;
                totalAmountConvertCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountConvertCol.Format.FormatString = "#,##0.00";
                totalAmountConvertCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(totalAmountConvertCol);

                //Column số lượng khả dụng
                HisMedicineInStockColumn availableAmoutCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmount_Display", 100, false, false);
                availableAmoutCol.VisibleIndex = 11;
                availableAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutCol.Format.FormatString = "#,##0.00";
                availableAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(availableAmoutCol);

                //Column Khả dụng theo đơn vị quy đổi (lấy số lượng khả dụng x tỷ lệ quy đổi)
                HisMedicineInStockColumn availableAmoutConvertCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_CONVERT_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmountConvert_Display", 100, false, false);
                availableAmoutConvertCol.VisibleIndex = 12;
                availableAmoutConvertCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutConvertCol.Format.FormatString = "#,##0.00";
                availableAmoutConvertCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(availableAmoutConvertCol);

                // Cơ số
                HisMedicineInStockColumn baseAmountCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BaseAmount_Display", 80, false, false);
                baseAmountCol.VisibleIndex = 13;
                baseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                baseAmountCol.Format.FormatString = "#,##0.00";
                baseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(baseAmountCol);

                // Cơ số thực
                //HisMedicineInStockColumn realBaseAmountCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REAL_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "RealBaseAmount", 80, false);
                //realBaseAmountCol.VisibleIndex = 12;
                //realBaseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                //realBaseAmountCol.Format.FormatString = "#,##0.";
                //realBaseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //ado.HisMedicineInStockColumns.Add(realBaseAmountCol);

                // Số lượng cần bù
                HisMedicineInStockColumn compensationAmountCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_COMPENSATION_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CompensationBaseAmount_Display", 80, false, false);
                compensationAmountCol.VisibleIndex = 14;
                compensationAmountCol.Format = new DevExpress.Utils.FormatInfo();
                compensationAmountCol.Format.FormatString = "#,##0.";
                compensationAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(compensationAmountCol);

                //Column nhà cung cấp
                HisMedicineInStockColumn impSupplierName = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SUPPLIER_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SUPPLIER_NAME", 100, false, false);
                impSupplierName.VisibleIndex = 15;
                ado.HisMedicineInStockColumns.Add(impSupplierName);

                //Column giá nhập
                HisMedicineInStockColumn impPriceCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_PRICE_DISPLAY", 100, false, false);
                impPriceCol.VisibleIndex = 16;
                impPriceCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                impPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(impPriceCol);

                //Column VAT %
                HisMedicineInStockColumn impVatRatioCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_VAT_RATIO_DISPLAY", 40, false, false);
                impVatRatioCol.VisibleIndex = 17;
                ado.HisMedicineInStockColumns.Add(impVatRatioCol);

                //Gia sau VAT
                HisMedicineInStockColumn impAfterVatCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_VAT_RATIO_NEXT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PRICE_AFTER_VAT", 100, false, false);
                impAfterVatCol.UnboundColumnType = UnboundColumnType.Object;
                impAfterVatCol.Format = new DevExpress.Utils.FormatInfo();
                impAfterVatCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                impAfterVatCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                impAfterVatCol.VisibleIndex = 18;
                ado.HisMedicineInStockColumns.Add(impAfterVatCol);

                //Column thành tiền
                HisMedicineInStockColumn totalPriceCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_PRICE", 100, false, false);
                totalPriceCol.VisibleIndex = 19;
                //totalPriceCol.isHAlignment = true;
                totalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                totalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(totalPriceCol);

                //Column giá bán sau vat
                HisMedicineInStockColumn totalExpPriceCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_EXP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_EXP_PRICE", 100, false, false);
                totalExpPriceCol.VisibleIndex = 20;
                //totalExpPriceCol.isHAlignment = true;
                totalExpPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalExpPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                totalExpPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(totalExpPriceCol);

                //Column hạn sử dụng
                HisMedicineInStockColumn expriredDateCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPIRED_DATE_DISPLAY", 100, false, false);
                expriredDateCol.UnboundColumnType = UnboundColumnType.Object;
                expriredDateCol.VisibleIndex = 21;
                ado.HisMedicineInStockColumns.Add(expriredDateCol);

                //Column số lô
                HisMedicineInStockColumn bidNumberCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PACKAGE_NUMBER", 100, false, false);
                bidNumberCol.VisibleIndex = 22;
                ado.HisMedicineInStockColumns.Add(bidNumberCol);

                //Column gói thầu
                HisMedicineInStockColumn packageNumberCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BID_NUMBER", 100, false, false);
                packageNumberCol.VisibleIndex = 23;
                ado.HisMedicineInStockColumns.Add(packageNumberCol);

                //Column số đăng ký
                HisMedicineInStockColumn registerNumberCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 100, false, false);
                registerNumberCol.VisibleIndex = 24;
                ado.HisMedicineInStockColumns.Add(registerNumberCol);

                //Column Lý do khóa
                HisMedicineInStockColumn reasonLock = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_LOCKING_REASON", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "LOCKING_REASON", 180, false, false);
                reasonLock.VisibleIndex = 25;
                ado.HisMedicineInStockColumns.Add(reasonLock);

                ////Column nhà cung cấp
                //HisMedicineInStockColumn supplierNameCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SUPPLIER_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SUPPLIER_NAME", 100, false);
                //supplierNameCol.VisibleIndex = 19;
                //ado.HisMedicineInStockColumns.Add(supplierNameCol);

                //Column số lượng cảnh báo
                HisMedicineInStockColumn alertMinInStockCol = new HisMedicineInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ALERT_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ALERT_MIN_IN_STOCK", 100, false, false);
                alertMinInStockCol.VisibleIndex = 26;
                alertMinInStockCol.Format = new DevExpress.Utils.FormatInfo();
                alertMinInStockCol.Format.FormatString = "#,##0.";
                alertMinInStockCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMedicineInStockColumns.Add(alertMinInStockCol);

                this.ucMedicineInfo = (UserControl)hisMediInStockProcessor.Run(ado);

                if (this.ucMedicineInfo != null)
                {
                    this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                    this.ucMedicineInfo.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitHisMaterialInStockTree()
        {
            try
            {
                hisMateInStockProcessor = new HisMaterialInStockProcessor();
                HisMaterialInStockInitADO ado = new HisMaterialInStockInitADO();
                ado.HisMaterialInStockColumns = new List<HisMaterialInStockColumn>();
                ado.IsShowSearchPanel = true;
                ado.KeyFieldName = "NodeId";
                ado.ParentFieldName = "ParentNodeId";
                ado.NameButtonClose = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonClose", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.NameButtonOpen = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonOpen", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.MenuItems = menuItemMaterial;
                ado.HisMaterialInStock_CustomUnboundColumnData = materialType_CustomUnboundColumnData;
                //ado.HisMaterialInStock_CustomDrawNodeCell = materialType_CustomDrawNodeCell;
                ado.HisMaterialInStock_GetSelectImage = materialType_GetSelectImage;
                ado.HisMaterialInStock_GetStateImage = materialType_GetStateImage;
                ado.HisMaterialInStock_SelectImageClick = materialType_SelectImageClick;
                ado.HisMaterialInStock_StateImageClick = materialType_StateImageClick;
                ado.HisMaterialInStockDoubleClick = materialType_DoubleClick;
                ado.SelectImageCollection = imageCollection1;
                ado.StateImageCollection = imageCollection1;
                ado.HisMaterialInStockNodeCellStyle = materialType_NodeCellStyle;
                ado.btnLock_buttonClick = btnLock_buttonClickMaterial;
                ado.btnUnLock_buttonClick = btnUnLock_buttonClickMaterial;
                ////khóa
                HisMaterialInStockColumn mediTypeLockCol = new HisMaterialInStockColumn(" ", "LOCK", 30, true);
                mediTypeLockCol.VisibleIndex = 0;
                ado.HisMaterialInStockColumns.Add(mediTypeLockCol);
                //Column mã
                HisMaterialInStockColumn mediTypeCodeNameCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_CODE", 75, false);
                mediTypeCodeNameCol.VisibleIndex = 1;
                ado.HisMaterialInStockColumns.Add(mediTypeCodeNameCol);
                //Column tên
                HisMaterialInStockColumn mediTypeNameCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "MATERIAL_TYPE_NAME", 200, false);
                mediTypeNameCol.VisibleIndex = 2;
                ado.HisMaterialInStockColumns.Add(mediTypeNameCol);
                //Column đơn vị tính
                HisMaterialInStockColumn serviceUnitNameCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERVICE_UNIT_NAME", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERVICE_UNIT_NAME", 100, false);
                serviceUnitNameCol.VisibleIndex = 3;
                ado.HisMaterialInStockColumns.Add(serviceUnitNameCol);

                //Column hàm lượng
                HisMaterialInStockColumn hamLuongCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_CONCENTRA", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CONCENTRA", 100, false);
                hamLuongCol.VisibleIndex = 4;
                ado.HisMaterialInStockColumns.Add(hamLuongCol);

                //Column số lượng tồn
                HisMaterialInStockColumn totalAmountCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TotalAmount_Display", 100, false);
                totalAmountCol.VisibleIndex = 5;
                totalAmountCol.Format = new DevExpress.Utils.FormatInfo();
                totalAmountCol.Format.FormatString = "#,##0.00";
                totalAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(totalAmountCol);

                //Column số lượng khả dụng
                HisMaterialInStockColumn availableAmoutCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_AVAILABLE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AvailableAmount_Display", 100, false);
                availableAmoutCol.VisibleIndex = 6;
                availableAmoutCol.Format = new DevExpress.Utils.FormatInfo();
                availableAmoutCol.Format.FormatString = "#,##0.00";
                availableAmoutCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(availableAmoutCol);

                // Cơ số
                HisMaterialInStockColumn baseAmountCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BaseAmount_Display", 80, false);
                baseAmountCol.VisibleIndex = 7;
                baseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                baseAmountCol.Format.FormatString = "#,##0.";
                baseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(baseAmountCol);

                // Cơ số thực
                //HisMaterialInStockColumn realBaseAmountCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REAL_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "RealBaseAmount", 80, false);
                //realBaseAmountCol.VisibleIndex = 7;
                //realBaseAmountCol.Format = new DevExpress.Utils.FormatInfo();
                //realBaseAmountCol.Format.FormatString = "#,##0.";
                //realBaseAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                //ado.HisMaterialInStockColumns.Add(realBaseAmountCol);

                // Số lượng cần bù
                HisMaterialInStockColumn compensationAmountCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_COMPENSATION_BASE_AMOUNT", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "CompensationBaseAmount_Display", 80, false);
                compensationAmountCol.VisibleIndex = 8;
                compensationAmountCol.Format = new DevExpress.Utils.FormatInfo();
                compensationAmountCol.Format.FormatString = "#,##0.00";
                compensationAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(compensationAmountCol);

                //Column giá nhập
                HisMaterialInStockColumn impPriceCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_PRICE_DISPLAY", 100, false);
                impPriceCol.VisibleIndex = 9;
                impPriceCol.Format = new DevExpress.Utils.FormatInfo();
                impPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                impPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(impPriceCol);

                //Column VAT %
                HisMaterialInStockColumn impVatRatioCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_IMP_VAT_RATIO", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IMP_VAT_RATIO_DISPLAY", 100, false);
                impVatRatioCol.VisibleIndex = 10;
                ado.HisMaterialInStockColumns.Add(impVatRatioCol);

                //Gia sau VAT
                HisMaterialInStockColumn impAfterVatCol = new HisMaterialInStockColumn("Giá sau VAT", "PRICE_AFTER_VAT", 100, false);
                impAfterVatCol.UnboundColumnType = UnboundColumnType.Object;
                impAfterVatCol.Format = new DevExpress.Utils.FormatInfo();
                impAfterVatCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                impAfterVatCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                impAfterVatCol.VisibleIndex = 11;
                ado.HisMaterialInStockColumns.Add(impAfterVatCol);

                //Column thành tiền
                HisMaterialInStockColumn totalPriceCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_PRICE", 100, false);
                totalPriceCol.VisibleIndex = 12;
                totalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                totalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(totalPriceCol);

                //Column giá bán sau vat
                HisMaterialInStockColumn totalExpPriceCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_EXP_PRICE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_EXP_PRICE", 100, false);
                totalExpPriceCol.VisibleIndex = 13;
                //totalExpPriceCol.isHAlignment = true;
                totalExpPriceCol.Format = new DevExpress.Utils.FormatInfo();
                totalExpPriceCol.Format.FormatString = "#,##0." + AddStringByConfig(HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                totalExpPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(totalExpPriceCol);

                //Column hạn sử dụng
                HisMaterialInStockColumn expriredDateCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_EXPIRED_DATE", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EXPIRED_DATE_DISPLAY", 100, false);
                expriredDateCol.UnboundColumnType = UnboundColumnType.Object;
                expriredDateCol.VisibleIndex = 14;
                ado.HisMaterialInStockColumns.Add(expriredDateCol);

                //Column gói thầu
                HisMaterialInStockColumn bidNumberCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BID_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BID_NUMBER", 100, false);
                bidNumberCol.VisibleIndex = 15;
                ado.HisMaterialInStockColumns.Add(bidNumberCol);

                //Column số lô
                HisMaterialInStockColumn packageNumberCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_PACKAGE_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "PACKAGE_NUMBER", 100, false);
                packageNumberCol.VisibleIndex = 16;
                ado.HisMaterialInStockColumns.Add(packageNumberCol);

                //Column Số seri
                HisMaterialInStockColumn serialNumberCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SERIAL_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SERIAL_NUMBER", 100, false);
                serialNumberCol.VisibleIndex = 17;
                ado.HisMaterialInStockColumns.Add(serialNumberCol);

                //Column số đăng ký
                HisMaterialInStockColumn registerNumberCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_REGISTER_NUMBER", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "REGISTER_NUMBER", 100, false);
                registerNumberCol.VisibleIndex = 18;
                ado.HisMaterialInStockColumns.Add(registerNumberCol);

                //Column Lý do khóa
                HisMaterialInStockColumn reasonLock = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_LOCKING_REASON", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "LOCKING_REASON", 180, false);
                reasonLock.VisibleIndex = 19;
                ado.HisMaterialInStockColumns.Add(reasonLock);

                //Column nhà cung cấp
                HisMaterialInStockColumn supplierNameCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_SUPPLIER_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "SUPPLIER_NAME", 100, false);
                supplierNameCol.VisibleIndex = 20;
                ado.HisMaterialInStockColumns.Add(supplierNameCol);

                //Column số lượng cảnh báo
                HisMaterialInStockColumn alertMinInStockCol = new HisMaterialInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_ALERT_MIN_IN_STOCK", Base.ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "ALERT_MIN_IN_STOCK", 100, false);
                alertMinInStockCol.VisibleIndex = 21;
                alertMinInStockCol.Format = new DevExpress.Utils.FormatInfo();
                alertMinInStockCol.Format.FormatString = "#,##0.00";
                alertMinInStockCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.HisMaterialInStockColumns.Add(alertMinInStockCol);

                this.ucMaterialInfo = (UserControl)hisMateInStockProcessor.Run(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void bloodType_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    HisBloodInStockSDO data = e.Row as HisBloodInStockSDO;
                    //if (data.ParentNodeId != null)
                    //{
                    if (e.Column.FieldName == "ExpiredDateStr" && !e.Node.HasChildren)
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString((long)(data.ExpiredDate ?? 0));
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitHisBloodTree()
        {
            try
            {
                hisBloodProcessor = new UC.HisBloodTypeInStock.HisBloodTypeInStockProcessor();
                HisBloodTypeInStockInitADO ado = new HisBloodTypeInStockInitADO();
                ado.HisBloodTypeInStockColumns = new List<HisBloodTypeInStockColumn>();
                ado.IsShowSearchPanel = true;
                ado.KeyFieldName = "NodeId";
                ado.ParentFieldName = "ParentNodeId";
                ado.NameButtonClose = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonClose", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.NameButtonOpen = Inventec.Common.Resource.Get.Value("UCMediStockSummary.NameButtonOpen", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                ado.HisBloodTypeInStock_CustomUnboundColumnData = bloodType_CustomUnboundColumnData;
                ado.HisBloodTypeInStock_GetSelectImage = bloodType_GetSelectImage;
                ado.HisBloodTypeInStock_GetStateImage = bloodType_GetStateImage;
                ado.SelectImageCollection = imageCollection1;
                ado.StateImageCollection = imageCollection1;
                ado.HisBloodTypeInStock_SelectImageClick = bloodType_SelectImageClick;
                ado.HisBloodTypeInStock_StateImageClick = bloodType_StateImageClick;
                //Column NULL
                HisBloodTypeInStockColumn colNull = new HisBloodTypeInStockColumn(" ", "NULL", 70, false);
                colNull.VisibleIndex = 0;
                ado.HisBloodTypeInStockColumns.Add(colNull);
                //Column mã
                HisBloodTypeInStockColumn bloodTypeCodeNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_CODE", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodTypeCode", 75, false);
                bloodTypeCodeNameCol.VisibleIndex = 1;
                ado.HisBloodTypeInStockColumns.Add(bloodTypeCodeNameCol);
                //Column tên
                HisBloodTypeInStockColumn bloodTypeNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_MEDICINE_TYPE_NAME", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodTypeName", 200, false);
                bloodTypeNameCol.VisibleIndex = 2;
                ado.HisBloodTypeInStockColumns.Add(bloodTypeNameCol);

                //Column Nhóm máu
                HisBloodTypeInStockColumn bloodAboCodeCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BLOOD_ABO_CODE", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodAboCode", 70, false);
                bloodAboCodeCol.VisibleIndex = 3;
                ado.HisBloodTypeInStockColumns.Add(bloodAboCodeCol);

                //Column Rh
                HisBloodTypeInStockColumn bloodRhCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_BLOOD_RH_CODE", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "BloodRhCode", 30, false);
                bloodRhCol.VisibleIndex = 4;
                ado.HisBloodTypeInStockColumns.Add(bloodRhCol);

                // Mã túi máu
                HisBloodTypeInStockColumn BloodCode = new HisBloodTypeInStockColumn("Mã túi máu", "BloodCode", 170, false);
                BloodCode.VisibleIndex = 5;
                ado.HisBloodTypeInStockColumns.Add(BloodCode);
                // Số lô
                HisBloodTypeInStockColumn bloodPackageNumber = new HisBloodTypeInStockColumn("Số lô", "PackageNumber", 100, false);
                bloodPackageNumber.VisibleIndex = 6;
                ado.HisBloodTypeInStockColumns.Add(bloodPackageNumber);
                // Hạn sử dụng
                HisBloodTypeInStockColumn bloodExpiredDate = new HisBloodTypeInStockColumn("Hạn sử dụng", "ExpiredDateStr", 120, false);
                bloodExpiredDate.VisibleIndex = 7;
                ado.HisBloodTypeInStockColumns.Add(bloodExpiredDate);

                // Ngày còn lại
                HisBloodTypeInStockColumn bloodExpiredDaysLeft = new HisBloodTypeInStockColumn("Ngày còn lại", "ExpiredDaysLeft", 120, false);
                bloodExpiredDaysLeft.VisibleIndex = 8;
                ado.HisBloodTypeInStockColumns.Add(bloodExpiredDaysLeft);

                // Nhà cung cấp
                HisBloodTypeInStockColumn bloodSupplierName = new HisBloodTypeInStockColumn("Nhà cung cấp", "SupplierName", 200, false);
                bloodSupplierName.VisibleIndex = 9;
                ado.HisBloodTypeInStockColumns.Add(bloodSupplierName);
                //Column dung tích
                HisBloodTypeInStockColumn serviceUnitNameCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__BLOOD_IN_STOCK__COLUMN_VOLUME", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "Volume", 100, false);
                serviceUnitNameCol.VisibleIndex = 10;
                ado.HisBloodTypeInStockColumns.Add(serviceUnitNameCol);

                //Column số lượng tồn
                HisBloodTypeInStockColumn totalAmountCol = new HisBloodTypeInStockColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_MEDI_STOCK_SUMMARY__MEDICINE_IN_STOCK__COLUMN_TOTAL_AMOUNT", ResourceLangManager.LanguageUCMediStockSummary, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "Amount", 80, false);
                totalAmountCol.VisibleIndex = 11;
                ado.HisBloodTypeInStockColumns.Add(totalAmountCol);

                this.ucBloodInfo = (UserControl)hisBloodProcessor.Run(ado);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataMediStockMetyAndMaty()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                MOS.Filter.HisMediStockMatyFilter HisMediStockMatyFilter = new MOS.Filter.HisMediStockMatyFilter();
                HisMediStockMaty = new List<HIS_MEDI_STOCK_MATY>();
                HisMediStockMaty = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_MATY>>("/api/HisMediStockMaty/Get", ApiConsumers.MosConsumer, HisMediStockMatyFilter, param);

                MOS.Filter.HisMediStockMetyFilter HisMediStockMetyFilter = new MOS.Filter.HisMediStockMetyFilter();
                HisMediStockMety = new List<HIS_MEDI_STOCK_METY>();
                HisMediStockMety = new BackendAdapter(param).Get<List<HIS_MEDI_STOCK_METY>>("/api/HisMediStockMety/Get", ApiConsumers.MosConsumer, HisMediStockMetyFilter, param);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowUCControl()
        {
            try
            {
                hisMediInStockProcessor.Reload(ucMedicineInfo, null, null);
                hisMateInStockProcessor.Reload(ucMaterialInfo, null, null);
                hisBloodProcessor.Reload(ucBloodInfo, new List<HisBloodInStockSDO>());

                this.mediStockIds = new List<long>();
                if (this._MediStocks != null && this._MediStocks.Count > 0)
                {
                    this.mediStockIds.AddRange(this._MediStocks.Where(p => p.IsCheck == true).Select(p => p.ID).ToList());
                }
                if (this.mediStockIds.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho", "Thông báo");
                    return;
                }

                bool isIncludeBaseAmount = (this.mediStockIds.Count == 1 && this._MediStocks.Any(a => a.ID == mediStockIds.FirstOrDefault() && a.IS_CABINET == 1));

                chkAlertMinStock.Enabled = this.mediStockIds.Count == 1;
                if (!chkAlertMinStock.Enabled && chkAlertMinStock.Checked)
                {
                    chkAlertMinStock.Checked = false;
                }
                chkHienThiLoHet.Enabled = this.mediStockIds.Count == 1;
                if (!chkHienThiLoHet.Enabled)
                    chkHienThiLoHet.Checked = false;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                this.panelControlMediMate.Controls.Clear();
                if (chkMedicine.Checked)
                {
                    if (this.ucMedicineInfo != null)
                    {
                        this.panelControlMediMate.Controls.Add(this.ucMedicineInfo);
                        this.ucMedicineInfo.Dock = DockStyle.Fill;

                        //Thuốc
                        MOS.Filter.HisMedicineStockViewFilter mediFilter = new MOS.Filter.HisMedicineStockViewFilter();
                        mediFilter.MEDI_STOCK_IDs = this.mediStockIds;
                        mediFilter.INCLUDE_EMPTY = chkShowLineZero.Checked;
                        mediFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;
                        mediFilter.INCLUDE_EMPTY_MEDICINE = chkHienThiLoHet.Checked;

                        if (cboIsActive.EditValue != null)
                        {
                            if (cboIsActive.EditValue.ToString().Equals("01"))
                            {
                                mediFilter.MEDICINE_IS_ACTIVE = null;
                            }
                            else if (cboIsActive.EditValue.ToString().Equals("03"))
                            {
                                mediFilter.MEDICINE_IS_ACTIVE = false;
                            }
                            else if (cboIsActive.EditValue.ToString().Equals("02"))
                            {
                                mediFilter.MEDICINE_IS_ACTIVE = true;
                            }
                        }

                        if (cboPatientType.EditValue != null)
                        {
                            mediFilter.INCLUDE_EXP_PRICE = true;
                            mediFilter.EXP_PATIENT_TYPE_ID = long.Parse(cboPatientType.EditValue.ToString());
                        }

                        lstMediInStocks = new List<HisMedicineInStockSDO>();
                        lstMediInStocks = new BackendAdapter(param).Get<List<HisMedicineInStockSDO>>(HisRequestUriStore.HIS_MEDICINE_GETVIEW_IN_STOCK_MEDICINE_TYPE_TREE, ApiConsumers.MosConsumer, mediFilter, param);

                        if (lstMediInStocks != null && lstMediInStocks.Count > 0)
                        {
                            var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                            if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                            {
                                var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                                if (dataMediTypes != null && dataMediTypes.Count > 0)
                                {
                                    lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMediInStocks = new List<HisMedicineInStockSDO>();
                                }
                            }
                            else
                            {
                                var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                                if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                                {
                                    var dataMediTypes = BackendDataWorker.Get<HIS_MEDICINE_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                    if (dataMediTypes != null && dataMediTypes.Count > 0)
                                    {
                                        lstMediInStocks = lstMediInStocks.Where(p => dataMediTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MEDICINE_TYPE_ID)).ToList();
                                    }
                                    else
                                    {
                                        lstMediInStocks = new List<HisMedicineInStockSDO>();
                                    }
                                }
                            }
                        }

                        if (chkAlertMinStock.Checked)
                            chkAlertMinStock_CheckedChanged(null, null);
                        else
                        {
                            if (ChkValidToTime.Checked)
                            {
                                ChkValidToTime_CheckedChanged(null, null);
                            }
                            else
                                if (ChkExpiredDate.Checked)
                                ChkExpiredDate_CheckedChanged(null, null);
                            else if (ChkNoExpiredDate.Checked)
                                ChkNoExpiredDate_CheckedChanged(null, null);
                            else
                                hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocks, this.mediStockIds);
                        }
                    }
                }
                else if (chkMaterial.Checked)
                {
                    if (this.ucMaterialInfo != null)
                    {
                        this.panelControlMediMate.Controls.Add(this.ucMaterialInfo);
                        this.ucMaterialInfo.Dock = DockStyle.Fill;

                        //Vật tư
                        MOS.Filter.HisMaterialStockViewFilter mateFilter = new MOS.Filter.HisMaterialStockViewFilter();
                        mateFilter.MEDI_STOCK_IDs = this.mediStockIds;
                        mateFilter.INCLUDE_EMPTY = chkShowLineZero.Checked;
                        mateFilter.INCLUDE_BASE_AMOUNT = isIncludeBaseAmount;
                        mateFilter.INCLUDE_EMPTY_MATERIAL = chkHienThiLoHet.Checked;
                        if (cboIsActive.EditValue != null)
                        {
                            if (cboIsActive.EditValue.ToString().Equals("01"))
                            {
                                mateFilter.MATERIAL_IS_ACTIVE = null;
                            }
                            else if (cboIsActive.EditValue.ToString().Equals("03"))
                            {
                                mateFilter.MATERIAL_IS_ACTIVE = false;
                            }
                            else if (cboIsActive.EditValue.ToString().Equals("02"))
                            {
                                mateFilter.MATERIAL_IS_ACTIVE = true;
                            }
                        }
                        if (cboPatientType.EditValue != null)
                        {
                            mateFilter.INCLUDE_EXP_PRICE = true;
                            mateFilter.EXP_PATIENT_TYPE_ID = long.Parse(cboPatientType.EditValue.ToString());
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("mateFilter", mateFilter));
                        lstMateInStocks = new List<HisMaterialInStockSDO>();
                        lstMateInStocks = new BackendAdapter(param).Get<List<HisMaterialInStockSDO>>(HisRequestUriStore.HIS_MATERIAL_GETVIEW_IN_STOCK_MATERIAL_TYPE_TREE, ApiConsumers.MosConsumer, mateFilter, param);


                        if (lstMateInStocks != null && lstMateInStocks.Count > 0)
                        {
                            var dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS == 1).ToList();
                            if (dataMediStocks != null && dataMediStocks.Count == this.mediStockIds.Count)
                            {
                                var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS == 1).ToList();
                                if (dataMateTypes != null && dataMateTypes.Count > 0)
                                {
                                    lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                                }
                                else
                                {
                                    lstMateInStocks = new List<HisMaterialInStockSDO>();
                                }
                            }
                            else
                            {
                                var dataMediStocksBUSINESS = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => this.mediStockIds.Contains(p.ID) && p.IS_BUSINESS != 1).ToList();
                                if (dataMediStocksBUSINESS != null && dataMediStocksBUSINESS.Count == this.mediStockIds.Count)
                                {
                                    var dataMateTypes = BackendDataWorker.Get<HIS_MATERIAL_TYPE>().Where(p => p.IS_BUSINESS != 1).ToList();
                                    if (dataMateTypes != null && dataMateTypes.Count > 0)
                                    {
                                        lstMateInStocks = lstMateInStocks.Where(p => dataMateTypes.Select(o => o.ID).Distinct().ToList().Contains(p.MATERIAL_TYPE_ID)).ToList();
                                    }
                                    else
                                    {
                                        lstMateInStocks = new List<HisMaterialInStockSDO>();
                                    }
                                }
                            }
                        }

                        if (chkAlertMinStock.Checked)
                            chkAlertMinStock_CheckedChanged(null, null);
                        else
                        {
                            if (ChkValidToTime.Checked)
                            {
                                ChkValidToTime_CheckedChanged(null, null);
                            }
                            else
                                if (ChkExpiredDate.Checked)
                                ChkExpiredDate_CheckedChanged(null, null);
                            else if (ChkNoExpiredDate.Checked)
                                ChkExpiredDate_CheckedChanged(null, null);
                            else
                                hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocks, this.mediStockIds);
                        }
                    }
                }
                else
                {
                    if (this.ucBloodInfo != null)
                    {
                        this.panelControlMediMate.Controls.Add(this.ucBloodInfo);
                        this.ucBloodInfo.Dock = DockStyle.Fill;

                        HisBloodStockViewFilter filterBlood = new HisBloodStockViewFilter();
                        filterBlood.MEDI_STOCK_IDs = this.mediStockIds;
                        if (ChkExpiredDate.Checked && dtExpiredDate.EditValue != null)
                            filterBlood.EXPIRED_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtExpiredDate.DateTime.ToString("yyyyMMdd") + "000000");
                        // filter.INCLUDE_EMPTY = chkShowLineZero.Checked;
                        lstBloodInStocks = new List<HisBloodInStockSDO>();
                        lstBloodInStocks = new BackendAdapter(param).Get<List<HisBloodInStockSDO>>("api/HisBlood/GetInStockBloodWithTypeTree", ApiConsumers.MosConsumer, filterBlood, param);
                        if (lstBloodInStocks != null)
                        {
                            lstBloodInStocks = lstBloodInStocks.OrderBy(o => o.BloodTypeCode).ToList();
                        }
                        hisBloodProcessor.Reload(ucBloodInfo, lstBloodInStocks);

                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataGridMediStock()
        {
            try
            {
                WaitingManager.Show();
                gridControlMediStock.DataSource = null;
                var _WorkPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.FirstOrDefault(p => p.RoomId == this.RoomId);
                if (_WorkPlace != null)
                {
                    _MediStocks = new List<MediStockADO>();

                    var datas = BackendDataWorker.Get<V_HIS_USER_ROOM>().Where(p => p.LOGINNAME.Trim() == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName().Trim() && _WorkPlace.BranchId == p.BRANCH_ID).ToList();
                    List<V_HIS_MEDI_STOCK> dataMediStocks = new List<V_HIS_MEDI_STOCK>();
                    if (datas != null)
                    {
                        List<long> roomIds = datas.Select(p => p.ROOM_ID).ToList();
                        dataMediStocks = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(p => roomIds.Contains(p.ROOM_ID)).ToList();
                    }

                    if (dataMediStocks != null && dataMediStocks.Count > 0)
                    {
                        this._MediStocks.AddRange((from r in dataMediStocks select new MediStockADO(r)).ToList());
                        var data = _MediStocks.FirstOrDefault(p => p.ROOM_ID == this.RoomId);
                        if (data != null)
                        {
                            var dataMedi = this._MediStocks.FirstOrDefault(p => p.ID == data.ID);
                            if (dataMedi != null)
                            {
                                dataMedi.IsCheck = true;
                                data.TYPE = 1;
                                this.mediStockIds.Clear();
                                this.mediStockIds.Add(dataMedi.ID);
                            }
                        }
                        this._MediStocks = this._MediStocks.OrderBy(p => p.TYPE).ToList();
                        gridControlMediStock.DataSource = _MediStocks;
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_MEDI_STOCK)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataGridMediStock();
                hisMediInStockProcessor.Reload(ucMedicineInfo, null, null);
                hisMateInStockProcessor.Reload(ucMaterialInfo, null, null);
                hisBloodProcessor.Reload(ucBloodInfo, new List<HisBloodInStockSDO>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMedicine.Checked)
                {
                    //if (this.mediStockIds.Count == 0)
                    //    return;
                    ShowUCControl();
                    isCheck = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkMaterial.Checked)
                {
                    //if (this.mediStockIds.Count == 0)
                    //    return;
                    ShowUCControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBlood_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkBlood.Checked)
                {
                    //if (this.mediStockIds.Count == 0)
                    //    return;
                    ShowUCControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintType.IN_PHIEU_TONG_HOP_TON_KHO_T_VT_M);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefesh_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.mediStockIds = new List<long>();
                LoadDataGridMediStock();
                hisMediInStockProcessor.Reload(ucMedicineInfo, null, null);
                hisMateInStockProcessor.Reload(ucMaterialInfo, null, null);
                hisBloodProcessor.Reload(ucBloodInfo, new List<HisBloodInStockSDO>());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnSearch()
        {
            try
            {
                btnSearch_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void bbtnRefesh()
        {
            try
            {
                btnRefesh_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            try
            {
                LoadPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPrint()
        {
            try
            {
                if (this.mediStockIds != null && this.mediStockIds.Count > 0)
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        WaitingManager.Show();
                        bool success = false;
                        CommonParam param = new CommonParam();
                        Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                        Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store();
                        Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();
                        var patientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                        #region Thuốc
                        if (chkMedicine.Checked)
                        {
                            Dictionary<long, MedicineInStockExportADO> dicData = new Dictionary<long, MedicineInStockExportADO>();
                            Dictionary<long, List<HIS_MEDICINE_PATY>> dicMedicinePaty = new Dictionary<long, List<HIS_MEDICINE_PATY>>();
                            List<MedicineInStockExportADO> lstMedicineExport = new List<MedicineInStockExportADO>();//all
                            //MOS.Filter.HisMedicineBeanViewFilter beanFilter = new MOS.Filter.HisMedicineBeanViewFilter();
                            //beanFilter.MEDI_STOCK_IDs = this.mediStockIds;
                            //var lstMedicineBeans = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_BEAN>>("api/HisMedicineBean/GetView", ApiConsumers.MosConsumer, beanFilter, param);
                            var lstMedicineBeans = (this.lstMediInStocks != null && this.lstMediInStocks.Count > 0) ? this.lstMediInStocks.Where(o => !o.isTypeNode && o.ID > 0).ToList() : null;
                            Inventec.Common.Logging.LogSystem.Info("lstMediInStocks" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => lstMediInStocks), lstMediInStocks.Where(o => o.MEDICINE_TYPE_CODE.Equals("914ACIDAM"))));
                            if (lstMedicineBeans != null && lstMedicineBeans.Count > 0)
                            {
                                var lstMediBeanGroup = lstMedicineBeans.GroupBy(p => p.ID).Select(grc => grc.ToList()).ToList();
                                List<long> medicineIds = lstMediBeanGroup.Select(p => p.FirstOrDefault().ID).ToList();
                                MOS.Filter.HisMedicinePatyFilter patyFilter = new MOS.Filter.HisMedicinePatyFilter();
                                patyFilter.MEDICINE_IDs = medicineIds;
                                var lstMediPaty = new BackendAdapter(param).Get<List<HIS_MEDICINE_PATY>>("api/HisMedicinePaty/Get", ApiConsumers.MosConsumer, patyFilter, param);
                                if (lstMediPaty != null)
                                {
                                    foreach (var paty in lstMediPaty)
                                    {
                                        if (!dicMedicinePaty.ContainsKey(paty.PATIENT_TYPE_ID))
                                            dicMedicinePaty[paty.PATIENT_TYPE_ID] = new List<HIS_MEDICINE_PATY>();
                                        dicMedicinePaty[paty.PATIENT_TYPE_ID].Add(paty);
                                    }
                                }
                                foreach (var itemGroup in lstMediBeanGroup)
                                {
                                    MedicineInStockExportADO ado = new MedicineInStockExportADO();
                                    Mapper.CreateMap<HisMedicineInStockSDO, MedicineInStockExportADO>();
                                    ado = Mapper.Map<HisMedicineInStockSDO, MedicineInStockExportADO>(itemGroup[0]);
                                    ado.AMOUNT = itemGroup.Sum(p => p.TotalAmount ?? 0);
                                    var expiredDate = itemGroup.FirstOrDefault(o => o.EXPIRED_DATE != null);
                                    var alertExpiredDate = itemGroup.FirstOrDefault(o => o.ALERT_EXPIRED_DATE != null);
                                    ado.EXPIRED_DATE_STR = expiredDate != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(expiredDate.EXPIRED_DATE)) : null;
                                    ado.ALERT_EXPIRED_DATE_STR = alertExpiredDate != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(alertExpiredDate.ALERT_EXPIRED_DATE)) : null;
                                    ado.IMP_PRICE_VAT = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
                                    ado.AVAILABLE_AMOUNT = itemGroup[0].AvailableAmount;
                                    ado.MEDICINE_ID = itemGroup[0].ID;
                                    ado.TDL_BID_NUMBER = itemGroup[0].BID_NUMBER;
                                    //MOS.Filter.HisMedicineTypeViewFilter mediTypeFilter = new MOS.Filter.HisMedicineTypeViewFilter();
                                    //mediTypeFilter.ID = itemGroup[0].MEDICINE_TYPE_ID;
                                    //var lstMedicineType = new BackendAdapter(param).Get<List<V_HIS_MEDICINE_TYPE>>(HisRequestUriStore.HIS_MEDICINE_TYPE_GETVIEW, ApiConsumers.MosConsumer, mediTypeFilter, param);
                                    var _medicineType = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(p => p.ID == itemGroup[0].MEDICINE_TYPE_ID);
                                    if (_medicineType != null)
                                    {
                                        ado.MEDICINE_TYPE_PROPRIETARY_NAME = _medicineType.MEDICINE_TYPE_PROPRIETARY_NAME;
                                        ado.MEDICINE_USE_FORM_CODE = _medicineType.MEDICINE_USE_FORM_CODE;
                                        ado.MEDICINE_USE_FORM_NAME = _medicineType.MEDICINE_USE_FORM_NAME;
                                        ado.MEDICINE_LINE_NAME = _medicineType.MEDICINE_LINE_NAME;
                                        ado.DESCRIPTION = _medicineType.DESCRIPTION;
                                        if (_medicineType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__GN)
                                        {
                                            ado.IS_ADDICTIVE_STR = "X";
                                        }
                                        if (_medicineType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__KS)
                                        {
                                            ado.IS_ANTIBIOTIC_STR = "X";
                                        }
                                        if (_medicineType.MEDICINE_GROUP_ID == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_GROUP.ID__HT)
                                        {
                                            ado.IS_NEUROLOGICAL_STR = "X";
                                        }
                                    }
                                    //if (this.lstMediInStocks != null && this.lstMediInStocks.Count > 0)
                                    //{
                                    //    //var _mediSdo = this.lstMediInStocks.FirstOrDefault(p => p.MEDICINE_TYPE_ID == itemGroup[0].MEDICINE_TYPE_ID && string.IsNullOrEmpty(p.ParentNodeId));
                                    //    var _mediSdo = this.lstMediInStocks.FirstOrDefault(p => p.MEDICINE_TYPE_ID == itemGroup[0].MEDICINE_TYPE_ID && p.ID == itemGroup[0].MEDICINE_ID);
                                    //    if (_mediSdo != null)
                                    //    {
                                    //        ado.AVAILABLE_AMOUNT = _mediSdo.AvailableAmount;
                                    //    }
                                    //}
                                    //lstMedicineExport.Add(ado);
                                    dicData[ado.MEDICINE_ID] = ado;
                                }
                            }
                            int count = 1;
                            foreach (var item in patientType)
                            {
                                if (count > 10)
                                    break;
                                System.Reflection.PropertyInfo piPatientName = typeof(MedicineInStockExportADO).GetProperty("PATIENT_TYPE_NAME");
                                System.Reflection.PropertyInfo piName = typeof(MedicineInStockExportADO).GetProperty("PATIENT_TYPE_NAME_" + count);
                                System.Reflection.PropertyInfo piPrice = typeof(MedicineInStockExportADO).GetProperty("EXP_PRICE_" + count);
                                System.Reflection.PropertyInfo piPriceVAT = typeof(MedicineInStockExportADO).GetProperty("EXP_VAT_RATIO");
                                if (!dicMedicinePaty.ContainsKey(item.ID))
                                {
                                    continue;
                                }
                                var datasss = dicMedicinePaty[item.ID];
                                foreach (var medicine in datasss)
                                {
                                    if (dicData.ContainsKey(medicine.MEDICINE_ID))
                                    {
                                        var rdo = dicData[medicine.MEDICINE_ID];
                                        piName.SetValue(rdo, item.PATIENT_TYPE_NAME);
                                        piPrice.SetValue(rdo, medicine.EXP_PRICE);
                                        piPatientName.SetValue(rdo, item.PATIENT_TYPE_NAME);
                                        piPriceVAT.SetValue(rdo, medicine.EXP_VAT_RATIO);
                                    }
                                }
                                //set key ten đơn
                                singleTag.AddSingleKey(store, "PATIENT_TYPE_NAME_" + count, item.PATIENT_TYPE_NAME);
                                count++;
                            }
                            foreach (var item in patientType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                            {
                                if (!dicMedicinePaty.ContainsKey(item.ID))
                                {
                                    continue;
                                }
                                var datasss = dicMedicinePaty[item.ID];
                                foreach (var medicine in datasss)
                                {
                                    if (dicData.ContainsKey(medicine.MEDICINE_ID))
                                    {
                                        var rdo = dicData[medicine.MEDICINE_ID];
                                        if (rdo.DicExpPrice == null)
                                        {
                                            rdo.DicExpPrice = new Dictionary<string, decimal>();
                                        }
                                        if (rdo.DicExpVatRatio == null)
                                        {
                                            rdo.DicExpVatRatio = new Dictionary<string, decimal>();
                                        }
                                        if (rdo.DicPatientTypeName == null)
                                        {
                                            rdo.DicPatientTypeName = new Dictionary<string, string>();
                                        }
                                        rdo.DicExpPrice["EXP_PRICE_" + item.PATIENT_TYPE_CODE] = medicine.EXP_PRICE;
                                        rdo.DicExpVatRatio["EXP_VAT_RATIO_" + item.PATIENT_TYPE_CODE] = medicine.EXP_VAT_RATIO;
                                        rdo.DicPatientTypeName["PATIENT_TYPE_NAME_" + item.PATIENT_TYPE_CODE] = item.PATIENT_TYPE_NAME;
                                    }
                                }
                            }
                            lstMedicineExport = dicData.Select(s => s.Value).OrderBy(p => p.MEDICINE_TYPE_ID).ToList();

                            this.fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "DanhMucThuoc.xls");
                            store.ReadTemplate(System.IO.Path.GetFullPath(this.fileName));
                            store.SetCommonFunctions();
                            objectTag.AddObjectData(store, "ExportResult", lstMedicineExport);
                        }
                        #endregion

                        #region Vật tư
                        else if (chkMaterial.Checked)
                        {
                            Dictionary<long, MaterialInStockExportADO> dicData = new Dictionary<long, MaterialInStockExportADO>();
                            Dictionary<long, List<HIS_MATERIAL_PATY>> dicMaterialPaty = new Dictionary<long, List<HIS_MATERIAL_PATY>>();
                            List<MaterialInStockExportADO> lstMaterialExport = new List<MaterialInStockExportADO>();//all
                            //MOS.Filter.HisMaterialBeanViewFilter materialBeanViewFilte = new MOS.Filter.HisMaterialBeanViewFilter();
                            //materialBeanViewFilte.MEDI_STOCK_IDs = this.mediStockIds;
                            //var lstMaterialBeans = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_BEAN>>("api/HisMaterialBean/GetView", ApiConsumers.MosConsumer, materialBeanViewFilte, param);
                            var lstMaterialBeans = (this.lstMateInStocks != null && this.lstMateInStocks.Count > 0) ? this.lstMateInStocks.Where(o => !o.isTypeNode && o.ID > 0).ToList() : null;
                            if (lstMaterialBeans != null && lstMaterialBeans.Count > 0)
                            {
                                var lstMateBeanGroup = lstMaterialBeans.GroupBy(p => p.ID).Select(grc => grc.ToList()).ToList();
                                List<long> materialIds = lstMateBeanGroup.Select(p => p.FirstOrDefault().ID).ToList();
                                MOS.Filter.HisMaterialPatyFilter matePatyFilter = new MOS.Filter.HisMaterialPatyFilter();
                                matePatyFilter.MATERIAL_IDs = materialIds;
                                var lstMatePaty = new BackendAdapter(param).Get<List<HIS_MATERIAL_PATY>>("api/HisMaterialPaty/Get", ApiConsumers.MosConsumer, matePatyFilter, param);

                                if (lstMatePaty != null && lstMatePaty.Count > 0)
                                {
                                    foreach (var itemMatePaty in lstMatePaty)
                                    {
                                        if (!dicMaterialPaty.ContainsKey(itemMatePaty.PATIENT_TYPE_ID))
                                            dicMaterialPaty[itemMatePaty.PATIENT_TYPE_ID] = new List<HIS_MATERIAL_PATY>();
                                        dicMaterialPaty[itemMatePaty.PATIENT_TYPE_ID].Add(itemMatePaty);
                                    }
                                }

                                foreach (var itemGroup in lstMateBeanGroup)
                                {
                                    MaterialInStockExportADO ado = new MaterialInStockExportADO();
                                    Mapper.CreateMap<HisMaterialInStockSDO, MaterialInStockExportADO>();
                                    ado = Mapper.Map<HisMaterialInStockSDO, MaterialInStockExportADO>(itemGroup[0]);
                                    ado.AMOUNT = itemGroup.Sum(p => p.TotalAmount ?? 0);
                                    var expiredDate = itemGroup.FirstOrDefault(o => o.EXPIRED_DATE != null);
                                    var alertExpiredDate = itemGroup.FirstOrDefault(o => o.ALERT_EXPIRED_DATE != null);
                                    ado.EXPIRED_DATE_STR = expiredDate != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(expiredDate.EXPIRED_DATE)) : null;
                                    ado.ALERT_EXPIRED_DATE_STR = alertExpiredDate != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(Convert.ToInt64(alertExpiredDate.ALERT_EXPIRED_DATE)) : null;

                                    ado.IMP_PRICE_VAT = ado.IMP_PRICE * (1 + ado.IMP_VAT_RATIO);
                                    ado.AVAILABLE_AMOUNT = itemGroup[0].AvailableAmount;
                                    ado.MATERIAL_ID = itemGroup[0].ID;
                                    ado.TDL_BID_NUMBER = itemGroup[0].BID_NUMBER;
                                    //MOS.Filter.HisMaterialTypeViewFilter mateTypeFilter = new MOS.Filter.HisMaterialTypeViewFilter();
                                    //mateTypeFilter.ID = itemGroup[0].MATERIAL_TYPE_ID;
                                    //var lstMaterialType = new BackendAdapter(param).Get<List<V_HIS_MATERIAL_TYPE>>(HisRequestUriStore.HIS_MATERIAL_TYPE_GETVIEW, ApiConsumers.MosConsumer, mateTypeFilter, param);
                                    var _materialType = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(p => p.ID == itemGroup[0].MATERIAL_TYPE_ID);
                                    if (_materialType != null)
                                    {
                                        if (_materialType.IS_CHEMICAL_SUBSTANCE == 1)
                                        {
                                            ado.IS_CHEMICAL_SUBSTANCE_STR = "X";
                                        }
                                    }
                                    dicData[ado.MATERIAL_ID] = ado;
                                }
                            }

                            int count = 1;
                            foreach (var item in patientType)
                            {
                                if (count > 10)
                                    break;
                                System.Reflection.PropertyInfo piPatientName = typeof(MaterialInStockExportADO).GetProperty("PATIENT_TYPE_NAME");
                                System.Reflection.PropertyInfo piName = typeof(MaterialInStockExportADO).GetProperty("PATIENT_TYPE_NAME_" + count);
                                System.Reflection.PropertyInfo piPrice = typeof(MaterialInStockExportADO).GetProperty("EXP_PRICE_" + count);
                                System.Reflection.PropertyInfo piPriceVAT = typeof(MaterialInStockExportADO).GetProperty("EXP_VAT_RATIO");
                                if (!dicMaterialPaty.ContainsKey(item.ID))
                                {
                                    continue;
                                }
                                var datasss = dicMaterialPaty[item.ID];
                                foreach (var material in datasss)
                                {
                                    if (dicData.ContainsKey(material.MATERIAL_ID))
                                    {
                                        var rdo = dicData[material.MATERIAL_ID];
                                        piName.SetValue(rdo, item.PATIENT_TYPE_NAME);
                                        piPrice.SetValue(rdo, material.EXP_PRICE);
                                        piPatientName.SetValue(rdo, item.PATIENT_TYPE_NAME);
                                        piPriceVAT.SetValue(rdo, material.EXP_VAT_RATIO);
                                    }
                                }
                                //set key ten đơn
                                singleTag.AddSingleKey(store, "PATIENT_TYPE_NAME_" + count, item.PATIENT_TYPE_NAME);
                                count++;
                            }
                            foreach (var item in patientType.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE))
                            {
                                if (!dicMaterialPaty.ContainsKey(item.ID))
                                {
                                    continue;
                                }
                                var datasss = dicMaterialPaty[item.ID];
                                foreach (var material in datasss)
                                {
                                    if (dicData.ContainsKey(material.MATERIAL_ID))
                                    {
                                        var rdo = dicData[material.MATERIAL_ID];
                                        if (rdo.DicExpPrice == null)
                                        {
                                            rdo.DicExpPrice = new Dictionary<string, decimal>();
                                        }
                                        if (rdo.DicExpVatRatio == null)
                                        {
                                            rdo.DicExpVatRatio = new Dictionary<string, decimal>();
                                        }
                                        if (rdo.DicPatientTypeName == null)
                                        {
                                            rdo.DicPatientTypeName = new Dictionary<string, string>();
                                        }
                                        rdo.DicExpPrice["EXP_PRICE_" + item.PATIENT_TYPE_CODE] = material.EXP_PRICE;
                                        rdo.DicExpVatRatio["EXP_VAT_RATIO_" + item.PATIENT_TYPE_CODE] = material.EXP_VAT_RATIO;
                                        rdo.DicPatientTypeName["PATIENT_TYPE_NAME_" + item.PATIENT_TYPE_CODE] = item.PATIENT_TYPE_NAME;
                                    }
                                }
                            }
                            lstMaterialExport = dicData.Select(s => s.Value).OrderBy(p => p.MATERIAL_TYPE_ID).ToList();

                            this.fileName = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp\\", "DanhMucVatTu.xls");
                            store.ReadTemplate(System.IO.Path.GetFullPath(this.fileName));
                            store.SetCommonFunctions();
                            objectTag.AddObjectData(store, "ExportResult", lstMaterialExport);
                        }
                        #endregion

                        #region ----Máu
                        else if (chkBlood.Checked)
                        {
                            if (ucBloodInfo != null)
                            {
                                hisBloodProcessor.Export(ucBloodInfo, saveFileDialog.FileName);
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                                {
                                    System.Diagnostics.Process.Start(saveFileDialog.FileName);
                                }
                            }
                            return;
                        }
                        #endregion

                        WaitingManager.Hide();

                        objectTag.SetUserFunction(store, "FlFuncElement", new FlFuncElementFunction());
                        success = store.OutFile(saveFileDialog.FileName);
                        MessageManager.Show(this.ParentForm, null, success);
                        if (!success)
                            return;
                        if (DevExpress.XtraEditors.XtraMessageBox.Show("Bạn có muốn mở file ngay?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(saveFileDialog.FileName);
                        }
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn chưa chọn kho", "Thông báo");
                    return;
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            try
            {
                var expandoDict = expando as IDictionary<string, object>;
                expandoDict[propertyName] = propertyValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWork_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                List<MediStockADO> listResult = new List<MediStockADO>();
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MediStockADO> rearchResult = new List<MediStockADO>();

                    rearchResult = this._MediStocks.Where(o =>
                                                     ((o.DEPARTMENT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.DEPARTMENT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDI_STOCK_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDI_STOCK_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.ROOM_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.ROOM_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     )
                                                     ).Distinct().ToList();


                    listResult.AddRange(rearchResult);
                }
                else
                {
                    listResult.AddRange(this._MediStocks);
                }
                gridControlMediStock.BeginUpdate();
                if (listResult != null && listResult.Count > 0)
                {
                    listResult = listResult.OrderBy(p => p.TYPE).ToList();
                }

                gridControlMediStock.DataSource = listResult;
                gridControlMediStock.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMediStock_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    if (hi.InRowCell)
                    {
                        if (hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit))
                        {
                            view.FocusedRowHandle = hi.RowHandle;
                            view.FocusedColumn = hi.Column;
                            view.ShowEditor();
                            DevExpress.XtraEditors.CheckEdit checkEdit = view.ActiveEditor as DevExpress.XtraEditors.CheckEdit;
                            DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo checkInfo = (DevExpress.XtraEditors.ViewInfo.CheckEditViewInfo)checkEdit.GetViewInfo();
                            Rectangle glyphRect = checkInfo.CheckInfo.GlyphRect;
                            GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                            Rectangle gridGlyphRect =
                                new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                 viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                 glyphRect.Width,
                                 glyphRect.Height);
                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                view.CloseEditor();
                                if (!view.IsCellSelected(hi.RowHandle, hi.Column))
                                {
                                    view.SelectCell(hi.RowHandle, hi.Column);
                                }
                                else
                                {
                                    view.UnselectCell(hi.RowHandle, hi.Column);
                                }
                            }
                            else
                            {

                                checkEdit.Checked = !checkEdit.Checked;
                                view.CloseEditor();

                                var dataRow = (MediStockADO)gridViewMediStock.GetRow(hi.RowHandle);
                                if (checkEdit.Checked && dataRow != null)
                                {
                                    dataRow.TYPE = 1;
                                }
                                else
                                {
                                    dataRow.TYPE = 999;
                                }
                                if (this._MediStocks != null && this._MediStocks.Count > 0)
                                {
                                    var dataChecks = this._MediStocks.Where(p => p.IsCheck == true).ToList();
                                    if (dataChecks != null && dataChecks.Count > 0)
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[5];
                                    }
                                    else
                                    {
                                        gridColumnCheck.Image = imageListIcon.Images[6];
                                    }

                                    chkHienThiLoHet.Enabled = dataChecks.Count == 1;
                                    if (!chkHienThiLoHet.Enabled)
                                        chkHienThiLoHet.Checked = false;
                                }
                            }
                            (e as DevExpress.Utils.DXMouseEventArgs).Handled = true;
                        }
                    }
                    if (hi.HitTest == GridHitTest.Column)
                    {
                        if (hi.Column.FieldName == "IsCheck")
                        {
                            gridColumnCheck.Image = imageListIcon.Images[5];
                            gridViewMediStock.BeginUpdate();
                            if (this._MediStocks == null)
                                this._MediStocks = new List<MediStockADO>();
                            if (isCheckAll == true)
                            {
                                foreach (var item in this._MediStocks)
                                {
                                    item.IsCheck = true;
                                    item.TYPE = 1;
                                }
                                isCheckAll = false;
                            }
                            else
                            {
                                gridColumnCheck.Image = imageListIcon.Images[6];
                                foreach (var item in this._MediStocks)
                                {
                                    item.IsCheck = false;
                                    item.TYPE = 999;
                                }
                                isCheckAll = true;
                            }
                            gridViewMediStock.EndUpdate();
                            chkHienThiLoHet.Enabled = this._MediStocks.Where(p => p.IsCheck == true).ToList().Count == 1;
                            if (!chkHienThiLoHet.Enabled)
                                chkHienThiLoHet.Checked = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private void chkAlertMinStock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkAlertMinStock.Enabled)
                {
                    return;
                }
                var date = Inventec.Common.TypeConvert.Parse.ToDateTime(dtExpiredDate.EditValue.ToString());
                var lastDayOfMonthTo = DateTime.DaysInMonth(date.Year, date.Month);
                var dat2 = Inventec.Common.TypeConvert.Parse.ToDateTime(lastDayOfMonthTo + "/" + date.Month + "/" + date.Year);
                var ExpiredDateTo = Inventec.Common.TypeConvert.Parse.ToInt64(
                          Convert.ToDateTime(dat2).ToString("yyyyMMdd") + "000000");
                WaitingManager.Show();

                List<HisMedicineInStockSDO> lstMediInStocksTemp;
                List<HisMaterialInStockSDO> lstMateInStocksTemp;

                if (chkMedicine.Checked)
                {
                    if (chkAlertMinStock.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode &&
                                (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                            )
                           .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                        {
                            if (ChkExpiredDate.Checked)
                            {
                                lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                            }
                            else if (ChkNoExpiredDate.Checked)
                            {
                                lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                            }
                        }
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (ChkExpiredDate.Checked)
                            ChkExpiredDate_CheckedChanged(null, null);
                        else if (ChkNoExpiredDate.Checked)
                            ChkNoExpiredDate_CheckedChanged(null, null);
                        else
                        {
                            lstMediInStocksTemp = lstMediInStocks.Where(o => o.ID > 0).ToList();
                            hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                        }
                    }

                }
                else if (chkMaterial.Checked)
                {
                    if (chkAlertMinStock.Checked)
                    {
                        var lstMadiInStocksNodeIds = lstMateInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode &&
                                (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                            ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMadiInStocksNodeIds.Contains(o.NodeId) || lstMadiInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                        {
                            if (ChkExpiredDate.Checked)
                            {
                                lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                            }
                            else if (ChkNoExpiredDate.Checked)
                            {
                                lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                            }
                        }
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (ChkExpiredDate.Checked)
                            chkAlertMinStock_CheckedChanged(null, null);
                        else if (ChkNoExpiredDate.Checked)
                            ChkNoExpiredDate_CheckedChanged(null, null);
                        else
                        {
                            lstMateInStocksTemp = lstMateInStocks.Where(o => o.ID > 0).ToList();
                            hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                        }
                    }

                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkExpiredDate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!ChkExpiredDate.Enabled)
                {
                    return;
                }
                if (ChkExpiredDate.Checked)
                {
                    ChkNoExpiredDate.Checked = false;
                    ChkExpiredDate.Checked = true;
                }
                var date = Inventec.Common.TypeConvert.Parse.ToDateTime(dtExpiredDate.EditValue.ToString());
                var lastDayOfMonthTo = DateTime.DaysInMonth(date.Year, date.Month);
                var dat2 = Inventec.Common.TypeConvert.Parse.ToDateTime(lastDayOfMonthTo + "/" + date.Month + "/" + date.Year);
                var ExpiredDateTo = Inventec.Common.TypeConvert.Parse.ToInt64(
                          Convert.ToDateTime(dat2).ToString("yyyyMMdd") + "000000");
                WaitingManager.Show();

                List<HisMedicineInStockSDO> lstMediInStocksTemp;
                List<HisMaterialInStockSDO> lstMateInStocksTemp;

                if (chkMedicine.Checked)
                {
                    if (ChkExpiredDate.Checked && chkAlertMinStock.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode &&
                                (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                            )
                           .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                            lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else if (ChkExpiredDate.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            )
                           .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                            lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        {
                            chkAlertMinStock_CheckedChanged(null, null);
                        }
                        else
                        {
                            lstMediInStocksTemp = lstMediInStocks.Where(o => o.ID > 0).ToList();
                            hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                        }
                    }

                }
                else if (chkMaterial.Checked)
                {
                    if (ChkExpiredDate.Checked && chkAlertMinStock.Checked)
                    {
                        var lstMadiInStocksNodeIds = lstMateInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode &&
                                (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                            ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMadiInStocksNodeIds.Contains(o.NodeId) || lstMadiInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                            lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else if (ChkExpiredDate.Checked)
                    {
                        var lstMadiInStocksNodeIds = lstMateInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMadiInStocksNodeIds.Contains(o.NodeId) || lstMadiInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                            lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE <= ExpiredDateTo || p.isTypeNode).ToList();
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        {
                            chkAlertMinStock_CheckedChanged(null, null);
                        }
                        else
                        {
                            lstMateInStocksTemp = lstMateInStocks.Where(o => o.ID > 0).ToList();
                            hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                        }

                    }


                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChkNoExpiredDate_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!ChkNoExpiredDate.Enabled)
                {
                    return;
                }
                if (ChkNoExpiredDate.Enabled)
                {
                    ChkExpiredDate.Checked = false;
                }
                WaitingManager.Show();

                List<HisMedicineInStockSDO> lstMediInStocksTemp;
                List<HisMaterialInStockSDO> lstMateInStocksTemp;

                if (chkMedicine.Checked)
                {
                    if (ChkNoExpiredDate.Checked && chkAlertMinStock.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                           Where(o =>
                               ((o.IS_LEAF ?? 0) == 1) &&
                               o.isTypeNode &&
                               (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                           )
                          .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                            lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else if (ChkNoExpiredDate.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            )
                           .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                            lstMediInStocksTemp = lstMediInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        {
                            chkAlertMinStock_CheckedChanged(null, null);
                        }
                        else
                        {
                            lstMediInStocksTemp = lstMediInStocks.Where(o => o.ID > 0).ToList();
                            hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                        }
                    }

                }
                else if (chkMaterial.Checked)
                {
                    if (ChkNoExpiredDate.Checked && chkAlertMinStock.Checked)
                    {
                        var lstMadiInStocksNodeIds = lstMateInStocks.
                           Where(o =>
                               ((o.IS_LEAF ?? 0) == 1) &&
                               o.isTypeNode &&
                               (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                           ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMadiInStocksNodeIds.Contains(o.NodeId) || lstMadiInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                            lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else if (ChkNoExpiredDate.Checked)
                    {
                        var lstMadiInStocksNodeIds = lstMateInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMadiInStocksNodeIds.Contains(o.NodeId) || lstMadiInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                            lstMateInStocksTemp = lstMateInStocksTemp.Where(p => p.EXPIRED_DATE == null || p.isTypeNode).ToList();
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        {
                            chkAlertMinStock_CheckedChanged(null, null);
                        }
                        else
                        {
                            lstMateInStocksTemp = lstMateInStocks.Where(o => o.ID > 0).ToList();
                            hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                        }
                    }

                }
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtExpiredDate_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (ChkExpiredDate.Enabled)
                    {
                        ChkExpiredDate_CheckedChanged(null, null);
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPatientType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboPatientType.EditValue = null;
                    IdComboPatientType = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                //không kiểm tra dữ liệu cbo xóa hay đổi sang dữ liệu khác cũng load lại danh sách
                if ((cbo.EditValue != null && Int64.Parse(cbo.EditValue.ToString()) == IdComboPatientType) || (cbo.EditValue == null && IdComboPatientType == 0))
                    return;
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChkValidToTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!ChkValidToTime.Enabled)
                {
                    return;
                }

                var date = Inventec.Common.TypeConvert.Parse.ToDateTime(dtValidToTime.EditValue.ToString());
                //var dat2 = Inventec.Common.TypeConvert.Parse.ToDateTime(date.Day + "/" + date.Month + "/" + date.Year);
                var ValidToTime = Inventec.Common.TypeConvert.Parse.ToInt64(
                          dtValidToTime.DateTime.ToString("yyyyMMdd") + "235959");

                WaitingManager.Show();

                List<HisMedicineInStockSDO> lstMediInStocksTemp;
                List<HisMaterialInStockSDO> lstMateInStocksTemp;
                List<HIS_BID> ListBidName;

                if (ChkValidToTime.Checked)
                {
                    ListBidName = BackendDataWorker.Get<HIS_BID>().Where(o => o.VALID_TO_TIME <= ValidToTime).ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListBidName), ListBidName));
                }
                else
                {
                    ListBidName = BackendDataWorker.Get<HIS_BID>();
                }

                if (chkMedicine.Checked)
                {
                    //    if (ChkValidToTime.Checked && chkAlertMinStock.Checked)
                    //    {
                    //        var lstMediInStocksNodes = lstMediInStocks.
                    //            Where(o =>
                    //                ((o.IS_LEAF ?? 0) == 1) &&
                    //                o.isTypeNode &&
                    //                (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                    //            )
                    //           .ToList();
                    //        var lstMediInStocksNodeIds = lstMediInStocksNodes
                    //            .Select(o => o.NodeId).ToList();
                    //        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();

                    //        lstMediInStocksTemp = lstMediInStocksTemp.Where(p => ListBidName.Select(o => o.BID_NUMBER).Distinct().ToList().Contains(p.BID_NUMBER) || p.isTypeNode).ToList();

                    //        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    //    }
                    //    else
                    //{
                    if (ChkValidToTime.Checked)
                    {
                        var lstMediInStocksNodes = lstMediInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            )
                           .ToList();
                        var lstMediInStocksNodeIds = lstMediInStocksNodes
                            .Select(o => o.NodeId).ToList();
                        lstMediInStocksTemp = lstMediInStocks.Where(o => lstMediInStocksNodeIds.Contains(o.NodeId) || lstMediInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMediInStocksTemp != null && lstMediInStocksTemp.Count > 0)
                            lstMediInStocksTemp = lstMediInStocksTemp.Where(p => ListBidName.Select(o => o.BID_NUMBER).Distinct().ToList().Contains(p.BID_NUMBER) || p.isTypeNode).ToList();


                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        //if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        //{
                        //    chkAlertMinStock_CheckedChanged(null, null);
                        //}
                        //else
                        //{
                        lstMediInStocksTemp = lstMediInStocks.Where(o => o.ID > 0).ToList();
                        hisMediInStockProcessor.Reload(ucMedicineInfo, lstMediInStocksTemp, this.mediStockIds);
                        //}
                    }
                    // }
                }
                else if (chkMaterial.Checked)
                {

                    //if (ChkValidToTime.Checked && chkAlertMinStock.Checked)
                    //{
                    //    var lstMateInStocksNodeIds = lstMateInStocks.
                    //        Where(o =>
                    //            ((o.IS_LEAF ?? 0) == 1) &&
                    //            o.isTypeNode &&
                    //            (o.ALERT_MIN_IN_STOCK.HasValue && o.TotalAmount.HasValue && o.AvailableAmount.HasValue && o.ALERT_MIN_IN_STOCK >= o.AvailableAmount)
                    //        ).Select(o => o.NodeId).ToList();
                    //    lstMateInStocksTemp = lstMateInStocks.Where(o => lstMateInStocksNodeIds.Contains(o.NodeId) || lstMateInStocksNodeIds.Contains(o.ParentNodeId)).ToList();

                    //    lstMateInStocksTemp = lstMateInStocksTemp.Where(p => ListBidName.Select(o => o.BID_NUMBER).Distinct().ToList().Contains(p.BID_NUMBER) || p.isTypeNode).ToList();

                    //    hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    //}
                    //else
                    if (ChkValidToTime.Checked)
                    {
                        var lstMateInStocksNodeIds = lstMateInStocks.
                            Where(o =>
                                ((o.IS_LEAF ?? 0) == 1) &&
                                o.isTypeNode
                            ).Select(o => o.NodeId).ToList();
                        lstMateInStocksTemp = lstMateInStocks.Where(o => lstMateInStocksNodeIds.Contains(o.NodeId) || lstMateInStocksNodeIds.Contains(o.ParentNodeId)).ToList();
                        if (lstMateInStocksTemp != null && lstMateInStocksTemp.Count > 0)
                            lstMateInStocksTemp = lstMateInStocksTemp.Where(p => ListBidName.Select(o => o.BID_NUMBER).Distinct().ToList().Contains(p.BID_NUMBER) || p.isTypeNode).ToList();

                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                    }
                    else
                    {
                        //if (chkAlertMinStock.Checked && chkAlertMinStock.Enabled)
                        //{
                        //    chkAlertMinStock_CheckedChanged(null, null);
                        //}
                        //else
                        //{
                        lstMateInStocksTemp = lstMateInStocks.Where(o => o.ID > 0).ToList();
                        hisMateInStockProcessor.Reload(ucMaterialInfo, lstMateInStocksTemp, this.mediStockIds);
                        //}
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIsActive_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                GridLookUpEdit cbo = sender as GridLookUpEdit;
                //không kiểm tra dữ liệu cbo xóa hay đổi sang dữ liệu khác cũng load lại danh sách
                if (cbo.EditValue.ToString() == IdComboStt)
                    return;
                ShowUCControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboIsActive_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboIsActive.EditValue = "01";
                    IdComboStt = "01";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }




        private class FlFuncElementFunction : FlexCel.Report.TFlexCelUserFunction
        {

            object result = null;
            public override object Evaluate(object[] parameters)
            {
                if (parameters == null || parameters.Length < 2)
                    throw new ArgumentException("Bad parameter count in call to Orders() user-defined function");


                try
                {
                    //string KeyGet = Convert.ToString(parameters[1]);
                    string KeyGet = "";
                    if (!String.IsNullOrEmpty(parameters[1].ToString()))
                    {
                        KeyGet = parameters[1].ToString().Replace("\"", string.Empty).Trim();
                    }

                    if (parameters[0] is Dictionary<string, int>)
                    {
                        Dictionary<string, int> DicGet = parameters[0] as Dictionary<string, int>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;//
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, long>)
                    {
                        Dictionary<string, long> DicGet = parameters[0] as Dictionary<string, long>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, decimal>)
                    {
                        Dictionary<string, decimal> DicGet = parameters[0] as Dictionary<string, decimal>;
                        if (String.IsNullOrEmpty(KeyGet)) return DicGet.Values.Sum();
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else if (parameters[0] is Dictionary<string, string>)
                    {
                        Dictionary<string, string> DicGet = parameters[0] as Dictionary<string, string>;
                        if (String.IsNullOrEmpty(KeyGet)) return null;
                        if (!DicGet.ContainsKey(KeyGet))
                        {
                            return null;
                        }
                        result = DicGet[KeyGet];
                    }
                    else
                    {
                        result = null;
                    }
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    return null;
                }

                return result;
            }
        }

        private void cboIsActive_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboIsActive.EditValue != null)
                    IdComboStt = cboIsActive.EditValue.ToString();
                else
                    IdComboStt = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPatientType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (cboPatientType.EditValue != null)
                    IdComboPatientType = Int64.Parse(cboPatientType.EditValue.ToString());
                else
                    IdComboPatientType = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkHienThiLoHet_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkHienThiLoHet.Checked)
                    chkShowLineZero.Checked = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkShowLineZero_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chkShowLineZero.Checked)
                    chkHienThiLoHet.Checked = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
