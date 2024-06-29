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
using HIS.UC.HisMestPeriodMety.ADO;

namespace HIS.UC.HisMestPeriodMety.Run
{
    public partial class UCHisMestPeriodMety : UserControl
    {
        #region Declare
        HisMestPeriodMetyInitADO HisMestPeriodMetyADO;
        BindingList<HisMestPeriodMetyADO> records;
        List<HisMestPeriodMetyADO> HisMestPeriodMetyADOs = new List<HisMestPeriodMetyADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMestPeriodMety_NodeCellStyle HisMestPeriodMetyNodeCellStyle;
        HisMestPeriodMety_CustomUnboundColumnData HisMestPeriodMety_CustomUnboundColumnData;
        HisMestPeriodMetyHandler HisMestPeriodMetyClick;
        HisMestPeriodMetyHandler HisMestPeriodMetyDoubleClick;
        HisMestPeriodMetyHandler HisMestPeriodMetyRowEnter;
        HisMestPeriodMety_GetStateImage HisMestPeriodMety_GetStateImage;
        HisMestPeriodMety_GetSelectImage HisMestPeriodMety_GetSelectImage;
        HisMestPeriodMetyHandler HisMestPeriodMety_StateImageClick;
        HisMestPeriodMetyHandler HisMestPeriodMety_SelectImageClick;
        HisMestPeriodMety_AfterCheck HisMestPeriodMety_AfterCheck;
        HisMestPeriodMety_BeforeCheck HisMestPeriodMety_BeforeCheck;
        HisMestPeriodMety_CheckAllNode HisMestPeriodMety_CheckAllNode;
        HisMestPeriodMety_CustomDrawNodeCell HisMestPeriodMety_CustomDrawNodeCell;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMestPeriodMetyHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMestPeriodMetyExpend = true;
        #endregion

