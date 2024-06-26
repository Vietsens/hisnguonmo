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
using HIS.UC.HisMateInStockByExpireDate.ADO;
using MOS.SDO;

namespace HIS.UC.HisMateInStockByExpireDate.Run
{
    public partial class UCHisMateInStockByExpireDate : UserControl
    {
        #region Declare
        HisMateInStockByExpireDateInitADO HisMateInStockByExpireDateADO;
        BindingList<HisMateInStockByExpireDateADO> records;
        List<HisMateInStockByExpireDateADO> HisMateInStockByExpireDateADOs = new List<HisMateInStockByExpireDateADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMateInStockByExpireDate_NodeCellStyle HisMateInStockByExpireDateNodeCellStyle;
        HisMateInStockByExpireDate_CustomUnboundColumnData HisMateInStockByExpireDate_CustomUnboundColumnData;
        HisMateInStockByExpireDateHandler HisMateInStockByExpireDateClick;
        HisMateInStockByExpireDateHandler HisMateInStockByExpireDateDoubleClick;
        HisMateInStockByExpireDateHandler HisMateInStockByExpireDateRowEnter;
        HisMateInStockByExpireDate_GetStateImage HisMateInStockByExpireDate_GetStateImage;
        HisMateInStockByExpireDate_GetSelectImage HisMateInStockByExpireDate_GetSelectImage;
        HisMateInStockByExpireDateHandler HisMateInStockByExpireDate_StateImageClick;
        HisMateInStockByExpireDateHandler HisMateInStockByExpireDate_SelectImageClick;
        HisMateInStockByExpireDate_AfterCheck HisMateInStockByExpireDate_AfterCheck;
        HisMateInStockByExpireDate_BeforeCheck HisMateInStockByExpireDate_BeforeCheck;
        HisMateInStockByExpireDate_CheckAllNode HisMateInStockByExpireDate_CheckAllNode;
        HisMateInStockByExpireDate_CustomDrawNodeCell HisMateInStockByExpireDate_CustomDrawNodeCell;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMateInStockByExpireDateHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMateInStockByExpireDateExpend = true;
        List<long> _MaterialTypeIds;
        #endregion

