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
using HIS.UC.HisBlood.ADO;

namespace HIS.UC.HisBlood.Run
{
    public partial class UCHisBlood : UserControl
    {
        #region Declare
        HisBloodInitADO HisBloodADO;
        BindingList<HisBloodADO> records;
        List<HisBloodADO> HisBloodADOs = new List<HisBloodADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisBlood_NodeCellStyle HisBloodNodeCellStyle;
        HisBlood_CustomUnboundColumnData HisBlood_CustomUnboundColumnData;
        HisBloodHandler HisBloodClick;
        HisBloodHandler HisBloodDoubleClick;
        HisBloodHandler HisBloodRowEnter;
        HisBlood_GetStateImage HisBlood_GetStateImage;
        HisBlood_GetSelectImage HisBlood_GetSelectImage;
        HisBloodHandler HisBlood_StateImageClick;
        HisBloodHandler HisBlood_SelectImageClick;
        HisBlood_AfterCheck HisBlood_AfterCheck;
        HisBlood_BeforeCheck HisBlood_BeforeCheck;
        HisBlood_CheckAllNode HisBlood_CheckAllNode;
        HisBlood_CustomDrawNodeCell HisBlood_CustomDrawNodeCell;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisBloodHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisBloodExpend = true;
        #endregion

        #region Construct
        public UCHisBlood(HisBloodInitADO HisBloodADO)
        {
            InitializeComponent();
            try
            {
                this.HisBloodADO = HisBloodADO;
                this.HisBloodNodeCellStyle = HisBloodADO.HisBloodNodeCellStyle;
                this.HisBloodClick = HisBloodADO.HisBloodClick;
                this.HisBloodDoubleClick = HisBloodADO.HisBloodDoubleClick;
                this.HisBloodRowEnter = HisBloodADO.HisBloodRowEnter;
                this.HisBlood_GetStateImage = HisBloodADO.HisBlood_GetStateImage;
                this.HisBlood_GetSelectImage = HisBloodADO.HisBlood_GetSelectImage;
                this.HisBlood_StateImageClick = HisBloodADO.HisBlood_StateImageClick;
                this.HisBlood_SelectImageClick = HisBloodADO.HisBlood_SelectImageClick;
                this.columnButtonEdits = HisBloodADO.ColumnButtonEdits;
                this.selectImageCollection = HisBloodADO.SelectImageCollection;
                this.stateImageCollection = HisBloodADO.StateImageCollection;
                this.updateSingleRow = HisBloodADO.UpdateSingleRow;
                this.HisBlood_CustomUnboundColumnData = HisBloodADO.HisBlood_CustomUnboundColumnData;
                this.menuItems = HisBloodADO.MenuItems;
                this.HisBlood_AfterCheck = HisBloodADO.HisBlood_AfterCheck;
                this.HisBlood_BeforeCheck = HisBloodADO.HisBlood_BeforeCheck;
                this.HisBlood_CheckAllNode = HisBloodADO.HisBlood_CheckAllNode;
                this.HisBlood_CustomDrawNodeCell = HisBloodADO.HisBlood_CustomDrawNodeCell;
                if (HisBloodADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisBloodADO.IsShowCheckNode.Value;
                }
                if (HisBloodADO.IsCreateParentNodeWithHisBloodExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisBloodExpend = HisBloodADO.IsCreateParentNodeWithHisBloodExpend.Value;
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
                if (HisBloodADO != null)
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
                HisBloodADOs = new List<HisBloodADO>();
                if (HisBloodADO.HisBloods != null)
                {
                    HisBloodADOs = (from r in HisBloodADO.HisBloods select new HisBloodADO(r)).ToList();
                    HisBloodADOs = HisBloodADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.BLOOD_TYPE_NAME).ToList();
                }
                records = new BindingList<HisBloodADO>(HisBloodADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.HisBlood_CheckAllNode != null)
                    this.HisBlood_CheckAllNode(trvService.Nodes);
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
                if (HisBloodADO.HisBloodColumns != null && HisBloodADO.HisBloodColumns.Count > 0)
                {
                    foreach (var svtr in HisBloodADO.HisBloodColumns)
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
                if (HisBloodADO.ColumnButtonEdits != null && HisBloodADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisBloodADO.ColumnButtonEdits)
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
                if (HisBloodADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisBloodADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisBloodADO.IsShowButtonAdd.HasValue)
                {
                    lciHisBloodAdd.Visibility = (HisBloodADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                BindingList<HisBloodADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisBloodADO> rearchResult = new List<HisBloodADO>();

                    rearchResult = HisBloodADOs.Where(o =>
                                                    ((o.BLOOD_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.BLOOD_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    //|| (o.HEIN_SERVICE_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    //|| (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<HisBloodADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisBloodADO>(HisBloodADOs);
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_BLOOD> HisBloods)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisBloodADO.HisBloods = HisBloods;
                if (this.HisBloodADO.HisBloods == null)
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
                if (HisBloodNodeCellStyle != null)
                {
                    HisBloodNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is HisBloodADO)
                    {
                        if (HisBloodRowEnter != null)
                        {
                            HisBloodRowEnter((HisBloodADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisBloodADO)
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
                if (this.HisBlood_BeforeCheck != null)
                    this.HisBlood_BeforeCheck(node);
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
                if (this.HisBlood_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisBloodADO)
                {
                    HisBlood_AfterCheck(e.Node, (HisBloodADO)row);
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
                        HisBloodHandler clickhandler = btn.Tag as HisBloodHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisBloodADO)
                            {
                                clickhandler((HisBloodADO)data);
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
                if (data != null && data is HisBloodADO)
                {
                    if (this.HisBlood_GetSelectImage != null)
                        this.HisBlood_GetSelectImage((HisBloodADO)data, e);
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
                if (data != null && data is HisBloodADO)
                {
                    if (this.HisBlood_GetStateImage != null)
                        this.HisBlood_GetStateImage((HisBloodADO)data, e);
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
                if (data != null && data is HisBloodADO)
                {
                    if (this.HisBlood_SelectImageClick != null)
                        this.HisBlood_SelectImageClick((HisBloodADO)data);
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
                if (data != null && data is HisBloodADO)
                {
                    if (this.HisBlood_StateImageClick != null)
                        this.HisBlood_StateImageClick((HisBloodADO)data);
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
                    HisBloodADO currentRow = e.Row as HisBloodADO;
                    if (currentRow == null || this.HisBlood_CustomUnboundColumnData == null) return;
                    this.HisBlood_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisBloodADO)
                {
                    var rowData = data as HisBloodADO;

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
                if (data != null && data is HisBloodADO)
                {
                    var rowData = data as HisBloodADO;
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
                            if (data != null && data is HisBloodADO)
                            {
                                foreach (var menu in this.menuItems((HisBloodADO)data))
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
                if (data != null && data is HisBloodADO)
                {
                    var rowData = data as HisBloodADO;
                    if (rowData != null && this.HisBlood_CustomDrawNodeCell != null)
                    {
                        this.HisBlood_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisBloodADO> GetListCheck()
        {
            List<HisBloodADO> result = new List<HisBloodADO>();
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
                result = new List<HisBloodADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisBloodADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisBloodADO)trvService.GetDataRecordByNode(node));
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
                    HisBloodADO HisBloodFocus = (HisBloodADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisBloodFocus != null && HisBloodDoubleClick != null)
                    {
                        HisBloodDoubleClick(HisBloodFocus);
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
                    HisBloodADO HisBloodFocus = (HisBloodADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisBloodFocus != null && this.HisBloodClick != null)
                    {
                        this.HisBloodClick(HisBloodFocus);
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
    }
}
