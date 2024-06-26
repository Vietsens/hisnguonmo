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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.ADO;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.Processor;
using HIS.Desktop.Plugins.BaseCompensationBillCreate.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
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

namespace HIS.Desktop.Plugins.BaseCompensationBillCreate
{
    public partial class frmBaseCompensationBillCreate : HIS.Desktop.Utility.FormBase
    {
        MOS.EFMODEL.DataModels.V_HIS_AGGR_EXP_MEST HisAggrExpMestRow;
        private HisChmsExpMestResultSDO chmsExpMestResult;
        internal HisChmsExpMestSDO HisChmsExpMestSDO = new HisChmsExpMestSDO();
        int positionHandle = -1;
        int positionHandleControl = -1;
        internal int ActionType = 0, ActionBosung = 0;
        private double idRow = -1;
        private int focusedRowHandleMedicine = -1;
        private int focusedRowHandleMaterial = -1;
        internal MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE VHisMedicineType { get; set; }
        internal HisMedicineTypeInStockSDO HisMedicineTypeInStockSDO { get; set; }
        internal MssMedicineTypeSDO MedicineTypeModel { get; set; }
        internal List<MssMedicineTypeSDO> ListVHisMedicineTypeProcess { get; set; }
        private List<MssExpMestMediMateForPrintSDO> listDataExpMestMediMatyForPrint;

        internal MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE VHisMaterialType { get; set; }
        internal HisMaterialTypeInStockSDO HisMaterialTypeInStockSDO { get; set; }
        internal MssMaterialTypeSDO MaterialTypeModel { get; set; }
        private decimal totalPrice = 0;
        private HisChmsExpMestResultSDO dataResult;
        List<MOS.SDO.HisMaterialTypeInStockSDO> materialTypeInStockSDOInAggr = null;
        List<MOS.SDO.HisMedicineTypeInStockSDO> medicineTypeInStockSDOInAggr = null;

        List<MOS.SDO.HisMedicineTypeInStockSDO> currentMedicineTypeInStockSDOs;
        List<MOS.SDO.HisMaterialTypeInStockSDO> currentMaterialTypeInStockSDOs;

        List<long> expMestIdsIn { get; set; }

        internal Inventec.Desktop.Common.Modules.Module currentModule;
        internal DelegateSelectData returnSuccess;

        public frmBaseCompensationBillCreate(Inventec.Desktop.Common.Modules.Module currentModule, List<long> expMesIds, long mediStockId, DelegateSelectData returnSuccess)
		:base(currentModule)
        {
            InitializeComponent();
            this.currentModule = currentModule;
            this.returnSuccess = returnSuccess;
            this.expMestIdsIn = expMesIds;
            this.HisAggrExpMestRow = new V_HIS_AGGR_EXP_MEST();
            this.HisAggrExpMestRow.MEDI_STOCK_ID = mediStockId;
        }

