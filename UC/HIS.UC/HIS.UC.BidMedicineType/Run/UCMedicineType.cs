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
using HIS.UC.MedicineType.ADO;
using Inventec.Desktop.Common.Message;

namespace HIS.UC.MedicineType.Run
{
    public partial class UCMedicineType : UserControl
    {
        #region Declare
        MedicineTypeInitADO MedicineTypeADO;
        BindingList<MedicineTypeADO> records;
        List<MedicineTypeADO> MedicineTypeADOs = new List<MedicineTypeADO>();
        List<ColumnButtonEditADO> columnButtonEdits;
        MedicineType_NodeCellStyle MedicineTypeNodeCellStyle;
        MedicineType_CustomUnboundColumnData MedicineType_CustomUnboundColumnData;
        MedicineTypeHandler MedicineTypeClick;
        MedicineTypeHandler MedicineTypeDoubleClick;
        MedicineTypeHandler MedicineTypeRowEnter;
        MedicineType_GetStateImage MedicineType_GetStateImage;
        MedicineType_GetSelectImage MedicineType_GetSelectImage;
        MedicineTypeHandler MedicineType_StateImageClick;
        MedicineTypeHandler MedicineType_SelectImageClick;
        MedicineType_AfterCheck MedicineType_AfterCheck;
        MedicineType_BeforeCheck MedicineType_BeforeCheck;
        MedicineType_CheckAllNode MedicineType_CheckAllNode;
        MedicineType_CustomDrawNodeCell MedicineType_CustomDrawNodeCell;
        CheckThieuThongTinBHYT_CheckChange checkThieuThongTinBHYT_CheckChange;
        MedicineType_ExportExcel MedicineType_ExportExcel;
        MedicineType_Import MedicineType_Import;
        MedicineType_Save MedicineType_Save;
        bool IsShowCheckNode;
        bool isAutoWidth;
        bool IsShowRadioThieuTTBHYT;
        bool isShowImport;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MedicineTypeHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMedicineTypeExpend = true;
        string savePath = null;
        #endregion

