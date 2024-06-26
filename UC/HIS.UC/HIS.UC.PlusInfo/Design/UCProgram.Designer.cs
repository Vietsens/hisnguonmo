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
namespace HIS.UC.PlusInfo.Design
{
    partial class UCProgram
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
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.txtProgramCode = new DevExpress.XtraEditors.TextEdit();
            this.cboProgram = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.lciProgram = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtProgramCode.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProgram.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProgram)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.txtProgramCode);
            this.layoutControl1.Controls.Add(this.cboProgram);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // txtProgramCode
            // 
            this.txtProgramCode.Location = new System.Drawing.Point(75, 0);
            this.txtProgramCode.Name = "txtProgramCode";
            this.txtProgramCode.Size = new System.Drawing.Size(52, 20);
            this.txtProgramCode.StyleController = this.layoutControl1;
            this.txtProgramCode.TabIndex = 0;
            this.txtProgramCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProgramCode_KeyDown);
            // 
            // cboProgram
            // 
            this.cboProgram.Location = new System.Drawing.Point(127, 0);
            this.cboProgram.Name = "cboProgram";
            this.cboProgram.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboProgram.Properties.AutoComplete = false;
            this.cboProgram.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
            this.cboProgram.Properties.NullText = "";
            this.cboProgram.Properties.View = this.gridLookUpEdit1View;
            this.cboProgram.Size = new System.Drawing.Size(92, 20);
            this.cboProgram.StyleController = this.layoutControl1;
            this.cboProgram.TabIndex = 1;
            this.cboProgram.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboProgram_Closed);
            this.cboProgram.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.cboProgram_ButtonClick);
            this.cboProgram.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboProgram_KeyDown);
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
            this.layoutControlItem1,
            this.lciProgram});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.cboProgram;
            this.layoutControlItem1.Location = new System.Drawing.Point(127, 0);
            this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 20);
            this.layoutControlItem1.MinSize = new System.Drawing.Size(50, 20);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlItem1.Size = new System.Drawing.Size(92, 24);
            this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextToControlDistance = 0;
            this.layoutControlItem1.TextVisible = false;
            // 
            // lciProgram
            // 
            this.lciProgram.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciProgram.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciProgram.Control = this.txtProgramCode;
            this.lciProgram.Location = new System.Drawing.Point(0, 0);
            this.lciProgram.MaxSize = new System.Drawing.Size(0, 20);
            this.lciProgram.MinSize = new System.Drawing.Size(127, 20);
            this.lciProgram.Name = "lciProgram";
            this.lciProgram.OptionsToolTip.ToolTip = "Chương trình";
            this.lciProgram.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciProgram.Size = new System.Drawing.Size(127, 24);
            this.lciProgram.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciProgram.Text = "CT:";
            this.lciProgram.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciProgram.TextSize = new System.Drawing.Size(70, 20);
            this.lciProgram.TextToControlDistance = 5;
            // 
            // UCProgram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCProgram";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCProgram_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtProgramCode.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cboProgram.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciProgram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        internal DevExpress.XtraEditors.TextEdit txtProgramCode;
        private DevExpress.XtraLayout.LayoutControlItem layoutControlItem1;
        private DevExpress.XtraLayout.LayoutControlItem lciProgram;
        private DevExpress.XtraEditors.GridLookUpEdit cboProgram;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
    }
}
