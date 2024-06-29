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
using HIS.UC.HisMedicineInStock.ADO;
using MOS.SDO;
using HIS.UC.MedicineInStock.ADO;

namespace HIS.UC.HisMedicineInStock.Run
{
    public partial class UCHisMedicineInStock : UserControl
    {
        #region Declare
        HisMedicineInStockInitADO HisMedicineInStockADO;
        BindingList<HisMedicineInStockADO> records;
        List<HisMedicineInStockADO> HisMedicineInStockADOs = new List<HisMedicineInStockADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMedicineInStock_NodeCellStyle HisMedicineInStockNodeCellStyle;
        HisMedicineInStock_CustomUnboundColumnData HisMedicineInStock_CustomUnboundColumnData;
        HisMedicineInStockHandler HisMedicineInStockClick;
        HisMedicineInStockHandler HisMedicineInStockDoubleClick;
        HisMedicineInStockHandler HisMedicineInStockRowEnter;
        HisMedicineInStock_GetStateImage HisMedicineInStock_GetStateImage;
        HisMedicineInStock_GetSelectImage HisMedicineInStock_GetSelectImage;
        HisMedicineInStockHandler HisMedicineInStock_StateImageClick;
        HisMedicineInStockHandler HisMedicineInStock_SelectImageClick;
        HisMedicineInStock_AfterCheck HisMedicineInStock_AfterCheck;
        HisMedicineInStock_BeforeCheck HisMedicineInStock_BeforeCheck;
        HisMedicineInStock_CheckAllNode HisMedicineInStock_CheckAllNode;
        HisMedicineInStock_CustomDrawNodeCell HisMedicineInStock_CustomDrawNodeCell;
        btnLock_buttonClick btnLock_buttonClick;
        btnUnLock_buttonClick btnUnLock_buttonClick;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMedicineInStockHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMedicineInStockExpend = true;
        List<long> MediStockIds;
        int rowFocus;
        List<KeyTreeADO> lstKey = new List<KeyTreeADO>();
        #endregion

