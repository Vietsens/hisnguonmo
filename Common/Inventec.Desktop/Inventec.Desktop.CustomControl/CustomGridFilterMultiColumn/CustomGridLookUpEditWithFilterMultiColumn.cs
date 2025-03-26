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
using System.Text;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors;
using System.ComponentModel;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid;
using DevExpress.XtraEditors.Popup;

namespace Inventec.Desktop.CustomControl
{
    [UserRepositoryItem("RegisterCustomGridLookUpEdit")]
    public class RepositoryItemCustomGridLookUpEdit : RepositoryItemGridLookUpEdit
    {
        static RepositoryItemCustomGridLookUpEdit() { RegisterCustomGridLookUpEdit(); }

        public RepositoryItemCustomGridLookUpEdit()
        {
            TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            AutoComplete = false;
        }
        [Browsable(false)]
        public override DevExpress.XtraEditors.Controls.TextEditStyles TextEditStyle { get { return base.TextEditStyle; } set { base.TextEditStyle = value; } }
        public const string CustomGridLookUpEditName = "CustomGridLookUpEditWithFilterMultiColumn";

        public override string EditorTypeName { get { return CustomGridLookUpEditName; } }

        public static void RegisterCustomGridLookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomGridLookUpEditName,
              typeof(CustomGridLookUpEditWithFilterMultiColumn), typeof(RepositoryItemCustomGridLookUpEdit),
              typeof(GridLookUpEditBaseViewInfo), new ButtonEditPainter(), true));
        }

        protected override GridView CreateViewInstance() { return new CustomGridViewWithFilterMultiColumn(); }
        protected override GridControl CreateGrid() { return new CustomGridControlWithFilterMultiColumn(); }
    }


    public class CustomGridLookUpEditWithFilterMultiColumn : GridLookUpEdit
    {
        static CustomGridLookUpEditWithFilterMultiColumn() { RepositoryItemCustomGridLookUpEdit.RegisterCustomGridLookUpEdit(); }

        public CustomGridLookUpEditWithFilterMultiColumn() : base() { }

        public override string EditorTypeName { get { return RepositoryItemCustomGridLookUpEdit.CustomGridLookUpEditName; } }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomGridLookUpEdit Properties { get { return base.Properties as RepositoryItemCustomGridLookUpEdit; } }

        protected override DevExpress.XtraEditors.Popup.PopupBaseForm CreatePopupForm() // NEW
        {
            return new MyGridLookUpPopupForm(this);
        }

        public override bool IsNeededKey(System.Windows.Forms.KeyEventArgs e)
        {
            return Properties.IsNeededKey(e.KeyData);
        }

        protected override bool IsAutoComplete
        {
            get
            {
                return true;
            }
        }
    }

    public class MyGridLookUpPopupForm : PopupGridLookUpEditForm // NEW
    {
        public MyGridLookUpPopupForm(GridLookUpEdit ownerEdit)
            : base(ownerEdit)
        {
        }

        protected override void OnKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Tab)
            {
                OwnerEdit.EditValue = QueryResultValue();
                this.OwnerEdit.SendKey(e);
            }
            base.OnKeyDown(e);
        }
    }
}
