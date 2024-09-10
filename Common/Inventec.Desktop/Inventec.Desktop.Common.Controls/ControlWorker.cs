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
