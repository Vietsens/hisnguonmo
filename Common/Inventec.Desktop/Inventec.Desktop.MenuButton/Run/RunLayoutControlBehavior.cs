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
using Inventec.Core;
using Inventec.Desktop.MenuButton.ADO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.MenuButton.Run
{
    public sealed class RunLayoutControlBehavior : IRun
    {
        MenuButtonInitADO menuPrintInitADO;
        DevExpress.XtraLayout.LayoutControl layoutControl;

        public RunLayoutControlBehavior()
            : base()
        {
        }

        public RunLayoutControlBehavior(CommonParam param, MenuButtonInitADO data, DevExpress.XtraLayout.LayoutControl panelcontrol)
            : base()
        {
            this.menuPrintInitADO = data;
            this.layoutControl = panelcontrol;
        }

        object IRun.Run()
        {
            try
            {
                MenuButtonResultADO result = new MenuButtonResultADO();
                if (Valid())
                {
                    this.layoutControl.BeginUpdate();
                    layoutControl.Root.Clear();
                    var groupItems = new List<BaseLayoutItem>();
                    Inventec.Common.Logging.LogSystem.Debug("1");
                    var buttonmenu__GroupsOrNoParent =
                        this.menuPrintInitADO.MenuPrintADOs.Where(o => o.IsGroup || (!o.IsGroup && String.IsNullOrEmpty(o.ParentCode))).OrderBy(o => o.NumOrder).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => buttonmenu__GroupsOrNoParent), buttonmenu__GroupsOrNoParent.Select(o => o.Code)));

                    int clientWidth = 0;
                    int clientHeight = 0;
                    int dem = 0;
                    if (buttonmenu__GroupsOrNoParent != null && buttonmenu__GroupsOrNoParent.Count > 0)
                    {
                        foreach (var mbG in buttonmenu__GroupsOrNoParent)
                        {
                            dem += 1;
                            if (mbG.IsGroup)
                            {
                                DevExpress.XtraEditors.DropDownButton menuButton = new DevExpress.XtraEditors.DropDownButton();
                                menuButton.Text = mbG.Caption;
                                menuButton.AutoWidthInLayoutControl = true;

                                var bmenus = this.menuPrintInitADO.MenuPrintADOs.Where(o => o.ParentCode == mbG.Code).ToList();
                                DXPopupMenu menu = new DXPopupMenu();
                                foreach (var item in bmenus)
                                {
                                    DXMenuItem menuItem1 = new DXMenuItem(item.Caption, item.EventHandler);
                                    menuItem1.Tag = item.Tag;
                                    menu.Items.Add(menuItem1);
                                }
                                menuButton.DropDownControl = menu;
                                menuButton.Click += (mbG.EventHandler != null ? mbG.EventHandler : menuButton_Click);

                                var itembtnSinglePrintTemp = new LayoutControlItem
                                {
                                    Control = menuButton,
                                    Name = String.Format("lci{0}", mbG.Code),
                                    TextVisible = false,
                                    SizeConstraintsType = ((mbG.MaxSizeWidth == 0 && mbG.MaxSizeHeight == 0 && mbG.MinSizeWidth == 0 && mbG.MinSizeHeight == 0) ? SizeConstraintsType.Default : SizeConstraintsType.Custom),
                                    MaxSize = new System.Drawing.Size(mbG.MaxSizeWidth, mbG.MaxSizeHeight),
                                    MinSize = new System.Drawing.Size(mbG.MinSizeWidth, mbG.MinSizeHeight)
                                };

                                groupItems.Add(itembtnSinglePrintTemp);
                                layoutControl.Root.Add(itembtnSinglePrintTemp);
                                if (groupItems != null && groupItems.Count > 1)
                                {
                                    itembtnSinglePrintTemp.Move(groupItems[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                }

                                clientWidth += menuButton.ClientSize.Width;
                                clientHeight = menuButton.ClientSize.Height;
                            }
                            else
                            {
                                DevExpress.XtraEditors.SimpleButton btnSinglePrint = new DevExpress.XtraEditors.SimpleButton();
                                btnSinglePrint.Text = mbG.Caption;
                                btnSinglePrint.Click += mbG.EventHandler;
                                btnSinglePrint.Tag = mbG.Tag;
                                btnSinglePrint.AutoWidthInLayoutControl = true;

                                var itembtnSinglePrintTemp = new LayoutControlItem
                                {
                                    Control = btnSinglePrint,
                                    Name = String.Format("lci{0}", mbG.Code),
                                    TextVisible = false,
                                    SizeConstraintsType = ((mbG.MaxSizeWidth == 0 && mbG.MaxSizeHeight == 0 && mbG.MinSizeWidth == 0 && mbG.MinSizeHeight == 0) ? SizeConstraintsType.Default : SizeConstraintsType.Custom),
                                    MaxSize = new System.Drawing.Size(mbG.MaxSizeWidth, mbG.MaxSizeHeight),
                                    MinSize = new System.Drawing.Size(mbG.MinSizeWidth, mbG.MinSizeHeight)
                                };
                                groupItems.Add(itembtnSinglePrintTemp);
                                layoutControl.Root.Add(itembtnSinglePrintTemp);
                                if (groupItems != null && groupItems.Count > 1)
                                {
                                    itembtnSinglePrintTemp.Move(groupItems[dem - 2], DevExpress.XtraLayout.Utils.InsertType.Right);
                                }

                                clientWidth += btnSinglePrint.ClientSize.Width;
                                clientHeight = btnSinglePrint.ClientSize.Height;
                            }
                        }
                    }
                    result.Width = clientWidth;
                    result.Height = clientHeight;
                    result.Success = true;
                    //foreach (LayoutControlItem item in groupItems)
                    //    item.SizeConstraintsType = SizeConstraintsType.Default;
                    this.layoutControl.EndUpdate();
                    Inventec.Common.Logging.LogSystem.Debug("2");
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
