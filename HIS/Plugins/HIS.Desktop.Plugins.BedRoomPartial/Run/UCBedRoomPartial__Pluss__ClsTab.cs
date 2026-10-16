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
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Plugins.BedRoomPartial.Base;
using HIS.Desktop.Plugins.BedRoomPartial.Key;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Task 57971 (NTP) - split the "CLS" order tab by service group (key HIS.Desktop.Plugins.BedRoomPartial.ClsTabOption = 1/2).
    /// The "CLS" tab is hidden and replaced by one tab per group (Test, Ultrasound, X-ray, CT / MRI, Functional tests, Endoscopy,
    /// Pathology, Other CLS + one tab per extra diagnostic imaging type of the catalog). Captions carry the number of services;
    /// the tab chosen by the user is kept when the patient or the date changes. Actions: UCBedRoomPartial__Pluss__ClsTabAction.cs.
    /// Key off: nothing in these files runs (former behaviour).
    /// </summary>
    public partial class UCBedRoomPartial : UserControlBase
    {
        /// <summary>Value of key ClsTabOption, read once in Load</summary>
        EnumClsTabOption clsTabOption = EnumClsTabOption.Off;
        /// <summary>Group tabs created at runtime, in display order</summary>
        List<ClsTabPage> clsTabPages = new List<ClsTabPage>();
        /// <summary>Name of the order tab page last chosen by the user - kept when switching patient or date</summary>
        string clsTabSelectedPageName = null;
        /// <summary>True while the code itself changes the selected tab or a tab visibility (not a user choice)</summary>
        bool isClsTabSelecting = false;
        /// <summary>Groups of the services of each service request of the current load (cross-group confirmation)</summary>
        Dictionary<long, HashSet<string>> clsGroupsByServiceReq = new Dictionary<long, HashSet<string>>();
        /// <summary>Diagnostic imaging services of the catalog by ID / by code (DIIM type lookup), rebuilt when the cache list changes</summary>
        Dictionary<long, V_HIS_SERVICE> clsServiceById;
        Dictionary<string, V_HIS_SERVICE> clsServiceByCode;
        List<V_HIS_SERVICE> clsServiceSource;

        /// <summary>
        /// True when the key splits the "CLS" tab.
        /// </summary>
        bool IsClsTabEnabled
        {
            get { return this.clsTabOption == EnumClsTabOption.ShowMainTabs || this.clsTabOption == EnumClsTabOption.ShowTabsWithData; }
        }

        /// <summary>
        /// Called in Load right after the language resources are set: reads the key and, when enabled, creates the fixed
        /// group tabs between "Tất cả" and "Thuốc, vật tư, máu" and hides the former "CLS" tab. If a group tab cannot be
        /// created the screen keeps the former "CLS" tab (key handled as off).
        /// </summary>
        private void InitClsTabs()
        {
            try
            {
                this.clsTabOption = HisConfigCFG.ClsTabOption;
                if (!this.IsClsTabEnabled)
                    return;

                bool isCreated = false;
                this.isClsTabSelecting = true;
                this.xtraTabControl1.BeginUpdate();
                try
                {
                    isCreated = this.CreateFixedClsTabPages();
                    if (isCreated)
                        this.ReplaceFormerClsTab();
                    else
                        this.RemoveClsTabPages();
                }
                finally
                {
                    this.xtraTabControl1.EndUpdate();
                    this.isClsTabSelecting = false;
                }

                if (!isCreated)
                {
                    this.clsTabOption = EnumClsTabOption.Off;
                    Inventec.Common.Logging.LogSystem.Warn("Task 57971: some CLS group tabs could not be created -> former CLS tab kept");
                    return;
                }
                this.xtraTabControl1.SelectedPageChanged += new TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
                this.xtraTabControl1.MouseDown += new MouseEventHandler(this.xtraTabControl1_MouseDown);
            }
            catch (Exception ex)
            {
                this.isClsTabSelecting = false;
                this.FallbackToFormerClsTab();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Creates the fixed group tabs; true when every one was created.
        /// </summary>
        private bool CreateFixedClsTabPages()
        {
            bool result = false;
            try
            {
                List<string> fixedGroups = ClsTabGroupStore.GetBedRoomFixedGroups();
                foreach (string groupCode in fixedGroups)
                {
                    this.CreateClsTabPage(groupCode, this.GetClsTabCaption(groupCode));
                }
                result = this.clsTabPages.Count == fixedGroups.Count;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Hides the former "CLS" tab once the group tabs exist and prepares the trees of the never-shown pages.
        /// </summary>
        private void ReplaceFormerClsTab()
        {
            try
            {
                this.xtraTabPage2.PageVisible = false;
                // Without the former cycling over TabPages[3..0], a tree on a never-shown page must already own
                // a window handle, otherwise it keeps the node structure of its first binding (wrong / blank rows).
                if (this.ucAll != null) this.ucAll.EnsureTreeHandle();
                if (this.ucMediMate != null) this.ucMediMate.EnsureTreeHandle();
                if (this.ucOrther != null) this.ucOrther.EnsureTreeHandle();
                if (this.ucCLS != null) this.ucCLS.EnsureTreeHandle();
                this.ApplyClsTabVisibility();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Creates one group tab (page + order tree) and inserts it at its place among the group tabs.
        /// </summary>
        private ClsTabPage CreateClsTabPage(string groupCode, string caption)
        {
            ClsTabPage result = null;
            XtraTabPage page = null;
            try
            {
                page = new XtraTabPage();
                page.Name = "xtraTabPageCls" + groupCode;
                page.Text = caption;
                page.Tag = groupCode;
                string fullName = caption;
                if (groupCode == ClsTabGroupStore.GROUP_TDCN)
                {
                    page.Tooltip = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPageCls.TDCN.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    if (!String.IsNullOrEmpty(page.Tooltip))
                        fullName = page.Tooltip;
                }

                UCTreeListService treeUc = new UCTreeListService(imageCollection1, currentModule);
                treeUc.Dock = DockStyle.Fill;
                treeUc.SetExpendCheckClick(ExpendCheck_Click);
                treeUc.SetAnticipateColumnsVisible(this.isShowAnticipateByUseDate);
                treeUc.SetEmptyText(Inventec.Common.Resource.Get.Value("UCBedRoomPartial.ClsTab.EmptyText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture()));
                page.Controls.Add(treeUc);
                treeUc.EnsureTreeHandle();

                result = new ClsTabPage();
                result.GroupCode = groupCode;
                result.BaseCaption = caption;
                result.FullName = fullName;
                result.Page = page;
                result.TreeUc = treeUc;

                long sortKey = ClsTabGroupStore.GetSortKey(groupCode);
                int countBefore = this.clsTabPages.Count(o => ClsTabGroupStore.GetSortKey(o.GroupCode) <= sortKey);
                int insertIndex = this.xtraTabControl1.TabPages.IndexOf(this.xtraTabPage1) + 1 + countBefore;
                this.xtraTabControl1.TabPages.Insert(insertIndex, page);
                this.clsTabPages.Add(result);
                this.clsTabPages = this.clsTabPages.OrderBy(o => ClsTabGroupStore.GetSortKey(o.GroupCode)).ToList();
            }
            catch (Exception ex)
            {
                // A page not kept in the tab list is released (its tree may already own a window handle)
                if (page != null && (result == null || !this.clsTabPages.Contains(result)))
                    this.DisposeClsTabPage(page);
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void DisposeClsTabPage(XtraTabPage page)
        {
            try
            {
                if (this.xtraTabControl1.TabPages.Contains(page))
                    this.xtraTabControl1.TabPages.Remove(page);
                foreach (UCTreeListService treeUc in page.Controls.OfType<UCTreeListService>().ToList())
                {
                    treeUc.DisposeData();
                }
                page.Dispose();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetClsTabCaption(string groupCode)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Resource.Get.Value("UCBedRoomPartial.xtraTabPageCls." + groupCode + ".Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                if (String.IsNullOrEmpty(result))
                    result = groupCode;
            }
            catch (Exception ex)
            {
                result = groupCode;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Removes the group tabs already created (tab creation failed at Load).
        /// </summary>
        private void RemoveClsTabPages()
        {
            try
            {
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    if (tab.Page != null)
                        this.DisposeClsTabPage(tab.Page);
                    tab.Page = null;
                    tab.TreeUc = null;
                }
                this.clsTabPages = new List<ClsTabPage>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Key handled as off: group tabs removed, former "CLS" tab shown again.
        /// </summary>
        private void FallbackToFormerClsTab()
        {
            try
            {
                this.RemoveClsTabPages();
                this.clsTabOption = EnumClsTabOption.Off;
                if (this.xtraTabPage2 != null)
                    this.xtraTabPage2.PageVisible = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Fills the group tabs from the type-grouped rows of one load (replaces the former "CLS" tab list).
        /// </summary>
        private void ReloadClsTabs(List<SereServADO> sortedRows)
        {
            try
            {
                this.EnsureClsServiceLookup();
                ClsTabDataBuilder builder = new ClsTabDataBuilder(this.clsServiceById, this.clsServiceByCode);
                builder.Build(sortedRows);
                this.clsGroupsByServiceReq = builder.GroupsByServiceReq;

                // Extra diagnostic imaging types of the catalog (e.g. PET/CT) get their own tab on first use
                foreach (string groupCode in builder.RowsByGroup.Keys.ToList())
                {
                    if (this.clsTabPages.Any(o => o.GroupCode == groupCode))
                        continue;
                    string caption;
                    if (!builder.DiimGroupNames.TryGetValue(groupCode, out caption) || String.IsNullOrEmpty(caption))
                        caption = groupCode;
                    this.CreateClsTabPage(groupCode, caption);
                }

                // Captions change with the counts: one header layout for all the tabs
                this.xtraTabControl1.BeginUpdate();
                try
                {
                    foreach (ClsTabPage tab in this.clsTabPages)
                    {
                        List<SereServADO> rows;
                        builder.RowsByGroup.TryGetValue(tab.GroupCode, out rows);
                        int count;
                        builder.ServiceCountByGroup.TryGetValue(tab.GroupCode, out count);
                        this.ReloadClsTabTree(tab, rows, count);
                    }
                }
                finally
                {
                    this.xtraTabControl1.EndUpdate();
                }
                this.ApplyClsTabVisibility();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ReloadClsTabTree(ClsTabPage tab, List<SereServADO> rows, int serviceCount)
        {
            try
            {
                string groupCode = tab.GroupCode;
                tab.TreeUc.ReLoad(treeView_Click, rows, this.RowCellClickBedRoom, data => this.EditFromClsTab(groupCode, data), data => this.DeleteFromClsTab(groupCode, data));
                tab.ServiceCount = serviceCount;
                tab.Page.Text = serviceCount > 0 ? String.Format("{0} ({1})", tab.BaseCaption, serviceCount) : tab.BaseCaption;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// No order in the selected date: empty every group tab.
        /// </summary>
        private void ClearClsTabs()
        {
            try
            {
                this.clsGroupsByServiceReq = new Dictionary<long, HashSet<string>>();
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    this.ReloadClsTabTree(tab, null, 0);
                }
                this.ApplyClsTabVisibility();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Option 1: main groups always shown, the others only with orders. Option 2: only groups with orders.
        /// </summary>
        private void ApplyClsTabVisibility()
        {
            bool oldSelecting = this.isClsTabSelecting;
            this.isClsTabSelecting = true;
            this.xtraTabControl1.BeginUpdate();
            try
            {
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    bool visible = tab.ServiceCount > 0
                        || (this.clsTabOption == EnumClsTabOption.ShowMainTabs && ClsTabGroupStore.IsCoreGroup(tab.GroupCode));
                    if (tab.Page.PageVisible != visible)
                        tab.Page.PageVisible = visible;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.xtraTabControl1.EndUpdate();
                this.isClsTabSelecting = oldSelecting;
            }
        }

        /// <summary>
        /// Selects the tab last chosen by the user when it is visible, otherwise "Tất cả"
        /// (replaces the former cycling that always ended on TabPages[0]), then scrolls the header row to it.
        /// </summary>
        private void ApplyClsTabSelection()
        {
            bool oldSelecting = this.isClsTabSelecting;
            this.isClsTabSelecting = true;
            try
            {
                XtraTabPage target = this.xtraTabPage1;
                if (!String.IsNullOrEmpty(this.clsTabSelectedPageName))
                {
                    XtraTabPage remembered = this.xtraTabControl1.TabPages.Cast<XtraTabPage>().FirstOrDefault(o => o.Name == this.clsTabSelectedPageName);
                    if (remembered != null && remembered.PageVisible)
                        target = remembered;
                }
                if (this.xtraTabControl1.SelectedTabPage != target)
                    this.xtraTabControl1.SelectedTabPage = target;
                // Group tabs shown on the left can push the selected header behind the scroll buttons
                if (this.xtraTabControl1.SelectedTabPage != null)
                    this.xtraTabControl1.MakePageVisible(this.xtraTabControl1.SelectedTabPage);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isClsTabSelecting = oldSelecting;
            }
        }

        private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            try
            {
                if (this.isClsTabSelecting || e.Page == null)
                    return;
                this.clsTabSelectedPageName = e.Page.Name;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// A click on the header of the page already shown (e.g. "Tất cả" selected because the remembered tab had no order)
        /// is a user choice too: it does not raise SelectedPageChanged.
        /// </summary>
        private void xtraTabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left)
                    return;
                XtraTabHitInfo hitInfo = this.xtraTabControl1.CalcHitInfo(e.Location);
                XtraTabPage page = hitInfo != null ? hitInfo.Page as XtraTabPage : null;
                if (page != null && hitInfo.HitTest == XtraTabHitTest.PageHeader && page == this.xtraTabControl1.SelectedTabPage)
                    this.clsTabSelectedPageName = page.Name;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Shows / hides the "Khám bệnh" tab (async load of the main exam). With group tabs the change is not a user
        /// choice: the remembered tab is selected again, or "Tất cả" when it is hidden. Key off: same as before.
        /// </summary>
        private void SetExamTabVisible(bool visible)
        {
            try
            {
                if (!this.IsClsTabEnabled)
                {
                    this.xtraTabPage5.PageVisible = visible;
                    return;
                }
                bool oldSelecting = this.isClsTabSelecting;
                this.isClsTabSelecting = true;
                try
                {
                    this.xtraTabPage5.PageVisible = visible;
                }
                finally
                {
                    this.isClsTabSelecting = oldSelecting;
                }
                this.ApplyClsTabSelection();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
