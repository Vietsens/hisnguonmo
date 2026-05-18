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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.Common;
using HIS.Desktop.Utility;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using System.Collections;
using System.Resources;
using EMR.Desktop.Plugins.EmrExamCategory.ADO;

namespace EMR.Desktop.Plugins.EmrExamCategory
{
    public partial class EmrExamCategoryForm : FormBase
    {
        #region Declare
        DelegateSelectData delegateSelect = null;
        Inventec.Desktop.Common.Modules.Module currentModule;

        List<EMR_EXAM_CATEGORY> listCategory = new List<EMR_EXAM_CATEGORY>();
        List<EMR_DOCUMENT_PAIR_RULE> listRule = new List<EMR_DOCUMENT_PAIR_RULE>();

        DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo catDragHitInfo;
        DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo ruleDragHitInfo;

        // Counter for assigning temp negative IDs to new categories
        // so rules can reference them before they are persisted
        long tempCategoryIdCounter = -1;
        #endregion

        #region Constructor
        public EmrExamCategoryForm(Inventec.Desktop.Common.Modules.Module module, DelegateSelectData delegateData)
            : base(module)
        {
            InitializeComponent();
            try
            {
                currentModule = module;
                this.delegateSelect = delegateData;
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public EmrExamCategoryForm(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            try
            {
                InitializeComponent();
                currentModule = module;
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Form Load
        private void EmrExamCategoryForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                LoadAddButtonIcons();

                // Subscribe event: click category → filter rule grid
                gridViewCat.FocusedRowChanged += gridViewCat_FocusedRowChanged;

                // Hide EXAM_CATEGORY_ID column in rule grid (implicit from selected category)
                gcolRuleExamCategory.Visible = false;

                LoadAllCategories();
                InitRuleLookupData();
                LoadAllRules();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadAddButtonIcons()
        {
            try
            {
                var addIcon = CreateAddIcon(16);
                btnCatAdd.Image = addIcon;
                btnCatAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
                btnRuleAdd.Image = addIcon;
                btnRuleAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                btnCatAdd.Text = "+";
                btnRuleAdd.Text = "+";
            }
        }

        private System.Drawing.Image CreateAddIcon(int size)
        {
            var bmp = new System.Drawing.Bitmap(size, size);
            using (var g = System.Drawing.Graphics.FromImage(bmp))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (var brush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(0, 171, 126)))
                    g.FillEllipse(brush, 0, 0, size - 1, size - 1);
                using (var pen = new System.Drawing.Pen(System.Drawing.Color.White, 2))
                {
                    int pad = size / 4;
                    int mid = size / 2;
                    g.DrawLine(pen, pad, mid, size - pad, mid);
                    g.DrawLine(pen, mid, pad, mid, size - pad);
                }
            }
            return bmp;
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resource.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "EMR.Desktop.Plugins.EmrExamCategory.Resource.Lang",
                    typeof(EmrExamCategoryForm).Assembly);

                // Form title
                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                    this.Text = this.currentModule.text;
                else
                    SetLangText(v => this.Text = v, "EmrExamCategoryForm.Text");

                // Panel titles
                SetLangText(v => this.lblCatTitle.Text = v, "EmrExamCategoryForm.lblCatTitle.Text");
                SetLangText(v => this.lblRuleTitle.Text = v, "EmrExamCategoryForm.lblRuleTitle.Text");

                // Buttons
                SetLangText(v => this.btnRefresh.Text = v, "EmrExamCategoryForm.btnRefresh.Text");
                SetLangText(v => this.btnSave.Text = v, "EmrExamCategoryForm.btnSave.Text");
                SetLangText(v => this.bbtnSave.Caption = v, "EmrExamCategoryForm.bbtnSave.Caption");
                SetLangText(v => this.bbtnRefresh.Caption = v, "EmrExamCategoryForm.bbtnRefresh.Caption");
                SetLangText(v => this.btnCatAdd.ToolTip = v, "EmrExamCategoryForm.btnCatAdd.ToolTip");
                SetLangText(v => this.btnRuleAdd.ToolTip = v, "EmrExamCategoryForm.btnRuleAdd.ToolTip");

                // Category grid columns
                SetLangText(v => this.gcolCatNumOrder.Caption = v, "EmrExamCategoryForm.gcolCatNumOrder.Caption");
                SetLangText(v => this.gcolCatCode.Caption = v, "EmrExamCategoryForm.gcolCatCode.Caption");
                SetLangText(v => this.gcolCatName.Caption = v, "EmrExamCategoryForm.gcolCatName.Caption");
                SetLangText(v => this.gcolCatCreateTime.Caption = v, "EmrExamCategoryForm.gcolCatCreateTime.Caption");
                SetLangText(v => this.gcolCatCreator.Caption = v, "EmrExamCategoryForm.gcolCatCreator.Caption");
                SetLangText(v => this.gcolCatModifyTime.Caption = v, "EmrExamCategoryForm.gcolCatModifyTime.Caption");
                SetLangText(v => this.gcolCatModifier.Caption = v, "EmrExamCategoryForm.gcolCatModifier.Caption");

                // Rule grid columns
                SetLangText(v => this.gcolRuleNumOrder.Caption = v, "EmrExamCategoryForm.gcolRuleNumOrder.Caption");
                SetLangText(v => this.gcolRulePattern.Caption = v, "EmrExamCategoryForm.gcolRulePattern.Caption");
                SetLangText(v => this.gcolRuleMatchType.Caption = v, "EmrExamCategoryForm.gcolRuleMatchType.Caption");
                SetLangText(v => this.gcolRuleKeyExtractor.Caption = v, "EmrExamCategoryForm.gcolRuleKeyExtractor.Caption");
                SetLangText(v => this.gcolRuleCreateTime.Caption = v, "EmrExamCategoryForm.gcolRuleCreateTime.Caption");
                SetLangText(v => this.gcolRuleCreator.Caption = v, "EmrExamCategoryForm.gcolRuleCreator.Caption");
                SetLangText(v => this.gcolRuleModifyTime.Caption = v, "EmrExamCategoryForm.gcolRuleModifyTime.Caption");
                SetLangText(v => this.gcolRuleModifier.Caption = v, "EmrExamCategoryForm.gcolRuleModifier.Caption");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Apply language value only when lookup succeeds.
        /// Keeps Designer-set fallback text when resx key missing or empty.
        /// </summary>
        private static void SetLangText(Action<string> setter, string key)
        {
            var val = GetLang(key);
            if (!string.IsNullOrEmpty(val))
                setter(val);
        }

        private static string GetLang(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    Resource.ResourceLanguageManager.LanguageResource,
                    LanguageManager.GetCulture());
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return "";
        }
        #endregion

        // ====================================================================
        // HELPERS
        // ====================================================================

        #region Helpers
        private EMR_EXAM_CATEGORY GetSelectedCategory()
        {
            try
            {
                if (gridViewCat.FocusedRowHandle >= 0)
                    return (EMR_EXAM_CATEGORY)gridViewCat.GetFocusedRow();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
            return null;
        }

        private void gridViewCat_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            try
            {
                BindRuleGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        // ====================================================================
        // CATEGORY: DATA LOADING
        // ====================================================================

        #region Category - Load
        private void LoadAllCategories()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                EmrExamCategoryFilter filter = new EmrExamCategoryFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "NUM_ORDER";
                var result = new BackendAdapter(param).Get<List<EMR_EXAM_CATEGORY>>(EmrRequestUriStore.EMR_EXAM_CATEGORY_GET, ApiConsumers.EmrConsumer, filter, param);
                listCategory = result ?? new List<EMR_EXAM_CATEGORY>();
                foreach (var item in listCategory)
                {
                    item.ROW_STATE = RowState.UNCHANGED;
                }
                BindCategoryGrid();
                InitRuleLookupData();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void BindCategoryGrid()
        {
            try
            {
                gridViewCat.BeginUpdate();
                gridControlCat.DataSource = null;
                gridControlCat.DataSource = listCategory.Where(c => c.ROW_STATE != RowState.DELETED).OrderBy(c => c.NUM_ORDER).ToList();
                gridViewCat.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        // ====================================================================
        // CATEGORY: GRID EVENTS
        // ====================================================================

        #region Category - Grid Events
        private void gridViewCat_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "CatDelete")
                {
                    EMR_EXAM_CATEGORY data = (EMR_EXAM_CATEGORY)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnCatDeleteEnable : btnCatDeleteDisable);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewCat_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    EMR_EXAM_CATEGORY pData = (EMR_EXAM_CATEGORY)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "CAT_MODIFY_TIME_STR")
                    {
                        e.Value = pData.MODIFY_TIME.HasValue
                            ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME.Value)
                            : "";
                    }
                    else if (e.Column.FieldName == "CAT_CREATE_TIME_STR")
                    {
                        e.Value = pData.CREATE_TIME.HasValue
                            ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME.Value)
                            : "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewCat_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var data = (EMR_EXAM_CATEGORY)gridViewCat.GetRow(e.RowHandle);
                if (data == null) return;

                if (data.ROW_STATE == RowState.UNCHANGED)
                    data.ROW_STATE = RowState.UPDATED;

                if (e.Column.FieldName == "NUM_ORDER")
                {
                    CatReorderAfterChange(data);
                    BindCategoryGrid();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCatAdd_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewCat.CloseEditor();
                gridViewCat.UpdateCurrentRow();

                long maxOrder = listCategory.Where(c => c.ROW_STATE != RowState.DELETED).Count() + 1;
                var newItem = new EMR_EXAM_CATEGORY
                {
                    ID = tempCategoryIdCounter--,
                    CATEGORY_CODE = "",
                    CATEGORY_NAME = "",
                    NUM_ORDER = maxOrder,
                    IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE,
                    IS_DELETE = 0,
                    ROW_STATE = RowState.NEW
                };
                listCategory.Add(newItem);
                BindCategoryGrid();


                // Focus the new row for editing
                int lastRow = gridViewCat.RowCount - 1;
                if (lastRow >= 0)
                {
                    gridViewCat.FocusedRowHandle = lastRow;
                    gridViewCat.FocusedColumn = gcolCatCode;
                    gridViewCat.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnCatDeleteEnable_Click(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (EMR_EXAM_CATEGORY)gridViewCat.GetFocusedRow();
                    if (rowData != null)
                    {
                        if (rowData.ROW_STATE == RowState.NEW)
                        {
                            // Item chưa persist — xóa khỏi list, cascade xóa rules NEW tham chiếu
                            listRule.RemoveAll(r => r.EXAM_CATEGORY_ID == rowData.ID);
                            listCategory.Remove(rowData);
                            BindCategoryGrid();
                            BindRuleGrid();
                        }
                        else
                        {
                            // Item đã có DB — gọi API Delete riêng lẻ NGAY LẬP TỨC
                            CommonParam param = new CommonParam();
                            WaitingManager.Show();
                            bool ok = new BackendAdapter(param).Post<bool>(
                                EmrRequestUriStore.EMR_EXAM_CATEGORY_DELETE,
                                ApiConsumers.EmrConsumer,
                                rowData.ID,
                                param);
                            WaitingManager.Hide();
                            if (ok)
                            {
                                listCategory.Remove(rowData);
                                BindCategoryGrid();
                                // BE cascade soft-delete rule → reload lại danh sách rule
                                LoadAllRules();
                            }
                            MessageManager.Show(this, param, ok);
                            SessionManager.ProcessTokenLost(param);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }
        #endregion

        // ====================================================================
        // CATEGORY: NUM_ORDER AUTO REORDER
        // ====================================================================

        #region Category - Reorder
        private void CatReorderAfterChange(EMR_EXAM_CATEGORY changedItem)
        {
            try
            {
                var activeItems = listCategory.Where(x => x.ROW_STATE != RowState.DELETED).OrderBy(x => x.NUM_ORDER).ToList();
                int totalCount = activeItems.Count;
                long newOrder = changedItem.NUM_ORDER;
                if (newOrder < 1) newOrder = 1;
                if (newOrder > totalCount) newOrder = totalCount;
                changedItem.NUM_ORDER = newOrder;

                activeItems.Remove(changedItem);
                int insertIdx = (int)newOrder - 1;
                if (insertIdx > activeItems.Count) insertIdx = activeItems.Count;
                activeItems.Insert(insertIdx, changedItem);

                for (int i = 0; i < activeItems.Count; i++)
                {
                    if (activeItems[i].NUM_ORDER != (i + 1))
                    {
                        activeItems[i].NUM_ORDER = i + 1;
                        if (activeItems[i].ROW_STATE == RowState.UNCHANGED)
                            activeItems[i].ROW_STATE = RowState.UPDATED;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        // ====================================================================
        // CATEGORY: DRAG & DROP
        // ====================================================================

        #region Category - Drag & Drop
        private void gridControlCat_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                catDragHitInfo = gridViewCat.CalcHitInfo(new Point(e.X, e.Y));
                if (!catDragHitInfo.InRow || catDragHitInfo.RowHandle < 0)
                    catDragHitInfo = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlCat_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left || catDragHitInfo == null) return;
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(
                    new Point(catDragHitInfo.HitPoint.X - dragSize.Width / 2,
                              catDragHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    gridControlCat.DoDragDrop(catDragHitInfo.RowHandle, DragDropEffects.Move);
                    catDragHitInfo = null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlCat_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = DragDropEffects.Move;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlCat_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                int sourceRowHandle = (int)e.Data.GetData(typeof(int));
                Point clientPoint = gridControlCat.PointToClient(new Point(e.X, e.Y));
                var targetHitInfo = gridViewCat.CalcHitInfo(clientPoint);
                int targetRowHandle = targetHitInfo.RowHandle;
                if (sourceRowHandle == targetRowHandle || targetRowHandle < 0) return;

                var sourceItem = (EMR_EXAM_CATEGORY)gridViewCat.GetRow(sourceRowHandle);
                var targetItem = (EMR_EXAM_CATEGORY)gridViewCat.GetRow(targetRowHandle);
                if (sourceItem == null || targetItem == null) return;

                sourceItem.NUM_ORDER = targetItem.NUM_ORDER;
                if (sourceItem.ROW_STATE == RowState.UNCHANGED)
                    sourceItem.ROW_STATE = RowState.UPDATED;
                CatReorderAfterChange(sourceItem);
                BindCategoryGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        // ====================================================================
        // RULE: DATA LOADING
        // ====================================================================

        #region Rule - Load
        private void LoadAllRules()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                EmrDocumentPairRuleFilter filter = new EmrDocumentPairRuleFilter();
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_FIELD = "NUM_ORDER";
                var result = new BackendAdapter(param).Get<List<EMR_DOCUMENT_PAIR_RULE>>(EmrRequestUriStore.EMR_DOCUMENT_PAIR_RULE_GET, ApiConsumers.EmrConsumer, filter, param);
                listRule = result ?? new List<EMR_DOCUMENT_PAIR_RULE>();
                foreach (var item in listRule)
                {
                    item.ROW_STATE = RowState.UNCHANGED;
                }
                BindRuleGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void BindRuleGrid()
        {
            try
            {
                var selectedCat = GetSelectedCategory();
                List<EMR_DOCUMENT_PAIR_RULE> filtered;

                if (selectedCat != null)
                {
                    // Only show rules for the selected category
                    filtered = listRule
                        .Where(r => r.ROW_STATE != RowState.DELETED && r.EXAM_CATEGORY_ID == selectedCat.ID)
                        .OrderBy(r => r.NUM_ORDER).ToList();
                }
                else
                {
                    // No category selected → rule grid is empty
                    filtered = new List<EMR_DOCUMENT_PAIR_RULE>();
                }

                gridViewRule.BeginUpdate();
                gridControlRule.DataSource = null;
                gridControlRule.DataSource = filtered;
                gridViewRule.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitRuleLookupData()
        {
            try
            {
                riRuleExamCategoryId.DataSource = listCategory.Where(c => c.ROW_STATE != RowState.DELETED).ToList();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        // ====================================================================
        // RULE: GRID EVENTS
        // ====================================================================

        #region Rule - Grid Events
        private void gridViewRule_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "RuleDelete")
                {
                    EMR_DOCUMENT_PAIR_RULE data = (EMR_DOCUMENT_PAIR_RULE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    e.RepositoryItem = (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE ? btnRuleDeleteEnable : btnRuleDeleteDisable);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewRule_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    EMR_DOCUMENT_PAIR_RULE pData = (EMR_DOCUMENT_PAIR_RULE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "RULE_MODIFY_TIME_STR")
                    {
                        e.Value = pData.MODIFY_TIME.HasValue
                            ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.MODIFY_TIME.Value)
                            : "";
                    }
                    else if (e.Column.FieldName == "RULE_CREATE_TIME_STR")
                    {
                        e.Value = pData.CREATE_TIME.HasValue
                            ? Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME.Value)
                            : "";
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewRule_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0) return;
                var data = (EMR_DOCUMENT_PAIR_RULE)gridViewRule.GetRow(e.RowHandle);
                if (data == null) return;

                if (data.ROW_STATE == RowState.UNCHANGED)
                    data.ROW_STATE = RowState.UPDATED;

                if (e.Column.FieldName == "NUM_ORDER")
                {
                    RuleReorderAfterChange(data);
                    BindRuleGrid();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnRuleAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedCat = GetSelectedCategory();
                if (selectedCat == null)
                {
                    ShowWarning(Resource.ResourceMessage.SelectCategoryBeforeAddRule);
                    return;
                }

                gridViewRule.CloseEditor();
                gridViewRule.UpdateCurrentRow();

                // NUM_ORDER only within the same category
                long maxOrder = listRule.Where(r => r.EXAM_CATEGORY_ID == selectedCat.ID && r.ROW_STATE != RowState.DELETED).Count() + 1;
                var newItem = new EMR_DOCUMENT_PAIR_RULE
                {
                    EXAM_CATEGORY_ID = selectedCat.ID,
                    PATTERN = "",
                    MATCH_TYPE = "PREFIX",
                    KEY_EXTRACTOR = "",
                    NUM_ORDER = maxOrder,
                    IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE,
                    IS_DELETE = 0,
                    ROW_STATE = RowState.NEW
                };
                listRule.Add(newItem);
                BindRuleGrid();

                // Focus the new row for editing
                int lastRow = gridViewRule.RowCount - 1;
                if (lastRow >= 0)
                {
                    gridViewRule.FocusedRowHandle = lastRow;
                    gridViewRule.FocusedColumn = gcolRulePattern;
                    gridViewRule.ShowEditor();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void btnRuleDeleteEnable_Click(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (MessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonHuyDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var rowData = (EMR_DOCUMENT_PAIR_RULE)gridViewRule.GetFocusedRow();
                    if (rowData != null)
                    {
                        if (rowData.ROW_STATE == RowState.NEW)
                        {
                            // Item chưa persist — xóa khỏi list luôn
                            listRule.Remove(rowData);
                            BindRuleGrid();
                        }
                        else
                        {
                            // Item đã có DB — gọi API Delete riêng lẻ NGAY LẬP TỨC
                            CommonParam param = new CommonParam();
                            WaitingManager.Show();
                            bool ok = new BackendAdapter(param).Post<bool>(
                                EmrRequestUriStore.EMR_DOCUMENT_PAIR_RULE_DELETE,
                                ApiConsumers.EmrConsumer,
                                rowData.ID,
                                param);
                            WaitingManager.Hide();
                            if (ok)
                            {
                                listRule.Remove(rowData);
                                BindRuleGrid();
                            }
                            MessageManager.Show(this, param, ok);
                            SessionManager.ProcessTokenLost(param);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }
        #endregion

        // ====================================================================
        // RULE: NUM_ORDER AUTO REORDER
        // ====================================================================

        #region Rule - Reorder
        private void RuleReorderAfterChange(EMR_DOCUMENT_PAIR_RULE changedItem)
        {
            try
            {
                // Only reorder rules within the same category
                var activeItems = listRule
                    .Where(x => x.EXAM_CATEGORY_ID == changedItem.EXAM_CATEGORY_ID && x.ROW_STATE != RowState.DELETED)
                    .OrderBy(x => x.NUM_ORDER).ToList();
                int totalCount = activeItems.Count;
                long newOrder = changedItem.NUM_ORDER;
                if (newOrder < 1) newOrder = 1;
                if (newOrder > totalCount) newOrder = totalCount;
                changedItem.NUM_ORDER = newOrder;

                activeItems.Remove(changedItem);
                int insertIdx = (int)newOrder - 1;
                if (insertIdx > activeItems.Count) insertIdx = activeItems.Count;
                activeItems.Insert(insertIdx, changedItem);

                for (int i = 0; i < activeItems.Count; i++)
                {
                    if (activeItems[i].NUM_ORDER != (i + 1))
                    {
                        activeItems[i].NUM_ORDER = i + 1;
                        if (activeItems[i].ROW_STATE == RowState.UNCHANGED)
                            activeItems[i].ROW_STATE = RowState.UPDATED;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        // ====================================================================
        // RULE: DRAG & DROP
        // ====================================================================

        #region Rule - Drag & Drop
        private void gridControlRule_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                ruleDragHitInfo = gridViewRule.CalcHitInfo(new Point(e.X, e.Y));
                if (!ruleDragHitInfo.InRow || ruleDragHitInfo.RowHandle < 0)
                    ruleDragHitInfo = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlRule_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Left || ruleDragHitInfo == null) return;
                Size dragSize = SystemInformation.DragSize;
                Rectangle dragRect = new Rectangle(
                    new Point(ruleDragHitInfo.HitPoint.X - dragSize.Width / 2,
                              ruleDragHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
                if (!dragRect.Contains(new Point(e.X, e.Y)))
                {
                    gridControlRule.DoDragDrop(ruleDragHitInfo.RowHandle, DragDropEffects.Move);
                    ruleDragHitInfo = null;
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlRule_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = DragDropEffects.Move;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void gridControlRule_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                int sourceRowHandle = (int)e.Data.GetData(typeof(int));
                Point clientPoint = gridControlRule.PointToClient(new Point(e.X, e.Y));
                var targetHitInfo = gridViewRule.CalcHitInfo(clientPoint);
                int targetRowHandle = targetHitInfo.RowHandle;
                if (sourceRowHandle == targetRowHandle || targetRowHandle < 0) return;

                var sourceItem = (EMR_DOCUMENT_PAIR_RULE)gridViewRule.GetRow(sourceRowHandle);
                var targetItem = (EMR_DOCUMENT_PAIR_RULE)gridViewRule.GetRow(targetRowHandle);
                if (sourceItem == null || targetItem == null) return;
                if (sourceItem.EXAM_CATEGORY_ID != targetItem.EXAM_CATEGORY_ID) return;

                RuleReorderByTarget(sourceItem, targetItem);
                BindRuleGrid();
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }

        private void RuleReorderByTarget(EMR_DOCUMENT_PAIR_RULE sourceItem, EMR_DOCUMENT_PAIR_RULE targetItem)
        {
            try
            {
                var activeItems = listRule
                    .Where(x => x.EXAM_CATEGORY_ID == sourceItem.EXAM_CATEGORY_ID && x.ROW_STATE != RowState.DELETED)
                    .OrderBy(x => x.NUM_ORDER).ToList();

                activeItems.Remove(sourceItem);
                int targetIdx = activeItems.IndexOf(targetItem);
                if (targetIdx < 0) targetIdx = activeItems.Count;
                activeItems.Insert(targetIdx, sourceItem);

                for (int i = 0; i < activeItems.Count; i++)
                {
                    long newOrder = i + 1;
                    if (activeItems[i].NUM_ORDER != newOrder)
                    {
                        activeItems[i].NUM_ORDER = newOrder;
                        if (activeItems[i].ROW_STATE == RowState.UNCHANGED)
                            activeItems[i].ROW_STATE = RowState.UPDATED;
                    }
                }
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion

        // ====================================================================
        // SAVE ALL + REFRESH → xem EmrExamCategoryForm__Save.cs
        // ====================================================================

        // ====================================================================
        // DISPOSE
        // ====================================================================

        #region Dispose
        public override void ProcessDisposeModuleDataAfterClose()
        {
            try
            {
                listCategory = null;
                listRule = null;
                delegateSelect = null;
                currentModule = null;
            }
            catch (Exception ex) { LogSystem.Warn(ex); }
        }
        #endregion
    }
}
