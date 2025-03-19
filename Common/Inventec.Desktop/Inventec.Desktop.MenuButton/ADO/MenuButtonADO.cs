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

namespace Inventec.Desktop.MenuButton.ADO
{
    public class MenuButtonADO
    {
        public string Caption { get; set; }
        public string Code { get; set; }
        public object Tag { get; set; }
        public bool IsGroup { get; set; }
        public string ParentCode { get; set; }
        public int NumOrder { get; set; }
        public EventHandler EventHandler { get; set; }
        public ItemClickEventHandler ItemClickEventHandler { get; set; }
        internal string ShortcutDisplay { get; set; }
        public string Tooltip { get; set; }
        public int MaxSizeWidth { get; set; }
        public int MaxSizeHeight { get; set; }
        public int MinSizeWidth { get; set; }
        public int MinSizeHeight { get; set; }
        public int? ImageIndex { get; set; }

        public MenuButtonADO() { }

        public MenuButtonADO(MenuButtonADO data)
        {
            try
            {
                this.Caption = data.Caption;
                this.Code = data.Code;
                this.ShortcutDisplay = data.ShortcutDisplay;
                this.EventHandler = data.EventHandler;
                this.ItemClickEventHandler = data.ItemClickEventHandler;
                this.Tag = data.Tag;
                this.Tooltip = data.Tooltip;
                this.ParentCode = data.ParentCode;
                this.NumOrder = data.NumOrder;
                this.MaxSizeWidth = data.MaxSizeWidth;
                this.MaxSizeHeight = data.MaxSizeHeight;
                this.MinSizeWidth = data.MinSizeWidth;
                this.MinSizeHeight = data.MinSizeHeight;
                this.ImageIndex = data.ImageIndex;

                this.Caption = String.Format("{0}{1}", data.Caption, (!String.IsNullOrEmpty(data.ShortcutDisplay) ? " (" + data.ShortcutDisplay + ")" : ""));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}
