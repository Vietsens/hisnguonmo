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
using DevExpress.XtraTab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class ModuleControlADO
    {
        public ModuleControlADO() { }
        public Control mControl { get; set; }
        public object lControl { get; set; }
        public string Title { get; set; }
        public string TitleLayout { get; set; }
        public object Value { get; set; }
        public string ControlKey { get; set; }
        public string ControlName { get; set; }
        public string ControlPath { get; set; }
        public string ControlType { get; set; }
        public bool IsVisible { get; set; }
        public bool IsChecked { get; set; }
        public string ParentControlName { get; set; }
        public string ParentControlKey { get; set; }
        public string CAPTION { get; set; }
        public string SHORTCUT { get; set; }
        public string CURRENT_SHORTCUT { get; set; }
        public string MODULE_LINK { get; set; }
        public string FUNCTION { get; set; }

        public bool IsTextVisible { get; set; }

    }


    public class ModuleControlProcess
    {
        bool hasIncludeControl;
        public ModuleControlProcess() { }

        public ModuleControlProcess(bool hasincludeControl)
        {
            this.hasIncludeControl = hasincludeControl;
        }

        public List<ModuleControlADO> GetControls(Control mainControl, List<ModuleControlADO> controlResultAlls = null, ModuleControlADO parent = null)
        {
            List<ModuleControlADO> controlResults = new List<ModuleControlADO>();
            try
            {
                // add parent 
                ModuleControlADO moduleParent = null;
                if (parent == null && mainControl != null)
                {
                    moduleParent = new ModuleControlADO();
                    if (this.hasIncludeControl)
                        moduleParent.mControl = mainControl;
                    moduleParent.ControlName = mainControl.Name;
                    moduleParent.ControlKey = mainControl.Name;
                    moduleParent.ControlType = mainControl.GetType().ToString();
                    moduleParent.ControlPath = mainControl.Name;
                    moduleParent.Title = mainControl.Text;
                    moduleParent.IsVisible = mainControl.Visible;
                    moduleParent.ParentControlKey = "";
                    moduleParent.ParentControlName = "";

                    controlResults.Add(moduleParent);
                }

                foreach (Control itemCtrl in mainControl.Controls)
                {
                    if (parent == null && itemCtrl.Parent.Name != mainControl.Name)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("itemCtrl.GetType()", itemCtrl.GetType().ToString())
                        + "___" + Inventec.Common.Logging.LogUtil.TraceData("itemCtrl.Name", itemCtrl.Name)
                        + "___" + Inventec.Common.Logging.LogUtil.TraceData("itemCtrl.Parent.Name", itemCtrl.Parent.Name)
                        + "___" + Inventec.Common.Logging.LogUtil.TraceData("mainControl.Name", mainControl.Name));
                        continue;
                    }

                    if (IsControlTypeAccept(itemCtrl))
                    {
                        ModuleControlADO moduleControl = new ModuleControlADO();
                        if (this.hasIncludeControl)
                            moduleControl.mControl = itemCtrl;
                        moduleControl.ControlName = itemCtrl.Name;
                        moduleControl.ControlKey = (parent != null ? parent.ControlPath + "." : "") + itemCtrl.Name;
                        moduleControl.ControlType = itemCtrl.GetType().ToString();
                        moduleControl.ControlPath = (parent != null ? parent.ControlPath + "." : "") + itemCtrl.Name;
                        ProcessTitleValue(moduleControl, itemCtrl, null);
                        moduleControl.IsVisible = itemCtrl.Visible;
                        if (moduleParent != null && parent == null && mainControl != null)
                        {
                            moduleControl.ParentControlKey = moduleParent.ControlKey;
                            moduleControl.ParentControlName = moduleParent.ControlName;
                        }
                        else if (parent != null)
                        {

                            moduleControl.ParentControlKey = parent.ControlKey;
                            moduleControl.ParentControlName = parent.ControlName;
                        }
                        if (controlResultAlls != null)
                            controlResultAlls.Add(moduleControl);
                        else
                            controlResults.Add(moduleControl);

                        if (itemCtrl is DevExpress.XtraLayout.LayoutControl)
                        {
                            if (controlResultAlls != null)
                                ProcessLayoutControlGroup(((DevExpress.XtraLayout.LayoutControl)itemCtrl).Root, controlResultAlls, moduleControl);
                            else
                                ProcessLayoutControlGroup(((DevExpress.XtraLayout.LayoutControl)itemCtrl).Root, controlResults, moduleControl);
                        }
                        else if (itemCtrl is DevExpress.XtraTab.XtraTabControl)
                        {
                            if (controlResultAlls != null)
                                ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)itemCtrl), controlResultAlls, moduleControl);
                            else
                                ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)itemCtrl), controlResults, moduleControl);
                        }
                        else if (itemCtrl is UserControl
                        || itemCtrl is DevExpress.XtraEditors.XtraUserControl
                        || itemCtrl is DevExpress.XtraEditors.PanelControl
                        || itemCtrl is DevExpress.XtraEditors.XtraScrollableControl
                        || itemCtrl is System.Windows.Forms.Panel
                        || itemCtrl is System.Windows.Forms.GroupBox
                        || itemCtrl is DevExpress.XtraNavBar.NavBarControl
                        || itemCtrl is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                        {
                            if (controlResultAlls != null)
                                GetControls(itemCtrl, controlResultAlls, moduleControl);
                            else
                                GetControls(itemCtrl, controlResults, moduleControl);
                        }
                        else
                        {
                            if (controlResultAlls != null)
                                SearchControlByNameForAll(controlResultAlls, itemCtrl, moduleControl);
                            else
                                SearchControlByNameForAll(controlResults, itemCtrl, moduleControl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return controlResults;
        }

        public IEnumerable<Control> GetControlHierarchy(Control root)
        {
            var queue = new Queue<Control>();
            queue.Enqueue(root);
            do
            {
                var control = queue.Dequeue();

                yield return control;

                foreach (var child in control.Controls.OfType<Control>())
                    queue.Enqueue(child);
            } while (queue.Count > 0);
        }

        private void ProcessTabcontrol(DevExpress.XtraTab.XtraTabControl tabControl, List<ModuleControlADO> controlResults, ModuleControlADO parent)
        {
            foreach (XtraTabPage item in tabControl.TabPages)
            {
                ModuleControlADO moduleControlGroup = new ModuleControlADO();
                moduleControlGroup.ControlName = ((XtraTabPage)item).Name;
                moduleControlGroup.ControlKey = (parent != null ? parent.ControlPath + "." : "") + ((XtraTabPage)item).Name;
                moduleControlGroup.ControlType = item.GetType().ToString();
                moduleControlGroup.ControlPath = (parent != null ? parent.ControlPath + "." : "") + ((XtraTabPage)item).Name;
                moduleControlGroup.Title = ((XtraTabPage)item).Text;
                moduleControlGroup.IsVisible = ((XtraTabPage)item).Visible;
                if (parent != null)
                {
                    moduleControlGroup.ParentControlKey = parent.ControlKey;
                    moduleControlGroup.ParentControlName = parent.ControlName;
                }
                if (this.hasIncludeControl)
                    moduleControlGroup.lControl = item;
                controlResults.Add(moduleControlGroup);

                GetControls(item, controlResults, moduleControlGroup);
            }
        }

        private void ProcessLayoutControlGroup(LayoutControlGroup group, List<ModuleControlADO> controlResults, ModuleControlADO parent)
        {
            ModuleControlADO moduleControl = new ModuleControlADO();
            moduleControl.ControlName = group.Name;
            moduleControl.ControlKey = (parent != null ? parent.ControlPath + "." : "") + group.Name;
            moduleControl.ControlType = group.GetType().ToString();
            moduleControl.ControlPath = (parent != null ? parent.ControlPath + "." : "") + group.Name;
            moduleControl.Title = group.Text;
            moduleControl.IsVisible = group.Visible;
            if (this.hasIncludeControl)
                moduleControl.lControl = group;
            if (parent != null)
            {
                moduleControl.ParentControlKey = parent.ControlKey;
                moduleControl.ParentControlName = parent.ControlName;
            }
            controlResults.Add(moduleControl);

            foreach (var item in group.Items)
            {
                if (item is LayoutControlGroup)
                {
                    ProcessLayoutControlGroup((LayoutControlGroup)item, controlResults, moduleControl);
                }
                if (item is TabbedControlGroup)
                {
                    ModuleControlADO moduleControlGroup = new ModuleControlADO();
                    moduleControlGroup.ControlName = ((TabbedControlGroup)item).Name;
                    moduleControlGroup.ControlKey = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((TabbedControlGroup)item).Name;
                    moduleControlGroup.ControlType = item.GetType().ToString();
                    moduleControlGroup.ControlPath = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((TabbedControlGroup)item).Name;
                    moduleControlGroup.Title = ((TabbedControlGroup)item).Text;
                    moduleControlGroup.IsVisible = ((TabbedControlGroup)item).Visible;
                    if (moduleControl != null)
                    {
                        moduleControlGroup.ParentControlKey = moduleControl.ControlKey;
                        moduleControlGroup.ParentControlName = moduleControl.ControlName;
                    }
                    if (this.hasIncludeControl)
                        moduleControlGroup.lControl = item;
                    controlResults.Add(moduleControlGroup);

                    ProcessTabbedGroup((TabbedControlGroup)item, controlResults, moduleControlGroup);
                }
                if (item is LayoutControlItem)
                {
                    ModuleControlADO moduleControlGroup = new ModuleControlADO();
                    moduleControlGroup.ControlName = ((LayoutControlItem)item).Name;
                    moduleControlGroup.ControlKey = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((LayoutControlItem)item).Name;
                    moduleControlGroup.ControlType = item.GetType().ToString();
                    moduleControlGroup.ControlPath = (moduleControl != null ? moduleControl.ControlPath + "." : "") + ((LayoutControlItem)item).Name;
                    moduleControlGroup.Title = ((LayoutControlItem)item).Text;
                    moduleControlGroup.IsVisible = ((LayoutControlItem)item).Visible;
                    moduleControlGroup.IsTextVisible = ((LayoutControlItem)item).TextVisible;
                    if (this.hasIncludeControl)
                        moduleControlGroup.lControl = item;
                    if (moduleControl != null)
                    {
                        moduleControlGroup.ParentControlKey = moduleControl.ControlKey;
                        moduleControlGroup.ParentControlName = moduleControl.ControlName;
                    }
                    controlResults.Add(moduleControlGroup);

                    ProcessLayoutControlItem((LayoutControlItem)item, controlResults, moduleControlGroup);
                }
            }
        }

        private void ProcessTabbedGroup(TabbedControlGroup group, List<ModuleControlADO> controlResults, ModuleControlADO parent)
        {
            if (group != null && group.TabPages != null)
            {
                foreach (LayoutControlGroup page in group.TabPages)
                {
                    ModuleControlADO moduleControl = new ModuleControlADO();
                    moduleControl.ControlName = page.Name;
                    moduleControl.ControlKey = (parent != null ? parent.ControlPath + "." : "") + page.Name;
                    moduleControl.ControlType = page.GetType().ToString();
                    moduleControl.ControlPath = (parent != null ? parent.ControlPath + "." : "") + page.Name;
                    moduleControl.Title = page.Text;
                    moduleControl.IsVisible = page.Visible;
                    if (this.hasIncludeControl)
                        moduleControl.lControl = page;
                    if (parent != null)
                    {
                        moduleControl.ParentControlKey = parent.ControlKey;
                        moduleControl.ParentControlName = parent.ControlName;
                    }
                    controlResults.Add(moduleControl);

                    ProcessLayoutControlGroup(page, controlResults, moduleControl);
                }
            }
        }

        private void ProcessLayoutControlItem(LayoutControlItem item, List<ModuleControlADO> controlResults, ModuleControlADO parent)
        {
            Control itemChild = item.Control;
            if (itemChild != null && IsControlTypeAccept(itemChild))
            {
                ModuleControlADO moduleControl = new ModuleControlADO();
                if (this.hasIncludeControl)
                    moduleControl.mControl = itemChild;
                moduleControl.ControlName = itemChild.Name;
                moduleControl.ControlKey = (parent != null ? parent.ControlPath + "." : "") + itemChild.Name;
                moduleControl.ControlType = itemChild.GetType().ToString();
                moduleControl.ControlPath = (parent != null ? parent.ControlPath + "." : "") + itemChild.Name;
                moduleControl.Title = itemChild.Text;
                moduleControl.IsVisible = itemChild.Visible;
                if (this.hasIncludeControl)
                    moduleControl.lControl = itemChild;
                if (parent != null)
                {
                    moduleControl.ParentControlKey = parent.ControlKey;
                    moduleControl.ParentControlName = parent.ControlName;
                }
                controlResults.Add(moduleControl);

                if (itemChild is LayoutControl)
                {
                    ProcessLayoutControlGroup(((LayoutControl)itemChild).Root, controlResults, moduleControl);
                }
                else if (itemChild is DevExpress.XtraTab.XtraTabControl)
                {
                    ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)itemChild), controlResults, moduleControl);
                }
                else if (itemChild is UserControl
                    || itemChild is DevExpress.XtraEditors.XtraUserControl
                    || itemChild is DevExpress.XtraEditors.PanelControl
                    || itemChild is DevExpress.XtraEditors.XtraScrollableControl
                    || itemChild is Panel
                    || itemChild is GroupBox
                    || itemChild is DevExpress.XtraNavBar.NavBarControl
                    || itemChild is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper
                    )
                {
                    GetControls(itemChild, controlResults, moduleControl);
                }
            }
        }

        private void ProcessTitleValue(ModuleControlADO moduleControl, Control itemChild, LayoutControlItem item)
        {
            if (itemChild is DevExpress.XtraEditors.SimpleButton
                || itemChild is DevExpress.XtraEditors.DropDownButton
                || itemChild is DevExpress.XtraLayout.LayoutControl
                || itemChild is ButtonBase
                || itemChild is DevExpress.XtraTab.XtraTabPage
                || itemChild is DevExpress.XtraEditors.BaseButton)
            {
                moduleControl.Value = "";
                moduleControl.Title = itemChild.Text;
                moduleControl.TitleLayout = item != null ? item.Text : "";
            }
            else
            {
                moduleControl.Value = itemChild.Text;
                moduleControl.Title = item != null ? item.Text : "";
                moduleControl.TitleLayout = item != null ? item.Text : "";
            }
        }

        private bool IsControlTypeAccept(Control control)
        {
            bool valid = true;
            if (control is DevExpress.XtraBars.BarDockControl
                || control is DevExpress.XtraLayout.Helpers.FakeFocusContainer
                //|| control is DevExpress.XtraLayout.LayoutControl
                || control.GetType().ToString().Contains("DevExpress.XtraLayout.Scrolling"))
            {
                valid = false;
            }
            return valid;
        }

        private void SearchControlByNameForAll(List<ModuleControlADO> controlResults, Control control, ModuleControlADO parent = null)
        {
            Control.ControlCollection childControl = control.Controls;

            if (childControl != null && childControl.Count > 0)
            {
                foreach (Control itemChild in childControl)
                {
                    if (IsControlTypeAccept(itemChild))
                    {
                        ModuleControlADO moduleControl = new ModuleControlADO();
                        if (this.hasIncludeControl)
                            moduleControl.mControl = itemChild;
                        moduleControl.ControlName = itemChild.Name;
                        moduleControl.ControlKey = (parent != null ? parent.ControlPath + "." : "") + itemChild.Name;
                        moduleControl.ControlType = itemChild.GetType().ToString();
                        moduleControl.ControlPath = (parent != null ? parent.ControlPath + "." : "") + itemChild.Name;
                        moduleControl.Value = itemChild.Text;
                        moduleControl.IsVisible = itemChild.Visible;
                        if (parent != null)
                        {
                            moduleControl.ParentControlKey = parent.ControlKey;
                            moduleControl.ParentControlName = parent.ControlName;
                        }
                        controlResults.Add(moduleControl);
                        if (itemChild.Controls != null && itemChild.Controls.Count > 0)
                        {
                            SearchControlByNameForAll(controlResults, itemChild, moduleControl);
                        }
                    }
                }
            }

        }
    }
}
