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
using HIS.UC.MetyMatyTypeInStock.ADO;
using MOS.SDO;
using System.Configuration;

namespace HIS.UC.MetyMatyTypeInStock.Run
{
    public partial class UCMetyMatyTypeInStock : UserControl
    {
        #region Declare
        MetyMatyTypeInStockInitADO MetyMatyTypeInStockADO;
        BindingList<MetyMatyTypeInStockADO> records;
        List<MetyMatyTypeInStockADO> MetyMatyTypeInStockADOs = new List<MetyMatyTypeInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        MetyMatyTypeInStock_NodeCellStyle MetyMatyTypeInStockNodeCellStyle;
        MetyMatyTypeInStock_CustomUnboundColumnData MetyMatyTypeInStock_CustomUnboundColumnData;
        MetyMatyTypeInStockHandler MetyMatyTypeInStockClick;
        MetyMatyTypeInStockHandler MetyMatyTypeInStockDoubleClick;
        MetyMatyTypeInStockHandler MetyMatyTypeInStockRowEnter;
        MetyMatyTypeInStock_GetStateImage MetyMatyTypeInStock_GetStateImage;
        MetyMatyTypeInStock_GetSelectImage MetyMatyTypeInStock_GetSelectImage;
        MetyMatyTypeInStockHandler MetyMatyTypeInStock_StateImageClick;
        MetyMatyTypeInStockHandler MetyMatyTypeInStock_SelectImageClick;
        MetyMatyTypeInStock_AfterCheck MetyMatyTypeInStock_AfterCheck;
        MetyMatyTypeInStock_BeforeCheck MetyMatyTypeInStock_BeforeCheck;
        MetyMatyTypeInStock_CheckAllNode MetyMatyTypeInStock_CheckAllNode;
        MetyMatyTypeInStock_CustomDrawNodeCell MetyMatyTypeInStock_CustomDrawNodeCell;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MetyMatyTypeInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMetyMatyTypeInStockExpend = true;
        bool isAutoSaveLayoutToRegistry = false;

        const string SOFTWARE_FOLDER = "SOFTWARE";
        const string COMPANY_FOLDER = "INVENTEC";
        static string APP_FOLDER = (ConfigurationManager.AppSettings["Inventec.Desktop.ApplicationCode"] ?? "HIS").ToString();
        const string LAYOUT_KEY = "UCMetyMatyTypeInStock.Layout";
        string regKey = String.Format("{0}\\{1}\\{2}\\{3}", SOFTWARE_FOLDER, COMPANY_FOLDER, APP_FOLDER, LAYOUT_KEY);
        bool isLayoutLoaded;
        #endregion

