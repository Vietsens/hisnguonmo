using System.Collections.Generic;

namespace Inventec.Desktop.MenuButton.ADO
{
    public class MenuButtonInitADO
    {
        public List<MenuButtonADO> MenuPrintADOs { get; set; }      
        public object ControlContainer { get; set; }
       
        public MenuButtonInitADO(List<MenuButtonADO> menuPrintADOs)
        {
            this.MenuPrintADOs = menuPrintADOs;
        }
    }
}
