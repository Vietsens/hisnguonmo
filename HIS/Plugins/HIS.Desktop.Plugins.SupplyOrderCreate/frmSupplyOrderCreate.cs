using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SupplyOrderCreate.ADO;
using HIS.Desktop.Plugins.SupplyOrderCreate.Base;
using HIS.Desktop.Plugins.SupplyOrderCreate.Resources;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    /// <summary>
    /// Supply order by medi stock: all draft orders of the stock are edited together (one order per supplier),
    /// confirm moves them to "ordered". An order not in draft is opened read-only.
    /// </summary>
    public partial class frmSupplyOrderCreate : FormBase
    {
        private const short IS_TRUE = 1;

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? initMediStockId;
        private V_HIS_SUPPLY_ORDER openOrder;
        private HIS.Desktop.Common.DelegateRefreshData refreshData;

        private List<OrderLineADO> lines = new List<OrderLineADO>();
        private List<V_HIS_SUPPLY_ORDER> drafts = new List<V_HIS_SUPPLY_ORDER>();
        private List<V_HIS_MEDI_STOCK> supplyStocks = new List<V_HIS_MEDI_STOCK>();
        private Dictionary<string, ItemTypeADO> dicItemType = new Dictionary<string, ItemTypeADO>();
        private List<HIS_SUPPLIER> suppliers = new List<HIS_SUPPLIER>();
        private V_HIS_SUPPLY_ORDER viewOrder;
        private object lastStockValue;
        private bool isDirty = false;
        private bool isBinding = false;

        public frmSupplyOrderCreate()
        {
            InitializeComponent();
        }

        public frmSupplyOrderCreate(Inventec.Desktop.Common.Modules.Module moduleData, long? mediStockId, V_HIS_SUPPLY_ORDER openOrder, HIS.Desktop.Common.DelegateRefreshData refreshData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.initMediStockId = mediStockId;
                this.openOrder = openOrder;
                this.refreshData = refreshData;
                if (moduleData != null && !String.IsNullOrWhiteSpace(moduleData.text))
                {
                    this.Text = moduleData.text;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool IsViewMode
        {
            get { return this.viewOrder != null; }
        }

        private void frmSupplyOrderCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.SetCaptionByLanguageKey();
                this.InitStockCombo();
                this.InitItemCombo();
                this.InitGrid();
                if (this.openOrder != null && this.openOrder.SUPPLY_ORDER_STT != SupplyConstant.ORDER_STT__DRAFT)
                {
                    this.viewOrder = this.openOrder;
                    this.SetStockValue(this.openOrder.MEDI_STOCK_ID);
                    this.LoadViewOrder();
                }
                else
                {
                    long? stockId = this.openOrder != null ? (long?)this.openOrder.MEDI_STOCK_ID : this.initMediStockId;
                    if (!stockId.HasValue && this.supplyStocks.Count == 1)
                    {
                        stockId = this.supplyStocks[0].ID;
                    }
                    if (stockId.HasValue)
                    {
                        this.SetStockValue(stockId.Value);
                        this.LoadDrafts();
                    }
                    else
                    {
                        this.ApplyMode();
                    }
                    if (this.supplyStocks.Count == 0)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NotConfig"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.S))
                {
                    if (this.btnSave.Visible && this.btnSave.Enabled)
                    {
                        this.btnSave_Click(null, null);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetIcon()
        {
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lblStock.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.lblStock.Text");
                this.lblItem.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.lblItem.Text");
                this.lblItemHint.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.lblItemHint.Text");
                this.btnLoadRemain.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnLoadRemain.Text");
                this.btnLoadRemain.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnLoadRemain.ToolTip");
                this.chkPreview.Properties.Caption = ResourceLanguageManager.Get("frmSupplyOrderCreate.chkPreview.Text");
                this.btnReload.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnReload.Text");
                this.btnSave.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnSave.Text");
                this.btnSave.ToolTip = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnSave.ToolTip");
                this.btnConfirm.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnConfirm.Text");
                this.btnClose.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.btnClose.Text");
                if (this.moduleData == null || String.IsNullOrWhiteSpace(this.moduleData.text))
                {
                    this.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.Text");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool HasControl(string controlCode)
        {
            try
            {
                return GlobalVariables.AcsAuthorizeSDO != null
                    && GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null
                    && GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Any(o => o.CONTROL_CODE == controlCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        internal static string GetOrderStatusText(short stt)
        {
            switch (stt)
            {
                case SupplyConstant.ORDER_STT__DRAFT:
                    return ResourceLanguageManager.Get("OrderStatus.Draft");
                case SupplyConstant.ORDER_STT__CONFIRMED:
                    return ResourceLanguageManager.Get("OrderStatus.Confirmed");
                case SupplyConstant.ORDER_STT__CANCELED:
                    return ResourceLanguageManager.Get("OrderStatus.Canceled");
                default:
                    return "";
            }
        }

        /// <summary>
        /// Edit: stock chosen, not an opened confirmed / canceled order
        /// </summary>
        private void ApplyMode()
        {
            try
            {
                bool view = this.IsViewMode;
                bool hasStock = this.GetSelectedStockId().HasValue;
                this.cboStock.Properties.ReadOnly = view;
                this.panelAdd.Visible = !view;
                this.cboItem.Enabled = hasStock;
                this.btnLoadRemain.Enabled = hasStock;
                this.lblDraft.Visible = !view;
                this.lblOrderInfo.Visible = view;

                this.colDelete.Visible = !view;
                if (!view)
                {
                    this.colDelete.VisibleIndex = 0;
                }
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in new DevExpress.XtraGrid.Columns.GridColumn[] { this.colApproved, this.colOrdered, this.colRemain, this.colNew })
                {
                    col.Visible = !view;
                }
                foreach (DevExpress.XtraGrid.Columns.GridColumn col in new DevExpress.XtraGrid.Columns.GridColumn[] { this.colArrived, this.colImported, this.colWait })
                {
                    col.Visible = view;
                }
                if (view)
                {
                    int index = this.colTotalWithVat.VisibleIndex + 1;
                    this.colArrived.VisibleIndex = index++;
                    this.colImported.VisibleIndex = index++;
                    this.colWait.VisibleIndex = index;
                }
                else
                {
                    int index = this.colSupplier.VisibleIndex + 1;
                    this.colApproved.VisibleIndex = index++;
                    this.colOrdered.VisibleIndex = index++;
                    this.colNew.VisibleIndex = index++;
                    this.colRemain.VisibleIndex = index;
                }
                this.gridViewLine.OptionsBehavior.Editable = !view;

                this.btnSave.Visible = !view;
                this.btnConfirm.Visible = !view;
                this.btnSave.Enabled = hasStock;
                bool canOrder = this.HasControl(ControlCode.ORDER);
                this.btnConfirm.Enabled = hasStock && canOrder;
                this.btnConfirm.ToolTip = canOrder ? "" : ResourceLanguageManager.Get("frmSupplyOrderCreate.btnConfirm.NoPermission");
                this.btnReload.Visible = hasStock || view;
                this.ArrangeButtons();
                this.RefreshGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Visible buttons are right aligned, no gap left by hidden ones
        /// </summary>
        private void ArrangeButtons()
        {
            int right = this.panelBottom.ClientSize.Width - 8;
            DevExpress.XtraEditors.SimpleButton[] buttons = new DevExpress.XtraEditors.SimpleButton[] { this.btnClose, this.btnConfirm, this.btnSave, this.btnReload };
            foreach (DevExpress.XtraEditors.SimpleButton btn in buttons)
            {
                if (!btn.Visible)
                {
                    continue;
                }
                btn.Left = right - btn.Width;
                right = btn.Left - 6;
            }
        }

        private void panelBottom_Resize(object sender, EventArgs e)
        {
            this.ArrangeButtons();
        }

        private void InitStockCombo()
        {
            try
            {
                List<string> codes = SupplyConstant.GetSupplyMediStockCodes();
                this.supplyStocks = codes.Count > 0
                    ? BackendDataWorker.Get<V_HIS_MEDI_STOCK>().Where(o => codes.Contains(o.MEDI_STOCK_CODE)).OrderBy(o => o.MEDI_STOCK_CODE).ToList()
                    : new List<V_HIS_MEDI_STOCK>();
                this.cboStock.Properties.DataSource = this.supplyStocks;
                this.cboStock.Properties.ValueMember = "ID";
                this.cboStock.Properties.DisplayMember = "MEDI_STOCK_NAME";
                this.cboStock.Properties.Columns.Clear();
                this.cboStock.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MEDI_STOCK_CODE", "", 80));
                this.cboStock.Properties.Columns.Add(new DevExpress.XtraEditors.Controls.LookUpColumnInfo("MEDI_STOCK_NAME", "", 220));
                this.cboStock.Properties.ShowHeader = false;
                this.cboStock.Properties.NullText = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Opened order of a stock no longer in the config: still show its name
        /// </summary>
        private void SetStockValue(long stockId)
        {
            try
            {
                this.isBinding = true;
                if (!this.supplyStocks.Any(o => o.ID == stockId))
                {
                    V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == stockId);
                    if (stock != null)
                    {
                        List<V_HIS_MEDI_STOCK> ds = new List<V_HIS_MEDI_STOCK>(this.supplyStocks);
                        ds.Add(stock);
                        this.cboStock.Properties.DataSource = ds;
                    }
                }
                this.cboStock.EditValue = stockId;
                this.lastStockValue = this.cboStock.EditValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isBinding = false;
            }
        }

        /// <summary>
        /// Medicine + material types (chemicals included), active, leaf, not stopped for import
        /// </summary>
        private void InitItemCombo()
        {
            try
            {
                string kindMedicine = ResourceLanguageManager.Get("Kind.Medicine");
                string kindMaterial = ResourceLanguageManager.Get("Kind.Material");
                string kindChemical = ResourceLanguageManager.Get("Kind.Chemical");
                List<ItemTypeADO> items = new List<ItemTypeADO>();
                foreach (V_HIS_MEDICINE_TYPE o in BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().Where(o => o.IS_ACTIVE == IS_TRUE && o.IS_LEAF == IS_TRUE && o.IS_STOP_IMP != IS_TRUE))
                {
                    ItemTypeADO ado = new ItemTypeADO();
                    ado.TYPE_ID = o.ID;
                    ado.IS_MATERIAL = false;
                    ado.KEY = ItemTypeADO.MakeKey(false, o.ID);
                    ado.TYPE_CODE = o.MEDICINE_TYPE_CODE;
                    ado.TYPE_NAME = o.MEDICINE_TYPE_NAME;
                    ado.SERVICE_UNIT_NAME = o.SERVICE_UNIT_NAME;
                    ado.IMP_UNIT_NAME = o.IMP_UNIT_NAME;
                    ado.IMP_UNIT_CONVERT_RATIO = o.IMP_UNIT_CONVERT_RATIO;
                    ado.PACKING = !String.IsNullOrWhiteSpace(o.PACKING_TYPE_NAME) ? o.PACKING_TYPE_NAME : o.CONCENTRA;
                    ado.KIND_NAME = o.IS_CHEMICAL_SUBSTANCE == IS_TRUE ? kindChemical : kindMedicine;
                    ado.IMP_PRICE = o.IMP_PRICE;
                    ado.IMP_VAT_RATIO = o.IMP_VAT_RATIO;
                    ado.SUPPLIER_IDS = ItemTypeADO.ParseIds(o.SUPPLIER_IDS);
                    ado.DISPLAY = ado.TYPE_CODE + " - " + ado.TYPE_NAME;
                    items.Add(ado);
                }
                foreach (V_HIS_MATERIAL_TYPE o in BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().Where(o => o.IS_ACTIVE == IS_TRUE && o.IS_LEAF == IS_TRUE && o.IS_STOP_IMP != IS_TRUE))
                {
                    ItemTypeADO ado = new ItemTypeADO();
                    ado.TYPE_ID = o.ID;
                    ado.IS_MATERIAL = true;
                    ado.KEY = ItemTypeADO.MakeKey(true, o.ID);
                    ado.TYPE_CODE = o.MATERIAL_TYPE_CODE;
                    ado.TYPE_NAME = o.MATERIAL_TYPE_NAME;
                    ado.SERVICE_UNIT_NAME = o.SERVICE_UNIT_NAME;
                    ado.IMP_UNIT_NAME = o.IMP_UNIT_NAME;
                    ado.IMP_UNIT_CONVERT_RATIO = o.IMP_UNIT_CONVERT_RATIO;
                    ado.PACKING = !String.IsNullOrWhiteSpace(o.PACKING_TYPE_NAME) ? o.PACKING_TYPE_NAME : o.CONCENTRA;
                    ado.KIND_NAME = o.IS_CHEMICAL_SUBSTANCE == IS_TRUE ? kindChemical : kindMaterial;
                    ado.IMP_PRICE = o.IMP_PRICE;
                    ado.IMP_VAT_RATIO = o.IMP_VAT_RATIO;
                    ado.SUPPLIER_IDS = ItemTypeADO.ParseIds(o.SUPPLIER_IDS);
                    ado.DISPLAY = ado.TYPE_CODE + " - " + ado.TYPE_NAME;
                    items.Add(ado);
                }
                this.dicItemType = items.GroupBy(o => o.KEY).ToDictionary(g => g.Key, g => g.First());
                this.suppliers = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == IS_TRUE).OrderBy(o => o.SUPPLIER_NAME).ToList();

                this.cboItem.Properties.DataSource = items.OrderBy(o => o.TYPE_NAME).ToList();
                this.cboItem.Properties.ValueMember = "KEY";
                this.cboItem.Properties.DisplayMember = "DISPLAY";
                this.cboItem.Properties.NullText = "";
                this.cboItem.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                this.cboItem.Properties.ImmediatePopup = true;
                this.cboItem.Properties.PopupFormSize = new Size(760, 300);
                DevExpress.XtraGrid.Views.Grid.GridView popup = this.cboItem.Properties.View;
                popup.Columns.Clear();
                popup.OptionsView.ShowColumnHeaders = true;
                popup.OptionsView.ShowIndicator = false;
                this.AddPopupColumn(popup, "TYPE_CODE", "frmSupplyOrderCreate.Column.TypeCode", 100, 0);
                this.AddPopupColumn(popup, "TYPE_NAME", "frmSupplyOrderCreate.Column.TypeName", 320, 1);
                this.AddPopupColumn(popup, "SERVICE_UNIT_NAME", "frmSupplyOrderCreate.Column.ServiceUnit", 80, 2);
                this.AddPopupColumn(popup, "PACKING", "frmSupplyOrderCreate.Column.Packing", 160, 3);
                this.AddPopupColumn(popup, "KIND_NAME", "frmSupplyOrderCreate.Column.Kind", 80, 4);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddPopupColumn(DevExpress.XtraGrid.Views.Grid.GridView view, string fieldName, string captionKey, int width, int visibleIndex)
        {
            DevExpress.XtraGrid.Columns.GridColumn col = view.Columns.AddField(fieldName);
            col.Caption = ResourceLanguageManager.Get(captionKey);
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
        }

        private long? GetSelectedStockId()
        {
            return this.cboStock.EditValue != null ? (long?)Convert.ToInt64(this.cboStock.EditValue) : null;
        }

        private string GetSupplierName(long? supplierId)
        {
            if (!supplierId.HasValue)
            {
                return ResourceLanguageManager.Get("frmSupplyOrderCreate.NoSupplier");
            }
            HIS_SUPPLIER supplier = this.suppliers.FirstOrDefault(o => o.ID == supplierId.Value)
                ?? BackendDataWorker.Get<HIS_SUPPLIER>().FirstOrDefault(o => o.ID == supplierId.Value);
            return supplier != null ? supplier.SUPPLIER_NAME : supplierId.Value.ToString();
        }

        /// <summary>
        /// Changing stock drops the lines on screen (orders are by stock)
        /// </summary>
        private void cboStock_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.isBinding)
                {
                    return;
                }
                if (this.isDirty && this.lines.Count > 0
                    && DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.ChangeStockWithoutSave"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    this.isBinding = true;
                    this.cboStock.EditValue = this.lastStockValue;
                    this.isBinding = false;
                    return;
                }
                this.lastStockValue = this.cboStock.EditValue;
                this.lines = new List<OrderLineADO>();
                this.drafts = new List<V_HIS_SUPPLY_ORDER>();
                this.isDirty = false;
                if (this.GetSelectedStockId().HasValue)
                {
                    this.LoadDrafts();
                }
                else
                {
                    this.ApplyMode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDraftLabel()
        {
            try
            {
                if (this.drafts.Count == 0)
                {
                    this.lblDraft.Text = ResourceLanguageManager.Get("frmSupplyOrderCreate.lblDraft.None");
                    return;
                }
                string list = String.Join("; ", this.drafts.Select(o => o.SUPPLY_ORDER_CODE + " (" + (o.SUPPLIER_NAME ?? this.GetSupplierName(o.SUPPLIER_ID)) + ")"));
                this.lblDraft.Text = string.Format(ResourceLanguageManager.Get("frmSupplyOrderCreate.lblDraft.Text"), this.drafts.Count, list);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateOrderInfo()
        {
            try
            {
                if (this.viewOrder == null)
                {
                    this.lblOrderInfo.Text = "";
                    return;
                }
                V_HIS_SUPPLY_ORDER o = this.viewOrder;
                string text = string.Format(ResourceLanguageManager.Get("frmSupplyOrderCreate.lblOrderInfo.Text"),
                    o.SUPPLY_ORDER_CODE,
                    o.SUPPLIER_NAME ?? this.GetSupplierName(o.SUPPLIER_ID),
                    GetOrderStatusText(o.SUPPLY_ORDER_STT),
                    o.ORDER_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(o.ORDER_TIME.Value) : "",
                    o.ORDER_USERNAME);
                if (o.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__CANCELED && !String.IsNullOrWhiteSpace(o.CANCEL_REASON))
                {
                    text += " - " + string.Format(ResourceLanguageManager.Get("frmSupplyOrderCreate.lblOrderInfo.Cancel"), o.CANCEL_REASON);
                }
                this.lblOrderInfo.Text = text;
                this.lblOrderInfo.ForeColor = o.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__CANCELED ? Color.Red : Color.DarkGreen;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPreview_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.colSupplierName.GroupIndex = this.chkPreview.Checked ? 0 : -1;
                if (this.chkPreview.Checked)
                {
                    this.gridViewLine.ExpandAllGroups();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmSupplyOrderCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.isDirty && !this.IsViewMode && this.lines.Count > 0)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.CloseWithoutSave"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    {
                        e.Cancel = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
