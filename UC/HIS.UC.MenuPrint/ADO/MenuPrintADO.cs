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
using DevExpress.XtraBars;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MenuPrint.ADO
{
    public class MenuPrintADO
    {
        public string Caption { get; set; }
        public string PrintTypeCode { get; set; }
        public object Tag { get; set; }
        public EventHandler EventHandler { get; set; }
        public ItemClickEventHandler ItemClickEventHandler { get; set; }
        internal string Shortcut { get; set; }
        public string Tooltip { get; set; }

        public MenuPrintADO() { }

        public MenuPrintADO(MenuPrintADO data, SAR.EFMODEL.DataModels.SAR_PRINT_TYPE printType)
        {
            try
            {                
                this.Caption = data.Caption;
                this.PrintTypeCode = data.PrintTypeCode;
                this.Shortcut = data.Shortcut;
                this.EventHandler = data.EventHandler;
                this.ItemClickEventHandler = data.ItemClickEventHandler;
                this.Tag = data.Tag;
                this.Tooltip = data.Tooltip;

                if (String.IsNullOrEmpty(data.Caption))
                {
                    this.Caption = String.Format("{0}{1}", printType != null ? printType.PRINT_TYPE_NAME : "", (!String.IsNullOrEmpty(data.Shortcut) ? " (" + data.Shortcut + ")" : ""));
                }
                else
                {
                    this.Caption = String.Format("{0}{1}", data.Caption, (!String.IsNullOrEmpty(data.Shortcut) ? " (" + data.Shortcut + ")" : ""));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
