/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * KSK refund by service of a paid bill (Chi tiet thanh toan -> "Hoan ung DV").
 * Applies only when HIS config MOS.HIS_TRANSACTION.REPAY_BY_BILL_SERVICE.PATIENT_TYPE_CODES has a value and the bill
 * is a not cancelled TT, not an adjustment bill, not a sale bill, has TREATMENT_ID and its patient type
 * (V_HIS_TRANSACTION.PATIENT_TYPE_ID, else HIS_TREATMENT.TDL_PATIENT_TYPE_ID) is in the config.
 *   - Tree: check boxes (default unchecked), columns "Trang thai thuc hien" + "Tien da hoan";
 *     only services that are not executed / not refunded / not medicine-material-blood / price unchanged can be ticked.
 *   - Button "Hoan ung DV" (left of "Luu hinh thuc thanh toan"): opens HIS.Desktop.Plugins.TransactionRepay with
 *     TransactionRepayADO { BillId, SereServBillIds, AutoAmount } -> reload tree + refunds -> ask to tick "Khong thuc hien".
 * Otherwise (config empty / bill not applicable) the form behaves exactly as before.
 */
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.UC.SereServTree;
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

namespace HIS.Desktop.Plugins.TransactionBillDetail
{
    public partial class frmTransactionBillDetail
    {
        /// <summary>
        /// Reason why a service (leaf of the tree) cannot be ticked for refund by service.
        /// </summary>
        private enum RepayByServiceInvalidReason
        {
            /// <summary>Service can be ticked</summary>
            None = 0,
            /// <summary>HIS_SERE_SERV_BILL.PRICE &lt;= 0: no check box, ticking is ignored silently</summary>
            NoPrice = 1,
            /// <summary>Service already executed</summary>
            Executed = 2,
            /// <summary>Service already refunded (HIS_SESE_BILL_REPAY not cancelled)</summary>
            Refunded = 3,
            /// <summary>Medicine, material or blood</summary>
            MedicineMaterialBlood = 4,
            /// <summary>HIS_SERE_SERV_BILL.PRICE differs from VIR_TOTAL_PATIENT_PRICE</summary>
            PriceChanged = 5
        }

        #region Declare RepayByService
        const string REPAY_BY_SERVICE__CONFIG_KEY = "MOS.HIS_TRANSACTION.REPAY_BY_BILL_SERVICE.PATIENT_TYPE_CODES";
        const string REPAY_BY_SERVICE__MODULE_LINK_TRANSACTION_REPAY = "HIS.Desktop.Plugins.TransactionRepay";
        const string REPAY_BY_SERVICE__COLUMN_EXECUTE_STATUS = "REPAY_BY_SERVICE__EXECUTE_STATUS";
        const string REPAY_BY_SERVICE__COLUMN_REFUNDED_AMOUNT = "REPAY_BY_SERVICE__REFUNDED_AMOUNT";

        bool isRepayByService = false;
        V_HIS_TRANSACTION currentBill = null;
        Dictionary<long, List<V_HIS_SESE_BILL_REPAY>> dicSeseBillRepay = new Dictionary<long, List<V_HIS_SESE_BILL_REPAY>>();
        DevExpress.XtraEditors.SimpleButton btnRepayByService;
        DevExpress.Utils.ToolTipController toolTipRepayByService;
        string repayByServiceDisabledReason = null;
        bool isRepayByServiceHintShown = false;
        string repayByServiceTextExecuted = "";
        string repayByServiceTextNotExecuted = "";
        #endregion

