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
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using DevExpress.Utils.Paint;
using DevExpress.Utils.Text;

namespace HIS.UC.MedicineType.Run
{
    public partial class UCMedicineType : UserControl
    {
        #region Declare
        MedicineTypeInitADO MedicineTypeInitADO;
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
        CboBid_EditValueChanged cboBid_EditValueChange;
        CboContract_EditValueChanged cboContract_EditValueChange;
        MedicineType_ExportExcel MedicineType_ExportExcel;
        MedicineType_Import MedicineType_Import;
        MedicineType_Save MedicineType_Save;
        ChkLock_CheckChange chkLock_CheckChange;
        MedicineType_PrintPriceList MedicineType_PrintPriceList;
        bool IsShowCheckNode;
        bool isAutoWidth;
        bool IsShowRadioThieuTTBHYT;
        bool isShowImport;
        bool isShowBid;
        bool isShowChkLock;
        bool isHightLightFilter = false;
        DevExpress.Utils.ImageCollection selectImageCollection;
        DevExpress.Utils.ImageCollection stateImageCollection;
        MedicineTypeHandler updateSingleRow;
        MenuItems menuItems;
        const int GroupType__All = 1;
        const int GroupType__Group = 2;
        bool isCreateParentNodeWithMedicineTypeExpend = true;
        string savePath = null;
        string[] HighlightedSearchTerms;
        int rowIndex = -1;
        int rowIndexTop = -1;
        TreeListNode treeListNode;
        #endregion

