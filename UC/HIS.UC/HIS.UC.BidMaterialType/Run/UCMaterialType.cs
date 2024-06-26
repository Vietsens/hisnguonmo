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
using HIS.UC.MaterialType.ADO;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.MaterialType.Run
{
    public partial class UCMaterialType : UserControl
    {
        #region Declare
        MaterialTypeInitADO MaterialTypeTreeADO;
        BindingList<MaterialTypeADO> records;
        List<MaterialTypeADO> MaterialTypeADOs = new List<MaterialTypeADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        MaterialType_NodeCellStyle MaterialTypeNodeCellStyle;
        MaterialType_CustomUnboundColumnData MaterialType_CustomUnboundColumnData;
        MaterialTypeHandler MaterialTypeClick;
        MaterialTypeHandler MaterialTypeDoubleClick;
        MaterialTypeHandler MaterialTypeRowEnter;
        MaterialType_GetStateImage MaterialType_GetStateImage;
        MaterialType_GetSelectImage MaterialType_GetSelectImage;
        MaterialTypeHandler MaterialType_StateImageClick;
        MaterialTypeHandler MaterialType_SelectImageClick;
        MaterialType_AfterCheck MaterialType_AfterCheck;
        MaterialType_BeforeCheck MaterialType_BeforeCheck;
        MaterialType_CheckAllNode MaterialType_CheckAllNode;
        MaterialType_CustomDrawNodeCell MaterialType_CustomDrawNodeCell;
        MaterialType_NewClick MaterialType_NewClick;
        MaterialType_RefeshData MaterialType_RefeshData;
        MaterialType_ExportExcel MaterialType_ExportExcel;
        MaterialType_Import MaterialType_Import;
        MaterialType_Save MaterialType_Save;
        bool IsShowCheckNode;
        bool isAutoWith;
        bool isShowImport;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MaterialTypeHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMaterialTypeExpend = true;
        #endregion

        #region Construct
        public UCMaterialType(MaterialTypeInitADO MaterialTypeADO)
        {
            InitializeComponent();
            try
            {
                this.MaterialTypeTreeADO = MaterialTypeADO;
                this.MaterialTypeNodeCellStyle = MaterialTypeADO.MaterialTypeNodeCellStyle;
                this.MaterialTypeClick = MaterialTypeADO.MaterialTypeClick;
                this.MaterialTypeDoubleClick = MaterialTypeADO.MaterialTypeDoubleClick;
                this.MaterialTypeRowEnter = MaterialTypeADO.MaterialTypeRowEnter;
                this.MaterialType_GetStateImage = MaterialTypeADO.MaterialType_GetStateImage;
                this.MaterialType_GetSelectImage = MaterialTypeADO.MaterialType_GetSelectImage;
                this.MaterialType_StateImageClick = MaterialTypeADO.MaterialType_StateImageClick;
                this.MaterialType_SelectImageClick = MaterialTypeADO.MaterialType_SelectImageClick;
                this.columnButtonEdits = MaterialTypeADO.ColumnButtonEdits;
                this.selectImageCollection = MaterialTypeADO.SelectImageCollection;
                this.stateImageCollection = MaterialTypeADO.StateImageCollection;
                this.updateSingleRow = MaterialTypeADO.UpdateSingleRow;
                this.MaterialType_CustomUnboundColumnData = MaterialTypeADO.MaterialType_CustomUnboundColumnData;
                this.menuItems = MaterialTypeADO.MenuItems;
                this.MaterialType_AfterCheck = MaterialTypeADO.MaterialType_AfterCheck;
                this.MaterialType_BeforeCheck = MaterialTypeADO.MaterialType_BeforeCheck;
                this.MaterialType_CheckAllNode = MaterialTypeADO.MaterialType_CheckAllNode;
                this.MaterialType_CustomDrawNodeCell = MaterialTypeADO.MaterialType_CustomDrawNodeCell;
                this.MaterialType_NewClick = MaterialTypeADO.MaterialType_NewClick;
                this.MaterialType_RefeshData = MaterialTypeADO.MaterialType_RefeshData;
                this.MaterialType_ExportExcel = MaterialTypeADO.MaterialType_ExportExcel;
                this.MaterialType_Import = MaterialTypeADO.MaterialType_Import;
                this.MaterialType_Save = MaterialTypeADO.MaterialType_Save;
                if (MaterialTypeADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = MaterialTypeADO.IsShowCheckNode.Value;
                }
                if (MaterialTypeADO.IsCreateParentNodeWithMaterialTypeExpend.HasValue)
                {
                    this.isCreateParentNodeWithMaterialTypeExpend = MaterialTypeTreeADO.IsCreateParentNodeWithMaterialTypeExpend.Value;
                }
                if (MaterialTypeADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWith = MaterialTypeADO.IsAutoWidth.Value;
                }
                if (MaterialTypeADO.IsShowImport.HasValue)
                {
                    isShowImport = MaterialTypeADO.IsShowImport.Value;
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
                if (MaterialTypeTreeADO != null)
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
                    trvService.MoveFirst();
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
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
                MaterialTypeADOs = new List<MaterialTypeADO>();
                if (MaterialTypeTreeADO.MaterialTypes != null)
                {
                    MaterialTypeADOs = (from r in MaterialTypeTreeADO.MaterialTypes select new MaterialTypeADO(r)).ToList();
                    MaterialTypeADOs = MaterialTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();
                }
                records = new BindingList<MaterialTypeADO>(MaterialTypeADOs);
                if (!String.IsNullOrEmpty(MaterialTypeTreeADO.KeyFieldName))
                    trvService.KeyFieldName = MaterialTypeTreeADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MaterialTypeTreeADO.ParentFieldName))
                    trvService.ParentFieldName = MaterialTypeTreeADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.MaterialType_CheckAllNode != null)
                    this.MaterialType_CheckAllNode(trvService.Nodes);
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
                this.trvService.OptionsView.AutoWidth = this.isAutoWith;
                if (MaterialTypeTreeADO.MaterialTypeColumns != null && MaterialTypeTreeADO.MaterialTypeColumns.Count > 0)
                {
                    foreach (var svtr in MaterialTypeTreeADO.MaterialTypeColumns)
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
                if (MaterialTypeTreeADO.ColumnButtonEdits != null && MaterialTypeTreeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MaterialTypeTreeADO.ColumnButtonEdits)
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
                if ((MaterialTypeTreeADO.IsShowImport.HasValue))
                {
                    lciImport.Visibility = (MaterialTypeTreeADO.IsShowImport.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciImport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (MaterialTypeTreeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MaterialTypeTreeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (MaterialTypeTreeADO.IsShowButtonAdd.HasValue)
                {
                    lciMedicineTypeAdd.Visibility = (MaterialTypeTreeADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (MaterialTypeTreeADO.IsShowExportExcel.HasValue)
                {
                    lciExportExcel.Visibility = (MaterialTypeTreeADO.IsShowExportExcel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MaterialTypeTreeADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MaterialTypeTreeADO.Keyword_NullValuePrompt;
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
                    var listdata = trvService.DataSource as BindingList<MaterialTypeADO>;
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

        private void SearchClick(string keyword)
        {
            try
            {
                BindingList<MaterialTypeADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MaterialTypeADO> rearchResult = new List<MaterialTypeADO>();

                    rearchResult = MaterialTypeADOs.Where(o =>
                                                    ((o.MATERIAL_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MATERIAL_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MANUFACTURER_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.PACKING_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.HEIN_SERVICE_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                    ).Distinct().ToList();

                    listResult = new BindingList<MaterialTypeADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<MaterialTypeADO>(MaterialTypeADOs);
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

        public void Excel()
        {
            try
            {
                btnExportExcel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<MaterialTypeADO> MaterialTypes)
        {
            try
            {
                txtKeyword.Text = "";
                this.MaterialTypeTreeADO.MaterialTypes = MaterialTypes;
                if (this.MaterialTypeTreeADO.MaterialTypes == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public object GetDataSource()
        {
            object result = null;
            try
            {
                return trvService.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (MaterialTypeNodeCellStyle != null)
                {
                    MaterialTypeNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is MaterialTypeADO)
                    {
                        if (MaterialTypeRowEnter != null)
                        {
                            MaterialTypeRowEnter((MaterialTypeADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is MaterialTypeADO)
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
                if (this.MaterialType_BeforeCheck != null)
                    this.MaterialType_BeforeCheck(node);
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
                if (this.MaterialType_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is MaterialTypeADO)
                {
                    MaterialType_AfterCheck(e.Node, (MaterialTypeADO)row);
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
                        MaterialTypeHandler clickhandler = btn.Tag as MaterialTypeHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is MaterialTypeADO)
                            {
                                clickhandler((MaterialTypeADO)data);
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
                if (data != null && data is MaterialTypeADO)
                {
                    if (this.MaterialType_GetSelectImage != null)
                        this.MaterialType_GetSelectImage((MaterialTypeADO)data, e);
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
                if (data != null && data is MaterialTypeADO)
                {
                    if (this.MaterialType_GetStateImage != null)
                        this.MaterialType_GetStateImage((MaterialTypeADO)data, e);
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
                if (data != null && data is MaterialTypeADO)
                {
                    if (this.MaterialType_SelectImageClick != null)
                        this.MaterialType_SelectImageClick((MaterialTypeADO)data);
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
                if (data != null && data is MaterialTypeADO)
                {
                    if (this.MaterialType_StateImageClick != null)
                        this.MaterialType_StateImageClick((MaterialTypeADO)data);
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
                    MaterialTypeADO currentRow = e.Row as MaterialTypeADO;
                    if (currentRow == null || this.MaterialType_CustomUnboundColumnData == null) return;
                    if (MaterialType_CustomUnboundColumnData != null)
                        this.MaterialType_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is MaterialTypeADO)
                {
                    var rowData = data as MaterialTypeADO;

                    //if (rowData.IsLeaf.HasValue && rowData.IsLeaf.Value)
                    //{
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

                    if (e.Column.FieldName == "IsCPNG")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit2;
                        if (rowData != null && rowData.IS_OUT_PARENT_FEE == 1)
                        {
                            rowData.IsCPNG = true;
                        }
                        else
                        {
                            rowData.IsCPNG = false;
                        }
                    }

                    if (e.Column.FieldName == "IsExprireDate")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit3;
                        if (rowData != null && rowData.IS_REQUIRE_HSD == 1)
                        {
                            rowData.IsExprireDate = true;
                        }
                        else
                        {
                            rowData.IsExprireDate = false;
                        }
                    }

                    if (e.Column.FieldName == "IsAutoExpend")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit4;
                        if (rowData != null && rowData.IS_AUTO_EXPEND == 1)
                        {
                            rowData.IsAutoExpend = true;
                        }
                        else
                        {
                            rowData.IsAutoExpend = false;
                        }
                    }
                    if (e.Column.FieldName == "IsAllowExportOdd")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit4;
                        if (rowData != null && rowData.IS_ALLOW_EXPORT_ODD == 1)
                        {
                            rowData.IsAllowExportOdd = true;
                        }
                        else
                        {
                            rowData.IsAllowExportOdd = false;
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
                if (data != null && data is MaterialTypeADO)
                {
                    var rowData = data as MaterialTypeADO;
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
                            if (data != null && data is MaterialTypeADO)
                            {
                                foreach (var menu in this.menuItems((MaterialTypeADO)data))
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
                if (data != null && data is MaterialTypeADO)
                {
                    var rowData = data as MaterialTypeADO;
                    if (rowData != null && this.MaterialType_CustomDrawNodeCell != null)
                    {
                        this.MaterialType_CustomDrawNodeCell(rowData, e);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MaterialTypeADO> GetListCheck()
        {
            List<MaterialTypeADO> result = new List<MaterialTypeADO>();
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
                result = new List<MaterialTypeADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MaterialTypeADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MaterialTypeADO)trvService.GetDataRecordByNode(node));
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
                    MaterialTypeADO materialTypeFocus = (MaterialTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (materialTypeFocus != null && MaterialTypeDoubleClick != null)
                    {
                        MaterialTypeDoubleClick(materialTypeFocus);
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
                    MaterialTypeADO materialTypeFocus = (MaterialTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (materialTypeFocus != null && MaterialTypeClick != null)
                    {
                        MaterialTypeClick(materialTypeFocus);
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
                if (this.MaterialType_NewClick != null && this.MaterialType_RefeshData != null)
                    this.MaterialType_NewClick(this.MaterialType_RefeshData);
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

        internal void EnableButtonSave(bool enable)
        {
            try
            {
                //btnSave.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        internal void ResetKeyWord()
        {
            try
            {
                txtKeyword.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string savePath = null;
        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                this.MaterialType_ExportExcel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                this.MaterialType_Import();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                this.MaterialType_Save();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        //internal enum ExportDataExcel
        //{
        //  EXPORT_DATA_EXCEL
        //}

        //void PrintProcess(ExportDataExcel printType)
        //{
        //  try
        //  {
        //    Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore
        //      (HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR,
        //      Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(),
        //      (HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath));

        //    switch (printType)
        //    {
        //      case ExportDataExcel.EXPORT_DATA_EXCEL:
        //        richEditorMain.RunPrintTemplate("Mps000201", DelegateRunPrinterTest);
        //        break;
        //      default:
        //        break;
        //    }
        //  }
        //  catch (Exception ex)
        //  {
        //    Inventec.Common.Logging.LogSystem.Error(ex);
        //  }
        //}

        //bool DelegateRunPrinterTest(string printTypeCode, string fileName)
        //{
        //  bool result = false;
        //  try
        //  {
        //    switch (printTypeCode)
        //    {
        //      case "Mps000201":
        //        LoadBieuMauPhieuYCInKetQuaXetNghiem(printTypeCode, fileName, ref result);
        //        break;
        //      default:
        //        break;
        //    }
        //  }
        //  catch (Exception ex)
        //  {
        //    Inventec.Common.Logging.LogSystem.Error(ex);
        //  }

        //  return result;
        //}

        //private void LoadBieuMauPhieuYCInKetQuaXetNghiem(string printTypeCode, string fileName, ref bool result)
        //{
        //  try
        //  {
        //    WaitingManager.Show();
        //    Inventec.Core.CommonParam param = new Inventec.Core.CommonParam();
        //    BindingList<MaterialTypeADO> list = new BindingList<ADO.MaterialTypeADO>();
        //    list = (BindingList<MaterialTypeADO>)trvService.DataSource;

        //    List<MaterialTypeADO> listDataSources = list.ToList();
        //    List<V_HIS_MATERIAL_TYPE> listMedicines = new List<V_HIS_MATERIAL_TYPE>();
        //    var listLeaf = listDataSources.Where(o => o.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_MATERIAL_TYPE.IS_LEAF__TRUE).ToList();
        //    AutoMapper.Mapper.CreateMap<MaterialTypeADO, V_HIS_MATERIAL_TYPE>();
        //    listMedicines = AutoMapper.Mapper.Map<List<V_HIS_MATERIAL_TYPE>>(listDataSources);
        //    //var listLeaf = MedicineTypeADO.MedicineTypes.Where(o => o.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE).ToList();
        //    if (listMedicines != null && listMedicines.Count > 0)
        //    {
        //      MPS.Processor.Mps000201.PDO.Mps000201PDO mps000014RDO = new MPS.Processor.Mps000201.PDO.Mps000201PDO(listMedicines);
        //      WaitingManager.Hide();
        //      MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000014RDO, MPS.ProcessorBase.PrintConfig.PreviewType.SaveFile, "", 1, savePath + ".xlsx");
        //      result = MPS.MpsPrinter.Run(PrintData);
        //      if (result)
        //        MessageManager.Show("Xuất exel thành công");
        //      else
        //        MessageManager.Show("Xuất exel thất bại");

        //    }
        //  }
        //  catch (Exception ex)
        //  {
        //    WaitingManager.Hide();
        //    Inventec.Common.Logging.LogSystem.Error(ex);
        //  }


    }
}
