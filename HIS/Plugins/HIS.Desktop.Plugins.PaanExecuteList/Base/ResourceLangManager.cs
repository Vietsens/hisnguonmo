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
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PaanExecuteList.Base
{
    /// <summary>
    /// Quan ly bo tai nguyen ngon ngu CHEP TU man hinh "Xu ly yeu cau kham/cls/pttt".
    ///
    /// Ly do phai co: menu chuot phai chep tu man cu dung khoang 28 khoa ngon ngu
    /// dang "UCExecuteRoom.btnXxx.Text". De giu nguyen nhan tieng Viet ma khong
    /// phai sua tay tung dong, ta chep nguyen file Lang.vi.resx cua man cu sang
    /// day, doi ten thanh LangExecuteRoom de khong dung voi Lang.resx cua chinh
    /// plugin nay.
    /// </summary>
    class ResourceLangManager
    {
        internal static ResourceManager LanguageUCExecuteRoom { get; set; }

        internal static void InitResourceLanguageManager()
        {
            try
            {
                LanguageUCExecuteRoom = new ResourceManager(
                    "HIS.Desktop.Plugins.PaanExecuteList.Resources.LangExecuteRoom",
                    typeof(HIS.Desktop.Plugins.PaanExecuteList.PaanExecuteListProcessor).Assembly);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
