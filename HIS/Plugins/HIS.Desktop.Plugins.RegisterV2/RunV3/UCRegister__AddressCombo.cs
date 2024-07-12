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
using MOS.SDO;
using HIS.UC.AddressCombo.ADO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCAddressInfo(HisPatientSDO data)
        {
            try
            {
                UCAddressADO dataAddress = new UCAddressADO();
                dataAddress.Commune_Code = data.COMMUNE_CODE;
                dataAddress.Commune_Name = data.COMMUNE_NAME;
                dataAddress.District_Code = data.DISTRICT_CODE;
                dataAddress.District_Name = data.DISTRICT_NAME;
                dataAddress.Province_Code = data.PROVINCE_CODE;
                dataAddress.Province_Name = data.PROVINCE_NAME;
                dataAddress.Address = data.ADDRESS;
                dataAddress.Phone = data.PHONE;
                this.ucAddressCombo1.SetValue(dataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
