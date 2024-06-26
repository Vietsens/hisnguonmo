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
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using His.Bhyt.InsuranceExpertise;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.ADO;
using HIS.Desktop.Plugins.Register.ValidationRule;
using HIS.Desktop.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private async void ProcessGetDataHrm(string valueSearch)
        {
            try
            {
                MOS.SDO.HisPatientSDO _PatientSDOByHrm = new MOS.SDO.HisPatientSDO();
                var _addressConnectHrm = (AddressConnectHrm)Newtonsoft.Json.JsonConvert.DeserializeObject<AddressConnectHrm>(HisConfigCFG.AddressConnectHrm);
                HIS.Desktop.Plugins.Library.RegisterConnectHrm.RegisterConnectHrmProcessor _RegisterConnectHrmProcessor = new Library.RegisterConnectHrm.RegisterConnectHrmProcessor();
                _PatientSDOByHrm = await _RegisterConnectHrmProcessor.GetDataHrm1(_addressConnectHrm.Address, _addressConnectHrm.Loginname, _addressConnectHrm.Password, _addressConnectHrm.GrantType, _addressConnectHrm.ClientId, _addressConnectHrm.ClientSecret, valueSearch);//"000017"
                this.Invoke(new MethodInvoker(delegate()
                                    {
                                        this.ProcessPatientCodeKeydown(_PatientSDOByHrm);
                                    }));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}
