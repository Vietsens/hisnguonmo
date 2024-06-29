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

namespace HIS.UC.PlusInfo
{
    public partial class UCPlusInfo : UserControlBase
    {
        public void SetDataAddress(object data, bool isCallByUCAddress)
        {
            try
            {
                if (data == null) return;

                if (this.ucCommuneNow1 != null && data.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                    this.ucCommuneNow1.SetValueFromUCAddress(data, isCallByUCAddress);
                if (this.ucProvinceOfBirth1 != null && data.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                    this.ucProvinceOfBirth1.SetValueFromUCAddress(data);
                if (this.ucProvinceNow1 != null && data.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                    this.ucProvinceNow1.SetValueFromUCAddress(data, isCallByUCAddress);
                if (this.ucDistrictNow1 != null && data.GetType() == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                    this.ucDistrictNow1.SetValueFromUCAddress(data, isCallByUCAddress);
                if (this.ucAddress1 != null && data.GetType() == typeof(string))
                    this.ucAddress1.SetValueAddress(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void SetAddressNow(string address)
        {
            try
            {
                this.ucAddress1.SetValueAddress(address);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDelegateToSetValueAddress()
        {
            try
            {
                if (this.ucDistrictNow1 != null && this.ucProvinceNow1 != null)
                    this.ucProvinceNow1.SetDelegateLoadDistrictByProvince(this.ucDistrictNow1.SetValueFromUCAddress);
                if (this.ucDistrictNow1 != null && this.ucCommuneNow1 != null)
                    this.ucDistrictNow1.SetDelegateLoadCommuneByDistrict(this.ucCommuneNow1.SetValueFromUCAddress);
                if (this.ucCommuneNow1 != null)
                    this.ucCommuneNow1.DelegateSetProDistByCommune(this.SetDataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDelegateToSetValueAddressKS()
        {
            try
            {
                if (this.ucDistrictOfBirth1 != null && this.ucProvinceOfBirth1 != null)
                    this.ucProvinceOfBirth1.SetDelegateLoadDistrictByProvince(this.ucDistrictOfBirth1.SetValueFromUCAddress);
                if (this.ucDistrictOfBirth1 != null && this.ucCommuneOfBirth1 != null)
                    this.ucDistrictOfBirth1.SetDelegateLoadCommuneByDistrict(this.ucCommuneOfBirth1.SetValueFromUCAddress);
                //if (this.ucCommuneOfBirth1 != null)
                //    this.ucCommuneOfBirth1.DelegateSetProDistByCommune(this.SetDataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshDataDistrictAndCommune(bool isReload)
        {
            try
            {
                if (this.ucDistrictNow1 != null)
                    this.ucDistrictNow1.RefreshUserControl(isReload);
                if (this.ucCommuneNow1 != null)
                    this.ucCommuneNow1.RefreshUserControl(isReload);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/RefreshDataDistrictAndCommnue:\n" + ex);
            }
        }

        private void RefreshDataDistrictAndCommuneOfBirth(bool isReload)
        {
            try
            {
                if (this.ucDistrictOfBirth1 != null)
                    this.ucDistrictOfBirth1.RefreshUserControl(isReload);
                if (this.ucCommuneOfBirth1 != null)
                    this.ucCommuneOfBirth1.RefreshUserControl(isReload);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/RefreshDataDistrictAndCommnue:\n" + ex);
            }
        }
    }
}
