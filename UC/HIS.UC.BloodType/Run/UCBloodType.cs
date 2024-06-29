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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList.Data;
using System.Collections;
using DevExpress.XtraTreeList;
using DevExpress.Utils.Menu;
using DevExpress.XtraTreeList.Nodes;
using HIS.UC.BloodType.ADO;

namespace HIS.UC.BloodType.Run
{
    public partial class UCBloodType : UserControl
    {
        #region Declare
        BloodTypeInitADO BloodTypeADO;
        BindingList<BloodTypeADO> records;
        List<BloodTypeADO> BloodTypeADOs = new List<BloodTypeADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        BloodType_NodeCellStyle BloodTypeNodeCellStyle;
        BloodType_CustomUnboundColumnData BloodType_CustomUnboundColumnData;
        BloodTypeHandler BloodTypeClick;
        BloodTypeHandler BloodTypeDoubleClick;
        BloodTypeHandler BloodTypeRowEnter;
        BloodType_GetStateImage BloodType_GetStateImage;
        BloodType_GetSelectImage BloodType_GetSelectImage;
        BloodTypeHandler BloodType_StateImageClick;
        BloodTypeHandler BloodType_SelectImageClick;
        BloodType_AfterCheck BloodType_AfterCheck;
        BloodType_BeforeCheck BloodType_BeforeCheck;
        BloodType_CheckAllNode BloodType_CheckAllNode;
        BloodType_CustomDrawNodeCell BloodType_CustomDrawNodeCell;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        BloodTypeHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithBloodTypeExpend = true;
        #endregion

