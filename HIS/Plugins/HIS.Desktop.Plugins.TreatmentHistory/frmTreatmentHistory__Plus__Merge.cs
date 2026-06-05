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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TreatmentHistory.ADO;
using HIS.Desktop.Plugins.TreatmentHistory.Resources;
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

namespace HIS.Desktop.Plugins.TreatmentHistory
{
    /// <summary>
    /// Chế độ "Gộp kết quả KCB theo nhóm dịch vụ" (config HIS.TREATMENT_HISTORY.MERGE_BY_SERVICE_TYPE).
    /// Toàn bộ control + logic gộp nằm tại đây, không đụng Designer và không phá luồng hiện tại khi config TẮT.
    /// </summary>
    public partial class frmTreatmentHistory : HIS.Desktop.Utility.FormBase
    {
        #region Declare Merge

        private const string API_HIS_TREATMENT_GET_VIEW = "api/HisTreatment/GetView";

        // Control dựng runtime
        private CheckEdit chkMergeByServiceType;
        private PopupContainerEdit popupPeriod;
        private PopupContainerControl popupContainerPeriod;
        private LabelControl lblPopupPeriodTitle;
        private TreeList treePeriod;
        private SimpleButton btnSelectAllPeriod;
        private SimpleButton btnUnselectAllPeriod;
        private SimpleButton btnApplyPeriod;
        private SimpleButton btnClosePeriod;
        private LabelControl lblNoData;
        private LayoutControlItem lciMergeCheck;
        private LayoutControlItem lciPeriod;

        // Trạng thái
        private bool mergeFeatureEnabled;
        private bool isBuildingPeriodTree;
        private List<V_HIS_TREATMENT> patientTreatments = new List<V_HIS_TREATMENT>();
        private List<long> selectedMergeTreatmentIds = new List<long>();
        private Dictionary<string, List<DHisSereServ2>> mergeDetailCache = new Dictionary<string, List<DHisSereServ2>>();
        private Dictionary<TreeListColumn, Tuple<string, bool, int>> originalTreeColState;

        /// <summary>Đang ở chế độ gộp (config bật + checkbox tích).</summary>
        private bool IsMergeMode
        {
            get { return mergeFeatureEnabled && chkMergeByServiceType != null && chkMergeByServiceType.Checked; }
        }

        #endregion

        #region Init

