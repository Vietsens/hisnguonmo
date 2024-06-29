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
using HIS.UC.MaterialTypeInStock.ADO;
using MOS.SDO;

namespace HIS.UC.MaterialTypeInStock.Run
{
    public partial class UCMaterialTypeInStock : UserControl
    {
        #region Declare
        MaterialTypeInStockInitADO MaterialTypeInStockTreeADO;
        BindingList<MaterialTypeInStockADO> records;
        List<MaterialTypeInStockADO> MaterialTypeInStockADOs = new List<MaterialTypeInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        MaterialTypeInStock_NodeCellStyle MaterialTypeInStockNodeCellStyle;
        MaterialTypeInStock_CustomUnboundColumnData MaterialTypeInStock_CustomUnboundColumnData;
        MaterialTypeInStockHandler MaterialTypeInStockClick;
        MaterialTypeInStockHandler MaterialTypeInStockDoubleClick;
        MaterialTypeInStockHandler MaterialTypeInStockRowEnter;
        MaterialTypeInStock_GetStateImage MaterialTypeInStock_GetStateImage;
        MaterialTypeInStock_GetSelectImage MaterialTypeInStock_GetSelectImage;
        MaterialTypeInStockHandler MaterialTypeInStock_StateImageClick;
        MaterialTypeInStockHandler MaterialTypeInStock_SelectImageClick;
        MaterialTypeInStock_AfterCheck MaterialTypeInStock_AfterCheck;
        MaterialTypeInStock_BeforeCheck MaterialTypeInStock_BeforeCheck;
        MaterialTypeInStock_CheckAllNode MaterialTypeInStock_CheckAllNode;
        MaterialTypeInStock_CustomDrawNodeCell MaterialTypeInStock_CustomDrawNodeCell;
        MaterialTypeInStock_NewClick MaterialTypeInStock_NewClick;
        MaterialTypeInStock_RefeshData MaterialTypeInStock_RefeshData;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MaterialTypeInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMaterialTypeInStockExpend = true;
        #endregion

