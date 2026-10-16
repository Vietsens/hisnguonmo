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
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using HIS.Desktop.Plugins.ContentSubclinical.ADO;
using HIS.Desktop.Plugins.ContentSubclinical.Base;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ContentSubclinical
{
    /// <summary>
    /// Task 57971 (NTP) - result screen opened by the "Kết quả CLS" button of the Bed room screen (view-only mode + flag CLS_TAB):
    /// a horizontal tab bar above the result tree: "Tất cả" + one tab per group having results (same group table as the Bed room).
    /// Every tab is fully expanded without expand / collapse; group tabs: date (bold) -> service -> index.
    /// The test filters only apply to the Test tab (greyed elsewhere); the SRI column is hidden outside "Tất cả" / Test;
    /// the image panel only shows the images of the current tab. Rows of each tab: frmContentSubclinical__Pluss__ClsTabRows.cs.
    /// Without the CLS_TAB flag (treatment sheet, exam, KSK... selection mode, headless modes) nothing in these files runs.
    /// </summary>
    public partial class frmContentSubclinical : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Caller asked for the result tabs (flag ClsTabGroupStore.ARG__CLS_TAB, view-only mode only)</summary>
        internal bool IsClsTabMode { get; set; }
        /// <summary>Group code of the tab to open first (flag CLS_TAB_INIT:&lt;code&gt;); empty or without data = "Tất cả"</summary>
        internal string InitialClsTabCode { get; set; }

        /// <summary>Code of the "Tất cả" tab (not a group)</summary>
        const string CLS_TAB_ALL = "ALL";
        /// <summary>Parent field used by the tree in tab mode (re-parented rows of the group tabs)</summary>
        const string CLS_TAB_PARENT_FIELD = "PARENT_ID__IN_TAB";
        const string CLS_TREE_PARENT_FIELD = "PARENT_ID__IN_SETY";
        /// <summary>Height of the header-only tab bar</summary>
        const int CLS_TAB_BAR_HEIGHT = 25;
        /// <summary>Name of the result tree in tab mode (own saved-layout file, see InitClsTabBar)</summary>
        const string CLS_TAB_TREE_NAME = "treeListServiceReqClsTab";

        XtraTabControl tabCls;
        LayoutControlItem lciTabCls;
        /// <summary>True while the code itself rebuilds / selects tab pages</summary>
        bool isClsTabBinding = false;
        bool isClsTabInitialApplied = false;
        string clsTabCurrentCode = CLS_TAB_ALL;
        /// <summary>Rows of "Tất cả": every sorted row of the date range (before the test filters), with the former tree structure</summary>
        List<TreeSereServADO> clsTabAllRows = new List<TreeSereServADO>();
        /// <summary>Rows of each group tab with their parent inside the tab (date row for service rows)</summary>
        Dictionary<string, List<KeyValuePair<TreeSereServADO, string>>> clsTabRowsByGroup = new Dictionary<string, List<KeyValuePair<TreeSereServADO, string>>>();
        int clsTabSriVisibleIndex = -1;
        Dictionary<long, V_HIS_SERVICE> clsServiceById;
        List<V_HIS_SERVICE> clsServiceSource;

        bool IsClsTabActive
        {
            get { return this.isViewOnly && this.IsClsTabMode && this.tabCls != null; }
        }

        /// <summary>
        /// "Chỉ số quan trọng" also limits the rows that are built (antibiogram, LIS notes). Not in tab mode: there it only
        /// filters the Test tab, "Tất cả" and the other tabs keep every row.
        /// </summary>
        bool IsImportantOnlyBuild
        {
            get { return this.chkImportant.Checked && !this.IsClsTabActive; }
        }

        /// <summary>
        /// Builds the tab bar right above the result tree (runtime layout item, no Designer change).
        /// Called in Load after ApplyViewOnlyMode, before the first LoadDataSS.
        /// </summary>
        private void InitClsTabBar()
        {
            try
            {
                if (!this.isViewOnly || !this.IsClsTabMode)
                    return;

                XtraTabControl tabControl = new XtraTabControl();
                tabControl.Name = "tabCls";
                tabControl.TabPages.Add(this.CreateClsTabPage(CLS_TAB_ALL, this.GetClsTabCaption(CLS_TAB_ALL)));

                this.layoutControl2.BeginUpdate();
                try
                {
                    this.lciTabCls = new LayoutControlItem(this.layoutControl2, tabControl);
                    this.lciTabCls.Name = "lciTabCls";
                    this.lciTabCls.TextVisible = false;
                    this.lciTabCls.SizeConstraintsType = SizeConstraintsType.Custom;
                    this.lciTabCls.MinSize = new Size(100, CLS_TAB_BAR_HEIGHT);
                    this.lciTabCls.MaxSize = new Size(0, CLS_TAB_BAR_HEIGHT);
                    this.lciTabCls.Move(this.layoutControlItem25, DevExpress.XtraLayout.Utils.InsertType.Top);
                }
                finally
                {
                    this.layoutControl2.EndUpdate();
                }

                this.clsTabSriVisibleIndex = this.treeListColumnSRI.VisibleIndex;
                this.treeListServiceReq.BeforeLoadLayout += new DevExpress.Utils.LayoutAllowEventHandler(this.treeListServiceReq_BeforeLoadLayout);
                this.treeListServiceReq.CustomDrawEmptyArea += new CustomDrawEmptyAreaEventHandler(this.treeListServiceReq_CustomDrawEmptyArea);
                this.treeListServiceReq.BeforeCollapse += new BeforeCollapseEventHandler(this.treeListServiceReq_BeforeCollapse);
                tabControl.SelectedPageChanged += new TabPageChangedEventHandler(this.tabCls_SelectedPageChanged);
                this.treeListServiceReq.ParentFieldName = CLS_TAB_PARENT_FIELD;
                // Saved tree layouts (HIS.Desktop.ApplyRestoreLayout.ModuleLinks) are keyed by the control path: the tab mode
                // gets its own file, so its parent field / hidden SRI column never reach the selection mode, and the tab mode
                // keeps the designed tree (no restore). Renamed last: the name stays tied to tabCls != null
                this.treeListServiceReq.Name = CLS_TAB_TREE_NAME;
                this.tabCls = tabControl;
            }
            catch (Exception ex)
            {
                // Tab mode off: the former tree must keep its own parent field
                this.tabCls = null;
                this.treeListServiceReq.ParentFieldName = CLS_TREE_PARENT_FIELD;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private XtraTabPage CreateClsTabPage(string code, string caption)
        {
            XtraTabPage page = new XtraTabPage();
            try
            {
                page.Name = "tabPageCls" + code;
                page.Text = caption;
                page.Tag = code;
                if (code == ClsTabGroupStore.GROUP_TDCN)
                    page.Tooltip = Inventec.Common.Resource.Get.Value("frmContentSubclinical.ClsTab.TDCN.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return page;
        }

        private string GetClsTabCaption(string code)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value("frmContentSubclinical.ClsTab." + code + ".Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return String.IsNullOrEmpty(result) ? code : result;
        }

        /// <summary>
        /// Called by LoadDataSS instead of binding the tree (tab mode only): rebuilds the tabs and binds the selected one.
        /// The test filters (Trên/Dưới ngưỡng, Chỉ số quan trọng) only apply to the Test tab: "Tất cả" and the other
        /// group tabs show every result of the date range.
        /// </summary>
        /// <param name="filteredRows">Sorted rows after the test filters (tab Test)</param>
        /// <param name="sourceRows">Sorted rows before the test filters ("Tất cả" and the other group tabs)</param>
        private void BindClsTabs(List<TreeSereServADO> filteredRows, List<TreeSereServADO> sourceRows)
        {
            try
            {
                List<TreeSereServADO> filtered = filteredRows ?? new List<TreeSereServADO>();
                List<TreeSereServADO> source = sourceRows ?? filtered;
                this.clsTabAllRows = source;

                Dictionary<string, TreeSereServADO> rowByKey = new Dictionary<string, TreeSereServADO>();
                AddRowsToIndex(rowByKey, source);
                AddRowsToIndex(rowByKey, filtered);
                Dictionary<string, string> diimGroupNames = new Dictionary<string, string>();
                Dictionary<string, string> groupByKey = this.ClassifyClsRows(rowByKey, diimGroupNames);

                Dictionary<string, int> serviceCountByGroup = CountClsServices(source, groupByKey);
                this.clsTabRowsByGroup = new Dictionary<string, List<KeyValuePair<TreeSereServADO, string>>>();
                foreach (string code in serviceCountByGroup.Keys)
                {
                    List<TreeSereServADO> groupSource = code == ClsTabGroupStore.GROUP_XN ? filtered : source;
                    this.clsTabRowsByGroup[code] = BuildClsGroupRows(code, groupSource, rowByKey, groupByKey);
                }

                this.RebuildClsTabPages(serviceCountByGroup, diimGroupNames);
                this.SelectClsTab(this.ChooseClsTab());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// No result (no treatment / no service request in the date range): only "Tất cả", empty tree and images.
        /// </summary>
        private void ClearClsTabs()
        {
            try
            {
                if (!this.IsClsTabActive)
                    return;
                this.clsTabAllRows = new List<TreeSereServADO>();
                this.clsTabRowsByGroup = new Dictionary<string, List<KeyValuePair<TreeSereServADO, string>>>();
                this.ImageADOs = new List<ImageADO>();
                this.RebuildClsTabPages(new Dictionary<string, int>(), new Dictionary<string, string>());
                this.SelectClsTab(CLS_TAB_ALL);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// "Tất cả" first, then one tab per group having results, in the shared group order; caption = name (number of services).
        /// </summary>
        private void RebuildClsTabPages(Dictionary<string, int> serviceCountByGroup, Dictionary<string, string> diimGroupNames)
        {
            bool oldBinding = this.isClsTabBinding;
            this.isClsTabBinding = true;
            try
            {
                this.tabCls.SelectedTabPageIndex = 0;
                for (int i = this.tabCls.TabPages.Count - 1; i >= 1; i--)
                {
                    XtraTabPage oldPage = this.tabCls.TabPages[i];
                    this.tabCls.TabPages.RemoveAt(i);
                    oldPage.Dispose();
                }
                foreach (string code in serviceCountByGroup.Keys.OrderBy(o => ClsTabGroupStore.GetSortKey(o)))
                {
                    string caption;
                    long diimTypeId;
                    if (!ClsTabGroupStore.TryGetDiimTypeId(code, out diimTypeId) || !diimGroupNames.TryGetValue(code, out caption))
                        caption = this.GetClsTabCaption(code);
                    this.tabCls.TabPages.Add(this.CreateClsTabPage(code, String.Format("{0} ({1})", caption, serviceCountByGroup[code])));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isClsTabBinding = oldBinding;
            }
        }

        /// <summary>
        /// First load: the tab of the group the user was on in the Bed room screen (when it has results).
        /// Later loads (Tìm, other treatment, filters): keep the current tab while it still has results, else "Tất cả".
        /// </summary>
        private string ChooseClsTab()
        {
            string result = CLS_TAB_ALL;
            try
            {
                if (!this.isClsTabInitialApplied)
                {
                    this.isClsTabInitialApplied = true;
                    if (!String.IsNullOrEmpty(this.InitialClsTabCode) && this.clsTabRowsByGroup.ContainsKey(this.InitialClsTabCode))
                        result = this.InitialClsTabCode;
                }
                else if (this.clsTabCurrentCode != CLS_TAB_ALL && this.clsTabRowsByGroup.ContainsKey(this.clsTabCurrentCode))
                {
                    result = this.clsTabCurrentCode;
                }
            }
            catch (Exception ex)
            {
                result = CLS_TAB_ALL;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void SelectClsTab(string code)
        {
            bool oldBinding = this.isClsTabBinding;
            this.isClsTabBinding = true;
            try
            {
                XtraTabPage page = this.tabCls.TabPages.Cast<XtraTabPage>().FirstOrDefault(o => (o.Tag as string) == code);
                if (page == null)
                {
                    page = this.tabCls.TabPages[0];
                    code = CLS_TAB_ALL;
                }
                this.tabCls.SelectedTabPage = page;
                this.tabCls.MakePageVisible(page);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isClsTabBinding = oldBinding;
            }
            this.BindClsTabTree(code);
        }

        private void tabCls_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            try
            {
                if (this.isClsTabBinding || e.Page == null)
                    return;
                this.BindClsTabTree(e.Page.Tag as string ?? CLS_TAB_ALL);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Binds the rows of one tab to the result tree (no API call), then filters, columns and images.
        /// </summary>
        private void BindClsTabTree(string code)
        {
            try
            {
                List<TreeSereServADO> rows = new List<TreeSereServADO>();
                List<KeyValuePair<TreeSereServADO, string>> groupRows;
                if (code != CLS_TAB_ALL && this.clsTabRowsByGroup.TryGetValue(code, out groupRows))
                {
                    foreach (KeyValuePair<TreeSereServADO, string> pair in groupRows)
                    {
                        pair.Key.PARENT_ID__IN_TAB = pair.Value;
                        rows.Add(pair.Key);
                    }
                }
                else
                {
                    code = CLS_TAB_ALL;
                    foreach (TreeSereServADO row in this.clsTabAllRows)
                    {
                        row.PARENT_ID__IN_TAB = row.PARENT_ID__IN_SETY;
                        rows.Add(row);
                    }
                }
                this.clsTabCurrentCode = code;

                // Every tab is shown fully expanded with date / service type header rows: no expand / collapse
                if (this.treeListServiceReq.ParentFieldName != CLS_TAB_PARENT_FIELD)
                    this.treeListServiceReq.ParentFieldName = CLS_TAB_PARENT_FIELD;
                this.treeListServiceReq.OptionsView.ShowButtons = false;
                this.treeListServiceReq.OptionsBehavior.AllowExpandOnDblClick = false;
                this.treeListServiceReq.DataSource = new BindingList<TreeSereServADO>(rows);
                this.treeListServiceReq.ExpandAll();
                // "In kết quả" prints the checked services of the bound tree: check every node of the tab
                // (the insert options "chỉ chỉ số quan trọng / không chọn thủ thuật" are hidden in view-only mode)
                foreach (DevExpress.XtraTreeList.Nodes.TreeListNode node in this.treeListServiceReq.Nodes)
                {
                    node.CheckAll();
                }

                this.SetClsTabFilterState(code);
                this.BindClsTabImages();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Test filters (Trên/Dưới ngưỡng, Chỉ số quan trọng, Hiển thị dịch vụ cha loại XN, Hiển thị kháng sinh đồ) only
        /// apply to the Test tab (greyed elsewhere). The SRI column is shown on "Tất cả" and the Test tab.
        /// </summary>
        private void SetClsTabFilterState(string code)
        {
            try
            {
                bool isTestTab = code == ClsTabGroupStore.GROUP_XN;
                this.chkAbove.Enabled = isTestTab;
                this.chkBelow.Enabled = isTestTab;
                this.chkImportant.Enabled = isTestTab;
                this.chkShowParentServiceGroup.Enabled = isTestTab;
                this.chkShowMicrobiological.Enabled = isTestTab;
                if (isTestTab || code == CLS_TAB_ALL)
                {
                    if (!this.treeListColumnSRI.Visible && this.clsTabSriVisibleIndex >= 0)
                        this.treeListColumnSRI.VisibleIndex = this.clsTabSriVisibleIndex;
                }
                else if (this.treeListColumnSRI.Visible)
                {
                    this.treeListColumnSRI.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Image panel: only the images of the services bound in the current tab (images were loaded once for all tabs).
        /// </summary>
        private void BindClsTabImages()
        {
            try
            {
                List<ImageADO> images = new List<ImageADO>();
                if (this.ImageADOs != null && this.ImageADOs.Count > 0)
                {
                    BindingList<TreeSereServADO> rows = this.treeListServiceReq.DataSource as BindingList<TreeSereServADO>;
                    HashSet<long> sereServIds = new HashSet<long>(rows != null ? rows.Where(o => o != null && o.ID > 0).Select(o => o.ID) : new List<long>());
                    images = this.ImageADOs.Where(o => sereServIds.Contains(o.SERE_SERV_ID)).ToList();
                }
                this.gridControl1.DataSource = images;
                this.gridControl1.RefreshDataSource();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
