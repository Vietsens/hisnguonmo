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
using HIS.UC.MedicineTypeInStock.ADO;
using MOS.SDO;

namespace HIS.UC.MedicineTypeInStock.Run
{
    public partial class UCMedicineTypeInStock : UserControl
    {
        #region Declare
        MedicineTypeInStockInitADO MedicineTypeInStockADO;
        BindingList<MedicineTypeInStockADO> records;
        List<MedicineTypeInStockADO> MedicineTypeInStockADOs = new List<MedicineTypeInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        MedicineTypeInStock_NodeCellStyle MedicineTypeInStockNodeCellStyle;
        MedicineTypeInStock_CustomUnboundColumnData MedicineTypeInStock_CustomUnboundColumnData;
        MedicineTypeInStockHandler MedicineTypeInStockClick;
        MedicineTypeInStockHandler MedicineTypeInStockDoubleClick;
        MedicineTypeInStockHandler MedicineTypeInStockRowEnter;
        MedicineTypeInStock_GetStateImage MedicineTypeInStock_GetStateImage;
        MedicineTypeInStock_GetSelectImage MedicineTypeInStock_GetSelectImage;
        MedicineTypeInStockHandler MedicineTypeInStock_StateImageClick;
        MedicineTypeInStockHandler MedicineTypeInStock_SelectImageClick;
        MedicineTypeInStock_AfterCheck MedicineTypeInStock_AfterCheck;
        MedicineTypeInStock_BeforeCheck MedicineTypeInStock_BeforeCheck;
        MedicineTypeInStock_CheckAllNode MedicineTypeInStock_CheckAllNode;
        MedicineTypeInStock_CustomDrawNodeCell MedicineTypeInStock_CustomDrawNodeCell;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MedicineTypeInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMedicineTypeInStockExpend = true;
        #endregion

