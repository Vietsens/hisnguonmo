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
using HIS.UC.HisMestPeriodBlty.ADO;

namespace HIS.UC.HisMestPeriodBlty.Run
{
    public partial class UCHisMestPeriodBlty : UserControl
    {
        #region Declare
        HisMestPeriodBltyInitADO HisMestPeriodBltyADO;
        BindingList<HisMestPeriodBltyADO> records;
        List<HisMestPeriodBltyADO> HisMestPeriodBltyADOs = new List<HisMestPeriodBltyADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMestPeriodBlty_NodeCellStyle HisMestPeriodBltyNodeCellStyle;
        HisMestPeriodBlty_CustomUnboundColumnData HisMestPeriodBlty_CustomUnboundColumnData;
        HisMestPeriodBltyHandler HisMestPeriodBltyClick;
        HisMestPeriodBltyHandler HisMestPeriodBltyDoubleClick;
        HisMestPeriodBltyHandler HisMestPeriodBltyRowEnter;
        HisMestPeriodBlty_GetStateImage HisMestPeriodBlty_GetStateImage;
        HisMestPeriodBlty_GetSelectImage HisMestPeriodBlty_GetSelectImage;
        HisMestPeriodBltyHandler HisMestPeriodBlty_StateImageClick;
        HisMestPeriodBltyHandler HisMestPeriodBlty_SelectImageClick;
        HisMestPeriodBlty_AfterCheck HisMestPeriodBlty_AfterCheck;
        HisMestPeriodBlty_BeforeCheck HisMestPeriodBlty_BeforeCheck;
        HisMestPeriodBlty_CheckAllNode HisMestPeriodBlty_CheckAllNode;
        HisMestPeriodBlty_CustomDrawNodeCell HisMestPeriodBlty_CustomDrawNodeCell;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMestPeriodBltyHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMestPeriodBltyExpend = true;
        #endregion

