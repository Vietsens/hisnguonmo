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
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.UC.ComboNational.Design.Template1
{
    public partial class Template1
    {
        public void LoadDataToComboQuocGia()
        {
            try
            {
                cboQuocTich.Properties.DataSource = listNational;
                cboQuocTich.Properties.DisplayMember = "NATIONAL_NAME";
                cboQuocTich.Properties.ValueMember = "NATIONAL_CODE";
                cboQuocTich.Properties.ForceInitialize();
                cboQuocTich.Properties.Columns.Clear();
                cboQuocTich.Properties.Columns.Add(new LookUpColumnInfo("NATIONAL_CODE", "", 100));
                cboQuocTich.Properties.Columns.Add(new LookUpColumnInfo("NATIONAL_NAME", "", 200));
                cboQuocTich.Properties.ShowHeader = false;
                cboQuocTich.Properties.ImmediatePopup = true;
                cboQuocTich.Properties.DropDownRows = 10;
                cboQuocTich.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void LoadQuocTichCombo(string searchCode)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    cboQuocTich.EditValue = null;
                    cboQuocTich.Focus();
                    cboQuocTich.ShowPopup();
                    //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                }
                else
                {
                    var data = listNational.Where(o => o.NATIONAL_CODE.Contains(searchCode)).ToList();
                    if (data != null)
                    {
                        if (data.Count == 1)
                        {
                            cboQuocTich.EditValue = data[0].NATIONAL_CODE;
                            txtMaQuocTich.Text = data[0].NATIONAL_CODE;
                            if (_FocusNext != null)
                            {
                                _FocusNext();
                            }
                            //txtCMND.Focus();
                            //txtCMND.SelectAll();
                        }
                        else if (data.Count > 1)
                        {
                            cboQuocTich.EditValue = null;
                            cboQuocTich.Focus();
                            cboQuocTich.ShowPopup();
                            //PopupProcess.SelectFirstRowPopup(control.cboQuocTich);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