        /// <summary>
        /// Khởi tạo control gộp. Gọi trong Load SAU InitControlState.
        /// Config TẮT => return ngay, màn hình hoạt động y nguyên hiện tại.
        /// </summary>
        private void InitMergeControls()
        {
            try
            {
                if (!Key.HisConfigCFG.IsMergeByServiceTypeEnabled) return;
                mergeFeatureEnabled = true;

                BuildMergeBarControls();
                BuildPeriodPopup();
                BuildNoDataLabel();
                SetCaptionMergeControls();

                // Khôi phục trạng thái checkbox đã lưu (chỉ lưu bool, không lưu danh sách đợt)
                RestoreMergeCheckState();

                // Nếu khôi phục là đang BẬT => set layout cột gộp + bật popup (cây dựng khi chọn đợt ở Grid 1)
                if (chkMergeByServiceType.Checked)
                {
                    popupPeriod.Enabled = true;
                    chkShowTabDetail.Enabled = false;
                    EnsureSimpleTreeActive();
                    ApplyMergeTreeColumns();
                }
                else
                {
                    popupPeriod.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BuildMergeBarControls()
        {
            try
            {
                this.chkMergeByServiceType = new CheckEdit();
                this.chkMergeByServiceType.Name = "chkMergeByServiceType";
                this.chkMergeByServiceType.Properties.Caption = "Gộp kết quả KCB";
                this.chkMergeByServiceType.StyleController = this.layoutControl1;
                this.chkMergeByServiceType.CheckedChanged += new EventHandler(this.chkMergeByServiceType_CheckedChanged);

                this.popupPeriod = new PopupContainerEdit();
                this.popupPeriod.Name = "popupPeriod";
                this.popupPeriod.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
                this.popupPeriod.Properties.NullValuePrompt = "Đợt điều trị cần gộp";
                this.popupPeriod.Properties.NullValuePromptShowForEmptyValue = true;
                this.popupPeriod.StyleController = this.layoutControl1;
                this.popupPeriod.Enabled = false;

                this.lciMergeCheck = new LayoutControlItem();
                this.lciMergeCheck.Control = this.chkMergeByServiceType;
                this.lciMergeCheck.Name = "lciMergeCheck";
                this.lciMergeCheck.TextVisible = false;
                this.lciMergeCheck.SizeConstraintsType = SizeConstraintsType.Custom;
                this.lciMergeCheck.MinSize = new Size(150, 24);
                this.lciMergeCheck.MaxSize = new Size(150, 24);

                this.lciPeriod = new LayoutControlItem();
                this.lciPeriod.Control = this.popupPeriod;
                this.lciPeriod.Name = "lciPeriod";
                this.lciPeriod.TextVisible = false;
                this.lciPeriod.SizeConstraintsType = SizeConstraintsType.Custom;
                this.lciPeriod.MinSize = new Size(190, 24);
                this.lciPeriod.MaxSize = new Size(190, 24);

                // Chèn ngay sau nút Tìm (layoutControlItem11): [Tìm][Gộp][Đợt][khoảng trống]
                // Neo vào item cố định để 2 control luôn nằm cạnh nhau, khoảng trống đẩy về mép phải.
                this.layoutControl1.BeginUpdate();
                this.layoutControlGroup1.AddItem(this.lciMergeCheck, this.layoutControlItem11, DevExpress.XtraLayout.Utils.InsertType.Right);
                this.layoutControlGroup1.AddItem(this.lciPeriod, this.lciMergeCheck, DevExpress.XtraLayout.Utils.InsertType.Right);
                this.layoutControl1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildPeriodPopup()
        {
            try
            {
                this.popupContainerPeriod = new PopupContainerControl();
                this.popupContainerPeriod.Name = "popupContainerPeriod";
                this.popupContainerPeriod.Size = new Size(440, 344);

                this.lblPopupPeriodTitle = new LabelControl();
                this.lblPopupPeriodTitle.Location = new Point(8, 8);
                this.lblPopupPeriodTitle.AutoSizeMode = LabelAutoSizeMode.Default;
                this.lblPopupPeriodTitle.Text = "Chọn đợt điều trị cần gộp";

                this.btnSelectAllPeriod = new SimpleButton();
                this.btnSelectAllPeriod.Text = "Chọn tất cả";
                this.btnSelectAllPeriod.Size = new Size(84, 22);
                this.btnSelectAllPeriod.Location = new Point(244, 6);
                this.btnSelectAllPeriod.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                this.btnSelectAllPeriod.Click += new EventHandler(this.btnSelectAllPeriod_Click);

                this.btnUnselectAllPeriod = new SimpleButton();
                this.btnUnselectAllPeriod.Text = "Bỏ chọn tất cả";
                this.btnUnselectAllPeriod.Size = new Size(104, 22);
                this.btnUnselectAllPeriod.Location = new Point(330, 6);
                this.btnUnselectAllPeriod.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                this.btnUnselectAllPeriod.Click += new EventHandler(this.btnUnselectAllPeriod_Click);

                this.treePeriod = new TreeList();
                this.treePeriod.Location = new Point(6, 32);
                this.treePeriod.Size = new Size(428, 268);
                this.treePeriod.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                this.treePeriod.OptionsView.ShowColumns = false;
                this.treePeriod.OptionsView.ShowCheckBoxes = true;
                this.treePeriod.OptionsView.ShowIndicator = false;
                this.treePeriod.OptionsBehavior.AllowIndeterminateCheckState = true;
                this.treePeriod.OptionsBehavior.Editable = false;
                TreeListColumn colPeriod = new TreeListColumn();
                colPeriod.Caption = "Đợt điều trị";
                colPeriod.FieldName = "Display";
                colPeriod.VisibleIndex = 0;
                colPeriod.OptionsColumn.AllowEdit = false;
                this.treePeriod.Columns.Add(colPeriod);
                this.treePeriod.AfterCheckNode += new NodeEventHandler(this.treePeriod_AfterCheckNode);

                this.btnApplyPeriod = new SimpleButton();
                this.btnApplyPeriod.Text = "Áp dụng";
                this.btnApplyPeriod.Size = new Size(78, 26);
                this.btnApplyPeriod.Location = new Point(270, 308);
                this.btnApplyPeriod.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                this.btnApplyPeriod.Click += new EventHandler(this.btnApplyPeriod_Click);

                this.btnClosePeriod = new SimpleButton();
                this.btnClosePeriod.Text = "Đóng";
                this.btnClosePeriod.Size = new Size(78, 26);
                this.btnClosePeriod.Location = new Point(352, 308);
                this.btnClosePeriod.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                this.btnClosePeriod.Click += new EventHandler(this.btnClosePeriod_Click);

                this.popupContainerPeriod.Controls.Add(this.lblPopupPeriodTitle);
                this.popupContainerPeriod.Controls.Add(this.btnSelectAllPeriod);
                this.popupContainerPeriod.Controls.Add(this.btnUnselectAllPeriod);
                this.popupContainerPeriod.Controls.Add(this.treePeriod);
                this.popupContainerPeriod.Controls.Add(this.btnApplyPeriod);
                this.popupContainerPeriod.Controls.Add(this.btnClosePeriod);

                this.popupContainerPeriod.Visible = false;
                this.Controls.Add(this.popupContainerPeriod);
                this.popupPeriod.Properties.PopupControl = this.popupContainerPeriod;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BuildNoDataLabel()
        {
            try
            {
                this.lblNoData = new LabelControl();
                this.lblNoData.Text = ResourceMessage.Merge__KhongCoDuLieuTrongKhoangThoiGian;
                this.lblNoData.Appearance.Options.UseTextOptions = true;
                this.lblNoData.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                this.lblNoData.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
                this.lblNoData.Appearance.ForeColor = Color.Gray;
                this.lblNoData.AutoSizeMode = LabelAutoSizeMode.None;
                this.lblNoData.Dock = DockStyle.Fill;
                this.lblNoData.Visible = false;
                this.tree_HisServiceReq2.Controls.Add(this.lblNoData);
                this.lblNoData.BringToFront();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionMergeControls()
        {
            try
            {
                if (Resources.ResourceLanguageManager.LanguageResource == null) return;
                this.chkMergeByServiceType.Properties.Caption = L("frmTreatmentHistory.chkMergeByServiceType.Text");
                this.popupPeriod.Properties.NullValuePrompt = L("frmTreatmentHistory.popupPeriod.NullValuePrompt");
                this.lblPopupPeriodTitle.Text = L("frmTreatmentHistory.popupPeriodTitle.Text");
                this.btnSelectAllPeriod.Text = L("frmTreatmentHistory.btnSelectAllPeriod.Text");
                this.btnUnselectAllPeriod.Text = L("frmTreatmentHistory.btnUnselectAllPeriod.Text");
                this.btnApplyPeriod.Text = L("frmTreatmentHistory.btnApplyPeriod.Text");
                this.btnClosePeriod.Text = L("frmTreatmentHistory.btnClosePeriod.Text");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string L(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return key;
        }

        #endregion

        #region ControlState (merge checkbox)

        private void RestoreMergeCheckState()
        {
            try
            {
                IsInitForm = true;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkMergeByServiceType.Name)
                        {
                            chkMergeByServiceType.Checked = item.VALUE == "1";
                        }
                    }
                }
                IsInitForm = false;
            }
            catch (Exception ex)
            {
                IsInitForm = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveMergeCheckState()
        {
            try
            {
                if (this.controlStateWorker == null) return;
                var item = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                    ? this.currentControlStateRDO.FirstOrDefault(o => o.KEY == chkMergeByServiceType.Name && o.MODULE_LINK == this.ModuleLink)
                    : null;

                if (item != null)
                {
                    item.VALUE = chkMergeByServiceType.Checked ? "1" : "";
                }
                else
                {
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(new HIS.Desktop.Library.CacheClient.ControlStateRDO()
                    {
                        KEY = chkMergeByServiceType.Name,
                        VALUE = chkMergeByServiceType.Checked ? "1" : "",
                        MODULE_LINK = this.ModuleLink
                    });
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Events

        private void chkMergeByServiceType_CheckedChanged(object sender, EventArgs e)
        {
            if (IsInitForm) return;
            try
            {
                SaveMergeCheckState();

                if (chkMergeByServiceType.Checked)
                {
                    if (rowCellClick == null || rowCellClick.ID <= 0)
                    {
                        XtraMessageBox.Show(ResourceMessage.Merge__ChonDotDieuTriTruoc, ResourceMessage.ThongBao,
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        IsInitForm = true;
                        chkMergeByServiceType.Checked = false;
                        IsInitForm = false;
                        SaveMergeCheckState();
                        return;
                    }

                    popupPeriod.Enabled = true;
                    chkShowTabDetail.Enabled = false;
                    EnsureSimpleTreeActive();
                    ApplyMergeTreeColumns();
                    BuildPeriodTree(rowCellClick.TDL_PATIENT_CODE);
                    ApplyMergePeriodSelection();
                    popupPeriod.ShowPopup();
                }
                else
                {
                    popupPeriod.Enabled = false;
                    chkShowTabDetail.Enabled = true;
                    ShowNoData(false);
                    RestoreNormalTreeColumns();
                    if (rowCellClick != null && rowCellClick.ID > 0)
                        LoadDataTreeServiceReq2(this, rowCellClick);
                    ToggleDetailTab(chkShowTabDetail.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSelectAllPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                SetAllPeriodCheckState(CheckState.Checked);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnUnselectAllPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                SetAllPeriodCheckState(CheckState.Unchecked);
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnApplyPeriod_Click(object sender, EventArgs e)
        {
            try
            {
                ApplyMergePeriodSelection();
                popupPeriod.ClosePopup();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Error(ex); }
        }

        private void btnClosePeriod_Click(object sender, EventArgs e)
        {
            try
            {
                popupPeriod.ClosePopup();
            }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void treePeriod_AfterCheckNode(object sender, NodeEventArgs e)
        {
            if (isBuildingPeriodTree) return;
            try
            {
                // Bật cờ chặn để các lần set CheckState bên dưới KHÔNG đệ quy lại event này
                isBuildingPeriodTree = true;
                treePeriod.BeginUpdate();
                // Cascade xuống con
                foreach (TreeListNode child in e.Node.Nodes)
                    child.CheckState = e.Node.CheckState;
                // Tính lại trạng thái cha
                if (e.Node.ParentNode != null)
                    UpdateParentCheckState(e.Node.ParentNode);
                treePeriod.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                isBuildingPeriodTree = false;
            }
        }

        /// <summary>
        /// Khi BẬT gộp + đổi đợt/BN ở Grid 1 => re-seed popup theo BN mới + dựng lại cây gộp.
        /// Gọi từ gridViewHisTreatment5_RowCellClick.
        /// </summary>
        private void OnGridRowSelectedInMergeMode()
        {
            try
            {
                if (rowCellClick == null) return;
                EnsureSimpleTreeActive();
                BuildPeriodTree(rowCellClick.TDL_PATIENT_CODE);
                ApplyMergePeriodSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Xử lý chọn node ở cây gộp (dùng chung cho Click + FocusedNodeChanged).
        /// Node lá (Tag = ServiceReqMergeNodeADO) => nạp Grid 3; node cha => xóa Grid 3.
        /// </summary>
        private void SelectMergeTreeNode(TreeListNode node)
        {
            try
            {
                if (node == null) return;
                ServiceReqMergeNodeADO ado = node.Tag as ServiceReqMergeNodeADO;
                if (ado != null)
                {
                    ProcessMergeLeafClick(ado);
                }
                else if (ucSereServ != null)
                {
                    treeSereServ7Processor.Reload(ucSereServ, new List<V_HIS_SERE_SERV_7>());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Period popup tree

        private void BuildPeriodTree(string patientCode)
        {
            try
            {
                if (string.IsNullOrEmpty(patientCode)) return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                filter.PATIENT_CODE__EXACT = patientCode;
                filter.ORDER_FIELD = "IN_TIME";
                filter.ORDER_DIRECTION = "DESC";
                patientTreatments = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>(
                    API_HIS_TREATMENT_GET_VIEW, ApiConsumers.MosConsumer, filter, param)
                    ?? new List<V_HIS_TREATMENT>();

                var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID, o => o.DEPARTMENT_NAME);

                isBuildingPeriodTree = true;
                treePeriod.BeginUnboundLoad();
                treePeriod.ClearNodes();

                // Gom theo năm (giảm dần)
                var byYear = patientTreatments
                    .GroupBy(o => (int)(o.IN_TIME / 10000000000L))
                    .OrderByDescending(g => g.Key);

                foreach (var yearGrp in byYear)
                {
                    TreatmentPeriodADO yearAdo = new TreatmentPeriodADO() { IsYear = true, Year = yearGrp.Key, Display = yearGrp.Key.ToString() };
                    TreeListNode yearNode = treePeriod.AppendNode(new object[] { yearAdo.Display }, null, yearAdo);

                    foreach (var t in yearGrp.OrderByDescending(o => o.IN_TIME))
                    {
                        string deptName = "";
                        if (t.LAST_DEPARTMENT_ID.HasValue) deptDict.TryGetValue(t.LAST_DEPARTMENT_ID.Value, out deptName);
                        string status = string.IsNullOrEmpty(t.TREATMENT_END_TYPE_NAME) ? ResourceMessage.DangDieuTri : t.TREATMENT_END_TYPE_NAME;
                        string display = string.Format("{0} · {1} · {2} · {3}",
                            t.TREATMENT_CODE,
                            Inventec.Common.DateTime.Convert.TimeNumberToDateString(t.IN_TIME),
                            deptName,
                            status);
                        TreatmentPeriodADO tAdo = new TreatmentPeriodADO() { IsYear = false, Year = yearGrp.Key, TreatmentId = t.ID, Display = display };
                        treePeriod.AppendNode(new object[] { tAdo.Display }, yearNode, tAdo);
                    }
                }

                AutoCheckCurrentYear();

                treePeriod.EndUnboundLoad();
                treePeriod.ExpandAll();
                isBuildingPeriodTree = false;
            }
            catch (Exception ex)
            {
                isBuildingPeriodTree = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        /// <summary>Tự tích nhánh năm hệ thống hiện tại; nếu BN không có đợt năm nay => tích năm gần nhất.</summary>
        private void AutoCheckCurrentYear()
        {
            try
            {
                if (treePeriod.Nodes.Count == 0) return;
                int currentYear = DateTime.Now.Year;
                TreeListNode target = null;
                foreach (TreeListNode yearNode in treePeriod.Nodes)
                {
                    TreatmentPeriodADO ado = yearNode.Tag as TreatmentPeriodADO;
                    if (ado != null && ado.Year == currentYear) { target = yearNode; break; }
                }
                if (target == null) target = treePeriod.Nodes[0]; // năm gần nhất (đã sort giảm dần)

                target.CheckState = CheckState.Checked;
                foreach (TreeListNode child in target.Nodes)
                    child.CheckState = CheckState.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetAllPeriodCheckState(CheckState state)
        {
            try
            {
                isBuildingPeriodTree = true;
                treePeriod.BeginUpdate();
                foreach (TreeListNode yearNode in treePeriod.Nodes)
                {
                    yearNode.CheckState = state;
                    foreach (TreeListNode child in yearNode.Nodes)
                        child.CheckState = state;
                }
                treePeriod.EndUpdate();
                isBuildingPeriodTree = false;
            }
            catch (Exception ex)
            {
                isBuildingPeriodTree = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateParentCheckState(TreeListNode parent)
        {
            try
            {
                int total = parent.Nodes.Count;
                if (total == 0) return;
                int chk = 0, indeter = 0;
                foreach (TreeListNode child in parent.Nodes)
                {
                    if (child.CheckState == CheckState.Checked) chk++;
                    else if (child.CheckState == CheckState.Indeterminate) indeter++;
                }
                if (chk == total) parent.CheckState = CheckState.Checked;
                else if (chk > 0 || indeter > 0) parent.CheckState = CheckState.Indeterminate;
                else parent.CheckState = CheckState.Unchecked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<long> GetCheckedTreatmentIds()
        {
            List<long> ids = new List<long>();
            try
            {
                foreach (TreeListNode yearNode in treePeriod.Nodes)
                {
                    foreach (TreeListNode child in yearNode.Nodes)
                    {
                        TreatmentPeriodADO ado = child.Tag as TreatmentPeriodADO;
                        if (ado != null && !ado.IsYear && child.CheckState == CheckState.Checked)
                            ids.Add(ado.TreatmentId);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ids;
        }

        private void ApplyMergePeriodSelection()
        {
            try
            {
                selectedMergeTreatmentIds = GetCheckedTreatmentIds();
                UpdatePeriodText();
                BuildMergeTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdatePeriodText()
        {
            try
            {
                if (selectedMergeTreatmentIds == null || selectedMergeTreatmentIds.Count == 0)
                {
                    popupPeriod.EditValue = null;
                    return;
                }
                var selSet = new HashSet<long>(selectedMergeTreatmentIds);
                int years = patientTreatments.Where(o => selSet.Contains(o.ID))
                    .Select(o => (int)(o.IN_TIME / 10000000000L)).Distinct().Count();
                popupPeriod.EditValue = string.Format(ResourceMessage.Merge__DaChonDotNam, selectedMergeTreatmentIds.Count, years);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Build merge tree (Grid 2 - 2 root)

        private void BuildMergeTree()
        {
            try
            {
                if (!IsMergeMode) return;
                WaitingManager.Show();
                mergeDetailCache.Clear();
                ApplyMergeTreeColumns();

                tree_HisServiceReq2.BeginUnboundLoad();
                try
                {
                    tree_HisServiceReq2.ClearNodes();

                    if (selectedMergeTreatmentIds == null || selectedMergeTreatmentIds.Count == 0)
                    {
                        ShowNoData(true);
                        return;
                    }

                    CommonParam param = new CommonParam();
                    HisServiceReqFilter filter = new HisServiceReqFilter();
                    filter.TREATMENT_IDs = selectedMergeTreatmentIds;
                    List<HIS_SERVICE_REQ> reqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                        HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, filter, param);

                    if (reqs == null || reqs.Count == 0)
                    {
                        ShowNoData(true);
                        return;
                    }

                    var ttDict = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().ToDictionary(o => o.ID, o => o.TREATMENT_TYPE_NAME);
                    var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID, o => o.DEPARTMENT_NAME);
                    var srtDict = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().ToDictionary(o => o.ID, o => o.SERVICE_REQ_TYPE_NAME);

                    BuildRootByTreatmentType(reqs, ttDict, deptDict, srtDict);
                    BuildRootByServiceReqType(reqs, srtDict);

                    ShowNoData(tree_HisServiceReq2.Nodes.Count == 0);
                }
                finally
                {
                    tree_HisServiceReq2.EndUnboundLoad();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        /// <summary>Root A — Theo diện điều trị (Diện -> Khoa thực hiện -> Loại y lệnh -> Ngày y lệnh).</summary>
        private void BuildRootByTreatmentType(List<HIS_SERVICE_REQ> reqs,
            Dictionary<long, string> ttDict, Dictionary<long, string> deptDict, Dictionary<long, string> srtDict)
        {
            try
            {
                TreeListNode rootA = tree_HisServiceReq2.AppendNode(MergeRow(L("frmTreatmentHistory.rootA.Caption"), "", ""), null, null);

                var byTt = reqs.GroupBy(o => o.TREATMENT_TYPE_ID ?? (o.TDL_TREATMENT_TYPE_ID ?? 0L));
                foreach (var ttGrp in byTt)
                {
                    string ttName = GetName(ttDict, ttGrp.Key);
                    TreeListNode nTt = tree_HisServiceReq2.AppendNode(MergeRow(ttName, FormatYLenh(ttGrp.Count()), ""), rootA, null);

                    foreach (var depGrp in ttGrp.GroupBy(o => o.EXECUTE_DEPARTMENT_ID))
                    {
                        string depName = GetName(deptDict, depGrp.Key);
                        TreeListNode nDep = tree_HisServiceReq2.AppendNode(MergeRow(depName, FormatYLenh(depGrp.Count()), ""), nTt, null);

                        foreach (var srtGrp in depGrp.GroupBy(o => o.SERVICE_REQ_TYPE_ID))
                        {
                            string srtName = GetName(srtDict, srtGrp.Key);
                            int countTime = srtGrp.Select(o => o.INTRUCTION_TIME).Distinct().Count();
                            TreeListNode nSrt = tree_HisServiceReq2.AppendNode(MergeRow(srtName, FormatLan(countTime), ""), nDep, null);

                            AppendTimeLeaves(nSrt, srtGrp, depGrp.Key);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Root B — Tổng hợp theo loại y lệnh (Loại y lệnh -> Ngày y lệnh), gom xuyên Diện + Khoa.</summary>
        private void BuildRootByServiceReqType(List<HIS_SERVICE_REQ> reqs, Dictionary<long, string> srtDict)
        {
            try
            {
                TreeListNode rootB = tree_HisServiceReq2.AppendNode(MergeRow(L("frmTreatmentHistory.rootB.Caption"), "", ""), null, null);

                foreach (var srtGrp in reqs.GroupBy(o => o.SERVICE_REQ_TYPE_ID))
                {
                    string srtName = GetName(srtDict, srtGrp.Key);
                    int countTime = srtGrp.Select(o => o.INTRUCTION_TIME).Distinct().Count();
                    TreeListNode nSrt = tree_HisServiceReq2.AppendNode(MergeRow(srtName, FormatLan(countTime), ""), rootB, null);

                    AppendTimeLeaves(nSrt, srtGrp, 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tạo node lá theo từng mốc INTRUCTION_TIME, gắn Tag = ServiceReqMergeNodeADO.</summary>
        private void AppendTimeLeaves(TreeListNode parent, IEnumerable<HIS_SERVICE_REQ> reqs, long executeDeptId)
        {
            try
            {
                foreach (var timeGrp in reqs.GroupBy(o => o.INTRUCTION_TIME).OrderBy(g => g.Key))
                {
                    string timeStr = FormatInstructionTime(timeGrp.Key);
                    ServiceReqMergeNodeADO ado = new ServiceReqMergeNodeADO();
                    ado.ServiceReqs = timeGrp.ToList();
                    ado.ServiceReqCodes = new HashSet<string>(timeGrp.Select(o => o.SERVICE_REQ_CODE).Where(c => !string.IsNullOrEmpty(c)));
                    ado.ExecuteDepartmentId = executeDeptId;
                    tree_HisServiceReq2.AppendNode(MergeRow(timeStr, ado.ServiceReqs.Count.ToString(), timeStr), parent, ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// object[] khớp thứ tự cột của tree_HisServiceReq2:
        /// [0]=Nhóm(treeListColDepartment), [1]=SL(treeListColumn2), [4]=Thời gian(treeListColumn4).
        /// </summary>
        private object[] MergeRow(string group, string count, string time)
        {
            return new object[] { group, count, null, null, time, null, null };
        }

        private string GetName(Dictionary<long, string> dict, long id)
        {
            string name;
            if (id != 0 && dict != null && dict.TryGetValue(id, out name) && !string.IsNullOrEmpty(name))
                return name;
            return ResourceMessage.Merge__KhongXacDinh;
        }

        private string FormatYLenh(int n)
        {
            return string.Format(ResourceMessage.Merge__CountYLenh, n);
        }

        private string FormatLan(int n)
        {
            return string.Format(ResourceMessage.Merge__CountLan, n);
        }

        private string FormatInstructionTime(long intructionTime)
        {
            try
            {
                string s = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime);
                if (!string.IsNullOrEmpty(s) && s.Length > 3) s = s.Substring(0, s.Length - 3); // bỏ giây -> dd/MM/yyyy HH:mm
                return s;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion

        #region Grid 3 (lazy load chi tiết theo node lá)

        private void ProcessMergeLeafClick(ServiceReqMergeNodeADO ado)
        {
            try
            {
                if (ado == null || ucSereServ == null) return;
                WaitingManager.Show();

                CommonParam param = new CommonParam();
                List<DHisSereServ2> all = new List<DHisSereServ2>();
                var pairs = ado.ServiceReqs.Select(o => new { o.TREATMENT_ID, o.INTRUCTION_DATE })
                    .GroupBy(o => o.TREATMENT_ID + "_" + o.INTRUCTION_DATE)
                    .Select(g => g.First());

                foreach (var p in pairs)
                {
                    string key = p.TREATMENT_ID + "_" + p.INTRUCTION_DATE;
                    List<DHisSereServ2> data;
                    if (!mergeDetailCache.TryGetValue(key, out data))
                    {
                        DHisSereServ2Filter f = new DHisSereServ2Filter();
                        f.TREATMENT_ID = p.TREATMENT_ID;
                        f.INTRUCTION_DATE = p.INTRUCTION_DATE;
                        data = new BackendAdapter(param).Get<List<DHisSereServ2>>(
                            "api/HisSereServ/GetDHisSereServ2", ApiConsumers.MosConsumer, f, param) ?? new List<DHisSereServ2>();
                        mergeDetailCache[key] = data;
                    }
                    all.AddRange(data);
                }

                // Lọc đúng tập y lệnh của node lá theo SERVICE_REQ_CODE
                List<DHisSereServ2> filtered = all.Where(o => ado.ServiceReqCodes.Contains(o.SERVICE_REQ_CODE)).ToList();
                List<V_HIS_SERE_SERV_7> list7 = MapToSereServ7(filtered);

                if (list7.Count > 0)
                    treeSereServ7Processor.Reload(ucSereServ, ado.ExecuteDepartmentId, list7);
                else
                    treeSereServ7Processor.Reload(ucSereServ, new List<V_HIS_SERE_SERV_7>(), null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private List<V_HIS_SERE_SERV_7> MapToSereServ7(List<DHisSereServ2> data)
        {
            List<V_HIS_SERE_SERV_7> result = new List<V_HIS_SERE_SERV_7>();
            try
            {
                if (data == null || data.Count == 0) return result;
                var serviceTypeDict = BackendDataWorker.Get<HIS_SERVICE_TYPE>().ToDictionary(o => o.ID);
                foreach (var item in data)
                {
                    V_HIS_SERE_SERV_7 ado = new V_HIS_SERE_SERV_7();
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_7>(ado, item);
                    ado.TDL_REQUEST_DEPARTMENT_ID = item.REQUEST_DEPARTMENT_ID ?? 0;
                    ado.ID = item.SERE_SERV_ID ?? 0;
                    ado.TDL_SERVICE_CODE = item.SERVICE_CODE;
                    ado.TDL_SERVICE_NAME = item.SERVICE_NAME;
                    ado.TDL_SERVICE_REQ_CODE = item.SERVICE_REQ_CODE;
                    HIS_SERVICE_TYPE serviceType;
                    if (item.TDL_SERVICE_TYPE_ID != null && serviceTypeDict.TryGetValue(item.TDL_SERVICE_TYPE_ID.Value, out serviceType))
                    {
                        ado.SERVICE_TYPE_NAME = serviceType.SERVICE_TYPE_NAME;
                        ado.SERVICE_TYPE_CODE = serviceType.SERVICE_TYPE_CODE;
                    }
                    result.Add(ado);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        #endregion

        #region Tree columns + helpers

        private void EnsureSimpleTreeActive()
        {
            try
            {
                if (ucSereServ == null) return;
                if (!panelControlTreeSere7.Controls.Contains(ucSereServ))
                {
                    panelControlTreeSere7.Controls.Clear();
                    panelControlTreeSere7.Controls.Add(ucSereServ);
                    ucSereServ.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveOriginalTreeColState()
        {
            try
            {
                if (originalTreeColState != null) return;
                originalTreeColState = new Dictionary<TreeListColumn, Tuple<string, bool, int>>();
                foreach (TreeListColumn col in tree_HisServiceReq2.Columns)
                    originalTreeColState[col] = Tuple.Create(col.Caption, col.Visible, col.VisibleIndex);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ApplyMergeTreeColumns()
        {
            try
            {
                SaveOriginalTreeColState();

                treeListColDepartment.Caption = L("frmTreatmentHistory.mergeColGroup.Caption");
                treeListColDepartment.Visible = true;
                treeListColDepartment.VisibleIndex = 0;

                treeListColumn4.Caption = L("frmTreatmentHistory.mergeColTime.Caption");
                treeListColumn4.Visible = true;
                treeListColumn4.VisibleIndex = 1;
                treeListColumn4.Width = 140;

                treeListColumn2.Caption = L("frmTreatmentHistory.mergeColCount.Caption");
                treeListColumn2.Visible = true;
                treeListColumn2.VisibleIndex = 2;
                treeListColumn2.Width = 70;

                treeListColumn5.Visible = false;
                treeListColumn6.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RestoreNormalTreeColumns()
        {
            try
            {
                if (originalTreeColState == null) return;
                foreach (var kvp in originalTreeColState)
                {
                    kvp.Key.Caption = kvp.Value.Item1;
                    kvp.Key.Visible = kvp.Value.Item2;
                    if (kvp.Value.Item2) kvp.Key.VisibleIndex = kvp.Value.Item3;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowNoData(bool show)
        {
            try
            {
                if (lblNoData == null) return;
                lblNoData.Visible = show;
                if (show) lblNoData.BringToFront();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
