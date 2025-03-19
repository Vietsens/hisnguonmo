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
using System.Windows.Forms;

namespace Inventec.Desktop.Controls
{
    public class ControlWorker
    {
        public static Control GetControlByNameInParentControl(string controlName, Control parentControl)
        {
            foreach (Control c in parentControl.Controls)
            {               
                if (c.Name == controlName)
                    return c;
                var childControl = c.Controls;
                if (childControl != null && childControl.Count > 0)
                {
                    var ctrolSearch = SearchControlByNameForAll(childControl, controlName);
                    if (ctrolSearch != null)
                    {
                        return ctrolSearch;
                    }
                }
            }

            return null;
        }

        static Control SearchControlByNameForAll(Control.ControlCollection listControl, string controlName)
        {
            foreach (Control c in listControl)
            {
                if (c.Name == controlName)
                {
                    return c;
                }

                Control.ControlCollection childControl = c.Controls;

                if (childControl != null && childControl.Count > 0)
                {
                    Control result = SearchControlByNameForAll(childControl, controlName);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static void ValidationProviderRemoveControlError(DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProvider1, DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider dxErrorProvider1)
        {
            IList<Control> invalidControls = dxValidationProvider1.GetInvalidControls();
            for (int i = invalidControls.Count - 1; i >= 0; i--)
            {
                dxValidationProvider1.RemoveControlError(invalidControls[i]);
            }
            dxErrorProvider1.ClearErrors();
        }
    }
}
