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
using Inventec.Core;
using Inventec.Desktop.MenuButton.ADO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Inventec.Desktop.MenuButton.Run
{
    public sealed class RunPanelControlBehavior : IRun
    {
        MenuButtonInitADO menuPrintInitADO;
        const int IS_NO_GROUP = 1;
        DevExpress.XtraEditors.PanelControl panelControl;

        public RunPanelControlBehavior()
            : base()
        {
        }

        public RunPanelControlBehavior(CommonParam param, MenuButtonInitADO data, DevExpress.XtraEditors.PanelControl panelControl)
            : base()
        {
            this.menuPrintInitADO = data;
            this.panelControl = panelControl;
        }

        object IRun.Run()
        {
            try
            {
                MenuButtonResultADO result = new MenuButtonResultADO();
                if (Valid())
                {
                    this.panelControl.Controls.Clear();
                    var buttonmenu__GroupsOrNoParent =
                        this.menuPrintInitADO.MenuPrintADOs.Where(o => o.IsGroup || !String.IsNullOrEmpty(o.ParentCode)).OrderBy(o => o.NumOrder).ToList();

                    int clientWidth = 0;
                    int clientHeight = 0;
                    int dem = 0;
                    if (buttonmenu__GroupsOrNoParent != null && buttonmenu__GroupsOrNoParent.Count > 0)
                    {
                        dem += 1;
                        int count = buttonmenu__GroupsOrNoParent.Count;
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
                                menuButton.Dock = System.Windows.Forms.DockStyle.Right;
                                this.panelControl.Controls.Add(menuButton);

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
                                if (count == 1)
                                {
                                    btnSinglePrint.Dock = System.Windows.Forms.DockStyle.Fill;
                                }
                                else
                                {
                                    btnSinglePrint.Dock = System.Windows.Forms.DockStyle.Right;
                                }
                                this.panelControl.Controls.Add(btnSinglePrint);

                                clientWidth += btnSinglePrint.ClientSize.Width;
                                clientHeight = btnSinglePrint.ClientSize.Height;
                            }
                        }
                    }
                    result.Width = clientWidth;
                    result.Height = clientHeight;
                    result.Success = true;
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
