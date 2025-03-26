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

namespace Inventec.UC.ComboProvince.Design.Template1
{
    internal partial class Template1
    {
        private void LoadDataToCombo()
        {
            try
            {
                cboTinh.Properties.DataSource = listData;
                cboTinh.Properties.DisplayMember = "PROVINCE_NAME";
                cboTinh.Properties.ValueMember = "PROVINCE_CODE";
                cboTinh.Properties.ForceInitialize();
                cboTinh.Properties.Columns.Clear();
                cboTinh.Properties.Columns.Add(new LookUpColumnInfo("PROVINCE_CODE", "", 100));
                cboTinh.Properties.Columns.Add(new LookUpColumnInfo("PROVINCE_NAME", "", 200));
                cboTinh.Properties.ShowHeader = false;
                cboTinh.Properties.ImmediatePopup = true;
                cboTinh.Properties.DropDownRows = 20;
                cboTinh.Properties.PopupWidth = 300;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadTinhThanhCombo(string searchCode, bool isExpand)
        {
            try
            {
                if (String.IsNullOrEmpty(searchCode))
                {
                    if (_SetValue != null) _SetValue();
                    //cboXaPhuong.Properties.DataSource = null;
                    //cboXaPhuong.EditValue = null;
                    //txtMaXaPhuong.Text = "";
                    //cboHuyen.Properties.DataSource = null;
                    //cboHuyen.EditValue = null;
                    //txtMaHuyen.Text = "";
                    cboTinh.EditValue = null;
                    cboTinh.Focus();
                    cboTinh.ShowPopup();
                    Process.PopupProcess.SelectFirstRowPopup(cboTinh);
                }
                else
                {
                    List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE> listResult = new List<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    listResult = listData.Where(o => o.PROVINCE_CODE.Contains(searchCode)).ToList();
                    if (listResult.Count == 1)
                    {
                        cboTinh.EditValue = listResult[0].PROVINCE_CODE;
                        txtMaTinh.Text = listResult[0].PROVINCE_CODE;
                        if (_LoadHuyenFromTinh != null) _LoadHuyenFromTinh(listResult[0].PROVINCE_CODE);
                        //LoadHuyenCombo("", listResult[0].PROVINCE_CODE, false);
                        if (isExpand)
                        {
                            if (_FocusCboHuyen != null) _FocusCboHuyen(false);
                            //txtMaHuyen.Focus();
                            //txtMaHuyen.SelectAll();
                        }
                    }
                    else if (listResult.Count > 1)
                    {
                        if (_SetValue != null) _SetValue();
                        //cboXaPhuong.Properties.DataSource = null;
                        //cboXaPhuong.EditValue = null;
                        //txtMaXaPhuong.Text = "";
                        //cboHuyen.Properties.DataSource = null;
                        //cboHuyen.EditValue = null;
                        //txtMaHuyen.Text = "";
                        cboTinh.EditValue = null;
                        if (isExpand)
                        {
                            cboTinh.Focus();
                            cboTinh.ShowPopup();
                            Process.PopupProcess.SelectFirstRowPopup(cboTinh);
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
