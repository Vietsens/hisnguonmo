using Inventec.Core;
using Inventec.Desktop.MenuButton.ADO;
using Inventec.Desktop.MenuButton.Run;
using System;

namespace Inventec.Desktop.MenuButton
{
    public class MenuButtonProcessor : BussinessBase
    {
        object uc;
        public MenuButtonProcessor()
            : base()
        {
        }

        public MenuButtonProcessor(CommonParam paramBusiness)
            : base(paramBusiness)
        {
        }

        public object Run(MenuButtonInitADO arg)
        {
            uc = null;
            try
            {
                IRun behavior = RunFactory.MakeIMenuPrint(param, arg);
                uc = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                uc = null;
            }
            return uc;
        }
    }
}
