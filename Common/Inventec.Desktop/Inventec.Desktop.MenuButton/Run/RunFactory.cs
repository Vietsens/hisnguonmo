using Inventec.Core;
using Inventec.Desktop.MenuButton.ADO;
using System;

namespace Inventec.Desktop.MenuButton.Run
{
    class RunFactory
    {
        internal static IRun MakeIMenuPrint(CommonParam param, object data)
        {
            IRun result = null;
            try
            {
                if (data is MenuButtonInitADO)
                {
                    MenuButtonInitADO initADO = data as MenuButtonInitADO;
                    if (initADO.ControlContainer != null && initADO.ControlContainer is DevExpress.XtraEditors.PanelControl)
                    {
                        result = new RunPanelControlBehavior(param, (MenuButtonInitADO)data, initADO.ControlContainer as DevExpress.XtraEditors.PanelControl);
                    }
                    else if (initADO.ControlContainer != null && initADO.ControlContainer is DevExpress.XtraLayout.LayoutControl)
                    {
                        result = new RunLayoutControlBehavior(param, (MenuButtonInitADO)data, initADO.ControlContainer as DevExpress.XtraLayout.LayoutControl);
                    }
                    else if (initADO.ControlContainer != null && initADO.ControlContainer is DevExpress.XtraBars.PopupMenu)
                    {
                        result = new RunPopupMenuBehavior(param, (MenuButtonInitADO)data, initADO.ControlContainer as DevExpress.XtraBars.PopupMenu);
                    }
                }

                if (result == null) throw new NullReferenceException();
            }
            catch (NullReferenceException ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Factory khong khoi tao duoc doi tuong." + data.GetType().ToString() + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data), ex);
                result = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}
