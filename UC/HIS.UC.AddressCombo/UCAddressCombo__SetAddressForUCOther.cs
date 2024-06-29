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
using HIS.Desktop.Utility;
using HIS.Desktop.LocalStorage.BackendData;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.DelegateRegister;

namespace HIS.UC.AddressCombo
{
    public partial class UCAddressCombo : UserControlBase
    {
        public void SetDelegateSendProvince(DelegateSendCodeProvince _dlg)
        {
            try
            {
                if (_dlg != null)
                {
                    this.dlgSendCodeProvince = _dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void SetDelegateSendCardSDO(DelegateSendCardSDO _dlg)
        {
            try
            {
                if (_dlg != null)
                {
                    this.dlgSendCardSDO = _dlg;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm gán delegate thực hiện gán dữ liệu thẻ
        /// </summary>
        /// <param name="_dlgSetAddress"></param>

        public void SetDelegateSetAddressUCHein(DelegateSetAddressUCHein _dlgSetAddress)
        {
            try
            {
                if (_dlgSetAddress != null)
                    this.dlgSetAddressUCHein = _dlgSetAddress;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm gán dữ liệu địa chỉ thẻ
        /// </summary>
        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                string address = "";
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    address = this.txtAddress2.Text;
                }
                else
                {
                    address = this.txtAddress.Text;
                }
                if (cboProvince.EditValue != null)
                {
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                    this.ChangeReplaceAddress(cboProvince.Text, "Tỉnh", ref address);
                }
                if (this.cboDistrict.EditValue != null)
                {
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                    this.ChangeReplaceAddress(cboDistrict.Text, "Huyện", ref address);
                }

                if (this.cboCommune.EditValue != null)
                {
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                    this.ChangeReplaceAddress(cboCommune.Text, "Xã", ref address);
                }

                IsNotLoadChangeAddressTxt = true;
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    this.txtAddress2.Text = address;
                }
                else
                    this.txtAddress.Text = address;
                IsNotLoadChangeAddressTxt = false;

                if (isReadCard)
                    return;
                //if (isPatientBHYT)//Bo di de th bn cu van fill du lieu uc hein
                //    return;
                string heinAddress = string.Format("{0}{1}{2}{3}", address, (commune != null ? " " + commune.INITIAL_NAME + " " + commune.COMMUNE_NAME : ""), (district != null ? ", " + district.INITIAL_NAME + " " + district.DISTRICT_NAME : ""), (province != null ? ", " + province.PROVINCE_NAME : ""));
                this.dlgSetAddressUCHein(heinAddress);
            }
            catch (Exception ex)
            {
                IsNotLoadChangeAddressTxt = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        /// <summary>
        /// Hàm gán delegate thực hiện gán thông tin địa chỉ vùng "Thông tin thêm"
        /// </summary>
        /// <param name="_dlgSetAddress"></param>
        public void SetDelegateSetAddressUCPlusInfo(DelegateSetAddressUCPlusInfo _dlgSetAddress)
        {
            try
            {
                if (_dlgSetAddress != null)
                    this.dlgSetAddressUCProvinceOfBirth = _dlgSetAddress;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Hàm gán thông tin địa chỉ thẻ
        /// </summary>
        private void SetValueForUCPlusInfo()
        {
            try
            {
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                province = BackendDataWorker.Get<V_SDA_PROVINCE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                if (this.dlgSetAddressUCProvinceOfBirth != null)
                    this.dlgSetAddressUCProvinceOfBirth(province, true);
                district = BackendDataWorker.Get<V_SDA_DISTRICT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);
                if (this.dlgSetAddressUCProvinceOfBirth != null)
                    this.dlgSetAddressUCProvinceOfBirth(district, true);
                commune = BackendDataWorker.Get<V_SDA_COMMUNE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.SDA_RS.COMMON.IS_ACTIVE__TRUE).ToList().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);
                if (this.dlgSetAddressUCProvinceOfBirth != null)
                    this.dlgSetAddressUCProvinceOfBirth(commune, true);
                if (this.dlgSendCodeProvince != null)
                    this.dlgSendCodeProvince(province != null ? province.PROVINCE_CODE : null);
                string address = "";
                if (HIS.Desktop.Plugins.Library.RegisterConfig.AppConfigs.IsShowLineFirstAddress == 2)
                {
                    address = (String.IsNullOrEmpty(txtAddress2.Text) == true ? "" : this.txtAddress2.Text);
                }
                else
                {
                    address = (String.IsNullOrEmpty(txtAddress.Text) == true ? "" : this.txtAddress.Text);
                }
                {
                    if (this.dlgSetAddressUCProvinceOfBirth != null)
                        this.dlgSetAddressUCProvinceOfBirth(address, true);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Set du lieu thong tin hanh chinh cho UCPlusInfo tu UCAddress khong thanh cong: \n" + ex);
            }
        }
    }
}
