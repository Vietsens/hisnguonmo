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

namespace Inventec.UC.ComboCommune.Design.Template1
{
    public partial class Template1
    {

        private void txtMaPhuongXa_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    string districtCode = "";
                    if (_ValueDistrict != null)
                    {
                        if (_ValueDistrict() != null) districtCode = _ValueDistrict().ToString();
                    }
                    LoadXaCombo(strValue.ToUpper(), districtCode, true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongXa_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboPhuongXa.EditValue != null && cboPhuongXa.EditValue != cboPhuongXa.OldEditValue)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listData.SingleOrDefault(o => o.COMMUNE_CODE == (cboPhuongXa.EditValue.ToString()) && o.DISTRICT_CODE == (_ValueDistrict().ToString()));
                        if (commune != null)
                        {
                            txtMaPhuongXa.Text = commune.COMMUNE_CODE;
                            if (_SetValueTHX != null) _SetValueTHX(commune);
                            //cboTHX.Properties.DataSource = GlobalStore.ListCommuneTHX;                            
                            //txtMaTHX.Text = commune.SEARCH_CODE;
                            //cboTHX.EditValue = commune.ID;
                            if (_FocusControlNext != null) _FocusControlNext();
                            //txtMaDanToc.Focus();
                            //txtMaDanToc.SelectAll();
                        }
                    }
                    else
                    {
                        if (_FocusControlNext != null) _FocusControlNext();
                        //txtMaDanToc.Focus();
                        //txtMaDanToc.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPhuongXa_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboPhuongXa.EditValue != null)
                    {
                        SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = listData.SingleOrDefault(o => o.COMMUNE_CODE == (cboPhuongXa.EditValue.ToString()) && o.DISTRICT_CODE == (_ValueDistrict().ToString()));
                        if (commune != null)
                        {
                            txtMaPhuongXa.Text = commune.COMMUNE_CODE;
                            if (_SetValueTHX != null) _SetValueTHX(commune);
                            //cboTHX.EditValue = commune.ID;
                            //txtMaTHX.Text = commune.SEARCH_CODE;
                            if (_FocusControlNext != null) _FocusControlNext();
                            //txtMaDanToc.Focus();
                            //txtMaDanToc.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboPhuongXa.EditValue = null;
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