        #region Construct
        public UCHisMestPeriodBlty(HisMestPeriodBltyInitADO HisMestPeriodBltyADO)
        {
            InitializeComponent();
            try
            {
                this.HisMestPeriodBltyADO = HisMestPeriodBltyADO;
                this.HisMestPeriodBltyNodeCellStyle = HisMestPeriodBltyADO.HisMestPeriodBltyNodeCellStyle;
                this.HisMestPeriodBltyClick = HisMestPeriodBltyADO.HisMestPeriodBltyClick;
                this.HisMestPeriodBltyDoubleClick = HisMestPeriodBltyADO.HisMestPeriodBltyDoubleClick;
                this.HisMestPeriodBltyRowEnter = HisMestPeriodBltyADO.HisMestPeriodBltyRowEnter;
                this.HisMestPeriodBlty_GetStateImage = HisMestPeriodBltyADO.HisMestPeriodBlty_GetStateImage;
                this.HisMestPeriodBlty_GetSelectImage = HisMestPeriodBltyADO.HisMestPeriodBlty_GetSelectImage;
                this.HisMestPeriodBlty_StateImageClick = HisMestPeriodBltyADO.HisMestPeriodBlty_StateImageClick;
                this.HisMestPeriodBlty_SelectImageClick = HisMestPeriodBltyADO.HisMestPeriodBlty_SelectImageClick;
                this.columnButtonEdits = HisMestPeriodBltyADO.ColumnButtonEdits;
                this.selectImageCollection = HisMestPeriodBltyADO.SelectImageCollection;
                this.stateImageCollection = HisMestPeriodBltyADO.StateImageCollection;
                this.updateSingleRow = HisMestPeriodBltyADO.UpdateSingleRow;
                this.HisMestPeriodBlty_CustomUnboundColumnData = HisMestPeriodBltyADO.HisMestPeriodBlty_CustomUnboundColumnData;
                this.menuItems = HisMestPeriodBltyADO.MenuItems;
                this.HisMestPeriodBlty_AfterCheck = HisMestPeriodBltyADO.HisMestPeriodBlty_AfterCheck;
                this.HisMestPeriodBlty_BeforeCheck = HisMestPeriodBltyADO.HisMestPeriodBlty_BeforeCheck;
                this.HisMestPeriodBlty_CheckAllNode = HisMestPeriodBltyADO.HisMestPeriodBlty_CheckAllNode;
                this.HisMestPeriodBlty_CustomDrawNodeCell = HisMestPeriodBltyADO.HisMestPeriodBlty_CustomDrawNodeCell;
                if (HisMestPeriodBltyADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMestPeriodBltyADO.IsShowCheckNode.Value;
                }
                if (HisMestPeriodBltyADO.IsCreateParentNodeWithHisMestPeriodBltyExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMestPeriodBltyExpend = HisMestPeriodBltyADO.IsCreateParentNodeWithHisMestPeriodBltyExpend.Value;
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
                if (HisMestPeriodBltyADO != null)
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
                HisMestPeriodBltyADOs = new List<HisMestPeriodBltyADO>();
                if (HisMestPeriodBltyADO.HisMestPeriodBltys != null)
                {
                    HisMestPeriodBltyADOs = (from r in HisMestPeriodBltyADO.HisMestPeriodBltys select new HisMestPeriodBltyADO(r)).ToList();
                    // HisMestPeriodBltyADOs = HisMestPeriodBltyADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.BLOOD_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMestPeriodBltyADO>(HisMestPeriodBltyADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.HisMestPeriodBlty_CheckAllNode != null)
                    this.HisMestPeriodBlty_CheckAllNode(trvService.Nodes);
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
                if (HisMestPeriodBltyADO.HisMestPeriodBltyColumns != null && HisMestPeriodBltyADO.HisMestPeriodBltyColumns.Count > 0)
                {
                    foreach (var svtr in HisMestPeriodBltyADO.HisMestPeriodBltyColumns)
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
                if (HisMestPeriodBltyADO.ColumnButtonEdits != null && HisMestPeriodBltyADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMestPeriodBltyADO.ColumnButtonEdits)
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
                if (HisMestPeriodBltyADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMestPeriodBltyADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMestPeriodBltyADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMestPeriodBltyAdd.Visibility = (HisMestPeriodBltyADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                BindingList<HisMestPeriodBltyADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMestPeriodBltyADO> rearchResult = new List<HisMestPeriodBltyADO>();

                    rearchResult = HisMestPeriodBltyADOs.Where(o =>
                                                    ((o.BLOOD_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.BLOOD_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDI_STOCK_PERIOD_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.PACKING_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.SERVICE_UNIT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<HisMestPeriodBltyADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMestPeriodBltyADO>(HisMestPeriodBltyADOs);
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_BLTY> HisMestPeriodBltys)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisMestPeriodBltyADO.HisMestPeriodBltys = HisMestPeriodBltys;
                if (this.HisMestPeriodBltyADO.HisMestPeriodBltys == null)
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
                var data = (HisMestPeriodBltyADO)trvService.GetDataRecordByNode(e.Node);
                if (HisMestPeriodBltyNodeCellStyle != null && data != null)
                {
                    HisMestPeriodBltyNodeCellStyle(data, e);
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
                    if (node != null && data != null && data is HisMestPeriodBltyADO)
                    {
                        if (HisMestPeriodBltyRowEnter != null)
                        {
                            HisMestPeriodBltyRowEnter((HisMestPeriodBltyADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMestPeriodBltyADO)
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
                if (this.HisMestPeriodBlty_BeforeCheck != null)
                    this.HisMestPeriodBlty_BeforeCheck(node);
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
                if (this.HisMestPeriodBlty_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMestPeriodBltyADO)
                {
                    HisMestPeriodBlty_AfterCheck(e.Node, (HisMestPeriodBltyADO)row);
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
                        HisMestPeriodBltyHandler clickhandler = btn.Tag as HisMestPeriodBltyHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMestPeriodBltyADO)
                            {
                                clickhandler((HisMestPeriodBltyADO)data);
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    if (this.HisMestPeriodBlty_GetSelectImage != null)
                        this.HisMestPeriodBlty_GetSelectImage((HisMestPeriodBltyADO)data, e);
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    if (this.HisMestPeriodBlty_GetStateImage != null)
                        this.HisMestPeriodBlty_GetStateImage((HisMestPeriodBltyADO)data, e);
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    if (this.HisMestPeriodBlty_SelectImageClick != null)
                        this.HisMestPeriodBlty_SelectImageClick((HisMestPeriodBltyADO)data);
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    if (this.HisMestPeriodBlty_StateImageClick != null)
                        this.HisMestPeriodBlty_StateImageClick((HisMestPeriodBltyADO)data);
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
                    HisMestPeriodBltyADO currentRow = e.Row as HisMestPeriodBltyADO;
                    if (currentRow == null || this.HisMestPeriodBlty_CustomUnboundColumnData == null) return;
                    this.HisMestPeriodBlty_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    var rowData = data as HisMestPeriodBltyADO;

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

                        //if (rowData != null && rowData.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_BLOOD_TYPE.IS_LEAF__TRUE)
                        //{
                        //    rowData.IsLeaf = true;
                        //}
                        //else
                        //{
                        //    rowData.IsLeaf = false;
                        //}
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    var rowData = data as HisMestPeriodBltyADO;
                    //if (rowData.IsLeaf != null && rowData.IsLeaf.Value)
                    //    rowData.IS_LEAF = IMSys.DbConfig.HIS_RS.HIS_BLOOD_TYPE.IS_LEAF__TRUE;
                    //else
                    //    rowData.IS_LEAF = null;
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
                            if (data != null && data is HisMestPeriodBltyADO)
                            {
                                foreach (var menu in this.menuItems((HisMestPeriodBltyADO)data))
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
                if (data != null && data is HisMestPeriodBltyADO)
                {
                    var rowData = data as HisMestPeriodBltyADO;
                    if (rowData != null && this.HisMestPeriodBlty_CustomDrawNodeCell != null)
                    {
                        this.HisMestPeriodBlty_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMestPeriodBltyADO> GetListCheck()
        {
            List<HisMestPeriodBltyADO> result = new List<HisMestPeriodBltyADO>();
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
                result = new List<HisMestPeriodBltyADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisMestPeriodBltyADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMestPeriodBltyADO)trvService.GetDataRecordByNode(node));
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
                    HisMestPeriodBltyADO HisMestPeriodBltyFocus = (HisMestPeriodBltyADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMestPeriodBltyFocus != null && HisMestPeriodBltyDoubleClick != null)
                    {
                        HisMestPeriodBltyDoubleClick(HisMestPeriodBltyFocus);
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
                    HisMestPeriodBltyADO HisMestPeriodBltyFocus = (HisMestPeriodBltyADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMestPeriodBltyFocus != null && this.HisMestPeriodBltyClick != null)
                    {
                        this.HisMestPeriodBltyClick(HisMestPeriodBltyFocus);
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
