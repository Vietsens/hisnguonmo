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

namespace Inventec.UC.ComboNational.Design.Template1
{
    public partial class Template1
    {

        private void txtMaQuocTich_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadQuocTichCombo(strValue.ToUpper());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboQuocTich_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboQuocTich.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL ethnic = listNational.SingleOrDefault(o => o.NATIONAL_CODE == (cboQuocTich.EditValue ?? 0).ToString());
                        if (ethnic != null)
                        {
                            txtMaQuocTich.Text = ethnic.NATIONAL_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtCMND.Focus();
                            //txtCMND.SelectAll();
                        }
                    }
                    else
                    {
                        if (_FocusNext != null)
                        {
                            _FocusNext();
                        }
                        //txtCMND.Focus();
                        //txtCMND.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboQuocTich_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboQuocTich.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.SDA_NATIONAL ethnic = listNational.SingleOrDefault(o => o.NATIONAL_CODE == (cboQuocTich.EditValue ?? 0).ToString());
                        if (ethnic != null)
                        {
                            txtMaQuocTich.Text = ethnic.NATIONAL_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtCMND.Focus();
                            //txtCMND.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboQuocTich.EditValue = null;
                    txtMaQuocTich.Text = "";
                    if (_FocusNext != null)
                    {
                        _FocusNext();
                    }
                    //txtCMND.Focus();
                    //txtCMND.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
