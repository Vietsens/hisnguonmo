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
using HIS.UC.MenuPrint.ADO;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MenuPrint.Run
{
    public sealed class RunPopupMenuBehavior : IRun
    {
        MenuPrintInitADO menuPrintInitADO;
        DevExpress.XtraBars.PopupMenu popupMenu;
        public RunPopupMenuBehavior()
            : base()
        {
        }

        public RunPopupMenuBehavior(CommonParam param, MenuPrintInitADO data, DevExpress.XtraBars.PopupMenu popupMenu)
            : base()
        {
            this.menuPrintInitADO = data;
            this.popupMenu = popupMenu;
        }

        object IRun.Run()
        {
            try
            {
                if (Valid())
                {
                    this.popupMenu.ItemLinks.Clear();
                    var printTypeCodes = menuPrintInitADO.SarPrintTypes.Select(o => o.PRINT_TYPE_CODE).Distinct().ToList();

                    var printType__Groups =
                        (
                         from m in this.menuPrintInitADO.MenuPrintADOs
                         from n in menuPrintInitADO.SarPrintTypes
                         where !String.IsNullOrEmpty(m.PrintTypeCode) && m.PrintTypeCode == n.PRINT_TYPE_CODE
                         && n.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                         && (n.IS_NO_GROUP == null || n.IS_NO_GROUP != 1)//TODO
                         select m
                        );

                    printType__Groups = printType__Groups ?? new List<MenuPrintADO>();

                    var PrintTypeCode__Groups = printType__Groups.Select(o => o.PrintTypeCode).ToArray();
                    var printType__NoGroups = this.menuPrintInitADO.MenuPrintADOs
                        .Where(o =>
                            PrintTypeCode__Groups == null
                            || PrintTypeCode__Groups.Count() == 0
                            || PrintTypeCode__Groups.Contains(o.PrintTypeCode));

                    if (printType__Groups != null && printType__Groups.Count() > 0)
                    {
                        BarSubItem menu = new BarSubItem();
                        menu.Caption = PrintMenuConfig.CAPTION_OTHER_MENU;

                        foreach (var printG in printType__Groups)
                        {
                            var printType = menuPrintInitADO.SarPrintTypes.FirstOrDefault(o => o.PRINT_TYPE_CODE == printG.PrintTypeCode);
                            if (printType != null)
                            {
                                if (!String.IsNullOrEmpty(printG.PrintTypeCode) && printTypeCodes.Contains(printG.PrintTypeCode))
                                {
                                    BarItem menuItem1 = new BarButtonItem();
                                    menuItem1.Caption = printG.Caption;
                                    menuItem1.Tag = printG.Tag;
                                    menuItem1.ItemClick += printG.ItemClickEventHandler;
                                    menu.AddItem(menuItem1);
                                }

                                this.popupMenu.AddItem(menu);
                            }
                        }
                    }

                    if (printType__NoGroups != null && printType__NoGroups.Count() > 0)
                    {
                        foreach (var printNoG in printType__NoGroups)
                        {
                            BarItem bottonItem1 = new BarButtonItem();
                            bottonItem1.Tag = printNoG.Tag;
                            bottonItem1.ItemClick += printNoG.ItemClickEventHandler;
                            bottonItem1.Caption = printNoG.Caption;
                            this.popupMenu.AddItem(bottonItem1);
                        }
                    }
                }

                return true;
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
