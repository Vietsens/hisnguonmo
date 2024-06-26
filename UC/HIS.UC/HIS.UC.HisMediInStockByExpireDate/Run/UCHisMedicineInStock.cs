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
using HIS.UC.HisMediInStockByExpireDate.ADO;
using MOS.SDO;

namespace HIS.UC.HisMediInStockByExpireDate.Run
{
    public partial class UCHisMediInStockByExpireDate : UserControl
    {
        #region Declare
        HisMediInStockByExpireDateInitADO HisMediInStockByExpireDateADO;
        BindingList<HisMediInStockByExpireDateADO> records;
        List<HisMediInStockByExpireDateADO> HisMediInStockByExpireDateADOs = new List<HisMediInStockByExpireDateADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        HisMediInStockByExpireDate_NodeCellStyle HisMediInStockByExpireDateNodeCellStyle;
        HisMediInStockByExpireDate_CustomUnboundColumnData HisMediInStockByExpireDate_CustomUnboundColumnData;
        HisMediInStockByExpireDateHandler HisMediInStockByExpireDateClick;
        HisMediInStockByExpireDateHandler HisMediInStockByExpireDateDoubleClick;
        HisMediInStockByExpireDateHandler HisMediInStockByExpireDateRowEnter;
        HisMediInStockByExpireDate_GetStateImage HisMediInStockByExpireDate_GetStateImage;
        HisMediInStockByExpireDate_GetSelectImage HisMediInStockByExpireDate_GetSelectImage;
        HisMediInStockByExpireDateHandler HisMediInStockByExpireDate_StateImageClick;
        HisMediInStockByExpireDateHandler HisMediInStockByExpireDate_SelectImageClick;
        HisMediInStockByExpireDate_AfterCheck HisMediInStockByExpireDate_AfterCheck;
        HisMediInStockByExpireDate_BeforeCheck HisMediInStockByExpireDate_BeforeCheck;
        HisMediInStockByExpireDate_CheckAllNode HisMediInStockByExpireDate_CheckAllNode;
        HisMediInStockByExpireDate_CustomDrawNodeCell HisMediInStockByExpireDate_CustomDrawNodeCell;
        bool IsShowCheckNode;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        HisMediInStockByExpireDateHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithHisMediInStockByExpireDateExpend = true;
        List<long> _MedicineTypeIds;
        #endregion

