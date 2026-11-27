using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.UC.SereServTree;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CreateTransReqQR.CreateTransReqQR
{
    /// <summary>
    /// vCong 58478: when opened from the service assignment screen (HIS.Desktop.Plugins.AssignService)
    /// and the hospital turns on HIS.Desktop.Plugins.CreateTransReqQR.AllowSelectServiceFromAssignService,
    /// only the services of the latest save are pre-checked, the user may check/uncheck freely
    /// and the QR is created only when the user presses "Tạo".
    /// </summary>
    public partial class frmCreateTransReqQR
    {
        /// <summary>
        /// Sere_serv IDs of the latest save on the service assignment screen.
        /// Not null = the screen was opened from the service assignment screen (empty when nothing saved yet).
        /// </summary>
        List<long> assignServiceSereServIds;

        /// <summary>Called with the payment request just created, so the caller can print it on the service request sheet.</summary>
        Action<HIS_TRANS_REQ> actTransReqCreated;

        /// <summary>True when the "select services" behaviour applies to this opening of the form.</summary>
        bool isSelectServiceMode;

        public frmCreateTransReqQR(Inventec.Desktop.Common.Modules.Module currentModule, TransReqQRADO ado, List<long> assignServiceSereServIds, Action<HIS_TRANS_REQ> actTransReqCreated)
            : this(currentModule, ado)
        {
            try
            {
                this.assignServiceSereServIds = assignServiceSereServIds;
                this.actTransReqCreated = actTransReqCreated;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Must run after HisConfigCFG.LoadConfig().</summary>
        private void InitSelectServiceMode()
        {
            try
            {
                this.isSelectServiceMode = HisConfigCFG.AllowSelectServiceFromAssignService
                    && this.assignServiceSereServIds != null
                    && this.inputTransReq != null
                    && this.inputTransReq.TreatmentId > 0
                    && this.inputTransReq.TransReqId == CreateReqType.DepositService
                    && this.inputTransReq.Transaction == null
                    && (this.inputTransReq.Transactions == null || this.inputTransReq.Transactions.Count == 0);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.isSelectServiceMode), this.isSelectServiceMode)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.assignServiceSereServIds), this.assignServiceSereServIds));
            }
            catch (Exception ex)
            {
                this.isSelectServiceMode = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Check only the leaves of the latest save; parents follow (recursive check state).</summary>
        private void CheckAssignServiceNodes(TreeListNodes treeListNodes)
        {
            try
            {
                // free check/uncheck from now on (the old BeforeCheck handler forces CheckAll when false)
                this.IsCheckNode = true;
                if (treeListNodes == null || treeListNodes.Count == 0)
                {
                    CalCulateTotalAmountDeposit();
                    return;
                }
                HashSet<long> preCheckedIds = new HashSet<long>(this.assignServiceSereServIds ?? new List<long>());
                List<TreeListNode> leaves = new List<TreeListNode>();
                CollectLeafNodes(treeListNodes, leaves);
                foreach (var node in leaves)
                {
                    var data = node.TreeList.GetDataRecordByNode(node) as SereServADO;
                    bool isCheck = data != null && preCheckedIds.Contains(data.ID);
                    node.TreeList.SetNodeCheckState(node, isCheck ? CheckState.Checked : CheckState.Unchecked, true);
                }
                CalCulateTotalAmountDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CollectLeafNodes(TreeListNodes nodes, List<TreeListNode> leaves)
        {
            foreach (TreeListNode node in nodes)
            {
                if (node.HasChildren)
                    CollectLeafNodes(node.Nodes, leaves);
                else
                    leaves.Add(node);
            }
        }

        /// <summary>Replaces the automatic QR creation on load when the "select services" behaviour applies.</summary>
        private void WaitUserSelectService()
        {
            try
            {
                this.IsCheckNode = true;
                btnCreate.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void NotifyTransReqCreated()
        {
            try
            {
                if (this.isSelectServiceMode && this.actTransReqCreated != null && this.currentTransReq != null)
                    this.actTransReqCreated(this.currentTransReq);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
