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
