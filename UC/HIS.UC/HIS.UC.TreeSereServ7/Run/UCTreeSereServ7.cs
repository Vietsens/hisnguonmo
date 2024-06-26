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
using HIS.Desktop.LocalStorage.BackendData;
using System.Drawing.Drawing2D;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;

namespace HIS.UC.TreeSereServ7.Run
{
    public partial class UCTreeSereServ7 : UserControl
    {
        #region Declare
        TreeSereServ7ADO TreeSereServ7ADO;
        BindingList<SereServADO> records;
        List<SereServADO> SereServADOs = new List<SereServADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        TreeSereServ7_NodeCellStyle sereServNodeCellStyle;
        TreeSereServ7_CustomNodeCellEdit sereServTree_CustomNodeCellEdit;
        TreeSereServ7_CustomUnboundColumnData sereServTree_CustomUnboundColumnData;
        SereServHandler sereServTreeClick;
        TreeSereServ7_GetStateImage sereServTree_GetStateImage;
        TreeSereServ7_GetSelectImage sereServTree_GetSelectImage;
        SereServHandler sereServTree_StateImageClick;
        SereServHandler sereServTree_DoubleClick;
        SereServHandler sereServTree_SelectImageClick;
        TreeSereServ7_AfterCheck sereServTree_AfterCheck;
        TreeSereServ7_BeforeCheck sereServTree_BeforeCheck;
        TreeSereServ7_CheckAllNode sereServTree_CheckAllNode;
        TreeSereServ7_CustomDrawNodeCell sereServTree_CustomDrawNodeCell;
        bool IsShowCheckNode;
        bool isAutoWidth;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        SereServHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithSereServExpend = true;
        long _DepartmentInput = 0;
        #endregion

