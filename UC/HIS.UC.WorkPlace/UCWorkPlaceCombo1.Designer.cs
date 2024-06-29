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
namespace HIS.UC.WorkPlace
{
    partial class UCWorkPlaceCombo1
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
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject2 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject3 = new DevExpress.Utils.SerializableAppearanceObject();
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject4 = new DevExpress.Utils.SerializableAppearanceObject();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtWorkPlaceCode1 = new DevExpress.XtraEditors.TextEdit();
            this.cboWorkPlace1 = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lblNoiLamViec = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkPlaceCode1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkPlace1.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblNoiLamViec)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtWorkPlaceCode1);
            this.layoutControl1.Controls.Add(this.cboWorkPlace1);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(202, 22);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtWorkPlaceCode1
            // 
            this.txtWorkPlaceCode1.Location = new System.Drawing.Point(75, 0);
            this.txtWorkPlaceCode1.Name = "txtWorkPlaceCode1";
            this.txtWorkPlaceCode1.Size = new System.Drawing.Size(43, 20);
            this.txtWorkPlaceCode1.StyleController = this.layoutControl1;
            this.txtWorkPlaceCode1.TabIndex = 4;
            this.txtWorkPlaceCode1.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtWorkPlaceCode_PreviewKeyDown);
            // 
            // cboWorkPlace1
            // 
            this.cboWorkPlace1.Location = new System.Drawing.Point(118, 0);
            this.cboWorkPlace1.Name = "cboWorkPlace1";
            this.cboWorkPlace1.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboWorkPlace1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), serializableAppearanceObject1, serializableAppearanceObject2, serializableAppearanceObject3, serializableAppearanceObject4, "", null, null, true)});
            this.cboWorkPlace1.Properties.NullText = "";
            this.cboWorkPlace1.Properties.View = this.gridLookUpEdit1View;
            this.cboWorkPlace1.Size = new System.Drawing.Size(84, 20);
            this.cboWorkPlace1.StyleController = this.layoutControl1;
            this.cboWorkPlace1.TabIndex = 5;
            this.cboWorkPlace1.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboWorkPlace_Closed);
            this.cboWorkPlace1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboWorkPlace1_ButtonClick);
            this.cboWorkPlace1.EditValueChanged += new System.EventHandler(this.cboWorkPlace1_EditValueChanged);
            this.cboWorkPlace1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cboWorkPlace_KeyUp);
            // 
            // gridLookUpEdit1View
            // 
            this.gridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.gridLookUpEdit1View.Name = "gridLookUpEdit1View";
            this.gridLookUpEdit1View.OptionsFilter.AllowFilterIncrementalSearch = false;
            this.gridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.lblNoiLamViec,
            this.layoutControlItem2});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(202, 22);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lblNoiLamViec
            // 
            this.lblNoiLamViec.AppearanceItemCaption.ForeColor = System.Drawing.Color.Black;
            this.lblNoiLamViec.AppearanceItemCaption.Options.UseForeColor = true;
            this.lblNoiLamViec.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lblNoiLamViec.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lblNoiLamViec.Control = this.txtWorkPlaceCode1;
            this.lblNoiLamViec.Location = new System.Drawing.Point(0, 0);
            this.lblNoiLamViec.MaxSize = new System.Drawing.Size(0, 20);
            this.lblNoiLamViec.MinSize = new System.Drawing.Size(118, 20);
            this.lblNoiLamViec.Name = "lblNoiLamViec";
            this.lblNoiLamViec.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lblNoiLamViec.Size = new System.Drawing.Size(118, 22);
            this.lblNoiLamViec.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lblNoiLamViec.Text = "Nơi làm việc:";
            this.lblNoiLamViec.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lblNoiLamViec.TextSize = new System.Drawing.Size(70, 20);
            this.lblNoiLamViec.TextToControlDistance = 5;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.cboWorkPlace1;
            this.layoutControlItem2.Location = new System.Drawing.Point(118, 0);
            this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem2.MinSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem2.Size = new System.Drawing.Size(84, 22);
            this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem2.TextSize = new System.Drawing.Size(-5, 0);
            this.layoutControlItem2.TextToControlDistance = 5;
            // 
            // dxValidationProvider1
            // 
            this.dxValidationProvider1.ValidationFailed += new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
            // 
            // UCWorkPlaceCombo1
            // 
            this.Appearance.Options.UseTextOptions = true;
            this.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCWorkPlaceCombo1";
            this.Size = new System.Drawing.Size(202, 22);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtWorkPlaceCode1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboWorkPlace1.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lblNoiLamViec)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxValidationProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraLayout.LayoutControlItem lblNoiLamViec;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        internal DevExpress.XtraEditors.TextEdit txtWorkPlaceCode1;
        internal DevExpress.XtraEditors.GridLookUpEdit cboWorkPlace1;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem2;
        private DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1;
    }
}
