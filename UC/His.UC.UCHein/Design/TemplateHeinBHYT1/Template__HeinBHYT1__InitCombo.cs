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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using His.UC.UCHein.Base;
using His.UC.UCHein.Config;
using His.UC.UCHein.ControlProcess;
using Inventec.Common.Logging;
using Inventec.Common.QrCodeBHYT;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace His.UC.UCHein.Design.TemplateHeinBHYT1
{
    public partial class Template__HeinBHYT1 : UserControl
    {

        /// <summary>
        /// Đổ dữ liệu vào combo loại đúng tuyến
        /// </summary>
        private void InitComboHeinRightRoute()
        {
            try
            {
                this.cboHeinRightRoute.Properties.DataSource = DataStore.HeinRightRouteTypes;
                this.cboHeinRightRoute.Properties.DisplayMember = "RIGHT_ROUTE_TYPE_NAME";
                this.cboHeinRightRoute.Properties.ValueMember = "ID";
                this.cboHeinRightRoute.Properties.ForceInitialize();
                this.cboHeinRightRoute.Properties.Columns.Clear();
                this.cboHeinRightRoute.Properties.Columns.Add(new LookUpColumnInfo("RIGHT_ROUTE_TYPE_CODE", "", 100));
                this.cboHeinRightRoute.Properties.Columns.Add(new LookUpColumnInfo("RIGHT_ROUTE_TYPE_NAME", "", 200));
                this.cboHeinRightRoute.Properties.ShowHeader = false;
                this.cboHeinRightRoute.Properties.ImmediatePopup = true;
                this.cboHeinRightRoute.Properties.DropDownRows = 20;
                this.cboHeinRightRoute.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


    }
}
