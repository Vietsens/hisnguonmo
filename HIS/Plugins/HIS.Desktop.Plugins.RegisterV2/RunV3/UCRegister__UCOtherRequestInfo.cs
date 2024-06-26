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
using HIS.UC.UCOtherServiceReqInfo.ADO;
using HIS.UC.UCPatientRaw.ADO;

namespace HIS.Desktop.Plugins.RegisterV2.Run2
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataIntoUCOtherServiceReqInfo(HisPatientSDO data)
        {
            try
            {
                this.ucOtherServiceReqInfo1.SetValueByPatientInfo(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoSetDataForOtherServiceReqInfo(bool value, long patientTypeId)
        {
            try
            {
                UCServiceReqInfoADO serviceReqInfoADO = ucOtherServiceReqInfo1.GetValue();
                serviceReqInfoADO.IsNotRequireFee = value;
                ucOtherServiceReqInfo1.SetValue(serviceReqInfoADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoSetTreatmentTypeCombo(long? value, DataResultADO dataResult = null)
        {
            try
            {
                if (value.HasValue)
                {
                    UCServiceReqInfoADO serviceReqInfoADO = ucOtherServiceReqInfo1.GetValue();
                    serviceReqInfoADO.TreatmentType_ID = value.Value;
                    if (dataResult != null && dataResult.HisPatientSDO != null)
                        serviceReqInfoADO.NOTE = dataResult.HisPatientSDO.NOTE;
                    ucOtherServiceReqInfo1.SetValue(serviceReqInfoADO);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SendPatientName(string _patientName)
        {
            try
            {
                if (this.ucOtherServiceReqInfo1 != null)
                {
                    this.ucOtherServiceReqInfo1.SetPatientName(_patientName);
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void SendPatientSDO(HisPatientSDO sdo)
        {
            try
            {
                if (this.ucAddressCombo1 != null)
                {
                    this.ucAddressCombo1.GetPatientSdo(sdo);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        
    }
}
