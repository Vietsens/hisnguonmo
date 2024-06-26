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
using HIS.UC.HisBloodTypeInStock.ADO;
using MOS.SDO;

namespace HIS.UC.HisBloodTypeInStock.Run
{
    public partial class UCHisBloodTypeInStock : UserControl
    {
        #region Declare
        HisBloodTypeInStockInitADO HisBloodTypeInStockADO;
        BindingList<HisBloodTypeInStockADO> records;
        List<HisBloodTypeInStockADO> HisBloodTypeInStockADOs = new List<HisBloodTypeInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisBloodTypeInStock_NodeCellStyle HisBloodTypeInStockNodeCellStyle;
        HisBloodTypeInStock_CustomUnboundColumnData HisBloodTypeInStock_CustomUnboundColumnData;
        HisBloodTypeInStockHandler HisBloodTypeInStockClick;
        HisBloodTypeInStockHandler HisBloodTypeInStockDoubleClick;
        HisBloodTypeInStockHandler HisBloodTypeInStockRowEnter;
        HisBloodTypeInStock_GetStateImage HisBloodTypeInStock_GetStateImage;
        HisBloodTypeInStock_GetSelectImage HisBloodTypeInStock_GetSelectImage;
        HisBloodTypeInStockHandler HisBloodTypeInStock_StateImageClick;
        HisBloodTypeInStockHandler HisBloodTypeInStock_SelectImageClick;
        HisBloodTypeInStock_AfterCheck HisBloodTypeInStock_AfterCheck;
        HisBloodTypeInStock_BeforeCheck HisBloodTypeInStock_BeforeCheck;
        HisBloodTypeInStock_CheckAllNode HisBloodTypeInStock_CheckAllNode;
        HisBloodTypeInStock_CustomDrawNodeCell HisBloodTypeInStock_CustomDrawNodeCell;
        HisBloodTypeInStock_NewClick HisBloodTypeInStock_NewClick;
        HisBloodTypeInStock_RefeshData HisBloodTypeInStock_RefeshData;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisBloodTypeInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisBloodTypeInStockExpend = true;
        #endregion

