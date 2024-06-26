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
    internal class ModuleControlDispose
    {
        internal static void DisposeAllControl(List<ModuleControlADO> moduleControls)
        {
            if (moduleControls != null && moduleControls.Count > 0)
            {
                var mdChilds = moduleControls.Where(o => o.mControl != null && o.ControlType != "DevExpress.XtraLayout.LayoutControl"
                    && o.ControlType != "DevExpress.XtraLayout.LayoutControlGroup"
                    && o.ControlType != "DevExpress.XtraLayout.LayoutControlItem"
                    && o.ControlType != "System.Windows.Forms.UserControl"
                    && o.ControlType != "DevExpress.XtraEditors.XtraUserControl"
                    && o.ControlType != "DevExpress.XtraEditors.PanelControl"
                    && o.ControlType != "DevExpress.XtraEditors.XtraScrollableControl"
                    && o.ControlType != "System.Windows.Forms.Panel"
                    && o.ControlType != "System.Windows.Forms.GroupBox").ToList();

                if (mdChilds != null && mdChilds.Count > 0)
                {
                    foreach (var moduleControlADO in mdChilds)
                    {
                        string controlName = moduleControlADO.ControlType + "_" + moduleControlADO.Title + "_" + moduleControlADO.CAPTION;
                        //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls[" + i + "].Name=" + itemDispose.Name + "____length1=" + length1);
                        DisposeAllChildOfControl(moduleControlADO.mControl);
                        try
                        {
                            moduleControlADO.mControl.Dispose();
                            GC.SuppressFinalize(moduleControlADO.mControl);
                            moduleControlADO.mControl = null;
                            Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                        }
                        catch (Exception exxx2)
                        {
                            Inventec.Common.Logging.LogSystem.Error("exxx2:itemDispose.Dispose", exxx2);
                        }
                    }
                }

                var mdParents = moduleControls.Where(o => o.mControl != null && !String.IsNullOrEmpty(o.ParentControlKey) && (o.ControlType == "DevExpress.XtraLayout.LayoutControl"
                    || o.ControlType == "DevExpress.XtraLayout.LayoutControlGroup"
                    || o.ControlType == "DevExpress.XtraLayout.LayoutControlItem"
                    || o.ControlType == "System.Windows.Forms.UserControl"
                    || o.ControlType == "DevExpress.XtraEditors.XtraUserControl"
                    || o.ControlType == "DevExpress.XtraEditors.PanelControl"
                    || o.ControlType == "DevExpress.XtraEditors.XtraScrollableControl"
                    || o.ControlType == "System.Windows.Forms.Panel"
                    || o.ControlType == "System.Windows.Forms.GroupBox")).ToList();

                if (mdParents != null && mdParents.Count > 0)
                {
                    foreach (var moduleControlADO in mdParents)
                    {
                        string controlName = moduleControlADO.ControlType + "_" + moduleControlADO.Title + "_" + moduleControlADO.CAPTION;
                        //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls[" + i + "].Name=" + itemDispose.Name + "____length1=" + length1);
                        DisposeAllChildOfControl(moduleControlADO.mControl);
                        try
                        {
                            moduleControlADO.mControl.Dispose();
                            GC.SuppressFinalize(moduleControlADO.mControl);
                            moduleControlADO.mControl = null;
                            Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                        }
                        catch (Exception exxx2)
                        {
                            Inventec.Common.Logging.LogSystem.Error("exxx2:itemDispose.Dispose", exxx2);
                        }
                    }
                }
            }
        }

        internal static void DisposeAllControl(Control parentControl)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllControl.parentControl.Name=" + parentControl.Name + ".1");
                string strDisposeAfterProcess = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.DisposeAfterProcessAndClose");
                Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.strDisposeAfterProcess =" + strDisposeAfterProcess);
                if (strDisposeAfterProcess == "1" || strDisposeAfterProcess == "2")
                {
                    Inventec.Common.Logging.LogSystem.Debug("ModuleControlDispose.SuppressFinalize.1");
                    GC.SuppressFinalize(parentControl);
                    Inventec.Common.Logging.LogSystem.Debug("ModuleControlDispose.SuppressFinalize.2");

                    Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllControl.2____Begin Disposing...");
                    if (parentControl != null && parentControl.Controls != null && parentControl.Controls.Count > 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllControl.3");
                        int i = 0;
                        int length = parentControl.Controls.Count;
                        for (i = length - 1; i >= 0; i--)
                        {
                            try
                            {
                                int length1 = parentControl.Controls.Count;
                                Control itemDispose = parentControl.Controls[i];
                                if (itemDispose != null)
                                {
                                    Inventec.Common.Logging.LogSystem.Info("parentControl." + itemDispose.GetType().ToString() + "-" + "itemDispose.Name=" + itemDispose.Name);
                                    DisposeControlAndReference(itemDispose);
                                }
                                else
                                {
                                    Inventec.Common.Logging.LogSystem.Info("itemDispose is null");
                                }
                            }
                            catch (Exception exxx)
                            {
                                Inventec.Common.Logging.LogSystem.Error("exxx:for", exxx);
                            }
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllControl.4____No config key => Not dispose");
                }

                Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllControl.5");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }

        private static void DisposeControlAndReference(Control itemCtrol)
        {
            string controlName = "";
            try
            {
                if (itemCtrol == null) return;
                controlName = itemCtrol.GetType().ToString() + "-" + itemCtrol.Name;
                //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeControlAndReference.itemCtrol.Name=" + itemCtrol.Name + ".1");
                DisposeAllChildOfControl(itemCtrol);
                try
                {
                    if (itemCtrol != null)
                    {
                        itemCtrol.Dispose();
                        GC.SuppressFinalize(itemCtrol);
                        Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                    }
                }
                catch (Exception exxx2)
                {
                    Inventec.Common.Logging.LogSystem.Error("ModuleControlDispose.DisposeControlAndReference:exxx2:controlName=" + controlName, exxx2);
                }
                //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeControlAndReference.2");
            }
            catch (Exception exxx)
            {
                Inventec.Common.Logging.LogSystem.Error("ModuleControlDispose.DisposeControlAndReference:exxx:controlName=" + controlName, exxx);
            }
        }

        private static void DisposeAllLayoutControlGroup(DevExpress.XtraLayout.LayoutControlGroup group)
        {
            if (group != null)
            {
                int length = (group.Items != null ? group.Items.Count : 0);
                if (length > 0)
                {
                    for (int i = length - 1; i >= 0; i--)
                    {
                        if (group.Items[i] is DevExpress.XtraLayout.LayoutControlGroup)
                            DisposeAllLayoutControlGroup((DevExpress.XtraLayout.LayoutControlGroup)group.Items[i]);
                        if (group.Items[i] is DevExpress.XtraLayout.TabbedControlGroup)
                            DisposeAllTabbedGroup((DevExpress.XtraLayout.TabbedControlGroup)group.Items[i]);
                        if (group.Items[i] is DevExpress.XtraLayout.LayoutControlItem)
                            DisposeAllLayoutControlItem(((DevExpress.XtraLayout.LayoutControlItem)group.Items[i]));
                    }
                }
            }
        }

        private static void DisposeAllTabbedGroup(DevExpress.XtraLayout.TabbedControlGroup group)
        {
            foreach (DevExpress.XtraLayout.LayoutControlGroup page in group.TabPages)
                DisposeAllLayoutControlGroup(page);
        }

        private static void DisposeAllLayoutControlItem(DevExpress.XtraLayout.LayoutControlItem item)
        {
            try
            {
                if (item == null) return;
                if (item != null && item.Control != null)
                {
                    Inventec.Common.Logging.LogSystem.Info("item.Name=" + item.Name + " - item.Control.Name=" + item.Control.Name);
                    if (item.Control is DevExpress.XtraLayout.LayoutControl)
                    {
                        DisposeAllLayoutControlGroup(((DevExpress.XtraLayout.LayoutControl)item.Control).Root);
                    }
                    else if (item.Control is DevExpress.XtraTab.XtraTabControl)
                    {
                        DisposeAllTabcontrol(((DevExpress.XtraTab.XtraTabControl)item.Control));
                    }
                    else if (item.Control is UserControl
                    || item.Control is DevExpress.XtraEditors.XtraUserControl
                    || item.Control is DevExpress.XtraEditors.PanelControl
                    || item.Control is DevExpress.XtraEditors.XtraScrollableControl
                    || item.Control is Panel
                    || item.Control is GroupBox)
                    {
                        DisposeControlAndReference(item.Control);
                    }
                    else
                    {
                        DisposeControlAndReference(item.Control);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void DisposeAllTabcontrol(DevExpress.XtraTab.XtraTabControl tabControl)
        {
            try
            {
                int length = (tabControl.TabPages != null ? tabControl.TabPages.Count : 0);
                if (length > 0)
                {
                    for (int i = length - 1; i >= 0; i--)
                    {
                        UserControl container = null;
                        if (tabControl.TabPages[i].Controls.Count > 0)
                        {
                            var control = tabControl.TabPages[i].Controls[0];

                            if (control != null && control is UserControl)
                            {
                                container = tabControl.TabPages[i].Controls[0] as UserControl;
                            }
                            else if (control != null && control is XtraUserControl)
                            {
                                container = tabControl.TabPages[i].Controls[0] as XtraUserControl;
                            }
                            DisposeControlAndReference(container);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private static void DisposeAllChildOfControl(Control parentControl)
        {
            int i = 0;
            string controlName = "";
            try
            {
                if (parentControl == null) return;

                //if (parentControl is DevExpress.XtraLayout.LayoutControl)
                //{
                //    DisposeAllLayoutControlGroup(((DevExpress.XtraLayout.LayoutControl)parentControl).Root);
                //}
                //else if (parentControl is DevExpress.XtraTab.XtraTabControl)
                //{
                //    DisposeAllTabcontrol(((DevExpress.XtraTab.XtraTabControl)parentControl));
                //}
                //else 
                //     if (parentControl is UserControl
                //|| parentControl is DevExpress.XtraEditors.XtraUserControl
                //|| parentControl is DevExpress.XtraEditors.PanelControl
                //|| parentControl is DevExpress.XtraEditors.XtraScrollableControl
                //|| parentControl is Panel
                //|| parentControl is GroupBox
                //)
                //{
                //int length = parentControl.Controls.Count;
                ////Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "]____length=" + length);
                //if (length > 0)
                //{
                //    for (i = length - 1; i >= 0; i--)
                //    {
                //        try
                //        {
                //            int length1 = parentControl.Controls.Count;
                //            Control itemDispose = parentControl.Controls[i];
                //            if (itemDispose != null)
                //            {
                //                controlName = itemDispose.GetType().ToString() + "-" + itemDispose.Name;
                //                //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls[" + i + "].Name=" + itemDispose.Name + "____length1=" + length1);
                //                DisposeAllChildOfControl(itemDispose);
                //                try
                //                {
                //                    itemDispose.Dispose();
                //                    GC.SuppressFinalize(itemDispose);
                //                    Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                //                }
                //                catch (Exception exxx2)
                //                {
                //                    Inventec.Common.Logging.LogSystem.Error("exxx2:itemDispose.Dispose", exxx2);
                //                }
                //            }
                //            else
                //            {
                //                Inventec.Common.Logging.LogSystem.Info("itemDispose is null");
                //            }
                //        }
                //        catch (Exception exxx)
                //        {
                //            Inventec.Common.Logging.LogSystem.Error("exxx:for", exxx);
                //        }
                //    }
                //}
                //}
                //else
                //{
                int length = parentControl.Controls.Count;
                if (length > 0)
                {
                    foreach (Control itemDispose in parentControl.Controls)
                    {
                        if (itemDispose != null)
                        {
                            controlName = itemDispose.GetType().ToString() + "-" + itemDispose.Name;
                            //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls[" + i + "].Name=" + itemDispose.Name + "____length1=" + length1);
                            DisposeAllChildOfControl(itemDispose);
                            try
                            {
                                itemDispose.Dispose();
                                GC.SuppressFinalize(itemDispose);
                                Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                            }
                            catch (Exception exxx2)
                            {
                                Inventec.Common.Logging.LogSystem.Error("exxx2:itemDispose.Dispose", exxx2);
                            }
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Info("itemDispose is null");
                        }
                    }

                    //for (i = length - 1; i >= 0; i--)
                    //{
                    //    try
                    //    {
                    //        int length1 = parentControl.Controls.Count;
                    //        Control itemDispose = parentControl.Controls[i];
                    //        if (itemDispose != null)
                    //        {
                    //            controlName = itemDispose.GetType().ToString() + "-" + itemDispose.Name;
                    //            //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls[" + i + "].Name=" + itemDispose.Name + "____length1=" + length1);
                    //            DisposeAllChildOfControl(itemDispose);
                    //            try
                    //            {
                    //                itemDispose.Dispose();
                    //                GC.SuppressFinalize(itemDispose);
                    //                Inventec.Common.Logging.LogSystem.Info("controlName =" + controlName + " is Dispose & GC.SuppressFinalize");
                    //            }
                    //            catch (Exception exxx2)
                    //            {
                    //                Inventec.Common.Logging.LogSystem.Error("exxx2:itemDispose.Dispose", exxx2);
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Inventec.Common.Logging.LogSystem.Info("itemDispose is null");
                    //        }
                    //    }
                    //    catch (Exception exxx)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Error("exxx:for", exxx);
                    //    }
                    //}
                }
                //    //Inventec.Common.Logging.LogSystem.Info("ModuleControlDispose.DisposeAllChildOfControl.parentControl[" + parentControl.Name + "].Controls.Count=" + length + "____Nothing...");
                //}
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error("exx:i=" + i + ", controlName=" + controlName, exx);
            }
        }
    }
}
