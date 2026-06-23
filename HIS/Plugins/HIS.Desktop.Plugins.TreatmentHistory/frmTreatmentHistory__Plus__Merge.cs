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
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
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
    /// Chế độ "Gộp kết quả KCB theo nhóm dịch vụ" (config HIS.TREATMENT_HISTORY.MERGE_BY_SERVICE_TYPE) — v1.3.
    /// - Grid 2 (tree_HisServiceReq2): 2 root song song, LÁ = từng dịch vụ (Mã DV / Tên DV / TG kết thúc).
    /// - Grid 3 (panelControlTreeSere7): XtraTabControl 3 tab (Xét nghiệm / Khám / Y lệnh khác) nhúng inline
    ///   Form chi tiết (SereServTein / ExamServiceReqResult / ServiceReqResultView) theo SERE_SERV ID node lá.
    /// Toàn bộ control + logic gộp dựng runtime, KHÔNG đụng Designer, KHÔNG phá luồng khi config TẮT.
    /// </summary>
    public partial class frmTreatmentHistory : HIS.Desktop.Utility.FormBase
    {
        #region Declare Merge

        private const string API_HIS_TREATMENT_GET_VIEW = "api/HisTreatment/GetView";
        private const string API_GET_DHIS_SERE_SERV2 = "api/HisSereServ/GetDHisSereServ2";

        // ModuleLink 3 plugin chi tiết nhúng vào Grid 3 (đều là FORM, nhận long sereServId + Module)
        private const string ML_SERE_SERV_TEIN = "HIS.Desktop.Plugins.SereServTein";
        private const string ML_EXAM_SERVICE_REQ_RESULT = "HIS.Desktop.Plugins.ExamServiceReqResult";
        private const string ML_SERVICE_REQ_RESULT_VIEW = "HIS.Desktop.Plugins.ServiceReqResultView";

        // Control thanh lọc (dựng runtime)
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

        // Grid 3 — tab nhúng Form chi tiết
        private XtraTabControl tabMergeDetail;
        private XtraTabPage xtraTabPageXN;
        private XtraTabPage xtraTabPageExam;
        private XtraTabPage xtraTabPageOther;
        private Form currentXnForm;
        private Form currentExamForm;
        private Form currentOtherForm;

        // Trạng thái
        private bool mergeFeatureEnabled;
        private bool isBuildingPeriodTree;
        private List<V_HIS_TREATMENT> patientTreatments = new List<V_HIS_TREATMENT>();
        private List<long> selectedMergeTreatmentIds = new List<long>();
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
                BuildMergeDetailTabs();
                BuildNoDataLabel();
                SetCaptionMergeControls();

                // Khôi phục trạng thái checkbox đã lưu (chỉ lưu bool, không lưu danh sách đợt)
                RestoreMergeCheckState();

                if (chkMergeByServiceType.Checked)
                {
                    popupPeriod.Enabled = true;
                    chkShowTabDetail.Enabled = false;
                    ApplyMergeTreeColumns();
                    ActivateMergeDetailPanel(true);
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

                // Chèn ngay sau nút Tìm: [Tìm][Gộp][Đợt][khoảng trống]
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

        /// <summary>Dựng XtraTabControl 3 tab nhúng Form chi tiết, đặt trong panelControlTreeSere7 (ẩn mặc định).</summary>
        private void BuildMergeDetailTabs()
        {
            try
            {
                this.tabMergeDetail = new XtraTabControl();
                this.tabMergeDetail.Name = "tabMergeDetail";
                this.tabMergeDetail.Dock = DockStyle.Fill;

                this.xtraTabPageXN = new XtraTabPage();
                this.xtraTabPageXN.Name = "xtraTabPageXN";
                this.xtraTabPageXN.Text = "Xét nghiệm";

                this.xtraTabPageExam = new XtraTabPage();
                this.xtraTabPageExam.Name = "xtraTabPageExam";
                this.xtraTabPageExam.Text = "Khám";

                this.xtraTabPageOther = new XtraTabPage();
                this.xtraTabPageOther.Name = "xtraTabPageOther";
                this.xtraTabPageOther.Text = "Y lệnh khác";

                this.tabMergeDetail.TabPages.AddRange(new XtraTabPage[] {
                    this.xtraTabPageXN, this.xtraTabPageExam, this.xtraTabPageOther });

                this.tabMergeDetail.Visible = false;
                this.panelControlTreeSere7.Controls.Add(this.tabMergeDetail);
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
                this.xtraTabPageXN.Text = L("frmTreatmentHistory.xtraTabPageXN.Text");
                this.xtraTabPageExam.Text = L("frmTreatmentHistory.xtraTabPageExam.Text");
                this.xtraTabPageOther.Text = L("frmTreatmentHistory.xtraTabPageOther.Text");
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
                    ApplyMergeTreeColumns();
                    ActivateMergeDetailPanel(true);
                    BuildPeriodTree(rowCellClick.TDL_PATIENT_CODE);
                    ApplyMergePeriodSelection();
                    popupPeriod.ShowPopup();
                }
                else
                {
                    popupPeriod.Enabled = false;
                    chkShowTabDetail.Enabled = true;
                    ShowNoData(false);
                    DisposeEmbeddedForms();
                    ActivateMergeDetailPanel(false);
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
            try { SetAllPeriodCheckState(CheckState.Checked); }
            catch (Exception ex) { Inventec.Common.Logging.LogSystem.Warn(ex); }
        }

        private void btnUnselectAllPeriod_Click(object sender, EventArgs e)
        {
            try { SetAllPeriodCheckState(CheckState.Unchecked); }
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
            try { popupPeriod.ClosePopup(); }
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
                foreach (TreeListNode child in e.Node.Nodes)
                    child.CheckState = e.Node.CheckState;
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
                ActivateMergeDetailPanel(true);
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
        /// Node lá (Tag = DHisSereServ2) => nhúng Form chi tiết theo loại DV; node cha => bỏ qua.
        /// </summary>
        private void SelectMergeTreeNode(TreeListNode node)
        {
            try
            {
                if (node == null || node.HasChildren) return;
                DHisSereServ2 leaf = node.Tag as DHisSereServ2;
                if (leaf != null)
                    HandleMergeLeafEmbed(leaf);
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
                if (target == null) target = treePeriod.Nodes[0];

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

        #region Build merge tree (Grid 2 - 2 root, lá = dịch vụ)

        private void BuildMergeTree()
        {
            try
            {
                if (!IsMergeMode) return;
                WaitingManager.Show();
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

                    // 1 call: HIS_SERVICE_REQ của tất cả đợt đã chọn (cung cấp Diện/Khoa/Loại/TG kết thúc)
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.TREATMENT_IDs = selectedMergeTreatmentIds;
                    List<HIS_SERVICE_REQ> reqs = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>(
                        HisRequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, param)
                        ?? new List<HIS_SERVICE_REQ>();

                    // Dịch vụ (sere_serv) từng đợt — lá của cây
                    List<DHisSereServ2> services = LoadServicesForTreatments(selectedMergeTreatmentIds, param);

                    if (reqs.Count == 0 || services.Count == 0)
                    {
                        ShowNoData(true);
                        return;
                    }

                    // Map SERVICE_REQ_CODE -> req (lấy Diện/Khoa/Loại/TG kết thúc)
                    Dictionary<string, HIS_SERVICE_REQ> reqByCode = new Dictionary<string, HIS_SERVICE_REQ>();
                    foreach (var r in reqs)
                        if (!string.IsNullOrEmpty(r.SERVICE_REQ_CODE) && !reqByCode.ContainsKey(r.SERVICE_REQ_CODE))
                            reqByCode[r.SERVICE_REQ_CODE] = r;

                    var ttDict = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().ToDictionary(o => o.ID, o => o.TREATMENT_TYPE_NAME);
                    var deptDict = BackendDataWorker.Get<HIS_DEPARTMENT>().ToDictionary(o => o.ID, o => o.DEPARTMENT_NAME);
                    var srtDict = BackendDataWorker.Get<HIS_SERVICE_REQ_TYPE>().ToDictionary(o => o.ID, o => o.SERVICE_REQ_TYPE_NAME);

                    // Chỉ giữ dịch vụ có y lệnh tương ứng (đảm bảo lấy được nhóm)
                    List<MergeServiceItem> items = new List<MergeServiceItem>();
                    foreach (var s in services)
                    {
                        HIS_SERVICE_REQ r;
                        if (string.IsNullOrEmpty(s.SERVICE_REQ_CODE) || !reqByCode.TryGetValue(s.SERVICE_REQ_CODE, out r)) continue;
                        items.Add(new MergeServiceItem() { Service = s, Req = r });
                    }
                    if (items.Count == 0) { ShowNoData(true); return; }

                    BuildRootByTreatmentType(items, ttDict, deptDict, srtDict);
                    BuildRootByServiceReqType(items, srtDict);

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

        /// <summary>Nạp dịch vụ (DHisSereServ2) của các đợt đã chọn — mỗi đợt 1 call (filter chỉ nhận 1 TREATMENT_ID).</summary>
        private List<DHisSereServ2> LoadServicesForTreatments(List<long> treatmentIds, CommonParam param)
        {
            List<DHisSereServ2> all = new List<DHisSereServ2>();
            try
            {
                foreach (long tid in treatmentIds)
                {
                    DHisSereServ2Filter f = new DHisSereServ2Filter();
                    f.TREATMENT_ID = tid;
                    var data = new BackendAdapter(param).Get<List<DHisSereServ2>>(
                        API_GET_DHIS_SERE_SERV2, ApiConsumers.MosConsumer, f, param);
                    if (data != null && data.Count > 0) all.AddRange(data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return all;
        }

        /// <summary>Root A — Theo diện điều trị: Diện -> Khoa thực hiện -> Loại y lệnh -> dịch vụ (lá).</summary>
        private void BuildRootByTreatmentType(List<MergeServiceItem> items,
            Dictionary<long, string> ttDict, Dictionary<long, string> deptDict, Dictionary<long, string> srtDict)
        {
            try
            {
                TreeListNode rootA = tree_HisServiceReq2.AppendNode(MergeRow(L("frmTreatmentHistory.rootA.Caption"), "", "", ""), null, null);

                foreach (var ttGrp in items.GroupBy(o => o.Req.TREATMENT_TYPE_ID ?? (o.Req.TDL_TREATMENT_TYPE_ID ?? 0L)))
                {
                    TreeListNode nTt = tree_HisServiceReq2.AppendNode(MergeRow(GetName(ttDict, ttGrp.Key), "", "", ""), rootA, null);

                    foreach (var depGrp in ttGrp.GroupBy(o => o.Req.EXECUTE_DEPARTMENT_ID))
                    {
                        TreeListNode nDep = tree_HisServiceReq2.AppendNode(MergeRow(GetName(deptDict, depGrp.Key), "", "", ""), nTt, null);

                        foreach (var srtGrp in depGrp.GroupBy(o => o.Req.SERVICE_REQ_TYPE_ID))
                        {
                            TreeListNode nSrt = tree_HisServiceReq2.AppendNode(MergeRow(GetName(srtDict, srtGrp.Key), "", "", ""), nDep, null);
                            AppendServiceLeaves(nSrt, srtGrp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Root B — Tổng hợp theo loại y lệnh: Loại y lệnh -> dịch vụ (lá), gom xuyên Diện + Khoa.</summary>
        private void BuildRootByServiceReqType(List<MergeServiceItem> items, Dictionary<long, string> srtDict)
        {
            try
            {
                TreeListNode rootB = tree_HisServiceReq2.AppendNode(MergeRow(L("frmTreatmentHistory.rootB.Caption"), "", "", ""), null, null);

                foreach (var srtGrp in items.GroupBy(o => o.Req.SERVICE_REQ_TYPE_ID))
                {
                    TreeListNode nSrt = tree_HisServiceReq2.AppendNode(MergeRow(GetName(srtDict, srtGrp.Key), "", "", ""), rootB, null);
                    AppendServiceLeaves(nSrt, srtGrp);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Tạo node lá theo từng dịch vụ; Tag = DHisSereServ2 (mang SERE_SERV_ID + TDL_SERVICE_TYPE_ID).</summary>
        private void AppendServiceLeaves(TreeListNode parent, IEnumerable<MergeServiceItem> items)
        {
            try
            {
                foreach (var it in items.OrderBy(o => o.Req.INTRUCTION_TIME))
                {
                    string timeStr = FormatTime(it.Req.INTRUCTION_TIME);
                    string finishStr = it.Req.FINISH_TIME.HasValue ? FormatTime(it.Req.FINISH_TIME.Value) : "";
                    tree_HisServiceReq2.AppendNode(
                        MergeRow(timeStr, it.Service.SERVICE_CODE, it.Service.SERVICE_NAME, finishStr),
                        parent, it.Service);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// object[] khớp thứ tự cột tree_HisServiceReq2:
        /// [0]=Nhóm/Ngày(treeListColDepartment), [4]=Mã DV(treeListColumn4), [5]=Tên DV(treeListColumn5), [6]=TG kết thúc(treeListColumn6).
        /// </summary>
        private object[] MergeRow(string col0, string maDV, string tenDV, string finish)
        {
            return new object[] { col0, null, null, null, maDV, tenDV, finish };
        }

        private string GetName(Dictionary<long, string> dict, long id)
        {
            string name;
            if (id != 0 && dict != null && dict.TryGetValue(id, out name) && !string.IsNullOrEmpty(name))
                return name;
            return ResourceMessage.Merge__KhongXacDinh;
        }

        private string FormatTime(long timeNumber)
        {
            try
            {
                string s = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(timeNumber);
                if (!string.IsNullOrEmpty(s) && s.Length > 3) s = s.Substring(0, s.Length - 3); // bỏ giây -> dd/MM/yyyy HH:mm
                return s;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        /// <summary>Cặp dịch vụ + y lệnh cha (để dựng cây lá theo dịch vụ nhưng nhóm theo y lệnh).</summary>
        private class MergeServiceItem
        {
            public DHisSereServ2 Service { get; set; }
            public HIS_SERVICE_REQ Req { get; set; }
        }

        #endregion

        #region Grid 3 — nhúng Form chi tiết vào tab

        /// <summary>Định tuyến node lá theo loại DV → nhúng Form chi tiết vào tab tương ứng.</summary>
        private void HandleMergeLeafEmbed(DHisSereServ2 leaf)
        {
            try
            {
                long sereServId = leaf.SERE_SERV_ID ?? 0;
                if (sereServId <= 0) return;
                long serviceTypeId = leaf.TDL_SERVICE_TYPE_ID ?? 0;

                if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                {
                    EmbedFormToTab(xtraTabPageXN, ML_SERE_SERV_TEIN, sereServId, ref currentXnForm);
                }
                else if (serviceTypeId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__KH)
                {
                    EmbedFormToTab(xtraTabPageExam, ML_EXAM_SERVICE_REQ_RESULT, sereServId, ref currentExamForm);
                }
                else
                {
                    EmbedFormToTab(xtraTabPageOther, ML_SERVICE_REQ_RESULT_VIEW, sereServId, ref currentOtherForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Nhúng Form chi tiết (TopLevel=false) vào XtraTabPage. Cùng SERE_SERV ID đang nhúng => giữ, không reload.
        /// </summary>
        private void EmbedFormToTab(XtraTabPage tabPage, string moduleLink, long sereServId, ref Form currentForm)
        {
            try
            {
                if (tabMergeDetail != null) tabMergeDetail.SelectedTabPage = tabPage;

                // Cùng dịch vụ đang nhúng → không reload
                if (currentForm != null && currentForm.Tag != null && (long)currentForm.Tag == sereServId)
                    return;

                // Dispose Form cũ trong tab
                if (currentForm != null)
                {
                    tabPage.Controls.Remove(currentForm);
                    currentForm.Dispose();
                    currentForm = null;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .FirstOrDefault(o => o.ModuleLink == moduleLink);
                if (moduleData == null || !moduleData.IsPlugin || moduleData.ExtensionInfo == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("Khong tim thay plugin chi tiet: " + moduleLink);
                    return;
                }

                WaitingManager.Show();
                var moduleWithRoom = PluginInstance.GetModuleWithWorkingRoom(
                    moduleData, currentModule.RoomId, currentModule.RoomTypeId);
                List<object> listArgs = new List<object>();
                listArgs.Add(sereServId);
                listArgs.Add(moduleWithRoom);
                var instance = PluginInstance.GetPluginInstance(moduleWithRoom, listArgs);
                WaitingManager.Hide();

                Form form = instance as Form;
                if (form == null) return;

                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Normal;
                form.Dock = DockStyle.Fill;
                form.Tag = sereServId;
                // Tránh đóng app khi Form con gọi Close()/DialogResult
                form.FormClosing += (s, ev) =>
                {
                    if (ev.CloseReason == CloseReason.UserClosing) ev.Cancel = true;
                };

                tabPage.Controls.Add(form);
                form.Show();
                currentForm = form;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Dispose cả 3 Form embedded — gọi khi TẮT gộp hoặc đóng form Lịch sử điều trị.</summary>
        private void DisposeEmbeddedForms()
        {
            try
            {
                DisposeOneEmbeddedForm(xtraTabPageXN, ref currentXnForm);
                DisposeOneEmbeddedForm(xtraTabPageExam, ref currentExamForm);
                DisposeOneEmbeddedForm(xtraTabPageOther, ref currentOtherForm);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DisposeOneEmbeddedForm(XtraTabPage tabPage, ref Form form)
        {
            if (form == null) return;
            try
            {
                if (tabPage != null) tabPage.Controls.Remove(form);
                form.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            form = null;
        }

        #endregion

        #region Grid 3 panel toggle + Tree columns + helpers

        /// <summary>BẬT gộp: hiện tab nhúng, ẩn cây/ tab chi tiết cũ. TẮT: ẩn tab nhúng, trả về cây cũ.</summary>
        private void ActivateMergeDetailPanel(bool merge)
        {
            try
            {
                if (tabMergeDetail == null) return;
                if (merge)
                {
                    // ToggleDetailTab (luồng cũ) gọi panel.Controls.Clear() khi TẮT gộp → tab bị gỡ;
                    // thêm lại nếu thiếu để bật/tắt/bật lại vẫn hiển thị đúng.
                    if (!panelControlTreeSere7.Controls.Contains(tabMergeDetail))
                        panelControlTreeSere7.Controls.Add(tabMergeDetail);
                    tabMergeDetail.Dock = DockStyle.Fill;
                    tabMergeDetail.Visible = true;
                    tabMergeDetail.BringToFront();
                    if (ucSereServ != null) ucSereServ.Visible = false;
                    if (ucTreeDetail != null) ucTreeDetail.Visible = false;
                }
                else
                {
                    tabMergeDetail.Visible = false;
                    if (ucSereServ != null) ucSereServ.Visible = true;
                    if (ucTreeDetail != null) ucTreeDetail.Visible = true;
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

        /// <summary>Cấu hình cột Grid 2 cho chế độ gộp: Nhóm/Ngày · Mã DV · Tên DV · TG kết thúc.</summary>
        private void ApplyMergeTreeColumns()
        {
            try
            {
                SaveOriginalTreeColState();

                treeListColDepartment.Caption = L("frmTreatmentHistory.mergeColGroup.Caption");
                treeListColDepartment.Visible = true;
                treeListColDepartment.VisibleIndex = 0;
                treeListColDepartment.Width = 320;

                treeListColumn4.Caption = L("frmTreatmentHistory.gridColumnMaDichVu.Caption");
                treeListColumn4.Visible = true;
                treeListColumn4.VisibleIndex = 1;
                treeListColumn4.Width = 110;

                treeListColumn5.Caption = L("frmTreatmentHistory.gridColumnTenDichVu.Caption");
                treeListColumn5.Visible = true;
                treeListColumn5.VisibleIndex = 2;
                treeListColumn5.Width = 240;

                treeListColumn6.Caption = L("frmTreatmentHistory.gridColumnTGKetThuc.Caption");
                treeListColumn6.Visible = true;
                treeListColumn6.VisibleIndex = 3;
                treeListColumn6.Width = 120;

                treeListColumn2.Visible = false;
                treeListColumn1.Visible = false;
                treeListColumn3.Visible = false;

                tree_HisServiceReq2.OptionsView.AutoWidth = false; // scrollbar ngang khi tổng cột > width
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
                tree_HisServiceReq2.OptionsView.AutoWidth = true;
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