        #region Construct
        public UCHisMediInStockByExpireDate(HisMediInStockByExpireDateInitADO HisMediInStockByExpireDateADO)
        {
            InitializeComponent();
            try
            {
                this.HisMediInStockByExpireDateADO = HisMediInStockByExpireDateADO;
                this.HisMediInStockByExpireDateNodeCellStyle = HisMediInStockByExpireDateADO.HisMediInStockByExpireDateNodeCellStyle;
                this.HisMediInStockByExpireDateClick = HisMediInStockByExpireDateADO.HisMediInStockByExpireDateClick;
                this.HisMediInStockByExpireDateDoubleClick = HisMediInStockByExpireDateADO.HisMediInStockByExpireDateDoubleClick;
                this.HisMediInStockByExpireDateRowEnter = HisMediInStockByExpireDateADO.HisMediInStockByExpireDateRowEnter;
                this.HisMediInStockByExpireDate_GetStateImage = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_GetStateImage;
                this.HisMediInStockByExpireDate_GetSelectImage = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_GetSelectImage;
                this.HisMediInStockByExpireDate_StateImageClick = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_StateImageClick;
                this.HisMediInStockByExpireDate_SelectImageClick = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_SelectImageClick;
                this.columnButtonEdits = HisMediInStockByExpireDateADO.ColumnButtonEdits;
                this.selectImageCollection = HisMediInStockByExpireDateADO.SelectImageCollection;
                this.stateImageCollection = HisMediInStockByExpireDateADO.StateImageCollection;
                this.updateSingleRow = HisMediInStockByExpireDateADO.UpdateSingleRow;
                this.HisMediInStockByExpireDate_CustomUnboundColumnData = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_CustomUnboundColumnData;
                this.menuItems = HisMediInStockByExpireDateADO.MenuItems;
                this.HisMediInStockByExpireDate_AfterCheck = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_AfterCheck;
                this.HisMediInStockByExpireDate_BeforeCheck = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_BeforeCheck;
                this.HisMediInStockByExpireDate_CheckAllNode = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_CheckAllNode;
                this.HisMediInStockByExpireDate_CustomDrawNodeCell = HisMediInStockByExpireDateADO.HisMediInStockByExpireDate_CustomDrawNodeCell;
                if (HisMediInStockByExpireDateADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = HisMediInStockByExpireDateADO.IsShowCheckNode.Value;
                }
                if (HisMediInStockByExpireDateADO.IsCreateParentNodeWithHisMediInStockByExpireDateExpend.HasValue)
                {
                    this.isCreateParentNodeWithHisMediInStockByExpireDateExpend = HisMediInStockByExpireDateADO.IsCreateParentNodeWithHisMediInStockByExpireDateExpend.Value;
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
                if (HisMediInStockByExpireDateADO != null)
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
                HisMediInStockByExpireDateADOs = new List<HisMediInStockByExpireDateADO>();
                if (HisMediInStockByExpireDateADO.HisMediInStockByExpireDates != null)
                {
                    foreach (var listItem in HisMediInStockByExpireDateADO.HisMediInStockByExpireDates)
                    {
                        //List<HisMediInStockByExpireDateADO> lstADO = new List<ADO.HisMediInStockByExpireDateADO>();
                        // lstADO.AddRange((from r in listItem select new HisMediInStockByExpireDateADO(r)).ToList());
                        var itemExpriedDate = listItem[0].EXPIRED_DATE;
                        HisMediInStockByExpireDateADO parentAdo = new HisMediInStockByExpireDateADO();
                        if (itemExpriedDate != null && itemExpriedDate > 0)
                        {
                            parentAdo.CONCRETE_ID__IN_DATE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExpriedDate.ToString());
                            parentAdo.MEDICINE_TYPE_CODE = Inventec.Common.DateTime.Convert.TimeNumberToDateString(itemExpriedDate.ToString());
                            HisMediInStockByExpireDateADOs.Add(parentAdo);
                            var listParent = listItem.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                            foreach (var parent in listParent)
                            {
                                //if (this._MedicineTypeIds != null && this._MedicineTypeIds.Count > 0)
                                //{
                                //    if (!this._MedicineTypeIds.Contains(parent.MEDICINE_TYPE_ID))
                                //    {
                                //        continue;
                                //    }
                                //}
                                HisMediInStockByExpireDateADO parentLevel1 = new HisMediInStockByExpireDateADO(parent);
                                parentLevel1.CONCRETE_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE + "_" + parent.ID;
                                parentLevel1.PARENT_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE;
                                HisMediInStockByExpireDateADOs.Add(parentLevel1);
                                CreateListChildByParent(parentLevel1, listItem);

                            }
                        }
                        else
                        {
                            parentAdo.CONCRETE_ID__IN_DATE = "Không xác định";
                            parentAdo.MEDICINE_TYPE_CODE = "Không xác định";
                            HisMediInStockByExpireDateADOs.Add(parentAdo);
                            var listParent = listItem.Where(o => String.IsNullOrEmpty(o.ParentNodeId)).ToList();
                            foreach (var parent in listParent)
                            {
                                //if (this._MedicineTypeIds != null && this._MedicineTypeIds.Count > 0)
                                //{
                                //    if (!this._MedicineTypeIds.Contains(parent.MEDICINE_TYPE_ID))
                                //    {
                                //        continue;
                                //    }
                                //}
                                HisMediInStockByExpireDateADO parentLevel1 = new HisMediInStockByExpireDateADO(parent);
                                parentLevel1.CONCRETE_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE + "_" + parent.ID;
                                parentLevel1.PARENT_ID__IN_DATE = parentAdo.CONCRETE_ID__IN_DATE;
                                HisMediInStockByExpireDateADOs.Add(parentLevel1);
                                CreateListChildByParent(parentLevel1, listItem);
                            }
                        }
                    }


                    HisMediInStockByExpireDateADOs = HisMediInStockByExpireDateADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
                records = new BindingList<HisMediInStockByExpireDateADO>(HisMediInStockByExpireDateADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.HisMediInStockByExpireDate_CheckAllNode != null)
                    this.HisMediInStockByExpireDate_CheckAllNode(trvService.Nodes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateListChildByParent(HisMediInStockByExpireDateADO parentLevel1, List<MOS.SDO.HisMedicineInStockSDO> listItem)
        {
            try
            {
                var listchild = listItem.Where(o => o.ParentNodeId == parentLevel1.NodeId).ToList();
                if (listchild != null)
                {
                    for (int child = 0; child < listchild.Count; child++)
                    {
                        HisMediInStockByExpireDateADO childAdo = new HisMediInStockByExpireDateADO(listchild[child]);
                        childAdo.CONCRETE_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE + "_" + listchild[child].ID;
                        childAdo.PARENT_ID__IN_DATE = parentLevel1.CONCRETE_ID__IN_DATE;
                        //Inventec.Common.Mapper.DataObjectMapper.Map<HisMediInStockByExpireDateADO>(childAdo, child);
                        string[] str = childAdo.CONCRETE_ID__IN_DATE.Split('_');
                        if (str.Length == 3)
                        {
                            childAdo.IS_MEDI_MATE = true;
                        }
                        if (child > 0 && listchild[child].MEDICINE_TYPE_CODE == listchild[child - 1].MEDICINE_TYPE_CODE)
                        {
                            childAdo.IS_MEDI_MATE = false ;
                        }
                        childAdo.bIsLeaf = (listchild[child].IS_LEAF == 1);
                        HisMediInStockByExpireDateADOs.Add(childAdo);
                        CreateListChildByParent(childAdo, listItem);
                    }
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
                if (HisMediInStockByExpireDateADO.HisMediInStockByExpireDateColumns != null && HisMediInStockByExpireDateADO.HisMediInStockByExpireDateColumns.Count > 0)
                {
                    foreach (var svtr in HisMediInStockByExpireDateADO.HisMediInStockByExpireDateColumns)
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

                if (HisMediInStockByExpireDateADO.ColumnButtonEdits != null && HisMediInStockByExpireDateADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in HisMediInStockByExpireDateADO.ColumnButtonEdits)
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
                if (HisMediInStockByExpireDateADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (HisMediInStockByExpireDateADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (HisMediInStockByExpireDateADO.IsShowButtonAdd.HasValue)
                {
                    lciHisMediInStockByExpireDateAdd.Visibility = (HisMediInStockByExpireDateADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                BindingList<HisMediInStockByExpireDateADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<HisMediInStockByExpireDateADO> rearchResult = new List<HisMediInStockByExpireDateADO>();

                    rearchResult = HisMediInStockByExpireDateADOs.Where(o =>
                                                     ((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     || (o.MEDICINE_TYPE_HEIN_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.ACTIVE_INGR_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                     )
                                                     ).Distinct().ToList();

                    //if (listParentSearch != null)
                    //{
                    //    rearchResult.AddRange(listParentSearch);
                    //    foreach (var item in listParentSearch)
                    //    {
                    //        var childs = GetChild(records, item);
                    //        if (childs != null && childs.Count > 0)
                    //        {
                    //            rearchResult.AddRange(childs);
                    //        }
                    //    }
                    //}

                    //rearchResult = rearchResult.Distinct().ToList();

                    listResult = new BindingList<HisMediInStockByExpireDateADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<HisMediInStockByExpireDateADO>(HisMediInStockByExpireDateADOs);
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

        public void Reload(List<List<HisMedicineInStockSDO>> HisMediInStockByExpireDates, List<long> MedicineTypeIds)
        {
            try
            {
                txtKeyword.Text = "";
                this.HisMediInStockByExpireDateADO.HisMediInStockByExpireDates = HisMediInStockByExpireDates;
                if (this.HisMediInStockByExpireDateADO.HisMediInStockByExpireDates == null)
                    records = null;
                this._MedicineTypeIds = MedicineTypeIds;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
        public List<HisMediInStockByExpireDateADO> GetListTreeView()
        {
            List<HisMediInStockByExpireDateADO> result = new List<HisMediInStockByExpireDateADO>();
            try
            {
                if (records != null)
                {
                    result = HisMediInStockByExpireDateADOs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new List<HisMediInStockByExpireDateADO>();
            }
            return result;
        }

        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (HisMediInStockByExpireDateNodeCellStyle != null)
                {
                    HisMediInStockByExpireDateNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is HisMediInStockByExpireDateADO)
                    {
                        if (HisMediInStockByExpireDateRowEnter != null)
                        {
                            HisMediInStockByExpireDateRowEnter((HisMediInStockByExpireDateADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is HisMediInStockByExpireDateADO)
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
                if (this.HisMediInStockByExpireDate_BeforeCheck != null)
                    this.HisMediInStockByExpireDate_BeforeCheck(node);
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
                if (this.HisMediInStockByExpireDate_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is HisMediInStockByExpireDateADO)
                {
                    HisMediInStockByExpireDate_AfterCheck(e.Node, (HisMediInStockByExpireDateADO)row);
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
                        HisMediInStockByExpireDateHandler clickhandler = btn.Tag as HisMediInStockByExpireDateHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is HisMediInStockByExpireDateADO)
                            {
                                clickhandler((HisMediInStockByExpireDateADO)data);
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    if (this.HisMediInStockByExpireDate_GetSelectImage != null)
                        this.HisMediInStockByExpireDate_GetSelectImage((HisMediInStockByExpireDateADO)data, e);
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    if (this.HisMediInStockByExpireDate_GetStateImage != null)
                        this.HisMediInStockByExpireDate_GetStateImage((HisMediInStockByExpireDateADO)data, e);
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    if (this.HisMediInStockByExpireDate_SelectImageClick != null)
                        this.HisMediInStockByExpireDate_SelectImageClick((HisMediInStockByExpireDateADO)data);
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    if (this.HisMediInStockByExpireDate_StateImageClick != null)
                        this.HisMediInStockByExpireDate_StateImageClick((HisMediInStockByExpireDateADO)data);
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
                    HisMediInStockByExpireDateADO currentRow = e.Row as HisMediInStockByExpireDateADO;
                    if (HisMediInStockByExpireDate_CustomUnboundColumnData == null || currentRow == null) return;
                    this.HisMediInStockByExpireDate_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    var rowData = data as HisMediInStockByExpireDateADO;

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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    var rowData = data as HisMediInStockByExpireDateADO;
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
                            if (data != null && data is HisMediInStockByExpireDateADO)
                            {
                                foreach (var menu in this.menuItems((HisMediInStockByExpireDateADO)data))
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
                if (data != null && data is HisMediInStockByExpireDateADO)
                {
                    var rowData = data as HisMediInStockByExpireDateADO;
                    if (rowData != null && this.HisMediInStockByExpireDate_CustomDrawNodeCell != null)
                    {
                        this.HisMediInStockByExpireDate_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<HisMediInStockByExpireDateADO> GetListCheck()
        {
            List<HisMediInStockByExpireDateADO> result = new List<HisMediInStockByExpireDateADO>();
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
                result = new List<HisMediInStockByExpireDateADO>();
            }
            return result;
        }

        //List<HisMediInStockByExpireDateADO> GetChild(BindingList<HisMediInStockByExpireDateADO> listData, HisMediInStockByExpireDateADO parent)
        //{
        //    List<HisMediInStockByExpireDateADO> result = new List<HisMediInStockByExpireDateADO>();
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

        private void GetListNodeCheck(ref List<HisMediInStockByExpireDateADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((HisMediInStockByExpireDateADO)trvService.GetDataRecordByNode(node));
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
                    HisMediInStockByExpireDateADO HisMediInStockByExpireDateFocus = (HisMediInStockByExpireDateADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMediInStockByExpireDateFocus != null && HisMediInStockByExpireDateDoubleClick != null)
                    {
                        HisMediInStockByExpireDateDoubleClick(HisMediInStockByExpireDateFocus);
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
                    HisMediInStockByExpireDateADO HisMediInStockByExpireDateFocus = (HisMediInStockByExpireDateADO)trvService.GetDataRecordByNode(hi.Node);
                    if (HisMediInStockByExpireDateFocus != null && this.HisMediInStockByExpireDateClick != null)
                    {
                        this.HisMediInStockByExpireDateClick(HisMediInStockByExpireDateFocus);
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