        #region Construct
        public UCMedicineType(MedicineTypeInitADO MedicineTypeADO)
        {
            InitializeComponent();
            try
            {
                this.MedicineTypeInitADO = MedicineTypeADO;
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
                this.cboBid_EditValueChange = MedicineTypeADO.cboBid_EditValueChanged;
                this.cboContract_EditValueChange = MedicineTypeADO.cboContract_EditValueChanged;
                this.MedicineType_ExportExcel = MedicineTypeADO.MedicineType_ExportExcel;
                this.MedicineType_Import = MedicineTypeADO.MedicineType_Import;
                this.MedicineType_Save = MedicineTypeADO.MedicineType_Save;
                this.MedicineType_PrintPriceList = MedicineTypeADO.MedicineType_PrintPriceList;
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
                    this.isShowImport = MedicineTypeADO.IsShowImport.Value;
                }
                if (MedicineTypeADO.IsShowBid.HasValue)
                {
                    this.isShowBid = MedicineTypeADO.IsShowBid.Value;
                }
                if (MedicineTypeADO.IsShowChkLock.HasValue)
                {
                    this.isShowChkLock = MedicineTypeADO.IsShowChkLock.Value;
                }
                if (MedicineTypeADO.IsHightLightFilter.HasValue)
                {
                    this.isHightLightFilter = MedicineTypeADO.IsHightLightFilter.Value;
                }
                this.chkLock_CheckChange = MedicineTypeADO.chkLock_CheckChange;
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
                if (MedicineTypeInitADO != null)
                {
                    InitComboBid(MedicineTypeInitADO.listBids);
                    InitComboContract(MedicineTypeInitADO.listContracts);
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
                SetCaptionByLanguageKeyNew();
                //SetCaptionByLanguageKey();
                trvService.ToolTipController = this.toolTipController1;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.MedicineType.Resources.Lang", typeof(HIS.UC.MedicineType.Run.UCMedicineType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.lciLock.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThieuThongTinBhyt.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciThieuThongTinBhyt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDuThongTinBHYT.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciDuThongTinBHYT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckAll.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciCheckAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportExcel.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.btnExportExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBid.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBid.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicineType.lciBid.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCMedicineType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.MedicineType.Resources.Lang", typeof(UCMedicineType).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.trvService.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("UCMedicineType.trvService.OptionsFind.FindNullPrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnActiveIngrBhytName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnActiveIngrBhytName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnConcentra.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnConcentra.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnManufactoryName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnManufactoryName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnTotalPrice.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnDiscount.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnVATPercent.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnVATPercent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnHeinServiceBHYTName.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnHeinServiceBHYTName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnTransactionCode.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnTransactionCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumnPrice.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumnPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnDVKTC.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnDVKTC.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnDVKTC.ToolTip = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnDVKTC.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeColumnIsExpend.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeColumnIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboContract.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicineType.cboContract.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPriceList.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.btnPriceList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkLock.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.chkLock.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBid.Properties.NullText = Inventec.Common.Resource.Get.Value("UCMedicineType.cboBid.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnImport.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.btnImport.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportExcel.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.btnExportExcel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCheckAll.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.chkCheckAll.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkDuThongTinBHYT.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.chkDuThongTinBHYT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkThieuThongTinBHYT.Properties.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.chkThieuThongTinBHYT.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciThieuThongTinBhyt.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciThieuThongTinBhyt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDuThongTinBHYT.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciDuThongTinBHYT.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCheckAll.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciCheckAll.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBid.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciBid.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciLock.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.lciLock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCMedicineType.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("UCMedicineType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                MedicineTypeADOs = new List<MedicineTypeADO>();
                if (MedicineTypeInitADO.MedicineTypes != null)
                {
                    MedicineTypeADOs = (from r in MedicineTypeInitADO.MedicineTypes select new MedicineTypeADO(r)).ToList();
                    MedicineTypeADOs = MedicineTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
                else if (MedicineTypeInitADO.MedicineTypeADOs != null)
                {
                    MedicineTypeADOs = MedicineTypeInitADO.MedicineTypeADOs;
                    MedicineTypeADOs = MedicineTypeADOs.OrderBy(o => o.NUM_ORDER).ThenBy(o => o.MEDICINE_TYPE_NAME).ToList();
                }
                records = new BindingList<MedicineTypeADO>(MedicineTypeADOs.Where(o => string.IsNullOrEmpty(txtKeyword.Text) || o.KEY_WORD.ToLower().Contains(txtKeyword.Text.ToLower())).ToList());

                if (!String.IsNullOrEmpty(MedicineTypeInitADO.KeyFieldName))
                    trvService.KeyFieldName = MedicineTypeInitADO.KeyFieldName;
                if (!String.IsNullOrEmpty(MedicineTypeInitADO.ParentFieldName))
                    trvService.ParentFieldName = MedicineTypeInitADO.ParentFieldName;

                trvService.DataSource = records;
                if (this.MedicineType_CheckAllNode != null)
                    this.MedicineType_CheckAllNode(trvService.Nodes);


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

        private void InitializeTree()
        {
            try
            {
                //Inventec.Common.Logging.LogSystem.Info("----InitializeTree----bat dau InitializeTree uc HIS.UC.MedicineType");
                this.trvService.OptionsView.ShowCheckBoxes = this.IsShowCheckNode;
                this.trvService.OptionsView.AutoWidth = this.isAutoWidth;
                if (MedicineTypeInitADO.MedicineTypeColumns != null && MedicineTypeInitADO.MedicineTypeColumns.Count > 0)
                {
                    foreach (var svtr in MedicineTypeInitADO.MedicineTypeColumns)
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
                if (MedicineTypeInitADO.ColumnButtonEdits != null && MedicineTypeInitADO.ColumnButtonEdits.Count > 0)
                {
                    foreach (var svtr in MedicineTypeInitADO.ColumnButtonEdits)
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
                        treeListColumn.Fixed = svtr.Fixed;
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
                if (MedicineTypeInitADO.IsShowImport.HasValue)
                {
                    lciImport.Visibility = (MedicineTypeInitADO.IsShowImport.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciImport.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                if (MedicineTypeInitADO.IsShowSearchPanel.HasValue)
                {
                    lciKeyword.Visibility = (MedicineTypeInitADO.IsShowSearchPanel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);

                }
                if (MedicineTypeInitADO.IsShowButtonAdd.HasValue)
                {
                    lciExportExcel.Visibility = (MedicineTypeInitADO.IsShowButtonAdd.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciExportExcel.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MedicineTypeInitADO.IsShowRadioThieuThongTinBHYT.HasValue)
                {
                    lciThieuThongTinBhyt.Visibility = (MedicineTypeInitADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    lciDuThongTinBHYT.Visibility = (MedicineTypeInitADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    lciCheckAll.Visibility = (MedicineTypeInitADO.IsShowRadioThieuThongTinBHYT.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciThieuThongTinBhyt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciDuThongTinBHYT.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCheckAll.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (lciKeyword.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    if (!String.IsNullOrEmpty(MedicineTypeInitADO.Keyword_NullValuePrompt))
                    {
                        txtKeyword.Properties.NullValuePrompt = MedicineTypeInitADO.Keyword_NullValuePrompt;
                        txtKeyword.Properties.NullValuePromptShowForEmptyValue = true;
                        txtKeyword.Properties.ShowNullValuePromptWhenFocused = true;
                    }
                }

                if (MedicineTypeInitADO.IsExportExcel.HasValue)
                {
                    lciExportExcel.Visibility = (MedicineTypeInitADO.IsExportExcel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                      : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                    lciPrintPriceList.Visibility = (MedicineTypeInitADO.IsExportExcel.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                      : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }

                if (MedicineTypeInitADO.IsShowChkLock.HasValue)
                {
                    lciLock.Visibility = (MedicineTypeInitADO.IsShowChkLock.Value ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                      : DevExpress.XtraLayout.Utils.LayoutVisibility.Never);
                }
                else
                {
                    lciLock.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MedicineTypeInitADO.IsShowBid == true)
                {
                    lciBid.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciKeyword.Size = new Size(360, 26);
                    lciBid.Size = new Size(150, 26);
                    //emptySpaceItem1.Size = new Size(100, 26);
                }
                else
                {
                    lciBid.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

                if (MedicineTypeInitADO.IsShowContract == true)
                {
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    emptySpaceItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciKeyword.Size = new Size(300, 26);
                    layoutControlItem1.Size = new Size(150, 26);
                }
                else
                {
                    layoutControlItem1.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
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

                    //rearchResult = MedicineTypeADOs.Where(o =>
                    //                                ((o.MEDICINE_TYPE_NAME ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())
                    //                                || (o.MEDICINE_TYPE_CODE ?? "").ToString().ToLower().Contains(keyword.Trim().ToLower())

                    //                                )
                    //                                ).Distinct().ToList();

                    rearchResult = MedicineTypeADOs.Where(o => o.KEY_WORD.ToLower().Contains(txtKeyword.Text.ToLower())).ToList();

                    listResult = new BindingList<MedicineTypeADO>(rearchResult);

                    HighlightedSearchTerms = keyword.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    listResult = new BindingList<MedicineTypeADO>(MedicineTypeADOs);
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
                    rowIndexTop = trvService.TopVisibleNodeIndex;
                    rowIndex = trvService.GetVisibleIndexByNode(e.Node);
                    treeListNode = e.Node;
                    if (this.MedicineType_SelectImageClick != null)
                    {
                        this.MedicineType_SelectImageClick((MedicineTypeADO)data);
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

        public void Reload(List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> MedicineTypes)
        {
            try
            {
            //    ProcessLoad(MedicineTypes);
            //    trvService.ExpandAll();
            this.MedicineTypeInitADO.MedicineTypes = MedicineTypes;
            if (this.MedicineTypeInitADO.MedicineTypes == null)
                records = null;
            BindTreePlus();
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
                this.MedicineTypeInitADO.MedicineTypeADOs = MedicineTypes;
                if (this.MedicineTypeInitADO.MedicineTypes == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void CreateList()
		{
			try
			{
                this.MedicineTypeInitADO.MedicineTypeADOs = new List<MedicineTypeADO>();
            }
			catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
		}

        public void ProcessLoad(List<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE> MedicineTypes)
        {
            try
            {
                this.MedicineTypeInitADO.MedicineTypes = MedicineTypes;
                if (this.MedicineTypeInitADO.MedicineTypes == null)
                    records = null;
                BindTreePlus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ProcessLoad(List<MedicineTypeADO> MedicineTypes)
        {
            try
            {
                //txtKeyword.Text = "";
                this.MedicineTypeInitADO.MedicineTypeADOs.AddRange(MedicineTypes);
                if (this.MedicineTypeInitADO.MedicineTypes == null)
                    records = null;
                BindTreePlus();
                trvService.ExpandAll();
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

        private void cboBid_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.cboBid_EditValueChange != null)
                {
                    if (cboBid.EditValue != null)
                    {
                        this.cboBid_EditValueChange(Inventec.Common.TypeConvert.Parse.ToInt64(cboBid.EditValue.ToString() ?? ""));
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = 16;
                    }
                    else
                    {
                        this.cboBid_EditValueChange(null);
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboBid_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
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

        private void chkLock_EditValueChanged(object sender, EventArgs e)
        {

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
                        var data = (MedicineTypeADO)trvService.GetDataRecordByNode(o);
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

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.chkLock_CheckChange != null)
                {
                    this.chkLock_CheckChange(this.chkLock.CheckState);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                if (btnPriceList.Enabled && this.MedicineType_PrintPriceList != null)
                    this.MedicineType_PrintPriceList(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void trvService_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
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

        private void gridLookUpEdit1_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
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
                        trvService.Columns.ColumnByFieldName("TDL_BID_GROUP_CODE").VisibleIndex = 16;
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

        public void SetEditValueContract(long? ContractId)
        {
            try
            {
                cboContract.EditValue = ContractId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public long? GetContractId()
        {
            long? ContractId = null;
            try
            {
                if (cboContract.EditValue != null)
                    ContractId = Inventec.Common.TypeConvert.Parse.ToInt64(cboContract.EditValue.ToString());
                else
                    ContractId = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return ContractId;
        }

        internal void Refresh(List<V_HIS_MEDICINE_TYPE> medicineTypes)
        {
            try
            {
                ProcessLoad(medicineTypes);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
