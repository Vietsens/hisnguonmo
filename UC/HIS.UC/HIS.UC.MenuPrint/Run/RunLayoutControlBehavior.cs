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
using DevExpress.Utils.Menu;
using DevExpress.XtraLayout;
using HIS.UC.MenuPrint.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MenuPrint.Run
{
    public sealed class RunLayoutControlBehavior : IRun
    {
        MenuPrintInitADO menuPrintInitADO;
        const int IS_NO_GROUP = 1;
        DevExpress.XtraLayout.LayoutControl layoutControl;

        public RunLayoutControlBehavior()
            : base()
        {
        }

        public RunLayoutControlBehavior(CommonParam param, MenuPrintInitADO data, DevExpress.XtraLayout.LayoutControl panelcontrol)
            : base()
        {
            this.menuPrintInitADO = data;
            this.layoutControl = panelcontrol;
        }

        object IRun.Run()
        {
            try
            {
                MenuPrintResultADO result = new MenuPrintResultADO();
                if (Valid())
                {
                    this.layoutControl.BeginUpdate();
                    layoutControl.Root.Clear();
                    var printTypeCodes = menuPrintInitADO.SarPrintTypes.Select(o => o.PRINT_TYPE_CODE).Distinct().ToList();
                    var groupItems = new List<BaseLayoutItem>();
                    //var spliterItems = new List<BaseLayoutItem>();

                    var printType__Groups =
                        (
                         from m in this.menuPrintInitADO.MenuPrintADOs
                         from n in menuPrintInitADO.SarPrintTypes
                         where
                         !String.IsNullOrEmpty(m.PrintTypeCode)
                         && m.PrintTypeCode == n.PRINT_TYPE_CODE
                         && n.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                         && (n.IS_NO_GROUP == null || n.IS_NO_GROUP != IS_NO_GROUP)//TODO
                         select new MenuPrintADO(m, n)
                        ).OrderBy(o => o.Shortcut).ThenBy(o => o.Caption).ToList();

                    printType__Groups = printType__Groups ?? new List<MenuPrintADO>();

                    var printType__NoGroups =
                        (
                         from m in this.menuPrintInitADO.MenuPrintADOs
                         from n in menuPrintInitADO.SarPrintTypes
                         where
                         !String.IsNullOrEmpty(m.PrintTypeCode)
                         && m.PrintTypeCode == n.PRINT_TYPE_CODE
                         && n.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                         && (n.IS_NO_GROUP == IS_NO_GROUP)//TODO
                         select new MenuPrintADO(m, n)
                        ).OrderBy(o => o.Shortcut).ThenBy(o => o.Caption).ToList();

                    string shortcut = "";
                    if ((printType__Groups != null && printType__Groups.Count >= 1
                        && (printType__NoGroups == null || printType__NoGroups.Count == 0))
                        || (printType__NoGroups != null && printType__NoGroups.Count == 1
                        && (printType__Groups == null || printType__Groups.Count == 0))

                        )
                    {
                        shortcut = "Ctrl P";
                    }

                    if (!this.menuPrintInitADO.IsUsingShortCut)
                    {
                        shortcut = "";
                    }

                    int clientWidth = 0;
                    int clientHeight = 0;

                    int dem = 0;
                    if (printType__Groups != null && printType__Groups.Count > 0)
                    {
                        DevExpress.XtraEditors.DropDownButton menuButton = new DevExpress.XtraEditors.DropDownButton();
                        menuButton.Text = String.Format("  {0}{1} ", PrintMenuConfig.CAPTION_OTHER_MENU, (!String.IsNullOrEmpty(shortcut) ? " (" + shortcut + ")" : ""));
                        //menuButton.AutoSize = true;
                        menuButton.AutoWidthInLayoutControl = true;
                        DXPopupMenu menu = new DXPopupMenu();
                        dem += 1;
                        foreach (var printG in printType__Groups)
                        {
                            var printType = menuPrintInitADO.SarPrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == printG.PrintTypeCode);
                            if (printType != null)
                            {
                                if (!String.IsNullOrEmpty(printG.PrintTypeCode) && printTypeCodes.Contains(printG.PrintTypeCode))
                                {
                                    DXMenuItem menuItem1 = new DXMenuItem(printG.Caption, printG.EventHandler);
                                    menuItem1.Tag = printG.Tag;
                                    menu.Items.Add(menuItem1);
                                }
                            }
                        }
                        menuButton.Click += menuButton_Click;
                        menuButton.DropDownControl = menu;
                        //menuButton.Dock = System.Windows.Forms.DockStyle.Right;
                        //itemMenuButton.Parent = layoutControl.Root;
                        //itemMenuButton.Name = "Layout Item 1";

                        var itemMenuButtonTemp = new LayoutControlItem
                        {
                            Control = menuButton,
                            Name = String.Format("itemMenuButtonTemp{0}", dem),
                            TextVisible = false,
                            SizeConstraintsType = SizeConstraintsType.Custom,
                            MaxSize = new System.Drawing.Size(this.menuPrintInitADO.MaxSizeWidth, this.menuPrintInitADO.MaxSizeHeight),
                            MinSize = new System.Drawing.Size(this.menuPrintInitADO.MinSizeWidth, this.menuPrintInitADO.MinSizeHeight)
                        };

                        //LayoutControlItem itemMenuButtonTemp = new LayoutControlItem();
                        //itemMenuButtonTemp.TextVisible = false;
                        //itemMenuButtonTemp.Control = menuButton;
                        //itemMenuButtonTemp.Text = "Name";
                        layoutControl.Root.Add(itemMenuButtonTemp);
                        groupItems.Add(itemMenuButtonTemp);
                        //this.layoutControl.Controls.Add(menuButton);
                        clientWidth += menuButton.ClientSize.Width;
                        clientHeight = menuButton.ClientSize.Height;
                    }

                    if (printType__NoGroups != null && printType__NoGroups.Count > 0)
                    {
                        foreach (var printNoG in printType__NoGroups)
                        {
                            dem += 1;
                            DevExpress.XtraEditors.SimpleButton btnSinglePrint = new DevExpress.XtraEditors.SimpleButton();
                            btnSinglePrint.Text = String.Format("{0}{1}", printNoG.Caption, (!String.IsNullOrEmpty(shortcut) ? " (" + shortcut + ")" : ""));
                            btnSinglePrint.Click += printNoG.EventHandler;
                            btnSinglePrint.Tag = printNoG.Tag;
                            //btnSinglePrint.AutoSize = true;
                            //btnSinglePrint.Size = btnSinglePrint.CalcBestFit(btnSinglePrint.CreateGraphics());
                            //btnSinglePrint.Dock = System.Windows.Forms.DockStyle.Right;
                            btnSinglePrint.AutoWidthInLayoutControl = true;

                            //LayoutControlItem itembtnSinglePrint = layoutControl.Root.AddItem();
                            // Add the item to the root group by setting its parent.
                            //itembtnSinglePrintTemp.Parent = layoutControl.Root;
                            //itembtnSinglePrintTemp.Name = "Layout Item 1";
                            //LayoutControlItem itembtnSinglePrintTemp = new LayoutControlItem();
                            //itembtnSinglePrintTemp.TextVisible = false;
                            //itembtnSinglePrintTemp.Control = btnSinglePrint;
                            //itembtnSinglePrintTemp.Text = "Name";

                            var itembtnSinglePrintTemp = new LayoutControlItem
                            {
                                Control = btnSinglePrint,
                                Name = String.Format("lcibtnSinglePrintTemp{0}", dem),
                                TextVisible = false,
                                SizeConstraintsType = SizeConstraintsType.Custom,
                                MaxSize = new System.Drawing.Size(this.menuPrintInitADO.MaxSizeWidth, this.menuPrintInitADO.MaxSizeHeight),
                                MinSize = new System.Drawing.Size(this.menuPrintInitADO.MinSizeWidth, this.menuPrintInitADO.MinSizeHeight)
                            };

                            groupItems.Add(itembtnSinglePrintTemp);

                            //if (dem > 1 && dem < printType__Groups.Count - 2)
                            //{
                            //    var splitter = new SplitterItem()
                            //    {
                            //        Name = String.Format("SplitterItem{0}", dem)
                            //    };
                            //    spliterItems.Add(splitter);
                            //}

                            layoutControl.Root.Add(itembtnSinglePrintTemp);
                            //mainLayoutControlGroup.Add(splitter);
                            if (groupItems != null && groupItems.Count > 1)
                            {
                                itembtnSinglePrintTemp.Move(groupItems[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                            }

                            //splitter.Move(itembtnSinglePrintTemp, DevExpress.XtraLayout.Utils.InsertType.Top);
                            //layoutControl.Root.Add(layoutControlItem);
                            //LayoutControlItem itembtnSinglePrint = layoutControl.Root.AddItem(itembtnSinglePrintTemp, DevExpress.XtraLayout.Utils.InsertType.Right);
                            //this.layoutControl.Controls.Add(btnSinglePrint);

                            clientWidth += btnSinglePrint.ClientSize.Width;
                            clientHeight = btnSinglePrint.ClientSize.Height;
                        }
                    }
                    result.Width = clientWidth;
                    result.Height = clientHeight;
                    result.Success = true;
                    foreach (LayoutControlItem item in groupItems)
                    {
                        if (item.Control != null && !String.IsNullOrWhiteSpace(item.Control.Text))
                        {
                            item.SizeConstraintsType = SizeConstraintsType.Custom;

                            item.MinSize = new System.Drawing.Size(item.Control.Text.Length * 8, this.menuPrintInitADO.MinSizeHeight);
                            item.MaxSize = new System.Drawing.Size(item.Control.Text.Length * 8, this.menuPrintInitADO.MaxSizeHeight);
                        }
                        else
                        {
                            item.SizeConstraintsType = SizeConstraintsType.Default;
                        }
                    }

                    this.layoutControl.EndUpdate();
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
            }
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            try
            {
                var ddbt = sender as DevExpress.XtraEditors.DropDownButton;
                if (ddbt != null)
                {
                    ddbt.ShowDropDown();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool Valid()
        {
            bool valid = true;
            try
            {
                if (menuPrintInitADO == null)
                    throw new ArgumentNullException("menuPrintInitADO is null");
                if (menuPrintInitADO.MenuPrintADOs == null || menuPrintInitADO.MenuPrintADOs.Count == 0)
                    throw new ArgumentNullException("MenuPrintADOs is null");
                if (menuPrintInitADO.ControlContainer == null)
                    throw new ArgumentNullException("ControlContainer is null");
                if (menuPrintInitADO.SarPrintTypes == null || menuPrintInitADO.SarPrintTypes.Count == 0)
                    throw new ArgumentNullException("SarPrintTypes is null");

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }


            return valid;
        }
    }
}
