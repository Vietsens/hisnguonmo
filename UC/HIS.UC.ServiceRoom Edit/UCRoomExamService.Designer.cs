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
namespace HIS.UC.ServiceRoom
{
    partial class UCRoomExamService
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCRoomExamService));
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtRoomCode = new DevExpress.XtraEditors.TextEdit();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.cboExamService = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit2View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtExamServiceCode = new DevExpress.XtraEditors.TextEdit();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciBtnDelete = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciExamService = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
            this.cboRoom = new HIS.UC.ServiceRoom.CustomControl.GridLookupEditCustom();
            this.gridLookUpEdit1View = new HIS.UC.ServiceRoom.CustomControl.GridControlCustom();
            this.lciCboRoom = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExamService.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExamServiceCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExamService)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboRoom)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtRoomCode);
            this.layoutControl1.Controls.Add(this.btnDelete);
            this.layoutControl1.Controls.Add(this.cboExamService);
            this.layoutControl1.Controls.Add(this.txtExamServiceCode);
            this.layoutControl1.Controls.Add(this.cboRoom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(404, 52);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtRoomCode
            // 
            this.txtRoomCode.Location = new System.Drawing.Point(77, 2);
            this.txtRoomCode.Name = "txtRoomCode";
            this.txtRoomCode.Size = new System.Drawing.Size(59, 20);
            this.txtRoomCode.StyleController = this.layoutControl1;
            this.txtRoomCode.TabIndex = 3;
            this.txtRoomCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtRoomCode_PreviewKeyDown);
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDelete.Location = new System.Drawing.Point(372, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(30, 22);
            this.btnDelete.StyleController = this.layoutControl1;
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cboExamService
            // 
            this.cboExamService.Location = new System.Drawing.Point(136, 26);
            this.cboExamService.Name = "cboExamService";
            this.cboExamService.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboExamService.Properties.AutoComplete = false;
            this.cboExamService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboExamService.Properties.NullText = "";
            this.cboExamService.Properties.View = this.gridLookUpEdit2View;
            this.cboExamService.Size = new System.Drawing.Size(232, 20);
            this.cboExamService.StyleController = this.layoutControl1;
            this.cboExamService.TabIndex = 6;
            this.cboExamService.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExamService_Closed);
            this.cboExamService.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExamService_ButtonClick);
            this.cboExamService.EditValueChanged += new System.EventHandler(this.cboExamService_EditValueChanged);
            this.cboExamService.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboExamService_KeyUp);
            // 
            // gridLookUpEdit2View
            // 
            this.gridLookUpEdit2View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit2View.Name = "gridLookUpEdit2View";
            this.gridLookUpEdit2View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit2View.OptionsView.ShowGroupPanel = false;
            // 
            // txtExamServiceCode
            // 
            this.txtExamServiceCode.Location = new System.Drawing.Point(77, 26);
            this.txtExamServiceCode.Name = "txtExamServiceCode";
            this.txtExamServiceCode.Size = new System.Drawing.Size(59, 20);
            this.txtExamServiceCode.StyleController = this.layoutControl1;
            this.txtExamServiceCode.TabIndex = 5;
            this.txtExamServiceCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtExamServiceCode_KeyDown);
            this.txtExamServiceCode.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExamServiceCode_PreviewKeyDown);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciCboRoom,
            this.lciBtnDelete,
            this.lciRoom,
            this.lciExamService,
            this.layoutControlItem3});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(404, 52);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciBtnDelete
            // 
            this.lciBtnDelete.Control = this.btnDelete;
            this.lciBtnDelete.Location = new System.Drawing.Point(370, 0);
            this.lciBtnDelete.MaxSize = new System.Drawing.Size(34, 26);
            this.lciBtnDelete.MinSize = new System.Drawing.Size(34, 24);
            this.lciBtnDelete.Name = "lciBtnDelete";
            this.lciBtnDelete.Size = new System.Drawing.Size(34, 52);
            this.lciBtnDelete.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnDelete.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDelete.TextVisible = false;
            // 
            // lciRoom
            // 
            this.lciRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoom.Control = this.txtRoomCode;
            this.lciRoom.Location = new System.Drawing.Point(0, 0);
            this.lciRoom.Name = "lciRoom";
            this.lciRoom.OptionsToolTip.ToolTip = "Phòng khám";
            this.lciRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciRoom.Size = new System.Drawing.Size(136, 24);
            this.lciRoom.Text = "Phòng:";
            this.lciRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoom.TextSize = new System.Drawing.Size(70, 20);
            this.lciRoom.TextToControlDistance = 5;
            // 
            // lciExamService
            // 
            this.lciExamService.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExamService.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExamService.Control = this.txtExamServiceCode;
            this.lciExamService.Location = new System.Drawing.Point(0, 24);
            this.lciExamService.Name = "lciExamService";
            this.lciExamService.OptionsToolTip.ToolTip = "Yêu cầu khám";
            this.lciExamService.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciExamService.Size = new System.Drawing.Size(136, 28);
            this.lciExamService.Text = "Yêu cầu:";
            this.lciExamService.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExamService.TextSize = new System.Drawing.Size(70, 20);
            this.lciExamService.TextToControlDistance = 5;
            // 
            // layoutControlItem3
            // 
            this.layoutControlItem3.Control = this.cboExamService;
            this.layoutControlItem3.Location = new System.Drawing.Point(136, 24);
            this.layoutControlItem3.Name = "layoutControlItem3";
            this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem3.Size = new System.Drawing.Size(234, 28);
            this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem3.TextVisible = false;
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(136, 2);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.AutoComplete = false;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.PopupFormMinSize = new System.Drawing.Size(600, 500);
            this.cboRoom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            this.cboRoom.Properties.View = this.gridLookUpEdit1View;
            this.cboRoom.Size = new System.Drawing.Size(232, 20);
            this.cboRoom.StyleController = this.layoutControl1;
            this.cboRoom.TabIndex = 4;
            this.cboRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
            this.cboRoom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboRoom_ButtonClick);
            this.cboRoom.EditValueChanged += new System.EventHandler(this.cboRoom_EditValueChanged);
            this.cboRoom.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyDown);
            this.cboRoom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.gridLookUpEdit1View.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsBehavior.AutoPopulateColumns = false;
            this.gridLookUpEdit1View.OptionsClipboard.AllowHtmlFormat = DevExpress.Utils.DefaultBoolean.True;
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.AllowHtmlDrawHeaders = true;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            this.gridLookUpEdit1View.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gridLookUpEdit1View_RowStyle);
            this.gridLookUpEdit1View.ColumnFilterChanged += new System.EventHandler(this.gridLookUpEdit1View_ColumnFilterChanged);
            // 
            // lciCboRoom
            // 
            this.lciCboRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciCboRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciCboRoom.Control = this.cboRoom;
            this.lciCboRoom.Location = new System.Drawing.Point(136, 0);
            this.lciCboRoom.Name = "lciCboRoom";
            this.lciCboRoom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.lciCboRoom.Size = new System.Drawing.Size(234, 24);
            this.lciCboRoom.Text = "Phòng khám:";
            this.lciCboRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciCboRoom.TextSize = new System.Drawing.Size(0, 0);
            this.lciCboRoom.TextToControlDistance = 0;
            this.lciCboRoom.TextVisible = false;
            // 
            // UCRoomExamService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCRoomExamService";
            this.Size = new System.Drawing.Size(404, 52);
            this.Load += new System.EventHandler(this.UCRoomExamService_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtRoomCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboExamService.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit2View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExamServiceCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExamService)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciCboRoom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraEditors.GridLookUpEdit cboExamService;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit2View;
        private DevExpress.XtraEditors.TextEdit txtExamServiceCode;
        private HIS.UC.ServiceRoom.CustomControl.GridLookupEditCustom cboRoom;
        private HIS.UC.ServiceRoom.CustomControl.GridControlCustom gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciCboRoom;
        private DevExpress.XtraLayout.LayoutControlItem lciExamService;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDelete;
        private DevExpress.XtraEditors.TextEdit txtRoomCode;
        private DevExpress.XtraLayout.LayoutControlItem lciRoom;
    }
}