        #region Construct
        public UCHisMestPeriodMety(HisMestPeriodMetyInitADO HisMestPeriodMetyADO)
        {
            InitializeComponent();
            try
            {
                this.HisMestPeriodMetyADO = HisMestPeriodMetyADO;
                this.HisMestPeriodMetyNodeCellStyle = HisMestPeriodMetyADO.HisMestPeriodMetyNodeCellStyle;
                this.HisMestPeriodMetyClick = HisMestPeriodMetyADO.HisMestPeriodMetyClick;
                this.HisMestPeriodMetyDoubleClick = HisMestPeriodMetyADO.HisMestPeriodMetyDoubleClick;
                this.HisMestPeriodMetyRowEnter = HisMestPeriodMetyADO.HisMestPeriodMetyRowEnter;
                this.HisMestPeriodMety_GetStateImage = HisMestPeriodMetyADO.HisMestPeriodMety_GetStateImage;
                this.HisMestPeriodMety_GetSelectImage = HisMestPeriodMetyADO.HisMestPeriodMety_GetSelectImage;
                this.HisMestPeriodMety_StateImageClick = HisMestPeriodMetyADO.HisMestPeriodMety_StateImageClick;
                this.HisMestPeriodMety_SelectImageClick = HisMestPeriodMetyADO.HisMestPeriodMety_SelectImageClick;
                this.columnButtonEdits = HisMestPeriodMetyADO.ColumnButtonEdits;
                this.selectImageCollection = HisMestPeriodMetyADO.SelectImageCollection;
                this.stateImageCollection = HisMestPeriodMetyADO.StateImageCollection;
                this.updateSingleRow = HisMestPeriodMetyADO.UpdateSingleRow;
                this.HisMestPeriodMety_CustomUnboundColumnData = HisMestPeriodMetyADO.HisMestPeriodMety_CustomUnboundColumnData;
                this.menuItems = HisMestPeriodMetyADO.MenuItems;
                this.HisMestPeriodMety_AfterCheck = HisMestPeriodMetyADO.HisMestPeriodMety_AfterCheck;
                this.HisMestPeriodMety_BeforeCheck = HisMestPeriodMetyADO.HisMestPeriodMety_BeforeCheck;
                this.HisMestPeriodMety_CheckAllNode = HisMestPeriodMetyADO.HisMestPeriodMety_CheckAllNode;
                this.HisMestPeriodMety_CustomDrawNodeCell = HisMestPeriodMetyADO.HisMestPeriodMety_CustomDrawNodeCell;
                if (HisMestPeriodMetyADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMestPeriodMetyADO.IsShowCheckNode.Value;
                }
                if (HisMestPeriodMetyADO.IsCreateParentNodeWithHisMestPeriodMetyExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMestPeriodMetyExpend = HisMestPeriodMetyADO.IsCreateParentNodeWithHisMestPeriodMetyExpend.Value;
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
                if (HisMestPeriodMetyADO != null)
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
                HisMestPeriodMetyADOs = new List<HisMestPeriodMetyADO>();
                if (HisMestPeriodMetyADO.HisMestPeriodMetys != null)
                {
                    HisMestPeriodMetyADOs = (from r in HisMestPeriodMetyADO.HisMestPeriodMetys select new HisMestPeriodMetyADO(r)).ToList();
                    // HisMestPeriodMetyADOs = HisMestPeriodMetyADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.BLOOD_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMestPeriodMetyADO>(HisMestPeriodMetyADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.HisMestPeriodMety_CheckAllNode != null)
                    this.HisMestPeriodMety_CheckAllNode(trvService.Nodes);
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
                if (HisMestPeriodMetyADO.HisMestPeriodMetyColumns != null && HisMestPeriodMetyADO.HisMestPeriodMetyColumns.Count > 0)
                {
                    foreach (var svtr in HisMestPeriodMetyADO.HisMestPeriodMetyColumns)
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
                if (HisMestPeriodMetyADO.ColumnButtonEdits != null && HisMestPeriodMetyADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMestPeriodMetyADO.ColumnButtonEdits)
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
                if (HisMestPeriodMetyADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMestPeriodMetyADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMestPeriodMetyADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMestPeriodMetyAdd.Visibility = (HisMestPeriodMetyADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                BindingList<HisMestPeriodMetyADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMestPeriodMetyADO> rearchResult = new List<HisMestPeriodMetyADO>();

                    rearchResult = HisMestPeriodMetyADOs.Where(o =>
                                                    ((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.ACTIVE_INGR_BHYT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<HisMestPeriodMetyADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMestPeriodMetyADO>(HisMestPeriodMetyADOs);
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_MEST_PERIOD_METY> HisMestPeriodMetys)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisMestPeriodMetyADO.HisMestPeriodMetys = HisMestPeriodMetys;
                if (this.HisMestPeriodMetyADO.HisMestPeriodMetys == null)
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
                var data = (HisMestPeriodMetyADO)trvService.GetDataRecordByNode(e.Node);
                if (HisMestPeriodMetyNodeCellStyle != null && data != null)
                {
                    HisMestPeriodMetyNodeCellStyle(data, e);
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
                    if (node != null && data != null && data is HisMestPeriodMetyADO)
                    {
                        if (HisMestPeriodMetyRowEnter != null)
                        {
                            HisMestPeriodMetyRowEnter((HisMestPeriodMetyADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMestPeriodMetyADO)
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
                if (this.HisMestPeriodMety_BeforeCheck != null)
                    this.HisMestPeriodMety_BeforeCheck(node);
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
                if (this.HisMestPeriodMety_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMestPeriodMetyADO)
                {
                    HisMestPeriodMety_AfterCheck(e.Node, (HisMestPeriodMetyADO)row);
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
                        HisMestPeriodMetyHandler clickhandler = btn.Tag as HisMestPeriodMetyHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMestPeriodMetyADO)
                            {
                                clickhandler((HisMestPeriodMetyADO)data);
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    if (this.HisMestPeriodMety_GetSelectImage != null)
                        this.HisMestPeriodMety_GetSelectImage((HisMestPeriodMetyADO)data, e);
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    if (this.HisMestPeriodMety_GetStateImage != null)
                        this.HisMestPeriodMety_GetStateImage((HisMestPeriodMetyADO)data, e);
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    if (this.HisMestPeriodMety_SelectImageClick != null)
                        this.HisMestPeriodMety_SelectImageClick((HisMestPeriodMetyADO)data);
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    if (this.HisMestPeriodMety_StateImageClick != null)
                        this.HisMestPeriodMety_StateImageClick((HisMestPeriodMetyADO)data);
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
                    HisMestPeriodMetyADO currentRow = e.Row as HisMestPeriodMetyADO;
                    if (currentRow == null || this.HisMestPeriodMety_CustomUnboundColumnData == null) return;
                    this.HisMestPeriodMety_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    var rowData = data as HisMestPeriodMetyADO;

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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    var rowData = data as HisMestPeriodMetyADO;
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
                            if (data != null && data is HisMestPeriodMetyADO)
                            {
                                foreach (var menu in this.menuItems((HisMestPeriodMetyADO)data))
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
                if (data != null && data is HisMestPeriodMetyADO)
                {
                    var rowData = data as HisMestPeriodMetyADO;
                    if (rowData != null && this.HisMestPeriodMety_CustomDrawNodeCell != null)
                    {
                        this.HisMestPeriodMety_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMestPeriodMetyADO> GetListCheck()
        {
            List<HisMestPeriodMetyADO> result = new List<HisMestPeriodMetyADO>();
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
                result = new List<HisMestPeriodMetyADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisMestPeriodMetyADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMestPeriodMetyADO)trvService.GetDataRecordByNode(node));
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
                    HisMestPeriodMetyADO HisMestPeriodMetyFocus = (HisMestPeriodMetyADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMestPeriodMetyFocus != null && HisMestPeriodMetyDoubleClick != null)
                    {
                        HisMestPeriodMetyDoubleClick(HisMestPeriodMetyFocus);
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
                    HisMestPeriodMetyADO HisMestPeriodMetyFocus = (HisMestPeriodMetyADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMestPeriodMetyFocus != null && this.HisMestPeriodMetyClick != null)
                    {
                        this.HisMestPeriodMetyClick(HisMestPeriodMetyFocus);
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