        #region Construct
        public UCHisMateInStockByExpireDate(HisMateInStockByExpireDateInitADO HisMateInStockByExpireDateADO)
        {
            InitializeComponent();
            try
            {
                this.HisMateInStockByExpireDateADO = HisMateInStockByExpireDateADO;
                this.HisMateInStockByExpireDateNodeCellStyle = HisMateInStockByExpireDateADO.HisMateInStockByExpireDateNodeCellStyle;
                this.HisMateInStockByExpireDateClick = HisMateInStockByExpireDateADO.HisMateInStockByExpireDateClick;
                this.HisMateInStockByExpireDateDoubleClick = HisMateInStockByExpireDateADO.HisMateInStockByExpireDateDoubleClick;
                this.HisMateInStockByExpireDateRowEnter = HisMateInStockByExpireDateADO.HisMateInStockByExpireDateRowEnter;
                this.HisMateInStockByExpireDate_GetStateImage = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_GetStateImage;
                this.HisMateInStockByExpireDate_GetSelectImage = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_GetSelectImage;
                this.HisMateInStockByExpireDate_StateImageClick = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_StateImageClick;
                this.HisMateInStockByExpireDate_SelectImageClick = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_SelectImageClick;
                this.columnButtonEdits = HisMateInStockByExpireDateADO.ColumnButtonEdits;
                this.selectImageCollection = HisMateInStockByExpireDateADO.SelectImageCollection;
                this.stateImageCollection = HisMateInStockByExpireDateADO.StateImageCollection;
                this.updateSingleRow = HisMateInStockByExpireDateADO.UpdateSingleRow;
                this.HisMateInStockByExpireDate_CustomUnboundColumnData = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_CustomUnboundColumnData;
                this.menuItems = HisMateInStockByExpireDateADO.MenuItems;
                this.HisMateInStockByExpireDate_AfterCheck = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_AfterCheck;
                this.HisMateInStockByExpireDate_BeforeCheck = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_BeforeCheck;
                this.HisMateInStockByExpireDate_CheckAllNode = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_CheckAllNode;
                this.HisMateInStockByExpireDate_CustomDrawNodeCell = HisMateInStockByExpireDateADO.HisMateInStockByExpireDate_CustomDrawNodeCell;
                if (HisMateInStockByExpireDateADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMateInStockByExpireDateADO.IsShowCheckNode.Value;
                }
                if (HisMateInStockByExpireDateADO.IsCreateParentNodeWithHisMateInStockByExpireDateExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMateInStockByExpireDateExpend = HisMateInStockByExpireDateADO.IsCreateParentNodeWithHisMateInStockByExpireDateExpend.Value;
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
                if (HisMateInStockByExpireDateADO != null)
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
                HisMateInStockByExpireDateADOs = new List<HisMateInStockByExpireDateADO>();
                if (HisMateInStockByExpireDateADO.HisMateInStockByExpireDates != null)
                {
                    foreach (var listItem in HisMateInStockByExpireDateADO.HisMateInStockByExpireDates)
                    {
                        List<HisMateInStockByExpireDateADO> lstADO = new List<ADO.HisMateInStockByExpireDateADO>();
                        var itemExpriedDate = listItem[0].EXPIRED_DATE;
                        HisMateInStockByExpireDateADO parentAdo = new HisMateInStockByExpireDateADO();
                        if (itemExpriedDate != null && itemExpriedDate > 0)
                        {
                            parentAdo.CONCRETE_ID__IN_DATE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExpriedDate.ToString());
                            parentAdo.MATERIAL_TYPE_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExpriedDate.ToString());
                            HisMateInStockByExpireDateADOs.Add(parentAdo);
                            var listParent = listItem.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                            foreach (var parent in listParent)
                            {
                                //if (this._MaterialTypeIds != null && this._MaterialTypeIds.Count > 0)
                                //{
                                //    if (!this._MaterialTypeIds.Contains(parent.MATERIAL_TYPE_ID))
                                //    {
                                //        continue;
                                //    }
                                //}
                                HisMateInStockByExpireDateADO parentLevel1 = new HisMateInStockByExpireDateADO(parent);
                                parentLevel1.CONCRETE_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE + "_" + parent.ID;
                                parentLevel1.PARENT_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE;
                                HisMateInStockByExpireDateADOs.Add(parentLevel1);
                                CreateListChildByParent(parentLevel1, listItem);

                            }
                        }
                        else
                        {
                            parentAdo.CONCRETE_ID__IN_DATE = "Không xác định";
                            parentAdo.MATERIAL_TYPE_CODE = "Không xác định";
                            HisMateInStockByExpireDateADOs.Add(parentAdo);
                            var listParent = listItem.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                            foreach (var parent in listParent)
                            {
                                //if (this._MaterialTypeIds != null && this._MaterialTypeIds.Count > 0)
                                //{
                                //    if (!this._MaterialTypeIds.Contains(parent.MATERIAL_TYPE_ID))
                                //    {
                                //        continue;
                                //    }
                                //}
                                HisMateInStockByExpireDateADO parentLevel1 = new HisMateInStockByExpireDateADO(parent);
                                parentLevel1.CONCRETE_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE + "_" + parent.ID;
                                parentLevel1.PARENT_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE;
                                HisMateInStockByExpireDateADOs.Add(parentLevel1);
                                CreateListChildByParent(parentLevel1, listItem);
                            }
                        }
                    }


                    HisMateInStockByExpireDateADOs = HisMateInStockByExpireDateADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMateInStockByExpireDateADO>(HisMateInStockByExpireDateADOs);
                trvService.DataSource = records;
                trvService.ExpandToLevel(0);
                if (this.HisMateInStockByExpireDate_CheckAllNode != null)
                    this.HisMateInStockByExpireDate_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateListChildByParent(HisMateInStockByExpireDateADO parentLevel1, List<MOS.SDO.HisMaterialInStockSDO> listItem)
        {
            try
            {
                var listchild = listItem.Where(o => o.ParentNodeId == parentLevel1.NodeId).ToList();
                if (listchild != null)
                {
                    for (int child = 0; child < listchild.Count; child++)
                    {
                        HisMateInStockByExpireDateADO childAdo = new HisMateInStockByExpireDateADO(listchild[child]);
                        childAdo.CONCRETE_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE + "_" + listchild[child].ID;
                        childAdo.PARENT_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE;
                        //Inventec.Common.Mapper.DataObjectMapper.Map<HisMediInStockByExpireDateADO>(childAdo, child);
                        string[] str = childAdo.CONCRETE_ID__IN_DATE.Split('_');
                        if (str.Length == 3)
                        {
                            childAdo.IS_MEDI_MATE = true;
                        }
                        if (child > 0 && listchild[child].MATERIAL_TYPE_CODE == listchild[child - 1].MATERIAL_TYPE_CODE)
                        {
                            childAdo.IS_MEDI_MATE = false;
                        }
                        childAdo.bIsLeaf = (listchild[child].IS_LEAF == 1);
                        HisMateInStockByExpireDateADOs.Add(childAdo);
                        CreateListChildByParent(childAdo, listItem);
                    }

                    //foreach (var child in listchild)
                    //{
                    //    HisMateInStockByExpireDateADO childAdo = new HisMateInStockByExpireDateADO(child);
                    //    childAdo.CONCRETE_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE + "_" + child.ID;
                    //    childAdo.PARENT_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE;
                    //    //Inventec.Common.Mapper.DataObjectMapper.Map<HisMateInStockByExpireDateADO>(childAdo, child);
                    //    //childAdo.bIsLeaf = (child.IS_LEAF == IMSys.DbConfig.HIS_RS.HIS_MEDICINE_TYPE.IS_LEAF__TRUE);
                    //    HisMateInStockByExpireDateADOs.Add(childAdo);
                    //    CreateListChildByParent(childAdo, listItem);
                    //}
                }
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
                if (HisMateInStockByExpireDateADO.HisMateInStockByExpireDateColumns != null && HisMateInStockByExpireDateADO.HisMateInStockByExpireDateColumns.Count > 0)
                {
                    foreach (var svtr in HisMateInStockByExpireDateADO.HisMateInStockByExpireDateColumns)
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

                if (HisMateInStockByExpireDateADO.ColumnButtonEdits != null && HisMateInStockByExpireDateADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMateInStockByExpireDateADO.ColumnButtonEdits)
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
                if (HisMateInStockByExpireDateADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMateInStockByExpireDateADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMateInStockByExpireDateADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMateInStockByExpireDateAdd.Visibility = (HisMateInStockByExpireDateADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                BindingList<HisMateInStockByExpireDateADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMateInStockByExpireDateADO> rearchResult = new List<HisMateInStockByExpireDateADO>();

                    rearchResult = HisMateInStockByExpireDateADOs.Where(o =>
                                                     ((o.MATERIAL_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MATERIAL_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                         //|| (o.MATERIAL_TYPE_HEIN_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     )
                                                     ).Distinct().ToList();


                    listResult = new BindingList<HisMateInStockByExpireDateADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMateInStockByExpireDateADO>(HisMateInStockByExpireDateADOs);
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

        public void Reload(List<List<HisMaterialInStockSDO>> HisMateInStockByExpireDates, List<long> MedicineTypeIds)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisMateInStockByExpireDateADO.HisMateInStockByExpireDates = HisMateInStockByExpireDates;
                if (this.HisMateInStockByExpireDateADO.HisMateInStockByExpireDates == null)
                    records = null;
                this._MaterialTypeIds = MedicineTypeIds;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<HisMateInStockByExpireDateADO> GetListTreeView()
        {
            List<HisMateInStockByExpireDateADO> result = new List<HisMateInStockByExpireDateADO>();
            try
            {
                if (records != null)
                {
                    result = HisMateInStockByExpireDateADOs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<HisMateInStockByExpireDateADO>();
            }
            return result;
        }
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (HisMateInStockByExpireDateNodeCellStyle != null)
                {
                    HisMateInStockByExpireDateNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is HisMateInStockByExpireDateADO)
                    {
                        if (HisMateInStockByExpireDateRowEnter != null)
                        {
                            HisMateInStockByExpireDateRowEnter((HisMateInStockByExpireDateADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMateInStockByExpireDateADO)
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
                if (this.HisMateInStockByExpireDate_BeforeCheck != null)
                    this.HisMateInStockByExpireDate_BeforeCheck(node);
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
                if (this.HisMateInStockByExpireDate_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMateInStockByExpireDateADO)
                {
                    HisMateInStockByExpireDate_AfterCheck(e.Node, (HisMateInStockByExpireDateADO)row);
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
                        HisMateInStockByExpireDateHandler clickhandler = btn.Tag as HisMateInStockByExpireDateHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMateInStockByExpireDateADO)
                            {
                                clickhandler((HisMateInStockByExpireDateADO)data);
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    if (this.HisMateInStockByExpireDate_GetSelectImage != null)
                        this.HisMateInStockByExpireDate_GetSelectImage((HisMateInStockByExpireDateADO)data, e);
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    if (this.HisMateInStockByExpireDate_GetStateImage != null)
                        this.HisMateInStockByExpireDate_GetStateImage((HisMateInStockByExpireDateADO)data, e);
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    if (this.HisMateInStockByExpireDate_SelectImageClick != null)
                        this.HisMateInStockByExpireDate_SelectImageClick((HisMateInStockByExpireDateADO)data);
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    if (this.HisMateInStockByExpireDate_StateImageClick != null)
                        this.HisMateInStockByExpireDate_StateImageClick((HisMateInStockByExpireDateADO)data);
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
                    HisMateInStockByExpireDateADO currentRow = e.Row as HisMateInStockByExpireDateADO;
                    if (HisMateInStockByExpireDate_CustomUnboundColumnData == null || currentRow == null) return;
                    this.HisMateInStockByExpireDate_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    var rowData = data as HisMateInStockByExpireDateADO;

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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    var rowData = data as HisMateInStockByExpireDateADO;
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
                            if (data != null && data is HisMateInStockByExpireDateADO)
                            {
                                foreach (var menu in this.menuItems((HisMateInStockByExpireDateADO)data))
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
                if (data != null && data is HisMateInStockByExpireDateADO)
                {
                    var rowData = data as HisMateInStockByExpireDateADO;
                    if (rowData != null && this.HisMateInStockByExpireDate_CustomDrawNodeCell != null)
                    {
                        this.HisMateInStockByExpireDate_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMateInStockByExpireDateADO> GetListCheck()
        {
            List<HisMateInStockByExpireDateADO> result = new List<HisMateInStockByExpireDateADO>();
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
                result = new List<HisMateInStockByExpireDateADO>();
            }
            return result;
        }

        //List<HisMateInStockByExpireDateADO> GetChild(BindingList<HisMateInStockByExpireDateADO> listData, HisMateInStockByExpireDateADO parent)
        //{
        //    List<HisMateInStockByExpireDateADO> result = new List<HisMateInStockByExpireDateADO>();
        //    var childData = listData.Where(o => o.PARENT_ID__IN_DATE == parent.CONCRETE_ID__IN_DATE).ToList();
        //    if (childData != null && childData.Count > 0)
        //    {
        //        result.AddRange(childData);
        //        foreach (var item in childData)
        //        {
        //            var childChild = GetChild(listData, item);
        //            if (childChild != null && childChild.Count > 0)
        //            {
        //                result.AddRange(childChild);
        //            }
        //        }
        //    }
        //    return result;
        //}

        private void GetListNodeCheck(ref List<HisMateInStockByExpireDateADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMateInStockByExpireDateADO)trvService.GetDataRecordByNode(node));
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
                    HisMateInStockByExpireDateADO HisMateInStockByExpireDateFocus = (HisMateInStockByExpireDateADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMateInStockByExpireDateFocus != null && HisMateInStockByExpireDateDoubleClick != null)
                    {
                        HisMateInStockByExpireDateDoubleClick(HisMateInStockByExpireDateFocus);
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
                    HisMateInStockByExpireDateADO HisMateInStockByExpireDateFocus = (HisMateInStockByExpireDateADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMateInStockByExpireDateFocus != null && this.HisMateInStockByExpireDateClick != null)
                    {
                        this.HisMateInStockByExpireDateClick(HisMateInStockByExpireDateFocus);
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

        internal void ExportExcel()
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                if (saveFile.ShowDialog() == DialogResult.OK)
                {
                    this.trvService.ExportToXlsx(saveFile.FileName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
