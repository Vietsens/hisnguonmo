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
using DevExpress.XtraLayout;
using DevExpress.XtraTab;
using HIS.Desktop.LocalStorage.ConfigCustomizaButton;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    internal class CustomizaButtonAndRestoreLayoutInControlProcess
    {
        const string CONFIG_KEY__HIS_KEY__APPLY_RESTORE_LAYOUT = "HIS.Desktop.ApplyRestoreLayout.ModuleLinks";
        internal CustomizaButtonAndRestoreLayoutInControlProcess() { }
        string ModuleLink;
        bool isRestoreLayout;
        List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_BUTTON> ControlEditorCustomizaControls { get; set; }
        List<DevExpress.XtraBars.BarManager> barManagerRunning;

        internal bool IsAllowRestoreLayout(string moduleLink)
        {
            bool isRestoreLayout = false;
            try
            {
                string moduleLinksApplys = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HIS_KEY__APPLY_RESTORE_LAYOUT));
                var arrModuleLinkApplys = !String.IsNullOrEmpty(moduleLinksApplys) ? moduleLinksApplys.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                isRestoreLayout = arrModuleLinkApplys != null && arrModuleLinkApplys.Count > 0 && !String.IsNullOrEmpty(moduleLink) && arrModuleLinkApplys.Contains(moduleLink);

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleLink), moduleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleLinksApplys), moduleLinksApplys) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRestoreLayout), isRestoreLayout));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isRestoreLayout;
        }

        internal void Run(Control control, string moduleLink, List<DevExpress.XtraBars.BarManager> barmanagers = null)
        {
            try
            {
                if (control == null || control.IsDisposed)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CustomizaButtonAndRestoreLayoutInControlProcess: control == null || control.IsDisposed" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink));
                }

                this.ModuleLink = moduleLink;
                this.barManagerRunning = barmanagers;
                this.ControlEditorCustomizaControls = ConfigCustomizaButtonWorker.GetByModule(this.ModuleLink);
                //string moduleLinksApplys = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HIS_KEY__APPLY_RESTORE_LAYOUT));
                //var arrModuleLinkApplys = !String.IsNullOrEmpty(moduleLinksApplys) ? moduleLinksApplys.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries).ToList() : null;
                isRestoreLayout = IsAllowRestoreLayout(this.ModuleLink);
                if ((this.ControlEditorCustomizaControls != null && this.ControlEditorCustomizaControls.Count > 0) || isRestoreLayout)
                {
                    //Xử lý customiza caption button & Restore Layout
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ControlEditorCustomizaControls), this.ControlEditorCustomizaControls) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isRestoreLayout), isRestoreLayout));
                    foreach (Control c in control.Controls)
                    {
                        if (c is LayoutControl)
                        {
                            ProcessLayoutControlGroup(((LayoutControl)c).Root, c.Name);
                        }
                        else if (c is DevExpress.XtraTab.XtraTabControl)
                        {
                            ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)c), c.Name);
                        }
                        else if (c is UserControl
                        || c is DevExpress.XtraEditors.XtraUserControl
                        || c is DevExpress.XtraEditors.PanelControl
                        || c is DevExpress.XtraEditors.XtraScrollableControl
                        || c is Panel
                         || c is GroupBox)
                        {
                            ProcessControlByType(c, c.Name);
                        }
                        else if (c is DevExpress.XtraEditors.SimpleButton
                            || c is System.Windows.Forms.Button
                            || c is DevExpress.XtraTab.XtraTabPage
                            || c is System.Windows.Forms.TabPage
                            || c is DevExpress.XtraEditors.CheckEdit
                            || c is System.Windows.Forms.CheckBox)
                        {
                            var hbtn = this.ControlEditorCustomizaControls.Where(o => o.BUTTON_PATH == c.Name).FirstOrDefault();
                            if (hbtn != null && !String.IsNullOrEmpty(hbtn.CAPTION))
                            {
                                c.Text = hbtn.CAPTION;

                                if (c is DevExpress.XtraEditors.SimpleButton || c is Button)
                                {
                                    //c.Text = hbtn.CAPTION;
                                    DevExpress.XtraEditors.SimpleButton sb = c as DevExpress.XtraEditors.SimpleButton;
                                    if (c is DevExpress.XtraEditors.SimpleButton && sb != null && !String.IsNullOrEmpty(sb.ToolTip))
                                    {
                                        sb.ToolTip = hbtn.CAPTION;
                                    }
                                }
                                else if (c is DevExpress.XtraTab.XtraTabPage)
                                {
                                    var glc = (DevExpress.XtraTab.XtraTabPage)c;

                                }
                                else if (c is DevExpress.XtraEditors.CheckEdit)
                                {
                                    var glc = (DevExpress.XtraEditors.CheckEdit)c;

                                }
                                Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + c.Name + " is Text = " + hbtn.CAPTION);
                            }
                        }
                        else if (isRestoreLayout && c is DevExpress.XtraGrid.GridControl)
                        {
                            RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                            DevExpress.XtraGrid.Views.Grid.GridView gridView = (c as DevExpress.XtraGrid.GridControl).MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ModuleLink), ModuleLink) + Inventec.Common.Logging.LogUtil.TraceData("gridView.name", gridView != null ? gridView.Name : ""));
                            restoreLayoutProcess.InitRestoreLayoutGridViewFromXml(gridView, gridView.Name);//StringUtil.Base64Encode(gridView.Name)
                        }
                        else if (isRestoreLayout && c is DevExpress.XtraTreeList.TreeList)
                        {
                            RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                            restoreLayoutProcess.InitRestoreLayoutTreeListFromXml((c as DevExpress.XtraTreeList.TreeList), StringUtil.Base64Encode(c.Name));
                        }
                    }


                    //Xử lý customiza shortcut button với các module dạng form (module dạng uc chỗ xử lý customiza shortcut xử lý ở chỗ khác không xử lý ở đây)        
                    //Vì đặc thù riêng của các module dạng form là dùng BarManager để tạo phím tắt, thông thể lấy động từ control và lớp base đã cung cấp hàm để các chức năng gọi truyền vào BarManager để bên trong này sử dụng
                    if (this.barManagerRunning != null && this.barManagerRunning.Count > 0)
                    {
                        foreach (BarManager manager in this.barManagerRunning)
                        {
                            //TODO
                            foreach (DevExpress.XtraBars.BarItem item in manager.Items)
                            {
                                if (item.GetType() == typeof(DevExpress.XtraBars.BarButtonItem) && item.ItemShortcut.Key != Keys.None)
                                {
                                    foreach (var bcus in this.ControlEditorCustomizaControls)
                                    {
                                        if (!String.IsNullOrEmpty(bcus.CURRENT_SHORTCUT) && !String.IsNullOrEmpty(bcus.SHORTCUT))
                                        {
                                            try
                                            {
                                                var btnKeyStroke = XKeysConverter.Parse(bcus.CURRENT_SHORTCUT);
                                                var oldShortcutKeys = (System.Windows.Forms.Keys)(btnKeyStroke);
                                                var newKey = (System.Windows.Forms.Keys)XKeysConverter.Parse(bcus.SHORTCUT);
                                                if (item.ItemShortcut.Key == oldShortcutKeys)
                                                {
                                                    item.ItemShortcut = new BarShortcut(newKey);
                                                    Inventec.Common.Logging.LogSystem.Info("this.ModuleLink=" + this.ModuleLink + "." + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => oldShortcutKeys), oldShortcutKeys.ToString()) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => newKey), item.ItemShortcut.ToString()));
                                                }
                                            }
                                            catch (Exception exx)
                                            {
                                                Inventec.Common.Logging.LogSystem.Warn("Gan shortcut cho nut that bai____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bcus), bcus) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ModuleLink), ModuleLink), exx);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " barManagerRunning.Count = 0");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessTabcontrol(DevExpress.XtraTab.XtraTabControl tabControl, string parentName)
        {
            //string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + tabControl.Name;

            foreach (XtraTabPage item in tabControl.TabPages)
            {
                //ModuleControlADO moduleControlGroup = new ModuleControlADO();
                //moduleControlGroup.ControlName = ((XtraTabPage)item).Name;
                //moduleControlGroup.ControlKey = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((XtraTabPage)item).Name;
                //moduleControlGroup.ControlType = item.GetType().ToString();
                //moduleControlGroup.ControlPath = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((XtraTabPage)item).Name;
                //moduleControlGroup.Title = ((XtraTabPage)item).Text;
                //moduleControlGroup.IsVisible = ((XtraTabPage)item).Visible;
                //if (moduleControl != null)
                //{
                //    moduleControlGroup.ParentControlKey = moduleControl.ControlKey;
                //    moduleControlGroup.ParentControlName = moduleControl.ControlName;
                //}

                //moduleControlGroup.lControl = item;

                string parentPathChild = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + ((XtraTabPage)item).Name;

                var hbtnLC = this.ControlEditorCustomizaControls.Where(o => o.BUTTON_PATH == parentPathChild).FirstOrDefault();
                if (hbtnLC != null && !String.IsNullOrEmpty(hbtnLC.CAPTION) && ((XtraTabPage)item).PageVisible)
                {
                    item.Text = hbtnLC.CAPTION;
                    Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + ((XtraTabPage)item).Name + " is Text = " + hbtnLC.CAPTION);
                }

                ProcessControlByType(item, parentPathChild);
            }
        }


        private void ProcessControlByType(Control itemCtrl, string parentName)
        {
            try
            {
                foreach (Control item in itemCtrl.Controls)
                {
                    string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + item.Name;
                    if (item is DevExpress.XtraLayout.LayoutControl)
                    {
                        ProcessLayoutControlGroup(((LayoutControl)item).Root, parentPath);
                    }
                    else if (item is DevExpress.XtraTab.XtraTabControl)
                    {
                        ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)item), parentPath);
                    }
                    else if (item is UserControl
                    || item is DevExpress.XtraEditors.XtraUserControl
                    || item is DevExpress.XtraEditors.PanelControl
                    || item is DevExpress.XtraEditors.XtraScrollableControl
                    || item is Panel
                    || item is GroupBox)
                    {
                        ProcessControlByType(item, parentPath);
                    }
                    else if (item is DevExpress.XtraEditors.SimpleButton
                            || item is System.Windows.Forms.Button
                            || item is DevExpress.XtraTab.XtraTabPage
                            || item is TabPage
                            || item is DevExpress.XtraEditors.CheckEdit
                            || item is System.Windows.Forms.CheckBox)
                    {
                        var hbtn = this.ControlEditorCustomizaControls.Where(o => o.BUTTON_PATH == parentPath).FirstOrDefault();
                        if (hbtn != null && !String.IsNullOrEmpty(hbtn.CAPTION))
                        {
                            item.Text = hbtn.CAPTION;
                            DevExpress.XtraEditors.SimpleButton sb = item as DevExpress.XtraEditors.SimpleButton;
                            if (item is DevExpress.XtraEditors.SimpleButton && sb != null && !String.IsNullOrEmpty(sb.ToolTip))
                            {
                                sb.ToolTip = hbtn.CAPTION;
                            }
                            Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Text = " + hbtn.CAPTION);
                        }
                    }
                    else if (isRestoreLayout && item is DevExpress.XtraGrid.GridControl)
                    {
                        RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                        DevExpress.XtraGrid.Views.Grid.GridView gridView = (item as DevExpress.XtraGrid.GridControl).MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                        restoreLayoutProcess.InitRestoreLayoutGridViewFromXml(gridView, parentPath);
                    }
                    else if (isRestoreLayout && item is DevExpress.XtraTreeList.TreeList)
                    {
                        RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                        restoreLayoutProcess.InitRestoreLayoutTreeListFromXml((item as DevExpress.XtraTreeList.TreeList), parentPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessLayoutControlGroup(LayoutControlGroup group, string parentName = "")
        {
            if (group != null)
            {
                string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + group.Name;
                foreach (var item in group.Items)
                {
                    if (item is LayoutControlGroup)
                        ProcessLayoutControlGroup((LayoutControlGroup)item, parentPath);
                    if (item is TabbedControlGroup)
                        ProcessTabbedGroup((TabbedControlGroup)item, parentPath);
                    if (item is LayoutControlItem)
                        ProcessLayoutControlItem(((LayoutControlItem)item), parentPath);
                }
            }
        }

        private void ProcessTabbedGroup(TabbedControlGroup group, string parentName = "")
        {
            string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + group.Name;
            foreach (LayoutControlGroup page in group.TabPages)
                ProcessLayoutControlGroup(page, parentPath);
        }

        private void ProcessLayoutControlItem(LayoutControlItem item, string parentName = "")
        {
            try
            {
                if (item == null) return;

                string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + item.Name;
                if (item != null && item.Control != null)
                {
                    string parentPathChild = (!String.IsNullOrEmpty(parentPath) ? parentPath + "." : "") + item.Control.Name;

                    var hbtnLC = this.ControlEditorCustomizaControls.Where(o => o.BUTTON_PATH == parentPath).FirstOrDefault();
                    if (hbtnLC != null && !String.IsNullOrEmpty(hbtnLC.CAPTION) && item.TextVisible)
                    {
                        item.Text = hbtnLC.CAPTION;
                        item.OptionsToolTip.ToolTip = hbtnLC.CAPTION;
                        Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Text = " + hbtnLC.CAPTION);
                    }

                    if (item.Control is DevExpress.XtraLayout.LayoutControl)
                    {
                        ProcessLayoutControlGroup(((LayoutControl)item.Control).Root, parentPathChild);
                    }
                    else if (item.Control is DevExpress.XtraTab.XtraTabControl)
                    {
                        ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)item.Control), parentPathChild);
                    }
                    else if (item.Control is UserControl
                    || item.Control is DevExpress.XtraEditors.XtraUserControl
                    || item.Control is DevExpress.XtraEditors.PanelControl
                    || item.Control is DevExpress.XtraEditors.XtraScrollableControl
                    || item.Control is Panel
                    || item.Control is GroupBox)
                    {
                        ProcessControlByType(item.Control, parentPathChild);
                    }
                    else if (
                            item.Control is DevExpress.XtraEditors.SimpleButton
                            || item.Control is System.Windows.Forms.Button
                            || item.Control is DevExpress.XtraTab.XtraTabPage
                            || item.Control is TabPage
                            || item.Control is DevExpress.XtraEditors.CheckEdit
                            || item.Control is System.Windows.Forms.CheckBox)
                    {
                        var hbtn = this.ControlEditorCustomizaControls.Where(o => o.BUTTON_PATH == parentPathChild).FirstOrDefault();
                        if (hbtn != null && !String.IsNullOrEmpty(hbtn.CAPTION))
                        {
                            item.Control.Text = hbtn.CAPTION;
                            if (item.Control is DevExpress.XtraEditors.SimpleButton)
                            {
                                DevExpress.XtraEditors.SimpleButton sb = item.Control as DevExpress.XtraEditors.SimpleButton;
                                if (item.Control is DevExpress.XtraEditors.SimpleButton && sb != null && !String.IsNullOrEmpty(sb.ToolTip))
                                {
                                    sb.ToolTip = hbtn.CAPTION;
                                }
                            }
                            Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + item.Control.Name + " is Text = " + hbtn.CAPTION);
                        }
                    }
                    else if (isRestoreLayout && item.Control is DevExpress.XtraGrid.GridControl)
                    {
                        RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                        DevExpress.XtraGrid.Views.Grid.GridView gridView = (item.Control as DevExpress.XtraGrid.GridControl).MainView as DevExpress.XtraGrid.Views.Grid.GridView;
                        restoreLayoutProcess.InitRestoreLayoutGridViewFromXml(gridView, parentPathChild);
                    }
                    else if (isRestoreLayout && item.Control is DevExpress.XtraTreeList.TreeList)
                    {
                        RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                        restoreLayoutProcess.InitRestoreLayoutTreeListFromXml(item.Control as DevExpress.XtraTreeList.TreeList, parentPathChild);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