        #region Construct
        public UCMaterialTypeInStock(MaterialTypeInStockInitADO MaterialTypeInStockADO)
        {
            InitializeComponent();
            try
            {
                this.MaterialTypeInStockTreeADO = MaterialTypeInStockADO;
                this.MaterialTypeInStockNodeCellStyle = MaterialTypeInStockADO.MaterialTypeInStockNodeCellStyle;
                this.MaterialTypeInStockClick = MaterialTypeInStockADO.MaterialTypeInStockClick;
                this.MaterialTypeInStockDoubleClick = MaterialTypeInStockADO.MaterialTypeInStockDoubleClick;
                this.MaterialTypeInStockRowEnter = MaterialTypeInStockADO.MaterialTypeInStockRowEnter;
                this.MaterialTypeInStock_GetStateImage = MaterialTypeInStockADO.MaterialTypeInStock_GetStateImage;
                this.MaterialTypeInStock_GetSelectImage = MaterialTypeInStockADO.MaterialTypeInStock_GetSelectImage;
                this.MaterialTypeInStock_StateImageClick = MaterialTypeInStockADO.MaterialTypeInStock_StateImageClick;
                this.MaterialTypeInStock_SelectImageClick = MaterialTypeInStockADO.MaterialTypeInStock_SelectImageClick;
                this.columnButtonEdits = MaterialTypeInStockADO.ColumnButtonEdits;
                this.selectImageCollection = MaterialTypeInStockADO.SelectImageCollection;
                this.stateImageCollection = MaterialTypeInStockADO.StateImageCollection;
                this.updateSingleRow = MaterialTypeInStockADO.UpdateSingleRow;
                this.MaterialTypeInStock_CustomUnboundColumnData = MaterialTypeInStockADO.MaterialTypeInStock_CustomUnboundColumnData;
                this.menuItems = MaterialTypeInStockADO.MenuItems;
                this.MaterialTypeInStock_AfterCheck = MaterialTypeInStockADO.MaterialTypeInStock_AfterCheck;
                this.MaterialTypeInStock_BeforeCheck = MaterialTypeInStockADO.MaterialTypeInStock_BeforeCheck;
                this.MaterialTypeInStock_CheckAllNode = MaterialTypeInStockADO.MaterialTypeInStock_CheckAllNode;
                this.MaterialTypeInStock_CustomDrawNodeCell = MaterialTypeInStockADO.MaterialTypeInStock_CustomDrawNodeCell;
                this.MaterialTypeInStock_NewClick = MaterialTypeInStockADO.MaterialTypeInStock_NewClick;
                this.MaterialTypeInStock_RefeshData = MaterialTypeInStockADO.MaterialTypeInStock_RefeshData;
                if (MaterialTypeInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = MaterialTypeInStockADO.IsShowCheckNode.Value;
                }
                if (MaterialTypeInStockADO.IsCreateParentNodeWithMaterialTypeInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithMaterialTypeInStockExpend = MaterialTypeInStockTreeADO.IsCreateParentNodeWithMaterialTypeInStockExpend.Value;
                }
                if (MaterialTypeInStockADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = MaterialTypeInStockADO.IsAutoWidth.Value;
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
                if (MaterialTypeInStockTreeADO != null)
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
                MaterialTypeInStockADOs = new List<MaterialTypeInStockADO>();
                if (MaterialTypeInStockTreeADO.MaterialTypeInStocks != null)
                {
                    MaterialTypeInStockADOs = (from r in MaterialTypeInStockTreeADO.MaterialTypeInStocks select new MaterialTypeInStockADO(r)).ToList();
                    MaterialTypeInStockADOs = MaterialTypeInStockADOs.OrderBy(o => o.NumOrder).ThenBy(o => o.MaterialTypeName).ToList();
                }
                records = new BindingList<MaterialTypeInStockADO>(MaterialTypeInStockADOs);
                if (!String.IsNullOrEmpty(MaterialTypeInStockTreeADO.KeyFieldName))
                    trvService.KeyFieldName = MaterialTypeInStockTreeADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MaterialTypeInStockTreeADO.ParentFieldName))
                    trvService.ParentFieldName = MaterialTypeInStockTreeADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.MaterialTypeInStock_CheckAllNode != null)
                    this.MaterialTypeInStock_CheckAllNode(trvService.Nodes);
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
                if (MaterialTypeInStockTreeADO.MaterialTypeInStockColumns != null && MaterialTypeInStockTreeADO.MaterialTypeInStockColumns.Count > 0)
                {
                    foreach (var svtr in MaterialTypeInStockTreeADO.MaterialTypeInStockColumns)
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
                if (MaterialTypeInStockTreeADO.ColumnButtonEdits != null && MaterialTypeInStockTreeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MaterialTypeInStockTreeADO.ColumnButtonEdits)
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
                if (MaterialTypeInStockTreeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MaterialTypeInStockTreeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (MaterialTypeInStockTreeADO.IsShowButtonAdd.HasValue)
                {
                    lciMedicineTypeAdd.Visibility = (MaterialTypeInStockTreeADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }

                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MaterialTypeInStockTreeADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MaterialTypeInStockTreeADO.Keyword_NullValuePrompt;
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

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var listData = trvService.DataSource as BindingList<MaterialTypeInStockADO>;
                    if (listData != null && listData.Count > 0)
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

        private void SearchClick(string keyword)
        {
            try
            {
                BindingList<MaterialTypeInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MaterialTypeInStockADO> rearchResult = new List<MaterialTypeInStockADO>();

                    rearchResult = MaterialTypeInStockADOs.Where(o =>
                                                    ((o.MaterialTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MaterialTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ManufacturerName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ServiceUnitName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MaterialTypeHeinName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                    ).Distinct().ToList();

                    listResult = new BindingList<MaterialTypeInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<MaterialTypeInStockADO>(MaterialTypeInStockADOs);
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
        public void New()
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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

        public void Reload(List<HisMaterialTypeInStockSDO> MaterialTypeInStocks)
        {
            try
            {
                txtKeyword.Text = "";
                this.MaterialTypeInStockTreeADO.MaterialTypeInStocks = MaterialTypeInStocks;
                if (this.MaterialTypeInStockTreeADO.MaterialTypeInStocks == null)
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
                if (MaterialTypeInStockNodeCellStyle != null)
                {
                    MaterialTypeInStockNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is MaterialTypeInStockADO)
                    {
                        if (MaterialTypeInStockRowEnter != null)
                        {
                            MaterialTypeInStockRowEnter((MaterialTypeInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is MaterialTypeInStockADO)
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
                if (this.MaterialTypeInStock_BeforeCheck != null)
                    this.MaterialTypeInStock_BeforeCheck(node);
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
                if (this.MaterialTypeInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is MaterialTypeInStockADO)
                {
                    MaterialTypeInStock_AfterCheck(e.Node, (MaterialTypeInStockADO)row);
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
                        MaterialTypeInStockHandler clickhandler = btn.Tag as MaterialTypeInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is MaterialTypeInStockADO)
                            {
                                clickhandler((MaterialTypeInStockADO)data);
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    if (this.MaterialTypeInStock_GetSelectImage != null)
                        this.MaterialTypeInStock_GetSelectImage((MaterialTypeInStockADO)data, e);
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    if (this.MaterialTypeInStock_GetStateImage != null)
                        this.MaterialTypeInStock_GetStateImage((MaterialTypeInStockADO)data, e);
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    if (this.MaterialTypeInStock_SelectImageClick != null)
                        this.MaterialTypeInStock_SelectImageClick((MaterialTypeInStockADO)data);
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    if (this.MaterialTypeInStock_StateImageClick != null)
                        this.MaterialTypeInStock_StateImageClick((MaterialTypeInStockADO)data);
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
                    MaterialTypeInStockADO currentRow = e.Row as MaterialTypeInStockADO;
                    if (currentRow == null) return;
                    this.MaterialTypeInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    var rowData = data as MaterialTypeInStockADO;

                    //if (rowData.IsLeaf.HasValue && rowData.IsLeaf.Value)
                    //{
                    if (e.Column.FieldName == "bIsLeaf")
                    {
                        if (this.updateSingleRow != null)
                        {
                            e.RepositoryItem = repositoryItemchkIsExpend__Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemchkIsExpend__Disable;
                        }

                        if (rowData != null && rowData.IsLeaf == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_LEAF__TRUE)
                        {
                            rowData.bIsLeaf = true;
                        }
                        else
                        {
                            rowData.bIsLeaf = false;
                        }
                    }
                    //}
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    var rowData = data as MaterialTypeInStockADO;
                    if (rowData.bIsLeaf != null && rowData.bIsLeaf.Value)
                        rowData.IsLeaf = IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_LEAF__TRUE;
                    else
                        rowData.IsLeaf = null;
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
                            if (data != null && data is MaterialTypeInStockADO)
                            {
                                foreach (var menu in this.menuItems((MaterialTypeInStockADO)data))
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
                if (data != null && data is MaterialTypeInStockADO)
                {
                    var rowData = data as MaterialTypeInStockADO;
                    if (rowData != null && this.MaterialTypeInStock_CustomDrawNodeCell != null)
                    {
                        this.MaterialTypeInStock_CustomDrawNodeCell(rowData, e);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MaterialTypeInStockADO> GetListCheck()
        {
            List<MaterialTypeInStockADO> result = new List<MaterialTypeInStockADO>();
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
                result = new List<MaterialTypeInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MaterialTypeInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MaterialTypeInStockADO)trvService.GetDataRecordByNode(node));
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
                    MaterialTypeInStockADO MaterialTypeInStockFocus = (MaterialTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MaterialTypeInStockFocus != null && MaterialTypeInStockDoubleClick != null)
                    {
                        MaterialTypeInStockDoubleClick(MaterialTypeInStockFocus);
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
                    MaterialTypeInStockADO MaterialTypeInStockFocus = (MaterialTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MaterialTypeInStockFocus != null && MaterialTypeInStockClick != null)
                    {
                        MaterialTypeInStockClick(MaterialTypeInStockFocus);
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
            try
            {
                if (this.MaterialTypeInStock_NewClick != null && this.MaterialTypeInStock_RefeshData != null)
                    this.MaterialTypeInStock_NewClick(this.MaterialTypeInStock_RefeshData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

    }
}