        #region Construct
        public UCTreeSereServ7(TreeSereServ7ADO sereServTreeADO)
        {
            InitializeComponent();
            try
            {
                this.TreeSereServ7ADO = sereServTreeADO;
                this.sereServNodeCellStyle = sereServTreeADO.SereServNodeCellStyle;
                this.sereServTreeClick = sereServTreeADO.TreeSereServ7Click;
                this.sereServTree_GetStateImage = sereServTreeADO.TreeSereServ7_GetStateImage;
                this.sereServTree_GetSelectImage = sereServTreeADO.TreeSereServ7_GetSelectImage;
                this.sereServTree_StateImageClick = sereServTreeADO.TreeSereServ7_StateImageClick;
                this.sereServTree_SelectImageClick = sereServTreeADO.TreeSereServ7_SelectImageClick;
                this.sereServTree_DoubleClick = sereServTreeADO.TreeSereServ7_DoubleClick;
                this.columnButtonEdits = sereServTreeADO.ColumnButtonEdits;
                this.selectImageCollection = sereServTreeADO.SelectImageCollection;
                this.stateImageCollection = sereServTreeADO.StateImageCollection;
                this.updateSingleRow = sereServTreeADO.UpdateSingleRow;
                this.sereServTree_CustomUnboundColumnData = sereServTreeADO.TreeSereServ7_CustomUnboundColumnData;
                this.sereServTree_CustomNodeCellEdit = sereServTreeADO.TreeSereServ7_CustomNodeCellEdit;
                this.menuItems = sereServTreeADO.MenuItems;
                this.sereServTree_AfterCheck = sereServTreeADO.TreeSereServ7_AfterCheck;
                this.sereServTree_BeforeCheck = sereServTreeADO.TreeSereServ7_BeforeCheck;
                this.sereServTree_CheckAllNode = sereServTreeADO.TreeSereServ7_CheckAllNode;
                this.sereServTree_CustomDrawNodeCell = sereServTreeADO.TreeSereServ7_CustomDrawNodeCell;
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
                this._DepartmentInput = sereServTreeADO.DepartmentID ?? 0;
                // this.Size = new Size(762,613);
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
                SetCaptionByLanguageKey();
                if (TreeSereServ7ADO != null)
                {
                    if (this.stateImageCollection != null)
                    {
                        trvService.StateImageList = this.stateImageCollection;
                    }
                    if (this.selectImageCollection != null)
                    {
                        trvService.SelectImageList = this.selectImageCollection;
                    }
                    InitializeTree();
                    //BindTreePlus();

                    cboFilterByDepartment.SelectedIndex = 0;

                    LoadTreeListByFilter(true, this.TreeSereServ7ADO.SereServs);

                    SetVisibleSearchPanel();
                    trvService.ToolTipController = toolTipController1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.TreeSereServ7.Resources.Lang", typeof(HIS.UC.TreeSereServ7.Run.UCTreeSereServ7).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.trvService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCTreeSereServ7.trvService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreeSereServ7.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnThuGon.Text = Inventec.Common.Resource.Get.Value("UCTreeSereServ7.btnThuGon.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChiTiet.Text = Inventec.Common.Resource.Get.Value("UCTreeSereServ7.btnChiTiet.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                //SereServADOs = new List<SereServADO>();
                //if (TreeSereServ7ADO.SereServs != null)
                //{
                //    var sereServs = (from r in TreeSereServ7ADO.SereServs select new SereServADO(r)).ToList();
                //    var listRootSety = sereServs.GroupBy(g => g.SERVICE_TYPE_ID).ToList();
                //    foreach (var rootSety in listRootSety)
                //    {
                //        var listBySety = rootSety.ToList<SereServADO>();
                //        SereServADO ssRootSety = new SereServADO();
                //        ssRootSety.CONCRETE_ID__IN_SETY = listBySety.First().SERVICE_TYPE_ID + "";
                //        ssRootSety.SERVICE_CODE = listBySety.First().SERVICE_TYPE_NAME;
                //        //ssRootSety.SERVICE_NAME = listBySety.First().SERVICE_TYPE_NAME;
                //        SereServADOs.Add(ssRootSety);
                //        foreach (var item in listBySety)
                //        {
                //            item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                //            item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                //            item.IsLeaf = true;
                //            //Ghi chú
                //            //
                //            if (item.SERVICE_TYPE_ID == HisServiceTypeCFG.SERVICE_TYPE_ID__MEDI)
                //            {
                //                item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_UNIT_NAME;//THuốc
                //            }
                //            else if (item.SERVICE_TYPE_ID == HisServiceTypeCFG.SERVICE_TYPE_ID__MATE)
                //            {
                //                item.NOTE_ADO = item.AMOUNT + "  -  " + item.SERVICE_TYPE_NAME;//Vật tư
                //            }
                //            else if (item.SERVICE_TYPE_ID == HisServiceTypeCFG.SERVICE_TYPE_ID__ENDO)
                //            {
                //                item.NOTE_ADO = "";//Nội soi
                //            }
                //            else if (item.SERVICE_TYPE_ID == HisServiceTypeCFG.SERVICE_TYPE_ID__EXAM)
                //            {
                //                item.NOTE_ADO = "";//Khám
                //            }
                //            else
                //            {
                //                item.NOTE_ADO = "";
                //            }
                //            SereServADOs.Add(item);
                //        }
                //    }
                //    SereServADOs = SereServADOs.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.SERVICE_NAME).ToList();
                //}
                //records = new BindingList<SereServADO>(SereServADOs);
                //trvService.DataSource = records;
                //trvService.ExpandAll();
                //if (this.sereServTree_CheckAllNode != null)
                //    this.sereServTree_CheckAllNode(trvService.Nodes);
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
                if (TreeSereServ7ADO.TreeSereServ7Columns != null && TreeSereServ7ADO.TreeSereServ7Columns.Count > 0)
                {
                    foreach (var svtr in TreeSereServ7ADO.TreeSereServ7Columns)
                    {
                        TreeListColumn col = this.trvService.Columns.AddField(svtr.FieldName);
                        col.Visible = svtr.Visible;
                        col.VisibleIndex = svtr.VisibleIndex;
                        col.Width = svtr.ColumnWidth;
                        col.FieldName = svtr.FieldName;
                        col.OptionsColumn.AllowEdit = svtr.AllowEdit;
                        col.Caption = svtr.Caption;
                        col.ToolTip = svtr.ToolTip;
                        //col.ImageIndex = 1;
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
                if (TreeSereServ7ADO.ColumnButtonEdits != null && TreeSereServ7ADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in TreeSereServ7ADO.ColumnButtonEdits)
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
                if (TreeSereServ7ADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (TreeSereServ7ADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(TreeSereServ7ADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = TreeSereServ7ADO.Keyword_NullValuePrompt;
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
        {
            try
            {

                if (sereServs != null)
                {
                    btnThuGon.Enabled = true;
                }
                else
                {
                    btnThuGon.Enabled = false;
                }
                if (btnChiTiet.Visible)
                {
                    btnThuGon.Visible = true;
                    btnChiTiet.Visible = false;
                }
                txtKeyword.Text = "";
                this.TreeSereServ7ADO.SereServs = sereServs;
                if (this.TreeSereServ7ADO.SereServs == null)
                    records = null;
                //BindTreePlus();
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<SereServADO> sereServs)
        {
            try
            {

                if (sereServs != null)
                {
                    btnThuGon.Enabled = true;
                }
                else
                {
                    btnThuGon.Enabled = false;
                }
                if (btnChiTiet.Visible)
                {
                    btnThuGon.Visible = true;
                    btnChiTiet.Visible = false;
                }
                txtKeyword.Text = "";
                this.TreeSereServ7ADO.SereServsAdo = sereServs;
                if (this.TreeSereServ7ADO.SereServsAdo == null)
                    records = null;
                //BindTreePlus();
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServsAdo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(List<MOS.SDO.DHisSereServ2> sereServsNew)
        {
            try
            {

                if (sereServsNew != null)
                {
                    btnThuGon.Enabled = true;
                }
                else
                {
                    btnThuGon.Enabled = false;
                }
                if (btnChiTiet.Visible)
                {
                    btnThuGon.Visible = true;
                    btnChiTiet.Visible = false;
                }
                txtKeyword.Text = "";
                this.TreeSereServ7ADO.SereServsNew = sereServsNew;
                if (this.TreeSereServ7ADO.SereServsNew == null)
                    records = null;
                //BindTreePlus();
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServsNew);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(long _department, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_7> sereServs)
        {
            try
            {
                this._DepartmentInput = _department;
                if (sereServs != null)
                {
                    btnThuGon.Enabled = true;
                }
                else
                {
                    btnThuGon.Enabled = false;
                }
                if (btnChiTiet.Visible)
                {
                    btnThuGon.Visible = true;
                    btnChiTiet.Visible = false;
                }
                txtKeyword.Text = "";
                this.TreeSereServ7ADO.SereServs = sereServs;
                if (this.TreeSereServ7ADO.SereServs == null)
                    records = null;
                //BindTreePlus();
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Reload(long _department, List<SereServADO> sereServs)
        {
            try
            {
                this._DepartmentInput = _department;
                if (sereServs != null)
                {
                    btnThuGon.Enabled = true;
                }
                else
                {
                    btnThuGon.Enabled = false;
                }
                if (btnChiTiet.Visible)
                {
                    btnThuGon.Visible = true;
                    btnChiTiet.Visible = false;
                }
                txtKeyword.Text = "";
                this.TreeSereServ7ADO.SereServsAdo = sereServs;
                if (this.TreeSereServ7ADO.SereServs == null)
                    records = null;
                //BindTreePlus();
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServsAdo);
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

        public V_HIS_SERE_SERV_7 GetValueFocus()
        {
            V_HIS_SERE_SERV_7 result = null;
            try
            {
                var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                if (data != null)
                {
                    result = (V_HIS_SERE_SERV_7)data;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
            return result;
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
        #endregion

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (data != null && data is SereServADO)
                {
                    try
                    {
                        SereServADO dataIsExcute = (SereServADO)data;
                        if (dataIsExcute != null && dataIsExcute.IS_NO_EXECUTE != null)
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    if (sereServNodeCellStyle != null)
                    {
                        sereServNodeCellStyle((SereServADO)data, e);
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
                if (e.IsGetData && e.Row != null)
                {
                    SereServADO currentRow = e.Row as SereServADO;
                    if (currentRow == null || this.sereServTree_CustomUnboundColumnData == null) return;
                    this.sereServTree_CustomUnboundColumnData(currentRow, e);
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
                    SereServADO rowData = data as SereServADO;
                    if (rowData == null) return;
                    this.sereServTree_CustomNodeCellEdit(rowData, e);
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
                        rowData.IS_EXPEND = 1;// IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_EXPEND__TRUE;
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

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
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
                                if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.SAMPLE_TIME != null)
                                    text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__DA_LAY_MAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                else
                                    text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__CHUA_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                if (data.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN && data.RECEIVE_SAMPLE_TIME != null)
                                    text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__DA_NHAN_MAU", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                                else
                                    text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__DANG_XU_LY", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__HOAN_THANH", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            //}
                        }
                        info = new DevExpress.Utils.ToolTipControlInfo(o, text);
                        e.Info = info;
                    }
                    else if (hi.HitInfoType == HitInfoType.StateImage)
                    {
                        var o = hi.Node;
                        var data = (SereServADO)trvService.GetDataRecordByNode(o);
                        if (data.TDL_SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN)
                            return;
                        string text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_SERE_SERV_7__KET_QUA_XET_NGHIEM", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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

        private void btnThuGon_Click(object sender, EventArgs e)
        {
            try
            {
                btnThuGon.Visible = false;
                btnChiTiet.Visible = true;
                Expand(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnChiTiet_Click(object sender, EventArgs e)
        {
            try
            {
                btnThuGon.Visible = true;
                btnChiTiet.Visible = false;
                Expand(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboFilterByDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                LoadTreeListByFilter(false, this.TreeSereServ7ADO.SereServs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void trvService_DoubleClick(object sender, EventArgs e)
        {
            try
            {

                TreeList tree = sender as TreeList;
                TreeListHitInfo hi = tree.CalcHitInfo(tree.PointToClient(Control.MousePosition));
                var data = trvService.GetDataRecordByNode(hi.Node);
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTree_DoubleClick != null)
                        this.sereServTree_DoubleClick((SereServADO)data);
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
                var data = trvService.GetDataRecordByNode(hi.Node);
                if (data != null && data is SereServADO)
                {
                    if (this.sereServTreeClick != null)
                        this.sereServTreeClick((SereServADO)data);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
