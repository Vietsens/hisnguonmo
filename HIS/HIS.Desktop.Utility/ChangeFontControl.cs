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
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using HIS.Desktop.LocalStorage.ConfigSystem;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class ChangeFontControl
    {
        const string CONFIG_KEY__HIS_KEY = "HIS.Desktop.ApplySpecialFont.ModuleLinks";
        const string CONFIG_KEY__HIS_KEY_FONT_NAME = "HIS.Desktop.ApplySpecialFont.FontName";

        static string nameFontUsed;
        public static string NameFontUsed
        {
            get
            {
                try
                {

                    nameFontUsed = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HIS_KEY_FONT_NAME));
                    if (String.IsNullOrEmpty(nameFontUsed))
                    {
                        return "TNKeyUni-Arial";
                    }
                    return nameFontUsed;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return String.Empty;
            }
        }

        static string moduleLinksApplySpecialFont;
        public static string ModuleLinksApplySpecialFont
        {
            get
            {
                try
                {
                    //if (String.IsNullOrEmpty(moduleLinksApplySpecialFont))
                    //{
                    moduleLinksApplySpecialFont = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__HIS_KEY));
                    //}
                    return moduleLinksApplySpecialFont;
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                return String.Empty;
            }
        }

        public static void SetFontTextControl(Control control, string fontName)
        {
            try
            {
                if (control == null || control.IsDisposed)
                {
                    Inventec.Common.Logging.LogSystem.Debug("SetFontTextControl: control == null || control.IsDisposed" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fontName), fontName));
                }

                if (control != null && !String.IsNullOrWhiteSpace(fontName) && FontCollection != null && FontCollection.Families != null && FontCollection.Families.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Tim thay font chu mo rong(ede, tay nguyen,...) de xu ly ghi de font cho viec hien thi duoc font chu nay__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fontName), fontName)
                        + Inventec.Common.Logging.LogUtil.TraceData("control.Name", control.Name)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ConfigSystems.FOLDER_FONT_BASE), ConfigSystems.FOLDER_FONT_BASE));

                    List<Control> ctr = new List<Control>();
                    List<Type> types = new List<Type>();
                    types.Add(typeof(DevExpress.XtraRichEdit.RichEditControl));
                    types.Add(typeof(DevExpress.XtraEditors.TextEdit));
                    types.Add(typeof(DevExpress.XtraEditors.MemoEdit));
                    types.Add(typeof(System.Windows.Forms.TextBox));
                    types.Add(typeof(DevExpress.XtraEditors.LabelControl));
                    types.Add(typeof(DevExpress.XtraLayout.LayoutControl));
                    types.Add(typeof(DevExpress.XtraEditors.PanelControl));
                    types.Add(typeof(DevExpress.XtraLayout.LayoutControlGroup));
                    types.Add(typeof(DevExpress.XtraLayout.LayoutControlItem));
                    types.Add(typeof(Label));
                    types.Add(typeof(DevExpress.XtraGrid.GridControl));
                    types.Add(typeof(DevExpress.XtraTreeList.TreeList));

                    var controlAlls = GetAll(control, types);
                    if (controlAlls == null || controlAlls.Count() == 0)
                        return;

                    ctr.AddRange(controlAlls);

                    if (ctr.Count > 0)
                    {
                        FontFamily ff = FontCollection.Families.FirstOrDefault(x => x.Name == fontName);
                        if (ff != null)
                        {
                            foreach (var item in ctr)
                            {
                                if (item is DevExpress.XtraGrid.GridControl)
                                {
                                    var gc = item as DevExpress.XtraGrid.GridControl;
                                    GridViewChangeFontSize(gc.MainView as DevExpress.XtraGrid.Views.Grid.GridView, new Font(ff, item.Font.Size));
                                }
                                else if (item is DevExpress.XtraTreeList.TreeList)
                                {
                                    var gc = item as DevExpress.XtraTreeList.TreeList;
                                    TreeViewChangeFontSize(gc, new Font(ff, item.Font.Size));
                                }
                                else
                                {
                                    Font robotoBold = new Font(ff, item.Font.Size);
                                    item.Font = robotoBold;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Debug("Khong tim thay font chu mo rong(ede, tay nguyen,...) de xu ly ghi de font cho viec hien thi duoc font chu nay" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fontName), fontName)
                        + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ChangeFontControl.ModuleLinksApplySpecialFont), ChangeFontControl.ModuleLinksApplySpecialFont));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static void GridViewChangeFontSize(GridView view, Font font)
        {
            foreach (DevExpress.XtraGrid.Columns.GridColumn col in view.Columns)
            {
                col.AppearanceCell.Font = font;
            }
        }

        private static void TreeViewChangeFontSize(TreeList view, Font font)
        {
            foreach (DevExpress.XtraTreeList.Columns.TreeListColumn col in view.Columns)
            {
                col.AppearanceCell.Font = font;
            }
        }

        private static IEnumerable<Control> GetAll(Control control, List<Type> types)
        {
            var controls = control.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAll(ctrl, types))
                                      .Concat(controls)
                                      .Where(c => types.Contains(c.GetType()));
        }

        private static PrivateFontCollection fontCollection;
        public static PrivateFontCollection FontCollection
        {
            get
            {
                if (fontCollection == null)
                {
                    fontCollection = new PrivateFontCollection();
                    string dllFolderPath = "Font";
                    string rootPath = AppDomain.CurrentDomain.BaseDirectory;

                    string[] dllFiles = Directory.GetFiles(rootPath + dllFolderPath, "*.ttf", SearchOption.TopDirectoryOnly);
                    if (dllFiles != null && dllFiles.Count() > 0)
                    {
                        foreach (var item in dllFiles)
                        {
                            fontCollection.AddFontFile(item);
                        }
                    }
                }
                return fontCollection;
            }
            set
            {
                fontCollection = value;
            }
        }
    }
}