        #region Construct
        public UCMetyMatyTypeInStock(MetyMatyTypeInStockInitADO MetyMatyTypeInStockADO)
        {
            InitializeComponent();
            try
            {
                this.MetyMatyTypeInStockADO = MetyMatyTypeInStockADO;
                this.MetyMatyTypeInStockNodeCellStyle = MetyMatyTypeInStockADO.MetyMatyTypeInStockNodeCellStyle;
                this.MetyMatyTypeInStockClick = MetyMatyTypeInStockADO.MetyMatyTypeInStockClick;
                this.MetyMatyTypeInStockDoubleClick = MetyMatyTypeInStockADO.MetyMatyTypeInStockDoubleClick;
                this.MetyMatyTypeInStockRowEnter = MetyMatyTypeInStockADO.MetyMatyTypeInStockRowEnter;
                this.MetyMatyTypeInStock_GetStateImage = MetyMatyTypeInStockADO.MetyMatyTypeInStock_GetStateImage;
                this.MetyMatyTypeInStock_GetSelectImage = MetyMatyTypeInStockADO.MetyMatyTypeInStock_GetSelectImage;
                this.MetyMatyTypeInStock_StateImageClick = MetyMatyTypeInStockADO.MetyMatyTypeInStock_StateImageClick;
                this.MetyMatyTypeInStock_SelectImageClick = MetyMatyTypeInStockADO.MetyMatyTypeInStock_SelectImageClick;
                this.columnButtonEdits = MetyMatyTypeInStockADO.ColumnButtonEdits;
                this.selectImageCollection = MetyMatyTypeInStockADO.SelectImageCollection;
                this.stateImageCollection = MetyMatyTypeInStockADO.StateImageCollection;
                this.updateSingleRow = MetyMatyTypeInStockADO.UpdateSingleRow;
                this.MetyMatyTypeInStock_CustomUnboundColumnData = MetyMatyTypeInStockADO.MetyMatyTypeInStock_CustomUnboundColumnData;
                this.menuItems = MetyMatyTypeInStockADO.MenuItems;
                this.MetyMatyTypeInStock_AfterCheck = MetyMatyTypeInStockADO.MetyMatyTypeInStock_AfterCheck;
                this.MetyMatyTypeInStock_BeforeCheck = MetyMatyTypeInStockADO.MetyMatyTypeInStock_BeforeCheck;
                this.MetyMatyTypeInStock_CheckAllNode = MetyMatyTypeInStockADO.MetyMatyTypeInStock_CheckAllNode;
                this.MetyMatyTypeInStock_CustomDrawNodeCell = MetyMatyTypeInStockADO.MetyMatyTypeInStock_CustomDrawNodeCell;
                if (MetyMatyTypeInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = MetyMatyTypeInStockADO.IsShowCheckNode.Value;
                }
                if (MetyMatyTypeInStockADO.IsCreateParentNodeWithMetyMatyTypeInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithMetyMatyTypeInStockExpend = MetyMatyTypeInStockADO.IsCreateParentNodeWithMetyMatyTypeInStockExpend.Value;
                }
                if (MetyMatyTypeInStockADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = MetyMatyTypeInStockADO.IsAutoWidth.Value;
                }
                if (MetyMatyTypeInStockADO.IsAutoSaveLayoutToRegistry.HasValue)
                {
                    this.isAutoSaveLayoutToRegistry = MetyMatyTypeInStockADO.IsAutoSaveLayoutToRegistry.Value;
                    if (!String.IsNullOrEmpty(MetyMatyTypeInStockADO.KeySaveLayoutToRegistry))
                    {
                        regKey = MetyMatyTypeInStockADO.KeySaveLayoutToRegistry;
                    }
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
                if (MetyMatyTypeInStockADO != null)
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

                    if (MetyMatyTypeInStockADO.IsEnableAppearanceFocusedCell.HasValue)
                        trvService.OptionsSelection.EnableAppearanceFocusedCell = MetyMatyTypeInStockADO.IsEnableAppearanceFocusedCell.Value;

                    if (MetyMatyTypeInStockADO.IsEnableAppearanceFocusedRow.HasValue)
                        trvService.OptionsSelection.EnableAppearanceFocusedRow = MetyMatyTypeInStockADO.IsEnableAppearanceFocusedRow.Value;

                    RestoreLayoutFromRegistry();
                    isLayoutLoaded = true;
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
                MetyMatyTypeInStockADOs = new List<MetyMatyTypeInStockADO>();
                if (MetyMatyTypeInStockADO.MetyMatyTypeInStocks != null)
                {
                    MetyMatyTypeInStockADOs = (from r in MetyMatyTypeInStockADO.MetyMatyTypeInStocks select new MetyMatyTypeInStockADO(r)).ToList();

                    MetyMatyTypeInStockADOs = MetyMatyTypeInStockADOs.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
                records = new BindingList<MetyMatyTypeInStockADO>(MetyMatyTypeInStockADOs);
                if (!String.IsNullOrEmpty(MetyMatyTypeInStockADO.KeyFieldName))
                    trvService.KeyFieldName = MetyMatyTypeInStockADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MetyMatyTypeInStockADO.ParentFieldName))
                    trvService.ParentFieldName = MetyMatyTypeInStockADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.MetyMatyTypeInStock_CheckAllNode != null)
                    this.MetyMatyTypeInStock_CheckAllNode(trvService.Nodes);
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
                if (MetyMatyTypeInStockADO.MetyMatyTypeInStockColumns != null && MetyMatyTypeInStockADO.MetyMatyTypeInStockColumns.Count > 0)
                {
                    foreach (var svtr in MetyMatyTypeInStockADO.MetyMatyTypeInStockColumns)
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

                if (MetyMatyTypeInStockADO.ColumnButtonEdits != null && MetyMatyTypeInStockADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MetyMatyTypeInStockADO.ColumnButtonEdits)
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
                if (MetyMatyTypeInStockADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MetyMatyTypeInStockADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (MetyMatyTypeInStockADO.IsShowButtonAdd.HasValue)
                {
                    lciMetyMatyTypeInStockAdd.Visibility = (MetyMatyTypeInStockADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }

                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MetyMatyTypeInStockADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MetyMatyTypeInStockADO.Keyword_NullValuePrompt;
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
                    var listData = trvService.DataSource as BindingList<MetyMatyTypeInStockADO>;
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
                BindingList<MetyMatyTypeInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    keyword = Inventec.Common.String.Convert.UnSignVNese(keyword.Trim().ToLower());
                    List<MetyMatyTypeInStockADO> rearchResult = new List<MetyMatyTypeInStockADO>();

                    rearchResult = MetyMatyTypeInStockADOs.Where(o =>
                                                    (Inventec.Common.String.Convert.UnSignVNese((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower()).Contains(keyword)
                                                    || ((o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower()).Contains(keyword)
                                                    || Inventec.Common.String.Convert.UnSignVNese((o.ACTIVE_INGR_BHYT_NAME ?? "").ToString().ToLower()).Contains(keyword)
                                                    || ((o.MANUFACTURER_NAME ?? "").ToString().ToLower()).Contains(keyword)
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<MetyMatyTypeInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<MetyMatyTypeInStockADO>(MetyMatyTypeInStockADOs);
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

        public void SearchByKeyword(string keyword)
        {
            try
            {
                txtKeyword.Text = keyword;
                SearchClick(txtKeyword.Text.Trim());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<D_HIS_MEDI_STOCK_1> MetyMatyTypeInStocks)
        {
            try
            {
                txtKeyword.Text = "";
                this.MetyMatyTypeInStockADO.MetyMatyTypeInStocks = MetyMatyTypeInStocks;
                if (this.MetyMatyTypeInStockADO.MetyMatyTypeInStocks == null)
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
                if (MetyMatyTypeInStockNodeCellStyle != null)
                {
                    MetyMatyTypeInStockNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is MetyMatyTypeInStockADO)
                    {
                        if (MetyMatyTypeInStockRowEnter != null)
                        {
                            MetyMatyTypeInStockRowEnter((MetyMatyTypeInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is MetyMatyTypeInStockADO)
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
                if (this.MetyMatyTypeInStock_BeforeCheck != null)
                    this.MetyMatyTypeInStock_BeforeCheck(node);
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
                if (this.MetyMatyTypeInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is MetyMatyTypeInStockADO)
                {
                    MetyMatyTypeInStock_AfterCheck(e.Node, (MetyMatyTypeInStockADO)row);
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
                        MetyMatyTypeInStockHandler clickhandler = btn.Tag as MetyMatyTypeInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is MetyMatyTypeInStockADO)
                            {
                                clickhandler((MetyMatyTypeInStockADO)data);
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    if (this.MetyMatyTypeInStock_GetSelectImage != null)
                        this.MetyMatyTypeInStock_GetSelectImage((MetyMatyTypeInStockADO)data, e);
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    if (this.MetyMatyTypeInStock_GetStateImage != null)
                        this.MetyMatyTypeInStock_GetStateImage((MetyMatyTypeInStockADO)data, e);
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    if (this.MetyMatyTypeInStock_SelectImageClick != null)
                        this.MetyMatyTypeInStock_SelectImageClick((MetyMatyTypeInStockADO)data);
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    if (this.MetyMatyTypeInStock_StateImageClick != null)
                        this.MetyMatyTypeInStock_StateImageClick((MetyMatyTypeInStockADO)data);
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
                    MetyMatyTypeInStockADO currentRow = e.Row as MetyMatyTypeInStockADO;
                    if (MetyMatyTypeInStock_CustomUnboundColumnData == null || currentRow == null) return;
                    this.MetyMatyTypeInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    var rowData = data as MetyMatyTypeInStockADO;

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

                        //if (rowData != null && rowData.IsLeaf == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE)
                        //{
                        //    rowData.bIsLeaf = true;
                        //}
                        //else
                        //{
                        //    rowData.bIsLeaf = false;
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    var rowData = data as MetyMatyTypeInStockADO;
                    //if (rowData.bIsLeaf != null && rowData.bIsLeaf.Value)
                    //    rowData.IsLeaf = IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE;
                    //else
                    //    rowData.IsLeaf = null;
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
                            if (data != null && data is MetyMatyTypeInStockADO)
                            {
                                foreach (var menu in this.menuItems((MetyMatyTypeInStockADO)data))
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
                if (data != null && data is MetyMatyTypeInStockADO)
                {
                    var rowData = data as MetyMatyTypeInStockADO;
                    if (rowData != null && this.MetyMatyTypeInStock_CustomDrawNodeCell != null)
                    {
                        this.MetyMatyTypeInStock_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<MetyMatyTypeInStockADO> GetListCheck()
        {
            List<MetyMatyTypeInStockADO> result = new List<MetyMatyTypeInStockADO>();
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
                result = new List<MetyMatyTypeInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<MetyMatyTypeInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MetyMatyTypeInStockADO)trvService.GetDataRecordByNode(node));
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
                    MetyMatyTypeInStockADO MetyMatyTypeInStockFocus = (MetyMatyTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MetyMatyTypeInStockFocus != null && MetyMatyTypeInStockDoubleClick != null)
                    {
                        MetyMatyTypeInStockDoubleClick(MetyMatyTypeInStockFocus);
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
                    MetyMatyTypeInStockADO MetyMatyTypeInStockFocus = (MetyMatyTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (MetyMatyTypeInStockFocus != null && this.MetyMatyTypeInStockClick != null)
                    {
                        this.MetyMatyTypeInStockClick(MetyMatyTypeInStockFocus);
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

        internal void SaveLayoutToRegistry()
        {
            try
            {
                trvService.SaveLayoutToRegistry(regKey);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void RestoreLayoutFromRegistry()
        {
            try
            {
                if (this.isAutoSaveLayoutToRegistry)
                {
                    trvService.RestoreLayoutFromRegistry(regKey);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_ColumnWidthChanged(object sender, ColumnChangedEventArgs e)
        {
            try
            {
                if (this.isAutoSaveLayoutToRegistry && isLayoutLoaded)
                {
                    SaveLayoutToRegistry();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
