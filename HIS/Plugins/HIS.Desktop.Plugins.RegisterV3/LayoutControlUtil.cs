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
using DevExpress.XtraLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV3
{
    class LayoutControlUtil
    {
        internal static LayoutControlItem AddToLayout(UserControl ucControl, bool textVisible, System.Drawing.Size size, System.Drawing.Size textSize, SizeConstraintsType sizeConstraintsType, System.Drawing.Size maxSize, System.Drawing.Size minSize)
        {
            int dem = 0;
            return  new LayoutControlItem
            {
                Control = ucControl,
                Name = String.Format("{0}{1}", ucControl.Name, dem),
                TextVisible = textVisible,
                Size = size,
                TextSize = textSize,
                SizeConstraintsType = sizeConstraintsType,
                MaxSize = maxSize,
                MinSize = minSize
            };
        }

        internal static void Move(LayoutControlItem itemMove, LayoutControlItem itemCenter, DevExpress.XtraLayout.Utils.InsertType insertType)
        {
            itemMove.Move(itemCenter, insertType);
        }
    }
}
