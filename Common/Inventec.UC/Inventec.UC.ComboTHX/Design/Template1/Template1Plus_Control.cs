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
using Inventec.Common.Logging;
using Inventec.UC.ComboTHX.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.UC.ComboTHX.Design.Template1
{
    public partial class Template1
    {
        private void txtMaTHX_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadTHX(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboTHX.EditValue != null)
                    {
                        ViewSdaCommuneModel commune = listData.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = listDistrict.FirstOrDefault(o => o.ID == commune.DISTRICT_ID);
                            if (districtDTO != null)
                            {
                                if (_LoadHuyen != null) _LoadHuyen(districtDTO.PROVINCE_CODE);
                                //LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                if (_SetValueTinh != null) _SetValueTinh(districtDTO.PROVINCE_CODE);
                                //cboTinh.EditValue = districtDTO.PROVINCE_CODE;
                                //txtMaTinh.Text = districtDTO.PROVINCE_CODE;
                            }
                            if (_LoadPhuongXa != null) _LoadPhuongXa(commune.DISTRICT_CODE);
                            //LoadXaCombo("", commune.DISTRICT_CODE, false);
                            txtMaTHX.Text = commune.SEARCH_CODE;
                            if (_SetValueHuyen != null) _SetValueHuyen(commune.DISTRICT_CODE);
                            //cboHuyen.EditValue = commune.DISTRICT_CODE;
                            //txtMaHuyen.Text = commune.DISTRICT_CODE;
                            if (_SetValuePhuongXa != null) _SetValuePhuongXa(commune.COMMUNE_CODE);
                            //cboXaPhuong.EditValue = commune.COMMUNE_CODE;
                            //txtMaXaPhuong.Text = commune.COMMUNE_CODE;
                            if (_FocusNext != null) _FocusNext();
                            //txtMaNgheNghiep.Focus();
                            //txtMaNgheNghiep.SelectAll();
                        }
                    }
                    else
                    {
                        if (_FocusTinh != null) _FocusTinh();
                        //txtMaTinh.Focus();
                        //txtMaTinh.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboTHX.EditValue != null)
                    {
                        ViewSdaCommuneModel commune = listData.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboTHX.EditValue ?? 0).ToString()));
                        if (commune != null)
                        {
                            var districtDTO = listDistrict.FirstOrDefault(o => o.ID == commune.DISTRICT_ID);
                            if (districtDTO != null)
                            {
                                if (_LoadHuyen != null) _LoadHuyen(districtDTO.PROVINCE_CODE);
                                //LoadHuyenCombo("", districtDTO.PROVINCE_CODE, false);
                                if (_SetValueTinh != null) _SetValueTinh(districtDTO.PROVINCE_CODE);
                                //cboTinh.EditValue = districtDTO.PROVINCE_CODE;
                                //txtMaTinh.Text = districtDTO.PROVINCE_CODE;
                            }
                            if (_LoadPhuongXa != null) _LoadPhuongXa(commune.DISTRICT_CODE);
                            //LoadXaCombo("", commune.DISTRICT_CODE, false);
                            txtMaTHX.Text = commune.SEARCH_CODE;
                            if (_SetValueHuyen != null) _SetValueHuyen(commune.DISTRICT_CODE);
                            //cboHuyen.EditValue = commune.DISTRICT_CODE;
                            //txtMaHuyen.Text = commune.DISTRICT_CODE;
                            if (_SetValuePhuongXa != null) _SetValuePhuongXa(commune.COMMUNE_CODE);
                            //cboXaPhuong.EditValue = commune.COMMUNE_CODE;
                            //txtMaXaPhuong.Text = commune.COMMUNE_CODE;
                            if (_FocusNext != null) _FocusNext();
                            //txtMaNgheNghiep.Focus();
                            //txtMaNgheNghiep.SelectAll();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    cboTHX.EditValue = null;
                    txtMaTHX.Text = "";
                    if (_FocusNext != null) _FocusNext();
                    //txtMaNgheNghiep.Focus();
                    //txtMaNgheNghiep.SelectAll();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTHX_Properties_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "RENDERER_PDC_NAME")
                {
                    var item = ((List<ViewSdaCommuneModel>)cboTHX.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0} - {1} {2} - {3} {4}", item.PROVINCE_NAME, item.DISTRICT_INITIAL_NAME, item.DISTRICT_NAME, item.INITIAL_NAME, item.COMMUNE_NAME);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
    }
}
