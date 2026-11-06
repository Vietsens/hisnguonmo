using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.ADO;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.Base;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.Resources;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate
{
    /// <summary>
    /// Supply anticipate: one form, modes Edit (creator), PharmacyConfirm, Approve (accountant), View
    /// </summary>
    public partial class frmSupplyAnticipateCreate : FormBase
    {
        internal enum FormMode
        {
            Edit,
            PharmacyConfirm,
            Approve,
            View
        }

        private const short IS_TRUE = 1;

        private Inventec.Desktop.Common.Modules.Module moduleData;
        private long? anticipateId;
        private HIS.Desktop.Common.DelegateRefreshData refreshData;

        private V_HIS_ANTICIPATE anticipate;
        private List<SupplyLineADO> lines = new List<SupplyLineADO>();
        private List<V_HIS_MEDI_STOCK> supplyStocks = new List<V_HIS_MEDI_STOCK>();
        private Dictionary<string, ItemTypeADO> dicItemType = new Dictionary<string, ItemTypeADO>();
        private List<HIS_SUPPLIER> suppliers = new List<HIS_SUPPLIER>();
        private FormMode mode = FormMode.View;
        private string loginName;
        private bool isDirty = false;
        private bool isBinding = false;

        public frmSupplyAnticipateCreate()
        {
            InitializeComponent();
        }

        public frmSupplyAnticipateCreate(Inventec.Desktop.Common.Modules.Module moduleData, long? anticipateId, HIS.Desktop.Common.DelegateRefreshData refreshData)
            : base(moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = moduleData;
                this.anticipateId = anticipateId;
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

        private void frmSupplyAnticipateCreate_Load(object sender, EventArgs e)
        {
            try
            {
                this.SetIcon();
                this.SetCaptionByLanguageKey();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.InitStockCombo();
                this.InitItemCombo();
                this.InitGrid();
                if (this.anticipateId.HasValue)
                {
                    this.LoadAnticipate();
                }
                else
                {
                    this.BindNew();
                }
                if (this.supplyStocks.Count == 0 && this.anticipate == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NotConfig"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                this.lblCode.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblCode.Text");
                this.lblStock.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblStock.Text");
                this.lblPeriod.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblPeriod.Text");
                this.lblStatus.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblStatus.Text");
                this.lblCreator.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblCreator.Text");
                this.lblDescription.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblDescription.Text");
                this.lblReturnReason.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblReturnReason.Text");
                this.lblItem.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblItem.Text");
                this.lblItemHint.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.lblItemHint.Text");
                this.btnReload.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnReload.Text");
                this.btnSave.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnSave.Text");
                this.btnSubmit.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnSubmit.Text");
                this.btnPharmacyConfirm.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnPharmacyConfirm.Text");
                this.btnApprove.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnApprove.Text");
                this.btnReturn.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnReturn.Text");
                this.btnClose.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnClose.Text");
                this.btnReload.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnReload.ToolTip");
                this.btnSave.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnSave.ToolTip");
                this.btnSubmit.ToolTip = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.btnSubmit.ToolTip");
                if (this.moduleData == null || String.IsNullOrWhiteSpace(this.moduleData.text))
                {
                    this.Text = ResourceLanguageManager.Get("frmSupplyAnticipateCreate.Text");
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

        private short CurrentStt()
        {
            return this.anticipate != null ? (this.anticipate.SUPPLY_STT ?? 0) : (short)0;
        }

        private static string GetStatusText(short? supplyStt)
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
                    return ResourceLanguageManager.Get("Status.New");
            }
        }

        private FormMode DetermineMode()
        {
            if (this.anticipate == null)
            {
                return FormMode.Edit;
            }
            short stt = this.CurrentStt();
            if ((stt == SupplyConstant.STT__DRAFT || stt == SupplyConstant.STT__RETURNED)
                && String.Equals(this.anticipate.REQUEST_LOGINNAME, this.loginName, StringComparison.OrdinalIgnoreCase))
            {
                return FormMode.Edit;
            }
            if (stt == SupplyConstant.STT__WAIT_PHARMACY && this.HasControl(ControlCode.PHARMACY_CONFIRM))
            {
                return FormMode.PharmacyConfirm;
            }
            if (stt == SupplyConstant.STT__WAIT_ACCOUNTANT && this.HasControl(ControlCode.APPROVE))
            {
                return FormMode.Approve;
            }
            return FormMode.View;
        }

        /// <summary>
        /// Enable/visible state of editors, columns and buttons by mode
        /// </summary>
        private void ApplyMode()
        {
            try
            {
                this.mode = this.DetermineMode();
                bool edit = this.mode == FormMode.Edit;
                bool approve = this.mode == FormMode.Approve;
                short stt = this.CurrentStt();
                bool useApproval = approve || stt == SupplyConstant.STT__APPROVED;
                foreach (SupplyLineADO line in this.lines)
                {
                    line.UseApprovalAmount = useApproval;
                }

                this.cboStock.Properties.ReadOnly = !edit;
                this.dtPeriod.Properties.ReadOnly = !edit;
                this.txtDescription.Properties.ReadOnly = !edit;
                this.panelAdd.Visible = edit;

                this.colAmount.OptionsColumn.AllowEdit = edit;
                this.colSupplier.OptionsColumn.AllowEdit = edit;
                this.colNote.OptionsColumn.AllowEdit = edit;
                this.colDelete.Visible = edit;
                if (edit)
                {
                    this.colDelete.VisibleIndex = 0;
                }
                this.colApprovalAmount.OptionsColumn.AllowEdit = approve;
                this.colApprovalAmount.Visible = approve || stt == SupplyConstant.STT__APPROVED || stt == SupplyConstant.STT__WAIT_ACCOUNTANT;
                if (this.colApprovalAmount.Visible)
                {
                    this.colApprovalAmount.VisibleIndex = this.colAmount.VisibleIndex + 1;
                }
                this.gridViewLine.OptionsBehavior.Editable = edit || approve;

                this.btnSave.Visible = edit;
                this.btnSubmit.Visible = edit;
                this.btnPharmacyConfirm.Visible = this.mode == FormMode.PharmacyConfirm;
                this.btnApprove.Visible = approve;
                this.btnReturn.Visible = approve;
                this.btnReload.Visible = this.anticipate != null;
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
            DevExpress.XtraEditors.SimpleButton[] buttons = new DevExpress.XtraEditors.SimpleButton[] { this.btnClose, this.btnReturn, this.btnApprove, this.btnPharmacyConfirm, this.btnSubmit, this.btnSave, this.btnReload };
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
                this.AddPopupColumn(popup, "TYPE_CODE", "frmSupplyAnticipateCreate.Column.TypeCode", 100, 0);
                this.AddPopupColumn(popup, "TYPE_NAME", "frmSupplyAnticipateCreate.Column.TypeName", 320, 1);
                this.AddPopupColumn(popup, "SERVICE_UNIT_NAME", "frmSupplyAnticipateCreate.Column.ServiceUnit", 80, 2);
                this.AddPopupColumn(popup, "PACKING", "frmSupplyAnticipateCreate.Column.Packing", 160, 3);
                this.AddPopupColumn(popup, "KIND_NAME", "frmSupplyAnticipateCreate.Column.Kind", 80, 4);
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

        private void BindNew()
        {
            try
            {
                this.isBinding = true;
                this.anticipate = null;
                this.txtCode.Text = "";
                this.txtStatus.Text = GetStatusText(null);
                this.txtCreator.Text = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                this.txtReturnReason.Text = "";
                this.txtDescription.Text = "";
                this.cboStock.EditValue = this.supplyStocks.Count == 1 ? (object)this.supplyStocks[0].ID : null;
                DateTime next = DateTime.Today.AddMonths(1);
                this.dtPeriod.DateTime = new DateTime(next.Year, next.Month, 1);
                this.lines = new List<SupplyLineADO>();
                this.isDirty = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                this.isBinding = false;
            }
            this.ApplyMode();
        }

        private void BindHeader()
        {
            try
            {
                this.isBinding = true;
                V_HIS_ANTICIPATE data = this.anticipate;
                if (data.RECEIVE_MEDI_STOCK_ID.HasValue && !this.supplyStocks.Any(o => o.ID == data.RECEIVE_MEDI_STOCK_ID.Value))
                {
                    V_HIS_MEDI_STOCK stock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ID == data.RECEIVE_MEDI_STOCK_ID.Value);
                    if (stock != null)
                    {
                        List<V_HIS_MEDI_STOCK> ds = new List<V_HIS_MEDI_STOCK>(this.supplyStocks);
                        ds.Add(stock);
                        this.cboStock.Properties.DataSource = ds;
                    }
                }
                this.txtCode.Text = data.ANTICIPATE_CODE;
                this.txtStatus.Text = GetStatusText(data.SUPPLY_STT);
                this.txtStatus.ForeColor = data.SUPPLY_STT == SupplyConstant.STT__RETURNED ? Color.Red : Color.Black;
                this.txtCreator.Text = data.REQUEST_USERNAME;
                this.txtDescription.Text = data.DESCRIPTION;
                this.txtReturnReason.Text = data.SUPPLY_STT == SupplyConstant.STT__RETURNED ? data.RETURN_REASON : "";
                this.cboStock.EditValue = data.RECEIVE_MEDI_STOCK_ID;
                if (data.SUPPLY_PERIOD.HasValue)
                {
                    DateTime? period = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(data.SUPPLY_PERIOD.Value);
                    if (period.HasValue)
                    {
                        this.dtPeriod.DateTime = period.Value;
                    }
                }
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

        private long? GetSelectedStockId()
        {
            return this.cboStock.EditValue != null ? (long?)Convert.ToInt64(this.cboStock.EditValue) : null;
        }

        private long? GetSelectedPeriod()
        {
            return this.dtPeriod.EditValue != null ? (long?)SupplyConstant.DateToPeriod(this.dtPeriod.DateTime) : null;
        }

        private void frmSupplyAnticipateCreate_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (this.isDirty && this.mode == FormMode.Edit && this.lines.Count > 0)
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
