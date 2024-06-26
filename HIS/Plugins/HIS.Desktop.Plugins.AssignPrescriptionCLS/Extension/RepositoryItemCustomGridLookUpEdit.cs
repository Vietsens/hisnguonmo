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
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.Extension
{
    [UserRepositoryItem("RegisterCustomGridLookUpEdit")]
    public class RepositoryItemCustomGridLookUpEdit : RepositoryItemGridLookUpEdit
    {
        static RepositoryItemCustomGridLookUpEdit() { RegisterCustomGridLookUpEdit(); }
        public RepositoryItemCustomGridLookUpEdit() { }
        public const string CustomGridLookUpEditName = "CustomGridLookUpEdit";
        public override string EditorTypeName { get { return CustomGridLookUpEditName; } }

        public static void RegisterCustomGridLookUpEdit()
        {
            EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(CustomGridLookUpEditName,
              typeof(CustomGridLookUpEdit), typeof(RepositoryItemCustomGridLookUpEdit),
              typeof(GridLookUpEditBaseViewInfo), new ButtonEditPainter(), true));
        }
    }
}
