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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.Controls
{
    /// <summary>
    /// Panel cuon nhung khong tu nhay theo control nhan focus.
    ///
    /// Panel.AutoScroll mac dinh se cuon toi control vua duoc focus. Voi bang nay, luc nap xong
    /// mot the phong o hang cuoi co the doat focus, ban to lap tuc nhay xuong day - nguoi dung
    /// mo len la thay phong 205 thay vi 201. Chan bang cach tra lai dung vi tri dang cuon.
    /// Cuon bang chuot va thanh cuon van hoat dong binh thuong.
    /// </summary>
    [ToolboxItem(true)]
    public class NoAutoScrollPanel : Panel
    {
        protected override Point ScrollToControl(Control activeControl)
        {
            return DisplayRectangle.Location;
        }
    }
}
