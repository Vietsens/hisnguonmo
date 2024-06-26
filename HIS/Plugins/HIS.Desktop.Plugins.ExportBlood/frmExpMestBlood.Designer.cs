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
namespace HIS.Desktop.Plugins.ExportBlood
{
    partial class frmExpMestBlood
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExpMestBlood));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.cboBloodType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnRCSave = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnRCPrint = new DevExpress.XtraBars.BarButtonItem();
            this.bbtnAssignService = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridLookUpBloodRhCode = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit3View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridLookUpBloodAboCode = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridLookUpVolume = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnAssignService = new DevExpress.XtraEditors.SimpleButton();
            this.btnPrint = new DevExpress.XtraEditors.SimpleButton();
            this.btnSave = new DevExpress.XtraEditors.SimpleButton();
            this.gridControlExpMestBlood = new DevExpress.XtraGrid.GridControl();
            this.gridViewExMestBlood = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ExpMestBlood_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_Delete = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnDeleteBlood = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn_ExpMestBlood_BloodCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_BloodTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_BloodTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_ExpiredDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemDtExpiredDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
            this.gridColumn_ExpMestBlood_Volume = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_BloodAbo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_BloodRhCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlood_PatientTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtBloodCode = new DevExpress.XtraEditors.TextEdit();
            this.panelControlExpiredDate = new DevExpress.XtraEditors.PanelControl();
            this.txtExpiredDate = new DevExpress.XtraEditors.ButtonEdit();
            this.dtExpiredDate = new DevExpress.XtraEditors.DateEdit();
            this.gridControlBlood = new DevExpress.XtraGrid.GridControl();
            this.gridViewBlood = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_Blood_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Blood_Add = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemBtnAddBlood = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.gridColumn_Blood_BloodCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Blood_BloodTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Blood_Volume = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Blood_BloodAbo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_Blood_BloodRh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblBloodTypeInfo = new DevExpress.XtraEditors.LabelControl();
            this.gridControlExpMestBlty = new DevExpress.XtraGrid.GridControl();
            this.gridViewExpMestBlty = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn_ExpMestBlty_Stt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_BloodTypeCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_BloodTypeName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_Volume = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_BloodAbo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_BloodRh = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn_ExpMestBlty_Amount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBloodTypeInfo = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutExpiredDate = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutBloodCode = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            this.dxValidationProvider2 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboBloodType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpBloodRhCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpBloodAboCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpVolume.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExMestBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDtExpiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDtExpiredDate.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBloodCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlExpiredDate)).BeginInit();
            this.panelControlExpiredDate.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtExpiredDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiredDate.Properties.CalendarTimeProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiredDate.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnAddBlood)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestBlty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestBlty)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBloodTypeInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpiredDate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBloodCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboBloodType);
            this.layoutControl1.Controls.Add(this.gridLookUpBloodRhCode);
            this.layoutControl1.Controls.Add(this.gridLookUpBloodAboCode);
            this.layoutControl1.Controls.Add(this.gridLookUpVolume);
            this.layoutControl1.Controls.Add(this.btnAssignService);
            this.layoutControl1.Controls.Add(this.btnPrint);
            this.layoutControl1.Controls.Add(this.btnSave);
            this.layoutControl1.Controls.Add(this.gridControlExpMestBlood);
            this.layoutControl1.Controls.Add(this.txtBloodCode);
            this.layoutControl1.Controls.Add(this.panelControlExpiredDate);
            this.layoutControl1.Controls.Add(this.gridControlBlood);
            this.layoutControl1.Controls.Add(this.lblBloodTypeInfo);
            this.layoutControl1.Controls.Add(this.gridControlExpMestBlty);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 29);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(1134, 633);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboBloodType
            // 
            this.cboBloodType.Location = new System.Drawing.Point(97, 122);
            this.cboBloodType.MenuManager = this.barManager1;
            this.cboBloodType.Name = "cboBloodType";
            this.cboBloodType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboBloodType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboBloodType.Properties.NullText = "";
            this.cboBloodType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboBloodType.Properties.View = this.gridView1;
            this.cboBloodType.Size = new System.Drawing.Size(169, 20);
            this.cboBloodType.StyleController = this.layoutControl1;
            this.cboBloodType.TabIndex = 16;
            this.cboBloodType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboBloodType_Closed);
            this.cboBloodType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboBloodType_KeyUp);
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnRCSave,
            this.bbtnRCPrint,
            this.bbtnAssignService});
            this.barManager1.MaxItemId = 3;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCSave),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnRCPrint),
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnAssignService)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // bbtnRCSave
            // 
            this.bbtnRCSave.Caption = "Lưu (Ctrl S)";
            this.bbtnRCSave.Id = 0;
            this.bbtnRCSave.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S));
            this.bbtnRCSave.Name = "bbtnRCSave";
            this.bbtnRCSave.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCSave_ItemClick);
            // 
            // bbtnRCPrint
            // 
            this.bbtnRCPrint.Caption = "In (Ctrl P)";
            this.bbtnRCPrint.Id = 1;
            this.bbtnRCPrint.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P));
            this.bbtnRCPrint.Name = "bbtnRCPrint";
            this.bbtnRCPrint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnRCPrint_ItemClick);
            // 
            // bbtnAssignService
            // 
            this.bbtnAssignService.Caption = "Chỉ định (Ctrl D)";
            this.bbtnAssignService.Id = 2;
            this.bbtnAssignService.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D));
            this.bbtnAssignService.Name = "bbtnAssignService";
            this.bbtnAssignService.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnAssignService_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(1134, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 662);
            this.barDockControlBottom.Size = new System.Drawing.Size(1134, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 633);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(1134, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 633);
            // 
            // gridView1
            // 
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            // 
            // gridLookUpBloodRhCode
            // 
            this.gridLookUpBloodRhCode.Location = new System.Drawing.Point(681, 122);
            this.gridLookUpBloodRhCode.Margin = new System.Windows.Forms.Padding(2);
            this.gridLookUpBloodRhCode.MenuManager = this.barManager1;
            this.gridLookUpBloodRhCode.Name = "gridLookUpBloodRhCode";
            this.gridLookUpBloodRhCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.gridLookUpBloodRhCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gridLookUpBloodRhCode.Properties.NullText = "";
            this.gridLookUpBloodRhCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.gridLookUpBloodRhCode.Properties.View = this.gridLookUpEdit3View;
            this.gridLookUpBloodRhCode.Size = new System.Drawing.Size(50, 20);
            this.gridLookUpBloodRhCode.StyleController = this.layoutControl1;
            this.gridLookUpBloodRhCode.TabIndex = 15;
            this.gridLookUpBloodRhCode.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.gridLookUpBloodRhCode_Closed);
            this.gridLookUpBloodRhCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridLookUpBloodRhCode_PreviewKeyDown);
            // 
            // gridLookUpEdit3View
            // 
            this.gridLookUpEdit3View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit3View.Name = "gridLookUpEdit3View";
            this.gridLookUpEdit3View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit3View.OptionsView.ShowGroupPanel = false;
            // 
            // gridLookUpBloodAboCode
            // 
            this.gridLookUpBloodAboCode.Location = new System.Drawing.Point(532, 122);
            this.gridLookUpBloodAboCode.Margin = new System.Windows.Forms.Padding(2);
            this.gridLookUpBloodAboCode.MenuManager = this.barManager1;
            this.gridLookUpBloodAboCode.Name = "gridLookUpBloodAboCode";
            this.gridLookUpBloodAboCode.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.gridLookUpBloodAboCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gridLookUpBloodAboCode.Properties.NullText = "";
            this.gridLookUpBloodAboCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.gridLookUpBloodAboCode.Properties.View = this.gridLookUpEdit2View;
            this.gridLookUpBloodAboCode.Size = new System.Drawing.Size(50, 20);
            this.gridLookUpBloodAboCode.StyleController = this.layoutControl1;
            this.gridLookUpBloodAboCode.TabIndex = 14;
            this.gridLookUpBloodAboCode.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.gridLookUpBloodAboCode_Closed);
            this.gridLookUpBloodAboCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridLookUpBloodAboCode_PreviewKeyDown);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // gridLookUpVolume
            // 
            this.gridLookUpVolume.Location = new System.Drawing.Point(365, 122);
            this.gridLookUpVolume.Margin = new System.Windows.Forms.Padding(2);
            this.gridLookUpVolume.MenuManager = this.barManager1;
            this.gridLookUpVolume.Name = "gridLookUpVolume";
            this.gridLookUpVolume.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.gridLookUpVolume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.gridLookUpVolume.Properties.NullText = "";
            this.gridLookUpVolume.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.gridLookUpVolume.Properties.View = this.gridLookUpEdit1View;
            this.gridLookUpVolume.Size = new System.Drawing.Size(68, 20);
            this.gridLookUpVolume.StyleController = this.layoutControl1;
            this.gridLookUpVolume.TabIndex = 13;
            this.gridLookUpVolume.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.gridLookUpVolume_Closed);
            this.gridLookUpVolume.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.gridLookUpVolume_PreviewKeyDown);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // btnAssignService
            // 
            this.btnAssignService.Enabled = false;
            this.btnAssignService.Location = new System.Drawing.Point(919, 609);
            this.btnAssignService.Name = "btnAssignService";
            this.btnAssignService.Size = new System.Drawing.Size(85, 22);
            this.btnAssignService.StyleController = this.layoutControl1;
            this.btnAssignService.TabIndex = 12;
            this.btnAssignService.Text = "Chỉ định (Ctrl D)";
            this.btnAssignService.Click += new System.EventHandler(this.btnAssignService_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1076, 609);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(56, 22);
            this.btnPrint.StyleController = this.layoutControl1;
            this.btnPrint.TabIndex = 11;
            this.btnPrint.Text = "In (Ctrl P)";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(1008, 609);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(64, 22);
            this.btnSave.StyleController = this.layoutControl1;
            this.btnSave.TabIndex = 10;
            this.btnSave.Text = "Lưu (Ctrl S)";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // gridControlExpMestBlood
            // 
            this.gridControlExpMestBlood.Location = new System.Drawing.Point(733, 0);
            this.gridControlExpMestBlood.MainView = this.gridViewExMestBlood;
            this.gridControlExpMestBlood.Name = "gridControlExpMestBlood";
            this.gridControlExpMestBlood.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemBtnDeleteBlood,
            this.repositoryItemDtExpiredDate});
            this.gridControlExpMestBlood.Size = new System.Drawing.Size(401, 607);
            this.gridControlExpMestBlood.TabIndex = 9;
            this.gridControlExpMestBlood.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExMestBlood});
            // 
            // gridViewExMestBlood
            // 
            this.gridViewExMestBlood.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ExpMestBlood_Stt,
            this.gridColumn_ExpMestBlood_Delete,
            this.gridColumn_ExpMestBlood_BloodCode,
            this.gridColumn_ExpMestBlood_BloodTypeCode,
            this.gridColumn_ExpMestBlood_BloodTypeName,
            this.gridColumn_ExpMestBlood_ExpiredDate,
            this.gridColumn_ExpMestBlood_Volume,
            this.gridColumn_ExpMestBlood_BloodAbo,
            this.gridColumn_ExpMestBlood_BloodRhCode,
            this.gridColumn_ExpMestBlood_PatientTypeName});
            this.gridViewExMestBlood.GridControl = this.gridControlExpMestBlood;
            this.gridViewExMestBlood.Name = "gridViewExMestBlood";
            this.gridViewExMestBlood.OptionsView.ColumnAutoWidth = false;
            this.gridViewExMestBlood.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewExMestBlood.OptionsView.ShowGroupPanel = false;
            this.gridViewExMestBlood.OptionsView.ShowIndicator = false;
            this.gridViewExMestBlood.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewExMestBlood_CellValueChanged);
            this.gridViewExMestBlood.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExMestBlood_CustomUnboundColumnData);
            // 
            // gridColumn_ExpMestBlood_Stt
            // 
            this.gridColumn_ExpMestBlood_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestBlood_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_Stt.Caption = "STT";
            this.gridColumn_ExpMestBlood_Stt.FieldName = "STT";
            this.gridColumn_ExpMestBlood_Stt.Name = "gridColumn_ExpMestBlood_Stt";
            this.gridColumn_ExpMestBlood_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestBlood_Stt.Visible = true;
            this.gridColumn_ExpMestBlood_Stt.VisibleIndex = 0;
            this.gridColumn_ExpMestBlood_Stt.Width = 30;
            // 
            // gridColumn_ExpMestBlood_Delete
            // 
            this.gridColumn_ExpMestBlood_Delete.Caption = "Xóa";
            this.gridColumn_ExpMestBlood_Delete.ColumnEdit = this.repositoryItemBtnDeleteBlood;
            this.gridColumn_ExpMestBlood_Delete.FieldName = "DELETE";
            this.gridColumn_ExpMestBlood_Delete.Name = "gridColumn_ExpMestBlood_Delete";
            this.gridColumn_ExpMestBlood_Delete.OptionsColumn.ShowCaption = false;
            this.gridColumn_ExpMestBlood_Delete.Visible = true;
            this.gridColumn_ExpMestBlood_Delete.VisibleIndex = 1;
            this.gridColumn_ExpMestBlood_Delete.Width = 25;
            // 
            // repositoryItemBtnDeleteBlood
            // 
            this.repositoryItemBtnDeleteBlood.AutoHeight = false;
            this.repositoryItemBtnDeleteBlood.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnDeleteBlood.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "Xóa", null, null, true)});
            this.repositoryItemBtnDeleteBlood.Name = "repositoryItemBtnDeleteBlood";
            this.repositoryItemBtnDeleteBlood.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnDeleteBlood.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnDeleteBlood_ButtonClick);
            // 
            // gridColumn_ExpMestBlood_BloodCode
            // 
            this.gridColumn_ExpMestBlood_BloodCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_BloodCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_BloodCode.Caption = "Mã vạch";
            this.gridColumn_ExpMestBlood_BloodCode.FieldName = "BLOOD_CODE";
            this.gridColumn_ExpMestBlood_BloodCode.Name = "gridColumn_ExpMestBlood_BloodCode";
            this.gridColumn_ExpMestBlood_BloodCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_BloodCode.Visible = true;
            this.gridColumn_ExpMestBlood_BloodCode.VisibleIndex = 2;
            this.gridColumn_ExpMestBlood_BloodCode.Width = 90;
            // 
            // gridColumn_ExpMestBlood_BloodTypeCode
            // 
            this.gridColumn_ExpMestBlood_BloodTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_BloodTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_BloodTypeCode.Caption = "Mã";
            this.gridColumn_ExpMestBlood_BloodTypeCode.FieldName = "BLOOD_TYPE_CODE";
            this.gridColumn_ExpMestBlood_BloodTypeCode.Name = "gridColumn_ExpMestBlood_BloodTypeCode";
            this.gridColumn_ExpMestBlood_BloodTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_BloodTypeCode.Visible = true;
            this.gridColumn_ExpMestBlood_BloodTypeCode.VisibleIndex = 3;
            this.gridColumn_ExpMestBlood_BloodTypeCode.Width = 70;
            // 
            // gridColumn_ExpMestBlood_BloodTypeName
            // 
            this.gridColumn_ExpMestBlood_BloodTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_BloodTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_BloodTypeName.Caption = "Tên";
            this.gridColumn_ExpMestBlood_BloodTypeName.FieldName = "BLOOD_TYPE_NAME";
            this.gridColumn_ExpMestBlood_BloodTypeName.Name = "gridColumn_ExpMestBlood_BloodTypeName";
            this.gridColumn_ExpMestBlood_BloodTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_BloodTypeName.Visible = true;
            this.gridColumn_ExpMestBlood_BloodTypeName.VisibleIndex = 4;
            this.gridColumn_ExpMestBlood_BloodTypeName.Width = 140;
            // 
            // gridColumn_ExpMestBlood_ExpiredDate
            // 
            this.gridColumn_ExpMestBlood_ExpiredDate.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_ExpiredDate.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_ExpiredDate.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_ExpiredDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_ExpiredDate.Caption = "Hạn dùng";
            this.gridColumn_ExpMestBlood_ExpiredDate.ColumnEdit = this.repositoryItemDtExpiredDate;
            this.gridColumn_ExpMestBlood_ExpiredDate.FieldName = "EXPIRED_DATE_STR";
            this.gridColumn_ExpMestBlood_ExpiredDate.Name = "gridColumn_ExpMestBlood_ExpiredDate";
            this.gridColumn_ExpMestBlood_ExpiredDate.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestBlood_ExpiredDate.Visible = true;
            this.gridColumn_ExpMestBlood_ExpiredDate.VisibleIndex = 5;
            this.gridColumn_ExpMestBlood_ExpiredDate.Width = 90;
            // 
            // repositoryItemDtExpiredDate
            // 
            this.repositoryItemDtExpiredDate.AutoHeight = false;
            this.repositoryItemDtExpiredDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDtExpiredDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repositoryItemDtExpiredDate.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.repositoryItemDtExpiredDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemDtExpiredDate.EditFormat.FormatString = "dd/MM/yyyy";
            this.repositoryItemDtExpiredDate.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemDtExpiredDate.Mask.EditMask = "dd/MM/yyyy";
            this.repositoryItemDtExpiredDate.Name = "repositoryItemDtExpiredDate";
            // 
            // gridColumn_ExpMestBlood_Volume
            // 
            this.gridColumn_ExpMestBlood_Volume.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_Volume.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_Volume.Caption = "Dung tích";
            this.gridColumn_ExpMestBlood_Volume.FieldName = "VOLUME";
            this.gridColumn_ExpMestBlood_Volume.Name = "gridColumn_ExpMestBlood_Volume";
            this.gridColumn_ExpMestBlood_Volume.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_Volume.Visible = true;
            this.gridColumn_ExpMestBlood_Volume.VisibleIndex = 6;
            this.gridColumn_ExpMestBlood_Volume.Width = 55;
            // 
            // gridColumn_ExpMestBlood_BloodAbo
            // 
            this.gridColumn_ExpMestBlood_BloodAbo.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_BloodAbo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_BloodAbo.Caption = "ABO";
            this.gridColumn_ExpMestBlood_BloodAbo.FieldName = "BLOOD_ABO_CODE";
            this.gridColumn_ExpMestBlood_BloodAbo.Name = "gridColumn_ExpMestBlood_BloodAbo";
            this.gridColumn_ExpMestBlood_BloodAbo.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_BloodAbo.Visible = true;
            this.gridColumn_ExpMestBlood_BloodAbo.VisibleIndex = 7;
            this.gridColumn_ExpMestBlood_BloodAbo.Width = 40;
            // 
            // gridColumn_ExpMestBlood_BloodRhCode
            // 
            this.gridColumn_ExpMestBlood_BloodRhCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_BloodRhCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_BloodRhCode.Caption = "Rh";
            this.gridColumn_ExpMestBlood_BloodRhCode.FieldName = "BLOOD_RH_CODE";
            this.gridColumn_ExpMestBlood_BloodRhCode.Name = "gridColumn_ExpMestBlood_BloodRhCode";
            this.gridColumn_ExpMestBlood_BloodRhCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_BloodRhCode.Visible = true;
            this.gridColumn_ExpMestBlood_BloodRhCode.VisibleIndex = 8;
            this.gridColumn_ExpMestBlood_BloodRhCode.Width = 40;
            // 
            // gridColumn_ExpMestBlood_PatientTypeName
            // 
            this.gridColumn_ExpMestBlood_PatientTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlood_PatientTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlood_PatientTypeName.Caption = "ĐTTT";
            this.gridColumn_ExpMestBlood_PatientTypeName.FieldName = "PATIENT_TYPE_NAME";
            this.gridColumn_ExpMestBlood_PatientTypeName.Name = "gridColumn_ExpMestBlood_PatientTypeName";
            this.gridColumn_ExpMestBlood_PatientTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlood_PatientTypeName.Visible = true;
            this.gridColumn_ExpMestBlood_PatientTypeName.VisibleIndex = 9;
            // 
            // txtBloodCode
            // 
            this.txtBloodCode.Location = new System.Drawing.Point(286, 611);
            this.txtBloodCode.Name = "txtBloodCode";
            this.txtBloodCode.Properties.MaxLength = 20;
            this.txtBloodCode.Size = new System.Drawing.Size(184, 20);
            this.txtBloodCode.StyleController = this.layoutControl1;
            this.txtBloodCode.TabIndex = 8;
            this.txtBloodCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtBloodCode_PreviewKeyDown);
            // 
            // panelControlExpiredDate
            // 
            this.panelControlExpiredDate.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControlExpiredDate.Controls.Add(this.txtExpiredDate);
            this.panelControlExpiredDate.Controls.Add(this.dtExpiredDate);
            this.panelControlExpiredDate.Location = new System.Drawing.Point(97, 611);
            this.panelControlExpiredDate.Name = "panelControlExpiredDate";
            this.panelControlExpiredDate.Size = new System.Drawing.Size(90, 20);
            this.panelControlExpiredDate.TabIndex = 7;
            // 
            // txtExpiredDate
            // 
            this.txtExpiredDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtExpiredDate.Location = new System.Drawing.Point(0, 0);
            this.txtExpiredDate.MenuManager = this.barManager1;
            this.txtExpiredDate.Name = "txtExpiredDate";
            this.txtExpiredDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Down)});
            this.txtExpiredDate.Properties.Mask.EditMask = "\\d{2}/\\d{2}/\\d{4}";
            this.txtExpiredDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
            this.txtExpiredDate.Properties.Mask.UseMaskAsDisplayFormat = true;
            this.txtExpiredDate.Size = new System.Drawing.Size(90, 20);
            this.txtExpiredDate.TabIndex = 1;
            this.txtExpiredDate.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtExpiredDate_ButtonClick);
            this.txtExpiredDate.InvalidValue += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.txtExpiredDate_InvalidValue);
            this.txtExpiredDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExpiredDate_KeyDown);
            this.txtExpiredDate.Leave += new System.EventHandler(this.txtExpiredDate_Leave);
            this.txtExpiredDate.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExpiredDate_PreviewKeyDown);
            this.txtExpiredDate.Validating += new System.ComponentModel.CancelEventHandler(this.txtExpiredDate_Validating);
            // 
            // dtExpiredDate
            // 
            this.dtExpiredDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtExpiredDate.EditValue = null;
            this.dtExpiredDate.Location = new System.Drawing.Point(0, 0);
            this.dtExpiredDate.Name = "dtExpiredDate";
            this.dtExpiredDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiredDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.dtExpiredDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
            this.dtExpiredDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpiredDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
            this.dtExpiredDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.dtExpiredDate.Size = new System.Drawing.Size(90, 20);
            this.dtExpiredDate.TabIndex = 0;
            this.dtExpiredDate.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.dtExpiredDate_Closed);
            this.dtExpiredDate.EditValueChanged += new System.EventHandler(this.dtExpiredDate_EditValueChanged);
            this.dtExpiredDate.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dtExpiredDate_KeyDown);
            // 
            // gridControlBlood
            // 
            this.gridControlBlood.Location = new System.Drawing.Point(0, 144);
            this.gridControlBlood.MainView = this.gridViewBlood;
            this.gridControlBlood.Name = "gridControlBlood";
            this.gridControlBlood.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemBtnAddBlood});
            this.gridControlBlood.Size = new System.Drawing.Size(733, 465);
            this.gridControlBlood.TabIndex = 6;
            this.gridControlBlood.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewBlood});
            // 
            // gridViewBlood
            // 
            this.gridViewBlood.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_Blood_Stt,
            this.gridColumn_Blood_Add,
            this.gridColumn_Blood_BloodCode,
            this.gridColumn_Blood_BloodTypeName,
            this.gridColumn_Blood_Volume,
            this.gridColumn_Blood_BloodAbo,
            this.gridColumn_Blood_BloodRh});
            this.gridViewBlood.GridControl = this.gridControlBlood;
            this.gridViewBlood.Name = "gridViewBlood";
            this.gridViewBlood.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.gridViewBlood.OptionsView.ShowGroupPanel = false;
            this.gridViewBlood.OptionsView.ShowIndicator = false;
            this.gridViewBlood.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewBlood_CustomUnboundColumnData);
            // 
            // gridColumn_Blood_Stt
            // 
            this.gridColumn_Blood_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_Blood_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_Stt.Caption = "STT";
            this.gridColumn_Blood_Stt.FieldName = "STT";
            this.gridColumn_Blood_Stt.Name = "gridColumn_Blood_Stt";
            this.gridColumn_Blood_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_Blood_Stt.Visible = true;
            this.gridColumn_Blood_Stt.VisibleIndex = 0;
            this.gridColumn_Blood_Stt.Width = 35;
            // 
            // gridColumn_Blood_Add
            // 
            this.gridColumn_Blood_Add.Caption = "Thêm";
            this.gridColumn_Blood_Add.ColumnEdit = this.repositoryItemBtnAddBlood;
            this.gridColumn_Blood_Add.FieldName = "ADD";
            this.gridColumn_Blood_Add.Name = "gridColumn_Blood_Add";
            this.gridColumn_Blood_Add.OptionsColumn.ShowCaption = false;
            this.gridColumn_Blood_Add.Visible = true;
            this.gridColumn_Blood_Add.VisibleIndex = 1;
            this.gridColumn_Blood_Add.Width = 25;
            // 
            // repositoryItemBtnAddBlood
            // 
            this.repositoryItemBtnAddBlood.AutoHeight = false;
            this.repositoryItemBtnAddBlood.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, ((System.Drawing.Image)(resources.GetObject("repositoryItemBtnAddBlood.Buttons"))), new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "Thêm", null, null, true)});
            this.repositoryItemBtnAddBlood.Name = "repositoryItemBtnAddBlood";
            this.repositoryItemBtnAddBlood.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
            this.repositoryItemBtnAddBlood.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemBtnAddBlood_ButtonClick);
            // 
            // gridColumn_Blood_BloodCode
            // 
            this.gridColumn_Blood_BloodCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_BloodCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_BloodCode.Caption = "Mã vạch";
            this.gridColumn_Blood_BloodCode.FieldName = "BLOOD_CODE";
            this.gridColumn_Blood_BloodCode.Name = "gridColumn_Blood_BloodCode";
            this.gridColumn_Blood_BloodCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_BloodCode.Visible = true;
            this.gridColumn_Blood_BloodCode.VisibleIndex = 2;
            this.gridColumn_Blood_BloodCode.Width = 100;
            // 
            // gridColumn_Blood_BloodTypeName
            // 
            this.gridColumn_Blood_BloodTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_BloodTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_BloodTypeName.Caption = "Loại máu";
            this.gridColumn_Blood_BloodTypeName.FieldName = "BLOOD_TYPE_NAME";
            this.gridColumn_Blood_BloodTypeName.Name = "gridColumn_Blood_BloodTypeName";
            this.gridColumn_Blood_BloodTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_BloodTypeName.Visible = true;
            this.gridColumn_Blood_BloodTypeName.VisibleIndex = 3;
            this.gridColumn_Blood_BloodTypeName.Width = 270;
            // 
            // gridColumn_Blood_Volume
            // 
            this.gridColumn_Blood_Volume.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_Volume.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_Volume.Caption = "Dung tích";
            this.gridColumn_Blood_Volume.FieldName = "VOLUME";
            this.gridColumn_Blood_Volume.Name = "gridColumn_Blood_Volume";
            this.gridColumn_Blood_Volume.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_Volume.Visible = true;
            this.gridColumn_Blood_Volume.VisibleIndex = 4;
            // 
            // gridColumn_Blood_BloodAbo
            // 
            this.gridColumn_Blood_BloodAbo.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_BloodAbo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_BloodAbo.Caption = "ABO";
            this.gridColumn_Blood_BloodAbo.FieldName = "BLOOD_ABO_CODE";
            this.gridColumn_Blood_BloodAbo.Name = "gridColumn_Blood_BloodAbo";
            this.gridColumn_Blood_BloodAbo.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_BloodAbo.Visible = true;
            this.gridColumn_Blood_BloodAbo.VisibleIndex = 5;
            // 
            // gridColumn_Blood_BloodRh
            // 
            this.gridColumn_Blood_BloodRh.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_Blood_BloodRh.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_Blood_BloodRh.Caption = "Rh";
            this.gridColumn_Blood_BloodRh.FieldName = "BLOOD_RH_CODE";
            this.gridColumn_Blood_BloodRh.Name = "gridColumn_Blood_BloodRh";
            this.gridColumn_Blood_BloodRh.OptionsColumn.AllowEdit = false;
            this.gridColumn_Blood_BloodRh.Visible = true;
            this.gridColumn_Blood_BloodRh.VisibleIndex = 6;
            // 
            // lblBloodTypeInfo
            // 
            this.lblBloodTypeInfo.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBloodTypeInfo.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblBloodTypeInfo.Location = new System.Drawing.Point(97, 98);
            this.lblBloodTypeInfo.Name = "lblBloodTypeInfo";
            this.lblBloodTypeInfo.Size = new System.Drawing.Size(634, 20);
            this.lblBloodTypeInfo.StyleController = this.layoutControl1;
            this.lblBloodTypeInfo.TabIndex = 5;
            // 
            // gridControlExpMestBlty
            // 
            this.gridControlExpMestBlty.Location = new System.Drawing.Point(0, 0);
            this.gridControlExpMestBlty.MainView = this.gridViewExpMestBlty;
            this.gridControlExpMestBlty.Name = "gridControlExpMestBlty";
            this.gridControlExpMestBlty.Size = new System.Drawing.Size(733, 96);
            this.gridControlExpMestBlty.TabIndex = 4;
            this.gridControlExpMestBlty.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridViewExpMestBlty});
            // 
            // gridViewExpMestBlty
            // 
            this.gridViewExpMestBlty.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn_ExpMestBlty_Stt,
            this.gridColumn_ExpMestBlty_BloodTypeCode,
            this.gridColumn_ExpMestBlty_BloodTypeName,
            this.gridColumn_ExpMestBlty_Volume,
            this.gridColumn_ExpMestBlty_BloodAbo,
            this.gridColumn_ExpMestBlty_BloodRh,
            this.gridColumn_ExpMestBlty_Amount});
            this.gridViewExpMestBlty.GridControl = this.gridControlExpMestBlty;
            this.gridViewExpMestBlty.Name = "gridViewExpMestBlty";
            this.gridViewExpMestBlty.OptionsView.ShowDetailButtons = false;
            this.gridViewExpMestBlty.OptionsView.ShowGroupPanel = false;
            this.gridViewExpMestBlty.OptionsView.ShowIndicator = false;
            this.gridViewExpMestBlty.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gridViewExpMestBlty_CustomUnboundColumnData);
            this.gridViewExpMestBlty.Click += new System.EventHandler(this.gridViewExpMestBlty_Click);
            // 
            // gridColumn_ExpMestBlty_Stt
            // 
            this.gridColumn_ExpMestBlty_Stt.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Stt.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestBlty_Stt.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Stt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_Stt.Caption = "STT";
            this.gridColumn_ExpMestBlty_Stt.FieldName = "STT";
            this.gridColumn_ExpMestBlty_Stt.Name = "gridColumn_ExpMestBlty_Stt";
            this.gridColumn_ExpMestBlty_Stt.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_Stt.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn_ExpMestBlty_Stt.Visible = true;
            this.gridColumn_ExpMestBlty_Stt.VisibleIndex = 0;
            this.gridColumn_ExpMestBlty_Stt.Width = 30;
            // 
            // gridColumn_ExpMestBlty_BloodTypeCode
            // 
            this.gridColumn_ExpMestBlty_BloodTypeCode.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_BloodTypeCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_BloodTypeCode.Caption = "Mã";
            this.gridColumn_ExpMestBlty_BloodTypeCode.FieldName = "BLOOD_TYPE_CODE";
            this.gridColumn_ExpMestBlty_BloodTypeCode.Name = "gridColumn_ExpMestBlty_BloodTypeCode";
            this.gridColumn_ExpMestBlty_BloodTypeCode.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_BloodTypeCode.Visible = true;
            this.gridColumn_ExpMestBlty_BloodTypeCode.VisibleIndex = 1;
            this.gridColumn_ExpMestBlty_BloodTypeCode.Width = 100;
            // 
            // gridColumn_ExpMestBlty_BloodTypeName
            // 
            this.gridColumn_ExpMestBlty_BloodTypeName.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_BloodTypeName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_BloodTypeName.Caption = "Tên";
            this.gridColumn_ExpMestBlty_BloodTypeName.FieldName = "BLOOD_TYPE_NAME";
            this.gridColumn_ExpMestBlty_BloodTypeName.Name = "gridColumn_ExpMestBlty_BloodTypeName";
            this.gridColumn_ExpMestBlty_BloodTypeName.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_BloodTypeName.Visible = true;
            this.gridColumn_ExpMestBlty_BloodTypeName.VisibleIndex = 2;
            this.gridColumn_ExpMestBlty_BloodTypeName.Width = 260;
            // 
            // gridColumn_ExpMestBlty_Volume
            // 
            this.gridColumn_ExpMestBlty_Volume.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Volume.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestBlty_Volume.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Volume.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_Volume.Caption = "Dung tích";
            this.gridColumn_ExpMestBlty_Volume.FieldName = "VOLUME";
            this.gridColumn_ExpMestBlty_Volume.Name = "gridColumn_ExpMestBlty_Volume";
            this.gridColumn_ExpMestBlty_Volume.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_Volume.Visible = true;
            this.gridColumn_ExpMestBlty_Volume.VisibleIndex = 3;
            this.gridColumn_ExpMestBlty_Volume.Width = 65;
            // 
            // gridColumn_ExpMestBlty_BloodAbo
            // 
            this.gridColumn_ExpMestBlty_BloodAbo.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_BloodAbo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_BloodAbo.Caption = "ABO";
            this.gridColumn_ExpMestBlty_BloodAbo.FieldName = "BLOOD_ABO_CODE";
            this.gridColumn_ExpMestBlty_BloodAbo.Name = "gridColumn_ExpMestBlty_BloodAbo";
            this.gridColumn_ExpMestBlty_BloodAbo.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_BloodAbo.Visible = true;
            this.gridColumn_ExpMestBlty_BloodAbo.VisibleIndex = 4;
            this.gridColumn_ExpMestBlty_BloodAbo.Width = 65;
            // 
            // gridColumn_ExpMestBlty_BloodRh
            // 
            this.gridColumn_ExpMestBlty_BloodRh.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_BloodRh.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_BloodRh.Caption = "Rh";
            this.gridColumn_ExpMestBlty_BloodRh.FieldName = "BLOOD_RH_CODE";
            this.gridColumn_ExpMestBlty_BloodRh.Name = "gridColumn_ExpMestBlty_BloodRh";
            this.gridColumn_ExpMestBlty_BloodRh.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_BloodRh.Visible = true;
            this.gridColumn_ExpMestBlty_BloodRh.VisibleIndex = 5;
            this.gridColumn_ExpMestBlty_BloodRh.Width = 65;
            // 
            // gridColumn_ExpMestBlty_Amount
            // 
            this.gridColumn_ExpMestBlty_Amount.AppearanceCell.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Amount.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.gridColumn_ExpMestBlty_Amount.AppearanceHeader.Options.UseTextOptions = true;
            this.gridColumn_ExpMestBlty_Amount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gridColumn_ExpMestBlty_Amount.Caption = "Số  lượng";
            this.gridColumn_ExpMestBlty_Amount.DisplayFormat.FormatString = "#,##0.00";
            this.gridColumn_ExpMestBlty_Amount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.gridColumn_ExpMestBlty_Amount.FieldName = "AMOUNT";
            this.gridColumn_ExpMestBlty_Amount.Name = "gridColumn_ExpMestBlty_Amount";
            this.gridColumn_ExpMestBlty_Amount.OptionsColumn.AllowEdit = false;
            this.gridColumn_ExpMestBlty_Amount.Visible = true;
            this.gridColumn_ExpMestBlty_Amount.VisibleIndex = 6;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutBloodTypeInfo,
            this.layoutControlItem3,
            this.layoutExpiredDate,
            this.layoutBloodCode,
            this.layoutControlItem6,
            this.layoutControlItem7,
            this.layoutControlItem8,
            this.emptySpaceItem1,
            this.emptySpaceItem2,
            this.layoutControlItem2,
            this.layoutControlItem4,
            this.layoutControlItem5,
            this.layoutControlItem9,
            this.layoutControlItem10});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(1134, 633);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.gridControlExpMestBlty;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(733, 96);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutBloodTypeInfo
            // 
            this.layoutBloodTypeInfo.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.layoutBloodTypeInfo.AppearanceItemCaption.Options.UseFont = true;
            this.layoutBloodTypeInfo.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBloodTypeInfo.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBloodTypeInfo.Control = this.lblBloodTypeInfo;
            this.layoutBloodTypeInfo.Location = new System.Drawing.Point(0, 96);
            this.layoutBloodTypeInfo.Name = "layoutBloodTypeInfo";
            this.layoutBloodTypeInfo.Size = new System.Drawing.Size(733, 24);
            this.layoutBloodTypeInfo.Text = "Thực xuất cho:";
            this.layoutBloodTypeInfo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBloodTypeInfo.TextSize = new System.Drawing.Size(90, 20);
            this.layoutBloodTypeInfo.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.gridControlBlood;
            this.layoutControlItem3.Location = new System.Drawing.Point(0, 144);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem3.Size = new System.Drawing.Size(733, 465);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutExpiredDate
            // 
            this.layoutExpiredDate.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutExpiredDate.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutExpiredDate.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutExpiredDate.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutExpiredDate.Control = this.panelControlExpiredDate;
            this.layoutExpiredDate.Location = new System.Drawing.Point(0, 609);
            this.layoutExpiredDate.Name = "layoutExpiredDate";
            this.layoutExpiredDate.Size = new System.Drawing.Size(189, 24);
            this.layoutExpiredDate.Text = "Hạn dùng:";
            this.layoutExpiredDate.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutExpiredDate.TextSize = new System.Drawing.Size(90, 0);
            this.layoutExpiredDate.TextToControlDistance = 5;
            // 
            // layoutBloodCode
            // 
            this.layoutBloodCode.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
            this.layoutBloodCode.AppearanceItemCaption.Options.UseForeColor = true;
            this.layoutBloodCode.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutBloodCode.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutBloodCode.Control = this.txtBloodCode;
            this.layoutBloodCode.Location = new System.Drawing.Point(189, 609);
            this.layoutBloodCode.Name = "layoutBloodCode";
            this.layoutBloodCode.Size = new System.Drawing.Size(283, 24);
            this.layoutBloodCode.Text = "Mã vạch:";
            this.layoutBloodCode.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutBloodCode.TextSize = new System.Drawing.Size(90, 13);
            this.layoutBloodCode.TextToControlDistance = 5;
            // 
            // layoutControlItem6
            // 
            this.layoutControlItem6.Control = this.gridControlExpMestBlood;
            this.layoutControlItem6.Location = new System.Drawing.Point(733, 0);
            this.layoutControlItem6.Name = "layoutControlItem6";
            this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem6.Size = new System.Drawing.Size(401, 607);
            this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem6.TextVisible = false;
            // 
            // layoutControlItem7
            // 
            this.layoutControlItem7.Control = this.btnSave;
            this.layoutControlItem7.Location = new System.Drawing.Point(1006, 607);
            this.layoutControlItem7.Name = "layoutControlItem7";
            this.layoutControlItem7.Size = new System.Drawing.Size(68, 26);
            this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem7.TextVisible = false;
            // 
            // layoutControlItem8
            // 
            this.layoutControlItem8.Control = this.btnPrint;
            this.layoutControlItem8.Location = new System.Drawing.Point(1074, 607);
            this.layoutControlItem8.Name = "layoutControlItem8";
            this.layoutControlItem8.Size = new System.Drawing.Size(60, 26);
            this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem8.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(472, 609);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(261, 24);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(733, 607);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(184, 26);
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.btnAssignService;
            this.layoutControlItem2.Location = new System.Drawing.Point(917, 607);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem4.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem4.Control = this.gridLookUpVolume;
            this.layoutControlItem4.Location = new System.Drawing.Point(268, 120);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(167, 24);
            this.layoutControlItem4.Text = "Dung tích:";
            this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem4.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem4.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem5.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem5.Control = this.gridLookUpBloodAboCode;
            this.layoutControlItem5.Location = new System.Drawing.Point(435, 120);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(149, 24);
            this.layoutControlItem5.Text = "Abo:";
            this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem5.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem5.TextToControlDistance = 5;
            // 
            // layoutControlItem9
            // 
            this.layoutControlItem9.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem9.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem9.Control = this.gridLookUpBloodRhCode;
            this.layoutControlItem9.Location = new System.Drawing.Point(584, 120);
            this.layoutControlItem9.Name = "layoutControlItem9";
            this.layoutControlItem9.Size = new System.Drawing.Size(149, 24);
            this.layoutControlItem9.Text = "Rh:";
            this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem9.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem9.TextToControlDistance = 5;
            // 
            // layoutControlItem10
            // 
            this.layoutControlItem10.AppearanceItemCaption.Options.UseTextOptions = true;
            this.layoutControlItem10.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.layoutControlItem10.Control = this.cboBloodType;
            this.layoutControlItem10.Location = new System.Drawing.Point(0, 120);
            this.layoutControlItem10.Name = "layoutControlItem10";
            this.layoutControlItem10.Size = new System.Drawing.Size(268, 24);
            this.layoutControlItem10.Text = "Loại máu:";
            this.layoutControlItem10.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem10.TextSize = new System.Drawing.Size(90, 20);
            this.layoutControlItem10.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxValidationProvider2
            // 
            this.dxValidationProvider2.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider2_ValidationFailed);
            // 
            // frmExpMestBlood
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 662);
            this.Controls.Add(this.layoutControl1);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "frmExpMestBlood";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thực xuất máu";
            this.Load += new System.EventHandler(this.frmExpMestBlood_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboBloodType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpBloodRhCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit3View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpBloodAboCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpVolume.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExMestBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnDeleteBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDtExpiredDate.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemDtExpiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtBloodCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControlExpiredDate)).EndInit();
            this.panelControlExpiredDate.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtExpiredDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiredDate.Properties.CalendarTimeProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtExpiredDate.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemBtnAddBlood)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridControlExpMestBlty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridViewExpMestBlty)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBloodTypeInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutExpiredDate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutBloodCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnPrint;
        private DevExpress.XtraEditors.SimpleButton btnSave;
        private DevExpress.XtraGrid.GridControl gridControlExpMestBlood;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExMestBlood;
        private DevExpress.XtraEditors.TextEdit txtBloodCode;
        private DevExpress.XtraEditors.PanelControl panelControlExpiredDate;
        private DevExpress.XtraGrid.GridControl gridControlBlood;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewBlood;
        private DevExpress.XtraEditors.LabelControl lblBloodTypeInfo;
        private DevExpress.XtraGrid.GridControl gridControlExpMestBlty;
        private DevExpress.XtraGrid.Views.Grid.GridView gridViewExpMestBlty;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutBloodTypeInfo;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutExpiredDate;
        private DevExpress.XtraLayout.LayoutControlItem layoutBloodCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem6;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem7;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_BloodTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_BloodTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_Volume;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_BloodAbo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_BloodRh;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlty_Amount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_Add;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_BloodCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_BloodTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_BloodAbo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_BloodRh;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnAddBlood;
        private DevExpress.XtraEditors.DateEdit dtExpiredDate;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_Blood_Volume;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_Stt;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_Delete;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemBtnDeleteBlood;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_BloodCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_BloodTypeCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_BloodTypeName;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_ExpiredDate;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_Volume;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_BloodAbo;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_BloodRhCode;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn_ExpMestBlood_PatientTypeName;
        private DevExpress.XtraBars.BarManager barManager1;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarButtonItem bbtnRCSave;
        private DevExpress.XtraBars.BarButtonItem bbtnRCPrint;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider2;
        private DevExpress.XtraEditors.ButtonEdit txtExpiredDate;
        private DevExpress.XtraEditors.Repository.RepositoryItemDateEdit repositoryItemDtExpiredDate;
        private DevExpress.XtraEditors.SimpleButton btnAssignService;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraBars.BarButtonItem bbtnAssignService;
        private DevExpress.XtraEditors.GridLookUpEdit gridLookUpBloodRhCode;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit3View;
        private DevExpress.XtraEditors.GridLookUpEdit gridLookUpBloodAboCode;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.GridLookUpEdit gridLookUpVolume;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem9;
        private DevExpress.XtraEditors.GridLookUpEdit cboBloodType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem10;
    }
}