        #region Construct
        public UCHisMedicineInStock(HisMedicineInStockInitADO HisMedicineInStockADO)
        {
            InitializeComponent();
            try
            {
                this.HisMedicineInStockADO = HisMedicineInStockADO;
                this.HisMedicineInStockNodeCellStyle = HisMedicineInStockADO.HisMedicineInStockNodeCellStyle;
                this.HisMedicineInStockClick = HisMedicineInStockADO.HisMedicineInStockClick;
                this.HisMedicineInStockDoubleClick = HisMedicineInStockADO.HisMedicineInStockDoubleClick;
                this.HisMedicineInStockRowEnter = HisMedicineInStockADO.HisMedicineInStockRowEnter;
                this.HisMedicineInStock_GetStateImage = HisMedicineInStockADO.HisMedicineInStock_GetStateImage;
                this.HisMedicineInStock_GetSelectImage = HisMedicineInStockADO.HisMedicineInStock_GetSelectImage;
                this.HisMedicineInStock_StateImageClick = HisMedicineInStockADO.HisMedicineInStock_StateImageClick;
                this.HisMedicineInStock_SelectImageClick = HisMedicineInStockADO.HisMedicineInStock_SelectImageClick;
                this.columnButtonEdits = HisMedicineInStockADO.ColumnButtonEdits;
                this.selectImageCollection = HisMedicineInStockADO.SelectImageCollection;
                this.stateImageCollection = HisMedicineInStockADO.StateImageCollection;
                this.updateSingleRow = HisMedicineInStockADO.UpdateSingleRow;
                this.HisMedicineInStock_CustomUnboundColumnData = HisMedicineInStockADO.HisMedicineInStock_CustomUnboundColumnData;
                this.menuItems = HisMedicineInStockADO.MenuItems;
                this.HisMedicineInStock_AfterCheck = HisMedicineInStockADO.HisMedicineInStock_AfterCheck;
                this.HisMedicineInStock_BeforeCheck = HisMedicineInStockADO.HisMedicineInStock_BeforeCheck;
                this.HisMedicineInStock_CheckAllNode = HisMedicineInStockADO.HisMedicineInStock_CheckAllNode;
                this.HisMedicineInStock_CustomDrawNodeCell = HisMedicineInStockADO.HisMedicineInStock_CustomDrawNodeCell;
                this.btnLock_buttonClick = HisMedicineInStockADO.btnLock_buttonClick;
                this.btnUnLock_buttonClick = HisMedicineInStockADO.btnUnLock_buttonClick;
                if (HisMedicineInStockADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMedicineInStockADO.IsShowCheckNode.Value;
                }
                if (HisMedicineInStockADO.IsCreateParentNodeWithHisMedicineInStockExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMedicineInStockExpend = HisMedicineInStockADO.IsCreateParentNodeWithHisMedicineInStockExpend.Value;
                }
                if (HisMedicineInStockADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = HisMedicineInStockADO.IsAutoWidth.Value;
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
                if (HisMedicineInStockADO != null)
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
        public void RefreshData(HisMedicineInStockADO item, decimal? parentAvailableAmount)
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

        private void BindTreePlus()
        {
            try
            {
                HisMedicineInStockADOs = new List<HisMedicineInStockADO>();
                if (HisMedicineInStockADO.HisMedicineInStocks != null)
                {
                //    HisMedicineInStockADOs = (from r in HisMedicineInStockADO.HisMedicineInStocks select new HisMedicineInStockADO(r)).ToList();
                    foreach (var r in HisMedicineInStockADO.HisMedicineInStocks)
                    {
                        HisMedicineInStockADO item = new HisMedicineInStockADO(r);
                        var child = HisMedicineInStockADO.HisMedicineInStocks.Where(o => o.ParentNodeId == r.NodeId).ToList();
                        if (child != null && child.Count > 0)
                        {
                            item.MEDICINE_TYPE_HEIN_NAME_PARENT = String.Join(";", child.Select(o=>o.MEDICINE_TYPE_HEIN_NAME));
                            item.ACTIVE_INGR_BHYT_CODE_PARENT = String.Join(";", child.Select(o => o.ACTIVE_INGR_BHYT_CODE));
                            item.ACTIVE_INGR_BHYT_NAME_PARENT = String.Join(";", child.Select(o => o.ACTIVE_INGR_BHYT_NAME));
                        }
                        HisMedicineInStockADOs.Add(item);
                    }
                    HisMedicineInStockADOs = HisMedicineInStockADOs.OrderBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMedicineInStockADO>(HisMedicineInStockADOs);
                if (!String.IsNullOrEmpty(HisMedicineInStockADO.KeyFieldName))
                    trvService.KeyFieldName = HisMedicineInStockADO.KeyFieldName;
                if (!String.IsNullOrEmpty(HisMedicineInStockADO.ParentFieldName))
                    trvService.ParentFieldName = HisMedicineInStockADO.ParentFieldName;
                trvService.DataSource = records;
                if (this.HisMedicineInStock_CheckAllNode != null)
                    this.HisMedicineInStock_CheckAllNode(trvService.Nodes);
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
                if (HisMedicineInStockADO.HisMedicineInStockColumns != null && HisMedicineInStockADO.HisMedicineInStockColumns.Count > 0)
                {
                    foreach (var svtr in HisMedicineInStockADO.HisMedicineInStockColumns)
                    {
                        TreeListColumn col = this.trvService.Columns.AddField(svtr.FieldName);
                        col.Visible = svtr.Visible;
                        col.VisibleIndex = svtr.VisibleIndex;
                        col.Width = svtr.ColumnWidth;
                        col.FieldName = svtr.FieldName;
                        col.OptionsColumn.AllowEdit = svtr.AllowEdit;
                        col.Caption = svtr.Caption;
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        if (svtr.isHAlignment)
                        {
                            col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                        }
                        if (svtr.UnboundColumnType != null && svtr.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = svtr.UnboundColumnType;
                        if (svtr.Format != null)
                        {
                            col.Format.FormatString = svtr.Format.FormatString;
                            col.Format.FormatType = svtr.Format.FormatType;
                        }
                    }
                }

                if (HisMedicineInStockADO.ColumnButtonEdits != null && HisMedicineInStockADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMedicineInStockADO.ColumnButtonEdits)
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
                if (HisMedicineInStockADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMedicineInStockADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMedicineInStockADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMedicineInStockAdd.Visibility = (HisMedicineInStockADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(HisMedicineInStockADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = HisMedicineInStockADO.Keyword_NullValuePrompt;
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
                BindingList<HisMedicineInStockADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMedicineInStockADO> rearchResult = new List<HisMedicineInStockADO>();

                    rearchResult = HisMedicineInStockADOs.Where(o =>
                                                    ((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_HEIN_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_HEIN_NAME_PARENT ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_CODE_PARENT ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_NAME_PARENT ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();
                    listResult = new BindingList<HisMedicineInStockADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMedicineInStockADO>(HisMedicineInStockADOs);
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

        public void Reload(List<HisMedicineInStockSDO> HisMedicineInStocks, List<long> MediStockID)
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
                this.MediStockIds = MediStockID;
                if (HisMedicineInStockADO == null)
                    HisMedicineInStockADO = new HisMedicineInStockInitADO();
                this.HisMedicineInStockADO.HisMedicineInStocks = HisMedicineInStocks;
                if (this.HisMedicineInStockADO.HisMedicineInStocks == null)
                    records = null;
                BindTreePlus();                
                lstKey = new List<KeyTreeADO>();
                if (HisMedicineInStocks != null)
                {
                    foreach (var item in HisMedicineInStocks)
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
        public List<HisMedicineInStockSDO> GetListAll()
        {
            List<HisMedicineInStockSDO> result = new List<HisMedicineInStockSDO>(); ;
            try
            {
                result = this.HisMedicineInStockADO.HisMedicineInStocks;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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
        public void DisposeControl()
        {
            try
            {
                rowFocus = 0;
                MediStockIds = null;
                isCreateParentNodeWithHisMedicineInStockExpend = false;
                menuItems = null;
                updateSingleRow = null;
                stateImageCollection = null;
                selectImageCollection = null;
                isAutoWidth = false;
                IsShowCheckNode = false;
                btnUnLock_buttonClick = null;
                btnLock_buttonClick = null;
                HisMedicineInStock_CustomDrawNodeCell = null;
                HisMedicineInStock_CheckAllNode = null;
                HisMedicineInStock_BeforeCheck = null;
                HisMedicineInStock_AfterCheck = null;
                HisMedicineInStock_SelectImageClick = null;
                HisMedicineInStock_StateImageClick = null;
                HisMedicineInStock_GetSelectImage = null;
                HisMedicineInStock_GetStateImage = null;
                HisMedicineInStockRowEnter = null;
                HisMedicineInStockDoubleClick = null;
                HisMedicineInStockClick = null;
                HisMedicineInStock_CustomUnboundColumnData = null;
                HisMedicineInStockNodeCellStyle = null;
                columnButtonEdits = null;
                HisMedicineInStockADOs = null;
                records = null;
                HisMedicineInStockADO = null;
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
                lciHisMedicineInStockAdd = null;
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
                //if (HisMedicineInStockNodeCellStyle != null)
                //{
                //    HisMedicineInStockNodeCellStyle(data, e.Appearance);
                //}
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is HisMedicineInStockADO)
                {
                    var rowData = data as HisMedicineInStockADO;
                    if (rowData != null && this.HisMedicineInStockNodeCellStyle != null)
                    {
                        this.HisMedicineInStockNodeCellStyle(rowData, e);
                    }
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
                    if (node != null && data != null && data is HisMedicineInStockADO)
                    {
                        if (HisMedicineInStockRowEnter != null)
                        {
                            HisMedicineInStockRowEnter((HisMedicineInStockADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMedicineInStockADO)
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
                if (this.HisMedicineInStock_BeforeCheck != null)
                    this.HisMedicineInStock_BeforeCheck(node);
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
                if (this.HisMedicineInStock_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMedicineInStockADO)
                {
                    HisMedicineInStock_AfterCheck(e.Node, (HisMedicineInStockADO)row);
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
                        HisMedicineInStockHandler clickhandler = btn.Tag as HisMedicineInStockHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMedicineInStockADO)
                            {
                                clickhandler((HisMedicineInStockADO)data);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    if (this.HisMedicineInStock_GetSelectImage != null)
                        this.HisMedicineInStock_GetSelectImage((HisMedicineInStockADO)data, e);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    if (this.HisMedicineInStock_GetStateImage != null)
                        this.HisMedicineInStock_GetStateImage((HisMedicineInStockADO)data, e);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    if (this.HisMedicineInStock_SelectImageClick != null)
                        this.HisMedicineInStock_SelectImageClick((HisMedicineInStockADO)data);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    if (this.HisMedicineInStock_StateImageClick != null)
                        this.HisMedicineInStock_StateImageClick((HisMedicineInStockADO)data);
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
                    HisMedicineInStockADO currentRow = e.Row as HisMedicineInStockADO;
                    if (currentRow == null) return;
                    this.HisMedicineInStock_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    var rowData = data as HisMedicineInStockADO;

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
                if (data != null && data is HisMedicineInStockADO)
                {
                    var rowData = data as HisMedicineInStockADO;
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
                            if (data != null && data is HisMedicineInStockADO)
                            {
                                foreach (var menu in this.menuItems((HisMedicineInStockADO)data, hitInfo.Node))
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    var rowData = data as HisMedicineInStockADO;
                    if (rowData != null && this.HisMedicineInStock_CustomDrawNodeCell != null)
                    {
                        this.HisMedicineInStock_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMedicineInStockADO> GetListCheck()
        {
            List<HisMedicineInStockADO> result = new List<HisMedicineInStockADO>();
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
                result = new List<HisMedicineInStockADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<HisMedicineInStockADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMedicineInStockADO)trvService.GetDataRecordByNode(node));
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
                    HisMedicineInStockADO HisMedicineInStockFocus = (HisMedicineInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMedicineInStockFocus != null && HisMedicineInStockDoubleClick != null)
                    {
                        HisMedicineInStockDoubleClick(HisMedicineInStockFocus);
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
                    HisMedicineInStockADO HisMedicineInStockFocus = (HisMedicineInStockADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMedicineInStockFocus != null && this.HisMedicineInStockClick != null)
                    {
                        this.HisMedicineInStockClick(HisMedicineInStockFocus);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    rowFocus = trvService.TopVisibleNodeIndex;
                    if (this.btnLock_buttonClick != null)
                        this.btnLock_buttonClick((HisMedicineInStockADO)data);
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
                if (data != null && data is HisMedicineInStockADO)
                {
                    rowFocus = trvService.TopVisibleNodeIndex;
                    if (this.btnUnLock_buttonClick != null)
                        this.btnUnLock_buttonClick((HisMedicineInStockADO)data);
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
                        if (lstKey.Exists(o=>o.ParentNodeId.Equals(item.NodeId)))
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
    }
}
