using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.ADO;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.Base;
using HIS.Desktop.Plugins.SupplyAnticipateCreate.Resources;
using HIS.Desktop.Utility;
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

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate
{
    public partial class frmSupplyAnticipateCreate
    {
        private void LoadAnticipate()
        {
            CommonParam param = new CommonParam();
            try
            {
                WaitingManager.Show();
                HisAnticipateViewFilter filter = new HisAnticipateViewFilter();
                filter.ID = this.anticipateId.Value;
                List<V_HIS_ANTICIPATE> data = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE>>(RequestUriStore.HIS_ANTICIPATE__GET_VIEW, ApiConsumers.MosConsumer, filter, param);
                V_HIS_ANTICIPATE anti = data != null ? data.FirstOrDefault() : null;
                List<V_HIS_ANTICIPATE_METY> metys = new List<V_HIS_ANTICIPATE_METY>();
                List<V_HIS_ANTICIPATE_MATY> matys = new List<V_HIS_ANTICIPATE_MATY>();
                if (anti != null)
                {
                    HisAnticipateMetyViewFilter metyFilter = new HisAnticipateMetyViewFilter();
                    metyFilter.ANTICIPATE_ID = anti.ID;
                    metys = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_METY>>(RequestUriStore.HIS_ANTICIPATE_METY__GET_VIEW, ApiConsumers.MosConsumer, metyFilter, param) ?? new List<V_HIS_ANTICIPATE_METY>();
                    HisAnticipateMatyViewFilter matyFilter = new HisAnticipateMatyViewFilter();
                    matyFilter.ANTICIPATE_ID = anti.ID;
                    matys = new BackendAdapter(param).Get<List<V_HIS_ANTICIPATE_MATY>>(RequestUriStore.HIS_ANTICIPATE_MATY__GET_VIEW, ApiConsumers.MosConsumer, matyFilter, param) ?? new List<V_HIS_ANTICIPATE_MATY>();
                }
                WaitingManager.Hide();
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (anti == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("SupplyAnticipateCreate: khong tim thay phieu ID=" + this.anticipateId);
                    this.BindNew();
                    return;
                }
                this.BindAnticipate(anti, metys, matys, true);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Bind header + lines; reference numbers already shown are kept by item key
        /// </summary>
        private void BindAnticipate(V_HIS_ANTICIPATE anti, List<V_HIS_ANTICIPATE_METY> metys, List<V_HIS_ANTICIPATE_MATY> matys, bool loadReference)
        {
            try
            {
                Dictionary<string, SupplyLineADO> oldLines = this.lines.GroupBy(o => o.KEY).ToDictionary(g => g.Key, g => g.First());
                this.anticipate = anti;
                this.anticipateId = anti.ID;
                this.BindHeader();

                List<SupplyLineADO> newLines = new List<SupplyLineADO>();
                foreach (V_HIS_ANTICIPATE_METY o in (metys ?? new List<V_HIS_ANTICIPATE_METY>()).OrderBy(o => o.ID))
                {
                    SupplyLineADO line = this.LineFromCatalog(false, o.MEDICINE_TYPE_ID, o.MEDICINE_TYPE_CODE, o.MEDICINE_TYPE_NAME, o.SERVICE_UNIT_NAME,
                        !String.IsNullOrWhiteSpace(o.PACKING_TYPE_NAME) ? o.PACKING_TYPE_NAME : o.CONCENTRA);
                    line.LINE_ID = o.ID;
                    line.SUPPLIER_ID = o.SUPPLIER_ID;
                    line.AMOUNT = o.AMOUNT;
                    line.APPROVAL_AMOUNT = o.APPROVAL_AMOUNT;
                    line.NOTE = o.NOTE;
                    if (o.IMP_PRICE.HasValue) line.IMP_PRICE = o.IMP_PRICE;
                    if (o.IMP_VAT_RATIO.HasValue) line.IMP_VAT_RATIO = o.IMP_VAT_RATIO;
                    newLines.Add(line);
                }
                foreach (V_HIS_ANTICIPATE_MATY o in (matys ?? new List<V_HIS_ANTICIPATE_MATY>()).OrderBy(o => o.ID))
                {
                    SupplyLineADO line = this.LineFromCatalog(true, o.MATERIAL_TYPE_ID, o.MATERIAL_TYPE_CODE, o.MATERIAL_TYPE_NAME, o.SERVICE_UNIT_NAME, o.PACKING_TYPE_NAME);
                    line.LINE_ID = o.ID;
                    line.SUPPLIER_ID = o.SUPPLIER_ID;
                    line.AMOUNT = o.AMOUNT;
                    line.APPROVAL_AMOUNT = o.APPROVAL_AMOUNT;
                    line.NOTE = o.NOTE;
                    if (o.IMP_PRICE.HasValue) line.IMP_PRICE = o.IMP_PRICE;
                    if (o.IMP_VAT_RATIO.HasValue) line.IMP_VAT_RATIO = o.IMP_VAT_RATIO;
                    newLines.Add(line);
                }
                foreach (SupplyLineADO line in newLines)
                {
                    SupplyLineADO old;
                    if (oldLines.TryGetValue(line.KEY, out old))
                    {
                        CopyReference(old, line);
                    }
                }
                this.lines = newLines;
                this.isDirty = false;
                this.ApplyMode();

                // Accountant: SL duyet defaults to SL xin
                if (this.mode == FormMode.Approve)
                {
                    foreach (SupplyLineADO line in this.lines.Where(o => !o.APPROVAL_AMOUNT.HasValue))
                    {
                        line.APPROVAL_AMOUNT = line.AMOUNT;
                    }
                    this.RefreshGrid();
                }
                if (loadReference)
                {
                    this.LoadReference(this.lines);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private SupplyLineADO LineFromCatalog(bool isMaterial, long typeId, string code, string name, string unit, string packing)
        {
            ItemTypeADO item;
            SupplyLineADO line;
            if (this.dicItemType.TryGetValue(ItemTypeADO.MakeKey(isMaterial, typeId), out item))
            {
                line = this.NewLine(item);
            }
            else
            {
                // item no longer active in catalog: keep data of the anticipate line
                line = new SupplyLineADO();
                line.KEY = ItemTypeADO.MakeKey(isMaterial, typeId);
                line.TYPE_ID = typeId;
                line.IS_MATERIAL = isMaterial;
                line.KIND_NAME = ResourceLanguageManager.Get(isMaterial ? "Kind.Material" : "Kind.Medicine");
                line.SUPPLIER_IDS = new List<long>();
            }
            line.TYPE_CODE = code;
            line.TYPE_NAME = name;
            line.SERVICE_UNIT_NAME = unit;
            if (!String.IsNullOrWhiteSpace(packing))
            {
                line.PACKING = packing;
            }
            return line;
        }

        private static void CopyReference(SupplyLineADO from, SupplyLineADO to)
        {
            to.TOTAL_IN_STOCK = from.TOTAL_IN_STOCK;
            to.STOCK_TIME_DISPLAY = from.STOCK_TIME_DISPLAY;
            to.EXP_MONTH_1 = from.EXP_MONTH_1;
            to.EXP_MONTH_2 = from.EXP_MONTH_2;
            to.EXP_MONTH_3 = from.EXP_MONTH_3;
            to.LAST_APPROVED_AMOUNT = from.LAST_APPROVED_AMOUNT;
            to.LAST_APPROVED_MONTH_DISPLAY = from.LAST_APPROVED_MONTH_DISPLAY;
            to.LAST_UNAPPROVED_AMOUNT = from.LAST_UNAPPROVED_AMOUNT;
        }

        /// <summary>
        /// Reference numbers (stock, 3-month export, last approved, unapproved part): display only, never copied into SL xin
        /// </summary>
        private void LoadReference(List<SupplyLineADO> targets)
        {
            CommonParam param = new CommonParam();
            try
            {
                long? stockId = this.GetSelectedStockId();
                long? period = this.GetSelectedPeriod();
                if (targets == null || targets.Count == 0)
                {
                    return;
                }
                if (!stockId.HasValue || !period.HasValue)
                {
                    foreach (SupplyLineADO line in targets)
                    {
                        CopyReference(new SupplyLineADO(), line);
                    }
                    this.gridControlLine.RefreshDataSource();
                    return;
                }
                HisAnticipateSupplyReferenceFilter filter = new HisAnticipateSupplyReferenceFilter();
                filter.MediStockId = stockId.Value;
                filter.SupplyPeriod = period.Value;
                filter.MedicineTypeIds = targets.Where(o => !o.IS_MATERIAL).Select(o => o.TYPE_ID).Distinct().ToList();
                filter.MaterialTypeIds = targets.Where(o => o.IS_MATERIAL).Select(o => o.TYPE_ID).Distinct().ToList();

                WaitingManager.Show();
                List<HisAnticipateSupplyReferenceSDO> refs = new BackendAdapter(param).Post<List<HisAnticipateSupplyReferenceSDO>>(RequestUriStore.HIS_ANTICIPATE__GET_SUPPLY_REFERENCE, ApiConsumers.MosConsumer, filter, param);
                WaitingManager.Hide();
                if (refs == null)
                {
                    MessageManager.Show(this, param, false);
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    return;
                }
                Dictionary<string, HisAnticipateSupplyReferenceSDO> dic = refs.GroupBy(o => ItemTypeADO.MakeKey(o.IsMaterial, o.TypeId)).ToDictionary(g => g.Key, g => g.First());
                DateTime? basePeriod = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(period.Value);
                foreach (SupplyLineADO line in targets)
                {
                    HisAnticipateSupplyReferenceSDO r;
                    if (!dic.TryGetValue(line.KEY, out r))
                    {
                        CopyReference(new SupplyLineADO(), line);
                        continue;
                    }
                    line.TOTAL_IN_STOCK = r.TotalInStock;
                    line.STOCK_TIME_DISPLAY = r.StockTime > 0 ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(r.StockTime) : "";
                    line.EXP_MONTH_1 = r.ExpMonth1;
                    line.EXP_MONTH_2 = r.ExpMonth2;
                    line.EXP_MONTH_3 = r.ExpMonth3;
                    line.LAST_APPROVED_AMOUNT = r.LastApprovedAmount;
                    line.LAST_APPROVED_MONTH_DISPLAY = MonthText(r.LastApprovedMonth, null, 0);
                    line.LAST_UNAPPROVED_AMOUNT = r.LastUnapprovedAmount;
                }
                HisAnticipateSupplyReferenceSDO first = refs.FirstOrDefault();
                if (first != null)
                {
                    this.SetExpMonthCaptions(first.Month1, first.Month2, first.Month3);
                }
                this.gridControlLine.RefreshDataSource();
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
            if (!this.GetSelectedPeriod().HasValue)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.PeriodRequired"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.dtPeriod.Focus();
                return false;
            }
            if (this.lines.Count == 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.NoLine"), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            List<SupplyLineADO> invalid = this.lines.Where(o => !o.AMOUNT.HasValue || o.AMOUNT.Value <= 0).ToList();
            if (invalid.Count > 0)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.AmountRequired"), String.Join(", ", invalid.Select(o => o.TYPE_CODE))), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.FocusLine(invalid[0]);
                this.gridViewLine.FocusedColumn = this.colAmount;
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
                if (this.mode != FormMode.Edit || !this.ValidateBeforeSave())
                {
                    return;
                }
                this.Save(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// SupplySave: create (no Id) or replace the draft / returned anticipate
        /// </summary>
        private bool Save(bool showSuccess)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                HisAnticipateSupplySaveSDO sdo = new HisAnticipateSupplySaveSDO();
                sdo.Id = this.anticipate != null ? (long?)this.anticipate.ID : null;
                sdo.ModifyTime = this.anticipate != null ? this.anticipate.MODIFY_TIME : null;
                sdo.MediStockId = this.GetSelectedStockId().Value;
                sdo.SupplyPeriod = this.GetSelectedPeriod().Value;
                sdo.Description = String.IsNullOrWhiteSpace(this.txtDescription.Text) ? null : this.txtDescription.Text.Trim();
                sdo.RequestRoomId = this.moduleData.RoomId;
                sdo.Medicines = this.lines.Where(o => !o.IS_MATERIAL).Select(o => new HisAnticipateSupplyMetySDO
                {
                    MedicineTypeId = o.TYPE_ID,
                    Amount = o.AMOUNT,
                    SupplierId = o.SUPPLIER_ID,
                    Note = o.NOTE
                }).ToList();
                sdo.Materials = this.lines.Where(o => o.IS_MATERIAL).Select(o => new HisAnticipateSupplyMatySDO
                {
                    MaterialTypeId = o.TYPE_ID,
                    Amount = o.AMOUNT,
                    SupplierId = o.SUPPLIER_ID,
                    Note = o.NOTE
                }).ToList();

                WaitingManager.Show();
                HisAnticipateSupplyResultSDO result = new BackendAdapter(param).Post<HisAnticipateSupplyResultSDO>(RequestUriStore.HIS_ANTICIPATE__SUPPLY_SAVE, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                success = result != null && result.Anticipate != null;
                if (!success || showSuccess)
                {
                    MessageManager.Show(this, param, success);
                }
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.BindAnticipate(result.Anticipate, result.Medicines, result.Materials, false);
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

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.gridViewLine.UpdateCurrentRow();
                if (this.mode != FormMode.Edit || !this.ValidateBeforeSave())
                {
                    return;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.ConfirmSubmit"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                if ((this.anticipate == null || this.isDirty) && !this.Save(false))
                {
                    return;
                }
                this.CallAction(RequestUriStore.HIS_ANTICIPATE__SUPPLY_SUBMIT, new HisAnticipateSupplyActionSDO { Id = this.anticipate.ID, ModifyTime = this.anticipate.MODIFY_TIME ?? 0 });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPharmacyConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.mode != FormMode.PharmacyConfirm)
                {
                    return;
                }
                if (DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.ConfirmPharmacy"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.CallAction(RequestUriStore.HIS_ANTICIPATE__SUPPLY_PHARMACY_CONFIRM, new HisAnticipateSupplyActionSDO { Id = this.anticipate.ID, ModifyTime = this.anticipate.MODIFY_TIME ?? 0 });
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<HisAnticipateSupplyApproveLineSDO> MakeApproveLines()
        {
            return this.lines.Where(o => o.LINE_ID.HasValue).Select(o => new HisAnticipateSupplyApproveLineSDO
            {
                IsMaterial = o.IS_MATERIAL,
                Id = o.LINE_ID.Value,
                ApprovalAmount = o.APPROVAL_AMOUNT
            }).ToList();
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.gridViewLine.UpdateCurrentRow();
                if (this.mode != FormMode.Approve)
                {
                    return;
                }
                string title = ResourceLanguageManager.Get("Message.Title");
                List<SupplyLineADO> invalid = this.lines.Where(o => !o.APPROVAL_AMOUNT.HasValue || o.APPROVAL_AMOUNT.Value < 0).ToList();
                if (invalid.Count > 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(this, string.Format(ResourceLanguageManager.Get("Message.ApprovalAmountRequired"), String.Join(", ", invalid.Select(o => o.TYPE_CODE))), title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.FocusLine(invalid[0]);
                    this.gridViewLine.FocusedColumn = this.colApprovalAmount;
                    return;
                }
                int zeroCount = this.lines.Count(o => o.APPROVAL_AMOUNT == 0);
                string confirm = zeroCount > 0
                    ? string.Format(ResourceLanguageManager.Get("Message.ConfirmApproveWithZero"), zeroCount)
                    : ResourceLanguageManager.Get("Message.ConfirmApprove");
                if (DevExpress.XtraEditors.XtraMessageBox.Show(this, confirm, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                HisAnticipateSupplyApproveSDO sdo = new HisAnticipateSupplyApproveSDO();
                sdo.Id = this.anticipate.ID;
                sdo.ModifyTime = this.anticipate.MODIFY_TIME ?? 0;
                sdo.Lines = this.MakeApproveLines();
                this.CallAction(RequestUriStore.HIS_ANTICIPATE__SUPPLY_APPROVE, sdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            try
            {
                this.gridViewLine.CloseEditor();
                this.gridViewLine.UpdateCurrentRow();
                if (this.mode != FormMode.Approve)
                {
                    return;
                }
                using (frmReturnReason frm = new frmReturnReason())
                {
                    if (frm.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }
                    HisAnticipateSupplyReturnSDO sdo = new HisAnticipateSupplyReturnSDO();
                    sdo.Id = this.anticipate.ID;
                    sdo.ModifyTime = this.anticipate.MODIFY_TIME ?? 0;
                    sdo.Reason = frm.Reason;
                    sdo.Lines = this.MakeApproveLines();
                    this.CallAction(RequestUriStore.HIS_ANTICIPATE__SUPPLY_RETURN, sdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// On failure the server message is shown and the numbers on screen are kept (Reload to get the latest version)
        /// </summary>
        private void CallAction(string uri, object sdo)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                WaitingManager.Show();
                HisAnticipateSupplyResultSDO result = new BackendAdapter(param).Post<HisAnticipateSupplyResultSDO>(uri, ApiConsumers.MosConsumer, sdo, param);
                WaitingManager.Hide();
                success = result != null && result.Anticipate != null;
                MessageManager.Show(this, param, success);
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    this.BindAnticipate(result.Anticipate, result.Medicines, result.Materials, false);
                    this.AfterChanged();
                }
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
                if (this.anticipate == null)
                {
                    return;
                }
                if (this.isDirty && DevExpress.XtraEditors.XtraMessageBox.Show(this, ResourceLanguageManager.Get("Message.ConfirmReload"), ResourceLanguageManager.Get("Message.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                this.lines = new List<SupplyLineADO>();
                this.LoadAnticipate();
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
