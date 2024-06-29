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
using DevExpress.XtraEditors.Controls;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Desktop.Common.LanguageManager;
using DevExpress.Utils.Text;
using DevExpress.Utils.Paint;
using System.Resources;

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
        cboBid_EditValueChanged cboBid_EditValueChange;
        cboContract_EditValueChanged cboContract_EditValueChange;
        MaterialType_PrintPriceList MaterialType_PrintPriceList;
        cboIsReusable_EditValueChanged cboReusable_EditValueChanged;

        bool IsShowCheckNode;
        bool isAutoWith;
        bool isShowImport;
        bool isShowBid;
        bool isShowChkLock;
        bool isHightLightFilter = false;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MaterialTypeHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMaterialTypeExpend = true;
        ChkLock_CheckChange chkLock_CheckChange;
        ChkLock_CheckChange chkGroupByMap_CheckChanged;
        bool isCheckGroupByMap;
        string[] HighlightedSearchTerms;
        int rowIndex = -1;
        int rowIndexTop = -1;
        TreeListNode treeListNode;
        #endregion

        #region Construct
        public UCMaterialType(MaterialTypeInitADO MaterialTypeADO)
        {
            InitializeComponent();
            try
            {
                SetCaptionByLanguageKey();
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
                this.cboBid_EditValueChange = MaterialTypeADO.cboBid_EditValueChanged;
                this.cboContract_EditValueChange = MaterialTypeADO.cboContract_EditValueChanged;
                this.MaterialType_PrintPriceList = MaterialTypeADO.MaterialType_PrintPriceList;
                this.cboReusable_EditValueChanged = MaterialTypeADO.cboIsReusable_EditValueChanged;
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
                if (MaterialTypeADO.IsShowBid.HasValue)
                {
                    this.isShowBid = MaterialTypeADO.IsShowBid.Value;
                }

                if (MaterialTypeADO.IsShowChkLock.HasValue)
                {
                    this.isShowChkLock = MaterialTypeADO.IsShowChkLock.Value;
                }
                if (MaterialTypeADO.IsHightLightFilter.HasValue)
                {
                    this.isHightLightFilter = MaterialTypeADO.IsHightLightFilter.Value;
                }
                if (MaterialTypeADO.IsCheckGroupByMap.HasValue)
                {
                    this.isCheckGroupByMap = MaterialTypeADO.IsCheckGroupByMap.Value;
                }
                this.chkLock_CheckChange = MaterialTypeADO.chkLock_CheckChange;
                this.chkGroupByMap_CheckChanged = MaterialTypeADO.chkGroupByMap_CheckChanged;
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
                    InitComboBid(MaterialTypeTreeADO.listBids);
                    InitComboContract(MaterialTypeTreeADO.listContracts);
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
                    this.trvService.ToolTipController = this.toolTipController1;
                    trvService.MoveFirst();
                    txtKeyword.Focus();
                    txtKeyword.SelectAll();
                    this.cboIsReusable.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCMaterialType
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.MaterialType.Resources.Lang", typeof(UCMaterialType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.trvService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCMaterialType.trvService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnServiceName.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnServiceCode.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnAmount.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnPriceNoVAT.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnPriceNoVAT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnVATPercent.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnVATPercent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnTotalPrice.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnTotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnTotalHeinPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnTotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnTotalPatientPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnDiscount.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnIsExpend.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeColumnIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkGroupByMap.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.checkGroupByMap.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMaterialType.cboContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPriceList.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.btnPriceList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkLock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.chkLock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBid.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMaterialType.cboBid.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportExcel.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.btnExportExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCMaterialType.layoutControlItem1.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.lciLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCMaterialType.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMaterialType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                else if (MaterialTypeTreeADO.MaterialTypeADOs != null)
                {
                    MaterialTypeADOs = MaterialTypeTreeADO.MaterialTypeADOs;
                    MaterialTypeADOs = MaterialTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();
                }

                if (lciCheckGroupByMap.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && checkGroupByMap.Checked
                    && MaterialTypeADOs != null && MaterialTypeADOs.Any(a => a.MATERIAL_TYPE_MAP_ID.HasValue))
                {
                    var Groups = MaterialTypeADOs.Where(o => o.MATERIAL_TYPE_MAP_ID.HasValue).GroupBy(g => g.MATERIAL_TYPE_MAP_ID.Value);

                    foreach (var g in Groups)
                    {
                        var childs = g.ToList();
                        MaterialTypeADO ado = new MaterialTypeADO();
                        ado.SetMaterialTypeMap(childs[0]);
                        childs.ForEach(o =>
                        {
                            o.ParentField = String.Format("MAP_{0}", o.MATERIAL_TYPE_MAP_ID.Value);
                        });
                        MaterialTypeADOs.Add(ado);
                    }
                    MaterialTypeADOs = MaterialTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MATERIAL_TYPE_NAME).ToList();
                }

                records = new BindingList<MaterialTypeADO>(MaterialTypeADOs.Where(o => string.IsNullOrEmpty(txtKeyword.Text) || o.KEY_WORD.ToLower().Contains(txtKeyword.Text.ToLower())).ToList());

                if (!String.IsNullOrEmpty(MaterialTypeTreeADO.KeyFieldName))
                    trvService.KeyFieldName = MaterialTypeTreeADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MaterialTypeTreeADO.ParentFieldName))
                    trvService.ParentFieldName = MaterialTypeTreeADO.ParentFieldName;

                trvService.DataSource = records;
                trvService.ExpandAll();
                if (this.MaterialType_CheckAllNode != null)
                {
                    this.MaterialType_CheckAllNode(trvService.Nodes);
                }

                if (rowIndex != -1 && rowIndexTop != -1)
                {
                    trvService.TopVisibleNodeIndex = rowIndexTop;
                    trvService.FocusedNode = trvService.GetNodeByVisibleIndex(rowIndex);
                }
                else
                {
                    rowIndex = -1;
                    rowIndexTop = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBid(List<V_HIS_BID> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BID_NAME", "", 300, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BID_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBid, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        col.ToolTip = svtr.ToolTip;
                        if (svtr.UnboundColumnType != null && svtr.UnboundColumnType != UnboundColumnType.Bound)
                            col.UnboundType = svtr.UnboundColumnType;
                        if (svtr.Format != null)
                        {
                            col.Format.FormatString = svtr.Format.FormatString;
                            col.Format.FormatType = svtr.Format.FormatType;
                        }
                        col.Fixed = svtr.Fixed;
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
                        treeListColumn.Fixed = svtr.Fixed;
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
                    lciPrintPriceList.Visibility = (MaterialTypeTreeADO.IsShowExportExcel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
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
                if (MaterialTypeTreeADO.IsShowBid == true)
                {
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    //emptySpaceItem1.Size = new Size(100, 26);
                }
                else
                {
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MaterialTypeTreeADO.IsShowContract == true)
                {
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    layoutControlItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MaterialTypeTreeADO.IsShowChkLock.HasValue)
                {
                    lciLock.Visibility = (MaterialTypeTreeADO.IsShowChkLock.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                      : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciLock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (chkGroupByMap_CheckChanged != null)
                {
                    lciCheckGroupByMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    checkGroupByMap.Checked = this.isCheckGroupByMap;
                }
                else
                {
                    lciCheckGroupByMap.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    checkGroupByMap.Checked = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                trvService.Focus();
                trvService.FocusedNode = trvService.Nodes[1];
                return;
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
            {   // Đây là UC
                BindingList<MaterialTypeADO> listResult = null;
                if (!String.IsNullOrEmpty(keyword.Trim()))
                {
                    List<MaterialTypeADO> rearchResult = new List<MaterialTypeADO>();

                    rearchResult = MaterialTypeADOs.Where(o => o.KEY_WORD
                    .ToLower().Contains(txtKeyword.Text.ToLower())
                    || (o.MATERIAL_TYPE_CODE ?? "")
                    .ToString().ToLower().Contains(keyword.Trim().ToLower())
                    || (o.HEIN_SERVICE_BHYT_CODE ?? "").ToString().ToLower()
                    .Contains(keyword.Trim().ToLower())
                    || (o.HEIN_SERVICE_BHYT_NAME ?? "")
                    .ToString().ToLower().Contains(keyword.Trim()
                    .ToLower()) || (o.REGISTER_NUMBER ?? "")
                    .ToString().ToLower().Contains(keyword.Trim().ToLower())
                    || (o.NATIONAL_NAME ?? "").ToString()
                    .ToLower().Contains(keyword.Trim().ToLower())
                    || (o.CONCENTRA ?? "").ToString().ToLower()
                    .Contains(keyword.Trim().ToLower())
                    || (o.MANUFACTURER_NAME ?? "").ToString()
                    .ToLower().Contains(keyword.Trim().ToLower())
                    ).Distinct().ToList();

                    if (rearchResult != null && rearchResult.Count > 0 && rearchResult.Any(a => !String.IsNullOrWhiteSpace(a.ParentField)))
                    {
                        List<string> keys = rearchResult.Where(o => !String.IsNullOrWhiteSpace(o.ParentField)).Select(s => s.ParentField).Distinct().ToList();
                        var rs = MaterialTypeADOs.Where(o => rearchResult.Contains(o) || keys.Contains(o.KeyField)).ToList();
                        rearchResult = rs;
                    }

                    listResult = new BindingList<MaterialTypeADO>(rearchResult);

                    HighlightedSearchTerms = keyword.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    listResult = new BindingList<MaterialTypeADO>(MaterialTypeADOs);
                    HighlightedSearchTerms = new String[0];
                }
                trvService.DataSource = listResult;
                trvService.ExpandAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void HighlightFilterSearchWords(Graphics graphics, string displayText, Rectangle textEditBounds, DevExpress.Utils.AppearanceObject appearance)
        {
            try
            {
                Size textSize;
                Rectangle matchingTextBounds;
                Int32 startIndex;
                Int32 matchIndex;
                Int32 drawPositionOffset;
                //If highlighted terms is not null, and column is description column
                if (this.HighlightedSearchTerms != null)
                {
                    //Set start index
                    startIndex = 0;
                    //Set draw position offset
                    drawPositionOffset = 0;
                    //Create brush
                    using (SolidBrush brush = new SolidBrush(Color.Gold))
                    {
                        //Loop though all parts of criteria
                        foreach (String part in HighlightedSearchTerms)
                        {
                            //Get match index
                            matchIndex = displayText.ToLower().IndexOf(part, startIndex);
                            //If matched
                            if (matchIndex != -1)
                            {
                                //If match index is not start index
                                if (matchIndex != startIndex)
                                {
                                    //Measure none matching text
                                    textSize = TextUtils.GetStringSize(graphics, displayText.Substring(startIndex, matchIndex - startIndex), appearance.Font,
                                        appearance.TextOptions.GetStringFormat());
                                    //Set draw position offset
                                    drawPositionOffset += textSize.Width;
                                }
                                //Measure matching text
                                textSize = TextUtils.GetStringSize(graphics, displayText.Substring(matchIndex, part.Length), appearance.Font, appearance.TextOptions.GetStringFormat());
                                //Get background rectangle for matching text
                                matchingTextBounds = new Rectangle(textEditBounds.Location, textSize);
                                matchingTextBounds.Offset(drawPositionOffset, 0);
                                //If matchingTextBounds falls outside of textEditBounds
                                if (matchingTextBounds.Location.X > textEditBounds.Location.X + textEditBounds.Width)
                                {
                                    //No need to go further
                                    break;
                                }
                                else
                                {
                                    //If matchingTextBounds extends outside of textEditBounds
                                    if (matchingTextBounds.Location.X + matchingTextBounds.Width > textEditBounds.Location.X + textEditBounds.Width)
                                    {
                                        //Shorten matchingTextBounds to fit within textEditBounds
                                        matchingTextBounds.Width = textEditBounds.Width - (matchingTextBounds.Location.X - textEditBounds.Location.X);
                                    }
                                    //Draw background
                                    XPaint.Graphics.FillRectangle(graphics, brush, matchingTextBounds);
                                    //Set start index to match index
                                    startIndex = matchIndex + part.Length;
                                    //Increase draw position offset
                                    drawPositionOffset += textSize.Width;
                                }
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE> MaterialTypes)
        {
            try
            {
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

        public void Reload(List<MaterialTypeADO> MaterialTypes)
        {
            try
            {
                this.MaterialTypeTreeADO.MaterialTypeADOs = MaterialTypes;
                if (this.MaterialTypeTreeADO.MaterialTypeADOs == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadBid(List<MOS.EFMODEL.DataModels.V_HIS_BID_1> bids)
        {
            try
            {
                cboBid.Properties.BeginUpdate();
                cboBid.Properties.DataSource = bids;
                cboBid.Properties.EndUpdate();
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

        public object GetDataFocus()
        {
            try
            {
                var node = trvService.FocusedNode;
                var data = trvService.GetDataRecordByNode(node);
                return data;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        public void SetEnableBid(bool enable)
        {
            try
            {
                cboBid.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public long? GetBidId()
        {
            long? bidId = null;
            try
            {
                if (cboBid.EditValue != null)
                    bidId = Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString());
                else
                    bidId = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return bidId;
        }

        public void SetEditValueBid(long? bidId)
        {
            try
            {
                cboBid.EditValue = bidId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool GetBidEnable()
        {
            bool result = false;
            try
            {
                result = cboBid.Enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
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

                if (e.Node.Focused && this.isHightLightFilter)
                {
                    e.Appearance.BackColor = Color.LightGreen;
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
                    rowIndexTop = trvService.TopVisibleNodeIndex;
                    rowIndex = trvService.GetVisibleIndexByNode(e.Node);
                    treeListNode = e.Node;
                    if (this.MaterialType_SelectImageClick != null)
                    {
                        this.MaterialType_SelectImageClick((MaterialTypeADO)data);
                    }

                    rowIndex = 1;
                    rowIndexTop = 1;
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
                    {
                        this.MaterialType_StateImageClick((MaterialTypeADO)data);
                    }
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
                    if (e.Column.FieldName == "IsMustPrepare")
                    {
                        e.RepositoryItem = repositoryItemCheckEdit4;
                        if (rowData != null && rowData.IS_MUST_PREPARE == 1)
                        {
                            rowData.IsMustPrepare = true;
                        }
                        else
                        {
                            rowData.IsMustPrepare = false;
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
                Rectangle textEditBounds;

                if (this.isHightLightFilter && this.HighlightedSearchTerms != null)
                {
                    textEditBounds = e.Bounds;
                    textEditBounds.Offset(2, 1);        // Offset is (2, 1) for GridControl or (3, 2) for TreeList
                    HighlightFilterSearchWords(e.Graphics, e.CellText, textEditBounds, e.Appearance);
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

        private void cboBid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboBid_EditValueChange != null)
                    if (cboBid.EditValue != null)
                    {
                        this.cboBid_EditValueChange(Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString() ?? ""));
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = 14;
                    }
                    else
                    {
                        this.cboBid_EditValueChange(null);
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = -1;
                    }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBid.Properties.Buttons[1].Visible = true;
                    cboBid.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            if (this.chkLock_CheckChange != null)
            {
                this.chkLock_CheckChange(chkLock.CheckState);
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
                        var data = (MaterialTypeADO)trvService.GetDataRecordByNode(o);
                        if (data != null)
                        {
                            if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            {
                                text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_MEDICINE_TYPE__KHOA", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
                            else
                            {
                                text = Inventec.Common.Resource.Get.Value("INIT_LANGUAGE__UC_TREE_MEDICINE_TYPE__BO_KHOA", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                            }
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

        private void txtKeyword_EditValueChanged(object sender, EventArgs e)
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

        private void btnPriceList_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnPriceList.Enabled && this.MaterialType_PrintPriceList != null)
                    this.MaterialType_PrintPriceList(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal void FocusBid()
        {
            try
            {
                cboBid.Focus();
                cboBid.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboContract_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboContract.Properties.Buttons[1].Visible = true;
                    cboContract.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboContract_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboContract_EditValueChange != null)
                {
                    if (cboContract.EditValue != null)
                    {
                        this.cboContract_EditValueChange(Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString() ?? ""));
                        trvService.Columns.ColumnByFieldName("AMOUNT_IN_BID").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("IMP_PRICE_IN_BID").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("IMP_VAT_RATIO_IN_BID").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("AMOUNT_IN_CONTRACT").VisibleIndex = 11;
                        trvService.Columns.ColumnByFieldName("IMP_PRICE_IN_CONTRACT").VisibleIndex = 12;
                        trvService.Columns.ColumnByFieldName("IMP_VAT_RATIO_IN_CONTRACT").VisibleIndex = 13;
                        trvService.Columns.ColumnByFieldName("DISCOUNT_FROM_DATE_STR").VisibleIndex = 14;
                        trvService.Columns.ColumnByFieldName("DISCOUNT_TO_DATE_STR").VisibleIndex = 15;
                    }
                    else
                    {
                        this.cboContract_EditValueChange(null);
                        trvService.Columns.ColumnByFieldName("AMOUNT_IN_BID").VisibleIndex = 11;
                        trvService.Columns.ColumnByFieldName("IMP_PRICE_IN_BID").VisibleIndex = 12;
                        trvService.Columns.ColumnByFieldName("IMP_VAT_RATIO_IN_BID").VisibleIndex = 13;
                        trvService.Columns.ColumnByFieldName("AMOUNT_IN_CONTRACT").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("IMP_PRICE_IN_CONTRACT").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("IMP_VAT_RATIO_IN_CONTRACT").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("DISCOUNT_FROM_DATE_STR").VisibleIndex = -1;
                        trvService.Columns.ColumnByFieldName("DISCOUNT_TO_DATE_STR").VisibleIndex = -1;
                    }

                    if (cboBid.EditValue != null)
                    {
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = 14;
                    }
                    else
                    {
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = -1;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboContract(List<HIS_MEDICAL_CONTRACT> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MEDICAL_CONTRACT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("MEDICAL_CONTRACT_NAME", "", 300, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MEDICAL_CONTRACT_CODE", "ID", columnInfos, false, 450);
                ControlEditorLoader.Load(cboContract, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadContract(List<MOS.EFMODEL.DataModels.HIS_MEDICAL_CONTRACT> contracts)
        {
            try
            {
                cboContract.Properties.BeginUpdate();
                cboContract.Properties.DataSource = contracts;
                cboContract.Properties.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetEnableContract(bool enable)
        {
            try
            {
                cboContract.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        public long? GetContractId()
        {
            long? contractId = null;
            try
            {
                if (cboContract.EditValue != null)
                    contractId = Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString());
                else
                    contractId = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return contractId;
        }

        public void SetEditValueContract(long? contractId)
        {
            try
            {
                cboContract.EditValue = contractId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool GetContractEnable()
        {
            bool result = false;
            try
            {
                result = cboContract.Enabled;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        internal void FocusContract()
        {
            try
            {
                cboContract.Focus();
                cboContract.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkGroupByMap_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkGroupByMap_CheckChanged != null) this.chkGroupByMap_CheckChanged(checkGroupByMap.CheckState);
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public bool GetStatusChkGroupByMap()
        {
            bool result = false;
            try
            {
                result = checkGroupByMap.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
            return result;
        }

        private void cboIsReusable_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboReusable_EditValueChanged != null)
                {
                    if (cboIsReusable.EditValue != null)
                    {
                        this.cboReusable_EditValueChanged(cboIsReusable.SelectedIndex);
                    }
                    else
                    {
                        this.cboIsReusable.SelectedIndex = 0;
                        this.cboReusable_EditValueChanged(null);
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
