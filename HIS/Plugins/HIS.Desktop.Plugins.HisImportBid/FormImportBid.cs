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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImportBid
{
    public partial class FormImportBid : HIS.Desktop.Utility.FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private System.Globalization.CultureInfo cultureLang;
        private List<ADO.ImportADO> ListDataImport;
        private List<ADO.ImportADO> LstDataImport;
        private bool checkClick;
        private string IsMedicine = "X";
        private int THUOC = 1;
        private int VATTU = 2;
        private int MAU = 3;

        private List<HIS_BID_TYPE> bidTypes;

        public FormImportBid()
        {
            InitializeComponent();
        }

        public FormImportBid(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;

                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                this.Text = module.text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormImportBid_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                LoadBidType();

                BtnSave.Enabled = false;
                BtnShowLineError.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBidType()
        {
            try
            {
                MOS.Filter.HisBidTypeFilter bidTypeFilter = new MOS.Filter.HisBidTypeFilter();
                bidTypes = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_BID_TYPE>>("/api/HisBidType/Get", ApiConsumer.ApiConsumers.MosConsumer, bidTypeFilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BtnDownload.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_DOWNLOAD");
                this.BtnImport.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_IMPORT");
                this.BtnSave.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SAVE");
                this.BtnShowLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__ERROR");
                this.GcBid_ActiveIngrBhytCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__ACTIVE_INGR_BHYT_CODE");
                this.GcBid_ActiveIngrBhytName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__ACTIVE_INGR_BHYT_NAME");
                this.GcBid_Amount.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__AMOUNT");
                this.GcBid_BidGroupCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_GROUP_CODE");
                this.GcBid_BidName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_NAME");
                this.GcBid_BidNumber.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_NUMBER");
                this.GcBid_BidNumber.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_EXTRA_CODE");
                this.GcBid_BidNumOrder.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_NUM_ORDER");
                this.GcBid_BidPackageCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_PACKAGE_CODE");
                this.GcBid_BidTypeCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_TYPE_CODE");
                this.GcBid_BidYear.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_YEAR");
                this.GcBid_Concentra.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__CONCENTRA");
                this.GcBid_ImpPrice.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__IMP_PRICE");
                this.GcBid_ImpVatRatio.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__IMP_VAT_RATIO");
                this.GcBid_ManufactureName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__MANUFACTURE_NAME");
                this.GcBid_MedicineTypeCode.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__MEDICINE_TYPE_CODE");
                this.GcBid_MedicineTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__MEDICINE_TYPE_NAME");
                this.GcBid_NationalName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__NATIONAL_NAME");
                this.GcBid_PackingTypeName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__PACKING_TYPE_NAME");
                this.GcBid_ServiceUnitName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__SERVICE_UNIT_NAME");
                this.GcBid_STT.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__STT");
                this.GcBid_SupplierName.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__SUPPLIER_NAME");
                this.RepositoryItemButtonDelete.Buttons[0].ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__RP_BTN_DELETE");
                this.RepositoryItemButtonError.Buttons[0].ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__RP_BTN_ERROR");

                this.gridColumn2.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__SMATERIAL_TYPE_MAP_CODE");
                this.gridColumn2.ToolTip = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__SMATERIAL_TYPE_MAP_CODE_TOOLTIP");
                this.gridColumn3.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__JOIN_BID_MATERIAL_TYPE_CODE");
                this.gridColumn4.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_MATERIAL_TYPE_CODE");
                this.gridColumn5.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BID_MATERIAL_TYPE_NAME");
                this.gridColumn6.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__HEIN_SERVICE_BHYT_NAME");

                this.gridColumn8.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__MEDICINE_REGISTER_NUMBER");
                this.gridColumn9.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__MONTH_LIFESPAN");
                this.gridColumn10.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__DAY_LIFESPAN");
                this.gridColumn11.Caption = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__HOUR_LIFESPAN");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BarButtonSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                BtnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_BID.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_BID";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|*.xlsx";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    WaitingManager.Show();
                    var import = new Inventec.Common.ExcelImport.Import();
                    if (import.ReadFileExcel(ofd.FileName))
                    {
                        var ImpMestListProcessor = import.Get<ADO.ImportADO>(0);
                        if (ImpMestListProcessor != null && ImpMestListProcessor.Count > 0)
                        {
                            this.ListDataImport = new List<ADO.ImportADO>();
                            var listMedicine = ImpMestListProcessor.Where(o => !String.IsNullOrWhiteSpace(o.IS_MEDICINE) && o.IS_MEDICINE.Trim().ToLower() == IsMedicine.ToLower()).ToList();
                            var listMaterial = ImpMestListProcessor.Where(o => String.IsNullOrWhiteSpace(o.IS_MEDICINE)&&o.IsNotNullRow==true).ToList();
                            addListMedicineTypeToProcessList(listMedicine);
                            addListMaterialTypeToProcessList(listMaterial);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListDataImport), ListDataImport));
                            if (this.ListDataImport != null && this.ListDataImport.Count > 0)
                            {
                                CheckValidData(ListDataImport);
                                SetDataSource(ListDataImport);

                                checkClick = false;
                                //BtnSave.Enabled = true;
                                BtnShowLineError.Enabled = true;
                            }

                            WaitingManager.Hide();
                        }
                        else
                        {
                            WaitingManager.Hide();
                            DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                        }
                    }
                    else
                    {
                        WaitingManager.Hide();
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceLanguageManager.HeThongTBKQXLYCCuaFrontendThatBai, Resources.ResourceLanguageManager.ThongBao, DevExpress.Utils.DefaultBoolean.True);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnShowLineError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnShowLineError.Enabled) return;

                checkClick = true;
                if (BtnShowLineError.Text == GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__ERROR"))
                {
                    BtnShowLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__OK");
                    var data = ListDataImport.Where(o => !string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(data);
                }
                else
                {
                    BtnShowLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__ERROR");
                    this.LstDataImport = new List<ADO.ImportADO>();
                    LstDataImport = ListDataImport.Where(o => string.IsNullOrEmpty(o.ERROR)).ToList();
                    SetDataSource(LstDataImport);
                    BtnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnSave.Enabled) return;
                BtnSave.Focus();
                GridViewBid.PostEditor();
                CommonParam paramCommon = new CommonParam();
                bool success = false;

                if (CheckValidDataForSave(ref paramCommon, LstDataImport))
                {
                    List<HIS_BID> ListBid = ProcessBidForSave();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ListBid), ListBid));
                    if (ListBid != null && ListBid.Count > 0)
                    {
                        List<string> bidError = new List<string>();
                        foreach (var bid in ListBid)
                        {
                            var apiResult = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Post<HIS_BID>("api/HisBid/Create", ApiConsumer.ApiConsumers.MosConsumer, bid, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (apiResult != null)
                            {
                                success = true;
                                BtnSave.Enabled = false;
                            }
                            else
                            {
                                bidError.Add(bid.BID_NUMBER);
                            }
                        }

                        if (bidError.Count > 0)
                        {
                            if (paramCommon.Messages == null) paramCommon.Messages = new List<string>();
                            paramCommon.Messages.Add("Các gói thầu không tạo được: " + string.Join(",", bidError));
                        }
                    }
                }

                if (paramCommon.Messages != null && paramCommon.Messages.Count > 0)
                {
                    paramCommon.Messages = paramCommon.Messages.Distinct().ToList();
                }
                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, paramCommon, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RepositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ImportADO)GridViewBid.GetFocusedRow();
                if (row != null)
                {
                    if (ListDataImport != null && ListDataImport.Count > 0)
                    {
                        ListDataImport.Remove(row);

                        SetDataSource(ListDataImport);

                        if (checkClick)
                        {
                            if (BtnShowLineError.Text == GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__ERROR"))
                            {
                                BtnShowLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__OK");
                            }
                            else
                            {
                                BtnShowLineError.Text = GetLanguageControl("HIS_DESKTOP_PLUGINS_HIS_IMPORT_BID__BTN_SHOW_LINE_ERROR__ERROR");
                            }
                            BtnShowLineError_Click(null, null);
                        }
                        //btnShowLineError_Click(null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RepositoryItemButtonError_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (ADO.ImportADO)GridViewBid.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.ERROR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewBid_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridViewBid_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string ERROR = (GridViewBid.GetRowCellValue(e.RowHandle, "ERROR") ?? "").ToString().Trim();
                    if (e.Column.FieldName == "ErrorLine")
                    {
                        if (!string.IsNullOrEmpty(ERROR))
                        {
                            e.RepositoryItem = RepositoryItemButtonError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataSource(List<ADO.ImportADO> dataSource)
        {
            try
            {
                GridControlBid.BeginUpdate();
                GridControlBid.DataSource = null;
                GridControlBid.DataSource = dataSource;
                GridControlBid.EndUpdate();
                CheckErrorLine();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckErrorLine()
        {
            try
            {
                if (ListDataImport != null && ListDataImport.Count > 0)
                {
                    var checkError = ListDataImport.Exists(o => !string.IsNullOrEmpty(o.ERROR));

                    if (!checkError)
                    {
                        this.LstDataImport = new List<ADO.ImportADO>();
                        this.LstDataImport = ListDataImport;
                    }
                    BtnSave.Enabled = !checkError;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckValidData(List<ADO.ImportADO> MedicineCheckeds__Send)
        {
            try
            {
                if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
                {
                    foreach (var item in MedicineCheckeds__Send)
                    {
                        var messageErr = ProcessMessError(item, MedicineCheckeds__Send);

                        if (!String.IsNullOrWhiteSpace(messageErr))
                        {
                            if (!String.IsNullOrWhiteSpace(item.ERROR))
                                item.ERROR += messageErr;
                            else
                                item.ERROR = messageErr;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addListMedicineTypeToProcessList(List<ADO.ImportADO> medicineTypeImports)
        {
            try
            {
                if (medicineTypeImports == null || medicineTypeImports.Count == 0)
                    return;

                foreach (var medicineTypeImport in medicineTypeImports)
                {
                    var medicineTypeNotExist = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.MEDICINE_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);
                    var bloodTypeNotExist = BackendDataWorker.Get<HIS_BLOOD_TYPE>().FirstOrDefault(o => o.BLOOD_TYPE_CODE == medicineTypeImport.MEDICINE_TYPE_CODE);

                    var medicineType = new ADO.ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ImportADO>(medicineType, medicineTypeImport);

                    if (medicineTypeNotExist == null && bloodTypeNotExist == null)
                        medicineType.ERROR = "Mã thuốc không hợp lệ. ";

                    if (medicineTypeNotExist != null)
                    {
                        medicineType.Type = THUOC;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ImportADO>(medicineType, medicineTypeNotExist);
                    }
                    else if (bloodTypeNotExist != null)
                    {
                        medicineType.Type = MAU;
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ImportADO>(medicineType, bloodTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = bloodTypeNotExist.BLOOD_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = bloodTypeNotExist.BLOOD_TYPE_NAME;
                    }

                    if (medicineTypeImport.SUPPLIER_CODE != null)
                    {
                        var supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.SUPPLIER_CODE == medicineTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                    }

                    medicineType.IdRow = setIdRow(this.ListDataImport);

                    medicineType.BID_NUM_ORDER = medicineTypeImport.BID_NUM_ORDER;
                    medicineType.IMP_PRICE = medicineTypeImport.IMP_PRICE;
                    medicineType.AMOUNT = medicineTypeImport.AMOUNT;

                    if (medicineTypeImport.IMP_VAT_RATIO != null)
                    {
                        medicineType.ImpVatRatio = medicineTypeImport.IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO = medicineTypeImport.IMP_VAT_RATIO / 100;
                    }
                    else
                    {
                        medicineType.ImpVatRatio = 0;
                        medicineType.IMP_VAT_RATIO = 0;
                    }

                    medicineType.BID_PACKAGE_CODE = medicineTypeImport.BID_PACKAGE_CODE;
                    medicineType.BID_GROUP_CODE = medicineTypeImport.BID_GROUP_CODE;
                    medicineType.BID_NAME = medicineTypeImport.BID_NAME;
                    medicineType.BID_EXTRA_CODE = medicineTypeImport.BID_EXTRA_CODE;
                    medicineType.BID_NUMBER = medicineTypeImport.BID_NUMBER;
                    medicineType.BID_TYPE_CODE = medicineTypeImport.BID_TYPE_CODE;
                    medicineType.BID_YEAR = medicineTypeImport.BID_YEAR;
                    medicineType.BID_APTHAU_CODE = medicineTypeImport.BID_APTHAU_CODE;
                    
                    if (!string.IsNullOrWhiteSpace(medicineTypeImport.VALID_FROM_TIME) &&
                        !DateTime.TryParseExact(medicineTypeImport.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ERROR = "Định dạng thời gian ko hợp lệ.";
                    }
                    else
                    {
                        medicineType.VALID_FROM_TIME = medicineTypeImport.VALID_FROM_TIME;
                    }

                    if (!string.IsNullOrWhiteSpace(medicineTypeImport.VALID_TO_TIME) &&
                        !DateTime.TryParseExact(medicineTypeImport.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ERROR = "Định dạng thời gian ko hợp lệ.";
                    }
                    else
                    {
                        medicineType.VALID_TO_TIME = medicineTypeImport.VALID_TO_TIME;
                    }

                    var bidType = bidTypes.FirstOrDefault(o => o.BID_TYPE_CODE == medicineTypeImport.BID_TYPE_CODE);
                    if (bidType != null)
                    {
                        medicineType.BID_TYPE_ID = bidType.ID;
                        medicineType.BID_TYPE_NAME = bidType.BID_TYPE_NAME;
                    }

                    medicineType.MATERIAL_TYPE_MAP_CODE = medicineTypeImport.MATERIAL_TYPE_MAP_CODE;
                    medicineType.JOIN_BID_MATERIAL_TYPE_CODE = medicineTypeImport.JOIN_BID_MATERIAL_TYPE_CODE;
                    medicineType.BID_MATERIAL_TYPE_CODE = medicineTypeImport.BID_MATERIAL_TYPE_CODE;
                    medicineType.BID_MATERIAL_TYPE_NAME = medicineTypeImport.BID_MATERIAL_TYPE_NAME;
                    medicineType.PACKING_TYPE_NAME = medicineTypeImport.PACKING_TYPE_NAME;
                    medicineType.HEIN_SERVICE_BHYT_NAME = medicineTypeImport.HEIN_SERVICE_BHYT_NAME;

                    medicineType.CONCENTRA = medicineTypeImport.CONCENTRA;
                    medicineType.REGISTER_NUMBER = medicineTypeImport.REGISTER_NUMBER;
                    medicineType.ACTIVE_INGR_BHYT_NAME = medicineTypeImport.ACTIVE_INGR_BHYT_NAME;

                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MEDICINE_USE_FORM_CODE))
                    {
                        var useForm = BackendDataWorker.Get<HIS_MEDICINE_USE_FORM>().FirstOrDefault(o => o.MEDICINE_USE_FORM_CODE == medicineTypeImport.MEDICINE_USE_FORM_CODE);
                        if (useForm != null)
                        {
                            medicineType.MEDICINE_USE_FORM_CODE = useForm.MEDICINE_USE_FORM_CODE;
                            medicineType.MEDICINE_USE_FORM_ID = useForm.ID;
                            medicineType.MEDICINE_USE_FORM_NAME = useForm.MEDICINE_USE_FORM_NAME;
                        }
                        else
                        {
                            medicineType.ERROR += "Mã đường dùng không hợp lệ. ";
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(medicineTypeImport.MANUFACTURER_CODE))
                    {
                        var hisManufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == medicineTypeImport.MANUFACTURER_CODE);
                        if (hisManufacturer != null)
                        {
                            medicineType.MANUFACTURER_ID = hisManufacturer.ID;
                            medicineType.MANUFACTURER_CODE = hisManufacturer.MANUFACTURER_CODE;
                            medicineType.MANUFACTURER_NAME = hisManufacturer.MANUFACTURER_NAME;
                        }
                        else
                        {
                            medicineType.ERROR += "Mã hãng sản xuất không hợp lệ. ";
                        }
                    }
                    

                    medicineType.NATIONAL_NAME = medicineTypeImport.NATIONAL_NAME;

                    medicineType.MONTH_LIFESPAN_STR = medicineTypeImport.MONTH_LIFESPAN_STR;
                    medicineType.DAY_LIFESPAN_STR = medicineTypeImport.DAY_LIFESPAN_STR;
                    medicineType.HOUR_LIFESPAN_STR = medicineTypeImport.HOUR_LIFESPAN_STR;

                    long month_lifespan = 0, day_lifespan = 0, hour_lifespan = 0;

                    if (long.TryParse(medicineTypeImport.MONTH_LIFESPAN_STR, out month_lifespan) && !String.IsNullOrEmpty(medicineTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN = month_lifespan;
                    }
                    else
                    {
                        medicineType.MONTH_LIFESPAN = null;
                    }

                    if (long.TryParse(medicineTypeImport.DAY_LIFESPAN_STR, out day_lifespan) && !String.IsNullOrEmpty(medicineTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN = day_lifespan;
                    }
                    else
                    {
                        medicineType.DAY_LIFESPAN = null;
                    }

                    if (long.TryParse(medicineTypeImport.HOUR_LIFESPAN_STR, out hour_lifespan) && !String.IsNullOrEmpty(medicineTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN = hour_lifespan;
                    }
                    else
                    {
                        medicineType.HOUR_LIFESPAN = null;
                    }

                    this.ListDataImport.Add(medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void addListMaterialTypeToProcessList(List<ADO.ImportADO> materialTypeImports)
        {
            try
            {
                if (materialTypeImports == null || materialTypeImports.Count == 0)
                    return;

                foreach (var materialTypeImport in materialTypeImports)
                {
                    var materialTypeNotExist = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.MATERIAL_TYPE_CODE == materialTypeImport.MEDICINE_TYPE_CODE);

                    var medicineType = new ADO.ImportADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ImportADO>(medicineType, materialTypeImport);
                    if (materialTypeNotExist == null)
                    {
                        if (string.IsNullOrEmpty(materialTypeImport.MEDICINE_TYPE_CODE) || (!string.IsNullOrEmpty(materialTypeImport.MATERIAL_TYPE_MAP_CODE) && !string.IsNullOrEmpty(materialTypeImport.MEDICINE_TYPE_CODE)))
                        {
                            medicineType.ERROR = "Mã vật tư không hợp lệ. ";
                        }
                    }
                    else
                    {
                        Inventec.Common.Mapper.DataObjectMapper.Map<ADO.ImportADO>(medicineType, materialTypeNotExist);
                        medicineType.MEDICINE_TYPE_CODE = materialTypeNotExist.MATERIAL_TYPE_CODE;
                        medicineType.MEDICINE_TYPE_NAME = materialTypeNotExist.MATERIAL_TYPE_NAME;
                    }

                    if (!string.IsNullOrEmpty(materialTypeImport.MATERIAL_TYPE_MAP_CODE))
                    {
                        var HisMaterialTypeMap = BackendDataWorker.Get<HIS_MATERIAL_TYPE_MAP>().FirstOrDefault(o => o.MATERIAL_TYPE_MAP_CODE == materialTypeImport.MATERIAL_TYPE_MAP_CODE);

                        if (HisMaterialTypeMap == null)
                        {
                            medicineType.ERROR += "Mã vật tư tương đương không hợp lệ. ";
                        }
                        else
                        {
                            medicineType.MATERIAL_TYPE_MAP_CODE = HisMaterialTypeMap.MATERIAL_TYPE_MAP_CODE;
                            if (string.IsNullOrEmpty(medicineType.MEDICINE_TYPE_NAME))
                            {
                                medicineType.MEDICINE_TYPE_NAME = HisMaterialTypeMap.MATERIAL_TYPE_MAP_NAME;
                            }
                        }
                    }

                    if (materialTypeImport.SUPPLIER_CODE != null)
                    {
                        var supplier = BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.SUPPLIER_CODE == materialTypeImport.SUPPLIER_CODE);
                        if (supplier != null)
                        {
                            medicineType.SUPPLIER_ID = supplier.ID;
                            medicineType.SUPPLIER_CODE = supplier.SUPPLIER_CODE;
                            medicineType.SUPPLIER_NAME = supplier.SUPPLIER_NAME;
                        }
                    }
                    medicineType.Type = VATTU;

                    medicineType.IdRow = setIdRow(this.ListDataImport);

                    medicineType.BID_NUM_ORDER = materialTypeImport.BID_NUM_ORDER;
                    medicineType.IMP_PRICE = materialTypeImport.IMP_PRICE;
                    medicineType.AMOUNT = materialTypeImport.AMOUNT;

                    if (materialTypeImport.IMP_VAT_RATIO != null)
                    {
                        medicineType.ImpVatRatio = materialTypeImport.IMP_VAT_RATIO;
                        medicineType.IMP_VAT_RATIO = materialTypeImport.IMP_VAT_RATIO / 100;
                    }
                    else
                    {
                        medicineType.ImpVatRatio = 0;
                        medicineType.IMP_VAT_RATIO = 0;
                    }

                    medicineType.BID_PACKAGE_CODE = materialTypeImport.BID_PACKAGE_CODE;
                    medicineType.BID_GROUP_CODE = materialTypeImport.BID_GROUP_CODE;
                    medicineType.BID_NAME = materialTypeImport.BID_NAME;
                    medicineType.BID_EXTRA_CODE = materialTypeImport.BID_EXTRA_CODE;
                    medicineType.BID_NUMBER = materialTypeImport.BID_NUMBER;
                    medicineType.BID_TYPE_CODE = materialTypeImport.BID_TYPE_CODE;
                    medicineType.BID_YEAR = materialTypeImport.BID_YEAR;
                    medicineType.BID_APTHAU_CODE = materialTypeImport.BID_APTHAU_CODE;

                    if (!string.IsNullOrWhiteSpace(materialTypeImport.VALID_TO_TIME) &&
                        !DateTime.TryParseExact(materialTypeImport.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ERROR = "Định dạng thời gian ko hợp lệ.";
                    }
                    else
                    {
                        medicineType.VALID_FROM_TIME = materialTypeImport.VALID_FROM_TIME;
                    }

                    if (!string.IsNullOrWhiteSpace(materialTypeImport.VALID_TO_TIME) &&
                        !DateTime.TryParseExact(materialTypeImport.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                    {
                        medicineType.ERROR = "Định dạng thời gian ko hợp lệ.";
                    }
                    else
                    {
                        medicineType.VALID_TO_TIME = materialTypeImport.VALID_TO_TIME;
                    }

                    if (DateTime.TryParseExact(medicineType.VALID_FROM_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateFromValue) &&
                        DateTime.TryParseExact(medicineType.VALID_TO_TIME, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateToValue))
                    {
                        long? fromValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateFromValue);
                        long? toValue = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dateToValue);

                        if (fromValue > toValue)
                        {
                            medicineType.ERROR = "Thời gian từ không được lớn hơn thời gian đến";
                        }
                    }     

                    var bidType = bidTypes.FirstOrDefault(o => o.BID_TYPE_CODE == materialTypeImport.BID_TYPE_CODE);
                    if (bidType != null)
                    {
                        medicineType.BID_TYPE_ID = bidType.ID;
                        medicineType.BID_TYPE_NAME = bidType.BID_TYPE_NAME;
                    }

                    medicineType.MATERIAL_TYPE_MAP_CODE = materialTypeImport.MATERIAL_TYPE_MAP_CODE;
                    medicineType.JOIN_BID_MATERIAL_TYPE_CODE = materialTypeImport.JOIN_BID_MATERIAL_TYPE_CODE;
                    medicineType.BID_MATERIAL_TYPE_CODE = materialTypeImport.BID_MATERIAL_TYPE_CODE;
                    medicineType.BID_MATERIAL_TYPE_NAME = materialTypeImport.BID_MATERIAL_TYPE_NAME;
                    medicineType.PACKING_TYPE_NAME = materialTypeImport.PACKING_TYPE_NAME;
                    medicineType.HEIN_SERVICE_BHYT_NAME = materialTypeImport.HEIN_SERVICE_BHYT_NAME;

                    medicineType.CONCENTRA = materialTypeImport.CONCENTRA;
                    medicineType.REGISTER_NUMBER = materialTypeImport.REGISTER_NUMBER;

                    if (!string.IsNullOrWhiteSpace(materialTypeImport.MANUFACTURER_CODE))
                    {
                        var hisManufacturer = BackendDataWorker.Get<HIS_MANUFACTURER>().FirstOrDefault(o => o.MANUFACTURER_CODE == materialTypeImport.MANUFACTURER_CODE);

                        if (hisManufacturer != null)
                        {
                            medicineType.MANUFACTURER_ID = hisManufacturer.ID;
                            medicineType.MANUFACTURER_CODE = hisManufacturer.MANUFACTURER_CODE;
                            medicineType.MANUFACTURER_NAME = hisManufacturer.MANUFACTURER_NAME;
                        }
                        else
                        {
                            medicineType.ERROR += "Mã hãng sản xuất không hợp lệ. ";
                        }
                    }
                    
                    medicineType.NATIONAL_NAME = materialTypeImport.NATIONAL_NAME;


                    medicineType.MONTH_LIFESPAN_STR = materialTypeImport.MONTH_LIFESPAN_STR;
                    medicineType.DAY_LIFESPAN_STR = materialTypeImport.DAY_LIFESPAN_STR;
                    medicineType.HOUR_LIFESPAN_STR = materialTypeImport.HOUR_LIFESPAN_STR;

                    long month_lifespan = 0, day_lifespan = 0, hour_lifespan = 0;

                    if (long.TryParse(materialTypeImport.MONTH_LIFESPAN_STR, out month_lifespan) && !String.IsNullOrEmpty(materialTypeImport.MONTH_LIFESPAN_STR))
                    {
                        medicineType.MONTH_LIFESPAN = month_lifespan;
                    }
                    else
                    {
                        medicineType.MONTH_LIFESPAN = null;
                    }

                    if (long.TryParse(materialTypeImport.DAY_LIFESPAN_STR, out day_lifespan) && !String.IsNullOrEmpty(materialTypeImport.DAY_LIFESPAN_STR))
                    {
                        medicineType.DAY_LIFESPAN = day_lifespan;
                    }
                    else
                    {
                        medicineType.DAY_LIFESPAN = null;
                    }

                    if (long.TryParse(materialTypeImport.HOUR_LIFESPAN_STR, out hour_lifespan) && !String.IsNullOrEmpty(materialTypeImport.HOUR_LIFESPAN_STR))
                    {
                        medicineType.HOUR_LIFESPAN = hour_lifespan;
                    }
                    else
                    {
                        medicineType.HOUR_LIFESPAN = null;
                    }

                    this.ListDataImport.Add(medicineType);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private double setIdRow(List<ADO.ImportADO> medicineTypes)
        {
            double currentIdRow = 0;
            try
            {
                if (medicineTypes != null && medicineTypes.Count > 0)
                {
                    var maxIdRow = medicineTypes.Max(o => o.IdRow);
                    currentIdRow = ++maxIdRow;
                }
                else
                {
                    currentIdRow = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return currentIdRow;
        }

        private bool CheckValidDataForSave(ref CommonParam param, List<ADO.ImportADO> MedicineCheckeds__Send)
        {
            bool valid = true;
            try
            {
                if (MedicineCheckeds__Send != null && MedicineCheckeds__Send.Count > 0)
                {
                    foreach (var item in MedicineCheckeds__Send)
                    {
                        string messageErr = "";
                        if (item.Type == THUOC)
                        {
                            messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoThuoc, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == VATTU)
                        {
                            messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoVatTu, item.MEDICINE_TYPE_NAME);
                        }
                        else if (item.Type == MAU)
                        {
                            messageErr = String.Format(Resources.ResourceLanguageManager.CanhBaoMau, item.MEDICINE_TYPE_NAME);
                        }

                        var Err = ProcessMessError(item, MedicineCheckeds__Send);

                        if (!String.IsNullOrWhiteSpace(Err))
                        {
                            param.Messages.Add(messageErr + Err + ";");
                        }
                    }
                }

                if (param.Messages.Count > 0)
                {
                    param.Messages = param.Messages.Distinct().ToList();
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                valid = false;
            }
            return valid;
        }

        private string ProcessMessError(ADO.ImportADO item, List<ADO.ImportADO> MedicineCheckeds__Send)
        {
            string result = null;
            try
            {
                List<string> messageErr = new List<string>();

                if (item.SUPPLIER_ID == null || item.SUPPLIER_ID <= 0)
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoNhaCungCap);

                if (!item.AMOUNT.HasValue)
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoSoLuong);
                else if (item.AMOUNT <= 0)
                    messageErr.Add(Resources.ResourceLanguageManager.SoLuongKhongDuocAm);

                if (!item.IMP_PRICE.HasValue)
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoGiaNhap);
                else if (item.IMP_PRICE < 0)
                    messageErr.Add(Resources.ResourceLanguageManager.GiaKhongDuocAm);

                if (!item.IMP_VAT_RATIO.HasValue)
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoVAT);
                else if (item.IMP_VAT_RATIO < 0)
                    messageErr.Add(Resources.ResourceLanguageManager.VATKhongDuocAm);

                if (String.IsNullOrWhiteSpace(item.BID_NUM_ORDER) && item.BID_NUM_ORDER.Length > 50)
                    messageErr.Add(Resources.ResourceLanguageManager.SttThauQuaDai);

                if (!String.IsNullOrWhiteSpace(item.BID_GROUP_CODE) && item.BID_GROUP_CODE.Length > 4)
                    messageErr.Add(Resources.ResourceLanguageManager.NhomThauQuaDai);

                if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type != VATTU && item.BID_PACKAGE_CODE.Length > 4)
                    messageErr.Add(Resources.ResourceLanguageManager.GoiThauQuaDai);

                if (!String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE) && item.Type == VATTU && item.BID_PACKAGE_CODE.Length > 4)
                    messageErr.Add("mã gói thầu dài hơn 4 ký tự ");

                if (!String.IsNullOrEmpty(item.BID_YEAR))
                {
                    bool valid = true;
                    if (item.BID_YEAR.Length > 4)
                    {
                        valid = false;
                    }

                    foreach (char c in item.BID_YEAR)
                    {
                        if (!Char.IsDigit(c))
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (!valid)
                        messageErr.Add(Resources.ResourceLanguageManager.SaiDinhDangNam);
                    else
                    {
                        int year = Convert.ToInt32(item.BID_YEAR);
                        if (year > DateTime.Now.Year)
                            messageErr.Add(Resources.ResourceLanguageManager.NamKhongDuocLonHonNamHienTai);
                    }
                }

                if (item.Type == VATTU)
                {
                    if (String.IsNullOrWhiteSpace(item.BID_YEAR))
                        messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinNam);

                    if (String.IsNullOrWhiteSpace(item.BID_PACKAGE_CODE))
                        messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinGoiThau);
                    else if (item.BID_PACKAGE_CODE.Length < 2)
                        messageErr.Add(Resources.ResourceLanguageManager.GoiThauKhongDuDoDai);

                    if (!String.IsNullOrEmpty(item.MEDICINE_TYPE_CODE) && !String.IsNullOrEmpty(item.MATERIAL_TYPE_MAP_CODE))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapMaVatTuKhiNhapMaVatTuTuongDuong);
                    }

                    if (!String.IsNullOrWhiteSpace(item.MATERIAL_TYPE_MAP_CODE) && item.MATERIAL_TYPE_MAP_CODE.Length > 25)
                        messageErr.Add(Resources.ResourceLanguageManager.MaVatTuTuongDuongQuaDai);

                    if (!string.IsNullOrEmpty(item.JOIN_BID_MATERIAL_TYPE_CODE) && item.JOIN_BID_MATERIAL_TYPE_CODE.Length > 50)
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.MaDuThauQuaDai);
                    }

                    if (!string.IsNullOrEmpty(item.BID_MATERIAL_TYPE_CODE) && item.BID_MATERIAL_TYPE_CODE.Length > 50)
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.MaTrungThauQuaDai);
                    }

                    if (!string.IsNullOrEmpty(item.BID_MATERIAL_TYPE_NAME) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.BID_MATERIAL_TYPE_NAME, 500))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.TenTrungThauQuaDai);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(item.MATERIAL_TYPE_MAP_CODE))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapMaVatTuTuongDuong);
                    }

                    if (!string.IsNullOrEmpty(item.JOIN_BID_MATERIAL_TYPE_CODE))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapMaDuThau);
                    }

                    if (!string.IsNullOrEmpty(item.BID_MATERIAL_TYPE_CODE))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapMaTrungThau);
                    }

                    if (!string.IsNullOrEmpty(item.BID_MATERIAL_TYPE_NAME))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapTenTrungThau);
                    }
                }

                if (item.Type == THUOC)
                {
                    if (!string.IsNullOrEmpty(item.PACKING_TYPE_NAME) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.PACKING_TYPE_NAME, 300))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.QuyCachDongGoiQuaDai);
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.HEIN_SERVICE_BHYT_NAME, 500))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.TenBHYTQuaDai);
                    }

                    if (!string.IsNullOrEmpty(item.REGISTER_NUMBER) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.REGISTER_NUMBER, 500))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.SoDangKyQuaDai);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.PACKING_TYPE_NAME))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapQuyCachDongGoi);
                    }

                    if (!string.IsNullOrEmpty(item.HEIN_SERVICE_BHYT_NAME))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapTenBHYT);
                    }

                    if (!string.IsNullOrEmpty(item.REGISTER_NUMBER))
                    {
                        messageErr.Add(Resources.ResourceLanguageManager.KhongDuocNhapSoDangKy);
                    }

                    if (!string.IsNullOrEmpty(item.ACTIVE_INGR_BHYT_NAME))
                    {
                        messageErr.Add("Không được nhập tên hoạt chất");
                    }
                }

                if (String.IsNullOrWhiteSpace(item.BID_NAME))
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinTenThau);

                if (String.IsNullOrWhiteSpace(item.BID_EXTRA_CODE))
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinMaQDThau);

                if (String.IsNullOrWhiteSpace(item.BID_NUMBER))
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinQDThau);

                if (!item.BID_TYPE_ID.HasValue)
                    messageErr.Add(Resources.ResourceLanguageManager.KhongCoThongTinLoaiThau);

                var listItem = MedicineCheckeds__Send.Where(o => o.MEDICINE_TYPE_CODE == item.MEDICINE_TYPE_CODE).ToList();
                if (listItem != null && listItem.Count > 1)
                {
                    foreach (var i in listItem)
                    {
                        if (i.BID_NUMBER == item.BID_NUMBER && i.SUPPLIER_ID == item.SUPPLIER_ID && i.IdRow != item.IdRow && i.BID_GROUP_CODE == item.BID_GROUP_CODE)
                        {
                            messageErr.Add(Resources.ResourceLanguageManager.BiTrung);
                            break;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.MONTH_LIFESPAN_STR) && item.MONTH_LIFESPAN == null)
                {
                    messageErr.Add(Resources.ResourceLanguageManager.ThangTuoiThoSaiDinhDang);
                }

                if (!string.IsNullOrEmpty(item.DAY_LIFESPAN_STR) && item.DAY_LIFESPAN == null)
                {
                    messageErr.Add(Resources.ResourceLanguageManager.NgayTuoiThoSaiDinhDang);
                }

                if (!string.IsNullOrEmpty(item.HOUR_LIFESPAN_STR) && item.HOUR_LIFESPAN == null)
                {
                    messageErr.Add(Resources.ResourceLanguageManager.GioTuoiThoSaiDinhDang);
                }

                if (!string.IsNullOrEmpty(item.CONCENTRA) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.CONCENTRA, 1000))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.NongDoHamLuongQuaDai);
                }

                if (!string.IsNullOrEmpty(item.MANUFACTURER_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MANUFACTURER_CODE, 6))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.HangsanxuatQuaDai);
                }

                if (!string.IsNullOrEmpty(item.NATIONAL_NAME) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.NATIONAL_NAME, 100))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.NuocsanxuatQuaDai);
                }

                if (item.MONTH_LIFESPAN != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.MONTH_LIFESPAN.ToString(), 19))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.ThangTuoiThoQuaDai);
                }

                if (item.DAY_LIFESPAN != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.DAY_LIFESPAN.ToString(), 19))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.NgayTuoiThoQuaDai);
                }

                if (item.HOUR_LIFESPAN != null && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.HOUR_LIFESPAN.ToString(), 19))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.GioTuoiThoQuaDai);
                }
                if (!string.IsNullOrEmpty(item.BID_EXTRA_CODE) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.BID_EXTRA_CODE, 50))
                {
                    messageErr.Add(Resources.ResourceLanguageManager.MaQDThauQuaDai);
                }
                if (!string.IsNullOrEmpty(item.BID_NUMBER) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(item.BID_NUMBER, 50))
                {
                    messageErr.Add("Thông tin quyết định thầu có độ dài quá 50");
                }
                int dem = 0;
                if (item.MONTH_LIFESPAN != null)
                {
                    dem++;
                }

                if (item.DAY_LIFESPAN != null)
                {
                    dem++;
                }

                if (item.HOUR_LIFESPAN != null)
                {
                    dem++;
                }

                if (dem > 1)
                {
                    messageErr.Add(Resources.ResourceLanguageManager.TuoiThoKhongHopLe);
                }

                if (messageErr.Count > 0)
                    result = String.Join(",", messageErr);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<HIS_BID> ProcessBidForSave()
        {
            List<HIS_BID> result = null;
            try
            {
                if (this.LstDataImport != null && this.LstDataImport.Count > 0)
                {
                    result = new List<HIS_BID>();

                    var groups = LstDataImport.GroupBy(o => new { o.BID_NAME, o.BID_EXTRA_CODE, o.BID_NUMBER, o.BID_TYPE_ID, o.BID_YEAR }).ToList();

                    foreach (var group in groups)
                    {
                        var bidModel = new MOS.EFMODEL.DataModels.HIS_BID();
                        bidModel.HIS_BID_BLOOD_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE>();
                        bidModel.HIS_BID_MATERIAL_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE>();
                        bidModel.HIS_BID_MEDICINE_TYPE = new List<MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE>();
                        bidModel.BID_NAME = group.First().BID_NAME;
                        bidModel.BID_NUMBER = group.First().BID_NUMBER;
                        bidModel.BID_EXTRA_CODE = group.First().BID_EXTRA_CODE;
                        bidModel.BID_TYPE_ID = group.First().BID_TYPE_ID;
                        bidModel.BID_YEAR = group.First().BID_YEAR;     

                        string ToTime = null;
                        foreach (var item in group)
                        {
                            ToTime = item.VALID_TO_TIME;

                            if (!string.IsNullOrEmpty(ToTime))
                            {
                                break;
                            }
                        }
                        string FromTime = null;
                        foreach (var item in group)
                        {
                            FromTime = item.VALID_FROM_TIME;

                            if (!string.IsNullOrEmpty(ToTime))
                            {
                                break;  
                            }
                        }
                        string ApThau = null;
                        foreach (var item in group)
                        {
                            ApThau = item.BID_APTHAU_CODE;

                            if (!string.IsNullOrEmpty(ApThau))
                            {
                                break;
                            }
                        }     

                        if (!string.IsNullOrWhiteSpace(group.First().VALID_FROM_TIME))    
                        {
                            if (DateTime.TryParseExact(group.First().VALID_FROM_TIME, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ValidFromTime))
                            {
                                bidModel.VALID_FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ValidFromTime);
                            }
                        }
                        else
                        {
                            if (FromTime != null)
                            {
                                if (DateTime.TryParseExact(FromTime, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ValidFromTime))
                                {
                                    bidModel.VALID_FROM_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ValidFromTime);
                                }
                            }
                            else
                            {
                                bidModel.VALID_FROM_TIME = null;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(group.First().VALID_TO_TIME))
                        {
                            if (DateTime.TryParseExact(group.First().VALID_TO_TIME, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ValidToTime))
                            {
                                bidModel.VALID_TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ValidToTime);
                            }
                        }
                        else
                        {
                            if (ToTime != null)
                            {
                                if (DateTime.TryParseExact(ToTime, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime ValidToTime))
                                {
                                    bidModel.VALID_TO_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(ValidToTime);
                                }
                            }
                            else
                            {
                                bidModel.VALID_TO_TIME = null;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(group.First().BID_APTHAU_CODE))
                        {
                            bidModel.BID_APTHAU_CODE = group.First().BID_APTHAU_CODE;
                        }
                        else
                        {
                            if (ApThau != null)
                            {
                                bidModel.BID_APTHAU_CODE = ApThau;
                            }
                            else
                            {
                                bidModel.BID_APTHAU_CODE = null;
                            }
                        }

                        foreach (var item in group)
                        {
                            if (item.Type == THUOC)
                            {
                                var bidMedicineType = new MOS.EFMODEL.DataModels.HIS_BID_MEDICINE_TYPE();
                                bidMedicineType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                                bidMedicineType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                                bidMedicineType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                                bidMedicineType.MEDICINE_TYPE_ID = item.ID;
                                bidMedicineType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                                bidMedicineType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                                bidMedicineType.BID_GROUP_CODE = item.BID_GROUP_CODE;
                                bidMedicineType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;
                                bidMedicineType.PACKING_TYPE_NAME = item.PACKING_TYPE_NAME;
                                bidMedicineType.HEIN_SERVICE_BHYT_NAME = item.HEIN_SERVICE_BHYT_NAME;
                                bidMedicineType.MEDICINE_USE_FORM_ID = item.MEDICINE_USE_FORM_ID;
                                bidMedicineType.ACTIVE_INGR_BHYT_NAME = item.ACTIVE_INGR_BHYT_NAME;
                                bidMedicineType.DOSAGE_FORM = item.DOSAGE_FORM;

                                bidMedicineType.CONCENTRA = item.CONCENTRA;
                                bidMedicineType.MEDICINE_REGISTER_NUMBER = item.REGISTER_NUMBER;
                                bidMedicineType.MANUFACTURER_ID = item.MANUFACTURER_ID;
                                bidMedicineType.NATIONAL_NAME = item.NATIONAL_NAME;
                                bidMedicineType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                                bidMedicineType.DAY_LIFESPAN = item.DAY_LIFESPAN;
                                bidMedicineType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;

                                bidModel.HIS_BID_MEDICINE_TYPE.Add(bidMedicineType);
                            }
                            else if (item.Type == VATTU)
                            {
                                var bidMaterialType = new MOS.EFMODEL.DataModels.HIS_BID_MATERIAL_TYPE();
                                bidMaterialType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                                bidMaterialType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                                bidMaterialType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;

                                bidMaterialType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                                bidMaterialType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);
                                bidMaterialType.BID_GROUP_CODE = item.BID_GROUP_CODE;
                                bidMaterialType.BID_PACKAGE_CODE = item.BID_PACKAGE_CODE;

                                var HisMaterialTypeMap = BackendDataWorker.Get<HIS_MATERIAL_TYPE_MAP>().FirstOrDefault(o => o.MATERIAL_TYPE_MAP_CODE == item.MATERIAL_TYPE_MAP_CODE);
                                if (HisMaterialTypeMap != null)
                                {
                                    bidMaterialType.MATERIAL_TYPE_MAP_ID = HisMaterialTypeMap.ID;
                                }
                                else
                                    bidMaterialType.MATERIAL_TYPE_ID = item.ID;

                                bidMaterialType.JOIN_BID_MATERIAL_TYPE_CODE = item.JOIN_BID_MATERIAL_TYPE_CODE;
                                bidMaterialType.BID_MATERIAL_TYPE_CODE = item.BID_MATERIAL_TYPE_CODE;
                                bidMaterialType.BID_MATERIAL_TYPE_NAME = item.BID_MATERIAL_TYPE_NAME;

                                bidMaterialType.CONCENTRA = item.CONCENTRA;
                                bidMaterialType.MANUFACTURER_ID = item.MANUFACTURER_ID;
                                bidMaterialType.NATIONAL_NAME = item.NATIONAL_NAME;
                                bidMaterialType.MONTH_LIFESPAN = item.MONTH_LIFESPAN;
                                bidMaterialType.DAY_LIFESPAN = item.DAY_LIFESPAN;
                                bidMaterialType.HOUR_LIFESPAN = item.HOUR_LIFESPAN;

                                bidModel.HIS_BID_MATERIAL_TYPE.Add(bidMaterialType);
                            }
                            else if (item.Type == MAU)
                            {
                                var bidBloodType = new MOS.EFMODEL.DataModels.HIS_BID_BLOOD_TYPE();
                                bidBloodType.AMOUNT = (decimal)(item.AMOUNT ?? 0);
                                bidBloodType.IMP_PRICE = (decimal)(item.IMP_PRICE ?? 0);
                                bidBloodType.IMP_VAT_RATIO = item.IMP_VAT_RATIO;
                                bidBloodType.BLOOD_TYPE_ID = item.ID;
                                bidBloodType.BID_NUM_ORDER = item.BID_NUM_ORDER;
                                bidBloodType.SUPPLIER_ID = (long)(item.SUPPLIER_ID ?? 0);

                                bidModel.HIS_BID_BLOOD_TYPE.Add(bidBloodType);
                            }
                        }

                        result.Add(bidModel);
                    }
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void BtnExportError_Click(object sender, EventArgs e)
        {
            try
            {
                if (!BtnExportError.Enabled) return;

                if (!this.ListDataImport.Exists(o => !String.IsNullOrWhiteSpace(o.ERROR)))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy dòng lỗi");
                    return;
                }

                Inventec.Common.FlexCellExport.Store store = new Inventec.Common.FlexCellExport.Store(true);

                string templateFile = System.IO.Path.Combine(Application.StartupPath + "\\Tmp\\Exp", "DanhSachLoiImportThau.xlsx");

                //chọn đường dẫn
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Excel 2007 later file (*.xlsx)|*.xlsx|Excel 97-2003 file(*.xls)|*.xls";
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    //getdata
                    WaitingManager.Show();

                    if (String.IsNullOrEmpty(templateFile))
                    {
                        store = null;
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Không tìm thấy file", templateFile));
                        return;
                    }

                    store.ReadTemplate(System.IO.Path.GetFullPath(templateFile));
                    if (store.TemplatePath == "")
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Biểu mẫu đang mở hoặc không tồn tại file template. Vui lòng kiểm tra lại. (" + templateFile + ")");
                        return;
                    }

                    var errorList = this.ListDataImport.Where(o => !String.IsNullOrWhiteSpace(o.ERROR)).ToList();
                    ProcessData(errorList, ref store);
                    WaitingManager.Hide();

                    if (store != null)
                    {
                        try
                        {
                            if (store.OutFile(saveFileDialog1.FileName))
                            {
                                DevExpress.XtraEditors.XtraMessageBox.Show("Xuất file thành công");

                                if (MessageBox.Show("Bạn có muốn mở file?",
                                    "Thông báo", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question) == DialogResult.Yes)
                                    System.Diagnostics.Process.Start(saveFileDialog1.FileName);
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    else
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Xử lý thất bại");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessData(List<ADO.ImportADO> errorList, ref Inventec.Common.FlexCellExport.Store store)
        {
            try
            {
                if (errorList != null && errorList.Count > 0)
                {
                    errorList.ForEach(o => o.ErrorDesc = o.ERROR);

                    Inventec.Common.FlexCellExport.ProcessSingleTag singleTag = new Inventec.Common.FlexCellExport.ProcessSingleTag();
                    Inventec.Common.FlexCellExport.ProcessObjectTag objectTag = new Inventec.Common.FlexCellExport.ProcessObjectTag();

                    store.SetCommonFunctions();
                    objectTag.AddObjectData(store, "ExportData", errorList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
