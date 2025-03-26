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
using Inventec.Core;
using Inventec.Desktop.MenuButton.ADO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.MenuButton.Run
{
    public sealed class RunPopupMenuBehavior : IRun
    {
        MenuButtonInitADO menuPrintInitADO;
        DevExpress.XtraBars.PopupMenu popupMenu;
        public RunPopupMenuBehavior()
            : base()
        {
        }

        public RunPopupMenuBehavior(CommonParam param, MenuButtonInitADO data, DevExpress.XtraBars.PopupMenu popupMenu)
            : base()
        {
            this.menuPrintInitADO = data;
            this.popupMenu = popupMenu;
        }

        object IRun.Run()
        {
            try
            {
                MenuButtonResultADO result = new MenuButtonResultADO();
                if (Valid())
                {
                    this.popupMenu.ItemLinks.Clear();
                    var buttonmenu__GroupsOrNoParent =
                        this.menuPrintInitADO.MenuPrintADOs.Where(o => o.IsGroup || (!o.IsGroup && String.IsNullOrEmpty(o.ParentCode))).OrderBy(o => o.NumOrder).ToList();

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => buttonmenu__GroupsOrNoParent), buttonmenu__GroupsOrNoParent.Select(o => o.Code)));

                    if (buttonmenu__GroupsOrNoParent != null && buttonmenu__GroupsOrNoParent.Count > 0)
                    {
                        foreach (var mbG in buttonmenu__GroupsOrNoParent)
                        {
                            if (mbG.IsGroup)
                            {
                                BarSubItem menu = new BarSubItem();
                                menu.Caption = mbG.Caption;
                                menu.PaintStyle = BarItemPaintStyle.Caption;
                                menu.Manager = this.popupMenu.Manager;
                                if (mbG.ImageIndex.HasValue)
                                {
                                    menu.ImageIndex = mbG.ImageIndex.Value;
                                }
                                var bmenus = this.menuPrintInitADO.MenuPrintADOs.Where(o => o.ParentCode == mbG.Code).ToList();
                                foreach (var item in bmenus)
                                {
                                    BarItem bottonItem1 = new BarButtonItem();
                                    bottonItem1.Tag = item.Tag;
                                    bottonItem1.ItemClick += item.ItemClickEventHandler;
                                    bottonItem1.Caption = item.Caption;
                                    bottonItem1.PaintStyle = BarItemPaintStyle.Caption;
                                    bottonItem1.Manager = this.popupMenu.Manager;
                                    if (item.ImageIndex.HasValue)
                                    {
                                        bottonItem1.ImageIndex = item.ImageIndex.Value;
                                    }
                                    menu.AddItem(bottonItem1);
                                }

                                this.popupMenu.AddItem(menu);
                            }
                            else
                            {
                                BarItem bottonItem1 = new BarButtonItem();
                                bottonItem1.Tag = mbG.Tag;
                                bottonItem1.ItemClick += mbG.ItemClickEventHandler;
                                bottonItem1.Caption = mbG.Caption;
                                bottonItem1.PaintStyle = BarItemPaintStyle.Caption;
                                bottonItem1.Manager = this.popupMenu.Manager;
                                if (mbG.ImageIndex.HasValue)
                                {
                                    bottonItem1.ImageIndex = mbG.ImageIndex.Value;
                                }
                                this.popupMenu.AddItem(bottonItem1);
                            }
                        }
                        result.Success = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return null;
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
