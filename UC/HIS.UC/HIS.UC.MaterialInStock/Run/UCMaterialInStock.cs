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
using HIS.UC.HisMaterialInStock.ADO;
using MOS.SDO;
using HIS.UC.MaterialInStock.ADO;

namespace HIS.UC.HisMaterialInStock.Run
{
    public partial class UCHisMaterialInStock : UserControl
    {
        #region Declare
        HisMaterialInStockInitADO HisMaterialInStockADO;
        BindingList<HisMaterialInStockADO> records;
        List<HisMaterialInStockADO> HisMaterialInStockADOs = new List<HisMaterialInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMaterialInStock_NodeCellStyle HisMaterialInStockNodeCellStyle;
        HisMaterialInStock_CustomUnboundColumnData HisMaterialInStock_CustomUnboundColumnData;
        HisMaterialInStockHandler HisMaterialInStockClick;
        HisMaterialInStockHandler HisMaterialInStockDoubleClick;
        HisMaterialInStockHandler HisMaterialInStockRowEnter;
        HisMaterialInStock_GetStateImage HisMaterialInStock_GetStateImage;
        HisMaterialInStock_GetSelectImage HisMaterialInStock_GetSelectImage;
        HisMaterialInStockHandler HisMaterialInStock_StateImageClick;
        HisMaterialInStockHandler HisMaterialInStock_SelectImageClick;
        HisMaterialInStock_AfterCheck HisMaterialInStock_AfterCheck;
        HisMaterialInStock_BeforeCheck HisMaterialInStock_BeforeCheck;
        HisMaterialInStock_CheckAllNode HisMaterialInStock_CheckAllNode;
        HisMaterialInStock_CustomDrawNodeCell HisMaterialInStock_CustomDrawNodeCell;
        btnLock_buttonClick btnLock_buttonClick;
        btnUnLock_buttonClick btnUnLock_buttonClick;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMaterialInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMaterialInStockExpend = true;
        List<long> MediStockIds;
        int rowFocus;
        List<KeyTreeADO> lstKey = new List<KeyTreeADO>();
        #endregion