        #region Construct
        public UCBloodType(BloodTypeInitADO BloodTypeADO)
        {
            InitializeComponent();
            try
            {
                this.BloodTypeADO = BloodTypeADO;
                this.BloodTypeNodeCellStyle = BloodTypeADO.BloodTypeNodeCellStyle;
                this.BloodTypeClick = BloodTypeADO.BloodTypeClick;
                this.BloodTypeDoubleClick = BloodTypeADO.BloodTypeDoubleClick;
                this.BloodTypeRowEnter = BloodTypeADO.BloodTypeRowEnter;
                this.BloodType_GetStateImage = BloodTypeADO.BloodType_GetStateImage;
                this.BloodType_GetSelectImage = BloodTypeADO.BloodType_GetSelectImage;
                this.BloodType_StateImageClick = BloodTypeADO.BloodType_StateImageClick;
                this.BloodType_SelectImageClick = BloodTypeADO.BloodType_SelectImageClick;
                this.columnButtonEdits = BloodTypeADO.ColumnButtonEdits;
                this.selectImageCollection = BloodTypeADO.SelectImageCollection;
                this.stateImageCollection = BloodTypeADO.StateImageCollection;
                this.updateSingleRow = BloodTypeADO.UpdateSingleRow;
                this.BloodType_CustomUnboundColumnData = BloodTypeADO.BloodType_CustomUnboundColumnData;
                this.menuItems = BloodTypeADO.MenuItems;
                this.BloodType_AfterCheck = BloodTypeADO.BloodType_AfterCheck;
                this.BloodType_BeforeCheck = BloodTypeADO.BloodType_BeforeCheck;
                this.BloodType_CheckAllNode = BloodTypeADO.BloodType_CheckAllNode;
                this.BloodType_CustomDrawNodeCell = BloodTypeADO.BloodType_CustomDrawNodeCell;
                if (BloodTypeADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = BloodTypeADO.IsShowCheckNode.Value;
                }
                if (BloodTypeADO.IsCreateParentNodeWithBloodTypeExpend.HasValue)
                {
                    this.isCreateParentNodeWithBloodTypeExpend = BloodTypeADO.IsCreateParentNodeWithBloodTypeExpend.Value;
                }
                if (BloodTypeADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = BloodTypeADO.IsAutoWidth.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Private method
        private void UCServiceTree_Load(object sender, EventArgs e)
        {
            try
            {
                if (BloodTypeADO != null)
                {
                    InitializeTree();
                    BindTreePlus();
                    if (this.stateImageCollection != null)
                    {
                        trvService.StateImageList = this.stateImageCollection;
                    }
                    if (this.selectImageCollection != null)
                    {
                        trvService.SelectImageList = this.selectImageCollection;
                    }
                    SetVisibleSearchPanel();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BindTreePlus()
        {
            try
            {
                BloodTypeADOs = new List<BloodTypeADO>();
                if (BloodTypeADO.BloodTypes != null)
                {
                    BloodTypeADOs = (from r in BloodTypeADO.BloodTypes select new BloodTypeADO(r)).ToList();
                    BloodTypeADOs = BloodTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.BLOOD_TYPE_NAME).ToList();
                }
                records = new BindingList<BloodTypeADO>(BloodTypeADOs);
                if (!String.IsNullOrEmpty(BloodTypeADO.KeyFieldName))
                    trvService.KeyFieldName = BloodTypeADO.KeyFieldName;
                if (!String.IsNullOrEmpty(BloodTypeADO.ParentFieldName))
                    trvService.ParentFieldName = BloodTypeADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.BloodType_CheckAllNode != null)
                    this.BloodType_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeTree()
        {
            try
            {
                this.trvService.OptionsView.ShowCheckBoxes = this.IsShowCheckNode;
                this.trvService.OptionsView.AutoWidth = this.isAutoWidth;
                if (BloodTypeADO.BloodTypeColumns != null && BloodTypeADO.BloodTypeColumns.Count > 0)
                {
                    foreach (var svtr in BloodTypeADO.BloodTypeColumns)
                    {
                        TreeListColumn col = this.trvService.Columns.AddField(svtr.FieldName);
                        col.Visible = svtr.Visible;
                        col.VisibleIndex = svtr.VisibleIndex;
                        col.Width = svtr.ColumnWidth;
                        col.FieldName = svtr.FieldName;
                        col.OptionsColumn.AllowEdit = svtr.AllowEdit;
                        col.Caption = svtr.Caption;
                        if (svtr.UnboundColumnType != null && svtr.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = svtr.UnboundColumnType;
                        if (svtr.Format != null)
                        {
                            col.Format.FormatString = svtr.Format.FormatString;
                            col.Format.FormatType = svtr.Format.FormatType;
                        }
                    }
                }
                if (BloodTypeADO.ColumnButtonEdits != null && BloodTypeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in BloodTypeADO.ColumnButtonEdits)
                    {
                        RepositoryItemButtonEdit buttonEdit = new RepositoryItemButtonEdit();
                        buttonEdit.AutoHeight = false;
                        buttonEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
                        DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
                        superToolTip.Items.Add(new DevExpress.Utils.ToolTipItem());
                        buttonEdit.Tag = svtr.ActionHandler;
                        buttonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph,svtr.Image,superToolTip)});
                        buttonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(RepositoryItemButtonClick);
                        DevExpress.XtraTreeList.Columns.TreeListColumn treeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();

                        treeListColumn.ColumnEdit = buttonEdit;
                        treeListColumn.Width = 20;
                        treeListColumn.ToolTip = svtr.Tooltip;
                        treeListColumn.VisibleIndex = svtr.VisibleIndex;
                        treeListColumn.Caption = svtr.Caption;
                        treeListColumn.ImageAlignment = StringAlignment.Center;
                        trvService.Columns.Add(treeListColumn);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetVisibleSearchPanel()
        {
            try
            {
                if (BloodTypeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (BloodTypeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (BloodTypeADO.IsShowButtonAdd.HasValue)
                {
                    lciBloodTypeAdd.Visibility = (BloodTypeADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(BloodTypeADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = BloodTypeADO.Keyword_NullValuePrompt;
                        txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
                        txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Down)
                {
                    trvService.Focus();
                    trvService.FocusedNode = trvService.Nodes[1];
                    return;
                }
                string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                SearchClick(strValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SearchClick(string keyword)
        {
            try
            {
                BindingList<BloodTypeADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<BloodTypeADO> rearchResult = new List<BloodTypeADO>();

                    rearchResult = BloodTypeADOs.Where(o =>
                                                    ((o.BLOOD_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.BLOOD_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.HEIN_SERVICE_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<BloodTypeADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<BloodTypeADO>(BloodTypeADOs);
                }
                trvService.DataSource = listResult;
                trvService.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                Search();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                SearchClick(txtKeyword.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_BLOOD_TYPE> BloodTypes)
        {
            try
            {
                txtKeyword.Text = "";
                this.BloodTypeADO.BloodTypes = BloodTypes;
                if (this.BloodTypeADO.BloodTypes == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (BloodTypeNodeCellStyle != null)
                {
                    BloodTypeNodeCellStyle(data, e.Appearance);
                }
                if (e.Node.Focused)
                {
                    e.Appearance.BackColor = Color.Blue;
                    e.Appearance.ForeColor = Color.White;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var node = trvService.FocusedNode;
                    var data = trvService.GetDataRecordByNode(node);
                    if (node != null && data != null && data is BloodTypeADO)
                    {
                        if (BloodTypeRowEnter != null)
                        {
                            BloodTypeRowEnter((BloodTypeADO)data);
                        }
                        else
                        {
                            txtKeyword.Focus();
                            txtKeyword.SelectAll();
                        }
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = trvService.FocusedNode;
                    var data = trvService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is BloodTypeADO)
                    {
                        node.Expanded = !node.Expanded;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);
                TreeListNode node = e.Node;
                if (this.BloodType_BeforeCheck != null)
                    this.BloodType_BeforeCheck(node);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                if (this.BloodType_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is BloodTypeADO)
                {
                    BloodType_AfterCheck(e.Node, (BloodTypeADO)row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RepositoryItemButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (sender != null && sender is DevExpress.XtraEditors.ButtonEdit)
                {
                    var btn = sender as DevExpress.XtraEditors.ButtonEdit;
                    if (btn.Tag != null)
                    {
                        BloodTypeHandler clickhandler = btn.Tag as BloodTypeHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is BloodTypeADO)
                            {
                                clickhandler((BloodTypeADO)data);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_GetSelectImage(object sender, DevExpress.XtraTreeList.GetSelectImageEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    if (this.BloodType_GetSelectImage != null)
                        this.BloodType_GetSelectImage((BloodTypeADO)data, e);
                    else
                        e.NodeImageIndex = -1;
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_GetStateImage(object sender, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    if (this.BloodType_GetStateImage != null)
                        this.BloodType_GetStateImage((BloodTypeADO)data, e);
                    else
                        e.NodeImageIndex = -1;
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_SelectImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    if (this.BloodType_SelectImageClick != null)
                        this.BloodType_SelectImageClick((BloodTypeADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_StateImageClick(object sender, DevExpress.XtraTreeList.NodeClickEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    if (this.BloodType_StateImageClick != null)
                        this.BloodType_StateImageClick((BloodTypeADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomUnboundColumnData(object sender, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    BloodTypeADO currentRow = e.Row as BloodTypeADO;
                    if (currentRow == null) return;
                    if (BloodType_CustomUnboundColumnData != null)
                        this.BloodType_CustomUnboundColumnData(currentRow, e);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    var rowData = data as BloodTypeADO;

                    if (e.Column.FieldName == "IsLeaf")
                    {
                        if (this.updateSingleRow != null)
                        {
                            e.RepositoryItem = repositoryItemchkIsExpend__Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemchkIsExpend__Disable;
                        }

                        if (rowData != null && rowData.IS_LEAF == 1)
                        {
                            rowData.IsLeaf = true;
                        }
                        else
                        {
                            rowData.IsLeaf = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemchkIsExpend__Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                if (data != null && data is BloodTypeADO)
                {
                    var rowData = data as BloodTypeADO;
                    if (rowData.IsLeaf != null && rowData.IsLeaf.Value)
                        rowData.IS_LEAF = 1;
                    else
                        rowData.IS_LEAF = null;
                    if (this.updateSingleRow != null)
                    {
                        updateSingleRow(rowData);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
        {
            try
            {
                if (this.menuItems != null)
                {
                    TreeList tree = sender as TreeList;
                    if (tree != null)
                    {
                        Point pt = tree.PointToClient(MousePosition);
                        TreeListHitInfo hitInfo = tree.CalcHitInfo(e.Point);
                        if (hitInfo != null && (hitInfo.HitInfoType == HitInfoType.Row
                            || hitInfo.HitInfoType == HitInfoType.Cell))
                        {
                            e.Menu.Items.Clear();
                            tree.FocusedNode = hitInfo.Node;
                            var data = trvService.GetDataRecordByNode(hitInfo.Node);
                            if (data != null && data is BloodTypeADO)
                            {
                                foreach (var menu in this.menuItems((BloodTypeADO)data))
                                {
                                    e.Menu.Items.Add(menu);
                                }
                                e.Menu.Show(pt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void clearAllMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                TreeListColumn clickedColumn = (sender as DXMenuItem).Tag as TreeListColumn;
                if (clickedColumn == null) return;
                TreeList tl = clickedColumn.TreeList;
                foreach (TreeListColumn column in tl.Columns)
                    column.SummaryFooter = SummaryItemType.None;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is BloodTypeADO)
                {
                    var rowData = data as BloodTypeADO;
                    if (rowData != null && this.BloodType_CustomDrawNodeCell != null)
                    {
                        this.BloodType_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<BloodTypeADO> GetListCheck()
        {
            List<BloodTypeADO> result = new List<BloodTypeADO>();
            try
            {
                foreach (TreeListNode node in trvService.Nodes)
                {
                    GetListNodeCheck(ref result, node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<BloodTypeADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<BloodTypeADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((BloodTypeADO)trvService.GetDataRecordByNode(node));
                    }
                }
                else
                {
                    foreach (TreeListNode child in node.Nodes)
                    {
                        GetListNodeCheck(ref result, child);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                if (hi.Node != null)
                {
                    BloodTypeADO BloodTypeFocus = (BloodTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (BloodTypeFocus != null && BloodTypeDoubleClick != null)
                    {
                        BloodTypeDoubleClick(BloodTypeFocus);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_Click(object sender, EventArgs e)
        {
            try
            {
                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                if (hi.Node != null)
                {
                    BloodTypeADO BloodTypeFocus = (BloodTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (BloodTypeFocus != null && this.BloodTypeClick != null)
                    {
                        this.BloodTypeClick(BloodTypeFocus);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {

        }

        internal void FocusKeyword()
        {
            try
            {
                txtKeyword.Focus();
                txtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var listdata = trvService.DataSource as BindingList<BloodTypeADO>;
                    if (listdata != null && listdata.Count > 0)
                    {
                        trvService.Focus();
                        KeyEventArgs key = new KeyEventArgs(Keys.Enter);
                        trvService_KeyDown(null, key);
                    }
                    else
                    {
                        txtKeyword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}
