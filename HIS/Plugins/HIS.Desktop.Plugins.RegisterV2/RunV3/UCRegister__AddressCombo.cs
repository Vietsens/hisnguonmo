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
using HIS.Desktop.Utility;
using HIS.UC.AddressCombo.ADO;
using HIS.UC.UCPatientRaw.ADO;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCAddressInfo(DataResultADO data)
        {
            try
            {
                UCAddressADO dataAddress = new UCAddressADO();
                dataAddress.Commune_Code = data.HisPatientSDO.COMMUNE_CODE;
                dataAddress.Commune_Name = data.HisPatientSDO.COMMUNE_NAME;
                dataAddress.District_Code = data.HisPatientSDO.DISTRICT_CODE;
                dataAddress.District_Name = data.HisPatientSDO.DISTRICT_NAME;
                dataAddress.Province_Code = data.HisPatientSDO.PROVINCE_CODE;
                dataAddress.Province_Name = data.HisPatientSDO.PROVINCE_NAME;
                dataAddress.Address = data.HisPatientSDO.ADDRESS;
                dataAddress.Phone = data.HisPatientSDO.PHONE;
                dataAddress.IsNoDistrict = data.IsNoDistrict;
                this.ucAddressCombo1.SetValue(dataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
