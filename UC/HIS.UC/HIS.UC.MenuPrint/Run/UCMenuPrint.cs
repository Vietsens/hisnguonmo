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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.MenuPrint.ADO;
using DevExpress.Utils.Menu;

namespace HIS.UC.MenuPrint
{
    public partial class UCMenuPrint : UserControl
    {
        MenuPrintInitADO menuPrintInitADO;
        public UCMenuPrint(MenuPrintInitADO menuPrintInitADO)
        {
            InitializeComponent();
            this.menuPrintInitADO = menuPrintInitADO;
        }

        private void UCMenuPrint_Load(object sender, EventArgs e)
        {
            try
            {
                if (Valid())
                {
                    if (this.menuPrintInitADO.MenuPrintADO.Childens != null && this.menuPrintInitADO.MenuPrintADO.Childens.Count > 0)
                    {
                        DevExpress.XtraEditors.DropDownButton menuButton = new DevExpress.XtraEditors.DropDownButton();
                        menuButton.Text = this.menuPrintInitADO.MenuPrintADO.Caption;

                        DXPopupMenu menu = new DXPopupMenu();
                        foreach (var item in this.menuPrintInitADO.MenuPrintADO.Childens)
                        {
                            DXMenuItem menuItem1 = new DXMenuItem(item.Caption, item.EventHandler);
                            menuItem1.Tag = item.Tag;
                            if (item.Shortcut != System.Windows.Forms.Shortcut.None)
                            {
                                menuItem1.Shortcut = item.Shortcut;
                            }
                            menu.Items.Add(menuItem1);
                        }

                        menuButton.DropDownControl = menu;

                        this.Controls.Add(menuButton);
                    }
                    else
                    {
                        DevExpress.XtraEditors.SimpleButton btnSinglePrint = new DevExpress.XtraEditors.SimpleButton();
                        btnSinglePrint.Text = this.menuPrintInitADO.MenuPrintADO.Caption;
                        btnSinglePrint.Click += this.menuPrintInitADO.MenuPrintADO.EventHandler;
                        btnSinglePrint.Tag = this.menuPrintInitADO.MenuPrintADO.Tag;

                        this.Controls.Add(btnSinglePrint);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool Valid()
        {
            bool valid = true;
            try
            {
                if (menuPrintInitADO == null)
                    throw new ArgumentNullException("menuPrintInitADO is null");
                if (menuPrintInitADO.MenuPrintADO == null)
                    throw new ArgumentNullException("menuPrintInitADO.MenuPrintADOs is null");
                if (menuPrintInitADO.SarPrintTypes == null || menuPrintInitADO.SarPrintTypes.Count == 0)
                    throw new ArgumentNullException("menuPrintInitADO.SarPrintTypes is null");

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