        #region Construct
        public UCMedicineType(MedicineTypeInitADO MedicineTypeADO)
        {
            InitializeComponent();
            try
            {
                //Inventec.Common.Logging.LogSystem.Info("---contructor UCMedicineType----Bat dau khoi tao uc HIS.UC.MedicineType");
                this.MedicineTypeADO = MedicineTypeADO;
                this.MedicineTypeNodeCellStyle = MedicineTypeADO.MedicineTypeNodeCellStyle;
                this.MedicineTypeClick = MedicineTypeADO.MedicineTypeClick;
                this.MedicineTypeDoubleClick = MedicineTypeADO.MedicineTypeDoubleClick;
                this.MedicineTypeRowEnter = MedicineTypeADO.MedicineTypeRowEnter;
                this.MedicineType_GetStateImage = MedicineTypeADO.MedicineType_GetStateImage;
                this.MedicineType_GetSelectImage = MedicineTypeADO.MedicineType_GetSelectImage;
                this.MedicineType_StateImageClick = MedicineTypeADO.MedicineType_StateImageClick;
                this.MedicineType_SelectImageClick = MedicineTypeADO.MedicineType_SelectImageClick;
                this.columnButtonEdits = MedicineTypeADO.ColumnButtonEdits;
                this.selectImageCollection = MedicineTypeADO.SelectImageCollection;
                this.stateImageCollection = MedicineTypeADO.StateImageCollection;
                this.updateSingleRow = MedicineTypeADO.UpdateSingleRow;
                this.MedicineType_CustomUnboundColumnData = MedicineTypeADO.MedicineType_CustomUnboundColumnData;
                this.menuItems = MedicineTypeADO.MenuItems;
                this.MedicineType_AfterCheck = MedicineTypeADO.MedicineType_AfterCheck;
                this.MedicineType_BeforeCheck = MedicineTypeADO.MedicineType_BeforeCheck;
                this.MedicineType_CheckAllNode = MedicineTypeADO.MedicineType_CheckAllNode;
                this.MedicineType_CustomDrawNodeCell = MedicineTypeADO.MedicineType_CustomDrawNodeCell;
                this.checkThieuThongTinBHYT_CheckChange = MedicineTypeADO.checkThieuThongTinBHYT_CheckChange;
                this.MedicineType_ExportExcel = MedicineTypeADO.MedicineType_ExportExcel;
                this.MedicineType_Import = MedicineTypeADO.MedicineType_Import;
                this.MedicineType_Save = MedicineTypeADO.MedicineType_Save;
                if (MedicineTypeADO.IsShowCheckNode.HasValue)
                {
                    this.IsShowCheckNode = MedicineTypeADO.IsShowCheckNode.Value;
                }
                if (MedicineTypeADO.IsCreateParentNodeWithMedicineTypeExpend.HasValue)
                {
                    this.isCreateParentNodeWithMedicineTypeExpend = MedicineTypeADO.IsCreateParentNodeWithMedicineTypeExpend.Value;
                }

                if (MedicineTypeADO.IsAutoWidth.HasValue)
                {
                    this.isAutoWidth = MedicineTypeADO.IsAutoWidth.Value;
                }
                if (MedicineTypeADO.IsShowImport.HasValue)
                {
                    this.isShowImport = MedicineTypeADO.IsAutoWidth.Value;
                }
                //Inventec.Common.Logging.LogSystem.Info("----contructor UCMedicineType----ket thuc khoi tao uc HIS.UC.MedicineType");
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
                //Inventec.Common.Logging.LogSystem.Info("----UCServiceTree_Load----bat dau load khoi tao uc HIS.UC.MedicineType");
                if (MedicineTypeADO != null)
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
                //Inventec.Common.Logging.LogSystem.Info("----UCServiceTree_Load----ket thuc load khoi tao uc HIS.UC.MedicineType");
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
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau BindTreePlus uc HIS.UC.MedicineType");
                MedicineTypeADOs = new List<MedicineTypeADO>();
                if (MedicineTypeADO.MedicineTypes != null)
                {
                    MedicineTypeADOs = (from r in MedicineTypeADO.MedicineTypes select new MedicineTypeADO(r)).ToList();
                    //    List<MedicineTypeADO> listMedicineTypeExpend = new List<MedicineTypeADO>();
                    //    if (this.isCreateParentNodeWithMedicineTypeExpend)
                    //    {
                    //        listMedicineTypeExpend = MedicineTypes.Where(o => o.IsExpend.HasValue && o.IsExpend.Value).ToList();
                    //        MedicineTypes = MedicineTypes.Where(o => o.IsExpend != true).ToList();
                    //    }
                    //    if (listMedicineTypeExpend != null && listMedicineTypeExpend.Count > 0)
                    //    {
                    //        MedicineTypeADO ssRootExpend = new MedicineTypeADO();
                    //        ssRootExpend.MEDICINE_TYPE_NAME = this.MedicineTypeADO.LayoutMedicineTypeExpend;
                    //        ssRootExpend.CONCRETE_ID__IN_SETY = "Expend";
                    //        MedicineTypeADOs.Add(ssRootExpend);
                    //        var listGroupBySety = listMedicineTypeExpend.GroupBy(o => o.SERVICE_TYPE_ID).ToList();
                    //        foreach (var group in listGroupBySety)
                    //        {
                    //            var listSub = group.ToList<MedicineTypeADO>();
                    //            MedicineTypeADO ssRootSety = new MedicineTypeADO();
                    //            ssRootSety.MEDICINE_TYPE_NAME = listSub.First().MEDICINE_TYPE_NAME;
                    //            ssRootSety.CONCRETE_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY + "_" + listSub.First().SERVICE_TYPE_ID;
                    //            ssRootSety.PARENT_ID__IN_SETY = ssRootExpend.CONCRETE_ID__IN_SETY;
                    //            MedicineTypeADOs.Add(ssRootSety);
                    //            foreach (var item in listSub)
                    //            {
                    //                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                    //                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                    //                item.IsLeaf = true;
                    //                MedicineTypeADOs.Add(item);
                    //            }
                    //        }
                    //    }
                    //    var listRoot = MedicineTypes.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    //    foreach (var rootPaty in listRoot)
                    //    {
                    //        var listByPaty = rootPaty.ToList<MedicineTypeADO>();
                    //        MedicineTypeADO ssRootPaty = new MedicineTypeADO();
                    //        ssRootPaty.CONCRETE_ID__IN_SETY = listByPaty.First().PATIENT_TYPE_ID + "";
                    //        //ssRootPaty.SERVICE_CODE = listByPaty.First().PATIENT_TYPE_CODE;
                    //        ssRootPaty.SERVICE_NAME = listByPaty.First().PATIENT_TYPE_NAME;
                    //        MedicineTypeADOs.Add(ssRootPaty);
                    //        var listRootSety = listByPaty.GroupBy(g => g.SERVICE_TYPE_ID).ToList();
                    //        foreach (var rootSety in listRootSety)
                    //        {
                    //            var listBySety = rootSety.ToList<MedicineTypeADO>();
                    //            MedicineTypeADO ssRootSety = new MedicineTypeADO();
                    //            ssRootSety.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + listBySety.First().SERVICE_TYPE_ID;
                    //            ssRootSety.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                    //            //ssRootSety.SERVICE_CODE = listBySety.First().SERVICE_TYPE_CODE;
                    //            ssRootSety.SERVICE_NAME = listBySety.First().SERVICE_TYPE_NAME;
                    //            MedicineTypeADOs.Add(ssRootSety);
                    //            foreach (var item in listBySety)
                    //            {
                    //                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                    //                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                    //                item.IsLeaf = true;
                    //                MedicineTypeADOs.Add(item);
                    //            }
                    //        }
                    //    }
                    //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau OrderBy MedicineTypeADOs uc HIS.UC.MedicineType");
                    MedicineTypeADOs = MedicineTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                    //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc OrderBy MedicineTypeADOs uc HIS.UC.MedicineType");
                }
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau new BindingList uc HIS.UC.MedicineType");
                records = new BindingList<MedicineTypeADO>(MedicineTypeADOs);
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc new BindingList uc HIS.UC.MedicineType");
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau gan dataSource trvService uc HIS.UC.MedicineType");
                trvService.DataSource = records;
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc gan dataSource trvService uc HIS.UC.MedicineType");
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau ExpandAll trvService uc HIS.UC.MedicineType");
                trvService.ExpandAll();
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc ExpandAll trvService uc HIS.UC.MedicineType");
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----bat dau MedicineType_CheckAllNode uc HIS.UC.MedicineType");
                if (this.MedicineType_CheckAllNode != null)
                    this.MedicineType_CheckAllNode(trvService.Nodes);
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc MedicineType_CheckAllNode uc HIS.UC.MedicineType");
                //Inventec.Common.Logging.LogSystem.Info("----BindTreePlus----ket thuc BindTreePlus uc HIS.UC.MedicineType");
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
                //Inventec.Common.Logging.LogSystem.Info("----InitializeTree----bat dau InitializeTree uc HIS.UC.MedicineType");
                this.trvService.OptionsView.ShowCheckBoxes = this.IsShowCheckNode;
                this.trvService.OptionsView.AutoWidth = this.isAutoWidth;
                if (MedicineTypeADO.MedicineTypeColumns != null && MedicineTypeADO.MedicineTypeColumns.Count > 0)
                {
                    foreach (var svtr in MedicineTypeADO.MedicineTypeColumns)
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
                if (MedicineTypeADO.ColumnButtonEdits != null && MedicineTypeADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MedicineTypeADO.ColumnButtonEdits)
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
                //Inventec.Common.Logging.LogSystem.Info("----InitializeTree----ket thuc InitializeTree uc HIS.UC.MedicineType");
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
                if (MedicineTypeADO.IsShowImport.HasValue)
                {
                    lciImport.Visibility = (MedicineTypeADO.IsShowImport.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciImport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (MedicineTypeADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MedicineTypeADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);

                }
                if (MedicineTypeADO.IsShowButtonAdd.HasValue)
                {
                    lciExportExcel.Visibility = (MedicineTypeADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciExportExcel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MedicineTypeADO.IsShowRadioThieuThongTinBHYT.HasValue)
                {
                    lciThieuThongTinBhyt.Visibility = (MedicineTypeADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    lciDuThongTinBHYT.Visibility = (MedicineTypeADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    lciCheckAll.Visibility = (MedicineTypeADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciThieuThongTinBhyt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciDuThongTinBHYT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCheckAll.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MedicineTypeADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MedicineTypeADO.Keyword_NullValuePrompt;
                        txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
                        txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
                    }
                }

                if (MedicineTypeADO.IsExportExcel.HasValue)
                {
                    lciExportExcel.Visibility = (MedicineTypeADO.IsExportExcel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                      : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                    var listdata = trvService.DataSource as BindingList<MedicineTypeADO>;
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
                BindingList<MedicineTypeADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MedicineTypeADO> rearchResult = new List<MedicineTypeADO>();

                    rearchResult = MedicineTypeADOs.Where(o =>
                                                    ((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_LINE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.MEDICINE_USE_FORM_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.HEIN_SERVICE_BHYT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    || (o.SERVICE_UNIT_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                                                    )
                                                    ).Distinct().ToList();

                    listResult = new BindingList<MedicineTypeADO>(rearchResult);
                }
                else
                {
                    listResult = new BindingList<MedicineTypeADO>(MedicineTypeADOs);
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

        private void trvService_NodeCellStyle(object sender, DevExpress.XtraTreeList.GetCustomNodeCellStyleEventArgs e)
        {
            try
            {
                var data = trvService.GetDataRecordByNode(e.Node);
                if (MedicineTypeNodeCellStyle != null)
                {
                    MedicineTypeNodeCellStyle(data, e.Appearance);
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
                    if (node != null && data != null && data is MedicineTypeADO)
                    {
                        if (MedicineTypeRowEnter != null)
                        {
                            MedicineTypeRowEnter((MedicineTypeADO)data);
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
                    if (node != null && node.HasChildren && data != null && data is MedicineTypeADO)
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
                if (this.MedicineType_BeforeCheck != null)
                    this.MedicineType_BeforeCheck(node);
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
                if (this.MedicineType_AfterCheck == null)
                    return;
                var row = trvService.GetDataRecordByNode(e.Node);
                if (row != null && row is MedicineTypeADO)
                {
                    MedicineType_AfterCheck(e.Node, (MedicineTypeADO)row);
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
                        MedicineTypeHandler clickhandler = btn.Tag as MedicineTypeHandler;
                        if (clickhandler != null)
                        {
                            var data = trvService.GetDataRecordByNode(trvService.FocusedNode);
                            if (data != null && data is MedicineTypeADO)
                            {
                                clickhandler((MedicineTypeADO)data);
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
                if (data != null && data is MedicineTypeADO)
                {
                    if (this.MedicineType_GetSelectImage != null)
                        this.MedicineType_GetSelectImage((MedicineTypeADO)data, e);
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
                if (data != null && data is MedicineTypeADO)
                {
                    if (this.MedicineType_GetStateImage != null)
                        this.MedicineType_GetStateImage((MedicineTypeADO)data, e);
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
                if (data != null && data is MedicineTypeADO)
                {
                    if (this.MedicineType_SelectImageClick != null)
                        this.MedicineType_SelectImageClick((MedicineTypeADO)data);
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
                if (data != null && data is MedicineTypeADO)
                {
                    if (this.MedicineType_StateImageClick != null)
                        this.MedicineType_StateImageClick((MedicineTypeADO)data);
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
                    MedicineTypeADO currentRow = e.Row as MedicineTypeADO;
                    if (currentRow == null) return;
                    if (MedicineType_CustomUnboundColumnData != null)
                        this.MedicineType_CustomUnboundColumnData(currentRow, e);
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
                if (data != null && data is MedicineTypeADO)
                {
                    var rowData = data as MedicineTypeADO;

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
                        e.RepositoryItem = repositoryItemCheckCPNG;

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
                        e.RepositoryItem = repositoryItemCheckEdit5;

                        if (rowData != null && rowData.IS_REQUIRE_HSD == 1)
                        {
                            rowData.IsExprireDate = true;
                        }
                        else
                        {
                            rowData.IsExprireDate = false;
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
                    if (e.Column.FieldName == "IsFood")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit4;
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
                if (data != null && data is MedicineTypeADO)
                {
                    var rowData = data as MedicineTypeADO;
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
                            if (data != null && data is MedicineTypeADO)
                            {
                                foreach (var menu in this.menuItems((MedicineTypeADO)data))
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
                if (data != null && data is MedicineTypeADO)
                {
                    var rowData = data as MedicineTypeADO;
                    if (rowData != null && this.MedicineType_CustomDrawNodeCell != null)
                    {
                        this.MedicineType_CustomDrawNodeCell(rowData, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetListNodeCheck(ref List<MedicineTypeADO> result, TreeListNode node)
        {
            try
            {
                if (node.Nodes == null || node.Nodes.Count == 0)
                {
                    if (node.Checked)
                    {
                        result.Add((MedicineTypeADO)trvService.GetDataRecordByNode(node));
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
                    MedicineTypeADO medicineTypeFocus = (MedicineTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (medicineTypeFocus != null && MedicineTypeDoubleClick != null)
                    {
                        MedicineTypeDoubleClick(medicineTypeFocus);
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
                    MedicineTypeADO medicineTypeFocus = (MedicineTypeADO)trvService.GetDataRecordByNode(hi.Node);
                    if (medicineTypeFocus != null && this.MedicineTypeClick != null)
                    {
                        this.MedicineTypeClick(medicineTypeFocus);
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

        public void Reload(List<MedicineTypeADO> MedicineTypes)
        {
            try
            {
                txtKeyword.Text = "";
                this.MedicineTypeADO.MedicineTypes = MedicineTypes;
                if (this.MedicineTypeADO.MedicineTypes == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public List<MedicineTypeADO> GetListCheck()
        {
            List<MedicineTypeADO> result = new List<MedicineTypeADO>();
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
                result = new List<MedicineTypeADO>();
            }
            return result;
        }

        public object GetData()
        {
            try
            {
                return trvService.DataSource;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        #endregion

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

        private void chkThieuThongTinBHYT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.checkThieuThongTinBHYT_CheckChange != null)
                {
                    this.checkThieuThongTinBHYT_CheckChange(chkCheckAll.CheckState, chkDuThongTinBHYT.CheckState, chkThieuThongTinBHYT.CheckState);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.checkThieuThongTinBHYT_CheckChange != null && chkCheckAll.CheckState == CheckState.Checked)
                {
                    this.checkThieuThongTinBHYT_CheckChange(chkCheckAll.CheckState, chkDuThongTinBHYT.CheckState, chkThieuThongTinBHYT.CheckState);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkDuThongTinBHYT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.checkThieuThongTinBHYT_CheckChange != null && chkDuThongTinBHYT.CheckState == CheckState.Checked)
                {
                    this.checkThieuThongTinBHYT_CheckChange(chkCheckAll.CheckState, chkDuThongTinBHYT.CheckState, chkThieuThongTinBHYT.CheckState);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                //if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                //{
                //    savePath = saveFileDialog1.FileName;

                //    PrintProcess(ExportDataExcel.EXPORT_DATA_EXCEL);
                //}
                if (this.MedicineType_ExportExcel != null)
                    this.MedicineType_ExportExcel();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportExcel_Click()
        {

        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.MedicineType_Import != null)
                    this.MedicineType_Import();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.MedicineType_Save != null)
                    this.MedicineType_Save();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }
    }
}
