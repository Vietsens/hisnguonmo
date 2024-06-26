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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.CareCreate
{
    public partial class CareCreate : HIS.Desktop.Utility.FormBase
    {

        internal static void LoadDataToComboYThuc(DevExpress.XtraEditors.LookUpEdit cboYThuc,object data)
        {
            try
            {
                cboYThuc.Properties.DataSource = data;
                cboYThuc.Properties.DisplayMember = "AWARENESS_NAME";
                cboYThuc.Properties.ValueMember = "ID";
                cboYThuc.Properties.ForceInitialize();
                cboYThuc.Properties.Columns.Clear();
                cboYThuc.Properties.Columns.Add(new LookUpColumnInfo("AWARENESS_NAME", "", 200));
                cboYThuc.Properties.ShowHeader = false;
                cboYThuc.Properties.ImmediatePopup = true;
                cboYThuc.Properties.DropDownRows = 20;
                cboYThuc.Properties.PopupWidth = 500;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    

    }
}