        #region Construct
        public UCMedicineTypeInStock(MedicineTypeInStockInitADO MedicineTypeInStockADO)
        {
            InitializeComponent();
            try
            {
                this.MedicineTypeInStockADO = MedicineTypeInStockADO;
                this.MedicineTypeInStockNodeCellStyle = MedicineTypeInStockADO.MedicineTypeInStockNodeCellStyle;
                this.MedicineTypeInStockClick = MedicineTypeInStockADO.MedicineTypeInStockClick;
                this.MedicineTypeInStockDoubleClick = MedicineTypeInStockADO.MedicineTypeInStockDoubleClick;
                this.MedicineTypeInStockRowEnter = MedicineTypeInStockADO.MedicineTypeInStockRowEnter;
                this.MedicineTypeInStock_GetStateImage = MedicineTypeInStockADO.MedicineTypeInStock_GetStateImage;
                this.MedicineTypeInStock_GetSelectImage = MedicineTypeInStockADO.MedicineTypeInStock_GetSelectImage;
                this.MedicineTypeInStock_StateImageClick = MedicineTypeInStockADO.MedicineTypeInStock_StateImageClick;
                this.MedicineTypeInStock_SelectImageClick = MedicineTypeInStockADO.MedicineTypeInStock_SelectImageClick;
                this.columnButtonEdits = MedicineTypeInStockADO.ColumnButtonEdits;
                this.selectImageCollection = MedicineTypeInStockADO.SelectImageCollection;
                this.stateImageCollection = MedicineTypeInStockADO.StateImageCollection;
                this.updateSingleRow = MedicineTypeInStockADO.UpdateSingleRow;
                this.MedicineTypeInStock_CustomUnboundColumnData = MedicineTypeInStockADO.MedicineTypeInStock_CustomUnboundColumnData;
                this.menuItems = MedicineTypeInStockADO.MenuItems;
                this.MedicineTypeInStock_AfterCheck = MedicineTypeInStockADO.MedicineTypeInStock_AfterCheck;
                this.MedicineTypeInStock_BeforeCheck = MedicineTypeInStockADO.MedicineTypeInStock_BeforeCheck;
                this.MedicineTypeInStock_CheckAllNode = MedicineTypeInStockADO.MedicineTypeInStock_CheckAllNode;
                this.MedicineTypeInStock_CustomDrawNodeCell = MedicineTypeInStockADO.MedicineTypeInStock_CustomDrawNodeCell;
                if (MedicineTypeInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = MedicineTypeInStockADO.IsShowCheckNode.Value;
                }
                if (MedicineTypeInStockADO.IsCreateParentNodeWithMedicineTypeInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithMedicineTypeInStockExpend = MedicineTypeInStockADO.IsCreateParentNodeWithMedicineTypeInStockExpend.Value;
                }
                if (MedicineTypeInStockADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = MedicineTypeInStockADO.IsAutoWidth.Value;
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
                if (MedicineTypeInStockADO != null)
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
                MedicineTypeInStockADOs = new List<MedicineTypeInStockADO>();
                if (MedicineTypeInStockADO.MedicineTypeInStocks != null)
                {
                    MedicineTypeInStockADOs = (from r in MedicineTypeInStockADO.MedicineTypeInStocks select new MedicineTypeInStockADO(r)).ToList();

                    MedicineTypeInStockADOs = MedicineTypeInStockADOs.OrderBy(o => o.NumOrder).ThenBy(o => o.MedicineTypeName).ToList();
                }
                records = new BindingList<MedicineTypeInStockADO>(MedicineTypeInStockADOs);
                if (!String.IsNullOrEmpty(MedicineTypeInStockADO.KeyFieldName))
                    trvService.KeyFieldName = MedicineTypeInStockADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MedicineTypeInStockADO.ParentFieldName))
                    trvService.ParentFieldName = MedicineTypeInStockADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.MedicineTypeInStock_CheckAllNode != null)
                    this.MedicineTypeInStock_CheckAllNode(trvService.Nodes);
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
                if (MedicineTypeInStockADO.MedicineTypeInStockColumns != null && MedicineTypeInStockADO.MedicineTypeInStockColumns.Count > 0)
                {
                    foreach (var svtr in MedicineTypeInStockADO.MedicineTypeInStockColumns)
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

                if (MedicineTypeInStockADO.ColumnButtonEdits != null && MedicineTypeInStockADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MedicineTypeInStockADO.ColumnButtonEdits)
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
                if (MedicineTypeInStockADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MedicineTypeInStockADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (MedicineTypeInStockADO.IsShowButtonAdd.HasValue)
                {
                    lciMedicineTypeInStockAdd.Visibility = (MedicineTypeInStockADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }

                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MedicineTypeInStockADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MedicineTypeInStockADO.Keyword_NullValuePrompt;
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
                    var listData = trvService.DataSource as BindingList<MedicineTypeInStockADO>;
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
                BindingList<MedicineTypeInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MedicineTypeInStockADO> rearchResult = new List<MedicineTypeInStockADO>();

                    rearchResult = MedicineTypeInStockADOs.Where(o =>
                                                    ((o.MedicineTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MedicineTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ActiveIngrBhytCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ActiveIngrBhytName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.Concentra ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ManufacturerCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ManufacturerName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MediStockCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MediStockName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.NationalCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.NationalName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.RegisterNumber ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ServiceUnitCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ServiceUnitName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ServiceUnitSymbol ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<MedicineTypeInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<MedicineTypeInStockADO>(MedicineTypeInStockADOs);
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

        public void Reload(List<HisMedicineTypeInStockSDO> MedicineTypeInStocks)
        {
            try
            {
                txtKeyword.Text = "";
                this.MedicineTypeInStockADO.MedicineTypeInStocks = MedicineTypeInStocks;
                if (this.MedicineTypeInStockADO.MedicineTypeInStocks == null)
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
                if (MedicineTypeInStockNodeCellStyle != null)
                {
                    MedicineTypeInStockNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is MedicineTypeInStockADO)
                    {
                        if (MedicineTypeInStockRowEnter != null)
                        {
                            MedicineTypeInStockRowEnter((MedicineTypeInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is MedicineTypeInStockADO)
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
                if (this.MedicineTypeInStock_BeforeCheck != null)
                    this.MedicineTypeInStock_BeforeCheck(node);
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
                if (this.MedicineTypeInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is MedicineTypeInStockADO)
                {
                    MedicineTypeInStock_AfterCheck(e.Node, (MedicineTypeInStockADO)row);
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
                        MedicineTypeInStockHandler clickhandler = btn.Tag as MedicineTypeInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is MedicineTypeInStockADO)
                            {
                                clickhandler((MedicineTypeInStockADO)data);
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    if (this.MedicineTypeInStock_GetSelectImage != null)
                        this.MedicineTypeInStock_GetSelectImage((MedicineTypeInStockADO)data, e);
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    if (this.MedicineTypeInStock_GetStateImage != null)
                        this.MedicineTypeInStock_GetStateImage((MedicineTypeInStockADO)data, e);
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    if (this.MedicineTypeInStock_SelectImageClick != null)
                        this.MedicineTypeInStock_SelectImageClick((MedicineTypeInStockADO)data);
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    if (this.MedicineTypeInStock_StateImageClick != null)
                        this.MedicineTypeInStock_StateImageClick((MedicineTypeInStockADO)data);
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
                    MedicineTypeInStockADO currentRow = e.Row as MedicineTypeInStockADO;
                    if (MedicineTypeInStock_CustomUnboundColumnData == null || currentRow == null) return;
                    this.MedicineTypeInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    var rowData = data as MedicineTypeInStockADO;

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

                        if (rowData != null && rowData.IsLeaf == 1)
                        {
                            rowData.bIsLeaf = true;
                        }
                        else
                        {
                            rowData.bIsLeaf = false;
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    var rowData = data as MedicineTypeInStockADO;
                    if (rowData.bIsLeaf != null && rowData.bIsLeaf.Value)
                        rowData.IsLeaf = 1;
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
                            if (data != null && data is MedicineTypeInStockADO)
                            {
                                foreach (var menu in this.menuItems((MedicineTypeInStockADO)data))
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
                if (data != null && data is MedicineTypeInStockADO)
                {
                    var rowData = data as MedicineTypeInStockADO;
                    if (rowData != null && this.MedicineTypeInStock_CustomDrawNodeCell != null)
                    {
                        this.MedicineTypeInStock_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MedicineTypeInStockADO> GetListCheck()
        {
            List<MedicineTypeInStockADO> result = new List<MedicineTypeInStockADO>();
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
                result = new List<MedicineTypeInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MedicineTypeInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MedicineTypeInStockADO)trvService.GetDataRecordByNode(node));
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
                    MedicineTypeInStockADO MedicineTypeInStockFocus = (MedicineTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MedicineTypeInStockFocus != null && MedicineTypeInStockDoubleClick != null)
                    {
                        MedicineTypeInStockDoubleClick(MedicineTypeInStockFocus);
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
                    MedicineTypeInStockADO MedicineTypeInStockFocus = (MedicineTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MedicineTypeInStockFocus != null && this.MedicineTypeInStockClick != null)
                    {
                        this.MedicineTypeInStockClick(MedicineTypeInStockFocus);
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
    }
}
