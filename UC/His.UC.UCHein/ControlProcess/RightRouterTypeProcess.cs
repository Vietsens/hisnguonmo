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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His.UC.UCHein.ControlProcess
{
    public class HeinRightRouterTypeProcess
    {
        public static void FillDataToComboHeinRightRouterType(DevExpress.XtraEditors.LookUpEdit cboHeinRightRouterType, List<MOS.LibraryHein.Bhyt.HeinRightRouteType.HeinRightRouteTypeData> data)
        {
            try
            {
                cboHeinRightRouterType.Properties.DataSource = data;
                cboHeinRightRouterType.Properties.DisplayMember = "HeinRightRouteTypeName";
                cboHeinRightRouterType.Properties.ValueMember = "HeinRightRouteTypeCode";
                cboHeinRightRouterType.Properties.ForceInitialize();
                cboHeinRightRouterType.Properties.Columns.Clear();
                cboHeinRightRouterType.Properties.Columns.Add(new LookUpColumnInfo("HeinRightRouteTypeCode", "", 100));
                cboHeinRightRouterType.Properties.Columns.Add(new LookUpColumnInfo("HeinRightRouteTypeName", "", 200));
                cboHeinRightRouterType.Properties.ShowHeader = false;
                cboHeinRightRouterType.Properties.ImmediatePopup = true;
                cboHeinRightRouterType.Properties.DropDownRows = 20;
                cboHeinRightRouterType.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
