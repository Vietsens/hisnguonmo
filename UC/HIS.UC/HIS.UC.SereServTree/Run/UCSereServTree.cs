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

namespace HIS.UC.SereServTree.Run
{
    public partial class UCSereServTree : UserControl
    {
        #region Declare
        SereServTreeADO SereServTreeADO;
        BindingList<SereServADO> records;
        List<SereServADO> SereServADOs = new List<SereServADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        SereServTree_NodeCellStyle sereServNodeCellStyle;
        SereServTree_CustomUnboundColumnData sereServTree_CustomUnboundColumnData;
        SereServHandler sereServTreeClick;
        SereServTree_GetStateImage sereServTree_GetStateImage;
        SereServTree_GetSelectImage sereServTree_GetSelectImage;
        SereServHandler sereServTree_StateImageClick;
        SereServHandler sereServTree_SelectImageClick;
        SereServTree_AfterCheck sereServTree_AfterCheck;
        SereServTree_BeforeCheck sereServTree_BeforeCheck;
        SereServTreeForBill_BeforeCheck sereServTreeForBill_BeforeCheck;
        SereServTree_ShowingEditor sereServTree_ShowingEditor;
        SereServTree_CheckAllNode sereServTree_CheckAllNode;
        SereServTree_CustomDrawNodeCell sereServTree_CustomDrawNodeCell;
        SereServTree_CustomDrawNodeCheckBox sereServTree_CustomDrawNodeCheckBox;
        bool IsShowCheckNode;
        bool isAutoWidth;
        bool isAdvance;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        SereServHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithSereServExpend = true;
        bool IsShowForRegisterV2 = false;
        #endregion

