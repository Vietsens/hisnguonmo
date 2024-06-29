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
        
        private void SetValueHeinAddressByAddressOfPatient()
        {
            try
            {
                if (isReadCard)
                    return;
                if (isPatientBHYT)
                    return;
                if (this.isReadCard)
                    return;
                if (this.cboProvince.EditValue == null && String.IsNullOrEmpty(this.txtAddress.Text))
                    return;
                string address = txtAddress.Text;
                SDA.EFMODEL.DataModels.V_SDA_COMMUNE commune = null;
                SDA.EFMODEL.DataModels.V_SDA_DISTRICT district = null;
                SDA.EFMODEL.DataModels.V_SDA_PROVINCE province = null;
                if (cboProvince.EditValue != null)
                {
                    province = BackendDataWorker.Get<V_SDA_PROVINCE>().FirstOrDefault(o => o.PROVINCE_CODE == (string)this.cboProvince.EditValue);
                }
                if (this.cboDistrict.EditValue != null)
                    district = BackendDataWorker.Get<V_SDA_DISTRICT>().FirstOrDefault(o => o.DISTRICT_CODE == (string)this.cboDistrict.EditValue);

                if (this.cboCommune.EditValue != null)
                    commune = BackendDataWorker.Get<V_SDA_COMMUNE>().FirstOrDefault(o => o.COMMUNE_CODE == (string)this.cboCommune.EditValue);

                string heinAddress = string.Format("{0}{1}{2}{3}", address, (commune != null ? " " + commune.INITIAL_NAME + " " + commune.COMMUNE_NAME : ""), (district != null ? ", " + district.INITIAL_NAME + " " + district.DISTRICT_NAME : ""), (province != null ? ", " + province.PROVINCE_NAME : ""));
                this.dlgSetAddressUCHein(heinAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

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

        public void SetDelegateSetAddressUCProvinceOfBirth(DelegateSetAddressUCProvince _dlgSetAddress)
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
    }
}
