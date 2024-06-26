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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.MenuPrint.ADO
{
    public class MenuPrintInitADO
    {
        public List<MenuPrintADO> MenuPrintADOs { get; set; }
        public List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE> SarPrintTypes { get; set; }
        public object ControlContainer { get; set; }
        public int MaxSizeWidth { get; set; }
        public int MaxSizeHeight { get; set; }
        public int MinSizeWidth { get; set; }
        public int MinSizeHeight { get; set; }

        private bool isTrue = true;
        public bool IsUsingShortCut 
        { 
            get { return isTrue; } 
            set { isTrue = value; } 
        }

        public MenuPrintInitADO(List<MenuPrintADO> menuPrintADOs, List<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE> sarPrintTypes)
        {
            this.MenuPrintADOs = menuPrintADOs;
            this.SarPrintTypes = sarPrintTypes;
        }
    }
}
