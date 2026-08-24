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
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Plugins.HisImpMestMediMate.ADO;
using HIS.Desktop.Plugins.HisImpMestMediMate.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisImpMestMediMate.HisImpMestMediMate
{
    public partial class UCHisImpMestMediMate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare

        /// <summary>Chia lo danh sach tham so khi goi API de khong vuot gioi han do dai request</summary>
        private const int MAX_REQUEST_LENGTH_PARAM = 500;

        private Inventec.Desktop.Common.Modules.Module _Module;

        private List<MedicineTypeSearchADO> listMedicineTypeSearch;
        private List<MaterialTypeSearchADO> listMaterialTypeSearch;

        private List<ImpMestMedicineADO> listMedicineResult;
        private List<ImpMestMaterialADO> listMaterialResult;

        #endregion

        public UCHisImpMestMediMate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this._Module = module;
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisImpMestMediMate.Resources.Lang",
                    typeof(UCHisImpMestMediMate).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisImpMestMediMate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                SetCaptionByLanguageKey();
                InitContainsAutoFilter();
                InitComboMedicineType();
                InitComboMaterialType();
                InitControlState();
                ApplyObjectMode();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Language

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HisImpMestMediMate.Resources.Lang",
                    typeof(UCHisImpMestMediMate).Assembly);

                this.chkMedicine.Properties.Caption = GetLang("UCHisImpMestMediMate.chkMedicine.Properties.Caption", this.chkMedicine.Properties.Caption);
                this.chkMaterial.Properties.Caption = GetLang("UCHisImpMestMediMate.chkMaterial.Properties.Caption", this.chkMaterial.Properties.Caption);
                this.lblFrom.Text = GetLang("UCHisImpMestMediMate.lblFrom.Text", this.lblFrom.Text);
                this.lblTo.Text = GetLang("UCHisImpMestMediMate.lblTo.Text", this.lblTo.Text);
                this.btnSearch.Text = GetLang("UCHisImpMestMediMate.btnSearch.Text", this.btnSearch.Text);
                this.btnExport.Text = GetLang("UCHisImpMestMediMate.btnExport.Text", this.btnExport.Text);
                this.btnClose.Text = GetLang("UCHisImpMestMediMate.btnClose.Text", this.btnClose.Text);
                this.cboMedicineType.Properties.NullValuePrompt = GetLang("UCHisImpMestMediMate.cboMedicineType.Properties.NullValuePrompt", this.cboMedicineType.Properties.NullValuePrompt);
                this.cboMaterialType.Properties.NullValuePrompt = GetLang("UCHisImpMestMediMate.cboMaterialType.Properties.NullValuePrompt", this.cboMaterialType.Properties.NullValuePrompt);

                SetGridCaption(this.colMedSTT, "colSTT");
                SetGridCaption(this.colMedImpMestCode, "colImpMestCode");
                SetGridCaption(this.colMedImpTime, "colImpTime");
                SetGridCaption(this.colMedDocumentNumber, "colDocumentNumber");
                SetGridCaption(this.colMedDocumentDate, "colDocumentDate");
                SetGridCaption(this.colMedBytNumOrder, "colBytNumOrder");
                SetGridCaption(this.colMedTypeCode, "colMedicineTypeCode");
                SetGridCaption(this.colMedTypeName, "colMedicineTypeName");
                SetGridCaption(this.colMedActiveIngredient, "colActiveIngredient");
                SetGridCaption(this.colMedServiceUnit, "colServiceUnit");
                SetGridCaption(this.colMedAmount, "colAmount");
                SetGridCaption(this.colMedSupplier, "colSupplier");
                SetGridCaption(this.colMedMediStock, "colMediStock");
                SetGridCaption(this.colMedImpSource, "colImpSource");
                SetGridCaption(this.colMedManufacturer, "colManufacturer");
                SetGridCaption(this.colMedNational, "colNational");
                SetGridCaption(this.colMedImpPrice, "colImpPrice");
                SetGridCaption(this.colMedTotalPrice, "colTotalPrice");
                SetGridCaption(this.colMedVatRatio, "colVatRatio");
                SetGridCaption(this.colMedImpPriceVat, "colImpPriceVat");
                SetGridCaption(this.colMedTotalPriceVat, "colTotalPriceVat");

                SetGridCaption(this.colMatSTT, "colSTT");
                SetGridCaption(this.colMatImpMestCode, "colImpMestCode");
                SetGridCaption(this.colMatImpTime, "colImpTime");
                SetGridCaption(this.colMatDocumentNumber, "colDocumentNumber");
                SetGridCaption(this.colMatDocumentDate, "colDocumentDate");
                SetGridCaption(this.colMatTypeCode, "colMaterialTypeCode");
                SetGridCaption(this.colMatTypeName, "colMaterialTypeName");
                SetGridCaption(this.colMatServiceUnit, "colServiceUnit");
                SetGridCaption(this.colMatAmount, "colAmount");
                SetGridCaption(this.colMatSupplier, "colSupplier");
                SetGridCaption(this.colMatMediStock, "colMediStock");
                SetGridCaption(this.colMatImpSource, "colImpSource");
                SetGridCaption(this.colMatManufacturer, "colManufacturer");
                SetGridCaption(this.colMatNational, "colNational");
                SetGridCaption(this.colMatImpPrice, "colImpPrice");
                SetGridCaption(this.colMatTotalPrice, "colTotalPrice");
                SetGridCaption(this.colMatVatRatio, "colVatRatio");
                SetGridCaption(this.colMatImpPriceVat, "colImpPriceVat");
                SetGridCaption(this.colMatTotalPriceVat, "colTotalPriceVat");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetGridCaption(DevExpress.XtraGrid.Columns.GridColumn column, string key)
        {
            try
            {
                column.Caption = GetLang("UCHisImpMestMediMate." + key + ".Caption", column.Caption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Lay chuoi da dich; neu thieu key thi giu nguyen gia tri mac dinh tren Designer</summary>
        private string GetLang(string key, string defaultValue)
        {
            try
            {
                string value = Inventec.Common.Resource.Get.Value(
                    key, Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                return string.IsNullOrEmpty(value) ? defaultValue : value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return defaultValue;
            }
        }

        #endregion

        #region Init combo

        private void InitComboMedicineType()
        {
            try
            {
                listMedicineTypeSearch = GlobalStore.MedicineTypes
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Select(o => new MedicineTypeSearchADO(o))
                    .OrderBy(o => o.MEDICINE_TYPE_NAME)
                    .ToList();

                var cbo = this.cboMedicineType;
                cbo.Properties.DataSource = listMedicineTypeSearch;
                cbo.Properties.DisplayMember = "MEDICINE_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = false;
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(900, 300);

                AddLookupColumn(cbo, "MEDICINE_TYPE_CODE", GetLang("UCHisImpMestMediMate.colMedicineTypeCode.Caption", "Mã thuốc"), 1, 90);
                AddLookupColumn(cbo, "MEDICINE_TYPE_NAME", GetLang("UCHisImpMestMediMate.colMedicineTypeName.Caption", "Tên thuốc - hàm lượng"), 2, 300);
                AddLookupColumn(cbo, "SERVICE_UNIT_NAME", GetLang("UCHisImpMestMediMate.colServiceUnit.Caption", "ĐVT"), 3, 60);
                AddLookupColumn(cbo, "ACTIVE_INGR_BHYT_CODE", GetLang("UCHisImpMestMediMate.colActiveIngredientCode.Caption", "Mã hoạt chất"), 4, 100);
                AddLookupColumn(cbo, "ACTIVE_INGR_BHYT_NAME", GetLang("UCHisImpMestMediMate.colActiveIngredient.Caption", "Hoạt chất"), 5, 260);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboMaterialType()
        {
            try
            {
                listMaterialTypeSearch = GlobalStore.MaterialTypes
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .Select(o => new MaterialTypeSearchADO(o))
                    .OrderBy(o => o.MATERIAL_TYPE_NAME)
                    .ToList();

                var cbo = this.cboMaterialType;
                cbo.Properties.DataSource = listMaterialTypeSearch;
                cbo.Properties.DisplayMember = "MATERIAL_TYPE_NAME";
                cbo.Properties.ValueMember = "ID";
                cbo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cbo.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cbo.Properties.ImmediatePopup = true;
                cbo.ForceInitialize();
                cbo.Properties.View.OptionsView.ShowAutoFilterRow = false;
                cbo.Properties.View.Columns.Clear();
                cbo.Properties.PopupFormSize = new Size(700, 300);

                AddLookupColumn(cbo, "MATERIAL_TYPE_CODE", GetLang("UCHisImpMestMediMate.colMaterialTypeCode.Caption", "Mã vật tư"), 1, 90);
                AddLookupColumn(cbo, "MATERIAL_TYPE_NAME", GetLang("UCHisImpMestMediMate.colMaterialTypeName.Caption", "Tên vật tư"), 2, 400);
                AddLookupColumn(cbo, "SERVICE_UNIT_NAME", GetLang("UCHisImpMestMediMate.colServiceUnit.Caption", "ĐVT"), 3, 60);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLookupColumn(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo,
            string fieldName, string caption, int visibleIndex, int width)
        {
            try
            {
                var column = cbo.Properties.View.Columns.AddField(fieldName);
                column.Caption = caption;
                column.Visible = true;
                column.VisibleIndex = visibleIndex;
                column.Width = width;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Doi tuong tra cuu (Thuoc / Vat tu)

        /// <summary>true = dang tra cuu Thuoc, false = dang tra cuu Vat tu</summary>
        private bool IsMedicineMode
        {
            get { return this.chkMedicine.Checked; }
        }

        private void chkMedicine_CheckedChanged(object sender, EventArgs e)
        {
            ObjectMode_CheckedChanged(sender, e);
        }

        private void chkMaterial_CheckedChanged(object sender, EventArgs e)
        {
            ObjectMode_CheckedChanged(sender, e);
        }

        /// <summary>Chan de quy khi dong bo lai 2 o tich</summary>
        private bool isSyncingObjectMode;

        /// <summary>
        /// Bat buoc luon co dung 1 doi tuong duoc chon:
        ///  - Tich 1 o    -> o do la doi tuong dang tra cuu, o con lai tu bo tich.
        ///  - Bo tich 1 o -> o con lai tu duoc bat (khong the bo tich ca 2).
        /// Doi doi tuong se don ket qua dang hien thi, khong tu dong tra cuu lai.
        /// </summary>
        private void ObjectMode_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isSyncingObjectMode)
                {
                    return;
                }

                var toggle = sender as DevExpress.XtraEditors.CheckEdit;
                if (toggle == null)
                {
                    return;
                }

                bool isMedicine = (toggle == this.chkMedicine) ? toggle.Checked : !toggle.Checked;

                isSyncingObjectMode = true;
                try
                {
                    this.chkMedicine.Checked = isMedicine;
                    this.chkMaterial.Checked = !isMedicine;
                }
                finally
                {
                    isSyncingObjectMode = false;
                }

                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                SaveControlState();
                ApplyObjectMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Hien/an o tra cuu va luoi tuong ung voi doi tuong dang chon</summary>
        private void ApplyObjectMode()
        {
            try
            {
                bool isMedicine = IsMedicineMode;

                this.lblType.Text = isMedicine
                    ? GetLang("UCHisImpMestMediMate.chkMedicine.Properties.Caption", "Thuốc") + ":"
                    : GetLang("UCHisImpMestMediMate.chkMaterial.Properties.Caption", "Vật tư") + ":";

                this.cboMedicineType.Visible = isMedicine;
                this.cboMaterialType.Visible = !isMedicine;

                ClearError();
                ClearResult();

                this.gridControlData.MainView = isMedicine
                    ? (BaseView)this.gridViewMedicine
                    : (BaseView)this.gridViewMaterial;

                if (isMedicine)
                    this.cboMedicineType.Focus();
                else
                    this.cboMaterialType.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tra cuu

        private void cboType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    Search();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void Search()
        {
            try
            {
                ClearError();

                if (!ValidTypeSelected() || !ValidTimeRange())
                {
                    return;
                }

                WaitingManager.Show();
                if (IsMedicineMode)
                {
                    LoadDataMedicine();
                }
                else
                {
                    LoadDataMaterial();
                }
                WaitingManager.Hide();

                if (GetCurrentRowCount() <= 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceMessage.KhongTonTaiHoaDonTuongUng,
                        Resources.ResourceMessage.ThongBao,
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Chua chon loai thuoc/vat tu thi canh bao ngay tai o tra cuu do</summary>
        private bool ValidTypeSelected()
        {
            try
            {
                if (IsMedicineMode)
                {
                    if (GetSelectedTypeId(this.cboMedicineType) <= 0)
                    {
                        this.cboMedicineType.ErrorText = Resources.ResourceMessage.BanChuaChonThuoc;
                        this.cboMedicineType.Focus();
                        return false;
                    }
                }
                else
                {
                    if (GetSelectedTypeId(this.cboMaterialType) <= 0)
                    {
                        this.cboMaterialType.ErrorText = Resources.ResourceMessage.BanChuaChonVatTu;
                        this.cboMaterialType.Focus();
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        /// <summary>Tu ngay - Den ngay la tuy chon, nhung neu nhap ca 2 thi phai hop le</summary>
        private bool ValidTimeRange()
        {
            try
            {
                long from = GetTimeFrom();
                long to = GetTimeTo();
                if (from > 0 && to > 0 && from > to)
                {
                    this.dtFrom.ErrorText = Resources.ResourceMessage.KhoangThoiGianKhongHopLe;
                    this.dtFrom.Focus();
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
            }
        }

        private long GetSelectedTypeId(Inventec.Desktop.CustomControl.CustomGridLookUpEditWithFilterMultiColumn cbo)
        {
            try
            {
                if (cbo.EditValue == null) return 0;
                return Inventec.Common.TypeConvert.Parse.ToInt64(cbo.EditValue.ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private long GetTimeFrom()
        {
            try
            {
                if (this.dtFrom.EditValue == null || this.dtFrom.DateTime == DateTime.MinValue) return 0;
                return Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(this.dtFrom.EditValue).ToString("yyyyMMdd") + "000000");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private long GetTimeTo()
        {
            try
            {
                if (this.dtTo.EditValue == null || this.dtTo.DateTime == DateTime.MinValue) return 0;
                return Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(this.dtTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return 0;
            }
        }

        private void ClearError()
        {
            try
            {
                this.cboMedicineType.ErrorText = "";
                this.cboMaterialType.ErrorText = "";
                this.dtFrom.ErrorText = "";
                this.dtTo.ErrorText = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ClearResult()
        {
            try
            {
                this.listMedicineResult = null;
                this.listMaterialResult = null;
                this.gridControlData.DataSource = null;
                this.lblTotal.Text = "";
                this.btnExport.Enabled = false;
                ClearColumnFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Cac cot dang chuoi tren luoi -> dong loc theo cot so khop kieu *chua*.
        /// Mac dinh cua luoi la so khop tu dau chuoi, nguoi dung phai go dung tu ky tu
        /// dau tien moi ra ket qua; doi sang *chua* de go doan giua cung tim duoc.
        /// </summary>
        private static readonly string[] TEXT_FILTER_FIELDS = new string[]
        {
            "IMP_MEST_CODE",
            "IMP_TIME_STR",
            "DOCUMENT_NUMBER",
            "DOCUMENT_DATE_STR",
            "BYT_NUM_ORDER",
            "TYPE_CODE",
            "TYPE_NAME",
            "ACTIVE_INGR_BHYT_NAME",
            "SERVICE_UNIT_NAME",
            "SUPPLIER_NAME",
            "MEDI_STOCK_NAME",
            "IMP_SOURCE_NAME",
            "MANUFACTURER_NAME",
            "NATIONAL_NAME"
        };

        private void InitContainsAutoFilter()
        {
            try
            {
                var views = new DevExpress.XtraGrid.Views.Grid.GridView[]
                {
                    this.gridViewMedicine,
                    this.gridViewMaterial
                };

                foreach (var view in views)
                {
                    foreach (DevExpress.XtraGrid.Columns.GridColumn column in view.Columns)
                    {
                        if (TEXT_FILTER_FIELDS.Contains(column.FieldName))
                        {
                            column.OptionsFilter.AutoFilterCondition =
                                DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Don dieu kien loc tren dong loc theo tung cot cua luoi, tranh viec
        /// dieu kien loc cu che bot ket qua cua lan tra cuu moi.
        /// </summary>
        private void ClearColumnFilter()
        {
            try
            {
                var columnView = this.gridControlData.MainView as ColumnView;
                if (columnView == null) return;

                columnView.ActiveFilterString = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private int GetCurrentRowCount()
        {
            if (IsMedicineMode)
                return this.listMedicineResult == null ? 0 : this.listMedicineResult.Count;
            return this.listMaterialResult == null ? 0 : this.listMaterialResult.Count;
        }

        /// <summary>Cap nhat so dong tim duoc va trang thai nut Excel</summary>
        private void UpdateResultState()
        {
            try
            {
                int count = GetCurrentRowCount();
                this.lblTotal.Text = count > 0
                    ? string.Format(Resources.ResourceMessage.TongSoDongTimDuoc, count)
                    : "";
                this.btnExport.Enabled = count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Ghep them Ten kho / Nguon nhap / Ngay hoa don

        /// <summary>Ten kho nhap lay tu danh muc kho da tai san tren may tram</summary>
        private string GetMediStockName(long mediStockId)
        {
            try
            {
                var stock = GlobalStore.MediStocks.FirstOrDefault(o => o.ID == mediStockId);
                return stock != null ? stock.MEDI_STOCK_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>Ten nguon nhap lay tu danh muc nguon nhap</summary>
        private string GetImpSourceName(long? impSourceId)
        {
            try
            {
                if (!impSourceId.HasValue || impSourceId.Value <= 0) return "";
                var source = GlobalStore.ImpSources.FirstOrDefault(o => o.ID == impSourceId.Value);
                return source != null ? source.IMP_SOURCE_NAME : "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return "";
            }
        }

        /// <summary>
        /// Lay Ngay hoa don cua cac phieu nhap lien quan.
        /// Chia lo danh sach Id de khong vuot gioi han do dai tham so request.
        /// </summary>
        private Dictionary<long, long?> GetDocumentDateByImpMest(List<long> impMestIds)
        {
            var result = new Dictionary<long, long?>();
            try
            {
                if (impMestIds == null || impMestIds.Count == 0) return result;

                var ids = impMestIds.Distinct().ToList();
                int skip = 0;
                while (ids.Count - skip > 0)
                {
                    var pageIds = ids.Skip(skip).Take(MAX_REQUEST_LENGTH_PARAM).ToList();
                    skip += MAX_REQUEST_LENGTH_PARAM;

                    var filter = new MOS.Filter.HisImpMestViewFilter();
                    filter.IDs = pageIds;
                    var data = new Inventec.Common.Adapter.BackendAdapter(new Inventec.Core.CommonParam())
                        .Get<List<V_HIS_IMP_MEST>>(
                            MediMateRequestUriStore.HIS_IMP_MEST_GETVIEW,
                            HIS.Desktop.ApiConsumer.ApiConsumers.MosConsumer,
                            filter, null);

                    if (data != null && data.Count > 0)
                    {
                        foreach (var item in data)
                        {
                            if (!result.ContainsKey(item.ID))
                            {
                                result.Add(item.ID, item.DOCUMENT_DATE);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Kết thúc

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                var form = this.FindForm();
                if (form != null)
                {
                    form.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
