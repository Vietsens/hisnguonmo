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
using DevExpress.XtraEditors;
using System.ComponentModel;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Extension
{
    [ToolboxItem(true)]
    public class CustomGridLookUpEdit : GridLookUpEdit
    {
        private DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit fProperties;
        private DevExpress.XtraGrid.Views.Grid.GridView fPropertiesView;
    
        static CustomGridLookUpEdit() { RepositoryItemCustomGridLookUpEdit.RegisterCustomGridLookUpEdit(); }
        public CustomGridLookUpEdit() : base() { }
        public override string EditorTypeName { get { return RepositoryItemCustomGridLookUpEdit.CustomGridLookUpEditName; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomGridLookUpEdit Properties { get { return base.Properties as RepositoryItemCustomGridLookUpEdit; } }

        protected override void ClosePopup(PopupCloseMode closeMode)
        {
            base.ClosePopup(PopupCloseMode.Normal);
        }

        private void InitializeComponent()
        {
            this.fProperties = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
            this.fPropertiesView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesView)).BeginInit();
            this.SuspendLayout();
            // 
            // fProperties
            // 
            this.fProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.fProperties.Name = "fProperties";
            this.fProperties.View = this.fPropertiesView;
            // 
            // fPropertiesView
            // 
            this.fPropertiesView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
            this.fPropertiesView.Name = "fPropertiesView";
            this.fPropertiesView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.fPropertiesView.OptionsView.ShowGroupPanel = false;
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesView)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