        #region Applicability
        /// <summary>Decide whether the bill uses refund by service. Called in the constructor BEFORE InitSereServTree (the UC reads IsShowCheckNode at creation).</summary>
        private void CheckRepayByServiceApplicable()
        {
            try
            {
                this.isRepayByService = false;
                if (this.billId <= 0)
                    return;

                string configValue = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(REPAY_BY_SERVICE__CONFIG_KEY);
                if (String.IsNullOrWhiteSpace(configValue))
                    return;

                List<string> patientTypeCodes = configValue.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.Trim())
                    .Where(o => !String.IsNullOrEmpty(o))
                    .ToList();
                if (patientTypeCodes.Count == 0)
                    return;

                var patientTypes = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                List<long> patientTypeIds = patientTypes != null
                    ? patientTypes.Where(o => patientTypeCodes.Contains(o.PATIENT_TYPE_CODE)).Select(o => o.ID).ToList()
                    : new List<long>();
                if (patientTypeIds.Count == 0)
                    return;

                HisTransactionViewFilter billFilter = new HisTransactionViewFilter();
                billFilter.ID = this.billId;
                var bills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, billFilter, null);
                V_HIS_TRANSACTION bill = bills != null ? bills.FirstOrDefault() : null;
                if (bill == null)
                    return;
                this.currentBill = bill;

                if (bill.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT
                    || bill.IS_CANCEL == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    || bill.IS_ADJUSTMENT == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                    || bill.SALE_TYPE_ID.HasValue
                    || !bill.TREATMENT_ID.HasValue)
                    return;

                long? patientTypeId = bill.PATIENT_TYPE_ID;
                if (!patientTypeId.HasValue)
                {
                    HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.ID = bill.TREATMENT_ID.Value;
                    var treatments = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, null);
                    HIS_TREATMENT treatment = treatments != null ? treatments.FirstOrDefault() : null;
                    if (treatment != null)
                        patientTypeId = treatment.TDL_PATIENT_TYPE_ID;
                }