        #region Construct
        public UCHisBloodTypeInStock(HisBloodTypeInStockInitADO HisBloodTypeInStockADO)
        {
            InitializeComponent();
            try
            {
                this.HisBloodTypeInStockADO = HisBloodTypeInStockADO;
                this.HisBloodTypeInStockNodeCellStyle = HisBloodTypeInStockADO.HisBloodTypeInStockNodeCellStyle;
                this.HisBloodTypeInStockClick = HisBloodTypeInStockADO.HisBloodTypeInStockClick;
                this.HisBloodTypeInStockDoubleClick = HisBloodTypeInStockADO.HisBloodTypeInStockDoubleClick;
                this.HisBloodTypeInStockRowEnter = HisBloodTypeInStockADO.HisBloodTypeInStockRowEnter;
                this.HisBloodTypeInStock_GetStateImage = HisBloodTypeInStockADO.HisBloodTypeInStock_GetStateImage;
                this.HisBloodTypeInStock_GetSelectImage = HisBloodTypeInStockADO.HisBloodTypeInStock_GetSelectImage;
                this.HisBloodTypeInStock_StateImageClick = HisBloodTypeInStockADO.HisBloodTypeInStock_StateImageClick;
                this.HisBloodTypeInStock_SelectImageClick = HisBloodTypeInStockADO.HisBloodTypeInStock_SelectImageClick;
                this.columnButtonEdits = HisBloodTypeInStockADO.ColumnButtonEdits;
                this.selectImageCollection = HisBloodTypeInStockADO.SelectImageCollection;
                this.stateImageCollection = HisBloodTypeInStockADO.StateImageCollection;
                this.updateSingleRow = HisBloodTypeInStockADO.UpdateSingleRow;
                this.HisBloodTypeInStock_CustomUnboundColumnData = HisBloodTypeInStockADO.HisBloodTypeInStock_CustomUnboundColumnData;
                this.menuItems = HisBloodTypeInStockADO.MenuItems;
                this.HisBloodTypeInStock_AfterCheck = HisBloodTypeInStockADO.HisBloodTypeInStock_AfterCheck;
                this.HisBloodTypeInStock_BeforeCheck = HisBloodTypeInStockADO.HisBloodTypeInStock_BeforeCheck;
                this.HisBloodTypeInStock_CheckAllNode = HisBloodTypeInStockADO.HisBloodTypeInStock_CheckAllNode;
                this.HisBloodTypeInStock_CustomDrawNodeCell = HisBloodTypeInStockADO.HisBloodTypeInStock_CustomDrawNodeCell;
                if (HisBloodTypeInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisBloodTypeInStockADO.IsShowCheckNode.Value;
                }
                if (HisBloodTypeInStockADO.IsCreateParentNodeWithHisBloodTypeInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisBloodTypeInStockExpend = HisBloodTypeInStockADO.IsCreateParentNodeWithHisBloodTypeInStockExpend.Value;
                }
                if (HisBloodTypeInStockADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = HisBloodTypeInStockADO.IsAutoWidth.Value;
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
                if (HisBloodTypeInStockADO != null)
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
                HisBloodTypeInStockADOs = new List<HisBloodTypeInStockADO>();
                if (HisBloodTypeInStockADO.HisBloodTypeInStocks != null)
                {
                    HisBloodTypeInStockADOs = (from r in HisBloodTypeInStockADO.HisBloodTypeInStocks select new HisBloodTypeInStockADO(r)).ToList();
                }
                else if (HisBloodTypeInStockADO.HisBloodInStocks != null)
                {
                    HisBloodTypeInStockADOs = (from r in HisBloodTypeInStockADO.HisBloodInStocks select new HisBloodTypeInStockADO(r)).ToList();
                }
                records = new BindingList<HisBloodTypeInStockADO>(HisBloodTypeInStockADOs);
                if (!String.IsNullOrEmpty(HisBloodTypeInStockADO.KeyFieldName))
                    trvService.KeyFieldName = HisBloodTypeInStockADO.KeyFieldName;
                if (!String.IsNullOrEmpty(HisBloodTypeInStockADO.ParentFieldName))
                    trvService.ParentFieldName = HisBloodTypeInStockADO.ParentFieldName;
                trvService.DataSource = records;
                //trvService.ExpandToLevel(0);
                trvService.ExpandAll();
                if (this.HisBloodTypeInStock_CheckAllNode != null)
                    this.HisBloodTypeInStock_CheckAllNode(trvService.Nodes);
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
                if (HisBloodTypeInStockADO.HisBloodTypeInStockColumns != null && HisBloodTypeInStockADO.HisBloodTypeInStockColumns.Count > 0)
                {
                    foreach (var svtr in HisBloodTypeInStockADO.HisBloodTypeInStockColumns)
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
                if (HisBloodTypeInStockADO.ColumnButtonEdits != null && HisBloodTypeInStockADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisBloodTypeInStockADO.ColumnButtonEdits)
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
                btnThuGon.Text = HisBloodTypeInStockADO.NameButtonClose;
                btnMoRong.Text = HisBloodTypeInStockADO.NameButtonOpen;
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
                if (HisBloodTypeInStockADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisBloodTypeInStockADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisBloodTypeInStockADO.IsShowButtonAdd.HasValue)
                {
                    lciHisBloodTypeInStockAdd.Visibility = (HisBloodTypeInStockADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisBloodTypeInStockADO.IsShowButtonExpand.HasValue)
                {
                    layoutControlItem2.Visibility = (HisBloodTypeInStockADO.IsShowButtonExpand.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    emptySpaceItem1.Visibility = (HisBloodTypeInStockADO.IsShowButtonExpand.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);

                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(HisBloodTypeInStockADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = HisBloodTypeInStockADO.Keyword_NullValuePrompt;
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
                BindingList<HisBloodTypeInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisBloodTypeInStockADO> rearchResult = new List<HisBloodTypeInStockADO>();

                    rearchResult = HisBloodTypeInStockADOs.Where(o =>
                                                    ((o.BloodTypeName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.BloodTypeCode ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    //|| (o.BloodTypeHeinName ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    //|| (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<HisBloodTypeInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisBloodTypeInStockADO>(HisBloodTypeInStockADOs);
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

        public void Reload(List<HisBloodTypeInStockSDO> HisBloodTypeInStocks)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisBloodTypeInStockADO.HisBloodTypeInStocks = HisBloodTypeInStocks;
                if (this.HisBloodTypeInStockADO.HisBloodTypeInStocks == null)
                    records = null;
                BindTreePlus();
                btnThuGon.Visible = true;
                btnMoRong.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<HisBloodInStockSDO> HisBloodInStocks)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisBloodTypeInStockADO.HisBloodInStocks = HisBloodInStocks;
                if (this.HisBloodTypeInStockADO.HisBloodInStocks == null)
                    records = null;
                BindTreePlus();
                btnThuGon.Visible = true;
                btnMoRong.Visible = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Export(string path)
        {
            try
            {
                this.trvService.ExportToXlsx(path);
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
        public void DisposeControl()
        {
            try
            {
                isCreateParentNodeWithHisBloodTypeInStockExpend = false;
                menuItems = null;
                updateSingleRow = null;
                stateImageCollection = null;
                selectImageCollection = null;
                isAutoWidth = false;
                IsShowCheckNode = false;
                HisBloodTypeInStock_RefeshData = null;
                HisBloodTypeInStock_NewClick = null;
                HisBloodTypeInStock_CustomDrawNodeCell = null;
                HisBloodTypeInStock_CheckAllNode = null;
                HisBloodTypeInStock_BeforeCheck = null;
                HisBloodTypeInStock_AfterCheck = null;
                HisBloodTypeInStock_SelectImageClick = null;
                HisBloodTypeInStock_StateImageClick = null;
                HisBloodTypeInStock_GetSelectImage = null;
                HisBloodTypeInStock_GetStateImage = null;
                HisBloodTypeInStockRowEnter = null;
                HisBloodTypeInStockDoubleClick = null;
                HisBloodTypeInStockClick = null;
                HisBloodTypeInStock_CustomUnboundColumnData = null;
                HisBloodTypeInStockNodeCellStyle = null;
                columnButtonEdits = null;
                HisBloodTypeInStockADOs = null;
                records = null;
                HisBloodTypeInStockADO = null;
                this.trvService.GetStateImage -= new DevExpress.XtraTreeList.GetStateImageEventHandler(this.trvService_GetStateImage);
                this.trvService.GetSelectImage -= new DevExpress.XtraTreeList.GetSelectImageEventHandler(this.trvService_GetSelectImage);
                this.trvService.StateImageClick -= new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_StateImageClick);
                this.trvService.SelectImageClick -= new DevExpress.XtraTreeList.NodeClickEventHandler(this.trvService_SelectImageClick);
                this.trvService.CustomNodeCellEdit -= new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(this.trvService_CustomNodeCellEdit);
                this.trvService.NodeCellStyle -= new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.trvService_NodeCellStyle);
                this.trvService.CustomUnboundColumnData -= new DevExpress.XtraTreeList.CustomColumnDataEventHandler(this.trvService_CustomUnboundColumnData);
                this.trvService.BeforeCheckNode -= new DevExpress.XtraTreeList.CheckNodeEventHandler(this.trvService_BeforeCheckNode);
                this.trvService.AfterCheckNode -= new DevExpress.XtraTreeList.NodeEventHandler(this.trvService_AfterCheckNode);
                this.trvService.CustomDrawNodeCell -= new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.trvService_CustomDrawNodeCell);
                this.trvService.PopupMenuShowing -= new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(this.trvService_PopupMenuShowing);
                this.trvService.Click -= new System.EventHandler(this.trvService_Click);
                this.trvService.DoubleClick -= new System.EventHandler(this.trvService_DoubleClick);
                this.trvService.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.trvService_KeyDown);
                this.repositoryItemchkIsExpend__Enable.CheckedChanged -= new System.EventHandler(this.repositoryItemchkIsExpend__Enable_CheckedChanged);
                this.btnThuGon.Click -= new System.EventHandler(this.btnThuGon_Click);
                this.btnMoRong.Click -= new System.EventHandler(this.btnMoRong_Click);
                this.btnNew.Click -= new System.EventHandler(this.btnNew_Click);
                this.txtKeyword.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
                this.txtKeyword.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyword_PreviewKeyDown);
                this.Load -= new System.EventHandler(this.UCServiceTree_Load);
                btnThuGon = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                btnMoRong = null;
                panelControl1 = null;
                treeListColumn11 = null;
                treeListColumn10 = null;
                treeListColumn8 = null;
                lciHisBloodTypeInStockAdd = null;
                btnNew = null;
                repositoryItemCheckEdit4 = null;
                treeListColumn7 = null;
                treeListColumn6 = null;
                treeListColumn4 = null;
                repositoryItemCheckEdit3 = null;
                repositoryItemCheckEdit2 = null;
                repositoryItemCheckEdit1 = null;
                treeListColumnPrice = null;
                repositoryItemchkIsExpend__Disable = null;
                repositoryItemchkIsExpend__Enable = null;
                treeColumnDVKTC = null;
                treeColumnDiscount = null;
                treeColumnVATPercent = null;
                treeColumnManufactoryName = null;
                treeColumnConcentra = null;
                treeColumnTotalPrice = null;
                treeColumnServiceUnitName = null;
                treeColumnHeinServiceBHYTName = null;
                treeColumnIsExpend = null;
                layoutControlItem4 = null;
                lciKeyword = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                txtKeyword = null;
                treeColumnTransactionCode = null;
                treeColumnHisBloodTypeInStockName = null;
                treeColumnHisBloodTypeInStockCode = null;
                trvService = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (HisBloodTypeInStockNodeCellStyle != null)
                {
                    HisBloodTypeInStockNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is HisBloodTypeInStockADO)
                    {
                        if (HisBloodTypeInStockRowEnter != null)
                        {
                            HisBloodTypeInStockRowEnter((HisBloodTypeInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisBloodTypeInStockADO)
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
                if (this.HisBloodTypeInStock_BeforeCheck != null)
                    this.HisBloodTypeInStock_BeforeCheck(node);
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
                if (this.HisBloodTypeInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisBloodTypeInStockADO)
                {
                    HisBloodTypeInStock_AfterCheck(e.Node, (HisBloodTypeInStockADO)row);
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
                        HisBloodTypeInStockHandler clickhandler = btn.Tag as HisBloodTypeInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisBloodTypeInStockADO)
                            {
                                clickhandler((HisBloodTypeInStockADO)data);
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    if (this.HisBloodTypeInStock_GetSelectImage != null)
                        this.HisBloodTypeInStock_GetSelectImage((HisBloodTypeInStockADO)data, e);
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    if (this.HisBloodTypeInStock_GetStateImage != null)
                        this.HisBloodTypeInStock_GetStateImage((HisBloodTypeInStockADO)data, e);
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    if (this.HisBloodTypeInStock_SelectImageClick != null)
                        this.HisBloodTypeInStock_SelectImageClick((HisBloodTypeInStockADO)data);
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    if (this.HisBloodTypeInStock_StateImageClick != null)
                        this.HisBloodTypeInStock_StateImageClick((HisBloodTypeInStockADO)data);
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
                    HisBloodTypeInStockADO currentRow = e.Row as HisBloodTypeInStockADO;
                    if (currentRow == null || this.HisBloodTypeInStock_CustomUnboundColumnData == null) return;
                    this.HisBloodTypeInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    var rowData = data as HisBloodTypeInStockADO;

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

                        //if (rowData != null && rowData.IsLeaf == 1)
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    var rowData = data as HisBloodTypeInStockADO;
                    //if (rowData.bIsLeaf != null && rowData.bIsLeaf.Value)
                    //    rowData.IsLeaf = 1;
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
                            if (data != null && data is HisBloodTypeInStockADO)
                            {
                                foreach (var menu in this.menuItems((HisBloodTypeInStockADO)data))
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
                if (data != null && data is HisBloodTypeInStockADO)
                {
                    var rowData = data as HisBloodTypeInStockADO;
                    if (rowData != null && this.HisBloodTypeInStock_CustomDrawNodeCell != null)
                    {
                        this.HisBloodTypeInStock_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisBloodTypeInStockADO> GetListCheck()
        {
            List<HisBloodTypeInStockADO> result = new List<HisBloodTypeInStockADO>();
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
                result = new List<HisBloodTypeInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisBloodTypeInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisBloodTypeInStockADO)trvService.GetDataRecordByNode(node));
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
                    HisBloodTypeInStockADO HisBloodTypeInStockFocus = (HisBloodTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisBloodTypeInStockFocus != null && HisBloodTypeInStockDoubleClick != null)
                    {
                        HisBloodTypeInStockDoubleClick(HisBloodTypeInStockFocus);
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
                    HisBloodTypeInStockADO HisBloodTypeInStockFocus = (HisBloodTypeInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisBloodTypeInStockFocus != null && this.HisBloodTypeInStockClick != null)
                    {
                        this.HisBloodTypeInStockClick(HisBloodTypeInStockFocus);
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
                if (this.HisBloodTypeInStock_NewClick != null && this.HisBloodTypeInStock_RefeshData != null)
                    this.HisBloodTypeInStock_NewClick(this.HisBloodTypeInStock_RefeshData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnThuGon_Click(object sender, EventArgs e)
        {
            try
            {
                btnThuGon.Visible = false;
                btnMoRong.Visible = true;
                Expand(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnMoRong_Click(object sender, EventArgs e)
        {
            try
            {
                btnThuGon.Visible = true;
                btnMoRong.Visible = false;
                Expand(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Expand(bool isExpand)
        {
            try
            {
                if (isExpand)
                {
                    trvService.ExpandAll();
                }
                else
                {
                    trvService.BeginUpdate();
                    trvService.CollapseAll();
                    //trvService.ExpandToLevel(0);
                    trvService.EndUpdate();
                }
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
                    var listData = trvService.DataSource as BindingList<HisBloodTypeInStockADO>;
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
    }
}