        #region Construct
        public UCSereServTree(SereServTreeADO sereServTreeADO)
        {
            InitializeComponent();
            try
            {
                this.SereServTreeADO = sereServTreeADO;
                this.sereServNodeCellStyle = sereServTreeADO.SereServNodeCellStyle;
                this.sereServTreeClick = sereServTreeADO.SereServTreeClick;
                this.sereServTree_GetStateImage = sereServTreeADO.SereServTree_GetStateImage;
                this.sereServTree_GetSelectImage = sereServTreeADO.SereServTree_GetSelectImage;
                this.sereServTree_StateImageClick = sereServTreeADO.SereServTree_StateImageClick;
                this.sereServTree_SelectImageClick = sereServTreeADO.SereServTree_SelectImageClick;
                this.columnButtonEdits = sereServTreeADO.ColumnButtonEdits;
                this.selectImageCollection = sereServTreeADO.SelectImageCollection;
                this.stateImageCollection = sereServTreeADO.StateImageCollection;
                this.updateSingleRow = sereServTreeADO.UpdateSingleRow;
                this.sereServTree_CustomUnboundColumnData = sereServTreeADO.SereServTree_CustomUnboundColumnData;
                this.menuItems = sereServTreeADO.MenuItems;
                this.sereServTree_AfterCheck = sereServTreeADO.SereServTree_AfterCheck;
                this.sereServTree_BeforeCheck = sereServTreeADO.SereServTree_BeforeCheck;
                this.sereServTreeForBill_BeforeCheck = sereServTreeADO.SereServTreeForBill_BeforeCheck;
                this.sereServTree_ShowingEditor = sereServTreeADO.sereServTree_ShowingEditor;
                this.sereServTree_CheckAllNode = sereServTreeADO.SereServTree_CheckAllNode;
                this.sereServTree_CustomDrawNodeCell = sereServTreeADO.SereServTree_CustomDrawNodeCell;
                this.sereServTree_CustomDrawNodeCheckBox = sereServTreeADO.SereServTree_CustomDrawNodeCheckBox;
                if (sereServTreeADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = sereServTreeADO.IsShowCheckNode.Value;
                }
                if (sereServTreeADO.IsCreateParentNodeWithSereServExpend.HasValue)
                {
                    this.isCreateParentNodeWithSereServExpend = sereServTreeADO.IsCreateParentNodeWithSereServExpend.Value;
                }
                if (sereServTreeADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = sereServTreeADO.IsAutoWidth.Value;
                }
                this.IsShowForRegisterV2 = sereServTreeADO.IsShowForRegisterV2;
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
                if (SereServTreeADO != null)
                {
                    if (SereServTreeADO.isAdvance == true)
                        isAdvance = true;

                    BindTreePlus();
                    if (this.stateImageCollection != null)
                    {
                        trvService.StateImageList = this.stateImageCollection;
                    }
                    if (this.selectImageCollection != null)
                    {
                        trvService.SelectImageList = this.selectImageCollection;
                    }
                    InitializeTree();
                    SetVisibleSearchPanel();
                    trvService.ToolTipController = toolTipController;
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
                SereServADOs = new List<SereServADO>();
                if (SereServTreeADO.SereServs != null)
                {
                    var sereServs = (from r in SereServTreeADO.SereServs select new SereServADO(r)).ToList();
                    List<SereServADO> listSereServExpend = new List<SereServADO>();
                    if (this.isCreateParentNodeWithSereServExpend)
                    {
                        listSereServExpend = sereServs.Where(o => o.IsExpend.HasValue && o.IsExpend.Value).ToList(); // && o.VIR_TOTAL_PATIENT_PRICE > 0
                        sereServs = sereServs.Where(o => o.IsExpend != true).ToList();
                    }
                    if (IsShowForRegisterV2)
                    {
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = "IsShowForRegisterV2";
                        ssRootPaty.TDL_SERVICE_NAME = "Tổng chi phí";
                        SereServADOs.Add(ssRootPaty);
                    }
                    if (listSereServExpend != null && listSereServExpend.Count > 0)
                    {
                        SereServADO ssRootExpend = new SereServADO();
                        ssRootExpend.TDL_SERVICE_NAME = this.SereServTreeADO.LayoutSereServExpend;
                        ssRootExpend.CONCRETE_ID__IN_SETY = "Expend";
                        if (IsShowForRegisterV2)
                        {
                            ssRootExpend.PARENT_ID__IN_SETY = "IsShowForRegisterV2";
                        }
                        SereServADOs.Add(ssRootExpend);
                        var listGroupBySety = listSereServExpend.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var group in listGroupBySety)
                        {
                            var listSub = group.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_SERVICE_NAME = listSub.First().TDL_SERVICE_NAME;
                            ssRootSety.AMOUNT_PLUS = listSub.Sum(o => o.AMOUNT);
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY + "_" + listSub.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY;
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listSub)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }


                    var listRoot = sereServs.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var rootPaty in listRoot)
                    {
                        var listByPaty = rootPaty.ToList<SereServADO>();
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = listByPaty.First().PATIENT_TYPE_ID + "";
                        if (IsShowForRegisterV2)
                        {
                            ssRootPaty.PARENT_ID__IN_SETY = "IsShowForRegisterV2";
                        }
                        //ssRootPaty.SERVICE_CODE = listByPaty.First().PATIENT_TYPE_CODE;
                        ssRootPaty.TDL_SERVICE_NAME = listByPaty.First().PATIENT_TYPE_NAME;
                        ssRootPaty.PATIENT_TYPE_ID = listByPaty.First().PATIENT_TYPE_ID;
                        SereServADOs.Add(ssRootPaty);
                        var listRootSety = listByPaty.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                            ssRootSety.PATIENT_TYPE_ID = ssRootPaty.PATIENT_TYPE_ID;
                            ssRootSety.TDL_SERVICE_NAME = this.isAdvance == true ? listBySety.First().SERVICE_TYPE_NAME + " (" + listBySety.Count() + "/" + +listBySety.Count() + ")"
                                : listBySety.First().SERVICE_TYPE_NAME;
                            ssRootSety.IsFather = true;
                            ssRootSety.AMOUNT = listBySety.Sum(o => o.AMOUNT);
                            ssRootSety.VIR_TOTAL_PRICE = listBySety.Sum(o => o.VIR_TOTAL_PRICE);
                            ssRootSety.VIR_TOTAL_HEIN_PRICE = listBySety.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            ssRootSety.VIR_TOTAL_PATIENT_PRICE = listBySety.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listBySety)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }
                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.TDL_SERVICE_CODE).ToList();
                }
                else if (SereServTreeADO.SereServDeposits != null)
                {
                    var sereServDeposits = (from r in SereServTreeADO.SereServDeposits select new SereServADO(r)).ToList();
                    List<SereServADO> listSereServExpend = new List<SereServADO>();
                    if (this.isCreateParentNodeWithSereServExpend)
                    {
                        listSereServExpend = sereServDeposits.Where(o => o.IsExpend.HasValue && o.IsExpend.Value).ToList(); // && o.VIR_TOTAL_PATIENT_PRICE > 0
                        sereServDeposits = sereServDeposits.Where(o => o.IsExpend != true).ToList();
                    }
                    if (IsShowForRegisterV2)
                    {
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = "IsShowForRegisterV2";
                        ssRootPaty.TDL_SERVICE_NAME = "Tổng chi phí";
                        SereServADOs.Add(ssRootPaty);
                    }
                    if (listSereServExpend != null && listSereServExpend.Count > 0)
                    {
                        SereServADO ssRootExpend = new SereServADO();
                        ssRootExpend.TDL_SERVICE_NAME = this.SereServTreeADO.LayoutSereServExpend;
                        ssRootExpend.CONCRETE_ID__IN_SETY = "Expend";
                        if (IsShowForRegisterV2)
                        {
                            ssRootExpend.PARENT_ID__IN_SETY = "IsShowForRegisterV2";
                        }
                        SereServADOs.Add(ssRootExpend);
                        var listGroupBySety = listSereServExpend.GroupBy(o => o.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var group in listGroupBySety)
                        {
                            var listSub = group.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.TDL_SERVICE_NAME = listSub.First().TDL_SERVICE_NAME;
                            ssRootSety.AMOUNT_PLUS = listSub.Sum(o => o.AMOUNT);
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY + "_" + listSub.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY;
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listSub)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }


                    var listRoot = sereServDeposits.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var rootPaty in listRoot)
                    {
                        var listByPaty = rootPaty.ToList<SereServADO>();
                        SereServADO ssRootPaty = new SereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = listByPaty.First().PATIENT_TYPE_ID + "";
                        if (IsShowForRegisterV2)
                        {
                            ssRootPaty.PARENT_ID__IN_SETY = "IsShowForRegisterV2";
                        }
                        //ssRootPaty.SERVICE_CODE = listByPaty.First().PATIENT_TYPE_CODE;
                        ssRootPaty.TDL_SERVICE_NAME = listByPaty.First().PATIENT_TYPE_NAME;
                        ssRootPaty.PATIENT_TYPE_ID = listByPaty.First().PATIENT_TYPE_ID;
                        SereServADOs.Add(ssRootPaty);
                        var listRootSety = listByPaty.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<SereServADO>();
                            SereServADO ssRootSety = new SereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                            ssRootSety.PATIENT_TYPE_ID = ssRootPaty.PATIENT_TYPE_ID;
                            ssRootSety.TDL_SERVICE_NAME = this.isAdvance == true ? listBySety.First().SERVICE_TYPE_NAME + " (" + listBySety.Count() + "/" + +listBySety.Count() + ")"
                                : listBySety.First().SERVICE_TYPE_NAME;
                            ssRootSety.IsFather = true;
                            ssRootSety.AMOUNT = listBySety.Sum(o => o.AMOUNT);
                            ssRootSety.VIR_TOTAL_PRICE = listBySety.Sum(o => o.VIR_TOTAL_PRICE);
                            ssRootSety.VIR_TOTAL_HEIN_PRICE = listBySety.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            ssRootSety.VIR_TOTAL_PATIENT_PRICE = listBySety.Sum(o => o.VIR_TOTAL_PATIENT_PRICE);
                            ssRootSety.TDL_SERVICE_REQ_CODE = ssRootSety.TDL_SERVICE_REQ_CODE;
                            ssRootSety.TDL_SERVICE_CODE = ssRootSety.TDL_SERVICE_CODE;
                            SereServADOs.Add(ssRootSety);
                            foreach (var item in listBySety)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                SereServADOs.Add(item);
                            }
                        }
                    }
                    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.TDL_SERVICE_CODE).ToList();
                }
                records = new BindingList<SereServADO>(SereServADOs);
                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
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
                if (SereServTreeADO.SereServTreeColumns != null && SereServTreeADO.SereServTreeColumns.Count > 0)
                {
                    foreach (var svtr in SereServTreeADO.SereServTreeColumns)
                    {
                        trvService.Columns.AddField(svtr.FieldName);
                        trvService.Columns[svtr.FieldName].FieldName = svtr.FieldName;
                        trvService.Columns[svtr.FieldName].Visible = svtr.Visible;
                        trvService.Columns[svtr.FieldName].VisibleIndex = svtr.VisibleIndex;
                        trvService.Columns[svtr.FieldName].Width = svtr.ColumnWidth;

                        trvService.Columns[svtr.FieldName].OptionsColumn.AllowEdit = svtr.AllowEdit;
                        trvService.Columns[svtr.FieldName].Caption = svtr.Caption;

                        trvService.Columns[svtr.FieldName].UnboundType = svtr.UnboundType;
                        if (svtr.Format != null)
                        {
                            trvService.Columns[svtr.FieldName].Format.FormatString = svtr.Format.FormatString;
                            trvService.Columns[svtr.FieldName].Format.FormatType = svtr.Format.FormatType;
                        }
                    }
                }
                if (SereServTreeADO.ColumnButtonEdits != null && SereServTreeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in SereServTreeADO.ColumnButtonEdits)
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
                if (SereServTreeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (SereServTreeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(SereServTreeADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = SereServTreeADO.Keyword_NullValuePrompt;
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
                BindingList<SereServADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<SereServADO> rearchResult = new List<SereServADO>();

                    var listParentSearch = SereServADOs.Where(o =>
                                                    ((o.TDL_SERVICE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.TDL_SERVICE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower()))
                                                    ).ToList();

                    if (listParentSearch != null)
                    {
                        rearchResult.AddRange(listParentSearch);
                        foreach (var item in listParentSearch)
                        {
                            var childs = GetChild(records, item);
                            if (childs != null && childs.Count > 0)
                            {
                                rearchResult.AddRange(childs);
                            }
                        }
                    }

                    rearchResult = rearchResult.Distinct().ToList();
                    listResult = new BindingList<SereServADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<SereServADO>(records);
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

        List<SereServADO> GetChild(BindingList<SereServADO> listData, SereServADO parent)
        {
            List<SereServADO> result = new List<SereServADO>();
            var childData = listData.Where(o => o.PARENT_ID__IN_SETY == parent.CONCRETE_ID__IN_SETY).ToList();
            if (childData != null && childData.Count > 0)
            {
                result.AddRange(childData);
                foreach (var item in childData)
                {
                    var childChild = GetChild(listData, item);
                    if (childChild != null && childChild.Count > 0)
                    {
                        result.AddRange(childChild);
                    }
                }
            }
            return result;
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs)
        {
            try
            {
                txtKeyword.Text = "";
                this.SereServTreeADO.SereServs = sereServs;
                if (this.SereServTreeADO.SereServs == null)
                    records = null;
                BindTreePlus();
                //this.SereServADOs = (from m in SereServTreeADO.SereServs select new SereServADO(m)).ToList();
                //records = new BindingList<SereServADO>(SereServADOs);
                //trvService.DataSource = records;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT> sereServDeposits)
        {
            try
            {
                txtKeyword.Text = "";
                this.SereServTreeADO.SereServDeposits = sereServDeposits;
                if (this.SereServTreeADO.SereServDeposits == null)
                    records = null;
                BindTreePlus();
                //this.SereServADOs = (from m in SereServTreeADO.SereServs select new SereServADO(m)).ToList();
                //records = new BindingList<SereServADO>(SereServADOs);
                //trvService.DataSource = records;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusKeyword()
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

        public void CheckAllNode()
        {
            try
            {
                if (this.sereServTree_CheckAllNode != null)
                    this.sereServTree_CheckAllNode(trvService.Nodes);
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
                if (sereServNodeCellStyle != null)
                {
                    sereServNodeCellStyle(data, e.Appearance);
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
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                }
                else if (e.KeyCode == Keys.Space)
                {
                    var node = trvService.FocusedNode;
                    var data = trvService.GetDataRecordByNode(node);
                    if (node != null && node.HasChildren && data != null && data is SereServADO)
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
                if (this.sereServTree_BeforeCheck != null)
                    this.sereServTree_BeforeCheck(node);

                if (this.sereServTreeForBill_BeforeCheck != null)
                    this.sereServTreeForBill_BeforeCheck(node, e);
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
                if (this.sereServTree_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is SereServADO)
                {
                    //e.Node.Checked = !e.Node.Checked;
                    //sereServTreeClick((SereServADO)row);
                    sereServTree_AfterCheck(e.Node, (SereServADO)row);

                    // minhnq
                    var lstdata = records.Where(o => o.IsFather == true).ToList();
                    if (lstdata != null && lstdata.Count() > 0)
                    {
                        var lstSelected = GetListCheck();
                        if (lstSelected != null && lstSelected.Count() > 0)
                        {
                            foreach (var item in lstdata)
                            {
                                var numServiceSelected = lstSelected.Where(o => o.PARENT_ID__IN_SETY == item.CONCRETE_ID__IN_SETY && o.IsLeaf == true).Count();
                                var sumService = records.Where(o => o.PARENT_ID__IN_SETY == item.CONCRETE_ID__IN_SETY).Count();
                                int index = item.TDL_SERVICE_NAME.IndexOf("(");
                                var oldName = index > 0 ? item.TDL_SERVICE_NAME.Substring(0, index - 1) : item.TDL_SERVICE_NAME;
                                item.TDL_SERVICE_NAME = oldName + " (" + numServiceSelected + "/" + sumService + ")";
                            }
                        }
                        else
                        {
                            foreach (var item in lstdata)
                            {
                                var sumService = records.Where(o => o.PARENT_ID__IN_SETY == item.CONCRETE_ID__IN_SETY).Count();
                                int index = item.TDL_SERVICE_NAME.IndexOf("(");
                                var oldName = index > 0 ? item.TDL_SERVICE_NAME.Substring(0, index - 1) : item.TDL_SERVICE_NAME;
                                item.TDL_SERVICE_NAME = oldName + " (" + "0" + "/" + sumService + ")";
                            }
                        }
                    }
                    trvService.DataSource = records;
                    trvService.ExpandAll();
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
                        SereServHandler clickhandler = btn.Tag as SereServHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is SereServADO)
                            {
                                clickhandler((SereServADO)data);
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
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTree_GetSelectImage != null)
                        this.sereServTree_GetSelectImage((SereServADO)data, e);
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
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTree_GetStateImage != null)
                        this.sereServTree_GetStateImage((SereServADO)data, e);
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
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTree_SelectImageClick != null)
                        this.sereServTree_SelectImageClick((SereServADO)data);
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
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTree_StateImageClick != null)
                        this.sereServTree_StateImageClick((SereServADO)data);
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
                    var data = trvService.GetDataRecordByNode(e.Node);
                    if (data != null && data is SereServADO)
                    {
                        var rowData = data as SereServADO;
                        if (rowData != null && this.sereServTree_CustomUnboundColumnData != null)
                        {
                            this.sereServTree_CustomUnboundColumnData(rowData, e);
                        }
                    }

                    //SereServADO currentRow = e.Row as SereServADO;
                    //if (currentRow == null) return;
                    //this.sereServTree_CustomUnboundColumnData(currentRow, e);

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
                if (data != null && data is SereServADO)
                {
                    var rowData = data as SereServADO;

                    if (rowData.IsLeaf.HasValue && rowData.IsLeaf.Value)
                    {
                        if (e.Column.FieldName == "IsExpend")
                        {
                            if (this.updateSingleRow != null)
                            {
                                e.RepositoryItem = repositoryItemchkIsExpend__Enable;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemchkIsExpend__Disable;
                            }

                            if (rowData != null && rowData.IS_EXPEND == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                rowData.IsExpend = true;
                            }
                            else
                            {
                                rowData.IsExpend = false;
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
                if (data != null && data is SereServADO)
                {
                    var rowData = data as SereServADO;
                    if (rowData.IsExpend != null && rowData.IsExpend.Value)
                        rowData.IS_EXPEND = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    else
                        rowData.IS_EXPEND = null;
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
                            if (data != null && data is SereServADO)
                            {
                                foreach (var menu in this.menuItems((SereServADO)data))
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
                if (data != null && data is SereServADO)
                {
                    var rowData = data as SereServADO;
                    if (rowData != null && this.sereServTree_CustomDrawNodeCell != null)
                    {
                        this.sereServTree_CustomDrawNodeCell(rowData, e);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
        {
            try
            {
                if (e.Node != null)
                {
                    var data = (SereServADO)trvService.GetDataRecordByNode(e.Node);
                    if (data != null && this.sereServTree_CustomDrawNodeCheckBox != null)
                    {
                        this.sereServTree_CustomDrawNodeCheckBox(data, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public List<SereServADO> GetListCheck()
        {
            List<SereServADO> result = new List<SereServADO>();
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
                result = new List<SereServADO>();
            }
            return result;
        }

        private void GetListNodeCheck(ref List<SereServADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((SereServADO)trvService.GetDataRecordByNode(node));
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

        private void trvService_ShowingEditor(object sender, CancelEventArgs e)
        {
        }

        private void trvService_ShownEditor(object sender, EventArgs e)
        {
            try
            {
                var focusNode = trvService.FocusedNode;
                if (this.sereServTree_ShowingEditor != null && focusNode != null)
                {
                    this.sereServTree_ShowingEditor(focusNode, sender);
                    //((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl is DevExpress.XtraTreeList.TreeList)// trvService)
                {
                    DevExpress.Utils.ToolTipControlInfo info = null;
                    TreeListHitInfo hi = trvService.CalcHitInfo(e.ControlMousePosition);
                    if (hi.HitInfoType == HitInfoType.SelectImage)
                    {
                        var o = hi.Node;
                        string text = "";
                        var data = (SereServADO)trvService.GetDataRecordByNode(o);
                        if (data != null)
                        {
                            if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                text = "Chưa xử lý";
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                text = "Đang xử lý";
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                text = "Hoàn thành";
                            }
                            //}
                        }
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
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
