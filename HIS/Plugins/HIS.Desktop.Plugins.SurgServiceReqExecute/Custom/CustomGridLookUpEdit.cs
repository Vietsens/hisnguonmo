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
using HIS.Desktop.Utilities.Extensions;

namespace MultiColumnFilterTest
{
    [UserRepositoryItem("RegisterCustomGridLookUpEditNew")]
    public class RepositoryItemCustomGridLookUpEditNew : RepositoryItemGridLookUpEdit
    {
        static RepositoryItemCustomGridLookUpEditNew() { RegisterCustomGridLookUpEditNew(); }

        public RepositoryItemCustomGridLookUpEditNew()
        {
            TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
            AutoComplete = false;
        }
        [Browsable(false)]
        public override DevExpress.XtraEditors.Controls.TextEditStyles TextEditStyle
        {
            get
            {
                return
                    base.TextEditStyle;
            }
            set { base.TextEditStyle = value; }
        }
        public const string CustomGridLookUpEditNewName = "CustomGridLookUpEditNew";

        public override string EditorTypeName { get { return CustomGridLookUpEditNewName; } }

        public static void RegisterCustomGridLookUpEditNew()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomGridLookUpEditNewName,
              typeof(CustomGridLookUpEditNew), typeof(RepositoryItemCustomGridLookUpEditNew),
              typeof(GridLookUpEditBaseViewInfo), new ButtonEditPainter(), true));
        }

        protected override GridView CreateViewInstance() { return new CustomGridView(); }
        protected override GridControl CreateGrid() { return new CustomGridControl(); }
    }


    public class CustomGridLookUpEditNew : GridLookUpEdit
    {
        static CustomGridLookUpEditNew()
        {
            RepositoryItemCustomGridLookUpEditNew.RegisterCustomGridLookUpEditNew();
        }

        public CustomGridLookUpEditNew() : base() { }

        public override string EditorTypeName
        {
            get
            {
                return
                    RepositoryItemCustomGridLookUpEditNew.CustomGridLookUpEditNewName;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new RepositoryItemCustomGridLookUpEditNew Properties
        {
            get
            {
                return base.Properties as
                    RepositoryItemCustomGridLookUpEditNew;
            }
        }
    }
}
