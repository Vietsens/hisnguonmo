using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SupplyAnticipateList.ADO;
using HIS.Desktop.Plugins.SupplyAnticipateList.Base;
using HIS.Desktop.Plugins.SupplyAnticipateList.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyAnticipateList
{
    /// <summary>
    /// Supply anticipates (HIS_ANTICIPATE.SUPPLY_STT has value) of the medi stocks in MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES
    /// </summary>
    public partial class UCSupplyAnticipateList : UserControlBase
    {
        private const string FORMAT_MONEY = "#,##0.####";

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private List<V_HIS_MEDI_STOCK> supplyStocks = new List<V_HIS_MEDI_STOCK>();
        private string loginName;

        public UCSupplyAnticipateList()
        {
            InitializeComponent();
        }

        public UCSupplyAnticipateList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
        }

        private void UCSupplyAnticipateList_Load(object sender, EventArgs e)
        {
            try
            {
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.SetCaptionByLanguageKey();
                this.InitStockCombo();
                this.InitStatusCombo();
                this.InitGrid();
                this.dtPeriodFrom.DateTime = DateTime.Today.AddMonths(-6);
                this.dtPeriodTo.DateTime = DateTime.Today.AddMonths(6);
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
                this.lblStock.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblStock.Text");
                this.lblPeriod.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblPeriod.Text");
                this.lblPeriodTo.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblPeriodTo.Text");
                this.lblStatus.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblStatus.Text");
                this.lblKeyword.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblKeyword.Text");
                this.lblNotConfig.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.lblNotConfig.Text");
                this.btnSearch.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnSearch.Text");
                this.btnCreate.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnCreate.Text");
                this.btnOpen.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnOpen.Text");
                this.btnPharmacyConfirm.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnPharmacyConfirm.Text");
                this.btnApprove.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnApprove.Text");
                this.btnDelete.Text = ResourceLanguageManager.Get("UCSupplyAnticipateList.btnDelete.Text");
                this.txtKeyword.Properties.NullValuePrompt = ResourceLanguageManager.Get("UCSupplyAnticipateList.txtKeyword.NullValuePrompt");
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

        internal static string GetStatusText(short? supplyStt)
        {
            switch (supplyStt ?? 0)
            {
                case SupplyConstant.STT__DRAFT:
                    return ResourceLanguageManager.Get("Status.Draft");
                case SupplyConstant.STT__WAIT_PHARMACY:
                    return ResourceLanguageManager.Get("Status.WaitPharmacy");
                case SupplyConstant.STT__WAIT_ACCOUNTANT:
                    return ResourceLanguageManager.Get("Status.WaitAccountant");
                case SupplyConstant.STT__RETURNED:
                    return ResourceLanguageManager.Get("Status.Returned");
                case SupplyConstant.STT__APPROVED:
                    return ResourceLanguageManager.Get("Status.Approved");
                default:
                    return "";
            }
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
                this.cboStock.Properties.NullText = ResourceLanguageManager.Get("UCSupplyAnticipateList.cboStock.NullText");
                this.cboStock.Properties.Buttons.Add(new EditorButton(ButtonPredefines.Delete));
                this.cboStock.ButtonClick += (s, e) =>
                {
                    if (e.Button.Kind == ButtonPredefines.Delete)
                    {
                        this.cboStock.EditValue = null;
                    }
                };
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
                foreach (short stt in new short[] { SupplyConstant.STT__DRAFT, SupplyConstant.STT__WAIT_PHARMACY, SupplyConstant.STT__WAIT_ACCOUNTANT, SupplyConstant.STT__RETURNED, SupplyConstant.STT__APPROVED })
                {
                    this.cboStatus.Properties.Items.Add(stt, GetStatusText(stt), CheckState.Unchecked, true);
                }
                this.cboStatus.Properties.SeparatorChar = ',';
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitGrid()
        {
            try
            {
                GridView view = this.gridViewAnticipate;
                view.Columns.Clear();
                int i = 0;
                this.AddColumn(view, "STT", "UCSupplyAnticipateList.Column.Stt", 45, i++, null);
                this.AddColumn(view, "ANTICIPATE_CODE", "UCSupplyAnticipateList.Column.AnticipateCode", 100, i++, null);
                this.AddColumn(view, "RECEIVE_MEDI_STOCK_NAME", "UCSupplyAnticipateList.Column.MediStock", 180, i++, null);
                this.AddColumn(view, "SUPPLY_PERIOD_DISPLAY", "UCSupplyAnticipateList.Column.Period", 70, i++, null);
                this.AddColumn(view, "CREATE_TIME_DISPLAY", "UCSupplyAnticipateList.Column.CreateTime", 115, i++, null);
                this.AddColumn(view, "REQUEST_USERNAME", "UCSupplyAnticipateList.Column.RequestUser", 140, i++, null);
                this.AddColumn(view, "SUPPLY_STT_DISPLAY", "UCSupplyAnticipateList.Column.Status", 110, i++, null);
                this.AddColumn(view, "SUPPLY_APPROVE_TIME_DISPLAY", "UCSupplyAnticipateList.Column.ApproveTime", 115, i++, null);
                this.AddColumn(view, "SUPPLY_APPROVE_USERNAME", "UCSupplyAnticipateList.Column.ApproveUser", 140, i++, null);
                this.AddColumn(view, "SUPPLY_TOTAL_PRICE", "UCSupplyAnticipateList.Column.TotalPrice", 120, i++, FORMAT_MONEY);
                this.AddColumn(view, "DESCRIPTION", "UCSupplyAnticipateList.Column.Description", 200, i++, null);
                this.AddColumn(view, "RETURN_REASON", "UCSupplyAnticipateList.Column.ReturnReason", 200, i++, null);
                view.OptionsView.ColumnAutoWidth = false;
                view.OptionsBehavior.Editable = false;
                view.OptionsView.ShowGroupPanel = false;
                view.OptionsView.ShowIndicator = false;
                view.FocusedRowChanged += (s, e) => this.ApplyButtonState();
                view.DoubleClick += this.gridViewAnticipate_DoubleClick;
                view.RowCellStyle += this.gridViewAnticipate_RowCellStyle;
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

        private void gridViewAnticipate_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "SUPPLY_STT_DISPLAY")
                {
                    return;
                }
                SupplyAnticipateADO row = this.gridViewAnticipate.GetRow(e.RowHandle) as SupplyAnticipateADO;
                if (row != null && row.SUPPLY_STT == SupplyConstant.STT__RETURNED)
                {
                    e.Appearance.ForeColor = Color.Red;
                }
                else if (row != null && row.SUPPLY_STT == SupplyConstant.STT__APPROVED)
                {
                    e.Appearance.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private SupplyAnticipateADO GetFocusedRow()
        {
            return this.gridViewAnticipate.GetFocusedRow() as SupplyAnticipateADO;
        }

        private void ApplyButtonState()
        {
            try
            {
                SupplyAnticipateADO row = this.GetFocusedRow();
                short stt = row != null ? (row.SUPPLY_STT ?? 0) : (short)0;
                this.btnOpen.Enabled = row != null;
                this.btnPharmacyConfirm.Enabled = stt == SupplyConstant.STT__WAIT_PHARMACY && this.HasControl(ControlCode.PHARMACY_CONFIRM);
                this.btnApprove.Enabled = stt == SupplyConstant.STT__WAIT_ACCOUNTANT && this.HasControl(ControlCode.APPROVE);
                this.btnDelete.Enabled = stt == SupplyConstant.STT__DRAFT && row.SUBMIT_TIME == null
                    && String.Equals(row.REQUEST_LOGINNAME, this.loginName, StringComparison.OrdinalIgnoreCase);
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
                HisAnticipateViewFilter filter = new HisAnticipateViewFilter();
                filter.IS_SUPPLY = true;
                filter.ORDER_FIELD = "CREATE_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (this.cboStock.EditValue != null)
                {
                    filter.RECEIVE_MEDI_STOCK_ID = Convert.ToInt64(this.cboStock.EditValue);
                }
                else
                {
                    filter.RECEIVE_MEDI_STOCK_IDs = this.supplyStocks.Select(o => o.ID).ToList();
                }
                if (this.dtPeriodFrom.EditValue != null)
                {
                    filter.SUPPLY_PERIOD_FROM = SupplyConstant.DateToPeriod(this.dtPeriodFrom.DateTime);
                }
                if (this.dtPeriodTo.EditValue != null)
                {
                    filter.SUPPLY_PERIOD_TO = SupplyConstant.DateToPeriod(this.dtPeriodTo.DateTime);
                }
                List<short> stts = new List<short>();
                foreach (CheckedListBoxItem item in this.cboStatus.Properties.Items)
                {
                    if (item.CheckState == CheckState.Checked)
                    {
                        stts.Add((short)item.Value);
                    }
                }
                if (stts.Count > 0)
                {
                    filter.SUPPLY_STTs = stts;
                }
                if (!String.IsNullOrWhiteSpace(this.txtKeyword.Text))
                {
                    filter.KEY_WORD = this.txtKeyword.Text.Trim();
                }

                WaitingManager.Show();
                List<V_HIS_ANTICIPATE> data = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE>>(RequestUriStore.HIS_ANTICIPATE__GET_VIEW, ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_ANTICIPATE>();
                List<SupplyAnticipateADO> rows = new List<SupplyAnticipateADO>();
                int stt = 1;
                foreach (V_HIS_ANTICIPATE item in data.Where(o => o.SUPPLY_STT.HasValue))
                {
                    SupplyAnticipateADO ado = new SupplyAnticipateADO(item);
                    ado.STT = stt++;
                    ado.SUPPLY_PERIOD_DISPLAY = SupplyConstant.PeriodToString(item.SUPPLY_PERIOD);
                    ado.CREATE_TIME_DISPLAY = item.CREATE_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.CREATE_TIME.Value) : "";
                    ado.SUPPLY_STT_DISPLAY = GetStatusText(item.SUPPLY_STT);
                    ado.SUPPLY_APPROVE_TIME_DISPLAY = item.SUPPLY_APPROVE_TIME.HasValue ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(item.SUPPLY_APPROVE_TIME.Value) : "";
                    rows.Add(ado);
                }
                this.gridControlAnticipate.DataSource = rows;
                WaitingManager.Hide();
                this.ApplyButtonState();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.FillDataToGrid();
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.FillDataToGrid();
            }
        }

        private void gridViewAnticipate_DoubleClick(object sender, EventArgs e)
        {
            DevExpress.Utils.DXMouseEventArgs ea = e as DevExpress.Utils.DXMouseEventArgs;
            if (ea != null)
            {
                GridHitInfo hit = this.gridViewAnticipate.CalcHitInfo(ea.Location);
                if (!hit.InRow)
                {
                    return;
                }
            }
            this.btnOpen_Click(null, null);
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            this.OpenCreateModule(null);
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            SupplyAnticipateADO row = this.GetFocusedRow();
            if (row != null)
            {
                this.OpenCreateModule(row.ID);
            }
        }

        private void btnPharmacyConfirm_Click(object sender, EventArgs e)
        {
            this.btnOpen_Click(null, null);
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            this.btnOpen_Click(null, null);
        }

        /// <summary>
        /// The create module chooses its mode (edit / pharmacy confirm / approve / view) by status and permission
        /// </summary>
        private void OpenCreateModule(long? anticipateId)
        {
            try
            {
                List<object> listArgs = new List<object>();
                if (anticipateId.HasValue)
                {
                    listArgs.Add(anticipateId.Value);
                }
                listArgs.Add((HIS.Desktop.Common.DelegateRefreshData)this.FillDataToGrid);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(SupplyConstant.MODULE_LINK__SUPPLY_ANTICIPATE_CREATE, this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                SupplyAnticipateADO row = this.GetFocusedRow();
                if (row == null || !this.btnDelete.Enabled)
                {
                    return;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(string.Format(ResourceLanguageManager.Get("UCSupplyAnticipateList.Message.ConfirmDelete"), row.ANTICIPATE_CODE),
                    ResourceLanguageManager.Get("UCSupplyAnticipateList.Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                HIS_ANTICIPATE data = new HIS_ANTICIPATE();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_ANTICIPATE>(data, row);
                WaitingManager.Show();
                success = new BackendAdapter(param).Post<bool>(RequestUriStore.HIS_ANTICIPATE__DELETE, ApiConsumers.MosConsumer, data, param);
                WaitingManager.Hide();
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
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

        public void Search()
        {
            if (this.btnSearch.Enabled)
            {
                this.FillDataToGrid();
            }
        }
    }
}
