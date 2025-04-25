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
using HIS.Desktop.LocalStorage.ConfigHideControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class HideControlInControlProcess
    {
        public HideControlInControlProcess() { }
        string ModuleLink;
        List<string> HideControls { get; set; }

        public void Run(Control control, string moduleLink)
        {
            try
            {
                this.ModuleLink = moduleLink;
                if (control == null || control.IsDisposed)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CustomizaButtonAndRestoreLayoutInControlProcess: control == null || control.IsDisposed" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink));
                }
                var hControls = ConfigHideControlWorker.GetByModule(this.ModuleLink);

                this.HideControls = (hControls != null && hControls.Count > 0) ? hControls.Select(o => o.CONTROL_PATH).ToList() : null;
                if (this.HideControls != null && this.HideControls.Count > 0)
                {
                    //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.HideControls), this.HideControls));
                    foreach (Control c in control.Controls)
                    {
                        if (this.HideControls != null && this.HideControls.Any(o => o == c.Name))
                        {
                            c.Visible = false;
                            Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + c.Name + " is Visibility = Never");
                        }
                        else
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
                            || c is GroupBox
                            || c is DevExpress.XtraNavBar.NavBarControl
                            || c is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                            {
                                ProcessControlByType(c, c.Name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessControlByType(Control itemCtrl, string parentName)
        {
            try
            {
                foreach (Control item in itemCtrl.Controls)
                {
                    string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + item.Name;
                    if (this.HideControls != null && this.HideControls.Any(o => o == parentPath))
                    {
                        item.Visible = false;
                        Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Visibility = Never");
                    }
                    else
                    {
                        if (item is DevExpress.XtraLayout.LayoutControl)
                        {
                            ProcessLayoutControlGroup(((LayoutControl)item).Root, parentPath);
                        }
                        else if (itemCtrl is DevExpress.XtraTab.XtraTabControl)
                        {
                            ProcessTabcontrol(((DevExpress.XtraTab.XtraTabControl)itemCtrl), parentPath);
                        }
                        else if (item is UserControl
                        || item is DevExpress.XtraEditors.XtraUserControl
                        || item is DevExpress.XtraEditors.PanelControl
                        || item is DevExpress.XtraEditors.XtraScrollableControl
                        || item is Panel
                        || item is GroupBox
                        || item is DevExpress.XtraNavBar.NavBarControl
                        || item is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                        {
                            ProcessControlByType(item, parentPath);
                        }
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
            foreach (XtraTabPage item in tabControl.TabPages)
            {
                string parentPathChild = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + ((XtraTabPage)item).Name;

                if (this.HideControls != null && this.HideControls.Any(o => o == parentPathChild))
                {
                    item.Visible = false;
                    ((XtraTabPage)item).PageVisible = false;
                    Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Visibility = Never");
                }

                ProcessControlByType(item, parentPathChild);
            }
        }

        private void ProcessLayoutControlGroup(LayoutControlGroup group, string parentName = "")
        {
            if (group != null)
            {
                string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + group.Name;
                if (this.HideControls != null && this.HideControls.Any(o => o == parentPath))
                {
                    group.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + group.Name + " is Visibility = Never");
                }
                else
                {
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
        }

        private void ProcessTabbedGroup(TabbedControlGroup group, string parentName = "")
        {
            string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + group.Name;
            if (this.HideControls != null && this.HideControls.Any(o => o == parentPath))
            {
                group.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + group.Name + " is Visibility = Never");
            }
            else
            {
                foreach (LayoutControlGroup page in group.TabPages)
                    ProcessLayoutControlGroup(page, parentPath);
            }
        }

        private void ProcessLayoutControlItem(LayoutControlItem item, string parentName = "")
        {
            try
            {
                if (item == null) return;

                string parentPath = (!String.IsNullOrEmpty(parentName) ? parentName + "." : "") + item.Name;
                if (this.HideControls != null && this.HideControls.Any(o => o == parentPath))
                {
                    item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Visibility = Never");
                }
                else
                {
                    if (item != null && item.Control != null)
                    {
                        string parentPathChild = (!String.IsNullOrEmpty(parentPath) ? parentPath + "." : "") + item.Control.Name;
                        if (this.HideControls != null && this.HideControls.Any(o => o == parentPathChild))
                        {
                            item.Control.Visible = false;
                            item.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                            Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + item.Control.Name + " is Visibility = Never");
                        }
                        else
                        {
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
                            || item.Control is GroupBox
                            || item.Control is DevExpress.XtraNavBar.NavBarControl
                            || item.Control is DevExpress.XtraNavBar.NavBarGroupControlContainerWrapper)
                            {
                                ProcessControlByType(item.Control, parentPathChild);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessControlItem(Control item)
        {
            try
            {
                if (item != null && this.HideControls != null && this.HideControls.Any(o => o == item.Name))
                {
                    item.Visible = false;
                    Inventec.Common.Logging.LogSystem.Debug("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Visible = false");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
