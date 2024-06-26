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
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial.Base
{
    internal class TabControlBaseProcess
    {
        /// <summary>
        /// Tạo thêm tab mới
        /// </summary>
        /// <param name="tabControl">Tên TabControl để thêm tabpage mới vào</param>
        /// <param name="name">Tên tabpage mới</param>
        internal static void TabCreating(XtraTabControl tabControl, string name, string text, UserControl uc, Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                if (tabControl.InvokeRequired)
                {
                    tabControl.Invoke(new MethodInvoker(delegate { TabCreatingCustomFill(tabControl, name, text, uc, true, module); }));
                }
                else
                {
                    TabCreatingCustomFill(tabControl, name, text, uc, true, module);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tạo thêm tab mới
        /// </summary>
        /// <param name="tabControl">Tên TabControl để thêm tabpage mới vào</param>
        /// <param name="name">Tên tabpage mới</param>
        internal static void TabCreatingCustomFill(XtraTabControl tabControl, string name, string text, UserControl uc, bool autoFill, Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                if (tabControl.Visible == false)
                {
                    tabControl.Visible = true;
                }

                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((module));

                int index = IsExistsModuleTabPage(tabControl, name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0"));
                if (index >= 0)
                {
                    if (tabControl.TabPages[index].PageVisible == false)
                        tabControl.TabPages[index].PageVisible = true;

                    tabControl.SelectedTabPage = tabControl.TabPages[index];
                }
                else
                {
                    XtraTabPage tabpage = new XtraTabPage { Text = text + (workPlace != null ? " (" + workPlace.RoomName + ")" : ""), Name = name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0") };
                    tabpage.Tag = module;
                    tabControl.TabPages.Add(tabpage);
                    tabControl.SelectedTabPage = tabpage;
                    uc.Parent = tabpage;
                    uc.Show();
                    if (autoFill)
                        uc.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void TabCreatingCustomFill(XtraTabControl tabControl, string name, string text, Form form, Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                if (tabControl.Visible == false)
                {
                    tabControl.Visible = true;
                }
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((module));

                int index = IsExistsModuleTabPage(tabControl, name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0"));
                if (index >= 0)
                {
                    if (tabControl.TabPages[index].PageVisible == false)
                        tabControl.TabPages[index].PageVisible = true;

                    tabControl.SelectedTabPage = tabControl.TabPages[index];
                }
                else
                {
                    XtraTabPage tabpage = new XtraTabPage { Text = text + (workPlace != null ? " (" + workPlace.RoomName + ")" : ""), Name = name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0") };
                    tabpage.Tag = module;
                    tabControl.TabPages.Add(tabpage);
                    tabControl.SelectedTabPage = tabpage;
                    form.TopLevel = false;
                    form.Parent = tabpage;
                    form.Show();
                    form.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra tabpage có tồn tại hay không
        /// </summary>
        /// <param name="tabControlName">Tên TabControl để kiểm tra</param>
        /// <param name="tabName">Tên tabpage cần kiểm tra</param>
        protected static int IsExistsModuleTabPage(XtraTabControl tabControlName, string tabName)
        {
            int result = -1;
            try
            {
                for (int i = 0; i < tabControlName.TabPages.Count; i++)
                {
                    if (tabControlName.TabPages[i].Name == tabName)
                    {
                        result = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}
