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
using HIS.UC.UCPatientRaw.ADO;
using MOS.SDO;

namespace HIS.Desktop.Plugins.RegisterV3.Run3
{
    public partial class UCRegister : UserControlBase
    {
        private void FillDataPatientRawInfo(HisPatientSDO data)
        {
            try
            {
                var oldDataPatient = this.ucPatientRaw1.GetValue();
                UCPatientRawADO dataAddress = new UCPatientRawADO();
                dataAddress.CARRER_ID = data.CAREER_ID;
                dataAddress.DOB = data.DOB;
                dataAddress.GENDER_ID = data.GENDER_ID;
                dataAddress.HEIN_CARD_NUMBER = data.HeinCardNumber;
                dataAddress.IS_HAS_NOT_DAY_DOB = data.IS_HAS_NOT_DAY_DOB;
                dataAddress.PATIENT_CODE = data.PATIENT_CODE;
                dataAddress.PATIENT_NAME = data.VIR_PATIENT_NAME;
                if (oldDataPatient != null)
                    dataAddress.PATIENTTYPE_ID = oldDataPatient.PATIENTTYPE_ID;
                this.ucPatientRaw1.SetValue(dataAddress);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}
