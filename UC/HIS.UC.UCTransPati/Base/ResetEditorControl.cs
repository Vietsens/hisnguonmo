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
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.UC.UCTransPati
{
    class ResetEditorControl
    {
        internal static void Reset(BaseEdit editor)
        {
            try
            {
                editor.Reset();
                editor.EditValue = null;
                editor.Text = "";
                if (editor is DevExpress.XtraEditors.LookUpEdit)
                {
                    if (((DevExpress.XtraEditors.LookUpEdit)editor).Properties.Buttons.Count > 1)
                        ((DevExpress.XtraEditors.LookUpEdit)editor).Properties.Buttons[1].Visible = false;
                }
                else if (editor is DevExpress.XtraEditors.GridLookUpEdit)
                {
                    if (((DevExpress.XtraEditors.GridLookUpEdit)editor).Properties.Buttons.Count > 1)
                        ((DevExpress.XtraEditors.GridLookUpEdit)editor).Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ResetAndFocus(DevExpress.XtraEditors.LookUpEdit cboEditor, bool isVisibleButtonDel)
        {
            try
            {
                cboEditor.EditValue = null;
                if (isVisibleButtonDel && cboEditor.Properties.Buttons.Count > 1)
                    cboEditor.Properties.Buttons[1].Visible = false;
                cboEditor.Focus();
                cboEditor.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ResetAndFocus(DevExpress.XtraEditors.GridLookUpEdit cboEditor, bool isVisibleButtonDel)
        {
            try
            {
                cboEditor.EditValue = null;
                if (isVisibleButtonDel && cboEditor.Properties.Buttons.Count > 1)
                    cboEditor.Properties.Buttons[1].Visible = false;
                cboEditor.Focus();
                cboEditor.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ResetAndFocus(DevExpress.XtraEditors.LookUpEdit cboEditor, bool isVisibleButtonDel, bool isSelectFirstRowPopup)
        {
            try
            {
                cboEditor.EditValue = null;
                if (isVisibleButtonDel && cboEditor.Properties.Buttons.Count > 1)
                    cboEditor.Properties.Buttons[1].Visible = false;
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRowPopup)
                    HIS.UC.UCTransPati.Init.PopupProcess.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void ResetAndFocus(DevExpress.XtraEditors.GridLookUpEdit cboEditor, bool isVisibleButtonDel, bool isSelectFirstRowPopup)
        {
            try
            {
                cboEditor.EditValue = null;
                if (isVisibleButtonDel && cboEditor.Properties.Buttons.Count > 1)
                    cboEditor.Properties.Buttons[1].Visible = false;
                cboEditor.Focus();
                cboEditor.ShowPopup();
                if (isSelectFirstRowPopup)
                    HIS.UC.UCTransPati.Init.PopupProcess.SelectFirstRowPopup(cboEditor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}