        public void LoadkeyFromLanguage()
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmBaseCompensationBillCreate_Load(object sender, EventArgs e)
        {
            try
            {
                SetIconFrm();
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                LoadkeyFromLanguage();

                this.ListVHisMedicineTypeProcess = LoadDetailDataByExpMestIds(this.expMestIdsIn);

                ValidControls();

                ResetDataInForm();

                LoadDataToComboExpMedistock();

                LoadDataToComboImpMedistock();

                LoadDataToGridMedicineTypeInStock();

                LoadDataToGridMaterialTypeInStock();

                LoadDetailDataByAggrExpMest();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void SetIconFrm()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandle = -1;
                if (!dxValidationProviderBoXung.Validate())
                    return;
                lblMediStockExport.Enabled = false;
                lblcboMediStockExport.Enabled = false;
                if (this.ActionBosung == GlobalVariables.ActionView) return;

                if (xtraTabControlExpMest.SelectedTabPageIndex == 0)
                {
                    AddMedicineClick();
                }
                else if (xtraTabControlExpMest.SelectedTabPageIndex == 1)
                {
                    AddMaterialClick();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadDataToGridMedicineTypeInStock();
                LoadDataToGridMaterialTypeInStock();
                ResetControlAddPanel();

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
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                this.positionHandleControl = -1;
                if (!dxValidationProviderControl.Validate())
                    return;
                if (!CheckValidAmountGridMedicine())
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Số lượng trong danh sách thuốc/ vật tư chuẩn bị xuất không hợp lệ, vui lòng kiểm tra lại.", "Thông báo");
                    gridControlDetailMedicineProcess.Focus();
                    return;
                }
                if (this.ListVHisMedicineTypeProcess == null || this.ListVHisMedicineTypeProcess.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Thiếu trường dữ liệu bắt buộc, vui lòng kiểm tra lại.", "Thông báo");
                    Inventec.Common.Logging.LogSystem.Debug("Thieu truong du lieu bat buoc, danh sach thuoc/vat tu can xuat khong co.");
                    return;
                }
                WaitingManager.Show();

                //this.HisExpMestType = LOGIC.Config.HisExpMestTypeCFG.EXP_MEST_TYPE_ID__CHMS;
                UpdateExpRequestDataFormToDTO();
                if (this.ActionType == GlobalVariables.ActionAdd)
                {
                    dataResult = new BackendAdapter(param).Post<HisChmsExpMestResultSDO>("/api/HisChmsExpMest/Create", ApiConsumers.MosConsumer, HisChmsExpMestSDO, param);
                    if (dataResult != null)
                    {
                        success = true;

                        gridControlDetailMedicineProcess.DataSource = null;

                        //Refesh lai du lieu medicine - material sau khi luu thanh cong
                        RefeshDataAfterSaveToDbMedicine();
                        RefeshDataAfterSaveToDbMaterial();

                        this.ActionType = GlobalVariables.ActionView;
                        this.ActionBosung = GlobalVariables.ActionView;

                        //Enable - disable cac button
                        SetEnableButtonControl(this.ActionType);

                        //Hien thi tong tien voi truong hop xuat ban
                        FillDataToAllSumMoneyLabel(dataResult);

                        this.returnSuccess(dataResult.ExpMest.ID);
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

        private void FillDataToAllSumMoneyLabel(HisChmsExpMestResultSDO dataResult)
        {
            try
            {
                decimal sumAllMoney = 0;
                if (dataResult.ExpMedicines != null && dataResult.ExpMedicines.Count > 0)
                {
                    foreach (var item in dataResult.ExpMedicines)
                    {
                        sumAllMoney += item.AMOUNT * (item.IMP_PRICE) * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
                    }
                }
                if (dataResult.ExpMaterials != null && dataResult.ExpMaterials.Count > 0)
                {
                    foreach (var item in dataResult.ExpMaterials)
                    {
                        sumAllMoney += item.AMOUNT * (item.IMP_PRICE) * (1 + (item.VAT_RATIO ?? 0)) - (item.DISCOUNT ?? 0);
                    }
                }

                txtTotalPriceAllGridDetail.Text = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(sumAllMoney);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void SetEnableButtonControl(int action)
        {
            try
            {
                btnAdd.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnRefresh.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                cboPrint.Enabled = (action == GlobalVariables.ActionView || action == GlobalVariables.ActionViewForEdit);
                btnCreateNew.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionView || action == GlobalVariables.ActionViewForEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void UpdateExpRequestDataFormToDTO()
        {
            try
            {
                //Xuat chuyen kho
                HisChmsExpMestSDO = new HisChmsExpMestSDO();
                HisChmsExpMestSDO.ChmsExpMest = new HIS_CHMS_EXP_MEST();
                HisChmsExpMestSDO.ExpMedicines = new List<HisMedicineTypeAmountSDO>();
                HisChmsExpMestSDO.ExpMaterials = new List<HisMaterialTypeAmountSDO>();
                HisChmsExpMestSDO.ExpMest = new HIS_EXP_MEST();
                UpdateDataFormToDTOHisChmsExpMest(HisChmsExpMestSDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreateNew_Click(object sender, EventArgs e)
        {
            try
            {
                ResetDataInForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlMedicineType_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                    if (
                        (
                        e.Modifiers == Keys.None
                        && view.IsLastRow
                        && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1
                        )
                        || (e.Modifiers == Keys.Shift
                        && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0)
                        )
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        grid.SelectNextControl(spinAmount, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMedicineType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                MedicineTypeKeyDown(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewMedicineType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                MedicineTypeRowCellClick(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlMaterialType_ProcessGridKey(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Tab)
                {
                    GridControl grid = sender as GridControl;
                    GridView view = grid.FocusedView as GridView;
                    if ((e.Modifiers == Keys.None && view.IsLastRow && view.FocusedColumn.VisibleIndex == view.VisibleColumns.Count - 1) || (e.Modifiers == Keys.Shift && view.IsFirstRow && view.FocusedColumn.VisibleIndex == 0))
                    {
                        if (view.IsEditing)
                            view.CloseEditor();
                        grid.SelectNextControl(spinAmount, e.Modifiers == Keys.None, false, false, true);
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                MaterialTypeKeyDown(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewMaterialType_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                MaterialTypeRowCellClick(sender, e);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetailMedicineProcess_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                var dataRow = (MssMedicineTypeSDO)gridViewDetailMedicineProcess.GetFocusedRow();
                if (dataRow != null)
                {
                    var expMestDTO = this.HisChmsExpMestSDO.ExpMedicines.FirstOrDefault(o => o.MedicineTypeId == dataRow.ID);
                    if (expMestDTO != null)
                    {
                        expMestDTO.Amount = dataRow.AMOUNT;

                        if (dataRow.IS_MEDICINE)
                        {
                            if (medicineTypeInStockSDOInAggr != null && medicineTypeInStockSDOInAggr.Count > 0)
                            {
                                var medicineInStock = medicineTypeInStockSDOInAggr.FirstOrDefault(o => o.ServiceId == dataRow.SERVICE_ID);
                                if (medicineInStock != null && medicineInStock.AvailableAmount < dataRow.AMOUNT)
                                {
                                    dataRow.ErrorMessage = ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;// "Số lượng xuất lớn hơn số lượng khả dụng trong kho"; //MessageUtil.GetMessage(LibraryMessage.Message.Enum.ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                                    dataRow.ErrorTypeAmount = ErrorType.Warning;
                                    //medicineInStock.AvailableAmount ?? 0;
                                }
                                else
                                {
                                    dataRow.ErrorMessage = "";
                                    dataRow.ErrorTypeAmount = ErrorType.None;
                                }
                            }
                        }
                        else
                        {
                            if (currentMedicineTypeInStockSDOs != null && currentMedicineTypeInStockSDOs.Count > 0)
                            {
                                var materialInStock = currentMedicineTypeInStockSDOs.FirstOrDefault(o => o.ServiceId == dataRow.SERVICE_ID);
                                if (materialInStock != null && materialInStock.AvailableAmount < dataRow.AMOUNT)
                                {
                                    dataRow.ErrorMessage = ResourceMessageLang.SoLuongXuatLonHonSoLuongKhaDungTrongKho;//"Số lượng xuất lớn hơn số lượng khả dụng trong kho";// MessageUtil.GetMessage(LibraryMessage.Message.Enum.ChiDinhThuoc_SoLuongXuatLonHonSpoLuongKhadungTrongKho);
                                    dataRow.ErrorTypeAmount = ErrorType.Warning;
                                    //model.AMOUNT = materialInStock.AvailableAmount ?? 0;
                                }
                                else
                                {
                                    dataRow.ErrorMessage = "";
                                    dataRow.ErrorTypeAmount = ErrorType.None;
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

        private void gridViewDetailMedicineProcess_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    if (e.Column.FieldName == "AMOUNT")
                    {
                        //string decimalSeparator = LanguageManager.GetCulture().NumberFormat.CurrencyDecimalSeparator;
                        //repositoryItemSpinEditAmount.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                        //repositoryItemSpinEditAmount.Properties.Mask.EditMask = @"\d+(\" + decimalSeparator + @"\d{0,2})?";
                        e.RepositoryItem = repositoryItemSpinEditAmount;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetailMedicineProcess_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
            {
                MssMedicineTypeSDO data = (MssMedicineTypeSDO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (data != null)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "REMOVE_SELECED_ROW")
                    {
                        e.Value = imageList1.Images[0];
                    }
                    else if (e.Column.FieldName == "VAT_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString((data.VAT ?? 0) * 100, 2);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong VAT_DISPLAY", ex);
                        }
                    }
                    else if (e.Column.FieldName == "DISCOUNT_RATIO_DISPLAY")
                    {
                        try
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString((data.DISCOUNT_RATIO ?? 0), 2);
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong DISCOUNT_RATIO", ex);
                        }
                    }
                }
            }
        }

        private void gridViewDetailMedicineProcess_CustomRowError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                var index = gridViewDetailMedicineProcess.GetDataSourceRowIndex(e.RowHandle);
                if (index < 0)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                var listFunds = gridControlDetailMedicineProcess.DataSource as List<MssMedicineTypeSDO>;
                var row = listFunds[index];
                if (e.ColumnName == "AMOUNT")
                {
                    if (row.ErrorTypeAmount == ErrorType.Warning)
                    {
                        e.Info.ErrorType = (ErrorType)(row.ErrorTypeAmount);
                        e.Info.ErrorText = (string)(row.ErrorMessage);
                    }
                    else
                    {
                        e.Info.ErrorType = (ErrorType)(ErrorType.None);
                        e.Info.ErrorText = "";
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    e.Info.ErrorType = (ErrorType)(ErrorType.None);
                    e.Info.ErrorText = "";
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetailMedicineProcess_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                if (e.ColumnName == "AMOUNT")
                {
                    gridViewDetailMedicineProcess_CustomRowError(sender, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewDetailMedicineExpView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data = (MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.EXPIRED_DATE ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong EXPIRED_DATE", ex);
                            }
                        }
                        else if (e.Column.FieldName == "TOTAL_PRICE")
                        {
                            try
                            {

                                decimal valueTotal = ((data.PRICE ?? 0) * (data.AMOUNT) * (1 + (data.VAT_RATIO ?? 0)) - (data.DISCOUNT ?? 0));
                                e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(valueTotal);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho truong TOTAL_PRICE", ex);
                            }

                        }
                        else if (e.Column.FieldName == "VAT_RATIO_DISPLAY")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound((data.VAT_RATIO ?? 0) * 100);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockImport_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboMediStockImport.EditValue != null)
                    {
                        var mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o =>
                                o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockImport.EditValue ?? 0).ToString())
                                && o.ROOM_TYPE_ID == HisRoomTypeCFG.HisRoomTypeId__Stock);
                        if (mediStockImp != null)
                        {
                            txtMediStockImportCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        }
                    }
                    txtDescription.SelectAll();
                    txtDescription.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboMediStockImport_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (cboMediStockImport.EditValue != null)
                {
                    var mediStockImp = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboMediStockImport.EditValue ?? "0").ToString())
                            && o.ROOM_TYPE_ID == HisRoomTypeCFG.HisRoomTypeId__Stock);
                    if (mediStockImp != null)
                    {
                        txtMediStockImportCode.Text = mediStockImp.MEDI_STOCK_CODE;
                        txtDescription.SelectAll();
                        txtDescription.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtMediStockImportCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (String.IsNullOrEmpty(txtMediStockImportCode.Text.Trim()))
                    {
                        cboMediStockImport.EditValue = null;
                        cboMediStockImport.Focus();
                        cboMediStockImport.ShowPopup();
                        PopupLoader.SelectFirstRowPopup(cboMediStockImport);
                    }
                    else
                    {
                        var data = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => o.MEDI_STOCK_CODE == txtMediStockImportCode.Text.Trim()
                            && o.ROOM_TYPE_ID == HisRoomTypeCFG.HisRoomTypeId__Stock).ToList();
                        if (data != null)
                        {
                            if (data.Count == 1)
                            {
                                cboMediStockImport.EditValue = data[0].ROOM_ID;
                                txtMediStockImportCode.Text = data[0].MEDI_STOCK_CODE;
                                txtDescription.Focus();
                                txtDescription.SelectAll();
                            }
                            else
                            {
                                cboMediStockImport.EditValue = null;
                                cboMediStockImport.Focus();
                                cboMediStockImport.ShowPopup();
                                PopupLoader.SelectFirstRowPopup(cboMediStockImport);
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

        private void txtKeyword__Medicine_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMedicineType.BeginUpdate();
                    if (!String.IsNullOrEmpty(txtKeyword__Medicine.Text.Trim()))
                    {
                        medicineTypeInStockSDOInAggr = currentMedicineTypeInStockSDOs.Where(o =>
                                                        (o.MedicineTypeName.ToLower().Contains(txtKeyword__Medicine.Text.Trim().ToLower())
                                                        || o.MedicineTypeCode.ToLower().Contains(txtKeyword__Medicine.Text.Trim().ToLower()))
                                                        ).ToList();
                    }
                    else
                    {
                        medicineTypeInStockSDOInAggr = currentMedicineTypeInStockSDOs;
                    }

                    gridViewMedicineType.GridControl.DataSource = medicineTypeInStockSDOInAggr;
                    gridViewMedicineType.EndUpdate();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewMedicineType.Focus();
                    gridViewMedicineType.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword__Material_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    gridViewMaterialType.BeginUpdate();
                    if (!String.IsNullOrEmpty(txtKeyword__Medicine.Text.Trim()))
                    {
                        materialTypeInStockSDOInAggr = currentMaterialTypeInStockSDOs.Where(o =>
                                                        (o.MaterialTypeName.ToLower().Contains(txtKeyword__Material.Text.Trim().ToLower())
                                                        || o.MaterialTypeCode.ToLower().Contains(txtKeyword__Material.Text.Trim().ToLower()))
                                                        ).ToList();
                    }
                    else
                    {
                        materialTypeInStockSDOInAggr = currentMaterialTypeInStockSDOs;
                    }

                    gridViewMaterialType.GridControl.DataSource = materialTypeInStockSDOInAggr;
                    gridViewMaterialType.EndUpdate();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    gridViewMaterialType.Focus();
                    gridViewMaterialType.FocusedRowHandle = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_RemoveSelectedRow_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (MssMedicineTypeSDO)gridViewDetailMedicineProcess.GetFocusedRow();
                if (data != null)
                {
                    this.ListVHisMedicineTypeProcess = this.ListVHisMedicineTypeProcess.Where(p => p.SERVICE_ID != data.SERVICE_ID).ToList();
                }
                gridControlDetailMedicineProcess.BeginUpdate();
                gridControlDetailMedicineProcess.DataSource = this.ListVHisMedicineTypeProcess;
                gridControlDetailMedicineProcess.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem_Save_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barButtonItem_LamLai_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barButtonItem__Add_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void cboPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickInPhieuXuatChuyenKho(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
