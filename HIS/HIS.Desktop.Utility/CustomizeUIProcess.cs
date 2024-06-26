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
    internal class CustomizeUIProcess
    {
        internal CustomizeUIProcess() { }
        string ModuleLink;
        List<SDA.EFMODEL.DataModels.SDA_CUSTOMIZE_UI> CustomizeUIs { get; set; }

        internal void Run(Control control, string moduleLink)
        {
            try
            {
                if (control == null || control.IsDisposed)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CustomizeUIProcess: control == null || control.IsDisposed" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink));
                }

                this.ModuleLink = moduleLink;
                this.CustomizeUIs = ConfigCustomizeUIWorker.GetByModule(this.ModuleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.ModuleLink), this.ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.CustomizeUIs), this.CustomizeUIs));
                if ((this.CustomizeUIs != null && this.CustomizeUIs.Count > 0))
                {
                    //Xử lý customize UI                    
                    foreach (Control c in control.Controls)
                    {
                        if (c is LayoutControl)
                        {
                            ProcessLayoutControlGroup(((LayoutControl)c).Root, c.Name);
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
                        else// if (c is DevExpress.XtraEditors.ButtonEdit || c is DevExpress.XtraEditors.TextEdit || c is DevExpress.XtraEditors.MemoEdit)
                        {
                            var hbtn = this.CustomizeUIs.Where(o => o.CONTROL_PATH == c.Name).FirstOrDefault();
                            if (hbtn != null && hbtn.IS_DEFAULT_FOCUS.HasValue && hbtn.IS_DEFAULT_FOCUS == 1)
                            {
                                ExecuteProcessFousControl(c);
                                Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + c.Name + " is Text = " + hbtn.CONTROL_CODE + ",IS_DEFAULT_FOCUS=" + hbtn.IS_DEFAULT_FOCUS);
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
                    if (item is DevExpress.XtraLayout.LayoutControl)
                    {
                        ProcessLayoutControlGroup(((LayoutControl)item).Root, parentPath);
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
                    else// if (item is DevExpress.XtraEditors.SimpleButton || item is Button)
                    {
                        var hbtn = this.CustomizeUIs.Where(o => o.CONTROL_PATH == parentPath).FirstOrDefault();
                        if (hbtn != null && hbtn.IS_DEFAULT_FOCUS.HasValue && hbtn.IS_DEFAULT_FOCUS == 1)
                        {
                            ExecuteProcessFousControl(item);
                            Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + item.Name + " is Text = " + hbtn.CONTROL_CODE + ",IS_DEFAULT_FOCUS=" + hbtn.IS_DEFAULT_FOCUS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ExecuteProcessFousControl(Control item)
        {
            if (item is DevExpress.XtraEditors.ButtonEdit)
            {
                var cbtn = item as DevExpress.XtraEditors.ButtonEdit;
                cbtn.SelectAll();
                cbtn.Focus();
            }
            else if (item is DevExpress.XtraEditors.TextEdit)
            {
                var cbtn = item as DevExpress.XtraEditors.TextEdit;
                cbtn.SelectAll();
                cbtn.Focus();
            }
            else if (item is DevExpress.XtraEditors.MemoEdit)
            {
                var cbtn = item as DevExpress.XtraEditors.MemoEdit;
                cbtn.SelectAll();
                cbtn.Focus();
            }
            else
            {
                //item.Select();
                item.Focus();
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
                    if (item.Control is DevExpress.XtraLayout.LayoutControl)
                    {
                        ProcessLayoutControlGroup(((LayoutControl)item.Control).Root, parentPathChild);
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
                    else// if (item.Control is DevExpress.XtraEditors.SimpleButton || item.Control is Button)
                    {
                        var hbtn = this.CustomizeUIs.Where(o => o.CONTROL_PATH == parentPathChild).FirstOrDefault();
                        if (hbtn != null && hbtn.IS_DEFAULT_FOCUS.HasValue && hbtn.IS_DEFAULT_FOCUS == 1)
                        {                            
                            ExecuteProcessFousControl(item.Control);
                            Inventec.Common.Logging.LogSystem.Info("this.ModuleLink " + this.ModuleLink + " - Control " + item.Control.Name + " is Text = " + hbtn.CONTROL_CODE + ",IS_DEFAULT_FOCUS=" + hbtn.IS_DEFAULT_FOCUS);
                        }
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
