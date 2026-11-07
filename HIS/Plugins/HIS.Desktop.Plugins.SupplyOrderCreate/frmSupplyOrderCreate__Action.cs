using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SupplyOrderCreate.ADO;
using HIS.Desktop.Plugins.SupplyOrderCreate.Base;
using HIS.Desktop.Plugins.SupplyOrderCreate.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.SupplyOrderCreate
{
    public partial class frmSupplyOrderCreate
    {
        /// <summary>
        /// Draft orders of the stock (any creator) are shown together, one line per item
        /// </summary>
        private void LoadDrafts()
        {
            CommonParam param = new CommonParam();
            try
            {
                long? stockId = this.GetSelectedStockId();
                if (!stockId.HasValue)
                {
                    return;
                }
                WaitingManager.Show();
                HisSupplyOrderViewFilter filter = new HisSupplyOrderViewFilter();
                filter.MEDI_STOCK_ID = stockId.Value;
                filter.SUPPLY_ORDER_STT = SupplyConstant.ORDER_STT__DRAFT;
                List<V_HIS_SUPPLY_ORDER> orders = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>(RequestUriStore.HIS_SUPPLY_ORDER__GET_VIEW, ApiConsumers.MosConsumer, filter, param) ?? new List<V_HIS_SUPPLY_ORDER>();
                List<HisSupplyOrderDetailSDO> details = this.GetDetails(orders.Select(o => o.ID).ToList(), param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                this.BindDrafts(orders, details);
                this.LoadBalance(this.lines);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisSupplyOrderDetailSDO> GetDetails(List<long> orderIds, CommonParam param)
        {
            if (orderIds == null || orderIds.Count == 0)
            {
                return new List<HisSupplyOrderDetailSDO>();
            }
            HisSupplyOrderDetailFilter filter = new HisSupplyOrderDetailFilter();
            filter.SUPPLY_ORDER_IDs = orderIds;
            return new BackendAdapter(param).Get<List<HisSupplyOrderDetailSDO>>(RequestUriStore.HIS_SUPPLY_ORDER_DETAIL__GET, ApiConsumers.MosConsumer, filter, param) ?? new List<HisSupplyOrderDetailSDO>();
        }

        /// <summary>
        /// Balance numbers already shown are kept by item key (reloaded after save only when missing)
        /// </summary>
        private void BindDrafts(List<V_HIS_SUPPLY_ORDER> orders, List<HisSupplyOrderDetailSDO> details)
        {
            try
            {
                Dictionary<string, OrderLineADO> oldLines = this.lines.GroupBy(o => o.KEY).ToDictionary(g => g.Key, g => g.First());
                this.drafts = (orders ?? new List<V_HIS_SUPPLY_ORDER>()).Where(o => o.SUPPLY_ORDER_STT == SupplyConstant.ORDER_STT__DRAFT).OrderBy(o => o.ID).ToList();
                Dictionary<long, V_HIS_SUPPLY_ORDER> dicOrder = this.drafts.ToDictionary(o => o.ID);
                List<OrderLineADO> newLines = new List<OrderLineADO>();
                foreach (HisSupplyOrderDetailSDO d in (details ?? new List<HisSupplyOrderDetailSDO>()).OrderBy(o => o.SUPPLY_ORDER_ID).ThenBy(o => o.ID))
                {
                    V_HIS_SUPPLY_ORDER order;
                    if (!dicOrder.TryGetValue(d.SUPPLY_ORDER_ID, out order))
                    {
                        continue;
                    }
                    OrderLineADO line = this.LineFromDetail(d, order);
                    OrderLineADO old;
                    if (oldLines.TryGetValue(line.KEY, out old))
                    {
                        CopyBalance(old, line);
                    }
                    newLines.Add(line);
                }
                this.lines = newLines;
                this.isDirty = false;
                this.UpdateDraftLabel();
                this.ApplyMode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private OrderLineADO LineFromDetail(HisSupplyOrderDetailSDO d, V_HIS_SUPPLY_ORDER order)
        {
            ItemTypeADO item;
            OrderLineADO line;
            if (this.dicItemType.TryGetValue(ItemTypeADO.MakeKey(d.IS_MATERIAL, d.TYPE_ID), out item))
            {
                line = this.NewLine(item);
            }
            else
            {
                // item no longer active in catalog: keep data of the order line
                line = new OrderLineADO();
                line.KEY = ItemTypeADO.MakeKey(d.IS_MATERIAL, d.TYPE_ID);
                line.TYPE_ID = d.TYPE_ID;
                line.IS_MATERIAL = d.IS_MATERIAL;
                line.KIND_NAME = ResourceLanguageManager.Get(d.IS_MATERIAL ? "Kind.Material" : "Kind.Medicine");
                line.SUPPLIER_IDS = new List<long>();
            }
            line.TYPE_CODE = d.TYPE_CODE;
            line.TYPE_NAME = d.TYPE_NAME;
            line.SERVICE_UNIT_NAME = d.SERVICE_UNIT_NAME;
            if (!String.IsNullOrWhiteSpace(d.IMP_UNIT_NAME))
            {
                line.IMP_UNIT_NAME = d.IMP_UNIT_NAME;
            }
            string packing = !String.IsNullOrWhiteSpace(d.PACKING_TYPE_NAME) ? d.PACKING_TYPE_NAME : d.CONCENTRA;
            if (!String.IsNullOrWhiteSpace(packing))
            {
                line.PACKING = packing;
            }
            if (d.IMP_UNIT_CONVERT_RATIO.HasValue)
            {
                line.IMP_UNIT_CONVERT_RATIO = d.IMP_UNIT_CONVERT_RATIO;
            }
            line.SUPPLIER_ID = order.SUPPLIER_ID;
            line.SUPPLY_ORDER_CODE = order.SUPPLY_ORDER_CODE;
            line.AMOUNT = d.AMOUNT;
            line.IMP_UNIT_AMOUNT = d.IMP_UNIT_AMOUNT;
            if (d.IMP_PRICE.HasValue) line.IMP_PRICE = d.IMP_PRICE;
            if (d.IMP_VAT_RATIO.HasValue) line.IMP_VAT_RATIO = d.IMP_VAT_RATIO;
            line.ARRIVED_AMOUNT = d.ARRIVED_AMOUNT;
            line.IMPORTED_AMOUNT = d.IMPORTED_AMOUNT;
            return line;
        }

        private static void CopyBalance(OrderLineADO from, OrderLineADO to)
        {
            to.APPROVED_AMOUNT = from.APPROVED_AMOUNT;
            to.ORDERED_AMOUNT = from.ORDERED_AMOUNT;
            to.REMAIN_AMOUNT = from.REMAIN_AMOUNT;
            to.NEW_AMOUNT = from.NEW_AMOUNT;
        }

        /// <summary>
        /// Confirmed / canceled order: read-only with arrived / imported amounts
        /// </summary>
        private void LoadViewOrder()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisSupplyOrderViewFilter filter = new HisSupplyOrderViewFilter();
                filter.ID = this.viewOrder.ID;
                List<V_HIS_SUPPLY_ORDER> orders = new BackendAdapter(param).Get<List<V_HIS_SUPPLY_ORDER>>(RequestUriStore.HIS_SUPPLY_ORDER__GET_VIEW, ApiConsumers.MosConsumer, filter, param);
                V_HIS_SUPPLY_ORDER order = orders != null ? orders.FirstOrDefault() : null;
                if (order != null)
                {
                    this.viewOrder = order;
                }
                List<HisSupplyOrderDetailSDO> details = this.GetDetails(new List<long> { this.viewOrder.ID }, param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                this.lines = details.OrderBy(o => o.ID).Select(o => this.LineFromDetail(o, this.viewOrder)).ToList();
                this.isDirty = false;
                this.UpdateOrderInfo();
                this.ApplyMode();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// F/O/R/N of the stock, computed by MOS from the documents
        /// </summary>
        private void LoadBalance(List<OrderLineADO> targets)
        {
            CommonParam param = new CommonParam();
            try
            {
                long? stockId = this.GetSelectedStockId();
                if (!stockId.HasValue || targets == null || targets.Count == 0 || this.IsViewMode)
                {
                    return;
                }
                HisSupplyOrderBalanceFilter filter = new HisSupplyOrderBalanceFilter();
                filter.MediStockId = stockId.Value;
                filter.MedicineTypeIds = targets.Where(o => !o.IS_MATERIAL).Select(o => o.TYPE_ID).Distinct().ToList();
                filter.MaterialTypeIds = targets.Where(o => o.IS_MATERIAL).Select(o => o.TYPE_ID).Distinct().ToList();
                WaitingManager.Show();
                List<HisSupplyOrderBalanceSDO> balances = new BackendAdapter(param).Post<List<HisSupplyOrderBalanceSDO>>(RequestUriStore.HIS_SUPPLY_ORDER__GET_BALANCE, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                if (balances == null)
                {
                    MessageManager.Show(this, param, false);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    return;
                }
                Dictionary<string, HisSupplyOrderBalanceSDO> dic = balances.GroupBy(o => ItemTypeADO.MakeKey(o.IsMaterial, o.TypeId)).ToDictionary(g => g.Key, g => g.First());
                foreach (OrderLineADO line in targets)
                {
                    HisSupplyOrderBalanceSDO b;
                    if (dic.TryGetValue(line.KEY, out b))
                    {
                        SetBalance(line, b);
                    }
                    else
                    {
                        CopyBalance(new OrderLineADO(), line);
                    }
                }
                this.gridControlLine.RefreshDataSource();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void SetBalance(OrderLineADO line, HisSupplyOrderBalanceSDO b)
        {
            line.APPROVED_AMOUNT = b.ApprovedAmount;
            line.ORDERED_AMOUNT = b.OrderedAmount;
            line.REMAIN_AMOUNT = b.RemainAmount;
            line.NEW_AMOUNT = b.NewAmount;
        }

        /// <summary>
        /// Items of the stock with approved anticipate still to order (R > 0) and not on screen yet; amount left empty
        /// </summary>
        private void btnLoadRemain_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                long? stockId = this.GetSelectedStockId();
                if (!stockId.HasValue || this.IsViewMode)
                {
                    return;
                }
                HisSupplyOrderBalanceFilter filter = new HisSupplyOrderBalanceFilter();
                filter.MediStockId = stockId.Value;
                WaitingManager.Show();
                List<HisSupplyOrderBalanceSDO> balances = new BackendAdapter(param).Post<List<HisSupplyOrderBalanceSDO>>(RequestUriStore.HIS_SUPPLY_ORDER__GET_BALANCE, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                if (balances == null)
                {
                    MessageManager.Show(this, param, false);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    return;
                }
                int added = 0;
                foreach (HisSupplyOrderBalanceSDO b in balances.Where(o => o.RemainAmount > 0))
                {
                    string key = ItemTypeADO.MakeKey(b.IsMaterial, b.TypeId);
                    ItemTypeADO item;
                    if (this.lines.Any(o => o.KEY == key) || !this.dicItemType.TryGetValue(key, out item))
                    {
                        continue;
                    }
                    OrderLineADO line = this.NewLine(item);
                    SetBalance(line, b);
                    this.lines.Add(line);
                    added++;
                }
                if (added == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NoRemain"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                this.isDirty = true;
                this.RefreshGrid();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ValidateBeforeSave()
        {
            string title = ResourceLanguageManager.Get("Message.Title");
            if (!this.GetSelectedStockId().HasValue)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.StockRequired"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.cboStock.Focus();
                return false;
            }
            if (this.lines.Count == 0 && this.drafts.Count == 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NoLine"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            List<OrderLineADO> invalid = this.lines.Where(o => !o.AMOUNT.HasValue || o.AMOUNT.Value <= 0).ToList();
            if (invalid.Count > 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.AmountRequired"), String.Join(", ", invalid.Select(o => o.TYPE_CODE))), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.FocusLine(invalid[0]);
                this.gridViewLine.FocusedColumn = this.colAmount;
                return false;
            }
            invalid = this.lines.Where(o => !o.SUPPLIER_ID.HasValue).ToList();
            if (invalid.Count > 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.SupplierRequired"), String.Join(", ", invalid.Select(o => o.TYPE_CODE))), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.FocusLine(invalid[0]);
                this.gridViewLine.FocusedColumn = this.colSupplier;
                return false;
            }
            invalid = this.lines.Where(o => !o.IMP_PRICE.HasValue).ToList();
            if (invalid.Count > 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.PriceMissing"), String.Join(", ", invalid.Select(o => o.TYPE_CODE))), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.FocusLine(invalid[0]);
                return false;
            }
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.gridViewLine.UpdateCurrentRow();
                if (this.IsViewMode || !this.ValidateBeforeSave())
                {
                    return;
                }
                this.SaveDraft(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// SaveDraft replaces the draft orders of the stock by the lines on screen, split one order per supplier
        /// </summary>
        private bool SaveDraft(bool showSuccess)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HisSupplyOrderSaveDraftSDO sdo = new HisSupplyOrderSaveDraftSDO();
                sdo.MediStockId = this.GetSelectedStockId().Value;
                sdo.DraftOrderIds = this.drafts.Select(o => o.ID).ToList();
                sdo.Lines = this.lines.Select(o => new HisSupplyOrderLineSDO
                {
                    TypeId = o.TYPE_ID,
                    IsMaterial = o.IS_MATERIAL,
                    SupplierId = o.SUPPLIER_ID.Value,
                    Amount = o.AMOUNT.Value,
                    ImpUnitAmount = o.HasRatio ? o.IMP_UNIT_AMOUNT : null
                }).ToList();

                WaitingManager.Show();
                List<HisSupplyOrderResultSDO> result = new BackendAdapter(param).Post<List<HisSupplyOrderResultSDO>>(RequestUriStore.HIS_SUPPLY_ORDER__SAVE_DRAFT, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                success = result != null;
                if (!success || showSuccess)
                {
                    MessageManager.Show(this, param, success);
                }
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    List<V_HIS_SUPPLY_ORDER> orders = result.Where(o => o.SupplyOrder != null).Select(o => o.SupplyOrder).ToList();
                    List<HisSupplyOrderDetailSDO> details = result.Where(o => o.Details != null).SelectMany(o => o.Details).ToList();
                    this.BindDrafts(orders, details);
                    List<OrderLineADO> noBalance = this.lines.Where(o => !o.APPROVED_AMOUNT.HasValue).ToList();
                    if (noBalance.Count > 0)
                    {
                        this.LoadBalance(noBalance);
                    }
                    this.AfterChanged();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        /// <summary>
        /// Confirm all drafts of the stock. Over remain: MOS returns the warnings, the user must tick "read" to confirm again
        /// </summary>
        private void btnConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.gridViewLine.UpdateCurrentRow();
                if (this.IsViewMode || !this.HasControl(ControlCode.ORDER) || !this.ValidateBeforeSave())
                {
                    return;
                }
                if (this.lines.Count == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NoLine"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if ((this.isDirty || this.drafts.Count == 0 || this.lines.Any(o => String.IsNullOrEmpty(o.SUPPLY_ORDER_CODE))) && !this.SaveDraft(false))
                {
                    return;
                }
                if (this.drafts.Count == 0)
                {
                    return;
                }
                string confirm = string.Format(ResourceLanguageManager.Get("Message.ConfirmOrder"), this.drafts.Count, String.Join(", ", this.drafts.Select(o => o.SUPPLY_ORDER_CODE)));
                if (DevExpress.XtraEditors.XtraMessageBox.Show(this, confirm, ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.CallConfirm(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CallConfirm(bool overWarningConfirmed)
        {
            CommonParam param = new CommonParam();
            try
            {
                HisSupplyOrderConfirmSDO sdo = new HisSupplyOrderConfirmSDO();
                sdo.IsOverWarningConfirmed = overWarningConfirmed;
                sdo.Orders = this.drafts.Select(o => new HisSupplyOrderVersionSDO { OrderId = o.ID, ModifyTime = o.MODIFY_TIME ?? 0 }).ToList();
                WaitingManager.Show();
                HisSupplyOrderConfirmResultSDO result = new BackendAdapter(param).Post<HisSupplyOrderConfirmResultSDO>(RequestUriStore.HIS_SUPPLY_ORDER__CONFIRM, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (result == null)
                {
                    MessageManager.Show(this, param, false);
                    return;
                }
                bool confirmed = result.Orders != null && result.Orders.Count > 0;
                if (!confirmed && result.OverWarnings != null && result.OverWarnings.Count > 0)
                {
                    if (overWarningConfirmed)
                    {
                        MessageManager.Show(this, param, false);
                        return;
                    }
                    using (frmOverWarning frm = new frmOverWarning(result.OverWarnings))
                    {
                        if (frm.ShowDialog(this) == DialogResult.OK)
                        {
                            this.CallConfirm(true);
                        }
                    }
                    return;
                }
                MessageManager.Show(this, param, confirmed);
                if (!confirmed)
                {
                    return;
                }
                List<string> codes = result.Orders.Where(o => o.SupplyOrder != null).Select(o => o.SupplyOrder.SUPPLY_ORDER_CODE).ToList();
                DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.OrderConfirmed"), codes.Count, String.Join(", ", codes)), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.lines = new List<OrderLineADO>();
                this.isDirty = false;
                this.LoadDrafts();
                this.AfterChanged();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReload_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.IsViewMode)
                {
                    this.LoadViewOrder();
                    return;
                }
                if (!this.GetSelectedStockId().HasValue)
                {
                    return;
                }
                if (this.isDirty && DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.ConfirmReload"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.lines = new List<OrderLineADO>();
                this.LoadDrafts();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AfterChanged()
        {
            try
            {
                if (this.refreshData != null)
                {
                    this.refreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
