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
namespace HIS.UC.ServiceRoomInfo.Run
{
    partial class UCServiceRoomInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCServiceRoomInfo));
            DevExpress.Utils.SuperToolTip superToolTip1 = new DevExpress.Utils.SuperToolTip();
            DevExpress.Utils.ToolTipItem toolTipItem1 = new DevExpress.Utils.ToolTipItem();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject5 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject6 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject7 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject8 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
            this.cboExamServiceType = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit5View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.txtExamServiceType = new DevExpress.XtraEditors.TextEdit();
            this.cboRoom = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciExamServiceType = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciRoom = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciBtnDelete = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            this.dxErrorProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboExamServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit5View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExamServiceType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExamServiceType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDelete)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.btnDelete);
            this.layoutControl1.Controls.Add(this.cboExamServiceType);
            this.layoutControl1.Controls.Add(this.txtExamServiceType);
            this.layoutControl1.Controls.Add(this.cboRoom);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsFocus.EnableAutoTabOrder = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(870, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // btnDelete
            // 
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
            this.btnDelete.Location = new System.Drawing.Point(838, 2);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(30, 20);
            this.btnDelete.StyleController = this.layoutControl1;
            toolTipItem1.Text = "Xóa bỏ";
            superToolTip1.Items.Add(toolTipItem1);
            this.btnDelete.SuperTip = superToolTip1;
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // cboExamServiceType
            // 
            this.cboExamServiceType.Location = new System.Drawing.Point(160, 2);
            this.cboExamServiceType.Name = "cboExamServiceType";
            this.cboExamServiceType.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboExamServiceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboExamServiceType.Properties.NullText = "";
            this.cboExamServiceType.Properties.View = this.gridLookUpEdit5View;
            this.cboExamServiceType.Size = new System.Drawing.Size(278, 20);
            this.cboExamServiceType.StyleController = this.layoutControl1;
            this.cboExamServiceType.TabIndex = 2;
            this.cboExamServiceType.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboExamServiceType_Closed);
            this.cboExamServiceType.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboExamServiceType_ButtonClick);
            this.cboExamServiceType.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboExamServiceType_KeyUp);
            // 
            // gridLookUpEdit5View
            // 
            this.gridLookUpEdit5View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit5View.Name = "gridLookUpEdit5View";
            this.gridLookUpEdit5View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit5View.OptionsView.ShowGroupPanel = false;
            // 
            // txtExamServiceType
            // 
            this.txtExamServiceType.Location = new System.Drawing.Point(97, 2);
            this.txtExamServiceType.Name = "txtExamServiceType";
            this.txtExamServiceType.Size = new System.Drawing.Size(63, 20);
            this.txtExamServiceType.StyleController = this.layoutControl1;
            this.txtExamServiceType.TabIndex = 1;
            this.txtExamServiceType.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtExamServiceTypeCode_PreviewKeyDown);
            // 
            // cboRoom
            // 
            this.cboRoom.Location = new System.Drawing.Point(537, 2);
            this.cboRoom.Name = "cboRoom";
            this.cboRoom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboRoom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject5, serializableAppearanceObject6, serializableAppearanceObject7, serializableAppearanceObject8, "", null, null, true)});
            this.cboRoom.Properties.NullText = "";
            this.cboRoom.Properties.View = this.gridLookUpEdit1View;
            this.cboRoom.Size = new System.Drawing.Size(297, 20);
            this.cboRoom.StyleController = this.layoutControl1;
            this.cboRoom.TabIndex = 3;
            this.cboRoom.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboRoom_Closed);
            this.cboRoom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboRoom_ButtonClick);
            this.cboRoom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboRoom_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lciExamServiceType,
            this.lciRoom,
            this.layoutControlItem5,
            this.lciBtnDelete});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(870, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciExamServiceType
            // 
            this.lciExamServiceType.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciExamServiceType.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciExamServiceType.Control = this.txtExamServiceType;
            this.lciExamServiceType.Location = new System.Drawing.Point(0, 0);
            this.lciExamServiceType.MaxSize = new System.Drawing.Size(0, 24);
            this.lciExamServiceType.MinSize = new System.Drawing.Size(20, 24);
            this.lciExamServiceType.Name = "lciExamServiceType";
            this.lciExamServiceType.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
            this.lciExamServiceType.Size = new System.Drawing.Size(160, 24);
            this.lciExamServiceType.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciExamServiceType.Text = "Yêu cầu khám:";
            this.lciExamServiceType.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciExamServiceType.TextSize = new System.Drawing.Size(90, 20);
            this.lciExamServiceType.TextToControlDistance = 5;
            // 
            // lciRoom
            // 
            this.lciRoom.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciRoom.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciRoom.Control = this.cboRoom;
            this.lciRoom.Location = new System.Drawing.Point(440, 0);
            this.lciRoom.MaxSize = new System.Drawing.Size(0, 24);
            this.lciRoom.MinSize = new System.Drawing.Size(149, 24);
            this.lciRoom.Name = "lciRoom";
            this.lciRoom.Size = new System.Drawing.Size(396, 24);
            this.lciRoom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciRoom.Text = "Phòng khám:";
            this.lciRoom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciRoom.TextSize = new System.Drawing.Size(90, 20);
            this.lciRoom.TextToControlDistance = 5;
            // 
            // layoutControlItem5
            // 
            this.layoutControlItem5.Control = this.cboExamServiceType;
            this.layoutControlItem5.Location = new System.Drawing.Point(160, 0);
            this.layoutControlItem5.Name = "layoutControlItem5";
            this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
            this.layoutControlItem5.Size = new System.Drawing.Size(280, 24);
            this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem5.TextVisible = false;
            // 
            // lciBtnDelete
            // 
            this.lciBtnDelete.Control = this.btnDelete;
            this.lciBtnDelete.Location = new System.Drawing.Point(836, 0);
            this.lciBtnDelete.MaxSize = new System.Drawing.Size(530, 26);
            this.lciBtnDelete.MinSize = new System.Drawing.Size(10, 10);
            this.lciBtnDelete.Name = "lciBtnDelete";
            this.lciBtnDelete.Size = new System.Drawing.Size(34, 24);
            this.lciBtnDelete.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciBtnDelete.TextSize = new System.Drawing.Size(0, 0);
            this.lciBtnDelete.TextVisible = false;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // dxErrorProvider1
            // 
            this.dxErrorProvider1.ContainerControl = this;
            // 
            // UCServiceRoomInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCServiceRoomInfo";
            this.Size = new System.Drawing.Size(870, 24);
            this.Load += new System.EventHandler(this.UCServiceRoomInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboExamServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit5View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtExamServiceType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboRoom.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciExamServiceType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciRoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciBtnDelete)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxErrorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.GridLookUpEdit cboExamServiceType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit5View;
        internal DevExpress.XtraEditors.TextEdit txtExamServiceType;
        private DevExpress.XtraLayout.LayoutControlItem lciRoom;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem5;
        private DevExpress.XtraLayout.LayoutControlItem lciExamServiceType;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        internal DevExpress.XtraEditors.GridLookUpEdit cboRoom;
        internal DevExpress.XtraEditors.SimpleButton btnDelete;
        private DevExpress.XtraLayout.LayoutControlItem lciBtnDelete;
        public DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
        private DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1;
    }
}
