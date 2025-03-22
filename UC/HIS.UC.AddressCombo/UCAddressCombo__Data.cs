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
using HIS.UC.AddressCombo.ADO;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Utility;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {

        public void SetValue(UCAddressADO data)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCAddressADO: SetValue___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                #region ---refesh ---
                this.txtAddress.Text = "";
                this.txtAddress2.Text = "";
                this.txtMaTHX.Text = "";
                this.cboTHX.EditValue = null;
                this.txtCommuneCode.Text = "";
                this.cboCommune.EditValue = null;
                this.txtProvinceCode.Text = "";
                this.cboProvince.EditValue = null;
                this.txtDistrictCode.Text = "";
                this.cboDistrict.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.txtPhone.Text = "";
                this.IsPressEnter = false;
                ResetRequiredField();
                #endregion

                if (data != null)
                {
                    if (data._FocusNextUserControl != null)
                        this.dlgFocusNextUserControl = data._FocusNextUserControl;
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_NAME == data.Province_Name);
                    if (province != null)
                    {
                        this.txtProvinceCode.Text = province.PROVINCE_CODE;
                        this.cboProvince.EditValue = province.PROVINCE_CODE;
                    }
                    var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => ((o.INITIAL_NAME + " " + o.DISTRICT_NAME) == data.District_Name || o.DISTRICT_NAME == data.District_Name || o.DISTRICT_CODE == data.District_Code) && o.PROVINCE_NAME == data.Province_Name);
                    if (district != null)
                    {   
                        this.LoadHuyenCombo("", district.PROVINCE_CODE, false);
                        this.txtDistrictCode.Text = district.DISTRICT_CODE;
                        this.cboDistrict.EditValue = district.DISTRICT_CODE;
                    }
                    var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o =>
                    ((o.INITIAL_NAME + " " + o.COMMUNE_NAME) == data.Commune_Name || o.COMMUNE_NAME == data.Commune_Name)
                    && ((o.DISTRICT_INITIAL_NAME + " " + o.DISTRICT_NAME) == data.District_Name || o.DISTRICT_NAME == data.District_Name));
                    if (commune != null)
                    {
                        this.LoadXaCombo("", commune.DISTRICT_CODE, false);
                        this.txtCommuneCode.Text = commune.COMMUNE_CODE;
                        this.cboCommune.EditValue = commune.COMMUNE_CODE;
                        this.cboTHX.EditValue = "C" + commune.ID;//ID_RAW
                        bool isSearchOrderByXHT = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS_DESKTOP_REGISTER__SEARCH_CODE__X/H/T") == "1" ? true : false;

                        this.txtMaTHX.Text = isSearchOrderByXHT ? String.Format("{0}{1}{2}", commune.SEARCH_CODE, district != null ? district.SEARCH_CODE : null, province != null ? province.SEARCH_CODE : null) : String.Format("{0}{1}{2}", province != null ? province.SEARCH_CODE : null, district != null ? district.SEARCH_CODE : null, commune.SEARCH_CODE);
                    }
                    else if (data.Province_Code != null && data.District_Code != null)
                    {
                        if (String.IsNullOrEmpty(data.Commune_Code))
                        {
                            var dist = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o =>
                    (o.DISTRICT_CODE == data.District_Code && o.PROVINCE_CODE == data.Province_Code));

                            if (dist != null)
                            {
                                var com = workingCommuneADO.FirstOrDefault(o =>
                        o.ID == -dist.ID);
                                if (com != null)
                                {
                                    this.cboTHX.EditValue = "C" + com.ID;
                                    this.txtMaTHX.Text = com.SEARCH_CODE_COMMUNE;
                                }
                            }
                        }
                    }
                    else if (data.Commune_Code != null && data.District_Code != null)
                    {
                        var communeTHX = workingCommuneADO.FirstOrDefault(o =>
                        (o.SEARCH_CODE_COMMUNE) == (province.SEARCH_CODE + district.SEARCH_CODE)
                        && o.ID < 0);
                        if (communeTHX != null)
                        {
                            this.cboTHX.EditValue = communeTHX.ID_RAW;
                            this.txtMaTHX.Text = communeTHX.SEARCH_CODE_COMMUNE;
                        }
                    }

                    if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                    {
                        this.txtAddress2.Text = data.Address;
                        this.txtAddress.Text = data.Phone;
                    }
                    else
                    {
                        this.txtAddress.Text = data.Address;
                    }
                    this.txtPhone.Text = data.Phone;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UCAddressADO GetValue()
        {
            if (!ValidateRequiredField())
                return null;
            UCAddressADO getData = new UCAddressADO();
            try
            {
                // getData.Commune_Code = txtCommuneCode.Text.Trim();
                getData.Commune_Code = (string)(cboCommune.EditValue ?? "");
                getData.Commune_Name = cboCommune.Text;
                //getData.District_Code = txtDistrictCode.Text.Trim();
                getData.District_Code = (string)(cboDistrict.EditValue ?? "");
                getData.District_Name = cboDistrict.Text;
                //getData.Province_Code = txtProvinceCode.Text.Trim();
                getData.Province_Code = (string)(cboProvince.EditValue ?? "");
                getData.Province_Name = cboProvince.Text;
                getData.Phone = txtPhone.Text;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    getData.Address = this.txtAddress2.Text;
                    getData.Phone = this.txtAddress.Text;
                }
                else
                {
                    getData.Address = this.txtAddress.Text;
                }

                Inventec.Common.Logging.LogSystem.Debug("UCAddressADO: GetValue___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => getData), getData));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                getData = null;
            }
            return getData;
        }

        public void RefreshUserControl()
        {
            try
            {
                UCAddressADO dataRefresh = new ADO.UCAddressADO();
                this.txtAddress.Text = "";
                this.txtAddress2.Text = "";
                this.txtMaTHX.Text = "";
                this.cboTHX.EditValue = null;
                this.txtCommuneCode.Text = "";
                this.cboCommune.EditValue = null;
                this.txtProvinceCode.Text = "";
                this.cboProvince.EditValue = null;
                this.txtDistrictCode.Text = "";
                this.cboDistrict.EditValue = null;
                this.cboTHX.Properties.Buttons[1].Visible = false;
                this.isPatientBHYT = false;
                this.txtPhone.Text = "";
                this.IsPressEnter = false;
                this.currentPatientSDO = null;
                this.ResetRequiredField();
                this.IsValidateAddressCombo(HIS.Desktop.Plugins.Library.RegisterConfig.HisConfigCFG.Validate__T_H_X);
                this.SetDefaultDataToControl(true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {

                this.positionHandleControl = -1;
                if (!this.dxValidationProviderControl.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderControl.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }

            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return valid;
        }

    }
}
