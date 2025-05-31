using System;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.SuggestPrescriptionsInfo
{
    partial class frmSuggestPrescriptionsInfo
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.lblEnd = new DevExpress.XtraEditors.LabelControl();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.btnOk = new DevExpress.XtraEditors.SimpleButton();
            this.grcService = new DevExpress.XtraGrid.GridControl();
            this.grvService = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repSpnAmount = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repMmEdit = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repChkExpend = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repBtnNote = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
            this.lblNote = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grcService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repSpnAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMmEdit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repChkExpend)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtnNote)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.lblEnd);
            this.layoutControl1.Controls.Add(this.btnCancel);
            this.layoutControl1.Controls.Add(this.btnOk);
            this.layoutControl1.Controls.Add(this.grcService);
            this.layoutControl1.Controls.Add(this.lblNote);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(900, 480);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // lblEnd
            // 
            this.lblEnd.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEnd.Appearance.ForeColor = System.Drawing.Color.Maroon;
            this.lblEnd.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblEnd.Location = new System.Drawing.Point(2, 2);
            this.lblEnd.Name = "lblEnd";
            this.lblEnd.Size = new System.Drawing.Size(896, 1);
            this.lblEnd.StyleController = this.layoutControl1;
            this.lblEnd.TabIndex = 8;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(735, 456);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(163, 22);
            this.btnCancel.StyleController = this.layoutControl1;
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Bỏ qua";
            this.btnCancel.Click += new System.EventHandler(this.btnClosefrm_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(554, 456);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(177, 22);
            this.btnOk.StyleController = this.layoutControl1;
            this.btnOk.TabIndex = 6;
            this.btnOk.Text = "Đồng ý";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // grcService
            // 
            this.grcService.Location = new System.Drawing.Point(2, 57);
            this.grcService.MainView = this.grvService;
            this.grcService.Margin = new System.Windows.Forms.Padding(3, 5, 3, 3);
            this.grcService.Name = "grcService";
            this.grcService.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repBtnNote,
            this.repSpnAmount,
            this.repChkExpend,
            this.repMmEdit});
            this.grcService.Size = new System.Drawing.Size(896, 395);
            this.grcService.TabIndex = 5;
            this.grcService.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grvService});
            // 
            // grvService
            // 
            this.grvService.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn11,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10});
            this.grvService.GridControl = this.grcService;
            this.grvService.Name = "grvService";
            this.grvService.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
            this.grvService.OptionsSelection.MultiSelect = true;
            this.grvService.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
            this.grvService.OptionsView.ColumnAutoWidth = false;
            this.grvService.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
            this.grvService.OptionsView.ShowDetailButtons = false;
            this.grvService.OptionsView.ShowGroupPanel = false;
            this.grvService.OptionsView.ShowIndicator = false;
            this.grvService.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.grvService_CustomUnboundColumnData);
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Mã thuốc - vật tư";
            this.gridColumn1.FieldName = "MEDICINE_TYPE_CODE";
            this.gridColumn1.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.OptionsColumn.AllowEdit = false;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 1;
            this.gridColumn1.Width = 100;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Tên thuốc - vật tư";
            this.gridColumn2.FieldName = "MEDICINE_TYPE_NAME";
            this.gridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 2;
            this.gridColumn2.Width = 200;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Số lượng";
            this.gridColumn3.ColumnEdit = this.repSpnAmount;
            this.gridColumn3.FieldName = "AMOUNT";
            this.gridColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
            this.gridColumn3.MaxWidth = 55;
            this.gridColumn3.MinWidth = 55;
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 3;
            this.gridColumn3.Width = 55;
            // 
            // repSpnAmount
            // 
            this.repSpnAmount.AutoHeight = false;
            this.repSpnAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repSpnAmount.MaxLength = 9999;
            this.repSpnAmount.MaxValue = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.repSpnAmount.Name = "repSpnAmount";
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Cách dùng";
            this.gridColumn4.ColumnEdit = this.repMmEdit;
            this.gridColumn4.FieldName = "HTU_CODE_NAME";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 4;
            this.gridColumn4.Width = 125;
            // 
            // repMmEdit
            // 
            this.repMmEdit.AutoHeight = false;
            this.repMmEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repMmEdit.Name = "repMmEdit";
            this.repMmEdit.PopupFormMinSize = new System.Drawing.Size(300, 100);
            this.repMmEdit.ShowIcon = false;
            this.repMmEdit.Popup += new System.EventHandler(this.repMmEdit_Popup);
            this.repMmEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repMmEdit_ButtonClick);
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "ĐVT";
            this.gridColumn5.FieldName = "SERVICE_UNIT_CODE_NAME";
            this.gridColumn5.MinWidth = 10;
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.ToolTip = "Đơn vị tính";
            this.gridColumn5.UnboundType = DevExpress.Data.UnboundColumnType.Object;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 5;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Hao phí";
            this.gridColumn11.ColumnEdit = this.repChkExpend;
            this.gridColumn11.FieldName = "IsExpend";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 6;
            this.gridColumn11.Width = 50;
            // 
            // repChkExpend
            // 
            this.repChkExpend.AutoHeight = false;
            this.repChkExpend.Name = "repChkExpend";
            this.repChkExpend.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Sáng";
            this.gridColumn6.ColumnEdit = this.repSpnAmount;
            this.gridColumn6.FieldName = "Morning";
            this.gridColumn6.MaxWidth = 40;
            this.gridColumn6.MinWidth = 50;
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 7;
            this.gridColumn6.Width = 50;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "Trưa";
            this.gridColumn7.ColumnEdit = this.repSpnAmount;
            this.gridColumn7.FieldName = "Noon";
            this.gridColumn7.MaxWidth = 40;
            this.gridColumn7.MinWidth = 50;
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 8;
            this.gridColumn7.Width = 50;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Chiều";
            this.gridColumn8.ColumnEdit = this.repSpnAmount;
            this.gridColumn8.FieldName = "Afternoon";
            this.gridColumn8.MaxWidth = 40;
            this.gridColumn8.MinWidth = 50;
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 9;
            this.gridColumn8.Width = 50;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Tối";
            this.gridColumn9.ColumnEdit = this.repSpnAmount;
            this.gridColumn9.FieldName = "Evening";
            this.gridColumn9.MaxWidth = 40;
            this.gridColumn9.MinWidth = 50;
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 10;
            this.gridColumn9.Width = 50;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "HDSD";
            this.gridColumn10.ColumnEdit = this.repMmEdit;
            this.gridColumn10.FieldName = "TUTORIAL";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.ToolTip = "Hướng dẫn sử dụng";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 11;
            this.gridColumn10.Width = 200;
            // 
            // repBtnNote
            // 
            this.repBtnNote.AutoHeight = false;
            this.repBtnNote.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.repBtnNote.Name = "repBtnNote";
            // 
            // lblNote
            // 
            this.lblNote.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblNote.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
            this.lblNote.LineVisible = true;
            this.lblNote.Location = new System.Drawing.Point(2, 32);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(896, 1);
            this.lblNote.StyleController = this.layoutControl1;
            this.lblNote.TabIndex = 4;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.layoutControlItem3,
            this.layoutControlItem4,
            this.emptySpaceItem1,
            this.layoutControlItem5});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(900, 480);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem1.Control = this.lblNote;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 5);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(900, 30);
            this.layoutControlItem1.Text = "Giải thích:";
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem1.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
            this.layoutControlItem2.Control = this.grcService;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 35);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(900, 419);
            this.layoutControlItem2.Text = "Đơn thuốc gợi ý:";
            this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(115, 17);
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.btnOk;
            this.layoutControlItem3.Location = new System.Drawing.Point(552, 454);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Size = new System.Drawing.Size(181, 26);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // layoutControlItem4
            // 
            this.layoutControlItem4.Control = this.btnCancel;
            this.layoutControlItem4.Location = new System.Drawing.Point(733, 454);
            this.layoutControlItem4.Name = "layoutControlItem4";
            this.layoutControlItem4.Size = new System.Drawing.Size(167, 26);
            this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem4.TextVisible = false;
            // 
            // emptySpaceItem1
            // 
            this.emptySpaceItem1.AllowHotTrack = false;
            this.emptySpaceItem1.Location = new System.Drawing.Point(0, 454);
            this.emptySpaceItem1.Name = "emptySpaceItem1";
            this.emptySpaceItem1.Size = new System.Drawing.Size(552, 26);
            this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.lblEnd;
            this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Size = new System.Drawing.Size(900, 5);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // frmSuggestPrescriptionsInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 480);
            this.Controls.Add(this.layoutControl1);
            this.Name = "frmSuggestPrescriptionsInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gợi ý kê đơn (AI)";
            this.Load += new System.EventHandler(this.frmSuggestPrescriptionsInfo_LoadAsync);
            this.Controls.SetChildIndex(this.layoutControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grcService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repSpnAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repMmEdit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repChkExpend)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repBtnNote)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraGrid.GridControl grcService;
        private DevExpress.XtraGrid.Views.Grid.GridView grvService;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit repSpnAmount;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repBtnNote;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repChkExpend;
        private DevExpress.XtraEditors.LabelControl lblNote;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.SimpleButton btnOk;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem4;
        private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
        private DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit repMmEdit;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraEditors.LabelControl lblEnd;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
    }
}