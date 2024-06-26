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
namespace HIS.Desktop.Plugins.ServiceExecute.UcWords
{
    partial class UcTelerikFullWord
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
            this.radRichTextEditorRuler1 = new Telerik.WinControls.UI.RadRichTextEditorRuler();
            this.radRichTextEditor1 = new Telerik.WinControls.UI.RadRichTextEditor();
            this.richTextEditorRibbonBar1 = new Telerik.WinControls.UI.RichTextEditorRibbonBar();
            ((System.ComponentModel.ISupportInitialize)(this.radRichTextEditorRuler1)).BeginInit();
            this.radRichTextEditorRuler1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radRichTextEditor1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.richTextEditorRibbonBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // radRichTextEditorRuler1
            // 
            this.radRichTextEditorRuler1.AssociatedRichTextBox = this.radRichTextEditor1;
            this.radRichTextEditorRuler1.Controls.Add(this.radRichTextEditor1);
            this.radRichTextEditorRuler1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radRichTextEditorRuler1.Location = new System.Drawing.Point(0, 169);
            this.radRichTextEditorRuler1.Name = "radRichTextEditorRuler1";
            this.radRichTextEditorRuler1.Size = new System.Drawing.Size(700, 321);
            this.radRichTextEditorRuler1.TabIndex = 0;
            // 
            // radRichTextEditor1
            // 
            this.radRichTextEditor1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(189)))), ((int)(((byte)(232)))));
            this.radRichTextEditor1.LayoutMode = Telerik.WinForms.Documents.Model.DocumentLayoutMode.Paged;
            this.radRichTextEditor1.Location = new System.Drawing.Point(29, 29);
            this.radRichTextEditor1.Name = "radRichTextEditor1";
            this.radRichTextEditor1.SelectionFill = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(78)))), ((int)(((byte)(158)))), ((int)(((byte)(255)))));
            this.radRichTextEditor1.Size = new System.Drawing.Size(670, 291);
            this.radRichTextEditor1.TabIndex = 0;
            this.radRichTextEditor1.ScaleFactorChanged += new System.EventHandler(this.radRichTextEditor1_ScaleFactorChanged);
            // 
            // richTextEditorRibbonBar1
            // 
            this.richTextEditorRibbonBar1.ApplicationMenuStyle = Telerik.WinControls.UI.ApplicationMenuStyle.BackstageView;
            this.richTextEditorRibbonBar1.AssociatedRichTextEditor = this.radRichTextEditor1;
            this.richTextEditorRibbonBar1.BuiltInStylesVersion = Telerik.WinForms.Documents.Model.Styles.BuiltInStylesVersion.Office2013;
            this.richTextEditorRibbonBar1.EnableKeyMap = false;
            // 
            // 
            // 
            this.richTextEditorRibbonBar1.ExitButton.Text = "Exit";
            this.richTextEditorRibbonBar1.LocalizationSettings.LayoutModeText = "Simplified Layout";
            this.richTextEditorRibbonBar1.Location = new System.Drawing.Point(0, 0);
            this.richTextEditorRibbonBar1.Name = "richTextEditorRibbonBar1";
            // 
            // 
            // 
            this.richTextEditorRibbonBar1.OptionsButton.Text = "Options";
            this.richTextEditorRibbonBar1.ShowLayoutModeButton = true;
            this.richTextEditorRibbonBar1.Size = new System.Drawing.Size(700, 169);
            this.richTextEditorRibbonBar1.TabIndex = 1;
            this.richTextEditorRibbonBar1.TabStop = false;
            this.richTextEditorRibbonBar1.Text = "richTextEditorRibbonBar1";
            ((Telerik.WinControls.UI.RadRibbonBarElement)(this.richTextEditorRibbonBar1.GetChildAt(0))).Text = "richTextEditorRibbonBar1";
            ((Telerik.WinControls.UI.RibbonBar.RibbonBarCaptionFillPrimitive)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(1))).Visibility = Telerik.WinControls.ElementVisibility.Visible;
            ((Telerik.WinControls.UI.RadQuickAccessToolBar)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(2))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RadRibbonBarCaption)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(3))).Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            ((Telerik.WinControls.UI.RibbonTabStripElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4))).Visibility = Telerik.WinControls.ElementVisibility.Visible;
            ((Telerik.WinControls.UI.StripViewItemContainer)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0))).Padding = new System.Windows.Forms.Padding(44, 0, 84, 0);
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(5))).Text = "Review";
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(5))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(5))).Name = "tabReview";
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(6))).Text = "View";
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(6))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RichTextEditorRibbonUI.RichTextEditorRibbonTab)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(4).GetChildAt(0).GetChildAt(0).GetChildAt(6))).Name = "tabView";
            ((Telerik.WinControls.UI.RadApplicationMenuButtonElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(5))).DisplayStyle = Telerik.WinControls.DisplayStyle.Text;
            ((Telerik.WinControls.UI.RadApplicationMenuButtonElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(5))).Text = "File";
            ((Telerik.WinControls.UI.RadApplicationMenuButtonElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(5))).Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            ((Telerik.WinControls.UI.StackLayoutElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6))).Visibility = Telerik.WinControls.ElementVisibility.Visible;
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(0))).ThumbOffset = 0;
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(0))).ToolTipText = "Simplified Layout";
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(0))).Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(0))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RadToggleSwitchElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(0))).PositionOffset = new System.Drawing.SizeF(-3F, 0F);
            ((Telerik.WinControls.UI.LightVisualElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(1))).Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
            ((Telerik.WinControls.UI.RadImageButtonElement)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(3))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            ((Telerik.WinControls.UI.RadMDIControlsItem)(this.richTextEditorRibbonBar1.GetChildAt(0).GetChildAt(6).GetChildAt(4))).Visibility = Telerik.WinControls.ElementVisibility.Hidden;
            // 
            // UcTelerikFullWord
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radRichTextEditorRuler1);
            this.Controls.Add(this.richTextEditorRibbonBar1);
            this.Name = "UcTelerikFullWord";
            this.Size = new System.Drawing.Size(700, 490);
            ((System.ComponentModel.ISupportInitialize)(this.radRichTextEditorRuler1)).EndInit();
            this.radRichTextEditorRuler1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.radRichTextEditor1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.richTextEditorRibbonBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadRichTextEditorRuler radRichTextEditorRuler1;
        internal Telerik.WinControls.UI.RadRichTextEditor radRichTextEditor1;
        private Telerik.WinControls.UI.RichTextEditorRibbonBar richTextEditorRibbonBar1;
    }
}
