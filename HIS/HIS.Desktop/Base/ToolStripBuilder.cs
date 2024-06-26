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
using Inventec.Common.Logging;
using Inventec.Desktop.Core.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Base
{
    public class ToolStripBuilder
    {
        public ToolStripBuilder()
        {
        }

        public class ItemTag
        {
            private readonly List<Inventec.Desktop.Common.Modules.Module> _node;
            private readonly IAction _view;

            public ItemTag(List<Inventec.Desktop.Common.Modules.Module> node, IAction view)
            {
                _node = node;
                _view = view;
            }

            public List<Inventec.Desktop.Common.Modules.Module> Node
            {
                get { return _node; }
            }

            public IAction View
            {
                get { return _view; }
            }
        }

        public delegate Control DelegateGetCurrentPage();
        public delegate Inventec.Desktop.Common.Modules.Module DelegateGetCurrentModule();

        static DelegateGetCurrentModule CurrentModule { get; set; }
        static DelegateGetCurrentPage currentGetCurrentPage { get; set; }
        #region Public API

        /// <summary>
        /// Called to build menus and toolbars.  Override this method to customize menu and toolbar building.
        /// </summary>
        /// <remarks>
        /// The default implementation simply clears and re-creates the toolstrip using methods on the
        /// utility class <see cref="ToolStripBuilder"/>.
        /// </remarks>
        /// <param name="kind"></param>
        /// <param name="toolStrip"></param>
        /// <param name="actionModel"></param>
        public List<DevExpress.XtraBars.BarItem> BuildToolStrip(List<Inventec.Desktop.Common.Modules.Module> actionModel,
            DelegateGetCurrentPage getCurrentPage,
            DelegateGetCurrentModule currentModule)
        {
            List<DevExpress.XtraBars.BarItem> result = null;
            try
            {
                CurrentModule = currentModule;
                currentGetCurrentPage = getCurrentPage;
                // avoid flicker
                //toolStrip.SuspendLayout();
                // very important to clean up the existing ones first
                //ToolStripBuilder.Clear(toolStrip.Items);

                if (actionModel != null && actionModel.Count > 0)
                {
                    result = new List<DevExpress.XtraBars.BarItem>();
                    if (actionModel.Count > 0)
                    {
                        // Toolstrip should only be visible if there are items on it                   
                        BuildMenu(result, actionModel);
                    }
                }

                //toolStrip.ResumeLayout();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        /// <summary>
        /// Builds a menu from the specified action model nodes.
        /// </summary>
        /// <param name="parentItemCollection"></param>
        /// <param name="nodes"></param>
        private void BuildMenu(List<DevExpress.XtraBars.BarItem> parentItemCollection, List<Inventec.Desktop.Common.Modules.Module> nodes)
        {
            try
            {
                foreach (Inventec.Desktop.Common.Modules.Module node in nodes)
                {
                    if (node.ExtensionInfo != null
                        && node.ExtensionInfo.ToolSet != null
                        && node.ExtensionInfo.ToolSet.Actions != null)
                    {
                        foreach (var item in node.ExtensionInfo.ToolSet.Actions)
                        {
                            var ShortcutKeys = (System.Windows.Forms.Keys)(((IClickAction)(item)).KeyStroke);
                            var scDisplay = Inventec.Desktop.Core.XKeysConverter.Format(((KeyboardAction)item).KeyStroke);
                            if (scDisplay == "Ctrl+C" || scDisplay == "Ctrl+V")
                            {
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => node.ModuleLink), node.ModuleLink) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => scDisplay), scDisplay));
                                continue;
                            }
                           
                            var itemBtn = (ShortcutKeys != null && ShortcutKeys != Keys.None) ? parentItemCollection.FirstOrDefault(o => o.ItemShortcut.Key == ShortcutKeys) : null;
                            if (itemBtn == null)
                            {
                                var bbtnItem = new DevExpress.XtraBars.BarButtonItem();
                                bbtnItem.ItemShortcut = new DevExpress.XtraBars.BarShortcut(ShortcutKeys);
                                bbtnItem.Name = node.ModuleCode;
                                bbtnItem.Tag = (IClickAction)(item);
                                bbtnItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(BarButtonItemClick);

                                parentItemCollection.Add(bbtnItem);
                            }
                        }
                    }

                    if (node.Children != null && node.Children.Count > 0)
                    {
                        BuildMenu(parentItemCollection, node.Children);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void BarButtonItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {               
                DevExpress.XtraBars.BarButtonItem clickedItem = (DevExpress.XtraBars.BarButtonItem)(e.Item);
                var action = clickedItem.Tag as IClickAction;
                var shortcutKeys = (System.Windows.Forms.Keys)(action.KeyStroke);
                var scDisplay = Inventec.Desktop.Core.XKeysConverter.Format(action.KeyStroke);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("shortcutKeys", shortcutKeys)
                        + Inventec.Common.Logging.LogUtil.TraceData("scDisplay", scDisplay)
                        + Inventec.Common.Logging.LogUtil.TraceData("CurrentModule", CurrentModule != null ? CurrentModule().ModuleLink : ""));

                if (CurrentModule != null
                    && CurrentModule() != null
                    && CurrentModule().ExtensionInfo != null
                    && CurrentModule().ExtensionInfo.ToolSet != null)
                {
                    var acts = CurrentModule().ExtensionInfo.ToolSet.Actions;

                    var act = (KeyboardAction)(acts.FirstOrDefault(o => ((System.Windows.Forms.Keys)(((IClickAction)(o)).KeyStroke)).GetHashCode() == shortcutKeys.GetHashCode()));
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("act.ActionID", act != null ? act.ActionID : "")
                        + "____" + Inventec.Common.Logging.LogUtil.TraceData("acts", acts)
                       );
                    if (act != null)
                    {
                        var asm = CurrentModule().PluginInfo.AssemblyResolve;
                        if (asm != null)
                        {
                            if (currentGetCurrentPage != null)
                            {
                                Control c = currentGetCurrentPage();
                                if (c == null)
                                    throw new Exception("GetCurrentPage failed");
                                Type type = c.GetType();
                                if (!String.IsNullOrEmpty(act.ActionID))
                                {
                                    string[] actionIds = act.ActionID.Split(':');
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("actionIds", actionIds));

                                    if (actionIds != null && actionIds.Count() > 0)
                                    {
                                        MethodInfo methodInfo = type.GetMethod(actionIds[1]);
                                        methodInfo.Invoke(currentGetCurrentPage(), null);
                                    }
                                }
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

        private Shortcut GetShortcutFromValue(int scValue)
        {
            return (Shortcut)scValue;
        }
        #endregion
    }
}
