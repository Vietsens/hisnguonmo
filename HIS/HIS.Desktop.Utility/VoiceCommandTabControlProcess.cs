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
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Utility
{
    internal delegate void SaveDataBeforeCloseExt(object uc);

    internal class VoiceCommandTabControlProcess
    {
        internal static Dictionary<string, SaveDataBeforeCloseExt> dicSaveData = new Dictionary<string, SaveDataBeforeCloseExt>();
        internal static System.Drawing.Font tabPageFont;

        static void SaveDataBeforeCloseV2(object uc)
        {
            try
            {
                CloseCameraFormOpened();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void OpenPluginPage(XtraTabControl tabControl, Inventec.Desktop.Common.Modules.Module module, string text, string pluginName)
        {
            try
            {
                int index = IsExistsModuleTabPage(tabControl, pluginName);
                if (index >= 0)
                {
                    if (tabControl.TabPages[index].PageVisible == false)
                        tabControl.TabPages[index].PageVisible = true;

                    tabControl.TabPages[index].Text = text;
                    //tabControl.TabPages[index] = name;
                    tabControl.TabPages[index].Tag = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(module, module.RoomId, module.RoomTypeId);
                    tabControl.SelectedTabPage = tabControl.TabPages[index];
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static bool AddPluginPage(string pluginName, string pageName, object pluginInstance, bool isUsed)
        {
            bool success = false;
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws
                           .Where(o => o.ModuleLink == pluginName).FirstOrDefault();
                if (moduleData == null) Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + pluginName);
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    Inventec.Desktop.Common.Modules.Module currentModule = new Inventec.Desktop.Common.Modules.Module();
                    currentModule = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0);
                    TabCreating(SessionManager.GetTabControlMain(), pageName, pageName, (System.Windows.Forms.UserControl)pluginInstance, currentModule, SaveDataBeforeCloseV2, false);
                    PluginInstanceInitWorker.AddPlugin(pageName, pluginInstance, isUsed);
                    success = true;
                }
                Inventec.Common.Logging.LogSystem.Debug("AddPluginPage. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        internal static void TabCreating(XtraTabControl tabControl, string name, string text, UserControl uc, Inventec.Desktop.Common.Modules.Module module, SaveDataBeforeCloseExt saveData)
        {
            try
            {
                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((module));
                if (saveData != null
                    && !dicSaveData.ContainsKey(name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0")))
                {
                    dicSaveData.Add(name + "_" + (workPlace != null ? (workPlace.RoomId + "_" + workPlace.RoomTypeId) : "0"), saveData);
                }
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

        internal static void TabCreating(XtraTabControl tabControl, string name, string text, UserControl uc, Inventec.Desktop.Common.Modules.Module module, SaveDataBeforeCloseExt saveData, bool isVisible)
        {
            try
            {
                TabCreatingCustomFill(tabControl, name, text, uc, true, module, isVisible);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CloseSelectedTabPage(XtraTabControl tabControl)
        {
            try
            {
                if (tabControl != null)
                {
                    CloseCameraFormOpened();

                    XtraTabPage page = tabControl.SelectedTabPage;
                    if (page == null || tabControl == null) throw new ArgumentNullException("XtraTabPage or XtraTabControl is null");
                    string pageName = page.Text;
                    UserControl container = null;
                    if (page.Controls.Count > 0)
                    {
                        var control = page.Controls[0];

                        if (control != null && control is UserControl)
                        {
                            container = page.Controls[0] as UserControl;
                        }
                        else if (control != null && control is XtraUserControl)
                        {
                            container = page.Controls[0] as XtraUserControl;
                        }
                    }

                    int i = tabControl.SelectedTabPageIndex;
                    tabControl.TabPages.Remove(page);

                    tabControl.SelectedTabPageIndex = i - 1;
                    page.Controls.Clear();
                    if (container != null)
                    {
                        container.Dispose();
                        GC.SuppressFinalize(container);//TODO
                        container = null;

                    }
                    page.Controls.Clear();
                    page.Text = "";
                    page.Appearance.Header.Font.Dispose();
                    page.Tag = null;
                    page.Dispose();
                    GC.SuppressFinalize(page);//TODO
                    page = null;
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    //GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void CloseButtonHandlerClick(XtraTabPage page, XtraTabControl xtab, Action LoadGridMenuTabPageExt)
        {
            try
            {
                if (page == null || xtab == null) throw new ArgumentNullException("XtraTabPage or XtraTabControl is null");
                CloseCameraFormOpened();
                string pageName = page.Text;
                UserControl container = null;
                if (page.Controls.Count > 0)
                {
                    var control = page.Controls[0];

                    if (control != null && control is UserControl)
                    {
                        container = page.Controls[0] as UserControl;
                    }
                    else if (control != null && control is XtraUserControl)
                    {
                        container = page.Controls[0] as XtraUserControl;
                    }
                }

                int i = xtab.SelectedTabPageIndex;
                xtab.TabPages.Remove(page);
                xtab.SelectedTabPageIndex = i - 1;
                RefeshUserControlWorker(container, page);
                if (xtab.TabPages.Count >= 5 && xtab.CustomHeaderButtons.Count > 0)
                {
                    if (LoadGridMenuTabPageExt != null)
                        LoadGridMenuTabPageExt();
                    xtab.CustomHeaderButtons[0].Visible = true;
                }
                else
                {
                    xtab.CustomHeaderButtons[0].Visible = false;
                }
                page.Controls.Clear();
                if (container != null)
                {
                    container.Dispose();
                    GC.SuppressFinalize(container);//TODO
                    container = null;
                }

                page.Controls.Clear();
                page.Text = "";
                page.Appearance.Header.Font.Dispose();
                page.Tag = null;
                page.Dispose();
                GC.SuppressFinalize(page);//TODO
                page = null;
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void RefeshUserControlWorker(UserControl container, XtraTabPage page)
        {
            try
            {
                if (dicSaveData != null && dicSaveData.Count > 0)
                {
                    var saveDt = dicSaveData.FirstOrDefault(o => o.Key == page.Name);
                    if (saveDt.Key != null && saveDt.Value != null)
                    {
                        if (container != null)
                        {
                            saveDt.Value(container);
                            dicSaveData.Remove(page.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void CloseCurrentTabPage(XtraTabPage page, XtraTabControl tabControl)
        {
            try
            {
                if (page == null) throw new ArgumentNullException("XtraTabPage is null");

                CloseCameraFormOpened();
                string pageName = page.Text;
                UserControl container = null;
                if (page.Controls.Count > 0)
                {
                    var control = page.Controls[0];

                    if (control != null && control is UserControl)
                    {
                        container = page.Controls[0] as UserControl;
                    }
                    else if (control != null && control is XtraUserControl)
                    {
                        container = page.Controls[0] as XtraUserControl;
                    }
                }

                int i = tabControl.SelectedTabPageIndex;
                tabControl.TabPages.Remove(page);
                tabControl.SelectedTabPageIndex = i - 1;

                if (container != null)
                {
                    container.Dispose();
                    GC.SuppressFinalize(container);//TODO
                    container = null;
                }

                page.Controls.Clear();
                page.Text = "";
                page.Appearance.Header.Font.Dispose();
                page.Tag = null;
                page.Dispose();
                GC.SuppressFinalize(page);//TODO
                page = null;

                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Tắt camera đang mở
        /// </summary>
        internal static void CloseCameraFormOpened()
        {
            try
            {
                foreach (var f in Application.OpenForms.Cast<Form>().ToList())
                {
                    if (f.Name == "frmCamera")
                    {
                        try
                        {
                            f.Close();
                            break;
                        }
                        catch
                        {
                            if (!f.IsDisposed) f.Invoke(new MethodInvoker(delegate()
                                {
                                    f.Close();
                                }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

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
                    tabpage.Tag = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(module, module.RoomId, module.RoomTypeId);
                    tabpage.Appearance.Header.ForeColor = System.Drawing.Color.Green;
                    if (tabPageFont == null || (tabPageFont.Name != tabpage.Appearance.Header.Font.Name && tabPageFont.Size != tabpage.Appearance.Header.Font.Size))
                    {
                        tabPageFont = new System.Drawing.Font(tabpage.Appearance.Header.Font.Name, tabpage.Appearance.Header.Font.Size, System.Drawing.FontStyle.Bold);
                    }
                    tabpage.Appearance.Header.Font = tabPageFont;
                    tabControl.TabPages.Add(tabpage);
                    tabControl.SelectedTabPage = tabpage;
                    uc.Parent = tabpage;
                    uc.Show();
                    if (autoFill)
                        uc.Dock = DockStyle.Fill;
                    if (tabControl.TabPages.Count >= 5 && tabControl.CustomHeaderButtons.Count > 0)
                    {
                        var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                        if (formMain != null)
                        {
                            Type type = formMain.GetType();
                            if (type != null)
                            {
                                MethodInfo methodInfo__ResetAllTabpageToDefault = type.GetMethod("LoadGridMenuTabPageExt");
                                if (methodInfo__ResetAllTabpageToDefault != null)
                                    methodInfo__ResetAllTabpageToDefault.Invoke(formMain, null);
                            }
                        }
                        tabControl.CustomHeaderButtons[0].Visible = true;
                    }
                    else
                    {
                        tabControl.CustomHeaderButtons[0].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void TabCreatingCustomFill(XtraTabControl tabControl, string name, string text, UserControl uc, bool autoFill, Inventec.Desktop.Common.Modules.Module module, bool isVisible)
        {
            try
            {
                if (tabControl.Visible == false)
                {
                    tabControl.Visible = true;
                }

                MOS.SDO.WorkPlaceSDO workPlace = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace((module));

                XtraTabPage tabpage = new XtraTabPage { Text = text + (workPlace != null ? " (" + workPlace.RoomName + ")" : ""), Name = name };
                tabpage.Tag = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(module, module.RoomId, module.RoomTypeId);
                tabpage.Appearance.Header.ForeColor = System.Drawing.Color.Green;
                if (tabPageFont == null || (tabPageFont.Name != tabpage.Appearance.Header.Font.Name && tabPageFont.Size != tabpage.Appearance.Header.Font.Size))
                {
                    tabPageFont = new System.Drawing.Font(tabpage.Appearance.Header.Font.Name, tabpage.Appearance.Header.Font.Size, System.Drawing.FontStyle.Bold);
                }
                tabpage.Appearance.Header.Font = tabPageFont;
                tabpage.PageVisible = false;

                tabControl.TabPages.Add(tabpage);
                uc.Parent = tabpage;
                uc.Show();
                if (autoFill)
                    uc.Dock = DockStyle.Fill;
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
                    tabpage.Tag = HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(module, module.RoomId, module.RoomTypeId);
                    tabpage.Appearance.Header.ForeColor = System.Drawing.Color.Green;
                    if (tabPageFont == null || (tabPageFont.Name != tabpage.Appearance.Header.Font.Name && tabPageFont.Size != tabpage.Appearance.Header.Font.Size))
                    {
                        tabPageFont = new System.Drawing.Font(tabpage.Appearance.Header.Font.Name, tabpage.Appearance.Header.Font.Size, System.Drawing.FontStyle.Bold);
                    }
                    tabpage.Appearance.Header.Font = tabPageFont;
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
