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
    partial class UCHouseHoldRelative
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
            this.cboHouseHoldRelative = new DevExpress.XtraEditors.GridLookUpEdit();
            this.gridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.lciHouseHoldRelative = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            this.layoutControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cboHouseHoldRelative.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHouseHoldRelative)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl1
            // 
            this.layoutControl1.Controls.Add(this.cboHouseHoldRelative);
            this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(219, 24);
            this.layoutControl1.TabIndex = 0;
            this.layoutControl1.Text = "layoutControl1";
            // 
            // cboHouseHoldRelative
            // 
            this.cboHouseHoldRelative.Location = new System.Drawing.Point(75, 0);
            this.cboHouseHoldRelative.Name = "cboHouseHoldRelative";
            this.cboHouseHoldRelative.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
            this.cboHouseHoldRelative.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cboHouseHoldRelative.Properties.NullText = "";
            this.cboHouseHoldRelative.Properties.View = this.gridLookUpEdit1View;
            this.cboHouseHoldRelative.Size = new System.Drawing.Size(144, 20);
            this.cboHouseHoldRelative.StyleController = this.layoutControl1;
            this.cboHouseHoldRelative.TabIndex = 4;
            this.cboHouseHoldRelative.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.cboHouseHoldRelative_Closed);
            this.cboHouseHoldRelative.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cboHouseHoldRelative_KeyDown);
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
            this.lciHouseHoldRelative});
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(219, 24);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // lciHouseHoldRelative
            // 
            this.lciHouseHoldRelative.AppearanceItemCaption.Options.UseTextOptions = true;
            this.lciHouseHoldRelative.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.lciHouseHoldRelative.Control = this.cboHouseHoldRelative;
            this.lciHouseHoldRelative.Location = new System.Drawing.Point(0, 0);
            this.lciHouseHoldRelative.MaxSize = new System.Drawing.Size(0, 20);
            this.lciHouseHoldRelative.MinSize = new System.Drawing.Size(129, 20);
            this.lciHouseHoldRelative.Name = "lciHouseHoldRelative";
            this.lciHouseHoldRelative.OptionsToolTip.ToolTip = "Quan hệ với chủ hộ";
            this.lciHouseHoldRelative.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.lciHouseHoldRelative.Size = new System.Drawing.Size(219, 24);
            this.lciHouseHoldRelative.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.lciHouseHoldRelative.Text = "QH với CH:";
            this.lciHouseHoldRelative.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
            this.lciHouseHoldRelative.TextSize = new System.Drawing.Size(70, 20);
            this.lciHouseHoldRelative.TextToControlDistance = 5;
            // 
            // UCHouseHoldRelative
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutControl1);
            this.Name = "UCHouseHoldRelative";
            this.Size = new System.Drawing.Size(219, 24);
            this.Load += new System.EventHandler(this.UCHouseHoldRelative_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            this.layoutControl1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cboHouseHoldRelative.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridLookUpEdit1View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lciHouseHoldRelative)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private DevExpress.XtraEditors.GridLookUpEdit cboHouseHoldRelative;
        private DevExpress.XtraGrid.Views.Grid.GridView gridLookUpEdit1View;
        private DevExpress.XtraLayout.LayoutControlItem lciHouseHoldRelative;
    }
}
