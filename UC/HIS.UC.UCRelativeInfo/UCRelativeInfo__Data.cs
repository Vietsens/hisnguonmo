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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.UC.UCRelativeInfo.ADO;
using HIS.Desktop.Utility;

namespace HIS.UC.UCRelativeInfo
{
    public partial class UCRelativeInfo : UserControlBase
    {
        #region Get - Set Data

        public UCRelativeADO GetValue()
        {
            UCRelativeADO getADO = new UCRelativeADO();
            try
            {
                getADO.RelativeName = txtHomePerson.Text;
                getADO.Correlated = txtCorrelated.Text;
                getADO.RelativeAddress = txtRelativeAddress.Text;
                getADO.RelativePhone = txtRelativePhone.Text;
                getADO.IsNeedSickLeaveCert = chkCapGiayNghiOm.Checked;
                string cmnd = this.txtRelativeCMNDNumber.Text.Trim();
                getADO.MotherName = txtMother.Text;
                getADO.FatherName = txtFather.Text;
                if (cmnd.Length == 9 || cmnd.Length == 12)
                    getADO.RelativeCMND = cmnd;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                getADO = null;
            }
            return getADO;
        }

        public void SetValue(UCRelativeADO setADO)
        {
            try
            {
                if (setADO != null)
                {
                    this.txtCorrelated.Text = setADO.Correlated;
                    this.txtRelativeAddress.Text = setADO.RelativeAddress;
                    this.txtRelativePhone.Text = setADO.RelativePhone;
                    this.chkCapGiayNghiOm.Checked = setADO.IsNeedSickLeaveCert.HasValue ? setADO.IsNeedSickLeaveCert.Value : false;
                    if (setADO.RelativeCMND == null && setADO.RelativeCCCD == null)
                        this.txtRelativeCMNDNumber.Text = "";
                    else
                        this.txtRelativeCMNDNumber.Text = setADO.RelativeCMND == null ? setADO.RelativeCCCD : setADO.RelativeCMND;
                    this.txtHomePerson.Text = setADO.RelativeName;
                    if (setADO._FocusNextUserControl != null)
                        this.dlgFocusNextUserControl = setADO._FocusNextUserControl;
                    this.txtFather.Text = setADO.FatherName;
                    this.txtMother.Text = setADO.MotherName;
                }
                else
                {
                    this.txtHomePerson.Text = "";
                    this.txtCorrelated.Text = "";
                    this.txtRelativeAddress.Text = "";
                    this.txtRelativeCMNDNumber.Text = "";
                    this.chkCapGiayNghiOm.Checked = false;
                    this.txtFather.Text = "";
                    this.txtMother.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetValue(UCRelativeADO setADO,bool isChild)
        {
            try
            {
                if (setADO != null)
                {
                    this.txtCorrelated.Text = setADO.Correlated;
                    this.txtRelativeAddress.Text = setADO.RelativeAddress;
                    this.txtRelativePhone.Text = setADO.RelativePhone;
                    this.chkCapGiayNghiOm.Checked = setADO.IsNeedSickLeaveCert.HasValue ? setADO.IsNeedSickLeaveCert.Value : false;
                    if (setADO.RelativeCMND == null && setADO.RelativeCCCD == null)
                        this.txtRelativeCMNDNumber.Text = "";
                    else
                        this.txtRelativeCMNDNumber.Text = setADO.RelativeCMND == null ? setADO.RelativeCCCD : setADO.RelativeCMND;
                    this.txtHomePerson.Text = setADO.RelativeName;
                    if (setADO._FocusNextUserControl != null)
                        this.dlgFocusNextUserControl = setADO._FocusNextUserControl;
                    this.txtFather.Text = setADO.FatherName;
                    this.txtMother.Text = setADO.MotherName;
                }
                else
                {
                    this.txtHomePerson.Text = "";
                    this.txtCorrelated.Text = "";
                    this.txtRelativeAddress.Text = "";
                    this.txtRelativeCMNDNumber.Text = "";
                    this.chkCapGiayNghiOm.Checked = false;
                    this.txtFather.Text = "";
                    this.txtMother.Text = "";
                }
                if (!isChild)
                {
                    this.SetValidateControl(false);
                    //this.lciHomPerson.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciCMND.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciAddress.AppearanceItemCaption.ForeColor = Color.Black;
                    //this.lciRelative.AppearanceItemCaption.ForeColor = Color.Black;
                    ResetRequiredField();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Refresh UserControl

        public void RefreshUserControl()
        {
            try
            {
                UCRelativeADO dataRefresh = new UCRelativeADO();
                this.txtCorrelated.Text = dataRefresh.Correlated;
                this.txtHomePerson.Text = dataRefresh.RelativeName;
                this.txtRelativeAddress.Text = dataRefresh.RelativeAddress;
                this.txtRelativePhone.Text = dataRefresh.RelativePhone;
                this.txtRelativeCMNDNumber.Text = dataRefresh.RelativeCMND;
                this.chkCapGiayNghiOm.Checked = false;            
                this.SetValidateControl(false);
                this.txtFather.Text = dataRefresh.FatherName;
                this.txtMother.Text = dataRefresh.MotherName;
                //this.ValidRelativeCMNDNumber();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}