                this.isRepayByService = patientTypeId.HasValue && patientTypeIds.Contains(patientTypeId.Value);
                Inventec.Common.Logging.LogSystem.Debug("[BillDetail] RepayByService billId=" + this.billId + ", patientTypeId=" + patientTypeId + ", isRepayByService=" + this.isRepayByService);
            }
            catch (Exception ex)
            {
                this.isRepayByService = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Load not cancelled refunds (V_HIS_SESE_BILL_REPAY) of the bill, grouped by TDL_SERE_SERV_ID.</summary>
        private void LoadSeseBillRepays()
        {
            try
            {
                this.dicSeseBillRepay = new Dictionary<long, List<V_HIS_SESE_BILL_REPAY>>();
                HisSeseBillRepayViewFilter filter = new HisSeseBillRepayViewFilter();
                filter.TDL_BILL_ID = this.billId;
                filter.IS_CANCEL = false;
                var repays = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SESE_BILL_REPAY>>("api/HisSeseBillRepay/GetView", ApiConsumers.MosConsumer, filter, null);
                if (repays != null)
                {
                    foreach (var item in repays)
                    {
                        if (item.IS_CANCEL == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            continue;
                        if (!this.dicSeseBillRepay.ContainsKey(item.TDL_SERE_SERV_ID))
                            this.dicSeseBillRepay[item.TDL_SERE_SERV_ID] = new List<V_HIS_SESE_BILL_REPAY>();
                        this.dicSeseBillRepay[item.TDL_SERE_SERV_ID].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Tree
        /// <summary>Add the 2 unbound columns "Trang thai thuc hien" and "Tien da hoan" at visibleIndex, visibleIndex + 1.</summary>
        private void AddRepayByServiceColumns(List<SereServTreeColumn> columns, int visibleIndex)
        {
            try
            {
                if (columns == null)
                    return;
                this.repayByServiceTextExecuted = GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__EXECUTE_STATUS__EXECUTED");
                this.repayByServiceTextNotExecuted = GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__EXECUTE_STATUS__NOT_EXECUTED");

                SereServTreeColumn executeStatusCol = new SereServTreeColumn(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_EXECUTE_STATUS"), REPAY_BY_SERVICE__COLUMN_EXECUTE_STATUS, 110, false);
                executeStatusCol.VisibleIndex = visibleIndex;
                executeStatusCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
                columns.Add(executeStatusCol);

                SereServTreeColumn refundedAmountCol = new SereServTreeColumn(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__TREE_SERE_SERV__COLUMN_REFUNDED_AMOUNT"), REPAY_BY_SERVICE__COLUMN_REFUNDED_AMOUNT, 110, false);
                refundedAmountCol.VisibleIndex = visibleIndex + 1;
                refundedAmountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Decimal;
                refundedAmountCol.Format = new DevExpress.Utils.FormatInfo();
                refundedAmountCol.Format.FormatString = "#,##0.0000";
                refundedAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                columns.Add(refundedAmountCol);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_RepayByService_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (data == null || e == null || e.Column == null || !e.IsGetData)
                    return;

                if (e.Column.FieldName == REPAY_BY_SERVICE__COLUMN_EXECUTE_STATUS)
                {
                    // Leaf (service) only
                    if (data.IsLeaf == true)
                        e.Value = IsSereServExecuted(data) ? this.repayByServiceTextExecuted : this.repayByServiceTextNotExecuted;
                    else
                        e.Value = "";
                }
                else if (e.Column.FieldName == REPAY_BY_SERVICE__COLUMN_REFUNDED_AMOUNT)
                {
                    // Leaf: sum AMOUNT of its refunds; group: sum of its children
                    if (data.IsLeaf == true)
                        e.Value = GetRefundedAmount(data.ID);
                    else
                        e.Value = GetRefundedAmountOfChildren(e.Node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Services with HIS_SERE_SERV_BILL.PRICE &lt;= 0 have no check box.</summary>
        private void treeSereServ_RepayByService_CustomDrawNodeCheckBox(SereServADO data, DevExpress.XtraTreeList.CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (data != null && e != null && data.IsLeaf == true && GetSereServBillPrice(data.ID) <= 0)
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// The plugin applies every check state itself (e.CanCheck = false) so that only valid services can be ticked;
        /// the UC's AfterCheck (group caption rewrite) is therefore never raised.
        /// </summary>
        private void treeSereServ_RepayByService_BeforeCheck(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node == null || e == null || node.TreeList == null)
                    return;
                e.CanCheck = false;

                TreeList treeList = node.TreeList;
                List<TreeListNode> leafNodes = new List<TreeListNode>();
                CollectRepayByServiceLeafNodes(node, leafNodes);

                bool isCheck = (e.PrevState != CheckState.Checked);
                if (isCheck)
                {
                    List<TreeListNode> validNodes = new List<TreeListNode>();
                    List<KeyValuePair<SereServADO, RepayByServiceInvalidReason>> invalids = new List<KeyValuePair<SereServADO, RepayByServiceInvalidReason>>();
                    foreach (var leafNode in leafNodes)
                    {
                        var leafData = treeList.GetDataRecordByNode(leafNode) as SereServADO;
                        if (leafData == null)
                            continue;
                        RepayByServiceInvalidReason reason = GetRepayByServiceInvalidReason(leafData);
                        if (reason == RepayByServiceInvalidReason.None)
                            validNodes.Add(leafNode);
                        else if (reason != RepayByServiceInvalidReason.NoPrice)
                            invalids.Add(new KeyValuePair<SereServADO, RepayByServiceInvalidReason>(leafData, reason));
                    }

                    if (node.HasChildren && validNodes.Count > 0 && validNodes.All(o => o.Checked))
                    {
                        // Group stays indeterminate because of invalid services: a click on it unticks all
                        foreach (var leafNode in leafNodes)
                            leafNode.CheckState = CheckState.Unchecked;
                    }
                    else
                    {
                        foreach (var leafNode in validNodes)
                            leafNode.CheckState = CheckState.Checked;

                        string message = BuildRepayByServiceInvalidMessage(invalids);
                        if (!String.IsNullOrEmpty(message))
                        {
                            // Show after the check event has finished (no modal box inside the tree mouse handling)
                            this.BeginInvoke(new MethodInvoker(delegate() { ShowRepayByServiceMessage(message); }));
                        }
                    }
                }
                else
                {
                    foreach (var leafNode in leafNodes)
                        leafNode.CheckState = CheckState.Unchecked;
                }

                foreach (TreeListNode rootNode in treeList.Nodes)
                {
                    RefreshRepayByServiceNodeCheckState(rootNode);
                }
                UpdateRepayByServiceButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CollectRepayByServiceLeafNodes(TreeListNode node, List<TreeListNode> result)
        {
            if (node == null)
                return;
            if (!node.HasChildren)
            {
                var data = node.TreeList.GetDataRecordByNode(node) as SereServADO;
                if (data != null && data.IsLeaf == true)
                    result.Add(node);
                return;
            }
            foreach (TreeListNode child in node.Nodes)
            {
                CollectRepayByServiceLeafNodes(child, result);
            }
        }

        /// <summary>Group check state follows its children: all checked -> Checked, some -> Indeterminate, none -> Unchecked.</summary>
        private CheckState RefreshRepayByServiceNodeCheckState(TreeListNode node)
        {
            if (node == null)
                return CheckState.Unchecked;
            if (!node.HasChildren)
                return node.CheckState;

            bool isAnyChecked = false;
            bool isAllChecked = true;
            foreach (TreeListNode child in node.Nodes)
            {
                CheckState childState = RefreshRepayByServiceNodeCheckState(child);
                if (childState != CheckState.Unchecked)
                    isAnyChecked = true;
                if (childState != CheckState.Checked)
                    isAllChecked = false;
            }
            CheckState newState = isAllChecked ? CheckState.Checked : (isAnyChecked ? CheckState.Indeterminate : CheckState.Unchecked);
            if (node.CheckState != newState)
                node.CheckState = newState;
            return newState;
        }

        private string BuildRepayByServiceInvalidMessage(List<KeyValuePair<SereServADO, RepayByServiceInvalidReason>> invalids)
        {
            List<string> lines = new List<string>();
            try
            {
                if (invalids == null || invalids.Count == 0)
                    return "";

                List<string> executedNames = invalids.Where(o => o.Value == RepayByServiceInvalidReason.Executed).Select(o => o.Key.TDL_SERVICE_NAME).Distinct().ToList();
                if (executedNames.Count > 0)
                    lines.Add(String.Format(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__SERVICE_EXECUTED"), String.Join(", ", executedNames)));

                // {1} = REPAY_CODE of the refund(s); one line per refund transaction
                var refundedGroups = invalids.Where(o => o.Value == RepayByServiceInvalidReason.Refunded)
                    .Select(o => o.Key)
                    .GroupBy(o => String.Join(", ", GetSeseBillRepays(o.ID).Select(r => r.REPAY_CODE).Distinct()))
                    .ToList();
                foreach (var group in refundedGroups)
                {
                    lines.Add(String.Format(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__SERVICE_REFUNDED"), String.Join(", ", group.Select(o => o.TDL_SERVICE_NAME).Distinct()), group.Key));
                }

                List<string> medicineMaterialBloodNames = invalids.Where(o => o.Value == RepayByServiceInvalidReason.MedicineMaterialBlood).Select(o => o.Key.TDL_SERVICE_NAME).Distinct().ToList();
                if (medicineMaterialBloodNames.Count > 0)
                    lines.Add(String.Format(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__SERVICE_MEDICINE_MATERIAL_BLOOD"), String.Join(", ", medicineMaterialBloodNames)));

                List<string> priceChangedNames = invalids.Where(o => o.Value == RepayByServiceInvalidReason.PriceChanged).Select(o => o.Key.TDL_SERVICE_NAME).Distinct().ToList();
                if (priceChangedNames.Count > 0)
                    lines.Add(String.Format(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__SERVICE_PRICE_CHANGED"), String.Join(", ", priceChangedNames)));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return String.Join(Environment.NewLine, lines);
        }

        /// <summary>Ticked services that can be refunded (GetListCheck filtered by validity).</summary>
        private List<SereServADO> GetRepayByServiceCheckedSereServs()
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (this.ssTreeProcessor == null || this.ucSereServTree == null)
                    return result;
                var checkedRows = this.ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (checkedRows != null)
                {
                    result = checkedRows
                        .Where(o => o != null && o.IsLeaf == true && GetRepayByServiceInvalidReason(o) == RepayByServiceInvalidReason.None)
                        .GroupBy(o => o.ID)
                        .Select(g => g.First())
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<SereServADO>();
            }
            return result;
        }
        #endregion

        #region Service data
        private RepayByServiceInvalidReason GetRepayByServiceInvalidReason(SereServADO data)
        {
            if (data == null || data.IsLeaf != true)
                return RepayByServiceInvalidReason.NoPrice;
            decimal sereServBillPrice = GetSereServBillPrice(data.ID);
            if (sereServBillPrice <= 0)
                return RepayByServiceInvalidReason.NoPrice;
            if (IsSereServExecuted(data))
                return RepayByServiceInvalidReason.Executed;
            if (GetSeseBillRepays(data.ID).Count > 0)
                return RepayByServiceInvalidReason.Refunded;
            if (data.MEDICINE_ID.HasValue || data.MATERIAL_ID.HasValue || data.BLOOD_ID.HasValue)
                return RepayByServiceInvalidReason.MedicineMaterialBlood;
            if (sereServBillPrice != (data.VIR_TOTAL_PATIENT_PRICE ?? 0))
                return RepayByServiceInvalidReason.PriceChanged;
            return RepayByServiceInvalidReason.None;
        }

        /// <summary>"Da thuc hien": request finished, or in progress and not confirmed "no execute".</summary>
        private bool IsSereServExecuted(SereServADO data)
        {
            if (data == null)
                return false;
            return data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT
                || (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL
                    && data.IS_CONFIRM_NO_EXCUTE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
        }

        /// <summary>Not cancelled HIS_SERE_SERV_BILL rows of the service in this bill.</summary>
        private List<HIS_SERE_SERV_BILL> GetActiveSereServBills(long sereServId)
        {
            List<HIS_SERE_SERV_BILL> result = new List<HIS_SERE_SERV_BILL>();
            List<HIS_SERE_SERV_BILL> sereServBills = null;
            if (this.dicSereServBill != null && this.dicSereServBill.TryGetValue(sereServId, out sereServBills) && sereServBills != null)
            {
                result = sereServBills.Where(o => o.IS_CANCEL != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
            }
            return result;
        }

        private decimal GetSereServBillPrice(long sereServId)
        {
            return GetActiveSereServBills(sereServId).Sum(o => o.PRICE);
        }

        private List<V_HIS_SESE_BILL_REPAY> GetSeseBillRepays(long sereServId)
        {
            List<V_HIS_SESE_BILL_REPAY> result = null;
            if (this.dicSeseBillRepay != null && this.dicSeseBillRepay.TryGetValue(sereServId, out result) && result != null)
                return result;
            return new List<V_HIS_SESE_BILL_REPAY>();
        }

        private decimal GetRefundedAmount(long sereServId)
        {
            return GetSeseBillRepays(sereServId).Sum(o => o.AMOUNT);
        }

        private decimal GetRefundedAmountOfChildren(TreeListNode node)
        {
            decimal result = 0;
            if (node == null || node.Nodes == null || node.TreeList == null)
                return result;
            foreach (TreeListNode child in node.Nodes)
            {
                if (child.HasChildren)
                {
                    result += GetRefundedAmountOfChildren(child);
                }
                else
                {
                    var childData = node.TreeList.GetDataRecordByNode(child) as SereServADO;
                    if (childData != null && childData.IsLeaf == true)
                        result += GetRefundedAmount(childData.ID);
                }
            }
            return result;
        }
        #endregion

        #region Button "Hoan ung DV"
        /// <summary>Create the button left of "Luu hinh thuc thanh toan" (or alone on the bottom row when the payform UC is not created). Called after InitPayformGrid.</summary>
        private void InitRepayByServiceButton()
        {
            try
            {
                if (!this.isRepayByService)
                    return;

                this.btnRepayByService = new DevExpress.XtraEditors.SimpleButton();
                this.btnRepayByService.Name = "btnRepayByService";
                this.btnRepayByService.Text = GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__BTN_REPAY_BY_SERVICE");
                this.btnRepayByService.Enabled = false;
                this.btnRepayByService.Click += btnRepayByService_Click;

                this.layoutControl1.BeginUpdate();
                try
                {
                    this.layoutControl1.Controls.Add(this.btnRepayByService);
                    var lciBtnRepay = new LayoutControlItem();
                    lciBtnRepay.Name = "lciBtnRepayByService";
                    lciBtnRepay.Control = this.btnRepayByService;
                    lciBtnRepay.TextVisible = false;
                    lciBtnRepay.SizeConstraintsType = SizeConstraintsType.Custom;
                    lciBtnRepay.MinSize = new System.Drawing.Size(150, 30);
                    lciBtnRepay.MaxSize = new System.Drawing.Size(150, 30);
                    if (this.emptyLeftOfSavePayform != null)
                    {
                        // [empty][Hoan ung DV][Luu hinh thuc thanh toan]
                        this.layoutControlGroup1.AddItem(lciBtnRepay, this.emptyLeftOfSavePayform, InsertType.Right);
                    }
                    else
                    {
                        // Payform UC not created: [empty][Hoan ung DV] below the tree
                        this.layoutControlGroup1.AddItem(lciBtnRepay);
                        var emptyLeft = new EmptySpaceItem();
                        emptyLeft.Name = "emptyLeftOfRepayByService";
                        this.layoutControlGroup1.AddItem(emptyLeft, lciBtnRepay, InsertType.Left);
                    }
                }
                finally
                {
                    this.layoutControl1.EndUpdate();
                }

                // A disabled button raises no mouse event: the reason is shown from the parent layout control
                this.toolTipRepayByService = new DevExpress.Utils.ToolTipController();
                this.layoutControl1.MouseMove += layoutControl1_RepayByService_MouseMove;
                this.layoutControl1.MouseLeave += layoutControl1_RepayByService_MouseLeave;
                this.Disposed += frmTransactionBillDetail_RepayByService_Disposed;

                RefreshRepayByServiceDisabledReason();
                UpdateRepayByServiceButtonState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Reasons (joined by new line) why the button is locked; empty when refund is allowed.</summary>
        private void RefreshRepayByServiceDisabledReason()
        {
            try
            {
                List<string> reasons = new List<string>();
                if (this.currentBill != null)
                {
                    if (!String.IsNullOrWhiteSpace(this.currentBill.INVOICE_CODE))
                        reasons.Add(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__REPAY_BY_SERVICE_DISABLED__E_INVOICE"));
                    if (this.currentBill.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                        reasons.Add(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__REPAY_BY_SERVICE_DISABLED__LOCKED"));
                    if ((this.currentBill.EXEMPTION ?? 0) > 0 || (this.currentBill.TDL_BILL_FUND_AMOUNT ?? 0) > 0)
                        reasons.Add(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__REPAY_BY_SERVICE_DISABLED__EXEMPTION_OR_FUND"));
                }
                if (GetTransactionRepayModule() == null || GetWorkingCashierRoom() == null)
                    reasons.Add(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__REPAY_BY_SERVICE_DISABLED__NO_PERMISSION"));
                this.repayByServiceDisabledReason = String.Join(Environment.NewLine, reasons);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Enabled only when no lock reason and at least 1 valid service is ticked.</summary>
        private void UpdateRepayByServiceButtonState()
        {
            try
            {
                if (this.btnRepayByService == null)
                    return;
                this.btnRepayByService.Enabled = String.IsNullOrEmpty(this.repayByServiceDisabledReason)
                    && GetRepayByServiceCheckedSereServs().Count > 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private Inventec.Desktop.Common.Modules.Module GetTransactionRepayModule()
        {
            var modules = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws;
            return modules != null ? modules.FirstOrDefault(o => o.ModuleLink == REPAY_BY_SERVICE__MODULE_LINK_TRANSACTION_REPAY) : null;
        }

        private V_HIS_CASHIER_ROOM GetWorkingCashierRoom()
        {
            if (this.currentModule == null)
                return null;
            var cashierRooms = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
            return cashierRooms != null ? cashierRooms.FirstOrDefault(o => o.ROOM_ID == this.currentModule.RoomId) : null;
        }

        private void layoutControl1_RepayByService_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (this.btnRepayByService == null || this.toolTipRepayByService == null)
                    return;
                bool isShowHint = !this.btnRepayByService.Enabled
                    && this.btnRepayByService.Visible
                    && !String.IsNullOrEmpty(this.repayByServiceDisabledReason)
                    && this.btnRepayByService.Bounds.Contains(e.Location);
                if (isShowHint && !this.isRepayByServiceHintShown)
                {
                    this.toolTipRepayByService.ShowHint(this.repayByServiceDisabledReason, this.layoutControl1.PointToScreen(new Point(e.X, e.Y + 20)));
                    this.isRepayByServiceHintShown = true;
                }
                else if (!isShowHint && this.isRepayByServiceHintShown)
                {
                    this.toolTipRepayByService.HideHint();
                    this.isRepayByServiceHintShown = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void layoutControl1_RepayByService_MouseLeave(object sender, EventArgs e)
        {
            try
            {
                if (this.toolTipRepayByService != null && this.isRepayByServiceHintShown)
                {
                    this.toolTipRepayByService.HideHint();
                    this.isRepayByServiceHintShown = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBillDetail_RepayByService_Disposed(object sender, EventArgs e)
        {
            try
            {
                if (this.toolTipRepayByService != null)
                {
                    this.toolTipRepayByService.Dispose();
                    this.toolTipRepayByService = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRepayByService_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.btnRepayByService == null || !this.btnRepayByService.Enabled
                    || this.currentBill == null || !this.currentBill.TREATMENT_ID.HasValue || this.currentModule == null)
                    return;

                List<SereServADO> checkedSereServs = GetRepayByServiceCheckedSereServs();
                if (checkedSereServs.Count == 0)
                    return;

                Inventec.Desktop.Common.Modules.Module moduleRaw = GetTransactionRepayModule();
                V_HIS_CASHIER_ROOM cashierRoom = GetWorkingCashierRoom();
                if (moduleRaw == null || cashierRoom == null || !moduleRaw.IsPlugin || moduleRaw.ExtensionInfo == null)
                {
                    ShowRepayByServiceMessage(GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__REPAY_BY_SERVICE_DISABLED__NO_PERMISSION"));
                    return;
                }

                List<long> sereServBillIds = new List<long>();
                decimal autoAmount = 0;
                foreach (var sereServ in checkedSereServs)
                {
                    var sereServBills = GetActiveSereServBills(sereServ.ID);
                    sereServBillIds.AddRange(sereServBills.Select(o => o.ID));
                    autoAmount += sereServBills.Sum(o => o.PRICE);
                }

                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleRaw, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                HIS.Desktop.ADO.TransactionRepayADO ado = new HIS.Desktop.ADO.TransactionRepayADO(this.currentBill.TREATMENT_ID.Value, cashierRoom.ID);
                ado.AutoAmount = autoAmount;
                // Key = BillId, Value = SereServBillIds; TransactionRepay nhan qua tham so, khong them truong vao TransactionRepayADO dung chung
                KeyValuePair<long, List<long>> billService = new KeyValuePair<long, List<long>>(this.billId, sereServBillIds.Distinct().ToList());
                Inventec.Common.Logging.LogSystem.Debug("[BillDetail] RepayByService open TransactionRepay " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ado), ado) + Inventec.Common.Logging.LogUtil.TraceData("SereServBillIds", billService.Value));

                List<object> listArgs = new List<object>();
                listArgs.Add(ado);
                listArgs.Add(moduleData);
                listArgs.Add(billService);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleData, listArgs);
                if (extenceInstance == null)
                {
                    throw new ArgumentNullException("extenceInstance is null");
                }
                ((Form)extenceInstance).ShowDialog();

                ReloadAfterRepayByService(checkedSereServs.Select(o => o.ID).ToList());
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>After the "Hoan ung" dialog: reload tree + refunds (ticks cleared); if the ticked services now have refunds, offer to tick "Khong thuc hien".</summary>
        private void ReloadAfterRepayByService(List<long> selectedSereServIds)
        {
            try
            {
                ReloadRepayByServiceData();

                List<long> refundedSereServIds = (selectedSereServIds ?? new List<long>())
                    .Where(o => GetSeseBillRepays(o).Count > 0)
                    .Distinct()
                    .ToList();
                if (refundedSereServIds.Count == 0)
                    return;

                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__CONFIRM_NO_EXECUTE"),
                    GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__TITLE"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    UpdateNoExecuteForRefundedServices(refundedSereServIds);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Reload bill, sere serv bills, refunds and the tree (new nodes => all unticked).</summary>
        private void ReloadRepayByServiceData()
        {
            try
            {
                WaitingManager.Show();
                // dicSereServBill was only appended to: clear it so rows are not duplicated after reload
                this.dicSereServBill.Clear();
                FillDataToSereServTree();
                WaitingManager.Hide();
                RefreshRepayByServiceDisabledReason();
                UpdateRepayByServiceButtonState();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>POST api/HisSereServ/UpdatePayslipInfo (Field = IS_NO_EXECUTE, IS_NO_EXECUTE = 1) for the refunded services.</summary>
        private void UpdateNoExecuteForRefundedServices(List<long> sereServIds)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {
                if (sereServIds == null || sereServIds.Count == 0 || this.currentBill == null || !this.currentBill.TREATMENT_ID.HasValue)
                    return;

                WaitingManager.Show();
                HisSereServFilter sereServFilter = new HisSereServFilter();
                sereServFilter.IDs = sereServIds;
                var sereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, sereServFilter, param);
                if (sereServs != null && sereServs.Count > 0)
                {
                    foreach (var sereServ in sereServs)
                    {
                        sereServ.IS_NO_EXECUTE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    }
                    HisSereServPayslipSDO sdo = new HisSereServPayslipSDO();
                    sdo.TreatmentId = this.currentBill.TREATMENT_ID.Value;
                    sdo.Field = UpdateField.IS_NO_EXECUTE;
                    sdo.SereServs = sereServs;
                    Inventec.Common.Logging.LogSystem.Debug("[BillDetail] RepayByService UpdatePayslipInfo IS_NO_EXECUTE sereServIds=" + String.Join(",", sereServIds));
                    var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_SERE_SERV>>("api/HisSereServ/UpdatePayslipInfo", ApiConsumers.MosConsumer, sdo, param);
                    success = result != null && result.Count > 0;
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
                if (success)
                {
                    ReloadRepayByServiceData();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Common
        private string GetRepayByServiceLang(string key)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value(key, Base.ResourceLangManager.LanguageFrmTransactionBillDetail, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void ShowRepayByServiceMessage(string message)
        {
            try
            {
                if (String.IsNullOrEmpty(message))
                    return;
                DevExpress.XtraEditors.XtraMessageBox.Show(message,
                    GetRepayByServiceLang("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL_DETAIL__MESSAGE__TITLE"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}