        #region Construct
        public UCHisMaterialInStock(HisMaterialInStockInitADO HisMaterialInStockADO)
        {
            InitializeComponent();
            try
            {
                this.HisMaterialInStockADO = HisMaterialInStockADO;
                this.HisMaterialInStockNodeCellStyle = HisMaterialInStockADO.HisMaterialInStockNodeCellStyle;
                this.HisMaterialInStockClick = HisMaterialInStockADO.HisMaterialInStockClick;
                this.HisMaterialInStockDoubleClick = HisMaterialInStockADO.HisMaterialInStockDoubleClick;
                this.HisMaterialInStockRowEnter = HisMaterialInStockADO.HisMaterialInStockRowEnter;
                this.HisMaterialInStock_GetStateImage = HisMaterialInStockADO.HisMaterialInStock_GetStateImage;
                this.HisMaterialInStock_GetSelectImage = HisMaterialInStockADO.HisMaterialInStock_GetSelectImage;
                this.HisMaterialInStock_StateImageClick = HisMaterialInStockADO.HisMaterialInStock_StateImageClick;
                this.HisMaterialInStock_SelectImageClick = HisMaterialInStockADO.HisMaterialInStock_SelectImageClick;
                this.columnButtonEdits = HisMaterialInStockADO.ColumnButtonEdits;
                this.selectImageCollection = HisMaterialInStockADO.SelectImageCollection;
                this.stateImageCollection = HisMaterialInStockADO.StateImageCollection;
                this.updateSingleRow = HisMaterialInStockADO.UpdateSingleRow;
                this.HisMaterialInStock_CustomUnboundColumnData = HisMaterialInStockADO.HisMaterialInStock_CustomUnboundColumnData;
                this.menuItems = HisMaterialInStockADO.MenuItems;
                this.HisMaterialInStock_AfterCheck = HisMaterialInStockADO.HisMaterialInStock_AfterCheck;
                this.HisMaterialInStock_BeforeCheck = HisMaterialInStockADO.HisMaterialInStock_BeforeCheck;
                this.HisMaterialInStock_CheckAllNode = HisMaterialInStockADO.HisMaterialInStock_CheckAllNode;
                this.btnLock_buttonClick = HisMaterialInStockADO.btnLock_buttonClick;
                this.btnUnLock_buttonClick = HisMaterialInStockADO.btnUnLock_buttonClick;
                this.HisMaterialInStock_CustomDrawNodeCell = HisMaterialInStockADO.HisMaterialInStock_CustomDrawNodeCell;
                if (HisMaterialInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMaterialInStockADO.IsShowCheckNode.Value;
                }
                if (HisMaterialInStockADO.IsCreateParentNodeWithHisMaterialInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMaterialInStockExpend = HisMaterialInStockADO.IsCreateParentNodeWithHisMaterialInStockExpend.Value;
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
                if (HisMaterialInStockADO != null)
                {
                    InitializeTree();
                    BindTreePlus();
                    chkDetails.Checked = true;
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
                HisMaterialInStockADOs = new List<HisMaterialInStockADO>();
                if (HisMaterialInStockADO.HisMaterialInStocks != null)
                {
                    foreach (var r in HisMaterialInStockADO.HisMaterialInStocks)
                    {
                        HisMaterialInStockADO item = new HisMaterialInStockADO(r);
                        var child = HisMaterialInStockADO.HisMaterialInStocks.Where(o => o.ParentNodeId == r.NodeId).ToList();
                        if (child != null && child.Count > 0)
                        {
                            item.MANUFACTURER_NAME_PARENT = String.Join(";", child.Select(o => o.MANUFACTURER_NAME));
                            item.SERIAL_NUMBER_PARENT = String.Join(";", child.Select(o => o.SERIAL_NUMBER));
                        }
                        HisMaterialInStockADOs.Add(item);
                    }
                    HisMaterialInStockADOs = HisMaterialInStockADOs.OrderBy(o => o.MATERIAL_TYPE_NAME).ToList();//.ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMaterialInStockADO>(HisMaterialInStockADOs);
                if (!String.IsNullOrEmpty(HisMaterialInStockADO.KeyFieldName))
                    trvService.KeyFieldName = HisMaterialInStockADO.KeyFieldName;
                if (!String.IsNullOrEmpty(HisMaterialInStockADO.ParentFieldName))
                    trvService.ParentFieldName = HisMaterialInStockADO.ParentFieldName;
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.HisMaterialInStock_CheckAllNode != null)
                    this.HisMaterialInStock_CheckAllNode(trvService.Nodes);
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
                if (HisMaterialInStockADO.HisMaterialInStockColumns != null && HisMaterialInStockADO.HisMaterialInStockColumns.Count > 0)
                {
                    foreach (var svtr in HisMaterialInStockADO.HisMaterialInStockColumns)
                    {
                        TreeListColumn col = this.trvService.Columns.AddField(svtr.FieldName);
                        col.Visible = svtr.Visible;
                        col.VisibleIndex = svtr.VisibleIndex;
                        col.Width = svtr.ColumnWidth;
                        col.FieldName = svtr.FieldName;
                        col.OptionsColumn.AllowEdit = svtr.AllowEdit;
                        col.Caption = svtr.Caption;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        if (svtr.UnboundColumnType != null && svtr.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = svtr.UnboundColumnType;
                        if (svtr.Format != null)
                        {
                            col.Format.FormatString = svtr.Format.FormatString;
                            col.Format.FormatType = svtr.Format.FormatType;
                        }
                    }
                }

                if (HisMaterialInStockADO.ColumnButtonEdits != null && HisMaterialInStockADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMaterialInStockADO.ColumnButtonEdits)
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
                if (HisMaterialInStockADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMaterialInStockADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMaterialInStockADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMaterialInStockAdd.Visibility = (HisMaterialInStockADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                bool IsDetails = chkDetails.Checked;
                bool IsCollapseAll = chkCollapseAll.Checked;
                bool IsCollapseByMedicine = chkCollapseByMedicine.Checked;
                chkDetails.Checked = false;
                chkCollapseAll.Checked = false;
                chkCollapseByMedicine.Checked = false;
                BindingList<HisMaterialInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMaterialInStockADO> rearchResult = new List<HisMaterialInStockADO>();

                    rearchResult = HisMaterialInStockADOs.Where(o =>
                                                    ((o.MATERIAL_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MATERIAL_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MANUFACTURER_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERIAL_NUMBER ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MANUFACTURER_NAME_PARENT ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERIAL_NUMBER_PARENT ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<HisMaterialInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMaterialInStockADO>(HisMaterialInStockADOs);
                }
                trvService.DataSource = listResult;
                chkDetails.Checked = IsDetails;
                chkCollapseAll.Checked = IsCollapseAll;
                chkCollapseByMedicine.Checked = IsCollapseByMedicine;
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
        public List<HisMaterialInStockSDO> GetListAll()
        {
            List<HisMaterialInStockSDO> result = new List<HisMaterialInStockSDO>(); ;
            try
            {
                result = this.HisMaterialInStockADO.HisMaterialInStocks;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
        public void RefreshData(HisMaterialInStockADO item, decimal? parentAvailableAmount)
        {
            try
            {
                trvService.FocusedNode.SetValue("AvailableAmount", item.AvailableAmount);
                trvService.FocusedNode.ParentNode.SetValue("AvailableAmount", parentAvailableAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void Reload(List<HisMaterialInStockSDO> HisMaterialInStocks,List<long> MediStockID)
        {
            try
            {
                txtKeyword.Text = "";
                bool IsDetails = chkDetails.Checked;
                bool IsCollapseAll = chkCollapseAll.Checked;
                bool IsCollapseByMedicine = chkCollapseByMedicine.Checked;
                chkDetails.Checked = false;
                chkCollapseAll.Checked = false;
                chkCollapseByMedicine.Checked = false;
                this.MediStockIds=MediStockID;
                this.HisMaterialInStockADO.HisMaterialInStocks = HisMaterialInStocks;
                if (this.HisMaterialInStockADO.HisMaterialInStocks == null)
                    records = null;
                BindTreePlus();
                lstKey = new List<KeyTreeADO>();
                if (HisMaterialInStocks != null)
                {
                    foreach (var item in HisMaterialInStocks)
                    {
                        KeyTreeADO ado = new KeyTreeADO();
                        ado.NodeId = item.NodeId;
                        ado.ParentNodeId = item.ParentNodeId;
                        lstKey.Add(ado);
                    }
                    foreach (var item in lstKey)
                    {
                        if (!string.IsNullOrEmpty(item.ParentNodeId) && !lstKey.Exists(o => o.NodeId.Equals(item.ParentNodeId)))
                            item.ParentNodeId = null;
                    }
                    lstKey = lstKey.Where(o => !string.IsNullOrEmpty(o.ParentNodeId)).ToList();
                }
                chkDetails.Checked = IsDetails;
                chkCollapseAll.Checked = IsCollapseAll;
                chkCollapseByMedicine.Checked = IsCollapseByMedicine;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void DisposeControl()
        {
            try
            {
                rowFocus = 0;
                MediStockIds = null;
                isCreateParentNodeWithHisMaterialInStockExpend = false;
                menuItems = null;
                updateSingleRow = null;
                stateImageCollection = null;
                selectImageCollection = null;
                IsShowCheckNode = false;
                btnUnLock_buttonClick = null;
                btnLock_buttonClick = null;
                HisMaterialInStock_CustomDrawNodeCell = null;
                HisMaterialInStock_CheckAllNode = null;
                HisMaterialInStock_BeforeCheck = null;
                HisMaterialInStock_AfterCheck = null;
                HisMaterialInStock_SelectImageClick = null;
                HisMaterialInStock_StateImageClick = null;
                HisMaterialInStock_GetSelectImage = null;
                HisMaterialInStock_GetStateImage = null;
                HisMaterialInStockRowEnter = null;
                HisMaterialInStockDoubleClick = null;
                HisMaterialInStockClick = null;
                HisMaterialInStock_CustomUnboundColumnData = null;
                HisMaterialInStockNodeCellStyle = null;
                columnButtonEdits = null;
                HisMaterialInStockADOs = null;
                records = null;
                HisMaterialInStockADO = null;
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
                this.btnLock.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnLock_ButtonClick);
                this.btnUnLock.ButtonClick -= new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnUnLock_ButtonClick);
                this.btnNew.Click -= new System.EventHandler(this.btnNew_Click);
                this.txtKeyword.KeyUp -= new System.Windows.Forms.KeyEventHandler(this.txtKeyword_KeyUp);
                this.Load -= new System.EventHandler(this.UCServiceTree_Load);
                btnUnLock = null;
                btnLock = null;
                emptySpaceItem1 = null;
                layoutControlItem2 = null;
                lciHisMaterialInStockAdd = null;
                btnNew = null;
                repositoryItemCheckEdit4 = null;
                repositoryItemCheckEdit3 = null;
                repositoryItemCheckEdit2 = null;
                repositoryItemCheckEdit1 = null;
                repositoryItemchkIsExpend__Disable = null;
                repositoryItemchkIsExpend__Enable = null;
                layoutControlItem4 = null;
                lciKeyword = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
                txtKeyword = null;
                trvService = null;
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
                //var data = trvService.GetDataRecordByNode(e.Node);
                //if (HisMaterialInStockNodeCellStyle != null)
                //{
                //    HisMaterialInStockNodeCellStyle(data, e.Appearance);
                //}
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is HisMaterialInStockADO)
                {
                    var rowData = data as HisMaterialInStockADO;
                    if (rowData != null && this.HisMaterialInStockNodeCellStyle != null)
                    {
                        this.HisMaterialInStockNodeCellStyle(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void FocusRow()
        {
            try
            {
                if (rowFocus > 0)
                {
                    trvService.TopVisibleNodeIndex = rowFocus;
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
                    if (node != null && data != null && data is HisMaterialInStockADO)
                    {
                        if (HisMaterialInStockRowEnter != null)
                        {
                            HisMaterialInStockRowEnter((HisMaterialInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMaterialInStockADO)
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
                if (this.HisMaterialInStock_BeforeCheck != null)
                    this.HisMaterialInStock_BeforeCheck(node);
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
                if (this.HisMaterialInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMaterialInStockADO)
                {
                    HisMaterialInStock_AfterCheck(e.Node, (HisMaterialInStockADO)row);
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
                        HisMaterialInStockHandler clickhandler = btn.Tag as HisMaterialInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMaterialInStockADO)
                            {
                                clickhandler((HisMaterialInStockADO)data);
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    if (this.HisMaterialInStock_GetSelectImage != null)
                        this.HisMaterialInStock_GetSelectImage((HisMaterialInStockADO)data, e);
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    if (this.HisMaterialInStock_GetStateImage != null)
                        this.HisMaterialInStock_GetStateImage((HisMaterialInStockADO)data, e);
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    if (this.HisMaterialInStock_SelectImageClick != null)
                        this.HisMaterialInStock_SelectImageClick((HisMaterialInStockADO)data);
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    if (this.HisMaterialInStock_StateImageClick != null)
                        this.HisMaterialInStock_StateImageClick((HisMaterialInStockADO)data);
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
                    HisMaterialInStockADO currentRow = e.Row as HisMaterialInStockADO;
                    if (currentRow == null) return;
                    this.HisMaterialInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    var rowData = data as HisMaterialInStockADO;

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

                        if (rowData != null && rowData.IS_LEAF == 1)
                        {
                            rowData.bIsLeaf = true;
                        }
                        else
                        {
                            rowData.bIsLeaf = false;
                        }
                    }
                    if (e.Column.FieldName == "LOCK")
                    {
                        if (this.MediStockIds.Count == 1 && rowData.isTypeNode == false)
                        {
                            if (rowData.AvailableAmount > 0)
                            {
                                e.RepositoryItem = btnLock;
                            }
                            else if (rowData.AvailableAmount == 0 && rowData.TotalAmount > 0)//tồn > 0, khả dụng = 0
                            {
                                e.RepositoryItem = btnUnLock;
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

        private void repositoryItemchkIsExpend__Enable_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                if (data != null && data is HisMaterialInStockADO)
                {
                    var rowData = data as HisMaterialInStockADO;
                    if (rowData.bIsLeaf != null && rowData.bIsLeaf.Value)
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
                            if (data != null && data is HisMaterialInStockADO)
                            {
                                foreach (var menu in this.menuItems((HisMaterialInStockADO)data, hitInfo.Node))
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
                if (data != null && data is HisMaterialInStockADO)
                {
                    var rowData = data as HisMaterialInStockADO;
                    if (rowData != null && this.HisMaterialInStock_CustomDrawNodeCell != null)
                    {
                        this.HisMaterialInStock_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMaterialInStockADO> GetListCheck()
        {
            List<HisMaterialInStockADO> result = new List<HisMaterialInStockADO>();
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
                result = new List<HisMaterialInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisMaterialInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMaterialInStockADO)trvService.GetDataRecordByNode(node));
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
                    HisMaterialInStockADO HisMaterialInStockFocus = (HisMaterialInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMaterialInStockFocus != null && HisMaterialInStockDoubleClick != null)
                    {
                        HisMaterialInStockDoubleClick(HisMaterialInStockFocus);
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
                    HisMaterialInStockADO HisMaterialInStockFocus = (HisMaterialInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMaterialInStockFocus != null && this.HisMaterialInStockClick != null)
                    {
                        this.HisMaterialInStockClick(HisMaterialInStockFocus);
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

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                if (data != null && data is HisMaterialInStockADO)
                {
                    rowFocus = trvService.TopVisibleNodeIndex;
                    if (this.btnLock_buttonClick != null)
                        this.btnLock_buttonClick((HisMaterialInStockADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                if (data != null && data is HisMaterialInStockADO)
                {
                    rowFocus = trvService.TopVisibleNodeIndex;
                    if (this.btnUnLock_buttonClick != null)
                        this.btnUnLock_buttonClick((HisMaterialInStockADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkDetails_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkDetails.Checked)
                    Expand(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCollapseByMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCollapseByMedicine.Checked)
                {
                    trvService.CollapseAll();
                    foreach (var item in lstKey)
                    {
                        var node = trvService.FindNodeByKeyID(item.ParentNodeId);
                        if (lstKey.Exists(o => o.ParentNodeId.Equals(item.NodeId)))
                        {
                            if(node != null)
                                node.Expanded = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCollapseAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCollapseAll.Checked)
                    Expand(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
