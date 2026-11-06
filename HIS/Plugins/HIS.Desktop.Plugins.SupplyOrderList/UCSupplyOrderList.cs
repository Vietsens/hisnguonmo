using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SupplyOrderList.ADO;
using HIS.Desktop.Plugins.SupplyOrderList.Base;
using HIS.Desktop.Plugins.SupplyOrderList.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderList
{
    /// <summary>
    /// Supply orders (HIS_SUPPLY_ORDER) of the medi stocks in MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES: follow up, send mail, cancel
    /// </summary>
    public partial class UCSupplyOrderList : UserControlBase
    {
        private const string FORMAT_MONEY = "#,##0.####";
        private const string FORMAT_AMOUNT = "#,##0.####";
        private const short IS_TRUE = 1;

        /// <summary>
        /// Value of the "not sent" item in the mail status filter (MAIL_STT null or 1)
        /// </summary>
        private const short MAIL_FILTER__NOT_SENT = 0;

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private List<V_HIS_MEDI_STOCK> supplyStocks = new List<V_HIS_MEDI_STOCK>();
        private Dictionary<long, List<OrderDetailADO>> dicDetail = new Dictionary<long, List<OrderDetailADO>>();

        public UCSupplyOrderList()
        {
            InitializeComponent();
        }

        public UCSupplyOrderList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
        }

        private void UCSupplyOrderList_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetCaptionByLanguageKey();
                this.InitStockCombo();
                this.InitSupplierCombo();
                this.InitStatusCombo();
                this.InitGridOrder();
                this.InitGridDetail();
                if (this.supplyStocks.Count == 0)
                {
                    this.lblNotConfig.Visible = true;
                    this.btnSearch.Enabled = false;
                    this.btnCreate.Enabled = false;
                }
                else
                {
                    this.lblNotConfig.Visible = false;
                    this.FillDataToGrid();
                }
                this.ApplyButtonState();
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
                this.lblStock.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblStock.Text");
                this.lblSupplier.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblSupplier.Text");
                this.lblStatus.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblStatus.Text");
                this.lblMailStatus.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblMailStatus.Text");
                this.lblFrom.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblFrom.Text");
                this.lblTo.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblTo.Text");
                this.lblCode.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblCode.Text");
                this.lblNotConfig.Text = ResourceLanguageManager.Get("UCSupplyOrderList.lblNotConfig.Text");
                this.btnSearch.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnSearch.Text");
                this.btnCreate.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnCreate.Text");
                this.btnView.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnView.Text");
                this.btnSendMail.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnSendMail.Text");
                this.btnCancelOrder.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnCancelOrder.Text");
                this.btnDeleteDraft.Text = ResourceLanguageManager.Get("UCSupplyOrderList.btnDeleteDraft.Text");
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

        internal static string GetMailStatusText(short? stt)
        {
            switch (stt ?? SupplyConstant.MAIL_STT__NOT_SENT)
            {
                case SupplyConstant.MAIL_STT__SENDING:
                    return ResourceLanguageManager.Get("MailStatus.Sending");
                case SupplyConstant.MAIL_STT__SUCCESS:
                    return ResourceLanguageManager.Get("MailStatus.Success");
                case SupplyConstant.MAIL_STT__ERROR:
                    return ResourceLanguageManager.Get("MailStatus.Error");
                default:
                    return ResourceLanguageManager.Get("MailStatus.NotSent");
            }
        }

        private void AddClearButton(DevExpress.XtraEditors.LookUpEdit editor)
        {
            editor.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
            editor.ButtonClick += (s, e) =>
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    editor.EditValue = null;
                }
            };
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
                this.cboStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_CODE", "", 80));
                this.cboStock.Properties.Columns.Add(new LookUpColumnInfo("MEDI_STOCK_NAME", "", 220));
                this.cboStock.Properties.ShowHeader = false;
                this.cboStock.Properties.NullText = ResourceLanguageManager.Get("UCSupplyOrderList.cboStock.NullText");
                this.AddClearButton(this.cboStock);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitSupplierCombo()
        {
            try
            {
                this.cboSupplier.Properties.DataSource = BackendDataWorker.Get<HIS_SUPPLIER>().Where(o => o.IS_ACTIVE == IS_TRUE).OrderBy(o => o.SUPPLIER_NAME).ToList();
                this.cboSupplier.Properties.ValueMember = "ID";
                this.cboSupplier.Properties.DisplayMember = "SUPPLIER_NAME";
                this.cboSupplier.Properties.Columns.Clear();
                this.cboSupplier.Properties.Columns.Add(new LookUpColumnInfo("SUPPLIER_CODE", "", 70));
                this.cboSupplier.Properties.Columns.Add(new LookUpColumnInfo("SUPPLIER_NAME", "", 250));
                this.cboSupplier.Properties.ShowHeader = false;
                this.cboSupplier.Properties.PopupWidth = 330;
                this.cboSupplier.Properties.SearchMode = SearchMode.AutoFilter;
                this.cboSupplier.Properties.NullText = "";
                this.AddClearButton(this.cboSupplier);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitStatusCombo()
        {
            try
            {
                this.cboStatus.Properties.Items.Clear();
                this.cboStatus.Properties.Items.Add(SupplyConstant.ORDER_STT__DRAFT, GetOrderStatusText(SupplyConstant.ORDER_STT__DRAFT), CheckState.Checked, true);
                this.cboStatus.Properties.Items.Add(SupplyConstant.ORDER_STT__CONFIRMED, GetOrderStatusText(SupplyConstant.ORDER_STT__CONFIRMED), CheckState.Checked, true);
                this.cboStatus.Properties.Items.Add(SupplyConstant.ORDER_STT__CANCELED, GetOrderStatusText(SupplyConstant.ORDER_STT__CANCELED), CheckState.Unchecked, true);
                this.cboStatus.Properties.SeparatorChar = ',';

                this.cboMailStatus.Properties.Items.Clear();
                this.cboMailStatus.Properties.Items.Add(MAIL_FILTER__NOT_SENT, GetMailStatusText(null), CheckState.Unchecked, true);
                foreach (short stt in new short[] { SupplyConstant.MAIL_STT__SENDING, SupplyConstant.MAIL_STT__SUCCESS, SupplyConstant.MAIL_STT__ERROR })
                {
                    this.cboMailStatus.Properties.Items.Add(stt, GetMailStatusText(stt), CheckState.Unchecked, true);
                }
                this.cboMailStatus.Properties.SeparatorChar = ',';
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridOrder()
        {
            try
            {
                GridView view = this.gridViewOrder;
                view.Columns.Clear();
                int i = 0;
                this.AddColumn(view, "STT", "UCSupplyOrderList.Column.Stt", 45, i++, null);
                this.AddColumn(view, "SUPPLY_ORDER_CODE", "UCSupplyOrderList.Column.OrderCode", 100, i++, null);
                this.AddColumn(view, "MEDI_STOCK_NAME", "UCSupplyOrderList.Column.MediStock", 160, i++, null);
                this.AddColumn(view, "SUPPLIER_NAME", "UCSupplyOrderList.Column.Supplier", 220, i++, null);
                this.AddColumn(view, "ORDER_TIME_DISPLAY", "UCSupplyOrderList.Column.OrderTime", 115, i++, null);
                this.AddColumn(view, "ORDER_USERNAME", "UCSupplyOrderList.Column.OrderUser", 130, i++, null);
                this.AddColumn(view, "STATUS_DISPLAY", "UCSupplyOrderList.Column.Status", 110, i++, null);
                this.AddColumn(view, "MAIL_STATUS_DISPLAY", "UCSupplyOrderList.Column.MailStatus", 90, i++, null);
                this.AddColumn(view, "TOTAL_PRICE", "UCSupplyOrderList.Column.TotalPrice", 110, i++, FORMAT_MONEY);
                this.AddColumn(view, "TOTAL_VAT_PRICE", "UCSupplyOrderList.Column.VatPrice", 100, i++, FORMAT_MONEY);
                this.AddColumn(view, "TOTAL_WITH_VAT", "UCSupplyOrderList.Column.TotalWithVat", 120, i++, FORMAT_MONEY);
                this.AddColumn(view, "SUM_AMOUNT", "UCSupplyOrderList.Column.SumAmount", 90, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "SUM_ARRIVED_AMOUNT", "UCSupplyOrderList.Column.SumArrived", 90, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "SUM_IMPORTED_AMOUNT", "UCSupplyOrderList.Column.SumImported", 90, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "IMP_MEST_COUNT", "UCSupplyOrderList.Column.ImpMestCount", 80, i++, "#,##0");
                this.AddColumn(view, "LAST_MAIL_TIME_DISPLAY", "UCSupplyOrderList.Column.LastMailTime", 115, i++, null);
                this.AddColumn(view, "LAST_MAIL_TO", "UCSupplyOrderList.Column.LastMailTo", 180, i++, null);
                this.AddColumn(view, "LAST_MAIL_ERROR", "UCSupplyOrderList.Column.LastMailError", 200, i++, null);
                this.AddColumn(view, "CANCEL_REASON", "UCSupplyOrderList.Column.CancelReason", 200, i++, null);
                this.AddColumn(view, "DESCRIPTION", "UCSupplyOrderList.Column.Description", 200, i++, null);
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.FocusedRowChanged += (s, e) => this.FocusedOrderChanged();
                view.DoubleClick += this.gridViewOrder_DoubleClick;
                view.RowCellStyle += this.gridViewOrder_RowCellStyle;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGridDetail()
        {
            try
            {
                GridView view = this.gridViewDetail;
                view.Columns.Clear();
                int i = 0;
                this.AddColumn(view, "STT", "UCSupplyOrderList.Column.Stt", 45, i++, null);
                this.AddColumn(view, "TYPE_CODE", "UCSupplyOrderList.Column.TypeCode", 90, i++, null);
                this.AddColumn(view, "TYPE_NAME", "UCSupplyOrderList.Column.TypeName", 240, i++, null);
                this.AddColumn(view, "SERVICE_UNIT_NAME", "UCSupplyOrderList.Column.ServiceUnit", 60, i++, null);
                this.AddColumn(view, "PACKING", "UCSupplyOrderList.Column.Packing", 110, i++, null);
                this.AddColumn(view, "IMP_UNIT_AMOUNT", "UCSupplyOrderList.Column.ImpUnitAmount", 75, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "IMP_UNIT_NAME", "UCSupplyOrderList.Column.ImpUnit", 60, i++, null);
                this.AddColumn(view, "AMOUNT", "UCSupplyOrderList.Column.Amount", 85, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "IMP_PRICE", "UCSupplyOrderList.Column.ImpPrice", 90, i++, FORMAT_MONEY);
                this.AddColumn(view, "VAT_PERCENT", "UCSupplyOrderList.Column.VatPercent", 55, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "TOTAL_PRICE", "UCSupplyOrderList.Column.TotalPrice", 100, i++, FORMAT_MONEY);
                this.AddColumn(view, "ARRIVED_AMOUNT", "UCSupplyOrderList.Column.Arrived", 85, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "IMPORTED_AMOUNT", "UCSupplyOrderList.Column.Imported", 85, i++, FORMAT_AMOUNT);
                this.AddColumn(view, "WAIT_AMOUNT", "UCSupplyOrderList.Column.Wait", 85, i++, FORMAT_AMOUNT);
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private GridColumn AddColumn(GridView view, string fieldName, string captionKey, int width, int visibleIndex, string format)
        {
            GridColumn col = new GridColumn();
            col.FieldName = fieldName;
            col.Caption = ResourceLanguageManager.Get(captionKey);
            col.Width = width;
            col.Visible = true;
            col.VisibleIndex = visibleIndex;
            col.OptionsColumn.AllowEdit = false;
            if (!String.IsNullOrEmpty(format))
            {
                col.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                col.DisplayFormat.FormatString = format;
                col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            }
            view.Columns.Add(col);
            return col;
        }

        private void gridViewOrder_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                SupplyOrderADO row = this.gridViewOrder.GetRow(e.RowHandle) as SupplyOrderADO;
                if (row == null)
                {
                    return;
                }
                if (e.Column.FieldName == "STATUS_DISPLAY")
                {
                    if (row.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__CANCELED)
                    {
                        e.Appearance.ForeColor = Color.Red;
                    }
                    else if (row.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__CONFIRMED)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                }
                else if (e.Column.FieldName == "MAIL_STATUS_DISPLAY" || e.Column.FieldName == "LAST_MAIL_ERROR")
                {
                    if (row.MAIL_STT == SupplyConstant.MAIL_STT__ERROR)
                    {
                        e.Appearance.ForeColor = Color.White;
                        e.Appearance.BackColor = Color.IndianRed;
                    }
                    else if (row.MAIL_STT == SupplyConstant.MAIL_STT__SUCCESS)
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private SupplyOrderADO GetFocusedRow()
        {
            return this.gridViewOrder.GetFocusedRow() as SupplyOrderADO;
        }

        private void FocusedOrderChanged()
        {
            try
            {
                SupplyOrderADO row = this.GetFocusedRow();
                List<OrderDetailADO> details;
                if (row == null || !this.dicDetail.TryGetValue(row.ID, out details))
                {
                    details = new List<OrderDetailADO>();
                }
                this.gridControlDetail.DataSource = details;
                this.ApplyButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyButtonState()
        {
            try
            {
                SupplyOrderADO row = this.GetFocusedRow();
                short stt = row != null ? row.SUPPLY_ORDER_STT : (short)0;
                bool confirmed = stt == SupplyConstant.ORDER_STT__CONFIRMED;
                this.btnView.Enabled = row != null;
                this.btnSendMail.Enabled = confirmed && this.HasControl(ControlCode.SEND_MAIL);
                this.btnSendMail.Text = ResourceLanguageManager.Get(row != null && row.MAIL_STT.HasValue && row.MAIL_STT != SupplyConstant.MAIL_STT__NOT_SENT
                    ? "UCSupplyOrderList.btnSendMail.Resend" : "UCSupplyOrderList.btnSendMail.Text");
                bool hasImport = row != null && (row.IMP_MEST_COUNT ?? 0) > 0;
                this.btnCancelOrder.Enabled = confirmed && !hasImport && this.HasControl(ControlCode.ORDER);
                this.btnCancelOrder.ToolTip = confirmed && hasImport ? ResourceLanguageManager.Get("UCSupplyOrderList.btnCancelOrder.HasImport") : "";
                this.btnDeleteDraft.Enabled = stt == SupplyConstant.ORDER_STT__DRAFT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FillDataToGrid()
        {
            CommonParam param = new CommonParam();
            try
            {
                if (this.supplyStocks.Count == 0)
                {
                    return;
                }
                HisSupplyOrderViewFilter filter = new HisSupplyOrderViewFilter();
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (this.cboStock.EditValue != null)
                {
                    filter.MEDI_STOCK_ID = Convert.ToInt64(this.cboStock.EditValue);
                }
                else
                {
                    filter.MEDI_STOCK_IDs = this.supplyStocks.Select(o => o.ID).ToList();
                }
                if (this.cboSupplier.EditValue != null)
                {
                    filter.SUPPLIER_ID = Convert.ToInt64(this.cboSupplier.EditValue);
                }
                List<short> stts = this.GetCheckedValues(this.cboStatus);
                if (stts.Count > 0)
                {
                    filter.SUPPLY_ORDER_STTs = stts;
                }
                if (this.dtFrom.EditValue != null)
                {
                    filter.ORDER_TIME_FROM = Convert.ToInt64(this.dtFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (this.dtTo.EditValue != null)
                {
                    filter.ORDER_TIME_TO = Convert.ToInt64(this.dtTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (!String.IsNullOrWhiteSpace(this.txtCode.Text))
                {
                    filter.SUPPLY_ORDER_CODE__EXACT = this.txtCode.Text.Trim();
                }

                WaitingManager.Show();
                List<V_HIS_SUPPLY_ORDER> data = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>(RequestUriStore.HIS_SUPPLY_ORDER__GET_VIEW, ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_SUPPLY_ORDER>();

                // mail status: "not sent" = MAIL_STT empty or 1
                List<short> mailStts = this.GetCheckedValues(this.cboMailStatus);
                if (mailStts.Count > 0)
                {
                    data = data.Where(o => mailStts.Contains((o.MAIL_STT.HasValue && o.MAIL_STT != SupplyConstant.MAIL_STT__NOT_SENT) ? o.MAIL_STT.Value : MAIL_FILTER__NOT_SENT)).ToList();
                }

                this.dicDetail = new Dictionary<long, List<OrderDetailADO>>();
                if (data.Count > 0)
                {
                    HisSupplyOrderDetailFilter detailFilter = new HisSupplyOrderDetailFilter();
                    detailFilter.SUPPLY_ORDER_IDs = data.Select(o => o.ID).ToList();
                    List<HisSupplyOrderDetailSDO> details = new BackendAdapter(param).Get<List<HisSupplyOrderDetailSDO>>(RequestUriStore.HIS_SUPPLY_ORDER_DETAIL__GET, ApiConsumers.MosConsumer, detailFilter, param) ?? new List<HisSupplyOrderDetailSDO>();
                    this.dicDetail = details.GroupBy(o => o.SUPPLY_ORDER_ID).ToDictionary(g => g.Key, g =>
                    {
                        int stt = 1;
                        return g.OrderBy(o => o.ID).Select(o => new OrderDetailADO(o) { STT = stt++ }).ToList();
                    });
                }

                List<SupplyOrderADO> rows = new List<SupplyOrderADO>();
                int index = 1;
                foreach (V_HIS_SUPPLY_ORDER item in data)
                {
                    SupplyOrderADO ado = new SupplyOrderADO(item);
                    ado.STT = index++;
                    ado.ORDER_TIME_DISPLAY = item.ORDER_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.ORDER_TIME.Value) : "";
                    ado.LAST_MAIL_TIME_DISPLAY = item.LAST_MAIL_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.LAST_MAIL_TIME.Value) : "";
                    ado.STATUS_DISPLAY = GetOrderStatusText(item.SUPPLY_ORDER_STT);
                    ado.MAIL_STATUS_DISPLAY = item.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__CONFIRMED || item.MAIL_STT.HasValue ? GetMailStatusText(item.MAIL_STT) : "";
                    List<OrderDetailADO> details;
                    if (this.dicDetail.TryGetValue(item.ID, out details))
                    {
                        ado.SUM_AMOUNT = details.Sum(o => o.AMOUNT);
                        ado.SUM_ARRIVED_AMOUNT = details.Sum(o => o.ARRIVED_AMOUNT);
                        ado.SUM_IMPORTED_AMOUNT = details.Sum(o => o.IMPORTED_AMOUNT);
                    }
                    rows.Add(ado);
                }
                this.gridControlOrder.DataSource = rows;
                WaitingManager.Hide();
                this.FocusedOrderChanged();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<short> GetCheckedValues(DevExpress.XtraEditors.CheckedComboBoxEdit editor)
        {
            List<short> result = new List<short>();
            foreach (CheckedListBoxItem item in editor.Properties.Items)
            {
                if (item.CheckState == CheckState.Checked)
                {
                    result.Add((short)item.Value);
                }
            }
            return result;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.FillDataToGrid();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.FillDataToGrid();
            }
        }

        private void gridViewOrder_DoubleClick(object sender, EventArgs e)
        {
            DevExpress.Utils.DXMouseEventArgs ea = e as DevExpress.Utils.DXMouseEventArgs;
            if (ea != null)
            {
                GridHitInfo hit = this.gridViewOrder.CalcHitInfo(ea.Location);
                if (!hit.InRow)
                {
                    return;
                }
            }
            this.btnView_Click(null, null);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            this.OpenCreateModule(null);
        }

        /// <summary>
        /// Draft: the create module opens all drafts of the stock for editing; other status: read-only
        /// </summary>
        private void btnView_Click(object sender, EventArgs e)
        {
            SupplyOrderADO row = this.GetFocusedRow();
            if (row != null)
            {
                this.OpenCreateModule(row);
            }
        }

        private void OpenCreateModule(SupplyOrderADO row)
        {
            try
            {
                List<object> listArgs = new List<object>();
                if (row != null)
                {
                    V_HIS_SUPPLY_ORDER order = new V_HIS_SUPPLY_ORDER();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SUPPLY_ORDER>(order, row);
                    listArgs.Add(order);
                }
                else if (this.cboStock.EditValue != null)
                {
                    listArgs.Add(Convert.ToInt64(this.cboStock.EditValue));
                }
                listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)this.FillDataToGrid);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SupplyConstant.MODULE_LINK__SUPPLY_ORDER_CREATE, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancelOrder_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                SupplyOrderADO row = this.GetFocusedRow();
                if (row == null || !this.btnCancelOrder.Enabled)
                {
                    return;
                }
                string reason;
                using (frmCancelReason frm = new frmCancelReason())
                {
                    if (frm.ShowDialog(this.ParentForm) != DialogResult.OK)
                    {
                        return;
                    }
                    reason = frm.Reason;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceLanguageManager.Get("UCSupplyOrderList.Message.ConfirmCancel"), row.SUPPLY_ORDER_CODE),
                    ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                HisSupplyOrderCancelSDO sdo = new HisSupplyOrderCancelSDO();
                sdo.OrderId = row.ID;
                sdo.ModifyTime = row.MODIFY_TIME ?? 0;
                sdo.CancelReason = reason;
                WaitingManager.Show();
                HisSupplyOrderResultSDO result = new BackendAdapter(param).Post<HisSupplyOrderResultSDO>(RequestUriStore.HIS_SUPPLY_ORDER__CANCEL, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                bool success = result != null && result.SupplyOrder != null;
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDeleteDraft_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                SupplyOrderADO row = this.GetFocusedRow();
                if (row == null || !this.btnDeleteDraft.Enabled)
                {
                    return;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceLanguageManager.Get("UCSupplyOrderList.Message.ConfirmDelete"), row.SUPPLY_ORDER_CODE),
                    ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                HisSupplyOrderVersionSDO sdo = new HisSupplyOrderVersionSDO();
                sdo.OrderId = row.ID;
                sdo.ModifyTime = row.MODIFY_TIME ?? 0;
                WaitingManager.Show();
                bool success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_SUPPLY_ORDER__DELETE, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Send / resend: same order, MOS keeps the latest result on the order (history in the event log)
        /// </summary>
        private void btnSendMail_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                SupplyOrderADO row = this.GetFocusedRow();
                if (row == null || !this.btnSendMail.Enabled)
                {
                    return;
                }
                List<OrderDetailADO> details;
                if (!this.dicDetail.TryGetValue(row.ID, out details))
                {
                    details = new List<OrderDetailADO>();
                }
                HisSupplyOrderSendMailSDO sdo;
                using (frmSendMail frm = new frmSendMail(row, details))
                {
                    if (frm.ShowDialog(this.ParentForm) != DialogResult.OK)
                    {
                        return;
                    }
                    sdo = frm.Result;
                }
                WaitingManager.Show();
                HisSupplyOrderResultSDO result = new BackendAdapter(param).Post<HisSupplyOrderResultSDO>(RequestUriStore.HIS_SUPPLY_ORDER__SEND_MAIL, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                bool success = result != null && result.SupplyOrder != null && result.SupplyOrder.MAIL_STT != SupplyConstant.MAIL_STT__ERROR;
                if (result != null && result.SupplyOrder != null && result.SupplyOrder.MAIL_STT == SupplyConstant.MAIL_STT__ERROR)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceLanguageManager.Get("UCSupplyOrderList.Message.MailError"), result.SupplyOrder.LAST_MAIL_ERROR),
                        ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageManager.Show(this.ParentForm, param, success);
                }
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (result != null)
                {
                    this.FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Search()
        {
            if (this.btnSearch.Enabled)
            {
                this.FillDataToGrid();
            }
        }
    }
}
